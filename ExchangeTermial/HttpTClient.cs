
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net.Security;
using System.Net.Mime;
using System.Net.Http;
using System.Reflection;

namespace ExchangeTermial

{

    public class HttpTunnelClient

    {

        #region Fields

        private HttpRequestEntity _entity;

        private SocketProxy _proxy;

        private Socket _sock;

        #endregion



        #region Properties

        public WebHeaderCollection Headers

        {

            get { return _entity.Headers; }

        }

        public NameValueCollection Form

        {

            get { return _entity.Form; }

        }

        public bool KeepAlive { get; set; }

        public bool HeadMethod { get; set; }

        public int SendReceiveTimeout { get; set; }

        #endregion



        #region Constructors

        public HttpTunnelClient(Uri proxyAddress = null)

        {

            if (proxyAddress != null)

            {

                _proxy = new SocketProxy()

                {

                    ProxyType = SocketProxyType.ptHTTP,

                    Address = SocketHelper.ParseEndPoint(proxyAddress.GetEndPoint())

                };

            }

            _entity = new HttpRequestEntity();

            _entity.Headers[HttpRequestHeader.Accept] = "*/*";

            _entity.Headers[HttpRequestHeader.Referer] = HttpClient.DefaultReferer;

            _entity.Headers[HttpRequestHeader.UserAgent] = HttpClient.DefaultUserAgent;

            this.KeepAlive = true;

            this.SendReceiveTimeout = 1000 * 30;

        }

        #endregion



        #region Methods

        private void Prepare(out string method, out string sForm)

        {

            method = sForm = null;

            _entity.Headers["Proxy-Connection"] = _entity.Headers[HttpRequestHeader.Connection] = this.KeepAlive ? "keep-alive" : "close";

            if (this.HeadMethod)

            {

                method = WebRequestMethods.Http.Head;

                return;

            }

            if (!_entity.HasValue)

            {

                method = WebRequestMethods.Http.Get;

                return;

            }

            sForm = _entity.GetFormString();

            _entity.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

            _entity.Headers[HttpRequestHeader.ContentLength] = Encoding.UTF8.GetByteCount(sForm).ToString();

            method = WebRequestMethods.Http.Post;

        }



        private void SetEncoding(StreamReader reader, Encoding encoding, out string bufferedString)

        {

            Type type = reader.GetType();

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var field = type.GetField("charPos", flags);

            int charPos = Convert.ToInt32(field.GetValue(reader));

            field = type.GetField("charLen", flags);

            int charLen = Convert.ToInt32(field.GetValue(reader));

            field = type.GetField("byteBuffer", flags);

            byte[] byteBuffer = (byte[])field.GetValue(reader);

            bufferedString = encoding.GetString(byteBuffer, charPos, charLen - charPos);



            field = type.GetField("encoding", flags);

            field.SetValue(reader, encoding);



            field = type.GetField("decoder", flags);

            field.SetValue(reader, encoding.GetDecoder());



            reader.DiscardBufferedData();

        }



        public HttpResponseEntity GetResponse(Uri requestUri, Action<Stream> onSend = null, Action<Stream> onReceive = null)

        {

            Contract.Requires(requestUri != null);



            _entity.Headers[HttpRequestHeader.Host] = requestUri.Host;

            string method, sForm;

            this.Prepare(out method, out sForm);



            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _sock.ReceiveTimeout = _sock.SendTimeout = this.SendReceiveTimeout;

            try

            {

                if (_proxy != null)

                {

                    _sock.Connect(_proxy.Address);

                    byte[] data = ProxyUtility.PrepareRequest(_proxy, SocketHelper.ParseEndPoint(requestUri.GetEndPoint()));

                    int sent = _sock.Send(data);

                    Array.Resize(ref data, 256);

                    int recv = _sock.Receive(data);

                    ProxyUtility.ValidateResponse(_proxy, data);

                }

                else

                {

                    _sock.ConnectSync(requestUri.Host, requestUri.Port);

                }



                var netStream = new NetworkStream(_sock, FileAccess.ReadWrite, false);

                Stream src = netStream;

                if (requestUri.Scheme == Uri.UriSchemeHttps)

                {

                    var sslStream = new SslStream(src);

                    sslStream.AuthenticateAsClient(requestUri.Host);

                    src = sslStream;

                }



                var writer = new StreamWriter(src, Encoding.ASCII);

                writer.WriteLine("{0} {1} HTTP/1.1", method, requestUri.PathAndQuery);

                var sHeaders = new StringBuilder(_entity.GetHeadersString());

                SocketHelper.Logger.DebugFormat("RequestHeaders:\r\n{0}\r\n", sHeaders);

                writer.WriteLine(sHeaders);

                writer.WriteLine();

                writer.Flush();

                if (onSend != null)

                {

                    onSend(src);

                }

                else

                {

                    if (sForm != null)

                    {

                        writer = new StreamWriter(src, Encoding.UTF8);

                        writer.Write(sForm);

                        writer.Flush();

                    }

                }



                var response = new HttpResponseEntity();

                var reader = new StreamReader(src, Encoding.ASCII);

                //HTTP/1.1 200 OK

                string line = reader.ReadLine();

                var status = line.Split(new char[] { ' ' }, 3);

                if (status.Length != 3)

                {

                    throw new WebException("InternalServerError");

                }

                response.StatusCode = (HttpStatusCode)int.Parse(status[1]);

                response.StatusDescription = status[2];

                sHeaders.Length = 0;

                while (!string.IsNullOrEmpty(line = reader.ReadLine()))

                {

                    int i = line.IndexOf(":");

                    string name = line.Substring(0, i), value = line.Substring(i + 2);

                    response.Headers.Add(name, value);

                    sHeaders.AppendLine(line);

                }

                SocketHelper.Logger.DebugFormat("ResponseHeaders:\r\n{0}\r\n", sHeaders);

                string contentType = response.Headers[HttpResponseHeader.ContentType];

                int charsetIndex;

                if (string.IsNullOrEmpty(contentType) || (charsetIndex = contentType.LastIndexOf("=")) == -1)

                {

                    response.ContentEncoding = Encoding.UTF8;

                }

                else

                {

                    string charset = contentType.Substring(charsetIndex + 1);

                    response.ContentEncoding = Encoding.GetEncoding(charset);

                }

                if (!this.HeadMethod)

                {

                    if (onReceive != null)

                    {

                        onReceive(src);

                    }

                    else

                    {

                        string bufferedString;

                        this.SetEncoding(reader, response.ContentEncoding, out bufferedString);

                        response.ResponseText = bufferedString;

                        response.ResponseText += reader.ReadToEnd();

                    }

                }

                return response;

            }

            finally

            {

                _entity.Clear(HttpRequestEntity.ItemKind.Form);

                if (_sock.Connected)

                {

                    _sock.Disconnect(this.KeepAlive);

                }

            }

        }

        #endregion

    }

}

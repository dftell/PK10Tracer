using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http;

namespace ExchangeTermial
{
    public class HttpClientToServer
    {
        HttpClient req = null;
        HttpClientHandler handler;
        string Host;
        public HttpClientToServer(string strHost)
        {
            Host = strHost;
        }




        public string getStringFromServer(string host, string url, string PostData, Encoding Encode, Func<string,bool> succFunc, Action<string,string> faileFunc, bool Post = true, CookieCollection inCookie = null)
        {
            if (handler == null)
            {
                handler = new HttpClientHandler();
                handler.AllowAutoRedirect = false;
                handler.UseCookies = true;
                handler.CookieContainer = new CookieContainer();
                if (inCookie != null)
                {
                    handler.CookieContainer.Add(inCookie);
                }
            }
            
            //handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            //handler.AutomaticDecompression = DecompressionMethods.None;
            /*
             request.Accept = "text/html, application/xhtml+xml, image/jxr, 
            request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 46.0.2486.0 Safari / 537.36 Edge / 13.10586";
            request.Method = "GET";
            request.Host = "192.168.1.104";
            request.KeepAlive = true;            
            request.Referer = "http://192.168.1.104/";            
            request.Headers.Add("Accept-Encoding", "gzip, deflate");            
            request.Headers.Add("Accept-Language", "zh-CN");
            */
            string ret = "";
            req = new HttpClient(handler);
            req.BaseAddress = new Uri(host);
            //req.ProtocolVersion = HttpVersion.Version10;
            //req.BaseAddress = new Uri(url);
            req.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            req.DefaultRequestHeaders.Add("Host", host.Replace("https://", "").Replace("http://", ""));
            req.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:50.0) Gecko/20100101 Firefox/50.0");
            req.DefaultRequestHeaders.Add("Accept-Language", "zh-Cn");

            string method = "Post";
            CookieCollection cc1 = new CookieCollection();
            //cc1.Add(new Cookie("ai_user", "", "/", req.DefaultRequestHeaders.Host));
            //cc1.Add(new Cookie("ai_session", "", "/", req.DefaultRequestHeaders.Host));
            handler.CookieContainer.Add(cc1);
            //req.DefaultRequestHeaders.Add("Cookie", "ai_user=;ai_session=;");
            if (!Post)
                method = "Get";
            req.DefaultRequestHeaders.Add("Method", method);
            req.DefaultRequestHeaders.Add("KeepAlive", "true");
            req.DefaultRequestHeaders.Add("Cookie", "");

            try
            {
                Uri accessUrl = new Uri(url);
                var res = req.GetAsync(url.Replace(host, "")).Result;
                bool changed = false;
                while (res.StatusCode == HttpStatusCode.RedirectKeepVerb)
                {
                    string fullUrl = host + accessUrl.ToString().Replace(host, "");
                    CookieCollection cc = handler.CookieContainer.GetCookies(new Uri(fullUrl));
                    //if (changed == false)
                    {
                        accessUrl = res.Headers.Location;
                        changed = true;
                    }
                    List<string> cookies = new List<string>();
                    if (1 ==1)
                    {
                        
                        for (int i = 0; i < cc.Count; i++)
                        {
                            cookies.Add(string.Format("{0}={1}", cc[i].Name, cc[i].Value));
                        }
                    }
                    req.DefaultRequestHeaders.Add("Set-Cookie", string.Join(";", cookies));
                    //////HttpClient hc = new HttpClient(handler);
                    //////req.DefaultRequestHeaders.ToList().ForEach(
                    //////    a=> {
                    //////        hc.DefaultRequestHeaders.Add(a.Key, a.Value);
                    //////    });

                    res = req.GetAsync(host + res.Headers.Location).Result;
                }
                ret = res.Content.ReadAsStringAsync().Result;
                bool succ = (succFunc==null)?false:succFunc.Invoke(ret);
                if (succ)
                    return ret;
                return null;
            }
            catch (Exception ce)
            {
                faileFunc?.Invoke(ce.Message,ce.StackTrace);
                return null;
            }
            return ret;

            //req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393";
            //req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
            //req.Method = "Post";
            //req.AllowAutoRedirect = true;


            //req.Headers.Add("Accept-Language", "zh-CN");
            //req.Headers.Add("Accept-Encoding", "gzip, deflate");
            //req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //req.Host = host.Replace("https://","").Replace("http://","");
            //req.Timeout = 150 * 1000;
            //req.Referer = host;
            //req.ContentLength = 0;
            //req.Headers.Add("Pragma","no-cache");
            //req.KeepAlive = true;
            //req.IfModifiedSince = DateTime.UtcNow;
            //req.CookieContainer = new CookieContainer();
            //CookieCollection cc = new CookieCollection();
            //cc.Add(new Cookie());
            //req.CookieContainer.Add(new Cookie("__RequestVerificationToken", "UzENB9CB49sNtfuRJ4CIhLB7NnhpYI8YQM_vB98zNXlFOCWh3In - olUCP4N250rFEDziu5flQk2eMSnyt7pRZBqx2YsaqVpEsL92HeylC_E1"));
            //req.CookieContainer.Add(cc);
            //req.CookieContainer.Add(new Cookie("__RequestVerificationToken", "UzENB9CB49sNtfuRJ4CIhLB7NnhpYI8YQM_vB98zNXlFOCWh3In - olUCP4N250rFEDziu5flQk2eMSnyt7pRZBqx2YsaqVpEsL92HeylC_E1"));
            //// ai_user = npx3P | 2020 - 03 - 26T13: 01:50.110Z; 
            ///token = b0a6f90be980824e6d46f32b88cb498f; 
            ///random = 5758; 
            ///__RequestVerificationToken = UzENB9CB49sNtfuRJ4CIhLB7NnhpYI8YQM_vB98zNXlFOCWh3In - olUCP4N250rFEDziu5flQk2eMSnyt7pRZBqx2YsaqVpEsL92HeylC_E1
            //if (!Post)
            //    req.Method = "Get";
            //else
            //    req.Method = "Post";
            /*
            try
            {
                if (!string.IsNullOrEmpty(PostData))
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(PostData);
                    Stream newStream = req.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
                    newStream.Write(byteArray, 0, byteArray.Length);
                    newStream.Close();
                }
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    ret = new StreamReader(wr.GetResponseStream(), Encode).ReadToEnd();
                    wr.Close();
                }
            }
            catch (Exception ce)
            {
                return ce.Message;

                //throw ce;
            }*/
            return ret;
        }

        public bool ConnectToServer(string host,string url, bool AutoRedirect, CookieCollection inCookie, Action<string,string> faileFunc)
        {
            if(handler == null)
                handler = new HttpClientHandler();
            //handler.AllowAutoRedirect = false;
            handler.UseCookies = true;
            handler.CookieContainer = new CookieContainer();

            var requestMessage =  new HttpRequestMessage
            {
                Version = HttpVersion.Version11
                
            };
            
            if (inCookie != null)
            {
                handler.CookieContainer.Add(inCookie);
            }
           
            string ret = "";
            req = new HttpClient(handler);
            
            req.DefaultRequestHeaders.Add("Method", WebRequestMethods.Http.Connect);
            //req.DefaultRequestHeaders.Add("Host", Host);
            req.DefaultRequestHeaders.Add("KeepAlive", "True");
            try
            {
                requestMessage.RequestUri = new Uri(url);
                requestMessage.Headers.Host = Host;
                
                //requestMessage.Method = HttpMethod.Head;WebRequestMethods.Http.Connect;
                //var res = req.GetAsync(url).Result;
                var res = req.SendAsync(requestMessage).Result;
                while (res.StatusCode == HttpStatusCode.RedirectKeepVerb)
                {
                    string location = res.Headers.Location.ToString() ;
                    //if(res.Headers)
                    if (handler.CookieContainer.Count>0)
                    {
                        break;
                    }
                    res = req.GetAsync(host + location).Result;
                    
                }
                return true;
            }
            catch(Exception ce)
            {
                faileFunc?.Invoke(ce.Message, ce.StackTrace);
                return false;
            }
            return true;
        }
    }

}

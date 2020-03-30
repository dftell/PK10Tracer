using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using IntelligentPlanning;
using IntelligentPlanning.CustomControls;
using IntelligentPlanning.ExDataGridView;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;

namespace ExchangeTermial
{
    public class ClientExchanger
    {

        string __RequestVerificationToken;//: beV-1NdJsxVXQQSKZgUR0g4GC3l__AsnSveidv5_Zs1EhuhU_4MHt5KwPkVaZFP_r5fKuhdSXjsUm7R6bHo-c6Oq1WLQ8UQTGmWJ0WH2Z2s1
        string CaptchaDeText;// 6189ab52205a4b1cb038e5a033647254
        string cookie;
        string loginName;
        string loginPwd;
        string verPwd;
        HttpClient req;
        string loginPageUrl
        {
            get
            {
                return string.Format(gc.LoginPage, host);
            }
        }
        string loginRequestUrl
        {
            get
            {
                return string.Format(gc.HostLoginUrl, host); 
            }
        }
        string betUrl
        {
            get
            {
                return string.Format(gc.HostBetUrl, host);
            }
        }

        string amountUrl
        {
            get
            {
                return string.Format(gc.HostAmountUrl, host);
            }
        }
        GlobalClass gc;
        PTBase ptobj;
        string host
        {
            get
            {
                return string.Format(gc.LoginUrlModel,gc.LoginDefaultHost);
            }
        }

        public ClientExchanger()
        {
            gc = Program.gc;
            
        }

        public bool Load()
        {
            
            string strLoginPage = ToServer(host,host, null, Encoding.UTF8, false);
            return true;
        }
        

        public bool Login(string id,string pwd,string verPwd)
        {
            //https://jhc3.msnmht.cn/Account/LoginVerify
            //
            try
            {
                
            }
            catch(Exception ce)
            {

            }
            return false;
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开
            return true;
        }
        public string ToServer(string host,string url, string PostData, Encoding Encode, bool Post = true)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;

            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false ;
            handler.UseCookies = true;
            handler.CookieContainer = new CookieContainer();
            
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
            req.DefaultRequestHeaders.Add("Cookie","");

            try
            {
                Uri accessUrl = new Uri(url);
                var res = req.GetAsync(url.Replace(host,"")).Result;   
                bool changed = false;
                while (res.StatusCode == HttpStatusCode.RedirectKeepVerb)
                {
                    string fullUrl = host + accessUrl.ToString().Replace(host,"");
                    CookieCollection cc =  handler.CookieContainer.GetCookies(new Uri(fullUrl));
                    //if (changed == false)
                    {
                        accessUrl = res.Headers.Location;
                        changed = true;
                    }
                    List<string> cookies = new List<string>();
                    for (int i = 0; i < cc.Count; i++)
                    {
                        cookies.Add(string.Format("{0}={1}", cc[i].Name, cc[i].Value));
                    }
                    
                    //req.DefaultRequestHeaders.Add("Set-Cookie", string.Join(";", cookies));
                    //////HttpClient hc = new HttpClient(handler);
                    //////req.DefaultRequestHeaders.ToList().ForEach(
                    //////    a=> {
                    //////        hc.DefaultRequestHeaders.Add(a.Key, a.Value);
                    //////    });
                    
                    res = req.GetAsync(host+res.Headers.Location).Result;
                }
                ret = res.Content.ReadAsStringAsync().Result;
 
            }
            catch(Exception ce)
            {
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
    }

    public class MyRequest: HttpClient
    {

    }

}

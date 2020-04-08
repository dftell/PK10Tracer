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
using System.Collections.Specialized;
using System.Windows.Forms;
using Microsoft.JScript;
using Microsoft.JScript.Vsa;
using Microsoft.CSharp;
using System.CodeDom;
using System.Web.Script.Serialization;

namespace ExchangeTermial
{
    public class ClientExchanger
    {
        
        public WebBrowser browser;
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

        CookieCollection CurrCookies;
        public void SetCookie(CookieCollection cc)
        {
            CurrCookies = cc;
        }

        public ClientExchanger(WebBrowser b,GlobalClass g)
        {
            browser = b;
            gc = g;
        }


    

        public bool Load()
        {
            
            //string strLoginPage = AccessWeb(host,loginPageUrl, null, Encoding.UTF8, false);
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

        public bool AccessWeb(string host, string url, string PostData, Encoding encoding, Func<string, bool> succFunc, Action<string, string> faileFunc, bool Post = true, CookieCollection inCookie = null, bool allowRedirect = false)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            
            //req. "zh-Cn");
            try
            {
                setRequest(req, Post, host, "", allowRedirect, inCookie);
                if(string.IsNullOrEmpty(PostData))
                {
                    req.ContentLength = 0;
                }
                else
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(PostData);
                    Stream newStream = req.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
                    newStream.Write(byteArray, 0, byteArray.Length);
                    newStream.Close();
                }
                string ret = null;
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                bool firstTime = true;
                while(!req.AllowAutoRedirect&& (res.StatusCode == HttpStatusCode.RedirectKeepVerb || res.StatusCode == HttpStatusCode.Redirect))
                {
                    
                    string currUrl = req.Address.ToString();
                    string reurl = host + res.Headers["Location"].ToString();
                    CookieCollection cc = res.Cookies;
                    req = (HttpWebRequest)WebRequest.Create(reurl);
                    setRequest(req, Post, host, currUrl,!firstTime,cc);
                    if (firstTime)
                    {
                        firstTime = false;
                    }
                    res = (HttpWebResponse)req.GetResponse();
                  
                }
                ret = new StreamReader(res.GetResponseStream()).ReadToEnd();
                return succFunc == null ? false : succFunc.Invoke(ret);
            }
            catch(Exception e)
            {
                faileFunc(e.Message, e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 如果从httpwebrequest发送，任何一个AddJscript里面，必须包括GenerateGuid()，getPostData(string ExpectNo,string EncodingMsg,string ruleid,string guid)和getUrl(guid)三个函数分别生成guid,post数据和url
        /// </summary>
        /// <param name="expectNo"></param>
        /// <param name="EncodingMsg"></param>
        /// <param name="dtpName"></param>
        /// <param name="orgMsg"></param>
        /// <param name="lastval"></param>
        /// <param name="ruleId"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        public bool sendInsts(string expectNo,string EncodingMsg, string dtpName, string orgMsg, double lastval,string ruleId, object sender,Func<string,bool> succFunc,Action<string,string> failFunc)
        {
            try
            {
                string guid = GenerateGuid();//无论如何，先生成一个guid,可用可不用
                if(string.IsNullOrEmpty(guid))
                {
                    guid = Guid.NewGuid().ToString();
                }
                string postData = getPostData(expectNo, EncodingMsg, ruleId, guid);
                if (string.IsNullOrEmpty(postData))
                {
                    failFunc?.Invoke("生成数据失败", "无法获取到数据");
                    return false;
                }
                string url = getUrl(guid);
                return AccessWeb(host,host + url, postData, Encoding.UTF8, succFunc, failFunc, true, this.CurrCookies, true);
            }
            catch(Exception ce)
            {
                failFunc(ce.Message, ce.StackTrace);
                return false;
            }
                //return false;
        }

        public string getPostData(string ExpectNo,string EncodingMsg,string ruleid,string guid)
        {
            VsaEngine Engine = VsaEngine.CreateEngine();
            string txt = GlobalClass.LoadTextFile("getPostData.js", string.Format("jscript\\{0}\\", gc.ForWeb));
            CSharpCodeProvider cc = new CSharpCodeProvider();
            // icc = cc.CreateCompiler();
            string allscript = string.Format("{0}getPostData('{1}','{2}','{3}','{4}');", txt, ExpectNo, EncodingMsg, ruleid, guid);
            try
            {

                object value = Eval.JScriptEvaluate(allscript, Engine);
                StringBuilder sb = new StringBuilder();
                JavaScriptSerializer json = new JavaScriptSerializer();
                json.Serialize(value, sb);
                return sb.ToString();
                //return DetailStringClass.ToJson(value);
            }
            catch (Exception ce)
            {
                return null;
            }
            finally
            {
                Engine = null;
            }
        }

        object ExecuteScript(string sExpression, string sCode)
         {
             MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
             scriptControl.UseSafeSubset = false;
             
             scriptControl.Language = "JScript";
             scriptControl.AddCode(sCode);
             try
             {
                 object str = scriptControl.Eval(sExpression);
                 return str;
             }
             catch (Exception ex)
             {
                 string str = ex.Message;
             }
             return null;
         }

        public string GenerateGuid()
        {
            try
            {
                string txt = GlobalClass.LoadTextFile("GenerateGuid.js", string.Format("jscript\\{0}\\", gc.ForWeb));
                string allscript = string.Format("{0};GenerateGuid();", txt);
                VsaEngine Engine = VsaEngine.CreateEngine();
                object value = Eval.JScriptEvaluate(allscript, Engine);
                return value?.ToString();
            }
            catch(Exception ce)
            {
                return null;
            }
        }

        public string getUrl(string guid)
        {
            try
            {
                string txt = GlobalClass.LoadTextFile("getUrl.js", string.Format("jscript\\{0}\\", gc.ForWeb));
                string allscript = string.Format("{0};getUrl('{1}');", txt, guid);
                VsaEngine Engine =VsaEngine.CreateEngine();
                object value = Eval.JScriptEvaluate(allscript, Engine);
                return value?.ToString();
            }
            catch (Exception ce)
            {
                return null;
            }
        }

        void setRequest(HttpWebRequest req,bool Post,string host,string referUrl,bool AllowRedirect = false, CookieCollection cc=null)
        {
            req.Method = Post ? "Post" : "GET";
            req.AllowAutoRedirect = AllowRedirect;
            req.ProtocolVersion = HttpVersion.Version11;
            //bool useCookie = req.SupportsCookieContainer;
            req.CookieContainer = new CookieContainer();
            if (cc != null)
                req.CookieContainer.Add(cc);
            //useCookie = req.SupportsCookieContainer;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.Host = host.Replace("https://", "").Replace("http://", "");
            if (!string.IsNullOrEmpty(referUrl))
                req.Referer = referUrl;
            else
                req.Referer = host;
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:35.0) Gecko/20100101 Firefox/35.0";
            req.Headers.Add("Accept-Encoding", "gzip,deflate");
            req.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.8,en;q=0.5,de-de;q=0.3");
            //req.Headers.Add("Host", host.Replace("https://", "").Replace("http://", ""));
            req.Headers.Add("DNT", "1");
            if(WebHeaderCollection.IsRestricted("Host"))
            {
                //req.Headers.Set(HttpRequestHeader.Host, host.Replace("https://", "").Replace("http://", ""));
                SetHeaderValue(req.Headers, "Host", host.Replace("https://", "").Replace("http://", ""));
            }
            
            req.Headers.Add("Upgrade-Insecure-Requests", "1");
            
            //req.Method = Post ? "Post" : "GET";
            if(cc != null)
            {

                //if (cc.Count>0 && cc[cc.Count-1].Name != "ai_user")
                //{
                //    string ai_user = string.Format("n3YUx |{0}", DateTime.UtcNow.ToString("yyyy MM ddThh:mm:ss.000"));
                //    cc.Add(new Cookie("ai_user", ai_user, "/", req.Host));
                //}
                List<string> cs = new List<string>();
                for (int i = 0; i < cc.Count; i++)
                    cs.Add(string.Format("{0}={1}", cc[i].Name, cc[i].Value));
                //req.CookieContainer.Add(cc);
                string strCookie = string.Join(";", cs);
                SetHeaderValue(req.Headers, "Cookie", strCookie);
            }
        }
        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
        public string ToServer(string host,string url, string PostData, Encoding Encode,Action<string> succFunc,Action<string> faileFunc, bool Post = true,CookieCollection inCookie=null)
        {
            

            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false ;
            handler.UseCookies = true;
            handler.CookieContainer = new CookieContainer();
            if(inCookie != null)
            {
                handler.CookieContainer.Add(inCookie);
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
                succFunc?.Invoke(ret);
            }
            catch(Exception ce)
            {
                faileFunc?.Invoke(ce.Message);
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

    public class MyRequest:WebResponse
    {
        
    }

}

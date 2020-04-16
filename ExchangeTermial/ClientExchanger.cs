using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
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
using System.Threading;
using WolfInv.com.WebRuleLib;
using System.Xml;
using System.Web;

namespace ExchangeTermial
{
    public enum WebStatus
    {
        NoIn, InHost, InLogin, Logined
    }
    public class ClientExchanger
    {
        private CookieContainer cookieContainer = new CookieContainer();
        WebInfoClass webinfo;
        public string Referer = string.Empty;
        WebStatus currStatus;
        HttpClientToServer hts ;
        public CookieContainer CookieContainer
        {
            get
            {
                return cookieContainer;
            }
        }

        public ClientExchanger(WebBrowser b, GlobalClass g)
        {
            browser = b;
            gc = g;
            webinfo = g.WebInfo;
            if (webinfo == null)
            {
                MessageBox.Show("webinfo config error!");
                throw new Exception("webinfo config error!");
            }
            webinfo.useHost = hostWithHead;
            hts = new HttpClientToServer(host);
        }

        public CookieCollection CurrCookies = new CookieCollection();

        public void AddCookies(CookieCollection ces)
        {
            CurrCookies.Add(ces);
        }

        public bool Load(string newurl)
        {
            browser.Navigate(loginPageUrl);
            
            return true;
            if (!string.IsNullOrEmpty(newurl))
                gc.LoginDefaultHost = newurl;
            if (!getConnect("http://" + host))
                return false;
            if(!getConnectCookie(hostWithHead+ webinfo.connectVerUrl))
            {
                return false;
            }
            if(!getConnectCookie(loginPageUrl,CurrCookies))
            {
                return false;
            }
            string post = string.Format("LoginID={0}&Password={1}", Program.gc.ClientUserName, Program.gc.ClientPassword);
            return AccessWeb(host, loginRequestUrl, post, Encoding.UTF8, (s) =>
            {
                return true;
            },
                (a, b) =>
                {
                    MessageBox.Show(string.Format("{0}:{1}", a, b));
                }, true, CurrCookies, false, hostWithHead + webinfo.loginPageUrl
                );
            return false;
            bool succ = hts.ConnectToServer(hostWithHead, hostWithHead, false, cookieContainer.GetCookies(new Uri(hostWithHead)),  null);// ; getConnect(loginPageUrl);
            string result = "";
            if (!succ)
            {
                return false;
            }
            //访问登陆页面
            post = string.Format("LoginID={0}&Password={1}", Program.gc.ClientUserName, Program.gc.ClientPassword);
            string body = hts.getStringFromServer(hostWithHead, loginPageUrl, "", Encoding.UTF8, (a) => { return true; }, null,false, cookieContainer.GetCookies(new Uri(hostWithHead)));            //CookieCollection cc = getServerCookie(loginPageUrl);
            if (body == null)
                return false;
            string key = webinfo.cookieKey;
            string keyval = null;
            HtmlTagClass htc = HtmlTagClass.getTagInfo(body, key);
            if (htc == null)
            {
                return false;
            }
            cookieContainer.SetCookies(new Uri(hostWithHead), getCookie(htc.KeyValue, htc.AttValue, "/"));
            //提交验证页面
            body = hts.getStringFromServer(host, webinfo.connectVerUrl, post, Encoding.UTF8, (a) => { return true; }, null,true, cookieContainer.GetCookies(new Uri(host)));
            return true;
            //for(int i=0;i<cc.Count;i++)
            //{
            //    CookieContainer.SetCookies(new Uri(hostWithHead),getCookie(cc[i].Name,cc[i].Value,cc[i].Path));
            //}
            browser.Navigate(loginPageUrl);

            //browser.AllowNavigation = false;
            //browser.Navigate(hostWithHead);
            browser.DocumentCompleted += DocumentCompleted;
            //browser.Navigated += Browser_Navigated;
            //browser.Navigating += Browser_Navigating;
            //string strLoginPage = AccessWeb(host,loginPageUrl, null, Encoding.UTF8, false);
            return true;
        }


        public string GetLastCookie()
        {
            var cookies = CookieContainer.GetCookies(new Uri(host));

            return $"last_wxuin={cookies["wxuin"].Value};wxuin={cookies["wxuin"].Value};" +
                $"webwxuvid={cookies["webwxuvid"].Value}; webwx_auth_ticket={cookies["webwx_auth_ticket"].Value}";
        }

        public bool getConnectCookie(string url,CookieCollection cc = null)
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                req = WebRequest.Create(url) as HttpWebRequest;
                //req.Proxy = new WebProxy(hostWithHead);
                req.Method = WebRequestMethods.Http.Get;
                req.ProtocolVersion = HttpVersion.Version11;
                req.CookieContainer = new CookieContainer();
                if (cc != null)
                    req.CookieContainer.Add(cc);
                req.KeepAlive = true;
                req.AllowAutoRedirect = false;
                req.Host = host;
                res = req.GetResponse() as HttpWebResponse;
                while (res.StatusCode == HttpStatusCode.RedirectKeepVerb)
                {
                    string reUrl = res.Headers["Location"].ToString();
                    if (req.CookieContainer.Count > 0)
                    {
                        Uri currDomain = req.RequestUri;
                        CookieCollection cl = req.CookieContainer.GetCookies(currDomain);
                        CurrCookies.Add(cl);
                        res.Close();
                        return true;
                    }
                    req = WebRequest.Create(hostWithHead+reUrl) as HttpWebRequest;
                    req.AllowAutoRedirect = false;
                    req.CookieContainer = new CookieContainer();
                    if(CurrCookies.Count>0)
                    {
                        req.CookieContainer.Add(CurrCookies);
                    }
                    res = req.GetResponse() as HttpWebResponse;
                }
                //while(response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.RedirectKeepVerb || response.StatusCode == HttpStatusCode.Moved)
                //{

                //}
            }
            catch (Exception ce)
            {
                return false;
            }
            return false;
        }

        public string getCookieString(CookieCollection cc)
        {
            List<string> ret = new List<string>();

            return string.Join(";", ret);
        }

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
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
                return string.Format(gc.LoginPage, hostWithHead);
            }
        }
        string loginRequestUrl
        {
            get
            {
                return string.Format(gc.HostLoginUrl, hostWithHead);
            }
        }
        string betUrl
        {
            get
            {
                return string.Format(gc.HostBetUrl, hostWithHead);
            }
        }

        string amountUrl
        {
            get
            {
                return string.Format(gc.HostAmountUrl, hostWithHead);
            }
        }
        GlobalClass gc;
        //PTBase ptobj;
        string hostWithHead
        {
            get
            {
                return string.Format(gc.LoginUrlModel, gc.LoginDefaultHost);
            }
        }

        string host
        {
            get
            {
                return hostWithHead.Replace("https://", "").Replace("http://", "");
            }
        }

        //CookieCollection CurrCookies;
       


        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (browser.ReadyState == WebBrowserReadyState.Complete)
            {
                string strCookie = browser.Document.Cookie;
                if (string.IsNullOrEmpty(strCookie))
                    return;
                if (browser.Url.ToString() == hostWithHead + webinfo.connectVerUrl)
                {
                    if (currStatus == WebStatus.NoIn)
                    {
                        CookieContainer.SetCookies(new Uri(hostWithHead), strCookie);
                        currStatus = WebStatus.InHost;
                        browser.Navigate(loginPageUrl);
                        return;
                    }
                }

                string body = browser.Document.Body.OuterHtml;
                string key = webinfo.cookieKey;
                string keyval = null;
                HtmlTagClass htc = HtmlTagClass.getTagInfo(body, key);
                if (htc == null)
                {
                    return;
                }


                cookieContainer.SetCookies(new Uri(hostWithHead), getCookie(htc.KeyValue, htc.AttValue, "/"));

                string[] arr = strCookie.Split(';');
                for (int i = 0; i < arr.Length; i++)
                {
                    string[] carr = arr[i].Split('=');
                    CookieContainer.SetCookies(new Uri(hostWithHead), getCookie(carr[0], carr.Length == 1 ? "" : carr[1], "/"));
                }
                if (browser.Url.ToString() == loginPageUrl)
                {
                    if (currStatus == WebStatus.InHost)
                    {
                        currStatus = WebStatus.InLogin;
                    }
                }
                //Login(gc.ClientUserName, gc.ClientPassword, null);
            }

        }
        public string getCookie(string name, string val, string path = "/")
        {
            string ret = string.Format("{0}={1}", name, val);
            return HttpUtility.UrlEncode(ret, Encoding.UTF8);
        }

        Cookie getCookie(string strCookie, string currHost = null)
        {
            if (string.IsNullOrEmpty(strCookie))
            {
                return null;
            }
            Cookie ret = null;
            //cookiename=value;Path=/;Domain=domainvalue;Max-Age=seconds;HttpOnly
            string[] arr = strCookie.Split(';');
            string keys = arr[0];
            string[] keyArr = keys.Split('=');
            if (keyArr.Length < 2)
                return null;
            string key = keyArr[0];
            string val = keyArr[1];
            string path = "";
            string domain = "";
            for (int i = 1; i < arr.Length; i++)
            {
                keyArr = arr[i].Split('=');
                if (keyArr[0].Trim() == "Path")
                {
                    if (keyArr.Length > 1)
                    {
                        path = keyArr[1];
                    }
                }
                if (keyArr[0].Trim() == "Domain")
                {
                    if (keyArr.Length > 1)
                    {
                        domain = keyArr[1];
                    }
                }
            }
            if (!string.IsNullOrEmpty(currHost))
            {
                if (string.IsNullOrEmpty(domain))
                    domain = currHost;
            }
            return new Cookie(key, val, path, domain);
        }

        CookieCollection GetCookieCollection(string strCookie, string currhost = null)
        {
            CookieCollection cc = new CookieCollection();
            return cc;
        }

        public bool Login(string id, string pwd, string verPwd)
        {
            //https://jhc3.msnmht.cn/Account/LoginVerify
            //
            try
            {
                string url = string.Format("{0}{1}", hostWithHead, webinfo.userAuthUrl);
                string post = string.Format("LoginID={0}&Password={1}", id, pwd);
                return AccessWeb(host, url, post, Encoding.UTF8, (s) =>
                   {
                       return true;
                   },
                (a, b) =>
                {
                    MessageBox.Show(string.Format("{0}:{1}", a, b));
                }, true, null, false, hostWithHead + webinfo.loginPageUrl
                );
            }
            catch (Exception ce)
            {

            }
            return false;
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开
            return true;
        }

        public bool AccessWeb(string host, string url, string PostData, Encoding encoding, Func<string, bool> succFunc, Action<string, string> faileFunc, bool Post = true, CookieCollection inCookie = null, bool allowRedirect = false, string referUrl = null)
        {
            try
            {
                string ret = GetOrPostFormString(url, PostData, Post, referUrl, allowRedirect);
                return succFunc(ret);
            }
            catch (Exception ce)
            {
                faileFunc(ce.Message, ce.StackTrace);
            }
            return false;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;
            ServicePointManager.CheckCertificateRevocationList = false;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            //req. "zh-Cn");
            try
            {
                //setRequest(req, Post, host, "", allowRedirect, inCookie);
                if (string.IsNullOrEmpty(PostData))
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
                while (!req.AllowAutoRedirect && (res.StatusCode == HttpStatusCode.RedirectKeepVerb || res.StatusCode == HttpStatusCode.Redirect))
                {

                    string currUrl = req.Address.ToString();
                    string reurl = host + res.Headers["Location"].ToString();
                    CookieCollection cc = res.Cookies;
                    req = (HttpWebRequest)WebRequest.Create(reurl);
                    //setRequest(req, Post, host, currUrl,!firstTime,cc);
                    if (firstTime)
                    {
                        firstTime = false;
                    }
                    res = (HttpWebResponse)req.GetResponse();

                }
                ret = new StreamReader(res.GetResponseStream()).ReadToEnd();
                return succFunc == null ? false : succFunc.Invoke(ret);
            }
            catch (Exception e)
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
        public bool sendInsts(string expectNo, string EncodingMsg, string dtpName, string orgMsg, double lastval, string ruleId, object sender, Func<string, bool> succFunc, Action<string, string> failFunc)
        {
            try
            {
                string guid = GenerateGuid();//无论如何，先生成一个guid,可用可不用
                if (string.IsNullOrEmpty(guid))
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
                return AccessWeb(host, hostWithHead + url, postData, Encoding.UTF8, succFunc, failFunc, true, this.CurrCookies, false);
            }
            catch (Exception ce)
            {
                failFunc(ce.Message, ce.StackTrace);
                return false;
            }
            //return false;
        }

        public string getPostData(string ExpectNo, string EncodingMsg, string ruleid, string guid)
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
            catch (Exception ce)
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
                VsaEngine Engine = VsaEngine.CreateEngine();
                object value = Eval.JScriptEvaluate(allscript, Engine);
                return value?.ToString();
            }
            catch (Exception ce)
            {
                return null;
            }
        }


        public string GetOrPostFormString(string url, string body, bool POST = true, string referUrl = null, bool allowDirect = false)
        {
            string result = string.Empty;
            Stream stream = Retry<Stream>(() =>
            {
                //return GetResponseStream(url,POST, "application/x-www-form-urlencoded", body,referUrl, allowDirect);
                return GetResponseStream(url, POST, "application/x-www-form-urlencoded", body, referUrl, allowDirect);
            });
            using (stream)
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            return result;
        }
        /// <summary>
        /// 三次重试机制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private T Retry<T>(Func<T> func)
        {
            int err = 0;
            while (err < 3)
            {
                try
                {
                    return func();
                }
                catch (WebException webExp)
                {
                    err++;
                    Thread.Sleep(5000);
                    if (err > 2)
                    {
                        throw webExp;
                    }
                }
            }
            return func();
        }

        public string PostJson<T>(string url, string body, bool POST = true, string referUrl = null, bool allowDirect = false)
        {
            string result = string.Empty;
            Stream stream = Retry<Stream>(() =>
            {
                //return GetResponseStream("POST", url, "application/json;charset=UTF-8", body);
                return GetResponseStream(url, POST, "application/x-www-form-urlencoded", body, referUrl, allowDirect);
            });
            using (stream)
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            return result;
        }

        public HttpWebRequest PretendChanle(HttpWebRequest request)
        {
#if !DEBUG
            //request.Proxy = null;
#endif
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Headers.Add("Accept-Encoding", "gzip,deflate,br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4");
            request.CookieContainer = new CookieContainer();
            request.AllowAutoRedirect = false;
            request.ServicePoint.Expect100Continue = false;
            request.Referer = Referer;
            request.Timeout = 35000;
            if (Referer != string.Empty)
            {
                request.Headers.Add("Origin", Referer);
            }
            return request;
        }

        public HttpWebRequest PretendWechat(HttpWebRequest request, bool redirect = false, string referUrl = null)
        {
#if !DEBUG
            //request.Proxy = null;
#endif
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Headers.Add("Accept-Encoding", "gzip,deflate,br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4");
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = redirect;
            request.Host = host;
            request.ServicePoint.Expect100Continue = false;
            request.Referer = referUrl;
            request.Timeout = 35000;
            //if (!string.IsNullOrEmpty(referUrl))
            //{
            //    request.Headers.Add("Origin", Referer);
            //}
            return request;
        }

        public bool getConnect(string url)
        {
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;
            //ServicePointManager.CheckCertificateRevocationList = false;
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                req = WebRequest.Create(url + ":443") as HttpWebRequest;
                req.ProtocolVersion = HttpVersion.Version11;
                req.Method = WebRequestMethods.Http.Connect;
                req.KeepAlive = true;
                //req.CookieContainer = new CookieContainer();
                //req.AllowAutoRedirect = true;
                req.Credentials = CredentialCache.DefaultCredentials;
                req.Host = host;
                res = req.GetResponse() as HttpWebResponse;
                res.Close();
                return true;
            }
            catch(Exception ce)
            {
                return false;
            }
            return false;
        }

        public Stream GetResponseStream(string url, bool post = true, string contentType = null, string body = null, string referUrl = null, bool AllowRedirect = false)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            request = WebRequest.Create(url) as HttpWebRequest;
            Stream stream = null;
            request = setRequest(request, post, host, body, referUrl, AllowRedirect, cookieContainer, contentType);
            response = request.GetResponse() as HttpWebResponse;
            while (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.RedirectKeepVerb)
            {
                //string oldUrl = request.RequestUri.ToString();
                string redirectUrl = response.Headers["Location"].ToString();
                request = WebRequest.Create(hostWithHead + redirectUrl) as HttpWebRequest;

                for (int i = 0; i < response.Cookies.Count; i++)
                {
                    Cookie cc = response.Cookies[i];
                    CookieContainer.SetCookies(new Uri(hostWithHead), getCookie(cc.Name, cc.Value, cc.Path));
                }
                request = setRequest(request, post, host, body, referUrl, true, cookieContainer, contentType);

                response = request.GetResponse() as HttpWebResponse;
            }
            stream = response.GetResponseStream();
            return stream;
        }

        CookieCollection getServerCookie(string url)
        {
            CookieCollection ret = new CookieCollection();
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.KeepAlive = true;
                request.CookieContainer = new CookieContainer();
                request.UserAgent = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.Headers.Add("Accept-Encoding", "gzip,deflate,br");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4");
                request.Method = "GET";
                request.Host = host;
                request.Headers.Add("x-requested-with", "XMLHttpRequest");
                request.AllowAutoRedirect = true;
                response = request.GetResponse() as HttpWebResponse;
                while (response.StatusCode == HttpStatusCode.RedirectKeepVerb)
                {
                    string newUrl = hostWithHead + response.Headers["Location"].ToString();
                    if (response.Cookies.Count > 0)
                    {

                        ret = response.Cookies;
                        return ret;
                    }
                    request = WebRequest.Create(newUrl) as HttpWebRequest;
                    request.KeepAlive = true;
                    request.CookieContainer = new CookieContainer();
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    request.Headers.Add("Accept-Encoding", "gzip,deflate,br");
                    request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4");
                    request.Method = "GET";
                    request.AllowAutoRedirect = false;
                    response = request.GetResponse() as HttpWebResponse;
                }
                ret = response.Cookies;
            }
            catch (Exception ce)
            {
                return null;
            }
            return ret;
        }


        HttpWebRequest setRequest(HttpWebRequest request, bool Post, string host, string body, string referUrl = null, bool AllowRedirect = false, CookieContainer cc = null, string contentType = null)
        {
#if !DEBUG
            //request.Proxy = null;
#endif
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Headers.Add("Accept-Encoding", "gzip,deflate,br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4");
            request.CookieContainer = cc;
            request.AllowAutoRedirect = AllowRedirect;
            request.Host = host;
            request.ServicePoint.Expect100Continue = false;
            request.Referer = referUrl;
            request.Timeout = 35000;
            if (contentType != null)
            {
                request.ContentType = contentType;
            }
            request.Method = WebRequestMethods.Http.Connect;
            if (Post)
            {
                request.Method = "POST";
                if (body != null)
                {
                    StreamWriter sw = new StreamWriter(request.GetRequestStream());
                    sw.Write(body);
                    sw.Flush();
                    sw.Close();
                }
            }
            else
            {
                request.Method = "GET";
                request.ContentLength = 0;
            }
            //if (!string.IsNullOrEmpty(referUrl))
            //{
            //    request.Headers.Add("Origin", Referer);
            //}
            return request;
            HttpWebRequest req = null;
            /*


            req.AllowAutoRedirect = AllowRedirect;
            req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            req.ProtocolVersion = HttpVersion.Version10;
            //bool useCookie = req.SupportsCookieContainer;
            req.CookieContainer = new CookieContainer();
            if (cc != null)
                req.CookieContainer.Add(cc);
            //useCookie = req.SupportsCookieContainer;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*;q=0.8";
            req.Host = host.Replace("https://", "").Replace("http://", "");
            if (!string.IsNullOrEmpty(referUrl))
                req.Referer = referUrl;
            req.UserAgent = DefaultUserAgent;
            req.KeepAlive = true;
            req.ServicePoint.Expect100Continue = false;
            req.Headers.Add("Accept-Encoding", "gzip,deflate");
            req.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.8,en;q=0.5,de-de;q=0.3");
            */
            /*
             request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Headers.Add("Accept-Encoding", "gzip,deflate,br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4");
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = false;
            request.ServicePoint.Expect100Continue = false;
            request.Referer = Referer;
            request.Timeout = 35000;
             
             
             */


            //req.Headers.Add("Host", host.Replace("https://", "").Replace("http://", ""));
            //req.Headers.Add("DNT", "1");
            if (WebHeaderCollection.IsRestricted("Host"))
            {
                //req.Headers.Set(HttpRequestHeader.Host, host.Replace("https://", "").Replace("http://", ""));
                SetHeaderValue(req.Headers, "Host", host.Replace("https://", "").Replace("http://", ""));
            }

            req.Headers.Add("Upgrade-Insecure-Requests", "1");

            //req.Method = Post ? "Post" : "GET";
            if (cc != null)
            {

                //if (cc.Count>0 && cc[cc.Count-1].Name != "ai_user")
                //{
                //    string ai_user = string.Format("n3YUx |{0}", DateTime.UtcNow.ToString("yyyy MM ddThh:mm:ss.000"));
                //    cc.Add(new Cookie("ai_user", ai_user, "/", req.Host));
                //}
                List<string> cs = new List<string>();

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
        public string ToServer(string host, string url, string PostData, Encoding Encode, Action<string> succFunc, Action<string> faileFunc, bool Post = true, CookieCollection inCookie = null)
        {


            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;
            handler.UseCookies = true;
            handler.CookieContainer = new CookieContainer();
            if (inCookie != null)
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

                    res = req.GetAsync(host + res.Headers.Location).Result;
                }
                ret = res.Content.ReadAsStringAsync().Result;
                succFunc?.Invoke(ret);
            }
            catch (Exception ce)
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

    

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading;
namespace WindowsFormsApplication1
{
    public partial class NetSelectFrm : Form
    {
        public NetSelectFrm()
        {
            InitializeComponent();
        }
    }
    public class HostInfo
    {
        public string name;
        public long times;
        public string RequestVerificationToken;
    }
    public class CommitClass
    {
        public static String strRequestVerificationToken;
        public static String strLastRequestVerificationToken;
        public static string strCookie;
        public static string strOrgCookie;
        public static string strReqCookie;
        public static String HostName;
        public static String DefaultHostName = "331";
        public static List<HostInfo> HostList;
        public static HtmlDocument doc;
        public static string JQueryLib = null;
         static Dictionary<string, string> FullCookies;
        public static string HomePage { get { return string.Format("https://www.kcai{0}.com",DefaultHostName); } }
        static bool WebLoaded = false;
        /*
        public static bool Login(String info){
        try {
            // Simulate network access.
            String urlAddress = "https://www.kcai%s.com/Login/PostLogin";
            urlAddress = String.Format(urlAddress,HostName);
            Uri url = new Uri(urlAddress);
            HttpWebRequest conn = (HttpURLConnection) url.openConnection();
            conn.setConnectTimeout(15000);
            conn.setRequestMethod("POST");
            conn.setReadTimeout(5000);
            conn.setRequestProperty("Set-Cookie",strRequestVerificationToken);
            OutputStream outStream = conn.getOutputStream();
            outStream.write(info.getBytes());
            // 判断请求Url是否成功
            if (conn.getResponseCode() != 200) {
                //throw new RuntimeException("请求url失败");
                return false;
            }
            InputStream inStream = conn.getInputStream();
            byte[] bt = LoginActivity.StreamTool.read(inStream);
            String retRes = new String(bt, "UTF-8");
            if(retRes.equals("suc")) {
                return true;
            }
        }
        catch (Exception e) {
            System.out.print(e.getMessage());
        }
        return false;
    }

        public static bool Send(String info)
        {
            InputStream inStream = null;
            try
            {
                String urlAddress = "https://www.kcai%s.com/Login/PostLogin";
                urlAddress = String.format(urlAddress, HostName);
                URL url = new URL(urlAddress);
                HttpURLConnection conn = (HttpURLConnection)url.openConnection();
                conn.setConnectTimeout(5000);
                conn.setRequestMethod("POST");
                // 判断请求Url是否成功
                if (conn.getResponseCode() != 200)
                {
                    throw new RuntimeException("请求url失败");
                }
                inStream = conn.getInputStream();
                byte[] bt = LoginActivity.StreamTool.read(inStream);
                String retRes = new String(bt, "UTF-8");
                JSONObject jobj = new JSONObject(retRes);
                if (retRes.equals("Succ"))
                {
                    return true;
                }
                inStream.close();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        */
        public static InstsClass GetInst()
        {
            InstsClass ret = null;
            try
            {
                String urlAddress = "http://www.wolfinv.com/requestInsts.asp";
                HttpWebRequest req = WebRequest.Create(urlAddress) as HttpWebRequest;
                //req.AllowAutoRedirect= true;
                req.Method = "GET";
                req.Timeout = 5000;
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }
                StreamReader reader = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                string strJson = reader.ReadToEnd();
                ret = new InstsClass();
                ret.FillByJson<InstsClass>(strJson);
                reader.Close();
            }
            catch (Exception e)
            {
                return ret;
            }
            return ret;
        }
        
        public static string getFullCookie()
        {
            if (FullCookies == null)
            {
                FullCookies = new Dictionary<string, string>();
                string[] cookies = strOrgCookie.Split(';');
                for (int i = 0; i < cookies.Length; i++)
                {
                    string cookieString = cookies[i].Trim();
                    if (cookieString.Length == 0 || cookieString.Split('=').Length < 2) continue;
                    string strKey=cookieString.Split('=')[0].Trim();
                    if (FullCookies.ContainsKey(strKey)) continue;
                    FullCookies.Add(strKey, cookieString);
                }
                cookies = strCookie.Split(',');
                for (int i = 0; i < cookies.Length; i++)
                {
                    string cookieString = cookies[i].Trim();
                    if (cookieString.Length == 0 || cookieString.Split('=').Length < 2) continue;
                    string strKey = cookieString.Split('=')[0].Trim();
                    if (FullCookies.ContainsKey(strKey)) continue;
                    FullCookies.Add(strKey, cookieString);
                }
            }
            return string.Join(";",FullCookies.Values.ToArray<string>());
        }

        public static void ResetWebStatus()
        {
            WebLoaded  =false;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }

        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134";

        public static CommitResultClass LoginHost(string strCode)
        {
            CommitResultClass ret = PostToHost("/Login/PostLogin",string.Format("{0}&__RequestVerificationToken={1}",strCode,strRequestVerificationToken)); 
            if(!ret.Suc)
                return ret;
            if(ret.StringResult != "suc") 
            {
                ret.Suc = false;
                ret.Message = ret.StringResult;
                return ret;
            }
            return ret;
        }

        public static CommitResultClass SendInst(string strCode)
        {
            CommitResultClass ret = PostToHost("/Bet/CqcSubmit",string.Format("{0}&__RequestVerificationToken={1}",strCode,strRequestVerificationToken)); 
            if(!ret.Suc)
                return ret;
            if(ret.StringResult.IndexOf("suc") <0) 
            {
                ret.Suc = false;
                ret.Message = ret.StringResult ;
                return ret;
            }
            ret.Suc = true;
            //ret.JsonResult =
            return ret;
        }

        static CommitResultClass PostToHost(string url,string strJson)
        {
            CommitResultClass ret = new CommitResultClass();
            String urlAddress = String.Format("https://www.kcai{0}.com{1}",DefaultHostName,url);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3 | (SecurityProtocolType)3072 | (SecurityProtocolType)768;
            HttpWebRequest req = null;
            try
            {
                req = WebRequest.Create(urlAddress) as HttpWebRequest;
                req.ProtocolVersion = HttpVersion.Version11;
                req.Timeout = 15000;
                req.UserAgent = DefaultUserAgent;
                //req.Host = CommitClass.HomePage.Replace("https://","");
                //req.Referer = CommitClass.HomePage ;
                //req.KeepAlive = true;
                //req.Accept = "application/json, text/javascript, */*; q=0.01";
                req.AllowAutoRedirect = true;
                //req.Host = urlAddress.Replace("https://", "");
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";//"Content-type", "application/x-www-form-urlencoded;charset=utf-8"
                //req.Headers["Content-type"] = "application/x-www-form-urlencoded;charset=utf-8";
                req.Headers["Origin"] = CommitClass.HomePage;
                //req.Headers["X-Requested-With"] = "XMLHttpRequest";// "X-Requested-With", "XMLHttpRequest" '
                req.Headers["Cookie"] = strReqCookie ; //strLastRequestVerificationToken!=null?strCookie.Replace(strRequestVerificationToken,strLastRequestVerificationToken):strRequestVerificationToken;
                req.Method = "POST";
                byte[] data = Encoding.UTF8.GetBytes(strJson);
                req.ContentLength = data.Length;
                Stream newStream = req.GetRequestStream();
                // Send the data.  
                newStream.Write(data, 0, data.Length);
                newStream.Close();
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    ret.Suc = false;
                    ret.Message = "无法打开网页";
                    ret.Error = ret.Message;
                    return ret;
                }
                StreamReader reader = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                string strRet = reader.ReadToEnd();
                reader.Close();
                ret.Suc = true;
                ret.StringResult = strRet;
                return ret;
            }
            catch (Exception ce)
            {
                ret.Error = ce.Message;
                ret.Message = ce.StackTrace;
                return ret;
            }
        }

        
        public static void SelectQuickestHost()
        {
            HostList = new List<HostInfo>();
            HostName = DefaultHostName;
            String hosts = "779";//,776,337,773,331,334,775,338,555";
            String[] hostArr = hosts.Split(',');
            long MinsecCnt = 100000000;
            for (int i = 0; i < hostArr.Length; i++)
            {
                long TimeDiff = 100000000;
                HostInfo hi = new HostInfo();
                hi.name = hostArr[i];
                hi.times = TimeDiff;
                hi.RequestVerificationToken = null;
                HostList.Add(hi);
                try
                {
                    String urlAddress = "https://www.kcai{0}.com";
                    String _RequestVerificationToken=null;
                    string _Cookie = null;
                    urlAddress = String.Format(urlAddress, hostArr[i]);
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3 | (SecurityProtocolType)3072 | (SecurityProtocolType)768;
                    HttpWebRequest req = WebRequest.Create(urlAddress) as HttpWebRequest;
                    req.ProtocolVersion = HttpVersion.Version11;
                    req.Timeout = 15000;
                    req.UserAgent = DefaultUserAgent;
                    req.AllowAutoRedirect = false;
                    req.Referer = urlAddress.Replace("https://", "");
                    //req.Host = urlAddress.Replace("https://", "");
                    req.ContentType = "application/x-www-form-urlencoded";
                    
                    req.Method = "GET";
                    DateTime currtime = DateTime.Now;
                    HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                    if (resp.StatusCode != HttpStatusCode.OK)
                    {
                        continue;
                    }
                    if (req.CookieContainer != null)
                    {
                    }

                    TimeDiff = DateTime.Now.Subtract(new DateTime()).Milliseconds;//访问时差

                    string[] keyvals = resp.Headers.GetValues("Set-Cookie");
                    Dictionary<string, string> CurrCookies = new Dictionary<string, string>();
                    if (keyvals.Length > 0)
                    {
                        _Cookie = resp.Headers["Set-Cookie"];
                        for (int j = 0; j < keyvals.Length; j++)
                        {
                            if (keyvals[j].Trim().Length == 0) continue;
                            string strKeyVal = keyvals[j].Trim().Split(';')[0];
                            string strKey = strKeyVal.Split('=')[0].Trim();
                            string strVal = strKeyVal.Split('=')[1].Trim();
                            if (strKey == "__RequestVerificationToken")
                                _RequestVerificationToken = strVal;
                            if (CurrCookies.ContainsKey(strKey)) continue;
                            CurrCookies.Add(strKey, string.Format("{0}={1}",strKey,strVal));
                        }
                    }
                    if (TimeDiff < MinsecCnt)
                    {
                        MinsecCnt = TimeDiff;
                        DefaultHostName = hostArr[i];
                        strRequestVerificationToken = _RequestVerificationToken;
                        strCookie = _Cookie;
                        hi.times = TimeDiff;
                        hi.RequestVerificationToken = _RequestVerificationToken;
                        strReqCookie = string.Join(",", CurrCookies.Values.ToArray<string>());
                    }
                }
                catch (Exception e)
                {
                    continue;
                }

            }
        }
        
        public static string TranslateByWB(string strIn)
        {
            
            if (WebLoaded == false)
            {
                return null;
            }
            if (!LoadHtml(true))
            {
                return null;
            }
            try
            {
                doc.InvokeScript("TranslateWB", new object[] { strIn });
                HtmlElement he = doc.GetElementById("OutputData");
                if (he == null) return null;
                return he.GetAttribute("value");
            }
            catch (Exception e)
            {
                return null;
            }

            return null;
        }

        public static string TranslateInstByWB(string expect,string strIn)
        {

            if (WebLoaded == false)
            {
                return null;
            }
            if (!LoadHtml(true))
            {
                return null;
            }
            try
            {
                doc.InvokeScript("TranslateInstWB", new object[] { expect,strIn });
                HtmlElement he = doc.GetElementById("OutputData");
                if (he == null) return null;
                return he.GetAttribute("value");
            }
            catch (Exception e)
            {
                return null;
            }

            return null;
        }


        public static bool LoadHtml(bool LocalFile)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "model.html";
            if (WebLoaded == false)
            {
                ////ThreadStart ts = new ThreadStart(OpenAWb);
                ////Thread th = new Thread(ts);
                ////th.IsBackground = true;
                ////th.SetApartmentState(ApartmentState.STA);
                DateTime curr = DateTime.Now;
                OpenAWb(LocalFile?path:HomePage);
                //th.Start();
                while (!WebLoaded)
                {
                    DateTime newCurr = DateTime.Now;
                    long secDiff = (newCurr.Ticks - curr.Ticks) / 10000000;
                    if (secDiff > 60)
                    {
                        return false;
                    }
                    Application.DoEvents();
                    Thread.Sleep(500);
                }
                if (WebLoaded)
                    return true;
                else
                    return false;
            }
            return true;
        }

        static void OpenAWb(string path)
        {
            
            try
            {
                WebBrowser wb1 = new WebBrowser();
                wb1.ScriptErrorsSuppressed = true;
                wb1.ProgressChanged += new WebBrowserProgressChangedEventHandler(wb_ProgressChanged);
                wb1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
                //wb1.Navigate("about:blank");
                wb1.Navigate(path);
                //wb.Url = new Uri(@"file://" + path);
                
            }
            catch (Exception ce)
            {
                return ;
            }
        }

        static void wb_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            wb_DocumentCompleted(sender, null);
        }

        static void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser obj = sender as WebBrowser;
            if (obj.ReadyState < WebBrowserReadyState.Complete) return;
            WebLoaded = true;
            doc = obj.Document;
            if (obj.Url.AbsoluteUri.IndexOf("www.kcai") > 0)
            {
                strOrgCookie = obj.Document.Cookie;
                HtmlElement _TokenEle = obj.Document.GetElementById("__RequestVerificationToken");
                if(_TokenEle != null)
                    strLastRequestVerificationToken = _TokenEle.GetAttribute("value");
            }
        }

        public static string Translate(string strIn)
        {
            CommitClass.getJQueryLib();
            string path = AppDomain.CurrentDomain.BaseDirectory + "pure.js";
            string str2 = File.ReadAllText(path);
            string fun = string.Format(@"Translate('{0}')", strIn);
            string result = ExecuteScript(fun, str2,null);
            return result;
        }

        /// <summary>
         /// 执行JS
         /// </summary>
         /// <param name="sExpression">参数体</param>
         /// <param name="sCode">JavaScript代码的字符串</param>
         /// <returns></returns>
         private static string ExecuteScript(string sExpression, string jsCode,string libCode)
         {
             MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
             scriptControl.UseSafeSubset = true;
             //scriptControl.
             scriptControl.Language = "JScript";
             try
             {
                 if(libCode!=null)
                    scriptControl.AddCode(libCode);
                 scriptControl.AddCode(jsCode);
                 string str = scriptControl.Eval(sExpression).ToString();
                 return str;
             }
             catch (Exception ex)
            {
                string str = ex.Message;
            }
            return null;
        }

         public static string getJQueryLib()
         {
             if (JQueryLib != null) return JQueryLib;
             string url = "http://code.jquery.com/jquery-latest.js";
             try
             {
              
                 HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
                 //req.AllowAutoRedirect= true;
                 req.UserAgent = DefaultUserAgent;
                 req.Method = "GET";
                 req.Timeout = 30000;
                 req.ContentType = "application/x-www-form-urlencoded";
                 HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                 if (resp.StatusCode != HttpStatusCode.OK)
                 {
                     return null;
                 }
                 StreamReader reader = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                 string strJson = reader.ReadToEnd();
                 JQueryLib = strJson;
                 reader.Close();
                 return strJson;
             }
             catch (Exception e)
             {
                 return null;
             }
             return null;
         }
    }

    public class CommitResultClass
    {
        public bool Suc;
        public string Error;
        public string Message;
        public string StringResult;
        public JsonableClass JsonResult;
    }

    public class HostUser : JsonableClass
    {
        public string userName;
        public string password;
        public string valicode;
    }

   

    public class InstsClass:JsonableClass
    {
        public String Expect;
        public String Insts;
    }

    public interface IJsonable
    {
        String FillByJson<T>(String strJson);
        String ToJsonString<T>();
    }

    public class  JsonableClass:IJsonable
    {
        public string FillByJson<T>(string strJson)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                T m = js.Deserialize<T>(strJson);// //反序列化
                FieldInfo[] ps =  m.GetType().GetFields();
                for (int i = 0; i < ps.Length; i++)
                {
                    ps[i].SetValue(this,ps[i].GetValue(m));
                }
            }
            catch (Exception ce)
            {
                return ce.Message;
            }
            return null;
        }

        public string ToJsonString<T>()
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                string jsonData = js.Serialize(this);//序列化
                return jsonData;
            }
            catch (Exception ce)
            {
                return null;
            }
        }
    }


    public class UserInfo:JsonableClass
    {
        public String UserCode;
        public String Password;
        public String BankCode;
        public String ExpireDate;
        public int GropuId;
        public int UserType;
        String outStr;
        public String ToInfoText()
        {
            String strModel = "%s;级别:%s;到期日:%s";
            return String.Format(strModel, this.UserCode, getUserTypeString(this.UserType), this.ExpireDate);
        }

        public String getUserTypeString(int n)
        {
            String ret = "";
            switch (n)
            {

                case 1:
                    ret = "一般猎手";
                    break;
                case 2:
                    ret = "高级猎手";
                    break;
                case 3:
                    ret = "专业猎手";
                    break;
                default:
                    ret = "管理员";
                    break;
            }
            return ret;
        }
    }

}

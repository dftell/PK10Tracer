using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.WebRuleLib;
using System.Threading;
using mshtml;
using System.IO;
using System.Net;

namespace LotteryDataCenter
{
    public partial class DataPointPage : UserControl
    {
        bool Logined = false;
        bool InSleep = false;
        bool WebBrowserLoad = false;
        bool ReadySleep =false;
        bool loading = false;
        WebRule wr;
        private System.Windows.Forms.WebBrowser webBrowser1;
        public LotteryAccessDataPoint ldp;
        public DataPointPage()
        {
            InitializeComponent();
            
        }

        private void DataPointPage_Load(object sender, EventArgs e)
        {
            
        }

        public void LoadPage()
        {
            if (ldp.needNavigateAccess)
            {
                string url = string.Format(ldp.loginPageUrl, ldp.host);
                /*
                ParameterizedThreadStart ps = new ParameterizedThreadStart(initWeb);
                Thread t = new Thread(ps);
                t.IsBackground = true;
                t.ApartmentState = ApartmentState.STA;
                t.Start();
                */
                initWeb(url);
            }
        }

        public void refreshData()
        {

        }

        public void initWeb(object url)
        {
            wr = WebRuleBuilder.Create(ldp.PointId, ldp, Program.gblc);
            this.webBrowser1 = new WebBrowser();
            initWebRule();
            this.webBrowser1.DocumentCompleted += webCompleted;
            this.webBrowser1.Parent = this;
            this.webBrowser1.Dock = DockStyle.Fill;
            this.rb_displayWeb.Checked = true;
            this.webBrowser1.Navigate(string.Format("{0}",url));
        }
        void initWebRule()
        {
            this.webBrowser1.ObjectForScripting = wr;
            wr.SuccLogin = AfterLogined;
            wr.SuccGetGameInfo = AfterGetGameInfo;
            wr.AJaxError = AfterAJaxError;
            wr.MsgBox = WXMsgBox;
        }

        private static string htmlstr;

        private static void GetHtmlWithBrowser(object url)
        {

            htmlstr = string.Empty;



            WebBrowser wb = new WebBrowser();
            wb.AllowNavigation = true;
            wb.Url = new Uri(url.ToString());
            DateTime dtime = DateTime.Now;
            double timespan = 0;
            while (timespan < 10 || wb.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
                DateTime time2 = DateTime.Now;
                timespan = (time2 - dtime).TotalSeconds;
            }                       
            if (wb.ReadyState == WebBrowserReadyState.Complete)
            {
                htmlstr = wb.DocumentText;
            }
        }



        /// <summary>
        /// 在单线程中启用浏览器
        /// </summary>
        public static void RunWithSingleThread(object url, ref string html)
        {
            ParameterizedThreadStart ps = new ParameterizedThreadStart(GetHtmlWithBrowser);
            Thread t = new Thread(ps);
            t.IsBackground = true;
            t.ApartmentState = ApartmentState.STA;
            t.Start(url);
            html = htmlstr;
        }

        void AfterAJaxError(WebServerReturnClass err)
        {
            WXMsgBox(err.Title ?? "Error", err.Msg);
            //Logined = false;
        }

        void AfterGetGameInfo(GameInfoClass gic)
        {
            if(!gic.Succ)
            {
                return;
            }
            else
            {

            }
        }
        void setLogined()
        {
            Logined = true;
            InSleep = false;
            WebBrowserLoad = true;
        }
        void AfterLogined(WebUserInfoClass wic)
        {
            if (!wic.Succ)
            {
                WXMsgBox(string.Format("账号{0}失败", ldp.loginId), wic.Msg);
                return;
            }
            //WXMsgBox(Program.gc.ClientAliasName, string.Format("登陆{0}成功！", Program.gc.ForWeb));
            setLogined();
            try
            {
                lock (webBrowser1)
                {
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.Navigate(string.Format(ldp.LotteryPage, ldp.host));
                }
                Application.DoEvents();
                Thread.Sleep(3 * 1000);
                //getGameInfo(70);
                Application.DoEvents();
                Thread.Sleep(2 * 1000);
                WXMsgBox(string.Format("账号{0}登录成功", ldp.loginId), "成功");
            }
            catch
            {

            }
        }

        public static void WXMsgBox(string title, string msg)
        {
            //MessageBox.Show(string.Format("{0}:{1}",title,msg));
            //return;
            Program.wxl.Log(title, msg, string.Format(Program.gblc.WXLogUrl, Program.gblc.WXSVRHost));
        }
        HtmlDocument HomeDocIE;
        HtmlDocument loginDocIE;
        bool IsLoadComplete(string key)
        {
            HtmlDocument doc = webBrowser1.Document;
            bool succ = WebRule.existElement(HomeDocIE ?? doc, key);
            if (succ)
            {
                HomeDocIE = doc;
                loginDocIE = doc;
                return true;
            }
            for (int i = 0; i < doc.Window.Frames.Count; i++)
            {
                succ = WebRule.existElement(doc.Window.Frames[i], key);
                if (succ)
                {
                    HomeDocIE = doc;
                    loginDocIE = doc;
                    return true;
                }
            }
            return false;
        }
        private void webCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                HtmlDocument doc = webBrowser1.Document;
                if (!Logined)
                {
                    if (IsLoadComplete(ldp.hostKey))//如果加载到主页，那就登录
                    {
                        string strUrl = doc.Url.ToString().Replace("//", "/");
                        string lotteryPage = string.Format(ldp.LotteryPage, string.Format(ldp.loginPageUrl, ldp.host)).Replace("//", "/");
                        if (strUrl == lotteryPage)
                        {


                            Logined = true;
                            InSleep = false;
                            WebBrowserLoad = true;
                            //TSMI_sendStatusInfor_Click(null, null);
                            return;
                        }
                        WXMsgBox("判断是在登陆页", "登录！");
                        //InSleep = false;
                        //WebBrowserLoad = true;
                        Login();
                        return;
                    }
                    //WXMsgBox("判断不在登陆页", doc.Body.OuterHtml.Substring(0,Math.Min(doc.Body.OuterHtml.Length,30)));
                    return;
                }
                
                //CurrVal = wr.GetCurrMoney(doc);
                if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
                {

                    //CookieCollection cc = getCookie(this.webBrowser1.Document.Cookie);
                    //if(Program.gc.SendMsgFromWebRequest == "1")
                    //    this.ce.SetCookie(cc);
                    HomeDocIE = this.webBrowser1.Document;
                    if (ReadySleep)
                    {
                        //LogableClass.ToLog(webBrowser1.Url.Host, "进入睡眠！");
                        ReadySleep = false;
                        return;
                    }
                    //LogableClass.ToLog(webBrowser1.Url.Host, "唤醒！");
                    bool IsVaildWeb = wr.IsVaildWeb(doc);
                    bool IsLogined = wr.IsLogined(doc);
                    if (IsVaildWeb && IsLogined == false)
                    {
                        Thread.Sleep(5 * 1000);
                        IsLogined = wr.IsLogined(doc);
                    }

                    bool IsLoadCompleted = IsLoadComplete(ldp.hostKey);// wr.IsLoadCompleted(doc);
                    if (IsLoadCompleted || IsLogined) //缓存已登录
                    {
                        WebBrowserLoad = true;
                    }
                    if (IsLoadCompleted)//一旦加载的有网站标志，那就是主页
                    {
                        HomeDocIE = webBrowser1.Document;
                    }
                    
                    if (!IsLoadCompleted)
                    {
                        loading = true;
                        //this.toolStripStatusLabel1.Text = "未完全载入！";
                        return;
                    }

                    loading = false;
                    if (IsVaildWeb || IsLogined)//是登录页面或者内容页面
                    {
                        if (IsVaildWeb)
                        {
                            if (!WebBrowserLoad || loginDocIE != null)//第一次载入
                            {
                                Login();

                            }
                            else//加载文件后事件
                            {
                                bool TryLogin = wr.IsLogined(doc);
                                //加载自定义脚本后的处理
                                if (TryLogin == false)
                                {
                                    //LogableClass.ToLog(webBrowser1.Url.Host, "还没完全加载！");
                                    return;
                                }
                                Logined = true;
                                if (!IsLogined)
                                {
                                    //LogableClass.ToLog(webBrowser1.Url.Host, "密码错误！");
                                    return;
                                }
                                else
                                {
                                    //LogableClass.ToLog(webBrowser1.Url.Host, "登堂入室！");
                                    return;
                                }

                            }
                        }
                        else//已经登录了
                        {
                            //不是登录网页就一定是已经登录了
                            //LogableClass.ToLog(webBrowser1.Url.Host, "网页载入后但未出现预期内容！");
                            return;
                        }
                    }
                    else //其他内容
                    {
                        if (WebBrowserLoad)//已经登入
                        {
                            if (Logined)//建议更换主机地址
                            {
                                //LogableClass.ToLog(webBrowser1.Url.Host, "无法访问主机，建议启动服务选择合适的备用服务！");
                                reLoadWebBrowser();
                                return;
                            }
                            else
                            {
                                reLoadWebBrowser();
                                //LogableClass.ToLog(webBrowser1.Url.Host, "登录后无法访问主机，建议更换主机重新登录！");
                                return;
                            }
                        }
                        else
                        {
                            //其他网页
                            //LogableClass.ToLog(webBrowser1.Url.Host, "睡眠综合症！");
                            return;
                        }
                    }
                }
                else
                {
                    if (wr.IsLogined(doc))
                    {
                        Logined = true;
                        WebBrowserLoad = true;
                    }
                    if (!WebBrowserLoad)//中间状态，不理睬
                    {
                        return;
                    }
                }
            }
            catch (Exception ce)
            {
                WXMsgBox(ce.Message, ce.StackTrace);
            }

        }

        void reLoadWebBrowser()
        {
            ReadySleep = true;
            string url = "https://www.baidu.com";
            try
            {
                lock (webBrowser1)
                    this.webBrowser1.Navigate(url);
                if ((webBrowser1.ReadyState == WebBrowserReadyState.Complete ))
                {
                    //this.webBrowser1.Navigate(url);
                }
                else
                {
                    WXMsgBox(string.Format("[{0}]程序们，再坚持一会", ldp.PointTitle), "让其他屌丝们干完事了一起睡！");
                }
            }
            catch (Exception ce)
            {
                ReadySleep = false;
                Thread.Sleep(2 * 60 * 1000);
                //LogableClass.ToLog("错误", "此刻无法入眠", string.Format("{0}:{1}", ce.Message, ce.StackTrace));
                Program.wxl.Log(string.Format("[{0}]错误", ldp.PointTitle), "此刻无法入眠，让我再酝酿一番如何？", string.Format("{0}:{1}", ce.Message, ce.StackTrace), string.Format(Program.gblc.WXLogUrl, Program.gblc.WXSVRHost));
                return;
            }
            WebBrowserLoad = false;
            Logined = false;
            InSleep = true;
            //Set_SendTime(false);
            Program.wxl.Log(string.Format("[{0}]洗洗睡吧，明日再收数据！", ldp.PointTitle), string.Format(Program.gblc.WXLogUrl, Program.gblc.WXSVRHost));
        }


        void Login()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            AddScript(loginDocIE ?? HomeDocIE); //执行js代码,未来修改为网络载入
            //MySleep(3 * 1000);//等待验证码图片载入
            (loginDocIE ?? HomeDocIE).InvokeScript("Login", new string[] { ldp.loginId, ldp.loginPwd, string.Format(Program.gblc.WXLogUrl, Program.gblc.WXSVRHost), Program.gblc.WXLogNoticeUser, ldp.host });
            WebBrowserLoad = true;
            
        }
        string getScriptText(string scriptName, string subFolder = "")
        {
            string ret = "";
            string FilePath = string.Format("{0}\\{1}\\{2}", Application.StartupPath, subFolder, scriptName);
            try
            {
                StreamReader fs = new StreamReader(FilePath, Encoding.Default);
                ret = fs.ReadToEnd();
                fs.Close();
            }
            catch (Exception e)
            {
                //throw e;
            }
            return ret;
        }

        void AddScript(HtmlDocument IEDoc, string ElementName = "body", string LanguageEncoding = "gb2312")
        {
            string scriptFile = ldp.host.Replace("https", "");
            scriptFile = scriptFile.Replace("http", "");
            scriptFile = scriptFile.Replace("://", "");
            scriptFile = scriptFile.Replace("/", "");
            string strFolderName = string.Format("jscript\\{0}", ldp.PointId);
            string jstxt = this.getScriptText("addJscript.js", strFolderName);
            IEDoc.Encoding = LanguageEncoding;
            HtmlElementCollection elHeads = IEDoc.GetElementsByTagName(ElementName);
            if (elHeads == null)
            {
                Program.wxl.Log("网页未正常加载，无法注入代码！");
                return;
            }
            if (elHeads.Count == 0)
            {
                Program.wxl.Log("未获取到文件头！");
                return;
            }
            HtmlElement head = elHeads[0];
            //创建script标签
            HtmlElement scriptEl = IEDoc.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            //给script标签加js内容
            element.text = jstxt;
            //将script标签添加到head标签中
            head.AppendChild(scriptEl);
            //ScriptLoaded = true;
        }


        public void login()
        {

        }

        private void rb_displayWeb_CheckedChanged(object sender, EventArgs e)
        {
            if(this.webBrowser1!=null)
            {
                
                this.webBrowser1.Visible = rb_displayWeb.Checked;
                if (this.webBrowser1.Visible)
                    this.webBrowser1.BringToFront();
                else
                    this.webBrowser1.SendToBack();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using mshtml;
using System.IO;
using Microsoft.VisualBasic;
using WolfInv.com.WebCommunicateClass;
using WolfInv.com.WebRuleLib;
using WolfInv.com.BaseObjectsLib;
using System.Timers;
using System.Threading;
using WolfInv.com.LogLib;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using WolfInv.com.RemoteObjectsLib;

namespace ExchangeTermial
{
    public partial class MainWindow : Form
    {
        Int64 NewExpect;
        bool WebBrowserLoad = false;
        Dictionary<int, RequestClass> CurrDic = new Dictionary<int, RequestClass>();
        DateTime LastInstsTime = DateTime.MaxValue;
        Dictionary<string, string> AssetUnitList;
        long RefreshTimes = 0;
        Dictionary<long, string> dicExistInst ;
        bool InSleep = false;
        bool Logined = false;
        WebRule wr = null;
        bool ReadySleep = false;
        double CurrVal
        {
            get
            {
                HtmlDocument doc = webBrowser1.Document;
                return wr.GetCurrMoney(doc);
            }
        }
        System.Timers.Timer SendStatusTimer = new System.Timers.Timer();
        public MainWindow()
        {
            InitializeComponent();
            wr = WebRuleBuilder.Create(Program.gc);
            this.Text = string.Format("{0}[{2}][v:{1}]", this.Text, Program.VerNo, Program.gc.ClientUserName);
            Program.Title = this.Text;
        }

        void Set_SendTime(bool Running,long InterVal = 20 * 60 * 1000)
        {
            
            SendStatusTimer.Interval = InterVal;
            SendStatusTimer.Elapsed += SendStatusTimer_Elapsed;
            SendStatusTimer.Enabled = Running;
            if(Running)
            {
                SendStatusTimer_Elapsed(null, null);
            }
        }

        private void SendStatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string ip = MyIpInfo;
            string id = Program.gc.ClientUserName;
            string money = string.Format("{0:f2}", CurrVal);
            string ver = Program.VerNo;
            string urlModel = "{0}{1}{2}";
            string getModel = @"?Ip={0}&User={1}&CurrVal={2}&Logined={3}&Sleep={4}&Load={5}&Ver={6}";
            string reqmsg = string.Format(getModel,ip,id,money,Logined?1:0,InSleep?1:0,WebBrowserLoad?1:0,ver);
            string url = string.Format(urlModel, Program.gc.InstHost, Program.gc.StatusUrlModel, reqmsg);
            CommResult cr = new CommunicateToServer().SendStatusInfo(url);
            if (!cr.Succ)
            {
                RefreshSendStatusInfo(cr.Message);
            }
            else
            {
                RefreshSendStatusInfo("朕已知悉！");
                //Program.wxl.Log("可用余额:" + money);
            }
        }

        string _MyIpInfo = null;
        string MyIpInfo
        {
            get
            {
                if (_MyIpInfo == null)
                {
                    _MyIpInfo = GetIpInfo();
                }
                return _MyIpInfo;
            }
        }

        private string GetIpInfo()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
                                                                    //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];
            string ip;
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    var content = webClient.DownloadString("http://www.net.cn/static/customercare/yourip.asp"); //站获得IP的网页 "http://www.ip138.com/ips1388.asp"
                                                                                                //判断IP是否合法
                    ip = new Regex(@"((\d{1,3}\.){3}\d{1,3})").Match(content).Groups[1].Value;
                }
                catch(Exception ce)
                {
                    ip = localaddr.ToString();
                }
            }
            return string.Format("{0}:{1}",hostName,ip);
            //return localaddr.ToString();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            AssetUnitList = getAssetLists();
            dicExistInst = new Dictionary<long, string>();
            LoadUrl();


        }

        void LoadUrl()
        {
            Logined = false;
            WebBrowserLoad = false;
            //string url = Program.gc.LoginUrlModel.Replace("{host}", Program.gc.LoginDefaultHost);
            string url = string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost);
            try
            {
                
                //this.webBrowser1 = null;
                //this.webBrowser1 = new WebBrowser();
                this.webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
                //this.webBrowser1.Url = new Uri(url);
                this.webBrowser1.Navigate(url);
                this.webBrowser1.ScriptErrorsSuppressed = true;
                LogableClass.ToLog(webBrowser1?.Url?.Host, "准备唤醒！");

            }
            catch(Exception ce)
            {
                LogableClass.ToLog("错误", ce.Message, ce.StackTrace);
                Program.wxl.Log("错误","唤醒网页错误",string.Format("{0}:{1}", ce.Message, ce.StackTrace));
            }
            this.timer_RequestInst.Interval = 60 * 1000;
            this.timer_RequestInst.Enabled = true;
            Set_SendTime(true);
        }

        void AddScript()
        {
            HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            //创建script标签
            HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            //给script标签加js内容
            string scriptFile = Program.gc.LoginUrlModel.Replace("https", "");
            scriptFile = scriptFile.Replace("http", "");
            scriptFile = scriptFile.Replace("://", "");
            ////scriptFile = scriptFile.Replace(".", "_");
            element.text = this.getScriptText(string.Format("{0}_Pure.js", scriptFile));
            //将script标签添加到head标签中
            head.AppendChild(scriptEl);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            HtmlDocument doc = webBrowser1.Document;
            //CurrVal = wr.GetCurrMoney(doc);
            if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
            {
                
                if (ReadySleep)
                {
                    LogableClass.ToLog(webBrowser1.Url.Host,"进入睡眠！");
                    ReadySleep = false;
                    return;
                }
                LogableClass.ToLog(webBrowser1.Url.Host, "唤醒！");
                bool IsVaildWeb = wr.IsVaildWeb(doc);
                bool IsLogined = wr.IsLogined(doc);
                if (IsVaildWeb||IsLogined)//是登录页面或者内容页面
                {
                    if (IsVaildWeb)
                    {
                        if (!WebBrowserLoad)//第一次载入
                        {
                            LogableClass.ToLog(webBrowser1.Url.Host, "首次进入，加载文件！");
                            AddScript(); //执行js代码,未来修改为网络载入
                            webBrowser1.Document.InvokeScript("Login", new string[] { Program.gc.ClientUserName, Program.gc.ClientPassword });
                            WebBrowserLoad = true;

                        }
                        else//加载文件后事件
                        {
                            bool TryLogin = wr.IsLogined(doc);
                            //加载自定义脚本后的处理
                            ////while (!TryLogin)
                            ////{
                            ////    if (wr.IsLogined(doc))
                            ////        break;
                            ////    Thread.Sleep(10 * 1000);
                            ////}
                            if(TryLogin == false)
                            {
                                LogableClass.ToLog(webBrowser1.Url.Host, "还没完全加载！");
                                return;
                            }
                            //CurrVal = wr.GetCurrMoney(doc);
                            Logined = true;
                            //this.timer_RequestInst_Tick(null, null);
                            if (!IsLogined)
                            {
                                LogableClass.ToLog(webBrowser1.Url.Host, "密码错误！");
                                return;
                            }
                            else
                            {
                                LogableClass.ToLog(webBrowser1.Url.Host, "登堂入室！");
                                return;
                            }

                        }
                    }
                    else//已经登录了
                    {
                        //不是登录网页就一定是已经登录了
                        LogableClass.ToLog(webBrowser1.Url.Host, "网页载入后但未出现预期内容！");
                        return;
                    }
                }
                else //其他内容
                {
                    if (WebBrowserLoad)//已经登入
                    {
                        if (Logined)//建议更换主机地址
                        {
                            LogableClass.ToLog(webBrowser1.Url.Host, "无法访问主机，建议启动服务选择合适的备用服务！");
                            reLoadWebBrowser();
                            return;
                        }
                        else
                        {
                            reLoadWebBrowser();
                            LogableClass.ToLog(webBrowser1.Url.Host, "登录后无法访问主机，建议更换主机重新登录！");
                            return;
                        }
                    }
                    else
                    {
                        //其他网页
                        LogableClass.ToLog(webBrowser1.Url.Host, "睡眠综合症！");
                        return;
                    }
                }
                
            }
            else
            {
                if (!WebBrowserLoad)//中间状态，不理睬
                {
                    return;
                }
                
                //////加载自定义脚本后的处理
                ////while(true)
                ////{
                ////    if(wr.IsLogined(doc))
                ////        break;
                ////    Thread.Sleep(60 * 1000);
                ////}
                ////Logined = true;
                ////this.timer_RequestInst_Tick(null, null);
            }
            //if (Logined)
            //{
            //    //网页中间刷新
                

            //}
            ////}
        }

        string getScriptText(string scriptName)
        {
            string ret = "";
            string FilePath = string.Format("{0}\\{1}", Application.StartupPath, scriptName);
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txt_ExpectNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        Dictionary<string,string> getAssetLists()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            CommunicateToServer wc = new CommunicateToServer();
            CommResult cr = wc.getRequestAssetList(GlobalClass.strAssetInfoURL);
            if (!cr.Succ)
            {
                this.statusStrip1.Items[0].Text = cr.Message;
                return ret;
            }
            mAssetUnitList ic = cr.Result[0] as mAssetUnitList;
            for(int i=0;i<ic.Count;i++)
            {
                ret.Add(ic.List[i].AssetId, ic.List[i].AssetName);
            }
            return ret;
        }

        private void timer_RequestInst_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                Random rd = new Random();
                int rndtime = rd.Next(1000, 60 * 1000);//增加随机数，防止单机多客户端并行同时下注
                if (InSleep)//还在睡觉,唤醒
                {
                    LoadUrl();
                    ////while(!Logined)
                    ////{
                    ////    System.Threading.Thread.Sleep(30*1000);
                    ////}
                    InSleep = false;
                    this.timer_RequestInst.Interval = 30 * 1000 + rndtime;//等待唤醒以后再访问
                    return;
                }
                DateTime CurrTime = DateTime.Now;
                CommunicateToServer wc = new CommunicateToServer();
                CommResult cr = wc.getRequestInsts(GlobalClass.strRequestInstsURL);
                if (!cr.Succ)
                {
                    this.statusStrip1.Items[0].Text = cr.Message;
                    return;
                }
                if (cr.Cnt != 1)
                {
                    this.statusStrip1.Items[0].Text = "无指令！";
                    return;
                }
                RequestClass ic = cr.Result[0] as RequestClass;
                if (ic == null)
                {
                    this.statusStrip1.Items[0].Text = "指令内容错误！";
                    Program.wxl.Log(this.statusStrip1.Items[0].Text);
                    return;
                }
                Int64 CurrExpectNo = Int64.Parse(ic.Expect);
                this.statusStrip1.Items[1].Text = DateTime.Now.ToLongTimeString();
                if (CurrExpectNo > this.NewExpect || RefreshTimes == 0) //获取到最新指令
                {
                    LastInstsTime = DateTime.Now;
                    int CurrMin = DateTime.Now.Minute % 5;

                    ////if (CurrMin % 5 < 2)
                    ////{
                    ////    this.timer_RequestInst.Interval = (2-CurrMin)*60000;//加分钟以后见
                    ////}
                    ////else
                    ////{
                    ////    this.timer_RequestInst.Interval = (5-CurrMin)*6000;//5分钟以后见
                    ////}
                    this.timer_RequestInst.Interval = 2 * 60 * 1000 + rndtime;//5分钟以后见,减掉1秒不*断收敛时间，防止延迟接收
                                                                              //ToAdd:填充各内容
                    this.txt_ExpectNo.Text = ic.Expect;
                    this.txt_OpenTime.Text = ic.LastTime;
                    this.txt_Insts.Text = ic.getUserInsts(Program.gc);
                    this.NewExpect = CurrExpectNo;
                    if (!Logined)
                    {
                        Logined = wr.IsLogined(this.webBrowser1.Document);
                    }
                    if (Logined)
                    {
                        if (RefreshTimes > 0)
                            this.btn_Send_Click(null, null);
                        RefreshTimes = 1;
                    }
                    else//如果没有登录，重新载入,当前指令不发送，只要不发送，这条指令就不会存入缓存。可以下次获取到再发
                    {
                        LoadUrl();

                    }

                }
                else
                {
                    if (CurrTime.Hour < 9)//如果在9点前
                    {
                        //下一个时间点是9：08
                        DateTime TargetTime = DateTime.Today.AddHours(9).AddMinutes(30);
                        this.timer_RequestInst.Interval = TargetTime.Subtract(CurrTime).TotalMilliseconds + rndtime;
                        KnockEgg();//敲蛋
                        Application.DoEvents();
                        Thread.Sleep(5000);//暂停，等发送消息
                        reLoadWebBrowser();//开百度网页，睡觉
                    }
                    else
                    {

                        if (CurrTime.Subtract(LastInstsTime).Minutes > 7)//如果离上期时间超过7分钟，说明数据未接收到，那不要再频繁以10秒访问服务器
                        {
                            this.timer_RequestInst.Interval = 2 * 60 * 1000 + rndtime;
                        }
                        else //一般未接收到，2*60秒以后再试
                        {
                            this.timer_RequestInst.Interval = 2 * 60 * 1000 + rndtime;
                        }
                    }
                }
                RefreshStatus();
            }
            catch(Exception ce)
            {
                LogableClass.ToLog("错误", "刷新指令",string.Format("{0}:{1}",ce.Message,ce.StackTrace));
                Program.wxl.Log("错误", "刷新指令", string.Format("{0}:{1}", ce.Message, ce.StackTrace));

            }
        }

        string ReCalcTheInstChips()
        {
            string ret = null;
            return ret;
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            if (this.txt_Insts.Text.Trim().Length == 0) return;
            if(dicExistInst.ContainsKey(this.NewExpect))
            {
                return;
            }
            dicExistInst.Add(this.NewExpect, this.txt_Insts.Text);
            Rule_ForKcaiCom rule = new Rule_ForKcaiCom(Program.gc);
            AddScript();
            string msg = rule.IntsToJsonString(this.txt_Insts.Text, Program.gc.ChipUnit);
            double lastval = this.CurrVal;
            //SendMsg
            webBrowser1.Document.InvokeScript("SendMsg", new string[] { this.NewExpect.ToString(), msg });
            Application.DoEvents();
            Thread.Sleep(5 * 1000);
            webBrowser1.Refresh();
            Application.DoEvents();
            Thread.Sleep(3 * 1000);
            string ValTip = "无";
            if(CurrVal<=lastval)
            {
                ValTip = "下注后金额无变化，您的下注可能在20秒内没被执行！请注意查看！";
            }
            Program.wxl.Log(string.Format("[{0}]:{1};下注前金额:{2:f2};现可用余额:{3:f2}；提示:{4}",this.NewExpect, this.txt_Insts.Text, lastval,this.CurrVal,ValTip));
            ////if (e != null)
            ////{
            ////    MessageBox.Show(msg);
            ////}
            
            if(sender != null)
            {
                Thread.Sleep(2 * 1000);
                MessageBox.Show(string.Format("下注前金额:{0:f2};现可用余额:{1:f2}；提示:{2}",lastval, this.CurrVal,ValTip));
            }
        }

        private void mnu_SetAssetUnitCnt_Click(object sender, EventArgs e)
        {
            frm_setting frm = new frm_setting(AssetUnitList);
            frm.ShowDialog();
        }

        private void mnu_RefreshInsts_Click(object sender, EventArgs e)
        {
            timer_RequestInst_Tick(null, null);
        }


        private void mnuRefreshWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reLoadWebBrowser();
        }

        void reLoadWebBrowser()
        {
            ReadySleep = true;
            string url = "https://www.baidu.com";
            this.webBrowser1.Navigate(url);
            WebBrowserLoad = false;
            Logined = false;
            InSleep = true;
            Set_SendTime(false);
       }

        private void reLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadUrl();
        }

        void RefreshStatus()
        {
            this.toolStripStatusLabel1.Text = string.Format("已加载页面:{0};已登录:{1};睡眠状态:{2}",WebBrowserLoad,Logined,InSleep);
            this.toolStripStatusLabel2.Text = string.Format("当前余额：{0}",CurrVal);
            this.statusStrip1.Items[2].Text = string.Format("{0}秒后见", this.timer_RequestInst.Interval / 1000);
            SendStatusTimer_Elapsed(null, null);
        }

        void RefreshSendStatusInfo(string msg)
        {
            toolStripStatusLabel3.Text = msg;
            toolStripStatusLabel3.Enabled = true;
            toolStripStatusLabel3.Visible = true;
        }

        private void TSMI_sendStatusInfor_Click(object sender, EventArgs e)
        {
            this.SendStatusTimer_Elapsed(null, null);
        }

        private void btn_TestJscript_Click(object sender, EventArgs e)
        {
            string[] arg = this.txt_NewInsts.Lines;
            
            //SendMsg
            string[] useArg = new string[arg.Length - 1];
            Array.Copy(arg, 1, useArg, 0, useArg.Length);
            try
            {
                AddScript();
                webBrowser1.Document.InvokeScript("JumpFillPage", null);
                Thread.Sleep(3000);
                AddScript();
                webBrowser1.Document.InvokeScript(arg[0], useArg);
            }
            catch(Exception ce)
            {
                MessageBox.Show(e.ToString());
            }
        }

        void KnockEgg()
        {
            AddScript();
            webBrowser1.Document.InvokeScript("ClickEgg",new object[] { Program.VerNo });
        }

        private void tsmi_knockTheEgg_Click(object sender, EventArgs e)
        {
            KnockEgg();
        }
    }
}

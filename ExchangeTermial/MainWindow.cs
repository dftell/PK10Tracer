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
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using WolfInv.com.RemoteObjectsLib;
//using WolfInv.Com.SocketLib;
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.ChargeLib;
using System.Runtime.InteropServices;
using Gecko;
using System.Security.Permissions;
using WolfInv.com.PK10CorePress;
using System.Windows.Forms.DataVisualization.Charting;

namespace ExchangeTermial
{
    public enum ExpectDataStatus
    {
        Created,CalcAmt,Push,SentOccurErr,Sent,NoValidate
    }
    delegate void CloseCallBack();
    public enum ShowCommands : int
    {

        SW_HIDE = 0,

        SW_SHOWNORMAL = 1,

        SW_NORMAL = 1,

        SW_SHOWMINIMIZED = 2,

        SW_SHOWMAXIMIZED = 3,

        SW_MAXIMIZE = 3,

        SW_SHOWNOACTIVATE = 4,

        SW_SHOW = 5,

        SW_MINIMIZE = 6,

        SW_SHOWMINNOACTIVE = 7,

        SW_SHOWNA = 8,

        SW_RESTORE = 9,

        SW_SHOWDEFAULT = 10,

        SW_FORCEMINIMIZE = 11,

        SW_MAX = 11

    }
    delegate void SetDataGridCallback<T>(string id, DataTable dt,T otherObj, string sort = null);
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class MainWindow : Form
    {
        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]

        //[DllImport("shell32.dll")]
        //static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, ShowCommands nShowCmd);


        
        internal static extern IntPtr GetCurrentProcess();
        UserBaseInfo currUser;
        Dictionary<string, DataStatusInfoClass> AllDataInfoDic = new Dictionary<string, DataStatusInfoClass>();
        bool isIE = false;
        Dictionary<string, Int64> NewExpects = new Dictionary<string, long>();
        bool WebBrowserLoad = false;
        Dictionary<int, RequestClass> CurrDic = new Dictionary<int, RequestClass>();
        DateTime LastInstsTime = DateTime.MaxValue;
        Dictionary<string, string> AssetUnitList;
        long RefreshTimes = 0;

        Dictionary<string,Dictionary<long, string>> dicExistInsts;
        bool InSleep = false;
        bool Logined = false;
        WebRule wr = null;
        
        bool ReadySleep = false;
        bool ScriptLoaded = false;
        int UsePort = -1;
        string UseIp = null;
        bool SocketRunning = false;
        public bool ForceReboot { get; set; }
        int ChanleUseTimes = -1;
        string const_ResponseModel = "reqId={0}&chargeAmt={1}&errcode={2}&msg={3}&imgData={4}&orderNum={5}&chargeAccount={6}";
        WebData chargeData;
        Dictionary<string, int> AllReq = new Dictionary<string, int>();
        string UseReqId = null;
        int UseAmt = 0;
        string reqChargeAmt = "";
        double _currVal;
        bool AppMode = true;
        double InitCash = 0;
        bool IsAdmin = false;
        double CurrVal
        {
            get
            {
                if(SendMsgFromWebRequest)
                {
                    //getAmount();
                    return _currVal;
                }
                HtmlDocument doc = null;
                try
                {
                    doc = webBrowser1.Document;
                }
                catch
                {

                }
                return isIE ? wr.GetCurrMoney(webBrowser1.Document) : wr.GetCurrMoney(geckoWebBrowser1.Document);
            }
            set
            {
                _currVal = value;
            }
        }
        System.Windows.Forms.Timer SendStatusTimer = new System.Windows.Forms.Timer() ;
        Dictionary<string,MyTimer> timers_RequestInst = new Dictionary<string, MyTimer>();
        //TcpServiceSocketAsync svr;
        int const_maxbuffsize = 1024 * 1024 * 2;
        ChargeRemoteClass chgRmt = null;
        ChargeOperator chgOpt = null;
        ClientExchanger ce;
        bool SendMsgFromWebRequest;
        bool NeedLoadGameInfo;
        bool LessThanWarnLine;
        public MainWindow()
        {
            InitializeComponent();
            ce = new ClientExchanger(webBrowser1,Program.gc);
            SendMsgFromWebRequest = (Program.gc.SendMsgFromWebRequest == "1");
            AppMode = Program.gc.AppMode==1 ? true : false;
            InitCash = Program.gc.RiskTimes * 400;
            IsAdmin = Program.gc.IsAdmin == 1;
        }

        public void setUser(UserBaseInfo bi)
        {
            currUser = bi;
        }
        void reLoad()
        {
            if (SendMsgFromWebRequest)
            {
                //ce.Load(null);
                //return;
                //ce.Login(Program.gc.ClientUserName, Program.gc.ClientPassword, null);
            }
            wr = WebRuleBuilder.Create(Program.gc.InstFormat,Program.gc, Program.gc);
            InitWebBrowser();
            initMenuItems();            
            
            this.Text = string.Format("{0}[{2}][v:{1}]", Program.gc.ForWeb, Program.VerNo, Program.gc.ClientUserName);
            Program.Title = this.Text;
            //webBrowser1.Url = new Uri("http://www.baidu.com");
            //svr = new TcpServiceSocketAsync(5,const_maxbuffsize);
            //svr.recvMessageEvent += Request;
            //UsePort = TcpServiceSocketAsync.getRandomPort(8080);
            chgRmt = new ChargeRemoteClass();            
            chgOpt = new ChargeOperator();
            chgOpt.OperateChargeForm = new Action<string, string,string,string>(Request);
            chgOpt.SvrNam = string.Format("{0}充值机", Program.gc.ClientUserName);
            chgRmt.Operate = chgOpt;
            NeedLoadGameInfo = false;


        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            LoadDDL();
            reLoad();
            //return;
            AssetUnitList = new Dictionary<string, string>();
            foreach (string key in GlobalClass.TypeDataPoints.Keys)
            {
                Dictionary<string, string> currDic = getAssetLists(key);
                foreach (string aid in currDic.Keys)
                {
                    if (!AssetUnitList.ContainsKey(aid))
                    {
                        AssetUnitList.Add(aid, currDic[aid]);
                    }
                }
            }
            dicExistInsts = new Dictionary<string, Dictionary<long, string>>();
            InSleep = true;

            //
            //LoadUrl(LoginPage);
            //Application.DoEvents();
            //MySleep(20 * 1000);//睡20秒等待webbrowser加载
            Application.DoEvents();
            //this.timer_RequestInst.Enabled = true;
            //this.timer_RequestInst.AutoReset = true;
            //timer_RequestInst_Tick(null, null);
            
            //Set_RequestTime(true, defaultInterVal());
            Set_SendTime(true, 5*60*1000);
            SendStatusTimer_Elapsed(null, null);
            InitTimers();
            
        }

        void LoadDDL()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("key");
            dt.Columns.Add("val");
            foreach (string key in GlobalClass.TypeDataPoints.Keys)
            {
                //DataRow dr = dt.NewRow();
                dt.Rows.Add(new string[] { key, key });
            }
            ddl_datatype.Items.Clear();
            ddl_datatype.DisplayMember = "val";
            ddl_datatype.ValueMember = "key";
            ddl_datatype.DataSource = dt;
            ddl_datatype.Refresh();
        }

        void RefreshData()
        {

        }
        void init_FireFox()
        {
            
            // 
            // geckoWebBrowser1
            // 
            if (!isIE)
            {
                this.geckoWebBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
                this.geckoWebBrowser1.FrameEventsPropagateToMainWindow = false;
                this.geckoWebBrowser1.Location = new System.Drawing.Point(0, 0);
                this.geckoWebBrowser1.Name = "geckoWebBrowser1";
                this.geckoWebBrowser1.Size = new System.Drawing.Size(1289, 258);
                this.geckoWebBrowser1.TabIndex = 5;
                this.geckoWebBrowser1.UseHttpActivityObserver = false;
            }
            if (!isIE)
                this.splitContainer1.Panel2.Controls.Add(this.geckoWebBrowser1);
        }
        void InitWebBrowser()
        {
            isIE = (Program.gc.UseBrowser.Trim().ToLower().Length == 0 || Program.gc.UseBrowser.Trim().ToLower().Equals("ie"));
            if (!isIE)
            {
                
                Xpcom.Initialize("Firefox");
                init_FireFox();
            }
            if (isIE)
            {
                
                this.webBrowser1.Visible = true;
                
                initWebRule();
                this.webBrowser1.Dock = DockStyle.Fill;
                //this.geckoWebBrowser1.Visible = false;
                //this.geckoWebBrowser1.Dock = DockStyle.None;
                this.webBrowser1.DocumentCompleted += DocumentCompleted_IE;
                this.webBrowser1.Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
                this.webBrowser1.ScriptErrorsSuppressed = true;
                //Gecko.CertOverrideService.GetService().ValidityOverride += geckoWebBrowser1_ValidityOverride;
                try
                {
                    //ShellExecute(IntPtr.Zero, "open", "rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 8","",ShowCommands.SW_HIDE);

                }
                catch(Exception ce)
                {

                }
                /*
             Temporary Internet Files  （Internet临时文件）
ClearMyTracksByProcess 8
Cookies
ClearMyTracksByProcess 2
History (历史记录)
ClearMyTracksByProcess 1
 Form. Data （表单数据）
ClearMyTracksByProcess 16
 Passwords (密码）
ClearMyTracksByProcess 32
 Delete All  （全部删除）
ClearMyTracksByProcess 255    
             */
            }
            else
            {
                this.webBrowser1.Visible = false;
                this.webBrowser1.Dock = DockStyle.None;
                this.geckoWebBrowser1.Visible = true;
                this.geckoWebBrowser1.Dock = DockStyle.Fill;
                //this.geckoWebBrowser1.DocumentCompleted += DocumentCompleted_FireFox;
                this.geckoWebBrowser1.DocumentCompleted += new EventHandler<Gecko.Events.GeckoDocumentCompletedEventArgs>(DocumentCompleted_FireFox);
                this.geckoWebBrowser1.DOMContentLoaded += GeckoWebBrowser1_DOMContentLoaded;
                geckoWebBrowser1.CreateControl();
            }
        }

        void initWebRule()
        {
            this.webBrowser1.ObjectForScripting = wr;
            wr.SuccLogin = AfterLogined;
            wr.SuccSend = AfterSendMsg;
            wr.CompleteSendMsg = AfterSendMsgComplete;
            wr.SuccGetGameInfo = AfterGetGameInfo;
            wr.SuccGetAmount = AfterGetAmount;
            wr.SuccGetBetRec = AfterGetBetRecordInfo;
            wr.AJaxError = AfterAJaxError;
            wr.SuccCancelBet = AfterCancelBet;
            wr.MsgBox = WXMsgBox;
        }

        void AfterGetGameInfo(GameInfoClass gic)
        {

        }

        void AfterCancelBet(WebServerReturnClass ret)
        {
            if(ret.Succ)
            {
                //refreshBetRecToolStripMenuItem_Click(null, null);
                //getBetRecInfo();
                ////new Task(() =>
                ////{
                ////    getBetRecInfo();
                ////}).Start();
                //this.cancelBetRecToolStripMenuItem_Click(null, null);
            }
            else
            {
                WXMsgBox("撤单错误", ret.Msg);
            }
        }

        void AfterAJaxError(WebServerReturnClass err)
        {
            WXMsgBox(err.Title??"Error", err.Msg);
            //Logined = false;
        }

        void AfterGetBetRecordInfo(BetRecordListClass brec)
        {
            if(brec.Succ)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("序号");
                dt.Columns.Add("关键字");
                dt.Columns.Add("彩种");
                dt.Columns.Add("规则");
                dt.Columns.Add("内容");
                dt.Columns.Add("状态");
                dt.Columns.Add("成本");
                dt.Columns.Add("返奖");
                dt.Columns.Add("盈利");
                dt.Columns.Add("时间");
                brec.Data.ForEach(
                    a =>
                    {
                    dt.Rows.Add(new object[]{
                            a.ID,
                            a.Key,
                            a.GameName,
                            a.BetType,
                            a.Nums,
                            
                            a.Status,
                            a.Cost,
                            a.ReturnAmount,
                            a.EarnedAmount,
                            a.CreateTime
                    });
                    }
                    );
                new Task(()=> {
                    try
                    {
                        //delegate void SetSpecDataGridCallback(DataTable dt);
                        SetDataGridDataTable(dgv_betRecs, dt ,brec, "时间 desc");
                        //this.dgv_betRecs.DataSource = dt;
                        //this.dgv_betRecs.Refresh();
                    }
                    catch
                    {

                    }
                }).Start();
            }
        }

        void SetDataGridDataTable<T>(DataGridView dg, DataTable dt, T otherObj, string sort = null)
        {

            dg.Invoke(new SetDataGridCallback<T>(SetDgTableById), new object[] { dg.Name, dt,otherObj, sort });
        }
        void SetDgTableById<T>(string id, DataTable dt,T otherObj, string sort)
        {
            Control[] ctrls = this.Controls.Find(id, true);
            if (ctrls.Length != 1)
            {
                return;
            }
            if (sort != null)
            {
                DataView dv = new DataView(dt);
                dv.Sort = sort;
                (ctrls[0] as DataGridView).DataSource = dv;
            }
            else
            {
                (ctrls[0] as DataGridView).DataSource = dt;
            }
            if (otherObj != null)
            {
                ObjectTable<T> brt = new ObjectTable<T>();
                brt.dt = dt;
                brt.otherObject = otherObj;
                (ctrls[0] as DataGridView).Tag = brt;
            }
            else
            {
                (ctrls[0] as DataGridView).Tag = dt;
            }
        }


        void AfterGetAmount(AmountInfoClass amt)
        {
            if (amt.Succ)
            {
                this.CurrVal = amt.CurrMoney;
                if (this.CurrVal < 0.25 * InitCash)
                {
                    if (!LessThanWarnLine)//只预告一次
                    {
                        WXMsgBox(string.Format("当前账户[{0}]余额预警!", Program.gc.ClientAliasName), string.Format("当前账户余额为{0}小于初始金额的25%，请考虑补充资金.", this.CurrVal));
                    }
                    LessThanWarnLine = true;
                }
                else
                {
                    LessThanWarnLine = false;
                }
                if (this.CurrVal > 2 * InitCash)
                {
                    
                        WXMsgBox(string.Format("当前账户[{0}]余额提示!", Program.gc.ClientAliasName), string.Format("当前账户余额为{0}大于初始金额的2倍，请考虑提现！", this.CurrVal));
              
                    
                }
                this.RefreshStatus();
            }
            else
            {
                Logined = false;
                InSleep = true;
                WebBrowserLoad = false;
                reLoadWebBrowser();
                WXMsgBox("获取到金额错误", "重置所有状态！需重新登陆");
            }
        }

        void AfterSendMsgComplete(string msg,string msg1)
        {

        }
        

        void AfterSendMsg(WebBetReturnInfoClass wbri)
        {
            long currExpect = 0;
            long.TryParse(wbri.SerialNo, out currExpect);
            //MessageBox.Show(wbri.betRecInfo);
            DataTypePoint dtp = null;
            if (this.timers_RequestInst.ContainsKey(wbri.dtp))
            {
                
                if (GlobalClass.TypeDataPoints.ContainsKey(wbri.dtp))
                {
                    dtp = GlobalClass.TypeDataPoints[wbri.dtp];
                }
                
            }
            if (wbri.Succ)
            {
                Logined = true;
                InSleep = false;
                WebBrowserLoad = true;
                CurrVal = wbri.restAmount;
                NewExpects[wbri.dtp] = currExpect;
                if (!dicExistInsts[wbri.dtp].ContainsKey(currExpect))
                {
                    dicExistInsts[wbri.dtp].Add(currExpect, wbri.SendData);
                }
                this.timers_RequestInst[wbri.dtp].Interval = defaultInterVal(dtp, true);
                bool needNotice = false;
                CurrVal = wbri.restAmount;
                if (IsAdmin)
                {
                    needNotice = true;
                }
                else
                {
                    if(CurrVal<0.25*InitCash)
                    {
                        needNotice = true;
                    }
                }
                if (needNotice)
                {
                    new Task(() =>
                    {
                        WXMsgBox(string.Format("[{3}]第{2}期投入:{0}元，当前余额:{1}元", wbri.betAmt, wbri.restAmount, currExpect, wbri.dtp), wbri.SendData);
                    }).Start();
                }
                double currBetAmt = 0;
                double.TryParse(wbri.betAmt,out currBetAmt);
                if(currBetAmt > 0.05*InitCash)
                {
                    WXMsgBox(string.Format("[{3}]第{2}期投入:{0}元,金额较大，超过初始金额的5%，当前余额:{1}元，请关注资金变动，如余额较小，请考虑充值，保证账户余额足够！", wbri.betAmt, wbri.restAmount, currExpect, wbri.dtp), wbri.SendData);
                }
                this.toolStripStatusLabel3.Text = wbri.SendData;
                ////new Task(() =>
                ////{
                ////    //this.cancelBetRecToolStripMenuItem_Click(null, null);
                ////    getBetRecInfo();
                ////}).Start();
            }
            else
            {
                this.timers_RequestInst[wbri.dtp].Interval = defaultInterVal(dtp);
                WXMsgBox(string.Format("[{1}]第{2}期发送[{0}]失败",wbri.SendData,wbri.dtp, currExpect), wbri.Msg);
            }
            
        }

        void AfterLogined(WebUserInfoClass wic)
        {
            if(!wic.Succ)
            {
                WXMsgBox(string.Format("账号{0}登录{1}失败",Program.gc.ClientUserName,Program.gc.ForWeb), wic.Msg);
                return;
            }
            //WXMsgBox(Program.gc.ClientAliasName, string.Format("登陆{0}成功！", Program.gc.ForWeb));
            setLogined();
            try
            {
                lock (webBrowser1)
                {
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.Navigate(string.Format(Program.gc.LotteryPage, string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost)));
                }
                Application.DoEvents();
                MySleep(3 * 1000);
                getAmount();
                //getGameInfo(70);
                Application.DoEvents();
                MySleep(2 * 1000);
                WXMsgBox(string.Format("账号{0}登录{1}成功", Program.gc.ClientUserName, Program.gc.ForWeb), string.Format("当前余额:{0}元,账号模式为{1},风险规模为{2}倍。", CurrVal,AppMode?"App模式":"自由投资模式",Program.gc.RiskTimes));
            }
            catch
            {

            }
        }

        void setLogined()
        {
            Logined = true;
            InSleep = false;
            WebBrowserLoad = true;
        }

        void getGameInfo(int n)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            //LogableClass.ToLog(webBrowser1.Url.Host, "首次进入，加载文件！");
            //if(!ScriptLoaded)
            AddScript(loginDocIE ?? HomeDocIE, null); //执行js代码,未来修改为网络载入
            //MySleep(3 * 1000);//等待验证码图片载入
            (loginDocIE ?? HomeDocIE).InvokeScript("getGameInfo", new string[] { string.Format(Program.gc.LoginUrlModel,Program.gc.LoginDefaultHost), n.ToString()});
            WebBrowserLoad = true;
        }

        void cancelBetRec(params object[] args)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            //LogableClass.ToLog(webBrowser1.Url.Host, "首次进入，加载文件！");
            //if(!ScriptLoaded)
            AddScript(loginDocIE ?? HomeDocIE, null); //执行js代码,未来修改为网络载入
            //MySleep(3 * 1000);//等待验证码图片载入
            List<string> inlist = new List<string>();
            inlist.Add(string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost));
            args.ToList().ForEach(a => inlist.Add(a?.ToString()));
            object[] strargs = inlist.ToArray();
            (loginDocIE ?? HomeDocIE).InvokeScript("CancelBet", strargs);
            //WebBrowserLoad = true;
        }

        void getBetRecInfo()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            //LogableClass.ToLog(webBrowser1.Url.Host, "首次进入，加载文件！");
            //if(!ScriptLoaded)
            AddScript(loginDocIE ?? HomeDocIE, null); //执行js代码,未来修改为网络载入
            //MySleep(3 * 1000);//等待验证码图片载入
            (loginDocIE ?? HomeDocIE).InvokeScript("getBetRecordList", new string[] { string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost) });
            WebBrowserLoad = true;
        }

        void getAmount()
        {
            
            webBrowser1.ScriptErrorsSuppressed = true;
            //LogableClass.ToLog(webBrowser1.Url.Host, "首次进入，加载文件！");
            //if(!ScriptLoaded)
            AddScript(loginDocIE ?? HomeDocIE, null); //执行js代码,未来修改为网络载入
            //MySleep(3 * 1000);//等待验证码图片载入
            (loginDocIE ?? HomeDocIE).InvokeScript("getGameAmount", new string[] { string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost) });
            WebBrowserLoad = true;
        }
        private void GeckoWebBrowser1_DOMContentLoaded(object sender, DomEventArgs e)
        {
            DocumentCompleted_FireFox(sender,null);
        }

        string Host
        {
            get
            {
                return string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost);
            }
        }

        string LoginPage
        {
            get
            {
                return string.Format(Program.gc.LoginPage, Host);
            }
        }

        
         string LotteryPage
        {
            get
            {
                return string.Format(Program.gc.LotteryPage, Host);
            }
        }

        
        void initMenuItems()
        {
            switchPlatformToolStripMenuItem.DropDownItems.Clear();
            foreach(var obj in GlobalClass.WebSites)
            {
                if (Program.gc.ForWeb.Equals(obj.Key))
                    continue;
                ToolStripMenuItem mi = new ToolStripMenuItem();
                mi.Text = obj.Value;
                mi.Tag = obj.Key;
                mi.Click += switchPlatformToolStripMenuItem_Click;
                switchPlatformToolStripMenuItem.DropDownItems.Add(mi);
            }    

            
        }
        void LoadUrl(string url)
        {
            if(string.IsNullOrEmpty(url))
                url = string.Format(Program.gc.LoginPage, string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost));
            
            if (!isIE)
            {
                LoadUrl_FireFox(url);
                loadUrl = geckoWebBrowser1.Url.ToString();
            }
            else
            {
                LoadUrl_IE(url);
                loadUrl = webBrowser1.Url.ToString();
            }
            //loadUrl = url;
        }

        void LoadUrl_IE(string url)
        {
            //if (loading)
            //{
            //    this.toolStripStatusLabel1.Text = "正在载入中！";
            //    return;
            //}
            Logined = false;
            WebBrowserLoad = false;
            //string url = Program.gc.LoginUrlModel.Replace("{host}", Program.gc.LoginDefaultHost);
            //string url = string.Format(Program.gc.LoginPage, string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost));
            //loadUrl = url;
            try
            {
                lock (webBrowser1)
                {
                    if (this.webBrowser1.Url != null && (this.webBrowser1.Url.ToString() == url || this.webBrowser1.Url.ToString() == string.Format(Program.gc.LotteryPage, string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost))))
                        return;
                    if (wr.IsLogined(webBrowser1.Document)||isInLotteryDocument())//如果已经登录了，不在加载
                    {
                        Logined = true;
                        WebBrowserLoad = true;
                        return;
                    }
                    //this.webBrowser1 = null;
                    //this.webBrowser1 = new WebBrowser();
                    //this.webBrowser1.DocumentCompleted += DocumentCompleted_IE;
                    //this.webBrowser1.Url = new Uri(url);
                    //if(this.webBrowser1.Url == null)
                    //{
                    //    this.webBrowser1.Url = new Uri(url);
                    //}
                    //else
                    this.webBrowser1.Navigate(url, false);
                    HomeDocIE = this.webBrowser1.Document;
                    this.webBrowser1.ScriptErrorsSuppressed = true;
                    Application.DoEvents();
                    MySleep(3 * 1000);
                    ScriptLoaded = false;
                    LogableClass.ToLog(webBrowser1?.Url?.Host, "准备唤醒！");
                    WXMsgBox(string.Format("[{0}]起来", Program.gc.ClientAliasName), "不愿做奴隶的程序们！");
                    if (ChanleUseTimes < 0)//载入时线路访问次数置0
                    {
                        ChanleUseTimes = 0;
                    }
                }
                ////else
                ////{
                ////    Program.wxl.Log("起不来", "睡眠不足！");
                ////    return;
                ////}

            }
            catch (Exception ce)
            {
                InSleep = true;
                Logined = false;
                LogableClass.ToLog("错误", ce.Message, ce.StackTrace);
                WXMsgBox("错误", string.Format("[{0}]起床困难户。", Program.gc.ClientAliasName));
                timers_RequestInst.Values.ToList().ForEach(a => { a.Interval = defaultInterVal(); });
                return;
            }
            Application.DoEvents();
            //Thread.Sleep(10 * 1000);

            //timers_RequestInst.ForEach(a => { a.Interval = 60 * 1000; });
            //this.timer_RequestInst.Enabled = true;
            //Set_SendTime(true);
        }

        int defaultInterVal(DataTypePoint dtp = null,bool receivedData = false)
        {
            if(dtp == null)
            {
                return getInterVal(GlobalClass.TypeDataPoints.First().Value, receivedData);
            }
            return getInterVal(dtp, receivedData);

        }

        void LoadUrl_FireFox(string url)
        {

            Logined = false;
            WebBrowserLoad = false;
            //string url = Program.gc.LoginUrlModel.Replace("{host}", Program.gc.LoginDefaultHost);
            
            try
            {
                lock (geckoWebBrowser1)
                {

                    //this.webBrowser1 = null;
                    //this.webBrowser1 = new WebBrowser();
                    //geckoWebBrowser1.DocumentCompleted += DocumentCompleted_FireFox;// .DocumentCompleted += webBrowser1_DocumentCompleted;

                    //this.webBrowser1.Url = new Uri(url);
                    //if(this.webBrowser1.Url == null)
                    //{
                    //    this.webBrowser1.Url = new Uri(url);
                    //}
                    //else
                    if (this.geckoWebBrowser1.Url != null && this.geckoWebBrowser1.Url.ToString() == url)
                        return;
                    this.geckoWebBrowser1.Navigate(url);
                    HomeDocFireFox = this.geckoWebBrowser1.Document;
                    // this.geckoWebBrowser1.scr = true;
                    Application.DoEvents();
                    MySleep(3 * 1000);
                    ScriptLoaded = false;
                    LogableClass.ToLog(geckoWebBrowser1?.Url?.Host, "准备唤醒！");
                    WXMsgBox(string.Format("[{0}]起来", Program.gc.ClientAliasName), "不愿做奴隶的程序们！");
                    if (ChanleUseTimes < 0)//载入时线路访问次数置0
                    {
                        ChanleUseTimes = 0;
                    }
                }
                ////else
                ////{
                ////    Program.wxl.Log("起不来", "睡眠不足！");
                ////    return;
                ////}

            }
            catch (Exception ce)
            {
                InSleep = true;
                Logined = false;
                LogableClass.ToLog("错误", ce.Message, ce.StackTrace);
                Program.wxl.Log("错误", string.Format("[{0}]起床困难户。", Program.gc.ClientAliasName), string.Format("{0}:{1}", ce.Message, ce.StackTrace), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                timers_RequestInst.Values.ToList().ForEach(a => { a.Interval = defaultInterVal(); });
                return;
            }
            Application.DoEvents();
            //Thread.Sleep(10 * 1000);

            timers_RequestInst.Values.ToList().ForEach(a => { a.Interval = defaultInterVal(); });
            //this.timer_RequestInst.Enabled = true;
            //Set_SendTime(true);
        }


        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            Program.wxl.Log(string.Format("{0}出现错误{1}",Program.gc.ClientAliasName,e.Description), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
            e.Handled = true;
        }

        
        void Request(string wxId,string wxName,string reqId,string chargeAmt)
        {
            //chargeMoneyToolStripMenuItem_Click(null, null);
            string url = string.Format(Program.gc.LoginPage, string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost));
            var rnd = new Random().Next();
            string fullurl = string.Format("{0}/Recharge/ThirdRecharge?amount={1}&t={2}&rnd={3}", url, chargeAmt, 139,rnd);
            Program.wxl.Log(string.Format("[收到充值请求]微信编号:{0};微信昵称:{1};请求Id:{2};请求金额:{3}！", wxId,wxName,reqId,chargeAmt ), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
            chgOpt.strAmt = chargeAmt.Trim();
            reqChargeAmt = chargeAmt.Trim();
            chgOpt.responsed = false;
            chgOpt.reqId = Guid.NewGuid().ToString();
            UseReqId = chgOpt.reqId;
            chgOpt.ResponseString = null;
            
            chargeData = null;
            Task.Run(() => {
                checkCharge();
            });
            webBrowser_charge.Navigate(fullurl);
        }



        
        Dictionary<string, DataTypePoint> dtps = new Dictionary<string, DataTypePoint>();
        void InitTimers()
        {
            dtps = GlobalClass.TypeDataPoints;
            //NewExpects = new Dictionary<string, long>();
            //dicExistInsts = new Dictionary<string, Dictionary<long, string>>();
            int ti = 0;
            foreach (string key in dtps.Keys)
            {
                MyTimer tm = new MyTimer();
                tm.dtp = dtps[key];
                tm.Tag = dtps[key];
                
                tm.Tick += timer_RequestInst_Tick;
                tm.Enabled = true;
                tm.Interval = defaultInterVal(dtps[key]);// (1 + new Random().Next(5)) * 1000;
                tm.index = ti;
                
                timers_RequestInst.Add(key,tm);
                if (!NewExpects.ContainsKey(key))
                {
                    NewExpects.Add(key, 0);
                    dicExistInsts.Add(key, new Dictionary<long, string>());
                }
                if (!dicExistInsts.ContainsKey(key))
                {
                    dicExistInsts.Add(key, new Dictionary<long, string>());
                }
                tm.Start();
                //timer_RequestInst_Tick(tm, null);
                ti++;
            }
        }

        void Set_SendTime(bool Running, int InterVal = 20 * 60 * 1000)
        {
            SendStatusTimer.Interval = InterVal;
            SendStatusTimer.Tick += SendStatusTimer_Elapsed;
            SendStatusTimer.Enabled = Running;
            
            ////if(Running)
            ////{
            ////    SendStatusTimer_Elapsed(null, null);
            ////}
        }

        void Set_RequestTime(bool Running, int InterVal = 20 * 60 * 1000)
        {
            foreach(string key in this.timers_RequestInst.Keys)
            {
                MyTimer a = this.timers_RequestInst[key];
                DataTypePoint dtp = a.Tag as DataTypePoint;

                a.Interval = InterVal;
                if (dtp.ReceiveSeconds > 0)
                {
                    a.Interval = defaultInterVal(dtp);//默认1/10周期
                }
                a.Tick += timer_RequestInst_Tick;
                a.Enabled = Running;
                //a.Start();
            }
        }



        private void CloseFrm()
        {
            //svr.CloseSocket();
            //svr = null;
            chgOpt = null;
            
            if (this.InvokeRequired)//Control.InvokeReauqired判断是否是创建控件线程，不是为true，则需要invoke到创建控件的线程，是就为false，直接操作控件
            {
                CloseCallBack stcb = new CloseCallBack(CloseFrm);
                this.Invoke(stcb, new object[] { });
            }
            else
            {
                this.Close();
            }
        }
        

        private void SendStatusTimer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                
                DateTime now = DateTime.Now;
                //if ()
                //    IsLoadCompleted = isIE ? wr.IsLoadCompleted(webBrowser1.Document) : wr.IsLoadCompleted(geckoWebBrowser1.Document);
                if (Logined == false)
                    Logined = isIE ? wr.IsLogined(webBrowser1.Document) : wr.IsLogined(geckoWebBrowser1.Document);
                if(Logined)
                {
                    WebBrowserLoad = true;
                    if(isIE)
                    {
                        HomeDocIE = webBrowser1.Document;
                        if(isInLotteryDocument())
                        {
      
                        }
                    }
                    else
                    {
                        HomeDocFireFox = geckoWebBrowser1.Document;
                    }
                    double currval = CurrVal;
                }
                if (dicExistInsts != null && dicExistInsts.Count > 0)
                {
                    if(!InSleep) //只有没睡才切换
                        SwitchChanle();
                }
                bool isDebug = false;
                string myinfo = MyIpInfo;
                string useip = myinfo.Split(':')[1];
                UseIp = useip;
                if(isDebug)
                    useip = "127.0.0.1";
                string ip = string.Format("{0}:{1}", useip, UsePort);
                string id = Program.gc.ClientUserName;
                string money = string.Format("{0:f2}", CurrVal);
                string ver = Program.VerNo;
                string urlModel = "{0}{1}{2}";
                string getModel = @"?Ip={0}&User={1}&CurrVal={2}&Logined={3}&Sleep={4}&Load={5}&Ver={6}";
                string reqmsg = string.Format(getModel, ip, id, money, Logined ? 1 : 0, InSleep ? 1 : 0, WebBrowserLoad ? 1 : 0, ver);
                string url = string.Format(urlModel, Program.gc.InstHost, Program.gc.StatusUrlModel, reqmsg);
                CommResult cr = new CommunicateToServer().SendStatusInfo(url);

                if (!cr.Succ)
                {
                    RefreshSendStatusInfo(cr.Message);
                }
                else
                {
                    if(!SocketRunning)//上报后启动socket服务
                    {
                        
                        //svr.Init();
                        //svr.Start(null,UsePort);
                        bool suc = chgRmt.CreateChannel(string.Format("CM_{0}", Program.gc.ClientUserName));
                        if (!suc)
                        {
                            //CloseFrm();
                            return;
                        }
                        SocketRunning = true;
                    }
                    RefreshSendStatusInfo("朕已知悉！");
                    //Program.wxl.Log("可用余额:" + money);
                }
            }
            catch(Exception ce)
            {

            }
        }
        void SwitchChanle()
        {
            releaseWebBrowser();
            
            return;
            //Program.wxl.Log("正在进行网络线路测速！",string.Format("当前线路:{0}", Program.gc.LoginDefaultHost));
            string hostname = wr.GetChanle(string.Format(Program.gc.WebNavUrl,Program.gc.NavHost), Program.gc.LoginDefaultHost, ChanleUseTimes >= 5);

            if (!Program.gc.LoginDefaultHost.Equals(hostname))//如果主机无法连接或者连接最差，切换主机
            {
                if (ChanleUseTimes >= 5)
                {
                    Program.wxl.Log(string.Format("客户端长期使用该线路[{0}次]，自动切换到新线路！", ChanleUseTimes), string.Format("已为您从线路{0}自动切换到线路{1}！", Program.gc.LoginDefaultHost, hostname));

                }
                else
                {
                    Program.wxl.Log("客户端无法连接到下注服务器，或者连速较慢，客户端自动切换线路！", string.Format("已为您从线路{0}自动切换到线路{1}！", Program.gc.LoginDefaultHost, hostname));
                }
                ChanleUseTimes = -1;
                Program.gc.LoginDefaultHost = hostname;
                GlobalClass.SetConfig(Program.gc.ForWeb);
                LoadUrl(loadUrl);
                Logined = true;//如果这个不设置，不会重新下注，而要等到多次刷新后才会为真
                return;
            }
            else
            {
                if (ChanleUseTimes >= 5)
                {
                    Program.wxl.Log(string.Format("客户端长期使用该线路[{0}次]，强制自动切换到新线路！", ChanleUseTimes)+ string.Format("已为您从线路{0}自动切换到线路{1}！", Program.gc.LoginDefaultHost, hostname));
                    ChanleUseTimes = -1;
                    Program.gc.LoginDefaultHost = hostname;
                    GlobalClass.SetConfig(Program.gc.ForWeb);
                    LoadUrl(loadUrl);
                    Logined = true;//如果这个不设置，不会重新下注，而要等到多次刷新后才会为真
                    return;
                }
            }

        }

        void Reboot()
        {
            Program.wxl.Log(string.Format("[{0}]自动重新启动客户端！",Program.gc.ClientAliasName));
            Program.User = Program.gc.ClientUserName;
            Program.strPassword = Program.gc.ClientPassword;
            Program.AutoLogin = true;
            Program.Reboot = true;
            //this.Close();
            this.DialogResult = DialogResult.OK;
            ForceReboot = true;
            CloseFrm();
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
                catch (Exception ce)
                {
                    ip = localaddr.ToString();
                }
            }
            return string.Format("{0}:{1}", hostName, ip);
            //return localaddr.ToString();
        }

        void releaseWebBrowser()
        {
            if(Program.gc.UseBrowser.Trim().ToLower().Equals("firefox"))
            {

            }
            else
            {
                releaseIE();
            }
        }


        void releaseIE()
        {
            try
            {
                //IntPtr pHandle = GetCurrentProcess();
                //SetProcessWorkingSetSize(pHandle, -1, -1);
            }
            catch
            {

            }
        }
        bool loading = false;
        
        HtmlDocument getDocument(GeckoDocument doc)
        {
            HtmlDocument ret = webBrowser1.Document;
            HtmlElementCollection elHeads = ret.GetElementsByTagName("head");
            if (elHeads == null)
            {
                Program.wxl.Log("网页未正常加载，无法注入代码！");
                return ret;
            }
            HtmlElement head = elHeads[0];
            try
            {
                head.OuterHtml = doc.Head.OuterHtml;
                ret.Body.OuterHtml = doc.Body.OuterHtml;
            }
            catch(Exception ce)
            {

            }
            return ret;
        }


        void AddScript(HtmlDocument IEDoc,GeckoDocument FireFoxDoc,string ElementName="body",string LanguageEncoding="gb2312")
        {
            //if (ScriptLoaded)
            //    return;
            string scriptFile = Program.gc.LoginUrlModel.Replace("https", "");
            scriptFile = scriptFile.Replace("http", "");
            scriptFile = scriptFile.Replace("://", "");
            scriptFile = scriptFile.Replace("/", "");
            string strFolderName = string.Format("jscript\\{0}", Program.gc.ForWeb);
            ////scriptFile = scriptFile.Replace(".", "_");
            string jstxt = this.getScriptText("addJscript.js",  strFolderName);
            if (isIE)
            {
                IEDoc.Encoding = LanguageEncoding;
                HtmlElementCollection elHeads = IEDoc.GetElementsByTagName(ElementName);
                if (elHeads == null)
                {
                    Program.wxl.Log("网页未正常加载，无法注入代码！");
                    return;
                }
                if(elHeads.Count == 0)
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
                
            }
            else
            {
                var js = FireFoxDoc.CreateHtmlElement("script");
                js.InnerHtml = jstxt;
                FireFoxDoc.Head.AppendChild(js);
            }
            ScriptLoaded = true;
        }

        void Login()
        {
            try
            {
                if (!isIE)
                {
                    Login_FireFox();
                }
                else
                {
                    Login_IE();
                }


            }
            catch (Exception ce)
            {

            }
        }
        void Login_FireFox()
        {
            //geckoWebBrowser1.ScriptErrorsSuppressed = true;
            LogableClass.ToLog(geckoWebBrowser1.Url.Host, "首次进入，加载文件！");
            //if(!ScriptLoaded)
            AddScript(null,loginDocFireFox??HomeDocFireFox); //执行js代码,未来修改为网络载入
            //MySleep(3 * 1000);//等待验证码图片载入
                              //geckoWebBrowser1.Document.InvokeScript("Login", new string[] { Program.gc.ClientUserName, Program.gc.ClientPassword, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost), Program.gc.WXLogNoticeUser });
            using (AutoJSContext context = new AutoJSContext(geckoWebBrowser1.Window))
            {
                string jstxt = string.Format("Login('{0}','{1}','{2}','{3}');", Program.gc.ClientUserName, Program.gc.ClientPassword, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost), Program.gc.WXLogNoticeUser);
                try
                {
                    context.EvaluateScript(jstxt);
                }
                catch(Exception ce)
                {
                    MessageBox.Show(string.Format("{0}:{1}", ce.Message, ce.StackTrace));
                    return;
                }
            }
            WebBrowserLoad = true;
        }


        private void geckoWebBrowser1_ValidityOverride(object sender, Gecko.Events.CertOverrideEventArgs e)
        {
            e.OverrideResult = Gecko.CertOverride.Mismatch | Gecko.CertOverride.Time | Gecko.CertOverride.Untrusted; e.Temporary = true; e.Handled = true;

        }


        void Login_IE()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            //webBrowser1.ScriptErrorsSuppressed = false;
            //LogableClass.ToLog(webBrowser1.Url.Host, "首次进入，加载文件！");
            //if(!ScriptLoaded)
            AddScript(loginDocIE ??HomeDocIE, null); //执行js代码,未来修改为网络载入
            //MySleep(3 * 1000);//等待验证码图片载入
            (loginDocIE ?? HomeDocIE).InvokeScript("Login", new string[] { currUser.WebLoginId, currUser.WebLoginPwd, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost), Program.gc.WXLogNoticeUser ,string.Format(Program.gc.LoginUrlModel,Program.gc.LoginDefaultHost)});
            WebBrowserLoad = true;
        }

        string loadUrl = null;
        HtmlDocument loginDocIE;
        HtmlDocument lotteryDocIE;
        HtmlDocument HomeDocIE;

        GeckoDocument loginDocFireFox;
        GeckoDocument lotteryDocFireFox;
        GeckoDocument HomeDocFireFox;

        CookieCollection getCookie(string strCookie)
        {
            CookieCollection cc = new CookieCollection();
            try
            {
                if (string.IsNullOrEmpty(strCookie))
                {
                    return cc;
                }
                string[] arr = strCookie.Split(';');
                for (int i = 0; i < arr.Length; i++)
                {
                    string strC = arr[i].Trim();
                    string[] cArr = strC.Split('=');
                    string name = cArr[0].Trim();
                    string val = cArr[1].Trim();
                    string dom = "";
                    if (cArr.Length > 2)
                        dom = cArr[2];
                    System.Net.Cookie ck = new System.Net.Cookie(name, val,"/",Host.Replace("https://","").Replace("http://",""));
                    cc.Add(ck);
                }
            }
            catch (Exception ce)
            {
                this.toolStripStatusLabel3.Text = string.Format("{0}:{1}", ce.Message, ce.StackTrace);
            }
            return cc;
        }

        private void DocumentCompleted_IE(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
            try
            {
                
                //if (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                //    return;
                //if((sender as WebBrowser).Url.ToString() != loadUrl)
                //{
                //    return;
                //}
                HtmlDocument doc = webBrowser1.Document;
                if (SendMsgFromWebRequest  && !Logined)
                {
                    if (IsLoadComplete(Program.gc.HostKey))//如果加载到主页，那就登录
                    {
                        string strUrl = doc.Url.ToString().Replace("//","/");
                        string lotteryPage = string.Format(Program.gc.LotteryPage, string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost)).Replace("//","/");
                        if(strUrl == lotteryPage)
                        {
                            getAmount();
                            MySleep(3 * 1000);
                            Logined = true;
                            InSleep = false;
                            WebBrowserLoad = true;
                            TSMI_sendStatusInfor_Click(null, null);
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
                if(SendMsgFromWebRequest)
                {
                    return;
                }
                //CurrVal = wr.GetCurrMoney(doc);
                if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
                {
                    
                    CookieCollection cc =getCookie(this.webBrowser1.Document.Cookie);
                    //if(Program.gc.SendMsgFromWebRequest == "1")
                    //    this.ce.SetCookie(cc);
                    HomeDocIE = this.webBrowser1.Document;
                    this.toolStripStatusLabel1.Text = "已载入";
                    if (ReadySleep)
                    {
                        LogableClass.ToLog(webBrowser1.Url.Host, "进入睡眠！");
                        ReadySleep = false;
                        return;
                    }
                    LogableClass.ToLog(webBrowser1.Url.Host, "唤醒！");
                    bool IsVaildWeb = wr.IsVaildWeb(doc);
                    bool IsLogined = wr.IsLogined(doc);
                    if (IsVaildWeb && IsLogined == false)
                    {
                        MySleep(5 * 1000);
                        IsLogined = wr.IsLogined(doc);
                    }

                    bool IsLoadCompleted = IsLoadComplete(Program.gc.HostKey);// wr.IsLoadCompleted(doc);
                    if (IsLoadCompleted || IsLogined) //缓存已登录
                    {
                        WebBrowserLoad = true;
                    }
                    if (IsLoadCompleted)//一旦加载的有网站标志，那就是主页
                    {
                        HomeDocIE = webBrowser1.Document;
                    }
                    if (isInLoginDocument())
                    {
                        //loginDocIE = webBrowser1.Document;
                       
                    }
                    if (isInLotteryDocument())//如果是投注页
                    {
                        //lotteryDocIE = webBrowser1.Document;
                    }
                    if (!IsLoadCompleted)
                    {
                        loading = true;
                        this.toolStripStatusLabel1.Text = "未完全载入！";
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
                                ////while (!TryLogin)
                                ////{
                                ////    if (wr.IsLogined(doc))
                                ////        break;
                                ////    Thread.Sleep(10 * 1000);
                                ////}
                                if (TryLogin == false)
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
                    if(wr.IsLogined(doc))
                    {
                        Logined = true;
                        WebBrowserLoad = true;
                    }
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
            catch(Exception ce)
            {
                this.toolStripStatusLabel3.Text = string.Format("{0}:{1}", ce.Message, ce.StackTrace);
            }
        }

        private bool isInLoginDocument()
        {
            if(isIE)
            {
                HtmlDocument doc = webBrowser1.Document;
                bool suc = WebRule.existElement(doc, Program.gc.LoginPageUserNameId);
                if(suc)
                {
                    HomeDocIE = doc;
                    return true;
                }
               
                for(int i=0;i<webBrowser1.Document.Window.Frames.Count;i++)
                {
                    suc = WebRule.existElement(webBrowser1.Document.Window.Frames[i].Document, Program.gc.LoginPageUserNameId);
                    if (suc)
                    {
                        HomeDocIE = webBrowser1.Document.Window.Frames[i].Document;
                        return true;
                    }

                }
                
            }
            else
            {
                GeckoDocument doc =  geckoWebBrowser1.Document;
                return WebRule.existElement(doc, Program.gc.LoginPageUserNameId);
            }
            return false;
        }

        private bool IsLoadComplete(string key)
        {
            
            if (isIE)
            {
                HtmlDocument doc = webBrowser1.Document;
                bool succ = WebRule.existElement(HomeDocIE??doc, key);
                if(succ)
                {
                    HomeDocIE = doc;
                    loginDocIE = doc;
                    return true;
                }
                for(int i=0;i<doc.Window.Frames.Count;i++)
                {
                    succ = WebRule.existElement(doc.Window.Frames[i], key);
                    if(succ)
                    {
                        HomeDocIE = doc;
                        loginDocIE = doc;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if (HomeDocFireFox == null)
                    return false;
                return WebRule.existElement(HomeDocFireFox, key);
            }
        }

        private bool isInLotteryDocument()
        {
            object obj = null;
            if (isIE)
            {
                
                obj = webBrowser1.Document;
                HtmlDocument doc = obj as HtmlDocument;
                bool succ = WebRule.existElement(doc, Program.gc.LetteryPageUserKeyId);
                if(succ)
                {
                    lotteryDocIE = doc;
                    return true;
                }
                for (int i = 0; i < doc.Window.Frames.Count; i++)
                {
                    succ = WebRule.existElement(doc.Window.Frames[i].Document, Program.gc.LetteryPageUserKeyId);
                    if (succ)
                    {
                        lotteryDocIE = doc.Window.Frames[i].Document;
                        return true;
                    }
                }
                return false;
            }
            else
                obj = geckoWebBrowser1.Document;
            return WebRule.existElement(obj, Program.gc.LetteryPageUserKeyId);

        }

        private void DocumentCompleted_FireFox(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            try
            {
                //if (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                //    return;
                //if((sender as WebBrowser).Url.ToString() != loadUrl)
                //{
                //    return;
                //}
                GeckoDocument doc = geckoWebBrowser1.Document;

                //CurrVal = wr.GetCurrMoney(doc);
                if (geckoWebBrowser1.Document.ReadyState == "complete")
                {

                    this.toolStripStatusLabel1.Text = "已载入";
                    if (ReadySleep)
                    {
                        LogableClass.ToLog(geckoWebBrowser1.Url.Host, "进入睡眠！");
                        ReadySleep = false;
                        return;
                    }
                    LogableClass.ToLog(geckoWebBrowser1.Url.Host, "唤醒！");
                    bool IsVaildWeb = wr.IsVaildWeb(doc);
                    bool IsLogined = wr.IsLogined(doc);
                    //if (IsVaildWeb && IsLogined == false)
                    //{
                    //    MySleep(5 * 1000);
                    //    IsLogined = wr.IsLogined(doc);
                    //}
                    HomeDocFireFox = geckoWebBrowser1.Document;
                    bool IsLoadCompleted = IsLoadComplete(Program.gc.HostKey);// wr.IsLoadCompleted(doc);
                    if(IsLoadCompleted||IsLogined) //缓存已登录
                    {
                        WebBrowserLoad = true;
                    }
                    if (IsLoadCompleted)//一旦加载的有网站标志，那就是主页
                    {
                        HomeDocFireFox = geckoWebBrowser1.Document;
                    }
                    if (isInLoginDocument())
                    {
                        loginDocFireFox = geckoWebBrowser1.Document;
                    }
                    if (isInLotteryDocument())//如果是投注页
                    {
                        lotteryDocFireFox = geckoWebBrowser1.Document;
                    }
                    if (!IsLoadCompleted)
                    {
                        loading = true;
                        this.toolStripStatusLabel1.Text = "未完全载入！";
                        return;
                    }
                    loading = false;
                    if (IsVaildWeb || IsLogined)//是登录页面或者内容页面
                    {
                        if (IsVaildWeb)
                        {
                            if (!WebBrowserLoad || loginDocFireFox!= null)//第一次载入 或者是在login页面
                            {
                                Login();

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
                                if (TryLogin == false)
                                {
                                    LogableClass.ToLog(geckoWebBrowser1.Url.Host, "还没完全加载！");
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
                                LogableClass.ToLog(geckoWebBrowser1.Url.Host, "无法访问主机，建议启动服务选择合适的备用服务！");
                                reLoadWebBrowser();
                                return;
                            }
                            else
                            {
                                reLoadWebBrowser();
                                LogableClass.ToLog(geckoWebBrowser1.Url.Host, "登录后无法访问主机，建议更换主机重新登录！");
                                return;
                            }
                        }
                        else
                        {
                            //其他网页
                            LogableClass.ToLog(geckoWebBrowser1.Url.Host, "睡眠综合症！");
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
            catch (Exception ce)
            {
                this.toolStripStatusLabel3.Text = string.Format("{0}:{1}", ce.Message, ce.StackTrace);
            }

        }

        string getScriptText(string scriptName,string subFolder="")
        {
            string ret = "";
            string FilePath = string.Format("{0}\\{1}\\{2}", Application.StartupPath,subFolder, scriptName);
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
            RefreshImages();
        }

        void MySleep(long milsec)
        {
            int tick = Environment.TickCount;
            while (Environment.TickCount - tick < milsec)
            {
                Application.DoEvents();
            }

        }

        
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        Dictionary<string, string> getAssetLists(string key)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            CommunicateToServer wc = new CommunicateToServer();
            CommResult cr = wc.getRequestAssetList(string.Format(GlobalClass.strAssetInfoURL,GlobalClass.TypeDataPoints[key].InstHost,key));
            if (!cr.Succ)
            {
                this.statusStrip1.Items[0].Text = cr.Message;
                return ret;
            }
            mAssetUnitList ic = cr.Result[0] as mAssetUnitList;
            for (int i = 0; i < ic.Count; i++)
            {
                ret.Add(ic.List[i].AssetId, ic.List[i].AssetName);
            }
            return ret;
        }

        private void timer_RequestInst_Tick(object sender, EventArgs e)
        {
            try
            {
                MyTimer tm = sender as MyTimer;
                if (tm == null)
                    tm = timers_RequestInst.Values.First();
                //tm.Enabled = false;
                DataTypePoint dtp = tm.Tag as DataTypePoint;
                if (dtp == null)
                    return;
                string dtpName = dtp.DataType;
                if (RefreshTimes >0 && !inWorkTimeRange())
                {
                    WXMsgBox(Program.gc.ClientAliasName, string.Format("当日交易结束，资金余额:{0}",CurrVal));
                    if(SendMsgFromWebRequest)
                    {
                        if(!InSleep)//开着百度睡觉，把登录状态改为false
                        {
                            Logined = false;
                            reLoadWebBrowser();
                            //reLoadWebBrowser();
                        }
                    }
                    else
                    {
                        if(!InSleep)
                        {
                            KnockEgg();
                            Application.DoEvents();
                            MySleep(10 * 1000);
                            Logined = false;
                            reLoadWebBrowser();
                        }

                    }

                    return;
                }
                DateTime CurrTime = DateTime.Now.AddHours(dtp.DiffHours).AddMinutes(dtp.DiffMinutes);//调整后的时间
                Random rd = new Random();
                int buffSec = (int)dtp.ReceiveSeconds / 60;
                int rndtime = rd.Next(1000, buffSec * 1000);//增加随机数，防止单机多客户端并行同时下注
                if (RefreshTimes > 0 && !inWorkTimeRange(dtpName))
                {
                    DateTime TargetTime = DateTime.Today.AddHours(dtp.ReceiveStartTime.Hour).AddMinutes(dtp.ReceiveStartTime.Minute);
                    tm.Interval = Math.Max(30*1000,(int)TargetTime.Subtract(CurrTime).TotalMilliseconds) + rndtime;
                    DateTime nextTime = DateTime.Now.AddMilliseconds(tm.Interval);
                    KnockEgg();//敲蛋
                    Application.DoEvents();
                    MySleep(10 * 1000);//暂停，等发送消息
                                       //reLoadWebBrowser();//开百度网页，睡觉
                    timers_RequestInst[tm.dtp.DataType].Interval = tm.Interval;//实际把计时器间隔往后推
                    WXMsgBox(string.Format("{0}下次启动时间！",dtpName), nextTime.ToString("yyyyMMdd HH:mm:ss"));
                    return;
                }
                if (!WebBrowserLoad)
                {//只能等待加载完成！
                    //return;
                }
                if (InSleep)
                {
                    string currUrl = webBrowser1.Url.ToString().Replace("https://", "").Replace("http://", "");//.Replace("/","");
                    string strHost = Host.Replace("https://", "").Replace("http://", "");
                    if (strHost.EndsWith("/"))
                    {
                        strHost = Host.Substring(0, Host.Length - 1);
                    }
                    if (currUrl.EndsWith("/"))
                    {
                        currUrl = currUrl.Substring(0, currUrl.Length - 1);
                    }
                    bool inHost = false;
                    if (currUrl == strHost)
                    {
                        inHost = true;
                    }
                    else
                    {
                        if(string.Format(Program.gc.LotteryPage,strHost) ==  currUrl)
                        {
                            InSleep = false;
                            Logined = true;
                            WebBrowserLoad = true;
                            WXMsgBox("重置状态", "已经登录");
                            return;
                        }
                    }
                    if (!inHost)
                    {
                        tm.Interval = 3 * defaultInterVal(tm.dtp);//给登录一点时间
                        string logpage = string.Format(Program.gc.LoginPage, Host);
                        try
                        {
                            
                            lock (webBrowser1)
                            {
                                
                                new Task(() =>
                                {
                                    webBrowser1.Navigate(logpage);
                                }).Start();
                                //Application.DoEvents();
                            }
                            //WXMsgBox("转到登录页", logpage);
                        }
                        catch(Exception ce)
                        {
                            WXMsgBox("转到登录页错误", string.Format("{0}:{1}", ce.Message, ce.StackTrace));
                        }
                        return;
                    }
                    else//如果在页面但是没有登录，就开始登录
                    {
                        Login();
                        tm.Interval = 2 * defaultInterVal(tm.dtp);//给登录一点时间
                    }
                }
                
                int ruleid = 0;
                if (rule == null)
                {
                    rule = WebRuleBuilder.Create(Program.gc.InstFormat,Program.gc, Program.gc);
                }
                if (rule.config != null)
                    int.TryParse(rule.config.lotteryTypes.ContainsKey(dtpName) ? rule.config.lotteryTypes[dtpName].ruleId : "0", out ruleid);
                if (NeedLoadGameInfo && Logined)
                {
                    
                    getGameInfo(ruleid);
                }
                
                CommunicateToServer wc = new CommunicateToServer();
                string testExpect = "";
                string strSubMode = "{0}/pk10/app/requestInsts.asp?Dtp={1}&App={2}&webcode={3}&loginname={4}&testExpect={5}";
                string strMainMode = "{0}/pk10/app/requestInsts.asp?Dtp={1}&testExpect={2}";
                string strRequestUrl = "";
                strRequestUrl = string.Format(strSubMode, dtp.InstHost, dtp.DataType,AppMode?1:0,Program.gc.ForWeb,Program.gc.ClientUserName, testExpect);
                //CommResult cr = wc.getRequestInsts<RequestClass>(string.Format("{0}/{1}", dtp.InstHost, string.Format("pk10/app/requestInsts.asp?Dtp={0}&testExpect={1}", dtp.DataType, testExpect)));
                CommResult cr = wc.getRequestInsts<RequestClass>(strRequestUrl);
                if (!cr.Succ)
                {
                    this.statusStrip1.Items[0].Text = cr.Message;
                    WXMsgBox(dtpName, "接收指令错误！");
                    return;
                }
                if (cr.Cnt != 1)
                {
                    this.statusStrip1.Items[0].Text = "无指令！";
                    Program.wxl.Log("无指令！", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                    return;
                }
                ///pk10/app/getwaveselecttimeamount.asp?type=xyft&expect=20200405120&cnt=1
                ///
                /*
                 public int EmergencyStop=0;        
        public int StopIgnoreLength = 10;//刹车忽略长度
        public int StopStepLen = 4;//刹车降速步长
        public int StopPower = 2;//刹车力度，0为立即刹车，其他为降速倒数
                 */
                string selectTimeAmtUrlModel = "pk10/app/getwaveselecttimeamount.asp?type={0}&expect={1}&cnt={2}&defaultReturnValue={3}&AutoResumeDefaultValue={4}&EmergencyStop={5}&StopIgnoreLength={6}&StopStepLen={7}&StopPower={8}&SelectFuncId={9}";
                RequestClass ic = cr.Result[0] as RequestClass;
                if (ic == null)
                {
                    this.statusStrip1.Items[0].Text = "指令内容错误！";
                    Program.wxl.Log(string.Format("[{0}]指令内容错误！",Program.gc.ClientAliasName), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                    return;
                }
                Int64 CurrExpectNo = Int64.Parse(ic.Expect);
                //if (this.statusStrip1.Items.Count >= 2)
                //{
                
                //}
                
                string strList = "";
                string txt = "";
                bool NewRec = false;
                bool NeedSaveAssetConfig = false;
                if (CurrExpectNo > this.NewExpects[dtp.DataType] && this.NewExpects[dtpName] > 0)
                {
                    NewRec = true;
                }
                if (NewRec ||this.NewExpects[dtp.DataType] == 0) //获取到最新指令
                {
                    getGameInfo(ruleid);
                    getAmount();
                    MySleep(2 * 1000);
                    //WXMsgBox(string.Format("{0}数据最新期{1}", dtpName, ic.Expect), ic.Insts);
                    LastInstsTime = DateTime.Now.AddHours(dtp.DiffHours).AddMinutes(dtp.DiffMinutes);
                    int CurrMin = DateTime.Now.Minute % 5;
                    //资产对象
                    //b原有值
                    //c返回值
                    //cc返回对象
                    ic.SelectTimeChanged = (a, b,c,cc) => {
                        Single currProb100 = 0.0F;
                        Single safeProb100 = 0.0F;
                        Single currProb300 = 0.0F;
                        Single safeProb300 = 0.0F;
                        Single currProb1000 = 0.0F;
                        Single safeProb1000 = 0.0F;
                        int No2MatchCnt = 0;
                        if (c.List.Count > 0)
                        {
                            
                            currProb100 = ExpectDataInfo.getProbByPeriod(c.List,100,out safeProb100);
                            currProb300 = ExpectDataInfo.getProbByPeriod(c.List, 300,out safeProb300);
                            currProb1000 = ExpectDataInfo.getProbByPeriod(c.List, 1000, out safeProb1000);
                            DataTable dt = DisplayAsTableClass.ToTable<MatchGroupClass>(c.List);
                            new Task(() =>
                            {
                                
                                //DataView dv = new DataView(dt);
                                SetDataGridDataTable<string>(this.dgv_matchGroupList, dt,null, "SerNo");
                            }).Start();
                            DataRow[] drs = dt.Select("MatchCnt>1","SerNo");
                            int maxId = int.Parse(dt.Rows[dt.Rows.Count - 1]["SerNo"].ToString());//最后一个SerNo
                            if(drs.Length>0)
                            {
                                maxId = int.Parse(drs[0]["SerNo"].ToString());
                            }
                            drs = dt.Select("SerNo < " + maxId);
                            No2MatchCnt = drs.ToList().Sum(dr => int.Parse(dr["chipCount"].ToString()));
                            //dv.Sort = "SerNo";
                            //this.dgv_matchGroupList.DataSource = dv;
                            //this.dgv_matchGroupList.Refresh();
                            //Application.DoEvents();

                        }
                        string strProbModel = "概率(%)100期:当前[{0}]/安全[{1}];300期:当前[{2}]/安全[{3}];1000期:当前[{4}]/安全[{5}];持续{6}次机会未出双响！";
                        string strProb = string.Format(strProbModel, currProb100, safeProb100, currProb300, safeProb300, currProb1000, safeProb1000, No2MatchCnt);
                        strList = string.Format("[{0}期提示:资产单元{5}[机会:{1};数量:{2}]];{3}最近5轮出现的组合信息为:{4}", CurrExpectNo,cc.ChanceCode,cc.UnitCost, strProb,string.Join(",", c.List.Select(curr =>string.Format("{0}次{1}个组合",curr.times, curr.chipCount)).Take(5).ToArray()), AssetUnitList[a]);
                        string strDiff = "";
                        if(Program.gc.AssetUnits[a].AutoEmergencyStop == 1 && judgeAutoABS(c))//如果需要自动计算刹车
                        {
                            NeedSaveAssetConfig = true;
                            
                            Program.gc.AssetUnits[a].EmergencyStop = 1;
                            if(IsAdmin)
                                WXMsgBox(string.Format("彩种:{0}第{1}期", dtpName, CurrExpectNo), "因行情运行至高位，自动启动ABS，下期命中后将自动停止投注，将当前值恢复到0！");
                        }
                        if (b != c.ReturnCnt)
                        {
                            Program.gc.AssetUnits[a].CurrTimes = c.ReturnCnt;
                            string autoSetting = "";
                            if(c!=null && c.List!= null && c.List.Count>0 && c.List[0].chipCount>Program.gc.AssetUnits[a].AutoTraceMinChips)//达到启动值后自动设置启动回复默认值为1，就可以自动跟踪了
                            {
                                Program.gc.AssetUnits[a].AutoResumeDefaultReturnValue = 1;
                                autoSetting = "跟踪到长期机会，自动点火跟随启动!";
                            }
                            if(b == 1 && c.ReturnCnt == 0 && Program.gc.AssetUnits[a].ZeroCloseResume == 1)//允许到0关闭恢复功能，如果从1恢复到0，将AutoResumeDefaultReturnValue置为0，等下次跟踪到了以后再启动
                            {
                                Program.gc.AssetUnits[a].AutoResumeDefaultReturnValue = 0;
                                autoSetting = "长期跟踪未命中，自动点火跟自动关闭，等待下次触发自动启动!如果觉得最近机会很好，可以手动打开！";
                            }
                            strDiff = string.Format("资产单元{0}信号由{1}改为{2},当前基本值为{3}!{4}", AssetUnitList[a], b, c.ReturnCnt,Program.gc.AssetUnits[a].value,string.IsNullOrEmpty(autoSetting)?"":autoSetting);
                            NeedSaveAssetConfig = true;
                        }
                        if (IsAdmin)
                        {
                            new Task(() =>
                            {
                                Program.wxl.Log(string.Format("{0};{1}", strList, strDiff), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                            }).Start();
                        }

                    };
                    if (!AppMode)
                    {
                        txt = ic.getUserInsts(Program.gc, dtp, ic.Expect, Program.gc.ForWeb, CurrVal,
                            (dp, ec, aic) =>
                            {

                                CommunicateToServer cts = new CommunicateToServer();
                            /*
                             "pk10/app/getwaveselecttimeamount.asp?
                             type={0}
                             &expect={1}
                             &cnt={2}
                             &defaultReturnValue={3}
                             &AutoResumeDefaultValue={4}
                             &EmergencyStop={5}
                             &StopIgnoreLength={6}
                             &StopStepLen={7}
                             &StopPower={8}";
                */
                                string strTestExpect = "";
                            // string selectTimeAmtUrlModel = "pk10/app/getwaveselecttimeamount.asp?type={0}&expect={1}&cnt={2}&defaultReturnValue={3}&AutoResumeDefaultValue={4}&EmergencyStop={5}&StopIgnoreLength={6}&StopStepLen={7}&StopPower={8}";
                            CommResult cs = wc.getRequestInsts<SelectTimeInstClass>(string.Format("{0}/{1}", dtp.InstHost, string.Format(
                                    selectTimeAmtUrlModel,
                                    dp,
                                    ec,
                                    aic.CurrTimes,
                                    aic.DefaultReturnTimes,
                                    aic.AutoResumeDefaultReturnValue,
                                    aic.EmergencyStop,
                                    aic.StopIgnoreLength,
                                    aic.StopStepLen,
                                    aic.StopPower,
                                    aic.SelectFuncId
                                    )));
                                if (!cs.Succ)
                                {
                                    this.statusStrip1.Items[0].Text = cr.Message;
                                    WXMsgBox(dtpName, string.Format("Func:{0}接收择时指令异常:{1} ",aic.SelectFuncId, cr.Message));
                                    return null;
                                }
                                if (cs.Cnt != 1)
                                {
                                    this.statusStrip1.Items[0].Text = "择时信息异常！";
                                    WXMsgBox(dtpName, "择时信息异常");
                                    return null;
                                }
                                SelectTimeInstClass sti = cs.Result[0] as SelectTimeInstClass;
                                return sti;
                            });
                    }
                    else
                    {
                        txt = ic.Insts;
                    }
                    string[] insts = txt.Trim().Replace("+", " ").Split(' ');
                    long AllSum = insts.Where(a => a.Trim().Length > 1).ToList().Select(a => long.Parse(a.Trim().Split('/')[2])).Sum();
                    
                    //this.NewExpects[dtpName] = CurrExpectNo;
                    tm.Interval = defaultInterVal(dtp);
                    if(NeedSaveAssetConfig)
                    {
                        new Task(()=>
                        {
                            if(!frm_setting.SaveToServer(Program.gc.AssetUnits, false))
                            {
                                WXMsgBox("择时自动修改配置后上传服务器失败！", "如果连续出现此信号在配置界面请手动保存，以免在关掉程序后重新下载配置后实际配置与预期不一样！");
                            }
                            
                        }).Start();
                        

                        

                    }
                    if (Logined)
                    {
                        if (NewRec)//新记录才发送，第一条记录不发送
                        {
                            
                            if (AllSum == 0)
                            {
                                new Task(() =>
                                {
                                    getGameInfo(ruleid);
                                    getAmount();
                                }).Start();
                                MySleep(2 * 1000);
                                //Program.wxl.Log(string.Format("[{1}]第{0}期资金余额:{2}", CurrExpectNo , Program.gc.ClientAliasName,CurrVal), "当期指令为空信号，或者金额为0", string.Format("{0}:[{1}]", "指令", txt), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                                SwitchChanle();//赶着空指令切换线路
                                //if (dicExistInsts.ContainsKey(dtpName) && !dicExistInsts[dtpName].ContainsKey(this.NewExpects[dtpName]))//防止多次发送
                                NewExpects[dtpName] = CurrExpectNo;
                                if(NewRec && !dicExistInsts[dtpName].ContainsKey(CurrExpectNo))
                                {
                                    dicExistInsts[dtpName].Add(CurrExpectNo, txt);
                                }
                                refreshWindow(dtp, ic, txt, strList);

                                return;
                            }
                            RefreshTimes = 1;
                            if (dicExistInsts.ContainsKey(dtpName) && !dicExistInsts[dtpName].ContainsKey(CurrExpectNo))//防止多次发送
                            {
                                if (txt.Trim().Length > 0)//浪费，一定是>0
                                {
                                    this.Send_Data(dtp, CurrExpectNo.ToString(),txt);
                                    if (!SendMsgFromWebRequest)//如果不是从web请求发送，必须发送完后要加入。web请求从事件成功后处理。
                                    {
                                        dicExistInsts[dtpName].Add(CurrExpectNo, txt);
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewExpects[dtpName] = CurrExpectNo;
                            WXMsgBox(string.Format("[{1}]第{0}期", CurrExpectNo, dtpName), string.Format("{0},{1}",txt,"刚登录第一期不自动发送，如果有需要请手动点击发送！"));
                        }

                    }
                    refreshWindow(dtp, ic, txt,strList);

                }
                else//非新数据
                {
                    //if (!inWorkTimeRange())
                    //{
                    //    return;
                    //}
                    //如果存在的字典里面包含了本彩种，但是本彩种没有最新的投注记录
                    if(dicExistInsts.ContainsKey(dtpName) && !dicExistInsts[dtpName].ContainsKey(CurrExpectNo) && dicExistInsts.Count>0)//非第一次
                    {
                        if (txt.Trim().Length > 0)
                        {
                            this.Send_Data(dtp, CurrExpectNo.ToString(), txt);
                            if (SendMsgFromWebRequest)
                            {
                                tm.Interval = defaultInterVal(tm.dtp, true);
                                WXMsgBox("补发送数据", txt);
                            }
                        }
                        //return;
                    }
                    if (CurrTime.TimeOfDay < dtp.ReceiveStartTime.TimeOfDay)//如果在9点前
                    {
                        //下一个时间点是9：08
                        //DateTime ReceiveStartTime = dtp.ReceiveStartTime;
                        DateTime TargetTime = DateTime.Today.AddHours(dtp.ReceiveStartTime.Hour).AddMinutes(dtp.ReceiveStartTime.Minute);
                        tm.Interval = Math.Max(30*1000,(int)TargetTime.Subtract(CurrTime).TotalMilliseconds) + rndtime;
                        DateTime nextTime = DateTime.Now.AddMilliseconds(tm.Interval);
                        KnockEgg();//敲蛋
                        Application.DoEvents();
                        MySleep(10*1000);//暂停，等发送消息
                        //reLoadWebBrowser();//开百度网页，睡觉
                        timers_RequestInst[tm.dtp.DataType].Interval = tm.Interval;//实际把计时器间隔往后推
                        WXMsgBox(string.Format("{0}下次启动时间！",dtp.DataType), nextTime.ToString("yyyyMMdd HH:mm:ss"));
                    }
                    else
                    {
                        int interVal = (int)getInterVal(dtp, false);
                        if (CurrTime.Subtract(LastInstsTime).Seconds > (int)dtp.ReceiveSeconds*3/2)//如果离上期时间超过7分钟，说明数据未接收到，那不要再频繁以10秒访问服务器
                        {
                            //tm.Interval = (int)dtp.ReceiveSeconds * 2*1000 / 5 + rndtime;
                            tm.Interval = 2 * interVal;
                        }
                        else //一般未接收到，2*60秒以后再试
                        {
                            //tm.Interval = (int)dtp.ReceiveSeconds * 1*1000 / 5 + rndtime;
                            tm.Interval = interVal;
                        }
                    }
                }
                RefreshStatus(tm.Interval);
            }
            catch (Exception ce)
            {
                new Task(() =>
                {
                    LogableClass.ToLog("错误", "刷新指令", string.Format("{0}:{1}", ce.Message, ce.StackTrace));
                    Program.wxl.Log("错误", string.Format("[{0}]刷新指令发生错误。", Program.gc.ClientAliasName), string.Format("{0}:{1}", ce.Message, ce.StackTrace), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                }).Start();
            }
        }
        static Dictionary<string,MatchGroupClass> AllMatchList;
        bool judgeAutoABS(SelectTimeInstClass sti)
        {
            //只有在命中后判定？
            /*一旦命中后就判定是否需要设置紧急制动，其他情况不去判定。
             * 
             */
            try
            {
                //if (sti.List[0].times > 1)
                //    return false;
                if(AllMatchList == null)
                {
                    AllMatchList = new Dictionary<string,MatchGroupClass>();
                }

                //跳过第一个
                sti.List.Skip(1).ToList().ForEach(a => {
                    if (!AllMatchList.ContainsKey(a.ExpectCode))//保存所有的matchlist
                        AllMatchList.Add(a.ExpectCode, a);
                });
                Dictionary<String, MatchGroupClass> useDic = AllMatchList.ToDictionary(a => a.Key, a => a.Value);
                useDic.Add(sti.List[0].ExpectCode,sti.List[0]);
                List<MatchGroupClass> mlist = useDic.Values.OrderByDescending(a=>a.ExpectCode).Take(500).OrderBy(a=>a.ExpectCode).ToList();//取前500次
                //DataTable dt = WaveDataCheckClass.CreateStandardDataTable();//建立一个标准模板
                double initValue = 400;//单利倍数，50对应20000，一份单位为1
                double currValue = initValue;
                int currWeight = 0;
                int idx = 0;
                double odds = Program.gc.Odds;
                List<TimeSerialPoint<double>> vals = new List<TimeSerialPoint<double>>();
                for(int i = 0;i<mlist.Count;i++ )//模拟单利
                {
                    idx = i;
                    MatchGroupClass mgc = mlist[i];

                    double cost = mgc.chipCount * 1;
                    double gained = mgc.MatchCnt * odds;
                    double profit = gained - cost;
                    currValue += profit;
                    currWeight += mgc.chipCount;
                    vals.Add(new TimeSerialPoint<double>(currWeight,currValue));
                }
                WaveDataCheckResultClass<double> ret = WaveDataCheckClass.getCheckResult<double>(vals, 400, NetSelectBasicValue.MinValue, 10);
                DataTable dt = ret.SamplestTable();
                if (IsAdmin)
                {
                    WXMsgBox(dt, "Weight", "Net");
                    WXMsgBox(string.Format("{0}[{1}]最后一个波段信息", sti.DataType, sti.Expect), ret.LastArea?.Descript());
                }
                if (!ret.LastArea.IsRaise)//如果是向下的，直接跳过，不可能止盈
                {
                    return false;
                }
                if(ret.LastArea.Len<5)//如果小于5轮，也无法判断是否需要止盈
                {
                    return false;
                }
                mlist = mlist.Skip(ret.LastArea.StartIndex).ToList();//最后一段数据
                if(ret.LastArea.DrawDownRate > 0.2)//大于20%，立即止盈
                {
                    return true;
                }
                if (mlist.Count < 5)//大于5期
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }            
            return false;
        }
        void refreshWindow(DataTypePoint dtp, RequestClass ic, string txt, string strList)
        {
            this.toolStripStatusLabel2.Text = DateTime.Now.ToLongTimeString();
            this.txt_ExpectNo.Text = ic.Expect;
            this.txt_OpenTime.Text = ic.LastTime;
            this.txt_OpenCode.Text = ic.LastOpenCode;
            this.txt_OpenTime.BackColor = Color.Green;
            this.txt_ExpectNo.BackColor = Color.Green;
            this.txt_OpenCode.BackColor = Color.Green;
            this.txt_Insts.Text = txt;
            this.txt_NewInsts.Text = strList;
            RefreshImages();
            DateTime preTime = DateTime.Parse(ic.LastTime).AddSeconds(dtp.ReceiveSeconds);
            if (ic.LastTime.Trim().Length > 0 && DateTime.Now > preTime)
            {
                this.txt_OpenTime.BackColor = Color.Red;
                Application.DoEvents();
                this.txt_ExpectNo.BackColor = Color.Red;
                Application.DoEvents();
                this.txt_OpenCode.BackColor = Color.Red;
                Application.DoEvents();
            }
        }
        bool inWorkTimeRange(string dtpName = null)
        {
            DateTime startTime;
            DateTime endTime;
            DateTime currTime = DateTime.Now;
            foreach (DataTypePoint dtp in dtpName==null?GlobalClass.TypeDataPoints.Values:GlobalClass.TypeDataPoints.Where(a=>a.Key == dtpName).Select(a=>a.Value))
            {
                DateTime useStartTime = dtp.ReceiveStartTime.AddHours(-1 * dtp.DiffHours).AddMinutes(-1 * dtp.DiffMinutes);
                DateTime useEndTime = (dtp.ReceiveEndTime == DateTime.MinValue ? DateTime.Parse("23:59:00") : dtp.ReceiveEndTime).AddHours(-1 * dtp.DiffHours).AddMinutes(-1 * dtp.DiffMinutes);
                DateTime currDayStartTime = DateTime.Today.AddTicks(useStartTime.TimeOfDay.Ticks);
                DateTime currDayEndTime = DateTime.Today.AddTicks(useEndTime.TimeOfDay.Ticks);
                bool crossDay = false;
                if(currDayStartTime>currDayEndTime)//跨天
                {
                    crossDay = true;
                    if(currTime<currDayEndTime)//已经跨天了
                    {
                        currDayStartTime = currDayStartTime.AddDays(-1);
                    }
                    else
                    {
                        currDayEndTime = currDayEndTime.AddDays(1);
                    }
                }
                
                if(currTime>currDayStartTime && currTime<currDayEndTime)//任何一个彩种，只要当前时间介于开始时间和结束时间之间，返回真，否者
                {
                    return true;
                }
            }
            return false;
        }

        string ReCalcTheInstChips()
        {
            string ret = null;
            return ret;
        }
        WebRule rule = null;
        private void Send_Data(DataTypePoint dtp,string currExpect,string strText)
        {
            try
            {
                string dtpName = dtp.DataType;
                //if (this.txt_Insts.Text.Trim().Length == 0) return;
                //string strText = dicExistInsts[dtpName].Last().Value;
                if (rule == null)
                {
                    rule = WebRuleBuilder.Create(Program.gc.InstFormat, Program.gc, Program.gc);
                    webBrowser1.ObjectForScripting = wr;
                }
                //if (!ScriptLoaded)
                AddScript(lotteryDocIE ?? HomeDocIE, lotteryDocFireFox ?? HomeDocFireFox);
                string msg = rule.IntsToJsonString(dtp,dtpName, strText, Program.gc.ChipUnit);
                double lastval = this.CurrVal;
                //SendMsg
                int maxCnt = 10;
                int sendCnt = 0;
                int sleepsec = 60;
                //while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                //{


                //    sendCnt++;

                //    Program.wxl.Log("警告", string.Format("[{1}]第{0}次浏览器加载未完成", sendCnt, Program.gc.ClientAliasName), string.Format("请检查线路{0}是否正常！", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost)));
                //    //LoadUrl();
                //    if (Program.gc.NeedAutoReset)//允许自动切换
                //        SwitchChanle();
                //    //Reboot();
                //    NewExpects[dtpName]--;//允许下次再接收数据发送
                //    RefreshTimes = 1;//让第一次发生错误后下次刷新指令会自动发送
                //    return;
                //    Application.DoEvents();

                //    Thread.Sleep(sleepsec * 1000);
                //    if (sendCnt > maxCnt)
                //    {
                //        LoadUrl();
                //        Program.wxl.Log("错误", "发送指令失败！", string.Format("连续{0}次未发送出下注指令！", sendCnt));
                //        return;
                //    }
                //}
                if (1 == 1)
                {
                    bool succ = sendInsts(currExpect,
                        msg,
                        dtpName,
                        strText, 
                        lastval, 
                        null, 
                        Program.gc.SendMsgFromWebRequest == "1");
                    if(!succ)
                    {
                        return;
                    }
                }
                else
                {
                    webBrowser1.Document.InvokeScript("SendMsg", new object[] {
                    this.NewExpects[dtpName].ToString().Substring(GlobalClass.TypeDataPoints[dtpName].ClientExpectCodeSubIndex),
                    msg,
                    Program.gc.ClientAliasName,
                    Program.gc.WXLogNoticeUser,
                    this.txt_Insts.Text,
                    lastval,
                    string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost),
                    rule.config.lotteryTypes[dtpName].ruleId
                });


                    //if (sender == null)
                    //{
                    //    this.statusStrip1.Text = "自动下注成功！";
                    //}

                    if (!dicExistInsts[dtpName].ContainsKey(this.NewExpects[dtpName]))
                    {
                        dicExistInsts[dtpName].Add(this.NewExpects[dtpName], strText);
                    }
                    this.toolStripStatusLabel1.Text = "已发送";
                }
                this.Cursor = Cursors.Default;
                //Application.DoEvents();
                //Thread.Sleep(5 * 1000);
                //webBrowser1.Refresh();
                //Application.DoEvents();
                //Thread.Sleep(3 * 1000);
                ////string ValTip = "无";
                ////if(CurrVal<=lastval)
                ////{
                ////    ValTip = "下注后金额无变化，您的下注可能在20秒内没被执行！请注意查看！";
                ////}
                //Program.wxl.Log(string.Format("[{0}]:{1};下注前金额:{2:f2};现可用余额:{3:f2}；提示:{4}",this.NewExpect, this.txt_Insts.Text, lastval,this.CurrVal,ValTip));
                ////if (e != null)
                ////{
                ////    MessageBox.Show(msg);
                ////}

                //if (sender != null)
                //{
                //    Thread.Sleep(2 * 1000);
                //    MessageBox.Show(string.Format("下注前金额:{0:f2};现可用余额:{1:f2}；提示:{2}", lastval, this.CurrVal, ValTip));
                //}
            }
            catch(Exception ce)
            {

            }

        }
        private void btn_Send_Click(object sender, EventArgs e)
        {
            if(sender!=null)
            {
                this.toolStripStatusLabel1.Text = "手动下注";
                this.Cursor = Cursors.WaitCursor;
            }
            
            if (this.txt_Insts.Text.Trim().Length == 0)
                return;
            string dtpName = null;
            //如果有对应的期号等于或者小于输入框内期号，对应的类型就是我们要找的类型
            var items = this.NewExpects.Where(a =>
            {
                if (a.Value.ToString() == this.txt_ExpectNo.Text || (a.Value + 1).ToString() == this.txt_ExpectNo.Text)
                    return true;
                return false;
            });
            if(items.Count()>0)
            {
                dtpName = items.First().Key;
            }
            if(dtpName == null)
            {
                dtpName = ddl_datatype.SelectedValue?.ToString();
            }
            if (dtpName == null)
            {
                MessageBox.Show("未获得对应的数据类型，可能是还没接收到任何数据！请手动选择要发送数据类型！");
                return;
            }
            DataTypePoint dtp = GlobalClass.TypeDataPoints[dtpName];
            if (rule==null)
                rule = WebRuleBuilder.Create(Program.gc.InstFormat, Program.gc, Program.gc);
            string strHost = string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost);
            string lotterUrl = string.Format(Program.gc.LotteryPage,strHost);
            try
            {
                
                if (isIE)
                {
                    //if (webBrowser1.Url.ToString() != lotterUrl)
                    //{
                    //    lock (webBrowser1)
                    //    {
                    //        webBrowser1.Navigate(lotterUrl);
                    //    }
                        
                    //}
                    //lotteryDocIE = webBrowser1.Document;
                }
                else
                {
                    if(geckoWebBrowser1.Url.ToString() != lotterUrl)
                    {
                        geckoWebBrowser1.Navigate(lotterUrl);
                    }
                    lotteryDocFireFox = geckoWebBrowser1.Document;
                }
                //rule = new Rule_ForKcaiCom(Program.gc);
                //if (!ScriptLoaded)
                if(!SendMsgFromWebRequest && !isInLotteryDocument())
                {
                    return;
                }
                AddScript(lotteryDocIE??HomeDocIE, lotteryDocFireFox ?? HomeDocFireFox);

                string msg = null;
                try
                {
                    msg = rule.IntsToJsonString(dtp,dtpName, this.txt_Insts.Text, Program.gc.ChipUnit);
                }
                catch (Exception ce)
                {
                    return;
                }
                double lastval = this.CurrVal;
                //SendMsg
                int maxCnt = 10;
                int sendCnt = 0;
                int sleepsec = 60;
                ////while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                ////{


                ////    sendCnt++;

                ////    Program.wxl.Log("警告", string.Format("[{1}]第{0}次浏览器加载未完成", sendCnt,Program.gc.ClientAliasName), string.Format("请检查线路{0}是否正常！", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost)));
                ////    //LoadUrl();
                ////    if (Program.gc.NeedAutoReset)//允许自动切换
                ////        SwitchChanle();
                ////    //Reboot();
                ////    NewExpects[NewExpects.First().Key] --;//允许下次再接收数据发送
                ////    RefreshTimes = 1;//让第一次发生错误后下次刷新指令会自动发送
                ////    return;
                ////    Application.DoEvents();

                ////    Thread.Sleep(sleepsec * 1000);
                ////    if (sendCnt > maxCnt)
                ////    {
                ////        LoadUrl();
                ////        Program.wxl.Log("错误", "发送指令失败！", string.Format("连续{0}次未发送出下注指令！", sendCnt));
                ////        return;
                ////    }
                ////}
                if (1 == 1)
                {
                    if(dicExistInsts.ContainsKey(dtpName) && dicExistInsts[dtpName].ContainsKey(this.NewExpects[dtpName]))
                    {
                        if(MessageBox.Show("继续发送指令?","当期指令已发送",MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    bool succ = sendInsts(this.txt_ExpectNo.Text.Trim(),msg, dtpName, this.txt_Insts.Text, lastval, sender,Program.gc.SendMsgFromWebRequest=="1");
                    if(!succ)
                    {
                        return;
                    }
                }
                else
                {
                    try
                    {
                        string ruleid = "0";
                        if (rule.config != null)
                            ruleid = rule.config.lotteryTypes.ContainsKey(dtpName) ? rule.config.lotteryTypes[dtpName].ruleId : "0";
                        if (isIE)
                        {
                            (lotteryDocIE ?? HomeDocIE).InvokeScript("SendMsg",
                                new object[]
                                {
                    this.txt_ExpectNo.Text.Substring(GlobalClass.TypeDataPoints.First().Value.ClientExpectCodeSubIndex),
                    msg,
                    Program.gc.ClientAliasName,
                    Program.gc.WXLogNoticeUser,
                    this.txt_Insts.Text,
                    lastval,
                    string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost),
                    int.Parse(ruleid)
                                    });
                        }
                        else
                        {
                            AutoJSContext context = new AutoJSContext(geckoWebBrowser1.Window);
                            string jstxt = string.Format("SendMsg('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7});",
                                this.NewExpects.First().Value.ToString().Substring(GlobalClass.TypeDataPoints.First().Value.ClientExpectCodeSubIndex),
                                msg,
                                Program.gc.ClientAliasName,
                                Program.gc.WXLogNoticeUser,
                                this.txt_Insts.Text,
                                lastval,
                                string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost),
                                int.Parse(ruleid));
                            context.EvaluateScript(jstxt);
                        }
                    }
                    catch (Exception ce)
                    {
                        MessageBox.Show(string.Format("{0}:{1}", ce, ce.StackTrace));
                        return;
                    }
                }
                if (sender == null)
                {
                    this.statusStrip1.Text = "自动下注成功！";
                }
                else
                {
                    //MessageBox.Show("手动下注成功！");
                    this.statusStrip1.Text = "手动下注成功!";
                }
                //if (!dicExistInsts.First().Value.ContainsKey(this.NewExpects.First().Value))
                //{
                //    dicExistInsts.First().Value.Add(this.NewExpects.First().Value, this.txt_Insts.Text);
                //}
            }
            catch (Exception ce)
            {
                this.toolStripStatusLabel1.Text = ce.Message;
                this.toolStripStatusLabel2.Text = ce.StackTrace;
                return;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            this.toolStripStatusLabel1.Text = "已发送";
            
            //Application.DoEvents();
            //Thread.Sleep(5 * 1000);
            //webBrowser1.Refresh();
            //Application.DoEvents();
            //Thread.Sleep(3 * 1000);
            ////string ValTip = "无";
            ////if(CurrVal<=lastval)
            ////{
            ////    ValTip = "下注后金额无变化，您的下注可能在20秒内没被执行！请注意查看！";
            ////}
            //Program.wxl.Log(string.Format("[{0}]:{1};下注前金额:{2:f2};现可用余额:{3:f2}；提示:{4}",this.NewExpect, this.txt_Insts.Text, lastval,this.CurrVal,ValTip));
            ////if (e != null)
            ////{
            ////    MessageBox.Show(msg);
            ////}

            //if (sender != null)
            //{
            //    Thread.Sleep(2 * 1000);
            //    MessageBox.Show(string.Format("下注前金额:{0:f2};现可用余额:{1:f2}；提示:{2}", lastval, this.CurrVal, ValTip));
            //}
        }
        bool succprocss(string json)
        {
            return true;
        }


        int getInterVal(DataTypePoint dtp,bool receivedData)
        {
            long RecSecs = dtp.ReceiveSeconds;
            int noRecVal = (int)RecSecs / 10;
            int rdm = new Random().Next(noRecVal);
            if (!receivedData)
            {
                return noRecVal*1000;
            }
            if(DateTime.Now.Subtract(LastInstsTime).Seconds > 3*RecSecs/2)//如果超过了3/2，就算是接收到了数据，也要以未接收的频率接收本期数据
            {
                return noRecVal*1000;
            }
            return 1000*(8* noRecVal + rdm); 
            
        }
       

        
        bool sendInsts(string expectNo,string EncodingMsg,string dtpName,string orgMsg,double lastval,object sender,bool sendFromWebRequest)
        {
            string ruleid = "0";
            if (rule.config != null)
                ruleid = rule.config.lotteryTypes.ContainsKey(dtpName) ? rule.config.lotteryTypes[dtpName].ruleId : "0";
            //if (sendFromWebRequest && sender== null)
            //{
            //    Program.wxl.Log(string.Format("[{0}]第[{1}]期投注",dtpName,expectNo), orgMsg , string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));

            //    return true;
            //    //ce.SetCo/*o*/kie(getCookie(this.webBrowser1.Document.Cookie));
            //    bool succ = ce.sendInsts(expectNo,EncodingMsg, dtpName, orgMsg, lastval, ruleid,sender,succprocss,(a,b)=>{
            //        if(sender!=null)
            //        {
            //            MessageBox.Show(string.Format("{0}:{1}", a, b));
            //        }
            //    });
            //    return succ;
            //}
            bool needLeftFill0 = false;
            DataTypePoint dtp =  GlobalClass.TypeDataPoints[dtpName];
            string sendExpect = expectNo;
            if (dtp.ClientSerLen >0)
            {
                int len = expectNo.Length;

                if(len<dtp.ExpectCodeDateLong+dtp.ClientSerLen)
                {
                    string strDate = expectNo.Substring(0, dtp.ExpectCodeDateLong);
                    string strSer = expectNo.Substring(dtp.ExpectCodeDateLong);
                    sendExpect = strDate + strSer.PadLeft(dtp.ClientSerLen, '0');
                    needLeftFill0 = true;
                }
            }
            sendExpect = sendExpect.Substring(GlobalClass.TypeDataPoints[dtpName].ClientExpectCodeSubIndex);
            string msg = EncodingMsg;
            try
            {
                
                if (isIE)
                {
                    (lotteryDocIE ?? HomeDocIE).InvokeScript("SendMsg",
                        new object[]
                        {
                        sendExpect
                    ,
                    msg,
                    Program.gc.ClientAliasName,
                    Program.gc.WXLogNoticeUser,
                    orgMsg,
                    lastval,
                    string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost),
                    int.Parse(ruleid),
                    string.Format(Program.gc.LoginUrlModel,Program.gc.LoginDefaultHost),
                    dtpName,
                    expectNo,
                    dtp.ClientNoNeedZero
                            });
                }
                else
                {
                    AutoJSContext context = new AutoJSContext(geckoWebBrowser1.Window);
                    string jstxt = string.Format("SendMsg('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7});",
                        sendExpect,
                        msg,
                        Program.gc.ClientAliasName,
                        Program.gc.WXLogNoticeUser,
                        orgMsg,
                        lastval,
                        string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost),
                        int.Parse(ruleid),
                        dtpName);
                    context.EvaluateScript(jstxt);
                }
            }
            catch (Exception ce)
            {
                if(sender != null)
                    MessageBox.Show(string.Format("{0}:{1}", ce, ce.StackTrace));
                return false;
            }
            return true;
        }

        private void mnu_SetAssetUnitCnt_Click(object sender, EventArgs e)
        {
            if(AppMode)
            {
                MessageBox.Show("App模式无法自由设置自己的资产单元的风险额度，如确有需求，请联系管理员。");
                return;
            }
            frm_setting frm = new frm_setting(AssetUnitList);
            frm.ShowDialog();
        }

        private void mnu_RefreshInsts_Click(object sender, EventArgs e)
        {
            timers_RequestInst.Values.ToList().ForEach(a => {
                timer_RequestInst_Tick(a, null);
            });
            
        }


        private void mnuRefreshWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(isIE)
            {
                loadUrl = this.webBrowser1.Url.ToString();
            }
            else
            {
                loadUrl = this.geckoWebBrowser1.Url.ToString();
            }
            reLoadWebBrowser();
            //loadUrl = string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost);
        }

        void reLoadWebBrowser()
        {
            ReadySleep = true;
            string url = "https://www.baidu.com";
            try
            {
                lock(webBrowser1)
                    this.webBrowser1.Navigate(url);
                if ((webBrowser1.ReadyState == WebBrowserReadyState.Complete && isIE)||(geckoWebBrowser1.StatusText=="completed" && isIE==false))
                {
                    //this.webBrowser1.Navigate(url);
                }
                else
                {
                    WXMsgBox(string.Format("[{0}]程序们，再坚持一会",Program.gc.ClientAliasName), "让其他屌丝们干完事了一起睡！");

                    //ReadySleep = false;
                    //MySleep(2 * 60 * 1000);
                    //return;
                }
            }
            catch (Exception ce)
            {
                ReadySleep = false;
                MySleep(2 * 60 * 1000);
                LogableClass.ToLog("错误", "此刻无法入眠", string.Format("{0}:{1}", ce.Message, ce.StackTrace));
                Program.wxl.Log(string.Format("[{0}]错误",Program.gc.ClientAliasName), "此刻无法入眠，让我再酝酿一番如何？", string.Format("{0}:{1}", ce.Message, ce.StackTrace), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                return;
            }
            WebBrowserLoad = false;
            Logined = false;
            InSleep = true;
            Set_SendTime(false);
            Program.wxl.Log(string.Format("[{0}]洗洗睡吧，明日再薅！",Program.gc.ClientAliasName), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
        }

        private void reLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(loadUrl == "about:blank")
            {
                loadUrl = null;
            }
            LoadUrl(loadUrl);
        }

        void RefreshStatus(int interval = 0)
        {
            if (this.timers_RequestInst.Count == 0)
                return;
            this.toolStripStatusLabel1.Text = string.Format("已加载页面:{0};已登录:{1};睡眠状态:{2}", WebBrowserLoad, Logined, InSleep);
            this.toolStripStatusLabel2.Text = string.Format("当前余额：{0}", CurrVal);
            if(interval>0)
                this.toolStripStatusLabel3.Text = string.Format("{1}:{0}秒后见", DateTime.Now.ToString(),interval / 1000);
            //SendStatusTimer_Elapsed(null, null);
        }

        void RefreshSendStatusInfo(string msg)
        {
            toolStripStatusLabel3.Text = msg;
            toolStripStatusLabel3.Enabled = true;
            toolStripStatusLabel3.Visible = true;
        }

        void RefreshImages()
        {
            try
            {
                this.pic_serImage.Load(getImageUrl(string.Format("lv_0.jpg?t={0}", Guid.NewGuid().ToString())));
                Application.DoEvents();
                this.pic_carImage.Load(getImageUrl(string.Format("lv_1.jpg?t={0}", Guid.NewGuid().ToString())));
                Application.DoEvents();
                this.pic_ChanceImage.Load(getImageUrl(string.Format("chances.jpg?t={0}", Guid.NewGuid().ToString())));
                Application.DoEvents();
                this.pic_chartImage.Load(getImageUrl(string.Format("chart.png?t={0}", Guid.NewGuid().ToString())));
                Application.DoEvents();
            }
            catch
            {

            }
        }
        
        string getImageUrl(string name)
        {
            return string.Format("{0}/chartImgs/{1}",GlobalClass.TypeDataPoints.First().Value.InstHost,name);
        }

        private void TSMI_sendStatusInfor_Click(object sender, EventArgs e)
        {
            this.SendStatusTimer_Elapsed(null, null);
            RefreshStatus();
        }

        private void btn_TestJscript_Click(object sender, EventArgs e)
        {
            string[] arg = this.txt_NewInsts.Lines;

            //SendMsg
            string[] useArg = new string[arg.Length - 1];
            Array.Copy(arg, 1, useArg, 0, useArg.Length);
            try
            {
                //if (!ScriptLoaded)
                AddScript(lotteryDocIE ?? HomeDocIE, lotteryDocFireFox ?? HomeDocFireFox);
                //AddScript();
                webBrowser1.Document.InvokeScript("JumpFillPage", null);
                MySleep(3000);
                //AddScript();
                webBrowser1.Document.InvokeScript(arg[0], useArg);
            }
            catch (Exception ce)
            {
                MessageBox.Show(e.ToString());
            }
        }

        void KnockEgg()
        {
            //if (!ScriptLoaded)
            AddScript(HomeDocIE, HomeDocFireFox);
            webBrowser1.Document.InvokeScript("ClickEgg", new object[] { Program.gc.ClientUserName, Program.gc.WXLogNoticeUser, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost) });
            //Program.wxl.Log("已为您砸金蛋！");
        }

        private void tsmi_knockTheEgg_Click(object sender, EventArgs e)
        {
            KnockEgg();
        }

        private void switchChanleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchChanle();
        }

        private void btn_SelfAddCombo_Click(object sender, EventArgs e)
        {
            ce.Load(txt_NewInsts.Text.Trim());
            
            //LoadUrl(null);
        }

        private void chargeMoneyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost);
            reqChargeAmt = "4499";
            string fullurl = string.Format("{0}//Recharge/ThirdRecharge?amount={1}&t={2}",url, reqChargeAmt, 139);
            webBrowser_charge.DocumentCompleted += WebBrowser_charge_DocumentCompleted;
            chargeData = null;
            Task.Run(() => {
                checkCharge();
            });
            webBrowser_charge.Navigate(fullurl);
            
            //webBrowser_charge.Dock = DockStyle.Fill;

        }
        void checkCharge()
        {
            WebData currData = new WebData();
            string ret = null;
            try
            {
                DateTime startTime = DateTime.Now;
                int TimeOut = 90;
                Application.DoEvents();
                while (chargeData == null)//事件没有改变前一直跑 
                {
                    if (DateTime.Now.Subtract(startTime).TotalSeconds >= TimeOut)
                    {
                        break;
                    }
                    MySleep(100);
                    if (chargeData != null)
                    {
                        break;
                    }
                }
                if(chargeData == null)
                {
                    ret = string.Format("errcode={0}&msg={1}&orderNum={2}&chargeAmt={3}&imgData={4}", 4005, currData.err_msg ?? "该服务器账号未正确配置,请联系管理员", currData.orderNum ?? "未知", currData.chargeAmt ?? "无", currData.ImgData ?? "");
                    Program.wxl.Log("超时:" + ret, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                    chgOpt.ResponseCompleted(ret);
                    Program.wxl.Log("已回送服务器！", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                    return;
                }
                startTime = DateTime.Now;
                
                DateTime sTime = DateTime.Now;
                currData.ImgData = chargeData.ImgData;
                currData.orderNum = chargeData.orderNum;
                currData.chargeAmt = chargeData.chargeAmt;
                currData.err_msg = chargeData.err_msg;
                Program.wxl.Log(string.Format("获取到第一个chargeData,订单:{0},金额:{1};",currData.orderNum,currData.chargeAmt), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                if (string.IsNullOrEmpty(currData.err_msg))
                {
                    //currData.err_msg = null;
                }
                currData.LoadCnt = 1;
                chargeData = null;
                int sTimeOut = 15;//15s未更新就是最后的
                while (true)
                {
                    Application.DoEvents();
                    if (chargeData != null)
                    {
                        currData.ImgData = chargeData.ImgData;
                        currData.orderNum = chargeData.orderNum;
                        currData.chargeAmt = chargeData.chargeAmt;
                        currData.err_msg = chargeData.err_msg;
                        currData.LoadCnt++;
                        chargeData = null;
                        sTime = DateTime.Now;
                        //Program.wxl.Log(string.Format("获取到第{0}个chargeData", currData.LoadCnt + 1), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                    }
                    if (DateTime.Now.Subtract(sTime).TotalSeconds > sTimeOut)//超时，返回最后的数据
                    {
                        chargeData = null;
                        if (!string.IsNullOrEmpty(currData.ImgData))
                        {
                            //"reqId={0}&chargeAmt={1}&errcode={2}&msg={3}&imgData={4}&orderNum={5}";
                            string allstr = string.Format(const_ResponseModel,
                                UseReqId,
                                currData.chargeAmt,
                                null,
                                currData.err_msg,
                                currData.ImgData,
                                currData.orderNum,
                                Program.gc.ClientUserName
                                );
                            chgOpt.ResponseCompleted(allstr);
                            Program.wxl.Log(string.Format("充值页面信息,此信息已返送回Web服务器处理！{0}", allstr.Substring(0, Math.Min(100, allstr.Length - 1))), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));

                            //Program.wxl.Log("已回送服务器！", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(currData.err_msg) || string.IsNullOrEmpty(currData.orderNum) || string.IsNullOrEmpty(currData.chargeAmt) || string.IsNullOrEmpty(currData.ImgData))
                            {

                                ret = string.Format("errcode={0}&msg={1}&orderNum={2}&chargeAmt={3}&imgData={4}",
                                    4001,
                                    currData.err_msg ?? "该服务器账号未正确配置,请联系管理员",
                                    currData.orderNum ?? "未知",
                                    currData.chargeAmt ?? chgOpt.strAmt,
                                    currData.ImgData);
                                chgOpt.ResponseCompleted(ret);
                                Program.wxl.Log(string.Format("充值错误:{0}", ret.Substring(0, Math.Min(100, ret.Length - 1))), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));

                                //Program.wxl.Log("已回送服务器！", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                            }
                        }

                        return;
                    }
                    MySleep(100);

                }
            }
            catch(Exception ce)
            {
                ret = string.Format("errcode={0}&msg={1}&orderNum={2}&chargeAmt={3}&imgData={4}", 
                    4009, 
                    currData.err_msg ?? ce.Message, 
                    currData.orderNum ?? "未知", 
                    currData.chargeAmt ?? chgOpt.strAmt, 
                    currData.ImgData ?? "");
                Program.wxl.Log("意外错误错误:" + ce.Message, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                chgOpt.ResponseCompleted(ret);
                //Program.wxl.Log("已回送服务器！", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
            }
        }

        private void WebBrowser_charge_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ////AddScript(); //执行js代码,未来修改为网络载入
            ////webBrowser_charge.Document.InvokeScript("getQCode", new string[] { Program.gc.ClientUserName, Program.gc.ClientPassword });
            ////HtmlDocument cdoc = webBrowser_charge.Document;
            ////HTMLDocument hdoc =  cdoc.DomDocument as HTMLDocument;
            ////IHTMLElementCollection hcols = hdoc.getElementsByTagName("canvas");
            string ret = null;
            string strUrl = null;
            string chargeId = null;
            string chargeAmt = null;
            string err_msg = null;
            
            if (webBrowser_charge.ReadyState == WebBrowserReadyState.Complete)
            {
                if (string.IsNullOrEmpty(chgOpt.strAmt))
                    chgOpt.strAmt = reqChargeAmt;
                Program.wxl.Log("充值页面加载完成！", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                HtmlDocument doc = webBrowser_charge.Document;
                try
                {
                    strUrl = wr.getChargeQCode(doc);
                    if(strUrl == null)
                    {
                        Program.wxl.Log(string.Format("充值网页文档为空！"), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                        return;
                    }
                    if(string.IsNullOrEmpty(strUrl))
                    {
                        Program.wxl.Log(string.Format("无法找到充值网页的二维码画板！"), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                        return;
                    }
                    string strOrderNum = wr.getChargeNum(doc);
                    string strChargeAmt = wr.getChargeAmt(doc);
                    chargeId = strOrderNum?.Split('：')[1];
                    chargeAmt = strChargeAmt?.Split('：')[1].Trim();
                    err_msg = wr.getErr_Msg(doc);
                    Program.wxl.Log(string.Format("请求金额[{3}]元,获取到充值网页信息！订单号:{0};金额:[{1}];消息:{2}", chargeId, chargeAmt, err_msg, chgOpt.strAmt), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                    MySleep(200);
                    double chargeAmtInt = 0.0;
                    int inputAmtInt = 0;
                    bool isInt = double.TryParse(chargeAmt??"0.0",out chargeAmtInt);
                    bool inIsInt = int.TryParse(chgOpt.strAmt, out inputAmtInt);
                    if (!isInt ||!inIsInt || ((int)chargeAmtInt) != inputAmtInt )//不是对应的这个金额，跳过！
                    {
                        if(chargeAmt == null)
                        {
                            if(chargeData == null)
                            {
                                chargeData = new WebData();
                                chargeData.err_msg = "账号暂停，请改期再刷！";
                            }
                            Program.wxl.Log(string.Format("充值网页已关闭！"), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));

                            return;
                        }
                        Program.wxl.Log(string.Format("请求金额{0}与充值金额{1}不一致！", chargeAmt, chgOpt.strAmt), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                        return;
                    }
                    else
                    {
                        err_msg = null;
                    }
                    if (chargeData == null)
                    {
                        chargeData = new WebData();
                        chargeData.ImgData = strUrl;
                        chargeData.orderNum = chargeId;
                        chargeData.chargeAmt = chargeAmt;
                        chargeData.err_msg = err_msg??"请求金额不一致！";
                    }
                    //Program.wxl.Log(string.Format("返回chargeData"), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                    return;
                }
                catch(Exception ce)
                {
                    if (chargeData == null)
                    {
                        chargeData = new WebData();
                        chargeData.ImgData = strUrl;
                        chargeData.orderNum = chargeId;
                        chargeData.chargeAmt = chargeAmt?? chgOpt.strAmt;
                        chargeData.err_msg = "载入页面错误："+ ce.Message;
                    }
                    Program.wxl.Log(string.Format("载入页面错误[{0}]:{1}",ce.Message,ce.StackTrace), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                    return;
                }
               
            }
            //WebBrowserLoad = true;

        }

        

        private void hideTheFloatWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.webBrowser_charge.Visible = !this.webBrowser_charge.Visible;
        }

        private void loadTheNavigateWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser_nav.ScriptErrorsSuppressed = true;
            webBrowser_nav.Navigate(Program.gc.NavHost);
        }

        private void userLoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void inLotteryHomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadUrl = string.Format(Program.gc.LotteryPage, string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost));
            LoadLotteryHome();
            loadUrl = string.Format(Program.gc.LotteryPage,string.Format(Program.gc.LoginUrlModel, Program.gc.LoginDefaultHost));
        }


        void LoadLotteryHome()
        {
            if(isIE)
            {
                AddScript(HomeDocIE, null);
                HomeDocIE.InvokeScript("gotoLotteryHome",new object[0] { });
            }
            else
            {
                AddScript(HomeDocIE, HomeDocFireFox);
                
            }
        }

        private void switchWebBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isIE = (!isIE);
            MainWindow_Load(null, null);
        }

        void testLMAZ()
        {
            
        }

        private void switchPlatformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string forWeb = (sender as ToolStripMenuItem).Tag.ToString();
            GlobalClass sgc = new GlobalClass(forWeb);
            if(!sgc.loadSucc)
            {
                return;
            }
            Program.gc = sgc;
            reLoad();
            MainWindow_Load(null, null);
        }

        private void refreshBetRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getBetRecInfo();
        }

        private void cancelBetRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            try
            {
                DataGridView dg = (menu.Owner as ContextMenuStrip).SourceControl as DataGridView;
                if (dg.SelectedRows.Count <= 0)
                    return;
                int index = dg.SelectedRows[0].Index;
                ObjectTable<BetRecordListClass> bdt = dg.Tag as ObjectTable < BetRecordListClass > ;
                if (bdt == null)
                    return;
                if (bdt.dt.Rows.Count < index + 1)
                {
                    MessageBox.Show("请选择有效记录撤销！");
                    return;
                }
                if(MessageBox.Show("确定撤销选择的记录?","撤销",MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                BetRecordClass rec = bdt.otherObject.Data[index];
                object[] objs =  wr.getBetKeys(this.webBrowser1.Document.Cookie,this.webBrowser1.Document.Body.OuterHtml, rec);
                this.cancelBetRec(objs);
            }
            catch (Exception ce)
            {
                string msg = ce.Message;
            }
        }
         
        private void refreshBetRecToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            getBetRecInfo();
        }

        private void setLoginedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setLogined();
        }
        public static void WXMsgBox(string title, string msg)
        {
            //MessageBox.Show(string.Format("{0}:{1}",title,msg));
            //return;
            Program.wxl.Log(title, msg, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
        }
        public static void WXMsgBox(DataTable dt,string XVal,String YVal)
        {
            string msg = getImage64String(dt,XVal,YVal);
            if(msg == null)
            {
                return;
            }
            Program.wxl.LogImage(msg, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
        }

        public static string getImage64String(DataTable data,string XVal,string YVal)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                Chart chart = chart1;// new Chart();
                chart.Dock = DockStyle.Fill;
                //chart.Width = 800;
                //chart.Height = 600;
                chart.Series.Clear();
                Series sr = new Series(data.TableName);
                sr.ChartType = SeriesChartType.Line;
                
                DataView dt = new DataView(data);
                //chart.DataSource = dt;
                sr.Points.DataBindXY(dt, XVal,dt,YVal);
                chart.Series.Add(sr);
                //sr.Points.DataBindY(dt, YVal);
                chart.Visible = true;
                chart.Show();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                
                byte[] arr = new byte[ms.Length];
                ms.Read(arr, 0, (int)ms.Length);                
                string ret = Convert.ToBase64String(arr);

                return ret;
            }
            catch
            {

            }
            finally
            {
                ms.Close();
            }

            return null;
        }


        /// <summary>
        /// 将图片转换成base64字符串。
        /// </summary>
        /// <param name="imagefile">需要转换的图片文件。</param>
        /// <returns>base64字符串。</returns>
        public string GetBase64FromImage(string imagefile)
        {
            string strbaser64 = "";
            
            try
            {
                Bitmap bmp = new Bitmap(imagefile);
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();

                    strbaser64 = Convert.ToBase64String(arr);
                }
            }
            catch (Exception)
            {
                throw new Exception("Something wrong during convert!");
            }

            return strbaser64;
        }
    }


    public class DataStatusInfoClass
    {
        public DataTypePoint dtp;
        public Dictionary<string, ExpectDataInfo> EachExpectDataInfo;
        public DataStatusInfoClass(DataTypePoint _dtp, RequestClass rc)
        {
            dtp = _dtp;

        }
    }

    

    public class ObjectTable<T>
    {
        public DataTable dt;
        public T otherObject;
    }

    public class MyTimer:System.Windows.Forms.Timer
    {
        public DataTypePoint dtp;
        public int index = 0;
    }
    public class FireFoxBrowser:GeckoWebBrowser
    {
        void Invode(string script)
        {
            //
            

            
        }
    }


    

    
}

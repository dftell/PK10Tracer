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
        double CurrVal = 0;
        public MainWindow()
        {
            InitializeComponent();
            wr = WebRuleBuilder.Create(Program.gc);
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
                
                
            }
            catch(Exception ce)
            {
                LogableClass.ToLog("错误", ce.Message, ce.StackTrace);
            }
            this.timer_RequestInst.Interval = 10 * 1000;
            this.timer_RequestInst.Enabled = true;
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
            if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
            {
                if (ReadySleep)
                {
                    LogableClass.ToLog("进入睡眠！");
                    ReadySleep = false;
                    return;
                }
                bool IsVaildWeb = wr.IsVaildWeb(doc);
                if (!WebBrowserLoad)//第一次载入
                {
                    if (!IsVaildWeb)//网页不可用
                    {
                        LogableClass.ToLog("网页载入后但未出现预期内容！");
                        return;
                    }
                    AddScript(); //执行js代码,未来修改为网络载入
                    webBrowser1.Document.InvokeScript("Login", new string[] { Program.gc.ClientUserName, Program.gc.ClientPassword });
                    WebBrowserLoad = true;
                    
                }
                else
                {
                    //其他网页
                    LogableClass.ToLog("睡眠综合症！");
                    return;
                }
            }
            else
            {
                if (!WebBrowserLoad)//中间状态，不理睬
                {
                    return;
                }
                if(Logined)
                {
                    //网页中间刷新
                    CurrVal = wr.GetCurrMoney(doc);

                }
                //加载自定义脚本后的处理
                while(true)
                {
                    if(wr.IsLogined(doc))
                        break;
                    Thread.Sleep(10 * 1000);
                }
                Logined = true;
                this.timer_RequestInst_Tick(null, null);
            }
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
                throw e;
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
            if (InSleep)//还在睡觉,唤醒
            {
                LoadUrl();
                ////while(!Logined)
                ////{
                ////    System.Threading.Thread.Sleep(30*1000);
                ////}
                InSleep = false;
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
                this.timer_RequestInst.Interval = 5 * 60 * 1000 -1000;//5分钟以后见,减掉1秒不*断收敛时间，防止延迟接收
                //ToAdd:填充各内容
                this.txt_ExpectNo.Text = ic.Expect;
                this.txt_OpenTime.Text = ic.LastTime;
                this.txt_Insts.Text = ic.getUserInsts(Program.gc);
                this.NewExpect = CurrExpectNo;
                if (Logined)
                {
                    if(RefreshTimes>0)
                        this.btn_Send_Click(null, null);
                    RefreshTimes = 1;
                }
                
            }
            else
            {
                if (CurrTime.Hour < 9)//如果在9点前
                {
                    //下一个时间点是9：08
                    DateTime TargetTime = DateTime.Today.AddHours(9).AddMinutes(30);
                    this.timer_RequestInst.Interval = TargetTime.Subtract(CurrTime).TotalMilliseconds;
                    reLoadWebBrowser();//开百度网页
                }
                else
                {
                    
                    if (CurrTime.Subtract(LastInstsTime).Minutes > 7)//如果离上期时间超过7分钟，说明数据未接收到，那不要再频繁以10秒访问服务器
                    {
                        this.timer_RequestInst.Interval = 4 * 60 * 1000;
                    }
                    else //一般未接收到，10秒以后再试
                    {
                        this.timer_RequestInst.Interval = 4 * 60 * 1000;
                    }
                }
            }
            
            RefreshStatus();
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
            //SendMsg
            webBrowser1.Document.InvokeScript("SendMsg", new string[] { this.NewExpect.ToString(), msg });
            ////if (e != null)
            ////{
            ////    MessageBox.Show(msg);
            ////}
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
        }
    }
}

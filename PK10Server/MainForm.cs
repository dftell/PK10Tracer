using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.ExchangeLib;
using WolfInv.com.PK10CorePress;
using WolfInv.com.SecurityLib;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using System.Drawing.Imaging;

namespace PK10Server
{
    
    public partial class MainForm<T>:Form where T:TimeSerialData
    {
        
        DataTypePoint dtp = null;
        bool isSecurity = false;
        ExpectList<T> _ViewDataList ;
        ExpectList<T> ViewDataList
        {
            get { return _ViewDataList; }
            set { _ViewDataList = value; }
        }
        //PK10ExpectReader er = new PK10ExpectReader();
        DataReader<T> er = null;
        long NewestExpectNo = 0;
        public long InputExpect;
        GlobalClass gobj;
        public MainForm():base()
        {
            
                InitializeComponent();
            
            gobj = new GlobalClass();
            dtp = GlobalClass.TypeDataPoints.First().Value;
            isSecurity = dtp.IsSecurityData == 1;
            er = DataReaderBuild.CreateReader<T>(dtp.DataType, null, null);
            this.Text = string.Format("{0}量化投资系统服务端", GlobalClass.DataTypes[dtp.DataType]);
            if(Program<T>.optFunc == null)
            {
                Program<T>.optFunc = new operateClass();
            }
            Program<T>.optFunc.RefreshMainWindow += refreshData;
        }


        void refreshData()
        {
            //Program<T>.AllGlobalSetting.wxlog.Log("退出服务", "意外停止服务", string.Format(Program<T>.gc.WXLogUrl, Program<T>.gc.WXSVRHost));
            timer_For_NewestData_Tick(null, null);
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            InitListView(listView_forSerial, "排");
            InitListView(listView_ForCar, "列");

            InitListView(listView_TXFFCData);
            InitListView(listView_PK10Data);
            
            this.timer_For_NewestData.Interval = 5*1000;
            //this.timer_For_NewestData_Tick(null, null);

            this.timer_For_CurrTime.Enabled = true;
            this.timer_For_CurrTime.Interval = 1000;
            this.timer_For_CurrTime_Tick(null, null);

            this.timer_For_getHtmlData.Enabled = true;
            this.timer_For_getHtmlData.Interval = 60000;
            this.timer_For_getHtmlData_Tick(null, null);
            ReLoad();
            ////ViewDataList = er.ReadNewestData(DateTime.Now.AddDays(-1));
            ////RefreshGrid();
            RefreshNewestData();
            this.wb_DSMonitor.ScriptErrorsSuppressed = true;
            //this.wb_DSMonitor.Url = new Uri(GlobalClass.TypeDataPoints[GlobalClass.TypeDataPoints.First().Key].RuntimeInfo.DefaultDataUrl);
            
        }

        public void ReLoad()
        {
            if (InputExpect > 0)
            {
                this.txt_NewestExpect.Text = (this.InputExpect - 1).ToString();
                this.btn_AddExpectNo_Click(null, null);
            }
            else
            {
                ViewDataList = er.ReadNewestData(DateTime.Today.AddDays(-1* dtp.CheckNewestDataDays));
            }
        }

        void StartReceiveDataService()
        {
            //this.timer_For_getHtmlData.Enabled = true;
            //this.timer_For_getHtmlData_Tick(null, null);
        }
        void StopReceiveService()
        {
            //this.timer_For_getHtmlData.Enabled = false;
        }

        void StartRefreshDataService()
        {
            this.timer_For_NewestData.Enabled = true;
            this.timer_For_NewestData_Tick(null, null);
        }

        void EndRefreshDataService()
        {
            this.timer_For_NewestData.Enabled = false;
        }

        public void RefreshGrid()
        {
            RefreshSerialData(this.listView_forSerial, true,this.gobj.MutliColMinTimes);
            RefreshSerialData(this.listView_ForCar, false, this.gobj.MutliColMinTimes);
        }


        void RefreshSerialData(ListView lv,bool byNo,int minRow)
        {
            try
            {


                ExpectList<T> el = ViewDataList; ;

                ExpectListProcessBuilder<T> elp = new ExpectListProcessBuilder<T>(dtp, el);
                BaseCollection<T> sc = elp.getProcess()?.getSerialData(180, byNo);
                if(sc == null || sc.Table == null)
                {
                    return;
                }
                sc.isByNo = byNo;
                lv.Items.Clear();
                for (int i = minRow - 1; i < sc.Table.Rows.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = (i - minRow + 1 + 1).ToString();
                    if (byNo)
                    {
                        for (int j = 0; j < sc.Table.Columns.Count; j++)
                        {
                            lvi.SubItems.Add(sc.Table.Rows[i][j].ToString());
                        }
                    }
                    else
                    {
                        for (int j = 0; j < sc.Table.Columns.Count; j++)
                        {
                            if (dtp.DataType == "PK10" || dtp.DataType == "XYFT")
                            {
                                lvi.SubItems.Add(sc.Table.Rows[i][string.Format("{0}", (j + 1) % 10)].ToString());
                            }
                            else
                            {
                                string colname = (j + 1).ToString().PadLeft(2, '0');
                                if (sc.Table.Columns.Contains(colname))
                                {
                                    lvi.SubItems.Add(sc.Table.Rows[i][colname].ToString());
                                }
                                else
                                {
                                    lvi.SubItems.Add(sc.Table.Rows[i]["0"].ToString());
                                }
                            }
                        }
                    }
                    for (int j = 1; j < sc.Table.Columns.Count; j++)
                    {
                        if (i + 1 == this.gobj.MinTimeForChance(j))
                        {
                            lvi.BackColor = Color.Yellow;
                            break;
                        }
                    }
                    lv.Items.Add(lvi);
                }
                string GR_Path = string.Format("lv_{0}.jpg", byNo ? 0 : 1);
                imageLogClass<T>.SaveImage(lv,GR_Path,ImageFormat.Jpeg);
                /*
                Bitmap image = new Bitmap(lv.Width, lv.Height);//初始化一个相同大小的窗口
                lv.DrawToBitmap(image, new Rectangle(0, 0, lv.Width, lv.Height));
                
                string fullFileName = string.Format("{0}\\imgs\\{1}", AppDomain.CurrentDomain.BaseDirectory, GR_Path);// GR_Path + "\\" + fileName + ".png";
                image.Save(fullFileName,System.Drawing.Imaging.ImageFormat.Jpeg);
                    string filename = string.Format("chartImg");
                Program<T>.wxlog.LogImageUrl(string.Format("{0}/chartImgs/{1}", GlobalClass.TypeDataPoints.First().Value.InstHost,GR_Path), string.Format(Program<T>.gc.WXLogUrl, Program<T>.gc.WXSVRHost));
                */
            }
            catch (Exception ce)
            {

            }
            
        }

        void InitListView(ListView lv, string strHeaderFlag)
        {
            lv.Columns.Clear();
            lv.Columns.Add("次数");
            for (int i = 1; i < 20; i++)
            {
                lv.Columns.Add(string.Format("{0}{1}", i, strHeaderFlag),70);
            }
            lv.Columns.Add(string.Format("{0}{1}", 0, strHeaderFlag),70);
        }

        private void toolStripMenuItem_backTest_Click(object sender, EventArgs e)
        {
            ////BackTestFrm frm = new BackTestFrm();
            ////frm.Show();
            
        }

        private void btn_AddExpectNo_Click(object sender, EventArgs e)
        {
            this.timer_For_NewestData.Enabled = false;
            long NextNo = long.Parse(this.txt_NewestExpect.Text.Trim());
            ViewDataList = er.ReadNewestData(NextNo+1,180,true,"000001.SH");
            if (ViewDataList == null || ViewDataList.Count == 0)
                return;
            RefreshGrid();
            RefreshNewestData();
        }

        void RefreshNewestData()
        {
            if (ViewDataList == null || ViewDataList.LastData == null)
                return;
            this.txt_NewestExpect.Text = ViewDataList.LastData.Expect;
            this.txt_NewestOpenCode.Text = ViewDataList.LastData.OpenCode;
            this.txt_NewestOpenTime.Text = ViewDataList.LastData.OpenTime.ToString();
            this.txt_NextExpectNo.Text = string.Format("{0}", ViewDataList.LastData.Expect.ToLong() + 1);
            FillOrgData(listView_PK10Data, ViewDataList);
        }

        void RefreshNewestTXFFCData()
        {
            TXFFCExpectReader<T> rd = new TXFFCExpectReader<T>();
            ExpectList<T> currEl = rd.ReadNewestData(DateTime.Now.AddDays(-1));
            FillOrgData(listView_TXFFCData, currEl);
        }

        private void btn_Subtract_Click(object sender, EventArgs e)
        {
            this.timer_For_NewestData.Enabled = false;
            long NextNo = this.txt_NewestExpect.Text.Trim().ToLong();
            ViewDataList = er.ReadNewestData(NextNo - 1, 180,true,"000001.SH");
            RefreshGrid();
            RefreshNewestData();
        }

        private void timer_For_NewestData_Tick(object sender, EventArgs e)
        {
            timer_For_NewestData.Enabled = true;
            DateTime CurrTime = DateTime.Now;
            ViewDataList = er.ReadNewestData(DateTime.Now.AddDays(-1* dtp.CheckNewestDataDays));
            if(ViewDataList== null ||ViewDataList.LastData == null)
            {
                return;
            }
            long CurrExpectNo = ViewDataList.LastData.Expect.ToLong();
            if (CurrExpectNo > this.NewestExpectNo)
            {
                this.timer_For_NewestData.Interval = 5*60*1000;//5分钟以后见
                RefreshGrid();
                
                this.NewestExpectNo = CurrExpectNo;
            }
            else
            {
                this.timer_For_NewestData.Interval = 60*1000;//10秒后见
            }
            RefreshNewestData();
        }

        private void timer_For_CurrTime_Tick(object sender, EventArgs e)
        {
            this.lbl_Timer.Text = DateTime.Now.ToString();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tMI_HistoryBackTest_Click(object sender, EventArgs e)
        {
            
            //BackTestFrm
        }

        private void TSMI_StartSvr_Click(object sender, EventArgs e)
        {
            StartReceiveDataService();
        }

        private void TSMI_StopSvr_Click(object sender, EventArgs e)
        {
            StopReceiveService();
        }

        private void timer_For_getHtmlData_Tick(object sender, EventArgs e)
        {
            if(GlobalClass.DataTypes.ContainsKey("TXFFC"))
            {
                RefreshNewestTXFFCData();
            }
            
            return;
            //////int secCnt = DateTime.Now.Second;
            //////if (secCnt > 10)
            //////{
            //////    timer_For_getHtmlData.Interval = (7+60 - secCnt) * 1000;
            //////}
            //////else if (secCnt <7)
            //////{
            //////    timer_For_getHtmlData.Interval = (7 - secCnt) * 1000;
            //////}
            //////else
            //////{
            //////    timer_For_getHtmlData.Interval = 60000;
            //////}
            //////TXFFC_HtmlDataClass hdc = new TXFFC_HtmlDataClass();
            //////ExpectList el =  hdc.getExpectList();
            //////TXFFCExpectReader rd = new TXFFCExpectReader();
            //////ExpectList currEl = rd.ReadNewestData(DateTime.Now.AddDays(-1*gobj.CheckNewestDataDays));
            //////rd.SaveNewestData(rd.getNewestData(el, currEl));
            //////currEl = rd.ReadNewestData(DateTime.Now.AddDays(-1*gobj.CheckNewestDataDays));
            //////FillOrgData(listView_TXFFCData, currEl);
        }

        void InitListView(ListView lv)
        {
            lv.Columns.Clear();
            lv.Columns.Add("次序", 50);
            lv.Columns.Add("代码",200);
            lv.Columns.Add("号码",200);
            lv.Columns.Add("时间",200);
        }

        void FillOrgData(ListView lv, ExpectList<T> el)
        {
            lv.Items.Clear();
            if (el == null||el.Count == 0) return;
            int cnt = (int)el.LastData.OpenTime.Subtract(el.FirstData.OpenTime).TotalMinutes + 1;
            for (int i = el.Count - 1;i>=0 ; i--)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = string.Format("{0}", el.Count-i );
                lvi.SubItems.Add(el[i].Expect.ToString());
                lvi.SubItems.Add(el[i].OpenCode?.ToString());
                lvi.SubItems.Add(el[i].OpenTime.ToString());
                lv.Items.Add(lvi);
            }
        }

        private void tsmi_getTXFFCHistoryTxtData_Click(object sender, EventArgs e)
        {
            TXFFCExpectReader<T> er = new TXFFCExpectReader<T>();
            TXFFC_HtmlDataClass rder = new TXFFC_HtmlDataClass(GlobalClass.TypeDataPoints["TXFFC"]);
            string strFolder = @"C:\Users\zhouys\Desktop\TXFFC";
            string fileType = "txt";
            
            DirectoryInfo dir = new DirectoryInfo(strFolder);
            FileInfo[] fil = dir.GetFiles();
            foreach (FileInfo f in fil)
            {
                long size = f.Length;
                if (f.FullName.ToLower().Substring(f.FullName.Length - fileType.Length) != fileType.ToLower())
                {
                    continue;//非指定类型跳过
                }
                ExpectList<T> el = rder.getFileData<T>(f.FullName);
                TXFFCExpectReader<T> rd = new TXFFCExpectReader<T>();
                ExpectList<T> currEl = rd.ReadHistory();
                rd.SaveHistoryData(rd.getNewestData(el, currEl));
                //currEl = rd.ReadNewestData(DateTime.Now.AddDays(-1));
                //FillOrgData(listView_TXFFCData, currEl);

            }

            
        }

        private void tsmi_getTXFFCHistoryFromWeb_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            TXFFC_HtmlDataClass rder = new TXFFC_HtmlDataClass(GlobalClass.TypeDataPoints["TXFFC"]);
            TXFFCExpectReader<T> er = new TXFFCExpectReader<T>();
            string StrBegDate = "2018-08-25";
            ExpectList<T> el = er.GetMissedData(true, StrBegDate);
            for (int i = 0; i < el.Count; i++)
            {
                ExpectList<T> tmpList = new ExpectList<T>(isSecurity);
                DateTime endT = el[i].OpenTime;
                DateTime begT = el[i].OpenTime.AddMinutes(-1 * el[i].MissedCnt-1);
                DateTime tt = DateTime.Parse(begT.ToShortDateString());
                DateTime begT0 = tt;
                while (tt <= endT)
                {
                    string strTt = tt.ToString("yyyy-MM-dd");
                    for (int j = 1; j <= 29; j++)
                    {
                        ExpectList<T> wlist = rder.getHistoryData<T>(strTt, j);//取到web
                        tmpList = ExpectList<T>.Concat(tmpList, wlist);
                        Application.DoEvents();
                        Thread.Sleep(800);
                    }
                    tt=tt.AddDays(1);
                }
                ExpectList<T> currEl = er.ReadHistory(begT0.ToString(),endT.AddDays(1).ToString());
                er.SaveHistoryData(er.getNewestData(tmpList, currEl));
            }
        }

        private void tsmiStartCalcToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void tsmi_StartRefreshWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartRefreshDataService();
        }

        private void tsmi_StopRefreshWindows_Click(object sender, EventArgs e)
        {
            EndRefreshDataService();
        }

        private void toolStripMenuItem_StragManager_Click(object sender, EventArgs e)
        {
            frm_StragManager<T> frm = new frm_StragManager<T>();
            frm.ShowDialog();
        }

        public void tsmi_RunMonitor_Click(object sender, EventArgs e)
        {
            frm_StragMonitor<T> frm = new frm_StragMonitor<T>();
            frm.Show();
        }

        private void ToolStripMenuItem_StragRunPlan_Click(object sender, EventArgs e)
        {
            //frm_StragPlanSetting frm = new frm_StragPlanSetting();
            frm_CommDBObjectsSetting<StragRunPlanClass<T>> frm = new frm_CommDBObjectsSetting<StragRunPlanClass<T>>();
            frm.OuterList = Program<T>.AllGlobalSetting.AllRunPlannings as Dictionary<string, StragRunPlanClass<T>>;
            frm.Show();
        }

        private void tsmi_AssetUnitMgr_Click(object sender, EventArgs e)
        {
            frm_CommDBObjectsSetting<AssetUnitClass<T>> frm = new frm_CommDBObjectsSetting<AssetUnitClass<T>>();
            frm.OuterList = Program<T>.AllGlobalSetting.AllAssetUnits;
            frm.Show();
        }

        private void ToolStripMenuItem_TestDataSrc_Click(object sender, EventArgs e)
        {
            PK10_HtmlDataClass hdc = new PK10_HtmlDataClass(GlobalClass.TypeDataPoints.First().Value);
            ExpectList<T> tmp = hdc.getExpectList<T>();
            if(tmp == null)
            {
                MessageBox.Show("数据源错误！");
                return;
            }
            if(tmp.Count == 0)
            {
                MessageBox.Show("数据长度为0！");
                return;
            }
            //MessageBox.Show(tmp.LastData.ToDetailString());
        }

        private void tsmi_getHistoryData_Click(object sender, EventArgs e)
        {
            string endPage = Interaction.InputBox("请输入结束页编号", "结束页编辑框", "", 100, 100).Trim();
            string[] pages = endPage.Split('-');
            int begi = int.Parse(pages[0].Trim());
            int endi = int.Parse(pages[1].Trim());
            HtmlDataClass rder = HtmlDataClass.CreateInstance(GlobalClass.TypeDataPoints.First().Value);
            CommExpectReader<T> er = DataReaderBuild.CreateReader<T>(GlobalClass.TypeDataPoints.First().Key, null, null) as CommExpectReader<T>;
            Application.DoEvents();
            
            ExpectList<T> tmpList = new ExpectList<T>(isSecurity);
            for (int i=begi;i<=endi;i++)
            {
                
                ExpectList<T> wlist = new ExpectList<T>(isSecurity);
                wlist = rder.getHistoryData<T>(null, i);//取到web
                ExpectList<T> wtrue = new ExpectList<T>(isSecurity);
                wtrue = er.getNewestData(wlist, tmpList);// 
                tmpList = ExpectList<T>.Concat(tmpList, wtrue);
                this.tssl_Count.Text = string.Format("访问第{1}页，共计获取到{0}条记录",tmpList.Count,i );
                if(tmpList.Count>=100)
                {
                    ExpectList<T> currEl = er.ReadHistory(tmpList.MinExpect.ToString(), 10000,"000001.SH");
                    ExpectList<T> NewEl = er.getNewestData(tmpList, currEl);
                    long res1 = er.SaveHistoryData(NewEl);
                    tmpList = new ExpectList<T>(isSecurity);
                    this.tssl_Count.Text = string.Format("访问第{1}页，共计成功保存了{0}条记录", res1, i);
                }
                Application.DoEvents();
                Thread.Sleep(10000);
            }
            ExpectList<T> currEl1 = er.ReadHistory(tmpList.MinExpect.ToString(), 10000,"000001.SH");
            ExpectList<T> NewEl1 = er.getNewestData(tmpList, currEl1);
            long res = er.SaveHistoryData(NewEl1);
            if(res>0)
            {
                MessageBox.Show(string.Format("成功获取到{0}条历史记录！", res));
            }
            else
            {
                MessageBox.Show(string.Format("保存{0}条历史记录失败！",NewEl1.Count));
            }
            ////string StrBegDate = "2018-08-25";
            ////ExpectList<T> el = er.GetMissedData<T>(true, StrBegDate);
            ////for (int i = 0; i < el.Count; i++)
            ////{
            ////    ExpectList<T> tmpList = new ExpectList<T>();
            ////    DateTime endT = el[i].OpenTime;
            ////    DateTime begT = el[i].OpenTime.AddMinutes(-1 * el[i].MissedCnt - 1);
            ////    DateTime tt = DateTime.Parse(begT.ToShortDateString());
            ////    DateTime begT0 = tt;
            ////    while (tt <= endT)
            ////    {
            ////        string strTt = tt.ToString("yyyy-MM-dd");
            ////        for (int j = 1; j <= 29; j++)
            ////        {
            ////            ExpectList<T> wlist = rder .getHistoryData<T>(strTt, j);//取到web
            ////            tmpList = ExpectList<T>.Concat(tmpList, wlist);
            ////            Application.DoEvents();
            ////            Thread.Sleep(800);
            ////        }
            ////        tt = tt.AddDays(1);
            ////    }
            ////    ExpectList<T> currEl = er.ReadHistory<T>(begT0.ToString(), endT.AddDays(1).ToString());
            ////    er.SaveHistoryData(er.getNewestData(tmpList, currEl));
            ////}

        }

        private void ToolStripMenuItem_SingleStragMonitor_Click(object sender, EventArgs e)
        {
            frm_MoniteStrag<T> frm = new frm_MoniteStrag<T>();
            frm.ShowDialog();
        }

        private void refreshAllDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program<T>.optFunc.RefreshData(this);
        }
    }

    public class imageLogClass<T> where T:TimeSerialData                                       
    {
        public static void SaveImage(Control lv,string GR_Path, ImageFormat fmt)
        {
            try
            {
                Bitmap image = new Bitmap(lv.Width, lv.Height);//初始化一个相同大小的窗口
                lv.DrawToBitmap(image, new Rectangle(0, 0, lv.Width, lv.Height));
                string fullFileName = string.Format("{0}\\imgs\\{1}", AppDomain.CurrentDomain.BaseDirectory, GR_Path);// GR_Path + "\\" + fileName + ".png";
                image.Save(fullFileName, fmt);
                string filename = string.Format("chartImg");
                Program<T>.wxlog.LogImageUrl(string.Format("{0}/chartImgs_{2}/{1}", GlobalClass.TypeDataPoints.First().Value.InstHost, GR_Path, GlobalClass.TypeDataPoints.First().Value.DataType), string.Format(Program<T>.gc.WXLogUrl, Program<T>.gc.WXSVRHost));

            }
            catch(Exception ce)
            {
                Program<T>.wxlog.Log(ce.Message,ce.StackTrace, string.Format(Program<T>.gc.WXLogUrl, Program<T>.gc.WXSVRHost));
            }
        }
    }
        
}

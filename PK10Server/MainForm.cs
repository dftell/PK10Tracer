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
    
    public partial class MainForm:BaseForm_DESIGN
    {
        
        DataTypePoint dtp = null;
        ExpectList<TimeSerialData> _ViewDataList ;
        ExpectList<TimeSerialData> ViewDataList
        {
            get { return _ViewDataList; }
            set { _ViewDataList = value; }
        }
        //PK10ExpectReader er = new PK10ExpectReader();
        DataReader er = null;
        long NewestExpectNo = 0;
        public long InputExpect;
        GlobalClass gobj;
        public MainForm():base()
        {
            
                InitializeComponent();
            
            gobj = new GlobalClass();
            dtp = GlobalClass.TypeDataPoints.First().Value;
            er = DataReaderBuild.CreateReader(dtp.DataType, null, null);
            if(Program.optFunc == null)
            {
                Program.optFunc = new operateClass();
            }
            Program.optFunc.RefreshMainWindow += refreshData;
        }


        void refreshData()
        {
            //Program.AllGlobalSetting.wxlog.Log("退出服务", "意外停止服务", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
            timer_For_NewestData_Tick(null, null);
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            InitListView(listView_forSerial, "名");
            InitListView(listView_ForCar, "车");

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
                ViewDataList = er.ReadNewestData<TimeSerialData>(DateTime.Today.AddDays(-1* dtp.CheckNewestDataDays));
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


                ExpectList<TimeSerialData> el = ViewDataList; ;

                ExpectListProcessBuilder<TimeSerialData> elp = new ExpectListProcessBuilder<TimeSerialData>(dtp, el);
                BaseCollection<TimeSerialData> sc = elp.getProcess().getSerialData(180, byNo);
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
                imageLogClass.SaveImage(lv,GR_Path,ImageFormat.Jpeg);
                /*
                Bitmap image = new Bitmap(lv.Width, lv.Height);//初始化一个相同大小的窗口
                lv.DrawToBitmap(image, new Rectangle(0, 0, lv.Width, lv.Height));
                
                string fullFileName = string.Format("{0}\\imgs\\{1}", AppDomain.CurrentDomain.BaseDirectory, GR_Path);// GR_Path + "\\" + fileName + ".png";
                image.Save(fullFileName,System.Drawing.Imaging.ImageFormat.Jpeg);
                    string filename = string.Format("chartImg");
                Program.wxlog.LogImageUrl(string.Format("{0}/chartImgs/{1}", GlobalClass.TypeDataPoints.First().Value.InstHost,GR_Path), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
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
            ViewDataList = er.ReadNewestData<TimeSerialData>(NextNo+1,180,true);
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
            this.txt_NextExpectNo.Text = string.Format("{0}", long.Parse(ViewDataList.LastData.Expect) + 1);
            FillOrgData(listView_PK10Data, ViewDataList);
        }

        void RefreshNewestTXFFCData()
        {
            TXFFCExpectReader rd = new TXFFCExpectReader();
            ExpectList<TimeSerialData> currEl = rd.ReadNewestData<TimeSerialData>(DateTime.Now.AddDays(-1));
            FillOrgData(listView_TXFFCData, currEl);
        }

        private void btn_Subtract_Click(object sender, EventArgs e)
        {
            this.timer_For_NewestData.Enabled = false;
            long NextNo = long.Parse(this.txt_NewestExpect.Text.Trim());
            ViewDataList = er.ReadNewestData<TimeSerialData>(NextNo - 1, 180,true);
            RefreshGrid();
            RefreshNewestData();
        }

        private void timer_For_NewestData_Tick(object sender, EventArgs e)
        {
            timer_For_NewestData.Enabled = true;
            DateTime CurrTime = DateTime.Now;
            ViewDataList = er.ReadNewestData<TimeSerialData>(DateTime.Now.AddDays(-1* dtp.CheckNewestDataDays));
            if(ViewDataList== null ||ViewDataList.LastData == null)
            {
                return;
            }
            long CurrExpectNo = long.Parse(ViewDataList.LastData.Expect);
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
            lv.Columns.Add("期号",200);
            lv.Columns.Add("开奖号码",200);
            lv.Columns.Add("开奖时间",200);
        }

        void FillOrgData(ListView lv, ExpectList<TimeSerialData> el)
        {
            lv.Items.Clear();
            if (el == null||el.Count == 0) return;
            int cnt = (int)el.LastData.OpenTime.Subtract(el.FirstData.OpenTime).TotalMinutes + 1;
            for (int i = el.Count - 1;i>=0 ; i--)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = string.Format("{0}", el.Count-i );
                lvi.SubItems.Add(el[i].Expect.ToString());
                lvi.SubItems.Add(el[i].OpenCode.ToString());
                lvi.SubItems.Add(el[i].OpenTime.ToString());
                lv.Items.Add(lvi);
            }
        }

        private void tsmi_getTXFFCHistoryTxtData_Click(object sender, EventArgs e)
        {
            TXFFCExpectReader er = new TXFFCExpectReader();
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
                ExpectList<TimeSerialData> el = rder.getFileData<TimeSerialData>(f.FullName);
                TXFFCExpectReader rd = new TXFFCExpectReader();
                ExpectList<TimeSerialData> currEl = rd.ReadHistory<TimeSerialData>();
                rd.SaveHistoryData(rd.getNewestData(el, currEl));
                //currEl = rd.ReadNewestData(DateTime.Now.AddDays(-1));
                //FillOrgData(listView_TXFFCData, currEl);

            }

            
        }

        private void tsmi_getTXFFCHistoryFromWeb_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            TXFFC_HtmlDataClass rder = new TXFFC_HtmlDataClass(GlobalClass.TypeDataPoints["TXFFC"]);
            TXFFCExpectReader er = new TXFFCExpectReader();
            string StrBegDate = "2018-08-25";
            ExpectList<TimeSerialData> el = er.GetMissedData<TimeSerialData>(true, StrBegDate);
            for (int i = 0; i < el.Count; i++)
            {
                ExpectList<TimeSerialData> tmpList = new ExpectList<TimeSerialData>();
                DateTime endT = el[i].OpenTime;
                DateTime begT = el[i].OpenTime.AddMinutes(-1 * el[i].MissedCnt-1);
                DateTime tt = DateTime.Parse(begT.ToShortDateString());
                DateTime begT0 = tt;
                while (tt <= endT)
                {
                    string strTt = tt.ToString("yyyy-MM-dd");
                    for (int j = 1; j <= 29; j++)
                    {
                        ExpectList<TimeSerialData> wlist = rder.getHistoryData<TimeSerialData>(strTt, j);//取到web
                        tmpList = ExpectList<TimeSerialData>.Concat(tmpList, wlist);
                        Application.DoEvents();
                        Thread.Sleep(800);
                    }
                    tt=tt.AddDays(1);
                }
                ExpectList<TimeSerialData> currEl = er.ReadHistory<TimeSerialData>(begT0.ToString(),endT.AddDays(1).ToString());
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
            frm_StragManager frm = new frm_StragManager();
            frm.ShowDialog();
        }

        public void tsmi_RunMonitor_Click(object sender, EventArgs e)
        {
            frm_StragMonitor<TimeSerialData> frm = new frm_StragMonitor<TimeSerialData>();
            frm.Show();
        }

        private void ToolStripMenuItem_StragRunPlan_Click(object sender, EventArgs e)
        {
            //frm_StragPlanSetting frm = new frm_StragPlanSetting();
            frm_CommDBObjectsSetting<StragRunPlanClass<TimeSerialData>> frm = new frm_CommDBObjectsSetting<StragRunPlanClass<TimeSerialData>>();
            frm.OuterList = Program.AllGlobalSetting.AllRunPlannings as Dictionary<string, StragRunPlanClass<TimeSerialData>>;
            frm.Show();
        }

        private void tsmi_AssetUnitMgr_Click(object sender, EventArgs e)
        {
            frm_CommDBObjectsSetting<AssetUnitClass> frm = new frm_CommDBObjectsSetting<AssetUnitClass>();
            frm.OuterList = Program.AllGlobalSetting.AllAssetUnits;
            frm.Show();
        }

        private void ToolStripMenuItem_TestDataSrc_Click(object sender, EventArgs e)
        {
            PK10_HtmlDataClass hdc = new PK10_HtmlDataClass(GlobalClass.TypeDataPoints.First().Value);
            ExpectList<TimeSerialData> tmp = hdc.getExpectList<TimeSerialData>();
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
            MessageBox.Show(tmp.LastData.ToDetailString());
        }

        private void tsmi_getHistoryData_Click(object sender, EventArgs e)
        {
            string endPage = Interaction.InputBox("请输入结束页编号", "结束页编辑框", "", 100, 100).Trim();
            string[] pages = endPage.Split('-');
            int begi = int.Parse(pages[0].Trim());
            int endi = int.Parse(pages[1].Trim());
            HtmlDataClass rder = HtmlDataClass.CreateInstance(GlobalClass.TypeDataPoints.First().Value);
            CommExpectReader er = DataReaderBuild.CreateReader(GlobalClass.TypeDataPoints.First().Key, null, null) as CommExpectReader;
            Application.DoEvents();
            
            ExpectList<TimeSerialData> tmpList = new ExpectList<TimeSerialData>();
            for (int i=begi;i<=endi;i++)
            {
                
                ExpectList<TimeSerialData> wlist = new ExpectList<TimeSerialData>();
                wlist = rder.getHistoryData<TimeSerialData>(null, i);//取到web
                ExpectList<TimeSerialData> wtrue = new ExpectList<TimeSerialData>();
                wtrue = er.getNewestData(wlist, tmpList);// 
                tmpList = ExpectList<TimeSerialData>.Concat(tmpList, wtrue);
                this.tssl_Count.Text = string.Format("访问第{1}页，共计获取到{0}条记录",tmpList.Count,i );
                if(tmpList.Count>=100)
                {
                    ExpectList<TimeSerialData> currEl = er.ReadHistory<TimeSerialData>(tmpList.MinExpect, 10000);
                    ExpectList<TimeSerialData> NewEl = er.getNewestData(tmpList, currEl);
                    long res1 = er.SaveHistoryData(NewEl);
                    tmpList = new ExpectList<TimeSerialData>();
                    this.tssl_Count.Text = string.Format("访问第{1}页，共计成功保存了{0}条记录", res1, i);
                }
                Application.DoEvents();
                Thread.Sleep(10000);
            }
            ExpectList<TimeSerialData> currEl1 = er.ReadHistory<TimeSerialData>(tmpList.MinExpect, 10000);
            ExpectList<TimeSerialData> NewEl1 = er.getNewestData(tmpList, currEl1);
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
            ////ExpectList<TimeSerialData> el = er.GetMissedData<TimeSerialData>(true, StrBegDate);
            ////for (int i = 0; i < el.Count; i++)
            ////{
            ////    ExpectList<TimeSerialData> tmpList = new ExpectList<TimeSerialData>();
            ////    DateTime endT = el[i].OpenTime;
            ////    DateTime begT = el[i].OpenTime.AddMinutes(-1 * el[i].MissedCnt - 1);
            ////    DateTime tt = DateTime.Parse(begT.ToShortDateString());
            ////    DateTime begT0 = tt;
            ////    while (tt <= endT)
            ////    {
            ////        string strTt = tt.ToString("yyyy-MM-dd");
            ////        for (int j = 1; j <= 29; j++)
            ////        {
            ////            ExpectList<TimeSerialData> wlist = rder .getHistoryData<TimeSerialData>(strTt, j);//取到web
            ////            tmpList = ExpectList<TimeSerialData>.Concat(tmpList, wlist);
            ////            Application.DoEvents();
            ////            Thread.Sleep(800);
            ////        }
            ////        tt = tt.AddDays(1);
            ////    }
            ////    ExpectList<TimeSerialData> currEl = er.ReadHistory<TimeSerialData>(begT0.ToString(), endT.AddDays(1).ToString());
            ////    er.SaveHistoryData(er.getNewestData(tmpList, currEl));
            ////}

        }

        private void ToolStripMenuItem_SingleStragMonitor_Click(object sender, EventArgs e)
        {
            frm_MoniteStrag frm = new frm_MoniteStrag();
            frm.ShowDialog();
        }

        private void refreshAllDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.optFunc.RefreshData(this);
        }
    }

    public class imageLogClass                                       
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
                Program.wxlog.LogImageUrl(string.Format("{0}/chartImgs/{1}", GlobalClass.TypeDataPoints.First().Value.InstHost, GR_Path), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));

            }
            catch(Exception ce)
            {
                Program.wxlog.Log(ce.Message,ce.StackTrace, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
            }
        }
    }
        
}

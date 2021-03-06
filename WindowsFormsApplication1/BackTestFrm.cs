﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using WolfInv.com.PK10CorePress;
using WolfInv.com.BackTestLib;
using WolfInv.com.Strags;
using PK10Server;
using WolfInv.com.ExchangeLib;
using System.Threading;
using WolfInv.com.GuideLib;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.DataVisualization.Charting.ChartTypes;
using WolfInv.com.ServerInitLib;
using BackTestSystem;
//using WolfInv.com.Strags;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
using WolfInv.com.WDDataInit;
using System.Collections.Concurrent;
using System.Collections;
using AIEquity;
using System.Threading.Tasks;

namespace BackTestSys
{
    
    public partial class BackTestFrm<T> : Form where T : TimeSerialData
    {
        BackgroundWorker bw;
        BackTestReturnClass<T> ret = null;
        BackTestClass<T> btc = null;
        MainForm<T> CheckFrm;
        Form1<T> SecCheckFrm;
        //Dictionary<string, ExchangeService<T>> ess;
        Hashtable ess;
        BaseStragClass<T> sc;
        Thread th = null;
        GlobalClass globalSetting = new GlobalClass();
        bool _RunVirExchange;
        List<StragRunPlanClass<T>> SCList = new List<StragRunPlanClass<T>>();
        GuideResult retData;
        Int64 lastSaveCnt = 0;
        bool RunVirExchange
        {
            get
            {
                return _RunVirExchange;
            }
            set
            {
                _RunVirExchange = value;
                if (_RunVirExchange && this.chart1.Series.Count > 0)
                {
                    this.chart1.Series[0].Points.Clear();//DataBindXY(moneyLines, "id", moneyLines, "val");
                    //this.dataGridView_ExchangeDetail.DataSource = null;
                    //this.btn_VirExchange.Text = "停止交易"; 
                }
                else
                {
                    //this.btn_VirExchange.Text = "模拟交易";
                }
                this.timer_Tip.Enabled = _RunVirExchange;
            }
        }

        public BackTestFrm()
        {
            InitializeComponent();
            Init_Picker();
        }

        void Init_Picker()
        {
            this.runPlanPicker1 = new WolfInv.com.ExchangeLib.RunPlanPicker<T>();
            // 
            // runPlanPicker1
            // 
            this.runPlanPicker1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runPlanPicker1.Location = new System.Drawing.Point(108, 21);
            this.runPlanPicker1.Margin = new System.Windows.Forms.Padding(6);
            this.runPlanPicker1.Name = "runPlanPicker1";
            this.runPlanPicker1.Plans = null;
            this.runPlanPicker1.Size = new System.Drawing.Size(288, 21);
            this.runPlanPicker1.TabIndex = 31;
            this.tabPage_usePlan.Controls.Add(this.runPlanPicker1);
        }
        private void BackTestFrm_Load(object sender, EventArgs e)
        {
            InitFrm();
            this.timer_Tip.Enabled = false;
            //this.txt_InitCash.Text = int.MaxValue.ToString();
        }
        void InitFrm()
        { 
            //PK10_HtmlDataClass hdc = new PK10_HtmlDataClass();
            //ExpectList el = hdc.getExpectList();


            ////List<string> ret =  ChanceClass.getAllSubCode("13579", 4);
            ////string strRet = string.Join(",",ret.ToArray());
            ////MessageBox.Show(strRet);
            ////return;

            ////DataTable strags = StragClass.getAllStrags();
            ////this.ddl_StragName.DataSource = strags;
            ////this.ddl_StragName.DisplayMember = "text";
            ////this.ddl_StragName.ValueMember = "value";
            this.listView1.ListViewItemSorter = new ListViewItemComparer();
            DataTable dt = new DataTable();
            dt.Columns.Add("编码");
            dt.Columns.Add("组合");
            dt.Columns.Add("开始期号");
            dt.Columns.Add("结束期号");
            dt.Columns.Add("个数");
            dt.Columns.Add("入场次数");
            dt.Columns.Add("持有次数");
            dt.Columns.Add("命中次数");
            dt.Columns.Add("开始时间");
            dt.Columns.Add("结束时间");
            this.listView1.Columns.Clear();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                this.listView1.Columns.Add(dt.Columns[i].ColumnName);
            }
            this.listView2.Columns.Clear();
            this.listView2.Columns.Add("", 0);
            this.listView2.Columns.Add("持续次数", 200);
            this.listView2.Columns.Add("机会数", 200);
            this.listView2.Columns.Add("命中数", 200);
            this.listView2.Columns.Add("投入个数", 200);
            this.listView2.Columns.Add("命中个数", 200);
            this.listView2.Columns.Add("胜率", 200);
            this.listView2.Columns.Add("进入胜率", 200);
            this.listView2.Columns.Add("命中胜率", 200);
            this.listView2.Columns.Add("日均次数", 200);
            this.listView2.ListViewItemSorter = new ListViewItemComparer();
            this.listView3.Columns.Clear();
            this.listView3.Columns.Add("编号", 200);
            this.listView3.Columns.Add("次数", 200);
            this.listView3.Columns.Add("机会数", 200);
            this.listView3.Columns.Add("首次胜数", 200);
            this.listView3.Columns.Add("首次概率", 200);
            this.listView3.ListViewItemSorter = new ListViewItemComparer();
            tsmi_ExportExcel.Click += new EventHandler(tsmi_ExportExcel_Click);
            tsmi_ImportExcel.Click += new EventHandler(Tsmi_ImportExcel_Click);
            this.dataGridView_ProbData.ContextMenuStrip = contextMenuStrip_ForListView;
            this.listView2.ContextMenuStrip = contextMenuStrip_ForListView;
            this.listView3.ContextMenuStrip = contextMenuStrip_ForListView;
            this.listView1.ContextMenuStrip = contextMenuStrip_ForListView;
            this.dataGridView_ExchangeDetail.ContextMenuStrip = contextMenuStrip_ForListView;
            CheckForIllegalCrossThreadCalls = false;
            LoadDataSrc(this.ddl_DataSource);
            if(GlobalClass.TypeDataPoints.First().Value.IsSecurityData==0)
            {
                this.txt_begExpNo.Text = "233049";
                
                this.txt_LoopCnt.Text = "50";
                this.txt_InitCash.Text = "10000000";
            }
            this.txt_endExpNo.Text = DateTime.Today.WDDate();
        }

        
        private void Tsmi_ImportExcel_Click(object sender, EventArgs e)
        {
            CExcel ce = new CExcel();
            object src = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl;
            if (src == null) return;
            ce.ImportExcel(src as DataGridView);
        }

        public static void LoadDataSrc(ComboBox ddl_DataSource)
        {
            ddl_DataSource.Items.Clear();
            DataTable dt = new DataTable();
            dt.Columns.Add("value");
            dt.Columns.Add("text");
            foreach (string key in GlobalClass.DataTypes.Keys)
            {
                DataRow dr = dt.NewRow();
                dr["value"] = key;
                dr["text"] = GlobalClass.DataTypes[key];
                dt.Rows.Add(dr);
            }
            ddl_DataSource.DataSource = dt;
            ddl_DataSource.DisplayMember = "text";
            ddl_DataSource.ValueMember = "value";

        }

        void tsmi_ExportExcel_Click(object sender, EventArgs e)
        {
            object src = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl;
            if (src == null) return;
            //ListView lv = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as ListView;
            //if (lv == null) return;
            CExcel ce = new CExcel();
            Control tagCtrl = src as Control;
            if(tagCtrl == null)
            {
                return;
            }
            else if (tagCtrl.Tag is DataTable)
            {
                ce.ExportExcel(tagCtrl.Tag as DataTable);
                return;
            }
            else if(tagCtrl.Tag is DataView)
            {
                ce.ExportExcel(tagCtrl.Tag as DataView);
                return;
            }
            else if (src is DataGridView)
            {
                ce.ExportExcel(src as DataGridView);
                return;
            }
            else if (src is ListView)
            {
                ce.ExportExcel(src as ListView);
                return;
            }
            else
                return;
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.statusStrip1.Items[0].Text = ret.Msg;
            this.statusStrip1.Items[1].Text = ret.LoopCnt.ToString();
            MessageBox.Show(ret.Msg);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //还原worker对象
            BackgroundWorker worker = sender as BackgroundWorker;
            //开始工作
            DoSomething(worker, e);
        }

        void Finished(string ex)
        {
            try
            {
                //this.timer_Tip.Enabled = false;
                if (checkBox_MixAll.Checked)
                {
                    AssetUnitClass<T> auc = SCList[0].AssetUnitInfo;
                    auc.SaveDataToFile();
                }
                else
                {
                    bool tmp = this.chkb_noDetailTable.Checked;
                    this.chkb_noDetailTable.Checked = false;
                    refreshExchangData(ex,ess, true);
                    this.chkb_noDetailTable.Checked = tmp;
                    SCList.ForEach(a =>
                    {
                        AssetUnitClass<T> auc = a.AssetUnitInfo;
                        auc.SaveDataToFile();
                    });
                }
                wxLog("量化投研回测系统", "交易执行完毕！");
                MessageBox.Show("执行完毕！");
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }
        Thread thrd = null;
        private void DoSomething(BackgroundWorker worker, DoWorkEventArgs e)
        {

            SettingClass setting = new SettingClass();
            setting.GrownMaxVal = int.Parse(this.txt_GrownMaxVal.Text);
            setting.GrownMinVal = int.Parse(this.txt_GrownMinVal.Text);
            setting.DispRows = int.Parse(this.txt_MinCols.Text);
            setting.minColTimes = new int[10];
            for (int i = 0; i < 9; i++)
            {
                TextBox tb = this.Controls.Find(string.Format("txt_minColTimes{0}", i + 1), true)[0] as TextBox;
                setting.minColTimes[i] = int.Parse(tb.Text);
            }
            #region 老的调用逻辑改为和正式运行一样的通过注入选择的计划列表，运行。除了调试不写入表，其他处理一样。其实在正常处理里面也可以使用调试模式，那会导致停止服务后无法保留交易数据
            //////////////////Assembly asmb = typeof(StragClass).Assembly;// Assembly.LoadFrom("EnterpriseServerBase.dll");
            //////////////////btc = new BackTestClass(long.Parse(txt_begExpNo.Text), long.Parse(txt_LoopCnt.Text), setting);
            //////////////////Type sct = asmb.GetType(ddl_StragName.SelectedValue.ToString());
            //////////////////StragClass sc = Activator.CreateInstance(sct) as StragClass;
            //////////////////sc.CommSetting = setting;
            //////////////////sc.ChipCount = int.Parse(this.txt_ChipCnt.Text);
            //////////////////sc.FixChipCnt = (this.txt_FixChipCnt.Text.Trim() == "0") ? false : true;
            //////////////////sc.ReviewExpectCnt = int.Parse(this.txt_reviewExpCnt.Text);
            //////////////////sc.InputMinTimes = int.Parse(this.txt_minInputTimes.Text);
            //////////////////sc.InputMaxTimes = int.Parse(this.txt_maxInputTimes.Text);
            //////////////////sc.ExcludeBS = this.chkb_exclueBS.Checked;
            //////////////////sc.ExcludeSD = this.chkb_exclueSD.Checked;
            //////////////////sc.BySer = this.chkb_bySer.Checked;
            //////////////////sc.OnlyBS = this.chkb_onlyBS.Checked;
            //////////////////sc.OnlySD = this.chkb_onlySD.Checked;
            //////////////////sc.GetRev = this.chkb_getRev.Checked;
            //////////////////if (sc is IProbCheckClass)
            //////////////////{
            //////////////////    (sc as IProbCheckClass).StdvCnt = double.Parse(this.txt_StdvCnt.Text);
            //////////////////}
            //////////////////sc.MinWinRate = (double)double.Parse(this.txt_Odds.Text) / double.Parse(this.txt_ChipCnt.Text) / double.Parse(this.txt_minRate.Text);
            ////////////////////凯利公式 (p*b-q)/q
            ////////////////////////double p = 1 / sc.MinWinRate;
            ////////////////////////double b = double.Parse(this.txt_Odds.Text);
            ////////////////////////double q = 1 - p;
            ////////////////////////sc.StagSetting = sc.getInitStagSetting();
            ////////////////////////sc.StagSetting.BaseType.ChipRate = (p * b - q) / q;
            ////////////////////////if (MessageBox.Show(sc.StagSetting.BaseType.ChipRate.ToString(), "胜率", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel) ;
            ////////////////////////{
            ////////////////////////    return;
            ////////////////////////}
            #endregion
            btc = new BackTestClass<T>(GlobalClass.TypeDataPoints[ddl_DataSource.SelectedValue.ToString()],chkb_useSpecList.Checked?txt_SecPools.Text:"", chkb_useBechmark.Checked ? txt_benchMarkCode.Text : "", txt_begExpNo.Text, long.Parse(txt_LoopCnt.Text),0, setting);
            this.listView1.Items.Clear();
            this.listView2.Items.Clear();
            this.listView3.Items.Clear();
            StragRunPlanClass<T>[] plans = this.runPlanPicker1.Plans as StragRunPlanClass<T>[];
            if (plans.Length == 0)
                return;
            SCList = plans.ToList();
            SCList.ForEach(p => p.PlanStrag.CommSetting = setting);
            //SCList.ForEach(p => p.PlanStrag.ChipCount = int.Parse(this.txt_ChipCnt.Text));
            SCList.ForEach(p => p.Running = true);
            SCList.ForEach(p => p.AutoRunning = true);
            SCList.ForEach(p => p.FixAmt = 1);
            SCList.ForEach(p => p.FixRate = 0.01);
            if(ckb_useCondition.Checked)
            {
                SCList.ForEach(a=> {
                    a.PlanStrag.BySer = this.chkb_bySer.Checked;
                    a.PlanStrag.ReviewExpectCnt = int.Parse(this.txt_reviewExpCnt.Text);
                    a.PlanStrag.ChipCount = int.Parse(this.txt_ChipCnt.Text);
                    a.PlanStrag.FixChipCnt = this.txt_FixChipCnt.Text.Trim() == "1";
                    a.PlanStrag.InputMinTimes = int.Parse(this.txt_minInputTimes.Text);
                    a.PlanStrag.InputMaxTimes = int.Parse(this.txt_maxInputTimes.Text);
                });
            }
            sc = SCList[0].PlanStrag;
            this.Cursor = Cursors.WaitCursor;
            //timer_Tip.Tick += new EventHandler(RefreshList);
            //this.timer_Tip.Interval = int.Parse(txt_Timer_Interval.Text) * 1000;
            //this.timer_Tip.Enabled = true;

            try
            {

                btc.FinishedProcess = new SuccEvent(Finished);
                btc.teststrag = sc;
                thrd = new Thread(new ThreadStart(btc.Run));
                thrd.Start();

            }
            catch (Exception ce)
            {
                ret = new BackTestReturnClass<T>();
                ret.ChanceList = new List<ChanceClass<T>>();
                ret.Msg = ce.Message;
                ret.succ = false;
                MessageBox.Show(ce.Message);
            }


            //this.RunVirExchange = true;


            this.Cursor = Cursors.Default;
            ////if (!ret.succ)
            ////    MessageBox.Show(ret.Msg);
            ////RefreshList(null, null);
            ////while(true)
            ////{
            ////    Thread.Sleep(1000);
            ////    RefreshList(null, null);
            ////    if (thrd.ThreadState == ThreadState.Stopped)
            ////    {
            ////        break;
            ////    }
            ////}
            ////this.timer_Tip.Enabled = false;
            ////RefreshList();
        }

        void RefreshList(object sender, EventArgs e)
        {
            try
            {
                RefreshList();
            }
            catch
            {
            }
        }

        void RefreshList()
        {
            //////if (!ret.succ)
            //////{
            //////    this.Cursor = Cursors.Default;
            //////    MessageBox.Show(ret.Msg);
            //////    return;
            //////}
            ret = btc.ret;
            if (ret == null)
                return;
            if (!ret.succ && ret.Msg != null)
            {

                //this.timer_Tip.Enabled = false;
                MessageBox.Show(ret.Msg);
                return;
            }
            this.listView1.Items.Clear();

            lock (ret.ChanceList)
            {
                for (int i = 0; i < ret.ChanceList.Count; i++)
                {
                    if (ret.ChanceList[i].HoldTimeCnt > int.Parse(this.txt_MinCols.Text))
                    {
                        ListViewItem li = new ListViewItem();
                        li.Text = i.ToString();
                        li.SubItems.Add(ret.ChanceList[i].ChanceCode);
                        li.SubItems.Add(ret.ChanceList[i].SignExpectNo);
                        li.SubItems.Add(ret.ChanceList[i].EndExpectNo);
                        li.SubItems.Add(ret.ChanceList[i].ChipCount.ToString());
                        li.SubItems.Add(ret.ChanceList[i].strInputTimes);
                        li.SubItems.Add(ret.ChanceList[i].HoldTimeCnt.ToString());
                        li.SubItems.Add(ret.ChanceList[i].MatchChips.ToString());
                        li.SubItems.Add(ret.ChanceList[i].CreateTime.ToString());
                        li.SubItems.Add(ret.ChanceList[i].UpdateTime.ToString());

                        this.listView1.Items.Add(li);


                    }
                }
                this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                //this.listView1.SelectedItems;
            }
            this.listView2.Items.Clear();
            lock (ret.InChipsDic)
            {
                int AllChips = ret.InChipsDic.Sum(it => it.Value);
                int AllWinChips = ret.WinChipsDic.Sum(it => it.Value);
                foreach (int key in ret.HoldCntDic.Keys)
                {
                    //AllChips += ret.InChipsDic[key];
                    ListViewItem li = new ListViewItem();
                    li.SubItems.Add(key.ToString());
                    li.SubItems.Add(ret.HoldCntDic[key].ToString());
                    li.SubItems.Add(ret.HoldWinCntDic[key].ToString());
                    li.SubItems.Add(ret.InChipsDic[key].ToString());
                    li.SubItems.Add(ret.WinChipsDic[key].ToString());
                    int preCnt = 0;
                    int preInChips = 0;
                    foreach (int mkey in ret.HoldCntDic.Keys)
                    {
                        if (key > mkey)
                        {
                            preCnt = preCnt + ret.HoldCntDic[mkey];
                            preInChips += ret.InChipsDic[mkey];
                        }
                    }
                    double bs = 100.00 * (float)ret.HoldCntDic[key] / (float)ret.ChanceList.Count;
                    double rbs = 100.00 * (float)ret.HoldWinCntDic[key] / (float)ret.ChanceList.Count;
                    double dr = (float)ret.HoldCntDic[key] / ((float)ret.LoopCnt / GlobalClass.TypeDataPoints[ddl_DataSource.SelectedValue.ToString()].ExpectCodeCounterMax);
                    double cr = 100.00 * (float)ret.HoldWinCntDic[key] / (ret.ChanceList.Count - preCnt);
                    double mr = 100.00 * (float)ret.WinChipsDic[key] / (AllChips - preInChips);
                    li.SubItems.Add(string.Format("{0:F}/{1:F}", bs, rbs));
                    //li.SubItems.Add(string.Format("100*{1}/({2}-{3})={0:F}", cr, ret.HoldCntDic[key], ret.ChanceList.Count, preCnt));
                    li.SubItems.Add(string.Format("{0:F}", cr));
                    li.SubItems.Add(mr.ToString());
                    li.SubItems.Add(string.Format("{0:F}", dr));
                    this.listView2.Items.Add(li);
                }

                this.toolStripStatusLabel1.Text = string.Format("第{4}次;合计数量：{0}({2}/{3})/{1}", ret.ChanceList.Count.ToString(), ret.LoopCnt, AllChips, AllWinChips, btc.testIndex);
                this.toolStripStatusLabel2.Text = "显示数量：" + this.listView1.Items.Count.ToString();
            }
            if (ret.succ)
            {
                //this.timer_Tip.Enabled = false;
            }
        }

        private void btn_startTest_Click(object sender, EventArgs e)
        {
            if (thrd == null)
            {
                if (MessageBox.Show("确定开始回测？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                if (MessageBox.Show("确定停止回测？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                {

                    return;
                }
                thrd.Abort();
                thrd = null;
                return;
            }
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            DoSomething(bw, null);
        }


        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            System.Windows.Forms.ListView lv = sender as System.Windows.Forms.ListView;
            // 检查点击的列是不是现在的排序列.
            if (e.Column == (lv.ListViewItemSorter as ListViewItemComparer).SortColumn)
            {
                // 重新设置此列的排序方法.
                if ((lv.ListViewItemSorter as ListViewItemComparer).Order == System.Windows.Forms.SortOrder.Ascending)
                {
                    (lv.ListViewItemSorter as ListViewItemComparer).Order = System.Windows.Forms.SortOrder.Descending;
                }
                else
                {
                    (lv.ListViewItemSorter as ListViewItemComparer).Order = System.Windows.Forms.SortOrder.Ascending;
                }
            }
            else
            {
                // 设置排序列，默认为正向排序
                (lv.ListViewItemSorter as ListViewItemComparer).SortColumn = e.Column;
                (lv.ListViewItemSorter as ListViewItemComparer).Order = System.Windows.Forms.SortOrder.Ascending;
            }
             // 用新的排序方法对ListView排序
             ((System.Windows.Forms.ListView)sender).Sort();

        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            ////OpenFileDialog ofd = new OpenFileDialog();
            ////ofd.Filter = "Excel文件(*.xls;*.xlsx)|*.xls;*.xlsx|所有文件|*.*";
            ////ofd.ValidateNames = true;
            ////ofd.CheckPathExists = true;
            ////ofd.CheckFileExists = true;
            ////if (ofd.ShowDialog() != DialogResult.OK)
            ////{
            ////    return;
            ////    //其他代码
            ////}
            ////string strFileName = ofd.FileName;
            CExcel ce = new CExcel();
            ce.ExportExcel(this.listView1);
        }

        private void tsmi_ExportCSV_Click(object sender, EventArgs e)
        {
            CExcel ce = new CExcel();
            ce.ExportCSV(this.listView1);
        }

        void tsmi_DisplayAll_Click(object sender, EventArgs e)
        {
            refreshExchangData("",ess, true);
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listView_ColumnClick(sender, e);
        }

        /// <summary>
        /// 滚动测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_roundTest_Click(object sender, EventArgs e)
        {
            if (this.runPlanPicker1.Plans.Length == 0)
            {
                return;
            }
            this.Cursor = Cursors.WaitCursor;
            SettingClass setting = new SettingClass();
            setting.GrownMaxVal = int.Parse(this.txt_GrownMaxVal.Text);
            setting.GrownMinVal = int.Parse(this.txt_GrownMinVal.Text);
            setting.DispRows = int.Parse(this.txt_MinCols.Text);
            setting.minColTimes = new int[10];
            for (int i = 0; i < 9; i++)
            {
                TextBox tb = this.Controls.Find(string.Format("txt_minColTimes{0}", i + 1), true)[0] as TextBox;
                setting.minColTimes[i] = int.Parse(tb.Text);
            }
            btc = new BackTestClass<T>(GlobalClass.TypeDataPoints[ddl_DataSource.SelectedValue.ToString()], chkb_useSpecList.Checked ? txt_SecPools.Text : "", chkb_useBechmark.Checked ? txt_benchMarkCode.Text : "", txt_begExpNo.Text, long.Parse(txt_LoopCnt.Text),0, setting);
            Assembly asmb = typeof(StragClass).Assembly;
            //////Type sct = asmb.GetType(ddl_StragName.SelectedValue.ToString());
            //////StragClass sc = Activator.CreateInstance(sct) as StragClass;
            BaseStragClass<T> sc = this.runPlanPicker1.Plans[0].PlanStrag as BaseStragClass<T>;
            sc.CommSetting = setting;
            sc.ChipCount = int.Parse(this.txt_ChipCnt.Text);
            sc.FixChipCnt = (this.txt_FixChipCnt.Text.Trim() == "0") ? false : true;
            sc.ReviewExpectCnt = int.Parse(this.txt_reviewExpCnt.Text);
            sc.InputMinTimes = int.Parse(this.txt_minInputTimes.Text);
            sc.InputMaxTimes = int.Parse(this.txt_maxInputTimes.Text);
            sc.ExcludeBS = this.chkb_exclueBS.Checked;
            sc.ExcludeSD = this.chkb_exclueSD.Checked;
            sc.BySer = this.chkb_bySer.Checked;
            sc.OnlyBS = this.chkb_onlyBS.Checked;
            sc.OnlySD = this.chkb_onlySD.Checked;
            this.runPlanPicker1.Plans[0].PlanStrag = sc as BaseStragClass<T>;
            RoundBackTestReturnClass<T> rbtr = null;
            try
            {
                int cycLong = int.Parse(txt_RoundCycLong.Text);
                int stepLong = int.Parse(txt_RoundStepLong.Text);
                rbtr = btc.RunRound(sc, cycLong, stepLong);
            }
            catch (Exception ce)
            {
                rbtr = new RoundBackTestReturnClass<T>();
                rbtr.Msg = ce.Message;
                rbtr.succ = false;
            }
            if (!rbtr.succ)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(rbtr.Msg);
                return;
            }
            listView3.Items.Clear();
            List<float> wins = rbtr.RoundWinRate;
            for (int i = 0; i < rbtr.RoundData.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = (i + 1).ToString();
                lvi.SubItems.Add(rbtr.RoundData[i].LoopCnt.ToString());
                lvi.SubItems.Add(rbtr.RoundData[i].ChanceList.Count.ToString());
                lvi.SubItems.Add(rbtr.RoundData[i].HoldCntDic[1].ToString());
                lvi.SubItems.Add(wins[i].ToString());
                listView3.Items.Add(lvi);
            }
            this.tabPage3.Show();
            this.Cursor = Cursors.Default;
            MessageBox.Show(rbtr.Msg);
        }

        private void listView3_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.listView_ColumnClick(sender, e);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.listView_ColumnClick(sender, e);
        }

        private void btn_CheckData_Click(object sender, EventArgs e)
        {
            //if (CheckFrm == null)
            //{
            CheckFrm = new MainForm<T>();
            CheckFrm.Show();
            return;
            //}
            ////CheckFrm.ReLoad();
            ////CheckFrm.Show();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
                return;

            //前提，listview禁止多选
            ListViewItem currentRow = listView1.SelectedItems[0];
            string strExpect = currentRow.SubItems[2].Text;
            //MessageBox.Show(strExpect);
            ////if (CheckFrm == null)
            ////{
            CheckFrm = new MainForm<T>();
            ////    CheckFrm.InputExpect = int.Parse(strExpect);
            ////    CheckFrm.Show();
            ////    return;
            ////}
            CheckFrm.InputExpect = long.Parse(strExpect);
            CheckFrm.Show();

        }

        /// <summary>
        /// 模拟回归
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_VirExchange_Click(object sender, EventArgs e)
        {
            this.dataGridView_ExchangeDetail.DataError += (a, b)=>{ };
            this.dataGridView_ProbData.DataError += (a, b) => { };
            
            if (this.ddl_DataSource.SelectedIndex < 0)
            {
                MessageBox.Show("请选择回测数据源！");
                return;
            }
            if (runPlanPicker1 == null || this.runPlanPicker1.Plans == null)
            {
                MessageBox.Show("请选择回测策略");
                return;
            }
            StragRunPlanClass<T>[] plans = this.runPlanPicker1.Plans as StragRunPlanClass<T>[];
            if (plans == null || plans.Length == 0)
                return;
            SCList = plans.ToList() as List<StragRunPlanClass<T>>;
            if (!RunVirExchange)
            {
                if (MessageBox.Show("确定开始模拟成交？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                wxLog("量化投研回测系统", "开始回测！");
                this.toolStripStatusLabel1.Text = "开始模拟交易！";
            }
            if (RunVirExchange)
            {
                if (MessageBox.Show("确定停止模拟成交？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                this.toolStripStatusLabel1.Text = "停止模拟交易！";
                try
                {
                    if (th.ThreadState == ThreadState.Running)
                    {
                        th.Abort();

                    }
                }
                catch
                {
                }
                th = null;
                //this.timer_Tip.Enabled = false;
                RunVirExchange = false;
                return;
            }
            try
            {
                this.RunVirExchange = true;
                DataTypePoint dtp = GlobalClass.TypeDataPoints[ddl_DataSource.SelectedValue.ToString()];
                //this.btn_VirExchange.Text = "停止";
                //this.Cursor = Cursors.WaitCursor;
                //es = new ExchangeService(int.Parse(this.txt_InitCash.Text),double.Parse(this.txt_Odds.Text));
                SettingClass setting = new SettingClass();
                setting.SetGlobalSetting(this.globalSetting);
                setting.GrownMaxVal = int.Parse(this.txt_GrownMaxVal.Text);
                setting.GrownMinVal = int.Parse(this.txt_GrownMinVal.Text);
                setting.DispRows = int.Parse(this.txt_MinCols.Text);
                setting.minColTimes = new int[10];
                setting.UseLocalWaveData = this.checkBox_UseBuffRsult.Checked;
                AssetUnitClass<T> auc = new AssetUnitClass<T>();
                if (Program<T>.AllSettings.AllAssetUnits.Count > 0)//如果存在资产管理单元，所有计划均绑定到第一个上进行测试
                {
                    auc = Program<T>.AllSettings.AllAssetUnits.Values.Last();

                }
                auc.TotalAsset = double.Parse(this.txt_InitCash.Text);
                //es = auc.getCurrExchangeServer();
                //if (es != null)
                //{
                //    es.Reset();
                //}
                //设置所有默认值
                if (ess == null)
                {
                    ess = new Hashtable();
                    //ess = new Dictionary<string, ExchangeService<T>>();
                }
                if (checkBox_MixAll.Checked)
                {
                    SCList.ForEach(p => p.AssetUnitInfo = auc);
                    ess.Add(SCList[0].AssetUnitInfo.UnitId, new ExchangeService<T>(dtp));
                }
                else
                {
                    //Series s = this.chart1.Series[0];
                    //this.chart1.Series.Clear();
                    SCList.ForEach(
                        a =>
                        {
                            AssetUnitClass<T> ac = new AssetUnitClass<T>();
                            ac.UnitName = a.Plan_Name;
                            ac.UnitId = Guid.NewGuid().ToString();
                            if (chkb_useOdds.Checked)
                            {
                                ac.Odds = double.Parse(txt_Odds.Text);
                            }
                            ac.TotalAsset = double.Parse(this.txt_InitCash.Text);
                            if (!Program<T>.AllSettings.AllAssetUnits.ContainsKey(ac.UnitId))
                            {
                                Program<T>.AllSettings.AllAssetUnits.Add(ac.UnitId, ac);
                            }
                            a.AssetUnitInfo = ac;
                            ExchangeService<T> es = ac.getCurrExchangeServer();
                            if (es != null)
                            {
                                es.Reset();
                            }
                            ess.Add(ac.UnitId, new ExchangeService<T>(dtp));
                        //Series cs = new Series(ac.UnitName);
                        //cs.ChartType = s.ChartType;
                        //cs.BorderWidth = new Random().Next(10);
                        //this.chart1.Series.Add(cs);
                        //ac.Run();
                    });
                }
                SCList.ForEach(p =>
                {
                    p.PlanStrag.CommSetting = setting;
                    p.Running = true;
                    p.AutoRunning = true;
                //p.PlanStrag.ReviewExpectCnt = int.Parse(this.txt_reviewExpCnt.Text);
            });
                if (this.ckb_useCondition.Checked)
                {
                    SCList.ForEach(a =>
                    {
                        a.PlanStrag.ReviewExpectCnt = int.Parse(this.txt_reviewExpCnt.Text);
                        a.PlanStrag.GetRev = this.chkb_getRev.Checked;
                        a.PlanStrag.InputMinTimes = int.Parse(this.txt_minInputTimes.Text);
                        a.PlanStrag.InputMaxTimes = int.Parse(this.txt_maxInputTimes.Text);
                        a.PlanStrag.FixChipCnt = (this.txt_FixChipCnt.Text.Trim() == "1");
                        a.PlanStrag.ChipCount = int.Parse(this.txt_ChipCnt.Text);
                    });
                }
                //SCList.ForEach(p => p.Running = true);
                //SCList.ForEach(p => p.AutoRunning = true);
                //SCList.ForEach(p => p.PlanStrag.ReviewExpectCnt = int.Parse(this.txt_reviewExpCnt.Text));
                //if (SCList.Count == 1)
                //{
                //    //SCList[0].PlanStrag = sc;
                //////SCList.ForEach(p=>p.IncreamType = this.checkBox_CreamModel.Checked ? InterestType.CompoundInterest : InterestType.SimpleInterest);

                //////////SCList.ForEach(p=>p.FixAmt = 1);
                //////////SCList.ForEach(p=>p.FixRate = 0.01);
                //////SCList.ForEach(p => p.AllowMaxHoldTimeCnt = int.Parse(txt_AllowMaxHoldTimeCnt.Text));
                //////if (SCList.Count == 1)
                //////{
                //////    SCList.ForEach(p => p.PlanStrag.ChipCount = int.Parse(this.txt_ChipCnt.Text));
                //////}
                //}
                #region 用新的计算进行回测
                ////////////////for (int i = 0; i < 9; i++)
                ////////////////{
                ////////////////    TextBox tb = this.Controls.Find(string.Format("txt_minColTimes{0}", i + 1), true)[0] as TextBox;
                ////////////////    setting.minColTimes[i] = int.Parse(tb.Text);
                ////////////////}
                ////////////////setting.MaxHoldingCnt = int.Parse(this.txt_MaxHoldChanceCnt.Text);
                ////////////////setting.Odds = double.Parse(this.txt_Odds.Text);
                ////////////////setting.InitCash = int.Parse(this.txt_InitCash.Text);
                ////////////////Assembly asmb = typeof(StragClass).Assembly;// Assembly.LoadFrom("EnterpriseServerBase.dll");
                ////////////////btc = new BackTestClass(long.Parse(txt_begExpNo.Text), long.Parse(txt_LoopCnt.Text), setting);
                ////////////////////Type sct = asmb.GetType(ddl_StragName.SelectedValue.ToString());


                ////////////////if (!checkBox_MixAll.Checked)
                ////////////////{
                ////////////////    ////sc = Activator.CreateInstance(sct) as StragClass;
                ////////////////    sc.CommSetting = setting;
                ////////////////    sc.ChipCount = int.Parse(this.txt_ChipCnt.Text);
                ////////////////    sc.FixChipCnt = (this.txt_FixChipCnt.Text.Trim() == "0") ? false : true;
                ////////////////    sc.ReviewExpectCnt = int.Parse(this.txt_reviewExpCnt.Text);
                ////////////////    sc.InputMinTimes = int.Parse(this.txt_minInputTimes.Text);
                ////////////////    if (sc.StagSetting.IsLongTermCalc)
                ////////////////    {
                ////////////////        sc.ReviewExpectCnt = sc.ReviewExpectCnt + sc.InputMinTimes;
                ////////////////    }
                ////////////////    sc.InputMaxTimes = int.Parse(this.txt_maxInputTimes.Text);
                ////////////////    sc.ExcludeBS = this.chkb_exclueBS.Checked;
                ////////////////    sc.ExcludeSD = this.chkb_exclueSD.Checked;
                ////////////////    sc.BySer = this.chkb_bySer.Checked;
                ////////////////    sc.OnlyBS = this.chkb_onlyBS.Checked;
                ////////////////    sc.OnlySD = this.chkb_onlySD.Checked;
                ////////////////    sc.GetRev = this.chkb_getRev.Checked;
                ////////////////    if (sc is IProbCheckClass)
                ////////////////    {
                ////////////////        (sc as IProbCheckClass).StdvCnt = double.Parse(this.txt_StdvCnt.Text);
                ////////////////    }
                ////////////////    //sc.AllowMaxHoldTimeCnt = int.Parse(this.txt_AllowMaxHoldTimeCnt.Text);
                ////////////////    sc.MinWinRate = double.Parse(this.txt_minRate.Text);
                ////////////////    ////double p = double.Parse(this.txt_ChipCnt.Text) / setting.Odds;
                ////////////////    ////double Normal_p = double.Parse(this.txt_ChipCnt.Text) / 10;
                ////////////////    ////double b = double.Parse(this.txt_Odds.Text);
                ////////////////    ////double q = 1 - p;
                ////////////////    sc.StagSetting = sc.getInitStagSetting();
                ////////////////    ////sc.StagSetting.BaseType.ChipRate = (p * b - q) / b;
                ////////////////    //sc.StagSetting.BaseType.IncrementType = this.checkBox_CreamModel.Checked ? InterestType.CompoundInterest : InterestType.SimpleInterest;

                ////////////////    if (setting.UseLocalWaveData && sc is ProbWaveSelectStragClass)//是长期概率分布类
                ////////////////    {
                ////////////////        DataTable dt = new PK10ProbWaveDataInterface().GetProWaveResult(long.Parse(txt_begExpNo.Text));
                ////////////////        (sc as ProbWaveSelectStragClass).LocalWaveData = new GuideResult(dt);
                ////////////////    }
                ////////////////    StragRunPlanClass sr = new StragRunPlanClass();
                ////////////////    sr.PlanStrag = sc;
                ////////////////    sr.IncreamType = this.checkBox_CreamModel.Checked ? InterestType.CompoundInterest : InterestType.SimpleInterest;
                ////////////////    sr.AutoRunning = true;
                ////////////////    sr.FixAmt = 1;
                ////////////////    sr.FixRate = 0.01;
                ////////////////    SCList.Add(sr);
                ////////////////}
                ////////////////else
                ////////////////{
                ////////////////    for (int i = 2; i < 9; i++)
                ////////////////    {
                ////////////////        for (int j = 0; j < 2; j++)
                ////////////////        {
                ////////////////            StragClass sobj = Activator.CreateInstance(sct) as StragClass;
                ////////////////            sobj.CommSetting = setting;
                ////////////////            sobj.ChipCount = i;
                ////////////////            sobj.FixChipCnt = (this.txt_FixChipCnt.Text.Trim() == "0") ? false : true;
                ////////////////            sobj.ReviewExpectCnt = sobj.CommSetting.minColTimes[i - 1] + 1;
                ////////////////            sobj.BySer = j==1;
                ////////////////            sobj.MinWinRate = double.Parse(this.txt_minRate.Text);
                ////////////////            //sobj.AllowMaxHoldTimeCnt = 100;
                ////////////////            sobj.StagSetting = sobj.getInitStagSetting();
                ////////////////            //sobj.StagSetting.BaseType.IncrementType = this.checkBox_CreamModel.Checked ? InterestType.CompoundInterest : InterestType.SimpleInterest;
                ////////////////            StragRunPlanClass sr = new StragRunPlanClass();
                ////////////////            sr.PlanStrag = sobj;
                ////////////////            sr.IncreamType = this.checkBox_CreamModel.Checked ? InterestType.CompoundInterest : InterestType.SimpleInterest;
                ////////////////            sr.AutoRunning = true;
                ////////////////            sr.FixAmt = 1;
                ////////////////            sr.FixRate = 0.01;
                ////////////////            SCList.Add(sr);
                ////////////////        }
                ////////////////    }
                ////////////////}
                //////////////////sc.MinWinRate = double.Parse(this.txt_Odds.Text) *double.Parse(this.txt_minRate.Text);
                //////////////////凯利公式 (p*b-q)/b
                //////////////////凯利公式 (p*b-q)/b
                //////////////////////DialogResult rs = MessageBox.Show(sc.StagSetting.BaseType.ChipRate.ToString(), "胜率", MessageBoxButtons.OKCancel) ;
                //////////////////////if (rs == DialogResult.Cancel)
                //////////////////////{
                //////////////////////    return;
                //////////////////////}
                #endregion
                //this.Cursor = Cursors.WaitCursor;
                this.chart1.Series.Clear();
                //this.chart1.DataSource = null;
                //this.timer_Tip.Interval = int.Parse(txt_Timer_Interval.Text) * 1000;
                //timer_Tip.Tick += new EventHandler(timer_Tip_Tick);
                //this.timer_Tip.Enabled = true;
                //this.timer_Tip.Enabled = true;

                //this.timer_Tip_Tick(null, null);
                //////try
                //////{
                string BegT = "";
                string EndT = "";

                if (ddl_DataSource.SelectedValue.ToString().Equals("CN_Stock_A"))
                {
                    DateTime dtbeg = DateTime.Parse(this.txt_begExpNo.Text);
                    DateTime dtend = DateTime.Parse(this.txt_endExpNo.Text);
                    BegT = dtbeg.WDDate();
                    EndT = dtend.WDDate();
                }
                else
                {
                    BegT = this.txt_begExpNo.Text;
                    EndT = this.txt_endExpNo.Text;
                }
                int maxReviews = SCList.Max(a => a.PlanStrag.ReviewExpectCnt);


                if (dtp.IsSecurityData == 1)
                {
                    WDDataInit<T>.vipDocRoot = Program<T>.AllSettings.gc.VipDocRootPath;
                    if (WDDataInit<T>.AllDays == null)
                    {
                        WDDataInit<T>.Init();
                    }
                    DateTime[] alldate = WDDataInit<T>.AllDays;
                    int index = alldate.IndexOf(BegT);
                    if (index < 0)
                        BegT = alldate.First().WDDate();
                    else
                        BegT = alldate[Math.Max(0, index - maxReviews)].WDDate();
                }
                if (btc == null || true)
                {
                    btc = new BackTestClass<T>(dtp, chkb_useSpecList.Checked ? txt_SecPools.Text : "", chkb_useBechmark.Checked ? txt_benchMarkCode.Text : "", BegT, long.Parse(txt_LoopCnt.Text) , maxReviews, setting, EndT);
                }
                btc.useBegExpect = this.txt_begExpNo.Text;//对股票必须要指定，实际必须从此时开始
                Application.DoEvents();
                th = new Thread(RunVirtual);
                th.Start();
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
            return;
        }
        ConcurrentDictionary<string, int> counter = new ConcurrentDictionary<string, int>();
        void RunVirtual()
        {
            try
            {
                Dictionary<string, CalcStragGroupClass<T>> rest = null;
                Program<T>.AllSettings.AllRunningPlanGrps = InitServerClass<T>.InitCalcStrags(GlobalClass.TypeDataPoints[this.ddl_DataSource.SelectedValue.ToString()], ref rest, Program<T>.AllSettings.AllStrags, SCList.ToDictionary(p => p.GUID, p => p ), Program<T>.AllSettings.AllAssetUnits, true, true);//注入plans
                btc.FinishedProcess = new SuccEvent(Finished);
                btc.ExpectProcessedEvent = (ex,es) => {
                    refreshExchangSeriveData(ex,es);
                };
                btc.StagInterProcessEvent = (strage,expect, code)=>
                {
                    //return;
                    string str = string.Format("{0}_{1}", strage, expect);
                    int curr = 0;
                    if (counter.ContainsKey(str))
                    {
                        curr = counter[str];
                    }
                    else
                    {
                        counter.TryAdd(str, curr);
                    }
                    curr++;
                    counter[str] = curr;
                    CallControlToWork(this.statusStrip1, (c, objs) =>
                    {
                        StatusStrip ctrl = c as StatusStrip;
                        //ctrl.Items[1].Text = string.Format("{0}策略第{1}个{2}", strage, curr, code);
                        int pre = (int)(100*curr/WDDataInit<T>.AllSecurities.Count);
                        (ctrl.Items[2] as ToolStripProgressBar).Value = pre;

                        Application.DoEvents();
                    },new object[] { });
                    
                };
                btc.ExpectTip = (ex,index, ess) =>
                {
                    if (ess == null)
                        return;
                    foreach (string key in ess.Keys)
                    {
                        CallControlToWork(this.statusStrip1, (c, objs) =>
                        {
                            ExchangeService<T> es = ess[key] as ExchangeService<T>;
                            StatusStrip ctrl = c as StatusStrip;
                            
                            string lastExpect = ex;
                            if (string.IsNullOrEmpty(lastExpect))
                            {
                                var items = es.getMoneys().OrderBy(a=>a.Key);
                                
                                if (items != null && items.Count() > 0)
                                    lastExpect = items.Last().Key;
                            }
                            wxLog(lastExpect, string.Format("第{3}次,当前收益率:{0};当前资产数量:{1}份;当前资产余额:{2}万", es?.GainedRate.ToString()??"0",es.getAssetCount(),(es.getAsset()/10000).ToEquitPrice(2),index));
                            return;
                            ctrl.Items[0].Text = string.Format("第{2}次,{0}/{1} {3}", es.CurrIndex.ToString(), es.ExpectCnt, btc.testIndex, lastExpect);
                            ctrl.Items[1].Text = string.Format("{0}% 最大值:[{1}%]   最小值:[{2}%] ", es.GainedRate.ToString(), es.MaxRate, es.MinRate);
                        }, new object[] { });
                    }
                };
                ret = btc.VirExchange(Program<T>.AllSettings as ServiceSetting<T>, ref ess, SCList.ToArray(), chkb_useSpecList.Checked ? txt_SecPools.Text : "");
            }
            catch (Exception ce)
            {
                MessageBox.Show(ce.StackTrace, ce.Message);
            }

            //MessageBox.Show(es.summary.ToString());
            //DataView moneyLines = new DataView(es.MoneyIncreamLine);
            //this.chart1.Series[0].Points.DataBindXY(moneyLines, "id", moneyLines, "val");
            //btn_VirExchange.Text = "模拟交易";
            //MessageBox.Show("执行完成！");
            this.RunVirExchange = false;
        }

        void wxLog(string topic, string msg)
        {
            try
            {
                Program<T>.AllSettings.wxlog?.Log(topic, msg, string.Format(Program<T>.AllSettings?.gc?.WXLogUrl, Program<T>.AllSettings?.gc?.WXSVRHost));
            }
            catch (Exception ce)
            {

            }
        }

        private void timer_Tip_Tick(object sender, EventArgs e)
        {

            this.timer_Tip.Interval = int.Parse(txt_Timer_Interval.Text) * 1000;
            return;
            refreshData(false);
        }
        void CallControlToWork(Control ctrl,Action<Control,object[]> action, object[] objs)
        {
            ctrl.Invoke(new SetControlCallback(action), new object[] { ctrl,objs});
        }

        void refreshData(bool forceDisplay=false)
        {
            try
            {
                DataTable dt = btc.SystemStdDevs;
                if (dt.Rows.Count > 0)
                {
                    string strExpect = dt.Rows[dt.Rows.Count - 1]["Expect"].ToString();
                    Int64 MinExpect = Int64.Parse(strExpect) - 180;
                    string sql = string.Format("Expect>={0}", MinExpect);
                    DataView dv_stddev = new DataView(dt);
                    dv_stddev.RowFilter = sql;
                    if (this.chart_ForSystemStdDev.Series.Count < dt.Columns.Count - 1)
                    {
                        this.chart_ForSystemStdDev.Series.Clear();
                        for (int i = 1; i < dt.Columns.Count; i++)
                        {
                            string strName = dt.Columns[i].ColumnName;
                            Series sr = new Series(strName);
                            sr.ChartType = SeriesChartType.Line;
                            if (strName == "StdDev" || strName == "StdMa20" || strName == "StdMa5")
                            {
                                sr.BorderWidth = 3;
                            }
                            //sr.IsValueShownAsLabel = true;
                            this.chart_ForSystemStdDev.Series.Add(sr);
                        }
                        this.chart_ForSystemStdDev.GetToolTipText += new EventHandler<ToolTipEventArgs>(chart_ForSystemStdDev_GetToolTipText);
                    }
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        this.chart_ForSystemStdDev.Series[i - 1].Points.DataBindXY(dv_stddev, dt.Columns[0].ColumnName, dv_stddev, dt.Columns[i].ColumnName);
                    }
                    //this.chart_ForSystemStdDev.DataSource = dv_stddev;

                    //this.chart_ForSystemStdDev.Series[0].Points.DataBindXY(dv_stddev, "Expect", dv_stddev, "StdDev");
                    //this.chart_ForSystemStdDev.Series[1].Points.DataBindXY(dv_stddev, "Expect", dv_stddev, "StdMa20");
                    //this.chart_ForSystemStdDev.Series[2].Points.DataBindXY(dv_stddev, "Expect", dv_stddev, "StdMa5");
                    this.chart_ForSystemStdDev.ChartAreas[0].AxisY.Maximum = 0.6;
                    this.chart_ForSystemStdDev.ChartAreas[0].AxisY.Minimum = 0.1;
                    this.chart_ForSystemStdDev.Show();


                    this.dg_forStdDev.DataSource = dv_stddev;
                    this.dg_forStdDev.Tag = dt;
                    this.dg_forStdDev.Refresh();
                    Application.DoEvents();
                }
                lock (ess)
                {
                    int ci = 0;
                    foreach (string key in ess.Keys)
                    {
                        ExchangeService<T> es = ess[key] as ExchangeService<T>;
                        this.toolStripStatusLabel1.Text = string.Format("第{2}次,{0}/{1}", es.CurrIndex.ToString(), es.ExpectCnt, btc.testIndex);
                        this.toolStripStatusLabel2.Text = string.Format("{0}% 最大值:[{1}%]   最小值:[{2}%] ", es.GainedRate.ToString(), es.MaxRate, es.MinRate);
                        DataTable copyDt = es.MoneyIncreamLine.Copy();
                        copyDt.Columns.Add("point", typeof(string));
                        for (int i = 0; i < copyDt.Rows.Count; i++)
                        {
                            if (this.ddl_DataSource.SelectedValue.ToString() != "PK10")
                                copyDt.Rows[i]["point"] = copyDt.Rows[i]["id"];//.ToString().Substring(0, 8);
                            else
                                copyDt.Rows[i]["point"] = copyDt.Rows[i]["id"];
                        }
                        DataView moneyLines = new DataView(copyDt);
                        if (this.chart1.Series.Count < 3 * ci + 3)
                        {
                            Series ss = new Series();
                            ss.Name = string.Format("{0}[Total]", es.getCurrAsset().UnitName);
                            ss.ChartType = SeriesChartType.Line;
                            ss.BorderWidth = (ci + 1);
                            this.chart1.Series.Add(ss);
                            ss = new Series();
                            ss.Name = string.Format("{0}[Cash]", es.getCurrAsset().UnitName);
                            ss.ChartType = SeriesChartType.Line;
                            ss.BorderWidth = (ci + 1);
                            this.chart1.Series.Add(ss);
                            ss = new Series();
                            ss.Name = string.Format("{0}[Asset]", es.getCurrAsset().UnitName);
                            ss.ChartType = SeriesChartType.Line;
                            ss.BorderWidth = (ci + 1);
                            this.chart1.Series.Add(ss);
                        }
                        this.chart1.Series[ci * 3].Points.DataBindXY(moneyLines, "point", moneyLines, "val");
                        this.chart1.Series[ci * 3].ToolTip = "期号:#VALX;当前值:#VAL";
                        this.chart1.Series[ci * 3 + 1].Points.DataBindXY(moneyLines, "point", moneyLines, "Cash");
                        this.chart1.Series[ci * 3 + 1].ToolTip = "期号:#VALX;当前值:#VAL";
                        this.chart1.Series[ci * 3 + 2].Points.DataBindXY(moneyLines, "point", moneyLines, "Asset");
                        this.chart1.Series[ci * 3 + 2].ToolTip = "期号:#VALX;当前值:#VAL";
                        //this.chart1.Series[ci].Name = SCList[0].AssetUnitInfo.UnitName;
                        ci++;
                        if (!this.chkb_noDetailTable.Checked||forceDisplay)
                            loadTableData(es);
                    }
                    chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;
                }

            }
            catch (Exception ce)
            {
                string msg = ce.Message;
            }
        }
        List<Task> tasks = new List<Task>(); 
        void refreshExchangSeriveData(string ex,Hashtable ess)
        {
            if(tasks.Count== 0)
            {
                Task task = Task.Factory.StartNew(() => {
                    try
                    {
                        refreshExchangData(ex,ess, false);
                    }
                    catch
                    {

                    }
                    tasks.Clear();
                });
                tasks.Add(task);
            }

        }
        void refreshExchangData(string ex,Hashtable ess, bool displayAll = false)
        {
            DataView moneyLines = null;
            try
            { 
                int ci = 0;
                foreach (string key in ess.Keys)
                {
                    ExchangeService<T> es = ess[key] as ExchangeService<T>;
                    
                    //this.toolStripStatusLabel1.Text = string.Format("第{2}次,{0}/{1}", es.CurrIndex.ToString(), es.ExpectCnt, btc.testIndex);
                    //this.toolStripStatusLabel2.Text = string.Format("{0}% 最大值:[{1}%]   最小值:[{2}%] ", es.GainedRate.ToString(), es.MaxRate, es.MinRate);

                    DataTable copyDt = es.MoneyIncreamLine.Copy();
                    copyDt.Columns.Add("point", typeof(string));
                    for (int i = 0; i < copyDt.Rows.Count; i++)
                    {
                        if (this.ddl_DataSource.SelectedValue.ToString() != "PK10")
                            copyDt.Rows[i]["point"] = copyDt.Rows[i]["id"];//.ToString().Substring(0, 8);
                        else
                            copyDt.Rows[i]["point"] = copyDt.Rows[i]["id"];
                    }
                    moneyLines = new DataView(copyDt);
                    moneyLines.Sort = "point";
                    copyDt = null;
                    chart1.Tag = moneyLines;
                    if (displayAll)
                    {
                        CallControlToWork(chart1, refreshGainedRateChart, new object[] { ci, es, moneyLines });
                        
                    }
                    ci++;
                    if (!this.chkb_noDetailTable.Checked)
                    {
                        CallControlToWork(dataGridView_ExchangeDetail, (ctrl, objs) =>
                        {
                            DataGridView dgv = ctrl as DataGridView;
                            ExchangeService<T> ces = objs[0] as ExchangeService<T>;
                            if(ces.ExchangeDetail == null|| ces.ExchangeDetail.Columns.Count == 0)
                            {
                                return;
                            }
                            
                            bool dAll = (bool)objs[1];
                            DataView dv = new DataView(ces.ExchangeDetail);
                            if (!dAll)
                                dv.RowFilter = "Closed=0";

                            try
                            {
                                if (dv != null && dv.Count > 0)
                                {
                                    dgv.DataError += new DataGridViewDataErrorEventHandler((s, e) => { });
                                    if (dAll)
                                    {
                                        dgv.DataSource = dv;
                                        Application.DoEvents();
                                    }
                                        //dgv.Refresh();
                                    //Application.DoEvents();
                                }
                            }
                            catch (Exception ce)
                            {

                            }
                            finally
                            {
                                dv = null;
                            }
                            
                            
                        }, new object[] { es, displayAll });
                    }                
                }
            }
            catch(Exception ce)
            {

            }
            finally
            {
                moneyLines = null;
            }
        }

        void refreshGainedRateChart(Control ctrl,object[] objs)
        {
            DataView moneyLines = objs[2] as DataView;
            try
            {
                Chart chart1 = ctrl as Chart;
                int ci = (int)objs[0];
                ExchangeService<T> es = objs[1] as ExchangeService<T>;
                
                if (chart1.Series.Count < 4 * ci + 4)
                {
                    Series ss = new Series();
                    ss.Name = string.Format("{0}[合计]", es.getCurrAsset().UnitName);
                    ss.ChartType = SeriesChartType.Line;
                    ss.BorderWidth = (ci + 1);
                    chart1.Series.Add(ss);
                    ss = new Series();
                    ss.Name = string.Format("{0}[现金]", es.getCurrAsset().UnitName);
                    ss.ChartType = SeriesChartType.Line;
                    ss.BorderWidth = (ci + 1);
                    this.chart1.Series.Add(ss);
                    ss = new Series();
                    ss.Name = string.Format("{0}[资产]", es.getCurrAsset().UnitName);
                    ss.ChartType = SeriesChartType.Line;
                    ss.BorderWidth = (ci + 1);
                    this.chart1.Series.Add(ss);
                    ss = new Series();
                    ss.Name = string.Format("{0}[参考标的]", es.getCurrAsset().UnitName);
                    ss.ChartType = SeriesChartType.Line;
                    ss.BorderWidth = (ci + 1);
                    this.chart1.Series.Add(ss);
                }
                chart1.Series[ci * 4].Points.DataBindXY(moneyLines, "point", moneyLines, "val");
                chart1.Series[ci * 4].ToolTip = "期号:#VALX;当前值:#VAL";
                chart1.Series[ci * 4 + 1].Points.DataBindXY(moneyLines, "point", moneyLines, "Cash");
                chart1.Series[ci * 4 + 1].ToolTip = "期号:#VALX;当前值:#VAL";
                chart1.Series[ci * 4 + 2].Points.DataBindXY(moneyLines, "point", moneyLines, "Asset");
                chart1.Series[ci * 4 + 2].ToolTip = "期号:#VALX;当前值:#VAL";
                chart1.Series[ci * 4 + 3].Points.DataBindXY(moneyLines, "point", moneyLines, "BenchMark");
                chart1.Series[ci * 4 + 3].ToolTip = "期号:#VALX;当前值:#VAL";
                //chart1.Tag = moneyLines;
                //this.chart1.Series[ci].Name = SCList[0].AssetUnitInfo.UnitName;
            }
            catch(Exception ce)
            {

            }
            finally
            {
                moneyLines = null;
            }


        }

        void loadTableData(ExchangeService<T> es)
        {
            try
            {
                lock (es.ExchangeDetail)
                {
                    
                    DataView vExchangeDetail = new DataView(es.ExchangeDetail);
                    //vExchangeDetail.Sort = "id desc";
                    this.dataGridView_ExchangeDetail.DataSource = vExchangeDetail;
                    this.dataGridView_ExchangeDetail.Refresh();
                    Application.DoEvents();
                    if (SCList.Count > 0)
                    {
                        if (SCList[0].PlanStrag.StagSetting.BaseType.traceType == TraceType.WaveTrace)
                        {
                            ProbWaveSelectStragClass pss = SCList[0].PlanStrag as ProbWaveSelectStragClass;
                            if (pss.BaseWaves().Tables.Count == 0)
                            {
                                return;
                            }
                            GuideResult gs = pss.BaseWaves().Tables[0] as GuideResult;
                            GuideResultSet gss = pss.GuideWaves();
                            Int32 rcnt = gs.Rows.Count;
                            if (!checkBox_UseBuffRsult.Checked)//如果不用本地数据
                            {
                                if (rcnt > 0 && (rcnt % 100) < 20 && rcnt > lastSaveCnt + 50)
                                {
                                    //保存一次数据
                                    retData = gs;
                                    lastSaveCnt = rcnt;
                                    new Thread(SaveWaveTable).Start();
                                }
                            }
                            DataView dvWv = new DataView(gs);
                            //this.chart_ForProb.ChartAreas[0].AxisY.MaximumAutoSize = false;
                            if (this.chart_ForProb.Series.Count < 3)
                            {
                                this.chart_ForProb.Series.Add(new Series() { Name = "Cash", ChartType = SeriesChartType.Line });
                                this.chart_ForProb.Series.Add(new Series() { Name = "Asset", ChartType = SeriesChartType.Line });
                            }
                            this.chart_ForProb.ChartAreas[0].AxisY.Maximum = gs.High;
                            this.chart_ForProb.ChartAreas[0].AxisY.Minimum = gs.Low;
                            this.chart_ForProb.Series[0].Points.DataBindXY(dvWv, "Id", dvWv, "val");
                            this.chart_ForProb.Series[1].Points.DataBindXY(dvWv, "Id", dvWv, "Cash");
                            this.chart_ForProb.Series[2].Points.DataBindXY(dvWv, "Id", dvWv, "Asset");
                            if (this.chart_ForProb.Series.Count < gss.Tables.Count + 1)
                            {
                                for (int i = this.chart_ForProb.Series.Count; i <= gss.Tables.Count; i++)
                                {
                                    Series si = new Series(string.Format("指标{0}", i));
                                    si.ChartType = SeriesChartType.Line;
                                    this.chart_ForProb.Series.Add(si);

                                }
                            }
                            for (int i = 0; i < gss.Tables.Count; i++)
                            {

                                DataView dv = new DataView(gss.Tables[i]);
                                this.chart_ForProb.Series[1 + i].Points.DataBindXY(dv, "Id", dv, "val");
                            }
                            this.dataGridView_ProbData.DataSource = dvWv;
                            Application.DoEvents();
                        }
                    }
                }
            }
            catch(Exception ce)
            {

            }
        }

        void chart_ForSystemStdDev_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            System.Windows.Forms.DataVisualization.Charting.HitTestResult Result = new System.Windows.Forms.DataVisualization.Charting.HitTestResult();

            Result = chart_ForSystemStdDev.HitTest(e.X, e.Y);
            Series sr = Result.Series;
            if (Result.PointIndex < 0)
            {
                return;
            }
            if (sr != null)
            {
                e.Text = string.Format("第{0}道:{1}", sr.Name, sr.Points[Result.PointIndex].YValues[0]);
            }
            ////Result.Series;// 就是点击获得的Series
            ////Result.Series.Points[Result.PointIndex].XValue;// 为点击Series X坐标
            ////Result.Series.Points[Result.PointIndex].YValue;// 为点击Series
        }

        


        private void contextMenuStrip_ForListView_Opening(object sender, CancelEventArgs e)
        {

        }

        void SaveWaveTable()
        {
            if (retData == null) return;
            PK10ProbWaveDataInterface<T> pwinter = new PK10ProbWaveDataInterface<T>();
            pwinter.SaveProbWaveResult(retData);
        }

        private void btn_CalcEr_Click(object sender, EventArgs e)
        {
            frm_CalcEr frm = new frm_CalcEr();
            frm.Show();
        }

        private void txt_begExpNo_DoubleClick(object sender, EventArgs e)
        {
            if(GlobalClass.TypeDataPoints.First().Value.IsSecurityData==1)
            {
                txt_endExpNo.Text = txt_begExpNo.Text.ToDate().AddYears(1).AddDays(-1).WDDate();
            }
            else
                txt_begExpNo.Text = "20140101001";
        }

        private void txt_begExpNo_Click(object sender, EventArgs e)
        {
            //txt_begExpNo.Text = string.Format("{0}", Int64.Parse(txt_begExpNo.Text) - 10000);
        }

        private void btn_DistrCheck_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView_ExchangeDetail_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataGridView_ExchangeDetail.CurrentRow.Index < 0)
                return;
            
            //前提，listview禁止多选
            DataGridViewRow dr = this.dataGridView_ExchangeDetail.CurrentRow;
            //ChanceCode,ExpectNo,Amount,Status,EndExpectNO,EndPrice
            string strExpect = dr.Cells["ExpectNo"].Value.ToString();
            string endExpect = dr.Cells["EndExpectNO"].Value.ToString();
            string code = dr.Cells["ChanceCode"].Value.ToString();
            if (GlobalClass.TypeDataPoints.First().Value.IsSecurityData == 0)
            {
                CheckFrm = new MainForm<T>();
                CheckFrm.InputExpect = long.Parse(strExpect);
                CheckFrm.Show();
            }
            else
            {
                if (SecCheckFrm == null)
                {
                    SecCheckFrm = new Form1<T>(code, strExpect.ToDate().WDDate(), endExpect.ToDate().WDDate(), 300);
                    SecCheckFrm.Show();
                }
                else
                {
                    SecCheckFrm.QuerySec(code, strExpect.WDDate(), endExpect, 300);
                    SecCheckFrm.Show();
                    SecCheckFrm.Focus();
                }
                
                Application.DoEvents();
            }
        }

        private void dg_forStdDev_DoubleClick(object sender, EventArgs e)
        {
            if (this.dg_forStdDev.CurrentRow.Index < 0)
                return;

            //前提，listview禁止多选
            DataGridViewRow dr = this.dg_forStdDev.CurrentRow;
            string strExpect = dr.Cells["Expect"].Value.ToString();
            //MessageBox.Show(strExpect);
            ////if (CheckFrm == null)
            ////{
            CheckFrm = new MainForm<T>();
            ////    CheckFrm.InputExpect = int.Parse(strExpect);
            ////    CheckFrm.Show();
            ////    return;
            ////}
            CheckFrm.InputExpect = int.Parse(strExpect);
            CheckFrm.Show();
        }

        private void btn_trainPlan_Click(object sender, EventArgs e)
        {
            frm_TrainForm<TimeSerialData> frm = new frm_TrainForm<TimeSerialData>();
            frm.ShowDialog();

        }

        private void ddl_DataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_DataSource.SelectedIndex < 0)
                return;
            string txtsrc = ddl_DataSource.SelectedValue.ToString();
            if (txtsrc.Equals("CN_Stock_A"))
            {
                this.txt_begExpNo.Text = "2008-1-1";
                this.txt_endExpNo.Text = "2018-12-31";
            }
        }

        private void BackTestFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                thrd.Abort();
                thrd = null;
                //this.timer_Tip.Enabled = false;
                //this.timer_Tip = null;
            }
            catch (Exception ce)
            {

            }

        }

        private void btn_singleTest_Click(object sender, EventArgs e)
        {
            frm_MoniteStrag<T> frm = new frm_MoniteStrag<T>();
            frm.Show();
        }

        private void txt_endExpNo_DoubleClick(object sender, EventArgs e)
        {
            this.txt_begExpNo.Text = this.txt_endExpNo.Text.ToYearStart();
        }
    }


    public class ListViewItemComparer : System.Collections.IComparer
    {
        public int SortColumn;
        public SortOrder OrderOfSort;
        private System.Collections.CaseInsensitiveComparer ObjectCompare;
        public ListViewItemComparer()
        {
            ObjectCompare = new System.Collections.CaseInsensitiveComparer();
        }

        /// <summary>
        /// 获取或设置排序方式.
        /// </summary>
        public System.Windows.Forms.SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

        public int Compare(object x, object y)
        {
            int returnVal = -1;
            DateTime xtime, ytime;
            long xlong, ylong;
            string xin, yin;
            xin = ((ListViewItem)x).SubItems[SortColumn].Text;
            yin = ((ListViewItem)y).SubItems[SortColumn].Text;
            if (DateTime.TryParse(xin, out xtime) && DateTime.TryParse(yin, out ytime))
            {
                returnVal = DateTime.Compare(xtime, ytime);
            }
            else if (long.TryParse(xin, out xlong) && long.TryParse(yin, out ylong))
            {
                returnVal = (int)(xlong - ylong);
            }
            else
            {
                returnVal = ObjectCompare.Compare(((ListViewItem)x).SubItems[SortColumn].Text,
                ((ListViewItem)y).SubItems[SortColumn].Text);
            }
            if (OrderOfSort == System.Windows.Forms.SortOrder.Ascending)
                return returnVal;
            else
                return -1 * returnVal;
        }
    }

    delegate void SetControlCallback(Control ctrl, object[] obj);
}

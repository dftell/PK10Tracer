using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using PK10CorePress;
using BackTestLib;
using Strags;
using PK10Server;
using ExchangeLib;
using System.Threading;
using GuideLib;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.DataVisualization.Charting.ChartTypes;
using ServerInitLib;
//using Strags;
namespace BackTestSys
{
    public partial class BackTestFrm : Form
    {
        BackgroundWorker bw;
        BackTestReturnClass ret=null;
        BackTestClass btc = null;
        MainForm CheckFrm;
        ExchangeService es;
        StragClass sc;
        Thread th = null;
        GlobalClass globalSetting = new GlobalClass();
        bool _RunVirExchange;
        List<StragRunPlanClass> SCList = new List<StragRunPlanClass>();
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
                if (_RunVirExchange)
                {
                    this.chart1.Series[0].Points.Clear();//DataBindXY(moneyLines, "id", moneyLines, "val");
                    this.dataGridView_ExchangeDetail.DataSource = null;
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
            this.listView2.Columns.Add("持续次数",200);
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
            this.dataGridView_ProbData.ContextMenuStrip = contextMenuStrip_ForListView;
            this.listView2.ContextMenuStrip = contextMenuStrip_ForListView;
            this.listView3.ContextMenuStrip = contextMenuStrip_ForListView;
            this.listView1.ContextMenuStrip = contextMenuStrip_ForListView;
            this.dataGridView_ExchangeDetail.ContextMenuStrip = contextMenuStrip_ForListView;
            CheckForIllegalCrossThreadCalls = false;
        }

        void tsmi_ExportExcel_Click(object sender, EventArgs e)
        {
            object src = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl;
            if (src == null) return ;
            //ListView lv = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as ListView;
            //if (lv == null) return;
            CExcel ce = new CExcel();
            if (src is DataGridView)
                ce.ExportExcel(src as DataGridView);
            else if (src is ListView)
                ce.ExportExcel(src as ListView);
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

        void Finished()
        {
            this.timer_Tip.Enabled = false;
            MessageBox.Show("执行完毕！");
        }

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
            btc = new BackTestClass(long.Parse(txt_begExpNo.Text), long.Parse(txt_LoopCnt.Text), setting);
            this.listView1.Items.Clear();
            this.listView2.Items.Clear();
            this.listView3.Items.Clear();
            StragRunPlanClass[] plans = this.runPlanPicker1.Plans;
            if (plans.Length == 0)
                return;
            SCList = plans.ToList();
            SCList.ForEach(p => p.PlanStrag.CommSetting = setting);
            SCList.ForEach(p => p.PlanStrag.ChipCount = int.Parse(this.txt_ChipCnt.Text));
            SCList.ForEach(p => p.Running = true);
            SCList.ForEach(p => p.AutoRunning = true);
            SCList.ForEach(p => p.FixAmt = 1);
            SCList.ForEach(p => p.FixRate = 0.01);
            sc = SCList[0].PlanStrag;
            this.Cursor = Cursors.WaitCursor;
            timer_Tip.Tick += new EventHandler(RefreshList);
            this.timer_Tip.Interval = int.Parse(txt_Timer_Interval.Text) * 1000;
            this.timer_Tip.Enabled = true;
            Thread thrd = null;
            try
            {

                btc.FinishedProcess = new SuccEvent(Finished);
                btc.teststrag = sc;
                thrd = new Thread(new ThreadStart(btc.Run));
                thrd.Start();
                
            }
            catch (Exception ce)
            {
                ret = new BackTestReturnClass();
                ret.ChanceList = new List<ChanceClass>();
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
                    double dr = (float)ret.HoldCntDic[key] / ((float)ret.LoopCnt / 180);
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
                this.timer_Tip.Enabled = false;
            }
        }

        private void btn_startTest_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定开始回测？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            DoSomething(bw,null);
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
            btc = new BackTestClass(long.Parse(txt_begExpNo.Text), long.Parse(txt_LoopCnt.Text), setting);
            Assembly asmb = typeof(StragClass).Assembly;
            //////Type sct = asmb.GetType(ddl_StragName.SelectedValue.ToString());
            //////StragClass sc = Activator.CreateInstance(sct) as StragClass;
            StragClass sc = this.runPlanPicker1.Plans[0].PlanStrag;
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
            this.runPlanPicker1.Plans[0].PlanStrag = sc;
            RoundBackTestReturnClass rbtr = null;
            try
            {
                int cycLong = int.Parse(txt_RoundCycLong.Text);
                int stepLong = int.Parse(txt_RoundStepLong.Text);
                rbtr = btc.RunRound(sc, cycLong, stepLong);
            }
            catch (Exception ce)
            {
                rbtr = new RoundBackTestReturnClass();
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
            for (int i = 0;i< rbtr.RoundData.Count; i++)
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
                CheckFrm = new MainForm();
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
                CheckFrm = new MainForm();
            ////    CheckFrm.InputExpect = int.Parse(strExpect);
            ////    CheckFrm.Show();
            ////    return;
            ////}
            CheckFrm.InputExpect = int.Parse(strExpect);
            CheckFrm.Show();
            
        }

        /// <summary>
        /// 模拟回归
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_VirExchange_Click(object sender, EventArgs e)
        {
            StragRunPlanClass[] plans = this.runPlanPicker1.Plans;
            if (plans == null || plans.Length == 0)
                return;
            SCList = plans.ToList();
            if (!RunVirExchange)
            {
                if (MessageBox.Show("确定开始模拟成交？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            if (RunVirExchange)
            {
                if (MessageBox.Show("确定停止模拟成交？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
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
                this.timer_Tip.Enabled = false;
                RunVirExchange = false;
                return;
            }
            this.RunVirExchange = true;
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
            AssetUnitClass auc = new AssetUnitClass();
            if(Program.AllSettings.AllAssetUnits.Count>0)//如果存在资产管理单元，所有计划均绑定到第一个上进行测试
            {
                auc = Program.AllSettings.AllAssetUnits.Values.First();
            }
            es = auc.ExchangeServer;
            if (es != null)
            {
                es.Reset();
            }
            //设置所有默认值
            SCList.ForEach(p => p.PlanStrag.CommSetting = setting);
            SCList.ForEach(p => p.AssetUnitInfo = auc);
            SCList.ForEach(p => p.Running = true);
            //if (SCList.Count == 1)
            //{
            //    //SCList[0].PlanStrag = sc;
            SCList.ForEach(p=>p.IncreamType = this.checkBox_CreamModel.Checked ? InterestType.CompoundInterest : InterestType.SimpleInterest);
            SCList.ForEach(p=>p.AutoRunning = true);
            ////SCList.ForEach(p=>p.FixAmt = 1);
            ////SCList.ForEach(p=>p.FixRate = 0.01);
            SCList.ForEach(p => p.AllowMaxHoldTimeCnt = int.Parse(txt_AllowMaxHoldTimeCnt.Text));
            if (SCList.Count == 1)
            {
                SCList.ForEach(p => p.PlanStrag.ChipCount = int.Parse(this.txt_ChipCnt.Text));
            }
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
            this.timer_Tip.Interval = int.Parse(txt_Timer_Interval.Text)*1000;
            timer_Tip.Tick += new EventHandler(timer_Tip_Tick);
            //this.timer_Tip.Enabled = true;
            this.timer_Tip.Enabled = true;

            //this.timer_Tip_Tick(null, null);
            //////try
            //////{
            if (btc == null)
                btc = btc = new BackTestClass(long.Parse(txt_begExpNo.Text), long.Parse(txt_LoopCnt.Text), setting);
            th = new Thread(RunVirtual);
            th.Start();
            return;
            //////}
            //////catch (Exception ce)
            //////{
            //////    ret = new BackTestReturnClass();
            //////    ret.ChanceList = new List<ChanceClass>();
            //////    ret.Msg = ce.Message;
            //////    ret.succ = false;
            //////}
        }

        void RunVirtual()
        {
            //try
            //{
            Dictionary<string, CalcStragGroupClass> rest = null;
            Program.AllSettings.AllRunningPlanGrps = InitServerClass.InitCalcStrags(ref rest, Program.AllSettings.AllStrags, SCList.ToDictionary(p => p.GUID, p => p), Program.AllSettings.AllAssetUnits, true,true);//注入plans
            btc.FinishedProcess = new SuccEvent(Finished);
            ret = btc.VirExchange(Program.AllSettings, ref es, SCList.ToArray());
            //}
            //catch (Exception ce)
            //{
            //    MessageBox.Show(ce.Message);
            //}
            
            //MessageBox.Show(es.summary.ToString());
            //DataView moneyLines = new DataView(es.MoneyIncreamLine);
            //this.chart1.Series[0].Points.DataBindXY(moneyLines, "id", moneyLines, "val");
            //btn_VirExchange.Text = "模拟交易";
            //MessageBox.Show("执行完成！");
            this.RunVirExchange = false;
        }

        private void timer_Tip_Tick(object sender, EventArgs e)
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
                }

                lock (es.MoneyIncreamLine)
                {
                    
                    this.toolStripStatusLabel1.Text = string.Format("第{2}次,{0}/{1}", es.CurrIndex.ToString(), es.ExpectCnt, btc.testIndex);
                    this.toolStripStatusLabel2.Text = string.Format("{0}% 最大值:[{1}%]   最小值:[{2}%] ", es.GainedRate.ToString(), es.MaxRate, es.MinRate);

                    DataView moneyLines = new DataView(es.MoneyIncreamLine);
                    if (this.chart1.Series.Count == 0)
                    {
                        Series ss = new Series();
                        ss.ChartType = SeriesChartType.Line;
                        this.chart1.Series.Add(ss);
                    }
                    this.chart1.Series[0].Points.DataBindXY(moneyLines, "id", moneyLines, "val");
                }
                lock (es.ExchangeDetail)
                {
                    DataView vExchangeDetail = new DataView(es.ExchangeDetail);
                    this.dataGridView_ExchangeDetail.DataSource = vExchangeDetail;
                    this.dataGridView_ExchangeDetail.Refresh();
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

                            this.chart_ForProb.ChartAreas[0].AxisY.Maximum = gs.High;
                            this.chart_ForProb.ChartAreas[0].AxisY.Minimum = gs.Low;
                            this.chart_ForProb.Series[0].Points.DataBindXY(dvWv, "Id", dvWv, "val");
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
                e.Text = string.Format("第{0}道:{1}",sr.Name,sr.Points[Result.PointIndex].YValues[0]);
            }
            ////Result.Series;// 就是点击获得的Series
            ////Result.Series.Points[Result.PointIndex].XValue;// 为点击Series X坐标
            ////Result.Series.Points[Result.PointIndex].YValue;// 为点击Series

        }

        private void BackTestFrm_Load(object sender, EventArgs e)
        {
            this.timer_Tip.Enabled = false;
            //this.txt_InitCash.Text = int.MaxValue.ToString();
        }

           
        private void contextMenuStrip_ForListView_Opening(object sender, CancelEventArgs e)
        {
            
        }

        void SaveWaveTable()
        {
            if (retData == null) return;
            PK10ProbWaveDataInterface pwinter = new PK10ProbWaveDataInterface();
            pwinter.SaveProbWaveResult(retData);
        }

        private void btn_CalcEr_Click(object sender, EventArgs e)
        {
            frm_CalcEr frm = new frm_CalcEr();
            frm.Show();
        }

        private void txt_begExpNo_DoubleClick(object sender, EventArgs e)
        {
            txt_begExpNo.Text = string.Format("{0}", Int64.Parse(txt_begExpNo.Text) + 10000);
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
            string strExpect = dr.Cells["ExpectNo"].Value.ToString();
            //MessageBox.Show(strExpect);
            ////if (CheckFrm == null)
            ////{
            CheckFrm = new MainForm();
            ////    CheckFrm.InputExpect = int.Parse(strExpect);
            ////    CheckFrm.Show();
            ////    return;
            ////}
            CheckFrm.InputExpect = int.Parse(strExpect);
            CheckFrm.Show();
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
            CheckFrm = new MainForm();
            ////    CheckFrm.InputExpect = int.Parse(strExpect);
            ////    CheckFrm.Show();
            ////    return;
            ////}
            CheckFrm.InputExpect = int.Parse(strExpect);
            CheckFrm.Show();
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
            DateTime xtime,ytime;
            long xlong,ylong;
            string xin,yin;
            xin= ((ListViewItem)x).SubItems[SortColumn].Text;
            yin= ((ListViewItem)y).SubItems[SortColumn].Text;
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


}

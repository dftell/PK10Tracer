using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using WolfInv.com.PK10CorePress;
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.LogLib;
using WolfInv.com.ServerInitLib;
using WolfInv.com.Strags;

using System.Diagnostics;
using WolfInv.com.ExchangeLib;
using WolfInv.com.BaseObjectsLib;
using System.Windows.Forms.DataVisualization.Charting;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
using System.Drawing.Imaging;
namespace PK10Server
{
    delegate void SetDataGridCallback(string id,DataTable dt,string sort=null);
    delegate void SetSpecDataGridCallback(DataTable dt);
    delegate void setChartCallback_del(Chart cht);
    delegate void setChartWithLenCallback(int len);
    public partial class frm_StragMonitor <T>: Form where T:TimeSerialData
    {
        string logname = "监控日志";
        System.Timers.Timer PK10DataTimer = new System.Timers.Timer();
        System.Timers.Timer LogTimer = new System.Timers.Timer();
        //PK10ExpectReader exread = new PK10ExpectReader();
        DataTypePoint dtp = null;
        DataReader exread = null;
        GlobalClass glb = new GlobalClass();
        SetDataGridCallback DgInvokeEvent;
        
        Dictionary<string, long> AssetTimeSummary = new Dictionary<string, long>();

        ServiceSetting<TimeSerialData> _UseSetting = null;//供后面调用一切服务内容用
        ServiceSetting<TimeSerialData> UseSetting
        {
            get
            {
                return Program.UseSetting;
            }
        }
        public frm_StragMonitor()
        {
            InitializeComponent();
            //chart_ForGuide_Paint
            dtp = GlobalClass.TypeDataPoints.First().Value;
            exread = DataReaderBuild.CreateReader(dtp.DataType, null, null);
            PK10DataTimer.Interval = GlobalClass.TypeDataPoints.First().Value.ReceiveSeconds * 1000;
            PK10DataTimer.AutoReset = true;
            PK10DataTimer.Elapsed += new ElapsedEventHandler(RefreshPK10Data);
            PK10DataTimer.Enabled = false;
            LogTimer.Interval = 3 * 60 * 1000;
            LogTimer.AutoReset = true;
            LogTimer.Elapsed += new ElapsedEventHandler(RefreshLogInfo);
            LogTimer.Enabled = false;
            //RefreshLogInfo(null,null);
            //RefreshPK10Data(null, null);
            //RefreshPK10Data();
            //RefreshPK10NoClosedChances();
            dg_StragList.ContextMenuStrip = this.contextMenuStrip_OperatePlan;
            //dg_NoCloseChances.ContextMenuStrip = this.contextMenuStrip_OperatePlan;
            CheckForIllegalCrossThreadCalls = false;
            Program.optFunc.RefreshMonitorWindow += refreshSvrData;
        }

        void refreshSvrData()
        {
            //Program.AllGlobalSetting.wxlog.Log("退出服务", "意外停止服务", string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
            RefreshPK10Data(null,null);
            //RefreshPK10NoClosedChances();
        }

        void Setdg_NoCloseChances(DataTable dt)
        {
            dg_NoCloseChances.DataSource = dt;
            dg_NoCloseChances.Refresh();
        }

        void Setdg_baseData(DataTable dt)
        {
            if (dt == null) return;
            DataView dv = new DataView(dt);
            dv.Sort = "expect desc";
            dg_baseData.DataSource = dv;
            dg_baseData.Tag = dt;
            dg_baseData.Refresh();
        }

        

        void SetDgTableById(string id, DataTable dt,string sort)
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
            (ctrls[0] as DataGridView).Tag = dt;
        }

        void SetDataGridDataTable(DataGridView dg, DataTable dt,string sort=null)
        {
            //if (dg.InvokeRequired)
            //{
                //DgInvokeEvent = new SetDataGridCallback(SetDgTableById);
                dg.Invoke(new SetDataGridCallback(SetDgTableById),new object[] { dg.Name, dt,sort});
            //}
            //else
            //{
            //    DgInvokeEvent = new SetDataGridCallback(SetDgTableById);

            //    SetDgTableById(dg.Name, dt);
            //}
            ////else
            ////{
            ////    dg.Tag = dt;
            ////    dg.DataSource = dt;
            ////    dg.Refresh();
            ////}
        }

        void RefreshPK10Data(object obj, ElapsedEventArgs e)
        {
            //RefrshStragAndPlan();
            this.Cursor = Cursors.WaitCursor;
            try
            {
                RefreshPK10Data();
                Application.DoEvents();
                RefreshPK10NoClosedChances();
                Application.DoEvents();
                RefrshStragAndPlan();
                Application.DoEvents();
                RefreshLogData();
            }
            catch
            {

            }
            this.Cursor = Cursors.Default;
        }

        

        void RefreshPK10Data()
        {
            ExpectList<T> ViewDataList = exread.ReadNewestData<T>(DateTime.Now.AddDays(-1* dtp.CheckNewestDataDays));
            if (ViewDataList == null) return;
            DataTable dt = ViewDataList.Table;
            dg_baseData.Invoke(new SetSpecDataGridCallback(Setdg_baseData), new object[] { dt });
            //SetDataGridDataTable(dg_baseData, dt);
        }



        void RefreshPK10NoClosedChances()
        {
            //DbChanceList<T> dc = new PK10ExpectReader().getNoCloseChances<T>(null);
            DbChanceList<T> dc = exread.getNoCloseChances<T>(null);
            if (dc == null) return;
            DataTable dt = dc.Table;
           
            SetDataGridDataTable(dg_NoCloseChances, dt);
            string GR_Path = "chances.jpg";
            imageLogClass.SaveImage(dg_NoCloseChances, GR_Path, ImageFormat.Jpeg);
        }

        void RefrshStragAndPlan(bool ForceRefresh = false)
        {
            try
            {
                if (UseSetting == null)
                {
                    return;
                }
                DataTable dt_strag = BaseStragClass<TimeSerialData>.ToTable<BaseStragClass<TimeSerialData>>(UseSetting.AllStrags.Values.ToList<BaseStragClass<TimeSerialData>>());
                if (dt_strag != null)
                {
                    
                    SetDataGridDataTable(dg_StragList, dt_strag);
                }
                DataTable dt_plans = StragRunPlanClass<TimeSerialData>.ToTable<StragRunPlanClass<TimeSerialData>>(UseSetting.AllRunPlannings.Values.ToList<StragRunPlanClass<TimeSerialData>>());
                if (dt_plans != null)
                {
                    SetDataGridDataTable(dg_stragStatus, dt_plans);
                }
                DataTable dt_grps = CalcStragGroupClass<TimeSerialData>.ToTable<CalcStragGroupClass<TimeSerialData>>(UseSetting.AllRunningPlanGrps.Values.ToList<CalcStragGroupClass<TimeSerialData>>());
                if (dt_grps != null)
                {
                    SetDataGridDataTable(dg_PlanGrps, dt_grps);
                }
                DataTable dt_assetunit = AssetUnitClass.ToTable<AssetUnitClass>(UseSetting.AllAssetUnits.Values.ToList<AssetUnitClass>());
                if (dt_assetunit != null)
                    SetDataGridDataTable(dg_AssetUnits, dt_assetunit);
                DataTable dt_exchange = null;
                foreach(AssetUnitClass auc in UseSetting.AllAssetUnits.Values)
                {
                    ExchangeService ed = auc.getCurrExchangeServer();
                    if(ed == null)
                    {
                        //MessageBox.Show(string.Format("{0}资产单元交易服务为空！", auc.UnitName));

                        continue;
                    }
                    DataTable assetdt = ed.ExchangeDetail;
                    if (assetdt == null)
                    {
                        //MessageBox.Show(string.Format("{0}资产单元交易记录为空！", auc.UnitName));

                        continue;
                    }
                    //MessageBox.Show(string.Format("{0}有新交易记录{1}条！列数:{2}", auc.UnitName, assetdt.Rows.Count,assetdt.Columns.Count));

                    if (assetdt != null)
                    {
                        if (dt_exchange == null)
                        {
                            dt_exchange = assetdt.Clone();
                        }
                        for (int i = 0; i < assetdt.Rows.Count; i++)
                        {
                            dt_exchange.Rows.Add(assetdt.Rows[i].ItemArray);
                        }
                    }
                }
                //MessageBox.Show(dt_exchange.Rows.Count.ToString());
                if(dt_exchange!= null)
                {
                    SetDataGridDataTable(dg_ExchangeList, dt_exchange,"createtime desc");
                }
                if (UseSetting.AllAssetUnits != null)
                {
                    lock (UseSetting.AllAssetUnits)
                    {
                        refresh_AssetChart();
                    }
                }
            }
            catch (Exception e)
            {
                string msg = string.Format("{0}:{1}", e.Message, e.StackTrace);
                if (ForceRefresh)
                {
                    MessageBox.Show(msg);
                }
            }
        }

        void RefreshLogInfo(object obj, ElapsedEventArgs e)
        {
            ////this.Cursor = Cursors.WaitCursor;
            //////RefreshLogData();
            ////this.Cursor = Cursors.Default;
        }

        void RefreshLogData(bool ForceRefresh = false)
        {
            try
            {
                if (UseSetting == null)
                {
                    return;
                }
                DataTable dt = UseSetting.LogTable;
                if (dt == null)
                    return;
                RefreshLogData(dt);
                
            }
            catch (Exception e)
            {
                string msg = string.Format("{0}:{1}",e.Message,e.StackTrace);
                if(ForceRefresh)
                {
                    MessageBox.Show(msg);
                }
            }
        }

        public void RefreshLogData(DataTable dt)
        {
            try
            {
                
                SetDataGridDataTable(dg_LoginList, dt,"时间 desc");
            }
            catch (Exception ce)
            {
                LogableClass.ToLog("监控终端", "刷新数据错误", ce.Message);
            }
        }

        private void frm_StragMonitor_Load(object sender, EventArgs e)
        {
            //RefreshPK10Data();
            //RefreshPK10NoClosedChances();
            refreshAssetUnitDll();
        }



        private void bootServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //string targetDir = string.Format(@"D:\BizMap\");//this is where testChange.bat lies
                Process proc = new Process();
                proc.StartInfo.WorkingDirectory = Application.StartupPath;
                proc.StartInfo.FileName = "startsvr.bat";
                //proc.StartInfo.Arguments = null;// string.Format("10");//this is argument
                //proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
                proc.Start();
                proc.WaitForExit();
                this.bootServiceToolStripMenuItem.Enabled = false;
                this.stopServiceToolStripMenuItem.Enabled = true;
                MessageBox.Show("成功启动");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }

        private void stopServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.WorkingDirectory = Application.StartupPath;
                proc.StartInfo.FileName = "stopsvr.bat";
                //proc.StartInfo.Arguments = null;// string.Format("10");//this is argument
                //proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
                proc.Start();
                proc.WaitForExit();
                this.bootServiceToolStripMenuItem.Enabled = true;
                this.stopServiceToolStripMenuItem.Enabled = false;
                MessageBox.Show("成功停止");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }
        
       
        
        void DisableAllMenus()
        {
            //return;
            for (int i = 0; i < this.contextMenuStrip_OperatePlan.Items.Count; i++)
            {
                this.contextMenuStrip_OperatePlan.Items[i].Visible = false;
                this.contextMenuStrip_OperatePlan.Items[i].Enabled = false;
            }
        }

        

        bool SetPlanStatus(bool Start,EventArgs e)
        {
             StragRunPlanClass<TimeSerialData> plan = getPlanAfterMouseUp(e as MouseEventArgs);
            if (plan == null) return false;
            if (!Start)
            {
                if (plan.PlanStrag is ITraceChance)
                {
                    if (MessageBox.Show("该计划执行的策略为策略级跟踪策略，选择出的组合无法自我跟踪，当停止执行该计划后，该计划所选出来的组合在下期将全部关闭，如果跟踪中途产生的浮动亏损将全部实现！确定要继续停止执行该计划？", "继续停止执行计划", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return false;
                    }
                }
                else
                {
                    if (MessageBox.Show("该计划执行的策略为组合级跟踪策略，选择出的组合将自我跟踪，直至原有停止逻辑满足，当停止执行该计划后，只会停止寻找新的组合，已选组合将继续运行！确定要继续停止执行该计划？", "继续停止执行计划", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return false;
                    }
                }
            }
            try
            {
                bool ret = UseSetting.SetPlanStatus(plan.GUID, Start);
                if (ret == true)
                {
                    DataTable dt_plans = StragRunPlanClass<T>.ToTable<StragRunPlanClass<TimeSerialData>>(UseSetting.AllRunPlannings.Values.ToList<StragRunPlanClass<TimeSerialData>>());
                    if (dt_plans == null)
                        return ret;
                    SetDataGridDataTable(dg_stragStatus, dt_plans);
                }
                return ret;
            }
            catch (Exception ce)
            {
                string msg = ce.Message;
            }
            return false;
        }


        StragRunPlanClass<TimeSerialData> getServicePlan(string guid)
        {
            try
            {
                if (UseSetting == null)
                {
                    return null;
                }
                if (UseSetting.AllRunPlannings.ContainsKey(guid))
                {
                    return UseSetting.AllRunPlannings[guid];
                }
            }
            catch (Exception ce)
            {
                string msg = ce.Message;
                MessageBox.Show(msg);
            }
            return null;
        }

        BaseStragClass<TimeSerialData> getServiceStrag(string guid)
        {
            try
            {
                if (UseSetting == null)
                {
                    return null;
                }
                if (UseSetting.AllStrags.ContainsKey(guid))
                {
                    return UseSetting.AllStrags[guid];
                }
            }
            catch (Exception ce)
            {
                string msg = ce.Message;
            }
            return null;
        }

               
        private void dg_AssetUnits_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (this.dg_AssetUnits.CurrentRow.Index < 0)
                {
                    return;
                }
                if (UseSetting == null) return;
                AssetUnitClass auc = null;
                try
                {
                    DataTable dt = this.dg_AssetUnits.Tag as DataTable;
                    string id = dt.Rows[dg_AssetUnits.CurrentRow.Index]["UnitId"].ToString();
                    auc = UseSetting.AllAssetUnits[id];
                    if (auc == null)
                        return;
                }
                catch
                {
                    return;
                }
                //dg_AssetUnits as
                DataView moneyLines = new DataView(auc.SummaryLine());
                lock (moneyLines)
                {
                    this.chart_ForGuide.Series[0].Points.DataBindXY(moneyLines, "id", moneyLines, "val");
                }
            }
            catch(Exception ce)
            {

            }
        }

        void refreshAssetUnitDll()
        {
            Dictionary<string, AssetUnitClass> assetUnits = null;
            if(UseSetting == null)
            {
                return;
            }
            try
            {
                assetUnits = UseSetting.AllAssetUnits;
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
            if (assetUnits == null)
                return;
            DataTable dt = new DataTable();
            dt.Columns.Add("name");
            dt.Columns.Add("value");
            foreach(string key in assetUnits.Keys)
            {
                DataRow dr = dt.NewRow();
                dr["name"] = assetUnits[key].UnitName;
                dr["value"] = assetUnits[key].UnitId;
                dt.Rows.Add(dr);
            }
            this.ddl_assetunits.ValueMember = "value";
            this.ddl_assetunits.DisplayMember = "name";
            this.ddl_assetunits.DataSource = dt;
            this.ddl_assetunits.Tag = assetUnits;
        }

        void refresh_AssetChart()
        {
            int DefaultDisplayLength = 1000;
            int Len = 0;
            if(!int.TryParse(txt_AssetTimeLength.Text,out Len))
            {
                Len = DefaultDisplayLength;
            }
            //this.chart_ForGuide.Invoke(new setChartCallback(setChart),chart);
            this.chart_ForGuide.Invoke(new setChartWithLenCallback(rePaintChart),Len);
            //rePaintChart(Len);
            //SaveChart();
            string GR_Path = "chart.png";
            imageLogClass.SaveImage(this.chart_ForGuide, GR_Path,ImageFormat.Png);
        }

        
        void rePaintChart(int Len)
        {
            try
            {


                Chart chrt = this.chart_ForGuide;
                ////chrt.Parent = this.chart_ForGuide.Parent;
                ////chrt.Left = this.chart_ForGuide.Left;
                ////chrt.Top = this.chart_ForGuide.Top;
                ////chrt.Dock = this.chart_ForGuide.Dock;

                Dictionary<string, AssetUnitClass> assetUnits = UseSetting.AllAssetUnits;
                //System.Threading.Thread.Sleep(10 * 1000);
                lock (assetUnits)
                {

                    ////////////string strExpect = dt.Rows[dt.Rows.Count - 1]["Expect"].ToString();
                    ////////////Int64 MinExpect = Int64.Parse(strExpect) - 180;
                    ////////////string sql = string.Format("Expect>={0}", MinExpect);
                    ////////////DataView dv_stddev = new DataView(dt);
                    ////////////dv_stddev.RowFilter = sql;
                    bool Changed = false;
                    List<string> keys = assetUnits.Keys.ToList();
                    List<AssetUnitClass> vals = assetUnits.Values.ToList();
                    //foreach (string id in assetUnits.Keys)
                    for (int ai = 0; ai < keys.Count; ai++)
                    {
                        string id = keys[ai];

                        DataTable dt = vals[ai].SummaryLine();
                        //MessageBox.Show(string.Format("资产单元{0}记录数{1}条。", assetUnits[id].UnitName, dt.Rows.Count));
                        if (this.AssetTimeSummary.ContainsKey(id) == false || (this.AssetTimeSummary.ContainsKey(id) && dt.Rows.Count != this.AssetTimeSummary[id]))
                        {
                            if (!this.AssetTimeSummary.ContainsKey(id))
                            {
                                this.AssetTimeSummary.Add(id, 0);
                            }
                            this.AssetTimeSummary[id] = dt.Rows.Count;
                            Changed = true;

                            //saveAssetLines(id, dt);//保存
                        }
                        vals[ai].SaveDataToFile();

                    }
                    //if (!Changed)//没有任何改变，不刷新
                    //    return;

                    int i = 0;
                    if (chrt.Series.Count < vals.Count - 1)
                    {
                        chrt.Series.Clear();

                        // foreach (string strName in assetUnits.Keys)
                        for (int ai = 0; ai < keys.Count; ai++)
                        {
                            string strName = keys[ai];
                            Series sr = new Series(vals[ai].UnitName);
                            sr.ChartType = SeriesChartType.Line;
                            chrt.Series.Add(sr);
                            i++;
                        }
                        //chrt.GetToolTipText += new EventHandler<ToolTipEventArgs>(chart_ForSystemStdDev_GetToolTipText);
                    }
                    i = 0;
                    //foreach (string strName in assetUnits.Keys)
                    AssetUnitClass needResetAssetUnit = null;
                    double needResetNet = 0;
                    for (int ai = 0; ai < keys.Count; ai++)
                    {
                        //System.Threading.Thread.Sleep(5 * 1000);
                        string strName = keys[ai];
                        DataTable dt = vals[ai].SummaryLine();
                        List<DataRow> drs = dt.Select("id>0", "id desc").Take<DataRow>(Len).ToList();
                        DataTable DispDt = dt.Clone();
                        for (int di = 0; di < drs.Count; di++)                               
                        {
                            DispDt.Rows.Add(drs[di].ItemArray);
                        }
                        DataView dv = new DataView(DispDt);
                        dv.Sort = "id asc";
                        chrt.Series[i].Points.DataBindY(dv, "val");
                        if(chrt.Series.Count<i+1)
                        {
                            Series sr = new Series();
                            sr.ChartType = SeriesChartType.Line;
                            chrt.Series.Add(sr);
                        }
                        if (dt.Rows.Count > 0)
                        {
                            if (strName.Trim().StartsWith("C")||strName.IndexOf("_C")>0)
                            {
                                double currVal = 0;
                                double.TryParse(dt.Rows[dt.Rows.Count - 1]["val"].ToString(), out currVal);
                                if (currVal < -35)//警告
                                {
                                    Program.wxlog.Log(string.Format("品种{0}复利类资产单元资产收益率报警！", GlobalClass.TypeDataPoints.First().Key),string.Format("资产单元[{0}]当前收益率{1}。",strName,currVal), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                                }
                                if (currVal >190)//警告
                                {
                                    Program.wxlog.Log(string.Format("品种{0}复利类资产单元资产收益率过高报警！", GlobalClass.TypeDataPoints.First().Key), string.Format("资产单元[{0}]当前收益率{1}。", strName, currVal), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                                }
                                if (currVal < -50)
                                {
                                    needResetAssetUnit = vals[ai];//一次恢复一个满足条件的最后一个资产单位
                                    needResetNet = currVal;
                                }
                                if(currVal>200)
                                {
                                    needResetAssetUnit = vals[ai];//一次恢复一个满足条件的最后一个资产单位
                                    needResetNet = currVal;
                                }
                            }
                        }
                        i++;
                    }
                    chrt.Show();
                    if(needResetAssetUnit!= null)
                    {
                        string msgModel = "复利类资产单元[{0}]当前资产收益率{1}小于-50%或大于200%，将自动将其归1，请视情况在客户端上调整其对应的风险配置。";
                        string msg = string.Format(msgModel, needResetAssetUnit.UnitName, needResetNet);
                        if (needResetAssetUnit.Resume())
                        {
                            Program.wxlog.Log(string.Format("品种{0}资产收益率归1成功.", GlobalClass.TypeDataPoints.First().Key), msg, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                        }
                        else
                        {
                            Program.wxlog.Log(string.Format("品种{0}资产收益率归1失败.", GlobalClass.TypeDataPoints.First().Key), msg ,string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                        }
                    }
                    //try
                    //{
                    //this.chart_ForGuide = chrt;
                    //this.chart_ForGuide.Show();
                    ////if (vals != null)
                    ////{
                    ////    this.chart_ForGuide.Invoke(new setChartCallback(setChart), chrt);
                    ////}
                    //}
                    //catch(Exception ce)
                    //{
                    //MessageBox.Show(ce.Message);
                    //}

                }
                //this.chart_ForSystemStdDev.DataSource = dv_stddev;

                //this.chart_ForSystemStdDev.Series[0].Points.DataBindXY(dv_stddev, "Expect", dv_stddev, "StdDev");
                //this.chart_ForSystemStdDev.Series[1].Points.DataBindXY(dv_stddev, "Expect", dv_stddev, "StdMa20");
                //this.chart_ForSystemStdDev.Series[2].Points.DataBindXY(dv_stddev, "Expect", dv_stddev, "StdMa5");
                //////cjrt.ChartAreas[0].AxisY.Maximum = 0.6;
                //////this.chart_ForSystemStdDev.ChartAreas[0].AxisY.Minimum = 0.1;
                //////this.chart_ForSystemStdDev.Show();
            }
            catch (Exception e)
            {
                LogableClass.ToLog("监控", e.Message, e.StackTrace);
            }
            

        }


        void SaveChart()
        {
            string GR_Path = string.Format("{0}\\imgs\\chart.png", AppDomain.CurrentDomain.BaseDirectory);
            string fullFileName = GR_Path;// GR_Path + "\\" + fileName + ".png";
            try
            {
                this.chart_ForGuide.SaveImage(fullFileName, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
                Program.wxlog.LogImageUrl(string.Format("{0}/chartImgs/chart.png",GlobalClass.TypeDataPoints.First().Value.InstHost), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
            }
            catch(Exception ce)
            {

            }
        }
             

        private void btn_adjustAssetTimeLength_Click(object sender, EventArgs e)
        {
            refresh_AssetChart();
        }
        void setChart(Chart cht)
        {
            try
            {
                if(UseSetting.AllAssetUnits != null)
                {
                    lock(UseSetting.AllAssetUnits)
                    {
                        chart_ForGuide = cht;
                        chart_ForGuide.Show();
                    }
                }
                
            }
            catch(Exception ce)
            {
                MessageBox.Show(string.Format("", ce.Message, ce.Source));
            }
            //chart_ForGuide_Paint
            //chart_ForGuide.Show();
        }
        private void btn_clearNet_Click(object sender, EventArgs e)
        {
            
            Dictionary<string, AssetUnitClass> units = this.ddl_assetunits.Tag as Dictionary<string, AssetUnitClass>;
            if (this.ddl_assetunits.SelectedIndex < 0) return;
            AssetUnitClass auc = units[this.ddl_assetunits.SelectedValue.ToString()];
            if (auc == null) return;
            if(MessageBox.Show("是否真的需要恢复资产单元到初始金额？如果点是，资产单元将恢复到设定的初始值，对于复利策略对应的资产单元，将恢复到100%状态，在连续亏损时请慎重点击该按钮，建议当资产单元达到10%以下才使用！", "确认恢复资产单元", MessageBoxButtons.YesNo)==DialogResult.No)
            {
                return;
            }
            try
            {
                auc.Resume();
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }

        #region 右键处理
        private void dg_stragStatus_MouseUp(object sender, MouseEventArgs e)
        {
            DisableAllMenus();
            object obj = getGridAfterMouseUp<StragRunPlanClass<TimeSerialData>>(this.dg_stragStatus, e) ;
            //MessageBox.Show(obj.GetType().ToString());
            StragRunPlanClass<T> strag = obj as StragRunPlanClass<T>;
            ///this.tmi_StartPlan.Enabled = false;
            ////this.tmi_StopPlan.Enabled = false;
            ////this.tmi_Edit.Enabled = false;
            //MessageBox.Show(strag?.GetType().ToString());
            tmi_refreshPlans.Enabled = true;
            if (strag == null)
            {
                //MessageBox.Show("依附对象为空！");
                return;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.tmi_Edit.Visible = true;
                this.tmi_Edit.Enabled = true;
                this.tmi_StopPlan.Visible = strag.Running;
                this.tmi_StopPlan.Enabled = strag.Running;
                this.tmi_StartPlan.Visible = !strag.Running;
                this.tmi_StartPlan.Enabled = !strag.Running;
            }
        }

        private void dg_StragList_MouseUp(object sender, MouseEventArgs e)
        {
            DisableAllMenus();
            StragClass strag = getGridAfterMouseUp<StragClass>(this.dg_StragList, e) as StragClass;
            tmi_refreshPlans.Enabled = true;
            if (strag == null) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.tmi_Edit.Visible = true;
                this.tmi_Edit.Enabled = true;
            }
        }
        private void dg_NoCloseChances_MouseUp(object sender, MouseEventArgs e)
        {
            DisableAllMenus();
            ChanceClass cc = getGridAfterMouseUp<ChanceClass>(this.dg_NoCloseChances, e) as ChanceClass;
             
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (cc != null)
                {
                    this.tmi_Delete.Visible = true;
                    this.tmi_Delete.Enabled = true;
                    this.tmi_Delete.Tag = cc;
                }
                this.tmi_refreshPlans.Visible = true;
                this.tmi_refreshPlans.Enabled = true;
                this.tmi_refreshPlans.Tag = cc;
            }
            dg_NoCloseChances.ContextMenuStrip = this.contextMenuStrip_OperatePlan;
        }

        private void dg_CloseChances_MouseUp(object sender, MouseEventArgs e)
        {
            DisableAllMenus();
            ChanceClass cc = getGridAfterMouseUp<ChanceClass>(this.dg_CloseChances, e) as ChanceClass;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (cc != null)
                {
                    this.tmi_Delete.Visible = true;
                    this.tmi_Delete.Enabled = true;
                    this.tmi_Delete.Tag = cc;
                }
                this.tmi_refreshPlans.Visible = true;
                this.tmi_refreshPlans.Enabled = true;
                this.tmi_refreshPlans.Tag = cc;
            }
            dg_CloseChances.ContextMenuStrip = this.contextMenuStrip_OperatePlan;
        }

        private void dg_baseData_MouseUp(object sender, MouseEventArgs e)
        {
            DisableAllMenus();
            ExpectData ed =  getGridAfterMouseUp<ExpectData>(this.dg_baseData, e) as ExpectData;
            
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (ed == null)
                {
                    this.tmi_Delete.Visible = true;
                    this.tmi_Delete.Enabled = true;
                    this.tmi_Delete.Tag = ed;
                }
                this.tmi_refreshPlans.Visible = true;
                this.tmi_refreshPlans.Enabled = true;
                this.tmi_refreshPlans.Tag = ed;
            }
            dg_baseData.ContextMenuStrip = this.contextMenuStrip_OperatePlan;
        }

        private void dg_LoginList_MouseUp(object sender, MouseEventArgs e)
        {
            DisableAllMenus();
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.tmi_refreshPlans.Visible = true;
                this.tmi_refreshPlans.Enabled = true;
            }
            dg_LoginList.ContextMenuStrip = this.contextMenuStrip_OperatePlan;
        }

        private void dg_ExchangeList_MouseUp(object sender, MouseEventArgs e)
        {
            DisableAllMenus();
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.tmi_refreshPlans.Visible = true;
                this.tmi_refreshPlans.Enabled = true;
            }
            dg_ExchangeList.ContextMenuStrip = this.contextMenuStrip_OperatePlan;
        }

        object getGridAfterMouseUp<T1>(DataGridView dg, MouseEventArgs e)
        {

            if (dg.SelectedRows.Count <= 0) return default(T);
            if (dg.SelectedRows[0].Index < 0) return default(T);
            int index = dg.SelectedRows[0].Index;
            DataTable dt = dg.Tag as DataTable;
            if (dt == null)
            {
                //MessageBox.Show("附加表为空！");
                return default(T);
            }
            if (dt.Rows.Count < index + 1)
            {
                MessageBox.Show("行数不足！");
                return default(T);
            }
            DataRow dr = dt.Rows[index];
            ////if (dr["GUID"] == null)
            ////{
            ////    MessageBox.Show("GUID未找到！");
            ////    return default(T);
            ////}
            string pguid = "";
            if (dt.Columns.Contains("GUID")) //支持所有guid
                pguid = dr["GUID"].ToString();
            
            if (typeof(T1) == typeof(BaseStragClass<T>))
            {
                return getServiceStrag(pguid);
            }
            else if (typeof(T1) == typeof(StragRunPlanClass<TimeSerialData>))
            {
                //MessageBox.Show("已经进入");
                return getServicePlan(pguid);
            }
            else if (typeof(T1) == typeof(ExpectData))
            {
                List<ExpectData> list = new ExpectData().FillByTable<ExpectData>(dt);
                return list[index];
            }
            else if (typeof(T1) == typeof(ChanceClass))
            {
                List<ChanceClass> list = new ChanceClass().FillByTable<ChanceClass>(dt);
                return list[index];
            }
            else
            {
                return default(T1);
            }
            //return plan;
        }

        StragRunPlanClass<TimeSerialData> getPlanAfterMouseUp(MouseEventArgs e)
        {

            if (this.dg_stragStatus.SelectedRows.Count <= 0) return null;
            if (this.dg_stragStatus.SelectedRows[0].Index < 0) return null;
            int index = this.dg_stragStatus.SelectedRows[0].Index;
            DataTable dt = this.dg_stragStatus.Tag as DataTable;
            if (dt == null) return null;
            if (dt.Rows.Count < index + 1)
            {
                return null;
            }
            DataRow dr = dt.Rows[index];
            if (dr["GUID"] == null)
                return null;
            string pguid = dr["GUID"].ToString();
            StragRunPlanClass<TimeSerialData> plan = getServicePlan(pguid);
            return plan;
        }


        #endregion

        #region 右键所有事件处理
        private void tsmi_refreshPlans_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            string sort = null;
            try
            {
                // menu.Owner
                DataGridView dg = (menu.Owner as ContextMenuStrip).SourceControl as DataGridView;
                if (dg == null) return;
                if (UseSetting == null)
                {
                    return;
                }
                DataTable dt = null;

                if (dg.Equals(this.dg_stragStatus))
                {
                    dt = StragRunPlanClass<TimeSerialData>.ToTable<StragRunPlanClass<TimeSerialData>>(UseSetting.AllRunPlannings.Values.ToList<StragRunPlanClass<TimeSerialData>>());

                }
                else if (dg.Equals(this.dg_NoCloseChances))
                {
                    //DbChanceList<T> dc = new PK10ExpectReader().getNoCloseChances<T>(null);
                    DbChanceList<T> dc = exread.getNoCloseChances<T>(null);
                    if (dc == null) return;
                    dt = dc.Table;
                }
                else if (dg.Equals(this.dg_CloseChances))
                {
                    //DbChanceList<T> dc = new PK10ExpectReader().getNoCloseChances<T>(null);
                    DbChanceList<T> dc = exread.getClosedChances<T>(null,2);
                    if (dc == null) return;
                    dt = dc.Table;
                    sort = "chanceindex desc";
                }
                else if (dg.Equals(this.dg_baseData))
                {
                    RefreshPK10Data();
                }
                else if (dg.Equals(this.dg_ExchangeList))
                {
                    RefrshStragAndPlan(true);
                }
                else if (dg.Equals(this.dg_LoginList))
                {
                    RefreshLogData(true);
                }
                else if (dg.Equals(this.dg_StragList))
                {
                    dt = BaseStragClass<TimeSerialData>.ToTable<BaseStragClass<TimeSerialData>>(UseSetting.AllStrags.Values.ToList<BaseStragClass<TimeSerialData>>());
                }
                if (dt == null)
                    return;
                SetDataGridDataTable(dg, dt,sort);
            }
            catch (Exception ce)
            {
                string msg = ce.Message;
                //MessageBox.Show(msg);
            }
        }

        private void tmi_Delete_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            try
            {
                object obj = menu.Tag;
                // menu.Owner
                DataGridView dg = (menu.Owner as ContextMenuStrip).SourceControl as DataGridView;
                if (dg == null) return;
                
                DataTable dt = null;
                //PK10ExpectReader pkreader = new PK10ExpectReader();
                if(MessageBox.Show("删除确定","确定删除此记录！",MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                int res = -1;
                if (dg.Equals(this.dg_NoCloseChances))
                {
                    res = exread.DeleteChanceByIndex((obj as ChanceClass).ChanceIndex.Value);
                }
                else if (dg.Equals(this.dg_baseData))
                {
                    res = exread.DeleteExpectData((obj as ExpectData).Expect);
                }
                if(res >0 )
                {
                    MessageBox.Show("成功删除此记录！");
                }
            }
            catch (Exception ce)
            {
                string msg = ce.Message;
            }
        }

        private void tmi_Edit_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menu = sender as ToolStripMenuItem;
                DataGridView dg = (menu.Owner as ContextMenuStrip).SourceControl as DataGridView;
                if (dg == null) return;
                if (dg.Equals(this.dg_StragList))
                {
                    StragClass strag = getGridAfterMouseUp<StragClass>(dg, null) as StragClass;
                    if (strag == null)
                    {
                        MessageBox.Show("策略为空");
                        return;
                    }
                    if (MessageBox.Show(string.Format("确定要修改策略:{0}", strag.StragScript), "修改策略", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                    frm_StragManager frm = new frm_StragManager();
                    frm.SpecList = UseSetting.AllStrags as Dictionary<string, BaseStragClass<TimeSerialData>>;
                    frm.SpecObject = strag;
                    frm.Show();
                }
                else if (dg.Equals(this.dg_stragStatus))
                {
                    StragRunPlanClass<T> strag = getGridAfterMouseUp<StragRunPlanClass<T>>(dg, null) as StragRunPlanClass<T>;
                    if (strag == null)
                    {
                        MessageBox.Show("计划为空");
                        return;
                    }
                    if (MessageBox.Show(string.Format("确定要修改计划:{0}", strag.StragName), "修改计划", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                    frm_StragPlanSetting frm = new frm_StragPlanSetting();
                    frm.SpecList = UseSetting.AllRunPlannings as Dictionary<string, StragRunPlanClass<TimeSerialData>>;
                    frm.SpecObject = strag as StragRunPlanClass<TimeSerialData>;
                    frm.Show();
                }
            }
            catch (Exception ce)
            {
                MessageBox.Show(string.Format("{0}:{1}", ce.Message, ce.StackTrace));
            }
        }
        private void tmi_StartPlan_Click(object sender, EventArgs e)
        {
            if (SetPlanStatus(true, e))
            {
                MessageBox.Show("成功启动计划！");
            }
            else
            {
                MessageBox.Show("启动计划失败！");
            }
        }

        private void tmi_StopPlan_Click(object senderc, EventArgs e)
        {
            if (SetPlanStatus(false, e))
            {
                MessageBox.Show("成功停止计划！");
            }
            else
            {
                MessageBox.Show("停止计划失败！");
            }
        }


        #endregion

        private void chart_ForGuide_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }

        private void contextMenuStrip_OperatePlan_Opening(object sender, CancelEventArgs e)
        {

        }
    }


}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using PK10CorePress;
using WinInterComminuteLib;
using LogLib;
using ServerInitLib;
using Strags;
using WinInterComminuteLib;
using System.Diagnostics;
using ExchangeLib;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace PK10Server
{
    delegate void SetDataGridCallback(string id,DataTable dt);
    delegate void SetSpecDataGridCallback(DataTable dt);
    public partial class frm_StragMonitor : Form
    {
        string logname = "监控日志";
        System.Timers.Timer PK10DataTimer = new System.Timers.Timer();
        System.Timers.Timer LogTimer = new System.Timers.Timer();
        PK10ExpectReader exread = new PK10ExpectReader();
        GlobalClass glb = new GlobalClass();
        SetDataGridCallback DgInvokeEvent;
        ServiceSetting _UseSetting = null;//供后面调用一切服务内容用
        Dictionary<string, long> AssetTimeSummary = new Dictionary<string, long>();
        ServiceSetting UseSetting
        {
            get
            {
                if (_UseSetting == null)
                {
                    try
                    {

                        WinComminuteClass wc = new WinInterComminuteLib.WinComminuteClass();
                        _UseSetting = wc.GetServerObject<ServiceSetting>(null);

                    }
                    catch (Exception ce)
                    {

                    }
                }
                return _UseSetting;
            }
        }
        
        public frm_StragMonitor()
        {
            InitializeComponent();
            PK10DataTimer.Interval = 1 * 10 * 1000;
            PK10DataTimer.AutoReset = true;
            PK10DataTimer.Elapsed += new ElapsedEventHandler(RefreshPK10Data);
            PK10DataTimer.Enabled = true;
            LogTimer.Interval = 3 * 10 * 1000;
            LogTimer.AutoReset = true;
            LogTimer.Elapsed += new ElapsedEventHandler(RefreshLogInfo);
            LogTimer.Enabled = false;
            //RefreshLogInfo(null,null);
            //RefreshPK10Data(null, null);
            //RefreshPK10Data();
            //RefreshPK10NoClosedChances();
            dg_StragList.ContextMenuStrip = this.contextMenuStrip_OperatePlan;
            CheckForIllegalCrossThreadCalls = false;
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
            dg_baseData.Refresh();
        }

        void SetDgTableById(string id, DataTable dt)
        {
            Control[] ctrls = this.Controls.Find(id, true);
            if (ctrls.Length != 1)
            {
                return;
            }
            (ctrls[0] as DataGridView).DataSource = dt;
            (ctrls[0] as DataGridView).Tag = dt;
        }

        void SetDataGridDataTable(DataGridView dg, DataTable dt)
        {
            //if (dg.InvokeRequired)
            //{
                //DgInvokeEvent = new SetDataGridCallback(SetDgTableById);
                dg.Invoke(new SetDataGridCallback(SetDgTableById), new object[] { dg.Name, dt });
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
                RefreshPK10NoClosedChances();
                RefrshStragAndPlan();
            }
            catch
            {

            }
            this.Cursor = Cursors.Default;
        }
        
        void RefreshPK10Data()
        {
            ExpectList ViewDataList = exread.ReadNewestData(DateTime.Now.AddDays(-1*glb.CheckNewestDataDays));
            if (ViewDataList == null) return;
            DataTable dt = ViewDataList.Table;
            dg_baseData.Invoke(new SetSpecDataGridCallback(Setdg_baseData), new object[] { dt });
            //SetDataGridDataTable(dg_baseData, dt);
        }

        void RefreshPK10NoClosedChances()
        {
            DbChanceList dc = new PK10ExpectReader().getNoCloseChances(null);
            if (dc == null) return;
            DataTable dt = dc.Table;
            SetDataGridDataTable(dg_NoCloseChances, dt);
        }

        void RefrshStragAndPlan()
        {
            try
            {
                if (UseSetting == null)
                {
                    return;
                }
                DataTable dt_strag = StragClass.ToTable<StragClass>(UseSetting.AllStrags.Values.ToList<StragClass>());
                if (dt_strag != null)
                {
                    SetDataGridDataTable(dg_StragList, dt_strag);
                }
                DataTable dt_plans = StragRunPlanClass.ToTable<StragRunPlanClass>(UseSetting.AllRunPlannings.Values.ToList<StragRunPlanClass>());
                if (dt_plans != null)
                {
                    SetDataGridDataTable(dg_stragStatus, dt_plans);
                }
                DataTable dt_grps = CalcStragGroupClass.ToTable<CalcStragGroupClass>(UseSetting.AllRunningPlanGrps.Values.ToList<CalcStragGroupClass>());
                if (dt_grps != null)
                {
                    SetDataGridDataTable(dg_PlanGrps, dt_grps);
                }
                DataTable dt_assetunit = AssetUnitClass.ToTable<AssetUnitClass>(UseSetting.AllAssetUnits.Values.ToList<AssetUnitClass>());
                if (dt_assetunit != null)
                    SetDataGridDataTable(dg_AssetUnits, dt_assetunit);
                refresh_AssetChart();
                
            }
            catch (Exception e)
            {
            }
        }

        void RefreshLogInfo(object obj, ElapsedEventArgs e)
        {
            ////this.Cursor = Cursors.WaitCursor;
            //////RefreshLogData();
            ////this.Cursor = Cursors.Default;
        }

        void RefreshLogData()
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
            }
        }

        public void RefreshLogData(DataTable dt)
        {
            try
            {
                SetDataGridDataTable(dg_LoginList, dt);
            }
            catch (Exception ce)
            {
                LogableClass.ToLog("监控终端", "刷新数据错误", ce.Message);
            }
        }

        private void frm_StragMonitor_Load(object sender, EventArgs e)
        {
            RefreshPK10Data();
            RefreshPK10NoClosedChances();
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
        
        private void dg_stragStatus_MouseUp(object sender, MouseEventArgs e)
        {
            DisableAllMenus();
            StragRunPlanClass strag = getGridAfterMouseUp<StragRunPlanClass>(this.dg_stragStatus, e) as StragRunPlanClass;
            ////this.tmi_StartPlan.Enabled = false;
            ////this.tmi_StopPlan.Enabled = false;
            ////this.tmi_Edit.Enabled = false;
            tmi_refreshPlans.Enabled = true; 
            if (strag == null) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.tmi_Edit.Enabled = true;
                this.tmi_StopPlan.Enabled = strag.Running;
                this.tmi_StartPlan.Enabled = !strag.Running;
            }
        }

        object getGridAfterMouseUp<T>(DataGridView dg, MouseEventArgs e)
        {

            if (dg.SelectedRows.Count <= 0) return default(T);
            if (dg.SelectedRows[0].Index < 0) return default(T);
            int index = dg.SelectedRows[0].Index;
            DataTable dt = dg.Tag as DataTable;
            if (dt == null)
            {
                MessageBox.Show("附加表为空！");
                return default(T);
            }
            if (dt.Rows.Count < index + 1)
            {
                MessageBox.Show("表长度不足！");
                return default(T);
            }
            DataRow dr = dt.Rows[index];
            if (dr["GUID"] == null)
            {
                MessageBox.Show("GUID未找到！");
                return default(T);
            }
            string pguid = dr["GUID"].ToString();
            if (typeof(T) == typeof(StragClass))
            {
                return getServiceStrag(pguid);
            }
            else if (typeof(T) == typeof(StragRunPlanClass))
            {
                return getServicePlan(pguid);
            }
            else
            {
                return default(T);
            }
            //return plan;
        }

        StragRunPlanClass getPlanAfterMouseUp(MouseEventArgs e)
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
            StragRunPlanClass plan = getServicePlan(pguid);
            return plan;
        }

        private void tsmi_refreshPlans_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
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
                   dt = StragRunPlanClass.ToTable<StragRunPlanClass>(UseSetting.AllRunPlannings.Values.ToList<StragRunPlanClass>());
                   
               }
               else if (dg.Equals(this.dg_StragList))
               {
                   dt = StragClass.ToTable<StragClass>(UseSetting.AllStrags.Values.ToList<StragClass>());
                   
               }
               if (dt == null)
                   return;
               SetDataGridDataTable(dg, dt);
            }
            catch (Exception ce)
            {
            }
        }

        void DisableAllMenus()
        {
            for (int i = 0; i < this.contextMenuStrip_OperatePlan.Items.Count; i++)
                this.contextMenuStrip_OperatePlan.Items[i].Enabled = false;
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

        bool SetPlanStatus(bool Start,EventArgs e)
        {
             StragRunPlanClass plan = getPlanAfterMouseUp(e as MouseEventArgs);
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
                    DataTable dt_plans = StragRunPlanClass.ToTable<StragRunPlanClass>(UseSetting.AllRunPlannings.Values.ToList<StragRunPlanClass>());
                    if (dt_plans == null)
                        return ret;
                    SetDataGridDataTable(dg_stragStatus, dt_plans);
                }
                return ret;
            }
            catch (Exception ce)
            {
            }
            return false;
        }


        StragRunPlanClass getServicePlan(string guid)
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
            }
            return null;
        }

        StragClass getServiceStrag(string guid)
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
            }
            return null;
        }

        private void dg_StragList_MouseUp(object sender, MouseEventArgs e)
        {
            DisableAllMenus();
            StragClass strag = getGridAfterMouseUp<StragClass>(this.dg_StragList, e) as StragClass;
            tmi_refreshPlans.Enabled = true;            
            if (strag == null) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.tmi_Edit.Enabled = true;
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
                    frm.SpecList = UseSetting.AllStrags;
                    frm.SpecObject = strag;
                    frm.Show();
                }
                else if (dg.Equals(this.dg_stragStatus))
                {
                    StragRunPlanClass strag = getGridAfterMouseUp<StragRunPlanClass>(dg, null) as StragRunPlanClass;
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
                    frm.SpecList = UseSetting.AllRunPlannings;
                    frm.SpecObject = strag;
                    frm.Show();
                }
            }
            catch(Exception ce)
            {
                MessageBox.Show(string.Format("{0}:{1}",ce.Message, ce.StackTrace));
            }
        }

        private void dg_AssetUnits_DoubleClick(object sender, EventArgs e)
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
            this.chart_ForGuide.Series[0].Points.DataBindXY(moneyLines, "id", moneyLines, "val");
        }


        void refresh_AssetChart()
        {
            int DefaultDisplayLength = 1000;
            int Len = 0;
            if(!int.TryParse(txt_AssetTimeLength.Text,out Len))
            {
                Len = DefaultDisplayLength;
            }
            try
            {
                Chart chrt = this.chart_ForGuide;
                Dictionary<string, AssetUnitClass> assetUnits = UseSetting.AllAssetUnits;
                ////////////string strExpect = dt.Rows[dt.Rows.Count - 1]["Expect"].ToString();
                ////////////Int64 MinExpect = Int64.Parse(strExpect) - 180;
                ////////////string sql = string.Format("Expect>={0}", MinExpect);
                ////////////DataView dv_stddev = new DataView(dt);
                ////////////dv_stddev.RowFilter = sql;
                bool Changed = false;
                foreach (string id in assetUnits.Keys)
                {
                    DataTable dt = assetUnits[id].SummaryLine();
                    if (this.AssetTimeSummary.ContainsKey(id) == false || (this.AssetTimeSummary.ContainsKey(id) && dt.Rows.Count != this.AssetTimeSummary[id]))
                    {
                        if(!this.AssetTimeSummary.ContainsKey(id))
                        {
                            this.AssetTimeSummary.Add(id, 0);
                        }
                        this.AssetTimeSummary[id] = dt.Rows.Count; 
                        Changed = true;
                        assetUnits[id].SaveDataToFile();
                        //saveAssetLines(id, dt);//保存
                    }
                }
                if (!Changed)//没有任何改变，不刷新
                    return;
                
                int i = 0;
                if (chrt.Series.Count < assetUnits.Count - 1)
                {
                    chrt.Series.Clear();
                    
                    foreach (string strName in assetUnits.Keys)
                    {
                        Series sr = new Series(assetUnits[strName].UnitName);
                        sr.ChartType = SeriesChartType.Line;
                        chrt.Series.Add(sr);
                        i++;
                    }
                    //chrt.GetToolTipText += new EventHandler<ToolTipEventArgs>(chart_ForSystemStdDev_GetToolTipText);
                }
                i = 0;
                foreach (string strName in assetUnits.Keys)
                {
                    DataTable dt = assetUnits[strName].SummaryLine();
                    var drs = dt.Select("id>0", "id desc").Take<DataRow>(Len);
                    DataTable DispDt = dt.Clone();
                    foreach(DataRow dr in drs)
                    {
                        DispDt.Rows.Add(dr.ItemArray);
                    }                    
                    DataView dv = new DataView(DispDt);
                    dv.Sort = "id asc";
                    chrt.Series[i].Points.DataBindY(dv, "val");
                    i++;
                }
                this.chart_ForGuide = chrt;
                this.chart_ForGuide.Show();
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
                LogLib.LogableClass.ToLog("监控", e.Message, e.StackTrace);
            }
        }
             

        private void btn_adjustAssetTimeLength_Click(object sender, EventArgs e)
        {
            refresh_AssetChart();
        }
    }

    
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.ExchangeLib;
using WolfInv.com.Strags;
using System.Reflection;
using WolfInv.com.SecurityLib;
using System.ServiceProcess;
using WolfInv.com.PK10CorePress;
using DataRecSvr;

namespace PK10Server
{
    public partial class frm_MoniteStrag : Form
    {
        public frm_MoniteStrag()
        {
            InitializeComponent();
        }
        bool Running = false;
        StragRunPlanClass<TimeSerialData> spr = null;
        BaseStragClass<TimeSerialData> runstg = null;
        string lastExpect = null;
        delegate void SetCtrlDelegate(string ctrlId, object obj);


        private void frm_MoniteStrag_Load(object sender, EventArgs e)
        {
            //Program.AllGlobalSetting.wxlog.Log("单策略监控","启用", string.Format(GlobalClass.LogUrl, GlobalClass.WXLogHost));
            this.txt_DtpName.Text = GlobalClass.TypeDataPoints.First().Key;
            timer1.Interval = 1000 * int.Parse(this.txt_intersec.Text.Trim());
            timer1.Tick += Timer1_Tick;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if(spr == null)
            {
                this.toolStripStatusLabel1.Text = "策略对象为空";
                return;
            }
            string sname = spr.StragName;
            
            try
            {
               
                if (runstg == null)
                {
                    //Assembly ass = Assembly.GetAssembly(typeof(StragClass));
                    //Type t = ass.GetType(sname);
                    //runstg = Activator.CreateInstance(t) as BaseStragClass<TimeSerialData>;
                    runstg = spr.PlanStrag;
                }

                if (runstg == null)
                {
                    return;
                }
                runstg.ReviewExpectCnt = int.Parse(this.txt_reviewcnt.Text);
                var dtps = GlobalClass.TypeDataPoints.Where(a => a.Key == this.txt_DtpName.Text.Trim());
                if(dtps.Count() == 0)
                {
                    MessageBox.Show("Dtp不存在");
                    return;
                }
                DataTypePoint dtp = dtps.First().Value;
                AmoutSerials amt = new AmoutSerials();
                ChanceClass<TimeSerialData> cc = new ChanceClass<TimeSerialData>();
                cc.ChanceCode = "C3/1,2,3";
                runstg.allowInvestmentMaxValue = spr.MaxLostAmount;
                runstg.setDataTypePoint(dtp);
                DataReader er = DataReaderBuild.CreateReader(dtp.DataType, null, null);
                ExpectList<TimeSerialData> ViewDataList = er.ReadNewestData<TimeSerialData>(DateTime.Now.AddDays(-30 * dtp.CheckNewestDataDays));
                string strexpect = ViewDataList.LastData.Expect;
                this.txt_lastExpect.Text = strexpect;
                this.lastExpect = strexpect;

                

                ExpectListProcessBuilder<TimeSerialData> elp = new ExpectListProcessBuilder<TimeSerialData>(dtp, ViewDataList);
                BaseCollection<TimeSerialData> sc = elp.getProcess().getSerialData(ViewDataList.Count, true);
                CalcService<TimeSerialData> calc = new CalcService<TimeSerialData>();
                calc.DataPoint = dtp;
                calc.IsTestBack = true;
                calc.CurrData = ViewDataList;
                Dictionary<string, CalcStragGroupClass<TimeSerialData>> allgroups = new Dictionary<string, CalcStragGroupClass<TimeSerialData>>();
                foreach(string key in Program.AllGlobalSetting.AllRunningPlanGrps.Keys)//备份计划组
                {
                    allgroups.Add(key, Program.AllGlobalSetting.AllRunningPlanGrps[key]);
                }
                Program.AllGlobalSetting.AllRunningPlanGrps = new Dictionary<string, CalcStragGroupClass<TimeSerialData>>();
                CalcStragGroupClass<TimeSerialData> csg = new CalcStragGroupClass<TimeSerialData>(dtp);
                csg.UseSPlans.Add(spr);
                csg.UseStrags.Add(spr.PlanStrag.GUID, spr.PlanStrag);
                csg.UseSerial = spr.PlanStrag.BySer;
                csg.UseAssetUnits.Add(spr.AssetUnitInfo.UnitId, spr.AssetUnitInfo);
                Program.AllGlobalSetting.AllRunningPlanGrps.Add(spr.GUID,csg);
                calc.setGlobalClass(Program.gc);
                calc.setAllSettingConfig(Program.AllGlobalSetting);//测试只使用单一计划组
                calc.OnFinishedCalc = (d) =>
                {
                    //MessageBox.Show(dtp.DataType + "执行完毕！");

                    foreach (string key in Program.AllGlobalSetting.AllNoClosedChanceList.Keys)
                    {
                        ChanceClass<TimeSerialData> nc = Program.AllGlobalSetting.AllNoClosedChanceList[key];

                        List<string> currLines = this.Txt_Chances.Lines.ToList();
                        currLines.Add(string.Format("{0}/{1}", nc.ChanceCode, nc.UnitCost));
                        Txt_Chances.Invoke(new SetCtrlDelegate(SetCtrlById), new object[]{this.Txt_Chances.Name,currLines.ToArray()});

                    }
                    this.Txt_Chances.Refresh();
                };
                calc.Calc();
                Program.AllGlobalSetting.AllRunningPlanGrps = allgroups;//恢复原有的计划组
                allgroups.Values.ToList().ForEach(
                    grp =>
                    {
                        lock (Program.AllGlobalSetting.AllStragIndexs)
                            foreach (var kv in grp.grpIndexs)
                            {
                                if (Program.AllGlobalSetting.AllStragIndexs.ContainsKey(kv.Key))
                                {
                                    Program.AllGlobalSetting.AllStragIndexs[kv.Key] = kv.Value;
                                }
                                else
                                {
                                    Program.AllGlobalSetting.AllStragIndexs.Add(kv.Key, kv.Value);
                                }
                            }
                    }
                    );
                return;

                List<ChanceClass<TimeSerialData>> cs = runstg.getChances(sc, ViewDataList.LastData);
                long val = (runstg as StragClass).getChipAmount(runstg.allowInvestmentMaxValue, cc, amt);
                
                if (ViewDataList == null || ViewDataList.Count == 0)
                {
                    this.toolStripStatusLabel1.Text = "未读取到数据！";
                    return;
                }
                if(ViewDataList.LastData == null)
                {
                    this.toolStripStatusLabel1.Text = "无数据";
                    return;
                }
                
                if (this.lastExpect == strexpect)
                {
                    this.toolStripStatusLabel1.Text = "无最新数据数据";
                    return;
                }
                
                
                
                
                
                this.Txt_Chances.Text = string.Join("\r\n", cs.Select(a => a.ChanceCode)) ;
                Program.AllGlobalSetting.wxlog.Log(string.Format("彩种{0}第{1}指令:\r\n{2}",dtp.DataType,this.lastExpect,this.Txt_Chances.Text));
            }
            catch (Exception ce)
            {
                btn_startMonite_Click(null, null);//出现意外，立即停止
                this.toolStripStatusLabel1.Text = ce.Message;
                return;
            }
        }

        void setChances(Control ctrl)
        {

        }

        void SetCtrlById(string id,object val)
        {
            Control[] ctrls = this.Controls.Find(id,false);
            if(ctrls.Length!=1)
            {
                return;
            }
            (ctrls[0] as TextBox).Lines =(string[]) val;
        }

        void SetCtrlById(string id, Action<Control> act)
        {
            Control[] ctrls = this.Controls.Find(id, false);
            if (ctrls.Length != 1)
            {
                return;
            }
            act(ctrls[0]);
        }

        private void btn_startMonite_Click(object sender, EventArgs e)
        {
            StragRunPlanClass<TimeSerialData>[] srps =  runPlanPicker1.Plans;
            if(srps == null || srps.Length == 0)
            {
                MessageBox.Show("请选择监控策略");
                return;
            }
            spr = srps[0];
            if(chkb_useTargeExpect.Checked)
            {
                useSpecExpect(this.txt_lastExpect.Text);
                return;
            }
            if (!Running)
            {
                if(!spr.AssetUnitInfo.Running)
                {
                    spr.AssetUnitInfo.Run();
                }
                Running = true;
                this.btn_startMonite.Text = "停止";
                Timer1_Tick(null, null);
            }
            else
            {
                Running = false;
                this.btn_startMonite.Text = "开始";
            }
            this.timer1.Enabled = Running;
        }

        void useSpecExpect(string strExpect)
        {
            try
            {

                if (runstg == null)
                {
                    //Assembly ass = Assembly.GetAssembly(typeof(StragClass));
                    //Type t = ass.GetType(sname);
                    //runstg = Activator.CreateInstance(t) as BaseStragClass<TimeSerialData>;
                    runstg = spr.PlanStrag;
                }

                if (runstg == null)
                {
                    return;
                }
                //runstg.ReviewExpectCnt = int.Parse(this.txt_reviewcnt.Text);
                var dtps = GlobalClass.TypeDataPoints.Where(a => a.Key == this.txt_DtpName.Text.Trim());
                if (dtps.Count() == 0)
                {
                    MessageBox.Show("Dtp不存在");
                    return;
                }
                DataTypePoint dtp = dtps.First().Value;
                AmoutSerials amt = new AmoutSerials();
                ChanceClass<TimeSerialData> cc = new ChanceClass<TimeSerialData>();
                cc.ChanceCode = "C3/1,2,3";
                runstg.allowInvestmentMaxValue = spr.MaxLostAmount;
                runstg.setDataTypePoint(dtp);
                DataReader er = DataReaderBuild.CreateReader(dtp.DataType, null, null);
                //ExpectList<TimeSerialData> ViewDataList = er.ReadHistory<TimeSerialData>( runstg.ReviewExpectCnt +20, this.txt_lastExpect.Text);
                long revCnt = runstg.ReviewExpectCnt;
                if(dtp.DataType != "PK10")
                {
                    long currLng = long.Parse(txt_lastExpect.Text);
                    string strDate = this.txt_lastExpect.Text.Substring(0,dtp.ExpectCodeDateLong);
                    DateTime currDate;
                    if (!DateTime.TryParseExact(strDate, dtp.ExpectCodeDateFormate,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AdjustToUniversal,
                    out currDate))
                    {
                        MessageBox.Show(strDate + "非正常日期格式！");
                        return;
                    }
                    int days = (int)Math.Ceiling((double)(revCnt / dtp.ExpectCodeCounterMax));//需要提前的数据长度除以单日最大数=日期数
                    long calcLng = long.Parse(currDate.AddDays(-1 * days).ToString(dtp.ExpectCodeDateFormate));
                    revCnt = currLng - calcLng;
                }
                ExpectList<TimeSerialData> ViewDataList = er.ReadNewestData<TimeSerialData>(long.Parse(this.txt_lastExpect.Text),(int)(revCnt + 20),false);
                string strexpect = ViewDataList.LastData.Expect;
                //this.txt_lastExpect.Text = strexpect;
                this.lastExpect = strexpect;



                ExpectListProcessBuilder<TimeSerialData> elp = new ExpectListProcessBuilder<TimeSerialData>(dtp, ViewDataList);
                BaseCollection<TimeSerialData> sc = elp.getProcess().getSerialData(ViewDataList.Count, true);
                CalcService<TimeSerialData> calc = new CalcService<TimeSerialData>();
                calc.DataPoint = dtp;
                calc.IsTestBack = true;
                calc.CurrData = ViewDataList;
                Dictionary<string, CalcStragGroupClass<TimeSerialData>> allgroups = new Dictionary<string, CalcStragGroupClass<TimeSerialData>>();
                if (Program.AllGlobalSetting != null)
                {
                    foreach (string key in Program.AllGlobalSetting.AllRunningPlanGrps.Keys)//备份计划组
                    {
                        allgroups.Add(key, Program.AllGlobalSetting.AllRunningPlanGrps[key]);
                    }
                }
                else
                {
                    Program.AllGlobalSetting = new WolfInv.com.ServerInitLib.ServiceSetting<TimeSerialData>();
                }
                Program.AllGlobalSetting.AllRunningPlanGrps = new Dictionary<string, CalcStragGroupClass<TimeSerialData>>();
                CalcStragGroupClass<TimeSerialData> csg = new CalcStragGroupClass<TimeSerialData>(dtp);
                csg.UseSPlans.Add(spr);
                csg.UseStrags.Add(spr.PlanStrag.GUID, spr.PlanStrag);
                csg.UseSerial = spr.PlanStrag.BySer;
                if(spr.AssetUnitInfo == null)
                {
                    spr.AssetUnitInfo = new AssetUnitClass();
                    spr.AssetUnitInfo.UnitId = "testSingleId";
                }
                csg.UseAssetUnits.Add(spr.AssetUnitInfo.UnitId, spr.AssetUnitInfo);
                Program.AllGlobalSetting.AllRunningPlanGrps.Add(spr.GUID, csg);
                calc.setGlobalClass(Program.gc);
                calc.setAllSettingConfig(Program.AllGlobalSetting);//测试只使用单一计划组
                calc.OnFinishedCalc = (d) =>
                {
                    //MessageBox.Show(dtp.DataType + "执行完毕！");

                    foreach (string key in Program.AllGlobalSetting.AllNoClosedChanceList.Keys)
                    {
                        ChanceClass<TimeSerialData> nc = Program.AllGlobalSetting.AllNoClosedChanceList[key];

                        List<string> currLines = this.Txt_Chances.Lines.ToList();
                        currLines.Add(string.Format("{2}=>{0}/{1}", nc.ChanceCode, nc.UnitCost,nc.ExpectCode));
                        Txt_Chances.Invoke(new SetCtrlDelegate(SetCtrlById), new object[] { this.Txt_Chances.Name, currLines.ToArray() });

                    }
                    this.Txt_Chances.Refresh();
                };
                calc.Calc();
                Program.AllGlobalSetting.AllRunningPlanGrps = allgroups;//恢复原有的计划组
                allgroups.Values.ToList().ForEach(
                    grp =>
                    {
                        lock (Program.AllGlobalSetting.AllStragIndexs)
                            foreach (var kv in grp.grpIndexs)
                            {
                                if (Program.AllGlobalSetting.AllStragIndexs.ContainsKey(kv.Key))
                                {
                                    Program.AllGlobalSetting.AllStragIndexs[kv.Key] = kv.Value;
                                }
                                else
                                {
                                    Program.AllGlobalSetting.AllStragIndexs.Add(kv.Key, kv.Value);
                                }
                            }
                    }
                    );
                return;
            }
            catch (Exception ce)
            {
                //btn_startMonite_Click(null, null);//出现意外，立即停止
                this.toolStripStatusLabel1.Text = ce.Message;
                MessageBox.Show(ce.Message);
                return;
            }

        }
    }
}

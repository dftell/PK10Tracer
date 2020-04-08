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
        
        private void frm_MoniteStrag_Load(object sender, EventArgs e)
        {
            Program.AllGlobalSetting.wxlog.Log("单策略监控","启用", string.Format(GlobalClass.LogUrl, GlobalClass.WXLogHost));
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
                DataTypePoint dtp = GlobalClass.TypeDataPoints.First().Value;
                AmoutSerials amt = new AmoutSerials();
                ChanceClass<TimeSerialData> cc = new ChanceClass<TimeSerialData>();
                cc.ChanceCode = "A3/1,2,3";
                runstg.allowInvestmentMaxValue = spr.MaxLostAmount;
                runstg.setDataTypePoint(dtp);
                DataReader er = DataReaderBuild.CreateReader(dtp.DataType, null, null);
                ExpectList<TimeSerialData> ViewDataList = er.ReadNewestData<TimeSerialData>(DateTime.Now.AddDays(-30 * dtp.CheckNewestDataDays));
                string strexpect = ViewDataList.LastData.Expect;
                this.txt_lastExpect.Text = strexpect;
                this.lastExpect = strexpect;
                ExpectListProcessBuilder<TimeSerialData> elp = new ExpectListProcessBuilder<TimeSerialData>(dtp, ViewDataList);
                BaseCollection<TimeSerialData> sc = elp.getProcess().getSerialData(ViewDataList.Count, true);
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

        private void btn_startMonite_Click(object sender, EventArgs e)
        {
            StragRunPlanClass<TimeSerialData>[] srps =  runPlanPicker1.Plans;
            if(srps == null || srps.Length == 0)
            {
                MessageBox.Show("请选择监控策略");
                return;
            }
            spr = srps[0];
            if(!Running)
            {
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
    }
}

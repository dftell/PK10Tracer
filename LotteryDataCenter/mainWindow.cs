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

namespace LotteryDataCenter
{
    public partial class mainWindow : Form
    {
        Dp_Timer[] timers;
        Dictionary<string, DateTime> LastInstsTimes;
        public mainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Load(object sender, EventArgs e)
        {
            init_Timers();
            
        }

        void init_Timers()
        {
            timers = new Dp_Timer[GlobalClass.DataTypes.Count];
            int i = 0;
            LastInstsTimes = new Dictionary<string, DateTime>();
            foreach(string key in GlobalClass.TypeDataPoints.Keys)
            {
                DataTypePoint dtp = GlobalClass.TypeDataPoints[key];
                timers[i] = new Dp_Timer(dtp);
                timers[i].Interval = defaultInterVal(dtp, false);
                timers[i].Enabled = true;
                timers[i].Tick += Timer_Tick;
                LastInstsTimes.Add(key, DateTime.MinValue);
                i++;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DataTypePoint dtp = (sender as Dp_Timer).dtp;
            if(dtp == null)
            {
                return;
            }
        }

        public void initForm()
        {
            this.tabControl1.TabPages.Clear();
            foreach (var dp in Program.dc.Points.Values)
            {
                TabPage tp = new TabPage(dp.PointTitle);
                DataPointPage dpp = new DataPointPage();
                dpp.ldp = dp;
                dpp.Dock = DockStyle.Fill;
                tp.Controls.Add(dpp);
                dpp.LoadPage();
                this.tabControl1.TabPages.Add(tp);
            }
        }
        int defaultInterVal(DataTypePoint dtp = null, bool receivedData = false)
        {
            if (dtp == null)
            {
                return getInterVal(GlobalClass.TypeDataPoints.First().Value, receivedData);
            }
            return getInterVal(dtp, receivedData);

        }
        int getInterVal(DataTypePoint dtp, bool receivedData)
        {
            long RecSecs = dtp.ReceiveSeconds;
            int noRecVal = (int)RecSecs / 10;
            int rdm = new Random().Next(noRecVal);
            if (!receivedData)
            {
                return noRecVal * 1000;
            }
            if (DateTime.Now.Subtract(LastInstsTimes[dtp.DataType]).Seconds > 3 * RecSecs / 2)//如果超过了3/2，就算是接收到了数据，也要以未接收的频率接收本期数据
            {
                return noRecVal * 1000;
            }
            return 1000 * (8 * noRecVal + rdm);
        }
    }


    public class Dp_Timer : Timer
    {
        public DataTypePoint dtp;
        public Dp_Timer(DataTypePoint _ldp)
        {
            dtp = _ldp;
            Tag = dtp;
        }
    }

}

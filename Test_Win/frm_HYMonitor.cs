using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using LogLib;
using DataRecSvr;
namespace Test_Win
{
    public partial class frm_HYMonitor : Form
    {
        SubscriptData sd ;
        public frm_HYMonitor()
        {
            InitializeComponent();
            InitService();
        }

        void InitService()
        {
            ServiceBase[] ServicesToRun;
            LogableClass.ToLog("构建计算服务", "开始");
            CalcService cs = new CalcService();
            LogableClass.ToLog("构建接收服务", "开始");
            sd = new SubscriptData();

            //只有接收数据是默认启动，计算服务由接收数据触发
            ServicesToRun = new ServiceBase[]
            {
                    sd
            };
            
            //ServiceBase.Run(ServicesToRun);
        }

        private void frm_HYMonitor_Load(object sender, EventArgs e)
        {
            this.btn_Stop.Enabled = false;
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            sd.Start();
            this.btn_Stop.Enabled = true;
            this.btn_Run.Enabled = false;
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            sd.Stop();
            this.btn_Stop.Enabled = false;
            this.btn_Run.Enabled = true;
        }
    }
}

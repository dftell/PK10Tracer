using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using WolfInv.com.LogLib;
using DataRecSvr;
namespace Test_Win
{
    public partial class frm_HYMonitor : Form
    {
        SubscriptData sd ;
        public frm_HYMonitor()
        {
            InitializeComponent();
         
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

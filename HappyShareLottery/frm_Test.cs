using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HappyShareLottery
{
    public partial class frm_Test : Form
    {
        public frm_Test()
        {
            InitializeComponent();
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            txt_returnUrl.Text =  ChartPushService.getShortLink(this.txt_testUrl.Text.Trim());

        }
    }
}

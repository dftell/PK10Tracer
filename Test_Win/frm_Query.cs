using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.CFZQ_LHProcess;
using WolfInv.com.StrategyLibForWD;
using WolfInv.com.SecurityLib;
namespace Test_Win
{
    public partial class frm_Query : Form
    {
        GlobalObj gb;
        public frm_Query(GlobalObj _gb)
        {
            gb = _gb;
            InitializeComponent();
        }

        private void btn_query_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string indexcode = new SIIIClass().SummaryCode; //必须先主动获取
            MTable mt = CommWDToolClass.getBkList(this.gb.w, indexcode, this.txt_EndT.Value);
            if (mt == null) return;
            if (mt.Count == 0)
            {
                statusStrip1.Text = string.Format("共计记录条数：{0}", 0);
                toolStripStatus_Right.Text = "无法查找到记录";
                dataGridView1.DataSource = null;
                dataGridView1.Show();
                this.Cursor = Cursors.Default;
                return;
            }
            MTable secTab = mt["wind_code"];
            string[] sectors = secTab.ToList<string>().ToArray();
            MTable tmp = CommWDToolClass.GetMutliSetData(this.gb.w,
                sectors,
                this.txt_EndT.Value,
                (Cycle)this.ddl_cycle.SelectedIndex,
                PriceAdj.Beyond,
                false,
                txt_Content.Text);
            DataTable fdt = tmp.GetTable();
            this.dataGridView1.DataSource = fdt;
            dataGridView1.Tag = fdt;
            dataGridView1.Show();
            this.Cursor = Cursors.Default;
        }
    }
}

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
using WAPIWrapperCSharp;
using WolfInv.com.BaseObjectsLib;
namespace Test_Win
{
    public partial class frm_siII : Form
    {
        GlobalObj gb=null;
        public frm_siII(GlobalObj _gb)
        {
            gb = _gb;
            InitializeComponent();
            initForm();
        }
         
        void initForm()
        {
            this.ddl_cycle.SelectedIndex = 0;
            this.ddl_priceAdj.SelectedIndex = 1;
        }
        private void btn_run_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string indexcode = new SIIIClass().SummaryCode; //必须先主动获取
            MTable mt = CommWDToolClass.getBkList(this.gb.w, indexcode, this.txt_EndT.Value);
            if (mt == null) return;
            if ( mt.Count == 0)
            {
                toolStripStatus_left.Text = string.Format("共计记录条数：{0}", 0);
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
                (PriceAdj)this.ddl_priceAdj.SelectedIndex,
                false,
                "KDJ.J");
            mt.AddColumnByArray("CurrCycleValue", tmp, "J");
            DateTime preDate = WDDayClass.Offset(this.gb.w, this.txt_EndT.Value, -1, (Cycle)this.ddl_cycle.SelectedIndex);
            tmp = CommWDToolClass.GetMutliSetData(this.gb.w,
                sectors,
                preDate,
                (Cycle)this.ddl_cycle.SelectedIndex,
                (PriceAdj)this.ddl_priceAdj.SelectedIndex,
                false,
                "KDJ.J");
            mt.AddColumnByArray("PreCycleValue", tmp, "J");
            //高一频度周期数据
            if (this.ddl_cycle.SelectedIndex > 0)
            {
                tmp = CommWDToolClass.GetMutliSetData(this.gb.w,
                sectors,
                this.txt_EndT.Value,
                (Cycle)(this.ddl_cycle.SelectedIndex-1),
                (PriceAdj)this.ddl_priceAdj.SelectedIndex,
                false,
                "KDJ.K,KDJ.J");
                mt.AddColumnByArray("LowCycleValue", tmp, "J");
                mt.AddColumnByArray("LowCycleK", tmp, "K");
                tmp = CommWDToolClass.GetMutliSetData(this.gb.w,
                sectors,
                preDate,
                (Cycle)(this.ddl_cycle.SelectedIndex-1),
                (PriceAdj)this.ddl_priceAdj.SelectedIndex,
                false,
                "KDJ.J");
                mt.AddColumnByArray("PreLowCycleValue", tmp, "J");
            }
            MTable dte = mt;
            MTable fdt = dte.Select("PreCycleValue>CurrCycleValue And (PreCycleValue <5 and CurrCycleValue<5)  And (LowCycleK<20 Or LowCycleValue<=5 Or PreLowCycleValue>LowCycleValue)");
            toolStripStatus_Right.Text = "完成";
            toolStripStatus_left.Text = string.Format("共计记录条数：{0}", fdt.Count);
            dataGridView1.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView1_CellMouseDoubleClick);
            dataGridView1.DataSource = fdt.GetTable();
            dataGridView1.Tag = fdt;
            dataGridView1.Show();
            this.Cursor = Cursors.Default;
        }

        void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
        }

        void DrawForm(Form frm)
        {
            frm.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            frm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            frm.ClientSize = new System.Drawing.Size(800, 600);
            frm.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            frm.Name = "frm_siII";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        void DrawGrid(DataGridView dg)
        {
            dg.Dock = System.Windows.Forms.DockStyle.Fill;
            dg.Location = new System.Drawing.Point(0, 0);
            dg.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            dg.Name = "dataGridView1";
            dg.ReadOnly = true;
            dg.RowTemplate.Height = 23;
            dg.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dg.Size = new System.Drawing.Size(788, 588);
            dg.TabIndex = 0;
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            
            //DataTable table = (DataTable)dataGridView1.DataSource;//数据源  
            //string secid = table.Rows[e.RowIndex]["wind_code"].ToString();
            //string SecName = table.Rows[e.RowIndex]["sec_name"].ToString();
            string secid = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["wind_code"].Value.ToString();
            string SecName = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["sec_name"].Value.ToString();
            SecIndexClass sic = new SIIIClass();
            sic.IndexCode = secid;
            secIndexBuilder sib = new secIndexBuilder(this.gb.w, sic);
            MTable mt = sib.getBkWeight(this.txt_EndT.Value);
            if (mt == null) return;
            DataTable dt = mt.GetTable();
            if (dt == null || dt.Rows.Count == 0 || !dt.Columns.Contains("i_weight"))
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(secid + "无详细信息！");
                return;
            }
            dt.Columns["i_weight"].DataType = typeof(float);
            dt.Columns.Add("CurrCycleValue", typeof(float));

            string[] sectors = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sectors[i] = dt.Rows[i]["wind_code"].ToString();
            }
            KDJGuidClass kdj = new KDJGuidClass(9, 3, 3);
            kdj.cycle = (Cycle)this.ddl_cycle.SelectedIndex;
            kdj.priceAdj = (PriceAdj)this.ddl_priceAdj.SelectedIndex;
            GuidBuilder gb = new GuidBuilder(this.gb.w, kdj);
            MTable newdt = gb.getRecords(sectors, this.txt_EndT.Value);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["CurrCycleValue"] = newdt.GetTable().Rows[i][0];
            }
            if (this.ddl_cycle.SelectedIndex > 0)
            {
                dt.Columns.Add("LowCycleValue", typeof(float));
                kdj.cycle = (Cycle)this.ddl_cycle.SelectedIndex - 1;
                gb = new GuidBuilder(gb.w, kdj);
                newdt = gb.getRecords(sectors, this.txt_EndT.Value);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["LowCycleValue"] = newdt.GetTable().Rows[i][0];
                }
            }
            Form dlgfrm = new Form();
            DrawForm(dlgfrm);
            DataGridView ddgrid = new DataGridView();
            dlgfrm.Controls.Add(ddgrid);
            DrawGrid(ddgrid);
            //dataGridView1.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView1_CellMouseDoubleClick);
            ddgrid.DataSource = dt;
            ddgrid.Tag = dt;
            ddgrid.Show();
            dlgfrm.TopMost = true;
            dlgfrm.Text = SecName;
            dlgfrm.ShowDialog(this);
            this.Cursor = Cursors.Default;
        }
    }
}

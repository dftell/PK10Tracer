using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
namespace ExchangeTermial
{
    public partial class frm_setting : Form
    {
        GlobalClass gc;
        Dictionary<string, string> aul;
        public frm_setting(Dictionary<string,string> assetUnitList)
        {
            InitializeComponent();
            aul = assetUnitList;
            LoadForm();
        }

        void LoadForm()
        {
            gc =  Program.gc;
            DataTable dt = getAssetUnitSetting(aul, gc.AssetUnits);
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Tag = dt;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            DataTable dt = this.dataGridView1.Tag as DataTable;
            Dictionary<string, int> ret = new Dictionary<string, int>();
            for(int i=0;i<dt.Rows.Count;i++)
            {
                ret.Add(dt.Rows[i]["id"].ToString(), int.Parse(dt.Rows[i]["cnt"].ToString()));
            }
            gc.AssetUnits = ret;
            try
            {
                GlobalClass.SetConfig();
                this.Close();
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }

        DataTable getAssetUnitSetting(Dictionary<string,string> aul,Dictionary<string,int> aus)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");
            dt.Columns.Add("cnt");
            
            foreach(string key in aul.Keys)
            {
                DataRow dr = dt.NewRow();
                dr["id"] = key;
                dr["name"] = aul[key];
                dr["cnt"] = aus.ContainsKey(key) ? aus[key] : 1;
                dt.Rows.Add(dr);
            }
            dt.Columns["id"].ReadOnly = true;
            dt.Columns["name"].ReadOnly = true;
            return dt;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int NewVal = int.Parse(dataGridView1.CurrentCell.Value.ToString());
                DataTable dt = this.dataGridView1.Tag as DataTable;
                dt.Rows[e.RowIndex]["cnt"] = NewVal;
                this.dataGridView1.Tag = dt;
            }
            catch(Exception ce)
            {
                string msg = ce.Message;
                return;
            }
        }
    }
}

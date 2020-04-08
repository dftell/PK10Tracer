using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.WebCommunicateClass;
//using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
using System.Web;
//using WolfInv.com.SecurityLib;
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
            try
            {
                DataTable dt = this.dataGridView1.Tag as DataTable;
                Dictionary<string, AssetInfoConfig> ret = new Dictionary<string, AssetInfoConfig>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    AssetInfoConfig aic = new AssetInfoConfig();
                    int.TryParse(dt.Rows[i]["cnt"]?.ToString(), out aic.value);
                    //int.TryParse(dt.Rows[i]["needSelectTime"]?.ToString(), out aic.NeedSelectTimes);
                    aic.NeedSelectTimes = (bool)dt.Rows[i]["needSelectTime"]==true ? 1 : 0;
                    double.TryParse(dt.Rows[i]["gainedUbound"]?.ToString(), out aic.maxStopGainedValue);
                    ret.Add(dt.Rows[i]["id"].ToString(), aic);
                }
                Program.gc.AssetUnits = ret;

                SaveToServer(ret);
                GlobalClass.SetConfig(Program.gc.ForWeb);
                this.Close();
            }
            catch (Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }

        void SaveToServer(Dictionary<string,AssetInfoConfig> assets)
        {
            string strReq = "<config  type='AssetUnits'>{0}</config>";
            string[] list = assets.Select(a => string.Format("{0}", GlobalClass.writeXmlItems(a.Value.getStringDic(),a.Key,a.Value.value))).ToArray();
            string urlModel = "http://www.wolfinv.com/pk10/app/UpdateUser.asp?UserId={0}&AssetConfig={1}&dir=1";
            string reqAsset = string.Format(strReq, string.Join("", list));
            string encode = HttpUtility.UrlEncode(reqAsset, Encoding.UTF8);
            string url = string.Format(urlModel, Program.UserId, encode);
            string succ = AccessWebServerClass.GetData(url, Encoding.UTF8);
            if(succ == "succ")
            {
                MessageBox.Show(succ);
            }

        }

        DataTable getAssetUnitSetting(Dictionary<string,string> aul,Dictionary<string,AssetInfoConfig> aus)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");
            dt.Columns.Add("cnt");
            dt.Columns.Add("needSelectTime",typeof(bool));
            dt.Columns.Add("gainedUbound");
            foreach(string key in aul.Keys)
            {
                DataRow dr = dt.NewRow();
                dr["id"] = key;
                dr["name"] = aul[key];
                dr["cnt"] = aus.ContainsKey(key) ? aus[key].value : 1;
                dr["needSelectTime"] = aus.ContainsKey(key) ? aus[key].NeedSelectTimes : 0;
                dr["gainedUbound"] = aus.ContainsKey(key) ? aus[key].maxStopGainedValue : 0;
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

                //int NewVal = int.Parse(dataGridView1.CurrentCell.Value.ToString());
                DataTable dt = this.dataGridView1.Tag as DataTable;
                dt.Rows[e.RowIndex][e.ColumnIndex] = dataGridView1.CurrentCell.Value;//  NewVal;
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

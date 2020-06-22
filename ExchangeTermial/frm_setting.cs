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
                    int.TryParse(dt.Rows[i]["currTimes"]?.ToString(), out aic.CurrTimes);
                    int.TryParse(dt.Rows[i]["DefaultReturnTimes"]?.ToString(), out aic.DefaultReturnTimes);
                    int.TryParse(dt.Rows[i]["AutoTraceMinChips"]?.ToString(), out aic.AutoTraceMinChips);
                    aic.AutoResumeDefaultReturnValue = (bool)dt.Rows[i]["AutoResumeDefaultReturnValue"] ? 1 : 0;
                    aic.EmergencyStop = (bool)dt.Rows[i]["EmergencyStop"] ? 1 : 0;
                    aic.AutoEmergencyStop = (bool)dt.Rows[i]["AutoEmergencyStop"] ? 1 : 0;
                    int.TryParse(dt.Rows[i]["StopStepLen"]?.ToString(), out aic.StopStepLen);
                    int.TryParse(dt.Rows[i]["StopIgnoreLength"]?.ToString(), out aic.StopIgnoreLength);
                    int.TryParse(dt.Rows[i]["StopPower"]?.ToString(), out aic.StopPower);
                    aic.ZeroCloseResume = (bool)dt.Rows[i]["ZeroCloseResume"] ? 1 : 0;
                    aic.NeedStopGained = (bool)dt.Rows[i]["NeedStopGained"]?1:0;
                    double.TryParse(dt.Rows[i]["gainedUbound"]?.ToString(), out aic.maxStopGainedValue);
                    ret.Add(dt.Rows[i]["id"].ToString(), aic);
                }
                Program.gc.AssetUnits = ret;

                if(!SaveToServer(ret,true))
                {
                    return;
                }
                GlobalClass.SetConfig(Program.gc.ForWeb);
                this.Close();
            }
            catch (Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }

        public static bool SaveToServer(Dictionary<string,AssetInfoConfig> assets,bool Tip=false)
        {
            string strReq = "<config  type='AssetUnits'>{0}</config>";
            string[] list = assets.Select(a => string.Format("{0}", GlobalClass.writeXmlItems(a.Value.getStringDic(),a.Key,""))).ToArray();
            string urlModel = "http://www.wolfinv.com/pk10/app/UpdateUser.asp?";
            
            string reqAsset = string.Format(strReq, string.Join("", list));
            string encode = HttpUtility.UrlEncode(reqAsset, Encoding.UTF8);
            string url = urlModel;
            string strPost = string.Format("UserId={0}&dir=1&AssetConfig={1}&web={2}", Program.UserId, encode,Program.gc.ForWeb);
            string succ = AccessWebServerClass.PostData(url,strPost, Encoding.UTF8);
            
            bool ret = (succ == "succ");
            if(!ret)
            {
                if (Tip)
                {
                    MessageBox.Show(succ);
                    Program.wxl.Log(succ);
                }
                else
                    MainWindow.WXMsgBox("保存配置失败！", succ);
                    
            }
            return ret;
        }

        DataTable getAssetUnitSetting(Dictionary<string,string> aul,Dictionary<string,AssetInfoConfig> aus)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");
            dt.Columns.Add("cnt");
            dt.Columns.Add("currTimes");
            dt.Columns.Add("DefaultReturnTimes");
            dt.Columns.Add("AutoResumeDefaultReturnValue",typeof(bool));
            dt.Columns.Add("ZeroCloseResume", typeof(bool));
            dt.Columns.Add("AutoTraceMinChips");
            
            dt.Columns.Add("AutoEmergencyStop",typeof(bool));
            dt.Columns.Add("EmergencyStop", typeof(bool));
            dt.Columns.Add("StopIgnoreLength");
            dt.Columns.Add("StopStepLen");
            dt.Columns.Add("StopPower");
            dt.Columns.Add("needSelectTime",typeof(bool));
            dt.Columns.Add("NeedStopGained", typeof(bool));
            dt.Columns.Add("gainedUbound");
            foreach(string key in aul.Keys)
            {
                DataRow dr = dt.NewRow();
                dr["id"] = key;
                dr["name"] = aul[key];
                dr["cnt"] = aus.ContainsKey(key) ? aus[key].value : 1;
                dr["currTimes"] = aus.ContainsKey(key) ? aus[key].CurrTimes : 0;
                dr["DefaultReturnTimes"] = aus.ContainsKey(key) ? aus[key].DefaultReturnTimes : 0;
                dr["AutoResumeDefaultReturnValue"] = aus.ContainsKey(key) ? aus[key].AutoResumeDefaultReturnValue : 0;
                dr["ZeroCloseResume"] = aus.ContainsKey(key) ? aus[key].ZeroCloseResume : 0;
                dr["AutoTraceMinChips"] = aus.ContainsKey(key) ? aus[key].AutoTraceMinChips : 0;
                dr["EmergencyStop"] = aus.ContainsKey(key) ? aus[key].EmergencyStop : 0;
                dr["AutoEmergencyStop"] = aus.ContainsKey(key) ? aus[key].AutoEmergencyStop : 0;
                dr["StopIgnoreLength"] = aus.ContainsKey(key) ? aus[key].StopIgnoreLength : 0;
                dr["StopStepLen"] = aus.ContainsKey(key) ? aus[key].StopStepLen : 0;
                dr["StopPower"] = aus.ContainsKey(key) ? aus[key].StopPower : 0;
                dr["needSelectTime"] = aus.ContainsKey(key) ? aus[key].NeedSelectTimes : 0;
                dr["NeedStopGained"] = aus.ContainsKey(key) ? aus[key].NeedStopGained : 0;
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

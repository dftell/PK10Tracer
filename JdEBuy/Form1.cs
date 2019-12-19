using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WolfInv.com.JdUnionLib;
using XmlProcess;
using WolfInv.Com.MetaDataCenter;
using WolfInv.Com.WCS_Process;
using System.Reflection;

namespace JdEBuy
{
    public partial class Form1 : Form
    {
        Timer downloadTimer ;
        JdUnion_GoodsDataLoadClass currJdc = null;
        public Form1(JdUnion_GoodsDataLoadClass jdc)
        {
            InitializeComponent();
            try
            {
                if (!Program.WCS_Inited)
                {
                    GlobalShare.MainAssem = Assembly.GetExecutingAssembly();
                    GlobalShare.AppDllPath = Application.StartupPath;
                    GlobalShare.Init(Application.StartupPath);
                    Program.ForceLogin();
                }
            }
            catch (Exception ce)
            {
                return;
            }
            currJdc = jdc;
            bool inited = JdUnion_GlbObject.Inited;
            //JdGoodsQueryClass.LoadAllcommissionGoods = loadAllData;
            downloadTimer = new Timer();
            downloadTimer.Interval = 2 * 60 * 60 * 1000;//6小时
            downloadTimer.Tick += DownloadTimer_Tick;
            downloadTimer.Enabled = false;
        }

       

        private void DownloadTimer_Tick(object sender, EventArgs e)
        {
            currJdc.downloadData();
        }

        void loadEliteData(eliteData el)
        {
            
            DataSet ds = el.data;
            EliteDataClass edc = new EliteDataClass(el.eliteId);
            for(int i=0;i<ds.Tables[0].Rows.Count;i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                JdGoodSummayInfoItemClass jsiic = new JdGoodSummayInfoItemClass();
                jsiic.skuId = dr["JGD02"].ToString();
                jsiic.skuName = dr["JGD03"].ToString();
                jsiic.couponLink = dr["JGD07"].ToString();
                jsiic.imgageUrl = dr["JGD08"].ToString();
                jsiic.materialUrl = dr["JGD09"].ToString();
                jsiic.price = dr["JGD11"].ToString();
                jsiic.discount = dr["JGD06"].ToString();
                if (dr["JGD15"] != null)
                    jsiic.elitId = int.Parse(dr["JGD15"].ToString());
                if (dr["JGD14"] != null)
                    jsiic.batchId = int.Parse(dr["JGD14"].ToString());
                if (dr["JGD16"] != null)
                    jsiic.isHot = int.Parse(dr["JGD16"].ToString());
                if (dr["JGD17"] != null)
                    jsiic.inOrderCount30Days = int.Parse(dr["JGD17"].ToString());
                edc.Data.Add(jsiic.skuId,jsiic);
            }
            edc.lastUpdateTime = DateTime.Now;
            JdGoodsQueryClass.updateElites(el.eliteId, edc);
        }

       private void btn_recieveData_Click(object sender, EventArgs e)
        {
            if (currJdc == null)
                currJdc = new JdUnion_GoodsDataLoadClass();
            this.downloadTimer.Enabled = true;
            
            currJdc.SaveClientData = SaveClientData;
            this.DownloadTimer_Tick(null, null);
        }
        
        void UpdateText(string msg)
        {
            List<string> txtlist = this.txt_answer.Lines.ToList();
            txtlist.Add(msg);            
            this.txt_answer.Lines = txtlist.ToArray();
            
            this.txt_answer.Focus();//获取焦点
            this.txt_answer.Select(this.txt_answer.TextLength, 0);//光标定位到文本最后
            this.txt_answer.ScrollToCaret();//滚动到光标处
            this.txt_answer.Refresh();
            Application.DoEvents();
        }

        public virtual bool SaveClientData(string DetailSource, UpdateData updata, DataRequestType type = DataRequestType.Update)
        {
            string GridSource = "JdUnion_Client_Goods_Full";
            string strRowId = "";
            string strKey = "JGD02";
            updata.keydpt = new DataPoint(strKey);
            //if (!updata.Updated) return true;
            DataSource ds = GlobalShare.mapDataSource[DetailSource];
            //ds.SourceName = DetailSource;
            List<DataCondition> conds = new List<DataCondition>();
            DataCondition dcond = new DataCondition();
            dcond.Datapoint = new DataPoint(strKey);
            //dcond.value = strRowId;
            conds.Add(dcond);
            //DataRequestType type = DataRequestType.Update;
            if (strRowId == null || strRowId == "")
            {
                type = DataRequestType.Add;
            }
            if (GlobalShare.mapDataSource.ContainsKey(GridSource))
            {
                DataSource grid_source = GlobalShare.mapDataSource[GridSource];
                ds.SubSource = grid_source;
            }
            //updata.SubItems.Where(a=>a.ReqType)
            string msg = GlobalShare.DataCenterClientObj.UpdateDataList(ds, dcond, updata, type);
            if (msg != null)
            {
                //MessageBox.Show(msg);
                return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
             Dictionary<string,JdGoodSummayInfoItemClass> ret = JdGoodsQueryClass.Query(this.txt_ask.Text,10);
            List<string> retStrs = ret.Select(a => {
                //a.Value.commissionUrl = a.Value.getMyUrl(null);
                if (a.Value.commissionUrl == null)
                    return null;
                return a.Value.getFullContent(!string.IsNullOrEmpty(a.Value.commissionUrl));
            }).ToList();
            string strRet = string.Join("\r\n",retStrs.Where(a=>string.IsNullOrEmpty(a)==false));
            this.txt_answer.Text  = strRet;
            this.Cursor = Cursors.Default;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            WolfInv.com.JdUnionLib.Form2 frm = new Form2();
            frm.Show();
        }
    }

}

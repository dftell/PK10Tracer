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
using System.Threading.Tasks;
namespace JdEBuy
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer downloadTimer ;
        JdUnion_GoodsDataLoadClass currJdc = null;
        public Form1(JdUnion_GoodsDataLoadClass jdc)
        {
            InitializeComponent();
            try
            {
                if(jdc == null)
                {
                    jdc = new JdUnion_GoodsDataLoadClass();
                }
                if (!Program.WCS_Inited)
                {
                    jdc.UpdateText("初始化wcs");
                    string msg = null;
                    bool succ = Program.Init_WCS(out msg);
                    if(!succ)
                    {
                        jdc.UpdateText(msg);
                    }
                    jdc.UpdateText(string.Format("初始化wcs完毕,数据源数量为{0}!",GlobalShare.DataPointMappings?.Count));
                }
            }
            catch (Exception ce)
            {
                jdc.UpdateText(ce.Message);
                return;
            }
            currJdc = jdc;
            bool inited = false;
            try
            {
                inited = JdUnion_GlbObject.Inited;
            }
            catch(Exception ce)
            {
                return;
            }
            //JdGoodsQueryClass.LoadAllcommissionGoods = loadAllData;
            downloadTimer = new System.Windows.Forms.Timer();
            downloadTimer.Interval = 6 * 60 * 60 * 1000;//6小时
            downloadTimer.Tick += DownloadTimer_Tick;
            downloadTimer.Enabled = false;
            jdc.UpdateText?.Invoke("京东数据接受模块界面加载成功！");
        }

       

        private void DownloadTimer_Tick(object sender, EventArgs e)
        {
            new Task(currJdc.downloadData).Start();
        }

        void loadEliteData(eliteData el)
        {
            
            List<DataRow> ds = el.data;
            EliteDataClass edc = new EliteDataClass(el.eliteId);
            for(int i=0;i<ds.Count;i++)
            {
                DataRow dr = ds[i];
                JdGoodSummayInfoItemClass jsiic = new JdGoodSummayInfoItemClass();
                jsiic.skuId = dr["JGD02"].ToString();
                jsiic.skuName = dr["JGD03"].ToString();
                jsiic.couponLink = dr["JGD07"].ToString();
                jsiic.imgageUrl = dr["JGD08"].ToString();
                jsiic.materialUrl = dr["JGD09"].ToString();
                jsiic.price = dr["JGD11"].ToString();
                jsiic.discount = dr["JGD06"].ToString();
                if (dr.Table.Columns.Contains("JGD15") && dr["JGD15"] != null)
                    jsiic.elitId = int.Parse(dr["JGD15"].ToString());
                if (dr.Table.Columns.Contains("JGD16") && dr["JGD16"]!= null && !string.IsNullOrEmpty(dr["JGD16"].ToString()))
                    jsiic.isHot = int.Parse(dr["JGD16"].ToString());
                if (dr.Table.Columns.Contains("JGD17") && dr["JGD17"] != null && !string.IsNullOrEmpty(dr["JGD17"].ToString()))
                    jsiic.inOrderCount30Days = int.Parse(dr["JGD17"].ToString());
                if (dr.Table.Columns.Contains("JGD18") && dr["JGD18"] != null && !string.IsNullOrEmpty(dr["JGD18"].ToString()))
                    jsiic.seckillEndTime = dr["JGD18"].ToString();
                if (dr.Table.Columns.Contains("JGD19") && dr["JGD19"] != null && !string.IsNullOrEmpty(dr["JGD19"].ToString()))
                    jsiic.seckillOriPrice = dr["JGD19"].ToString();
                if (dr.Table.Columns.Contains("JGD20") && dr["JGD20"] != null && !string.IsNullOrEmpty(dr["JGD20"].ToString()))
                    jsiic.seckillPrice = dr["JGD20"].ToString();
                if (dr.Table.Columns.Contains("JGD21") && dr["JGD21"] != null && !string.IsNullOrEmpty(dr["JGD21"].ToString()))
                    jsiic.seckillStartTime = dr["JGD21"].ToString();
                if (!edc.Data.ContainsKey(jsiic.skuId))
                    edc.Data.Add(jsiic.skuId,jsiic);
            }
            edc.lastUpdateTime = DateTime.Now;
            JdGoodsQueryClass.updateElites(el.eliteId, edc);
        }

       private void btn_recieveData_Click(object sender, EventArgs e)
        {
            try
            {
                if (currJdc == null)
                    currJdc = new JdUnion_GoodsDataLoadClass();
                this.downloadTimer.Enabled = true;
                currJdc.onReceiveData = loadEliteData;
                currJdc.SaveClientData = SaveClientData;//保存单批数据
                currJdc.onSavedData = updateGlobalQueryData;//保存完数据处理，更新globalqueryobject
                this.DownloadTimer_Tick(null, null);
            }
            catch(Exception ce)
            {
                currJdc.UpdateText(string.Format("接收数据出现未知错误：{0}",ce.Message));
            }
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
            return true;
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
            JdGoodsQueryClass.LoadPromotionGoodsinfo = getInfoBySukIds;
            string msg = null;
            this.txt_answer.Text = "";
             Dictionary<string,JdGoodSummayInfoItemClass> ret = JdGoodsQueryClass.QueryFromLocal(this.txt_ask.Text,out msg);
            if(msg != null)
            {
                this.txt_answer.Text = msg;
                this.Cursor = Cursors.Default;
                return;
            }
            List<string> retStrs = ret.OrderBy(a=>a.Value.price).Select(a => {
                //a.Value.commissionUrl = a.Value.getMyUrl(null);
                if (a.Value?.materialUrl == null)
                    return null;
                a.Value.shortLinkFunc = JdGoodsQueryClass.getShortLink;
                return a.Value.getFullContent(true) + a.Value.getShortLink("1969838238");
                //return a.Value.getFullContent(!string.IsNullOrEmpty(a.Value?.commissionUrl)) ;
            }).ToList();
            if (retStrs.Count > 0)
            {
                string strRet = string.Join("\r\n", retStrs.Where(a => string.IsNullOrEmpty(a) == false));
                this.txt_answer.Text = strRet;
            }
            this.Cursor = Cursors.Default;

        }

        List<JdGoodSummayInfoItemClass> getInfoBySukIds(List<string> skids)
        {
            return JdGoodsQueryClass.getInfoBySukIds(skids);
            //JdUnion_Goods_PromotionGoodsinfo_Class
            string dsName = "JDUnion_PromotionGoodsinfo";
            List<JdGoodSummayInfoItemClass> ret = new List<JdGoodSummayInfoItemClass>();
            if(skids.Count==0)
            {
                return ret;
            }
            List<DataCondition> dcs = new List<DataCondition>();
            DataCondition dc = new DataCondition();
            dc.Datapoint = new DataPoint("skuIds");
            dc.value = string.Join(",", skids);
            dcs.Add(dc);
            string msg = null;
            bool isExtra = false;
            DataSet ds = DataSource.InitDataSource(dsName, dcs, GlobalShare.UserAppInfos.First().Key, out msg, ref isExtra,false);
            if(msg != null)
            {
                return ret;
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                JdGoodSummayInfoItemClass jsiic = new JdGoodSummayInfoItemClass();
                jsiic.skuId = dr["JGD02"].ToString();
                jsiic.skuName = dr["JGD03"].ToString();
                jsiic.imgageUrl = dr["JGD08"].ToString();
                jsiic.materialUrl = dr["JGD09"].ToString();
                jsiic.price = dr["JGD11"].ToString();
                if (dr.Table.Columns.Contains("JGD18"))
                {
                    jsiic.seckillEndTime = dr["JGD18"].ToString();
                    jsiic.seckillOriPrice = dr["JGD19"].ToString();
                    jsiic.seckillPrice = dr["JGD20"].ToString();
                    jsiic.seckillStartTime = dr["JGD21"].ToString();
                }
                ret.Add(jsiic);
            }
            return ret;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WolfInv.com.JdUnionLib.Form2 frm = new Form2();
            frm.Show();
        }

        void updateGlobalQueryData(Dictionary<string,UpdateData> data)
        {
            Dictionary<string, JdGoodSummayInfoItemClass> updateData = new Dictionary<string, JdGoodSummayInfoItemClass>();
            try
            {
                foreach (string key in data.Keys)
                {
                    if (updateData.ContainsKey(key))
                    {
                        continue;
                    }
                    UpdateData dr = data[key];
                    JdGoodSummayInfoItemClass jsiic = new JdGoodSummayInfoItemClass();
                    jsiic.skuId = dr.Items["JGD02"].value;
                    jsiic.skuName = dr.Items["JGD03"].value;
                    jsiic.couponLink = dr.Items["JGD07"].value;
                    jsiic.imgageUrl = dr.Items["JGD08"].value;
                    jsiic.materialUrl = dr.Items["JGD09"].value;
                    jsiic.price = dr.Items["JGD11"].value;
                    if(dr.Items.ContainsKey("JGD06") && dr.Items["JGD06"] != null)
                        jsiic.discount = dr.Items["JGD06"].value;
                    if (dr.Items.ContainsKey("JGD15") && dr.Items["JGD15"] != null)
                        jsiic.elitId = int.Parse(dr.Items["JGD15"].value);
                    if (dr.Items.ContainsKey("JGD14") && dr.Items["JGD14"] != null)
                        jsiic.batchId = int.Parse(dr.Items["JGD14"].value);
                    if (dr.Items.ContainsKey("JGD16") &&dr.Items["JGD16"] != null)
                        jsiic.isHot = int.Parse(dr.Items["JGD16"].value);
                    if (dr.Items.ContainsKey("JGD17") && dr.Items["JGD17"] != null)
                        jsiic.inOrderCount30Days = int.Parse(dr.Items["JGD17"].value);
                    if (dr.Items.ContainsKey("JGD18") && dr.Items["JGD18"] != null )
                        jsiic.seckillEndTime = dr.Items["JGD18"].ToString();
                    if (dr.Items.ContainsKey("JGD19") && dr.Items["JGD19"] != null )
                        jsiic.seckillOriPrice = dr.Items["JGD19"].ToString();
                    if (dr.Items.ContainsKey("JGD20") && dr.Items["JGD20"] != null )
                        jsiic.seckillPrice = dr.Items["JGD20"].ToString();
                    if (dr.Items.ContainsKey("JGD21") && dr.Items["JGD21"] != null )
                        jsiic.seckillStartTime = dr.Items["JGD21"].ToString();
                    updateData.Add(key, jsiic);
                }
                currJdc.UpdateText(string.Format("更新全局数据成功！"));
            }
            catch (Exception ce)
            {
                currJdc.UpdateText(string.Format("更新全局数据错误:{0}[{1}]",ce.Message,ce.StackTrace));
            }
            finally
            {
                JdGoodsQueryClass.updateAllData(updateData);
            }
        }
    }

}

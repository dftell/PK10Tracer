using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WolfInv.com.JdUnionLib;
using XmlProcess;
using System.Data;
using WolfInv.Com.MetaDataCenter;
using WolfInv.Com.WCS_Process;

namespace JdEBuy
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            bool inited = JdUnion_GlbObject.Inited;
            JdGoodsQueryClass.LoadAllcommissionGoods = loadAllData;
        }

        void loadAllData()
        {
            string datasourceName = "JdUnion_Goods";
            string msg = null;
            DataSet ds = DataSource.InitDataSource(datasourceName, new string[] { },new string[] { },out msg,true);
            if(msg != null)
            {
                MessageBox.Show(msg);
                return;
            }
            JdGoodsQueryClass.AllcommissionGoods = new Dictionary<string, JdGoodSummayInfoItemClass>();
            JdGoodsQueryClass.AllKeys = new Dictionary<string, List<string>>();
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
                JdGoodsQueryClass.AllcommissionGoods.Add(jsiic.skuId, jsiic);
                List<string> keys = JdGoodsQueryClass.splitTheWords(jsiic.skuName,true);
                JdGoodsQueryClass.AllKeys.Add(jsiic.skuId, keys);
            }
        }


        private void btn_recieveData_Click(object sender, EventArgs e)
        {
            List<int> list = JdUnion_GlbObject.getElites();
            Dictionary<string, string> cols = null;
            string datasourceName = "JdUnion_Goods";
            int ErrCnt = 0;
            try
            {
                string msg = null;
                bool isExtra = false;
                DataSet ds =  DataSource.InitDataSource(datasourceName, new List<DataCondition>(), Program.UserId, out msg, ref isExtra);
                List<UpdateData> ups = DataSource.DataSet2UpdateData(ds, datasourceName, Program.UserId);
                
                for(int i=0;i<ups.Count;i++)
                {
                    bool succ = SaveClientData(datasourceName, ups[i], DataRequestType.Add);
                    if(!succ)
                    {
                        //MessageBox.Show(string.Format("商品{0}保存错误！", ups[i].keyvalue));
                        ErrCnt++;
                    }
                    else
                    {
                        
                    }
                }
            }
            catch (Exception ce)
            {

            }
            if(ErrCnt>0)
            {
                MessageBox.Show(string.Format("错误条数为{0}条！",ErrCnt));
            }
        }

        public virtual bool SaveClientData(string DetailSource, UpdateData updata, DataRequestType type = DataRequestType.Update)
        {
            string GridSource = "";
            string strRowId = "";
            string strKey = "JGD02";
            updata.keydpt = new DataPoint(strKey);
            //if (!updata.Updated) return true;
            DataSource ds = GlobalShare.mapDataSource[DetailSource];
            List<DataCondition> conds = new List<DataCondition>();
            DataCondition dcond = new DataCondition();
            dcond.Datapoint = new DataPoint(strKey);
            dcond.value = strRowId;
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
             Dictionary<string,JdGoodSummayInfoItemClass> ret = JdGoodsQueryClass.Query(this.txt_ask.Text);
            string strRet = string.Join("/r/n",ret.Select(a=>a.Value.getFullContent()));
            this.txt_answer.Text  = strRet;

        }
    }
}

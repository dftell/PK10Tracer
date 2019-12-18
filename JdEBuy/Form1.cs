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
        Timer downloadTimer ;
        JdUnion_GoodsDataLoadClass currJdc = null;
        public Form1(JdUnion_GoodsDataLoadClass jdc)
        {
            InitializeComponent();
            currJdc = jdc;
            bool inited = JdUnion_GlbObject.Inited;
            JdGoodsQueryClass.LoadAllcommissionGoods = loadAllData;
            downloadTimer = new Timer();
            downloadTimer.Interval = 6 * 60 * 60 * 1000;//6小时
            downloadTimer.Tick += DownloadTimer_Tick;
        }

        private void DownloadTimer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void loadAllData()
        {
            string datasourceName = "JdUnion_Client_Goods_Coupon_NoXml";
            string msg = null;
            DataSource dsobj = GlobalShare.UserAppInfos.First().Value.mapDataSource[datasourceName];
            DataSet ds = DataSource.InitDataSource(dsobj,new List<DataCondition>(),out msg,true);
            if(msg != null)
            {
                UpdateText(msg);
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
            JdGoodsQueryClass.Inited = true;
        }

       private void btn_recieveData_Click(object sender, EventArgs e)
        {
            if (currJdc == null)
                currJdc = new JdUnion_GoodsDataLoadClass();
            currJdc.SaveClientData = SaveClientData;
            currJdc.UpdateText = UpdateText;
            currJdc.downloadData();
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

    public class JdUnion_GoodsDataLoadClass
    {
        public Action<string> UpdateText;
        public Func<string, UpdateData, DataRequestType, bool> SaveClientData;// ("jdUnion_BatchLoad", batchData, DataRequestType.Add)

        HashSet<string> loadAllKeys()
        {
            HashSet<string> ret = new HashSet<string>();
            string datasourceName = "JdUnion_Goods_Keys";
            string msg = null;
            DataSet ds = DataSource.InitDataSource(GlobalShare.UserAppInfos.First().Value.mapDataSource[datasourceName],new List<DataCondition>(), out msg);
            if (msg != null)
            {
                UpdateText?.Invoke(msg);
                return null;
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];

                string key = dr["JGD02"].ToString();
                if (!ret.Contains(key))
                    ret.Add(key);
            }
            return ret;
        }
        public Action<DataSet> onReceiveData;

        public void downloadData()
        {
            UpdateText.Invoke(string.Format("-------------开始下载 {0}------------", DateTime.Now));
            //List<int> list = JdUnion_GlbObject.getElites();
            //Dictionary<string, string> cols = null;
            HashSet<string> allExistKeys = loadAllKeys();
            if (allExistKeys == null)
                return;
            List<int> list = JdUnion_GlbObject.getElites();
            UpdateText?.Invoke(string.Format("当前数据库存在记录数{0}条！", allExistKeys.Count));
            Dictionary<string, int> dic = new Dictionary<string, int>();
            int ErrCnt = 0;
            int SaveCnt = 0;
            try
            {
                foreach (int elit in list)
                {
                    string msg = null;
                    bool isExtra = false;
                    List<DataCondition> dics = new List<DataCondition>();
                    DataCondition dc = new DataCondition();
                    dc.Datapoint = new DataPoint("goodsReq/eliteId");
                    dc.value = elit.ToString();
                    dics.Add(dc);
                    DataSet ds = DataSource.InitDataSource("JdUnion_Goods", dics, Program.UserId, out msg, ref isExtra);
                    new Task(receiveData, ds).Start();
                    List<UpdateData> ups = DataSource.DataSet2UpdateData(ds, "jdUnion_BatchLoad", Program.UserId);
                    UpdateText?.Invoke(string.Format("总共接收到{0}条记录", ups.Count));
                    UpdateData batchData = new UpdateData();
                    //batchData.Items.Add("JGD01", null);
                    int batchCnt = 1000;
                    for (int i = 0; i < ups.Count; i++)
                    {
                        string key = ups[i].Items["JGD02"].value;
                        if (allExistKeys.Contains(key))
                        {
                            continue;
                        }
                        if (dic.ContainsKey(key))
                        {
                            dic[key] = dic[key] + 1;
                            continue;
                        }
                        else
                        {
                            dic.Add(key, 1);
                        }
                        ups[i].keydpt = new DataPoint("JGD02");
                        ups[i].keyvalue = key;
                        ups[i].ReqType = DataRequestType.Add;
                        batchData.SubItems.Add(ups[i]);
                        if (i == ups.Count - 1 || batchData.SubItems.Count == batchCnt)
                        {
                            bool succ = (SaveClientData == null) ? false : (SaveClientData.Invoke("jdUnion_BatchLoad", batchData, DataRequestType.Add));
                            if (!succ)
                            {
                                //MessageBox.Show(string.Format("商品{0}保存错误！", ups[i].keyvalue));
                                ErrCnt++;
                            }
                            else
                            {
                                SaveCnt += batchData.SubItems.Count;
                            }
                            if ((SaveCnt % 100) == 0 && SaveCnt > 0)
                            {
                                UpdateText?.Invoke(string.Format("\r\n已经处理了{0}条数据，其中不重复记录数{1}条,成功保存了{2}条！", i, dic.Count, SaveCnt));
                                Application.DoEvents();
                            }
                            batchData = new UpdateData();
                        }


                    }
                    if (batchData.SubItems.Count > 0)//最后的不能错过。
                    {
                        bool succ = SaveClientData == null ? false : SaveClientData.Invoke("jdUnion_BatchLoad", batchData, DataRequestType.Add);
                        if (!succ)
                        {
                            //MessageBox.Show(string.Format("商品{0}保存错误！", ups[i].keyvalue));
                            ErrCnt++;
                        }
                        else
                        {
                            SaveCnt += batchData.SubItems.Count;
                        }
                    }
                    if (dic.Count > 0)
                    {
                        UpdateText?.Invoke(string.Format("共计条数为{0}条，实际保存条数为{1}条！", ups.Count, dic.Count));
                    }
                    if (ErrCnt > 0)
                    {
                        UpdateText?.Invoke(string.Format("错误条数为{0}条！", ErrCnt));
                    }
                }
            }
            catch (Exception ce)
            {
                UpdateText?.Invoke(string.Format("错误条数为{0}条！", ErrCnt));

            }
            finally
            {
                //this.Cursor = Cursors.Default;
            }

        }

        void receiveData(object obj)
        {
            onReceiveData?.Invoke(obj as DataSet);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.JdUnionLib;
using WolfInv.Com.MetaDataCenter;
using WolfInv.Com.WCS_Process;

namespace JdEBuy
{
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

        long? getCurrBatchNo()
        {
            string datasourceName = "JdUnion_Goods_BatchIds";
            string msg = null;
            DataSet ds = DataSource.InitDataSource(GlobalShare.UserAppInfos.First().Value.mapDataSource[datasourceName], new List<DataCondition>(), out msg);
            if (msg != null)
            {
                UpdateText?.Invoke(msg);
                return null;
            }
            string currDate = DateTime.Now.ToString("yyyyMMdd");
            long currBase = long.Parse(currDate) * 100;
            DataRow[] drs = ds.Tables[0].Select(string.Format("JBTH1>{0}", currBase), "JBTH1 desc");//年月日*100+当日批次号
            if(drs.Length==0)
            {
                return currBase + 1;
            }
            return  long.Parse(drs[0].ToString()) + 1;
            //return ret;
            
        }

        public Action<eliteData> onReceiveData;

        public void downloadData()
        {
            UpdateText.Invoke(string.Format("-------------开始下载 {0}------------", DateTime.Now));
            long? batchId = getCurrBatchNo();
            if (batchId == null)
            {
                UpdateText?.Invoke(string.Format("无法获取到批次号！"));
                return;
            }
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
                    if(msg != null)
                    {
                        UpdateText?.Invoke(string.Format("获取分类数据{0}时出现错误，内容为{1}", elit,msg));
                        continue;
                    }
                    eliteData ed = new eliteData();
                    ed.eliteId = elit;
                    ed.data = ds;
                    new Task(receiveData, ed).Start();
                    List<UpdateData> ups = DataSource.DataSet2UpdateData(ds, "jdUnion_BatchLoad", Program.UserId);
                    UpdateText?.Invoke(string.Format("总共接收到{0}条记录", ups.Count));
                    UpdateData batchData = new UpdateData();
                    batchData.keydpt = new DataPoint("JBTH1");
                    batchData.keyvalue = batchId.Value.ToString();
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
                            }
                            batchData = new UpdateData();
                        }


                    }
                    if (batchData.SubItems.Count > 0)//最后的不能错过。
                    {
                        bool succ = SaveClientData == null ? false : SaveClientData.Invoke("jdUnion_BatchLoad", batchData, DataRequestType.Add);
                        if (!succ)
                        {
                            for (int i = 0; i < ups.Count; i++)
                            {
                                string key = ups[i].Items["JGD02"].value;
                                if(!allExistKeys.Contains(key))
                                {
                                    allExistKeys.Add(key);
                                }
                            }
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
            onReceiveData?.Invoke(obj as eliteData);
        }

        
    }

   
}

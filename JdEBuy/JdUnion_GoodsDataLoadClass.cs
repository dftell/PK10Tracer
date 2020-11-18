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
            DataSource dsr = null;
            if(GlobalShare.mapDataSource.ContainsKey(datasourceName))
            {
                dsr = GlobalShare.mapDataSource[datasourceName];
            }
            if(dsr == null)
            {

                return null;
            }
            DataSet ds = DataSource.InitDataSource(dsr, new List<DataCondition>(), out msg);
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
            return  long.Parse(drs[0][0].ToString()) + 1;
            //return ret;
            
        }


        public Action<eliteData> onReceiveData;
        public Action<Dictionary<string, UpdateData>> onSavedData;

        public void downloadData()
        {
            bool dataConnected = false;
            HashSet<string> allExistKeys = new HashSet<string>();
            Dictionary<string, UpdateData> newAddRec = new Dictionary<string, UpdateData>();
            try
            {
                UpdateText?.Invoke(string.Format("-------------开始下载 {0}------------", DateTime.Now));
                long? batchId = getCurrBatchNo();
                if (batchId == null)
                {
                    
                    UpdateText?.Invoke(string.Format("无法获取到批次号！"));
                }
                else
                {
                    dataConnected = true;
                }
                if(batchId ==null)
                {
                    batchId = 100 * (DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day) + 1;
                }
                UpdateText?.Invoke(string.Format("当前批次号:{0}", batchId));
                List<DataCondition> currDayConditions = new List<DataCondition>();
                DataCondition dcc = new DataCondition();
                dcc.Datapoint = new DataPoint("JGD14");
                dcc.strOpt = ">";
                dcc.value = string.Format("{0}", 100 * (batchId / 100));
                currDayConditions.Add(dcc);
                string msg = null;
                if (dataConnected)//如果连接上了数据库
                {
                    string dsName = "JdUnion_Client_Goods_NoXml";
                    DataSource dss = null;
                    if(GlobalShare.mapDataSource.ContainsKey(dsName))
                        dss = GlobalShare.mapDataSource[dsName];
                    if(dss == null)
                    {
                        UpdateText?.Invoke(string.Format("数据源{0}不存在！",dsName));
                        return;
                    }
                    DataSet currDayData = DataSource.InitDataSource(dss, currDayConditions, out msg);
                    if (currDayData == null)
                    {
                        UpdateText?.Invoke(string.Format("获取当日数据失败！"));
                        return;
                    }

                    if (currDayData != null)
                    {
                        for (int i = 0; i < currDayData.Tables[0].Rows.Count; i++)
                        {
                            eliteData tmp = new eliteData();
                            DataRow dr = currDayData.Tables[0].Rows[i];
                            string eli = dr["JGD15"].ToString();
                            tmp.eliteId = int.Parse(eli);

                            tmp.data = new List<DataRow>();
                            tmp.data.Add(dr);
                            new Task(receiveData, tmp).Start();
                        }
                    }

                    //List<int> list = JdUnion_GlbObject.getElites();
                    //Dictionary<string, string> cols = null;
                    allExistKeys = loadAllKeys();
                    if (allExistKeys == null)
                    {
                        allExistKeys = new HashSet<string>();
                    }
                }
                List<int> list = JdUnion_GlbObject.getElites();
                UpdateText?.Invoke(string.Format("当前数据库存在记录数{0}条！", allExistKeys.Count));
                
                int ErrCnt = 0;
                int SaveCnt = 0;
                try
                {
                    foreach (int elit in list)//遍历每个elite
                    {
                        msg = null;
                        bool isExtra = false;
                        List<DataCondition> dics = new List<DataCondition>();
                        DataCondition dc = new DataCondition();
                        DataSource dsrc = GlobalShare.mapDataSource["JdUnion_Goods"];
                        dc.Datapoint = new DataPoint("goodsReq/eliteId");
                        dc.value = elit.ToString();
                        dics.Add(dc);
                        DataSet ds = DataSource.InitDataSource(dsrc,dics, out msg);
                        //DataSet ds = new DataSet();
                        if (msg != null)
                        {
                            UpdateText?.Invoke(string.Format("获取分类数据{0}时出现错误，内容为{1}", elit, msg));
                            continue;
                        }
                        eliteData ed = new eliteData();
                        ed.eliteId = elit;
                        ed.data = new List<DataRow>();

                        List<UpdateData> ups = DataSource.DataSet2UpdateData(ds, "jdUnion_BatchLoad");
                        if(ups == null)
                        {
                            UpdateText?.Invoke(string.Format("{0}类转换错误！",  elit));
                            continue;
                        }
                        UpdateText?.Invoke(string.Format("{1}类总共接收到{0}条记录", ups.Count, Enum.GetName(typeof(goodsElite),elit)));
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
                            ed.data.Add(ds.Tables[0].Rows[i]);
                            ups[i].keydpt = new DataPoint("JGD02");
                            ups[i].keyvalue = key;
                            ups[i].ReqType = DataRequestType.Add;
                            if (ups[i].Items.ContainsKey("JGD14"))
                            {
                                ups[i].Items["JGD14"].value = batchData.keyvalue;
                            }
                            else
                            {
                                ups[i].Items.Add("JGD14", new UpdateItem("JGD14", batchData.keyvalue));
                            }
                            batchData.SubItems.Add(ups[i]);
                            if (i == ups.Count - 1 || batchData.SubItems.Count == batchCnt)//分批保存
                            {
                                bool succ = (SaveClientData == null) ? false : (SaveClientData.Invoke("jdUnion_BatchLoad", batchData, DataRequestType.Add));
                                if (!succ)
                                {
                                    ErrCnt += batchData.SubItems.Count;
                                    UpdateText?.Invoke(string.Format("预期存储条数为{0}条，实际保存条数为{1}条！错误:{2}", batchData.SubItems.Count,0,""));
                                }
                                else
                                {
                                    SaveCnt += batchData.SubItems.Count;
                                    for (int k = 0; k < batchData.SubItems.Count; k++)
                                    {
                                        string skey = batchData.SubItems[k].Items["JGD02"].value;
                                        if (!allExistKeys.Contains(skey))
                                        {
                                            allExistKeys.Add(skey);

                                        }
                                        if (!newAddRec.ContainsKey(skey))
                                        {
                                            newAddRec.Add(skey, ups[k]);
                                        }
                                    }
                                    UpdateText?.Invoke(string.Format("共计条数为{0}条，实际保存条数为{1}条！", ups.Count, batchData.SubItems.Count));
                                }
                                batchData = new UpdateData();
                            }
                        }
                        if (batchData.SubItems.Count > 0)//最后的不能错过。最后一期可能是跳过了的，批处理数量也可能没有达到
                        {
                            bool succ = SaveClientData == null ? false : SaveClientData.Invoke("jdUnion_BatchLoad", batchData, DataRequestType.Add);
                            if (!succ)
                            {
                                //MessageBox.Show(string.Format("商品{0}保存错误！", ups[i].keyvalue));
                                ErrCnt += batchData.SubItems.Count;
                            }
                            else
                            {
                                SaveCnt += batchData.SubItems.Count;
                                for (int k = 0; k < batchData.SubItems.Count; k++)
                                {
                                    string skey = batchData.SubItems[k].Items["JGD02"].value;
                                    if (!allExistKeys.Contains(skey))
                                    {
                                        allExistKeys.Add(skey);

                                    }
                                    if (!newAddRec.ContainsKey(skey))
                                    {
                                        newAddRec.Add(skey, ups[k]);
                                    }
                                }
                                UpdateText?.Invoke(string.Format("共计条数为{0}条，实际保存条数为{1}条！", ups.Count, batchData.SubItems.Count));
                            }
                        }

                        if (ErrCnt > 0)
                        {
                            UpdateText?.Invoke(string.Format("错误条数为{0}条！", ErrCnt));
                        }
                        new Task(receiveData, ed).Start();
                    }

                }
                catch (Exception ce)
                {
                    UpdateText?.Invoke(string.Format("错误条数为{0}条！{1}[{2}]", ErrCnt,ce.Message,ce.StackTrace));

                }
                finally
                {
                    //this.Cursor = Cursors.Default;
                }
                
            }
            catch(Exception ce)
            {
                UpdateText?.Invoke(string.Format("下载数据错误:{0}[{1}]",ce.Message,ce.StackTrace));
            }
            finally
            {
                onSavedData?.Invoke(newAddRec);
            }

        }

        void receiveData(object obj)
        {
            onReceiveData?.Invoke(obj as eliteData);
        }

        
    }

   
}

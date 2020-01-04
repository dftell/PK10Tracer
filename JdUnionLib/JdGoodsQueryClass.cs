using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using ShootSeg;
using WolfInv.Com.AccessWebLib;
using WolfInv.Com.MetaDataCenter;
using WolfInv.Com.WCS_Process;

namespace WolfInv.com.JdUnionLib
{
    public class JdGoodsQueryClass
    {
        public static Dictionary<string, Dictionary<string, string>> glbUrls = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, Dictionary<string, string>> shortLinks = new Dictionary<string, Dictionary<string, string>>();

        public static string NavigateUrl = "http://share.wolfinv.com";
        public static string MyPublic = "关注公众号【武府投资】乐享智购模块查询";
        public static Dictionary<string, JdGoodSummayInfoItemClass> AllcommissionGoods;
        public static Dictionary<string, List<string>> AllKeys;
        public static Action LoadAllcommissionGoods;
        public static bool Inited;
        public static Dictionary<int, EliteDataClass> AllElitesData;
        static Segment seg = null;
        public static Func<List<string>,List<JdGoodSummayInfoItemClass>> LoadPromotionGoodsinfo;


        static JdGoodsQueryClass()
        {
            AllElitesData = new Dictionary<int, EliteDataClass>();
            List<int> list = JdUnion_GlbObject.getElites();
            foreach(int elite in list)
            {
                AllElitesData.Add(elite, new EliteDataClass(elite));
            }
        }

        public static Dictionary<string,JdGoodSummayInfoItemClass> Query(string name,int defaultReturnCnt = 3)
        {
            Dictionary<string, JdGoodSummayInfoItemClass> ret = new Dictionary<string, JdGoodSummayInfoItemClass>();
            if(!Inited)//先判断有没有初始化完成
            {
                LoadAllcommissionGoods?.Invoke();
            }
            if(AllcommissionGoods == null)
            {
                return ret;
            }
            List<string> woldKeys = splitTheWords(name, true);
            int maxWight = 0;
            for(int i=0;i<woldKeys.Count;i++)
            {
                int weight = (int)Math.Pow(2, i);
                if (woldKeys[i].Length == 1)//单字全重为0
                {
                    weight = 1;
                }
                if(woldKeys[i] == "\r\n")
                {
                    weight = 0;
                }
                maxWight += weight;
            }
            Dictionary<string, int> matchWeights = new Dictionary<string, int>();

            foreach(string key in AllKeys.Keys)
            {
                List<string> keys = AllKeys[key];
                int sum = 0;
                for (int k = 0; k < keys.Count; k++)
                {
                    
                    for (int i = 0; i < woldKeys.Count; i++) //关键词匹配次数 权重,前低后高,按2倍增
                    {
                        int weight = (int)Math.Pow(2, i);
                        if (woldKeys[i].Length == 1)//单字全重为0
                        {
                            weight = 1;
                        }
                        if (woldKeys[i] == "\r\n")
                        {
                            weight = 0;
                        }
                        if (keys[k] == woldKeys[i])
                        {
                            sum += weight;
                            break;
                        }
                    }
                }
                matchWeights.Add(key, sum);
            }
            var items = matchWeights.OrderByDescending(a => a.Value);
            if(items.First().Value == maxWight)
            {
                int i = 0;
                foreach (var val in items.Where(a => a.Value == maxWight))
                {
                    JdGoodSummayInfoItemClass item = AllcommissionGoods[val.Key];
                    ret.Add(item.skuId, item);
                    i++;
                    if (i >= defaultReturnCnt)
                        break;
                }
                return ret;
            }
            foreach(var val in items.Where(a=>a.Value>0).Take(defaultReturnCnt))
            {
                JdGoodSummayInfoItemClass item = AllcommissionGoods[val.Key];
                ret.Add(item.skuId, item);
            }
            return ret;
        }

        /// <summary>
        /// 分解句子
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static List<string> splitTheWords(string words, bool noRepeat = false)
        {
            List<string> ret = new List<string>();
            if(seg == null)
            {
                seg = new Segment();
                seg.InitWordDics();
            }
            seg.Separator = "/";
            string retArr = seg.SegmentText(words, true);
            string[] arr = retArr.Split('/');
            if (!noRepeat)
            {
                return arr.ToList();
            }
            for(int i=0;i<arr.Length;i++)
            {
                if(!ret.Contains(arr[i]))
                {
                    ret.Add(arr[i]);
                }
            }
            return ret;
        }

        public static void updateElites(int eliteId,EliteDataClass elData)
        {
            lock(AllElitesData)
            {
                if (AllElitesData.ContainsKey(eliteId))
                {
                    AllElitesData[eliteId] = elData;
                }
                else
                    AllElitesData.Add(eliteId, elData);
            }
        }

        public static void updateAllData(Dictionary<string,JdGoodSummayInfoItemClass> data)
        {
            if(AllcommissionGoods == null)
            {
                AllcommissionGoods = data;
                Inited = true;
                return;
            }
            lock(AllcommissionGoods)
            {
                data.Values.ToList().ForEach(a => {
                    if (!AllcommissionGoods.ContainsKey(a.skuId))
                        AllcommissionGoods.Add(a.skuId, a);
                });
                Inited = true;
            }
        }

        public static Dictionary<string,JdGoodSummayInfoItemClass> QueryWeb(string keyWord,int defaultReturnCnt=10)
        {
            //AccessWebServerClass awc = new AccessWebServerClass();
            Dictionary<string, JdGoodSummayInfoItemClass> ret = new Dictionary<string, JdGoodSummayInfoItemClass>();
            try
            {
                string url = "https://union.jd.com/api/goods/search";
                
                string strJson = JdUnion_GlbObject.getJsonText("jd.union.search.model");
                if (string.IsNullOrEmpty(strJson))
                {
                    return ret;
                }
                string strPostData = strJson.Replace("{0}", keyWord);
                string retJson = AccessWebServerClass.PostData(url, strPostData, Encoding.UTF8);
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                searchReturnData returnResult = javaScriptSerializer.Deserialize<searchReturnData>(retJson);
                if (returnResult.code != 200)
                {
                    return ret;
                }
                List<string> noMatchedList = new List<string>();
                for(int i=0;i<returnResult.data.unionGoods.Count;i++)
                {
                    string id = returnResult.data.unionGoods[i][0].skuId;
                    JdGoodSummayInfoItemClass ji = new JdGoodSummayInfoItemClass();
                    if (AllcommissionGoods == null)
                        AllcommissionGoods = new Dictionary<string, JdGoodSummayInfoItemClass>();
                    if (AllcommissionGoods.ContainsKey(id))
                    {
                        ji = AllcommissionGoods[id];
                        ret.Add(id, ji);
                    }
                    else
                    {
                        if(noMatchedList.Count< defaultReturnCnt)
                            noMatchedList.Add(id);
                    }
                    
                }
                List<JdGoodSummayInfoItemClass> res = null;
                if (LoadPromotionGoodsinfo != null)
                {
                    res = LoadPromotionGoodsinfo(noMatchedList);
                    
                }
                else
                {
                    res = getInfoBySukIds(noMatchedList);
                }
                res.ForEach(a =>
                {
                    if (!ret.ContainsKey(a.skuId))
                    {
                        ret.Add(a.skuId, a);
                    }
                });
                return ret;
            }
            catch(Exception ce)
            {
                
            }
            return ret;
        }

        static List<JdGoodSummayInfoItemClass> getInfoBySukIds(List<string> skids)
        {
            //JdUnion_Goods_PromotionGoodsinfo_Class
            string dsName = "JDUnion_PromotionGoodsinfo";
            List<JdGoodSummayInfoItemClass> ret = new List<JdGoodSummayInfoItemClass>();
            if (skids.Count == 0)
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
            DataSet ds = DataSource.InitDataSource(dsName, dcs, GlobalShare.UserAppInfos.First().Key, out msg, ref isExtra, false);
            if (msg != null)
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
                jsiic.discount = "";
                jsiic.couponLink = "";
                ret.Add(jsiic);
            }
            return ret;
        }



        class searchReturnData
        {
            public int code;
            public string message;
            public searchReturnGoodsList data;
        }

        class searchReturnGoodsList
        {
             public List<List<JdSearchGoods>> unionGoods;
        }

        class JdSearchGoodsList:List<JdSearchGoods>
        {

        }

        class JdSearchGoods
        {
            public string skuId;
            public string skuName;
            public string materialUrl;
        }
    }
    public class eliteData
    {
        public int eliteId;
        public List<DataRow> data;
    }
    public class EliteDataClass
    {
        public int eliteId { get; set; }
        public Dictionary<string,JdGoodSummayInfoItemClass> Data = new Dictionary<string,JdGoodSummayInfoItemClass>();
        public DateTime lastUpdateTime = DateTime.MinValue;
        public EliteDataClass(int e)
        {
            eliteId = e;
        }

    }
}

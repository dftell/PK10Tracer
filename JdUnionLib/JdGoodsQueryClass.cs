using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using ShootSeg;
using WolfInv.Com.AccessWebLib;
using WolfInv.Com.MetaDataCenter;
using WolfInv.Com.WCS_Process;
using XmlProcess;

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

        public static Dictionary<string, JdGoodSummayInfoItemClass> QueryWeb(string keyWord, int defaultReturnCnt = 10)
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
                for (int i = 0; i < returnResult.data.unionGoods.Count; i++)
                {
                    string id = returnResult.data.unionGoods[i][0].skuId;
                    JdSearchGoods jsg = returnResult.data.unionGoods[i][0];
                    JdGoodSummayInfoItemClass ji = new JdGoodSummayInfoItemClass();
                    if (AllcommissionGoods == null)
                        AllcommissionGoods = new Dictionary<string, JdGoodSummayInfoItemClass>();
                    if (AllcommissionGoods.ContainsKey(id))
                    {
                        ji = AllcommissionGoods[id];

                        if (ret.Count > defaultReturnCnt)
                        {
                            break;
                        }
                        ret.Add(id, ji);
                    }
                    else
                    {
                        if (jsg.hasCoupon == 0)
                        {
                            if (noMatchedList.Count < defaultReturnCnt)
                                noMatchedList.Add(id);
                        }
                        else //如果显示有优惠券，直接用搜到的信息
                        {
                            ji = jsg;
                            ret.Add(id, ji);
                        }
                    }

                }
                List<JdGoodSummayInfoItemClass> res = null;
                if (noMatchedList.Count > 0)
                {
                    if (LoadPromotionGoodsinfo != null)
                    {
                        res = LoadPromotionGoodsinfo(noMatchedList);

                    }
                    else
                    {
                        res = getInfoBySukIds(noMatchedList);
                    }
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
            catch (Exception ce)
            {

            }
            return ret;
        }


        public static Dictionary<string,JdGoodSummayInfoItemClass> Query(string name,out string msg,int defaultReturnCnt = 3)
        {
            msg = null;
            Dictionary<string, JdGoodSummayInfoItemClass> ret = new Dictionary<string, JdGoodSummayInfoItemClass>();
            List<DataCondition> dics = new List<DataCondition>();
            DataCondition dc = new DataCondition();
            dc.Datapoint = new DataPoint("goodsReqDTO/keyword"); //JdUnion_Goods  goodsReq/eliteId
            //JDUnion_Goodsinfo goodsReqDTO/keyword
            dc.value = name;
            dics.Add(dc);
            dc = new DataCondition();
            dc.Datapoint = new DataPoint("goodsReqDTO/pageSize"); //JdUnion_Goods  goodsReq/eliteId
            //JDUnion_Goodsinfo goodsReqDTO/keyword
            dc.value = defaultReturnCnt.ToString();
            dics.Add(dc);
            bool isExtra = false;
            string strSrc = "JDUnion_Goodsinfo";
            if (!GlobalShare.mapDataSource.ContainsKey(strSrc))
            {
                msg = "源文件不存在！";
                return ret;
            }
            DataSource dsr = GlobalShare.mapDataSource[strSrc];
            DataSet ds = DataSource.InitDataSource(strSrc, dics, GlobalShare.DefaultUser, out msg, ref isExtra);
            if(ds == null)
            {
                return ret;
            }
            List<UpdateData> ups = DataSource.DataSet2UpdateData(ds, strSrc, GlobalShare.DefaultUser);
            if(ups == null)
            {
                msg = "原始转换为用户数据失败！";
                return ret;
            }
            for(int i=0;i<ups.Count;i++)
            {
                JdGoodSummayInfoItemClass jii = new JdGoodSummayInfoItemClass();
                jii = ups[i].Inject<JdGoodSummayInfoItemClass>(dsr.extradataconvertconfigObject?.Mappings);
                if(!ret.ContainsKey(jii.skuId))
                    ret.Add(jii.skuId, jii);
            }
            return ret;

            
        }

        public static Dictionary<string, JdGoodSummayInfoItemClass> QueryFromLocal(string name,out string msg, int defaultReturnCnt = 3)
        {
            msg = null;
            Dictionary<string, JdGoodSummayInfoItemClass> ret = new Dictionary<string, JdGoodSummayInfoItemClass>();
            try
            {

                if (!Inited)//先判断有没有初始化完成
                {
                    LoadAllcommissionGoods?.Invoke();
                }
                if (AllcommissionGoods == null)
                {
                    msg = "商品信息为空！";
                    return ret;
                }
                List<string> woldKeys = splitTheWords(name, true);
                int maxWight = 0;
                for (int i = 0; i < woldKeys.Count; i++)
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
                    maxWight += weight;
                }
                Dictionary<string, int> matchWeights = new Dictionary<string, int>();
                if(AllKeys == null)
                {
                    AllKeys = new Dictionary<string, List<string>>();
                    AllcommissionGoods.Values.ToList().ForEach(
                        a =>
                        {
                            List<string> keys = JdGoodsQueryClass.splitTheWords(a.skuName, true);
                            if(!AllKeys.ContainsKey(a.skuId))
                                AllKeys.Add(a.skuId, keys);
                        }
                        );
                }
                foreach (string key in AllKeys.Keys)
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
                if(items.Count()==0)
                {
                    return ret;
                }
                if (items.First().Value == maxWight)
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
                foreach (var val in items.Where(a => a.Value > 0).Take(defaultReturnCnt))
                {
                    JdGoodSummayInfoItemClass item = AllcommissionGoods[val.Key];
                    ret.Add(item.skuId, item);
                }
            }
            catch(Exception ce)
            {
                msg = string.Format("{0}:{1}", ce.Message, ce.StackTrace);
                return ret;
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
            retArr = retArr.Replace("\r", "").Replace("\n","");
            string[] arr = retArr.Split('/');
            if (!noRepeat)
            {
                return arr.ToList();
            }
            for(int i=0;i<arr.Length;i++)
            {
                if(string.IsNullOrEmpty(arr[i])||string.IsNullOrWhiteSpace(arr[i]))
                {
                    continue;
                }
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

        
        public static List<JdGoodSummayInfoItemClass> getInfoBySukIds(List<string> skids)
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
                if (dr.Table.Columns.Contains("JGD18"))
                {
                    jsiic.seckillEndTime = dr["JGD18"].ToString();
                    jsiic.seckillOriPrice = dr["JGD19"].ToString();
                    jsiic.seckillPrice = dr["JGD20"].ToString();
                    jsiic.seckillStartTime = dr["JGD21"].ToString();
                }
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
        
        public static string getShortLink(string url)
        {
            string createUrl = "https://dwz.cn/admin/v2/create";
            String SIGN = "7d2772ad2792943aa67ae1213f9288d9";
            string content_type = "application/json";
            string strUrl = "{\"Url\":\"{0}\"}";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(createUrl);
            string ret = null;
            try
            {
                req.Method = "Post";
                //req.Headers.Add("Content-Type", content_type);
                req.ContentType = content_type;
                req.Headers.Add("Token", SIGN);


                string Data = strUrl.Replace("{0}", url);
                byte[] byteArray = Encoding.UTF8.GetBytes(Data);
                Stream newStream = req.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    ret = new StreamReader(wr.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                    wr.Close();
                }
                if (!string.IsNullOrEmpty(ret))
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    shortLinkReturnResult returnResult = javaScriptSerializer.Deserialize<shortLinkReturnResult>(ret);
                    if (returnResult.Code != 0)
                    {
                        return null;
                    }
                    return returnResult.ShortUrl;
                }
                return null;
            }
            catch (Exception ce)
            {

            }
            return "";
        }


        public class JdSearchGoods: JdGoodSummayInfoItemClass
        {
            //public string couponLink;
            public double couponDiscount;
            public double couponQuota;
            public double lowerPrice;
            public double finalPrice;
            public int hasCoupon;
            public double wlPrice;
            public double wlCommission;
            public double wlCommissionRatio;
            //public string skuId;
            //public string skuName;
            //public string materialUrl;
            /*
             "inOrderComm30Days": 110640.69,
          "inOrderCount30Days": 4356,
          "isZY": 1,
          "skuId": 100002033667,
          "skuName": "创维 (SKYWORTH) 450升 冰箱双开门 对开门 双变频节能静音 除菌率＞99.9% 家用风冷无霜 超薄嵌入 W450BP",
          "materialUrl": "jfs/t1/84763/7/19578/202673/5ea14486E73f1c259/825cc78b74cfb406.jpg",
          "shopName": "创维冰箱洗衣机自营旗舰店",
          "shopId": 1000007373,
          "wlPrice": 1899.00,
          "wlCommission": 27.59,
          "wlCommissionRatio": 1.5,
          "isPinGou": 0,
          "hasCoupon": 1,
          "isCommonPlan": 1,
          "planId": 932942496,
          "orientationFlag": 0,
          "isLock": 0,
          "couponLink": "https://coupon.m.jd.com/coupons/show.action?key=cb31546dddad43a5afe780d98ef8e1dc&roleId=29290043",
          "couponDiscount": 60.0,
          "couponQuota": 999.0,
          "couponRemainCnt": 83958,
          "vid": 1000007373,
          "finalPrice": 1839.00,
          "deliveryType": 1,
          "goodCommentsShare": 98,
          "requestId": "s_efbfd9838e1047f9bf9c246f648d7028",
          "abParam": "SVC",
          "abVersion": "SVC",
          "isCare": 0,
          "bonusActivityInfo": {
            "isPreOrRunActivity": 1,
            "bonus": false
          },
          "lowerPrice": 1899.00
             */
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
        public Dictionary<string, JdGoodSummayInfoItemClass> Data = new Dictionary<string, JdGoodSummayInfoItemClass>();
        public DateTime lastUpdateTime = DateTime.MinValue;
        public EliteDataClass(int e)
        {
            eliteId = e;
        }

    }

    public class shortLinkReturnResult
    {
        public int Code;
        public string ShortUrl;
        public string LongUrl;
        public string ErrMsg;
    }
}

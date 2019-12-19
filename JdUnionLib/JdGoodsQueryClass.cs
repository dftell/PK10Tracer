using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShootSeg;

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
                LoadAllcommissionGoods();
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
    }
    public class eliteData
    {
        public int eliteId;
        public DataSet data;
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

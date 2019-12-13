using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShootSeg;
namespace WolfInv.com.JdUnionLib
{
    public class JdGoodsQueryClass
    {
        public static string NavigateUrl = "http://share.wolfinv.com";
        public static string MyPublic = "关注公众号【武府投资】乐享智购模块查询";
        public static Dictionary<string, JdGoodSummayInfoItemClass> AllcommissionGoods;
        public static Dictionary<string, List<string>> AllKeys;
        public static Action LoadAllcommissionGoods;
        public static bool Inited;
        public static Dictionary<string,JdGoodSummayInfoItemClass> Query(string name)
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
                    weight = 0;
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
                            weight = 0;
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

                foreach (var val in items.Where(a => a.Value == maxWight))
                {
                    JdGoodSummayInfoItemClass item = AllcommissionGoods[val.Key];
                    ret.Add(item.skuId, item);
                    return ret;
                }
            }
            foreach(var val in items.Where(a=>a.Value>0).Take(3))
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
        public static List<string> splitTheWords(string words,bool noRepeat = false)
        {
            List<string> ret = new List<string>();
            Segment seg = new Segment();
            seg.InitWordDics();
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
    }

    public class JdGoodSummayInfoItemClass
    {
        public string skuId { get; set; }
        public string skuName { get; set; }
        public string shopName { get; set; }
        public string price { get; set; }
        public string discount { get; set; }

        public string imgageUrl { get; set; }
        public string materialUrl { get; set; }
        public string brandName { get; set; }
        public string commissionUrl { get; set; }
        public string couponLink { get; set; }
        public string getFullContent(bool commissionUrl=false)
        {
            string ret = null;
            if (commissionUrl)
            {
                ret = @"{0}
————————
京东价：￥{1:2f}
内购价：￥{2:2f}
————————
领券+下单：{3}

";
                ret = string.Format(ret, skuName, price, float.Parse(price) - float.Parse(discount), this.commissionUrl.StartsWith("http:") ? "" : "http://" + this.commissionUrl);
            }
            else
            {
                ret = @"{0}
————————
京东价：￥{1:2f}
内购价：￥{2:2f}
————————
优惠券：{3}
商品:{4}

";
                float realprice = float.Parse(price) - float.Parse(discount);
                ret = string.Format(ret, skuName, price, realprice.ToString(), couponLink.StartsWith("http:")?"":"http://"+ couponLink, materialUrl.StartsWith("http:") ? "" : "http://" + materialUrl);
            }
            
            return ret.Replace("http:////","http://").Replace("http://http","http");
        }
    }
}

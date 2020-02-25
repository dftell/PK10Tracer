using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using WolfInv.com.JdUnionLib;

namespace HappyShareLottery
{
    public class wxChartPushClass
    {
        public Action<string> MessageTo;
        public string chartUid { get; set; }
        public string chartName { get; set; }
        public static Dictionary<string, string> sysDefaultCommSetting;
        public Dictionary<string, string> MySetting;
        public static Dictionary<string, string> EliteDic;
        public string subUnionId { get; set; }
        DateTime startTime;
        DateTime endTime;
        int CurrTimes = 0;
        int defaultInterMinutes = 30;//默认间隔为30分钟
        bool needPushNews = true;
        bool needPushKnowedge = true;
        float maxPrice = float.MaxValue;
        public int interMinutes { get; set; }
        bool needPushGeeting = true;
        int picsOnce = 1;
        public bool needSleep = false;//修改时间
        public int needSleepMinuts = 30;//需要调整Timer的间隔
        public Dictionary<string, JdGoodSummayInfoItemClass> CurrDayNeedPushGoods;
        public List<int> pushElites;
        public bool disabled { get; set; }
        public HashSet<string> pushedGoods = new HashSet<string>();
        public Func<string, string> shortUrlFunc;
        public wxChartPushClass()
        {
            startTime = DateTime.MinValue;
            endTime = DateTime.MinValue;
            MySetting = new Dictionary<string, string>();
            MySetting = sysDefaultCommSetting.ToDictionary(a=>a.Key,a=>a.Value);
        }



        public void Init(XmlNode node)
        {
            initEliteList(node.SelectSingleNode("elites"));
            foreach (XmlAttribute att in node.Attributes)
            {

                if (MySetting.ContainsKey(att.Name))
                {
                    MySetting[att.Name] = att.Value;
                }
                else
                {
                    MySetting.Add(att.Name, att.Value);
                }
            }
            if (MySetting.ContainsKey("name"))
            {
                chartName = MySetting["name"];
            }
            subUnionId = "1969838238";
            if (MySetting.ContainsKey("subUnionId"))
            {
                subUnionId = MySetting["subUnionId"];
            }
            if (MySetting.ContainsKey("pushType"))
            {
                string strElites = MySetting["pushType"];
                string[] strArr = strElites.Split('|');
                foreach (string el in strArr)
                {
                    if (EliteDic.ContainsKey(el))
                    {
                        pushElites.Add(int.Parse(EliteDic[el]));
                    }
                }
            }
            if (MySetting.ContainsKey("interMinutes"))//间隔分钟数
            {

                int mins = 0;
                bool succ = int.TryParse(MySetting["interMinutes"], out mins);
                if (succ)
                    defaultInterMinutes = mins;
                interMinutes = defaultInterMinutes;
            }
            if(MySetting.ContainsKey("maxPrice"))
            {
                float maxprice = 0;
                bool succ = float.TryParse(MySetting["maxPrice"], out maxprice);
                if (succ)
                    maxPrice = maxprice;
                if (maxPrice == 0)
                    maxPrice = float.MaxValue;
            }
            if (MySetting.ContainsKey("picsOneTime"))//一次发送条数
            {

                int mins = 0;
                bool succ = int.TryParse(MySetting["picsOneTime"], out mins);
                if (succ)
                    picsOnce = mins;
            }
            if (MySetting.ContainsKey("disabled"))
            {
                if (MySetting["disabled"] == "1")
                {
                    disabled = true;
                }
            }
        }

        public static void initEliteList(XmlNode node)
        {
            if (EliteDic != null)
            {
                return;
            }
            EliteDic = new Dictionary<string, string>();
            foreach (XmlNode enode in node.SelectNodes("elites/elite"))
            {
                //<elite id="1" name="好券商品"/>
                string id = XmlUtilEx.GetSubNodeText(enode, "@id");
                string name = XmlUtilEx.GetSubNodeText(enode, "@name");
                if (!EliteDic.ContainsKey(name))
                    EliteDic.Add(name, id);
            }
        }

        public static void initSysSetting(XmlNode node)
        {
            if (sysDefaultCommSetting != null)
            {
                return;
            }
            sysDefaultCommSetting = new Dictionary<string, string>();
            foreach (XmlNode dnode in node.ChildNodes)
            {
                if (dnode is XmlElement)
                {
                    string ename = dnode.Name;
                    if (sysDefaultCommSetting.ContainsKey(ename))
                        continue;
                    sysDefaultCommSetting.Add(ename, dnode.InnerText);
                }
            }
        }

        bool inPushTime()
        {

            string strStartTime = null;
            if (MySetting.ContainsKey("startTime"))
            {
                strStartTime = MySetting["startTime"];
            }
            if (string.IsNullOrEmpty(strStartTime))
            {
                strStartTime = "8:00";
            }
            startTime = DateTime.Now.Date.Add(Convert.ToDateTime(strStartTime).TimeOfDay);
            string strEndTime = null;
            if (MySetting.ContainsKey("endTime"))
            {
                strEndTime = MySetting["endTime"];
            }
            if (string.IsNullOrEmpty(strEndTime))
            {
                strEndTime = "23:55";
            }
            endTime = DateTime.Now.Date.Add(Convert.ToDateTime(strEndTime).TimeOfDay);
            DateTime CurrTime = DateTime.Now;
            if (CurrTime >= startTime && CurrTime <= endTime)
            {
                needSleep = false;
                return true;
            }
            return false;
        }

        bool willEndThePush()
        {
            if (inPushTime() == false)
                return false;
            if (DateTime.Now.AddMinutes(defaultInterMinutes) > endTime)
                return true;
            return false;
        }


        /// <summary>
        /// 推送消息
        /// </summary>
        public void pushGoods()
        {

            if (!inPushTime())
            {
                CurrTimes = 0;//归0
            }
            if (CurrTimes == 0)//如果是一天的开始
            {
                pushNews();
            }
            CurrTimes++;
            pushGoodsContent(CurrTimes, picsOnce);
            if (willEndThePush())//如果是一天的结尾
            {
                pushGeeting();
                needSleep = true;
                needSleepMinuts = (int)(startTime.AddDays(1).Subtract(DateTime.Now).TotalMinutes);
            }
        }

        public void InitChatGoods()
        {
            CurrDayNeedPushGoods = new Dictionary<string, JdGoodSummayInfoItemClass>();
            int pushTimes = (int)(endTime.Subtract(startTime).TotalMinutes/interMinutes / picsOnce);//总分钟数/每次个数+1
            CurrDayNeedPushGoods = FilterGoods().Take(pushTimes).ToDictionary(a => a.Key, a => a.Value);
        }

        public Dictionary<string, int> getEliteString()
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            return ret;
        }

        Dictionary<string, JdGoodSummayInfoItemClass> FilterGoods()
        {
            Dictionary<string, JdGoodSummayInfoItemClass> ret = new Dictionary<string, JdGoodSummayInfoItemClass>();
            lock (JdGoodsQueryClass.AllElitesData)
            {
                JdGoodsQueryClass.AllElitesData.Values.ToList().ForEach(
                    a => a.Data.Values.ToList().ForEach(
                        b =>
                        {
                            if (!string.IsNullOrEmpty(b.discount))
                            {
                                if (!ret.ContainsKey(b.skuId))
                                    ret.Add(b.skuId, b);
                            }
                        }
                    )
                    );
                return ret;
            }
        }

        /// <summary>
        /// 推送新闻
        /// </summary>
        public void pushNews()
        {
            if (!JdGoodsQueryClass.Inited)
                return;

        }

        /// <summary>
        /// 推送问候语
        /// </summary>
        public void pushGeeting()
        {

        }

        /// <summary>
        /// 推送商品内容
        /// </summary>
        /// <param name="ps"></param>
        public void pushGoodsContent(int times, int ps)
        {
            if (CurrDayNeedPushGoods == null)
            {
                InitChatGoods();
            }
            if (CurrDayNeedPushGoods == null ||CurrDayNeedPushGoods.Count == 0)
            {
                return;
            }
            if(CurrDayNeedPushGoods.Count<((times-1)*ps))
            {
                return;
            }
            int index = (times - 1) * ps;
            int pushcnt = 0;
            var currGoods = CurrDayNeedPushGoods.OrderByDescending(a => a.Value.inOrderCount30Days);
            foreach(var key in currGoods)
            {
                if (pushcnt == picsOnce)//达到次数
                {
                    break;
                }
                JdGoodSummayInfoItemClass jdi = key.Value;
                jdi.shortLinkFunc = getShortLink;
                if (pushedGoods.Contains(jdi.skuId))
                    continue;
                if ((float.Parse(jdi.price)-float.Parse(jdi.discount)) > this.maxPrice)//折后价超出最大价格，跳过
                    continue;
                if (2*(float.Parse(jdi.price) - float.Parse(jdi.discount)) > float.Parse(jdi.price))//折后价超出最大价格，跳过
                    continue;
                Program.plancolls.MsgProcess.SendUrlImgMsg(jdi.imgageUrl,chartUid);
                string fullMsg = string.Format("{0}{1}", jdi.getFullContent(true), jdi.getShortLink(subUnionId));
                Program.plancolls.MsgProcess.SendMsg(fullMsg,chartUid);
                MessageTo(string.Format("{0}[{1}] 发送商品{2}消息。",DateTime.Now.ToLongDateString(),this.chartName,jdi.skuName));
                pushcnt++;
                pushedGoods.Add(jdi.skuId);
            }
        }

        string getShortLink(string strUrl)
        {
            return shortUrlFunc?.Invoke(strUrl);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.WebCommunicateClass;
using WolfInv.com.BaseObjectsLib;
using System.Windows.Forms;
using XmlProcess;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;
using Gecko;

namespace WolfInv.com.WebRuleLib
{
    public class WebConfig
    {
        public float WebWholeOdds = 0;

        public Dictionary<string, LotteryTypes> lotteryTypes;

        public BetUnits Units;

        public WebConfig()
        {
            lotteryTypes = new Dictionary<string, LotteryTypes>();
            Units = new BetUnits();
        }

        public void LoadXml(XmlNode doc)
        {
            WebWholeOdds = float.Parse(XmlUtil.GetSubNodeText(doc,"config/@odds"));
            lotteryTypes = LotteryTypes.getListFromXml(doc.SelectSingleNode("config/Lotteries"));
            Units.LoadXml(doc.SelectSingleNode("config/Units"));
        }
    }

    public class LotteryTypes:List<LotteryBetRuleClass>
    {
        public string Id { get; set; }
        public string ruleId { get; set; }
        public string Name { get; set; }

        Dictionary<string, LotteryBetRuleClass> t_AllRules;
        public Dictionary<string,LotteryBetRuleClass> AllRules
        {
            get
            {
                if(t_AllRules!= null)
                {
                    return t_AllRules;
                }
                t_AllRules = this.ToDictionary(a => a.BetRule, b=>b);
                return t_AllRules;
            }
        }

        public static Dictionary<string, LotteryTypes> getListFromXml(XmlNode node)
        {
            Dictionary<string, LotteryTypes> ret = new Dictionary<string, LotteryTypes>();
            
 
            XmlNodeList nodes = node.SelectNodes("Lottery");
            foreach(XmlNode sn in nodes)
            {
                LotteryTypes lt = new LotteryTypes();
                string strKey = XmlUtil.GetSubNodeText(sn,"@id");
                lt.LoadXml(sn);
                ret.Add(strKey,lt);
            }
            return ret;
        }

        public void LoadXml(XmlNode node)
        {
            Dictionary<string, LotteryTypes> ret = new Dictionary<string, LotteryTypes>();
            Id = XmlUtil.GetSubNodeText(node, "@id");
            ruleId = XmlUtil.GetSubNodeText(node, "@ruleId");
            Name = XmlUtil.GetSubNodeText(node, "@name");
            XmlNodeList nodes = node.SelectNodes("BetTypes/BetType");
            foreach (XmlNode sn in nodes)
            {
                LotteryBetRuleClass lbr = new LotteryBetRuleClass();
                lbr.LoadXml(sn);
                this.Add(lbr);
            }
        }
    }
    /*
         "cqc": 1,
    "tjc": 2,
    "jlk3": 3,
    "jsk3": 4,
    "pl3": 5,
    "fc3d": 6,
    "klpk": 7,
    "pk10": 8,
    "wfc": 9,
    "sd11x5": 10,
    "ggc": 11,
    "kck3": 12,
    "kcjs3d": 13,
    "xjssc": 14,
    "gxk3": 15,
    "ahk3": 16,
    "hubk3": 17,
    "hebk3": 18,
    "shk3": 19,
    "yfc": 20,
    "gd11x5": 21,
    "jx11x5": 22,
    "sh11x5": 23,
    "hlj11x5": 24,
    "bjk3": 27,
    "js11x5": 28,
    "hgffc": 29,
    "heb11x5": 30,
    "yfcnew": 31,
    "yfk3": 32,
    "txffc": 33,
    "xyft": 34,
    "paoma": 35,
    "btbffc": 36
         */
    public class LotteryBetRuleClass
    {
        public string BetRule { get; set; }
        public string BetType { get; set; }
        public string BetRuleName { get; set; }
        
        public void LoadXml(XmlNode node)
        {
            //<BetType bettype="801" betrule="8010101">猜冠军</BetType>
            BetRule = XmlUtil.GetSubNodeText(node, "@betrule");
            BetType = XmlUtil.GetSubNodeText(node, "@bettype");
            BetRuleName = XmlUtil.GetSubNodeText(node, ".");
        }
    }

    public class BetUnits:List<BetUnit>
    {
        public BetUnit DefaultUnit
        {
            get
            {
                if (this.Count > 0)
                    return this[0];
                return null;
            }
        }

        public void LoadXml(XmlNode node)
        {
            int cnt = int.Parse(XmlUtil.GetSubNodeText(node, "@basecnt"));
            float level = float.Parse(XmlUtil.GetSubNodeText(node, "@baselevel"));
            XmlNodeList nodes = node.SelectNodes("Node");
            foreach(XmlNode sn in nodes)
            {
                BetUnit bu = new BetUnit();
                bu.baseCnt = cnt;
                bu.baseLevel = level;
                bu.LoadXml(sn);
                this.Add(bu);
            }
        }
    }
    public class BetUnit
    {
        protected double baseval = 0.1;
        public string id { get; set; }
        public string Name { get; set; }
        public int Power { get; set; } //0.1的次方，分为2
        public int baseCnt { get; set; }//基本数
        public Single baseLevel { get; set; }
        public BetUnit() //默认0.02
        {
            id = "cent";
            Name = "分";
            Power = 2;
            baseCnt = 2;
            baseLevel = 0.1F;
        }

        public double getOneUnitValue()
        {
            return baseCnt * Math.Pow(baseLevel , Power);
        }
        public void LoadXml(XmlNode node)
        {
            id = XmlUtil.GetSubNodeText(node, "@id");
            Name = XmlUtil.GetSubNodeText(node, "@name");
            Power = int.Parse(XmlUtil.GetSubNodeText(node, "@power"));
        }
    }

    public abstract class WebRule : ILotteryRule
    {
        //public abstract string IntsListToJsonString(List<InstClass> Insts);
        public virtual string IntsToJsonString(string LotteryName,String ccs, int unit)
        {
            return ccs;
        }
        public GlobalClass GobalSetting;
        public WebConfig config;
        protected WebRule(GlobalClass setting)
        {
            GobalSetting = setting;
        }


        public void Load(string filename,string foldername)
        {
            config = new WebConfig();
            string xmltext = GlobalClass.LoadTextFile(filename, foldername);
            if (xmltext != null)
            {
                XmlDocument xmldoc = new XmlDocument();
                try
                {
                    xmldoc.LoadXml(xmltext);
                    config.LoadXml(xmldoc);
                }
                catch (Exception ce)
                {
                    return;
                }

            }
        }










        #region 获取或判断对象值
        public  bool IsLoadCompleted(HtmlDocument indoc)
        {
            string strNotice = GobalSetting.HostKey;

            return WebRule.existElement(indoc, GobalSetting.HostKey);

        }

        public  bool IsVaildWeb(GeckoDocument doc)
        {
            return true;
        }

        public bool IsLogined(GeckoDocument doc)
        {
            return WebRule.existElement(doc, GobalSetting.LoginedFlag);
            return doc?.GetElementById(GobalSetting.LoginedFlag) != null;
        }

        public bool IsLoadCompleted(GeckoDocument indoc)
        {
            string strNotice = GobalSetting.HostKey;

            return WebRule.existElement(indoc, GobalSetting.HostKey);


            if (indoc.Head.OuterHtml.IndexOf(strNotice) > 0)
            {
                return true;
            }
            return false;
        }

        public bool IsLogined(HtmlDocument doc)
        {
            return WebRule.existElement(doc, GobalSetting.LoginedFlag);
            return true;
        }

        public double GetCurrMoney(HtmlDocument doc)
        {
            HtmlElement ElPoint = doc?.GetElementById(GobalSetting.AmountId);
            double ret = 0;
            if (ElPoint != null)
            {
                double.TryParse(ElPoint?.InnerText, out ret);
            }
            return ret;
        }

        public double GetCurrMoney(GeckoDocument doc)
        {
            string outval = "";
            bool succ = WebRule.existElement(doc, GobalSetting.AmountId, out outval);
            if (succ)
            {
                double outRes = 0.00;
                if (double.TryParse(outval, out outRes))
                {
                    return outRes;
                }
                return 0;
            }
            return 0;
            GeckoElement ElPoint = doc?.GetElementById(GobalSetting.AmountId);
            double ret = 0;
            if (ElPoint != null)
            {
                double.TryParse(ElPoint?.TextContent, out ret);
            }
            return ret;
        }


        public bool IsVaildWeb(HtmlDocument doc)
        {
            return true;
        }

        #endregion



        protected abstract Dictionary<string, int> GetChanlesInfo(string url);

        public string GetChanle(string url,string CurrChanle,bool ForceGetFastChanle=false)
        {
            Dictionary<string, int> chls = GetChanlesInfo(url);//获得所有线路的信息
            string ret = CurrChanle;
            if (!chls.ContainsKey(CurrChanle))//如果通道已经不包括当前通道了，获取最新通道
            {
                ret = GetFastChanle(chls);
            }
            else//当前通道仍在通道清单中
            {
                if (chls.Where(a => a.Value == 0).Select(a => a.Key).ToList().Contains(CurrChanle))//如果当前通道已经无法访问，切换为最快的线路！
                {
                    ret = GetFastChanle(chls);
                }
                else
                {
                    if (ForceGetFastChanle)//强制获取最快线路
                    {
                        ret = GetFastChanle(chls);
                    }
                    else
                    {
                        if (chls.Where(a => a.Value > 0).OrderBy(a => a.Value).First().Key.Equals(CurrChanle))//如果能访问的通道中当前通道排名最后，切换为最快线路
                        {
                            ret = GetFastChanle(chls);
                        }
                    }
                }
            }
            return ret ?? CurrChanle;//如果未空使用当前通道
        }

        string GetFastChanle(Dictionary<string, int> Chanles)
        {
            if (Chanles == null || Chanles.Count == 0)
                return null;
            int val = Chanles.Where(a => a.Value > 0).Max(a => a.Value);//获取最大网速值
            if (val <= 0)//小于等于0返回空
                return null;
            return Chanles.Where(a => a.Value == val)?.First().Key;//返回等于最大网速值得那个通道编号
        }

        public abstract object getVerCodeImage(HtmlDocument doc);

        public abstract string getRealUrl(string html);

        public abstract string getChargeQCode(HtmlDocument doc);

        public abstract string getChargeNum(HtmlDocument doc);
        public abstract string getChargeAmt(HtmlDocument doc);

        public abstract string getErr_Msg(HtmlDocument doc);

        public static bool existElement(object doc, string tag)
        {
            string outval = null;
            return existElement(doc, tag,out outval);
        }

        public static bool existElement(object doc,string tag,out string outVal)
        {
            bool isIE = doc is HtmlDocument;
            string[] arr = tag.Split('|'); //p|class|lkjlfd 
            string tagName = arr[0];//tag p ，a，table ，td，div
            string tagKeyType = "";//class ,id,name
            string tagKeyValue = "";//
            if (arr.Length > 1)
            {
                tagKeyType = arr[1];
            }
            if (arr.Length > 2)
            {
                tagKeyValue = tag.Substring(tagName.Length + 1 + tagKeyType.Length + 1);//除去 tag 和 约束[id|name]的值
            }
            if (isIE)
            {
                HtmlElementCollection hec = (doc as HtmlDocument).GetElementsByTagName(tagName);
                foreach (HtmlElement he in hec)
                {
                    string idval = null;
                    idval = he.GetAttribute(tagKeyType.Trim());
                    if (string.IsNullOrEmpty(idval)) //标签都没有
                        continue;
                    if (idval?.Trim() == tagKeyValue)//如果匹配上了
                    {
                        outVal = he.InnerText;
                        return true;
                    }
                }
                outVal = null;
                return false;
            }
            else
            {
                GeckoElementCollection hec = (doc as GeckoDocument).GetElementsByTagName(tagName);
                foreach (GeckoElement he in hec)
                {
                    string idval = null;
                    idval = he.GetAttribute(tagKeyType.Trim());
                    if (string.IsNullOrEmpty(idval)) //标签都没有
                        continue;
                    if (idval?.Trim() == tagKeyValue)//如果匹配上了
                    {
                        outVal = he.TextContent;
                        return true;
                    }
                }
                outVal = null;
                return false;
            }
        }
    }



    public class WebRuleBuilder
    {
        public static WebRule Create(GlobalClass gc)
        {
            if(gc.ForWeb.ToLower().IndexOf("kcai")>=0)
            {
                return new Rule_ForKcaiCom(gc);
            }
            ////else if()
            ////{

            ////}
            return new JsProcessRuleClass(gc);
        }

        
    }
}

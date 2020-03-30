using System.Collections.Generic;
using System.Linq;
using XmlProcess;
using System.Xml;

namespace WolfInv.com.WebRuleLib
{
    public class LotteryTypes:List<LotteryBetRuleClass>
    {
        public string Id { get; set; }
        public string ruleId { get; set; }
        public string Name { get; set; }

        public int elementCount { get; set; }
        public int seletElementCnt { get; set; }
        public int baseTimes { get; set; }

        public int calcTimes { get; set; }

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
            elementCount = 10;
            baseTimes = 2;
            seletElementCnt = 10;
            string strElementCnt = XmlUtil.GetSubNodeText(node, "@elementCount");
            string strSeletElementCnt = XmlUtil.GetSubNodeText(node, "@seletElementCnt");
            string strBaseTimes = XmlUtil.GetSubNodeText(node, "@baseTimes");
            string strCalcTimes = XmlUtil.GetSubNodeText(node, "@calcTimes");
            if(!string.IsNullOrEmpty(strElementCnt))
            {
                elementCount = int.Parse(strElementCnt);
            }
            if(!string.IsNullOrEmpty(strSeletElementCnt))
            {
                seletElementCnt = int.Parse(strSeletElementCnt);
            }
            if(!string.IsNullOrEmpty(strBaseTimes))
            {
                baseTimes = int.Parse(strBaseTimes);
            }
            if(!string.IsNullOrEmpty(strCalcTimes))
            {
                calcTimes = int.Parse(strCalcTimes);
            }
            XmlNodeList nodes = node.SelectNodes("BetTypes/BetType");
            foreach (XmlNode sn in nodes)
            {
                LotteryBetRuleClass lbr = new LotteryBetRuleClass();
                lbr.LoadXml(sn);
                this.Add(lbr);
            }
        }
    }
}

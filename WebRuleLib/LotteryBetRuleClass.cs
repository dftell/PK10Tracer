using XmlProcess;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
namespace WolfInv.com.WebRuleLib
{
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

        public string instType { get; set; }

        public double oddsTimes { get; set; }
        public Dictionary<string, string> OddsDic;
        
        public void LoadXml(XmlNode node)
        {
            //<BetType bettype="801" betrule="8010101">猜冠军</BetType>
            BetRule = XmlUtil.GetSubNodeText(node, "@betrule");
            BetType = XmlUtil.GetSubNodeText(node, "@bettype");
            instType = XmlUtil.GetSubNodeText(node, "@instType");
            double fOddsTimes = 1;
            oddsTimes = 1;
            if (double.TryParse(XmlUtil.GetSubNodeText(node,"@oddsTimes"),out fOddsTimes))
            {
                oddsTimes = fOddsTimes;
            }
            XmlNodeList nodes = node.SelectNodes("./Odds");
            OddsDic = new Dictionary<string, string>();
            foreach(XmlNode onode  in nodes)
            {
                string odd = XmlUtil.GetSubNodeText(onode, "@base");
                string val = XmlUtil.GetSubNodeText(onode, "@value");
                if(!OddsDic.ContainsKey(odd))
                {
                    OddsDic.Add(odd, val);
                }
            }
            BetRuleName = XmlUtil.GetSubNodeText(node, ".");
        }
    }
}

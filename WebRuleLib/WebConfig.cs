using System.Collections.Generic;
using XmlProcess;
using System.Xml;

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
}

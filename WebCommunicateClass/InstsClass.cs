using System;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
namespace WolfInv.com.WebCommunicateClass
{

    public class InstClass
    {
        public String ruleId;
        public String betNum;//'车数*前后区间所有车号数量
        public String itemTimes;//
        public String selNums;//具体下注码
        public String jsOdds;//赔率
        public int priceMode;//价格模式

        public String GetJson()
        {
            String outStr = "";
            try
            {
                if (outStr.Length == 0)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    return js.Serialize(this);
                    ////JSONObject js = new JSONObject();

                    ////js.put("ruleId", ruleId);
                    ////js.put("betNum", betNum);
                    ////js.put("itemTimes", itemTimes);
                    ////js.put("selNums", selNums);
                    ////js.put("jsOdds", jsOdds);
                    ////js.put("priceMode", priceMode);
                    ////outStr = js.toString();
                }
            }
            catch (Exception ce)
            {
                return "";
            }
            return outStr;
        }
    }
}

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
        public String itemTimes;//单项金额
        public String itemUnitTimes;//单项倍数 add in 2020/5/30
        public String selNums;//具体下注码 
        public string fullSelNums;//完整的下注码，对不需要分页的有效 add in 2020/5/30
        public String jsOdds;//赔率
        public int priceMode;//价格模式
        public double Unit=0.02;
        public Func<InstClass,string> ToWebJson;
        public String GetJson(bool dirctToJson = false)
        {
            if(dirctToJson == false && ToWebJson != null)
            {
                return ToWebJson(this);
            }
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

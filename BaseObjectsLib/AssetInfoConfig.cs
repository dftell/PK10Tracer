using System.Collections.Generic;
using System.Linq;
namespace WolfInv.com.BaseObjectsLib
{
    public class AssetInfoConfig:DetailStringClass
    {
        public int value = 0; //当前金额
        public int NeedSelectTimes = 0;//需要择时
        //public int MaxTracingTimes = 0;//最大追踪次数
        public double maxStopGainedValue = 50000;//止盈金额
        public AssetInfoConfig()
        {

        }

        public AssetInfoConfig(int val)
        {
            value = val;
        }

        public AssetInfoConfig(string val)
        {
            int.TryParse(val, out value);
        }
        public AssetInfoConfig(Dictionary<string,string> vals)
        {
            loadFromStringDic(vals);
        }
        void loadFromStringDic(Dictionary<string,string> vals)
        {
            if(vals.ContainsKey("value"))
            {
                int.TryParse(vals["value"], out value);
            }
            else
            {
                if(vals.Count == 1)
                {
                    int.TryParse(vals.Keys.First(), out value);
                }
            }
            if (vals.ContainsKey("NeedSelectTimes"))
            {
                int.TryParse(vals["NeedSelectTimes"], out NeedSelectTimes);
            }
            if (vals.ContainsKey("maxStopGainedValue"))
            {
                double.TryParse(vals["maxStopGainedValue"], out maxStopGainedValue);
            }
        }

        public Dictionary<string,string> getStringDic()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            ret.Add("value",value.ToString());
            ret.Add("NeedSelectTimes", NeedSelectTimes.ToString());
            ret.Add("maxStopGainedValue", maxStopGainedValue.ToString());
            return ret;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
namespace WolfInv.com.BaseObjectsLib
{
    public class AssetInfoConfig:DetailStringClass
    {
        public int value = 0; //当前金额
        public int NeedSelectTimes = 0;//需要择时
        public int SelectFuncId = 0;//0 二项分布择时， 1 凌波微步择时
        public int CurrTimes = 0;
        //public int MaxTracingTimes = 0;//最大追踪次数
        public int NeedStopGained = 0;//需要止盈
        public int AutoResumeDefaultReturnValue = 0;//自动恢复默认值
        public int ZeroCloseResume = 0;//值归零后自动把自动回复默认值功能关闭，一直要到触发条件后才打开
        public int AutoTraceMinChips = 40;//大于此值后客户端将AutoResumeDefaultReturnValue设为1
        #region 制动参数
        /*
        紧急制动 
        紧急制动生效后
        1、有仓命中后直接停止，忽略自动恢复
        2、无论持仓无仓均将返回值设为0，
        3、无仓高位点火后紧急制动自动取消
           */
        public int AutoEmergencyStop = 0;//自动启动紧急制动，设为1后，算法计算出需要自动
        public int EmergencyStop=0;     //自动刹车标志
        public int StopIgnoreLength = 10;//刹车忽略长度
        public int StopStepLen = 4;//刹车降速步长
        public int StopPower = 2;//刹车力度，0为立即刹车，其他为降速倒数
        #endregion

        public int DefaultReturnTimes = 0;//触发条件后返回倍数，传入此参数，如果设置为>0则返回该值，如果为0则返回1
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
            if(vals.ContainsKey("SelectFuncId"))
            {
                int.TryParse(vals["SelectFuncId"], out SelectFuncId);
            }
            if (vals.ContainsKey("maxStopGainedValue"))
            {
                double.TryParse(vals["maxStopGainedValue"], out maxStopGainedValue);
            }
            if(vals.ContainsKey("AutoResumeDefaultReturnValue"))
            {
                int.TryParse(vals["AutoResumeDefaultReturnValue"], out AutoResumeDefaultReturnValue);
            }
            if (vals.ContainsKey("ZeroCloseResume"))
            {
                int.TryParse(vals["ZeroCloseResume"], out ZeroCloseResume);
            }
            if (vals.ContainsKey("NeedStopGained"))
            {
                int.TryParse(vals["NeedStopGained"], out NeedStopGained);
            }
            if(vals.ContainsKey("currTimes"))
            {
                int.TryParse(vals["currTimes"], out CurrTimes);
            }
            if(vals.ContainsKey("DefaultReturnTimes"))
            {
                int.TryParse(vals["DefaultReturnTimes"], out DefaultReturnTimes);
            }
            if(vals.ContainsKey("EmergencyStop"))
            {
                int.TryParse(vals["EmergencyStop"], out EmergencyStop);
            }
            if(vals.ContainsKey("AutoEmergencyStop"))
            {
                int.TryParse(vals["AutoEmergencyStop"], out AutoEmergencyStop);
            }
            if(vals.ContainsKey("AutoTraceMinChips"))
            {
                int.TryParse(vals["AutoTraceMinChips"], out AutoTraceMinChips);
            }
        }

        public Dictionary<string,string> getStringDic()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            ret.Add("value",value.ToString());
            ret.Add("NeedSelectTimes", NeedSelectTimes.ToString());
            ret.Add("SelectFuncId", SelectFuncId.ToString());
            ret.Add("NeedStopGained", NeedStopGained.ToString());
            ret.Add("maxStopGainedValue", maxStopGainedValue.ToString());
            ret.Add("DefaultReturnTimes", DefaultReturnTimes.ToString());
            ret.Add("AutoResumeDefaultReturnValue", AutoResumeDefaultReturnValue.ToString());
            ret.Add("ZeroCloseResume", ZeroCloseResume.ToString());
            ret.Add("EmergencyStop", EmergencyStop.ToString());
            ret.Add("AutoEmergencyStop", AutoEmergencyStop.ToString());
            ret.Add("AutoTraceMinChips", AutoTraceMinChips.ToString());
            ret.Add("currTimes", CurrTimes.ToString());
            return ret;
        }
    }
}

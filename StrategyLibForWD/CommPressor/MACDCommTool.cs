using System;
//using MathNet.Numerics;
//using MathNet.Numerics.Statistics;
using WolfInv.com.GuideLib;
namespace WolfInv.com.StrategyLibForWD
{
    public class MACDCommTool
    {
        public static FilterResult IsFirstBuyPoint(MACDDataItem secinfo, Delegate CallBackFunc)
        {
            FilterResult ret = new FilterResult();
            if (secinfo.DEA >= 0)
            {
                ret.Msg.Append("第一买点，DEA必须在0线以下！");
                return ret;
            }
            if (secinfo.pct_chg  > 10)
            {
                ret.Msg.Append("涨幅太大！");
                return ret;
            }
            
            return ret;
        }
    }
}

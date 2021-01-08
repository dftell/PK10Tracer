using System;
using WolfInv.com.ProbMathLib;
namespace WolfInv.com.BaseObjectsLib
{
    public class NolimitTraceChance<T> : TraceChance<T> where T : TimeSerialData
    {
        public override bool CheckNeedEndTheChance(ChanceClass<T> cc, bool LastExpectMatched,bool review=false)
        {
            throw new NotImplementedException();
        }

        public override double getChipAmount(double RestCash, ChanceClass<T> cc, AmoutSerials amts)
        {
            try
            {
                //////if (amts.Serials[cc.ChipCount - 1].Length == 0)
                //////{
                //////    Log("获取单码金额", "队列长度为0");
                //////}
                //////else
                //////{
                //////    Log(string.Format("获取到{0}码金额:{1}",cc.ChipCount,cc.HoldTimeCnt),string.Join(",",amts.Serials[cc.ChipCount-1]));
                //////}
                
                
                if (cc.IncrementType == InterestType.CompoundInterest)
                {
                    double rate = KellyMethodClass.KellyFormula(cc.ChipCount,10,9.75,1.01);
                    long ret = (long)Math.Floor((double)(RestCash * rate ));
                    return ret;
                }
                if(cc.AllowMaxHoldTimeCnt > 0 && cc.HoldTimeCnt > cc.AllowMaxHoldTimeCnt)
                {
                    return 0;
                }
                int chips = cc.ChipCount - 1;
                int maxcnt = amts.MaxHoldCnts[chips];
                int bShift = 0;
                if (cc.HoldTimeCnt > maxcnt)
                {
                    Log("风险", "达到最大上限", string.Format("机会{0}持有次数达到{1}次总投入金额已为{2}",cc.ChanceCode,cc.HoldTimeCnt,Cost));
                    bShift = (int)maxcnt * 2 / 3;
                }
                int RCnt = (cc.HoldTimeCnt % (maxcnt+1)) + bShift-1;
                return amts.Serials[chips][RCnt];
            }
            catch (Exception e)
            {
                Log("错误",string.Format("TraceChance,获取单码金额错误:{0}", e.Message),e.StackTrace);
            }
            return 1;
        }
    }


    
}

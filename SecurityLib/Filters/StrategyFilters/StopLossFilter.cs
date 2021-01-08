using System;
using System.Linq;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    public class StopLossFilter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public StopLossFilter(string endExpect, CommSecurityProcessClass<T> cpc, PriceAdj priceAdj = PriceAdj.Fore, Cycle cyc = Cycle.Day) : base(endExpect, cpc, priceAdj, cyc)
        {

        }
        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override SelectResult ExecFilter(CommStrategyInClass Input)
        {
           
            KLineData<T> klineData = kLineData;// new KLineData<T>(EndExpect,this.SecObj.SecPriceInfo);
            DateTime[] dates = klineData.Expects.Select(a=>a.ToDate()).ToArray();
            SelectResult ret = new SelectResult();
            CommSecurityProcessClass<T> cpc = SecObj;
            ret.Enable = false;
            int startDate = dates.IndexOf(Input.StartDate);
            if (startDate < 0)
                return ret;
            
            double signPrice = klineData.Closes[startDate];
            double lastPrice = klineData.Closes.Last();      
            double rate = 100 * (lastPrice - signPrice) / signPrice;
            if (rate < -15.0 || rate > 200)//10%止损，100%止盈
            {
                ret.Enable = true;
                ret.Status = rate < -15 ? "止损" : "止盈";
                return ret;
            }
            //买入大跌逻辑止损逻辑


            return ret;
        }
    }

}

using System;
using System.Linq;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    public class StopLossFilter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public StopLossFilter(string endExpect, CommSecurityProcessClass<T> cpc, PriceAdj priceAdj = PriceAdj.Beyond, Cycle cyc = Cycle.Day) : base(endExpect, cpc, priceAdj, cyc)
        {

        }
        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override SelectResult ExecFilter(CommStrategyInClass Input)
        {
            kLineData.getSingleData = this.getSingleData;
            KLineData<T> klineData = kLineData;// new KLineData<T>(EndExpect,this.SecObj.SecPriceInfo);
            DateTime[] dates = klineData.Expects.Select(a=>a.ToDate()).ToArray();
            SelectResult ret = new SelectResult();
            CommSecurityProcessClass<T> cpc = SecObj;
            ret.Enable = false;
            int si = 0;
            int startDate = klineData.ExpectIndex(Input.SignDate,out si);
            double lastPrice = klineData.Closes.Last();      
            double slprice = 0;
            if(!string.IsNullOrEmpty(Input.StopPriceDate))
            {
                int slDate = dates.IndexOf(Input.StopPriceDate);
                if (slDate>=0)
                {
                    slprice = klineData.Lows[slDate];
                }
            }
            if(Input.useStopLossPrice && lastPrice < slprice)
            {
                ret.Enable = true;
                ret.Status = string.Format("止损价:{0}",Input.StopPrice);
                return ret;
            }
            //if (rate < -15.0 || rate > 200)//10%止损，100%止盈
            //{
            //    ret.Enable = true;
            //    ret.Status = rate < -15 ? "止损" : "止盈";
            //    return ret;
            //}
            //买入大跌逻辑止损逻辑


            return ret;
        }
    }

}

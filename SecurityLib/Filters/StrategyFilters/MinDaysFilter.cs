using System;
using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    /// <summary>
    /// 最小长度滤网
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MinDaysFilter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public MinDaysFilter(string endExpect, CommSecurityProcessClass<T> cpc, PriceAdj priceAdj = PriceAdj.Beyond, Cycle cyc = Cycle.Day) : base(endExpect, cpc,priceAdj,cyc)
        {

        }
        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override SelectResult ExecFilter(CommStrategyInClass Input)
        {
            //List<T> list = this.SecObj.SecPriceInfo;
            //var klineData = new KLineData<T>(EndExpect,this.SecObj.SecPriceInfo);
            ///ToDo:反转选股
            SelectResult ret = new SelectResult();
            //CommSecurityProcessClass<T> ret = this.SecObj;
            ret.Enable = false;
            if(kLineData.Length < Input.MinDays)
            {
                ret.Enable = false;
                return ret;
            }
            ret.Enable = true;
            return ret;
        }
    }
}

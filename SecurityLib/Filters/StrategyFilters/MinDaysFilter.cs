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
        public MinDaysFilter(CommSecurityProcessClass<T> cpc) : base(cpc)
        {

        }
        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass<T> ExecFilter(CommStrategyInClass Input)
        {
            List<T> list = this.SecObj.SecPriceInfo;
            var klineData = new KLineData<T>(list);
            ///ToDo:反转选股
            CommSecurityProcessClass<T> ret = this.SecObj;
            ret.Enable = false;
            if(klineData.Length < Input.MinDays)
            {
                ret.Enable = false;
                return ret;
            }
            ret.Enable = true;
            return ret;
        }
    }
}

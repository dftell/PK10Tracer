using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    public class IsSTFilter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public IsSTFilter(string expect, CommSecurityProcessClass<T> secinfo, PriceAdj priceAdj = PriceAdj.Fore, Cycle cyc = Cycle.Day) : base(expect, secinfo, priceAdj, cyc)
        {

        }

        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override SelectResult ExecFilter(CommStrategyInClass Input)
        {
            SelectResult ret = new SelectResult();
            if(SecObj.StockInfo==null)
            {
                ret.Enable = false;
                return ret;
            }
            if(SecObj.StockInfo.KeyName.Contains("ST"))
            {
                ret.Enable = false;
                return ret;
            }
            ret.Enable = true;
            return ret;
        }
    }
}

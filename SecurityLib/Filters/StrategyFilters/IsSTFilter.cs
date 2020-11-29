using System;
using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    public class IsSTFilter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public IsSTFilter(CommSecurityProcessClass<T> cpc) : base(cpc)
        {

        }

        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass<T> ExecFilter(CommStrategyInClass Input)
        {
            if(SecObj.StockInfo==null)
            {
                SecObj.Enable = false;
                return SecObj;
            }
            if(SecObj.StockInfo.KeyName.Contains("ST"))
            {
                SecObj.Enable = false;
                return SecObj;
            }
            SecObj.Enable = true;
            return SecObj;
        }
    }
}

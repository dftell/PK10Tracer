using System;
using System.Linq;
using System.Text;
using System.Data;
using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    /// <summary>
    /// 申万二级行业选股
    /// </summary>
    public class CommSI2IndustryStrategy<T> : CommIndustryStrategy<T> where T:TimeSerialData
    {
        public CommSI2IndustryStrategy(CommDataIntface<T> _w) : base(_w) { }
        public override CommSecurityProcessClass<T> ReverseSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass<T> BreachSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass<T> BalanceSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }





        public override BaseDataTable ReadSecuritySerialData(string Code)
        {
            throw new NotImplementedException();
        }
    }
}

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
    public class CommSI2IndustryStrategy:CommIndustryStrategy
    {
        public CommSI2IndustryStrategy(CommDataIntface _w) : base(_w) { }
        public override CommSecurityProcessClass ReverseSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass BreachSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass BalanceSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }





        public override BaseDataTable ReadSecuritySerialData(string Code)
        {
            throw new NotImplementedException();
        }
    }
}

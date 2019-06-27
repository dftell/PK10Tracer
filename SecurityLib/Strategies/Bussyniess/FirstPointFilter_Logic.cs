using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib.Strategies.Bussyniess
{
    public class FirstPointFilter_Logic_Strategy : CommStrategyBaseClass
    {
        public FirstPointFilter_Logic_Strategy(CommDataIntface _w) : base(_w)
        {
        }

        public override CommSecurityProcessClass BalanceSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass BreachSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override BaseDataTable ReadSecuritySerialData(string Code)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass ReverseSelectSecurity(CommStrategyInClass Input)
        {
            ///ToDo:反转选股
            CommSecurityProcessClass ret = new CommSecurityProcessClass();
            //Input.SecsPool;
            return ret;
        }
    }
}

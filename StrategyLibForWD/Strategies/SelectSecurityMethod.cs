using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfInv.com.StrategyLibForWD
{
    public interface iSelectSecurityMethod
    {
    }

    public interface iReverseMethod
    {
        /// <summary>
        /// 反转选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        SecurityProcessClass ReverseSelectSecurity(StrategyInClass Input);
    }

    public interface iBreachMethod
    {
        /// <summary>
        /// 突破选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        SecurityProcessClass BreachSelectSecurity(StrategyInClass Input);
    }

    public interface iBalanceMethod
    {
        /// <summary>
        /// 反转突破结合选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        SecurityProcessClass BalanceSelectSecurity(StrategyInClass Input);
    }
}

namespace WolfInv.com.SecurityLib
{
    public interface iCommBalanceMethod
    {
        /// <summary>
        /// 反转突破结合选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        CommSecurityProcessClass BalanceSelectSecurity(CommStrategyInClass Input);
    }
}

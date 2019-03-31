namespace WolfInv.com.SecurityLib
{
    public interface iCommBreachMethod
    {
        /// <summary>
        /// 突破选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        CommSecurityProcessClass BreachSelectSecurity(CommStrategyInClass Input);
    }
}

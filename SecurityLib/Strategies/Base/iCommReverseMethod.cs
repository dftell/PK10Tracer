namespace WolfInv.com.SecurityLib
{
    public interface iCommReverseMethod
    {
        /// <summary>
        /// 反转选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        CommSecurityProcessClass ReverseSelectSecurity(CommStrategyInClass Input);
    }
}

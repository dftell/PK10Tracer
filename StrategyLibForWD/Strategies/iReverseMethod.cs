namespace WolfInv.com.StrategyLibForWD
{
    public interface iReverseMethod
    {
        /// <summary>
        /// 反转选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        SecurityProcessClass ReverseSelectSecurity(StrategyInClass Input);
    }
}

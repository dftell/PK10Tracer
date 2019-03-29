namespace WolfInv.com.StrategyLibForWD
{
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

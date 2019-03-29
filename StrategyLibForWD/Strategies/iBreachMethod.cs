namespace WolfInv.com.StrategyLibForWD
{
    public interface iBreachMethod
    {
        /// <summary>
        /// 突破选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        SecurityProcessClass BreachSelectSecurity(StrategyInClass Input);
    }
}

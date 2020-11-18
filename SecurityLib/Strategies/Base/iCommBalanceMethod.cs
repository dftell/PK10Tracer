using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public interface iCommBalanceMethod<T> where T:TimeSerialData
    {
        /// <summary>
        /// 反转突破结合选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        CommSecurityProcessClass<T> BalanceSelectSecurity(CommStrategyInClass Input);
    }
}

using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    public interface iCommBalanceMethod<T> where T:TimeSerialData
    {
        /// <summary>
        /// 反转突破结合选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        SelectResult BalanceSelectSecurity(CommStrategyInClass Input);
    }
}

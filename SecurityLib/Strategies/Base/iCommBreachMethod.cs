using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    public interface iCommBreachMethod<T> where T : TimeSerialData
    {
        /// <summary>
        /// 突破选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        SelectResult BreachSelectSecurity(CommStrategyInClass Input);
    }
}

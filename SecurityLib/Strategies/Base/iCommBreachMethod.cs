using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public interface iCommBreachMethod<T> where T : TimeSerialData
    {
        /// <summary>
        /// 突破选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        CommSecurityProcessClass<T> BreachSelectSecurity(CommStrategyInClass Input);
    }
}

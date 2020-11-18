using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public interface iCommReverseMethod<T> where T : TimeSerialData
    {
        /// <summary>
        /// 反转选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        CommSecurityProcessClass<T> ReverseSelectSecurity(CommStrategyInClass Input);
    }
}

using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    public interface iCommReverseMethod<T> where T : TimeSerialData
    {
        /// <summary>
        /// 反转选股
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        SelectResult ReverseSelectSecurity(CommStrategyInClass Input);
    }
}

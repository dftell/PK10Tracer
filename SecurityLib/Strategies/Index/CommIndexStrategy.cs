using WolfInv.com.GuideLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    #region 基础类及接口
    /// <summary>
    /// 指数/组合选择策略
    /// </summary>
    public abstract class CommIndexStrategy<T> : CommStrategyBaseClass<T> where T:TimeSerialData
    {
        public CommIndexStrategy(CommDataIntface<T> _w) : base(_w) { }
    }
    #endregion
}

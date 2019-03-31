using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    #region 基础类及接口
    /// <summary>
    /// 指数/组合选择策略
    /// </summary>
    public abstract class CommIndexStrategy : CommStrategyBaseClass
    {
        public CommIndexStrategy(CommDataIntface _w) : base(_w) { }
    }
    #endregion
}

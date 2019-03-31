using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    #region 基础类及接口

    /// <summary>
    /// 行业选择策略
    /// </summary>
    public abstract class CommIndustryStrategy : CommIndexStrategy, iCommIndustryStrategy
    {
        public CommIndustryStrategy(CommDataIntface _w) : base(_w) { }
        

        
    }
    #endregion
}

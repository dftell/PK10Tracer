using WolfInv.com.GuideLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    #region 基础类及接口

    /// <summary>
    /// 行业选择策略
    /// </summary>
    public abstract class CommIndustryStrategy<T> : CommIndexStrategy<T>, iCommIndustryStrategy<T> where T:TimeSerialData
    {
        public CommIndustryStrategy(CommDataIntface<T> _w) : base(_w) { }
        

        
    }
    #endregion
}

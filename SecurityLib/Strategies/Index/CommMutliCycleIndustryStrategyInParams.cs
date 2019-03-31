using System.Collections.Generic;
namespace WolfInv.com.SecurityLib
{
    #region 基础类及接口

    public class CommMutliCycleIndustryStrategyInParams : CommIndustryStrategyInParams
    {
        public string IndexName { get; set; }
        public List<string> InputList { get; set; }
    }
    #endregion
}

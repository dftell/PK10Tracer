using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    
    public class CommIndustryStrategyInParams:CommStrategyInClass
    {
        public CommIndustryType industryType { get; set; }
        public CommIndustryStrategyInParams()
        {
            industryType = CommIndustryType.SWI2;//默认使用申万二级
            prcAdj = PriceAdj.Beyond;
            Cyc = Cycle.Week;
        }
    }
}

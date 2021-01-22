using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    public abstract class MutliCycle_Filter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public MutliCycle_Filter(string endExpect, CommSecurityProcessClass<T> cpc, PriceAdj priceAdj = PriceAdj.Beyond, Cycle cyc = Cycle.Day) : base(endExpect, cpc, priceAdj, cyc)
        {

        }

        public abstract SelectResult CycleExecFilter(Cycle maxCycle, KLineData<T> kLineData, CommStrategyInClass Input);
    }
}

using WAPIWrapperCSharp;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.StrategyLibForWD
{
    public abstract class CommGuidProcess_ForWD : CommGuidProcess<TimeSerialData>
    {
        protected WindAPI w;
        public PriceAdj prcAdj;
        public Cycle cycle;
        protected CommDataBuilder<TimeSerialData> gbc;
        protected CommGuidProcess_ForWD() : base()
        {

        }
        public CommGuidProcess_ForWD(CommDataInterface_ForWD cdi) : base(cdi)
        {
            w = cdi.w;
        }

        public CommGuidProcess_ForWD(CommDataInterface_ForWD cdi,  Cycle cyc, PriceAdj rate) : base(cdi,cyc,rate)
        {
            w = cdi.w;
        }
        public CommGuidProcess_ForWD(CommDataInterface_ForWD cdi, CommDataBuilder_ForWD _gbc, Cycle cyc, PriceAdj rate) : base(cdi, _gbc,cyc, rate)
        {
            w = cdi.w;
        }
    }

}


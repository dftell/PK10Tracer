
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class CommGuidProcess_ForMG : CommGuidProcess
    {
        protected MongoDataReader w;
        public PriceAdj prcAdj;
        public Cycle cycle;
        protected CommDataBuilder gbc;
        protected CommGuidProcess_ForMG() : base()
        {

        }
        public CommGuidProcess_ForMG(CommDataInterface_ForMG cdi) : base(cdi)
        {
            w = cdi.w;
        }

        public CommGuidProcess_ForMG(CommDataInterface_ForMG cdi,  Cycle cyc, PriceAdj rate) : base(cdi,cyc,rate)
        {
            w = cdi.w;
        }
        public CommGuidProcess_ForMG(CommDataInterface_ForMG cdi, CommDataBuilder_ForMG _gbc, Cycle cyc, PriceAdj rate) : base(cdi, _gbc,cyc, rate)
        {
            w = cdi.w;
        }
    }

}


using WolfInv.com.CFZQ_LHProcess;
using WAPIWrapperCSharp;
using WolfInv.com.GuideLib;

namespace WolfInv.com.StrategyLibForWD
{
    public class CommDataBuilder : WDBuilder
    {
        ////public PriceAdj prcAdj;
        ////public Cycle cycle;
        protected GuidBaseClass gbc;
        protected string strParamsStyle;
        public CommDataBuilder(WindAPI w) : base(w) { }
        public CommDataBuilder(WindAPI _w, GuidBaseClass guidClass)
            : base(_w)
        {
            gbc = guidClass;
        }
    }

}


using WAPIWrapperCSharp;
using WolfInv.com.GuideLib;
namespace WolfInv.com.StrategyLibForWD
{
    public class CommDataInterface_ForWD:CommDataIntface
    {
        public WindAPI w;
        protected CommDataInterface_ForWD():base()
        {

        }

        public CommDataInterface_ForWD(WindAPI _w) : base()
        {
            w = _w;
        }
    }

}


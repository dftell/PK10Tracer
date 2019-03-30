using WAPIWrapperCSharp;
using WolfInv.com.GuideLib;
namespace WolfInv.com.StrategyLibForWD
{
    public abstract class CommDataBuilder_ForWD:CommDataBuilder
    {
        protected WindAPI w;
        protected CommDataBuilder_ForWD():base()
        {

        }
        public CommDataBuilder_ForWD(WindAPI _w) : base(new CommDataInterface_ForWD(_w))
        {
            w = _w;
        }
        public CommDataBuilder_ForWD(WindAPI _w, GuidBaseClass guidClass):base(new CommDataInterface_ForWD(_w), guidClass)
        {
            w = _w;
        }
    }

}


using WolfInv.com.GuideLib;

namespace WolfInv.com.GuideLib
{
    public class CommDataBuilder
    {
        ////public PriceAdj prcAdj;
        ////public Cycle cycle;
        public CommDataIntface DataInterFace;
        protected GuidBaseClass gbc;
        protected string strParamsStyle;

        protected CommDataBuilder()
        {

        }

        ////public CommDataBuilder(object DataInterface)
        ////{

        ////}

        public CommDataBuilder(CommDataIntface cdi) {
            DataInterFace = cdi;
        }
        public CommDataBuilder(CommDataIntface cdi, GuidBaseClass guidClass)
        {
            DataInterFace = cdi;
            gbc = guidClass;
        }
    }

}


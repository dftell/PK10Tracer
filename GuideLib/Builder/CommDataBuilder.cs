using WolfInv.com.GuideLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.GuideLib
{
    public class CommDataBuilder<T> where T:TimeSerialData
    {
        ////public PriceAdj prcAdj;
        ////public Cycle cycle;
        public CommDataIntface<T> DataInterFace;
        protected GuidBaseClass gbc;
        protected string strParamsStyle;

        protected CommDataBuilder()
        {

        }

        ////public CommDataBuilder(object DataInterface)
        ////{

        ////}

        public CommDataBuilder(CommDataIntface<T> cdi) {
            DataInterFace = cdi;
        }
        public CommDataBuilder(CommDataIntface<T> cdi, GuidBaseClass guidClass)
        {
            DataInterFace = cdi;
            gbc = guidClass;
        }
    }

}


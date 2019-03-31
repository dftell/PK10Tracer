
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    public class CommDataInterface_ForMG:CommDataIntface
    {
        //public DataReader w;
        public MongoDataReader w;
        protected CommDataInterface_ForMG():base()
        {

        }

        public CommDataInterface_ForMG(MongoDataReader _w) : base()
        {
            w = _w;
        }
    }

}


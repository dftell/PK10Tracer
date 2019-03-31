
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class CommDataBuilder_ForMG:CommDataBuilder
    {
        protected MongoDataReader w;
        protected CommDataBuilder_ForMG():base()
        {

        }
        public CommDataBuilder_ForMG(MongoDataReader _w) : base(new CommDataInterface_ForMG(_w))
        {
            w = _w;
        }
        public CommDataBuilder_ForMG(MongoDataReader _w, GuidBaseClass guidClass):base(new CommDataInterface_ForMG(_w), guidClass)
        {
            w = _w;
        }
    }

}


//using CFZQ_JRGCDB;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class CommSecurityProcessClass<T> where T:TimeSerialData
    {

        public MongoReturnDataList<T> SecInfo;
        public bool Enable;
        public CommSecurityProcessClass()
        {
        }

        public CommSecurityProcessClass(MongoReturnDataList<T> dr)
        {
            if (dr == null) return;
            SecInfo = dr;
            Enable = false;
        }
    }
}
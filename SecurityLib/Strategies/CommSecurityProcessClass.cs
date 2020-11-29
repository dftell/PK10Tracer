//using CFZQ_JRGCDB;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class CommSecurityProcessClass<T> where T:TimeSerialData
    {
        public StockInfoMongoData StockInfo;
        public MongoReturnDataList<T> SecPriceInfo;
        public bool Enable;
        public CommSecurityProcessClass()
        {
        }

        public CommSecurityProcessClass(StockInfoMongoData secInfo, MongoReturnDataList<T> dr)
        {
            if (dr == null) return;
            SecPriceInfo = dr;
            StockInfo = secInfo;
            Enable = false;
        }
    }
}
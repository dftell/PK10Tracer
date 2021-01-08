//using CFZQ_JRGCDB;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    public class CommSecurityProcessClass<T> where T:TimeSerialData
    {
        public StockInfoMongoData StockInfo;
        public MongoReturnDataList<T> SecPriceInfo;
        //public bool Enable;
        public CommSecurityProcessClass()
        {
            //Result = new SelectResult();
        }

        public CommSecurityProcessClass(StockInfoMongoData secInfo, MongoReturnDataList<T> dr)
        {
            if (dr == null) return;
            SecPriceInfo = dr;
            StockInfo = secInfo;
            //Enable = false;
            //Result = new SelectResult();
            
        }
    }
}
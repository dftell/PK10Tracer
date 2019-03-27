using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class  DateSerialCodeDataReader: MongoDataReader, IAllCodeDateSerialDataList, ICodeDateSerialDataList
    {
        protected string[] secCodes;
        protected DateSerialCodeDataReader()
        {

        }

        public DateSerialCodeDataReader(string db, string docname, string[] codes):base(db,docname)
        {
            secCodes = codes;
            builder = new DateSerialCodeDataBuilder(db, docname, codes);
        }

        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(bool DateAsc) where T : MongoData;
        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT, bool DateAsc) where T : MongoData;
        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT, string EndT, bool DateAsc) where T : MongoData;
        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string endT, int Cnt, bool DateAsc) where T : MongoData;        //
        //public abstract MongoDataList GetAllTimeSerialList();
    }
}

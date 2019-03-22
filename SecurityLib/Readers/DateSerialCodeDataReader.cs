using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class  DateSerialCodeDataReader: MongoDataReader, IAllCodeDateSerialDataList, ICodeDateSerialDataList
    {
        protected string[] secCodes;

        
        public DateSerialCodeDataReader(string db, string docname, string[] codes):base(db,docname)
        {
            secCodes = codes;
            builder = new DateSerialCodeDataBuilder(db, docname, codes);
        }

        public abstract MongoDataDictionary GetAllCodeDateSerialDataList(bool DateAsc);
        public abstract MongoDataDictionary GetAllCodeDateSerialDataList(string begT, bool DateAsc);
        public abstract MongoDataDictionary GetAllCodeDateSerialDataList(string begT, string EndT, bool DateAsc);
        public abstract MongoDataDictionary GetAllCodeDateSerialDataList(string endT, int Cnt, bool DateAsc);
    }
}

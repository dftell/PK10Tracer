using System.Collections.Generic;
using WolfInv.com.DbAccessLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class DateSerialDatabuilder : MongoDataBuilder,IDateSerialDatabuilder
    {
        protected DateSerialDatabuilder(string _db, string docname) : base(_db, docname)
        {

        }
        public string DateFieldName { get ; set ; }

        public abstract MongoReturnDataList<T> getData<T>(bool Asc) where T :MongoData;
        public abstract MongoReturnDataList<T> getData<T>(string begT, bool Asc) where T : MongoData;
        public abstract MongoReturnDataList<T> getData<T>(string begT, string endT, bool Asc) where T : MongoData;
        public abstract MongoReturnDataList<T> getData<T>(string endt, int Cycs, bool Asc) where T : MongoData;
        public abstract MongoReturnDataList<T> getFullTimeSerial<T>() where T : MongoData;
    }
}

using System.Collections.Generic;
using WolfInv.com.DbAccessLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class MongoDataReader
    {
        string DbTypeName;
        
        protected string TableName;
        protected MongoDataBuilder builder;
        protected MongoDataReader(string db,string docname)
        {
            DbTypeName = db;
            TableName = docname;
            
        }

    }

    public interface IAllCodeDateSerialDataList
    {
        MongoDataDictionary GetAllCodeDateSerialDataList(bool DateAsc);
    }
    
    public interface ICodeDateSerialDataList
    {
        MongoDataDictionary GetAllCodeDateSerialDataList(string begT,bool DateAsc);
        MongoDataDictionary GetAllCodeDateSerialDataList(string begT,string EndT, bool DateAsc);
        MongoDataDictionary GetAllCodeDateSerialDataList(string endT,int Cnt, bool DateAsc);
    }
}

using System.Collections.Generic;
using WolfInv.com.DbAccessLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class MongoDataReader
    {
        protected string DbTypeName;
        
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
        MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(bool DateAsc) where T : class, new();
    }

    public interface IGetAllTimeSerialList
    {
        MongoReturnDataList<T> GetAllTimeSerialList<T>() where T : class, new();
    }

    public interface ICodeDataList
    {
        MongoReturnDataList<T> GetAllCodeDataList<T>(bool Stoped) where T : class, new();
        MongoReturnDataList<T> GetAllCodeDataList<T>() where T : class, new();
    }
    
    public interface ICodeDateSerialDataList
    {
        MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT,bool DateAsc) where T : class, new();
        MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT,string EndT, bool DateAsc) where T : class, new();
        MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string endT,int Cnt, bool DateAsc) where T : class, new();
    }
}

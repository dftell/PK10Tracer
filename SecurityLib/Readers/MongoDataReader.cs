using System.Collections.Generic;
using WolfInv.com.DbAccessLib;
using WolfInv.com.BaseObjectsLib;
using System;

namespace WolfInv.com.SecurityLib
{
    public abstract class MongoDataReader:DataReader
    {
        protected string DbTypeName;
        
        protected string TableName;
        protected MongoDataBuilder builder;
        protected MongoDataReader()
        {

        }
        protected MongoDataReader(string db)
        {
            DbTypeName = db;
        }
        protected MongoDataReader(string db,string docname)
        {
            DbTypeName = db;
            TableName = docname;
            
        }

    }

    public interface IAllCodeDateSerialDataList
    {
        MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(bool DateAsc) where T : MongoData;
    }

    public interface IGetAllTimeSerialList
    {
        MongoReturnDataList<T> GetAllTimeSerialList<T>() where T : MongoData;
    }

    public interface ICodeDataList
    {
        MongoReturnDataList<T> GetAllCodeDataList<T>(bool Stoped) where T : MongoData;
        MongoReturnDataList<T> GetAllCodeDataList<T>() where T : MongoData;
    }
    
    public interface ICodeDateSerialDataList
    {
        MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT,bool DateAsc) where T : MongoData;
        MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT,string EndT, bool DateAsc) where T : MongoData;
        MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string endT,int Cnt, bool DateAsc) where T : MongoData;
    }
}

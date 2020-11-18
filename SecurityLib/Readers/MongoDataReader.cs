using System.Collections.Generic;
using WolfInv.com.DbAccessLib;
using WolfInv.com.BaseObjectsLib;
using System;

namespace WolfInv.com.SecurityLib
{
    public abstract class MongoDataReader<T>:DataReader<T> where T:TimeSerialData
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

    public interface IAllCodeDateSerialDataList<T> where T:TimeSerialData
    {
        MongoDataDictionary<T> GetAllCodeDateSerialDataList(bool DateAsc);
    }

    public interface IGetAllTimeSerialList<T> where T : TimeSerialData
    {
        MongoReturnDataList<T> GetAllTimeSerialList() ;
    }

    public interface ICodeDataList<T> where T : TimeSerialData
    {
        MongoReturnDataList<T> GetAllCodeDataList(bool Stoped) ;
        MongoReturnDataList<T> GetAllCodeDataList();
    }
    
    public interface ICodeDateSerialDataList<T> where T : TimeSerialData
    {
        MongoDataDictionary<T> GetAllCodeDateSerialDataList(string begT,bool DateAsc);
        MongoDataDictionary<T> GetAllCodeDateSerialDataList(string begT,string EndT, bool DateAsc);
        MongoDataDictionary<T> GetAllCodeDateSerialDataList(string endT,int Cnt, bool DateAsc) ;
    }
}

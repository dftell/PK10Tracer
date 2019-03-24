using WolfInv.com.DbAccessLib;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public interface IFindCodeData
    {
        string[] Codes { get; set; }
        string CodeFieldName { get; set; }
    }

    public interface IFindCodeListData
    {
        MongoReturnDataList<T> getData<T>(bool StopExchange) where T : class, new();
        MongoReturnDataList<T> getData<T>() where T : class, new();
    }

    public interface IFindDateSerialData
    {
        string DateFieldName { get; set; }
    }

    //日期序列数据构造器
    public interface IDateSerialDatabuilder: IFindDateSerialData
    {
        MongoReturnDataList<T> getData<T>(bool Asc) where T : class, new();
        MongoReturnDataList<T> getData<T>(string begT,bool Asc) where T : class, new();
        MongoReturnDataList<T> getData<T>(string begT,string endT, bool Asc) where T : class, new();
        MongoReturnDataList<T> getData<T>(string endt,int Cycs, bool Asc) where T : class, new();

        MongoReturnDataList<T> getFullTimeSerial<T>() where T : class, new();
    }
    public abstract class MongoDataBuilder 
    {
        protected MongoDBBase _mongoDB;
        protected string TableName;
        protected MongoDataBuilder(string dbType,string collName)
        {
            Init(dbType, collName);
        }

        protected void Init(string db, string collName)
        {
            _mongoDB =  GlobalClass.getCurrNoSQLDb(db);
            TableName = collName;
        }
    }

}

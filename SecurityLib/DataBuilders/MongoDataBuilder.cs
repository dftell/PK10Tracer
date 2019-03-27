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
        MongoReturnDataList<T1> getData<T1>(bool StopExchange) where T1 : MongoData;
        MongoReturnDataList<T1> getData<T1>() where T1 : MongoData;
    }

    public interface IFindDateSerialData
    {
        string DateFieldName { get; set; }
    }

    //日期序列数据构造器
    public interface IDateSerialDatabuilder: IFindDateSerialData
    {
        MongoReturnDataList<T1> getData<T1>(bool Asc) where T1 : MongoData;
        MongoReturnDataList<T1> getData<T1>(string begT,bool Asc) where T1 : MongoData;
        MongoReturnDataList<T1> getData<T1>(string begT,string endT, bool Asc) where T1 : MongoData;
        MongoReturnDataList<T1> getData<T1>(string endt,int Cycs, bool Asc) where T1 : MongoData;

        MongoReturnDataList<T1> getFullTimeSerial<T1>() where T1 : MongoData;
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

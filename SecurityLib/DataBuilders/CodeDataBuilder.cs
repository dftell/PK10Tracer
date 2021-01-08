using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class CodeDataBuilder: MongoDataBuilder, IFindCodeData,IFindCodeListData
    {
        public CodeDataBuilder(string _db, string DocName, string[] codes) : base(_db, DocName)
        {
            Codes = codes;
            CodeFieldName = "code";
            
        }

        public string[] Codes { get; set; }
        public string CodeFieldName { get; set; }

        public MongoReturnDataList<T> getData<T>(bool IncludeStoped) where T : TimeSerialData
        {
            ///StopExchange 暂时不知道怎么用
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            SortDefinition<T> sort = null;
            //查询字段
            string[] fileds = null;
            return new MongoReturnDataList<T>(new StockInfoMongoData(null,null), _mongoDB.FindList<T>(this.TableName, filter, fileds, sort),true);
        }

        public MongoReturnDataList<T> getData<T>() where T : TimeSerialData
        {
            return getData<T>(false);
        }
    }

    public class FinanceDataBuilder : MongoDataBuilder, IFindCodeData, IFindCodeListData
    {
        public FinanceDataBuilder(string _db, string DocName, string[] codes) : base(_db, DocName)
        {
            Codes = codes;
            CodeFieldName = "code";
            this.TableName = "stock_info";
        }

        public string[] Codes { get; set; }
        public string CodeFieldName { get; set; }

        public MongoReturnDataList<T> getData<T>(bool IncludeStoped) where T : TimeSerialData
        {
            ///StopExchange 暂时不知道怎么用
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            SortDefinition<T> sort = null;
            //查询字段
            string[] fileds = null;
            return new MongoReturnDataList<T>(new StockInfoMongoData(null,null), _mongoDB.FindList<T>(this.TableName, filter, fileds, sort),true);
        }

        public MongoReturnDataList<T> getData<T>() where T : TimeSerialData
        {
            return getData<T>(false);
        }
    }

}

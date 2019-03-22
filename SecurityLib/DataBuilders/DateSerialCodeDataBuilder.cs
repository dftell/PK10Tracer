using System.Collections.Generic;
using WolfInv.com.DbAccessLib;
using MongoDB.Bson;
using MongoDB.Driver;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{

    public class DateSerialCodeDataBuilder : DateSerialDatabuilder,IFindCodeData
    {
        public DateSerialCodeDataBuilder(string _db,string DocName,string[] codes):base(_db,DocName)
        {
            Codes = codes;
        }
        public string CodeFieldName { get; set; }
        public string[] Codes { get; set; }

        public override List<T> getData<T>(bool Asc)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            if (Codes.Length == 1)
                filter = filter & Builders<T>.Filter.Eq(a => (a as ICodeData).code, Codes[0]);
            else
                filter = filter & Builders<T>.Filter.In(a => (a as ICodeData).code, Codes);
            SortDefinition<T> sort = null;
            if (Asc)
                sort = Builders<T>.Sort.Ascending(a => (a as IDateData).date);
            else
                sort = Builders<T>.Sort.Descending(a => (a as IDateData).date);
            //查询字段
            string[] fileds = null;
            return _mongoDB.FindList<T>(this.TableName, filter, fileds, sort);
        }

        public override List<T> getData<T>(string begT, bool Asc)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            if (Codes.Length == 1)
                filter = filter & Builders<T>.Filter.Eq(a => (a as ICodeData).code, Codes[0]);
            else
                filter = filter & Builders<T>.Filter.In(a => (a as ICodeData).code, Codes);
            filter = filter & Builders<T>.Filter.Gte(a => (a as IDateData).date,begT);
            SortDefinition<T> sort = null;
            if(Asc)
                sort = Builders<T>.Sort.Ascending(a => (a as IDateData).date);
            else
                sort = Builders<T>.Sort.Descending(a => (a as IDateData).date);
            //查询字段
            string[] fileds = null;
            return _mongoDB.FindList<T>(this.TableName, filter, fileds, sort);
        }

        public override List<T> getData<T>(string begT, string endT, bool Asc)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            if (Codes.Length == 1)
                filter = filter & Builders<T>.Filter.Eq(a => (a as ICodeData).code, Codes[0]);
            else
                filter = filter & Builders<T>.Filter.In(a => (a as ICodeData).code, Codes);
            filter = filter & Builders<T>.Filter.Gte(a => (a as IDateData).date, begT);
            filter = filter & Builders<T>.Filter.Lte(a => (a as IDateData).date, endT);
            SortDefinition<T> sort = null;
            if (Asc)
                sort = Builders<T>.Sort.Ascending(a => (a as IDateData).date);
            else
                sort = Builders<T>.Sort.Descending(a => (a as IDateData).date);
            //查询字段
            string[] fileds = null;
            return _mongoDB.FindList<T>(this.TableName, filter, fileds, sort);
        }

        public override List<T> getData<T>(string endT, int Cycs, bool Asc)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            if (Codes.Length == 1)
                filter = filter & Builders<T>.Filter.Eq(a => (a as ICodeData).code, Codes[0]);
            else
                filter = filter & Builders<T>.Filter.In(a => (a as ICodeData).code, Codes);
            filter = filter & Builders<T>.Filter.Lte(a => (a as IDateData).date, endT);
            SortDefinition<T> sort = null;
            if (Asc)
                sort = Builders<T>.Sort.Ascending(a => (a as IDateData).date);
            else
                sort = Builders<T>.Sort.Descending(a => (a as IDateData).date);
            //查询字段
            string[] fileds = null;
            return _mongoDB.FindList<T>(this.TableName, filter, fileds, sort,Cycs);
        }
    }
}

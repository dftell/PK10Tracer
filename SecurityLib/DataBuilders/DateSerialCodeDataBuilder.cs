using System.Collections.Generic;
using WolfInv.com.DbAccessLib;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
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

        public override MongoReturnDataList<T> getData<T>(bool Asc) 
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            if (Codes != null && Codes.Length > 0)
            {
                if (Codes.Length == 1)
                    filter = filter & Builders<T>.Filter.Eq(a => (a as ICodeData).code, Codes[0]);
                else
                    filter = filter & Builders<T>.Filter.In(a => (a as ICodeData).code, Codes);
            }
            SortDefinition<T> sort = null;
            if (Asc)
                sort = Builders<T>.Sort.Ascending(a => (a as IDateStampData).date_stamp);
            else
                sort = Builders<T>.Sort.Descending(a => (a as IDateStampData).date_stamp);
            //查询字段
            string[] fileds = null;
            return new MongoReturnDataList<T>(_mongoDB.FindList<T>(this.TableName, filter, fileds, sort));
            
        }

        public override MongoReturnDataList<T> getData<T>(string begT, bool Asc)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            if (Codes.Length == 1)
                filter = filter & Builders<T>.Filter.Eq(a => (a as ICodeData).code, Codes[0]);
            else
                filter = filter & Builders<T>.Filter.In(a => (a as ICodeData).code, Codes);
            filter = filter & Builders<T>.Filter.Gte(a => (a as IDateStampData).date_stamp, MongoDateTime.Stamp(begT));
            SortDefinition<T> sort = null;
            FieldDefinition<T> fd = "{code_1_date_stamp_1:1}";
            if (Asc)
                //sort = Builders<T>.Sort.Ascending(a => (a as IDateStampData).date_stamp);
                sort = Builders<T>.Sort.Ascending(fd);
            else
                //sort = Builders<T>.Sort.Descending(a => (a as IDateStampData).date_stamp);
                sort = Builders<T>.Sort.Descending(fd);
            //查询字段
            string[] fileds = null;
            return new MongoReturnDataList<T>(_mongoDB.FindList<T>(this.TableName, filter, fileds, sort));
        }

        public override MongoReturnDataList<T> getData<T>(string begT, string endT, bool Asc)
        {
            ////FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            ////if (Codes.Length == 1)
            ////    filter = filter & Builders<T>.Filter.Eq(a => (a as ICodeData).code, Codes[0]);
            ////else
            ////{
            ////    //filter = filter & Builders<T>.Filter.In(a => (a as ICodeData).code, Codes);
            ////    FieldDefinition<T> existsval = "{code:{$exist:{0}}}".Replace("{0}",string.Join(",", Codes));
            ////    filter = filter & Builders<T>.Filter.Exists(existsval, true);
            ////}
            ////filter = filter & Builders<T>.Filter.Gte(a => (a as IDateStampData).date_stamp, MongoDateTime.Stamp(begT));
            ////filter = filter & Builders<T>.Filter.Lte(a => (a as IDateStampData).date_stamp, MongoDateTime.Stamp(endT));
            ////SortDefinition<T> sort = null;
            ////FieldDefinition<T> fd = "{code:1,date_stamp:1}";
            ////if (Asc)
            ////{
            ////    //    var sort = SortBy.Ascending("surname").Descending("email");
            ////    sort = Builders<T>.Sort.Ascending(fd);
            ////}
            ////else
            ////{
            ////    sort = Builders<T>.Sort.Descending(fd);
            ////}
            //查询字段
            //string[] fileds = null;
            
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            if(Codes == null || Codes.Length == 0)
            {
                return ret;
            }
            string sql = "{date_stamp:{$lte:{1},$gte:{2}},code:'{0}'}".Replace("{0}",Codes[0]).Replace("{1}",MongoDateTime.Stamp(endT).ToString()).Replace("{2}", MongoDateTime.Stamp(begT).ToString());
            if (Codes.Length > 1)
            {
                sql = "{date_stamp:{$lte:{1},$gte:{2}},code:{$in:['{0}']}}".Replace("{0}", string.Join("','",Codes)).Replace("{1}", MongoDateTime.Stamp(endT).ToString()).Replace("{2}", MongoDateTime.Stamp(begT).ToString()); ;
            }
            string sort = "{code_1_date_stamp_1:1}";
            return new MongoReturnDataList<T>(_mongoDB.FindList<T>(this.TableName, sql,sort));
        }

        
        public override MongoReturnDataList<T> getData<T>(string endT, int Cycs, bool Asc)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            if (Codes.Length == 1)
                filter = filter & Builders<T>.Filter.Eq(a => (a as ICodeDateStampData).code, Codes[0]);
            else
                filter = filter & Builders<T>.Filter.In(a => (a as ICodeDateStampData).code, Codes);
            filter = filter & Builders<T>.Filter.Lte(a => (a as ICodeDateStampData).date_stamp, MongoDateTime.Stamp(endT));
            SortDefinition<T> sort = null;
            FieldDefinition<T> fd = "{code_1_date_stamp_1:1}";
            if (Asc)
            {
                //    var sort = SortBy.Ascending("surname").Descending("email");
                sort = Builders<T>.Sort.Ascending(fd);
            }
            else
            {
                sort = Builders<T>.Sort.Descending(fd);
            }
            //查询字段
            string[] fileds = null;
            return new MongoReturnDataList<T>(_mongoDB.FindList<T>(this.TableName, filter, fileds, sort,Cycs));
        }

        public override MongoReturnDataList<T1> getDataGroupBy<T1>(string[] sqls)
        {
            List<T1> ret = _mongoDB.GroupBy<T1>(this.TableName, sqls);
            return new MongoReturnDataList<T1>(ret);
        }

        public override MongoReturnDataList<T> getFullTimeSerial<T>()
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            if (Codes.Length == 1)
                filter = filter & Builders<T>.Filter.Eq(a => (a as ICodeData).code, Codes[0]);
            else
                filter = filter & Builders<T>.Filter.In(a => (a as ICodeData).code, Codes);
            SortDefinition<T> sort = Builders<T>.Sort.Ascending(a => (a as IDateStampData).date_stamp);//升序
            //查询字段
            string[] fileds = null;// new string[] {"_id", "date" };
            return new MongoReturnDataList<T>(_mongoDB.FindList<T>(this.TableName, filter, fileds, sort));
        }
    }

   
}

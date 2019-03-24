using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;

namespace WolfInv.com.BaseObjectsLib
{
    public class MongoData:DisplayAsTableClass, IObjectId,ICloneable,IMatchFilter
    {
        public BsonObjectId _id { get; set; }
        public Object Clone()
        {
            return DetailStringClass.GetObjectByXml<OneCycleData>(this.ToDetailString());
        }

        public bool Match<T>(FilterDefinition<T> filter)
        {
            Type t = this.GetType();
            foreach(BsonElement key in filter.ToBsonDocument())
            {
                string name = key.Name;
                BsonValue val = key.Value;
                MemberInfo mi = t.GetProperty(name);
                object thisval = (mi as PropertyInfo)?.GetValue(this);
                if ( mi == null)
                {
                    mi = t.GetField(name);
                    thisval = (mi as FieldInfo)?.GetValue(this);
                }
                if(mi == null)
                {
                    return false;
                }
                bool ret =  (thisval?.Equals(val)).Value;
                if (ret == false)
                    return false;
            }
            return true;
        }
    }

    public interface IObjectId
    {
        BsonObjectId _id { get; set; }
    }

    public interface IMatchFilter
    {
        bool Match<T>(FilterDefinition<T> filter);
    }

    public interface ICodeData
    {
        string code { get; set; }
    }

    public class MongoReturnDataList<T> : List<T>
    {
        public MongoReturnDataList(List<T> list)
        {
            list.ForEach(p => this.Add(p));
        }

        public MongoReturnDataList()
        {

        }

        public MongoReturnDataList<T> Query(MongoFilter<T> condition)
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            List<T> list =this.FindAll(p => (p as MongoData).Match<T>(condition));
            this.Clear();
            list.ForEach(p => ret.Add(p));
            return ret;
        }

        
    }

    public class MongoFilter<TDocument> : FilterDefinition<TDocument>
    {
        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            throw new NotImplementedException();
        }
    }

}
;

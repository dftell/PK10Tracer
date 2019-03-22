using System;
using MongoDB.Bson;
namespace WolfInv.com.BaseObjectsLib
{
    public class MongoData:DisplayAsTableClass, IObjectId,ICloneable
    {
        public BsonObjectId _id { get; set; }
        public Object Clone()
        {
            return DetailStringClass.GetObjectByXml<OneCycleData>(this.ToDetailString());
        }
    }

    public interface IObjectId
    {
        BsonObjectId _id { get; set; }
    }

    public interface ICodeData
    {
        string code { get; set; }
    }

    
}
;

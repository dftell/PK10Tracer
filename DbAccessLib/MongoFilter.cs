using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
namespace WolfInv.com.DbAccessLib
{
    public class MongoFilter<TDocument> : FilterDefinition<TDocument>
    {
        FilterDefinition<TDocument> filter;
        public MongoFilter()
        {
           // filter = builder.Empty;
        }

        public MongoFilter(FilterDefinition<TDocument> AFilter)
        {
            filter = AFilter;
        }

        public FilterDefinition<TDocument> getFilter()
        {
            return filter;
        }



        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            throw new NotImplementedException();
        }

        public MongoFilter<TDocument> NewEq<TField>(MongoFilter<TDocument, TDocument> tdkey, TDocument tdval) 
        {
            MongoFilter<TDocument> ret = new MongoFilter<TDocument>(Builders<TDocument>.Filter.Eq<TDocument>(tdkey, tdval));
            return ret;            
        }

        public MongoFilter<TDocument> NewGt<TField>(MongoFilter<TDocument, TDocument> tdkey, TDocument tdval)
        {
            MongoFilter<TDocument> ret = new MongoFilter<TDocument>(Builders<TDocument>.Filter.Gt<TDocument>(tdkey, tdval));
            return ret;
        }

        public MongoFilter<TDocument> NewGte<TField>(MongoFilter<TDocument, TDocument> tdkey, TDocument tdval)
        {
            MongoFilter<TDocument> ret = new MongoFilter<TDocument>(Builders<TDocument>.Filter.Gte<TDocument>(tdkey, tdval));
            return ret;
        }

        public MongoFilter<TDocument> NewLt<TField>(MongoFilter<TDocument, TDocument> tdkey, TDocument tdval)
        {
            MongoFilter<TDocument> ret = new MongoFilter<TDocument>(Builders<TDocument>.Filter.Lt<TDocument>(tdkey, tdval));
            return ret;
        }

        public MongoFilter<TDocument> NewLte<TField>(MongoFilter<TDocument, TDocument> tdkey, TDocument tdval)
        {
            MongoFilter<TDocument> ret = new MongoFilter<TDocument>(Builders<TDocument>.Filter.Lte<TDocument>(tdkey, tdval));
            return ret;
        }

        public MongoFilter<TDocument> NewIn<TField>(MongoFilter<TDocument, TDocument> tdkey,IEnumerable<TDocument>  tdvals)
        {
            MongoFilter<TDocument> ret = new MongoFilter<TDocument>(Builders<TDocument>.Filter.In<TDocument>(tdkey, tdvals));
            return ret;
        }
    }

    public class MongoFilter<TDocument, TField> : FieldDefinition<TDocument, TField>
    {
 
        public override RenderedFieldDefinition<TField> Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            throw new NotImplementedException();
        }

    }



}

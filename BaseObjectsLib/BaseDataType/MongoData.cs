using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection;
using MongoDB.Bson.Serialization;
using WolfInv.com.DbAccessLib;
using System.Collections;
using System.Collections.Generic;
namespace WolfInv.com.BaseObjectsLib
{

    public class MongoData : DisplayAsTableClass, IObjectId, ICloneable, ICompareFilter, IConvertible, IFormatProvider
    {
        public MongoData()
        {

        }



        public BsonObjectId _id { get; set; }



        public T Clone<T>() where T : MongoData
        {
            return ConvertionExtensions.Clone<T>(this as T);
        }

        public object Clone()
        {
            return ConvertionExtensions.Clone(this);
        }


        public bool Compr(BsonElement bs)
        {
            Type t = this.GetType();
            BsonElement key = bs;
            BsonValue val = bs.Value;
            return ConvertionExtensions.Equal(this, bs.Name, getBstrVal, bs.Value);
        }

        Func<object, object> getBstrVal = delegate (object val)
        {
            return BsonValue.Create(val) as object;
        };

        public bool Compr(BsonDocument bs)
        {
            Type t = this.GetType();
            foreach (BsonElement key in bs)
            {
                string name = key.Name;
                BsonValue val = key.Value;
                if (!ConvertionExtensions.Equal(this, name, getBstrVal, val))
                    return false;
            }
            return true;
        }


        public MongoData ExtentData;
        public void AddExtentData(MongoData ExData)
        {
            ExtentData = ExData;
        }

        ////public string ItemName<TMongoData>(Func<TMongoData,PropertyInfo> func)
        ////{
        ////    return null;
        ////}

        #region IConvertible
        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }


        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        TypeCode IConvertible.GetTypeCode()
        {
            throw new NotImplementedException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return DetailStringClass.GetObjectByXml(this.ToXml(), conversionType);
        }

        public object GetFormat(Type formatType)
        {
            return this.ToXml();
        }
        #endregion

    }

    
    public interface IObjectId
    {
        BsonObjectId _id { get; set; }
    }

    public interface ICompareFilter
    {
        bool Compr(BsonElement filter);
    }

    public interface ICodeData
    {
        string code { get; set; }
    }

    

}
;

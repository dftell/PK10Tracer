using System;
using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract  class  DateSerialCodeDataReader: MongoDataReader, IAllCodeDateSerialDataList, ICodeDateSerialDataList
    {
        protected string[] secCodes;
        protected DateSerialCodeDataReader()
        {

        }
        public DateSerialCodeDataReader(string db):base(db)
        {
            
        }
        public DateSerialCodeDataReader(string db, string docname, string[] codes):base(db,docname)
        {
            secCodes = codes;
            builder = new DateSerialCodeDataBuilder(db, docname, codes);
        }

        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(bool DateAsc) where T : MongoData;
        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT, bool DateAsc) where T : MongoData;
        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT, string EndT, bool DateAsc) where T : MongoData;
        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string endT, int Cnt, bool DateAsc) where T : MongoData;        //

        public override ExpectList<T> GetMissedData<T>(bool IsHistoryData, string strBegT)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> getNewestData<T>(ExpectList<T> NewestData, ExpectList<T> ExistData)
        {
            throw new NotImplementedException();
        }

        public override DbChanceList<T> getNoCloseChances<T>(string strDataOwner)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory<T>()
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory<T>(long buffs)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory<T>(long From, long buffs)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory<T>(long From, long buffs, bool desc)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory<T>(string begt, string endt)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadNewestData<T>(DateTime fromdate)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadNewestData<T>(int LastLng)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadNewestData<T>(int ExpectNo, int Cnt)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadNewestData<T>(int ExpectNo, int Cnt, bool FromHistoryTable)
        {
            throw new NotImplementedException();
        }

        public override int SaveChances<T>(List<ChanceClass<T>> list, string strDataOwner)
        {
            throw new NotImplementedException();
        }

        public override int SaveHistoryData<T>(ExpectList<T> InData)
        {
            throw new NotImplementedException();
        }

        public override int SaveNewestData<T>(ExpectList<T> InData)
        {
            throw new NotImplementedException();
        }
        //public abstract MongoDataList GetAllTimeSerialList();
    }
}

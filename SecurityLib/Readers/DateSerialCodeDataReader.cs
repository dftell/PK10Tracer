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
            return ReadHistory<T>(From, buffs, true);
        }

        public override ExpectList<T> ReadHistory<T>(long From, long buffs, bool desc)
        {
            DateTime dt = new DateTime( From);
            MongoDataDictionary<T> res = GetAllCodeDateSerialDataList<T>(string.Format("{0}-{1}-{2}",dt.Year,dt.Month.ToString().PadLeft(2,'0'),dt.Day.ToString().PadLeft(2,'0')), true);
            Dictionary<string, MongoReturnDataList<T>> data = res;
            ExpectList<T> ret = new ExpectList<T>(data, true);
            return ret;
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
        public override int DeleteChanceByIndex(long index, string strDataOwner = null)
        {
            throw new NotImplementedException();
        }

        public override int DeleteExpectData(string expectid)
        {
            throw new NotImplementedException();
        }

        public override void ExecProduce(string Procs)
        {
            throw new NotImplementedException();
        }
    }
}

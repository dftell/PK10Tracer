using System;
using System.Collections.Generic;
using WolfInv.com.LogLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class DataReader :LogableClass, IDataReader
    {
        protected string strDataType;
        protected string strChanceTable;
        protected string strResultTable;
        protected string strNewestTable;
        protected string strHistoryTable;
        protected string strMissHistoryTable ;
        protected string strMissNewestTable ;
        protected DataTypePoint dtp = null;

        protected void InitTables()
        {
            dtp = GlobalClass.TypeDataPoints[strDataType];

            strChanceTable = dtp.ChanceTable;
            strResultTable = dtp.ResultTable;
            strNewestTable = dtp.NewestTable;
            strHistoryTable = dtp.HistoryTable;
            strMissHistoryTable = dtp.MissHistoryTable;
            strMissNewestTable = dtp.MissNewestTable;
        }

        public abstract ExpectList<T> GetMissedData<T>(bool IsHistoryData, string strBegT) where T : TimeSerialData;
        public abstract ExpectList<T> getNewestData<T>(ExpectList<T> NewestData, ExpectList<T> ExistData) where T : TimeSerialData;
        public abstract DbChanceList<T> getNoCloseChances<T>(string strDataOwner) where T : TimeSerialData;
        public abstract ExpectList<T> ReadHistory<T>() where T : TimeSerialData;
        public abstract ExpectList<T> ReadHistory<T>(long buffs) where T : TimeSerialData;
        public abstract ExpectList<T> ReadHistory<T>(long From, long buffs) where T : TimeSerialData;
        public abstract ExpectList<T> ReadHistory<T>(long From, long buffs, bool desc) where T : TimeSerialData;
        public abstract ExpectList<T> ReadHistory<T>(string begt, string endt) where T : TimeSerialData;
        public abstract ExpectList<T> ReadNewestData<T>(DateTime fromdate) where T : TimeSerialData;
        public abstract ExpectList<T> ReadNewestData<T>(int LastLng) where T : TimeSerialData;
        public abstract ExpectList<T> ReadNewestData<T>(int ExpectNo, int Cnt) where T : TimeSerialData;
        public abstract ExpectList<T> ReadNewestData<T>(int ExpectNo, int Cnt, bool FromHistoryTable) where T : TimeSerialData;
        public abstract int SaveChances<T>(List<ChanceClass<T>> list, string strDataOwner=null) where T : TimeSerialData;
        public abstract int SaveHistoryData<T>(ExpectList<T> InData) where T : TimeSerialData;
        public abstract int SaveNewestData<T>(ExpectList<T> InData) where T : TimeSerialData;

        public abstract int DeleteChanceByIndex(long index, string strDataOwner = null);

        public abstract int DeleteExpectData(string expectid);

        public abstract void ExecProduce(string Procs);
    }

}
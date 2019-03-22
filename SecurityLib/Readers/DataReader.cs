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

        public abstract ExpectList GetMissedData(bool IsHistoryData, string strBegT);
        public abstract ExpectList getNewestData(ExpectList NewestData, ExpectList ExistData);
        public abstract DbChanceList getNoCloseChances(string strDataOwner);
        public abstract ExpectList ReadHistory();
        public abstract ExpectList ReadHistory(long buffs);
        public abstract ExpectList ReadHistory(long From, long buffs);
        public abstract ExpectList ReadHistory(long From, long buffs, bool desc);
        public abstract ExpectList ReadHistory(string begt, string endt);
        public abstract ExpectList ReadNewestData(DateTime fromdate);
        public abstract ExpectList ReadNewestData(int LastLng);
        public abstract ExpectList ReadNewestData(int ExpectNo, int Cnt);
        public abstract ExpectList ReadNewestData(int ExpectNo, int Cnt, bool FromHistoryTable);
        public abstract int SaveChances(List<ChanceClass> list, string strDataOwner);
        public abstract int SaveHistoryData(ExpectList InData);
        public abstract int SaveNewestData(ExpectList InData);
    }
}
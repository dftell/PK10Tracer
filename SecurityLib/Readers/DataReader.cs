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

        //public abstract string getNextExpectNo(string expect);

        public abstract ExpectList<T> GetMissedData<T>(bool IsHistoryData, string strBegT) where T : TimeSerialData;
        public abstract ExpectList<T> getNewestData<T>(ExpectList<T> NewestData, ExpectList<T> ExistData) where T : TimeSerialData;
        public virtual DbChanceList<T> getNoCloseChances<T>(string strDataOwner) where T : TimeSerialData
        {
            return null;
        }
        public virtual DbChanceList<T> getClosedChances<T>(string strDataOwner,int PassedDays) where T : TimeSerialData
        {
            return null;
        }
        public abstract ExpectList<T> ReadHistory<T>() where T : TimeSerialData;
        public abstract ExpectList<T> ReadHistory<T>(long buffs) where T : TimeSerialData;
        public abstract ExpectList<T> ReadHistory<T>(long From, long buffs) where T : TimeSerialData;
        public abstract ExpectList<T> ReadHistory<T>(long From, long buffs, bool desc) where T : TimeSerialData;
        public abstract ExpectList<T> ReadHistory<T>(string begt, string endt) where T : TimeSerialData;
        public abstract ExpectList<T> ReadNewestData<T>(DateTime fromdate) where T : TimeSerialData;
        public abstract ExpectList<T> ReadNewestData<T>(int LastLng) where T : TimeSerialData;
        public abstract ExpectList<T> ReadNewestData<T>(long ExpectNo, int Cnt) where T : TimeSerialData;
        public abstract ExpectList<T> ReadNewestData<T>(long ExpectNo, int Cnt, bool FromHistoryTable) where T : TimeSerialData;
        public abstract int SaveChances<T>(List<ChanceClass<T>> list, string strDataOwner=null) where T : TimeSerialData;
        public abstract int SaveHistoryData<T>(ExpectList<T> InData) where T : TimeSerialData;
        public abstract int SaveNewestData<T>(ExpectList<T> InData) where T : TimeSerialData;

        public abstract System.Data.DataSet ReadExData(DataTypePoint dtp, string expectNo, Func<DataTypePoint, string, System.Data.DataSet> ConvertFunc);

        public abstract int DeleteChanceByIndex(long index, string strDataOwner = null);

        public abstract int DeleteExpectData(string expectid);

        public abstract void ExecProduce(string Procs);
        public  string getNextExpectNo(string expect)
        {
            int DateLong = GlobalClass.TypeDataPoints[this.strDataType].ExpectCodeDateLong;
            int CounterMax = GlobalClass.TypeDataPoints[this.strDataType].ExpectCodeCounterMax;
            string dateFmt = GlobalClass.TypeDataPoints[this.strDataType].ExpectCodeDateFormate;
            //if(DateLong==0)
            //    return string.Format("{0}", long.Parse(expect) + 1);
            string strDate = expect.Substring(0, DateLong);
            string strCounter = expect.Substring(DateLong);
            int CounterLong = expect.Length - strDate.Length;
            long counterValue = long.Parse(strCounter);
            DateTime currDate;
            if (!DateTime.TryParseExact(strDate, dateFmt,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AdjustToUniversal,
                out currDate))//不是日期，继续合上
            {
                return string.Format("{0}{1}", strDate, string.Format("{0}", counterValue + 1).PadLeft(CounterLong, '0'));
            }
            else
            {
                if (counterValue < CounterMax)
                {
                    return string.Format("{0}{1}", strDate, string.Format("{0}", counterValue + 1).PadLeft(CounterLong, '0'));
                }
                else
                {
                    return string.Format("{0}{1}", currDate.AddDays(1).ToString(dateFmt), "1".PadLeft(CounterLong, '0'));
                }
            }
        }

        public abstract void updateExpectInfo(string dataType, string nextExpect, string currExpect);
    }

}
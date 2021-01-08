using System;
using System.Collections.Generic;
using WolfInv.com.LogLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class DataReader<T> : LogableClass, IDataReader<T> where T:TimeSerialData
    {
        protected string strDataType;
        protected string strChanceTable;
        protected string strResultTable;
        protected string strNewestTable;
        protected string strHistoryTable;
        protected string strMissHistoryTable;
        protected string strMissNewestTable;
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

        public abstract ExpectList<T> GetMissedData(bool IsHistoryData, string strBegT) ;
        public abstract ExpectList<T> getNewestData(ExpectList<T> NewestData, ExpectList<T> ExistData) ;
        public virtual DbChanceList<T> getNoCloseChances(string strDataOwner) 
        {
            return null;
        }
        public virtual DbChanceList<T> getClosedChances(string strDataOwner, int PassedDays) 
        {
            return null;
        }
        public abstract ExpectList<T> ReadHistory() ;
        public abstract ExpectList<T> ReadHistory(long buffs,string codes) ;
        public abstract ExpectList<T> ReadHistory(string From, long buffs,string codes) ;
        public abstract ExpectList<T> ReadHistory(long cnt, string endExpect,string codes) ;
        public abstract ExpectList<T> ReadHistory(string From, long buffs,string codes, bool desc) ;
        public abstract ExpectList<T> ReadHistory(string begt, string endt,string codes) ;
        public abstract ExpectList<T> ReadNewestData(DateTime fromdate) ;
        public abstract ExpectList<T> ReadNewestData(int LastLng) ;
        public abstract ExpectList<T> ReadNewestData(long ExpectNo, int Cnt) ;
        public ExpectList<T> ReadNewestData(long ExpectNo, int Cnt, bool FromHistoryTable, string Code) 
        {
            return ReadNewestData(ExpectNo.ToString(),Cnt,FromHistoryTable,Code);
        }
        public abstract ExpectList<T> ReadNewestData(string ExpectNo, int Cnt, bool FromHistoryTable, string Code) ;
        public abstract int SaveChances(List<ChanceClass<T>> list, string strDataOwner=null) ;
        public abstract int SaveHistoryData(ExpectList<T> InData) ;
        public abstract int SaveNewestData(ExpectList<T> InData) ;

        public abstract System.Data.DataSet ReadExData(DataTypePoint dtp, string expectNo, Func<DataTypePoint, string, System.Data.DataSet> ConvertFunc);

        public abstract int DeleteChanceByIndex(long index, string strDataOwner = null);

        public abstract int DeleteExpectData(string expectid);

        public abstract void ExecProduce(string Procs);
        public static  string getNextExpectNo(string expect,DataTypePoint dtp)
        {
            int DateLong = dtp.ExpectCodeDateLong;
            int CounterMax = dtp.ExpectCodeCounterMax;
            int CounterLong = dtp.ExpectCodeCounterLen;
            string dateFmt = dtp.ExpectCodeDateFormate;
            //if(DateLong==0)
            //    return string.Format("{0}", long.Parse(expect) + 1);
            string strDate = expect.Substring(0, DateLong);
            string strCounter = expect.Substring(DateLong);
            if(CounterLong<=0)//如果默认值为0
                CounterLong = expect.Length - strDate.Length;
            long counterValue = long.Parse(strCounter);
            DateTime currDate;
            try
            {
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
            catch(Exception ce)
            {
                ToLog(ce.Message, strDate);
                throw ce;
                return string.Format("{0}{1}", strDate, string.Format("{0}", counterValue + 1).PadLeft(CounterLong, '0'));
            }
        }

        public static long getInterExpectCnt(string expectFrom,string expectTo,DataTypePoint dtp)
        {
            if(dtp.IsSecurityData == 1)
            {
                int days =(int)expectTo.ToDate().Subtract(expectFrom.ToDate()).TotalDays;
                return days;
            }
            long lFrom = long.Parse(expectFrom);
            long lTo = long.Parse(expectTo);
            if (lTo <= lFrom ) //
                return lFrom-lTo;
            if(dtp.DataType == "PK10")
            {
                return lTo - lFrom;
            }
            if(lTo-lFrom<dtp.ExpectCodeCounterMax)
            {
                return Math.Max(1,lTo - lFrom);
            }
            string currExpect = expectFrom;
            long ret = 1;
            while(true)
            {
                currExpect = getNextExpectNo(currExpect, dtp);
                //ToLog("循环获得的当前期号", currExpect);
                if(currExpect.Trim() == expectTo.Trim())
                {
                    return ret;
                }
                ret++;
                //if (ret> 1000)
                //{
                //    return 1;
                //}
            }
        }
        public abstract void updateExpectInfo(string dataType, string nextExpect, string currExpect,string openCode,string openTime);

        public static string getStdExpect(string strExpect, DataTypePoint dtp)
        {
            if (dtp.DataType == "PK10")
                return strExpect;
            int len = dtp.ExpectCodeCounterLen;
            if (len ==0)
            {
                len = dtp.ExpectCodeCounterMax.ToString().Length;
            }
            int currLen = strExpect.Trim().Length;
            if(currLen == dtp.ExpectCodeDateLong+len)
            {
                return strExpect;
            }
            if(currLen>dtp.ExpectCodeDateLong+len)
            {
                string strDate = strExpect.Substring(0, dtp.ExpectCodeDateLong);
                string strCount = strExpect.Substring(dtp.ExpectCodeDateLong);
                return strDate + strCount.Substring(strCount.Length - len);//取右边的，最后的len位
            }
            else
            {
                throw new Exception("期号长度小于标准长度，请检查！");
            }
            return strExpect;
        }
    }

}
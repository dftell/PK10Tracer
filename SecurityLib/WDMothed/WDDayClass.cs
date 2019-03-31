using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib
{
    public class MGDayClass
    {
        public static DateTime[] getTradeDates(MongoDataReader w, DateTime begt, DateTime endt, Cycle cyc)
        {
            TDaysGuidClas tgc = new TDaysGuidClas();
            tgc.cycle = cyc;
            TDayGuildBuilder_ForMG tgb = new TDayGuildBuilder_ForMG(w,tgc);
            MTable ret = tgb.getRecords(begt, endt);
            return ret.ToList<DateTime>().ToArray();
        }

        public static DateTime[] getTradeDates(MongoDataReader w, string SecCode, DateTime begt, DateTime endt, Cycle cyc)
        {
            BaseDataTable tb = CommWDToolClass.GetBaseSerialData(w, SecCode, begt, endt, cyc, PriceAdj.UnDo, BaseDataPoint.trade_status, BaseDataPoint.sec_type, BaseDataPoint.close);
            BaseDataTable ttb = tb.AvaliableData;
            return ttb["DateTime"].ToList<DateTime>().ToArray();
        }

        public static DateTime[] getTradeDates(MongoDataReader w, DateTime begt, DateTime endt)
        {
            return getTradeDates(w, begt, endt, Cycle.Day);
        }

        public static int getTradeDays(MongoDataReader w, DateTime begt, DateTime endt)
        {
            TDaysGuidClas tgc = new TDaysGuidClas();
            TDayGuildBuilder_ForMG tgb = new TDayGuildBuilder_ForMG(w, tgc);
            MTable ret = tgb.getRecordsCount(begt, endt);
            return (int)ret.GetTable().Rows[0][0];
        }

        public static DateTime Offset(MongoDataReader w, DateTime  endt,int N,Cycle cyc)
        {
            TDaysGuidClas tgc = new TDaysGuidClas();
            tgc.cycle = cyc;
            TDayGuildBuilder_ForMG tgb = new TDayGuildBuilder_ForMG(w, tgc);
            MTable ret = tgb.getRecords(endt,N);
            DateTime lastdate = Convert.ToDateTime(ret.GetTable().Rows[0][0]);
            if(cyc == Cycle.Day) return lastdate;
            DateTime[] dates = getTradeDates(w, lastdate, endt);
            for (int i = 0; i < dates.Length-1; i++)
            {
                if (dates[i].AddDays(1).CompareTo(dates[i + 1]) < 0)
                {
                    return dates[i];
                }
            }
            return lastdate;

        }

        public static DateTime Offset(MongoDataReader w, BaseDataItemClass SecItem, DateTime dt, int N, Cycle cyc)
        {
            DateTime begt = SecItem.Ipo_date;
            if(SecItem.SecType == SecType.Index)
            {
                //指数不停牌，自然日按交易日*7/5，放大到10/2=2倍指定开始日
                switch(cyc)
                {
                    case Cycle.Day:
                        {
                            begt =  dt.AddDays(-2*N);
                            break;
                        }
                    case Cycle.Week:
                        {
                            begt = dt.AddDays(-7*N);
                            break;
                        }
                    case Cycle.Month:
                        {
                            begt = dt.AddMonths(N);
                            break;
                        }
                    case Cycle.Year:
                        {
                            begt = dt.AddYears(N);
                            break;
                        }
                }
            
            }
            if(begt.CompareTo(SecItem.Ipo_date)<0)//任何情况（index的ipo日期为1899-12-31）如果开始日期小于ipo日期，开始日期设置为Ipo日期。
            {
                begt = SecItem.Ipo_date;
            }

            
            BaseDataTable tb = CommMGToolClass.GetBaseSerialData(
                w, 
                SecItem.WindCode,
                begt,
                dt, 
                Cycle.Day, 
                PriceAdj.UnDo, 
                BaseDataPoint.trade_status,BaseDataPoint.sec_type);
            BaseDataTable ttb = tb.AvaliableData;
            if(ttb.Count == 0) return dt;//如果数据为空，返回回览日
            if (ttb.Count >N) return ((BaseDataItemClass)ttb[ttb.Count -N]).DateTime;
            return ((BaseDataItemClass)ttb[0]).DateTime;
        }
        /// <summary>
        /// 市场最后交易日
        /// </summary>
        /// <param name="w"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime LastTradeDay(MongoDataReader w, DateTime dt, Cycle cy)
        {
            DateTime[] ret = getTradeDates(w, dt.AddDays(-20), dt,cy);
            if (ret.Length > 0)
            {
                return ret[ret.Length - 1];
            }
            return DateTime.MinValue;
        }

        public static DateTime LastTradeDay(MongoDataReader w, DateTime dt)
        {
            return LastTradeDay(w, dt, Cycle.Day);
        }

        public static DateTime LastTradeDay(MongoDataReader w, string SecCode, DateTime dt)
        {
            BaseDataTable tb =  CommMGToolClass.GetBaseSerialData(w, SecCode, dt.AddDays(-1000), dt, Cycle.Day, PriceAdj.UnDo,BaseDataPoint.trade_status,BaseDataPoint.sec_type,BaseDataPoint.close);
            BaseDataTable ttb = tb.AvaliableData;
            if (ttb.Count == 0) return DateTime.MinValue;
            return ((BaseDataItemClass)ttb[ttb.Count - 1]).DateTime;
        }
    }
}

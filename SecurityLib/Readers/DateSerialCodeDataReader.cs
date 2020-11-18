using System;
using System.Collections.Generic;
using System.Linq;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract  class  DateSerialCodeDataReader<T>: MongoDataReader<T>, IAllCodeDateSerialDataList<T>, ICodeDateSerialDataList<T> where T :TimeSerialData
    {
        protected string[] secCodes;
        static ExpectList<T> AllData = null;
        static List<string> AllDates = null;

        protected DateSerialCodeDataReader()
        {
            if (AllData == null)
            {
                getAllData();
                //AllDates = AllData.Select(a => a.Expect).ToList();
            }
        }
        public DateSerialCodeDataReader(string db):base(db)
        {
            if (AllData == null)
            {
                getAllData();
            }
        }
        public DateSerialCodeDataReader(string db, string docname, string[] codes):base(db,docname)
        {
            if (AllData == null)
            {
                getAllData();
            }
            secCodes = codes;
            builder = new DateSerialCodeDataBuilder(db, docname, codes);
        }

        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList(bool DateAsc) ;
        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList(string begT, bool DateAsc);
        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList(string begT, string EndT, bool DateAsc);
        public abstract MongoDataDictionary<T> GetAllCodeDateSerialDataList(string endT, int Cnt, bool DateAsc) ;        //

        public override ExpectList<T> GetMissedData(bool IsHistoryData, string strBegT)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> getNewestData(ExpectList<T> NewestData, ExpectList<T> ExistData)
        {
            throw new NotImplementedException();
        }

        public override DbChanceList<T> getNoCloseChances(string strDataOwner)
        {
            return new DbChanceList<T>();
        }

        public override ExpectList<T> ReadHistory()
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory(long buffs)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory(long From, long buffs)
        {
            return ReadHistory(From, buffs, true);
        }

        public override ExpectList<T> ReadHistory(long From, long buffs, bool desc)
        {
            DateTime dt = new DateTime( From);
            DateTime test = dt.AddDays(-1);
            
            DateTime et = dt.AddTicks(dt.Subtract(test).Ticks*buffs);
            string begt = string.Format("{0}-{1}-{2}", dt.Year, dt.Month.ToString().PadLeft(2, '0'), dt.Day.ToString().PadLeft(2, '0'));
            string endt = string.Format("{0}-{1}-{2}", et.Year, et.Month.ToString().PadLeft(2, '0'), et.Day.ToString().PadLeft(2, '0'));


            ////MongoDataDictionary<T> res = GetAllCodeDateSerialDataList(string.Format("{0}-{1}-{2}",dt.Year,dt.Month.ToString().PadLeft(2,'0'),dt.Day.ToString().PadLeft(2,'0')), true);
            ////Dictionary<string, MongoReturnDataList<T>> data = res;
            ////ExpectList<T> ret = new ExpectList(data, true);

            return ReadHistory(begt,endt);
        }

        public override ExpectList<T> ReadHistory(string begt, string endt)
        {
            DateTime dt = DateTime.Parse(begt);
            MongoDataDictionary<T> res = GetAllCodeDateSerialDataList(begt,endt, true);
            Dictionary<string, MongoReturnDataList<T>> data = res;
            ExpectList<T> ret = new ExpectList<T>(data, true);
            ExpectList<T> retlist = new ExpectList<T>();
            ret.DataList.ForEach(a => {
                ExpectData<T> ed = new ExpectData<T>();
                ed.CurrTime = null;
                ed.Key = "Security";
                foreach(string code in a.Keys)
                {
                    if(ed.CurrTime == null)
                    {
                        ed.CurrTime = a[code].Expect.WDDate();
                    }
                    ed.Expect = a[code].Expect;
                    ed.Add(code,a[code] as T);
                }
                retlist.Add(ed);
            });
            return retlist;
        }

        public override ExpectList<T> ReadNewestData(DateTime fromdate)
        {
            string expect = fromdate.WDDate();
            
            
            if (AllDates == null||AllDates.Count == 0)
                return new ExpectList<T>();
            int cnt = 0;
            if(AllDates.Count>0)
            {
                if(AllDates.First().ToDate()> fromdate || AllDates.Last().ToDate()<fromdate)
                {
                    return new ExpectList<T>();
                }
            }
            for(int i=0;i< AllDates.Count;i++)
            {
                if(AllDates[i].ToDate()>=fromdate)
                {
                    cnt = i;
                    break;
                }
            }
            ExpectList<T> ret = new ExpectList<T>();
            var items = AllData.LastDatas(AllDates.Count - cnt, true);
            for(int i = 0;i < items.Count;i++)
            {
                ExpectData<T> item = items[i] as ExpectData<T>;
                ret.Add(item);
            }
            return ret;
        }

        public override ExpectList<T> ReadNewestData(int LastLng)
        {
            ExpectList<T> ret = new ExpectList<T>();
            AllData.LastDatas(LastLng, true).ForEach(a => ret.Add(a as ExpectData<T>));
            return ret;
        }

        public override ExpectList<T> ReadNewestData(long ExpectNo, int Cnt)
        {
            throw new NotImplementedException();
        }
        static void getAllData(int localDataLen=1000)
        {
            Dictionary<string, string> allequites = WDDataInit.WDDataInit<T>.AllSecurities;
            if (allequites == null || allequites.Count == 0)
            {
                WDDataInit.WDDataInit<T>.Init();

            }
            if (!WDDataInit.WDDataInit<T>.Loaded &&!WDDataInit.WDDataInit<T>.Loading)
            {
                WDDataInit.WDDataInit<T>.loadAllEquitSerials(8,20 ,true, true, localDataLen, true);
            }
            if (WDDataInit.WDDataInit<T>.Loaded)
            {
                MongoDataDictionary<T> datas = WDDataInit.WDDataInit<T>.getAllSerialData();
                if (datas != null)
                {
                    AllData = datas.ToExpectList();
                    AllDates = datas.getAllDates();
                }
            }
            else
            {
                MongoDataDictionary<T> res = WDDataInit.WDDataInit<T>.getAllSerialData();
                AllData = res.ToExpectList();
                AllDates = WDDataInit.WDDataInit<T>.getAllSerialData().getAllDates();
            }
           
        }
        public override ExpectList<T> ReadNewestData(string ExpectNo, int Cnt, bool FromHistoryTable)
        {
            if (AllData == null)
            {
                getAllData();
            }
            if(AllData == null)
                return new ExpectList<T>();
            ExpectList<T> tres = AllData.LastDatas(ExpectNo, Cnt, FromHistoryTable);
            

            //tres?.ForEach(a => ret.Add(a as ExpectData<T1>));
            return tres;
        }

        public override int SaveChances(List<ChanceClass<T>> list, string strDataOwner)
        {
            throw new NotImplementedException();
        }

        public override int SaveHistoryData(ExpectList<T> InData)
        {
            throw new NotImplementedException();
        }

        public override int SaveNewestData(ExpectList<T> InData)
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

        public override void updateExpectInfo(string dataType, string nextExpect, string currExpect,string openCode,string openTime)
        {
            throw new NotImplementedException();
        }
    }
}

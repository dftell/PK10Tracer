using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.DbAccessLib;
using WolfInv.com.WDDataInit;
namespace WolfInv.com.SecurityLib
{
    public abstract  class  DateSerialCodeDataReader<T>: MongoDataReader<T>, IAllCodeDateSerialDataList<T>, ICodeDateSerialDataList<T> where T :TimeSerialData
    {
        protected string[] secCodes;
        static ExpectList<T> AllData = null;
        static DateTime[] AllDates = null;

        protected DateSerialCodeDataReader()
        {
            if (AllData == null)
            {
                //getAllData();
                //AllDates = AllData.Select(a => a.Expect).ToList();
            }
        }
        public DateSerialCodeDataReader(string db):base(db)
        {
            if (AllData == null)
            {
                //getAllData();
            }
        }
        public DateSerialCodeDataReader(string db, string docname, string[] codes):base(db,docname)
        {
            if (AllData == null)
            {
                //getAllData();
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

        public override ExpectList<T> ReadHistory(long buffs,string codes)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory(string From, long buffs, string codes)
        {
            return ReadHistory(From, buffs,codes, true);
        }

        public override ExpectList<T> ReadHistory(string From, long buffs, string codes, bool desc)
        {
            DateTime dt = From.ToDate();
            DateTime test = dt.AddDays(-1);
            AllDates = WDDataInit<T>.AllDays;
            int index = AllDates.IndexOf(From);
            DateTime et = AllDates[Math.Min(AllDates.Length - 1, index + buffs)];
            string begt = From;
            string endt = et.WDDate();


            ////MongoDataDictionary<T> res = GetAllCodeDateSerialDataList(string.Format("{0}-{1}-{2}",dt.Year,dt.Month.ToString().PadLeft(2,'0'),dt.Day.ToString().PadLeft(2,'0')), true);
            ////Dictionary<string, MongoReturnDataList<T>> data = res;
            ////ExpectList<T> ret = new ExpectList(data, true);

            return ReadHistory(begt,endt,codes);
        }

        public override ExpectList<T> ReadHistory(string begt, string endt, string codes)
        {
            getAllData(codes,0, begt, endt, true); 
            if (AllData == null)
                return new ExpectList<T>(true);
            ExpectList<T> tres = AllData;

            return tres;
            /*
            //tres?.ForEach(a => ret.Add(a as ExpectData<T1>));

            DateTime dt = DateTime.Parse(begt);
            MongoDataDictionary<T> res = GetAllCodeDateSerialDataList(begt,endt, true);
            Dictionary<string, MongoReturnDataList<T>> data = res;
            ExpectList<T> ret = new ExpectList<T>(data, true,true);
            ExpectList<T> retlist = new ExpectList<T>(true);
            ret.DataList.ForEach(a => {
                ExpectData<T> ed = new ExpectData<T>(true);
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
            */
        }

        public override ExpectList<T> ReadNewestData(DateTime fromdate)
        {
            string expect = fromdate.WDDate();
            
            
            if (AllDates == null||AllDates.Length == 0)
                return new ExpectList<T>(true);
            int cnt = 0;
            if(AllDates.Length>0)
            {
                if(AllDates.First()> fromdate || AllDates.Last()<fromdate)
                {
                    return new ExpectList<T>(true);
                }
            }
            for(int i=0;i< AllDates.Length;i++)
            {
                if(AllDates[i]>=fromdate)
                {
                    cnt = i;
                    break;
                }
            }
            ExpectList<T> ret = new ExpectList<T>(true);
            var items = AllData.LastDatas(AllDates.Length - cnt, true);
            for(int i = 0;i < items.Count;i++)
            {
                ExpectData<T> item = items[i] as ExpectData<T>;
                ret.Add(item);
            }
            return ret;
        }

        public override ExpectList<T> ReadNewestData(int LastLng)
        {
            ExpectList<T> ret = new ExpectList<T>(true);
            AllData.LastDatas(LastLng, true).ForEach(a => ret.Add(a as ExpectData<T>));
            return ret;
        }

        public override ExpectList<T> ReadNewestData(long ExpectNo, int Cnt)
        {
            throw new NotImplementedException();
        }
        static void getAllData(string codes,int localDataLen=1000,string fromdate=null,string todate=null,bool reread=false)
        {
            try
            {
                Dictionary<string, string> allequites = WDDataInit.WDDataInit<T>.AllSecurities;
                if (allequites == null || allequites.Count == 0)
                {
                    WDDataInit.WDDataInit<T>.Init();
                }
                bool needRead = false;
                if (!WDDataInit.WDDataInit<T>.Loaded && !WDDataInit.WDDataInit<T>.Loading)
                {
                    needRead = true;
                }
                if (!needRead)
                {
                    needRead = reread;
                }
                //WDDataInit<T>.Debug = true;
                if (needRead)//需要读取
                {
                    if (string.IsNullOrEmpty(codes))
                        WDDataInit<T>.loadAllEquitSerials(10, 20, true, true, localDataLen, fromdate, todate, true);
                    else
                    {
                        string[] codesArr = codes.Split(';');
                        for (int i = 0; i < codesArr.Length; i++)
                        {
                            string name = "";
                            string code = codesArr[i].WDCode();
                            if (allequites != null && allequites.ContainsKey(code))
                            {
                                name = allequites[code];
                            }
                            WDDataInit<T>.loadEquitSerial(code, name, true, true, localDataLen, fromdate, todate);

                        }
                    }
                }
                if (WDDataInit<T>.Loaded)
                {
                    MongoDataDictionary<T> datas = WDDataInit.WDDataInit<T>.getAllSerialData(codes);
                    if (datas != null)
                    {
                        AllData = datas.ToExpectList();
                        AllDates = datas.getAllDates();
                    }
                }
                else
                {
                    MongoDataDictionary<T> res = WDDataInit.WDDataInit<T>.getAllSerialData(codes);
                    AllData = res.ToExpectList();
                    AllDates = WDDataInit.WDDataInit<T>.getAllSerialData(codes).getAllDates();
                }
            }
            catch(Exception ce)
            {

            }
            finally
            {
                //GC.Collect();
            }
        }
        public override ExpectList<T> ReadNewestData(string ExpectNo, int Cnt, bool FromHistoryTable,string codes)
        {
            if (AllData == null || !ExpectNo.Equals(AllData.LastData.Expect))
            {
                getAllData(codes,Cnt, null, ExpectNo, true);
            }
            if(AllData == null)
                return new ExpectList<T>(true);
            ExpectList<T> tres = AllData.LastDatas(ExpectNo, Cnt, FromHistoryTable);
            //tres?.ForEach(a => ret.Add(a as ExpectData<T1>));
            return tres;
        }

        public override int SaveChances(List<ChanceClass<T>> list, string strDataOwner)
        {
            if (list.Count == 0)
                return 0;
            DbChanceList<T> ret = new DbChanceList<T>();
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select top 0 * from {0}", strChanceTable);
            DataTable dt = db.getTableBySqlAndList<ChanceClass<T>>(new ConditionSql(sql), list);
            if (dt == null)
            {
                ToLog("保存机会数据错误", "根据数据表结构和提供的列表返回的机会列表错误！");
                return -1;
            }
            if (strDataOwner == null || strDataOwner.Trim().Length == 0)
                sql = string.Format("Select * from {0} where IsEnd=0", strChanceTable);
            else
                sql = string.Format("Select * from {0} where IsEnd=0 and UserId='{1}'", strChanceTable, strDataOwner);
            return db.UpdateOrNewList(new ConditionSql(sql), dt);
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

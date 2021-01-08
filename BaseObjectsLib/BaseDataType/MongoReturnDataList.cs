using System;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Data;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;

namespace WolfInv.com.BaseObjectsLib
{
    public class MongoReturnDataList<T> : List<T>, ICloneable where T :TimeSerialData
    {
        public bool isSecurity = false;
        public StockInfoMongoData SecInfo;
        public bool Disable;
        ConcurrentDictionary<string, int> _maps;
        ConcurrentDictionary<string,int> maps
        {
            get
            {
                
                bool needUpdate = false;
                //LockSlim.EnterWriteLock();
                try
                {
                    if (_maps == null)
                    {
                        _maps = new ConcurrentDictionary<string, int>();
                    }
                    if (_maps.Count != this.Count)
                    {
                        needUpdate = true;
                    }
                    if (needUpdate)
                    {
                        lock (_maps)
                        {
                            _maps = new ConcurrentDictionary<string, int>();
                            for (int i = 0; i < this.Count; i++)
                            {
                                //ExchangeMongoData data = this[i] as ExchangeMongoData;
                                TimeSerialData data = this[i];
                                string date = this[i]?.Expect;
                                if (!date.isDate())
                                {
                                    date = this[i]?.OpenTime.ToString();
                                }
                                if (string.IsNullOrEmpty(date))
                                    continue;
                                lock (_maps)
                                {
                                    if (!_maps.ContainsKey(date))
                                    {
                                        if (!_maps.TryAdd(date, i))
                                        {

                                        }
                                    }
                                    else
                                    {
                                        _maps[date] = i;
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    //LockSlim.ExitWriteLock();
                }
                return _maps;
            }
        }
        public MongoReturnDataList(StockInfoMongoData info,List<T> list,bool sectype)
        {
            SecInfo = info;
            isSecurity = sectype;
            list?.ForEach(p => this.Add(p));
        }

        public MongoReturnDataList(StockInfoMongoData info,bool sectype)
        {
            SecInfo = info;
            isSecurity = sectype;
        }

        public T this[string key]
        {
            get
            {
                
                //lock (maps)
                LockSlim.EnterUpgradeableReadLock();
                try
                {
                    if (maps.ContainsKey(key) && maps[key] >= 0 && maps[key] < this.Count)
                    {
                        return this[maps[key]];
                    }

                }
                finally
                {
                    LockSlim.ExitUpgradeableReadLock();
                }
                return null;
            }
            set
            {
                //lock (maps)
                //LockSlim.EnterWriteLock();
                try
                {
                    if (maps.ContainsKey(key) && maps[key] >= 0 && maps[key] < this.Count)
                    {
                        this[maps[key]] = value;
                    }
                    else
                        this.Add(value);
                }
                finally
                {
                    //LockSlim.ExitWriteLock();
                }
                return;
            }
        }

        public MongoReturnDataList<T> LastDays(int N)
        {
            int len = Math.Min(this.Count, N);
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>(this.SecInfo,this.isSecurity);
            var items = this.Skip(len - N);
            foreach (var item in items)
            {
                ret.Add(item.Expect , item);
            }
            return ret;
        }

        public MongoReturnDataList<T> AfterDate(string Date)
        {
            DateTime cprDate = Date.ToDate();
            if(this.Count==0)
            {
                return this;
            }
            List<T> useList = this.OrderBy(a => a.Expect.ToDate()).ToList();
            int index = -1;
            if (cprDate > useList.Last().Expect.ToDate())
            {
                index = useList.Count - 1;
            }
            else if(cprDate < useList.First().Expect.ToDate())
            {
                index = 0;
            }
            else
                index = useList.Where(a => a.Expect.ToDate() < Date.ToDate()).Count();
            if (index < 0)
                return new MongoReturnDataList<T>(this.SecInfo, isSecurity);
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>(this.SecInfo, isSecurity);
            foreach (var key in useList.Skip(index))
            {
                ret.Add(key.Expect , key);
            }
            return ret;
        }


        public void Add(string key,T value)
        {
            this.Add(value);
        }
        ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
        public bool ContainKey(string key)
        {
            //lock (maps.Keys)
            
            LockSlim.EnterUpgradeableReadLock();
            try
            {
                bool succ = maps.ContainsKey(key);
                if (succ)
                {
                    return maps[key] >= 0 && maps[key] < this.Count;
                }
                return false;

            }
            catch
            {
                return false;
            }
            finally
            {
                LockSlim.ExitUpgradeableReadLock();
            }

        }

        public List<string> Keys
        {
            get
            {
                return maps.Keys.ToList();
            }
        }
        public DataTable GetMainDataTable()
        {
            DataTable dt = DisplayAsTableClass.ToTable<T>(this);
            return dt;
        }

        public DataTable GetExtendDataTable<EXT>() where EXT:MongoData
        { 
            List<EXT> Exlist = this.ToList<EXT>(a => a.ExtentData() as EXT);
            DataTable dtExData = DisplayAsTableClass.ToTable<EXT>(Exlist);
            return dtExData;
        }

        public MongoReturnDataList<T> Query<Field>(string col,Field val) 
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>(this.SecInfo, isSecurity);
            try
            {
                BsonElement bs = new BsonElement(col,BsonValue.Create(val));
                //LogLib.LogableClass.ToLog("查询条件", string.Format("{0}=>{1}",col,val));
                List<T> list = this.FindAll(p => p.Compr(bs)==true);
                
                //this.Clear();
                list?.ForEach(p => ret.Add(p));
            }
            catch(Exception ce)
            {
                LogLib.LogableClass.ToLog(string.Format("结果链查询错误[{0}]", ce.Message),ce.StackTrace);
            }
            return ret;
        }

        public MongoReturnDataList<T> Query(BsonDocument func)
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>(this.SecInfo, isSecurity);
            try
            {
                List<T> list = this.FindAll(p => (p as MongoData).Compr(func));
                //this.Clear();
                list.ForEach(p => ret.Add(p));
            }
            catch (Exception ce)
            {
                LogLib.LogableClass.ToLog("结果链查询", ce.Message);
            }
            return ret;
        }

        public object Clone()
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>(this.SecInfo, isSecurity);
            this.ForEach(a => ret.Add(a.Clone() as T));
            return ret;
        }
        public MongoReturnDataList<T> Copy(bool Deeply = false)
        {            MongoReturnDataList<T> ret = new MongoReturnDataList<T>(this.SecInfo, isSecurity);
            this.ForEach(a => ret.Add(Deeply ? a.Clone<T>():a));
            return ret;
        }

        public List<TField> Loc<TField>(int[] indies, string FieldName)
        {
            List<TField> ret = new List<TField>();
            foreach (int i in indies)
            {
                T doc = this[i];
                ret.Add(ConvertionExtensions.GetValue<T, TField>(doc, FieldName));
            }
            return ret;
        }

        /// <summary>
        /// 定位
        /// </summary>
        /// <typeparam name="T">本身类</typeparam>
        /// <typeparam name="SubT">子对象</typeparam>
        /// <typeparam name="TField">定位字段名</typeparam>
        /// <param name="indies"></param>
        /// <param name="func"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public List<TField> Loc<Tdata, TField>(int[] indies,Func<T,Tdata> func,string FieldName)
        {
            List<TField> ret = new List<TField>();
            foreach (int i in indies)
            {
                T doc = this[i];
                ret.Add(ConvertionExtensions.GetValue<Tdata, TField>(func(this[i]), FieldName));
            }
            return ret;
        }

        public TField Loc<TField>(int index, string fieldname)
        {
            int[] inds = new int[] { index };
            List<TField> list = Loc<TField>(inds, fieldname);
            if (list == null)
                return default(TField);
            return list[0];
        }



        
        public List<TField> ToList<TField>(Func<T,TField> func)
        {
            List<TField> ret = new List<TField>();
            this.ForEach(a=>ret.Add(func(a)));
            return ret;
        }

        public MongoReturnDataList<T> GetDataByIndies(int[] indies)
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>(this.SecInfo, isSecurity);
            for (int i=0;i< indies.Length; i++)
            {
                ret.Add(this[indies[i]]);
            }
            return ret;
        }

        public MongoReturnDataList<T> GetFirstData(int len)
        {
            int[] Arr = new int[len];
            for (int i = 0; i < len; i++)
                Arr[i] = i;
            return GetDataByIndies(Arr);
        }

        public MongoReturnDataList<T> GetLastData(int len,string expect,string expectFormat="yyyy-MM-dd",string longFormat="yyyyMMdd")
        {
            lock (this)
            {
                int[] Arr = new int[len];
                var items = this.Where(a => a.Expect.ToLong(longFormat, expectFormat) <= expect.ToLong(longFormat, expectFormat)).OrderBy(a => a.Expect);
                MongoReturnDataList<T> ret = new MongoReturnDataList<T>(this.SecInfo, isSecurity);
                foreach (var item in items.Skip(Math.Max(0, items.Count() - len)))
                {
                    ret.Add(item);
                }
                return ret;
                for (int i = this.Count - len; i < Count; i++)
                    Arr[i - (this.Count - len)] = i;
                return GetDataByIndies(Arr);
            }
        }

        /*
        public static implicit operator MongoReturnDataList<T>(MongoReturnDataList<T> v)
        {
            throw new NotImplementedException();
        }
        */
    }

    public static class DateString
    {
        public static bool isDate(this string str,string format="yyyy-MM-dd")
        {
            DateTime outres;
            return DateTime.TryParseExact(str, format, null, System.Globalization.DateTimeStyles.None, out outres);
        }
    }
    

}


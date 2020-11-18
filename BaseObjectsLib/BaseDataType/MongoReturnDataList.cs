using System;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Data;
using System.Linq;
namespace WolfInv.com.BaseObjectsLib
{
    public class MongoReturnDataList<T> : List<T>, ICloneable where T :TimeSerialData
    {
        public bool Disable;
        Dictionary<string, int> _maps;
        Dictionary<string,int> maps
        {
            get
            {
                bool needUpdate = false; ;
                if(_maps == null ||_maps.Count != this.Count)
                {
                    needUpdate = true ;
                }
                if(needUpdate )
                {
                    _maps = new Dictionary<string, int>();
                    for(int i=0;i<this.Count;i++)
                    {
                        //ExchangeMongoData data = this[i] as ExchangeMongoData;
                        TimeSerialData data = this[i];
                        string date = this[i]?.Expect;
                        if (string.IsNullOrEmpty(date))
                            continue;
                        if (!_maps.ContainsKey(data?.Expect))
                        {
                            _maps.Add(date, i);
                        }
                        else
                        {
                            _maps[date] = i;
                        }
                    }
                }
                return _maps;
            }
        }
        public MongoReturnDataList(List<T> list)
        {
            list?.ForEach(p => this.Add(p));
        }

        public MongoReturnDataList()
        {

        }

        public T this[string key]
        {
            get
            {
                if(maps.ContainsKey(key) && maps[key]>=0 && maps[key]<this.Count)
                {
                    return this[maps[key]];
                }
                return null;
            }
            set
            {
                if (maps.ContainsKey(key) && maps[key] >= 0 && maps[key] < this.Count)
                {
                    this[maps[key]] = value;
                    return;
                }
                this.Add(value);
            }
        }

        public MongoReturnDataList<T> LastDays(int N)
        {
            int len = Math.Min(this.Count, N);
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
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
            if (cprDate > this.Last().Expect.ToDate() || cprDate < this.First().Expect.ToDate())
            {
                return new MongoReturnDataList<T>();
            }
            int index = this.Keys.ToList().Where(a => a.ToDate() < Date.ToDate()).Count();
            if (index < 0)
                return new MongoReturnDataList<T>();
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            foreach (var key in this.Skip(index))
            {
                ret.Add(key.Expect , key);
            }
            return ret;
        }


        public void Add(string key,T value)
        {
            this.Add(value);
        }

        public bool ContainKey(string key)
        {
            bool succ = maps.ContainsKey(key);
            if(succ)
            {
                return maps[key] >= 0 && maps[key] < this.Count;
            }
            return succ;
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
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
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
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
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
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            this.ForEach(a => ret.Add(a.Clone() as T));
            return ret;
        }
        public MongoReturnDataList<T> Copy(bool Deeply = false)
        {            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
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
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
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

        public MongoReturnDataList<T> GetLastData(int len,string expect)
        {
            int[] Arr = new int[len];
            var items = this.Where(a => a.Expect.ToLong() <= expect.ToLong()).OrderBy(a=>a.Expect);
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            foreach(var item in items.Skip(Math.Max(0, items.Count() - len)))
            {
                ret.Add(item);
            }
            return ret;
            for (int i = this.Count-len; i < Count; i++)
                Arr[i-(this.Count - len)] = i;
            return GetDataByIndies(Arr);
        }

    }

    
    

}
;

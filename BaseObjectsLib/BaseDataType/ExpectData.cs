using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace WolfInv.com.BaseObjectsLib
{
    public abstract class LottyData:TimeSerialData
    {

    }
    [Serializable]
    public class ExpectData<T> : TimeSerialData, IDictionary<string, T> where T : TimeSerialData
    {
        Dictionary<string, T> list = new Dictionary<string, T>();
        public int _ArrLen = 10;//对pk10
        public int ArrLen
        {
            get
            {
                return _ArrLen;
            }
            set
            {
                _ArrLen = value;
            }
        }

        public ExpectData()
        {

        }

        public ExpectData(T a)
        {
            this.Expect = a.Expect;
            this.OpenCode = a.OpenCode;
            this.OpenTime = a.OpenTime;
            if (this.ContainsKey(this.Key))
                this.Add(this.Key, a);
        }
        
        public bool IsValidData()
        {
            string[] vals = ValueList;
            if(vals.Length != ArrLen)
            {
                return false;
            }
            Dictionary<int, string> all = new Dictionary<int, string>();
            for(int i=0;i<vals.Length;i++)
            {
                int r = -1;
                bool conv = int.TryParse(vals[i], out r);
                if (conv == false)
                    return false;
                if (all.ContainsKey(r))
                    return false;
                all.Add(r, vals[i]);
            }
            if (all.Count != ArrLen) //多此一举
                return false;
            //完整性
            ////for(int i=1;i<=10;i++)
            ////{
            ////    if (!all.ContainsKey(i))
            ////        return false;
            ////}
            return true;
        }

        public T this[string key]
        {
            get
            {
                return list[key];
            }
            set
            {
                list[key] = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return list?.Keys;
            }
        }

        public ICollection<T> Values
        {
            get { return list?.Values; }
        }

        public int Count
        {
            get {
                return list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public void Add(string key, T value)
        {
            list.Add(key, value);
        }

        public void Add(KeyValuePair<string, T> item)
        {

            list.Add(item.Key,item.Value);
        }

        public void Clear()
        {
            list.Clear();
        }

        ////public override object Clone()
        ////{
        ////    Dictionary<string, T> ret = new Dictionary<string, T>();
        ////    foreach (string key in list.Keys)
        ////        ret.Add(key, list[key].Clone<T>());
        ////    return ret;
        ////}

        public bool Contains(KeyValuePair<string, T> item)
        {
            return list.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return list.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            //KeyValuePair<string, T>[] ret = 
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return list.Remove(key);
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            return list.Remove(item.Key);
        }

        public bool TryGetValue(string key, out T value)
        {
            return list.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }



        #region 无奈实现下
        public string[] ValueList
        {
            get
            {
                string[] ret = OpenCode.Trim().Split(',');
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = ret[i].Trim().Substring(1);
                }
                return ret;
            }
        }

        public long LExpectNo
        {
            get { return long.Parse(Expect); }
        }
        public long ExpectIndex
        {
            get
            {
                long v = LExpectNo;
                if (LExpectNo >= 344978) //34977
                {
                    v = v - 1;
                }
                if (LExpectNo >= 356395)//2013/4/16 356375-356395 missed
                {
                    v = v - 19;
                }
                return v;
            }
        }

        
        #endregion
    }






    /*
    public class ExpectData:ExpectData<MongoData>
    {
        Int64 _Eid;
        ExpectDataClass _occurObj;
        ExpectDataClass OccurObj
        {
            get
            {
                _occurObj = this.Values.First();
                if(_occurObj == null)
                {
                    _occurObj = new ExpectDataClass();
                    this.Add("Lotty", _occurObj);
                }
                _occurObj = this.Values.First();
                return _occurObj;
            }
        }

        public Int64 EId
        {
            get
            {
                return OccurObj.EId;
            }
            set
            {
                OccurObj.EId = value;
            }
        }


        public int MissedCnt
        {
            get
            {
                return OccurObj.MissedCnt;
            }
            set
            {
                OccurObj.MissedCnt = value;
            }
        }
        public string LastExpect
        {
            get
            {
                return OccurObj.LastExpect;
            }
            set
            {
                OccurObj.LastExpect = value;
            }
        }

        public string Expect
        {
            get
            {
                return OccurObj.LastExpect;
            }
            set
            {
                OccurObj.LastExpect = value;
            }
        }

        public string OpenCode;

        public DateTime OpenTime { get; set; }
    }
    */
    public class ExpectDataEEE: ExpectData<TimeSerialData>
{
        //////public Int64 EId;
        //////public int MissedCnt;
        //////public string LastExpect;
        //////public string Expect;
        //////public string OpenCode;

        //////public DateTime OpenTime { get; set; }

        public string[] ValueList
        {
            get
            {
                string[] ret = OpenCode.Trim().Split(',');
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = ret[i].Trim().Substring(1);
                }
                return ret;
            }
        }
        public long LExpectNo
        {
            get { return long.Parse(Expect); }
        }
        public long ExpectIndex
        {
            get 
            {
                long v = LExpectNo;
                if (LExpectNo >= 344978) //34977
                {
                    v = v - 1;
                }
                if (LExpectNo >= 356395)//2013/4/16 356375-356395 missed
                {
                    v = v - 19;
                }
                return v;
            }
        }

        //////public override object Clone()
        //////{
        //////    ExpectData ret = new ExpectData();
        //////    ret.Expect = this.Expect;
        //////    ret.OpenCode = this.OpenCode;
        //////    ret.OpenTime = this.OpenTime;
        //////    if(ret.CurrData!= null)
        //////    {
        //////        ret.CurrData = this.CurrData.Clone();
        //////    }
        //////    return ret;
        //////}

        public string PreExpect
        {
            get
            {
                if (Expect == null || Expect.Trim().Length == 0)
                    return null;
                return string.Format("{0}",Int64.Parse(Expect) - 1);
            }
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.BaseObjectsLib
{

    public class ExpectList<T> : List<ExpectData<T>> where T :TimeSerialData
    {
        bool Readed=false;
        protected Dictionary<string, MongoReturnDataList<T>> _Data;
        protected Dictionary<string, MongoReturnDataList<T>> Data
        {
            get
            {
                if(Readed)
                {
                    return _Data;
                }
                _Data = new Dictionary<string, MongoReturnDataList<T>>();
                if (_MyData.Count == 0) return _Data;
                if (_MyData[0].Keys == null || _MyData[0].Count == 0)
                    _Data.Add("Lotty", new MongoReturnDataList<T>());
                for (int i=0;i<_MyData.Count;i++)
                {
                    if (_MyData[i].Keys == null || _MyData[i].Count == 0)//对彩票
                    {
                        T val = _MyData[i].CopyTo<T>();
                        _Data[val.Key].Add(val);
                    }
                    else
                    {
                        foreach (string key in _MyData[i].Keys)
                        {
                            if (!_Data.ContainsKey(key))
                            {
                                _Data.Add(key, new MongoReturnDataList<T>());
                            }
                            _Data[key].Add(_MyData[i][key]);
                        }
                    }
                }
                Readed = true;
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }

        //public DataTypePoint UseType;
        public Cycle Cyc = Cycle.Expect;
        List<ExpectData<T>> _MyData= new List<ExpectData<T>>();
        
        protected List<ExpectData<T>> MyData
        {
            get
            {
                if (_MyData == null)
                    throw new Exception("对象为空");
                return _MyData;
            }
        }

        public List<ExpectData<T>> DataList
        {
            get
            {
                return MyData;
            }
        }

        public ExpectData<T> LastData
        {
            get
            {
                //////if (MyData.Count == 0)
                //////{
                //////    string test = "1";
                //////    test = "1";
                //////}
                if (MyData == null || MyData.Count == 0)
                    return null;
                return MyData[MyData.Count - 1];
            }
        }

        
        public ExpectList()
        {
            Data = new Dictionary<string, MongoReturnDataList<T>>();
            _MyData = new List<ExpectData<T>>();
        }

        public ExpectData<T> FirstData
        {
            get { return MyData[0]; }
        }

        
        public long MinExpect
        {
            get
            {
                return MyData.Select(a => long.Parse(a.Expect)).Min();
            }
        }
        

        public long MissExpectCount()
        {
            if (this.Count <= 1)
                return 1;
            int cnt = this.Count;
            long lastid = long.Parse(this[cnt - 1].Expect);
            long preid = long.Parse(this[cnt - 2].Expect);
            if(this[cnt-1].ExpectSerialByType==1)//如果是连续期号，直接计算差
                return lastid - preid;
            if(this[cnt - 1].ExpectSerialByType == 2)//如果是按日期
            {
                string e1 =  this[cnt - 1].Expect.Trim();
                string e2 = this[cnt - 2].Expect.Trim();
                if (e1.Length != e2.Length)
                    return 999;
                int sL =  this[cnt - 1].ExpectSerialLong;//编号长度
                int dL = e1.Length - sL;//日期长度
                int s1 = int.Parse(e1.Substring(dL, sL));//最后值序号
                int s2 = int.Parse(e2.Substring(dL, sL));//倒数第二个序号
                if (e1.Substring(0,dL).Equals(e2.Substring(0,dL)))//如果日期相等
                {
                    return s1 - s2;
                }
                else
                {
                    return s2;//默认前面一个的值是正常的。？？？？？
                }

            }
            return 1;
        }
        
        public ExpectList(Dictionary<string, MongoReturnDataList<T>> _data,bool NeedReTime)
        {
            _MyData = new List<ExpectData<T>>();
            List<string> datelist = new List<string>();
            if (NeedReTime)
            {
                var times = _data.Values.ToList().Select(a => a.Select(b => b.CurrTime));
                foreach (var itime in times)
                {
                    foreach (string key in itime)
                    {
                        if (!datelist.Contains(key))
                        {
                            datelist.Add(key);
                        }
                    }
                }
                datelist.OrderBy(a => a);
                for (int i = 0; i < datelist.Count; i++)
                {
                    ExpectData<T> newData = new ExpectData<T>();
                    string strCurrDate = datelist[i];
                    foreach (MongoReturnDataList<T> vals in _data.Values)
                    {
                        var val = vals.Where(a => a.CurrTime == strCurrDate);
                        if (val == null || val.Count<T>() == 0)
                            continue;
                        T vData = val.First();
                        
                        newData.Add(vData.Key, vData);
                    }
                    newData.Expect = newData.First().Value.Expect;
                    newData.OpenCode = newData.First().Value.OpenCode;
                    newData.OpenTime = newData.First().Value.OpenTime;
                    _MyData.Add(newData);

                }
                
            }
            else
            {
                _data.First().Value.ForEach(a=> _MyData.Add(new ExpectData<T>(a)));
            }
            
            Data = _data;
            Readed = false;
        }

        public ExpectList(DataTable dt)
        {
            _MyData = new List<ExpectData<T>>();
            if (dt == null) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ExpectData<T> ed = new ExpectData<T>();
                ed.Expect = dt.Rows[i]["Expect"].ToString();
                ed.OpenCode = dt.Rows[i]["OpenCode"].ToString();
                string[] arr = ed.OpenCode.Split(',');
                if(arr.Length>1)
                {
                    for(int r=0;r<arr.Length;r++)
                    {
                        arr[r] = arr[r].PadLeft(2, '0');
                    }
                    ed.OpenCode = string.Join(",",arr);
                }
                ed.OpenTime = DateTime.Parse(dt.Rows[i]["OpenTime"].ToString());
                if (dt.Columns.Contains("EId"))
                {
                    ed.EId = int.Parse(dt.Rows[i]["EId"].ToString());
                    ed.MissedCnt = int.Parse(dt.Rows[i]["MissedCnt"].ToString());
                    ed.LastExpect = dt.Rows[i]["LastExpect"].ToString();
                }
                MyData.Add(ed);
            }
            Readed = false;
        }

        public ExpectList<T> getSubArray(int FromIndex, int len)
        {
            DataTable dt = null;
            ExpectList<T> ret = new ExpectList<T>(dt);
            if (MyData.Count < FromIndex + len)
                throw new Exception("选区的子段超出母数据长度！");
            for (int i = FromIndex; i < FromIndex + len; i++)
            {
                ret.Add(MyData[i]);
            }
            return ret;
        }

        DataTable _table;
        public DataTable Table
        {
            get
            {
                if (_table == null)
                {
                    _table = new DataTable();
                    _table.Columns.Add("Expect", typeof(string));
                    _table.Columns.Add("OpenCode", typeof(string));
                    _table.Columns.Add("OpenTime", typeof(DateTime));
                }
                lock (_table)
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        DataRow dr = _table.NewRow();
                        dr[0] = this[i].Expect;
                        dr[1] = this[i].OpenCode;
                        dr[2] = this[i].OpenTime;
                        _table.Rows.Add(dr);
                    }
                }
                return _table;
            }
        }
        public ExpectList<T> FirstDatas(int RecLng)
        {
            Dictionary<string, MongoReturnDataList<T>> ret = new Dictionary<string, MongoReturnDataList<T>>();
            int cnt = Data.Select(a => a.Value.Count).Min();
            if (RecLng > cnt)
                throw new Exception("请求长度超出目标列表长度！");
            foreach(string key in Data.Keys)
            {
                ret.Add(key, Data[key].GetFirstData(RecLng));
            }
            return new ExpectList<T>(ret,false);
        }

        public ExpectList<T> LastDatas(int RecLng,bool ResortTime)
        {
            Dictionary<string, MongoReturnDataList<T>> ret = new Dictionary<string, MongoReturnDataList<T>>();
            int cnt = Data.Select(a => a.Value.Count).Min();
            if (RecLng > cnt)
                throw new Exception("请求长度超出目标列表长度！");
            foreach (string key in Data.Keys)
            {
                ret.Add(key, Data[key].GetLastData(RecLng));
            }
            return new ExpectList<T>(ret, ResortTime);
        }

        public int IndexOf(string ExpectNo)
        {
            long ret = long.Parse(ExpectNo);
            long begid = long.Parse(this.FirstData.Expect);
            long endid = long.Parse(this.LastData.Expect);
            if (ret > endid || ret < begid)
                return -1;//throw new Exception("指定的期号不在列表中");
            if (endid - begid + 1 == this.Count)
            {
                return (int)(ret - begid);
            }
            int shiftid = Math.Max((int)(ret - begid) / 2, 0);
            while (shiftid >= 0)
            {
                int i = shiftid;
                while (i < this.Count)
                {
                    if (this[i].Expect == ExpectNo)
                        return i;
                    i++;
                }
                if (i == this.Count)
                    shiftid = Math.Max(shiftid / 2, 0);

            }
            return -1;
        }

        public static ExpectList<T> Concat(ExpectList<T> descList, params ExpectList<T>[] addList)
        {
            ExpectList<T> ret = new ExpectList<T>();
            if (descList == null)
                descList = new ExpectList<T>();
            for(int i=0;i<descList.Count;i++)
            {
                ret.Add(descList[i]);
            }
            if (ret == null) ret = new ExpectList<T>();
            for (int i = 0; i < addList.Length; i++)
            {
                if (addList[i] == null) continue;
                for (int j = 0; j < addList[i].Count; j++)
                {
                    ret.Add(addList[i][j]);
                }
            }
            return ret;
        }
        protected Dictionary<string, Dictionary<string, OneCycleData>> Tables;

        public ExpectList(string DataType, Cycle cyc, DataSet ds)
        {
            Tables = new Dictionary<string, Dictionary<string, OneCycleData>>();
            long MaxCnt = 0;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                long cnt = ds.Tables[i].Rows.Count;
                if (cnt > MaxCnt)
                {
                    MaxCnt = cnt;
                }
            }

            for (int n = 0; n < ds.Tables.Count; n++)
            {
                OneCycleData ocd = new OneCycleData();
                List<OneCycleData> list = ocd.FillByTable<OneCycleData>(ds.Tables[n]);
                Tables.Add(ds.Tables[n].TableName, list.ToDictionary(p => p.date, p => p));
            }
            for (int i = 0; i < MaxCnt; i++)
            {
                ExpectData<T> ed = new ExpectData<T>();
                //ed.ListData

            }

        }

        public int IndexOf(ExpectData<T> item)
        {
            return MyData.IndexOf(item);
        }

        
        public void Insert(int index, ExpectData<T> item)
        {
            MyData.Insert(index, item);
            Readed = false;
        }

        public void RemoveAt(int index)
        {
            MyData.RemoveAt(index);
            Readed = false;
        }

        public ExpectData<T> this[int index]
        {
            get
            {
                return MyData[index];
            }
            set
            {
                if (value != null)
                {
                    MyData[index] = (ExpectData<T>)value.Clone();
                }
                else
                {
                    MyData[index] = null;
                }
            }
        }

        public void Add(ExpectData<T> item)
        {
            MyData.Add(item);
            Readed = false;
        }

        public void Clear()
        {
            MyData.Clear();
            Readed = false;
        }

        public bool Contains(ExpectData<T> item)
        {
            return MyData.Contains(item);
        }

        public void CopyTo(ExpectData<T>[] array, int arrayIndex)
        {
            MyData.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return MyData.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(ExpectData<T> item)
        {
            Readed = false;
            return MyData.Remove(item);
            
        }

        public IEnumerator<ExpectData<T>> GetEnumerator()
        {
            return MyData.GetEnumerator();
        }

        
    }

    

    #region 废弃类
    /*
    public class ExpectList : IList<ExpectData>, IExpectList<MongoData>
    {
        public DataTypePoint UseType;
        public Cycle Cyc = Cycle.Expect;
        List<ExpectData> _MyData;
        Dictionary<string, Dictionary<string,OneCycleData>> Tables;
        List<ExpectData> MyData
        {
            get
            {
                if (_MyData == null)
                    throw new Exception("对象为空");
                return _MyData;
            }
        }

        public ExpectData LastData
        {
            get
            {
                //////if (MyData.Count == 0)
                //////{
                //////    string test = "1";
                //////    test = "1";
                //////}
                return MyData[MyData.Count - 1];
            }
        }

        public ExpectData FirstData
        {
            get { return MyData[0]; }
        }

        public ExpectList<MongoData> LastDatas(int RecLng)
        {
            ExpectList ret = new ExpectList();
            if (RecLng == this.Count) return this;
            if (RecLng > this.Count)
                throw new Exception("请求长度超出目标列表长度！");
            for (int i = this.Count - RecLng; i < this.Count; i++)
            {
                ret.Add(this[i].Clone() as ExpectData);
            }
            return ret;
        }

        public ExpectList<MongoData> FirstDatas(int RecLng)
        {
            ExpectList ret = new ExpectList();
            if (RecLng == this.Count) return this;
            if (RecLng > this.Count)
                throw new Exception("请求长度超出目标列表长度！");
            for (int i = 0; i < RecLng - 1; i++)
            {
                ret.Add(this[i].Clone() as ExpectData);
            }
            return ret;
        }

        public ExpectList()
        {
            _MyData = new List<ExpectData>();
        }
        public ExpectList(DataTable dt)
        {
            _MyData = new List<ExpectData>();
            if (dt == null) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ExpectData ed = new ExpectData();
                ed.Expect = dt.Rows[i]["Expect"].ToString();
                ed.OpenCode = dt.Rows[i]["OpenCode"].ToString();
                ed.OpenTime = DateTime.Parse(dt.Rows[i]["OpenTime"].ToString());
                if (dt.Columns.Contains("EId"))
                {
                    ed.EId = int.Parse(dt.Rows[i]["EId"].ToString());
                    ed.MissedCnt = int.Parse(dt.Rows[i]["MissedCnt"].ToString());
                    ed.LastExpect = dt.Rows[i]["LastExpect"].ToString();
                }
                MyData.Add(ed);
            }
        }

        public ExpectList(string DataType,Cycle cyc, DataSet ds)
        {
            Tables = new Dictionary<string, Dictionary<string,OneCycleData>>();
            long MaxCnt = 0;
            for(int i=0;i<ds.Tables.Count;i++)
            {
                long cnt = ds.Tables[i].Rows.Count;
                if(cnt>MaxCnt)
                {
                    MaxCnt = cnt;
                }
            }
            
            for (int n = 0; n < ds.Tables.Count; n++)
            {
                OneCycleData ocd = new OneCycleData();
                List<OneCycleData>  list = ocd.FillByTable<OneCycleData>(ds.Tables[n]);
                Tables.Add(ds.Tables[n].TableName, list.ToDictionary(p=>p.date,p=>p));
            }
            for (int i = 0; i < MaxCnt; i++)
            {
                ExpectData ed = new ExpectData();
                //ed.ListData

            }
            
        }

        public int IndexOf(ExpectData item)
        {
            return MyData.IndexOf(item);
        }

        public int IndexOf(string ExpectNo)
        {
            long ret = long.Parse(ExpectNo);
            long begid = long.Parse(this.FirstData.Expect);
            long endid = long.Parse(this.LastData.Expect);
            if (ret > endid || ret < begid)
                return -1;//throw new Exception("指定的期号不在列表中");
            if (endid - begid + 1 == this.Count)
            {
                return (int)(ret - begid);
            }
            int shiftid = Math.Max((int)(ret - begid) / 2, 0);
            while (shiftid >= 0)
            {
                int i = shiftid;
                while (i < this.Count)
                {
                    if (this[i].Expect == ExpectNo)
                        return i;
                    i++;
                }
                if (i == this.Count)
                    shiftid = Math.Max(shiftid / 2, 0);

            }
            return -1;
        }

        public void Insert(int index, ExpectData item)
        {
            MyData.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            MyData.RemoveAt(index);
        }

        public ExpectData<MongoData> this[int index]
        {
            get
            {
                return MyData[index];
            }
            set
            {
                if (value != null)
                {
                    MyData[index] = (ExpectData)value.Clone();
                }
                else
                {
                    MyData[index] = null;
                }
            }
        }

        public void Add(ExpectData item)
        {
            MyData.Add(item);
        }

        public void Clear()
        {
            MyData.Clear();
        }

        public bool Contains(ExpectData item)
        {
            return MyData.Contains(item);
        }

        public void CopyTo(ExpectData[] array, int arrayIndex)
        {
            MyData.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return MyData.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(ExpectData item)
        {
            return MyData.Remove(item);
        }

        public IEnumerator<ExpectData> GetEnumerator()
        {
            return MyData.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public static ExpectList<MongoData> Concat(ExpectList<MongoData> descList, params ExpectList<MongoData>[] addList)
        {
            ExpectList<MongoData> ret = descList;
            if (ret == null) ret = new ExpectList<MongoData>();
            for (int i = 0; i < addList.Length; i++)
            {
                if (addList[i] == null) continue;
                for (int j = 0; j < addList[i].Count; j++)
                {
                    ret.Add((ExpectData<MongoData>)addList[i][j].Clone());
                }
            }
            return ret;
        }

        public ExpectList getSubArray(int FromIndex, int len)
        {
            ExpectList ret = new ExpectList();
            if (MyData.Count < FromIndex + len)
                throw new Exception("选区的子段超出母数据长度！");
            for (int i = FromIndex; i < FromIndex + len; i++)
            {
                ret.Add(MyData[i]);
            }
            return ret;
        }

        DataTable _table;
        public DataTable Table
        {
            get
            {
                if (_table == null)
                {
                    _table = new DataTable();
                    _table.Columns.Add("Expect", typeof(string));
                    _table.Columns.Add("OpenCode", typeof(string));
                    _table.Columns.Add("OpenTime", typeof(DateTime));
                }
                for (int i = 0; i < this.Count; i++)
                {
                    DataRow dr = _table.NewRow();
                    dr[0] = this[i].Expect;
                    dr[1] = this[i].OpenCode;
                    dr[2] = this[i].OpenTime;
                    _table.Rows.Add(dr);
                }
                return _table;
            }
        }
    }
    */
    #endregion
}

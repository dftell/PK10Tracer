using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.BaseObjectsLib
{
    public class ExpectList : IList<ExpectData>
    {
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

        public ExpectList LastDatas(int RecLng)
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

        public ExpectList FirstDatas(int RecLng)
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

        public ExpectData this[int index]
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

        public static ExpectList Concat(ExpectList descList, params ExpectList[] addList)
        {
            ExpectList ret = descList;
            if (ret == null) ret = new ExpectList();
            for (int i = 0; i < addList.Length; i++)
            {
                if (addList[i] == null) continue;
                for (int j = 0; j < addList[i].Count; j++)
                {
                    ret.Add((ExpectData)addList[i][j].Clone());
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

}

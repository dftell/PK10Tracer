using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.GuideLib
{
    public class GuideResult :DataTableEx,IDictionary<string,double>
    {
        Dictionary<string, double> valDic = new Dictionary<string, double>();
        public GuideResult()
        {
        }

        public GuideResult(string TabName)
        {
            this.TableName = TabName;
        }

        public GuideResult(DataTable dt)
        {
            this.Columns.Clear();
            this.Rows.Clear();
            if (dt == null) return;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                this.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
            }
            for(int i=0;i<dt.Rows.Count;i++)
            {
                DataRow dr = this.NewRow();
                dr.ItemArray = dt.Rows[i].ItemArray;
                this.Rows.Add(dt.Rows[i].ItemArray);
            }
        }

        public List<double> getColumnData()
        {
            List<double> ret = new List<double>();
            this.getColumnData("val", ref ret);
            return ret;
        }

        public double High
        {
            get
            {
                return this.Select().Max(c => double.Parse(c["val"].ToString()));
            }
        }
        public double Low
        {
            get
            {
                return this.Select().Min(c => double.Parse(c["val"].ToString()));
            }
        }

        public void Fill(int rowid, double val)
        {
            this.Rows[rowid]["val"] = val.ToString();
        }

        public void Add(string key, double value)
        {
            Int64 preId = -1;
            Int64 index = Int64.Parse(key);
            if (this.ContainsKey(key))
            {
                throw new Exception("存在相同期号数据！");
            }
            DataRow dr = this.getRowByKeyAndVal("Expect", index-1);
            if (dr != null)
            {
                preId = Int64.Parse(dr["Id"].ToString());
            }
            DataRow newDr = this.NewRow();
            newDr["Id"] = preId + 1;
            newDr["Expect"] = index - 1;
            newDr["val"] = value;
        }

        public bool ContainsKey(string key)
        {
            DataRow[] drs = this.Select(string.Format("Expect={0}", key));
            if (drs.Length == 0)
            {
                return false;
            }
            else if (drs.Length > 1)
            {
                throw new Exception("同一期号多条数据");
            }
            return true;
        }

        public ICollection<string> Keys
        {
            get {
                List<string> expectlist = new List<string>();
                this.getColumnData("Expect",ref expectlist);
                return expectlist;
            }
        }

        public bool Remove(string key)
        {
            return false;
        }

        public bool TryGetValue(string key, out double value)
        {
            value = double.NaN;
            DataRow dr = this.getRowByKeyAndVal("Expect", key);
            if (dr == null)
                return false;
            value = double.Parse(dr["val"].ToString());
            return true;
        }

        public ICollection<double> Values
        {
            get {
                List<double> expectlist = new List<double>();
                this.getColumnData("Expect", ref expectlist);
                return expectlist;
            }
        }

        public double this[string key]
        {
            
            get
            {
                if(valDic == null) 
                    valDic = new Dictionary<string,double>();
                if(valDic.ContainsKey(key))
                {
                    return valDic[key];
                }
                if (!this.ContainsKey(key))
                {
                    throw new Exception("不存在的期号数据");
                }
                else
                {
                    double outret = 0;
                    if (this.TryGetValue(key, out outret))
                    {
                        if (!valDic.ContainsKey(key))
                            valDic.Add(key, outret);
                    }
                }
                if (valDic.ContainsKey(key))
                {
                    return valDic[key];
                }
                return double.NaN;
            }
            set
            {
                if (valDic != null)
                {
                    DataRow dr = this.getRowByKeyAndVal("Expect", key);
                    if(this.setRowByKeyAndVal(dr, "val", value.ToString()))
                    {
                        if (valDic.ContainsKey(key))
                        {
                            valDic[key] = value;
                        }
                        else
                        {
                            valDic.Add(key, value);
                        }
                    }
                    
                }
            }
        }

        public void Add(KeyValuePair<string, double> item)
        {
            this.Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<string, double> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, double>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return this.Rows.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<string, double> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace PK10CorePress
{
    public class DataTableEx:DataTable
    {
        public bool getColumnData(int colindex,ref List<int> retList)
        {
            retList = new List<int>();
            if (this.Columns.Count > colindex + 1 || this.Columns.Count < 1)
            {
                throw new Exception("选择的整数列不存在！");
            }
            try
            {
                for (int i = 0; i < this.Rows.Count; i++)
                {
                    retList.Add(int.Parse(this.Rows[i][colindex].ToString()));
                }
            }
            catch (Exception ce)
            {
                throw ce;
            }
            return true;
        }

        public bool getColumnData(string colname, ref List<int> retList)
        {
            retList = new List<int>();
            if (!this.Columns.Contains(colname))
            {
                throw new Exception("选择的整数列不存在！");
            }
            try
            {
                for (int i = 0; i < this.Rows.Count; i++)
                {
                    retList.Add(int.Parse(this.Rows[i][colname].ToString()));
                }
            }
            catch (Exception ce)
            {
                throw ce;
            }
            return true;
        }

        public bool getColumnData(int colindex, ref List<double> retList)
        {
            retList = new List<double>();
            if (this.Columns.Count > colindex + 1 || this.Columns.Count < 1)
            {
                throw new Exception("选择的小数列不存在！");
            }
            try
            {
                for (int i = 0; i < this.Rows.Count; i++)
                {
                    retList.Add(double.Parse(this.Rows[i][colindex].ToString()));
                }
            }
            catch (Exception ce)
            {
                throw ce;
            }
            return true;
        }

        public bool getColumnData(string colname, ref List<double> retList)
        {
            retList = new List<double>();
            if (!this.Columns.Contains(colname))
            {
                throw new Exception("选择的小数列不存在！");
            }
            try
            {
                for (int i = 0; i < this.Rows.Count; i++)
                {
                    retList.Add(double.Parse(this.Rows[i][colname].ToString()));
                }
            }
            catch (Exception ce)
            {
                throw ce;
            }
            return true;
        }

        public bool getColumnData(int colindex, ref List<string> retList)
        {
            retList = new List<string>();
            if (this.Columns.Count > colindex + 1 || this.Columns.Count < 1)
            {
                throw new Exception("选择的字符串列不存在！");
            }
            try
            {
                for (int i = 0; i < this.Rows.Count; i++)
                {
                    retList.Add(this.Rows[i][colindex].ToString());
                }
            }
            catch (Exception ce)
            {
                throw ce;
            }
            return true;
        }

        public bool getColumnData(string colname, ref List<string> retList)
        {
            retList = new List<string>();
            if (!this.Columns.Contains(colname))
            {
                throw new Exception("选择的字符串列不存在！");
            }
            try
            {
                for (int i = 0; i < this.Rows.Count; i++)
                {
                    retList.Add(this.Rows[i][colname].ToString());
                }
            }
            catch (Exception ce)
            {
                throw ce;
            }
            return true;
        }

        public DataRow getRowByKeyAndVal(string key, object val)
        {
            string sql = string.Format("{0}={1}", key, val);
            DataRow[] drs = this.Select(sql);
            if(drs.Length > 1)
            {
                throw new Exception("指定条件非唯一！");
            }
            if (drs.Length == 0) return null;
            return drs[0];
        }

        public bool setRowByKeyAndVal(DataRow dr,string key, string val)
        {
            if (dr == null) return false;
            if (dr.Table.Equals(this)) return false;
            dr[key] = val;
            return true;
        }

        public new DataTableEx Copy()
        {
            return (DataTableEx)base.Copy();
        }
    }
}

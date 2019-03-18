using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data ;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
namespace CFZQ_LHProcess
{
    #region Matrix class
    public interface iFillable
    {
        void FillByDataRow(DataRow dr);
        DataRow FillRow(DataRow dr);
        
    }

    public interface iListFillable
    {
        void FillByItems<T>(T[] item);
        T getObjectById<T>(int id);
    }

    public class MatrixTableCell<T>:IQueryable<T>
    {
        public int RowId{get;set;}
        public string ColumnName { get; set; }
        public T Value { get; set; }

        #region IEnumerable<T> 成员

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IQueryable 成员

        public Type ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public Expression Expression
        {
            get { throw new NotImplementedException(); }
        }

        public IQueryProvider Provider
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class MatrixTableRows<T>
    {
        public  MatrixTableRows()
        {
        }

        MatrixTableCell<T> _Cell;
        protected Dictionary<string, MatrixTableCell<T>> List;

        public MatrixTableCell<T> this[string colname]
        {
            get
            {
                return List[colname]; 
            }
            set
            {
                List[colname] = value;
            }
        }
    }

    #endregion



    /// <summary>
    /// 矩阵表类
    /// </summary>
    /// <remarks>
    /// zhouys,2017/7/1
    /// </remarks>
    public class MTable:IQueryable,iListFillable,IEnumerable<iFillable>
    {
        protected List<Dictionary<string, object>> tList;
        protected DataTable tTable;
        Dictionary<string, int> tTColumns;
        bool NewAddColumns;
        bool TableUpdateFlag;
        protected Type ItemType;
        public MTable()
        {
            tList = new List<Dictionary<string, object>>();
            tTable = new DataTable();
            tTColumns = new Dictionary<string, int>();  
        }

        public MTable(DataTable dt):this()
        {
            tTable = dt.Copy();
        }

        /// <summary>
        /// 初始化表结构，将重新格式化表，所有数据将被清除
        /// </summary>
        /// <param name="colsinfo"></param>
        public void InitTableStructure(params object[] colsinfo)
        {
            Dictionary<string,Type> dics =  WDDataAdapter.getColumnTypes(colsinfo);
            Table = new DataTable();
            foreach (string key in dics.Keys)
            {
                Table.Columns.Add(key, dics[key]);
            }
        }

        public void Contact(MTable tb)
        {
            for(int i=0;i<tb.Count;i++)
            {
               DataRow dr = this.Table.NewRow();
               dr.ItemArray = tb.Table.Rows[i].ItemArray;
            }
        }

        public void Union(MTable tb)
        {
            if (tb.Count>0 && this.Count>0 && tb.Count != this.Count)
            {
                throw (new Exception("联合的表行数需要一致！"));
            }
            for (int i = 0; i < tb.GetTable().Columns.Count; i++)
            {
                DataColumn dc = tb.GetTable().Columns[i];
                if (!this.Table.Columns.Contains(dc.ColumnName))
                {
                    Type t = dc.DataType;
                    this.AddColumnByArray(dc.ColumnName,tb,dc.ColumnName);
                }
            }
        }

        protected DataTable Table
        {
            get
            {
                if (!TableUpdateFlag) return tTable;
                tTable = new DataTable();
                tTable.Columns.Add(new DataColumn("thisRowIndex"));
                tTable.Columns.Add(new DataColumn("thisRow"));
                foreach (string key in TColumns.Keys)
                {
                    tTable.Columns.Add(new DataColumn(key));
                }
                for (int i = 0; i < tList.Count ; i++)
                {
                    DataRow dr = tTable.NewRow();
                    Dictionary<string,object> dic = tList[i];
                    dr["thisRowIndex"] = i;
                    dr["thisRowIndex"] = dic;
                    foreach (string key in dic.Keys)
                    {
                        dr[key] = dic[key];
                    }
                    tTable.Rows.Add(dr);
                }
                TableUpdateFlag = false;
                return tTable;
            }
            set
            {
                if (value == null)
                {
                    this.tTable = null;
                    return;
                }
                tTable = value.Clone();
                FillList(value);
            }
        }

        public void UpdateColumnType(string strcol, Type type)
        {

        }

        public DataTable GetTable()
        {
            return Table;
        }

        public void FillTable(DataTable dt)
        {
            this.tTable = dt;
        }

        public void FillList(DataRow[] Drs)
        {
            this.tTable.Clear();
            DataTable dt = null;
            if (Drs.Length == 0)
            {
                return;   
            }
            dt = Drs[0].Table;
            this.tTable = dt.Clone();
            for (int i = 0; i < Drs.Length; i++)
            {
                DataRow dr = this.tTable.NewRow();
                dr.ItemArray = Drs[i].ItemArray;
                this.tTable.Rows.Add(dr);
            }
        }

        void FillList(DataTable dt)
        {
            FillList(dt.Select());
        }

        protected Dictionary<string, int> TColumns
        {
            get
            {
                if (!NewAddColumns) return tTColumns;
                NewAddColumns = false;
                return tTColumns;
            }
        }

        void AddNewColumn(string[] columns)
        {
            for (int i = 0; i < columns.Length ; i++)
            {
                string strcolname = columns[i];
                if(tTColumns.ContainsKey(strcolname)) continue;
                tTColumns.Add(strcolname,tTColumns.Count);
                NewAddColumns = true;
            }
            return;
        }

        public object this[int index, string colname]
        {
            get
            {
                TableUpdateFlag = false;
                if (tList[index] != null && tList[index].ContainsKey(colname))
                    return tList[index][colname];
                return null;
            }
            set
            {
                if (index > tList.Count - 1)
                {
                    while (tList.Count < index + 1)//补齐行
                    {
                        tList.Add(new Dictionary<string, object>());
                        TableUpdateFlag = true;
                    }
                }
                if (!tList[index].ContainsKey(colname))
                {
                    AddNewColumn(new string[]{colname});
                    this.Table.Columns.Add(colname, value.GetType());
                    tList[index].Add(colname, null);
                    TableUpdateFlag = true;
                }
                tList[index][colname] = value;
                TableUpdateFlag = true;
            }
        }

        public MTable this[string indexRange, string colnames]
        {
            get
            {
                List<int> SelectRows = getRowsByStr(indexRange);
                string SelectColumns = getColumnsByStr(colnames) ;
                return getCrossTable(SelectRows, SelectColumns);
            }
        }

        public virtual iFillable this[int index]
        {
             get
            {
                throw (new Exception("未实现方法！"));
                //return this[index.ToString() ,":"];
            }
        }

        
        public MTable this[string colnames]
        {
            get
            {
                return this[":", colnames];
            }
            set
            {
                if (value != null)
                {

                }
            }
        }

        public MTable this[params string[] colnames]
        {
            get
            {
                return this[":", string.Join(",",colnames)];
            }
            set
            {
                if (value != null)
                {

                }
            }
        }

        protected MTable getCrossTable(List<int> SelectRows, string SelectCols)
        {
            MTable ret = null;
            string[] ColArr = SelectCols.Trim().Split(',');
            if (SelectCols.Trim() == "*")
                ColArr = this.TColumns.Keys.ToArray<string>();
            ret = getTableByRC(SelectRows.ToArray<int>(), ColArr);
            return ret;
        }
        
        protected MTable getTableByRC(int[] rows,string[] selectCols)
        {
            MTable ret = new MTable();
            for (int i = 0; i < selectCols.Length; i++)
            {
                if(!this.Table.Columns.Contains(selectCols[i]))
                {
                    throw (new Exception("不存在该列！"));
                }

                ret.Table.Columns.Add(selectCols[i], this.Table.Columns[selectCols[i]].DataType);
            }
            if (this.tTable == null) return null;
            for (int i = 0; i < rows.Length ; i++)
            {
                if (i >= this.Table.Rows.Count) 
                {
                    throw new Exception("选出的行号越界！"); 
                }
                DataRow dr = ret.tTable.NewRow();
                for (int j = 0; j < selectCols.Length ; j++)
                {
                    dr[selectCols[j]] = this.Table.Rows[i][selectCols[j]];
                }
                ret.Table.Rows.Add(dr);
            }
            return ret;
        }

        #region 获得行列范围
        List<int> getRowsByStr(string indexRange)
        {
            List<int> SelectRows = new List<int>();
            if (indexRange == null || indexRange.Trim().Length == 0 || indexRange.Trim() == ":")//如果行号为空或者冒号，全选所有行
            {
                return getSerList(0, this.tTable.Rows.Count );
            }
            else
            {
                string[] arr = indexRange.Trim().Split(':');
                if (arr.Length == 1)//单纯数字
                {
                    string strNum = arr[0].Trim();
                    int iRow = -1;
                    if (!int.TryParse(strNum, out iRow) || iRow < 0)// 如果行输入非零及正数字
                    {
                        throw new Exception("行号必须为0或正数！");
                        return SelectRows;
                    }
        
                    return SelectRows;
                }
                else//以冒号分割行号
                {
                    string strBeg = arr[0].Trim();
                    string strEnd = arr[1].Trim();
                    int iBeg = 0;
                    int iEnd = tList.Count;
                    if (strBeg.Length > 0 && (!int.TryParse(strBeg, out iBeg) || iBeg < 0))
                    {
                        throw new Exception("开始行号必须为0或正数！");
                    }
                    if (strEnd.Length > 0 && (!int.TryParse(strEnd, out iEnd) || iEnd < 0))
                    {
                        throw new Exception("结束行号必须为0或正数！");
                    }
                    if (arr.Length > 2)//如果出现多个冒号
                    {
                        throw new Exception("行号间不能多于一个冒号！");
                        return SelectRows;
                    }

                    if (strBeg == "")
                    {
                        return getSerList(0, iEnd);
                    }
                    if (strEnd == "")
                    {
                        return getSerList(iBeg, tTable.Rows.Count);
                    }
                    return getSerList(iBeg, iEnd);
                }
            }
            //return SelectRows;
        }

        
        string getColumnsByStr(string indexRange)
        {
            string SelectCols = "";
            if (tList == null) return SelectCols;
            if (indexRange == null || indexRange.Trim().Length == 0 || indexRange.Trim() == ":" || indexRange.Trim() == "*")//如果行号为空或者冒号，全选所有列
            {
                return "*";
            }
            else
            {
                return indexRange;
            }
            //return SelectCols;
        }


        List<int> getSerList(int From, int To)
        {
            List<int> ret = new List<int>();
            for (int i = From; i < To; i++) ret.Add(i);
            return ret;
        }
        #endregion

        #region 接口成员

        #region IQueryable 成员

        public Type ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public Expression Expression
        {
            get { throw new NotImplementedException(); }
        }

        public IQueryProvider Provider
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEnumerable 成员

        public System.Collections.IEnumerator GetEnumerator()
        {
            
            throw new NotImplementedException();
        }

        List<iFillable> _IEnumerators;

        private List<iFillable> IEnumerators()
        {
            if (_IEnumerators == null)
            {
                _IEnumerators = new List<iFillable>();
                if (!typeof(iListFillable).IsAssignableFrom(this.GetType()))
                {
                    throw (new Exception("非可填充类型！"));
                }
                Type t = this.GetType();
                if (this.tTable.Rows.Count == 0) return _IEnumerators;
                for (int i = 0; i < this.tTable.Rows.Count; i++)
                {
                    object obj = this[i];
                    if (typeof(iFillable).IsAssignableFrom(obj.GetType()))
                    {
                        iFillable obj1 = (iFillable)this[i];
                        _IEnumerators.Add(obj1);
                    }
                }

            }
            return _IEnumerators;

        }
        ////IEnumerator<T> GetEnumerator<T>()
        ////{
        ////    //IEnumerators = new List<iFillable>();

        ////    for (int i = 0; i < this.tTable.Rows.Count; i++)
        ////    {
        ////        yield return (T)IEnumerators()[i] ;
        ////    }
        ////}


        public T getObjectById<T>(int id)
        {
            if (id < 0 || id >= this.tTable.Rows.Count) return default(T);
            //Type t= typeof();//iFillablTe
            Type t = typeof(T);
            object obj = t.Assembly.CreateInstance(t.FullName);
            (obj as iFillable).FillRow(this.tTable.Rows[id]);
            return (T)obj;
        }

        IEnumerator<iFillable> IEnumerable<iFillable>.GetEnumerator()
        {
            for (int i = 0; i < this.tTable.Rows.Count; i++)
            {
                yield return IEnumerators()[i];
            }
        }

        #endregion
        #endregion

        #region 属性
        public int Count
        {
            get { return this.tTable.Rows.Count; }
        }
        #endregion

        /// <summary>
        /// 导出为List,该函数只能导出单列MTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ToList<T>()
        {
            if (this.tTable == null)
                this.tTable = new DataTable();
            if (this.tTable.Columns.Count != 1)
                throw new Exception("该函数只能导出数组类型的MTable");
            string key = this.Table.Columns[0].ColumnName;//
            List<T> list = new List<T>();
            for (int i = 0; i < this.Table.Rows.Count; i++)
            {
                list.Add((T)Convert.ChangeType(Table.Rows[i][key],typeof(T)));
            }
            return list;
        }

        public List<T> ToList<T>(string colname)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < this.tTable.Rows.Count; i++)
            {
                list.Add((T)tTable.Rows[i][colname]);
            }
            return list;
        }

        public List<T> ToList<T>(string rows,string colname)
        {
            if (rows == null || rows.Trim().Length == 0 || rows.Trim() == "*" || rows.Trim() == ":")
            {
                return ToList<T>(colname);
            }
            MTable ret = this[rows, colname];
            return ret.ToList<T>(colname);
            
        }

        public string ToRowData(int rowid)
        {
            List<string> sb = new List<string> ();
            for (int i = 0; i < this.Table.Columns.Count; i++)
            {
                sb.Add(string.Format("{0}:{1}",this.Table.Columns[i].ColumnName,this.Table.Rows[rowid][this.Table.Columns[i].ColumnName].ToString()));
            }
            return string.Join(";",sb.ToArray());
        }

        public List<T> ToFillableList<T>()
        {
            if (typeof(iFillable).IsAssignableFrom(typeof(T)))
            {
                string test;
            }
            else
                return null;
            List<T> ret = new List<T>();
            for (int i = 0; i < this.tTable.Rows.Count; i++)
            {
                Type t = typeof(T);
                iFillable obj = t.Assembly.CreateInstance(t.FullName) as iFillable;
                obj.FillByDataRow(this.tTable.Rows[i]);
                ret.Add((T)obj);
            }
            return ret;
        }

        public void AddColumnByArray(string ColumnName,MTable tb,string col)
        {
            Type t = tb.GetTable().Columns[col].DataType;
            if (Table == null)
            {
                Table = new DataTable();
            }
            if (this.Count > 0 && tb.Count > 0 && this.Count != tb.Count)
            {
                throw (new Exception("插入新列元素数量与现有元素数量不匹配！"));
            }
            if (!this.Table.Columns.Contains(ColumnName))
            {
                this.Table.Columns.Add(ColumnName, t);
            }
            DataTable dt = tb.GetTable();
            for (int i = 0; i < dt.Rows.Count ; i++) 
            {
                DataRow dr = null;
                if(i<this.Table.Rows.Count)
                    dr = this.Table.Rows[i];
                if (dr == null)
                {
                    dr = this.Table.NewRow();
                    this.Table.Rows.Add(dr);
                }
                if (dt.Rows[i].IsNull(col))
                    continue;
                dr[ColumnName] = Convert.ChangeType(dt.Rows[i][col],t);
            }
        }

        public void AddColumnByArray<T>(string ColumnName, T val)
        {
            if (Table == null)
            {
                Table = new DataTable();
            }
            if (!this.Table.Columns.Contains(ColumnName))
            {
                this.Table.Columns.Add(ColumnName, typeof(T));
            }
            for (int i = 0; i < this.Table.Rows.Count - 1; i++)
            {
                DataRow dr = this.Table.Rows[i];
                dr[ColumnName] = val;
            }
        }

        public void AddColumnByArray<T>(string ColumnName, IEnumerable<T> vals)
        {
            if (vals.Count<T>() != Table.Rows.Count)
            {
                throw (new Exception("插入新列元素数量与现有元素数量不匹配！"));
            }
            if (Table == null)
            {
                Table = new DataTable();
            }
            
            if (!this.Table.Columns.Contains(ColumnName))
            {
                this.Table.Columns.Add(ColumnName, typeof(T));
            }
            int i = 0;
            foreach (T val in vals)
            {
                DataRow dr = this.Table.Rows[i];
                dr[ColumnName] = Convert.ChangeType(val,typeof(T));
                i++;
            }
        }

        public MTable Select(string sql)
        {
            if (this.Table == null) return null;
            MTable ret = new MTable();
            ret.Table = this.Table.Clone();
            if (sql == null || sql.Trim().Length == 0) 
                return ret;
            DataRow[] drs = this.Table.Select(sql);
            for (long i = 0; i < drs.Length; i++)
            {
                DataRow dr = ret.Table.NewRow();
                dr.ItemArray = drs[i].ItemArray;
                ret.Table.Rows.Add(drs[i].ItemArray);
            }
            return ret;
        }

        public void FillByItems<T>(T[] items)
        {
            if ( !typeof(iFillable).IsAssignableFrom(typeof(T)))
            {
                return;
            }
            this.tTable.Rows.Clear();
            for (int i = 0; i < items.Length; i++)
            {
                DataRow dr = this.tTable.NewRow();
                dr = (items[i] as iFillable).FillRow(dr);
                this.tTable.Rows.Add(dr);
            }
            return;
            
            
        }

        

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.tTable.Rows.Count; i++)
            {
                yield return IEnumerators()[i];
            }
        }
    }

    public interface ISelectable
    {
        DataTable Select(string sql);
    }
}

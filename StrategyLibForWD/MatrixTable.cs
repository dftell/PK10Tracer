using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data ;
using System.Data.Sql;
using System.Data.SqlClient;
namespace StrategyLibForWD
{
    #region Matrix class

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
    public class MTable:IQueryable
    {
        protected List<Dictionary<string, object>> tList;
        protected DataTable tTable;
        Dictionary<string, int> tTColumns;
        bool NewAddColumns;
        bool TableUpdateFlag;
        public MTable()
        {
            tList = new List<Dictionary<string, object>>();
            tTable = new DataTable();
            tTColumns = new Dictionary<string, int>();  
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
                for (int i = 0; i < tList.Count - 1; i++)
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
                tTable = value.Copy();
                FillList(value);
            }
        }
        public DataTable GetTable()
        {
            return Table;
        }

        public void FillTable(DataTable dt)
        {
            this.Table = dt;
        }

        void FillList(DataTable dt)
        {
            tList = new List<Dictionary<string, object>>();
            tTColumns = new Dictionary<string, int>();
            //加入列头
            string[] cols = new string[dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count - 1; i++) cols[i] = dt.Columns[i].ColumnName;
            AddNewColumn(cols);
            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {
                Dictionary<string, object> tItem = new Dictionary<string, object>();
                foreach(string key in tTColumns.Keys)
                {
                    tItem.Add(key, dt.Rows[i][key]);
                }
                tList.Add(tItem);
                //dt.Rows[i]["thisRowIndex"] = i;
                //dt.Rows[i]["thisRow"] = tItem;
            }
            TableUpdateFlag = true;
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
            for (int i = 0; i < columns.Length - 1; i++)
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
            if (this.Table == null) return null;
            for (int i = 0; i < rows.Length - 1; i++)
            {
                if (i >= this.Table.Rows.Count) { throw new Exception("选出的行号越界！"); }
                Dictionary<string, int> row = new Dictionary<string, int>();
                for (int j = 0; j < selectCols.Length - 1; j++)
                {
                    ret[i, selectCols[j]] = this.Table.Rows[i][selectCols[j]];
                }
            }
            return null;
        }
        #region 获得行列范围
        List<int> getRowsByStr(string indexRange)
        {
            List<int> SelectRows = new List<int>();
            if (tList == null) return SelectRows;
            if (indexRange == null || indexRange.Trim().Length == 0 || indexRange.Trim() == ":")//如果行号为空或者冒号，全选所有行
            {
                return getSerList(0, this.tList.Count - 1);
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
                    SelectRows.Clear();
                    SelectRows.Add(iRow);
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
                        return getSerList(iBeg, tList.Count);
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
            for (int i = From; i < To; i++) ret[i - From] = i;
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

        #endregion
        #endregion
    }
}

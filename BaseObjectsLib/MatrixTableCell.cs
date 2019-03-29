using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
namespace WolfInv.com.BaseObjectsLib
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

#endregion

    public interface ISelectable
    {
        DataTable Select(string sql);
    }
}

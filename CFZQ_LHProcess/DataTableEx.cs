using System;
using System.Collections.Generic;
using System.Data;


namespace CFZQ_LHProcess
{
    public class DataTableEx:DataTable,ISelectable
    {
        protected DataTable _dt;
           
        public DataTableEx(DataTable dt)
        {
            _dt = dt;
        }

        public DataTable Select(string sql)
        {
            if (_dt == null) return _dt;
            DataTable outdt = _dt.Clone();
            if (sql == null || sql.Trim().Length == 0) return outdt;
            DataRow[] drs = _dt.Select(sql);
            for (long i = 0; i < drs.Length; i++)
            {
                DataRow dr = outdt.NewRow();
               dr.ItemArray= drs[i].ItemArray; 
               outdt.Rows.Add(drs[i].ItemArray);
            }
            return outdt;
        }
    }
  
}

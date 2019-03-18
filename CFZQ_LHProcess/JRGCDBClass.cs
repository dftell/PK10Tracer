using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.Sql;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Common;
using System.Threading;
namespace WolfInv.com.CFZQ_LHProcess
{
    public class JRGCDBClass
    {
        string sConnectString ;
        //SqlConnection conn;
        OleDbConnection conn;
        public JRGCDBClass()
        {
            sConnectString = CFZQ_LHProcess.App.Default.ConnStr;
        }

        void Connect()
        {
            if(sConnectString == null || sConnectString.Length == 0 ) 
            {
                throw new Exception("无法获取连接字符串！");
            }
            if (conn == null) conn = new OleDbConnection();
            if (conn.State == ConnectionState.Closed) conn.ConnectionString = sConnectString;
            if (conn.State == ConnectionState.Closed) 
                conn.Open();
         }

        public void Close()
        {
            if (conn == null) return;
            if (conn.State == ConnectionState.Open) conn.Close();
        }

        public DataSet GetDataSet(string sql)
        {
            Connect();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            DataSet  ds = new DataSet();
            OleDbDataAdapter adp = new OleDbDataAdapter(cmd);
            adp.Fill(ds);
            adp = null;
            cmd = null;
            return ds;
        }

        public Int64 ExcelSql(string sql)
        {
            Connect();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            int ret =  cmd.ExecuteNonQuery();
            cmd = null;
            return ret;

        }

        public Int64 Save(DataTable dt, string sql)
        {
            int ret = -1;
            
                Connect();
                OleDbDataAdapter adp = new OleDbDataAdapter();
                adp.SelectCommand = new OleDbCommand(sql, conn);
                OleDbCommandBuilder bulider = new OleDbCommandBuilder(adp);
                //builder.QuotePrefix = "[";
                //builder.QuoteSuffix = "]";
                DataRow[] drs = dt.Select();
                adp.Fill(dt);
                ret = adp.Update(dt);
                
              
            return ret;
        }

        public Int64 Save(DataSet ds, string[] sqls)
        {
            long ret = 0;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable dt = ds.Tables[i];
                long plancnt = dt.Rows.Count;
                long updatecnt = Save(dt, sqls[i]);
                if (plancnt > updatecnt)
                {
                    throw (new Exception("保存数量不一致！"));
                }
                ret = ret + updatecnt;
            }
            return ret;
        }

        public Int64 BatchSave(DataSet ds, string[] sqls)
        {
            int ret = -1;

            Connect();
            OleDbCommand cmd = null;
            OleDbDataAdapter adp ;
            OleDbCommandBuilder bulider;
            //OleDbTransaction trans = conn.BeginTransaction();
            cmd = new OleDbCommand();
            cmd.Connection = conn;
            //cmd.Transaction = trans;
            string sql = "";
            DataTable dt = null;
            try
            {
                
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    adp = new OleDbDataAdapter(cmd);
                    bulider = new OleDbCommandBuilder(adp);
                    //builder.QuotePrefix = "[";
                    //builder.QuoteSuffix = "]";
                    int plancnt = ds.Tables[i].Rows.Count; 
                    adp.Fill(ds.Tables[i]);
                    dt = ds.Tables[i];
                    sql = sqls[i];
                    ret = adp.Update(ds.Tables[i]);
                    if (ret < plancnt)
                    {
                        Exception e = new Exception("实际保存记录数少于计划数量！");
                        throw(e);
                    }
                }
                //cmd.Transaction.Commit();
            }
            catch (Exception e)
            {
                //cmd.Transaction.Rollback();
                throw(e);
            }
            return ret;
        }
    }
}

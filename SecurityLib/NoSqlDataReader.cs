using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.DbAccessLib;
using System.Data;
namespace WolfInv.com.SecurityLib
{
    public class NoSqlDataReader : DataReader
    {
        public NoSqlDataReader(string DataTable,Cycle DataCycle)
        {
            this.strNewestTable = string.Format("{0}_{1}",DataTable,DataCycle);
            this.strHistoryTable = DataTable;
        }
        public override ExpectList GetMissedData(bool IsHistoryData, string strBegT)
        {
            throw new NotImplementedException();
        }

        public override ExpectList getNewestData(ExpectList NewestData, ExpectList ExistData)
        {
            throw new NotImplementedException();
        }

        public override DbChanceList getNoCloseChances(string strDataOwner)
        {
            throw new NotImplementedException();
        }

        public override ExpectList ReadHistory()
        {
            throw new NotImplementedException();
        }

        public override ExpectList ReadHistory(long buffs)
        {
            throw new NotImplementedException();
        }

        public override ExpectList ReadHistory(long From, long buffs)
        {
            throw new NotImplementedException();
        }

        public override ExpectList ReadHistory(long From, long buffs, bool desc)
        {
            throw new NotImplementedException();
        }

        public override ExpectList ReadHistory(string begt, string endt)
        {
            MongoDbClass db = GlobalClass.getCurrNoSQLDb();
            string sql = string.Format("select  * from {0}  where opentime between '{1}' and '{2}'", strHistoryTable, begt, endt);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public override ExpectList ReadNewestData(DateTime fromdate)
        {
            throw new NotImplementedException();
        }

        public override ExpectList ReadNewestData(int LastLng)
        {
            throw new NotImplementedException();
        }

        public override ExpectList ReadNewestData(int ExpectNo, int Cnt)
        {
            throw new NotImplementedException();
        }

        public override ExpectList ReadNewestData(int ExpectNo, int Cnt, bool FromHistoryTable)
        {
            throw new NotImplementedException();
        }

        public override int SaveChances(List<ChanceClass> list, string strDataOwner)
        {
            throw new NotImplementedException();
        }

        public override int SaveHistoryData(ExpectList InData)
        {
            throw new NotImplementedException();
        }

        public override int SaveNewestData(ExpectList InData)
        {
            throw new NotImplementedException();
        }
    }
}

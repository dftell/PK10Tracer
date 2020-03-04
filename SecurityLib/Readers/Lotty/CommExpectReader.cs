using System;
using System.Collections.Generic;
using System.Data;
using WolfInv.com.DbAccessLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    //Sql data reader
    public abstract class CommExpectReader : DataReader
    {
        
        
        public override ExpectList<T> ReadHistory<T>(long From, long buffs)
        {
            return ReadHistory<T>(From, buffs, false);
        }


        public override ExpectList<T> ReadHistory<T>(long From,long buffs,bool desc)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select top {0} * from {2} where expect>='{1}'  order by expect {3}", buffs, From,strHistoryTable,desc?"desc":"");//modify by zhouys 2019/1/8
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList<T>(ds.Tables[0]);
        }


        public override ExpectList<T> ReadHistory<T>(long buffs)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select top {0} * from {2}  order by expect desc", buffs,  strHistoryTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList<T>(ds.Tables[0]);
        }

        public override ExpectList<T> ReadHistory<T>()
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select  * from {0}  order by expect", strHistoryTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList<T>(ds.Tables[0]);
        }

        public override ExpectList<T> ReadHistory<T>(string begt,string endt)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select  * from {0}  where opentime between '{1}' and '{2}'", strHistoryTable,begt,endt);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList<T>(ds.Tables[0]);
        }

        public override ExpectList<T> ReadNewestData<T>(DateTime fromdate)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select * from {1} where opentime>='{0}' order by expect", fromdate.ToShortDateString(),strNewestTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList<T>(ds.Tables[0]);
        }

        public override ExpectList<T> ReadNewestData<T>(int LastLng)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select * from (select top {0} * from {1} order by expect desc) a order by expect", LastLng, strNewestTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList<T>(ds.Tables[0]);
        }

        public override ExpectList<T> ReadNewestData<T>(long ExpectNo, int Cnt)
        {
            return ReadNewestData<T>(ExpectNo, Cnt, false);
        }

        public override ExpectList<T> ReadNewestData<T>(long ExpectNo, int Cnt,bool FromHistoryTable)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select * from {2} where Expect<='{0}' and Expect>({0}-{1}) order by expect", ExpectNo, Cnt, FromHistoryTable?strHistoryTable:strNewestTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList<T>(ds.Tables[0]);
        }

        public override int SaveNewestData<T>(ExpectList<T> InData)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select top 0 * from {0}", strNewestTable);
            return db.SaveList(new ConditionSql(sql), InData.Table);
        }

        public override ExpectList<T> GetMissedData<T>(bool IsHistoryData,string strBegT)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select * from {1} where opentime>='{0}'", strBegT, IsHistoryData?strMissHistoryTable:strMissNewestTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList<T>(ds.Tables[0]);
        }


        public override int SaveHistoryData<T>(ExpectList<T> InData)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select top 0 * from {0}", strHistoryTable);
            return db.SaveList(new ConditionSql(sql), InData.Table);
        }


        public int SaveProbWaveResult(DataTable dt)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select * from {0}",strResultTable );
            return db.SaveNewList(new ConditionSql(sql), dt);
        }

        public DataTable GetProWaveResult(Int64 begid)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select * from {0} where Expect>='{1}' order by Expect", this.strResultTable, begid);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return ds.Tables[0];
        }

        public override ExpectList<T> getNewestData<T>(ExpectList<T> NewestData, ExpectList<T> ExistData)
        {
            DataTable dt = null;
            ExpectList<T> ret = new ExpectList<T>(dt);
            if (NewestData == null) return ret;
            if(ExistData == null)
            {
                ExistData = new ExpectList<T>();
            }
            HashSet<string> existDic = new HashSet<string>();
            for (int i = 0; i < ExistData.Count; i++)
            {
                existDic.Add(ExistData[i].Expect);
            }
            for (int i = NewestData.Count - 1;i>=0 ; i--)
            {
                if (existDic.Contains(NewestData[i].Expect))
                {
                    continue;
                }
                ret.Add(NewestData[i]);
            }
            return ret;
        }

        public override DbChanceList<T> getNoCloseChances<T>(string strDataOwner)
        {
           DbChanceList<T> ret = new DbChanceList<T>();
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = null;
            if (strDataOwner == null || strDataOwner.Trim().Length == 0)
                sql = string.Format("Select * from {0} where IsEnd=0", strChanceTable);
            else
                sql = string.Format("Select * from {0} where IsEnd=0 and UserId='{1}'", strChanceTable, strDataOwner);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            //ToLog("数据库结果",string.Format("未关闭机会数量为{0}",ds.Tables[0].Rows.Count));
            ret = new DbChanceList<T>(ds.Tables[0]);
            return ret;
        }

        public override DbChanceList<T> getClosedChances<T>(string strDataOwner, int PassedDays)
        {
            DbChanceList<T> ret = new DbChanceList<T>();
            DbClass db = GlobalClass.getCurrDb(strDataType);
            DateTime dt = DateTime.Today.AddDays(-1*PassedDays);
            string sql = null;
            if (strDataOwner == null || strDataOwner.Trim().Length == 0)
                sql = string.Format("Select * from {0} where IsEnd=1 and execdate>='{1}'", strChanceTable,dt.ToString("yyyy-MM-dd"));
            else
                sql = string.Format("Select * from {0} where IsEnd=1 and UserId='{1}' and execdate>='{2}'", strChanceTable, strDataOwner, dt.ToLongDateString());
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            //ToLog("数据库结果",string.Format("未关闭机会数量为{0}",ds.Tables[0].Rows.Count));
            ret = new DbChanceList<T>(ds.Tables[0]);
            return ret;
        }

        public override int SaveChances<T>(List<ChanceClass<T>> list,string strDataOwner=null)
        {
            if (list.Count == 0)
                return 0;
            DbChanceList<T> ret = new DbChanceList<T>();
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = string.Format("select top 0 * from {0}", strChanceTable);
            DataTable dt = db.getTableBySqlAndList<ChanceClass<T>>(new ConditionSql(sql), list);
            if (dt == null)
            {
                ToLog("保存机会数据错误", "根据数据表结构和提供的列表返回的机会列表错误！");
                return -1;
            }
            if (strDataOwner == null || strDataOwner.Trim().Length == 0)
                sql = string.Format("Select * from {0} where IsEnd=0", strChanceTable);
            else
                sql = string.Format("Select * from {0} where IsEnd=0 and UserId='{1}'", strChanceTable,strDataOwner);
            return db.UpdateOrNewList(new ConditionSql(sql), dt);

        }

        public override int DeleteChanceByIndex(long index,string strDataOwner = null)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = null;
            if (strDataOwner == null || strDataOwner.Trim().Length == 0)
                sql = string.Format("Delete  from {0} where ChanceIndex={1}", strChanceTable,index);
            else
                sql = string.Format("Select * from {0} where ChanceIndex={1} and UserId='{2}'", strChanceTable, index,strDataOwner);
            return db.ExecSql(new ConditionSql(sql));
        }

        public override int DeleteExpectData(string expectid)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = null;
            sql = string.Format("Delete  from {0} where expect='{1}'", strNewestTable, expectid);
            return db.ExecSql(new ConditionSql(sql));
        }

        public override void ExecProduce(string Procs)
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = null;
            sql = Procs;
            db.ExecSql(new ConditionSql(sql));
        }

        public override void updateExpectInfo(string dataType, string nextExpect, string currExpect=null)
        
        {
            DbClass db = GlobalClass.getCurrDb(strDataType);
            string sql = null;
            if (currExpect == null)
            {
                sql = string.Format("update dataExpectInfoTable set nextExpect='{0}' where dataType='{1}'", nextExpect,  dataType);
            }
            else
            {
                sql = string.Format("update dataExpectInfoTable set nextExpect='{0}',currExpect='{1}' where dataType='{2}'", nextExpect, currExpect, dataType);
            }
            db.ExecSql(new ConditionSql(sql));
        }
    }

}

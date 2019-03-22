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
        
        public override ExpectList ReadHistory(long From, long buffs)
        {
            return ReadHistory(From, buffs, false);
        }


        public override ExpectList ReadHistory(long From,long buffs,bool desc)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top {0} * from {2} where expect>='{1}'  order by expect {3}", buffs, From,strHistoryTable,desc?"desc":"");//modify by zhouys 2019/1/8
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }


        public override ExpectList ReadHistory(long buffs)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top {0} * from {2}  order by expect desc", buffs,  strHistoryTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public override ExpectList ReadHistory()
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select  * from {0}  order by expect", strHistoryTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public override ExpectList ReadHistory(string begt,string endt)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select  * from {0}  where opentime between '{1}' and '{2}'", strHistoryTable,begt,endt);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public override ExpectList ReadNewestData(DateTime fromdate)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {1} where opentime>='{0}' order by expect", fromdate.ToShortDateString(),strNewestTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public override ExpectList ReadNewestData(int LastLng)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from (select top {0} * from {1} order by expect desc) order by expect", LastLng, strNewestTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public override ExpectList ReadNewestData(int ExpectNo, int Cnt)
        {
            return ReadNewestData(ExpectNo, Cnt, false);
        }

        public override ExpectList ReadNewestData(int ExpectNo, int Cnt,bool FromHistoryTable)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {2} where Expect<='{0}' and Expect>({0}-{1}) order by expect", ExpectNo, Cnt, FromHistoryTable?strHistoryTable:strNewestTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public override int SaveNewestData(ExpectList InData)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top 0 * from {0}", strNewestTable);
            return db.SaveList(new ConditionSql(sql), InData.Table);
        }

        public override ExpectList GetMissedData(bool IsHistoryData,string strBegT)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {1} where opentime>='{0}'", strBegT, IsHistoryData?strMissHistoryTable:strMissNewestTable);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }


        public override int SaveHistoryData(ExpectList InData)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top 0 * from {0}", strHistoryTable);
            return db.SaveList(new ConditionSql(sql), InData.Table);
        }


        public int SaveProbWaveResult(DataTable dt)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {0}",strResultTable );
            return db.SaveNewList(new ConditionSql(sql), dt);
        }

        public DataTable GetProWaveResult(Int64 begid)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {0} where Expect>='{1}' order by Expect", this.strResultTable, begid);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            return ds.Tables[0];
        }

        public override ExpectList getNewestData(ExpectList NewestData, ExpectList ExistData)
        {
            ExpectList ret = new ExpectList();
            if (NewestData == null) return ret;
            if (ExistData == null) return NewestData;
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
                ret.Add((ExpectData)NewestData[i].Clone());
            }
            return ret;
        }

        public override DbChanceList getNoCloseChances(string strDataOwner)
        {
           DbChanceList ret = new DbChanceList();
            DbClass db = GlobalClass.getCurrDb();
            string sql = null;
            if (strDataOwner == null || strDataOwner.Trim().Length == 0)
                sql = string.Format("Select * from {0} where IsEnd=0", strChanceTable);
            else
                sql = string.Format("Select * from {0} where IsEnd=0 and UserId='{1}'", strChanceTable, strDataOwner);
            DataSet ds = db.Query(new ConditionSql(sql));
            if (ds == null) return null;
            //ToLog("数据库结果",string.Format("未关闭机会数量为{0}",ds.Tables[0].Rows.Count));
            ret = new DbChanceList(ds.Tables[0]);
            return ret;
        }

        public override int SaveChances(List<ChanceClass> list,string strDataOwner)
        {
            if (list.Count == 0)
                return 0;
            DbChanceList ret = new DbChanceList();
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top 0 * from {0}", strChanceTable);
            DataTable dt = db.getTableBySqlAndList<ChanceClass>(new ConditionSql(sql), list);
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
    }

}

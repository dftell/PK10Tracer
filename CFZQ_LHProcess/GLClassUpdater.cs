using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Sql;
using System.Threading;
using System.Threading.Tasks;
namespace CFZQ_LHProcess
{
    public class GLClassProcess
    {
        public object CheckPin;
        public string UpdateAllGLList(DateTime EndT)
        {
            JRGCDBClass jgdb = new JRGCDBClass();
            DataTable dt = getRecords(null, EndT);
            string sql = "select * from SectorTable";
            DataSet ds = jgdb.GetDataSet(sql);
            if (ds == null)
            {
                jgdb.Close();
                return "无法从本地数据库中获取到概念数据！";
            }
            DataTable dbdt = ds.Tables[0];
            string[] seccodes = new string[dbdt.Rows.Count] ;
            for(int i=0;i<dbdt.Rows.Count;i++)
            {
                seccodes[i] = dbdt.Rows[i]["SectorCode"].ToString();
            }
            string strSecCodes = string.Join("','",seccodes);
            string Filtersql = string.Format("wind_code not in ('{0}')", strSecCodes);
            DataRow[] drs;
            if (strSecCodes.Length == 0)
                drs = dt.Select();
            else
                drs = dt.Select(Filtersql);
            if (drs.Length == 0) return null;
            List<string> sectorList = new List<string>();
            for (int i = 0; i < drs.Length; i++)
            {
                sectorList.Add(drs[i]["wind_code"].ToString());
            }
            strSecCodes = string.Join(",",sectorList.ToArray());
            WSSProcess wss = new WSSProcess();
            dt = wss.getRecods(strSecCodes, "windcode,sec_name,launchdate");
            if (dt == null || dt.Rows.Count == 0)
            {
                jgdb.Close();
                return "无法从万得数据库中获取到概念其它信息！";
            }
            DataTable InsertTable = dbdt.Copy();
            InsertTable.Rows.Clear();
             for(int i=0;i<dt.Rows.Count;i++)
            {
                DataRow dr = InsertTable.NewRow();
                dr["SectorCode"] = dt.Rows[i]["WINDCODE"];
                dr["SectorName"] = dt.Rows[i]["SEC_NAME"];
                dr["SectorCreateDate"] = dt.Rows[i]["LAUNCHDATE"];
                dr["LastModifyDate"] = dt.Rows[i]["LAUNCHDATE"];
                dr["HQModifyDate"] = dt.Rows[i]["LAUNCHDATE"];
                InsertTable.Rows.Add(dr);
            }

            Int64 ret = jgdb.Save(InsertTable, sql);
            jgdb.Close();
            //if (InsertTable.Rows.Count != ret) return string.Format("计划保存{0}条数据，实际保存{1}条数据！", InsertTable.Rows.Count , ret);
            return null;
        }

        public string UpdateAllGLNumbers(DateTime EndT)
        {
            JRGCDBClass jgdb = new JRGCDBClass();
            
            string sql = string.Format("select * from SectorTable where LastModifyDate<'{0}'",EndT.ToShortDateString());
            DataSet ds = jgdb.GetDataSet(sql);
            
            if (ds == null)
            {
                jgdb.Close();
                return "无法从本地数据库中获取到概念数据！";
            }
            DataTable dt = ds.Tables[0];
            List<GLThreadProcess> threadpool = new List<GLThreadProcess>();
            List<MultiTask> datas = new List<MultiTask>();
            foreach (DataRow dr in dt.Rows) { datas.Add(new GLTask(dr,1)); } 
            for (int i = 0; i < 10; i++)//遍历所有概念  dt.Rows.Count
            {
                GLThreadProcess glthrdp = new GLThreadProcess(jgdb, EndT);
                glthrdp.FillPool(datas);
                threadpool.Add(glthrdp);
                Thread trd = new Thread(new ThreadStart(glthrdp.Execute));
                glthrdp.Trdobj = trd;
                trd.Start();
                
            }
            CheckPin = threadpool;
            return null;
        }

        public string UpdateAllGLHQs(DateTime EndT)
        {
            JRGCDBClass jgdb = new JRGCDBClass();

            string sql = string.Format("select * from SectorTable where HQModifyDate is null or HQModifyDate<'{0}'", EndT.ToShortDateString());
            DataSet ds = jgdb.GetDataSet(sql);
            if (ds == null)
            {
                jgdb.Close();
                return "无法从本地数据库中获取到概念数据！";
            }
            DataTable dt = ds.Tables[0];
            List<GLThreadProcess> threadpool = new List<GLThreadProcess>();
            List<MultiTask> datas = new List<MultiTask>();
            foreach (DataRow dr in dt.Rows) { datas.Add(new GLTask(dr,2)); }
            for (int i = 0; i < 10; i++)//遍历所有概念  dt.Rows.Count
            {
                GLThreadProcess glthrdp = new GLThreadProcess(jgdb, EndT);
                glthrdp.SaveDataPool = new List<ThdTask>();
                glthrdp.FillPool(datas);
                threadpool.Add(glthrdp);
                Thread trd = new Thread(new ThreadStart(glthrdp.Execute));
                glthrdp.Trdobj = trd;
                trd.Start();
            }
            CheckPin = threadpool;
            
            return null;
        }
        
        public DataTable getRecords(string strCode, DateTime dt)
        {
            GLClass globj = new GLClass(strCode);
            WSETClass wsetobj;
            if (strCode == null || strCode.Length == 0)//如果是板块类
            {
                wsetobj = new WSETMarketClass(globj.GLCode, dt);
            }
            else//如果是指数类
            {
                wsetobj = new WSETIndexClass(globj.GLCode, dt);
            }
            return WDDataAdapter.getRecords(wsetobj.getDataSet());
        }

        public Int64 getRecordCount(string strCode, DateTime dt)
        {
            GLClass globj = new GLClass(strCode);
            WSETClass wsetobj;
            if (strCode == null || strCode.Length == 0)//如果是板块类
            {
                wsetobj = new WSETMarketClass(globj.GLCode, dt);
            }
            else//如果是指数类
            {
                wsetobj = new WSETIndexClass(globj.GLCode, dt);
            }

            return wsetobj.getDataSetCount();
        }

        public DataTable getHQData(string strCode, DateTime begT,DateTime endT)
        {
            GLClass globj = new GLClass(strCode);
            WSDClass wsetobj = new WSDClass(strCode, "open,high,low,close,pct_chg,volume,windcode", begT, endT);
            //WSDClass wsetobj = new WSDClass(strCode, "open,close,volume,windcode", begT, endT);
            return WDDataAdapter.getRecords(wsetobj.getDataSet());
        }
    }

    public class GLThreadProcess : MultiTaskThrd
    {
        public GLThreadProcess(JRGCDBClass _jgdb, DateTime _endt)
        {
            jgdb = _jgdb;
            EndT = _endt;
        }

        public override void Execute()
        {
            //DataRow[] drs = obj as DataRow[];
            AddData();//加一个数据
            while (ExecObj != null)
            {

                GLTask glt = (ExecObj as GLTask);
                if (glt.Method == 1)
                    UpdateMember(jgdb, ExecObj.Data as DataRow, EndT);//执行更新成分股
                else if (glt.Method == 2)
                    UpdateHQData(jgdb, ExecObj.Data as DataRow, EndT);//执行更新行情
                AddData();//加一个数据
            }
            FillSaveData();
            while (SaveOjb != null)
            {
                SaveDb();
                FillSaveData();
            }
            FinishFlag = true;

        }

        string SaveDb()
        {
            if (SaveOjb == null) return null;
            SaveDataTask sdt = SaveOjb as SaveDataTask;
            DataSet ds = sdt.Data as DataSet;
            jgdb.Save(ds, sdt.Sqls);
            return null;
        }

        /// <summary>
        /// 更新概念指数的历史成分股变动日数据
        /// </summary>
        /// <param name="jgdb"></param>
        /// <param name="tdr"></param>
        /// <param name="EndT"></param>
        /// <returns></returns>
        string UpdateMember(JRGCDBClass jgdb, DataRow tdr, DateTime EndT)
        {
            DataTable dt = tdr.Table;
            DataTable DBdt = new DataTable();
            DBdt.Columns.Add("SectorCode");
            DBdt.Columns.Add("ChangeDate");
            DBdt.Columns.Add("MumberList");
            DBdt.Columns.Add("MumberCnt");
            string GLCode = tdr["SectorCode"].ToString();
            string GLName = tdr["SectorName"].ToString();
            DateTime LunchDate = (DateTime)tdr["SectorCreateDate"];
            GLClassProcess glcp = new GLClassProcess();

            DateTime BegT = (DateTime)tdr["LastModifyDate"];
            Int64 iMemCnt = 0;
            bool bMemCnt = Int64.TryParse(tdr["CurrMumberCount"].ToString(),out iMemCnt);
            DateTime LoopT = BegT;
            string strLastMemberList = null;
            DateTime lastDate = BegT;
            while (LoopT <= EndT)//从最后更新日开始，每一个月检查一次成员数量是否不一样，忽略只调整成员内容，不调整数量的情况
            {
                Int64 MemCnt = glcp.getRecordCount(GLCode, LoopT);
                if (MemCnt != iMemCnt) //如果该日成分股数量与上一日期的不同
                {
                    bool isFirstDay = (LunchDate == LoopT);
                    DataRow dr = DBdt.NewRow();
                    dr["SectorCode"] = GLCode;

                    for (int j = 31; j >= 0; j--)//往前一个月每日检查
                    {
                        DateTime dd = LoopT;
                        Int64 ddcnt = MemCnt;
                        if (!isFirstDay)
                        {
                            dd = LoopT.AddDays(-1 * j);
                            if (dd <= lastDate) continue;//一个月内有多个价格
                            CheckPin = string.Format("概念{0}:{1}已计算到日期{2}", GLCode, GLName, dd.ToShortDateString());
                            ddcnt = glcp.getRecordCount(GLCode, dd);
                        }
                        else
                        {
                            CheckPin = string.Format("概念{0}:{1}建立日期{2}", GLCode, GLName, dd.ToShortDateString());
                        }
                        if (ddcnt != iMemCnt)//如果当日数量不等于前值，标志为改变日期
                        {
                            dr["ChangeDate"] = dd;//更新日期
                            DataTable memdt = glcp.getRecords(GLCode, dd);
                            dr["MumberCnt"] = memdt.Rows.Count;//股票数量
                            List<string> memberList = new List<string>();
                            foreach (DataRow wdr in memdt.Rows)
                            {
                                string secid = wdr["wind_code"].ToString();
                                string[] arr = secid.Split('.');
                                secid = string.Format("{0}.{1}", arr[1], arr[0]);
                                memberList.Add(secid);
                            }
                            string strChange = string.Join(",", memberList.ToArray());
                            dr["MumberList"] = strChange;
                            strLastMemberList = strChange;
                            if (dd < LoopT) LoopT = dd;
                            lastDate = LoopT;
                            iMemCnt = memberList.Count;
                            break;
                        }
                    }
                    DBdt.Rows.Add(dr);
                }
                LoopT = LoopT.AddMonths(1);
                CheckPin = string.Format("概念{0}:{1}已计算到日期{2}", GLCode, GLName, LoopT.ToShortDateString());
            }
            if (strLastMemberList != null)
            {
                tdr["CurrMumberCount"] = iMemCnt;
                tdr["CurrMumberList"] = strLastMemberList;
                if (LoopT > EndT)
                {
                    LoopT = EndT;
                }
                tdr["LastModifyDate"] = LoopT;
            }
            string sql = string.Format("select * from SectorMemberTable where SectorCode='{0}'", GLCode);
            for (int i = DBdt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = DBdt.Rows[i];
                if (dr["SectorCode"].ToString() == "884221.WI")
                {
                    int itest = 32;
                }
                if (dr["ChangeDate"].ToString() == "")
                {
                    DBdt.Rows.Remove(dr);
                }
            }
            jgdb.Save(DBdt, sql);
            sql = string.Format("select * from SectorTable where SectorCode='{0}'", GLCode);
            jgdb.Save(dt, sql);

            CheckPin = string.Format("概念{0}:{1}已保存完毕。最后日期{2}！", GLCode, GLName, LoopT.ToShortDateString());
            return null;
        }

        string UpdateHQData(JRGCDBClass jgdb, DataRow tdr, DateTime EndT)
        {
            DataTable dt = tdr.Table.Clone();
            //DataTable DBdt = new DataTable();
            //DBdt.Columns.Add("SectorCode");
            //DBdt.Columns.Add("HQDate");
            //DBdt.Columns.Add("POpen");
            //DBdt.Columns.Add("PHigh");
            //DBdt.Columns.Add("PClose");
            //DBdt.Columns.Add("PLow");
            //DBdt.Columns.Add("Zf");
            //DBdt.Columns.Add("Volume");
            string GLCode = tdr["SectorCode"].ToString();
            string GLName = tdr["SectorName"].ToString();
            DateTime LunchDate = (DateTime)tdr["SectorCreateDate"];
            DateTime HQDate = LunchDate;
            if (tdr["HQModifyDate"] != null)
            {
                DateTime tDate;
                if (DateTime.TryParse(tdr["HQModifyDate"].ToString(), out tDate))
                {
                    HQDate = tDate;
                }
            }
            GLClassProcess glcp = new GLClassProcess();
            DateTime BegT = HQDate;
            DataTable WDtb = glcp.getHQData(GLCode, BegT, EndT);
            if (WDtb == null) throw (new Exception("无法从万得获取到数据！"));
            CheckPin = string.Format("概念{0}:{1}获取到行情数据{2}条！", GLCode, GLName, (WDtb == null ? 0 : WDtb.Rows.Count));
            DataRow[] drs = WDtb.Select("VOLUME is not null", "DateTime");
            DataTable savetb = WDtb.Clone();
            for (int i = 0; i < drs.Length; i++)
            {
                savetb.Rows.Add(drs[i].ItemArray);
            }
            savetb.Columns["WINDCODE"].ColumnName = "SectorCode";
            savetb.Columns["DateTime"].ColumnName = "HQDate";
            savetb.Columns["OPEN"].ColumnName = "POpen";
            savetb.Columns["HIGH"].ColumnName = "PHigh";
            savetb.Columns["CLOSE"].ColumnName = "PClose";
            savetb.Columns["LOW"].ColumnName = "PLow";
            //savetb.Columns["PCT_CHG"].ColumnName = "Zf";
            savetb.Columns["VOLUME"].ColumnName = "Volume";
            string sql = string.Format("select * from SectorHQTable where SectorCode='{0}' and HQDate between '{1}' and '{2}'", GLCode, BegT, EndT);
            SaveDataTask sdt = new SaveDataTask();
            this.SaveDataPool.Add(sdt);
            sdt.Sqls = new string[] { "", "" };
            DataSet ds = new DataSet();
            ds.Tables.Add(savetb);
            sdt.Data = ds;
            sdt.Sqls[0] = sql;
            CheckPin = string.Format("概念{0}:{1}已提交数据保存程序执行。最后日期{2}！", GLCode, GLName, EndT.ToShortDateString());
            
            return null;
            //this.SaveDataPool.Add(sdt);
            sql = string.Format("select * from SectorTable where SectorCode='{0}'", GLCode, BegT, EndT);
            tdr["HQModifyDate"] = EndT;
            if (savetb != null && savetb.Rows.Count > 0)
            {
                tdr["HQStartDate"] = savetb.Rows[0]["HQDate"];
            }
            dt.Rows.Add(tdr.ItemArray);
            if (dt.Rows.Count > 1)
            {
                throw (new Exception("概念信息重复！"));
            }
            ds.Tables.Add(dt);
            sdt.Sqls[1] = sql;
            
            //update sectortable set hqmodifydate = sectorcreatedate
            CheckPin = string.Format("概念{0}:{1}已提交数据保存程序执行。最后日期{2}！", GLCode, GLName, EndT.ToShortDateString());
            dt = null;
            return null;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;
using BaseObjectsLib;
using LogLib;
using System.Reflection;
namespace PK10CorePress
{
    public class SettingClass : DetailStringClass
    {
        

        public SettingClass()
        {
            GlobalSetting = new GlobalClass();
        }
        GlobalClass GlobalSetting;
        public GlobalClass GetGlobalSetting()
        {
            return GlobalSetting;
        }
        public void SetGlobalSetting(GlobalClass value)
        {
            GlobalSetting = value;
            if (value != null)
            {
                DispRows = value.MutliColMinTimes;
                minColTimes =new int[8];
                for(int i=0;i<8;i++)
                {
                    minColTimes[i] = value.MinTimeForChance(i+1);
                }
                Odds = value.Odds;
                MaxHoldingCnt = 1000;
                InitCash = value.InterVal;
            }
        }
        public int DispRows{get;set;}
        public int[] minColTimes { get; set; }
        public int GrownMinVal { get; set; }
        public int GrownMaxVal { get; set; }
        public double Odds { get; set; }
        public int MaxHoldingCnt { get; set; }
        public Int64 InitCash { get; set; }
        public bool UseLocalWaveData { get; set; }

    }
    
    public class DbClass:LogableClass
    {
        public DbClass()
        {
            _logname = "数据操作";
        }
        SqlConnection conn;
        string ConnStr = "";
        string ConnStrModel = "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}";
        public DbClass(string servername, string loginUser, string loginPwd, string dbname)
        {
            ConnStr = string.Format(ConnStrModel, servername, dbname, loginUser, loginPwd);
            conn = new SqlConnection(ConnStr);
            OpenConnect();
        }

        bool OpenConnect()
        {
            try
            {
                if (conn.State == ConnectionState.Open) return true;
                conn.Open();
                return true;
            }
            catch (Exception ce)
            {
                Log("错误","连接错误", ce.Message);
                return false;
            }
        }

        public DataSet Query(string sql)
        {
            if (OpenConnect() == false)
            {
                return null;
            }
            DataSet ret = new DataSet();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                da.Fill(ret);
            }
            catch (Exception ce)
            {
                Log("错误","查询错误", string.Format("{0}:{1}",sql,ce.Message));
                return null;
            }
            return ret;
        }

        public int SaveList(string sql,DataTable dt)
        {
            if (OpenConnect() == false)
            {
                return -1;
            }
            try
            {
                SqlCommand selectCMD = new SqlCommand(sql, conn);
                SqlDataAdapter sda = new SqlDataAdapter(selectCMD);
                //上面的语句中使用select 0，不是为了查询出数据，而是要查询出表结构以向DataTable中填充表结构
                sda.Fill(dt);
                SqlCommandBuilder scb = new SqlCommandBuilder(sda);
                //执行更新
                return sda.Update(dt);
                //使DataTable保存更新
                //dt.AcceptChanges();
            }
            catch (Exception ce)
            {
                Log("错误","保存数据错误", string.Format("{0}:{1}", sql, ce.Message));
                return -1;
            }
        }
        /// <summary>
        /// 支持单主键插入新增数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int SaveNewList(string sql, DataTable dt)
        {
            if (OpenConnect() == false)
            {
                return -1;
            }
            DataTable insertTab = null;
            DataTable newDt = null;
            try
            {
                newDt = new DataTable();
                SqlCommand selectCMD = new SqlCommand(sql, conn);
                SqlDataAdapter sda = new SqlDataAdapter(selectCMD);
                sda.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                //上面的语句中使用select 0，不是为了查询出数据，而是要查询出表结构以向DataTable中填充表结构
                sda.Fill(newDt);
                DataColumn[] pcols = newDt.PrimaryKey;
                if (pcols.Length != 1) return 0;
                var queryNew =
                from rNew in dt.AsEnumerable()
                join rOld in newDt.AsEnumerable()
                on rNew.Field<Int64>(pcols[0].ColumnName) equals rOld.Field<Int64>(pcols[0].ColumnName)
                into JoinedTable
                from Addtional in JoinedTable.DefaultIfEmpty()
                select new { 
                 newData= rNew,oldData = Addtional
                };
                insertTab = newDt.Clone();
                foreach (var obj in queryNew)
                {
                    if(obj != null && obj.newData != null && obj.oldData == null)
                        insertTab.Rows.Add(obj.newData.ItemArray);
                }
                SqlCommandBuilder scb = new SqlCommandBuilder(sda);
                //执行更新
                return sda.Update(insertTab);
                //使DataTable保存更新
                //dt.AcceptChanges();
            }
            catch (Exception ce)
            {
                Log("错误","保存新增数据错误", string.Format("{0}:{1}", sql, ce.Message));
                return -1;
            }
        }

        /// <summary>
        /// 执行非删除以外的所有更新，数据表需要指定唯一的key
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int UpdateOrNewList(string sql, DataTable dt)
        {
            object lockobj = new object();
            lock (lockobj)
            {
                if (OpenConnect() == false)
                {
                    return -1;
                }
                DataTable insertTab = null;
                DataTable updateTab = null;
                DataTable oldDt = null;
                try
                {
                    oldDt = new DataTable();
                    SqlCommand selectCMD = new SqlCommand(sql, conn);
                    SqlDataAdapter sda = new SqlDataAdapter(selectCMD);
                    sda.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    //上面的语句中使用select 0，不是为了查询出数据，而是要查询出表结构以向DataTable中填充表结构
                    sda.Fill(oldDt);
                    DataColumn[] pcols = oldDt.PrimaryKey;
                    if (pcols.Length != 1) return 0;
                    var queryNew =
                    from rNew in dt.AsEnumerable()
                    join rOld in oldDt.AsEnumerable()
                    on rNew.Field<Int64?>(pcols[0].ColumnName) equals rOld.Field<Int64?>(pcols[0].ColumnName)
                    into JoinedTable
                    from Addtional in JoinedTable.DefaultIfEmpty()
                    select new
                    {
                        newData = rNew,
                        oldData = Addtional
                    };
                    insertTab = oldDt.Clone();
                    updateTab = oldDt.Clone();

                    foreach (var obj in queryNew)
                    {
                        if (obj != null && obj.newData != null && obj.oldData == null)
                            insertTab.Rows.Add(obj.newData.ItemArray);
                        if (obj != null && obj.newData != null && obj.oldData != null)
                        {
                            updateTab.Rows.Add(obj.newData.ItemArray);
                        }
                    }
                    SqlCommandBuilder scb = new SqlCommandBuilder(sda);
                    //执行更新
                    oldDt.AcceptChanges();
                    DataTable dtNew = oldDt.Clone();
                    for (int i = 0; i < updateTab.Rows.Count; i++)
                    {
                        DataRow dr = updateTab.Rows[i];
                        for (int j = 0; j < oldDt.Rows.Count; j++)//遍历原表中所有记录
                        {
                            if (dr[pcols[0].ColumnName].Equals(oldDt.Rows[j][pcols[0]]))//主键相等
                            {
                                oldDt.Rows[j].SetModified();
                                //oldDt.Rows[j].ItemArray = dr.ItemArray;//用新表中替代

                                for (int c = 0; c < oldDt.Columns.Count; c++)
                                {
                                    if (oldDt.Columns[c].ColumnName.Equals(pcols[0].ColumnName))//主键跳过
                                    {
                                        continue;
                                    }
                                    oldDt.Rows[j][oldDt.Columns[c].ColumnName] = dr[oldDt.Columns[c].ColumnName];
                                }
                                break;
                            }
                        }
                    }



                    int SameRet = sda.Update(oldDt);
                    int NewRet = sda.Update(insertTab);
                    return SameRet + NewRet;
                    //使DataTable保存更新
                    //dt.AcceptChanges();
                }
                catch (Exception ce)
                {
                    Log("错误", "新增/更新数据错误", string.Format("{0}:{1}", sql, ce.StackTrace));
                    return -1;
                }
            }
        }


        ////////public int SaveNewList<T>(string sql, List<T> list)
        ////////{
        ////////    return 0;
        ////////}

        ////////public int SaveList<T>(string sql, List<T> list)
        ////////{
        ////////    return 0;
        ////////}

        static Dictionary<Type, Dictionary<string, MemberInfoTypeItem>> TypesList = new Dictionary<Type,Dictionary<string,MemberInfoTypeItem>>();

        public DataTable getTableBySqlAndList<T>(string sql, List<T> list)
        {
            object obj = new object();
            lock (TypesList)
            {
                DataTable dt = new DataTable();
                DataSet ds = Query(sql);
                if (ds == null)
                {
                    return null;
                }
                dt = ds.Tables[0].Clone();
                DataColumn[] pkeys = dt.PrimaryKey;
                if (pkeys.Length > 1)
                {
                    ToLog("错误","主键数量不唯一", pkeys.Length.ToString());
                }
                Dictionary<string,MemberInfoTypeItem> DataCols = null;
            
                if (TypesList == null)
                {
                    TypesList = new Dictionary<Type, Dictionary<string, MemberInfoTypeItem>>();
                }
                if (TypesList.ContainsKey(typeof(T)))
                {
                    DataCols = TypesList[typeof(T)];
                }
                List<string> tbcol = new List<string>();
                if (DataCols == null)
                {
                    DataCols = new Dictionary<string, MemberInfoTypeItem>();

                    
                    MemberInfo[] mis = typeof(T).GetMembers();
                    for (int i = 0; i < mis.Length; i++)
                    {
                        MemberInfo mi = mis[i];
                        string name = mi.Name;
                        if (dt.Columns.Contains(name))//表数据列里面包括该成员
                        {
                            tbcol.Add(dt.Columns[name].ColumnName);
                            if (!DataCols.ContainsKey(name))
                            {
                                DataCols.Add(name, new MemberInfoTypeItem(mi, dt.Columns[name].DataType));
                            }
                            else
                            {
                                ToLog("错误","通过数据源获取数据列表错误", string.Format("存在相同的列:{0}", name));
                            }
                        }
                    }
                    if (!TypesList.ContainsKey(typeof(T)))
                    {
                        TypesList.Add(typeof(T), DataCols);
                    }
                    else
                    {
                        //ToLog("存在数据表结构", typeof(T).ToString());
                    }
                }
                //ToLog("输出数据表结构", string.Join(",", DataCols.Keys.ToList<string>().ToArray()));
                //ToLog("映射数据表结构", string.Join(",", tbcol.ToArray()));
                for (int i = 0; i < list.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    foreach (string key in DataCols.Keys)
                    {
                        MemberInfoTypeItem miti = DataCols[key];
                        MemberInfo mi = miti.MemInfo;
                        object val = null;
                        if (mi is PropertyInfo)
                        {
                            val = (mi as PropertyInfo).GetValue(list[i], null);
                        }
                        else
                        {
                            val = (mi as FieldInfo).GetValue(list[i]);
                        }
                        if (val == null)
                        {
                            //ToLog("域值为空！", mi.Name);
                            continue;
                        }
                        if (dt.Columns.Contains(key))
                        {
                            dr[key] = Convert.ChangeType(val, miti.DbType);
                        }
                        else
                        {
                            ToLog("错误","保存数据列表", "表数据结构突然消失");
                        }
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            
        }
    
        
    }

    class MemberInfoTypeItem
    {
        public MemberInfo MemInfo;
        public Type DbType;
        public MemberInfoTypeItem(MemberInfo mi,Type dbtype)
        {
            MemInfo = mi;
            DbType = dbtype;
        }
    }
}

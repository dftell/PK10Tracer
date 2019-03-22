using System;
using System.Collections.Generic;
using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;

namespace WolfInv.com.DbAccessLib
{
    public class MongoDbClass : CommDbClass
    {
        IMongoDatabase db;
        IMongoCollection<BsonDocument> Coll;
        public MongoDbClass()
        {
        }
    
        protected override void InitStr()
        {
            _logname = "MongoDB数据操作";
            ConnStrModel = "mongodb://{2}{0}/{1}";//ConnStrModel="Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}";

        }

        public MongoDbClass(string servername, string loginUser, string loginPwd, string dbname) : base()
        {
            InitStr();
            string sec = "";
            if(loginUser.Length > 0)
                sec = string.Format("{0}:{1}@", loginUser, loginPwd);
            ConnStr = string.Format(ConnStrModel, servername, dbname, sec);
            this.DbName = dbname;
            //conn = new SqlConnection(ConnStr);
            OpenConnect();
        }
        public override DataTable getTableBySqlAndList<T>(ConditionSql sql, List<T> list)
        {
            throw new NotImplementedException();
        }

        public override DataSet Query(ConditionSql sql)
        {
            if (OpenConnect() == false)
            {
                return null;
            }
            DataSet ret = new DataSet();
            try
            {
                Coll = db.GetCollection<BsonDocument>(sql.DocumentName);
                if (sql.Filters == null)
                    return ret;
                //
                IFindFluent<BsonDocument, BsonDocument> result = Coll.Find<BsonDocument>(sql.Filters);
                List<BsonDocument> FltRes = Coll.Find<BsonDocument>(sql.Filters).ToList();
                //FltRes.Sort(sql.Sorts.e);
                List<BsonDocument> res = FltRes;// result.Sort(sql.Sorts).ToList();
                ////SqlDataAdapter da = new SqlDataAdapter(sql.sql, conn);
                ////da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                ////da.Fill(ret);
                ret = Fill(res);
            }
            catch (Exception ce)
            {
                Log("错误", "查询错误", string.Format("{0}:{1}", sql.Filters.ToJson(), ce.Message));
                return null;
            }
            return ret;
        }

        public override int SaveList(ConditionSql sql, DataTable dt)
        {
            throw new NotImplementedException();
        }

        public override int SaveNewList(ConditionSql sql, DataTable dt)
        {
            throw new NotImplementedException();
        }

        public override int UpdateOrNewList(ConditionSql sql, DataTable dt)
        {
            throw new NotImplementedException();
        }

        protected override bool OpenConnect()
        {
            try
            {
                if (db != null)
                    return true;
                this._client = new MongoClient(this.ConnStr);
                //未开启验证模式数据库连接
                // var client = new MongoClient("mongodb://127.0.0.1:27017"); 
                //指定要操作的数据库
                db = DBClient.GetDatabase(this.DbName);
            }
            catch(Exception ce)
            {
                Log("连接失败",ce.Message);
                return false;
            }
            //var database = client.GetDatabase(new MongoUrl(connectionString).DatabaseName);
            return true;
        }


        public static DataSet Fill(List< BsonDocument> result)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(ToTable(result));
            return ds;
        }

        public static DataTable ToTable(List<BsonDocument> result)
        {
            DataTable dt = new DataTable();
            if (result == null) return dt;
            Dictionary<string, Type> cols = new Dictionary<string, Type>();
            int rowindex = 0;
            foreach(var item in result)
            {
                
                foreach(BsonDocument val in item.Values)
                {
                    BsonElement be = val.GetElement(0);
                    BsonValue bv = val.GetValue(be.Name);
                    if(!cols.ContainsKey(be.Name))
                    {
                        dt.Columns.Add(be.Name, bv.GetType());
                        cols.Add(be.Name, bv.GetType());
                    }
                }
                DataRow dr = dt.NewRow();
                foreach (BsonDocument val in item.Values)
                {
                    BsonElement be = val.GetElement(0);
                    BsonValue bv = val.GetValue(be.Name);
                    dr[be.Name] = bv;
                }
                dt.Rows.Add(dr);
                rowindex++;
            }
            return dt;
        }
    }

    
}

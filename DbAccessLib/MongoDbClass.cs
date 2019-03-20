using System;
using System.Collections.Generic;
using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;
namespace WolfInv.com.DbAccessLib
{
    public class MongoDbClass : CommDbClass
    {
        public MongoDbClass()
        {
        }
    
        protected override void InitStr()
        {
            _logname = "MongoDB数据操作";
            ConnStrModel = "mongodb://{2}@{0}/{1}";//ConnStrModel="Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}";

        }

        public MongoDbClass(string servername, string loginUser, string loginPwd, string dbname) : base()
        {
            InitStr();
            string sec = "";
            if(loginUser.Length > 0)
                sec = string.Format("{0}:{1}", loginUser, loginPwd);
            ConnStr = string.Format(ConnStrModel, servername, dbname, sec);
            this.DbName = dbname;
            //conn = new SqlConnection(ConnStr);
            OpenConnect();
        }
        public override DataTable getTableBySqlAndList<T>(string sql, List<T> list)
        {
            throw new NotImplementedException();
        }

        public override DataSet Query(string sql)
        {
            throw new NotImplementedException();
        }

        public override int SaveList(string sql, DataTable dt)
        {
            throw new NotImplementedException();
        }

        public override int SaveNewList(string sql, DataTable dt)
        {
            throw new NotImplementedException();
        }

        public override int UpdateOrNewList(string sql, DataTable dt)
        {
            throw new NotImplementedException();
        }

        protected override bool OpenConnect()
        {
            var client = new MongoClient(this.ConnStr);
            //未开启验证模式数据库连接
            // var client = new MongoClient("mongodb://127.0.0.1:27017"); 
            //指定要操作的数据库
            _db = client.GetDatabase(this.DbName);
            return true;
        }
    }
}

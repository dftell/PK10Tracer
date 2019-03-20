using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WolfInv.com.LogLib;
using MongoDB.Driver;
namespace WolfInv.com.DbAccessLib
{
    public abstract class CommDbClass: LogableClass
    {
        protected abstract void InitStr();
        protected SqlConnection conn;
        protected IMongoDatabase _db;
        protected string DbName;
        protected string ConnStr = "";
        protected  string ConnStrModel = "";
        protected abstract bool OpenConnect();
        public abstract DataSet Query(string sql);
        public abstract int SaveList(string sql, DataTable dt);

        public abstract int SaveNewList(string sql, DataTable dt);

        public abstract int UpdateOrNewList(string sql, DataTable dt);

        public abstract DataTable getTableBySqlAndList<T>(string sql, List<T> list);

    }
}

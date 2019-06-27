using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WolfInv.com.LogLib;
using MongoDB.Driver;
using MongoDB.Bson;
namespace WolfInv.com.DbAccessLib
{
    public class ConditionSql
    {
        public string[] SecCodes;
        public string sql { get; set; }//支持sql
        public string DocumentName { get; set; }
        public FilterDefinition<BsonDocument> Filters;//支持mongo
        public SortDefinition<BsonDocument> Sorts;//
        public ConditionSql(string _sql)
        {
            sql = _sql;
        }

        public ConditionSql(string _DocumentName,FilterDefinition<BsonDocument> _sql)
        {
            DocumentName = _DocumentName;
            Filters = _sql;
        }

        public ConditionSql(string _DocumentName, FilterDefinition<BsonDocument> _sql, SortDefinition<BsonDocument> _sort)
        {
            DocumentName = _DocumentName;
            Filters = _sql;
            Sorts = _sort;
        }


    }
    public abstract class CommDbClass: LogableClass
    {
        protected abstract void InitStr();
        protected SqlConnection conn;
        protected string DbName;
        protected string ConnStr = "";
        protected  string ConnStrModel = "";
        protected abstract bool OpenConnect();
        public abstract DataSet Query(ConditionSql sql);
        public abstract int SaveList(ConditionSql sql, DataTable dt);

        public abstract int SaveNewList(ConditionSql sql, DataTable dt);

        public abstract int UpdateOrNewList(ConditionSql sql, DataTable dt);

        public abstract DataTable getTableBySqlAndList<T>(ConditionSql sql, List<T> list);

        public abstract int ExecSql(ConditionSql sql);
    }
}

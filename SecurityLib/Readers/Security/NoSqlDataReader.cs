using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.DbAccessLib;
using System.Data;
using MongoDB.Driver.Core;
namespace WolfInv.com.SecurityLib
{
    public class NoSqlDataReader : DataReader 
    {
        string datatype;
        string[] secCodes;
        string TableName;
        MongoDBBase db;
        public NoSqlDataReader(string DataType, string DocName, string[] codes, Cycle cyc)
        {
            db = GlobalClass.getCurrNoSQLDb(DataType);
            TableName = DocName;
            secCodes = codes;
            datatype = DataType;
        }

        public override int DeleteChanceByIndex(long index, string strDataOwner = null)
        {
            throw new NotImplementedException();
        }

        public override int DeleteExpectData(string expectid)
        {
            throw new NotImplementedException();
        }

        public override void ExecProduce(string Procs)
        {
            throw new NotImplementedException();
        }

        ////public NoSqlDataReader(string DataTable,Cycle DataCycle)
        ////{
        ////    this.strNewestTable = string.Format("{0}_{1}",DataTable,DataCycle);
        ////    this.strHistoryTable = DataTable;
        ////}
        public override ExpectList<T> GetMissedData<T>(bool IsHistoryData, string strBegT)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> getNewestData<T>(ExpectList<T> NewestData, ExpectList<T> ExistData)
        {
            throw new NotImplementedException();
        }

        public override DbChanceList<T> getNoCloseChances<T>(string strDataOwner)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory<T>()
        {
            return null;
        }
             
        

        public override ExpectList<T> ReadHistory<T>(long buffs)
        {
            return null;
        }

        public override ExpectList<T> ReadHistory<T>(long From, long buffs)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory<T>(long From, long buffs, bool desc)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory<T>(string begt, string endt)
        {
            return null;
            ///
            /*
             def QA_fetch_stock_day(code, start, end, format='numpy{ get; set; } frequence='day{ get; set; } collections=DATABASE.stock_day):
    """'获取股票日线'

    Returns:
        [type] -- [description]

        感谢@几何大佬的提示
        https://docs.mongodb.com/manual/tutorial/project-fields-from-query-results/#return-the-specified-fields-and-the-id-field-only

    """

    start = str(start)[0:10]
    end = str(end)[0:10]
    #code= [code] if isinstance(code,str) else code

    # code checking
    code = QA_util_code_tolist(code)

    if QA_util_date_valid(end):

        __data = []
        cursor = collections.find({
            'code': {'$in': code}, "date_stamp": {
                "$lte": QA_util_date_stamp(end),
                "$gte": QA_util_date_stamp(start)}}, {"_id": 0}, batch_size=10000)
        #res=[QA_util_dict_remove_key(data, '_id') for data in cursor]

        res = pd.DataFrame([item for item in cursor])
        try:
            res = res.assign(volume=res.vol, date=pd.to_datetime(
                res.date)).drop_duplicates((['date{ get; set; } 'code'])).query('volume>1').set_index('date{ get; set; } drop=False)
            res = res.ix[:, ['code{ get; set; } 'open{ get; set; } 'high{ get; set; } 'low{ get; set; }
                             'close{ get; set; } 'volume{ get; set; } 'amount{ get; set; } 'date']]
        except:
            res = None
        if format in ['P{ get; set; } 'p{ get; set; } 'pandas{ get; set; } 'pd']:
            return res
        elif format in ['json{ get; set; } 'dict']:
            return QA_util_to_json_from_pandas(res)
        # 多种数据格式
        elif format in ['n{ get; set; } 'N{ get; set; } 'numpy']:
            return numpy.asarray(res)
        elif format in ['list{ get; set; } 'l{ get; set; } 'L']:
            return numpy.asarray(res).tolist()
        else:
            print("QA Error QA_fetch_stock_day format parameter %s is none of  \"P, p, pandas, pd , json, dict , n, N, numpy, list, l, L, !\" " % format)
            return None
    else:
        QA_util_log_info(
            'QA Error QA_fetch_stock_day data parameter start=%s end=%s is not right' % (start, end))

*/
        }

        public override ExpectList<T> ReadNewestData<T>(DateTime fromdate)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadNewestData<T>(int LastLng)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadNewestData<T>(long ExpectNo, int Cnt)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadNewestData<T>(long ExpectNo, int Cnt, bool FromHistoryTable)
        {
            throw new NotImplementedException();
        }

        public override int SaveChances<T>(List<ChanceClass<T>> list, string strDataOwner)
        {
            throw new NotImplementedException();
        }

        public override int SaveHistoryData<T>(ExpectList<T> InData)
        {
            throw new NotImplementedException();
        }

        public override int SaveNewestData<T>(ExpectList<T> InData)
        {
            throw new NotImplementedException();
        }

        public override void updateExpectInfo(string dataType, string nextExpect, string currExpect)
        {
            throw new NotImplementedException();
        }
    }

}

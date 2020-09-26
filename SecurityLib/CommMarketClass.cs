using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
namespace WolfInv.com.SecurityLib
{
    public class CommMarketClass
    {
        DataTypePoint dtp;
        public CommMarketClass(DataTypePoint _dtp)
        {
            dtp = _dtp;
        }
        public string[] GetMarketsStocks(string indexname, string endt, int MinOnMarketDays, bool NextDay, out int[] onMarketDays, bool ExcludeST=false, bool MAFilter=false)
        {
            string[] ret = new string[1] { indexname };
            if(indexname == null || indexname.Trim().Length==0) throw new Exception("市场，板块，指数不能为空！");
            string[] IndexList = indexname.Split(';');
            if (IndexList.Length == 1)//指数或者个股
            {
                if(IndexList[0] == "000001")//A股，目前只支持全A指数
                {
                    SecurityReader secreader = new SecurityReader(dtp.DataType, dtp.HistoryTable);
                    string[] insql = new string[4];
                    insql[0] = "{$match:{date:{$lt:'{0}'}}}".Replace("{0}", endt); ;
                    insql[1] = "{$group : {_id : '$code', OnMarketDays:{$sum:1}}}";
                    insql[2] = "{$match:{OnMarketDays:{$gt:{0}}}}".Replace("{0}",MinOnMarketDays.ToString());
                    insql[3] = "{$project:{_id:0,code:'$_id',OnMarketDays:1}}";
                    MongoReturnDataList<StockInfoMongoData> datas = secreader.GetAllVaildList<StockInfoMongoData>(insql);
                    string[] res = datas.ToList(a => a.code).ToArray();
                    onMarketDays = datas.ToList(a => a.OnMarketDays).ToArray();
                    return res;
                }
                else//单个个股
                {
                    SecurityReader secreader = new SecurityReader(dtp.DataType, dtp.HistoryTable);
                    string[] insql = new string[4];
                    insql[0] = "{$match:{date:{$lt:'{0}'},code:'{1}'}}".Replace("{0}", endt).Replace("{1}",indexname);
                    insql[1] = "{$group : {_id : '$code', OnMarketDays:{$sum:1}}}";
                    insql[2] = "{$match:{OnMarketDays:{$gt:{0}}}}".Replace("{0}", MinOnMarketDays.ToString());
                    insql[3] = "{$project:{_id:0,code:'$_id',OnMarketDays:1}}";
                    MongoReturnDataList<StockInfoMongoData> datas = secreader.GetAllVaildList<StockInfoMongoData>(insql);
                    onMarketDays = datas.ToList(a => a.OnMarketDays).ToArray();
                    return datas.ToList(a => a.code).ToArray();
                }
            }
            else//多个个股
            {
                SecurityReader secreader = new SecurityReader(dtp.DataType, dtp.HistoryTable);
                string[] insql = new string[4];
                insql[0] = "{$match:{date:{$lt:'{0}'}}}".Replace("{0}", endt); ;
                insql[1] = "{$group : {_id : '$code', OnMarketDays:{$sum:1}}}";
                string codes = string.Join("','", IndexList);
                insql[2] = "{$match:{OnMarketDays:{$gt:{0}},code:{$in:'{1}'}}}".Replace("{0}", MinOnMarketDays.ToString()).Replace("{1}", codes); ;
                insql[3] = "{$project:{_id:0,code:'$_id',OnMarketDays:1}}";
                MongoReturnDataList<StockInfoMongoData> datas = secreader.GetAllVaildList<StockInfoMongoData>(insql);
                onMarketDays = datas.ToList(a => a.OnMarketDays).ToArray();
                return datas.ToList(a => a.code).ToArray();
            }
            return ret;
        }

        public DateTime[] getMarketCalendar(string indexName,DateTime endt)
        {
            SecurityReader secreader = new SecurityReader(dtp.DataType, dtp.HistoryTable);
            string[] insql = new string[4];
            insql[0] = "{$match:{date:{$lt:'{0}'}}}".Replace("{0}", endt.ToString("yyyy-MM-dd")); ;
            insql[1] = "{$match:{code:'{0}'}}".Replace("{0}", indexName);
            insql[2] = "{$sort:{date:1}}";
            insql[3] = "{$project:{date:1}}";
            MongoReturnDataList<ExchangeMongoData> datas = secreader.GetAllVaildList<ExchangeMongoData>(insql);
            return datas.ToList(a => DateTime.Parse(a.date)).ToArray();
        }
    }
}

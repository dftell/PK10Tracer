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
        public string[] GetMarketsStocks(string indexname, string endt, int MinOnMarketDays, bool NextDay, bool ExcludeST, bool MAFilter)
        {
            string[] ret = new string[1] { indexname };
            if(indexname == null || indexname.Trim().Length==0) throw new Exception("市场，板块，指数不能为空！");
            string[] IndexList = indexname.Split(';');
            if (IndexList.Length == 1)//指数或者个股
            {
                if(IndexList[0] == "000001")//A股
                {
                    SecurityReader secreader = new SecurityReader(dtp.DataType, dtp.HistoryTable);
                    string[] insql = new string[4];
                    insql[0] = "{$match:{date:{$lt:'{0}'}}}".Replace("{0}", endt); ;
                    insql[1] = "{$group : {_id : '$code', OnMarketDays:{$sum:1}}}";
                    insql[2] = "{$match:{OnMarketDays:{$gt:{0}}}}".Replace("{0}",MinOnMarketDays.ToString());
                    insql[3] = "{$project:{_id:0,code:'$_id',OnMarketDays:1}}";
                    MongoReturnDataList<StockInfoMongoData> datas = secreader.GetAllVaildList<StockInfoMongoData>(insql);
                    return datas.ToList(a => a.code).ToArray();
                }
                else//个股
                {

                }
            }
            return ret;
        }
    }
}

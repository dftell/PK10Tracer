using System.Data;
using System.Collections.Generic;
namespace WolfInv.com.BaseObjectsLib
{
    public interface IExchangeData: ICodeDateStampData, IOHLCData, iVolAmount, IDateStampData
    {

    }
    public class StockMongoData : ExchangeMongoData
    {

    }

    public class StockIndexMongoData : StockMongoData
    {
        public int up_count { get; set; }
        public int down_count { get; set; }
    }

    public class StockInfoMongoData : MongoData, ICodeData
    {
        public string code { get; set; }
        public int volunit { get; set; }

        /// <summary>
        /// 精确到小数点后位数
        /// </summary>
        public int decimal_point { get; set; }
        public string name { get; set; }
        //前期价格
        public double pre_close { get; set; }
        /// <summary>
        /// 交易所
        /// </summary>
        public string sse { get; set; }
        /// <summary>
        /// 证券市场
        /// </summary>
        public string sec { get; set; }

        /// <summary>
        /// 上市天数
        /// </summary>
        public int OnMarketDays { get; set; }
        string _fullcode;
        public string FullCode
        {
            get
            {
                if (_fullcode == null)
                    _fullcode = string.Format("{0}.{1}", code, sse);
                return _fullcode;
            }
        }
    }

    public class ExchangeMongoData : MongoData, IExchangeData
    {
        public string date { get; set; }
        public double date_stamp { get; set; }

        public Cycle Cyc;
        public string code { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double vol { get; set; }
        public double amount { get; set; }

        public OneCycleData PreData;
        public OneCycleData NextData;
        double _ChgRate = 0;
        /// <summary>
        /// 涨幅
        /// </summary>
        public double ChgRate
        {
            get
            {
                if (_ChgRate == 0)
                {
                    if (PreData == null)
                        return 0;
                    if (PreData.close == 0)
                        return 0;
                    _ChgRate = 100 * (close - PreData.close) / PreData.close;
                }
                return _ChgRate;
            }
        }



        public DataSet ToDataSet<T>(List<T> list) where T: MongoData
        {
            return DataListConverter<T>.ToDataSet(list, "code");
        }
    }

    public interface IOHLCData
    {
        double open { get; set; }
        double high { get; set; }
        double low { get; set; }
        double close { get; set; }
    }

    public interface IIndexData
    {
        int up_count { get; set; }
        int down_count { get; set; }
    }

    public interface iVolAmount
    {
        double vol { get; set; }
        double amount { get; set; }
    }
}
;

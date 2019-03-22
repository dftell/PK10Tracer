using System.Data;
using System.Collections.Generic;
namespace WolfInv.com.BaseObjectsLib
{
    public interface IExchangeData: IDateData,ICodeData,IOHLCData, iVolAmount
    {

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



        public DataSet ToDataSet<T>(List<T> list)
        {
            return DataListConverter.ToDataSet(list, "code");
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

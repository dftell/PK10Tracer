using System.Collections;
using System.Collections.Generic;
using System.Data;
namespace WolfInv.com.BaseObjectsLib
{
    public class OneCycleData: MongoData, IExchangeData
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

        //////public OneCycleData PreData;
        //////public OneCycleData NextData;
        double _ChgRate = 0;
        /// <summary>
        /// 涨幅
        /// </summary>
        ////public double ChgRate
        ////{
        ////    get
        ////    {
        ////        if (_ChgRate == 0)
        ////        {
        ////            if (PreData == null)
        ////                return 0;
        ////            if (PreData.close == 0)
        ////                return 0;
        ////            _ChgRate = 100 * (close - PreData.close) / PreData.close;
        ////        }
        ////        return _ChgRate;
        ////    }
        ////}

        

        public DataSet ToDataSet<T>(List<T> list) where T: TimeSerialData
        {
            return DataListConverter<T>.ToDataSet(list, "code");
        }
    }

    
    public interface IDateData
    {
        string date { get; set; }
    }
    
    public interface IDateStampData
    {
        double date_stamp { get; set; }
    }

    public interface ICodeDateStampData:ICodeData,IDateStampData
    {

    }

    
}
;

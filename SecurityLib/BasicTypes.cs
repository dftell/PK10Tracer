using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfInv.com.SecurityLib
{

    public class SecBuffData
    {
    }

    ////public class WDSetBuffData : SecBuffData
    ////{

    ////}

    ////public class WDHisBuffData : SecBuffData
    ////{
    ////}
    //周期
    public enum Cycle
    {
        Day, Week, Month, Quarter, SemiYear, Year,Minute,Tick
    }
    //复权方式
    public enum PriceAdj
    {
        Fore, UnDo, Beyond, Target
    }

    public class SecurityInfoClass 
    {
        public string StockID { get; set; }
        public string StockName { get; set; }
        public string IsST { get; set; }

        //行情快照
        public OneCycleData DataReview;

        public bool IsIndex { get; set; }
        
        public bool IsTradeDay { get; set; }
        public DateTime FirstDay { get; set; }
        public int OnMarketDays() 
        { 
            return 0; 
        }

        HistoryData hisData;
        public HistoryData historyData 
        {
            get 
            { 
                return hisData;
            } 
        }
        
    }


    public class OneCycleData
    {
        public Cycle Cyc;
        public decimal Open;
        public decimal High;
        public decimal Low;
        public decimal Close;
        public long Val;
        public OneCycleData PreData;
        public OneCycleData NextData;
        decimal _ChgRate = 0;
        /// <summary>
        /// 涨幅
        /// </summary>
        public decimal ChgRate
        {
            get
            {
                if (_ChgRate == 0)
                {
                    if (PreData == null)
                        return 0;
                    if (PreData.Close == 0)
                        return 0;
                    _ChgRate = 100 * (Close - PreData.Close) / PreData.Close;
                }
                return _ChgRate;
            }
        }
        

    }

}

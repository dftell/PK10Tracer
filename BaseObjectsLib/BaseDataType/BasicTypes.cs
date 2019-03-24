using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfInv.com.BaseObjectsLib
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
        Day, Week, Month, Quarter, SemiYear, Year,Minute,Tick,Expect
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

    
}

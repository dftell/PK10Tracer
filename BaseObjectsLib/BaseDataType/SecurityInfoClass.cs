using System;

namespace WolfInv.com.BaseObjectsLib
{
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

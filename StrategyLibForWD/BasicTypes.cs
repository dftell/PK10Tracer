using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyLibForWD
{
    /// <summary>
    /// 万得缓存数据基础类
    /// </summary>
    public class WDBuffData
    {
    }

    public class WDSetBuffData : WDBuffData
    {

    }

    public class WDHisBuffData : WDBuffData
    {
    }

    //周期
    public enum Cycle
    {
        Day, Week, Month, Quarter, SemiYear, Year
    }
    //复权方式
    public enum PriceAdj
    {
        UnDo, Fore, Beyond, Target
    }

    public class Security : WDBuffData
    {
        public string StockID { get; set; }
        public string StockName { get; set; }
        public string IsST { get; set; }
        public decimal LastClose { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
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

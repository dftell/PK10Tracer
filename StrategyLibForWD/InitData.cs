using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyLibForWD
{
    //初始化基础数据
    public class InitData
    {
    }
    /// <summary>
    /// 系统证券基础数据
    /// </summary>
    public class BaseData
    {
        public DateTime CurrDate { get; set; }
        public Cycle CurrCycle { get; set; }
        public PriceAdj CurrRate { get; set; }
        public static SystemParams<string, Dictionary<string, WDBuffData>> SysParams;
        public delegate void ParamData(string Security,WDBuffData Data);
        static BaseData()
        {
            SysParams = new SystemParams<string, Dictionary<string, WDBuffData>>();
        }

        static void InitData()
        {

        }

        
    }

    public delegate void DataAssset();
    
    public class SystemParams<key,val>:Dictionary<key,val>
    {

    }

    public class HistoryData
    {
        //public
    }

    public enum BaseDataType
    {
        /// <summary>
        /// 指数数据
        /// </summary>
        IndexData,
        /// <summary>
        /// 股票数据
        /// </summary>
        StockData
    }

    ///System Params
    ///IndexData StockData
    ///History Data
   

    
}

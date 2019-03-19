using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfInv.com.SecurityLib
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
        public static SystemParams<string, Dictionary<string, SecBuffData>> SysParams;
        public delegate void ParamData(string Security,SecBuffData Data);
        static BaseData()
        {
            SysParams = new SystemParams<string, Dictionary<string, SecBuffData>>();
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
        Equit,
        Fund,
        Bond,
        Cash,
        Lotty
    }

    ///System Params
    ///IndexData StockData
    ///History Data
   

    
}

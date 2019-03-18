using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFZQ_LHProcess;
using WAPIWrapperCSharp;
using System.Reflection;
namespace StrategyLibForWD
{
    //通用股票指数类
    public class SecIndexClass: IWDIndexSet
    {
        public string SummaryCode;
        public string IndexCode;
        public SecIndexClass()
        {
        }

        public SecIndexClass(string sectorid)
        {
            if (sectorid != null && (IndexCode == null || IndexCode.Trim().Length == 0))
                IndexCode = sectorid;
        }

        ////public  MTable getBkbyDate(string indexname, DateTime EndT)
        ////{
        ////    //SecIndexClass sic = new SecIndexClass(w,indexname);
        ////    secIndexBuilder sib = new secIndexBuilder(this);
        ////    MTable ret = sib.getBkList(EndT);
        ////    return ret;
        ////}
        ////public MTable getBkWeightbyDate(string indexname, DateTime EndT)
        ////{
        ////    SecIndexClass sic = new SecIndexClass(w,indexname);
        ////    secIndexBuilder sib = new secIndexBuilder(sic);
        ////    MTable ret = sib.getBkWeight(EndT);
        ////    return ret;
        ////}

        
    }

    public class CommIndexTool
    {
        public static List<Type> Allndices()
        {
            List<Type> ret = new List<Type>();
            Type secT = typeof(SecIndexClass);
            Type[] types =  secT.Assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if(secT.IsAssignableFrom(types[i]))//secindex子类
                {
                    ret.Add(types[i]);
                }
            }
            return ret;
        }
    }

    /// <summary>
    /// 全市场指数
    /// </summary>
    public class AllMarketEquitClass : SecIndexClass
    {
        public AllMarketEquitClass()
        {
            SummaryCode = "a001010100000000";
        }
    }

    /// <summary>
    /// 万得概念指数
    /// </summary>
    public class WDConceptClass : SecIndexClass
    {
        public WDConceptClass()
        {
            SummaryCode = "a39901012c000000";
        }
    }

    #region 各行业指数

    /// <summary>
    /// 申万一级行业类
    /// </summary>
    public class SIIClass : SecIndexClass
    {
        public SIIClass()
        {
            SummaryCode = "a39901011g000000";
        }


    }
    /// <summary>
    /// 申万二级行业类
    /// </summary>
    public  class SIIIClass : SecIndexClass
    {
        public SIIIClass()
        {
            SummaryCode = "a39901011h000000";
        }


    }
    

    /// <summary>
    /// 中信证券一级行业指数
    /// </summary>
    public class CSIClass : SecIndexClass
    {
        public CSIClass()
        {
            SummaryCode = "a39901012e000000";
        }
    }
    /// <summary>
    /// 中信证券二级行业指数
    /// </summary>
    public class CSIIClass : SecIndexClass
    {
        public CSIIClass()
        {
            SummaryCode = "a39901012f000000";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CSRCIClass : SecIndexClass
    {
        public CSRCIClass()
        {
            SummaryCode = "a39901012f000000";
        }
    }
    #endregion 

    /// <summary>
    /// 行业指数类型
    /// </summary>
    public enum IndustryType
    {
        /// <summary>
        /// 中国证监会行业分类
        /// </summary>
        CSRCI,
        /// <summary>
        /// 申万行业分类I级
        /// </summary>
        SWI1,
        /// <summary>
        /// 申万行业分类II级
        /// </summary>
        SWI2,
        /// <summary>
        /// 中信行业分级I级
        /// </summary>
        CSI1,
        /// <summary>
        /// 中信行业分级II级
        /// </summary>
        CSI2
    }

    
    /// <summary>
    /// 通用指数接口
    /// </summary>
    public interface IWDIndexSet
    {
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace WolfInv.com.SecurityLib
{
    //通用股票指数类
    public class CommSecIndexClass: IIndexSet
    {
        public string SummaryCode;
        public string IndexCode;
        public CommSecIndexClass()
        {
        }

        public CommSecIndexClass(string sectorid)
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

    public class IndexTool
    {
        public static List<Type> Allndices()
        {
            List<Type> ret = new List<Type>();
            Type secT = typeof(CommSecIndexClass);
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
    public class CommAllMarketEquitClass : CommSecIndexClass
    {
        public CommAllMarketEquitClass()
        {
            SummaryCode = "a001010100000000";
        }
    }

    /// <summary>
    /// 万得概念指数
    /// </summary>
    public class CommConceptClass : CommSecIndexClass
    {
        public CommConceptClass()
        {
            SummaryCode = "a39901012c000000";
        }
    }

    #region 各行业指数

    /// <summary>
    /// 申万一级行业类
    /// </summary>
    public class CommSIIClass : CommSecIndexClass
    {
        public CommSIIClass()
        {
            SummaryCode = "a39901011g000000";
        }


    }
    /// <summary>
    /// 申万二级行业类
    /// </summary>
    public  class CommSIIIClass : CommSecIndexClass
    {
        public CommSIIIClass()
        {
            SummaryCode = "a39901011h000000";
        }


    }
    

    /// <summary>
    /// 中信证券一级行业指数
    /// </summary>
    public class CommCSIClass : CommSecIndexClass
    {
        public CommCSIClass()
        {
            SummaryCode = "a39901012e000000";
        }
    }
    /// <summary>
    /// 中信证券二级行业指数
    /// </summary>
    public class CommCSIIClass : CommSecIndexClass
    {
        public CommCSIIClass()
        {
            SummaryCode = "a39901012f000000";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CommCSRCIClass : CommSecIndexClass
    {
        public CommCSRCIClass()
        {
            SummaryCode = "a39901012f000000";
        }
    }
    #endregion 

    /// <summary>
    /// 行业指数类型
    /// </summary>
    public enum CommIndustryType
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
    public interface IIndexSet
    {
        
    }
}

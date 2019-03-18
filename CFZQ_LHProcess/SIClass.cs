using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFZQ_LHProcess
{
    //通用股票指数类
    public class SecIndexClass:IWDIndexSet
    {
        public string SummaryCode;
    }
    /// <summary>
    /// 申万二级行业类
    /// </summary>
    public class SIIIClass : SecIndexClass
    {
        public SIIIClass()
        {
            SummaryCode = "a39901011h000000";
        }

        public SIIIClass(string sectorid)
        {
            if(sectorid != null && sectorid.Trim().Length>0)
                SummaryCode = sectorid;
        }
    }
    /// <summary>
    /// 通用指数接口
    /// </summary>
    public interface IWDIndexSet
    {
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.StrategyLibForWD;
using WolfInv.com.SecurityLib;
using WolfInv.com.CFZQ_LHProcess;
using System.Data;
using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.StockStrategiesLib
{
    interface IDebugable
    {
        void Debug(string txt);
    }
    public class DebugableClass:IDebugable
    {
        bool _Enabled;
        StringBuilder _DebugMessages;
        public bool Enabled
        {
            get;
            set;
        }
        public StringBuilder DebugMessages
        {
            get;
            set;
        }
        public void Debug(string txt)
        {
            if (_DebugMessages == null)
            {
                _DebugMessages = new StringBuilder();
            }
            _DebugMessages.Append(txt);
        }
    }
    public class ArrayableClass : DebugableClass
    {
        public List<string> Echo()
        {
            List<string> ret = new List<string>();
            return ret;
        }

        public string ToClassInfo(ArrayableClass clsobj)
        {
            return "";
        }
    }
    public abstract class SelectBase : ArrayableClass
    {
        /// <summary>
        /// 股票池
        /// </summary>
        public List<string> SecsPool;
        /// <summary>
        /// 选股范围
        /// </summary>
        public string SecIndex;
        public DateTime EndT;
        public Cycle Cyc;
        public PriceAdj Rate;
        /// <summary>
        /// 上市天数
        /// </summary>
        public long OnMarketDays;
        /// <summary>
        /// 前推日期数
        /// </summary>
        public long FareViewDays;
        /// <summary>
        /// 是否排除ST股票
        /// </summary>
        public bool IsExcludeST;
        /// <summary>
        /// 黑名单
        /// </summary>
        public List<string> ExcludeSecList;
        /// <summary>
        /// 是否20日均线过滤
        /// </summary>
        public bool IsMAFilter;
        /// <summary>
        /// 最大选股数量
        /// </summary>
        public int TopN;
        public RunResultClass Exec()
        {
            RunResultClass ret = new RunResultClass();
            RunNoticeClass rc = CheckInput();
            if (!rc.Success)
            {
                ret.Notice = rc;
                return ret;
            }
            return ret;
        }

        /// <summary>
        /// 参数检查
        /// </summary>
        /// <returns></returns>
        public RunNoticeClass CheckInput()
        {
            RunNoticeClass ret = new RunNoticeClass();
            if((SecsPool == null || SecsPool.Count ==0) && (SecIndex == null || SecIndex.Trim().Length == 0))
            {
                ret.Msg = "备选池和指数不能同时为空！";
                return ret;
            }
            ret.Success = true;
            return ret;
        }

        /// <summary>
        /// 基础过滤
        /// </summary>
        public void BaseFilter()
        {
            ///
        }

        
    }
    
    /// <summary>
    /// 运行通知类
    /// </summary>
    public class RunNoticeClass
    {
        public bool Success;
        public StringBuilder DebugMsg;
        public string Msg;
    }

    /// <summary>
    /// 运行结果类
    /// </summary>
    public class RunResultClass 
    {
        public RunNoticeClass Notice;
        public DataTable Result;
    }
}

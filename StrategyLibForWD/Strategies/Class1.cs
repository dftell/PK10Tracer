using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using CFZQ_JRGCDB;
using WolfInv.com.CFZQ_LHProcess;
using WAPIWrapperCSharp;
using System.Data;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.StrategyLibForWD
{
    /// <summary>
    /// 策略输入业务参数类
    /// </summary>
    public class StrategyInClass
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
        public PriceAdj prcAdj;
        /// <summary>
        /// 上市天数
        /// </summary>
        public Int64 OnMarketDays;
        /// <summary>
        /// 当日计算前日数据
        /// </summary>
        public bool CalcLastData;
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

    }

    /// <summary>
    /// 运行通知类
    /// </summary>
    public class RunNoticeClass
    {
        public bool Success;
        public StringBuilder DebugMsg;
        public string Msg;
        public RunNoticeClass()
        {
            Success = false;
            DebugMsg = new StringBuilder();
            Msg = null;
        }
    }

    /// <summary>
    /// 运行结果类
    /// </summary>
    public class RunResultClass
    {
        public RunNoticeClass Notice;
        public MTable Result;
        public RunResultClass()
        {
            Notice = new RunNoticeClass();
            Result = new MTable();
        }
    }

    interface iReadSecuritySerialData
    {
        BaseDataTable ReadSecuritySerialData(string Code);
    }
    /// <summary>
    /// 选股策略基类
    /// </summary>
    public abstract class StrategyBaseClass : WDBuilder, iBalanceMethod, iBreachMethod, iReverseMethod, iReadSecuritySerialData
    {
        public StrategyBaseClass(WindAPI _w)
            : base(_w)
        {
        }
        public StrategyInClass InParam { get; set; }
        /// <summary>
        /// 策略周期类型
        /// </summary>
        public StrategyCycleType CycType { get; set; }
        /// <summary>
        /// 策略逻辑类型
        /// </summary>
        public StrategyLogicType LogicType { get; set; }

        /// <summary>
        /// 证券代码数组
        /// </summary>
        protected string[] SecList
        {
            get
            {
                if (this.SelectTable != null)
                {
                    return this.SelectTable[":", "Code"].ToList<string>().ToArray();
                }
                return new string[0];
            }
        }
        protected BaseDataTable SelectTable;//证券清单
        
        public RunResultClass Execute()
        {
            RunResultClass ret = new RunResultClass();
            RunNoticeClass msg = InParamsCheck(InParam);
            if (!msg.Success)
            {
                ret.Notice = msg;
                return ret;
            }
            msg = new RunNoticeClass();
            msg = BaseFilter();
            if (!msg.Success)
            {
                ret.Notice = msg;
                return ret;
            }
            msg = BaseFilter();
            if (!msg.Success)
            {
                ret.Notice = msg;
                return ret;
            }
            return ExecSelect();
        }
        
        public RunNoticeClass InParamsCheck(StrategyInClass Input)
        {
            RunNoticeClass ret = new RunNoticeClass();
            if (InParam == null)
            {
                ret.Msg = "输入参数对象不能为空！";
                return ret;
            }
            if ((InParam.SecsPool == null || InParam.SecsPool.Count == 0) && (InParam.SecIndex == null || InParam.SecIndex.Trim().Length == 0))
            {
                ret.Msg = "备选池和指数不能同时为空！";
                return ret;
            }
            return ret;
        }

        /// <summary>
        /// 基础过滤
        /// </summary>
        public RunNoticeClass BaseFilter()
        {
            RunNoticeClass ret = new RunNoticeClass();
            BaseDataTable sectab = CommWDToolClass.GetMarketsStocks(w, InParam.SecIndex, InParam.EndT, InParam.OnMarketDays, InParam.CalcLastData, InParam.IsExcludeST, InParam.IsMAFilter, InParam.ExcludeSecList);
            this.SelectTable = sectab;
            this.SelectTable.AddColumnByArray<bool>("Enable", false);
            return ret;
        }

        public RunResultClass ExecSelect()
        {
            RunResultClass ret = new RunResultClass();
            for (int i = 0; i < this.SelectTable.Count; i++)
            {
                SecurityProcessClass spc = SingleSecPreProcess((BaseDataItemClass)this.SelectTable[i]);
                this.SelectTable[i, "Enable"] = spc.Enable;
            }
            return LastProcess(this.SelectTable);
        }

        public SecurityProcessClass SingleSecPreProcess(BaseDataItemClass dr)
        {
            SecurityProcessClass ret = new SecurityProcessClass(dr);
            switch (LogicType)
            {
                case StrategyLogicType.Reverse:
                    ret = this.ReverseSelectSecurity(InParam);
                    break;
                case StrategyLogicType.Breach:
                    ret = this.BreachSelectSecurity(InParam);
                    break;
                case StrategyLogicType.Balance:
                    ret = this.BalanceSelectSecurity(InParam);
                    break;
                default:
                    break;
                
            }
            return ret;
        }

        public RunResultClass LastProcess(MTable mt)
        {
            RunResultClass ret = new RunResultClass();
            ret.Result = mt[string.Format(":{0}" ,InParam.TopN-1), "*"];
            return ret;
        }

        public abstract SecurityProcessClass BalanceSelectSecurity(StrategyInClass Input);

        public abstract SecurityProcessClass BreachSelectSecurity(StrategyInClass Input);

        public abstract SecurityProcessClass ReverseSelectSecurity(StrategyInClass Input);

        public abstract BaseDataTable ReadSecuritySerialData(string Code);
    }

    /// <summary>
    /// 策略周期类型
    /// </summary>
    public enum StrategyCycleType
    {
        /// <summary>
        /// 单周期
        /// </summary>
        SingleCycle, 
        /// <summary>
        /// 多周期
        /// </summary>
        MultiCycle
    }

    /// <summary>
    /// 策略逻辑类型
    /// </summary>
    public enum StrategyLogicType
    {
        /// <summary>
        /// 反转性逻辑
        /// </summary>
        Reverse,
        /// <summary>
        /// 突破性逻辑
        /// </summary>
        Breach,
        /// <summary>
        /// 平衡性逻辑
        /// </summary>
        Balance
    }

    public class SecurityProcessClass
    {

        public BaseDataItemClass SecInfo;
        public bool Enable;
        public SecurityProcessClass()
        {
        }

        public SecurityProcessClass(BaseDataItemClass dr)
        {
            if (dr == null) return;
            SecInfo = dr;
            Enable = false;
        }
    }
}
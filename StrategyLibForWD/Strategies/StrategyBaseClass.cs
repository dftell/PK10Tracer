using System.Linq;
//using CFZQ_JRGCDB;
using WolfInv.com.CFZQ_LHProcess;
using WAPIWrapperCSharp;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.StrategyLibForWD
{
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
            BaseDataTable sectab = CommWSSToolClass.GetMarketsStocks(w, InParam.SecIndex, InParam.EndT, InParam.OnMarketDays, InParam.CalcLastData, InParam.IsExcludeST, InParam.IsMAFilter, InParam.ExcludeSecList);
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
}
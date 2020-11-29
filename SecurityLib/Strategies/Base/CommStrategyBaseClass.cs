using System.Linq;
//using CFZQ_JRGCDB;

using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using System.Collections.Generic;
namespace WolfInv.com.SecurityLib
{
    /// <summary>
    /// 选股策略基类
    /// </summary>
    public abstract class CommStrategyBaseClass<T> : CommDataBuilder<T>, iCommBalanceMethod<T>, iCommBreachMethod<T>, iCommReverseMethod<T>, iCommReadSecuritySerialData where T:TimeSerialData
    {
        public CommStrategyBaseClass(CommDataIntface<T> _w)
            : base(_w)
        {
            SelectTable = _w.getData();
        }
        public CommStrategyInClass InParam { get; set; }
        /// <summary>
        /// 策略周期类型
        /// </summary>
        public CommStrategyCycleType CycType { get; set; }
        /// <summary>
        /// 策略逻辑类型
        /// </summary>
        public CommStrategyLogicType LogicType { get; set; }

        /// <summary>
        /// 证券代码数组
        /// </summary>
        protected string[] SecList
        {
            get
            {
                if (this.SelectTable != null)
                {
                    return this.SelectTable.Keys.ToArray();
                }
                return new string[0];
            }
        }
        //protected BaseDataTable SelectTable;//证券清单
        protected MongoDataDictionary<T> SelectTable;//证券清单

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
        
        public RunNoticeClass InParamsCheck(CommStrategyInClass Input)
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
            //BaseDataTable sectab = CommWDToolClass.GetMarketsStocks(w, InParam.SecIndex, InParam.EndT, InParam.OnMarketDays, InParam.CalcLastData, InParam.IsExcludeST, InParam.IsMAFilter, InParam.ExcludeSecList);
            //this.SelectTable = sectab;
            //this.SelectTable;// AddColumnByArray<bool>("Enable", false);
            return ret;
        }

        public RunResultClass ExecSelect()
        {
            RunResultClass ret = new RunResultClass();
            foreach (string key in this.SelectTable.Keys)
            {
                CommSecurityProcessClass<T> spc = SingleSecPreProcess(key,this.SelectTable[key]);
                //this.SelectTable[i, "Enable"] = spc.Enable;
                this.SelectTable[key].Disable = !spc.Enable;
            }
            return LastProcess(this.SelectTable);
        }

        public CommSecurityProcessClass<T> SingleSecPreProcess(string key,MongoReturnDataList<T> dr)
        {
            CommSecurityProcessClass<T> ret = new CommSecurityProcessClass<T>(dr.SecInfo,dr);
            CommStrategyInClass OneIn = InParam;
            OneIn.SecIndex = key;
            OneIn.SecsPool = new List<string>();
            switch (LogicType)
            {
                case CommStrategyLogicType.Reverse:
                    ret = this.ReverseSelectSecurity(OneIn);
                    break;
                case CommStrategyLogicType.Breach:
                    ret = this.BreachSelectSecurity(OneIn);
                    break;
                case CommStrategyLogicType.Balance:
                    ret = this.BalanceSelectSecurity(OneIn);
                    break;
                default:
                    break;
                
            }
            return ret;
        }

        public RunResultClass LastProcess(MongoDataDictionary<T> mt)
        {
            RunResultClass ret = new RunResultClass();
            ret.Result = mt.Where(a=>!a.Value.Disable).Select(a=>a.Key).ToList();// mt[string.Format(":{0}" ,InParam.TopN-1), "*"];
            return ret;
        }

        public abstract CommSecurityProcessClass<T> BalanceSelectSecurity(CommStrategyInClass Input);

        public abstract CommSecurityProcessClass<T> BreachSelectSecurity(CommStrategyInClass Input);

        public abstract CommSecurityProcessClass<T> ReverseSelectSecurity(CommStrategyInClass Input);

        public abstract BaseDataTable ReadSecuritySerialData(string Code);
    }
}
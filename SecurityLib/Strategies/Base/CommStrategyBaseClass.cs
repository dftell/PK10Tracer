using System.Linq;
//using CFZQ_JRGCDB;

using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace WolfInv.com.SecurityLib
{
    public delegate void FinishedEvent(string key);
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
        public FinishedEvent afterSelectSingleSecurity;

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
        protected ConcurrentDictionary<string, MongoReturnDataList<T>> SelectTable;//证券清单

        protected RunResultClass FilterResult;//中间过滤结果

        public RunResultClass Execute()
        {
            RunResultClass ret = new RunResultClass();
            RunNoticeClass msg = InParamsCheck(InParam);
            if (!msg.Success)
            {
                ret.Notice = msg;
                return ret;
            }
            int noFilterCnt = this.SelectTable.Count;
            ret = BaseFilter();
            msg = ret.Notice;
            if (!msg.Success)
            {
                ret.Notice = msg;
                return ret;
            }
            int afterFilterCnt = ret.Result.Count;
            return ExecSelect(ret);
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
            ret.Success = true;
            return ret;
        }

        /// <summary>
        /// 基础过滤,过滤掉检查日停牌股票
        /// </summary>
        public RunResultClass BaseFilter()
        {
            RunResultClass ret = new RunResultClass();
            //MongoDataDictionary<T> filterResult = new MongoDataDictionary<T>(true);
            foreach(string key in this.SelectTable.Keys)
            {
                if(this.SelectTable[key].Last().Expect == InParam.EndExpect)
                {
                    if(string.IsNullOrEmpty(this.SelectTable[key].SecInfo.name))//过滤掉各类参考指数
                    {
                        continue;
                    }
                    if(InParam.IsExcludeST)
                    {
                        if(this.SelectTable[key].SecInfo.name.ToUpper().Contains("ST"))
                        {
                            continue;
                        }
                    }
                    SelectResult sr = new SelectResult();
                    sr.Enable = true;
                    sr.Key = key;
                    ret.Result.Add(sr);
                    //if(!filterResult.ContainsKey(key))
                    //    filterResult.TryAdd(key, this.SelectTable[key]);
                }
            }
            //this.SelectTable = filterResult;
            //BaseDataTable sectab = CommWDToolClass.GetMarketsStocks(w, InParam.SecIndex, InParam.EndT, InParam.OnMarketDays, InParam.CalcLastData, InParam.IsExcludeST, InParam.IsMAFilter, InParam.ExcludeSecList);
            //this.SelectTable = sectab;
            //this.SelectTable;// AddColumnByArray<bool>("Enable", false);
            ret.Notice = new RunNoticeClass();
            ret.Notice.Success= true;
            //GC.Collect();
            return ret;
        }

        public RunResultClass ExecSelect(RunResultClass filterValues)
        {
            RunResultClass ret = new RunResultClass();
            //改写成多线程
            /*
           foreach (string key in this.SelectTable.Keys)
           {
               CommSecurityProcessClass<T> spc = SingleSecPreProcess(key, this.SelectTable[key]);
               if(spc.Result.Enable)
                {
                    ret.Result.Add(spc.Result);
                }
           }
            
           */
            ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
            WolfTaskClass.MultiTaskProcess<string, MongoReturnDataList<T>,string,string>(filterValues.Result.Select(a=>a.Key).ToList(), this.SelectTable,
              this.SelectTable.Keys.ToDictionary(a=>a,a=>a),
              (k,v,ms,subset,notice) =>
              {
                  if(v==null)
                  {
                      return;
                  }
                  SelectResult spc = SingleSecPreProcess(k,v);
                
                  if(spc.Enable)
                  {
                      //LockSlim.EnterWriteLock();
                      ret.Result.Add(spc);
                      //LockSlim.ExitWriteLock();
                  }
                  notice.Invoke(k);
                  spc = null;
                  
              },
              (k) =>
              {
                  Task.Factory.StartNew(() =>
                  {
                      afterSelectSingleSecurity?.Invoke(k);
                  });
              },
              10
              , 5,true);    
            return LastProcess(ret);
        }

        public SelectResult SingleSecPreProcess(string key,MongoReturnDataList<T> dr)
        {
            SelectResult ret = new SelectResult();
            //CommSecurityProcessClass<T> ret = new CommSecurityProcessClass<T>();
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

        public RunResultClass LastProcess(RunResultClass mt)
        {
            RunResultClass ret = new RunResultClass();
            var items = mt.Result.Where(a => a.Enable == true);
            if (items.Count() > 0)
                items = items.OrderByDescending(a => a.Weight).Take(InParam.TopN);
            if(items.Count()>0)
                ret.Result =  items.ToList();// mt[string.Format(":{0}" ,InParam.TopN-1), "*"];
            return ret;
        }

        public abstract SelectResult BalanceSelectSecurity(CommStrategyInClass Input);

        public abstract SelectResult BreachSelectSecurity(CommStrategyInClass Input);

        public abstract SelectResult ReverseSelectSecurity(CommStrategyInClass Input);

        public abstract BaseDataTable ReadSecuritySerialData(string Code);
    }
}
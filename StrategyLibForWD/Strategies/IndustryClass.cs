using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.CFZQ_LHProcess;
using System.Data;
using WAPIWrapperCSharp;
using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.StrategyLibForWD
{
    #region 基础类及接口
    /// <summary>
    /// 行业类
    /// </summary>
    public abstract class IndustryClass:SecIndexClass
    {
        public IndustryClass()
        {
        }
    }
    
    /// <summary>
    /// 申万二级行业类
    /// </summary>
    public class SWIndustryClassII : IndustryClass
    {
        public SWIndustryClassII()
        {
        }
        ////public MTable getBkbyDate(DateTime EndT)
        ////{
        ////    return getBkbyDate(null, EndT);
        ////}
    }
    
    /// <summary>
    /// 行业选择策略
    /// </summary>
    public abstract class IndustryStrategy : IndexStrategy, iIndustryStrategy
    {
        public IndustryStrategy(WindAPI _w) : base(_w) { }
        

        
    }
    /// <summary>
    /// 指数/组合选择策略
    /// </summary>
    public abstract class IndexStrategy : StrategyBaseClass
    {
        public IndexStrategy(WindAPI _w) : base(_w) { }
    }
    public class IndustryStrategyInParams:StrategyInClass
    {
        public IndustryType industryType { get; set; }
        public IndustryStrategyInParams()
        {
            industryType = IndustryType.SWI2;//默认使用申万二级
            prcAdj = PriceAdj.Fore;
            Cyc = Cycle.Week;
        }
    }

    public class MutliCycleIndustryStrategyInParams : IndustryStrategyInParams
    {
        public string IndexName { get; set; }
        public List<string> InputList { get; set; }
    }

    public class IndustryStrategyOutResult : RunResultClass
    {
        public IndustryStrategyOutResult():base()
        {
        }
        public MTable SubSecuityList { get; set; }
    }

    
    public interface iIndustryStrategy:iReverseMethod,iBreachMethod,iBalanceMethod
    {
        
    }
    #endregion

 
    /// <summary>
    /// 多周期行业选股策略类
    /// </summary>
    public class IndustryMutliCycleStrategy : IndustryStrategy
    {
        public IndustryMutliCycleStrategy(WindAPI _w) : base(_w) { }
        public override SecurityProcessClass ReverseSelectSecurity(StrategyInClass Input)
        {
            MutliCycleIndustryStrategyInParams InParam = null;
            return null;
        }

        public override SecurityProcessClass BreachSelectSecurity(StrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override SecurityProcessClass BalanceSelectSecurity(StrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override BaseDataTable ReadSecuritySerialData(string Code)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 申万二级行业选股
    /// </summary>
    public class SI2IndustryStrategy:IndustryStrategy
    {
        public SI2IndustryStrategy(WindAPI _w) : base(_w) { }
        public override SecurityProcessClass ReverseSelectSecurity(StrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override SecurityProcessClass BreachSelectSecurity(StrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override SecurityProcessClass BalanceSelectSecurity(StrategyInClass Input)
        {
            throw new NotImplementedException();
        }





        public override BaseDataTable ReadSecuritySerialData(string Code)
        {
            throw new NotImplementedException();
        }
    }
}

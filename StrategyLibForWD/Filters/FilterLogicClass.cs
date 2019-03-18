using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.CFZQ_LHProcess;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.StrategyLibForWD
{

    public delegate BaseDataTable CallBackData(string SecCode,int cnt);

    /// <summary>
    /// 过滤逻辑基类
    /// </summary>
    public abstract class FilterLogicBaseClass : iCallBackWDable
    {
        public StrategyBaseClass ExecStrategy;
        public BaseDataItemClass BaseInfo;
        public string secCode;
        public DateTime Endt;
        public PriceAdj Rate;
        public Cycle Cycle;
        /// <summary>
        /// 是否右侧选证券
        /// </summary>
        public bool IsRightSelect;
        public SecurityProcessClass SecObj;
        public string FilterSubFunc;
        /// <summary>
        /// 回览视窗周期数
        /// </summary>
        public int PassViewDays;
        /// <summary>
        /// 缓冲周期数
        /// </summary>
        public int BuffDays;

        public FilterLogicBaseClass(SecurityProcessClass secinfo)
        {
            SecObj = secinfo;
        }

        public FilterLogicBaseClass(Cycle cyc,PriceAdj rate )
        {
            Cycle = cyc;
            Rate = rate;
        }


        public virtual SecurityProcessClass ExecFilter()
        {
            return SecObj;
        }

        public abstract BaseDataTable GetData(int RecordCnt);
    }

    public class FilterResult 
    {
        public bool Enalbe;
        public StringBuilder Msg;
        public decimal[] DisplayValues;
        public FilterResult()
        {
            Msg = new StringBuilder();
        }
    }

    public interface iCallBackWDable
    {
       BaseDataTable GetData(int RecordCnt);
    }
}

using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    /// <summary>
    /// 过滤逻辑基类
    /// </summary>
    public abstract class CommFilterLogicBaseClass<T> : iCommCallBackable where T:TimeSerialData
    {
        protected MongoDataDictionary<T> AllSecs;
        public CommStrategyBaseClass<T> ExecStrategy { get; set; }
        public BaseDataItemClass BaseInfo;
        public string secCode;
        protected string EndExpect;
        public PriceAdj Rate;
        public Cycle Cycle;
        protected double[] zeroLines; 
        /// <summary>
        /// 是否右侧选证券
        /// </summary>
        public bool IsRightSelect;
        public CommSecurityProcessClass<T> SecObj;
        public string FilterSubFunc;
        /// <summary>
        /// 回览视窗周期数
        /// </summary>
        public int PassViewDays;
        /// <summary>
        /// 缓冲周期数
        /// </summary>
        public int BuffDays;


        protected KLineData<T>  kLineData;

        public CommFilterLogicBaseClass(string endExpect, CommSecurityProcessClass<T> secinfo,PriceAdj priceAdj= PriceAdj.Fore, Cycle cyc= Cycle.Day)
        {
            EndExpect = endExpect;
            SecObj = secinfo;
            Cycle = cyc;
            Rate = priceAdj;
            kLineData = new KLineData<T>(EndExpect, secinfo.SecPriceInfo,priceAdj,cyc);
        }
        public CommFilterLogicBaseClass(string endExpect, MongoDataDictionary<T> allData, CommSecurityProcessClass<T> secinfo, PriceAdj priceAdj = PriceAdj.Fore, Cycle cyc = Cycle.Day)
        {
            EndExpect = endExpect;
            AllSecs = allData;
            SecObj = secinfo;
            Cycle = cyc;
            Rate = priceAdj;
            kLineData = new KLineData<T>(EndExpect, this.SecObj.SecPriceInfo,priceAdj,cyc);
        }
        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="cyc"></param>
        /// <param name="rate"></param>
        public CommFilterLogicBaseClass(Cycle cyc,PriceAdj rate )
        {
            Cycle = cyc;
            Rate = rate;
        }


        public virtual SelectResult ExecFilter(CommStrategyInClass Input)
        {
            return new SelectResult();
        }

        public abstract BaseDataTable GetData(int RecordCnt);

    }
}

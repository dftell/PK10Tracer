using System;
using WolfInv.com.BaseObjectsLib;
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
        public DateTime Endt;
        public PriceAdj Rate;
        public Cycle Cycle;
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

        public CommFilterLogicBaseClass(CommSecurityProcessClass<T> secinfo)
        {
            SecObj = secinfo;
        }
        public CommFilterLogicBaseClass(MongoDataDictionary<T> allData, CommSecurityProcessClass<T> secinfo)
        {
            AllSecs = allData;
            SecObj = secinfo;
        }

        public CommFilterLogicBaseClass(Cycle cyc,PriceAdj rate )
        {
            Cycle = cyc;
            Rate = rate;
        }


        public virtual CommSecurityProcessClass<T> ExecFilter(CommStrategyInClass Input)
        {
            return SecObj;
        }

        public abstract BaseDataTable GetData(int RecordCnt);

    }
}

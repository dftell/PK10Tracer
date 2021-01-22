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
        public KLineData<T>.getSingleDataFunc getSingleData;
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

        public CommFilterLogicBaseClass(string endExpect, CommSecurityProcessClass<T> secinfo,PriceAdj priceAdj= PriceAdj.Beyond, Cycle cyc= Cycle.Day)
        {
            EndExpect = endExpect;
            SecObj = secinfo;
            Cycle = cyc;
            Rate = priceAdj;
            kLineData = new KLineData<T>(EndExpect, secinfo.SecPriceInfo,priceAdj,cyc);
            
        }
        public CommFilterLogicBaseClass(string endExpect, MongoDataDictionary<T> allData, CommSecurityProcessClass<T> secinfo, PriceAdj priceAdj = PriceAdj.Beyond, Cycle cyc = Cycle.Day)
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

    public abstract class CloseCommFilterLogicBaseClass<T>:CommFilterLogicBaseClass<T>  where T:TimeSerialData
    {
        
        public CloseCommFilterLogicBaseClass(string endExpect, CommSecurityProcessClass<T> secinfo, PriceAdj priceAdj = PriceAdj.Beyond, Cycle cyc = Cycle.Day) : base(endExpect, secinfo, priceAdj, cyc)
        {

        }
        /// <summary>
        /// 止损处理
        /// </summary>
        /// <returns></returns>
        public abstract SelectResult StopLossProcess(CommStrategyInClass Input);

        protected int  calcZTDays(KLineData<T> line,int startIndex,out int serialZtDays)
        {
            int lastZtIndex=0;
            int ztDays = 0;
            serialZtDays = 0;
            for (int i = startIndex + 1; i < line.Length; i++)
            {
                if (line.IsUpStop(i))//如果涨停
                {
                    ztDays++;
                    if (lastZtIndex == i - 1)
                    {
                        serialZtDays++;
                    }
                    lastZtIndex = i;
                    if (serialZtDays == 0)
                        serialZtDays = 1;
                }
                else
                {
                    lastZtIndex = 0;
                }
            }
            return ztDays;
        }
    }
}

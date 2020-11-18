using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using static WolfInv.com.GuideLib.GuideToolClass;

namespace WolfInv.com.GuideLib.Filter
{
    public class FirstPointClass<T>: FilterBaseClass<T> where T :TimeSerialData
    {
        public FirstPointClass(List<T> data):base(data)
        {

        }
        /*
         ISLT0GREEN:=MACD.DEA>MACD.DIF AND MACD.DEA<0;{当前为绿柱，且DEA小于0}
DEAGT0CNT:=MAX(200,BARSLAST(CROSS(MACD.DEA,0)));
DEALT0CNT:=BARSLAST(CROSS(0,MACD.DEA));
DEACRSCNT:= BARSLAST(CROSS(MACD.DIF,MACD.DEA));{最后一次DEA上穿DIF的天数}
ALLHIGH:=HHV(HIGH,MAX(250,DEAGT0CNT));{最高点}
FONTLOW:=REF(LLV(LOW,DEAGT0CNT-DEACRSCNT),DEACRSCNT);{最后一段上涨或者企稳前的最低点}
MIDHIGH:=HHV(HIGH,DEACRSCNT);{最后一段上涨或者企稳前的最高点}
MIDRANGE:= (MIDHIGH-FONTLOW)/FONTLOW;{中间涨幅}
MINLLV:=LLV(LOW,DEAGT0CNT);
FONTDOWN:=(ALLHIGH-FONTLOW)/ALLHIGH;{前期跌幅}
LASTDOWN:=(MIDHIGH-MINLLV)/MIDHIGH;{最后跌幅}
FONTDOWNGTLAST:=FONTDOWN>LASTDOWN*DOWNRATE;
DEACNTGT60:=DEAGT0CNT>60;
TWISTCNT:=COUNT(CROSS(MACD.DEA,MACD.DIF),DEALT0CNT);
ISLOWPRICE:= LOW<=LLV(LOW,DEAGT0CNT);
MAXDEA :=HHV(MACD.DEA,VWCNT);
NOMACDBOTTOM:=LLV(MACD.MACD,3)>LLV(MACD.MACD,DEALT0CNT);
LASTRANGE:=100* (MIDHIGH-MINLLV)/CLOSE;
ISBOTTOM:=MACD.MACD>REF(MACD.MACD,1) AND REF(MACD.MACD,1)<REF(MACD.MACD,2);
LASTRANGE>EXPECTRANGE AND ISBOTTOM AND NOMACDBOTTOM AND ISLT0GREEN AND DEACNTGT60 AND TWISTCNT>=TWTCNT AND ISLOWPRICE;

             */
        public override bool matched()
        {
            
            //working
            
            MACDCollection macd = klineData.Closes.MACD();
            double[] closes = klineData.Closes;
            //ISLT0GREEN:=MACD.DEA>MACD.DIF AND MACD.DEA<0;{当前为绿柱，且DEA小于0}
            if (macd.MACDs.Last(1) > 0 ||macd.DEAs.Last(1)>0)//macd要为负
                return false;            
            double[] zeroLine = closes.ToConst(0);
            //DEAGT0CNT:= MAX(200, BARSLAST(CROSS(MACD.DEA, 0)));
            int DEAGT0CNT = Math.Max(200, macd.DEAs.LastMatchCondition(zeroLine, GuideToolClass.CrossUp));
            //DEALT0CNT:= BARSLAST(CROSS(0, MACD.DEA));
            int DEALT0CNT = (0.00D).LastMatchCondition(macd.DEAs, GuideToolClass.Cross);
            //DEACRSCNT:= BARSLAST(CROSS(MACD.DIF,MACD.DEA));{最后一次DEA上穿DIF的天数}
            int DEACRSCNT = macd.DIFs.LastMatchCondition(macd.DEAs, GuideToolClass.Cross);
            //ALLHIGH:= HHV(HIGH, MAX(250, DEAGT0CNT));{ 最高点}
            double ALLHIGH = klineData.Highs.LastSector(DEAGT0CNT).HHV();
            if (macd.Count > 2)//macd 要向上
            {
                if (macd.MACDs.Last(1) < macd.MACDs.Last(2))
                {
                    return false;
                }
            }
            return true;
        }
    }
    public class KLineData<T> where T : TimeSerialData
    {
        public int[] Indies;
        public string[] Expects;
        public double[] Closes;
        public double[] Opens;
        public double[] Highs;
        public double[] Lows;
        public double[] Vols;
        public KLineData(List<T> list) 
        {
            if (list == null)
                return;
            Indies = new int[list.Count];
            Expects = new string[list.Count];
            Closes = new double[list.Count];
            Opens = new double[list.Count];
            Highs = new double[list.Count];
            Lows = new double[list.Count];
            Vols = new double[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                StockMongoData smd = list[i] as StockMongoData;
                Indies[i] = i;
                Expects[i] = smd.Expect;
                Closes[i] = smd.close;
                Opens[i] = smd.open;
                Highs[i] = smd.high;
                Lows[i] = smd.low;
                Vols[i] = smd.vol;
            }
        }
    }


    public abstract class FilterBaseClass<T> where T:TimeSerialData
    {
        protected int Length; 
        protected List<T> DataList;
        protected FilterBaseClass(List<T> data)
        {
            DataList = data;
            klineData = new KLineData<T>(data);
            Length = data.Count;
        }
        public abstract bool matched();
        public KLineData<T> klineData;
        }

    /*
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

        public FilterLogicBaseClass(Cycle cyc, PriceAdj rate)
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

    */

}

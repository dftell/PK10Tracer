using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using WolfInv.com.GuideLib.LinkGuid;
using WolfInv.com.SecurityLib.Filters.StrategyFilters;

namespace WolfInv.com.SecurityLib.Strategies.Bussyniess
{
    public class FirstPointFilter_Logic_Strategy<T> : CommStrategyBaseClass<T> where T:TimeSerialData
    {
        public FirstPointFilter_Logic_Strategy(CommDataIntface<T> _w) : base(_w)
        {
        }



        public override CommSecurityProcessClass<T> BalanceSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass<T> BreachSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override BaseDataTable ReadSecuritySerialData(string Code)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 反转选股逻辑
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public override CommSecurityProcessClass<T> ReverseSelectSecurity(CommStrategyInClass Input)
        {
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
TWISTCNT:=COUNT(CROSS(MACD.DEA,MACD.DIF),DEALT0CNT);//缠绕次数
ISLOWPRICE:= LOW<=LLV(LOW,DEAGT0CNT); //最低价
MAXDEA :=HHV(MACD.DEA,VWCNT);
NOMACDBOTTOM:=LLV(MACD.MACD,3)>LLV(MACD.MACD,DEALT0CNT);
LASTRANGE:=100* (MIDHIGH-MINLLV)/CLOSE;
ISBOTTOM:=MACD.MACD>REF(MACD.MACD,1) AND REF(MACD.MACD,1)<REF(MACD.MACD,2);
LASTRANGE>EXPECTRANGE AND ISBOTTOM AND NOMACDBOTTOM AND ISLT0GREEN AND DEACNTGT60 AND TWISTCNT>=TWTCNT AND ISLOWPRICE;

             */
            MongoReturnDataList<T> pricesInfo =  SelectTable[Input.SecIndex];
            CommSecurityProcessClass<T> ret = new CommSecurityProcessClass<T>(pricesInfo.SecInfo,pricesInfo);
            ret.Enable = true;
            CommFilterLogicBaseClass<T> filter = new MinDaysFilter<T>(ret);
            ret = filter.ExecFilter(Input);
            if (!ret.Enable)
                return ret;
            filter = new IsSTFilter<T>(ret);
            ret = filter.ExecFilter(Input);
            if (!ret.Enable)
                return ret;
            filter = new MACDBottomRevFilter<T>(ret);
            ret = filter.ExecFilter(Input);
            if (!ret.Enable)
                return ret;
            List<T> list = SelectTable[Input.SecIndex];
            var klineData = new KLineData<T>(list);
            ///ToDo:反转选股
            ret.Enable = false;
            MACDCollection macd = klineData.Closes.MACD();
            double[] closes = klineData.Closes;
            double[] opens = klineData.Opens;
            
            //ISLT0GREEN:=MACD.DEA>MACD.DIF AND MACD.DEA<0;{当前为绿柱，且DEA小于0}

            double[] zeroLine = (0D).ToConst(closes);
            //DEAGT0CNT:= MAX(200, BARSLAST(CROSS(MACD.DEA, 0)));
            int DEAGT0CNT = macd.DEAs.LastMatchCondition(zeroLine, GuideToolClass.CrossUp);
            if(DEAGT0CNT< InParam.ReferDays)
            {
                return ret;
            }
            DEAGT0CNT = Math.Max(200, DEAGT0CNT);
            //ISLOWPRICE:= LOW <= LLV(LOW, DEAGT0CNT);
            bool ISLOWPRICE = klineData.Lows.LLV(3) <= klineData.Lows.LLV(DEAGT0CNT);
            if (!ISLOWPRICE)
                return ret;
            //DEALT0CNT:= BARSLAST(CROSS(0, MACD.DEA));
            int DEALT0CNT = (0.00D).LastMatchCondition(macd.DEAs, GuideToolClass.Cross);
            if(DEALT0CNT<=0)
            {
                return ret;
            }
            //TWISTCNT:= COUNT(CROSS(MACD.DEA, MACD.DIF), DEALT0CNT);
            int TWISTCNT = macd.DEAs.Count(GuideToolClass.Cross, macd.DIFs, DEALT0CNT);
            if (TWISTCNT < 2)
            {
                return ret;
            }
            
            //DEACRSCNT:= BARSLAST(CROSS(MACD.DIF,MACD.DEA));{最后一次DEA上穿DIF的天数}
            int DEACRSCNT = macd.DIFs.LastMatchCondition(macd.DEAs, GuideToolClass.Cross);
            //ALLHIGH:= HHV(HIGH, MAX(250, DEAGT0CNT));{ 最高点}
            double ALLHIGH = klineData.Highs.LastSector(DEAGT0CNT).HHV();
            //FONTLOW:= REF(LLV(LOW, DEAGT0CNT - DEACRSCNT), DEACRSCNT);{ 最后一段上涨或者企稳前的最低点}
            double FONTLOW = klineData.Ref(DEAGT0CNT).Lows.LLV(DEAGT0CNT - DEACRSCNT);
            //MIDHIGH:= HHV(HIGH, DEACRSCNT);{ 最后一段上涨或者企稳前的最高点}
            double MIDHIGH = klineData.Highs.HHV(DEACRSCNT);
            //MIDRANGE:= (MIDHIGH - FONTLOW) / FONTLOW;{ 中间涨幅}
            double MIDRANGE = (MIDHIGH - FONTLOW) / FONTLOW;
            //MINLLV:= LLV(LOW, DEAGT0CNT);
            double MINLLV = klineData.Lows.LLV(DEAGT0CNT);
            //FONTDOWN:= (ALLHIGH - FONTLOW) / ALLHIGH;{ 前期跌幅}
            double FONTDOWN = (ALLHIGH - FONTLOW) / ALLHIGH;
            //LASTDOWN:= (MIDHIGH - MINLLV) / MIDHIGH;{ 最后跌幅}
            double LASTDOWN = (MIDHIGH - MINLLV) / MIDHIGH;
            //FONTDOWNGTLAST:= FONTDOWN > LASTDOWN * DOWNRATE;
            bool FONTDOWNGTLAST = FONTDOWN > LASTDOWN * 1.5;
            if (!FONTDOWNGTLAST)
                return ret;
            //MAXDEA:= HHV(MACD.DEA, VWCNT);
            double MAXDEA = macd.DEAs.HHV(200);
            //NOMACDBOTTOM:= LLV(MACD.MACD, 3) > LLV(MACD.MACD, DEALT0CNT);
            //LASTRANGE:= 100 * (MIDHIGH - MINLLV) / CLOSE;
            double LASTRANGE = 100 * (MIDHIGH - MINLLV) / closes.Last(1);
            //ISBOTTOM:= MACD.MACD > REF(MACD.MACD, 1) AND REF(MACD.MACD,1)< REF(MACD.MACD, 2);
            //LASTRANGE>EXPECTRANGE AND ISBOTTOM AND NOMACDBOTTOM AND ISLT0GREEN AND DEACNTGT60 AND TWISTCNT>=TWTCNT AND ISLOWPRICE;
            if(LASTRANGE < 10)
            {
                return ret;
            }
            ret.Enable = true;
            return ret;
            //Input.SecsPool;

        }
    }
}

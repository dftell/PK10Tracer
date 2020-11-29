using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    /// <summary>
    /// 最小长度滤网
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MACDTopRevFilter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public MACDTopRevFilter(CommSecurityProcessClass<T> cpc):base(cpc)
        {

        }
        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass<T> ExecFilter(CommStrategyInClass Input)
        {
            List<T> list = this.SecObj.SecPriceInfo;
            var klineData = new KLineData<T>(list);
            ///ToDo:反转选股
            CommSecurityProcessClass<T> ret = SecObj;
            ret.Enable = false;
            MACDCollection macd = klineData.Closes.MACD();
            double[] closes = klineData.Closes;
            double[] opens = klineData.Opens;
            //ISLT0GREEN:=MACD.DEA>MACD.DIF AND MACD.DEA<0;{当前为红柱}
            if (macd.MACDs.Last(1) < 0 || macd.DEAs.Last(1) < 0)//macd和dea要为正，非正，返回
                return ret;
            if (Math.Max(closes.Last(2), opens.Last(2)) < closes.Last(1))//股票还在上涨，没有低于开收盘价
            {
                return ret;
            }
            if (macd.MACDs.Last(1) > macd.MACDs.Last(2))//macd还在上升，返回
            {
                return ret;
            }
            double[] zeroLine = (0D).ToConst(closes);
            //DEAGT0CNT:= MAX(200, BARSLAST(CROSS(MACD.DEA, 0)));
            int DEAGT0CNT = macd.DEAs.LastMatchCondition(zeroLine, GuideToolClass.CrossUp);
            if (DEAGT0CNT < Input.ReferDays)
            {
                return ret;
            }
            //最后一次DEA为正的周期数
            int lastDEAGT0Days = macd.DEAs.LastMatchCondition(zeroLine, GuideToolClass.Cross);
            //DEA为正以来的最大MACD


            //最后一次红柱产生周期数
            int lastMacdRedDays = macd.DIFs.LastMatchCondition(macd.DEAs, GuideToolClass.Cross);
             //最近buffs天数的最大收盘价格
            double buffsHigh = klineData.Highs.LLV(Input.BuffDays);
            double buffsClose = Math.Max(klineData.Opens.HHV(Input.BuffDays), closes.HHV(Input.BuffDays));


            DEAGT0CNT = Math.Max(200, DEAGT0CNT);
            //ISLOWPRICE:= LOW <= LLV(LOW, DEAGT0CNT);
            //如果最近buffs日内最高价或者收盘价为前期最高最高价或者最高收盘价，价格已经到顶了
            bool ISTopPRICE = (klineData.Highs.HHV(Input.BuffDays) >= klineData.Highs.HHV(DEAGT0CNT) || klineData.Closes.HHV(Input.BuffDays) >= klineData.Closes.HHV(DEAGT0CNT)) ;
            if (!ISTopPRICE)
                return ret;
            //DEALT0CNT:= BARSLAST(CROSS(0, MACD.DEA));
            int DEALT0CNT = (0.00D).LastMatchCondition(macd.DEAs, GuideToolClass.Cross);
            if (DEALT0CNT <= 0)
            {
                return ret;
            }
            //TWISTCNT:= COUNT(CROSS(MACD.DEA, MACD.DIF), DEALT0CNT);            
            double FONTMACD = macd.MACDs.LLV(DEALT0CNT);//下跌以来最低MACD
            double CURRMACD = macd.MACDs.LLV(3);        //下跌以来当前时段MACD
            double FONTPRICE = klineData.Closes.LLV(DEALT0CNT);//下跌以来最低价格
            double CURRPRICE = klineData.Closes.LLV(3);//下跌以来当前时段价格
            if (CURRMACD < FONTMACD && CURRPRICE >= FONTPRICE)//顶背离
            {

            }
            else
            {
                return ret;
            }
            ret.Enable = true;
            return ret;
        }
    }
    public class StopLossFilter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public StopLossFilter(CommSecurityProcessClass<T> cpc) : base(cpc)
        {

        }
        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override CommSecurityProcessClass<T> ExecFilter(CommStrategyInClass Input)
        {
            KLineData<T> klineData = new KLineData<T>(this.SecObj.SecPriceInfo);
            DateTime[] dates = klineData.Expects.Select(a=>a.ToDate()).ToArray();
            CommSecurityProcessClass<T> ret = SecObj;
            ret.Enable = false;
            int startDate = dates.IndexOf(Input.StartDate);
            if (startDate < 0)
                return ret;
            double signPrice = klineData.Closes[startDate];
            double lastPrice = klineData.Closes.Last();      
            double rate = 100 * (lastPrice - signPrice) / signPrice;
            if (rate > -10.0)
            {
                return ret;
            }
            ret.Enable = true;
            return ret;
        }
    }

}

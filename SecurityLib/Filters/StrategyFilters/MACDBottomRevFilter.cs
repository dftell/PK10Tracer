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
    public class MACDBottomRevFilter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public MACDBottomRevFilter(CommSecurityProcessClass<T> cpc):base(cpc)
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
            //ISLT0GREEN:=MACD.DEA>MACD.DIF AND MACD.DEA<0;{当前为绿柱，且DEA小于0}
            if (macd.MACDs.Last(1) > 0 || macd.DEAs.Last(1) > 0)//macd要为负
                return ret;
            if (Math.Max(closes.Last(2), opens.Last(2)) > closes.Last(1))
            {
                return ret;
            }
            if (macd.MACDs.Last(1) < macd.MACDs.Last(2))
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
            DEAGT0CNT = Math.Max(200, DEAGT0CNT);
            int MACDLT0CNT = macd.MACDs.LastMatchCondition(zeroLine, GuideToolClass.Cross);//最后一次MACD绿柱长度
            //ISLOWPRICE:= LOW <= LLV(LOW, DEAGT0CNT);
            bool ISLOWPRICE = klineData.Lows.LLV(3) <= klineData.Lows.LLV(DEAGT0CNT);
            if (!ISLOWPRICE)
                return ret;
            //DEALT0CNT:= BARSLAST(CROSS(0, MACD.DEA));
            int DEALT0CNT = (0.00D).LastMatchCondition(macd.DEAs, GuideToolClass.Cross);
            if (DEALT0CNT <= 0)
            {
                return ret;
            }
            //TWISTCNT:= COUNT(CROSS(MACD.DEA, MACD.DIF), DEALT0CNT);            
            double FONTMACD = macd.MACDs.LLV(DEALT0CNT);//下跌以来最低MACD
            double CURRMACD = macd.MACDs.LLV(MACDLT0CNT);        //最后一段MACD绿柱极值
            double FONTPRICE = klineData.Closes.LLV(DEALT0CNT);//下跌以来最低价格
            double CURRPRICE = klineData.Closes.LLV(3);//下跌以来当前时段价格
            if (CURRMACD > FONTMACD && CURRPRICE <= FONTPRICE)//底背离
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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using WolfInv.com.GuideLib.Filter;
using WolfInv.com.GuideLib.LinkGuid;
using static WolfInv.com.GuideLib.GuideToolClass;

namespace WolfInv.com.SecurityLib.Strategies.Bussyniess
{
    public class FirstPointFilter_Logic_Strategy<T> : CommStrategyBaseClass<T> where T:TimeSerialData
    {
        public FirstPointFilter_Logic_Strategy(CommDataIntface _w) : base(_w)
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
            List<T> list = SelectTable[Input.SecIndex];
            var klineData = new KLineData<T>(list);
            ///ToDo:反转选股
            CommSecurityProcessClass<T> ret = new CommSecurityProcessClass<T>();
            ret.Enable = false;
            MACDCollection macd = klineData.Closes.MACD();
            double[] closes = klineData.Closes;
            //ISLT0GREEN:=MACD.DEA>MACD.DIF AND MACD.DEA<0;{当前为绿柱，且DEA小于0}
            if (macd.MACDs.Last(1) > 0 || macd.DEAs.Last(1) > 0)//macd要为负
                return ret;
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
                    return ret;
                }
            }
            ret.Enable = true;
            return ret;
            //Input.SecsPool;
            return ret;
        }
    }
}

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



        public override SelectResult BalanceSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override SelectResult BreachSelectSecurity(CommStrategyInClass Input)
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
        public override SelectResult ReverseSelectSecurity(CommStrategyInClass Input)
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
            SelectResult ret = new SelectResult();
            if(!SelectTable.ContainsKey(Input.SecIndex))
            {
                return ret;
            }
            if(SelectTable[Input.SecIndex] == null)
            {
                return ret;
            }
            MongoReturnDataList<T> pricesInfo = SelectTable[Input.SecIndex];
            CommSecurityProcessClass<T> cspc = new CommSecurityProcessClass<T>(pricesInfo.SecInfo, pricesInfo);
            CommFilterLogicBaseClass<T> filter = new MinDaysFilter<T>(Input.EndExpect, cspc);
            try
            {
                ret.Key = Input.SecIndex;
                ret.Enable = false;
                
                //filter.EndExpect = Input.EndExpect;
                ret = filter.ExecFilter(Input);
                if (!ret.Enable)
                    return ret;
                filter = new IsSTFilter<T>(Input.EndExpect, cspc);
                ret = filter.ExecFilter(Input);
                if (!ret.Enable)
                    return ret;
                filter = new MACDBottomRevFilter<T>(Input.EndExpect, cspc,PriceAdj.Fore,Cycle.Week);
                ret = filter.ExecFilter(Input);
                if (!ret.Enable)
                    return ret;
                return ret;
            }
            catch(Exception ce)
            {
                return ret;
            }
            finally
            {
                pricesInfo = null;
                filter = null;
                cspc = null;
            }
            
        }
    }
}

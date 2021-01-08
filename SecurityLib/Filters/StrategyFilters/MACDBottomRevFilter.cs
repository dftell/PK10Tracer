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
    public class MACDBottomRevFilter<T> : CommFilterLogicBaseClass<T>,iMACDFilter<T> where T : TimeSerialData
    {
        public MACDBottomRevFilter(string endExpect,CommSecurityProcessClass<T> cpc, PriceAdj priceAdj = PriceAdj.Fore, Cycle cyc = Cycle.Day) :base(endExpect,cpc, priceAdj, cyc)
        {

        }
        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override SelectResult ExecFilter(CommStrategyInClass Input)
        {
            SelectResult ret = new SelectResult();
            ret.Key = Input.SecIndex;
            ret.Enable = false;
            MACDCollection macd = kLineData.Closes.MACD();
            MaxMinElementClass<double>[] closeMMArr = null;
            MaxMinElementClass<double>[] MACDMMArr = null;
            MaxMinElementClass<double>[] DEAMMArr = null;
            try
            {
                //List<T> list = this.SecObj.SecPriceInfo;
                //var klineData = new KLineData<T>(EndExpect, this.SecObj.SecPriceInfo,PriceAdj.Beyond);
                this.zeroLines = (0D).ToConst(kLineData.Closes);
                ///ToDo:反转选股
                
                if (1 == 1)
                {
                    //DEA在上，只抄底
                    if (macd.DEAs.Last(1) >= 0)
                    {
                        return ret;
                    }
                    int maxUseLen = zeroLines.LastMatchCondition(macd.DEAs, GuideToolClass.CrossUp);
                    if (maxUseLen < 20)//Input.ReferDays)
                    {
                        //maxUseLen = Input.ReferDays;
                        return ret;
                    }
                    closeMMArr = kLineData.Closes.LastSector(maxUseLen).MaxMinArray<double>(10);
                    MACDMMArr = macd.MACDs.LastSector(maxUseLen).MaxMinArray<double>(5);
                    DEAMMArr = macd.DEAs.LastSector(maxUseLen).MaxMinArray<double>(5);

                    if (MACDMMArr.Last(1).Value < MACDMMArr.Last(2).Value)//macd朝下，不理他
                    {
                        return ret;
                    }
                    if (DEAMMArr.Last(1).Value > DEAMMArr.Last(2).Value)//保证当前趋势向下
                    {
                        return ret;
                    }
                    if (DEAMMArr.Length < 3)
                    {
                        return ret;
                    }
                    if(MACDMMArr.Length<5)
                    {
                        return ret;
                    }
                    //东山再起，最后一次起的最高或者相当高
                    if(MACDMMArr.Last(3).Value< MACDMMArr.Max(a=>a.Value)*0.8)
                    {
                        return ret;
                    }
                    bool isRaised = true;
                    if (DEAMMArr.Last(1).Value < DEAMMArr.Last(3).Value)//保证前段并未涨得过多
                    {
                        isRaised = false;
                        return ret;
                    }
                    if (closeMMArr.Last(1).Value < closeMMArr.Last(2).Value)//close朝下，不理他
                    {
                        return ret;
                    }
                    if (closeMMArr.Length < 5)
                    {
                        return ret;
                    }
                    //
                    double maxVal = kLineData.Highs.Max();
                    double maxDownRate = 100 * (maxVal - closeMMArr.Last(2).Value) / maxVal;
                    double downRate = Math.Abs(100 * (closeMMArr.Last(2).Value - closeMMArr.Last(3).Value) / closeMMArr.Last(3).Value);
                    double upRate = 100 * (closeMMArr.Last(3).Value - closeMMArr.Last(4).Value) / closeMMArr.Last(4).Value;
                    double lastUpRate = 100 * (closeMMArr.Last(1).Value - closeMMArr.Last(2).Value) / closeMMArr.Last(2).Value;
                    /*
                    if (maxDownRate < 40)//跌少了
                    {
                        return ret;
                    }
                    if (downRate < 20)
                    {
                        return ret;
                    }
                    if (maxDownRate < 50 && maxDownRate / downRate < 2)
                    {
                        return ret;
                    }
                    if (downRate < 50 && maxDownRate / downRate < 2)
                    {
                        return ret;
                    }

                    if (upRate > 20)//前期涨多了
                    {
                        return ret;
                    }
                    */
                    if (lastUpRate > 5)//最后一期涨多了
                    {
                        return ret;
                    }
                    
                    //前一个价格的极值是最小值，上涨两个周期得到确认
                    if (closeMMArr.Last(2).Value <= closeMMArr.Select(a => a.Value).Min() && closeMMArr.Last(1).index > closeMMArr.Last(2).index + 1)
                    {
                        //找前期最后一个绿macd值，或者以价格序号替代
                        var items = MACDMMArr.Where(a => a.Value < 0 && a.index < MACDMMArr.Last(2).index);
                        if (items.Count() == 0)
                            return ret;
                        items = items.OrderBy(a => a.index);
                        //if (DEAMMArr.Last(1).Value > DEAMMArr.Last(2).Value) 
                        if (MACDMMArr.Last(2).Value > items.Last().Value / 2)
                        {
                            ret.ReferValues = new object[] { closeMMArr.Last(2), MACDMMArr.Last(2), DEAMMArr.Last(2) };
                            ret.Weight = (isRaised ? 2 : 1).ToWeight(1000) + maxDownRate.ToSimpleNumber(10).ToWeight(100) + downRate.ToSimpleNumber(5).ToWeight(10) + lastUpRate.ToSimpleNumber(1).ToWeight(1);
                            ret.Status = string.Format("上升:{0};大降幅:{1};最后升幅:{2};中途跌幅:{3}", isRaised, maxDownRate.ToEquitPrice(2), lastUpRate.ToEquitPrice(2), downRate.ToEquitPrice(2));
                            ret.Enable = true;
                            return ret;
                        }
                    }
                    return ret;
                   
                }
               
            }
            catch(Exception ce)
            {
                return ret;
            }
            finally
            {
                this.SecObj = null;
                kLineData = null;
                MACDMMArr = null;
                DEAMMArr = null;
                closeMMArr = null;
                //GC.Collect();
            }
        }

        public bool CurrInGreen(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays)
        {
            throw new NotImplementedException();
        }

        public bool CurrInRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays)
        {
            throw new NotImplementedException();
        }

        public bool CurrInNearGreenArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays)
        {
            throw new NotImplementedException();
        }

        public bool CurrInFarGreenArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays)
        {
            throw new NotImplementedException();
        }

        public bool CurrInNearRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays)
        {
            throw new NotImplementedException();
        }

        public bool CurrInFarRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays)
        {
            throw new NotImplementedException();
        }

        public bool CurrInMultiHumpRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays, MaxMinElementClass<double>[] humps,bool isNear)
        {
            throw new NotImplementedException();
        }

        public bool CurrInMultiHumpGreenArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays, MaxMinElementClass<double>[] humps,bool isNear)
        {
            throw new NotImplementedException();
        }
    }
}

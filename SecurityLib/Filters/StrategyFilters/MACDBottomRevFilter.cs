using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    /// <summary>
    /// 多周期策略
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MACDBottomRevFilter<T> : CommFilterLogicBaseClass<T> where T : TimeSerialData
    {
        public MACDBottomRevFilter(string endExpect,CommSecurityProcessClass<T> cpc, PriceAdj priceAdj = PriceAdj.Beyond, Cycle cyc = Cycle.Day) :base(endExpect,cpc, priceAdj, cyc)
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
            try
            {
                //List<T> list = this.SecObj.SecPriceInfo;
                //var klineData = new KLineData<T>(EndExpect, this.SecObj.SecPriceInfo,PriceAdj.Beyond);
                this.zeroLines = (0D).ToConst(kLineData.Closes);
                ///ToDo:反转选股
                
                if (1 == 1)
                {
                    //只做空头
                    if(macd.DEAs.Last(1)>macd.DEAs.Last(2))
                    {
                        return ret;
                    }
                    if(macd.MACDs.Last(2)>0)
                    {
                        return ret;
                    }
                    
                    //DEA在上，只抄底
                    if (macd.DEAs.Last(1) >= 0)
                    {
                        return ret;
                    }
                    /*
                    int maxUseLen = zeroLines.LastMatchCondition(macd.DEAs, GuideToolClass.CrossUp);
                    if (maxUseLen < 20)//Input.ReferDays)
                    {
                        //maxUseLen = Input.ReferDays;
                        return ret;
                    }*/
                    //下跌以来的周数
                    int maxUseLen = zeroLines.LastMatchCondition(macd.DEAs, GuideToolClass.CrossUp);
                    if (maxUseLen < 15)//Input.ReferDays)//趋势持续20周以上
                    {
                        //maxUseLen = Input.ReferDays;
                        return ret;
                    }
                    //closeMMArr = kLineData.Closes.LastSector(maxUseLen).MaxMinArray<double>(10);
                    
                    if (macd.MACDs.Last(1)<macd.MACDs.Last(2))//macd朝下，不理他
                    {
                        return ret;
                    }
                    
                    ////if (DEAMMArr.Length < 3)
                    ////{
                    ////    return ret;
                    ////}
                    //if(MACDMMArr.Length<4)
                    //{
                    //    return ret;
                    //}
                    bool isRaised = true;
                    //东山再起，最后一次起的最高或者相当高
                    /*if(MACDMMArr.Last(3).Value< MACDMMArr.Max(a=>a.Value)*0.8)
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
                    */
                    //
                    //double maxVal = kLineData.Highs.Max();
                    //double maxDownRate = 100 * (maxVal - closeMMArr.Last(2).Value) / maxVal;
                    //double downRate = Math.Abs(100 * (closeMMArr.Last(2).Value - closeMMArr.Last(3).Value) / closeMMArr.Last(3).Value);
                    double lastUpRate = kLineData.RaiseRates.Last();
                    //double lastUpRate = 100 * (closeMMArr.Last(1).Value - closeMMArr.Last(2).Value) / closeMMArr.Last(2).Value;
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
                    if (lastUpRate > 10)//最后一期涨多了
                    {
                        return ret;
                    }
                    int macdDownDays = zeroLines.LastMatchCondition(macd.MACDs, GuideToolClass.Cross);//macd最后下跌周期数
                    if(macdDownDays>=maxUseLen)//如果最后一次macd下穿0线还在dea下穿之前，那以2计（默认最近2期是MACD下探0线的）
                    {
                        macdDownDays = 2;
                    }
                    var items = kLineData.lowCycleData.LastSector(maxUseLen).FirstSector(maxUseLen - macdDownDays);
                    if(items.Count() == 0)
                    {
                        return ret;
                    }
                    double priceLow = items.Min(a=>a.low);
                    int macdLowIndex = kLineData.lowCycleData.Where(a=>a.low == priceLow).Last().index;//最低价对应的macd位置
                    double macdLow = macd.MACDs[macdLowIndex];                    
                    double macdRate = 100*(kLineData.Closes.LastSector(macdDownDays).Min()-macdLow)/macdLow;
                    double priceRate = 100*(priceLow-kLineData.lowCycleData.LastSector(macdDownDays).Min(a => a.low))/ priceLow;//下跌幅度
                    //最近两周的收盘价是最小值
                    if (priceRate>0)
                    {
                        //最近两周的macd的最小值大于下跌以来的最小值
                        if (macdRate>0)
                        {
                            MaxMinElementClass<double>[] MACDMMArr = macd.MACDs.LastSector(maxUseLen).MaxMinArray<double>(5);
                            //DEAMMArr = macd.DEAs.LastSector(maxUseLen).MaxMinArray<double>(5);
                            if (MACDMMArr.Where(a => a.Value < 0).Count() < (MACDMMArr.Last().Value<0?4:3))//macd极值数小于0的数量要大于3，2次纠缠
                            {
                                return ret;
                            }
                            //LockSlim.EnterWriteLock();
                            ret.Key = kLineData.code;
                            double lowPrice = kLineData.Lows.LastSector(macdDownDays).Min();
                            string lowExpect = kLineData.lowCycleData.Last(macdDownDays).low > kLineData.lowCycleData.Last(1).low ? kLineData.lowCycleData.Last(1).lowExpect : kLineData.lowCycleData.Last(2).lowExpect;
                            ret.ReferValues = new object[] { lowPrice, lowExpect };//以前周期最低价作为止损值
                            ret.Weight = (priceRate / 10).ToSimpleNumber(10).ToWeight(10000) 
                                + (kLineData.Length-macdLowIndex+1).ToSimpleNumber(20).ToWeight(1000) 
                                + (macdRate<0?11:Math.Max(macdRate/ 0.5,10)).ToSimpleNumber(11).ToWeight(100) +   lastUpRate.ToSimpleNumber(2).ToWeight(10);
                            ret.Status = string.Format("价格背离度:{0};与前期低点周期数:{1};趋势背离度:{2};", 
                                (priceRate / 10).ToSimpleNumber(10),
                                (kLineData.Length - macdLowIndex + 1).ToSimpleNumber(10),
                                (macdRate < 0 ? 11 : Math.Max(macdRate / 0.5, 10)).ToSimpleNumber(11), 
                                priceRate);
                            ret.Enable = true;
                            //LockSlim.ExitWriteLock();
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
                //MACDMMArr = null;
                //DEAMMArr = null;
                //closeMMArr = null;
                //GC.Collect();
            }
        }

    }
}

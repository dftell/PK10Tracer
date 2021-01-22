using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    /// <summary>
    /// 最小长度滤网
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MutliCycle_MACDBottomRevFilter<T> : MutliCycle_Filter<T> where T : TimeSerialData
    {
        public MutliCycle_MACDBottomRevFilter(string endExpect,CommSecurityProcessClass<T> cpc, PriceAdj priceAdj = PriceAdj.Beyond, Cycle cyc = Cycle.Day) :base(endExpect,cpc, priceAdj, cyc)
        {

        }
        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }
        public override SelectResult ExecFilter(CommStrategyInClass Input)
        {
            SelectResult ret = null;
            KLineData<T> useData = kLineData;
            Cycle maxCycle = Input.useMaxCycle;
            for(int i=(int)Input.useMaxCycle;i>=(int)Input.useMinCycle;--i)
            {
                if(i<(int)Input.useMaxCycle)
                {
                    useData = new KLineData<T>(useData.Expect, useData.orgList, useData.code, useData.name, useData.Adj, (Cycle)i);
                    
                }
                ret = CycleExecFilter(maxCycle,useData, Input);
                if (!ret.Enable)
                {
                    return ret;
                }
            }
            return ret;
        }
        public override SelectResult CycleExecFilter(Cycle maxCycle,KLineData<T> uData, CommStrategyInClass Input)
        {
            SelectResult ret = new SelectResult();
            ret.Key = Input.SecIndex;
            ret.Enable = false;
            double[] closes = uData.Closes;
            MACDCollection macd = closes.MACD();
            try
            {
                this.zeroLines = (0D).ToConst(closes);
                ///ToDo:反转选股

                if (1 == 1)
                {
                    //只做空头
                    //if(macd.DEAs.Last(1)>macd.DEAs.Last(2))
                    //{
                    //    return ret;
                    //}
                    //刚变不动
                    int cycDiff = (int)maxCycle + 1 - (int)uData.LineCycle;
                    if (maxCycle == Cycle.Week)//如果最大周期是周，最后检查item周期恒等于2
                    {
                        cycDiff = 1;
                    }
                    var items = macd.MACDs.LastSector((int)(Math.Pow(2, cycDiff)));//2,4,8,最多有8天
                    if (items.Count() > 0 && items.Min() > 0)//可以放松日线的条件，通过涨幅来约束
                    {
                        items = null;
                        return ret;
                    }
                    items = null;
                    //DEA在上，只抄底
                    if (macd.DEAs.Last(1) >= 0)
                    {
                        return ret;
                    }
                    //DEA下跌来的周期数
                    int maxUseLen = zeroLines.LastMatchCondition(macd.DEAs, GuideToolClass.CrossUp);
                    if (maxUseLen < 10)//Input.ReferDays)//趋势持续20周以上
                    {
                        //maxUseLen = Input.ReferDays;
                        return ret;
                    }
                    if (macd.MACDs.Last(1) < macd.MACDs.Last(2))//macd朝下，不理他
                    {
                        return ret;
                    }

                    bool isRaised = false;
                    if (macd.MACDs.LastSector(2).Min() > 0)//已在零线上了
                    {
                        isRaised = true;
                    }
                    double lastUpRate = uData.RaiseRates.Last();
                    if (lastUpRate > 10)//最后一期涨多了
                    {
                        return ret;
                    }
                    int MacdRedCnt = macd.MACDs.Count(GuideToolClass.Cross, zeroLines, maxUseLen);
                    bool Twisted = true;
                    if (MacdRedCnt == 0)//未出现红段,缠
                    {
                        Twisted = false;
                    }
                    int macdDownDays = zeroLines.LastMatchCondition(macd.MACDs, GuideToolClass.Cross);//macd最后下跌周期数
                    if (macdDownDays >= maxUseLen)//如果最后一次macd下穿0线还在dea下穿之前，那以2计（默认最近2期是MACD下探0线的）
                    {
                        if (Twisted)
                        {
                            macdDownDays = 2;
                        }
                    }
                    int igronLen = isRaised ? 2 : 1;
                    double[] lastPriceItems = new double[0];
                    double[] lastMacdItems = new double[0];
                    double[] currPriceItems = new double[0];
                    double[] currMacdItems = new double[0];
                    double[] lastGreenMACDs = macd.MACDs.LastSector(macdDownDays);
                    MaxMinElementClass<double>[] macdMM = lastGreenMACDs.MaxMinArray(igronLen);//获取最后一段MACD绿柱分段
                    int useIndex = 0;
                    if (Twisted)
                    {
                        lastPriceItems = closes.LastSector(maxUseLen).FirstSector(maxUseLen - macdDownDays);
                        lastMacdItems = macd.MACDs.LastSector(maxUseLen).FirstSector(maxUseLen - macdDownDays);
                        currPriceItems = closes.LastSector(maxUseLen).LastSector(macdDownDays);
                        currMacdItems = lastGreenMACDs;
                        useIndex = closes.Length - macdDownDays;
                    }
                    if (macdMM.Length > 3)//一直在下跌,或者继续下跌，3为造成一次波谷，5为造成2次波谷
                    {
                        if (macdMM.Length == 4)
                        {

                        }
                        int lastHighPoint = macdMM.Last(3).index + 1; //倒数第三个点是上次波峰
                        lastPriceItems = closes.LastSector(macdDownDays).FirstSector(lastHighPoint);//上次波峰以前的价格
                        lastMacdItems = lastGreenMACDs.FirstSector(lastHighPoint);//上次波峰以前的macd
                        currPriceItems = closes.LastSector(macdDownDays).LastSector(macdDownDays - lastHighPoint);
                        currMacdItems = lastGreenMACDs.LastSector(macdDownDays - lastHighPoint);
                        useIndex = closes.Length - macdDownDays + lastHighPoint;
                    }

                    if (lastPriceItems.Length == 0 || lastMacdItems.Length ==0 || currMacdItems.Length == 0 || currPriceItems.Length == 0)
                    {
                        return ret;
                    }
                    double priceLow = lastPriceItems.Min();
                    double macdLow = lastMacdItems.Min();
                    double priceCurrLow = currPriceItems.Min();
                    double macdCurrLow = currMacdItems.Min();                    
                    /*
                    //MaxMinElementClass<double>[] MACDMMArr = macd.MACDs.LastSector(macdDownDays).MaxMinArray(isRaised?2:1);
                    //如果最后一段macd绿柱有多个分段。只有一个谷，长度为3上下上，多个长度为5+
                    if (macdMM.Length > 3)
                    {

                            int baseLen = closes.Length - macdDownDays;
                            //最后一个波峰为分割点
                            int splitPoint = baseLen + macdMM.Last(3).index;
                            //最后一个波峰后剩余的长度
                            int beyLen = closes.Length - splitPoint;
                            //倒数第一个波峰以前的最低macd
                            macdLow = macd.MACDs.FirstSector(splitPoint).Min();
                            //当前macd取波峰后的macd低点
                            macdCurrLow = macd.MACDs.LastSector(beyLen).Min();
                            //前面的最低价等于最后一个波峰以前的最低价
                            priceLow = closes.FirstSector(splitPoint).Min();
                            //最近最低价等于最后一个波峰以来的最低价
                            priceCurrLow = closes.LastSector(beyLen).Min();
                        
                    }
                    */
                    double macdRate = 100*(macdCurrLow - macdLow )/1;//macd是否上升
                    double priceRate = 100*(priceCurrLow -priceLow ) / priceLow;//下跌幅度
                    //最近两周的收盘价是最小值
                    if (priceRate<0)
                    {
                        //最近两周的macd的最小值大于下跌以来的最小值
                        if (macdRate>0)
                        {
                            //MACDMMArr = macd.MACDs.LastSector(maxUseLen).MaxMinArray<double>(1);
                            //DEAMMArr = macd.DEAs.LastSector(maxUseLen).MaxMinArray<double>(5);
                            //月线不要求要有纠缠
                            if (uData.LineCycle < Cycle.Month)
                            {
                                if (!Twisted)
                                {
                                    if (macdMM.Where(a => a.Value < 0).Count() < (macdMM.Last().Value < 0 ? 4 : 3))//macd极值数小于0的数量要大于4，3次纠缠
                                    {
                                        return ret;
                                    }
                                }
                            }
                            //LockSlim.EnterWriteLock();
                            ret.Key = uData.code;
                            double lowPrice = uData.Lows.LastSector(macdDownDays).Min();
                    
                            int lowIndex = useIndex+ currPriceItems.ToList().LastIndexOf(priceCurrLow);
                            string lowExpect = uData.isLowCycle? uData.lowCycleData.Last(2).lowExpect : uData.Expects[lowIndex];
                            ret.ReferValues = new object[] { lowPrice, lowExpect };//以前周期最低价作为止损值
                            ret.Weight = 1;
                            ret.Status = string.Format("价格背离度:{0};与前期低点周期数:{1};趋势背离度:{2};",
                                (priceRate / 10).ToSimpleNumber(10),
                                (kLineData.Length - macd.MACDs.ToList().IndexOf(macdLow) + 1).ToSimpleNumber(10),
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
            }
        }
    }
}

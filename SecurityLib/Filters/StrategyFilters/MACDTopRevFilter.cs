using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib.Filters.StrategyFilters
{
    /// <summary>
    /// 最小长度滤网
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MACDTopRevFilter<T> : CloseCommFilterLogicBaseClass<T>, iMACDFilter<T> where T : TimeSerialData
    {
        public MACDTopRevFilter(string endExpect,CommSecurityProcessClass<T> cpc, PriceAdj priceAdj = PriceAdj.Beyond, Cycle cyc = Cycle.Day) :base(endExpect,cpc, priceAdj, cyc)
        {

        }
        public override BaseDataTable GetData(int RecordCnt)
        {
            throw new NotImplementedException();
        }

        public override SelectResult ExecFilter(CommStrategyInClass Input)
        {
            kLineData.getSingleData = this.getSingleData;
            //List<T> list = this.SecObj.SecPriceInfo;            
            var klineData = kLineData;//  new KLineData<T>(EndExpect, this.SecObj.SecPriceInfo,PriceAdj.Beyond);
            klineData = KLineData<T>.reGetKLineData(klineData, Input.SignDate);
            this.zeroLines = (0D).ToConst(klineData.Closes);
            MACDCollection macd = klineData.Closes.MACD();
            double[] MA60 = kLineData.Closes.MA(60);
            bool isRaise = false;
            if((macd.DEAs.Last(2)>macd.DEAs.Last(3) && (macd.DEAs.Last(3)-macd.DEAs.Last(4)<macd.DEAs.Last(2)- macd.DEAs.Last(3)))||(MA60.Last(2)>MA60.Last(3) && (MA60.Last(3)-MA60.Last(4))<(MA60.Last(2)-MA60.Last(3))))//DEA上升或者MA60上升代表上涨
            {
                isRaise = true;
            }
            SelectResult ret = new SelectResult();// SecObj;
            ret.Enable = false;
            //if (isRaise)
            //{
            //    return ret ;
            //}
            //排除第二买点
            if(macd.DEAs.Last()>0 ||MA60.Last()>MA60.Last(2) || MA60.Last(2)>MA60.Last(3))
            {
                int deaUpDays = macd.DEAs.LastMatchCondition(zeroLines, GuideToolClass.CrossUp);//DEA上穿0线日数
                int downCnt = zeroLines.Count(GuideToolClass.CrossUp, macd.MACDs, deaUpDays);
                //第一次下跌，那是第二买点，不管它
                if(downCnt<=1)
                {
                    return ret;
                }

            }



            DateTime[] dates = klineData.Expects.Select(a => a.ToDate()).ToArray();           
            int startIndex = dates.IndexOf(Input.SignDate);//信号日的索引
            if (startIndex < 0)//找不到开始日期，直接结束
            {
                ret.Enable = true;
                return ret;
            }
            var stopRes = StopLossProcess(Input);
            if(stopRes.Enable)//止损
            {
                ret.Enable = true;
                ret.Status = stopRes.Status;
                return ret;
            }

            int holddays = dates.Length - startIndex;//信号日以来持有天数
            bool isZY = false;
            double zf = 100*(klineData.Closes.Last(1) - klineData[Input.SignDate].close) / klineData[Input.SignDate].close;
            ///ToDo:反转选股
            
            double[] closes = klineData.Closes;

            double[] opens = klineData.Opens;
            
            int maxUseLen = Math.Max(200, zeroLines.LastMatchCondition(macd.DEAs, GuideToolClass.CrossUp));
            //MaxMinElementClass<double>[] deaMaxMinArr = macd.DEAs.MaxMinArray(5);
            MaxMinElementClass<double>[] macdMMArr = macd.MACDs.MaxMinArray(0);
            
            /*if (deaMaxMinArr.Last(1).Value >= deaMaxMinArr.Last(2).Value && deaMaxMinArr.Last(1).Value<0)//dea<0情况，只要DEA线往上走，都不要平仓
            {
                return ret;
            }*/
            if (1 == 1)
            {
                //刚买入MACD一直没抬头并下跌的情况
                if (macdMMArr.Last(1).Value < macdMMArr.Last(2).Value)
                {
                    if(macd.MACDs.LastSector(holddays).Max()<0)
                    {
                        if (macdMMArr.Last(1).index > macdMMArr.Last(2).index + 1)
                        {
                            ret.Enable = true;
                            ret.Status = string.Format( "{0}:买入后持续下跌，确认形成趋势后出场;",klineData.LineCycle.ToString());
                            return ret;
                        }
                    }
                    //一定是上一个macd低点
                    //如果是买入后有5个极值点，就可以通过最后两次的相对关系来判断
                    var items = macdMMArr.Where(a => a.index > startIndex - 5).ToArray();//信号日前3天，
                    //只对刚买入macd在0线上下纠缠，多次纠缠后要对比上升和下降趋势的最大值
                    if (items.Length <= 7)
                    {
                        if (macdMMArr.Last(4).Value > macdMMArr.Last(2).Value)
                        {
                            if (macdMMArr.Last(3).Value > macdMMArr.Last(1).Value && macdMMArr.Last(1).Value < 0)
                            {
                                ret.Enable = true;
                                ret.Status = string.Format("{0}:买入后,上升趋势减弱，跌势加速，直接退出！",klineData.LineCycle.ToString());
                                return ret;
                            }
                        }
                    }
                    if (items.Length >= 5)//如果买入后有两轮macd数据比较
                    {
                        
                        //首先判断下跌趋势
                        if (macdMMArr.Last(5).Value > macdMMArr.Last(3).Value)//远期macd低点高于近期macd低点，说明向下趋势逐渐增大
                        {
                            

                            //再次判断上升趋势
                            if (items.Max(a=>a.Value) > macdMMArr.Last(2).Value * 0.8)//远期macd高点高于近期macd高点，说明向上趋势也减少了
                            {
                                
                                string raiseReason = string.Format("{0}:买入后，上升趋势减弱，下降趋势增大退出;",klineData.LineCycle.ToString());
                                if (macdMMArr.Last(2).Value < 0)//高位值小于0，快跑，别等了
                                {
                                    ret.Enable = true;
                                    ret.Status = raiseReason + "当前低位立即退出;";
                                    return ret;
                                }
                                else
                                {
                                    //double[] holdMacds = macd.MACDs.LastSector(holddays);//持有后macd下穿次数为0
                                    int crsCnt = zeroLines.Count(GuideToolClass.Cross, macd.MACDs, holddays);
                                    if (crsCnt > 0) //下穿一次以后再考虑
                                    {
                                        if (macdMMArr.Last(1).index > macdMMArr.Last(2).index + 1)
                                        {
                                            //根本不需要判断价格。直接可以出局
                                            ret.Enable = true;
                                            ret.Status = raiseReason + "当前高位延迟退出;";
                                            return ret;

                                        }
                                    }
                                }
                            }
                        }
                    }
                    else//只有最低点和一轮上涨然后下落
                    {
                        if(items.Length>=3)//
                        {
                            if(macdMMArr.Last(2).Value<0)//当前顶点位置在0线下
                            {
                                //根本不需要判断价格。直接可以出局
                                ret.Enable = true;
                                ret.Status = string.Format("{0}:买入后，短期内上升无力，跌势加速，直接退出！",klineData.LineCycle.ToString());
                                return ret;
                            }                            
                        }
                    }
                    if (1 == 0)
                    {
                        if (items.Length <= 5)//只有两轮MACD，无论什么情况都要去判断下涨幅情况，过大，就跑，包括存在连续涨停
                        {
                            if (items.Length < 2 || macdMMArr.Last(1).index > macdMMArr.Last(2).index + 5)
                            {
                                //超过一天，极值3or5日内闪烁，不处理
                                return ret;
                            }
                            string raiseReason = null;

                            //var items = macdMMArr.Where(a => a.index > startIndex - 5).ToArray();//信号日前3天，
                            int downDays = zeroLines.LastMatchCondition(macd.DEAs, GuideToolClass.CrossUp);
                            int upDays = 0;
                            if (macd.DEAs.Last(1) > 0)
                            {
                                upDays = macd.DEAs.LastMatchCondition(zeroLines, GuideToolClass.Cross);//上升日期数
                            }
                            //特殊情况是正好在持仓这段时间dea上穿然后下探0线下
                            //由于本情况主要检测买入后短时间的特例，
                            //而且买入点都是dea深探，这种情况理论上不存在
                            int totalDays = downDays + upDays;
                            if (totalDays < holddays)//DEA在0线上反复
                            {
                                return ret;
                            }
                            ////////////前期macd最高点所形成的价格高点，前期平台的高点，即为下跌过程中到买入信号点前的最大强度反抽的高点,
                            //////////double farHigh = closes[macdMMArr.Where(a => (a.index < startIndex && a.index > klineData.Length - totalDays) && a.Value > 0).OrderBy(a => a.Value).Last().index];//冲破前期平台最高点
                            //////////MaxMinElementClass<double>[] closeMMArr = closes.MaxMinArray(holddays);
                            ////////////价格短时间内就高于了极值的一半
                            ////////////默认当前价格最近处macd的高位等于最近高位以来的最高价
                            //////////double currHigh = klineData.Closes[macdMMArr.Last(2).index];
                            //////////int highIndex = macdMMArr.Last(2).index;
                            //////////double farLow = closeMMArr.Where(a => a.index < startIndex && a.index < highIndex).Min(a => a.Value);
                            ////////////前期5轮macd波动中最大值对应的价格
                            //////////int signIndex = klineData.Expects.IndexOf(Input.SignDate);
                            //////////double signPrice = klineData.Closes[signIndex];
                            //////////double currZf = 100 * (currHigh - signPrice) / signPrice;
                            ////////////如果短期内最高点超过了整体下跌来的一半并且超出了近两轮涨幅的最高点，先出了再说
                            //////////if (currZf > 15 && (currHigh > (closeMMArr.Max(a => a.Value) + farLow) / 2 || currHigh > farHigh))
                            //////////{
                            //////////    raiseReason = string.Format("如果短期内最高点({2})涨幅{3}%超过了整体下跌来的一半的价格({0})或者超出了前期平台反抽的最高点({1})", (closeMMArr.Max(a => a.Value) + farLow) / 2, farHigh, currHigh, currZf.ToEquitPrice(2));
                            //////////    //quickRaise = true;
                            //////////}

                        }
                    }

                }
                return ret;
            }

           
        }

        public bool CurrInGreen(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays)
        {
            //最后一段绿柱在当前日期前日数
            int lastGreenAreaLen = zeroLines.LastMatchCondition(macds.MACDs, GuideToolClass.Cross);
            //最后一段红柱在当前日期前日数
            int lastRedAreaLen = macds.MACDs.LastMatchCondition(zeroLines, GuideToolClass.Cross);
            //如果最后一段绿柱的开始日期大于信号日期，说明当前这段红柱线已经远处的红柱了，转入远处红色区域处理
            if (kline.Expects[kline.Length - lastRedAreaLen].ToDate() > Input.SignDate.ToDate())
            {
                return CurrInFarGreenArea(Input, kline, macds, holdDays, lastRedAreaLen);
            }
            else//刚刚由绿转红,转入近处红色区域处理
            {
                return CurrInNearGreenArea(Input, kline, macds, holdDays, lastRedAreaLen);
            }
        }
        /// <summary>
        /// 当前为上涨区
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <returns></returns>
        public bool CurrInRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays)
        {
            //最后一段绿柱在当前日期前日数
            int lastGreenAreaLen = zeroLines.LastMatchCondition(macds.MACDs, GuideToolClass.Cross);
            //最后一段红柱在当前日期前日数
            int lastRedAreaLen = macds.MACDs.LastMatchCondition(zeroLines, GuideToolClass.Cross);
            //如果最后一段绿柱的开始日期大于信号日期，说明当前这段红柱线已经远处的红柱了，转入远处红色区域处理
            if (kline.Expects[kline.Length - lastGreenAreaLen].ToDate()>Input.SignDate.ToDate())
            {
                return CurrInFarRedArea(Input,kline,macds,holdDays,lastRedAreaLen);
            }
            else//刚刚由绿转红,转入近处红色区域处理
            {
                return CurrInNearRedArea(Input, kline, macds, holdDays,lastRedAreaLen); 
            }
        }

        /// <summary>
        /// 近处下跌区间
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <returns></returns>
        public bool CurrInNearGreenArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 远处下跌区间
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <returns></returns>
        public bool CurrInFarGreenArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 近处上涨区间
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <returns></returns>
        public bool CurrInNearRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays)
        {
            //峰值数组
            MaxMinElementClass<double>[] humpArr = macds.MACDs.MaxMinArray(5);
            //如果极值数组最后3个元素均大于0，一定是一个多峰数组，
            //单边上涨倒数第二个元素小于0
            //单峰倒数第三个元素小于0
            if(humpArr.Length>3 && humpArr.LastSector(3).Where(a=>a.Value>0).Count() == 3)//一定是多峰
            {
                return CurrInMultiHumpRedArea(Input, kline, macds, holdDays, currStatusHoldDays, humpArr, true);
            }
            MaxMinElementClass<double>[] humpDEA = macds.DEAs.MaxMinArray(5);
            //DEA下跌，且长度持续1期以上
            bool DEADown = humpDEA.Last(1).Value < humpDEA.Last(2).Value && humpDEA.Last(1).index > humpDEA.Last(2).index + 1;
            if(humpArr.Last(2).Value<0)//单边上升过程中，不用管
            {
                return false;
            }
            //以下为单峰
            if(humpArr.Last(1).index==humpArr.Last(2).index+1)//未确定确实下降趋势，不管它
            {
                return false;
            }
            //以下为单峰已确定下降趋势
            if(macds.DEAs.Last(1)<0 && DEADown)//DEA在0线以下，并且已经确定下跌，退出
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 远处上涨区间
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <returns></returns>
        public bool CurrInFarRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays)
        {
            //峰值数组
            MaxMinElementClass<double>[] humpArr = macds.MACDs.LastSector(currStatusHoldDays).MaxMinArray(5);
            //如果数组长度大于2，一定是一个多峰数组
            if (humpArr.Length > 2)
            {
                return CurrInMultiHumpRedArea(Input, kline, macds, holdDays, currStatusHoldDays, humpArr,false);
            }
            return false;
        }

        /// <summary>
        /// 当前在多峰上涨空间
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <param name="humps"></param>
        /// <param name="isNear"></param>
        /// <returns></returns>
        public bool CurrInMultiHumpRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays, MaxMinElementClass<double>[] humps, bool isNear)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 当前在多峰下跌空间
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <param name="humps"></param>
        /// <param name="isNear"></param>
        /// <returns></returns>

        public bool CurrInMultiHumpGreenArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays, MaxMinElementClass<double>[] humps, bool isNear)
        {
            throw new NotImplementedException();
        }

        public override SelectResult StopLossProcess(CommStrategyInClass Input)//专门看日线级别
        {
            SelectResult ret = new SelectResult();
            KLineData<T> dayKLineData = new KLineData<T>(kLineData.Expect,kLineData.orgList,kLineData.code,kLineData.name);//日线数据
            //DateTime[] dates = dayKLineData.Expects.Select(a => a.ToDate()).ToArray();
            double slprice = 0;
            int slDate = -1;
            if (!string.IsNullOrEmpty(Input.StopPriceDate))
            {
                dayKLineData = KLineData<T>.reGetKLineData(dayKLineData, Input.StopPriceDate);
                int si = 0;
                slDate = dayKLineData.ExpectIndex(Input.StopPriceDate, out si);
                slprice = dayKLineData.Lows[slDate];
                MACDCollection macd = dayKLineData.Closes.MACD();
                if (macd.MACDs.Last() < macd.MACDs[slDate] && slprice> dayKLineData.Closes.Last())//如果价格和macd均较指定止损日低，则止损
                {
                    ret.Enable = true;
                    ret.Status = string.Format("{1}:趋势及价格都低于止损价所在位置，止损价:{0}",Input.StopPrice,dayKLineData.LineCycle.ToString());
                    return ret;
                }
            }
            slDate = dayKLineData.Expects.IndexOf(Input.SignDate);
            if(slDate<0)
            {
                dayKLineData = KLineData<T>.reGetKLineData(dayKLineData, Input.SignDate);
                slDate = dayKLineData.Expects.IndexOf(Input.SignDate);
            }
            bool QuickyRaise = false;
            string raiseReason = "";
            if (slDate >= 0)
            {
                int serialZtDays = 0;
                int ZTDays = calcZTDays(dayKLineData, slDate, out serialZtDays);
                if(ZTDays>3 && serialZtDays > 2)
                {
                    raiseReason = string.Format("短期内出现{0}次涨停，其中连续涨停天数为{1}天", ZTDays, serialZtDays);
                    QuickyRaise = true;
                }
            }
            //如果是快速拉升，目前只考虑连续涨停
            if(QuickyRaise)
            {
                MACDCollection macd = dayKLineData.Closes.MACD();
                MaxMinElementClass<double>[] macdMMArr = macd.MACDs.MaxMinArray(3);
                if(macdMMArr.Last(1).Value>macdMMArr.Last(2).Value)//macd上升时不考虑
                {
                    return ret;
                }
                if (macdMMArr.Last(2).Value < 0)//高位值小于0，快跑，别等了
                {
                    ret.Enable = true;
                    ret.Status = raiseReason + ";当前低位立即退出";
                    return ret;
                }
                else
                {
                    if (macdMMArr.Last(1).index > macdMMArr.Last(2).index + 1)
                    {
                        //根本不需要判断价格。直接可以出局
                        ret.Enable = true;
                        ret.Status = raiseReason + ";当前高位延迟退出";
                        return ret;

                    }
                }
            }
            return ret;
        }

        
        
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.SecurityLib;
using WolfInv.com.GuideLib.LinkGuid;
using WolfInv.com.GuideLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
using WolfInv.com.Strags;
using WolfInv.com.SecurityLib;
using WolfInv.com.SecurityLib.Filters.StrategyFilters;
using System.ComponentModel;
using System.Collections.Concurrent;

namespace WolfInv.com.Strags.Security
{
    public abstract class BaseSecurityStragClass<T> : BaseStragClass<T>, ITraceChance<T> where T : TimeSerialData
    {
        
        protected CommFilterLogicBaseClass<T> LogicFilter;
        ConcurrentDictionary<string, MongoReturnDataList<T>> _currDic;

        FinishedEvent _interProcessFinished;
        protected KLineData<T>.getSingleDataFunc getSingleData;
        public void setSingleDataFunc(KLineData<T>.getSingleDataFunc func)
        {
            getSingleData = func;
        }
        public void interProcessFinished(FinishedEvent ev)
        {
            _interProcessFinished = ev;
        }

        public FinishedEvent getInterProcessFinished()
        {
            return _interProcessFinished;
        }

        
        protected ConcurrentDictionary<string, MongoReturnDataList<T>> CurrSecDic
        {
            get
            {
                return _currDic;
            }
            set
            {
                _currDic = value;
            }
        }
        public void FillSecDic(ConcurrentDictionary<string,MongoReturnDataList<T>> val)
        {
            _currDic = val;
        }
        public abstract bool IsTracing { get; set; }

        protected BaseSecurityStragClass()
        {

        }

        /// <summary>
        /// 是否根据利润阶梯控制资产单元敞口
        /// </summary>
        [DescriptionAttribute("是否根据利润阶梯控制资产单元风险敞口"),
        DisplayName("是否根据利润阶梯控制资产单元敞口.允许最大的风险敞口为:BR+(STEP+1)*SR"),
        CategoryAttribute("持仓设置"),
        DefaultValueAttribute(true)]
        public bool needUseProfitStepControlRiskExposure { get; set; }
        [DescriptionAttribute("基本风险敞口(%)"),
        DisplayName("基本风险敞口(%)"),
        CategoryAttribute("持仓设置"),
        DefaultValueAttribute(6.0)]
        public double BasicRiskExposure { get; set; }
        [DescriptionAttribute("利润阶梯单步间距(%)"),
        DisplayName("利润阶梯单步间距(%)"),
        CategoryAttribute("持仓设置"),
        DefaultValueAttribute(1.0)]
        public double ProfitStepIntelVal { get; set; }

        [DescriptionAttribute("单步允许增加风险敞口规模(%)"),
        DisplayName("单步允许增加风险敞口规模(%)"),
        CategoryAttribute("持仓设置"),
        DefaultValueAttribute(2.0)]
        public double StepAllowAddRiskExposure { get; set; }
        [DescriptionAttribute("是否使用止损价自动止损"),
        DisplayName("是否使用止损价自动止损，如果为否，需要在策略内自己按止损价止损"),
        CategoryAttribute("风控设置"),
        DefaultValueAttribute(false)]
        public bool useStopLossPrice { get; set; }
        /*
        /// 是否用参考代码，并非benchMark
        /// benchMark只是在展示时使用，不在策略级使用
        /// 这三个参数用来指定参考证券代码
        /// 在获取数据时先把这些代码对应的数据取出来
        /// 在策略使用时可以在数据里提出来使用
        */
        [DescriptionAttribute("是否使用引用证券池"),
        DisplayName("是否使用引用证券池"),
        CategoryAttribute("引用设置"),
        DefaultValueAttribute(false)]
        public bool useRefrenceCodes { get; set; }
        [DescriptionAttribute("引用证券池"),
        DisplayName("引用证券池"),
        CategoryAttribute("引用设置"),
        DefaultValueAttribute(null)]
        public string RefrenceCodes { get; set; }
        [DescriptionAttribute("是否多周期选股策略"),
        DisplayName("是否多周期选股策略"),
        CategoryAttribute("周期设置"),
        DefaultValueAttribute(false)]
        public bool IsMutliCycle { get; set; }
        [DescriptionAttribute("选股使用最大周期"),
        DisplayName("选股使用最大周期"),
        CategoryAttribute("周期设置"),
        DefaultValueAttribute(Cycle.Week)]
        public Cycle MaxCycle { get; set; }
        [DescriptionAttribute("选股使用最小周期"),
        DisplayName("选股使用最小周期"),
        CategoryAttribute("周期设置"),
        DefaultValueAttribute(Cycle.Day)]
        public Cycle MinCycle { get; set; }
        public string[] RefrenceArray
        {
            get
            {
                if (!useRefrenceCodes)
                    return null;
                if(string.IsNullOrEmpty(RefrenceCodes))
                {
                    return null;
                }
                var arr = RefrenceCodes.Split(';');
                return arr.Where(a => string.IsNullOrEmpty(a) == false).ToArray();
            }
        }

        public override Type getTheChanceType()
        {
            return this.GetType();
        }

        public double getChipAmount(double RestCash, ChanceClass<T> cc1, AmoutSerials amts)
        {
            SecurityChance<T> cc = cc1 as SecurityChance<T>;
            if (cc.FixAmt != null && cc.FixAmt.Value > 0)
            {
                if (CurrSecDic.ContainsKey(cc.ChanceCode))
                {
                    /*
                    string strDate = cc.ExpectCode;//信号日，暂时默认交易日不除权除息
                    KLineData<T> klines = new KLineData<T>(LastUseData().LastData.Expect, CurrSecDic[cc.ChanceCode]);//默认后复权
                    double signPrice = klines[strDate].close;//交易日收盘价
                    double usePrice = (cc as SecurityChance<T>).signPrice * klines.Closes.Last(1) / signPrice;
                    (cc as SecurityChance<T>).currUnitPrice = usePrice;
                    //KLineData<T> klines = new KLineData<T>(LastUseData().LastData.Expect, CurrSecDic[cc.ChanceCode], PriceAdj.Beyond);//一定要用后复权
                    */
                    double amt = cc.openPrice;
                    if (cc.ChipCount == 0)//还未定量，第一次定价
                    {
                        cc.ChipCount = Math.Max(1, (int)Math.Floor(cc.FixAmt.Value / 100 / amt)) * 100;
                    }
                    else
                    {
                        amt = cc.currUnitPrice;
                    }
                    return amt;
                }
            }
            return 100;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="cc"></param>
        /// <param name="a">code</param>
        /// <param name="secSerialData"></param>
        /// <param name="ed"></param>
        /// <param name="ped"></param>
        /// <param name="review"></param>
        /// <returns></returns>
        public bool TryOpenExchange(CommStrategyInClass inputParam, SecurityChance<T> cc, MongoReturnDataList<T> secSerialData, ExpectData<T> ed, ExpectData<T> ped, bool review)
        {
            if (cc.ReferValues != null)
            {
                object[] objs = cc.ReferValues as object[];
                if (objs != null)
                {
                    if (objs.Length > 1)
                    {
                        inputParam.StopPrice = (double)objs[0];
                        inputParam.StopPriceDate = objs[1]?.ToString();
                        cc.stopPrice = inputParam.StopPrice;
                        cc.stopPriceDate = inputParam.StopPriceDate;
                    }
                }
            }
            KLineData<T> klines = new KLineData<T>(ed.Expect, secSerialData, PriceAdj.Beyond);
            if (klines.IsUpStop(null))//如果涨停，不能买入
            {
                return false;
            }
            //开盘涨幅超过幅度
            if (inputParam.allowMaxRaiseRate > 0 && klines.OpenRaiseRate > inputParam.allowMaxRaiseRate)
            {
                return false;
            }
            //如果按前日在收盘前交易,如果收盘涨幅和最高价涨幅的平均值大于指定涨幅，不进行交易
            if (review && inputParam.allowMaxRaiseRate > 0 && (klines.RaiseRates.Last(1) + klines.HighRaiseRate) / 2 > inputParam.allowMaxRaiseRate)
            {
                return false;
            }
            //开盘跌幅超过幅度
            if (inputParam.allowMaxDownRate < 0 && klines.OpenRaiseRate < inputParam.allowMaxDownRate)
            {
                return false;
            }
            //如果按前日在收盘前交易,如果收盘涨幅和最低价跌幅的平均价大于指定涨幅，不进行交易
            if (review && inputParam.allowMaxDownRate > 0 && (klines.RaiseRates.Last(1) + klines.LowRaiseRate) / 2 < inputParam.allowMaxDownRate)
            {
                return false;
            }
            //如果当期无交易记录，停牌，不予交易
            if (!ed.ContainsKey(cc.ChanceCode))
            {
                return false;
            }
            StockMongoData psmd = ped[cc.ChanceCode] as StockMongoData;
            StockMongoData smd = ed[cc.ChanceCode] as StockMongoData;
            StockMongoData slsmd = secSerialData[cc.stopPriceDate] as StockMongoData;
            
            //cc.ChanceCode = a;
            cc.SignExpectNo = ed.Expect;//信号期
            cc.exchangeExpect = ed.Expect;//实际交易期
            cc.ExpectCode = ped.Expect;//信号期
            //价格信息
            cc.UnitCost = psmd.close;//这个价格后面不用，因为在框架里被到处改动，无法确定
            cc.openPrice = (smd.close*0.8 + smd.high*0.2); //在收盘前30分钟内交易完毕，取收盘价和最高价的平均值替代//(CurrSecDic[a].Last() as StockMongoData).close;
            if (slsmd != null)
            {
                double stopPrice = slsmd.low;
                if (cc.openPrice < stopPrice)//如果交易价已经小于止损价，直接不参与交易
                {
                    return false;
                }
            }

            cc.currUnitPrice = smd.close;//按当日收盘价算
            cc.signPrice = smd.close;//信号价，和后面对比的,后面价格都是以此日期为对比
            cc.ChipCount = 0;//必须不要指定，在后面计算价格时再根据计划配置的单机会金额来确定
            if (!review)
            {
                return true;
            }
            if (klines.Expects.Last(1).ToDate() < ed.Expect.ToDate())
            {
                return false;
            }
            return true;
        }

        public bool TryCloseExchange(SecurityChance<T> cc, MongoReturnDataList<T> secSerialData, ExpectData<T> ed, bool review)
        {
            KLineData<T> klines = new KLineData<T>(ed.Expect, secSerialData, PriceAdj.Beyond);
            if (!review)
            {
                return true;
            }
            if (klines.IsStoped())//停牌不交易
            {
                return false;
            }
            if (klines.IsDownStop(null))//跌停不交易
            {
                return false;
            }
            klines.getSingleData = getSingleData;
            klines = KLineData<T>.reGetKLineData(klines, cc.SignExpectNo);
            bool isStop = klines.IsStoped();
            int lastId = isStop ? 1 : 2;//如果停牌，索引就是倒数第一个，否则是第二个
            double rSignPrice = klines[cc.SignExpectNo].close;//信号日收盘价
            double signPrice = cc.signPrice;
            double a = signPrice / rSignPrice;
            cc.EndExpectNo = ed.Expect;
            cc.currUnitPrice = klines.Closes.Last(1) * a;
            cc.closePrice = (klines.Opens.Last(1)*0.8 + klines.Lows.Last(1)*0.2) *a;//平仓以股票开盘价及最低价的中间价替代
            return true;
        }

        public void RefreshPrice(string expectNo, SecurityChance<T> cc, MongoReturnDataList<T> secPriceInfo)
        {
            string strDate = cc.SignExpectNo;//信号日，暂时默认交易日不除权除息,如果恰好除权除息，开盘取的为真实价，会有错误。
            KLineData<T> klines = new KLineData<T>(expectNo, secPriceInfo, PriceAdj.Beyond);//默认后复权
            bool isStop = klines.IsStoped();
            int lastId = isStop ? 1 : 2;//如果停牌，索引就是倒数第一个，否则是第二个
            if(klines.Expects.IndexOf(cc.SignExpectNo)<0)//如果找不到信号日期
            {
                klines = KLineData<T>.getKlineData(klines, klines.code, expectNo, cc.SignExpectNo,getSingleData);
                if (klines == null)
                    return;
            }
            double rSignPrice = klines[cc.SignExpectNo].close;//信号日收盘价
            double signPrice = cc.signPrice;
            double a = signPrice / rSignPrice;
            cc.currUnitPrice =  klines.Closes.Last(1) * a;
            double lastPrice = klines.Closes.Last(lastId) * a ;//停牌上日取最后一日的价格，否则资产部分会连续出现价差
            if (klines.Expects.Last(lastId) == cc.exchangeExpect)//如果上期时间是
            {
                lastPrice =  cc.openPrice *a;
            }
            cc.preUnitPrice = lastPrice;

            /*
            double signPrice = klines[strDate].close;//信号日收盘价
            cc.currUnitPrice = cc.signPrice * klines.Closes.Last(1) / signPrice;
            cc.preUnitPrice = cc.signPrice * klines.Closes.Last(klines.IsStoped() ? 1 : 2) / signPrice;//停牌上日取最后一日的价格，否则资产部分会连续出现价差
            */
        }

        

        public virtual bool CheckNeedEndTheChance(ChanceClass<T> rcc, bool LastExpectMatched, bool review)
        {
            SelectResult res = new SelectResult();
            string expectNo = LastUseData().LastData.Expect;//当期
            SecurityChance<T> cc = rcc as SecurityChance<T>;
            ExpectList<T> AllData = LastUseData();
            ExpectList<T> list = AllData;
            ExpectData<T> useData = list.LastData;
            string useExpect = useData.Expect;
            if (review)
            {
                list = AllData.ExcludeLastLong(1);
                useData = list.LastData;
                useExpect = useData.Expect;
            }
            
            if (CurrSecDic == null)
            {
                return false;
            }
            if (!CurrSecDic.ContainsKey(cc.ChanceCode))
            {
                return false;
            }
            MongoReturnDataList<T> sec = CurrSecDic[cc.ChanceCode];
            if (sec.Last().Expect != LastUseData().LastData.Expect)
            {
                return false;
            }
            MongoReturnDataList<T> checkUseSec = new MongoReturnDataList<T>(sec.SecInfo, sec.isSecurity);
            sec.ForEach(a => {
                if (a.Expect.ToDate() >= expectNo.ToDate())
                    return;
                checkUseSec.Add(a);
            });
            if (cc.needEndChance)//如果上期已经需要关闭
            {
                cc.endStatus += ";上期跌停或停牌";
                //再检查是否需要关闭
                cc.currCantEndChance = !TryCloseExchange(cc, sec, AllData.LastData, review);
                rcc = cc;
                return true;
            }
            if(sec.Last().Expect != LastUseData().LastData.Expect)
            {
                return false;
            }
            cc.HoldTimeCnt = Math.Max(1, sec.Where(a => a.Expect.ToDate() > cc.ExpectCode.ToDate()).Count())-1;//持仓天数+1 只要是非停牌
            CommStrategyInClass fpf = new CommStrategyInClass();
            fpf.SecIndex = cc.ChanceCode;
            fpf.EndExpect = useExpect;
            fpf.MinDays = this.InputMaxTimes;
            fpf.ReferDays = this.InputMinTimes;
            fpf.BuffDays = this.ChipCount;
            fpf.TopN = this.SingeExpectSelectTopN;
            fpf.StartDate = cc.exchangeExpect;
            fpf.StopPriceDate = cc.stopPriceDate;
            fpf.StopPrice = cc.stopPrice;
            fpf.useStopLossPrice = useStopLossPrice;
            ///过滤检查使用的必须用提前一天的数据序列
            CommSecurityProcessClass<T> secprs = new CommSecurityProcessClass<T>(checkUseSec.SecInfo, checkUseSec);
            CommFilterLogicBaseClass<T> checkSec = new StopLossFilter<T>(fpf.EndExpect, secprs,PriceAdj.Beyond,cc.HoldTimeCnt>60?Cycle.Week:Cycle.Day);
            checkSec.getSingleData = getSingleData;
            res = checkSec.ExecFilter(fpf);
            if (res.Enable)//止损，检查是否能交易！
            {
                cc.endStatus = res.Status;
                cc.currCantEndChance = !TryCloseExchange(cc as SecurityChance<T>, sec, AllData.LastData, review);
                return true;
            }
            RefreshPrice(AllData.LastData.Expect, cc as SecurityChance<T>, sec);
            rcc = cc;
            return false;
        }

        /// <summary>
        /// 根据资产盈利规模分配风险敞口
        /// 允许最大的风险敞口为:BR+(STEP+1)*SR
        /// BR：基础敞口。默认基础敞口为6%
        /// STEP:盈利超出盈利最小计算幅度数，如计算幅度数为2%，盈利为7.5，则步数为7.5/2=3。默认计算幅度为1%
        /// SR:单步允许增加风险敞口幅度。默认为2%
        /// 如盈利为7.5%，参数均为默认值，则最大风险敞口为6%+(7.5/1+1)*2%=6%+8*2%=22%
        /// </summary>
        /// <param name="currTotal"></param>
        /// <param name="InitCash"></param>
        /// <returns></returns>
        public double getAllowMaxRiseExp(double currTotal,double InitCash)
        {
            double BR = this.BasicRiskExposure;
            double currProfitRisk = 100 * (currTotal - InitCash) / InitCash;
            int step = (int)Math.Floor(currProfitRisk / this.ProfitStepIntelVal);
            double total =  BR + Math.Max(step + 1, 0) * this.StepAllowAddRiskExposure;
            return total/100;
        }
    }
}

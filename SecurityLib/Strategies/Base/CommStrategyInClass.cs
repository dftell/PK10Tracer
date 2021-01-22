using System;
using System.Collections.Generic;
//using CFZQ_JRGCDB;
using System.Data;
using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    /// <summary>
    /// 策略输入业务参数类
    /// </summary>
    public class CommStrategyInClass
    {
        /// <summary>
        /// 股票池
        /// </summary>
        public List<string> SecsPool;
        /// <summary>
        /// 选股范围
        /// </summary>
        public string SecIndex;
        public string EndExpect;
        public Cycle Cyc;
        public PriceAdj prcAdj;
        /// <summary>
        /// 上市天数
        /// </summary>
        public Int64 OnMarketDays;
        /// <summary>
        /// 当日计算前日数据
        /// </summary>
        public bool CalcLastData;
        /// <summary>
        /// 前推日期数
        /// </summary>
        public long FareViewDays;
        /// <summary>
        /// 是否排除ST股票
        /// </summary>
        public bool IsExcludeST = true;
        /// <summary>
        /// 黑名单
        /// </summary>
        public List<string> ExcludeSecList;
        /// <summary>
        /// 是否20日均线过滤
        /// </summary>
        public bool IsMAFilter;
        /// <summary>
        /// 最大选股数量
        /// </summary>
        public int TopN=20;
        /// <summary>
        /// 最小日期数
        /// </summary>
        public int MinDays;
        public int ReferDays;
        public int BuffDays;
        /// <summary>
        /// 信号日
        /// </summary>
        public string SignDate;
        /// <summary>
        /// 实际交易日
        /// </summary>
        public string StartDate;
        /// <summary>
        /// 交易日开盘允许最大的涨幅，超出不交易
        /// </summary>
        public double allowMaxRaiseRate;
        /// <summary>
        /// 交易日开盘允许最大的跌幅，超出不交易
        /// </summary>
        public double allowMaxDownRate;
        /// <summary>
        /// 是否使用止损价
        /// </summary>
        public bool useStopLossPrice;
        /// <summary>
        /// 止损价
        /// </summary>
        public double StopPrice;
        /// <summary>
        /// 止损价对应日期
        /// </summary>
        public string StopPriceDate;

        public bool useMutliCycle;
        public Cycle useMaxCycle;
        public Cycle useMinCycle;

    }
}
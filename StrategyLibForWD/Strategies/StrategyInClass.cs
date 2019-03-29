using System;
using System.Collections.Generic;
//using CFZQ_JRGCDB;
using System.Data;
using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.StrategyLibForWD
{
    /// <summary>
    /// 策略输入业务参数类
    /// </summary>
    public class StrategyInClass
    {
        /// <summary>
        /// 股票池
        /// </summary>
        public List<string> SecsPool;
        /// <summary>
        /// 选股范围
        /// </summary>
        public string SecIndex;
        public DateTime EndT;
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
        public bool IsExcludeST;
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
        public int TopN;

    }
}
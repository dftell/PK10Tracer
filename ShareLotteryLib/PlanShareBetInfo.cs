using System;
namespace WolfInv.com.ShareLotteryLib
{
    /// <summary>
    /// 计划投注信息
    /// </summary>
    public class PlanShareBetInfo
    {
        public PlanShareBetInfo(decimal ishareAmt)
        {
            shareAmt = ishareAmt;
        }
        public decimal shareAmt = 1;
        #region 基本信息
        /// <summary>
        /// 微信ID
        /// </summary>
        public string betWxName { get; set; }
        /// <summary>
        /// 微信昵称
        /// </summary>
        public string betNikeName { get; set; }
        /// <summary>
        /// 认购份数
        /// </summary>
        public int subscribeShares { get; set; } 
        /// <summary>
        /// 认购时间
        /// </summary>
        public DateTime subscribeTime { get; set; }
        /// <summary>
        /// 需要支付金额
        /// </summary>
        public decimal needPayAmount { get; set; }
  
        /// <summary>
        /// 支付完成
        /// </summary>
        public bool Payed { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime payTime { get; set; }
        /// <summary>
        /// 付向微信账户
        /// </summary>
        public string payToWxAccount { get; set; }
        #endregion

        public string toSubscribeString()
        {
            string m = "{0}认购{1}份，金额:{2},{3}";
            string ret = string.Format(m, betNikeName, subscribeShares, subscribeShares*shareAmt,Payed?"已付款":"未付款");
            return ret;
        }

        public string toProfitString(decimal shareProfit)
        {
            string m = "{0}:{1}份返奖{2}";
            string ret = string.Format(m, betNikeName, subscribeShares, shareProfit*subscribeShares);
            return ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WolfInv.com.JdUnionLib
{
    //jd.union.open.goods.jingfen.query
    public class jd_union_goods_jingfen_query_response
    {
        public jd_union_open_goods_jingfen_query_response jd_union_open_goods_jingfen_query_response { get; set; }
    }

    public class jd_union_open_goods_jingfen_query_response
    {
        public string code { get; set; }
        public jd_union_response_result result { get; set; }
    }

    public class jd_union_response_result
    {
        public string code { get; set; }
        public string message { get; set; }
        public string requestId { get; set; }
        public int totalCount { get; set; }
        public List<jfGoodsResp> data { get; set; }
    }

    public class jfGoodsResp
    {
        /// <summary>
        /// 品牌
        /// </summary>
        ///
        public string brandCode { get; set; }
        public string brandName { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int comments { get; set; }
        /// <summary>
        /// 返佣信息
        /// </summary>
        public commissionInfo commissionInfo { get; set; }

        /// <summary>
        /// 优惠券信息，返回内容为空说明该SKU无可用优惠券
        /// </summary>
        public couponInfo couponInfo { get; set; }
        /// <summary>
        /// 商品好评率
        /// </summary>
        public double goodCommentsShare { get; set; }
        /// <summary>
        /// 商品落地页
        /// </summary>
        public string materialUrl { get; set; }
        public priceInfo priceInfo { get; set; }
        public shopInfo shopInfo { get; set; }
        public long skuId { get; set; }
        public string skuName { get; set; }
        /// <summary>
        /// 爆款
        /// </summary>
        public int isHot { get; set; }
        /// <summary>
        /// spuid，其值为同款商品的主skuid
        /// </summary>
        public long spuid { get; set; }
        /// <summary>
        /// g=自营，p=pop
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        ///     资源信息
        /// </summary>
        public resourceInfo resourceInfo { get; set; }
    }

    public class commissionInfo
    {
        public double commission { get; set; }
        public double commissionShare { get; set; }

    }

    public class couponInfo
    {
        /// <summary>
        /// 优惠券合集
        /// </summary>
        public List<coupon> couponList { get; set; }
    }

    public class coupon
    {
        /// <summary>
        /// 券种类(优惠券种类：0 - 全品类，1 - 限品类（自营商品），2 - 限店铺，3 - 店铺限商品券)
        /// </summary>
        public int bindType { get; set; }
        /// <summary>
        /// 券面额
        /// </summary>
        public int discount { get; set; }
        public string link { get; set; }
        /// <summary>
        ///         券使用平台(平台类型：0 - 全平台券，1 - 限平台券)
        /// </summary>
        public int platformType { get; set; }
        /// <summary>
        /// 消费限额
        /// </summary>
        public double quota { get; set; }
        /// <summary>
        /// 最优优惠券
        /// </summary>
        public int isBest { get; set; }
        public long getStartTime{get;set;}
        public long getEndTime { get; set; }
        public long useStartTime { get; set; }
        public long useEndTime { get; set; }

    }

    public class priceInfo
    {
        public double price { get; set; }
    }

    public class shopInfo
    {
        public string shopName { get; set; }
        public string shopId { get; set; }
    }

    public class resourceInfo
    {
        public int eliteId { get; set; }
        public string eliteName { get; set; }
    }

    public class imageInfo
    {
        public List<urlInfo> imageList { get; set; }
    }

    public class urlInfo
    {
        public string url { get; set; }
    }
}

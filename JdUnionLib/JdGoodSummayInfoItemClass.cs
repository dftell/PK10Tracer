using System.Xml;
using XmlProcess;

namespace WolfInv.com.JdUnionLib
{
    public class JdGoodSummayInfoItemClass
    {
        public string skuId { get; set; }
        public string skuName { get; set; }
        public string shopName { get; set; }
        public string price { get; set; }
        public string discount { get; set; }

        public string imgageUrl { get; set; }
        public string materialUrl { get; set; }
        public string brandName { get; set; }
        public string commissionUrl { get; set; }
        public string couponLink { get; set; }

        public string getMyUrl(string subid = null)
        {
            if (string.IsNullOrEmpty(this.materialUrl))
                return null;
            JdUnion_Goods_Link jgl = JdUnion_GlbObject.CreateBusinessClass(typeof(JdUnion_Goods_Link)) as JdUnion_Goods_Link;
            //jgl.InitClass(jgl.Module);//必须初始化，获取到json设置才能用。
            XmlDocument xmldoc = null;
            string url = System.Web.HttpUtility.UrlEncode(this.materialUrl);
            string strPath_SiteId = string.Format("promotionCodeReq/siteId");
            string strPath_MaterialId = string.Format("promotionCodeReq/materialId");
            string strPath_CouponUrl = string.Format("promotionCodeReq/couponUrl");
            string strPath_PositionId = string.Format("promotionCodeReq/positionId");
            jgl.setBussiessItems(strPath_SiteId, jgl.siteId);
            jgl.setBussiessItems(strPath_MaterialId, url);

            if(!string.IsNullOrEmpty(this.couponLink))
            {
                url = System.Web.HttpUtility.UrlEncode(this.couponLink);
                jgl.setBussiessItems(strPath_CouponUrl, url);
            }
            if(!string.IsNullOrEmpty(subid))
            {
                jgl.setBussiessItems(strPath_PositionId, subid);
            }
            jgl.sign = null;
            XmlDocument xmlschema = null;
            string msg = null;
            bool succ = jgl.getXmlData(ref xmldoc, ref xmlschema, ref msg, false);
            if (succ == false)
            {
                if (msg != null)
                {

                }
                return null;
            }
            return XmlUtil.GetSubNodeText(xmldoc, "root/url/url");
        }
        public string getFullContent(bool commissionUrl=false)
        {
            string ret = null;
            if (commissionUrl)
            {
                ret = @"{0}
————————
京东价：￥{1:2f}
内购价：￥{2:2f}
————————
领券+下单：{3}

";
                ret = string.Format(ret, skuName, price, float.Parse(price) - float.Parse(discount), this.commissionUrl.StartsWith("http:") ? "" : "http://" + this.commissionUrl);
            }
            else
            {
                ret = @"{0}
————————
京东价：￥{1:2f}
内购价：￥{2:2f}
————————
优惠券：{3}
商品:{4}

";
                float realprice = float.Parse(price) - float.Parse(discount);
                ret = string.Format(ret, skuName, price, realprice.ToString(), couponLink.StartsWith("http:")?"":"http://"+ couponLink, materialUrl.StartsWith("http:") ? "" : "http://" + materialUrl);
            }
            
            return ret.Replace("http:////","http://").Replace("http://http","http");
        }
    }
}

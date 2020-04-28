using System.Collections.Generic;
using System.Xml;
using XmlProcess;
using System.Linq;
using System;

namespace WolfInv.com.JdUnionLib
{
    public class JdGoodSummayInfoItemClass
    {


        public Func<string, string> shortLinkFunc;
        public string getShortLink(string suid = null)
        {
            string orgUrl = "http://www.wolfinv.com/ebuy/navigateTo.asp?";
            string strSuid = suid;
            if (strSuid == null)
            {
                strSuid = "";
            }
            Dictionary<string, string> iddic = new Dictionary<string, string>();
            lock (JdGoodsQueryClass.shortLinks)
            {
                if (JdGoodsQueryClass.shortLinks.ContainsKey(this.skuId))
                {
                    iddic = JdGoodsQueryClass.shortLinks[skuId];
                }
                if (iddic.ContainsKey(strSuid))
                    return iddic[strSuid];
                string link = getMyUrl(strSuid);
                if (string.IsNullOrEmpty(link))
                    return this.couponLink;
                link = orgUrl + link;
                string ret = shortLinkFunc?.Invoke(link);
                if (ret == null)
                    ret = link;
                if (!iddic.ContainsKey(strSuid))
                {
                    iddic.Add(strSuid, ret);
                    JdGoodsQueryClass.shortLinks[skuId] = iddic;
                }
                return ret;
            }

        }
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
        public long batchId { get; set; }
        public int isHot { get; set; }
        public int elitId { get; set; }
        public int inOrderCount30Days { get; set; }

        public string seckillEndTime { get;set;}
        public string seckillOriPrice { get; set; }
        public string seckillPrice { get; set; }
        public string seckillStartTime { get; set; }

        public string getMyUrl(string subid = null)
        {
            string dicsid = subid;
            if(subid == null)
            {
                dicsid = "";
            }
            if (string.IsNullOrEmpty(this.materialUrl))
                return null;
            lock (JdGoodsQueryClass.glbUrls)
            {
                Dictionary<string, string> dicurl = new Dictionary<string, string>();

                if (JdGoodsQueryClass.glbUrls.ContainsKey(this.skuId))
                {
                    dicurl = JdGoodsQueryClass.glbUrls[this.skuId];
                }
                if (dicurl.ContainsKey(dicsid))
                {
                    return dicurl[dicsid];
                }


                JdUnion_Goods_Link jgl = JdUnion_GlbObject.CreateBusinessClass(typeof(JdUnion_Goods_Link)) as JdUnion_Goods_Link;
                //jgl.InitClass(jgl.Module);//必须初始化，获取到json设置才能用。
                XmlDocument xmldoc = null;
                string url = this.materialUrl;
                string strPath_SiteId = string.Format("promotionCodeReq/siteId");
                string strPath_MaterialId = string.Format("promotionCodeReq/materialId");
                string strPath_CouponUrl = string.Format("promotionCodeReq/couponUrl");
                string strPath_PositionId = string.Format("promotionCodeReq/positionId");
                string strChainType = "promotionCodeReq/chainType";
                jgl.setBussiessItems(strPath_SiteId, jgl.siteId);
                jgl.setBussiessItems(strPath_MaterialId, url);

                if (!string.IsNullOrEmpty(this.couponLink))
                {
                    url = this.couponLink;
                    jgl.setBussiessItems(strPath_CouponUrl, url);
                }
                if (!string.IsNullOrEmpty(subid))
                {
                    jgl.setBussiessItems(strPath_PositionId, subid);
                }
                jgl.setBussiessItems(strChainType, "1");
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
                string urlRet = XmlUtil.GetSubNodeText(xmldoc, "NewDataSet/getResult/url");
                
                    if(!dicurl.ContainsKey(dicsid))
                        dicurl.Add(dicsid, urlRet);
                    JdGoodsQueryClass.glbUrls[this.skuId] = dicurl;
                
                return urlRet;
            }
        }
        public string getFullContent(bool commissionUrl=false,string messageModule = null)
        {
            if(this is JdGoodsQueryClass.JdSearchGoods)
            {
                if(string.IsNullOrEmpty(price))
                {
                    price = (this as JdGoodsQueryClass.JdSearchGoods).wlPrice.ToString();
                }
                if(string.IsNullOrEmpty(discount))
                {
                    discount = (this as JdGoodsQueryClass.JdSearchGoods).couponDiscount.ToString();
                }
            }
            if(WxFaceImage.Inited== false)
            {
                
            }
            string ret = null;
            if (commissionUrl)
            {
                ret = messageModule;
                if (string.IsNullOrEmpty(ret))
                {
                    ret = @"{3}{0}
————————
京东价：￥{1:2f}
内购价：￥{2:2f}
————————
{4}领券+下单：

";
                }
                if (string.IsNullOrEmpty(discount))
                {
                    discount = "0";
                }
                if (string.IsNullOrEmpty(couponLink))
                {
                    couponLink = "";
                }
                float realprice = float.Parse(price) - float.Parse(discount);
                ret = string.Format(ret, skuName, price, realprice.ToString(),WxFaceImage.getAFace(),WxFaceImage.getAFace());
            }
            else
            {
                ret = messageModule;
                if (string.IsNullOrEmpty(ret))
                    ret = @"{0}
————————
京东价：￥{1:2f}
内购价：￥{2:2f}
————————
优惠券：{3}
商品:{4}

";
                if(string.IsNullOrEmpty(discount))
                {
                    discount = "0";
                }
                if(string.IsNullOrEmpty(couponLink))
                {
                    couponLink = "";
                }
                float realprice = float.Parse(price) - float.Parse(discount);
                ret = string.Format(ret, skuName, price, realprice.ToString(), couponLink.StartsWith("http:")?"":"http://"+ couponLink, getShortLink(null));
            }
            
            return ret.Replace("http:////","http://").Replace("http://http","http");
        }
    }

    public class WxFaceImage
    {
        static Random rd;
        static string goodFaces = "/:heart,/:cake,/:sun,/:gift,/:footb,/:moon,/:hug,/:li";
        static string[] arr;
        public static bool Inited;
        public static string getAFace(int index=0)
        {
            if (rd == null)
                rd = new Random();
            int ind = (rd.Next() % arr.Length) ;
            return arr[Math.Min(ind,arr.Length-1)];
        }

        static WxFaceImage()
        {
            arr = goodFaces.Split(',');
            rd = new Random(arr.Length - 1);
            Inited = true;
        }
            

    }
}

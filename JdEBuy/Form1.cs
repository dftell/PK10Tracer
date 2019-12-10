using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WolfInv.com.JdUnionLib;
using XmlProcess;
using System.Data;
namespace JdEBuy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            bool inited = JdUnion_GlbObject.Inited;
        }

        private void btn_recieveData_Click(object sender, EventArgs e)
        {
            List<int> list = JdUnion_GlbObject.getElites();            
            try
            {
                foreach (int elite in list)
                {
                    JdUnion_Goods_List_Class jgl = JdUnion_GlbObject.CreateBusinessClass(typeof(JdUnion_Goods_List_Class)) as JdUnion_Goods_List_Class;
                    //jgl.InitClass(jgl.Module);//必须初始化，获取到json设置才能用。
                    XmlDocument xmldoc = null;
                    XmlDocument xmlschema = null;
                    string strElite = string.Format("goodsReq/eliteId");
                    jgl.setBussiessItems(strElite, elite.ToString());
                    string msg = null;
                    jgl.sign = null;
                    bool succ = jgl.getXmlData(ref xmldoc, ref xmlschema, ref msg,true);
                    
                    XmlNodeList nodes = xmldoc.SelectNodes("root/data");
                    foreach(XmlNode node in nodes)
                    {
                        XmlNode root = node.SelectSingleNode(".");
                        string strSkuid = XmlUtil.GetSubNodeText(root, "skuId");
                        string strSkuName = XmlUtil.GetSubNodeText(root, "skuName");
                        string strShopId = XmlUtil.GetSubNodeText(root, "shopInfo/shopId");
                        string strShopName = XmlUtil.GetSubNodeText(root, "shopInfo/shopName");
                        string couponBest = XmlUtil.GetSubNodeText(root, "couponInfo/couponList[isBest=1]/discount");
                        string couponBestLink = XmlUtil.GetSubNodeText(root, "couponInfo/couponList[isBest=1]/link");
                        string firstImgUrl = XmlUtil.GetSubNodeText(root, "imageInfo/imageList[1]/url");
                        string materialUrl = XmlUtil.GetSubNodeText(root, "materialUrl");
                        string strbrandName = XmlUtil.GetSubNodeText(root, "brandName");
                        string strPrice = XmlUtil.GetSubNodeText(root, "priceInfo/price");
                        List<string> strCList = new List<string>();
                        for (int i = 1; i <= 3; i++)
                        {
                            XmlNode cnode = root.SelectSingleNode("categoryInfo/cid" + i.ToString());
                            //XmlNode cNameNode = root.SelectSingleNode(string.Format("cid{0}Name", i));
                            strCList.Add(cnode.InnerText);
                        }
                        string strCategories =  string.Join(",",strCList.Where(a => a.Trim().Length > 0));//分类
                        string strXml = root.OuterXml;
                    }
                }
            }
            catch(Exception ce)
            {

            }
        }
    }
}

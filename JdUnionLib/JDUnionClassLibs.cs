using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.Com.AccessWebLib;
using WolfInv.Com.JsLib;
using System.Xml;
using System.Data;
using XmlProcess;
namespace WolfInv.com.JdUnionLib
{

    public abstract class JDYSCM_Class: JdyRequestClass
    {
        public JDYSCM_Class()
        {

        }
        public string dbId { get; set; } //账套Id
        
        public int page { get; set; }   //当前页码
        public int totalsize { get; set; }  //当前返回总记录数
        public int records { get; set; }    //总记录数
        public int totalPages { get; set; }	//总页数
        //public List<JDYSCM_Item_Class> items { get; set; }

        public class JDYSCM_Item_Class:JdyJsonClass
        {

        }

        
    }

    
    public abstract class JDYSCM_Bussiness_Class: JDYSCM_Class
    {

        public bool InitRequestJson()
        {
            base.InitRequestJson();
            ReqJson = string.Format("access_token={0}&dbId={1}", jdy_GlbObject.Access_token, jdy_GlbObject.dbId);

            return true;

        }

        public string getUrl()
        {
            string url = string.Format("{0}?{1}", ReqUrl, ReqJson);
            return url;
        }

        public new string PostRequest()
        {
            if (ReqJson == null)
            {
                if (!this.InitRequestJson())
                {
                    //return null;
                }
            }
            
            return AccessWebServerClass.PostData(getUrl(), this.Req_PostData??"", Encoding.UTF8);
        }
    }

    public abstract class JDYSCM_Bussiness_List_Class : JDYSCM_Bussiness_Class
    {
        public JDYSCM_Bussiness_Filter_Class filter;
        public class JDYSCM_Bussiness_Filter_Class:JsonableClass<JDYSCM_Bussiness_Filter_Class>
        {
            //public string updTimeBegin { get; set; }//": "2019-01-08",
            //public string updTimeEnd { get; set; }//":"2019-10-09",
            public int pageSize { get; set; }//":20,
            public int page { get; set; }//":1
        }

        public void RequestSizeAndPage(int pageSize, int page, XmlNode reqnode = null)
        {
            if(this.Module.RequestMethodUseGET)
            {
                this.ReqJson = string.Format("{0}&pageSize={1}&page={2}", this.ReqJson, pageSize, page); 
                return;
            }
            XmlDocument doc = getFilterSchema();
            if(doc == null)
            {
                return;
            }
            XmlNode root = doc.SelectSingleNode("filter");
            if (root == null)
                return;
            XmlNode node = root.SelectSingleNode("pageSize");
            if (node == null)
            {
                node = XmlUtil.AddSubNode(root, "pageSize", pageSize.ToString());
            }
            else
            {
                node.InnerText = pageSize.ToString();
            }
            node = root.SelectSingleNode("page");
            if (node == null)
            {
                node = XmlUtil.AddSubNode(root, "page", page.ToString());
            }
            else
            {
                node.InnerText = page.ToString();
            }
            if(reqnode!=null)
            {
                for(int i=0;i<reqnode.ChildNodes.Count;i++)
                {
                    XmlNode cnode = reqnode.ChildNodes[i];
                    string val = XmlUtil.GetSubNodeText(cnode, "@value");
                    if (string.IsNullOrEmpty(val))//值为空，直接跳过
                    {
                        continue;
                    }
                    string name = cnode.Name;
                    XmlNode currnode = root.SelectSingleNode(name);
                    if(currnode == null)
                    {
                        currnode = doc.CreateElement(name);
                        root.AppendChild(currnode);
                    }
                    currnode.InnerText = val.Trim();
                }
            }
            this.Req_PostData =  XML_JSON.XML2Json(doc, "filter");
        }

        public virtual string RequestDataSet(DataSet ds)
        {
            XmlDocument doc = getRequestSchema();
            if (doc == null)
            {
                return null;
            }
            XmlNamespaceManager xmlm = new XmlNamespaceManager(doc.NameTable);
            xmlm.AddNamespace("json", "http://james.newtonking.com/projects/json");//添加命名空间

            XmlNode root = doc.SelectSingleNode(".");
            if(root == null)
            {
                root = doc.CreateElement("req");
            }
            root = doc.SelectSingleNode("req");
            XmlNodeList tables = root.SelectNodes("Schema/Table");
            List<TableGuider> tablist = new List<TableGuider>();
            for(int i=0;i<tables.Count;i++)
            {
                tablist.Add(new TableGuider(tables[i]));
            }
            root.RemoveChild(root.SelectSingleNode("Schema"));
            //root.AppendChild(doc.CreateElement(tablist[0].TableName));
            root = FillXmlByDatable(root, ds, 0, tablist);
            
            string ret = XML_JSON.XML2Json(doc, root.Name,true);
            tablist.ForEach(a =>
            {
                ret = ret.Replace(string.Format("${0}", a.TableName), a.TableName);

            });
            return ret;
        }
        XmlNode FillXmlByDatable(XmlNode parent, DataSet ds,int dtindex, List<TableGuider> guds)
        {
            if(ds.Tables.Count <= dtindex)
            {
                throw new Exception("Jdy:数据集超出索引！");
            }
            if (guds.Count <= dtindex)
            {
                throw new Exception("Jdy:Schema配置超出索引！");
            }
            
            TableGuider tg = guds[dtindex];
            TableGuider pretg = null;
            if(dtindex>0)
            {
                pretg = guds[dtindex - 1];
            }
            if(tg.TableName==null || tg.TableName.Trim().Length == 0)
                throw new Exception("Jdy:Schema配置Json超出索引！");
            DataTable dt = ds.Tables[dtindex];
            
            string RepeatItem =  guds[dtindex].TableName;
            XmlDocument doc = parent.OwnerDocument;
            if (doc == null)
                doc = parent as XmlDocument;
            XmlNamespaceManager xmlm = new XmlNamespaceManager(doc.NameTable);
            xmlm.AddNamespace("json", "http://james.newtonking.com/projects/json");//添加命名空间
            string filter = "";
            if (pretg == null ||pretg.KeyValue == null )
            {
                filter = "1=1";
            }
            else
            {
                filter = string.Format("{0}='{1}'", pretg.NextRef ,pretg.KeyValue);
            }
            DataRow[] drs = dt.Select(filter);
            for (int i = 0; i < drs.Length; i++)
            {
               
                XmlNode node =  doc.CreateElement(RepeatItem, xmlm.LookupNamespace("json"));
                XmlAttribute att = doc.CreateAttribute("json:Array", xmlm.LookupNamespace("json"));
                att.Value = "true";
                node.Attributes.Append(att);
                //XmlUtil.AddAttribute(node, "json:Array", "true");
                string keyval = null;
                for(int j=0;j<dt.Columns.Count;j++)
                {
                    string col = dt.Columns[j].ColumnName;
                    string val = drs[i][col].ToString();
                    XmlNode subnode = doc.CreateElement(col);
                    subnode.InnerText = val;
                    if (tg.Key == null || tg.Key.Trim().Length < dtindex + 1)
                    {
                        node.AppendChild(subnode);
                        continue;
                    }
                    if(tg.Key != null && tg.Key == col.ToLower())
                    {
                        keyval = val;
                    }
                    node.AppendChild(subnode);
                }
                if (keyval == null)//没遇到主键，下一行
                {
                    parent.AppendChild(node);
                    continue;
                }
                int NextIdx = dtindex + 1;
                if(ds.Tables.Count<= NextIdx)
                {
                    parent.AppendChild(node);
                    continue;
                }
                guds[NextIdx].KeyValue = keyval;
                node = FillXmlByDatable(node, ds, NextIdx, guds);
                parent.AppendChild(node);
            }
            return parent;
        }


        public XmlDocument getFilterSchema()
        {
            if (this.Module.RequestSchema == null)
                this.Module.RequestSchema = "";
            string xmlreq = null;
            if (xmlreq == null || xmlreq.Trim().Length == 0)//如果获取不到，获取过滤请求
                xmlreq = jdy_GlbObject.getText("Schema\\System.Bussiness.Item.Filter.Model.xml", "", "");
            if (xmlreq == null || xmlreq.Trim().Length == 0)
                return null;
            XmlDocument ret = new XmlDocument();
            try
            {
                ret.LoadXml(xmlreq);
                return ret;
            }
            catch
            {
                return null;
            }
        }


        public XmlDocument getRequestSchema()
        {
            if (this.Module.RequestSchema == null)
                this.Module.RequestSchema = "";
            string xmlreq = jdy_GlbObject.getText(this.Module.RequestSchema);
            if (xmlreq == null || xmlreq.Trim().Length == 0)//如果获取不到，获取过滤请求
                xmlreq = jdy_GlbObject.getText("Schema\\System.Bussiness.Item.Model.xml", "", "");
            if (xmlreq == null || xmlreq.Trim().Length == 0)
                return null;
            XmlDocument ret = new XmlDocument();
            try
            {
                ret.LoadXml(xmlreq);
                return ret;
            }
            catch
            {
                return null;
            }
        }
    }

    
    public abstract class JDYSCM_Bussiness_Add_Return_Class : JdyRequestClass
    {


        public class JDYSCM_Bussiness_Filter_Class : JsonableClass<JDYSCM_Bussiness_Filter_Class>
        {
            //public string updTimeBegin { get; set; }//": "2019-01-08",
            //public string updTimeEnd { get; set; }//":"2019-10-09",
            public int pageSize { get; set; }//":20,
            public int page { get; set; }//":1
        }
    }


    public class JDYSCM_Service_List_Class: JDYSCM_Class
    {
        public new List<JDYSCM_Service_List_Item_Class> items { get; set; }
        public class JDYSCM_Service_List_Item_Class : JDYSCM_Class.JDYSCM_Item_Class
        {
            public string dbId { get; set; }
            public bool isFree {get;set;}
            public string name { get; set; }
            public string endDate { get; set; }
            public string beginDate { get; set; }
            //// "dbId": 7950938951,
            ////"isFree": false,
            ////"name": "在线进销存3.0（标准版）",
            ////"endDate": "2022-02-24",
            ////"beginDate": "2017-02-24"

        }

        public bool InitRequestJson()
        {
            base.InitRequestJson();
            ReqJson = string.Format("access_token={0}", jdy_GlbObject.Access_token);

            return true;

        }
    }

    
    public class JDYSCM_Product_List_Class: JDYSCM_Bussiness_List_Class
    {
        public JDYSCM_Product_List_Class()
        {

        }

        public List<JDYSCM_Bussiness_Item_Product_List_Class> items { get; set; }

        public class JDYSCM_Bussiness_Filter_Product_Class : JDYSCM_Bussiness_List_Class.JDYSCM_Bussiness_Filter_Class
        {
            public string productNumber { get; set; }
	         public string productName { get; set; }//": "iphone7",
	         public string spec { get; set; }//": "64G",
	         public int status { get; set; }//": 0,
	         public string categoryId { get; set; }//":""
	         
        }

        public class JDYSCM_Bussiness_Item_Product_List_Class
        {
            public string id { get; set; }
            public string productNumber { get; set; }
            public string productName { get; set; }
            public string spec { get; set; }
            public string categoryId { get; set; }
            public string categoryName { get; set; }
            public string unitType { get; set; }
            public string unitTypeName { get; set; }
            public string unit { get; set; }
            public string unitName { get; set; }
            public string barcode { get; set; }
            public string primaryStock { get; set; }
            public string retailPrice { get; set; }
            public string wholeSalePrice { get; set; }
            public string vipPrice { get; set; }
            public string discount { get; set; }
            public string discount2 { get; set; }
            public string elsPurPrice { get; set; }
            public bool isDeleted { get; set; }
            public DateTime createTime { get; set; }
            public DateTime updateTime { get; set; }
            public bool isWarranty { get; set; }
            public int safeDays { get; set; }
            public int advanceDays { get; set; }
        }
    }
    public class JDYSCM_Product_Add_Class : JDYSCM_Bussiness_Class
    {
        public List<JDYSCM_Bussiness_Item_Product_Add_Class> items;
        public class JDYSCM_Bussiness_Item_Product_Add_Class:JsonableClass<JDYSCM_Bussiness_Item_Product_Add_Class>
        {
            public string productNumber { get; set; }
            public string productName { get; set; }
            public string spec { get; set; }
            public string category { get; set; }
            public string unitType { get; set; }
            public string unit { get; set; }
            public string barcode { get; set; }
            public string primaryStock { get; set; }
            public int retailPrice { get; set; }
            public int wholeSalePrice { get; set; }
            public int vipPrice { get; set; }
            public int discount { get; set; }
            public int discount2 { get; set; }
            public int elsPurPrice { get; set; }
        }
    }
   
    public class JDYSCM_Product_Unit_List_Class: JDYSCM_Bussiness_List_Class
    {
        public List<Product_Unit_Class> items { get; set; }
        public class Product_Unit_Class
        {
            public string name { get; set; }
            public string ID { get; set; }
            public List<Product_Unit_Item> entries { get; set; }
        }

        public class Product_Unit_Item
        {
            public string name { get; set; }
            public string ID { get; set; }
        }
    }

    public class JDYSCM_WareHouse_List_Class: JDYSCM_Bussiness_List_Class
    {
        public List<WareHouse_List_Item> items { get; set; }
        //https://api.kingdee.com/jdyscm/warehouse/list
        public class WareHouse_List_Item: SimpleListItem
        {
            public bool isDeleted { get; set; }
        }
    }

    public class JDYSCM_Supplier_List_Class: JDYSCM_Bussiness_List_Class
    {
        public List<Supplier_Litem_Item> items{ get; set; }
        //https://api.kingdee.com/jdyscm/supplier/list
        public class Supplier_Litem_Item: SimpleListItem
        {
            
            public string contact { get; set; }
        }
    }
    public class JDYSCM_Category_List_Class : JDYSCM_Bussiness_List_Class
    {
        public List<Category_Litem_Item> items { get; set; }
        //https://api.kingdee.com/jdyscm/supplier/list
        public class Category_Litem_Item : SimpleListItem
        {

            public string parentId { get; set; }
        }
    }
    public class JDYSCM_Customer_List_Class : JDYSCM_Bussiness_List_Class
    {
        public List<Customer_Litem_Item> items { get; set; }
        //https://api.kingdee.com/jdyscm/supplier/list
        public class Customer_Litem_Item : SimpleListItem
        {
            public string id { get; set; }


            public string category { get; set; }
            public string level { get; set; }
            public string employeeName { get; set; }
            public string difMoney { get; set; }
            public string taxPayerNo { get; set; }
            public string bank { get; set; }
            public string cardNo { get; set; }
            public string beginDate { get; set; }
            public long amount { get; set; }
            public long periodMoney { get; set; }
            public string remark { get; set; }
            public bool isDeleted { get; set; }
            public string createTime { get; set; }
            public string updateTime { get; set; }
            public List<Customer_Contactors_Item> contacts { get; set; }
        }

        public class Customer_Contactors_Item
        {
            public string name { get; set; }
            public string im { get; set; }
            public string mobile { get; set; }
            public string id { get; set; }
            public string phone { get; set; }
            public string address { get; set; }
            public bool isPrimary { get; set; }
        }
    }

    public class SimpleListItem
    {
        public string number { get; set; }
        public string name { get; set; }
    }
}

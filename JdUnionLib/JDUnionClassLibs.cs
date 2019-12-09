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

    public abstract class JdUnion_Class: JdUnion_RequestClass
    {
        public JdUnion_Class()
        {

        }
        public string dbId { get; set; } //账套Id
        
        public int page { get; set; }   //当前页码
        public int totalsize { get; set; }  //当前返回总记录数
        public int records { get; set; }    //总记录数
        public int totalPages { get; set; }	//总页数
        //public List<JDYSCM_Item_Class> items { get; set; }

        public class JdUnion_Item_Class:JdUnion_JsonClass
        {

        }

        
    }

    
    public abstract class JdUnion_Bussiness_Class: JdUnion_Class
    {

        public bool InitRequestJson()
        {
            base.InitRequestJson();
            ReqJson = FullRequestString;

            return true;

        }

        public string getUrl()
        {
            string url = string.Format("{0}{1}", ReqUrl, ReqJson);
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

    public abstract class JdUnion_Bussiness_List_Class : JdUnion_Bussiness_Class
    {
        public JdUnion_Bussiness_Filter_Class filter;
        public class JdUnion_Bussiness_Filter_Class:JsonableClass<JdUnion_Bussiness_Filter_Class>
        {
            //public string updTimeBegin { get; set; }//": "2019-01-08",
            //public string updTimeEnd { get; set; }//":"2019-10-09",
            public int pageSize { get; set; }//":20,
            public int pageIndex { get; set; }//":1
        }

        public void RequestSizeAndPage(int pageSize, int page, XmlNode reqnode = null)
        {
            if(this.Module.RequestMethodUseGET)
            {
                this.ReqJson = string.Format("{0}&pageSize={1}&pageIndex={2}", this.ReqJson, pageSize, page); 
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
            node = root.SelectSingleNode("pageIndex");
            if (node == null)
            {
                node = XmlUtil.AddSubNode(root, "pageIndex", page.ToString());
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
                xmlreq = JdUnion_GlbObject.getText("Schema\\System.Bussiness.Item.Filter.Model.xml", "", "");
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
            string xmlreq = JdUnion_GlbObject.getText(this.Module.RequestSchema);
            if (xmlreq == null || xmlreq.Trim().Length == 0)//如果获取不到，获取过滤请求
                xmlreq = JdUnion_GlbObject.getText("Schema\\System.Bussiness.Item.Model.xml", "", "");
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

    
    
    


}

using System;
using System.Collections.Generic;
using System.Linq;
using WolfInv.Com.JsLib;
using System.Xml;
using System.Data;
using XmlProcess;
using System.Reflection;
using System.Web;

namespace WolfInv.com.JdUnionLib
{
    public abstract class JdUnion_Bussiness_List_Class : JdUnion_Bussiness_Class
    {

        public JdUnion_Bussiness_Filter_Class pager = new JdUnion_Bussiness_Filter_Class();
        public class JdUnion_Bussiness_Filter_Class:JsonableClass<JdUnion_Bussiness_Filter_Class>
        {
            //public string updTimeBegin { get; set; }//": "2019-01-08",
            //public string updTimeEnd { get; set; }//":"2019-10-09",
            int _pageSize = 50;
            int _pageIndex = 1;
            public int pageSize { get { return _pageSize; } set { _pageSize = value; } }//":20,
            public int pageIndex { get { return _pageIndex; } set { _pageIndex = value; } }//":1
        }

        public void RequestSizeAndPage(int pageSize, int page, XmlNode reqnode = null)
        {
            if(this.Module.RequestMethodUseGET)
            {
                Dictionary<string, string> obj = new Dictionary<string, string>();

                obj.Add(string.Format("{0}/pageSize", this.params_360buy.First().Key), pageSize.ToString());
                obj.Add(string.Format("{0}/pageIndex", this.params_360buy.First().Key), page.ToString());
                this.setBussiessItems(obj);
                return;
                //this.ReqJson = string.Format("{0}&pageSize={1}&pageIndex={2}", this.ReqJson, pageSize, page); 
                KeyValuePair<string, object> fp = this.params_360buy.First();
                if(fp.Value == null)
                {
                    this.params_360buy[fp.Key] = "";
                }
                if(fp.Value.ToString().StartsWith("{") && fp.Value.ToString().EndsWith("}"))
                {
                    string xmljson = "{" + string.Format("\"{0}\":{1}", fp.Key, fp.Value) + "}";
                    XmlDocument doc = new XmlDocument();
                    try
                    {
                        doc.LoadXml(XML_JSON.Json2XML(xmljson));
                        XmlNode root = doc.SelectSingleNode(fp.Key);
                        XmlNode IndexNode = root.SelectSingleNode("pageIndex");
                        XmlNode SizeNode = root.SelectSingleNode("pageSize");
                        if(IndexNode == null)
                        {
                            XmlUtil.AddSubNode(root, "pageIndex", page.ToString());
                        }
                        else
                        {
                            IndexNode.InnerText = page.ToString();
                        }
                        if(SizeNode == null)
                        {
                            XmlUtil.AddSubNode(root, "pageIndex", pageSize.ToString());
                        }
                        else
                        {
                            SizeNode.InnerText = pageSize.ToString();
                        }
                        this.params_360buy[fp.Key]= XML_JSON.XML2Json(doc, fp.Key,true);
                    }
                    catch(Exception ce)
                    {

                    }
                }
                return;
                Dictionary<string,object> curr = (fp.Value as Dictionary<string, object>);
                if(curr.ContainsKey("pageSize"))
                {
                    curr["pageSize"] = pageSize;
                }
                else
                {
                    curr.Add("pageSize", pageSize);
                }
                if (curr.ContainsKey("pageIndex"))
                {
                    curr["pageIndex"] = pageSize;
                }
                else
                {
                    curr.Add("pageIndex", pageSize);
                }
                return;
            }
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
                throw new Exception("JdUion:数据集超出索引！");
            }
            if (guds.Count <= dtindex)
            {
                throw new Exception("JdUion:Schema配置超出索引！");
            }
            
            TableGuider tg = guds[dtindex];
            TableGuider pretg = null;
            if(dtindex>0)
            {
                pretg = guds[dtindex - 1];
            }
            if(tg.TableName==null || tg.TableName.Trim().Length == 0)
                throw new Exception("JdUion:Schema配置Json超出索引！");
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

        public bool getXmlData( ref XmlDocument retdoc, ref XmlDocument xmlschemaDoc, ref string msg,bool onlyFirstPage = true)
        {
            JdUnion_Bussiness_List_Class jdyreq = this;
            string strDefaultName = "DataTable1";
            string strRootName = "NewDataSet";
            string ret = null;
            XmlDocument doc = new XmlDocument();
            if(xmlschemaDoc == null)
                xmlschemaDoc = new XmlDocument();
            msg = null;
            try
            {
                //jdyreq.InitClass(jdyreq.Module);
                jdyreq.sign = null;
                jdyreq.RequestSizeAndPage(jdyreq.pager.pageSize, 1, null);
                jdyreq.InitRequestJson();
                ret = jdyreq.GetRequest();
                xmlschemaDoc = jdyreq.getRequestSchema();
                XmlSchemaClass xmlSchema = new XmlSchemaClass(xmlschemaDoc);
                //string strXml = XML_JSON.Json2XML(ret);
                XmlDocument tmp = jdyreq.getRealXml(ret, out msg);
                if(msg != null)
                {
                    return false;
                }
                doc.LoadXml(tmp.OuterXml);



                if (onlyFirstPage)
                {
                    return getXmlDataSet(doc, xmlSchema, ref retdoc, ref msg);
                    //return true;
                }
                int? totalCount = XmlNodeIntVal(doc, xmlSchema.rootNodePath, xmlSchema.totalCountItemName);//, //int.Parse(XmlUtil.GetSubNodeText(tmp, xmlSchema.pageCountItemName));
                int? totalsize = XmlNodeIntVal(doc, xmlSchema.rootNodePath, xmlSchema.totalCountItemName);
                if(totalsize == null)
                    totalsize = this.pager.pageSize;//按实际返回的数量计算页大小
                int totalPage = (totalCount.Value / totalsize.Value) + ((totalCount.Value % totalsize.Value) == 0 ? 0 : 1);
                if (totalPage > 1)//如果不止一页
                {
                    XmlNode rootnode = doc.SelectSingleNode(xmlSchema.rootNodePath);
                    for (int i = 2; i <= totalPage; i++)
                    {
                        jdyreq.RequestSizeAndPage(this.pager.pageSize, i, null);
                        jdyreq.sign = null;//必须重置
                        jdyreq.InitRequestJson();
                        ret = jdyreq.GetRequest();
                        
                        XmlDocument ntmp = jdyreq.getRealXml(ret,out msg);
                        if(msg != null)
                        {
                            return false;
                        }
                        foreach(XmlNode node in ntmp.SelectNodes(xmlSchema.dataItemName))//子节点全部加入根结点
                        {
                            rootnode.AppendChild(doc.ImportNode(node, true));
                        }
                    }
                }
                return getXmlDataSet(doc, xmlSchema, ref retdoc, ref msg);
            }
            catch (Exception e)
            {
                msg = e.Message;
                return false;
            }
            //return true;
        }

        public int? XmlNodeIntVal(XmlDocument xmldoc,string rootPath,string subPath)
        {
            string xPath = string.Format("{0}/{1}", rootPath, subPath);
            string strVal = XmlUtil.GetSubNodeText(xmldoc, xPath);
            if(string.IsNullOrEmpty(strVal))
            {
                return null;
            }
            int ival = 0;
            if(int.TryParse(strVal,out ival))
            {
                return ival;
            }
            return null;
        }



        bool getXmlDataSet(XmlDocument xmldoc,XmlSchemaClass xmlSchema,ref XmlDocument retDoc, ref string msg)
        {
            
            if (xmlSchema == null)
            {
                retDoc = xmldoc;
            }
            retDoc = new XmlDocument();
            Dictionary<string, TableGuider> tablist = xmlSchema.TableList;
            retDoc.LoadXml("<NewDataSet/>");
            if (xmldoc == null)
            {
                return false;
            }
            
            if (tablist.Count == 0)
            {
                msg = "未指定指引配置！";
                return false;
            }
            TableGuider tg = tablist.First().Value;
            if(tg == null)
            {
                msg = "未配置表结构！";
                return false;
            }
            List<TableGuider> subtabs = tablist.Values.Skip(1).ToList();
            string mainname = tg.TableName;
            XmlNode rootnode = retDoc.SelectSingleNode("NewDataSet");
            if (string.IsNullOrEmpty(mainname))
            {
                msg = "指定指引中主表名未配置！";
                return false;
            }
            XmlNamespaceManager xmlm = new XmlNamespaceManager(xmldoc.NameTable);
            xmlm.AddNamespace("json", "http://james.newtonking.com/projects/json");//添加命名空间
            XmlNode root = xmldoc.SelectSingleNode(tg.rootNodePath);
            XmlNodeList itemnodes = root.SelectNodes(tg.dataItemName);
            foreach (XmlNode dataNode in itemnodes)
            {
                string submsg = null;
                if (XmlUtil.GetSubNodeText(itemnodes[0], tg.msgItemName) != "" && XmlUtil.GetSubNodeText(dataNode, tg.msgItemName) != "0")
                {
                    submsg = XmlUtil.GetSubNodeText(dataNode, tg.msgItemName);
                    msg = submsg;
                    return false;
                }

                //XmlNode NewRow = xmldoc.CreateElement(mainname, xmlm.LookupNamespace("json"));
                XmlNode NewRow = retDoc.CreateElement(mainname);
                //XmlAttribute att = xmldoc.CreateAttribute("json:Array", xmlm.LookupNamespace("json"));
                //att.Value = "true";
                //NewRow.Attributes.Append(att);
                string tableKey = tg.Key;
                string keyValue = null;
                if(tg.Columns.Count>0)
                {
                    foreach(string key in tg.Columns.Keys)
                    {
                        ResponseDataColumn rdc = tg.Columns[key];
                        XmlNode cellNode = retDoc.CreateElement(key);
                        if (!rdc.outXml)
                            cellNode.InnerText = XmlUtil.GetSubNodeText(dataNode, rdc.xPath);
                        else
                        {
                            //System.Web.HttpUtility.UrlEncode
                            string strOutXml = HttpUtility.HtmlEncode(dataNode.SelectSingleNode(rdc.xPath)?.OuterXml);
                            cellNode.InnerXml = strOutXml;
                        }
                        if (key == tableKey)
                        {
                            keyValue = cellNode.InnerText;
                        }
                        NewRow.AppendChild(cellNode);
                    }
                }
                
                rootnode.AppendChild(NewRow);
                for(int i=0;i< subtabs.Count;i++)
                {
                    TableGuider stg = subtabs[i];
                    string sItemPath = string.Format("{0}/{1}", stg.rootNodePath, stg.dataItemName);
                    XmlNodeList sDataNodes = dataNode.SelectNodes(sItemPath);
                    if(sDataNodes.Count==0)
                    {
                        continue;
                    }
                    XmlNode subTableRow = retDoc.CreateElement(stg.TableName);
                    foreach(XmlNode sitem in sDataNodes)
                    {

                        if (stg.Columns.Count > 0)
                        {
                            bool ExistKey = false;
                            bool ExistKeyItem = false;
                            foreach (string key in stg.Columns.Keys)
                            {
                                ResponseDataColumn rdc = stg.Columns[key];
                                XmlNode cellNode = retDoc.CreateElement(key);
                                if (!rdc.outXml)
                                    cellNode.InnerText = XmlUtil.GetSubNodeText(dataNode, rdc.xPath);
                                else
                                    cellNode.InnerXml = XmlUtil.GetSubNodeXml(dataNode, rdc.xPath);
                                subTableRow.AppendChild(cellNode);
                                if(key == tableKey )
                                {
                                    ExistKeyItem = true;
                                    if( cellNode.InnerText == keyValue)
                                        ExistKey = true;
                                }
                            }
                            XmlNode keyNode = null;
                            if (ExistKeyItem)//存在键项
                            {
                                if(!ExistKey)//存在同名键，另外新建一个表名_健名，如果虚拟键恰巧存在就通过修改表名避免
                                {
                                    string parentKey = string.Format("{0}_{1}", tg.TableName, tableKey);
                                    XmlNode virtualKeyNode = retDoc.CreateElement(parentKey);
                                    virtualKeyNode.InnerText = keyValue;
                                    subTableRow.AppendChild(virtualKeyNode);

                                }

                            }
                            else //没有存在键相，直接插入一个
                            { 
                                keyNode = retDoc.CreateElement(tableKey);
                                keyNode.InnerText = keyValue;
                                subTableRow.AppendChild(keyNode);
                            }
                            
                            
                        }
                    }
                    rootnode.AppendChild(subTableRow);
                    
                }
            }    
            return true;
        }

    }






}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using WolfInv.Com.AccessWebLib;
using WolfInv.Com.JsLib;
using XmlProcess;

namespace WolfInv.com.JdUnionLib
{
    public abstract class JdUnion_Bussiness_Class: JdUnion_Class
    {
        Dictionary<string, TableGuider> dictabs = new Dictionary<string, TableGuider>();
        public bool InitRequestJson()
        {
            this.sign = null;
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
        bool getXmlDataSet(XmlDocument xmldoc, XmlSchemaClass xmlSchema, ref XmlDocument retDoc, ref string msg)
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
            if (tg == null)
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
            string message = XmlUtil.GetSubNodeText(root, tg.msgItemName);
            if(!string.IsNullOrEmpty(message))
            {
                if (message.Trim() != "success")
                {
                    msg = message;
                    return false;
                }
            }
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
                if (tg.Columns.Count > 0)
                {
                    foreach (string key in tg.Columns.Keys)
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
                for (int i = 0; i < subtabs.Count; i++)
                {
                    TableGuider stg = subtabs[i];
                    string sItemPath = string.Format("{0}/{1}", stg.rootNodePath, stg.dataItemName);
                    XmlNodeList sDataNodes = dataNode.SelectNodes(sItemPath);
                    if (sDataNodes.Count == 0)
                    {
                        continue;
                    }
                    XmlNode subTableRow = retDoc.CreateElement(stg.TableName);
                    foreach (XmlNode sitem in sDataNodes)
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
                                if (key == tableKey)
                                {
                                    ExistKeyItem = true;
                                    if (cellNode.InnerText == keyValue)
                                        ExistKey = true;
                                }
                            }
                            XmlNode keyNode = null;
                            if (ExistKeyItem)//存在键项
                            {
                                if (!ExistKey)//存在同名键，另外新建一个表名_健名，如果虚拟键恰巧存在就通过修改表名避免
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

        public bool getXmlData(ref XmlDocument retdoc, ref XmlDocument xmlschemaDoc, ref string msg, bool onlyFirstPage = true, bool onlyOrgData = false)
        {
            JdUnion_Bussiness_Class jdyreq = this;
            string strDefaultName = "DataTable1";
            string strRootName = "NewDataSet";
            string ret = null;
            XmlDocument doc = new XmlDocument();
            if (xmlschemaDoc == null)
                xmlschemaDoc = new XmlDocument();
            msg = null;
            try
            {
                //jdyreq.InitClass(jdyreq.Module);
                jdyreq.sign = null;
                if (jdyreq is JdUnion_Bussiness_List_Class)
                {
                    (jdyreq as JdUnion_Bussiness_List_Class).RequestSizeAndPage((jdyreq as JdUnion_Bussiness_List_Class).pager.pageSize, 1, null);
                }
                jdyreq.InitRequestJson();
                ret = jdyreq.GetRequest();
                xmlschemaDoc = jdyreq.getRequestSchema();
                XmlSchemaClass xmlSchema = new XmlSchemaClass(xmlschemaDoc);
                //string strXml = XML_JSON.Json2XML(ret);
                XmlDocument tmp = jdyreq.getRealXml(ret, out msg);
                if (msg != null)
                {
                    return false;
                }
                doc.LoadXml(tmp.OuterXml);
                if (onlyOrgData)
                {
                    if (retdoc == null)
                        retdoc = new XmlDocument();
                    retdoc.LoadXml(tmp.OuterXml);
                    return true;
                }
                if (jdyreq is JdUnion_Bussiness_List_Class)
                {
                    if (onlyFirstPage)
                    {
                        return getXmlDataSet(doc, xmlSchema, ref retdoc, ref msg);
                        //return true;
                    }
                    //return true;
                }
                else
                {
                    return getXmlDataSet(doc, xmlSchema, ref retdoc, ref msg);
                }
                
                
                int? totalCount = XmlNodeIntVal(doc, xmlSchema.rootNodePath, xmlSchema.totalCountItemName);//, //int.Parse(XmlUtil.GetSubNodeText(tmp, xmlSchema.pageCountItemName));
                int? totalsize = XmlNodeIntVal(doc, xmlSchema.rootNodePath, xmlSchema.pageSizeItemName);
                if (totalsize == null)
                    totalsize = (this as JdUnion_Bussiness_List_Class).pager.pageSize;//按实际返回的数量计算页大小
                int totalPage = (totalCount.Value / totalsize.Value) + ((totalCount.Value % totalsize.Value) == 0 ? 0 : 1);
                if (totalPage > 1)//如果不止一页
                {
                    XmlNode rootnode = doc.SelectSingleNode(xmlSchema.rootNodePath);
                    for (int i = 2; i <= totalPage; i++)
                    {
                        (jdyreq as JdUnion_Bussiness_List_Class).RequestSizeAndPage((this as JdUnion_Bussiness_List_Class).pager.pageSize, i, null);
                        jdyreq.sign = null;//必须重置
                        jdyreq.InitRequestJson();
                        ret = jdyreq.GetRequest();

                        XmlDocument ntmp = jdyreq.getRealXml(ret, out msg);
                        if (msg != null)
                        {
                            continue;
                            //return false;
                        }
                        string xpath = string.Format("{0}/{1}", xmlSchema.rootNodePath, xmlSchema.dataItemName);
                        foreach (XmlNode node in ntmp.SelectNodes(xpath))//子节点全部加入根结点
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


        public int? XmlNodeIntVal(XmlDocument xmldoc, string rootPath, string subPath)
        {
            string xPath = string.Format("{0}/{1}", rootPath, subPath);
            string strVal = XmlUtil.GetSubNodeText(xmldoc, xPath);
            if (string.IsNullOrEmpty(strVal))
            {
                return null;
            }
            int ival = 0;
            if (int.TryParse(strVal, out ival))
            {
                return ival;
            }
            return null;
        }





        public XmlDocument getRealXml(string strJson, out string msg)
        {
            msg = null;
            XmlDocument ret = new XmlDocument();
            try
            {
                string retXml = XML_JSON.Json2XML(strJson);
                if (retXml == null)
                    return null;
                ret.LoadXml(retXml);
                XmlNode root = ret.SelectSingleNode(".");
                XmlNode code = root.FirstChild.SelectSingleNode("code");
                if (code.InnerText != "0")
                {
                    msg = code.InnerText;
                    return null;
                }
                XmlNode message = root.FirstChild.SelectSingleNode("message");
                
                XmlNode result = ret.FirstChild.SelectSingleNode("result");
                
                string res = "{\"root\":{0}}".Replace("{0}", result.InnerText.Trim());
                string strResult = XML_JSON.Json2XML(res);
                ret.LoadXml(strResult);
                return ret;
            }
            catch (Exception ce)
            {
                msg = ce.Message;
                return null;
            }
            return ret;
        }

        public void setBussiessItems(string path, string val)
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add(path, val);
            setBussiessItems(obj);
        }
        public void setBussiessItems(Dictionary<string,string> keyVals)
        {
            string rootName = "param_json";
            XmlDocument xmldoc = new XmlDocument();
            
            try
            {
                string stringBussiness = this.Buy360String(this.params_360buy);
                string strJson = "{\"param_json\":{0}}".Replace("{0}", stringBussiness);
                string strXml = XML_JSON.Json2XML(strJson);
               
                xmldoc.LoadXml(strXml);
                XmlNode rootNode = xmldoc.SelectSingleNode(rootName);
                foreach (string path in keyVals.Keys)
                {
                    string val = keyVals[path];
                    string strPath = string.Format("{0}", path);
                    XmlNode node = rootNode.SelectSingleNode(strPath);
                    if (node == null)
                    {
                        node = XmlUtilEx.CreateNode(rootNode,strPath);
                    }
                    node.InnerText = val;
                }
                Reset360BuyItems(xmldoc);
            }
            catch(Exception ce)
            {

            }
            
            return;
        }



        void Reset360BuyItems(XmlDocument xmldoc)
        {
            if (xmldoc == null)
                return;
            this.params_360buy.Clear();
            foreach(XmlNode node in xmldoc.FirstChild.ChildNodes)
            {
                string name = node.Name;
                string strPath = string.Format("{0}/{1}", xmldoc.FirstChild.Name, name);
                string strJson = XML_JSON.XML2Json(xmldoc, strPath, true);
                this.params_360buy.Add(name, strJson);
            }
        }
    }

    
    public class XmlUtilEx:XmlUtil
    {
        public static XmlNode CreateNode(XmlNode TopNode, string xPath)
        {
            XmlNode currNode = TopNode;
            XmlNode ret = null;
            XmlNode NewNode = null;
            string[] paths = xPath.Split('/');//分为
            for(int i=0;i< paths.Length;i++)
            {
                string strCheckPath = string.Join("/", paths, 0, i+1);
                NewNode = currNode.SelectSingleNode(strCheckPath);
                string NewName = paths[i].Trim();
                if(NewNode == null)
                {
                    string ParentXPath = string.Join("/", paths, 0, i);
                    XmlNode parentNode = TopNode.SelectSingleNode(ParentXPath);
                    if(NewName.IndexOf("[")<0 && NewName.EndsWith("]") == false)//未包含[],纯节点或者属性
                    {
                        if (NewName.StartsWith("@"))
                        {
                            NewNode = TopNode.OwnerDocument.CreateAttribute(NewName.Replace("@", ""));
                        }
                        else
                        {
                            NewNode = TopNode.OwnerDocument.CreateElement(NewName);
                        }
                        parentNode.AppendChild(NewNode);
                    }
                    else //支持一层[], 且值不能包含=号//内可能是@，节点=val形式
                    {
                        string pureName = NewName.Substring(0, NewName.IndexOf("[") + 1);
                        NewNode = TopNode.OwnerDocument.CreateElement(pureName);
                        parentNode.AppendChild(NewNode);
                        string strAtts = NewName.Substring(NewName.IndexOf("[") + 1);//去掉[
                        strAtts = strAtts.Trim().Substring(0, strAtts.Trim().Length - 1);//去掉]
                        string[] Arrs = strAtts.Split('=');
                        if(Arrs.Length != 2)
                        {
                            throw new Exception("非[@]xxx=yyy形式");
                        }
                        string val = Arrs[0].Replace("\"", "").Replace("'", "");
                        if (Arrs[0].Trim().StartsWith("@"))
                        {
                            string attName = Arrs[0].Replace("@", "");
                            XmlAttribute att = TopNode.OwnerDocument.CreateAttribute(attName);
                            att.Value = val;
                            NewNode.Attributes.Append(att);
                        }
                        else
                        {
                            NewNode.AppendChild(TopNode.OwnerDocument.CreateElement(Arrs[0].Trim()));
                        }
                    }
                    
                }
                
            }
            return NewNode;
        }
    }



}

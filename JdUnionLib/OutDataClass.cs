using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using WolfInv.Com.WCSExtraDataInterface;
using WolfInv.Com.JsLib;
using XmlProcess;
using System.Reflection;
namespace WolfInv.com.JdUnionLib
{
    public class JdUnion_OutDataClass:IWCSExtraDataInterface
    {
        string strAccess_Token;
        string strdbId;
        public JdUnion_OutDataClass()
        {
            //strAccess_Token = jdy_GlbObject.Access_token;
            //strdbId = jdy_GlbObject.dbId;
        }

        
        string getTranReqs(XmlNode node)
        {
            if (node == null)
                return null;
            return null;
        }

        public bool getXmlData(XmlNode config, ref XmlDocument doc,ref XmlDocument xmlschemaDoc,ref string msg)
        {
            string strDefaultName = "DataTable1";
            string strRootName = "NewDataSet";
            string ret = "";
            string strName = XmlUtil.GetSubNodeText(config, "module/@name");
            string strReqJson = null;

            XmlNode xmlreq = config.SelectSingleNode("req");
            Assembly assem = Assembly.GetExecutingAssembly();
            try
            {
                List<int> list = JdUnion_GlbObject.getElites();
                Dictionary<string, string> cols = null;
                Type t = assem.GetType(string.Format("{0}.{1}",assem.FullName.Split(',')[0],strName));
                if(t == null)
                {
                    msg = "无法识别的外部访问类";
                    return false;
                }
                XmlSchemaClass schema = null;
                XmlNode rootNode = null;
                foreach (int elite in list)
                {
                    JdUnion_Goods_List_Class jgl = JdUnion_GlbObject.CreateBusinessClass(typeof(JdUnion_Goods_List_Class)) as JdUnion_Goods_List_Class;
                    if (cols == null)
                    {
                        cols = new Dictionary<string, string>();

                    }
                    //jgl.InitClass(jgl.Module);//必须初始化，获取到json设置才能用。
                    XmlDocument xmldoc = null;
                    string strElite = string.Format("goodsReq/eliteId");
                    jgl.setBussiessItems(strElite, elite.ToString());
                    jgl.sign = null;
                    XmlDocument xmlschema = null;
                    bool succ = jgl.getXmlData(ref xmldoc, ref xmlschema, ref msg, false);
                    if (succ == false)
                    {
                        if(msg != null)
                        {
                            
                        }
                        continue;
                    }
                    if(schema == null)
                    {
                        schema = new XmlSchemaClass(xmlschema);
                        xmlschemaDoc = xmlschema;
                    }
                    if (doc == null)
                    {
                        doc = xmldoc;
                        rootNode = doc.SelectSingleNode(strRootName);
                    }
                    else //把新的节点全部复制过去
                    {
                        foreach (string key in schema.TableList.Keys)
                        {
                            string xpath = string.Format("{0}/{1}", strRootName, key);
                            foreach (XmlNode node in xmldoc.SelectNodes(xpath))
                            {
                                rootNode.AppendChild(doc.ImportNode(node, true));
                            }
                        }
                    }
                }
                return true;

                /*
                jdyreq.InitClass(JdUnion_GlbObject.mlist[t.Name]);
                jdyreq.InitRequestJson();
                jdyreq.RequestSizeAndPage(20, 1,xmlreq);
                if(JdUnion_GlbObject.mlist[t.Name].RequestMethodUseGET)
                {
                    ret = jdyreq.GetRequest();
                }
                else
                    ret = jdyreq.PostRequest();
                string strXml = XML_JSON.Json2XML(ret);
                XmlDocument schemadoc = jdyreq.getRequestSchema();
                XmlDocument tmp = new XmlDocument();
                tmp.LoadXml(strXml);
                Json2XmlClass j2x = new Json2XmlClass(schemadoc);
                if(!j2x.getDataSetXml(ret, ref doc, ref msg))
                {
                    return false;
                }
                int totalPage = int.Parse(XmlUtil.GetSubNodeText(tmp, "root/totalPages"));
                int totalsize = int.Parse(XmlUtil.GetSubNodeText(tmp, "root/totalsize"));
                if (totalPage>1)//如果不止一页
                {
                    for(int i=2;i<=totalPage;i++)
                    {
                        jdyreq.RequestSizeAndPage(totalsize, i,xmlreq);
                        if (JdUnion_GlbObject.mlist[t.Name].RequestMethodUseGET)
                        {
                            ret = jdyreq.GetRequest();
                        }
                        else
                            ret = jdyreq.PostRequest();
                        strXml = XML_JSON.Json2XML("{\"root\":{0}}".Replace("{0}", ret));
                        tmp = new XmlDocument();
                        tmp.LoadXml(strXml);
                        if(!j2x.getDataSetXml(ret,ref doc,ref msg))
                        {
                            return false;
                        }
                    }
                }
                */

            }
            catch(Exception e)
            {
                msg = e.Message;
                return false;
            }
            return true;
        }

       

       
       
        public bool writeXmlData(XmlNode config, DataSet data,ref XmlDocument doc, ref XmlDocument xmlschema,ref string msg, string writetype = "Add")
        {
            doc = null;
            string strDefaultName = "DataTable1";
            string strRootName = "NewDataSet";
            string ret = "";
            string strName = XmlUtil.GetSubNodeText(config, "module/@name");
            
            
            Assembly assem = Assembly.GetExecutingAssembly();
            strName = string.Format(strName, writetype);
            try
            {
                Type t = assem.GetType(string.Format("{0}.{1}", assem.FullName.Split(',')[0], strName));
                if (t == null)
                {
                    msg = "无法识别的类型";
                    return false;
                }

                JdUnion_Bussiness_List_Class jdyreq = Activator.CreateInstance(t) as JdUnion_Bussiness_List_Class;
                ////if (jdyreq is JDYSCM_SaleOrder_Update_Class)
                ////{
                ////    msg = "销售订单外部数据不能修改，只能新增或删除！";
                ////    return false;
                ////}
                jdyreq.InitClass(JdUnion_GlbObject.mlist[t.Name]);
                jdyreq.InitRequestJson();
                XmlDocument schemadoc = jdyreq.getRequestSchema();
                Json2XmlClass j2x = new Json2XmlClass(schemadoc);
                //jdyreq.Req_PostData = getRequestJson(jdyreq,data);
                jdyreq.Req_PostData = j2x.getJsonString(data);
                if (JdUnion_GlbObject.mlist[t.Name].RequestMethodUseGET)
                {
                    ret = jdyreq.GetRequest();
                }
                else
                    ret = jdyreq.PostRequest();
                string strXml = XML_JSON.Json2XML("{\"root\":{0}}".Replace("{0}", ret));
                XmlDocument tmp = new XmlDocument();
                tmp.LoadXml(strXml);
                ////doc = FillXml(tmp, doc, strRootName, strDefaultName);
                ////xmlschema = getSchema(doc.SelectSingleNode(strRootName), strDefaultName);

                ////doc = ClearSubNode(doc, strRootName, strDefaultName);
                
                if(!j2x.getDataSetXml(ret,ref doc,ref msg))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                return false;
            }
            return true;
        }

        string getRequestJson(JdUnion_Bussiness_List_Class jc,DataSet ds)
        {
            return jc.RequestDataSet(ds);
            return null;
        }

        public bool getJsonData(XmlNode config, ref string strJson, ref string msg)
        {
            msg = "方法未实现！";
            return false;
        }

        public bool getDataSet(XmlNode config, ref DataSet ds, ref string msg)
        {
            msg = "方法未实现！";
            return false;
        }

        public bool writeJsonData(XmlNode config, DataSet data, ref string strJson, ref string msg, string writetype = "Add")
        {
            msg = "方法未实现！";
            return false;
        }

        public bool writeDataSet(XmlNode config, DataSet data, ref DataSet ret, ref string msg, string writetype = "Add")
        {
            msg = "方法未实现！";
            return false;
        }
    }

   
}

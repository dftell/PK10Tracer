using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                XmlNode result = ret.FirstChild.SelectSingleNode("result");
                string res = "{\"root\":{0}}".Replace("{0}", result.InnerText.Trim());
                string strResult = XML_JSON.Json2XML(res);
                ret.LoadXml(strResult);
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

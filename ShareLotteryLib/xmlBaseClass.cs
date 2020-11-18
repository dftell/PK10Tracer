using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using XmlProcess;
using System.Xml.Linq;
namespace WolfInv.com.ShareLotteryLib
{
    public class xmlBaseClass
    {
        public static bool ReadData(Type t, object obj, Dictionary<string, string> list,string subKey="key",string subVal="value")
        {
            try
            {
                //Type t = this.GetType();
                FieldInfo[] fs = t.GetFields();

                for (int i = 0; i < fs.Length; i++)
                {
                    if (list.ContainsKey(fs[i].Name))
                    {
                        //ToLog(fs[i].Name, list[fs[i].Name]);
                        if ((fs[i].FieldType.IsSubclassOf(typeof(System.ValueType)) == false) && (fs[i].FieldType != typeof(string)))//如果是对象，并且不是字符串，要特别处理
                        {
                            object sobj = Activator.CreateInstance(fs[i].FieldType);
                            string strVal = list[fs[i].Name];
                            Dictionary<string, string> slist = new Dictionary<string, string>();
                            XmlDocument xmldoc = new XmlDocument();
                            try
                            {
                                xmldoc.LoadXml(strVal);
                                XmlNodeList nodes = xmldoc.SelectNodes("./item");

                                for (int d = 0; d < nodes.Count; d++)
                                {
                                    string key = null, val = null;
                                    XmlAttribute att = nodes[d].Attributes[subKey];
                                    if (att != null)
                                    {
                                        key = att.Value;
                                    }
                                    if (string.IsNullOrEmpty(key))
                                        continue;
                                    att = nodes[d].Attributes["value"];
                                    if (att != null)
                                        val = att.Value;
                                    if (slist.ContainsKey(key) == false)
                                    {
                                        slist.Add(key, val);
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }
                            ReadData(sobj.GetType(), sobj, slist);//再读取子节点进对象
                            try
                            {
                                fs[i].SetValue(obj, sobj);
                            }
                            catch
                            {

                            }
                            continue;
                        }
                        try
                        {
                            fs[i].SetValue(obj, Convert.ChangeType(list[fs[i].Name], fs[i].FieldType));
                        }
                        catch (Exception ce)
                        {
                           
                            //throw ce;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static List<T> ReadDatas<T>(List<string> list)
        {
            List<T> ret = new List<T>();
            try
            {
                //Type t = this.GetType();
                FieldInfo[] fs = typeof(T).GetFields();
                MethodInfo mi = typeof(xmlBaseClass).GetMethod("ReadDatas");
                
                XmlDocument tmp = new XmlDocument();
                foreach (string key in list)
                {
                    T obj = (T)Activator.CreateInstance(typeof(T));
                    for (int i = 0; i < fs.Length; i++)
                    {
                        try
                        {
                            tmp.LoadXml(key);
                            XmlNode root = tmp.DocumentElement;
                            List<object> sublist = new List<object>();
                            //只针对List对象类型
                            if ((fs[i].FieldType.IsSubclassOf(typeof(System.ValueType)) == false) && (fs[i].FieldType != typeof(string)))//如果是对象，并且不是字符串，要特别处理
                            {
                                Type baseType = fs[i].FieldType.GetGenericArguments()[0];
                                if (baseType == null)
                                    return ret;
                                object sobj = Activator.CreateInstance(baseType);
                                List<string> slist = ReadXml(root.SelectNodes(string.Format("./{0}", fs[i].Name)));
                                MethodInfo umi  = mi.MakeGenericMethod(new Type[] { baseType });
                                
                                object sres = umi.Invoke(null, new object[] { slist });
                                try
                                {
                                    fs[i].SetValue(obj,sres);
                                }
                                catch
                                {

                                }
                                continue;
                            }
                            string attName = string.Format("@{0}", fs[i].Name);
                            string val = XmlUtil.GetSubNodeText(root, attName);
                            try
                            {
                                object currval = fs[i].GetValue(obj);
                                if (!string.IsNullOrEmpty(val))
                                {
                                    fs[i].SetValue(obj, Convert.ChangeType(val, fs[i].FieldType));
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        catch(Exception ce)
                        {
                            continue;
                        }
                    }
                    ret.Add(obj);
                }
            }
            catch (Exception e)
            {
                return ret;
            }
            return ret;
        }

        public static Dictionary<string, string> ReadXml(XmlNodeList nodes, string key, bool isObject = false, string val = null)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach(XmlNode node in nodes)
            {
                string keyval = XmlUtil.GetSubNodeText(node, key);
                string valval = isObject ?  string.Format("{0}", node.OuterXml):XmlUtil.GetSubNodeText(node, val);
                ret.Add(keyval,valval);
            }
            return ret;
        }
        public static List<string> ReadXml(XmlNodeList nodes)
        {
            List<string> ret = new List<string>();
            foreach (XmlNode node in nodes)
            {
                string valval =  string.Format("{0}", node.OuterXml);
                ret.Add(valval);
            }
            return ret;
        }

    }

}

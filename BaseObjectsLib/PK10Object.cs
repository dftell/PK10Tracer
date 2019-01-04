using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using LogLib;
using System.ComponentModel;
namespace BaseObjectsLib
{
    /// <summary>
    /// 所有数据类的基类
    /// </summary>
    public class RecordObject : MarshalByRefObject  
    {

        
    }

    public interface iDetailListParamsable
    {
        string ToDetailString();
    }

    public interface iSerialJsonClass<T>
    {
         T getObjectByJsonString(string str);
    }
    
    [Serializable]
    public class InnerClass<T> : DetailStringClass
    {
        public List<T> list;
    }

    [Serializable]
    public class DetailStringClass:LogableClass,iDetailListParamsable,iLog
    {

        public DetailStringClass()
        {
            SetDefaultValueAttribute();
        }

        public static List<T> getObjectListByXml<T>(string strXml)
        {
            InnerClass<T> innerObj= new InnerClass<T>();

            List<T> ret = new List<T>();
            if (strXml == null || strXml.Trim().Length == 0) return ret;
            //(T)Convert.ChangeType(
            innerObj = (InnerClass<T>)GetObjectByXml(strXml,innerObj.GetType());
            if (ret == null)
                return null;
            ret = innerObj.list;
            ////}
            ////catch(Exception e)
            ////{
            ////    return null;
            ////}
            return ret;
        }

        public static string getXmlByObjectList<T>(List<T> list)
        {
            InnerClass<T> ic = new InnerClass<T>();
            ic.list = list;
            return ic.ToXml();
        }

        public string ToDetailString()
        {
            string ret = "";
            Type MyType = this.GetType();
            PropertyInfo[] pps = MyType.GetProperties();
            for (int i = 0; i < pps.Length; i++)
            {
                if (pps[i].PropertyType.IsValueType || pps[i].PropertyType == typeof(string))
                {
                    ret = string.Format("{0}{3}\"{1}\":\"{2}\"", ret, pps[i].Name, pps[i].GetValue(this, null),ret.Length==0?"":",");
                }
                else
                {
                    if (pps[i] is iDetailListParamsable)
                    {
                        ret = string.Format("{0}{3}\"{1}\":{2}", ret, pps[i].Name, "{" + (pps[i] as iDetailListParamsable).ToDetailString() + "}", ret.Length == 0 ? "" : ",");
                    }
                }
            }
            FieldInfo[] mems = MyType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < mems.Length; i++)
            {
                if (mems[i].FieldType.IsValueType||mems[i].FieldType ==typeof(string))
                {
                    ret = string.Format("{0}{3}\"{1}\":\"{2}\"", ret, mems[i].Name, mems[i].GetValue(this), ret.Length == 0 ? "" : ",");
                }
                else
                {
                    if (mems[i] is iDetailListParamsable)
                    {
                        ret = string.Format("{0}{3}\"{1}\":{2}", ret, mems[i].Name, "{" + (mems[i] as iDetailListParamsable).ToDetailString() + "}", ret.Length == 0 ? "" : ",");
                    }
                }
            }
            return "{"+ret+"}";
        }

        public override string ToString()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(this);
            //return ToDetailString();
        }

        public string ToXml()
        {
            return XmlHelper.XmlSerialize(this, Encoding.UTF8);
        }

        public static T GetObjectByXml<T>(string str)
        {
            return XmlHelper.XmlDeserialize<T>(str, Encoding.UTF8);
        }

        public static object GetObjectByXml(string str, Type objtpye)
        {
            return XmlHelper.XmlDeserialize(str, Encoding.UTF8, objtpye);
        }

        protected void SetDefaultValueAttribute()
        {
            PropertyInfo[] pps = this.GetType().GetProperties();
            for (int i = 0; i < pps.Length; i++)
            {
                PropertyInfo pi = pps[i];
                //Attribute[] atts = Attribute.GetCustomAttributes(
                DefaultValueAttribute att = Attribute.GetCustomAttribute(pi as MemberInfo, typeof(DefaultValueAttribute), true) as DefaultValueAttribute;
                if (att != null)
                {
                    if (pi.CanWrite)
                    {
                        object val = ConvertionExtensions.ConvertTo((IConvertible)att.Value, pi.PropertyType);
                        pi.SetValue(this, val, null);
                    }
                }
            }
        }
    }
    
    public static class XmlHelper
    {
        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = "    ";

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
                writer.Close();
            }
        }

        /// <summary>
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>序列化产生的XML字符串</returns>
        public static string XmlSerialize(object o, Encoding encoding)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, o, encoding);

                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public static void XmlSerializeToFile(object o, string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializeInternal(file, o, encoding);
            }
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string s, Encoding encoding)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return (T)mySerializer.Deserialize(sr);
                }
            }
        }

        public static object XmlDeserialize(string s, Encoding encoding,Type tp)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer mySerializer = new XmlSerializer(tp);
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return mySerializer.Deserialize(sr);
                }
            }
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            string xml = File.ReadAllText(path, encoding);
            return XmlDeserialize<T>(xml, encoding);
        }

       
    
        
    }

    public interface iDbFile
    {
        bool AllowMutliSelect { get; }
        string strKey { get; }
        string strDbXml { get; }
        List<T> getAllListFromDb<T>();
        bool SaveDBFile<T>(List<T> list);
        string strKeyValue();
        string strObjectName { get; }
    }
}

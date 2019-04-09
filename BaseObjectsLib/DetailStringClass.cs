using System;
using System.Collections.Generic;
using System.Text;
using WolfInv.com.LogLib;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.ComponentModel;

namespace WolfInv.com.BaseObjectsLib
{

    public interface iDetailListParamsable
    {
        string ToDetailString();
    }

    [Serializable]
    public class InnerClass<T> : DetailStringClass
    {
        public List<T> list;
    }

    [Serializable]
    public class DetailStringClass : LogableClass, iDetailListParamsable, iLog
    {

        public DetailStringClass()
        {
            SetDefaultValueAttribute();
        }

        public static List<T> getObjectListByXml<T>(string strXml)
        {
            InnerClass<T> innerObj = new InnerClass<T>();

            List<T> ret = new List<T>();
            if (strXml == null || strXml.Trim().Length == 0) return ret;
            //(T)Convert.ChangeType(
            innerObj = (InnerClass<T>)GetObjectByXml(strXml, innerObj.GetType());
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
            if (ic.list.Count == 0)
                return "";
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
                    ret = string.Format("{0}{3}\"{1}\":\"{2}\"", ret, pps[i].Name, pps[i].GetValue(this, null), ret.Length == 0 ? "" : ",");
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
                if (mems[i].FieldType.IsValueType || mems[i].FieldType == typeof(string))
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
            return "{" + ret + "}";
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

        public object getValue(string strFieldName)
        {
            Type t = this.GetType();
            FieldInfo fi = t.GetField(strFieldName);
            if (fi == null)
            {
                PropertyInfo pi = t.GetProperty(strFieldName);
                if(pi == null)
                {
                    return null;
                }
                return pi.GetValue(this);
            }
            return fi.GetValue(this);
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

        public T CopyTo<T>()
        {
            return ConvertionExtensions.CopyTo<T>(this);
        }
    }

    

}

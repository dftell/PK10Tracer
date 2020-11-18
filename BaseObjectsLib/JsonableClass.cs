using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
namespace WolfInv.com.BaseObjectsLib
{
    [AttributeUsage(AttributeTargets.All)]
    public class ChangeTypeAttribute : Attribute
    {
        private Type sourceType;
        private Type targetType;
        public ChangeTypeAttribute(Type sourceType, Type targetType)
        {
            this.sourceType = sourceType;
            this.targetType = targetType;
        }

        public virtual Type SourceType
        {
            get { return sourceType; }
        }

        public virtual Type TargetType
        {
            get { return targetType; }
        }
    }
    public class JsonableClass<T>
    {
        public T FromJson(string strjson)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Deserialize<T>(strjson);
            }
            catch(Exception ce)
            {
                return default(T);
            }
        }
        public static T1 FromJson<T1>(string strjson)
        {
            return FromJson<T1,string,string>(strjson);
        }
        public static T1 FromJson<T1, FromT, ToT>(string strjson, Converter<FromT, ToT> converter = null,int ArrLevel=1)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Deserialize<T1>(strjson);
            }
            catch (Exception ce)
            {
                try
                {
                    return TranferObject<T1, FromT, ToT>(strjson,converter,ArrLevel);
                }
                catch (Exception ne)
                {
                    return default(T1);
                }
            }
        }
        #region 补充
        
        public static string TranferJson<T>(T obj, string DateTimeFormats = "yyyy-MM-dd HH:mm:ss")
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = DateTimeFormats };
            string postData = JsonConvert.SerializeObject(obj, Formatting.Indented, timeConverter);
            //获取ChangeType特性，改变json内容
            PropertyInfo[] propertys = typeof(T).GetProperties();
            if (propertys != null && propertys.Length > 0)
            {
                foreach (PropertyInfo p in propertys)
                {
                    object[] objAttrs = p.GetCustomAttributes(typeof(ChangeTypeAttribute), true);//获取自定义特性  
                    if (objAttrs != null && objAttrs.Length > 0)
                    {
                        ChangeTypeAttribute attr = objAttrs[0] as ChangeTypeAttribute;
                        postData = DealObjectToJson(attr.SourceType, attr.TargetType, p.Name, postData);
                    }
                };
            }
            return postData;
        }
        
        /// <summary>
        /// 对象转成json时根据 源类型 和 目标类型 找到节点转换
        /// </summary>
        /// <param name="sourceType">实体属性类型</param>
        /// <param name="targetType">JSON节点类型</param>
        /// <param name="nodeName">节点</param>
        /// <param name="json">源JSON</param>
        /// <returns></returns>
        private static string DealObjectToJson(Type sourceType, Type targetType, string nodeName, string json)
        {
            if (sourceType == typeof(string) && targetType == typeof(int[]))
            {
                string content = JObject.Parse(json).SelectToken(nodeName).ToString();
                string[] tmp = content.Split(',');
                int[] iNums = Array.ConvertAll(tmp, int.Parse);
                JArray array = JArray.FromObject(iNums);
                JObject tokenselect = JObject.Parse(json);
                tokenselect[nodeName] = array;
                json = tokenselect.ToString();
            }
            else
            {
                ;
            }
            return json;
        }
        
        public static T TranferObject<T, FromT, ToT>(string json, Converter<FromT, ToT> converter,int ArrLevel=1)
        {
            return AnalyzeResult<T,FromT,ToT>(json, converter,ArrLevel);
        }

        private static T AnalyzeResult<T, FromT, ToT>(string resultJson,Converter<FromT,ToT> converter,int ArrLevel=1)
        {
            T result = default(T);
            if (string.IsNullOrEmpty(resultJson) == false)
            {
                //先抓换json内容，再转换成类
                string json = resultJson;
                PropertyInfo[] propertys = typeof(T).GetProperties();
                if (propertys != null && propertys.Length > 0)
                {
                    foreach (PropertyInfo p in propertys)
                    {
                        object[] objAttrs = p.GetCustomAttributes(typeof(ChangeTypeAttribute), true);
                        if (objAttrs != null && objAttrs.Length > 0)
                        {
                            ChangeTypeAttribute attr = objAttrs[0] as ChangeTypeAttribute;
                            json = DealJsonToObject<FromT,ToT > (attr.SourceType, attr.TargetType, p.Name, json,converter,ArrLevel);
                        }
                    };
                }

                result = JsonConvert.DeserializeObject<T>(json);
            }
            return result;
        }

        /// <summary>
        /// json转换成对象时根据 源类型和目标类型 将json对应的节点值转换成源类型值
        /// </summary>
        /// <param name="sourceType">实体属性类型</param>
        /// <param name="targetType">JSON节点类型</param>
        /// <param name="nodeName">节点</param>
        /// <param name="json">源JSON</param>
        /// <returns></returns>
        private static string DealJsonToObject<FromT,ToT>(Type sourceType, Type targetType, string nodeName, string json, Converter<FromT,ToT> convFunc,int ArrLevel = 1)
        {
            //if (sourceType == typeof(string) && targetType == typeof(int[]))
            //{
            JToken content = JObject.Parse(json).SelectToken(nodeName);
            List<string> allElements = new List<string>();
            JArray array = new JArray();
            //JObject tokenselect = JObject.Parse(json);
            //tokenselect[nodeName] = array;
            //json = tokenselect.ToString();
            foreach (var sub in content.Children<JArray>())
            {
                object val = null;
                try
                {
                    val = sub.ToObject(sourceType);
                    //allElements.Add()
                    array.Add(sub);
                }
                catch
                { }
            }
            foreach (var sub in content.Children<JValue>())
            {
                ToT val = default(ToT);
                try
                {
                    val = (ToT)sub.ToObject(targetType);
                    List<object> vals = getList(ArrLevel, val);
                    //ToT[] arr = Array.ConvertAll<FromT,ToT>(vals, convFunc);
                    JArray tarr = JArray.FromObject(vals);
                    array.Add(tarr);
                }
                catch(Exception ce)
                {

                }
            }

            string value = null;// string.Join(",", obj.ToArray());
            JObject tokenselect = JObject.Parse(json);
            tokenselect[nodeName] = array;
            json = tokenselect.ToString();
            //}
            return json;
        }

        static List<object> getList(int n,object val)
        {
            if(n>1)
            {
                List<object> ret = new List<object>();
                ret.Add(getList(n - 1, val));
                return ret;
            }
            List<object> res = new List<object>();
            res.Add(val);
            return res;
        }
        /**/
        #endregion
        public string ToJson()
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(this);
            }
            catch
            {
                return null;
            }
        }
    }
}

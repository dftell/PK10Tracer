using System;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using System.Linq;
namespace WolfInv.com.BaseObjectsLib
{
    public static class ConvertionExtensions
    {
        public static T ConvertTo<T>(this IConvertible convertibleValue)
        {
            if(convertibleValue == null)
            {
                return default(T);
            }
            if (string.IsNullOrEmpty(convertibleValue.ToString()))
            {
                return default(T);
            }
            if (!typeof(T).IsGenericType)
            {
                return (T)Convert.ChangeType(convertibleValue, typeof(T));
            }
            else
            {
                Type genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return (T)Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(typeof(T)));
                }
            }
            throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".", convertibleValue.GetType().FullName, typeof(T).FullName));
        }

        public static object ConvertTo(this IConvertible convertibleValue,Type T)
        {
            if(convertibleValue == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(convertibleValue.ToString()))
            {
                return null;
            }
            if (!T.IsGenericType)
            {
                return Convert.ChangeType(convertibleValue, T);
            }
            else
            {
                Type genericTypeDefinition = T.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(T));
                }
            }
            throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".", convertibleValue.GetType().FullName, T.FullName));
        }

    
        public static TField GetValue<TDocument,TField>(TDocument tdoc,string keyname)
        {
            TField ret = default(TField);
            if (tdoc == null)
                return ret;
            Type t = tdoc.GetType();
            MemberInfo mi = t.GetProperty(keyname);
            if(mi != null)
            {
                object val = (mi as PropertyInfo).GetValue(tdoc);
                if (val == null)
                    return ret;
                ret = ConvertionExtensions.ConvertTo<TField>(val as IConvertible);
                return ret;
            }
            else
            {
                mi = t.GetField(keyname);
                if (mi == null)
                    return ret;
                object val = (mi as FieldInfo).GetValue(tdoc);
                if (val == null)
                    return ret;
                ret = ConvertionExtensions.ConvertTo<TField>(val as IConvertible);
                return ret;
            }
        }

        public static TField GetValue<TDocument,TSubDocument, TField>(TDocument Ptdoc, Func<TDocument, TSubDocument> keynameFunc,string keyname)
        {
            TField ret = default(TField);
            if (Ptdoc == null)
                return ret;
            TSubDocument tdoc = keynameFunc(Ptdoc);
            if (tdoc == null)
                return ret;
            Type t = tdoc.GetType();
            MemberInfo mi = t.GetProperty(keyname);
            if (mi != null)
            {
                object val = (mi as PropertyInfo).GetValue(tdoc);
                if (val == null)
                    return ret;
                ret = ConvertionExtensions.ConvertTo<TField>(val as IConvertible);
                return ret;
            }
            else
            {
                mi = t.GetField(keyname);
                if (mi == null)
                    return ret;
                object val = (mi as FieldInfo).GetValue(tdoc);
                if (val == null)
                    return ret;
                ret = ConvertionExtensions.ConvertTo<TField>(val as IConvertible);
                return ret;
            }
        }

        public static TField GetValue<TDocument,  TField>(Func<TDocument> keynameFunc, string keyname)
        {
            TDocument Ptdoc = keynameFunc();
            TField ret = default(TField);
            if (Ptdoc == null)
                return ret;
            TDocument tdoc = Ptdoc;// keynameFunc(Ptdoc);
            if (tdoc == null)
                return ret;
            Type t = tdoc.GetType();
            MemberInfo mi = t.GetProperty(keyname);
            if (mi != null)
            {
                object val = (mi as PropertyInfo).GetValue(tdoc);
                if (val == null)
                    return ret;
                ret = ConvertionExtensions.ConvertTo<TField>(val as IConvertible);
                return ret;
            }
            else
            {
                mi = t.GetField(keyname);
                if (mi == null)
                    return ret;
                object val = (mi as FieldInfo).GetValue(tdoc);
                if (val == null)
                    return ret;
                ret = ConvertionExtensions.ConvertTo<TField>(val as IConvertible);
                return ret;
            }
        }

        public static object GetValue(object tdoc, string keyname)
        {
            object ret = null;
            if (tdoc == null)
                return ret;
            Type t = tdoc.GetType();
            MemberInfo mi = t.GetProperty(keyname);
            if (mi != null)
            {
                object val = (mi as PropertyInfo).GetValue(tdoc);
                return val;
            }
            else
            {
                mi = t.GetField(keyname);
                if (mi == null)
                    return null;
                object val = (mi as FieldInfo).GetValue(tdoc);
                return val;
            }
        }

        public static TField GetValue<TField>(object tdoc, string keyname)
        {
            TField ret = default(TField);
            if (tdoc == null)
                return ret;
            Type t = tdoc.GetType();
            MemberInfo mi = t.GetProperty(keyname);
            if (mi != null)
            {
                object val = (mi as PropertyInfo).GetValue(tdoc);
                if (val == null)
                    return ret;
                ret = ConvertionExtensions.ConvertTo<TField>(val as IConvertible);
                return ret;
            }
            else
            {
                mi = t.GetField(keyname);
                object val = (mi as FieldInfo).GetValue(tdoc);
                if (val == null)
                    return ret;
                ret = ConvertionExtensions.ConvertTo<TField>(val as IConvertible);
                return ret;
            }
        }
        public static bool SetValue<TDocument, TField>(TDocument tdoc, string keyname,TField Newval)
        {
            try
            {
                if (tdoc == null)
                    return false;
                Type t = tdoc.GetType();
                object val = ConvertionExtensions.ConvertTo<TField>(Newval as IConvertible);
                MemberInfo mi = t.GetProperty(keyname);
                if (mi != null)
                {

                    (mi as PropertyInfo).SetValue(tdoc, val);
                    return true;
                }
                else
                {
                    mi = t.GetField(keyname);
                    (mi as FieldInfo).SetValue(tdoc, val);
                    return true;
                }
            }
            catch(Exception ce)
            {
                LogLib.LogableClass.ToLog("ConvertionExtensions填充数据错误！", ce.Message);
            }
            return false;
        }

        public static bool SetValue<TDocument,TSubDocument, TField>(TDocument Ptdoc,Func<TDocument,TSubDocument> func, string keyname, TField Newval)
        {
            try
            {
                if(Ptdoc == null)
                {
                    return false;
                }
                TSubDocument tdoc = func(Ptdoc);
                if (tdoc == null)
                {
                    tdoc = CreateInstance<TSubDocument>();
                }
                Type t = tdoc.GetType();
                object val = ConvertionExtensions.ConvertTo<TField>(Newval as IConvertible);
                MemberInfo mi = t.GetProperty(keyname);
                if (mi != null)
                {

                    (mi as PropertyInfo).SetValue(tdoc, val);
                    return true;
                }
                else
                {
                    mi = t.GetField(keyname);
                    (mi as FieldInfo).SetValue(tdoc, val);
                    return true;
                }
            }
            catch (Exception ce)
            {
                LogLib.LogableClass.ToLog("ConvertionExtensions填充数据错误！", ce.Message);
            }
            return false;
        }

        public static bool Equal(object tdoc, string keyname,object val)
        {
            object objval = GetValue(tdoc, keyname);
            
            if(objval == null)//不管是不是真的为空，先干掉
            {
                return false;
            }
            if(objval.Equals(val) || objval == val)
            {
                LogLib.LogableClass.ToLog("判定", string.Format("{3}>>>>{0}=><{4}>.{1}==<{5}>.{2}", keyname, objval, val,true, objval?.GetType(),val?.GetType()));
                return true;
            }
            LogLib.LogableClass.ToLog("判定", string.Format("{3}>>>>{0}=><{4}>.{1}==<{5}>.{2}", keyname, objval, val, false, objval?.GetType(), val?.GetType()));
            return false;
        }

        public static bool Equal(object tdoc, string keyname, Func<object,object> act, object val)
        {
            object objval = GetValue(tdoc, keyname);

            if (objval == null)//不管是不是真的为空，先干掉
            {
                return false;
            }
            if (act(objval).Equals(val) )
            {
                //LogLib.LogableClass.ToLog("判定", string.Format("{3}>>>>{0}=><{4}>.{1}==<{5}>.{2}", keyname, objval, val, true, objval?.GetType(), val?.GetType()));
                return true;
            }
            //LogLib.LogableClass.ToLog("判定", string.Format("{3}>>>>{0}=><{4}>.{1}==<{5}>.{2}", keyname, objval, val, false, objval?.GetType(), val?.GetType()));
            return false;
        }
        public static T CreateInstance<T>()
        {
            Type t = typeof(T);
            T obj = Activator.CreateInstance<T>();
            return obj;
        }

        public static object CreateInstance(Type t)
        {
            //Type t = typeof(T);
            object obj = Activator.CreateInstance(t);
            return obj;
        }

        public static Dictionary<string, Type> GetAllProperties<T>(bool includeParent = false)
        {
            Dictionary<string, Type> ret = new Dictionary<string, Type>();
            Type t = typeof(T);
            PropertyInfo[] pis = null;
            if (includeParent)
                pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            else
                pis = t.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                ret.Add(pi.Name, pi.PropertyType);
            }
            return ret;
        }

        public static Dictionary<string, Type> GetAllProperties(Type T, bool includeParent = false)
        {
            Dictionary<string, Type> ret = new Dictionary<string, Type>();
            Type t = T;
            PropertyInfo[] pis = null;
            if (includeParent)
                pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public );
            else
                pis = t.GetProperties();
            
            foreach (PropertyInfo pi in pis)
            {
                if(!ret.ContainsKey(pi.Name))
                    ret.Add(pi.Name, pi.PropertyType);
            }
            return ret;
        }

        public static T Clone<T>(T obj)
        {
            if (typeof(T).IsGenericType)
            {
                return obj;
            }
            T ret = CreateInstance<T>();
            Type t = typeof(T);
            PropertyInfo[] pis = t.GetProperties(BindingFlags.Instance| BindingFlags.Public);
            foreach (PropertyInfo pi in pis)
            {
                try
                {
                    pi.SetValue(ret, GetValue(obj, pi.Name));
                }
                catch(Exception e)
                {

                }
            }
            return ret;
        }
        
        public static object Clone(object obj)
        {
            if (obj == null) return obj;
            Type t = obj.GetType();
            if (t.IsGenericType)
            {
                return obj;
            }
            object ret = null;
            try
            {
                ret = Activator.CreateInstance(t);
            }
            catch //如果无法实例化对象，就返回空，退出来
            {
                return null;
            }
            PropertyInfo[] pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty );
            foreach (PropertyInfo pi in pis)
            {
                try
                {
                    pi.SetValue(ret, GetValue(obj, pi.Name));
                }
                catch
                {

                }
            }
            return ret;
        }

        public static T CopyTo<T>(object obj)
        {
            if (obj == null)
                return default(T);
            Type t = typeof(T);
            Type tCopy = obj.GetType();
            T ret = CreateInstance<T>();
            Dictionary<string, Type> retPips = GetAllProperties(t);
            Dictionary<string, Type> copyPips = GetAllProperties(tCopy);
            foreach (string key in retPips.Keys)
            {
                if(!copyPips.ContainsKey(key))
                {
                    continue;
                }
                if (!retPips[key].Equals(copyPips[key]))
                {
                    continue;
                }
                try
                {
                    object val = tCopy.GetProperty(key).GetValue(obj);
                    t.GetProperty(key).SetValue(ret, val);
                }
                catch(Exception ce)
                {

                }
            }
            return ret;
        }

        public static bool FillTo(object fromobj,ref object ret,bool includeParent=false)
        {
            if (fromobj == null || ret == null)
                return false;
            Type t = ret.GetType();
            Type tCopy = fromobj.GetType();
            //object ret = obj;
            Dictionary<string, Type> retPips = GetAllProperties(t, includeParent);
            Dictionary<string, Type> copyPips = GetAllProperties(tCopy, includeParent);
            foreach (string key in retPips.Keys)
            {
                if (!copyPips.ContainsKey(key))
                {
                    continue;
                }
                if (!retPips[key].Equals(copyPips[key]))
                {
                    continue;
                }
                try
                {
                    object val = tCopy.GetProperty(key).GetValue(fromobj);
                    t.GetProperty(key).SetValue(ret, val);
                }
                catch (Exception ce)
                {
                    //obj = ret;//有多少返回多少
                    //return false;
                }
            }
            //obj = ret;
            return true ;
        }
    }

    public delegate T[] paramObjs<T>(T[] data, params object[] args);
    public static class Extends
    {
        public static T[] fullFuncs<T>(paramObjs<T> func, T[] data, params object[] ars)
        {
            return func(data, ars);
        }
        public static T[] Get<T>(this T[] data, int N, paramObjs<T> func)
        {
            return func(data, N);
        }

        public static BaseDataTable NDay<T>(this ExpectList<T> data, int N, params object[] objs) where T:TimeSerialData
        {
            ExpectList<T> useData = data.LastDatas(data.LastData.Expect,N,true);
            BaseDataTable ret = new BaseDataTable();
            List<string> cols = new List<string>();
            List<T[]> funcs = new List<T[]>();
            int colcnt = 0;
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is string)
                {
                    cols.Add((string)objs[i]);
                    colcnt++;
                }
                else
                {
                    if (i >= 2 * colcnt)
                    {
                        break;
                    }
                    funcs.Add(objs[i] as T[]);
                }
            }
            List<Single[]> vals = new List<float[]>();
            for (int i = 0; i < cols.Count; i++)
            {
                ret.GetTable().Columns.Add(cols[i]);

            }

            for (int i = 0; i < useData.Count; i++)
            {
                DataRow dr = ret.GetTable().NewRow();
                for (int c = 0; c < cols.Count; c++)
                {
                    dr[c] = funcs[c][i];
                }
                ret.GetTable().Rows.Add(dr);
            }
            return ret;
        }

    }

    public static class WDEquitCodeExtenel
    {
        public static string WDCode(this string code)
        {
            string ret = code;
            if (code.Split('.').Length > 1)
                return ret;
            string flg = "SZ";
            if (code.StartsWith("6"))
            {
                flg = "SH";
            }
            return string.Format("{0}.{1}", code, flg);
        }

        public static string WDDate(this string datetime, string srcFormat = "yyyy-MM-dd", string toFormat = "yyyy-MM-dd")
        {
            DateTime dt;
            bool succ = DateTime.TryParseExact(datetime, srcFormat, null, System.Globalization.DateTimeStyles.None, out dt);
            if (succ)
                return dt.ToString(toFormat);
            return datetime;
        }

        public static string WDDate(this DateTime datetime, string format = "yyyy-MM-dd")
        {
            return datetime.ToString(format);
        }

        public static string ToYearStart(this string datetime, string format = "yyyy-MM-dd")
        {
            DateTime dt = datetime.ToDate(format);
            dt = dt.AddMonths(-dt.Month + 1);
            dt = dt.AddDays(-dt.Day + 1);
            return dt.WDDate(format);
        }

        public static DateTime ToDate(this string datetime, string format = "yyyy-MM-dd")
        {

            DateTime dt;
            bool succ = DateTime.TryParse(datetime, out dt);//只要是日期，不管是什么格式，都返回正常的
            if (succ)
                return dt;
            succ = DateTime.TryParseExact(datetime, format, null, System.Globalization.DateTimeStyles.None, out dt);
            if (succ)
                return dt;
            return dt;
        }

        public static int IndexOf(this string[] arr, string date, string format = "yyyy-MM-dd")
        {
            if (arr == null||arr.Length == 0)
                return -1;
            if (string.IsNullOrEmpty(date))
                return -1;
            DateTime[] dArr = new DateTime[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                dArr[i] = arr[i].ToDate(format);
            return dArr.IndexOf(date);
        }
        public static int IndexOf(this DateTime[] arr,string date)
        {
            List<DateTime> useArr = arr.ToList().OrderBy(a => a).ToList();
            int ret = useArr.IndexOf(date.ToDate());
            if(ret >=0)
            {
                return ret;
            }
            if (date.ToDate() > useArr.Last() || date.ToDate() < useArr.First())
            {
                return -1;
            }
            int gtcnt = useArr.Where(a => a > date.ToDate()).Count();
            return useArr.Count - gtcnt;
        }

        public static long ToLong(this string datetime,string toFormat = "yyyyMMddHHmmss",string fromFormat ="yyyy-MM-dd")
        {
            DateTime dt = ToDate(datetime,fromFormat);
            long ret = 0;
            long.TryParse(dt.WDDate(toFormat), out ret);
            return ret;
        }
    }


}

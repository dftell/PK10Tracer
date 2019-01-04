using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Data;
namespace BaseObjectsLib
{
    public class DisplayAsTableClass : DetailStringClass
    {
        Dictionary<string, List<MemberInfo>> TableStructure = new Dictionary<string, List<MemberInfo>>();

        public static DataTable ToTable<T>(List<T> list, bool UseDisplayNameAsColumnName, bool OnlyDisplayDefName)
        {
            Dictionary<string, MemberInfo> dic = null;
            return ToTable<T>(list, UseDisplayNameAsColumnName, OnlyDisplayDefName, ref dic);
        }

        public static DataTable ToTable<T>(List<T> list, bool OnlyDisplayDefName)
        {
            Dictionary<string, MemberInfo> dic = null;
            return ToTable<T>(list, false, OnlyDisplayDefName, ref dic);
        }

        public static DataTable ToTable<T>(List<T> list)
        {
            return ToTable<T>(list, false);
        }

        public static DataTable ToTable<T>(List<T> list, bool UseDisplayNameAsColumnName, bool OnlyDisplayDefName, ref Dictionary<string, MemberInfo> DisIs)
        {
            DataTable ret = new DataTable();
            try
            {
                MemberInfo[] pis = typeof(T).GetMembers();
                //List<string> Titles;
                if (DisIs == null)
                {
                    DisIs = new Dictionary<string, MemberInfo>();
                    for (int i = 0; i < pis.Length; i++)
                    {
                        if (pis[i].MemberType != MemberTypes.Property && pis[i].MemberType != MemberTypes.Field)
                        {
                            continue;
                        }
                        Type memtype = (pis[i].MemberType == MemberTypes.Property) ? (pis[i] as PropertyInfo).PropertyType : (pis[i] as FieldInfo).FieldType;
                        ////if (pis[i].GetType().IsClass)//如果是对象，不载入
                        ////    continue;
                        //if (pis[i] is PropertyInfo)
                        //{
                        if ((memtype.IsClass && memtype != typeof(string)))//类对象跳过
                        {
                            continue;
                        }
                        //}
                        DisplayNameAttribute DNA = Attribute.GetCustomAttribute(pis[i], typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                        string strTitle = pis[i].Name;
                        if (DNA != null && UseDisplayNameAsColumnName)
                        {
                            strTitle = DNA.DisplayName;
                        }
                        if (OnlyDisplayDefName && DNA == null)
                        {
                            continue;
                        }
                        if (DisIs.ContainsKey(strTitle))
                        {
                            ToLog("错误", "转换为表", string.Format("存在相同的关键字.{0}", strTitle));
                            continue;
                        }
                        DisIs.Add(strTitle, pis[i]);
                    }
                }
                foreach (string key in DisIs.Keys)
                {
                    Type memtype = (DisIs[key].MemberType == MemberTypes.Property) ? (DisIs[key] as PropertyInfo).PropertyType : (DisIs[key] as FieldInfo).FieldType;
                    Type genericTypeDefinition = memtype;
                    if (genericTypeDefinition.IsGenericType)
                    {
                        if (genericTypeDefinition.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            genericTypeDefinition = genericTypeDefinition.GetGenericArguments()[0];
                        }
                    }
                    DataColumn dc = new DataColumn(key, genericTypeDefinition);
                    ret.Columns.Add(dc);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    DataRow dr = ret.NewRow();
                    foreach (string key in DisIs.Keys)
                    {
                        T obj = (T)list[i];
                        Type memtype = (DisIs[key].MemberType == MemberTypes.Property) ? (DisIs[key] as PropertyInfo).PropertyType : (DisIs[key] as FieldInfo).FieldType;
                        Type genericTypeDefinition = memtype;
                        if (genericTypeDefinition.IsGenericType)
                        {
                            if (genericTypeDefinition.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                genericTypeDefinition = genericTypeDefinition.GetGenericArguments()[0];
                            }
                        }
                        object val = DisIs[key].MemberType == MemberTypes.Property ? (DisIs[key] as PropertyInfo).GetValue(obj, null) : (DisIs[key] as FieldInfo).GetValue(obj);
                        if (val == null) continue;
                        val = ConvertionExtensions.ConvertTo((IConvertible)val, genericTypeDefinition);
                        if (DisIs[key] is PropertyInfo)
                        {
                            dr[key] = val;
                        }
                        else
                        {
                            dr[key] = val;
                        }
                    }
                    ret.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                ToLog("错误", "将对象列表转换为数据表错误！", string.Format("{0}:{1}",e.Message,e.StackTrace));
            }
            return ret;
        }

        public List<T> FillByTable<T>(DataTable dt)
        {
            List<MemberInfo> ret = null;
            return FillByTable<T>(dt, ref ret);
        }

        public List<T> FillByTable<T>(DataTable dt,ref List<MemberInfo> TableBuffs)
        {
            
            List<T> ret = new List<T>();
            Type t = typeof(T);
            if (TableStructure != null && TableStructure.ContainsKey(typeof(T).ToString()))
            {
                TableBuffs = TableStructure[typeof(T).ToString()];
            }
            if (dt == null || dt.Columns.Count == 0)
            {
                Log("错误","数据源错误", "为空/或者列为空");
            }
            if (TableBuffs == null)
            {
                TableBuffs = new List<MemberInfo>();
                MemberInfo[] mis = t.GetMembers();
                for (int i = 0; i < mis.Length; i++)
                {
                    if (dt.Columns.Contains(mis[i].Name))
                    {
                        if (!TableBuffs.Contains(mis[i]))
                        {
                            TableBuffs.Add(mis[i]);
                        }
                        else
                        {
                            Log("错误","表结构", string.Format("存在相同的列{0}.", mis[i].Name));
                        }
                    }
                }
            }
            //Log("输入表结构列表数量", TableBuffs.Count.ToString());
            if (TableStructure != null && TableBuffs != null)
            {
                if(!TableStructure.ContainsKey(typeof(T).ToString()))
                {
                    TableStructure.Add(typeof(T).ToString(), TableBuffs);
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                T cc = (T)Activator.CreateInstance(typeof(T));
                DataRow dr = dt.Rows[i];
                for (int j = 0; j < TableBuffs.Count; j++)
                {
                    

                   //return Convert.ChangeType(value, t);

                    MemberInfo mi = TableBuffs[j];
                    Type ty = (mi is PropertyInfo) ? (mi as PropertyInfo).PropertyType : (mi as FieldInfo).FieldType;
                    object val = dr[mi.Name];
                    if (val == null) continue;
                    val = ConvertionExtensions.ConvertTo((IConvertible)val,ty);
                    if (mi is PropertyInfo)
                    {
                        (mi as PropertyInfo).SetValue(cc, val, null);
                    }
                    if (mi is FieldInfo)
                    {
                        (mi as FieldInfo).SetValue(cc,val);
                    }
                }
                ret.Add(cc);
            }
            return ret;
        }

        
    }

    public static class ConvertionExtensions
    {
        public static T ConvertTo<T>(this IConvertible convertibleValue)
        {
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
    }


}

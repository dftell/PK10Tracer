using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using WolfInv.com.LogLib;
using System.ComponentModel;
using System.Data;
namespace WolfInv.com.BaseObjectsLib
{
    

    

    public interface iSerialJsonClass<T>
    {
         T getObjectByJsonString(string str);
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

    public class ClassOperateTool
    {
        public static List<Type> getAllSubClass(Type ParentClass)
        {
            List<Type> types = new List<Type>();
            Assembly ass = ParentClass.Assembly;
            Assembly[] assArr = new Assembly[1] { ass };
            Type[] AllType = ass.GetTypes();
            for (int i = 0; i < AllType.Length; i++)
            {
                Type t = AllType[i];
                while (t.BaseType != null)//寻找基类是stragclass的所有非抽象类
                {
                    if (t.BaseType.Equals(ParentClass))
                    {
                        if (!AllType[i].IsAbstract)
                        {
                            types.Add(AllType[i]);
                            break;
                        }
                    }
                    t = t.BaseType;
                }
            }
            return types;
        }

        public static DataTable getAllSubClass(Type ParentClass,string TextName,string ValueName)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(TextName);
            dt.Columns.Add(ValueName,typeof(Type));
            List<Type> res = getAllSubClass(ParentClass);
            for(int i=0;i<res.Count;i++)
            {
                DataRow dr = dt.NewRow();
                dr[TextName] = res[i].Name;
                dr[ValueName] = res[i];
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static object getInstanceByType(Type className)
        {
            Assembly asmb = className.Assembly;// Assembly.LoadFrom("EnterpriseServerBase.dll");
            Type sct = className;
            object sc = Activator.CreateInstance(sct);
            return sc;
        }
    }
}

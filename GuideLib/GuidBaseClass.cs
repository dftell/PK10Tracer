using System;
using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
using System.Reflection;
using System.Linq;
using Microsoft.VisualBasic;
namespace WolfInv.com.GuideLib
{
    /// <summary>
    /// 技术指标基类
    /// </summary>
    public abstract class GuidBaseClass
    {
        public HashSet<string> Fields;
        protected string strParamStyle;
        public DateTime tradeDate;
        string _strFiled;
        public String GuidName
        {
            get
            {
                if (Fields == null)
                    return _strFiled;
                return _strFiled ?? string.Join(",", Fields);
            }
            set
            {
                _strFiled = value;
            }
        }
        public Cycle cycle;
        public PriceAdj priceAdj;
        protected string _strParam;
        public string strParam{get{return getParamString();}}
        public abstract string getParamString();
        public GuidBaseClass()
        {
        }

        public static GuidBaseClass CreateGuideInstance(string GuideName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string ClassName = string.Format("{0}GuidClass", GuideName);
            var val = from t in assembly.GetTypes()
                      where Strings.Right(t.Name, ClassName.Length) == ClassName
                      select t;
            Type ct = null;
            if (val.Count<Type>() == 1)
            {
                ct = val.First<Type>();
            }
            return Activator.CreateInstance(ct) as GuidBaseClass;
        }
    }

}


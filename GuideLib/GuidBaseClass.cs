using System;
using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
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
    }

}


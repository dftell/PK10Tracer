//using CFZQ_JRGCDB;
using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
namespace WolfInv.com.GuideLib
{
    /// <summary>
    /// 运行结果类
    /// </summary>
    public class RunResultClass
    {
        public RunNoticeClass Notice;
        public MTable Table;
        public List<SelectResult> Result;
        public RunResultClass()
        {
            Notice = new RunNoticeClass();
            Result = new List<SelectResult>();
        }
    }

    public class SelectResult
    {
        public bool Enable;
        public string Key;
        public double Weight;
        public string Status;
        /// <summary>
        /// 参考值，赋值给chance，在退出时参考使用
        /// </summary>
        public object[] ReferValues;
    }
}
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
        public List<string> Result;
        public RunResultClass()
        {
            Notice = new RunNoticeClass();
            Result = new List<string>();
        }
    }
}
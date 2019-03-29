//using CFZQ_JRGCDB;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.StrategyLibForWD
{
    /// <summary>
    /// 运行结果类
    /// </summary>
    public class RunResultClass
    {
        public RunNoticeClass Notice;
        public MTable Result;
        public RunResultClass()
        {
            Notice = new RunNoticeClass();
            Result = new MTable();
        }
    }
}
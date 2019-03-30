using System.Text;

namespace WolfInv.com.GuideLib
{
    /// <summary>
    /// 运行通知类
    /// </summary>
    public class RunNoticeClass
    {
        public bool Success;
        public StringBuilder DebugMsg;
        public string Msg;
        public RunNoticeClass()
        {
            Success = false;
            DebugMsg = new StringBuilder();
            Msg = null;
        }
    }
}
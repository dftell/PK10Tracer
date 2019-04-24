using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.LogLib;
namespace ClientSvr
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            Run:
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ExecWinFormControlSvr()
            };
            try
            {
                ServiceBase.Run(ServicesToRun);
            }
            catch(Exception ce)
            {
                LogableClass.ToLog("系统异常", ce.Message, ce.StackTrace);
                goto Run;
            }
        }
    }
}

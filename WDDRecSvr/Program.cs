using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
using WolfInv.com.WDDataInit;
using WolfInv.com.WinInterComminuteLib;
namespace WDDRecSvr
{
    static class Program
    {
        public static GlobalClass gc = null;
        public static WXLogClass wxlog;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            gc = new GlobalClass();
            string url = gc.WXLogUrl;
            url = gc.WXLogNoticeUser;
            ServiceBase[] ServicesToRun;
            LogableClass.ToLog("构建接收服务", "开始");
            //new WDDataInit().CreateChannel("SECData", true);
            //ipcsvr.CreateChannel<WDDataInit>("WDDataInit", "IPC_SECDATA");
            wxlog = new WXLogClass(gc.ClientUserName, gc.WXLogNoticeUser, gc.WXLogUrl);
            wxlog.Log("初始化系统", "各种配置读取完毕并有效初始化！", string.Format(gc.WXLogUrl, gc.WXSVRHost));
            ServicesToRun = new ServiceBase[]
            {
                new Service1<TimeSerialData>()
            };
            ServiceBase.Run(ServicesToRun);
        }

        

    }
}

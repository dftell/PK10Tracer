using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using WolfInv.com.ServerInitLib;
using WolfInv.com.LogLib;
using System.Timers;
using WolfInv.com.ExchangeLib;
using WolfInv.com.BaseObjectsLib;
namespace DataRecSvr
{
    public static class Program
    {
        static void Main()
        {
            Program<TimeSerialData>.Main();
        }
    }

    public static class Program<T> where  T:TimeSerialData
    {
        public static ServiceSetting<T> AllServiceConfig;
        public static GlobalClass gc = null;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        public static void Main()
        {
            
            try
            {
                gc = new GlobalClass();
                string url = gc.WXLogUrl;
                url = gc.WXLogNoticeUser;
                ServiceBase[] ServicesToRun;
                LogableClass.ToLog("构建计算服务", "开始");
                CalcService<T> cs = new CalcService<T>();
                LogableClass.ToLog("构建接收服务", "开始");
                ReceiveService<T> rs = new ReceiveService<T>();
                //SubscriptData sd = new SubscriptData();
                rs.CalcProcess = cs;
                //只有接收数据是默认启动，计算服务由接收数据触发
                ServicesToRun = new ServiceBase[] 
			    { 
                    rs//,sd
			    };
                LogableClass.ToLog("初始化服务器全局设置", "开始");
                InitSystem();
                AllServiceConfig.wxlog.Log("初始化系统", "各种配置读取完毕并有效初始化！", string.Format(gc.WXLogUrl, gc.WXSVRHost));
                LogableClass.ToLog("启动通道", "开始");
                new CommuniteClass().StartIPCServer();
                ServiceBase.Run(ServicesToRun);
                //AllServiceConfig.wxlog.Log("退出服务", "意外停止服务", string.Format(gc.WXLogUrl, gc.WXSVRHost));
            }
            catch (Exception e)
            {
                LogableClass.ToLog("初始化服务失败", e.StackTrace);
                AllServiceConfig.wxlog.Log("退出服务，运行错误", string.Format("{0}[{1}]",e.Message,e.StackTrace), string.Format(gc.WXLogUrl, gc.WXSVRHost));
            }

        }

        

        static void InitSystem()
        {
            AllServiceConfig = new ServiceSetting<T>();
            AllServiceConfig.Init(null);
            AllServiceConfig.GrpThePlan(false);
            AllServiceConfig.CreateChannel(GlobalClass.TypeDataPoints.First().Key,true);//根据不同的数据建立不同的端口 必须使用独占模式

            AllServiceConfig.AllAssetUnits.Values.ToList<AssetUnitClass>().ForEach(p => p.Run());//打开各开关
            //RemoteCommClass<ServiceSetting>.SetRemoteInst(AllServiceConfig);
            //AllServiceConfig.AllLogs = new LogInfo().GetLogAfterDate(DateTime.Today.AddHours(-1));
        }
    }
}

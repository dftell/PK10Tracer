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
        public static ServiceSetting<TimeSerialData> AllServiceConfig;
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {

            try
            {
                GlobalClass gc = new GlobalClass();
                string url = gc.WXLogUrl;
                url = gc.WXLogNoticeUser;
                ServiceBase[] ServicesToRun;
                LogableClass.ToLog("构建计算服务", "开始");
                CalcService<TimeSerialData> cs = new CalcService<TimeSerialData>();
                LogableClass.ToLog("构建接收服务", "开始");
                ReceiveService<TimeSerialData> rs = new ReceiveService<TimeSerialData>();
                //SubscriptData sd = new SubscriptData();
                rs.CalcProcess = cs;
                //只有接收数据是默认启动，计算服务由接收数据触发
                ServicesToRun = new ServiceBase[] 
			    { 
                    rs//,sd
			    };
                LogableClass.ToLog("初始化服务器全局设置", "开始");
                InitSystem();
                LogableClass.ToLog("启动通道", "开始");
                new CommuniteClass().StartIPCServer();
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception e)
            {
                LogableClass.ToLog("初始化服务失败", e.StackTrace);
            }

        }

        

        static void InitSystem()
        {
            AllServiceConfig = new ServiceSetting<TimeSerialData>();
            AllServiceConfig.Init(null);
            AllServiceConfig.GrpThePlan(false);
            AllServiceConfig.CreateChannel(null);

            AllServiceConfig.AllAssetUnits.Values.ToList<AssetUnitClass>().ForEach(p => p.Run());//打开各开关
            //RemoteCommClass<ServiceSetting>.SetRemoteInst(AllServiceConfig);
            //AllServiceConfig.AllLogs = new LogInfo().GetLogAfterDate(DateTime.Today.AddHours(-1));
        }
    }
}

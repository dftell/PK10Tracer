using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WAPIWrapperCSharp;
using WolfInv.com.StrategyLibForWD;
using WolfInv.com.SecurityLib;
using WolfInv.com.CFZQ_LHProcess;
using WolfInv.com.ServerInitLib;
using WolfInv.com.ExchangeLib;
using WolfInv.com.LogLib;
using WolfInv.com.WinInterComminuteLib;
using DataRecSvr;

namespace Test_Win
{
    public class GlobalObj
    {
        public WindAPI w;
        public SystemGlobal Sys;
    }
    static class Program
    {
        public static ServiceSetting AllServiceConfig;
        private delegate void barDelegate(int no);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            LogableClass.ToLog("初始化服务器全局设置", "开始");
            InitSystem();
            LogableClass.ToLog("启动通道", "开始");
            new CommuniteClass().StartIPCServer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            GlobalObj gb = new GlobalObj();
            gb.w = new WindAPI();
            gb.w.start();
            
            Form2 frm = new Form2(gb);
            Application.Run(frm);
        }

        static void InitSystem()
        {
            AllServiceConfig = new ServiceSetting();
            AllServiceConfig.Init(null);
            AllServiceConfig.GrpThePlan(false);
            AllServiceConfig.CreateChannel(null);

            AllServiceConfig.AllAssetUnits.Values.ToList<AssetUnitClass>().ForEach(p => p.Run());//打开各开关
            //RemoteCommClass<ServiceSetting>.SetRemoteInst(AllServiceConfig);
            //AllServiceConfig.AllLogs = new LogInfo().GetLogAfterDate(DateTime.Today.AddHours(-1));
        }

    }
}

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
using WolfInv.com.BaseObjectsLib;
namespace Test_Win
{
    public class GlobalObj
    {
        public WindAPI w;
        public SystemGlobal Sys;
    }
    static class Program
    {
        //public static ServiceSetting AllServiceConfig;
        private delegate void barDelegate(int no);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main1()
        {
            LogableClass.ToLog("初始化服务器全局设置", "开始");
            InitSystem<TimeSerialData>();
            LogableClass.ToLog("启动通道", "开始");
            new CommuniteClass().StartIPCServer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GlobalObj gb = new GlobalObj();
            gb.w = new WindAPI();
            gb.w.start();
            ReceiveService<TimeSerialData> rc = new ReceiveService<TimeSerialData>();
            rc.Start();
            Form2 frm = new Form2(gb);
            Application.Run(frm);
        }



        public static ServiceSetting<TimeSerialData> AllServiceConfig;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //ServiceBase[] ServicesToRun;

                //只有接收数据是默认启动，计算服务由接收数据触发
                //ServicesToRun = new ServiceBase[]
                //{
                //    rs,sd
                //};
                LogableClass.ToLog("初始化服务器全局设置", "开始");
                InitSystem<TimeSerialData>();
                LogableClass.ToLog("启动通道", "开始");
                new CommuniteClass().StartIPCServer();
                //ServiceBase.Run(ServicesToRun);
                GlobalObj gb = new GlobalObj();
                gb.w = new WindAPI();
                //gb.w.start();
                //new ReceiveService().Start();

                Form2 frm = new Form2(gb);
                Application.Run(frm);
            }
            catch (Exception e)
            {
                LogableClass.ToLog("初始化服务失败", e.StackTrace);
            }

        }



        static void InitSystem<T>() where T : TimeSerialData
        {
            LogableClass.ToLog("构建计算服务", "开始");
            CalcService<T> cs = new CalcService<T>();
            LogableClass.ToLog("构建接收服务", "开始");
            ReceiveService<T> rs = new ReceiveService<T>();
            //SubscriptData sd = new SubscriptData();
            rs.CalcProcess = cs;
            AllServiceConfig = new ServiceSetting<T>() as ServiceSetting<TimeSerialData>;
            AllServiceConfig.Init(null);
            AllServiceConfig.GrpThePlan(false);
            AllServiceConfig.CreateChannel(null);

            AllServiceConfig.AllAssetUnits.Values.ToList<AssetUnitClass>().ForEach(p => p.Run());//打开各开关
            //RemoteCommClass<ServiceSetting>.SetRemoteInst(AllServiceConfig);
            //AllServiceConfig.AllLogs = new LogInfo().GetLogAfterDate(DateTime.Today.AddHours(-1));
            DataRecSvr.Program.AllServiceConfig = AllServiceConfig;
            rs.Start();
        }
    }
}
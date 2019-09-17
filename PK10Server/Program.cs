﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using WolfInv.com.ServerInitLib;
using WolfInv.com.ProbMathLib;
using WolfInv.com.LogLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.ProbMathLib;
namespace PK10Server
{
    //////static class Program
    //////{
    //////    [STAThread]
    //////    static void Main()
    //////    {

    //////        Application.EnableVisualStyles();
    //////        Application.SetCompatibleTextRenderingDefault(false);

    //////        /**
    //////         * 当前用户是管理员的时候，直接启动应用程序
    //////         * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
    //////         */
    //////        //获得当前登录的Windows用户标示
    //////        System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
    //////        System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
    //////        //判断当前登录用户是否为管理员
    //////        if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
    //////        {
    //////            //如果是管理员，则直接运行
    //////            Application.Run(new MainForm());
    //////        }
    //////        else
    //////        {
    //////            //创建启动对象
    //////            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
    //////            startInfo.UseShellExecute = true;
    //////            startInfo.WorkingDirectory = Environment.CurrentDirectory;
    //////            startInfo.FileName = Application.ExecutablePath;
    //////            //设置启动动作,确保以管理员身份运行
    //////            startInfo.Verb = "runas";
    //////            try
    //////            {
    //////                System.Diagnostics.Process.Start(startInfo);
    //////            }
    //////            catch
    //////            {
    //////                return;
    //////            }
    //////            //退出
    //////            Application.Exit();
    //////        }
    //////    }
    //////}
    static class Program
    {
        ////public static GlobalClass gc;
        ////public static Dictionary<string, StragClass> AllStragList;
        ////public static Dictionary<string, StragRunPlanClass<T>> AllRunPlans;
        public static ServiceSetting<TimeSerialData> AllGlobalSetting;
 
        public static frm_StragMonitor<TimeSerialData> frm_Monitor;
        static WXLogClass wxlog;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ////new c1().printName();
            ////new c2().printName();
            ////new c3().printName();
            ////return;
            try
            {
                GlobalClass gc = new GlobalClass();

                LogableClass.ToLog("测试", "看看");
                InitSystem();
                AllGlobalSetting.wxlog.Log("初始化系统", "各种配置读取完毕并有效初始化！", string.Format(gc.WXLogUrl, gc.WXSVRHost));
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
                AllGlobalSetting.wxlog.Log("关闭程序", "终止界面", string.Format(gc.WXLogUrl, gc.WXSVRHost));
            }
            catch(Exception ce)
            {
                MessageBox.Show(string.Format("{0}:{1}", ce.Message, ce.StackTrace));
            }
        }

        static void InitSystem()
        {
            AllGlobalSetting = new ServiceSetting<TimeSerialData>();
            AllGlobalSetting.Init(null);
            AllGlobalSetting.GrpThePlan(false);
            
        }

        
        
    }

    class c1
    {
        public static string Name = "C1";
        public void printName()
        {
            MessageBox.Show(Name);
        }
    }

    class c2:c1
    {
        public c2()
        {
            Name = "c2";
        }
    
    }

    class c3 : c1
    {

    }
}

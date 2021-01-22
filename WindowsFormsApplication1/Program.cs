using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WolfInv.com.ServerInitLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.WDDataInit;
namespace BackTestSys
{

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Program<TimeSerialData>.Main();
        }
    }
    static class Program<T> where T:TimeSerialData
    {
        public static ServiceSetting<T> AllSettings;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //////if (!CommitClass.LoadHtml())
            //////{
            //////    MessageBox.Show("无法加载控件！");
            //////    return;
            //////}
            
            AllSettings = new ServiceSetting<T>();
            AllSettings.Init(null);
            GlobalClass gc = new GlobalClass();
            string url = gc.WXLogUrl;
            url = gc.WXLogNoticeUser;
            //WDDataInit<T>.vipDocRoot = AllSettings.gc.VipDocRootPath;
            try
            {
                
                AllSettings.wxlog.Log("初始化系统", "各种配置读取完毕并有效初始化！", string.Format(gc.WXLogUrl, gc.WXSVRHost));
                Application.Run(new BackTestFrm<T>());
            }
            catch(Exception ce)
            {
                AllSettings.wxlog.Log("异常退出系统", ce.Message, string.Format(gc.WXLogUrl, gc.WXSVRHost));

            }
        }
    }
}

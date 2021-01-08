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
            //WDDataInit<T>.vipDocRoot = AllSettings.gc.VipDocRootPath;
            try
            {
                Application.Run(new BackTestFrm<T>());
            }
            catch(Exception ce)
            {

            }
        }
    }
}

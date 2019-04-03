using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WolfInv.com.ServerInitLib;
using WolfInv.com.BaseObjectsLib;
namespace BackTestSys
{
    

    static class Program
    {
        public static ServiceSetting<TimeSerialData> AllSettings;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //////if (!CommitClass.LoadHtml())
            //////{
            //////    MessageBox.Show("无法加载控件！");
            //////    return;
            //////}
            AllSettings = new ServiceSetting<TimeSerialData>();
            AllSettings.Init(null);
            Application.Run(new BackTestFrm<TimeSerialData>());
        }
    }
}

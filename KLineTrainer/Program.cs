using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.ServerInitLib;
using WolfInv.com.WDDataInitLib;
namespace KLineTrainer
{
    static class Program
    {
        public static GlobalClass gb;
        public static DataTypePoint dtp;
        public static ServiceSetting<TimeSerialData> AllSettings;
        public static InitLocalDataClass LocalData;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            gb = new GlobalClass();
            dtp = GlobalClass.TypeDataPoints.First().Value;
            InitLocalDataClass.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

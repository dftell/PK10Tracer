using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
//using WolfInv.com.SecurityLib;
namespace ExchangeTermial
{
    public static class Program
    {
        public static GlobalClass gc;
        public static string VerNo;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            VerNo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            gc = new GlobalClass();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            Application.Exit();
        }
    }
}

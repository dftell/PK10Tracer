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
        static void Main(string[] args)
        {
            VerNo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            gc = new GlobalClass();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string strName=null;
            string strPassword=null;
            if(args != null && args.Length >= 2)
            {
                strName = args[0];
                strPassword = args[1];
            }
            Application.Run(new Form1(strName,strPassword));
            Application.Exit();
        }
    }
}

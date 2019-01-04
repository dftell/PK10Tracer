using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PK10CorePress;
namespace ExchangeTermial
{
    public static class Program
    {
        public static GlobalClass gc;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            gc = new GlobalClass();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            Application.Exit();
        }
    }
}

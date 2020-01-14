using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace WolfInv.com.MaxEntClass
{
    

   
    static class Program
    {
        
        //public static ServiceSetting AllServiceConfig;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {


           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            ////gb.w = new WindAPI();
            ////gb.w.start();
      
            Form1 frm = new Form1();
            Application.Run(frm);
        }
    }
}
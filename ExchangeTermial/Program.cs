using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
//using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
using Gecko;
namespace ExchangeTermial
{
    public static class Program
    {
        public static GlobalClass gc;
        public static string VerNo;
        public static WXLogClass wxl;
        public static string Title;
        public static string User;
        public static int UserId;
        public static bool Reboot;
        public static string strName = null;
        public static string strPassword = null;
        public static bool AutoLogin = false;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            VerNo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            
            //Xpcom.Initialize("Firefox");
            gc = new GlobalClass();
            System.Drawing.Image img = null;
            //string retver = VerPwdClass.getString(img);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(args != null && args.Length >= 2)
            {
                strName = args[0];
                strPassword = args[1];
                AutoLogin = true;
            }
            Form1 frm = null;
            wxl = new WXLogClass("客户端",gc.WXLogNoticeUser,string.Format(gc.WXLogUrl,gc.WXSVRHost));//指定默认登录用户，为捕捉第一次产生错误用。
            //ContinueRun:
            try
            {
                
                //var programDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                //Gecko.Xpcom.Initialize(Path.Combine(programDirectory, "xulrunner"));

                frm =  new Form1(strName, strPassword, AutoLogin);
                string msg = wxl.Log(string.Format("{0}","客户端启动！"));
                
                Application.Run(frm);
                
                wxl.Log(string.Format("{0}", "客户端退出！"));

            }
            catch(Exception ce)
            {
                wxl = new WXLogClass(User, gc.WXLogNoticeUser, gc.WXLogUrl);
                LogableClass.ToLog("错误","退出界面:"+ce.Message,ce.StackTrace);
                wxl.Log("错误退出交易终端，请立即手动启动终端！", string.Format("{0}:退出界面!",Title),string.Format("详细原因[{0}]:{1}" , ce.Message, ce.StackTrace));
                ////AutoLogin = true;
                ////if(frm != null)
                ////    GC.SuppressFinalize(frm);
                ////frm = null;

                //goto ContinueRun;
            }
            finally
            {
                if(frm != null)
                   GC.SuppressFinalize(frm);
            }
            //Application.Exit();
        }
    }
}

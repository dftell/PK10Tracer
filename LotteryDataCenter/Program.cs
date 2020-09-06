using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
namespace LotteryDataCenter
{
    public static class Program
    {
        public static GlobalClass gblc;
        public static DataPointConfigClass dc;
        public static WXLogClass wxl;
        [STAThread]
        static void Main()
        {
            gblc = new GlobalClass();
            dc = new DataPointConfigClass();
            wxl = new WXLogClass("数据服务器启动", gblc.WXLogNoticeUser, string.Format(gblc.WXLogUrl, gblc.WXSVRHost));//指定默认登录用户，为捕捉第一次产生错误用。
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            receiveCenter rc = new receiveCenter();
            mainWindow frm = new mainWindow();
            rc.frm = frm;
            rc.Start(null);
            frm.initForm();
            Application.Run(frm);
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main1()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new receiveCenter()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

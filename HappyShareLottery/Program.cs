using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.ShareLotteryLib;
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.WXMsgCom;
namespace HappyShareLottery
{
    static class Program
    {
        public static WebInterfaceClass wif;
        public static ShareLotteryPlanCollection plancolls;
        public static Timer Heart_Timer;
        static int Heart_minutes = 5;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                frm_planMonitor frm = new frm_planMonitor();
                Heart_Timer = new Timer();
                Heart_Timer.Interval = Heart_minutes * 60 * 1000;
                Heart_Timer.Enabled = true;
                
                Heart_Timer.Tick += Heart_Timer_Tick;
                string strclassname = typeof(WebInterfaceClass).Name.Split('\'')[0];
                string url = string.Format("ipc://IPC_{0}/{1}", "wxmsg", strclassname);
                //LogableClass.ToLog("监控终端", "刷新数据", url);
                //_UseSetting = wc.GetServerObject<ServiceSetting<TimeSerialData>>(url);
                WinComminuteClass comm = new WinComminuteClass();
                wif = comm.GetServerObject<WebInterfaceClass>(url, false);
                Heart_Timer.Start();
                Heart_Timer_Tick(null, null);
                wxMessageReceiveSvr svr = new wxMessageReceiveSvr(wif,frm);
                //svr.Monitor = new frm_planMonitor();
                //svr.wif = wif;
                svr.Start();
                
                
                Application.Run(svr.Monitor);
            }
            catch (Exception ce)
            {
                MessageBox.Show(string.Format("{0}:{1}",ce.Message,ce.StackTrace));
                return;
            }
            finally
            {
                Heart_Timer.Stop();
                wif = null;
            }
        }

        private static void Heart_Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                wif.HeartCheck();
            }
            catch
            {

            }
        }
    }
}

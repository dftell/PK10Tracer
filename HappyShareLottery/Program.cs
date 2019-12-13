using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.JdUnionLib;
using WolfInv.com.ShareLotteryLib;
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.WXMsgCom;
using WolfInv.Com.MetaDataCenter;
using WolfInv.Com.WCS_Process;

namespace HappyShareLottery
{
    static class Program
    {
        public static WebInterfaceClass wif;
        public static ShareLotteryPlanCollection plancolls;
        public static Timer Heart_Timer;
        static int Heart_minutes = 5;
        public static string UserId = "testUser";
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
                try
                {
                    GlobalShare.MainAssem = Assembly.GetExecutingAssembly();
                    GlobalShare.AppDllPath = Application.StartupPath;
                    GlobalShare.Init(Application.StartupPath);
                    ForceLogin();
                }
                catch (Exception ce)
                {
                    return;
                }
                


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

        
        static void ForceLogin()//强制登录！
        {
            string LoginName = UserId;
            CITMSUser user = new CITMSUser();
            user.LoginName = LoginName;
            UserGlobalShare userinfo = new UserGlobalShare(user.LoginName);
            userinfo.appinfo.UserInfo = new UpdateData();//重新实例化
            DataSet ds = new DataSet();
            ////foreach (DataColumn col in ds.Tables[0].Columns)
            ////{
            ////    string strcol = col.ColumnName;
            ////    UpdateItem ui = new UpdateItem(strcol, dr[strcol].ToString());
            ////    userinfo.appinfo.UserInfo.Items.Add(strcol, ui);
            ////}

            DataSource.GetDataSourceMapping();
            userinfo.mapDataSource = DataSource.GetGlobalSourcesClone();
            userinfo.UpdateSource();//替换datasource
            userinfo.CurrUser = user;
            if (!GlobalShare.UserAppInfos.ContainsKey(LoginName))
                GlobalShare.UserAppInfos.Add(LoginName, userinfo);
            UserPerm uperm = new UserPerm(GlobalShare.SystemAppInfo.PermUserPoint, user.LoginName, userinfo);
            //uperm.ToXml();
            //GlobalShare.DataChoices = ; userinfo.InitDataChoices(

            //
            GlobalShare.DataChoices = DataChoice.InitDataChoiceMappings(null);
            userinfo.DataChoices = DataChoice.InitDataChoiceMappings(userinfo);

            SystemConts.ToDictionary().Values.ToList().ForEach(a => {
                if (!userinfo.appinfo.UserInfo.Items.ContainsKey(a.datapoint.Name))
                {
                    userinfo.appinfo.UserInfo.Items.Add(a.datapoint.Name, a);
                }
            });
        }

    }
}

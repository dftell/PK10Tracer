using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.Com.MetaDataCenter;
using WolfInv.Com.WCS_Process;

namespace JdEBuy
{
    static class Program
    {
        public static string UserId = "测试账户";
        public static bool WCS_Inited;
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                GlobalShare.MainAssem = Assembly.GetExecutingAssembly();
                GlobalShare.AppDllPath = Application.StartupPath;
                GlobalShare.Init(Application.StartupPath);
                ForceLogin();
                WCS_Inited = true;
            }
            catch(Exception ce)
            {
                return;
            }
            Application.Run(new Form1(null));
        }

        public static void ForceLogin()//强制登录！
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

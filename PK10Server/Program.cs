using System;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
using WolfInv.com.ServerInitLib;
namespace PK10Server
{
    //////static class Program
    //////{
    //////    [STAThread]
    //////    static void Main()
    //////    {

    //////        Application.EnableVisualStyles();
    //////        Application.SetCompatibleTextRenderingDefault(false);

    //////        /**
    //////         * 当前用户是管理员的时候，直接启动应用程序
    //////         * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
    //////         */
    //////        //获得当前登录的Windows用户标示
    //////        System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
    //////        System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
    //////        //判断当前登录用户是否为管理员
    //////        if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
    //////        {
    //////            //如果是管理员，则直接运行
    //////            Application.Run(new MainForm());
    //////        }
    //////        else
    //////        {
    //////            //创建启动对象
    //////            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
    //////            startInfo.UseShellExecute = true;
    //////            startInfo.WorkingDirectory = Environment.CurrentDirectory;
    //////            startInfo.FileName = Application.ExecutablePath;
    //////            //设置启动动作,确保以管理员身份运行
    //////            startInfo.Verb = "runas";
    //////            try
    //////            {
    //////                System.Diagnostics.Process.Start(startInfo);
    //////            }
    //////            catch
    //////            {
    //////                return;
    //////            }
    //////            //退出
    //////            Application.Exit();
    //////        }
    //////    }
    //////}
    static class Program
    {
        ////public static GlobalClass gc;
        ////public static Dictionary<string, StragClass> AllStragList;
        ////public static Dictionary<string, StragRunPlanClass<T>> AllRunPlans;
        public static ServiceSetting<TimeSerialData> AllGlobalSetting;
 
        public static frm_StragMonitor<TimeSerialData> frm_Monitor;
        static WXLogClass wxlog;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ////new c1().printName();
            ////new c2().printName();
            ////new c3().printName();
            ////return;
            try
            {
                GlobalClass gc = new GlobalClass();

                LogableClass.ToLog("测试", "看看");
                InitSystem();
                AllGlobalSetting.wxlog.Log("初始化系统", "各种配置读取完毕并有效初始化！", string.Format(gc.WXLogUrl, gc.WXSVRHost));
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //设置应用程序处理异常方式：ThreadException处理
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Application.Run(new MainForm());
                AllGlobalSetting.wxlog.Log("关闭程序", "终止界面", string.Format(gc.WXLogUrl, gc.WXSVRHost));
            }
            catch(Exception ce)
            {
                MessageBox.Show(string.Format("{0}:{1}", ce.Message, ce.StackTrace));
            }
        }

        static void InitSystem()
        {
            AllGlobalSetting = new ServiceSetting<TimeSerialData>();
            AllGlobalSetting.Init(null);
            AllGlobalSetting.GrpThePlan(false);
            
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //LogManager.WriteLog(str);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //LogManager.WriteLog(str);
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");
            return sb.ToString();
        }


    }

    class c1
    {
        public static string Name = "C1";
        public void printName()
        {
            MessageBox.Show(Name);
        }
    }

    class c2:c1
    {
        public c2()
        {
            Name = "c2";
        }
    
    }

    class c3 : c1
    {

    }
}

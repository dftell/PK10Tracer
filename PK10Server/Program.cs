using System;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
using WolfInv.com.ServerInitLib;
using WolfInv.com.WinInterComminuteLib;
using System.Linq;
using DataRecSvr;
namespace PK10Server
{

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Program<TimeSerialData>.Main();
        }
    }

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
        public static class Program<T> where T:TimeSerialData
    {
        ////public static GlobalClass gc;
        ////public static Dictionary<string, StragClass> AllStragList;
        ////public static Dictionary<string, StragRunPlanClass<T>> AllRunPlans;
        public static ServiceSetting<T> AllGlobalSetting;
 
        public static frm_StragMonitor<T> frm_Monitor;
        public static WXLogClass wxlog;
        public static GlobalClass gc;
        public static operateClass optFunc;
        static Timer tm_heart;
        static MainForm<T> frm;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ////new c1().printName();
            ////new c2().printName();
            ////new c3().printName();
            ////return;
            try
            {
               gc  = new GlobalClass();
                optFunc = new operateClass();
                //LogableClass.ToLog("测试", "看看");
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
                wxlog = new WXLogClass("客户端", gc.WXLogNoticeUser, string.Format(gc.WXLogUrl, gc.WXSVRHost));//指定默认登录用户，为捕捉第一次产生错误用。
                
                if (GlobalClass.TypeDataPoints.First().Value.onlyDebug==9)//永远不执行，到内部去执行测试界面
                {
                    testWindow frm1 = new testWindow();
                    Application.Run(frm1);
                }
                else
                {
                    frm = new MainForm<T>();
                    tm_heart = new Timer();
                    tm_heart.Enabled = true;
                    tm_heart.Interval = 800;
                    tm_heart.Tick += Tm_heart_Tick;
                    Application.Run(frm);
                }
                
                AllGlobalSetting.wxlog.Log("关闭程序", "终止界面", string.Format(gc.WXLogUrl, gc.WXSVRHost));
            }
            catch(Exception ce)
            {
                MessageBox.Show(string.Format("{0}:{1}", ce.Message, ce.StackTrace));
            }
        }

        private static void Tm_heart_Tick(object sender, EventArgs e)
        {
            int bigCycle = (int)((GlobalClass.TypeDataPoints.Values.First() == null) ? 5 * 1000 : GlobalClass.TypeDataPoints.First().Value.ReceiveSeconds * 1000);
            try
            {
                if (UseSetting != null)
                {
                    tm_heart.Interval = 800;
                    bool haveRec = UseSetting.haveReceiveData;
                    if (haveRec)
                    {
                        CalcFinishedEvent(DateTime.Now);
                    }
                }
                else
                {
                    if (tm_heart.Interval < bigCycle)//只有当间隔小于大周期时才提醒，默认让它继续工作，一直到连接到为再自动改为小周期。
                    {
                        tm_heart.Enabled = false;
                        MessageBox.Show(string.Format("服务已经终止！将变更刷新时间为{0}秒！",bigCycle/1000));
                        tm_heart.Interval = bigCycle;
                        tm_heart.Enabled = true;
                    }
                }
            }
            catch(Exception ce)
            {
                
                tm_heart.Enabled = false;//停掉
                MessageBox.Show(string.Format("{0}", ce.Message));
            }
            
        }

        static void CalcFinishedEvent(DateTime dt)
        {
            AllGlobalSetting.wxlog.Log("接收到服务计算完成消息", string.Format("服务器计算完成时间:{0}",dt), string.Format(gc.WXLogUrl, gc.WXSVRHost));
            optFunc.RefreshData(frm);
        }

        static ServiceSetting<T> _UseSetting = null;//供后面调用一切服务内容用
        public static ServiceSetting<T> UseSetting
        {
            get
            {
                if (_UseSetting == null)
                {
                    try
                    {

                        WinComminuteClass wc = new WinComminuteClass();
                        string strclassname = typeof(ServiceSetting<T>).Name.Split('\'')[0];
                        string url = string.Format("ipc://IPC_{0}/{1}", GlobalClass.TypeDataPoints.First().Key, strclassname);
                        LogableClass.ToLog("监控终端", "刷新数据", url);
                        _UseSetting = wc.GetServerObject<ServiceSetting<T>>(url, false);
                    }
                    catch (Exception ce)
                    {
                        string msg = ce.Message;
                        return null;
                        //MessageBox.Show(string.Format("获取用户设置错误:{0}", ce.Message));
                    }
                }
                return _UseSetting;
            }
        }




        static void InitSystem()
        {
            AllGlobalSetting = new ServiceSetting<T>();
            AllGlobalSetting.Init(null);
            AllGlobalSetting.GrpThePlan(false);
            
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            //MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //LogManager.WriteLog(str);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            //MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    public class operateClass
    {
        public Action RefreshMainWindow;
        public Action RefreshMonitorWindow;

        public void RefreshData<T>(MainForm<T> frm) where T:TimeSerialData
        {
            System.Threading.Thread.Sleep(1000);
            if (this.RefreshMainWindow == null)
            {
                Program<T>.wxlog.Log("无法刷新","主数据窗口事件未初始化！", string.Format(Program<T>.gc.WXLogUrl, Program<T>.gc.WXSVRHost));
            }
            else
            {

                Program<T>.wxlog.Log("刷新主数据", "主数据刷新！", string.Format(Program<T>.gc.WXLogUrl, Program<T>.gc.WXSVRHost));
                this.RefreshMainWindow();
                
            }
            //System.Threading.Thread.Sleep(1000);
            if (this.RefreshMonitorWindow == null)
            {
                Program<T>.wxlog.Log("无法刷新", "主监控窗口事件未初始化！", string.Format(Program<T>.gc.WXLogUrl, Program<T>.gc.WXSVRHost));
                frm.tsmi_RunMonitor_Click(null,null);
            }
            else
            {
                Program<T>.wxlog.Log("刷新监控数据", "主监控窗口数据刷新！", string.Format(Program<T>.gc.WXLogUrl, Program<T>.gc.WXSVRHost));
                this.RefreshMonitorWindow();
            }
        }
    }

}

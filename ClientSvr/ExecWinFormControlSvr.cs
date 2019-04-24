using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using WolfInv.com.LogLib;
using System.Threading;
using System.IO;
using System.Security.AccessControl;

namespace ClientSvr
{
    partial class ExecWinFormControlSvr : ServiceBase,iLog
    {
        System.Timers.Timer httpTimer = new System.Timers.Timer();
        static Dictionary<long, Process> AllProcess;
        static string InstUrl = "http://www.wolfinv.com/PK10/app/SelfAccountExecList.asp";
        public string LogName
        {
            get
            {
                return "交易程序管理器";
            }
        }

        public ExecWinFormControlSvr()
        {
            InitializeComponent();
            Frm_Monitor frm = new Frm_Monitor();
            
        }


        void InitTimers()
        {
            httpTimer.Enabled = true;
            httpTimer.Interval = 1 * 1000;
            httpTimer.AutoReset = true;
            httpTimer.Elapsed += HttpTimer_Elapsed;
        }

        private void HttpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshSystemProcesses();
            WebClient wc = new WebClient();
            try
            {
                string url = wc.DownloadString(InstUrl);

            }
            catch(Exception ce)
            {
                Log(ce.Message, ce.StackTrace);
            }
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }

        static void RefreshSystemProcesses()
        {
            string key = "ExchangeTermial";
            AllProcess = new Dictionary<long, Process>();
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                if (AllProcess.ContainsKey(p.Id))
                    continue;
                if(!p.ProcessName.Equals(key))
                {
                    continue;
                }
                AllProcess.Add(p.Id, p);
                //string info = p.Id + "  " + p.ProcessName + "  " + p.MainWindowTitle + "  " + p.StartTime + p.VirtualMemorySize64;
            }
        }

        public void Log(string logname, string Topic, string msg)
        {
            LogableClass.ToLog(LogName, Topic, msg);
        }

        public void Log(string Topic, string Msg)
        {
            Log(LogName, Topic, Msg);
        }

        public void Log(string msg)
        {
            Log("Msg",msg);
        }

        public bool UpdateClient(string key,bool ForceClose = false)
        {
            if(ForceClose)
            {
                if(!ShutProcess(GetProcessByName(key)))
                {
                    return false;
                }
                Thread.Sleep(1000);
            }
            string keypath = GetKeyPath(key);
            string newpath = GetKeyPath("Newest");
            try
            {
                if (Directory.Exists(keypath))
                {
                    Directory.Delete(keypath,true);
                }
                CopyDir(newpath, keypath);
            }
            catch(Exception ce)
            {
                return false;
            }

            return true;
        }

        public string GetKeyPath(string key)
        {
            return string.Format("{0}\\{1}\\", AppDomain.CurrentDomain.BaseDirectory,key);
        }


        public Process GetProcessByName(string key)
        {
            foreach(Process p in AllProcess.Values)
            {
                if(p.MainWindowTitle.IndexOf(string.Format("[{0}]",key))>0)
                {
                    return p;
                }
            }
            return null;
        }
        
        

        public bool ShutProcess(Process p)
        {
            try
            {
                while (!p.CloseMainWindow())
                {
                    p.Kill();
                }
                return true;
            }
            catch(Exception ce)
            {
                return false;
            }
        }

        public void CopyHosts()
        {
            string hostfile = string.Format("{0}\\hosts.bat", AppDomain.CurrentDomain.BaseDirectory);
            Process.Start(hostfile);
            
        }

        private static void CopyDir(DirectoryInfo origin, string target)
        {
            if (!target.EndsWith("\\"))
            {
                target += "\\";
            }
            if (!Directory.Exists(target))
            {
                Create(target);
                //Create(target, origin.GetAccessControl());
            }
            FileInfo[] fileList = origin.GetFiles();
            DirectoryInfo[] dirList = origin.GetDirectories();
            foreach (FileInfo fi in fileList)
            {
                File.Copy(fi.FullName, target + fi.Name, true);
            }
            foreach (DirectoryInfo di in dirList)
            {
                CopyDir(di, target + di.Name);
            }
            DirectoryInfo tmp = new DirectoryInfo(target);
            tmp.Attributes = origin.Attributes;
            tmp.SetAccessControl(origin.GetAccessControl());
        }

        private static void CopyDir(string origin, string target)
        {
            if (!origin.EndsWith("\\"))
            {
                origin += "\\";
            }
            if (!target.EndsWith("\\"))
            {
                target += "\\";
            }

            DirectoryInfo info = new DirectoryInfo(origin);
            if (!Exist(target))
            {
                Create(target, info.GetAccessControl());//创建目录,访问权限
            }

            FileInfo[] fileList = info.GetFiles();
            DirectoryInfo[] dirList = info.GetDirectories();
            foreach (FileInfo fi in fileList)
            {
                File.Copy(fi.FullName, target + fi.Name, true);
            }
            foreach (DirectoryInfo di in dirList)
            {
                //CopyDir(origin + "\\" + di.Name, target + "\\" + di.Name);
                CopyDir(di.FullName, target + "\\" + di.Name);
            }
            //设置目录属性和访问权限
            DirectoryInfo tmp = new DirectoryInfo(target);
            tmp.Attributes = info.Attributes;
            tmp.SetAccessControl(info.GetAccessControl());
        }

        static void Create(string key,DirectorySecurity ds = null)
        {
            Directory.CreateDirectory(key,ds);
        }

        static bool Exist(string key)
        {
            return Directory.Exists(key);
        }
    }



    public class ExecInfo
    {
        public string User;
        public string Password;
        public string Path;
        public string GrpConfig;
        public bool Running;
        public DateTime StartTime;
        

        public bool RunExe()
        {
            return true;
        }

        public bool CheckRunning()
        {
            return true;
        }

        
    }

}

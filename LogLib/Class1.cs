using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection;
namespace LogLib
{
    public abstract class LogClass:MarshalByRefObject
    {
        protected static Dictionary<DateTime,string> LogBuffs = new Dictionary<DateTime,string>();
        static string strFile = "log.txt";
        object lockobj =new object();

        void WriteBase(string txt, string strLogName)
        {
            WriteBase(txt, strLogName, "log");
        }
        void threadWrite(string txt, string strLogName, string strLogType)
        {
            threadWrite(txt, null, strLogName, strLogType);
        }
        void threadWrite(string txt,string specPath, string strLogName, string strLogType)
        {
            threadWrite(txt, specPath, strLogName, strLogType, false,false);
        }
        
        protected void threadWrite(string txt,string specPath,string strLogName,string strLogType,bool NoNeedTime,bool WriteOver)
        {
            string str = string.Format("{0}.{1}", strLogName, strLogType);
            string strPath = Assembly.GetExecutingAssembly().Location; //typeof(LogClass).Assembly.Location;
            string strLogPath = string.Format("{0}\\{1}", specPath==null?Path.GetDirectoryName(strPath):specPath, str);
            //FileStream fs = new FileStream(strLogPath, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                DateTime dt = DateTime.Now;
                if (!LogBuffs.ContainsKey(dt))
                {
                    LogBuffs.Add(dt, string.Format("{0}_{1}", strLogName, txt));
                }
                StreamWriter sr = new StreamWriter(strLogPath, !WriteOver);
                if (NoNeedTime)
                {
                    sr.WriteLine(string.Format("{0}", txt));
                }
                else
                {
                    sr.WriteLine(string.Format("{0}:{1}", dt.ToString(), txt));
                    
                }
                sr.Close();
            }
            catch (Exception ce)
            {
                //WriteBase(ce.Message,"错误");
            }
        }




        void WriteBase(string txt,string strLogName,string strLogType)
        {
            lock (lockobj)
            {
                threadWrite(txt, strLogName, strLogType);
            }
        }

         public void Write(string Topic, string msg, string serviceName)
        {
            WriteBase(string.Format("[{0}]{1}", Topic, msg), serviceName);
        }
        public void Write(string Topic, string msg)
        {
            Write(string.Format("[{0}]{1}", Topic, msg));
        }

        public void Write(string txt)
        {
            WriteBase(txt, "comm");
        }

        public void WriteWithFunc(string ServiceName, string msg)
        {
            WriteWithFunc(ServiceName, "",msg, 1);
        }
        public void WriteWithFunc(string ServiceName, string topic, string msg)
        {
            WriteWithFunc(ServiceName,topic, msg, 1);
        }
        public  void WriteWithFunc(string ServiceName, string topic, string msg, int deep)//直接调用为1级，每增加一级间隔deep加1
        {
            var st = new System.Diagnostics.StackTrace();
            while (st.GetFrame(deep) == null)
            {
                deep--;
                
            }
            Write(string.Format("{0}:{1}", topic, st.GetFrame(deep).GetMethod().Name), msg, ServiceName);
        }

        protected void WriteToFile(string txt,string specPath, string filename, string filetype)
        {
            threadWrite(txt, specPath,filename, filetype, true,false);
        }
    }

    public class LogInfo : LogClass
    {
        public Dictionary<DateTime, string> GetLogAfterDate(DateTime dt)
        {
            //LogableClass.ToLog("获取指定时间以后的日志列表", dt.ToString());
            if (LogBuffs == null) 
                LogBuffs = new Dictionary<DateTime, string>();
            Dictionary<DateTime, string> tmp = new Dictionary<DateTime, string>();
            foreach (DateTime t in LogBuffs.Keys)
            {
                if (t.CompareTo(dt)>0)
                {
                    tmp.Add(t,LogBuffs[t]);
                }
            }
            //tmp = LogBuffs.Where(t => (t.Key.CompareTo(dt) > 0)) as Dictionary<DateTime, string>;
            if (tmp == null)
            {
                LogableClass.ToLog("原始日志数量", LogBuffs.Count.ToString());
                return new Dictionary<DateTime, string>();
            }
            //LogableClass.ToLog("获取到日志数量", tmp.Count.ToString());
            return tmp;
        }

        public void ClearBuff(DateTime befDate)
        {
            LogableClass.ToLog("清空指定时间以后的日志列表", befDate.ToString());
            if (LogBuffs == null)
                LogBuffs = new Dictionary<DateTime, string>();
            Dictionary<DateTime, string> tmp = LogBuffs.Where(t => t.Key > befDate) as Dictionary<DateTime, string>;
            LogBuffs = tmp;
        }

        public void ClearBuff()
        {
            LogBuffs.Clear();// 
        }

        public DataTable GetLogTableAfterDate(DateTime dt)
        {
            DataTable tab = new DataTable();
            tab.Columns.Add("LogTime", typeof(DateTime));
            tab.Columns.Add("Log", typeof(string));
            Dictionary<DateTime, string> ret = GetLogAfterDate(dt);
            if (ret == null)
            {
                return tab;
            }
            foreach (DateTime key in ret.Keys)
            {
                DataRow dr = tab.NewRow();
                dr[0] = key;
                dr[1] = ret[key];
                tab.Rows.Add(dr);
            }
            return tab;

        }
        
        public void WriteFile(string txt, string specPath, string filename, string filetype,bool NoTime,bool writeover)
        {
            threadWrite(txt, specPath, filename, filetype, NoTime, writeover);
        }
    }

    public interface iLog
    {
        string LogName { get;}
        void Log(string logname, string Topic, string msg);
        void Log(string Topic, string Msg);
        void Log(string msg);
    }

    public class LogableClass:MarshalByRefObject,iLog
    {
        static LogInfo LogClass = new LogInfo(); 
        protected static string _logname = "System";
        public string LogName
        {
            get { return _logname; }
        }

        public void Log(string topic, string Msg)
        {
            Log(LogName, topic, Msg);
        }

        public void Log(string msg)
        {
            LogClass.WriteWithFunc(LogName, "", msg, 2);
        }

        public void Log(string logname, string Topic, string msg)
        {
            LogClass.WriteWithFunc(logname, Topic, msg, 2);
        }

        public static void ToLog(string topic, string Msg)
        {
            LogClass.WriteWithFunc(_logname, topic, Msg, 2);
        }

        public static void ToLog(string msg)
        {
            LogClass.WriteWithFunc(_logname, "", msg, 3);
        }
        public static void ToLog(string logname, string Topic, string msg)
        {
            LogClass.WriteWithFunc(logname, Topic, msg, 2);
        }
    }
}

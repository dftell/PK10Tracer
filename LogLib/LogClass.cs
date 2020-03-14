using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;
namespace WolfInv.com.LogLib
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
            Write(string.Format("{0}:{1}", topic, string.Join("<<", st.GetFrames().Skip(2).Take(5).Select(a=>a.GetMethod().Name))), msg, ServiceName);
        }

        protected void WriteToFile(string txt,string specPath, string filename, string filetype)
        {
            threadWrite(txt, specPath,filename, filetype, true,false);
        }
    }
}

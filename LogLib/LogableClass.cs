using System;
namespace WolfInv.com.LogLib
{
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

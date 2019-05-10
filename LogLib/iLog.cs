namespace WolfInv.com.LogLib
{
    public interface iLog
    {
        string LogName { get;}
        void Log(string logname, string Topic, string msg);
        void Log(string Topic, string Msg);
        void Log(string msg);
    }
}

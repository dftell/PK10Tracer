using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using BaseObjectsLib;
using LogLib;
using System.Security.Principal;
namespace WinInterComminuteLib
{
    public class WinComminuteClass:LogableClass
    {
        static List<IpcClientChannel> iccs = new List<IpcClientChannel>();

        public T GetServerObject<T>(string specName)
        {
            string FullName = typeof(T).FullName;
            string[] NameArr = FullName.Split('.');
            string ClassName = NameArr[NameArr.Length - 1];
            if (specName != null && specName.Trim().Length > 0)
            {
                ClassName = specName;
            }
            IpcClientChannel icc = new IpcClientChannel();
            if (ChannelServices.GetChannel(icc.ChannelName) == null)
            {
                Log("IPC客户端日志", "注册客户端通道", icc.ChannelName);
                ChannelServices.RegisterChannel(icc);
                iccs.Add(icc);
            }
            else
            {
                Log("IPC客户端日志", "存在客户端通道", icc.ChannelName);
            }
            T ret = default(T);
            object obj;
            Log("IPC客户端日志", "检查是否管理员", IsRoot().ToString());
            try
            {
                string ChannleName =  string.Format("ipc://WolfIPC_Channel/{0}", ClassName);
                Log("IPC客户端日志","访问通道", ChannleName);
                obj = Activator.GetObject(typeof(T), ChannleName);
                ret = (T)obj;
            }
            catch (Exception e)
            {
                Log("IPC客户端日志", "访问通道失败", e.Message);
                return ret;
            }
            return ret; //返回一个空内容的壳，需要调用GetRemoteData实际调取数据
        }

        public static bool IsRoot()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();  
            WindowsPrincipal principal = new WindowsPrincipal(identity);  
            return principal.IsInRole(WindowsBuiltInRole.Administrator);  
        }
    }
}

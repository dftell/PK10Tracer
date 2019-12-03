using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
//using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
using System.Security.Principal;

namespace WolfInv.com.WinInterComminuteLib
{
    public class WinComminuteClass:LogableClass
    {
        static Dictionary<string,IpcClientChannel> iccs =new Dictionary<string,IpcClientChannel>();

        public T GetServerObject<T>(string url,bool WriteLog=false) where T:MarshalByRefObject
        {
            string FullName = typeof(T).Name;
            string ClassName = FullName.Split('\'')[0];
            T ret = default(T);
            try
            {

                //string ChannleName = string.Format("ipc://IPC_{0}/{1}", specName, ClassName);
                string ChannleName = url;
                ToLog("IPC客户端日志", "指定客户端通道URL", ChannleName);
                string cname = string.Format("{0}", ClassName);
                IpcClientChannel icc = null;// new IpcClientChannel(cname,null);
                //ChannelServices.RegisterChannel(icc, false);
                //if (ChannelServices.GetChannel(icc.ChannelName) == null)
                if(!iccs.ContainsKey(cname))
                {
                    icc = new IpcClientChannel();
                    ToLog("IPC客户端日志", "注册客户端通道", icc.ChannelName);
                    ChannelServices.RegisterChannel(icc,false);
                    ////WellKnownClientTypeEntry remotEntry = new WellKnownClientTypeEntry(typeof(T), ChannleName);
                    RemotingConfiguration.RegisterWellKnownClientType(typeof(T), ChannleName);
                    iccs.Add(cname, icc);
                    
                    
                }
                else
                {
                    ToLog("IPC客户端日志", "存在客户端通道", cname);
                    //iccs[cname];
                    icc = iccs[cname];
                }
                
                //string[] allstr  = ChannelServices.GetUrlsForObject(ret);
                //if (allstr != null)
                ToLog("IPC客户端日志", "所有可通道", getAllChannelsInfo());
                object obj;
                //Log("IPC客户端日志", "检查是否管理员", IsRoot().ToString());
                

                //Log("IPC客户端日志","访问通道", ChannleName);
                obj = Activator.GetObject(typeof(T), ChannleName);
                //obj = remotEntry;
                //ret = (T)obj;// (T)Convert.ChangeType(remotEntry, typeof(T));
                
                ret = (T)obj;
                (ret as RemoteServerClass).Success = true;
                //if (WriteLog)
                //    Log("IPC客户端日志", "访问通道成功", ChannleName);
            }
            catch (Exception e)
            {
                ToLog("IPC客户端日志", "访问通道失败", string.Format("{0}:{1}", e.Message, e.StackTrace));
                if (ret is RemoteServerClass)
                {
                    (ret as RemoteServerClass).Success = false;
                    (ret as RemoteServerClass).Message = string.Format("{0}:{1}", e.Message, e.StackTrace);
                }
                return ret;
            }
            return ret; //返回一个空内容的壳，需要调用GetRemoteData实际调取数据
        }

        public bool GetServerObject<T>(string url) where T : MarshalByRefObject
        {
            string FullName = typeof(T).Name;
            string ClassName = FullName.Split('\'')[0];
            //T ret = default(T);
            try
            {

                //string ChannleName = string.Format("ipc://IPC_{0}/{1}", specName, ClassName);
                string ChannleName = url;
                ToLog("IPC客户端日志", "指定客户端通道URL", ChannleName);
                string cname = string.Format("{0}", ClassName);
                IpcClientChannel icc = null;// new IpcClientChannel(cname,null);
                //ChannelServices.RegisterChannel(icc, false);
                //if (ChannelServices.GetChannel(icc.ChannelName) == null)
                if (!iccs.ContainsKey(cname))
                {
                    icc = new IpcClientChannel();
                    ToLog("IPC客户端日志", "注册客户端通道", icc.ChannelName);
                    ChannelServices.RegisterChannel(icc, false);
                    //WellKnownClientTypeEntry remotEntry = new WellKnownClientTypeEntry(typeof(T), ChannleName);
                    RemotingConfiguration.RegisterWellKnownClientType(typeof(T), ChannleName);
                    iccs.Add(cname, icc);
                }
                else
                {
                    ToLog("IPC客户端日志", "存在客户端通道", cname);
                    icc = iccs[cname];
                }
                
                ToLog("IPC客户端日志", "所有可通道", getAllChannelsInfo());
                return true;
            }
            catch (Exception e)
            {
                ToLog("IPC客户端日志", "访问通道失败", string.Format("{0}:{1}", e.Message, e.StackTrace));
                
                return false;
            }
            //return ret; //返回一个空内容的壳，需要调用GetRemoteData实际调取数据
        }

        public static bool IsRoot()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();  
            WindowsPrincipal principal = new WindowsPrincipal(identity);  
            return principal.IsInRole(WindowsBuiltInRole.Administrator);  
        }

        public static string getAllChannelsInfo()
        {
            string[] allstr = ChannelServices.RegisteredChannels.Select(a => {
                IChannel ic = a;
                if (ic is IpcServerChannel)
                {
                    IpcServerChannel sic = ic as IpcServerChannel;
                    return sic.GetChannelUri();
                }
                else
                {
                    IpcClientChannel cic = ic as IpcClientChannel;
                    return cic.ChannelName;
                }
                return "";
            }).ToArray();
            return string.Join(";", allstr);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
//using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
using System.Reflection;
using System.Data;
using WolfInv.com.RemoteObjectsLib;
using System.Security.AccessControl;

namespace WolfInv.com.WinInterComminuteLib
{

    public class IPCService:LogableClass
    {
        static Dictionary<string,IpcServerChannel>  channels = new Dictionary<string,IpcServerChannel>();
        public bool CreateChannel<T>(string specName=null,string channelName= "WolfIPC_Channel")
        {
            string FullName = typeof(T).FullName;
            string[] NameArr = FullName.Split('.');
            string ClassName = NameArr[NameArr.Length-1];
            if (specName != null)
            {
                ClassName = specName;
            }
            //string channelName = "wolfin";
            string ChannelName = channelName;// "WolfIPC_Channel";
            try
            {
                ToLog("IPC服务端日志", "检查是否是管理员", WinComminuteClass.IsRoot().ToString());
                ToLog("IPC服务端日志", "正在初始化通道", ChannelName);
                if (!channels.ContainsKey(ChannelName))
                {
                    ToLog("IPC服务端日志", "注册通道", ChannelName);
                    Hashtable ht = new Hashtable();
                    ht["portname"] = ChannelName;
                    ht["name"] = "ipc";
                    ht["authorizedGroup"] = "everyone";
                    IpcServerChannel channel = new IpcServerChannel(ht,null);
                    ChannelServices.RegisterChannel(channel,true);
                    channels.Add(ChannelName, channel);
                }
                ToLog("IPC服务端日志", "正在注册通道绑定数据类型", ClassName);
                //IpcServerChannel channel = new IpcServerChannel(string.Format("WolfIPC_Channel"));
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), ClassName, WellKnownObjectMode.SingleCall);
                //RemoteCommClass<T> obj = new RemoteCommClass<T>();
                ToLog("IPC服务端日志", "绑定数据类型完毕", ClassName);
                
            }
            catch (Exception e)
            {
                ToLog(string.Format("IPC服务端日志", "初始化通道[{0}]失败", FullName), e.Message);
                return false;
            }
            return true;
        }
    }

    public delegate List<T> GetIPCDataDeleage<T>(params object[] args);
    public delegate void ExeOperateDeleage();
    public delegate void ExeOperateWithDataTableDeleage(DataTable dt);
    public delegate DataTable GetTableDeleage(params object[] args);
    public class RemoteCommClass<T>:CommResult<T>
    {
        public RemoteCommClass()
        {
            _RemoteInst = (T)Activator.CreateInstance(typeof(T));
        }
    
        /// <summary>
        /// T类型的远程实例，单个对象直接取该成员，数组取result
        /// </summary>
        public static T _RemoteInst;

        public T RemoteInst
        {
            get
            {
                return _RemoteInst;
            }
        }
             

        public static void SetRemoteInst(T obj)
        {
            LogableClass.ToLog("填充远程值",obj.ToString());
            _RemoteInst = obj;
        }

        public ExeOperateDeleage ExecuteEvent; 
        
        public GetIPCDataDeleage<T> IPCDataEvent;
        /// <summary>
        /// 客户端获得远程内部数据
        /// </summary>
        /// <param name="args"></param>
        public List<T> GetRemoteData(params object[] args)
        {
            try
            {
                Result = IPCDataEvent(args);
            }
            catch (Exception e)
            {
                LogableClass.ToLog("获取远程端数据错误", e.Message);
                return null;
            }
            if (Result == null)
                return null;
            Cnt = Result.Count;
            return Result;
        }

        public void Test(string strtest)
        {
            LogableClass.ToLog("远程日志","远程调用", strtest);
        }
    }

    public class RemoteServerClass : LogableClass
    {
        protected Dictionary<string, IpcServerChannel> channels = new Dictionary<string, IpcServerChannel>();
        public bool Success=true;
        public string Message;
        public bool CreateChannel(string specName,bool useSingleton = false)
        {
            string FullName = this.GetType().Name;
            string ClassName = FullName.Split('\'')[0];
            //if (specName != null)
            //{
            //    ClassName = specName;
            //}
            string ChannelName = string.Format("IPC_{0}", specName);
            string cname = string.Format("{0}_{1}", specName, ClassName);
            //if (ClassName != null)
            //    ChannelName = ClassName;
            try
            {
                ToLog("IPC服务端日志", "检查是否是管理员", WinComminuteClass.IsRoot().ToString());
                ToLog("IPC服务端日志", "正在初始化通道", ChannelName);
                if (!channels.ContainsKey(ChannelName))
                {
                    ToLog("IPC服务端日志", "注册通道", ChannelName);
                    Hashtable ht = new Hashtable();
                    ht["portName"] = ChannelName;
                    ht["name"] = "ipc";
                    ht["authorizedGroup"] = "everyone";

                    BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
                    serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                    BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
                    //IDictionary props = new Hashtable();
                    //props["port"] = Convert.ToInt32(txtClientPort.Text);
                    //HttpChannel chan = new HttpChannel(props, clientProv, serverProv);
                    //CommonSecurityDescriptor csd = new CommonSecurityDescriptor();
                    IpcServerChannel channel = new IpcServerChannel(ht, serverProv);
                    //IpcServerChannel channel = new IpcServerChannel(ChannelName);
                    ChannelServices.RegisterChannel(channel, false);
                    //ToLog("IPC服务端日志", "正在注册通道绑定数据类型", ClassName);
                    ToLog("IPC服务端日志", "注册通道完毕", channel.ChannelName);
                    channels.Add(ChannelName, channel);
                    WellKnownObjectMode mode = useSingleton ? WellKnownObjectMode.Singleton : WellKnownObjectMode.SingleCall;
                    RemotingConfiguration.RegisterWellKnownServiceType(this.GetType(), ClassName, mode);
                    ToLog("IPC服务端日志", "绑定数据类型完毕", ClassName);
                }
                
                ToLog("IPC服务端日志", string.Format("初始化通道成功"), WinComminuteClass.getAllChannelsInfo());
                //IpcServerChannel channel = new IpcServerChannel(string.Format("WolfIPC_Channel"));
                //RemoteCommClass<T> obj = new RemoteCommClass<T>();


            }
            catch (Exception e)
            {
                ToLog("IPC服务端日志", string.Format("初始化通道[{0},{1}]失败",specName, FullName), e.Message);
                return false;
            }
            return true;
        }

    }



}





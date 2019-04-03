using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.ExchangeLib;
using WolfInv.com.LogLib;
using WolfInv.com.ServerInitLib;
using WolfInv.com.Strags;
using System.Data;
namespace DataRecSvr
{
    public class CommuniteClass
    {
        public void StartIPCServer()
        {
            //LogableClass.ToLog("通道初始化", "准备建立");//启动所有需要获取的类
            IPCService ipcsvr = new IPCService();
            //ipcsvr.CreateChannel<LogInfo>();
            //ipcsvr.CreateChannel<RemoteCommClass<ServiceSetting>>();
            ////ipcsvr.CreateChannel<String>();
            ////ipcsvr.CreateChannel<Dictionary<DateTime, string>>("LogDictionary");
            ////ipcsvr.CreateChannel<GlobalClass>();
            ////ipcsvr.CreateChannel<DataTable>();
            ////ipcsvr.CreateChannel<ExchangeService>();
            ////ipcsvr.CreateChannel<Dictionary<string, StragClass>>("StragClass");
            ////ipcsvr.CreateChannel<Dictionary<string, StragRunPlanClass<T>>>("StragRunPlanClass");
            ////ipcsvr.CreateChannel<Dictionary<string, CalcStragGroupClass<T>>>("CalcStragGroupClass");
            ////ipcsvr.CreateChannel<Dictionary<string, ChanceClass<T>>>("ChanceClass");
            ////ipcsvr.CreateChannel<LogInfo>();
            //LogableClass.ToLog("通道初始化", "完成");
            //RemoteCommClass<ServiceSetting>.SetRemoteInst(Program.AllServiceConfig);
        }
    }
}

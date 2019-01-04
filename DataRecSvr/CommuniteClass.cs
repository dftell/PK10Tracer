using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinInterComminuteLib;
using PK10CorePress;
using BaseObjectsLib;
using ExchangeLib;
using LogLib;
using ServerInitLib;
using Strags;
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
            ////ipcsvr.CreateChannel<Dictionary<string, StragRunPlanClass>>("StragRunPlanClass");
            ////ipcsvr.CreateChannel<Dictionary<string, CalcStragGroupClass>>("CalcStragGroupClass");
            ////ipcsvr.CreateChannel<Dictionary<string, ChanceClass>>("ChanceClass");
            ////ipcsvr.CreateChannel<LogInfo>();
            //LogableClass.ToLog("通道初始化", "完成");
            //RemoteCommClass<ServiceSetting>.SetRemoteInst(Program.AllServiceConfig);
        }
    }
}

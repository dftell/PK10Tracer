﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using WolfInv.com.LogLib;
using WolfInv.com.ExchangeLib;
using System.Data;
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.ServerInitLib
{
    public class ServiceSetting :RemoteServerClass
    {
        public static ServiceSetting lastInst;
        public ServiceSetting()
        {
            lastInst = this;
        }
                
        static GlobalClass _gc;//全局配置
        static ExchangeService _ES;
        static Dictionary<string, StragClass> _AllStrags;
        static Dictionary<string, StragRunPlanClass> _AllRunPlannings;
        static Dictionary<string, CalcStragGroupClass> _AllRunningPlanGrps;
        static Dictionary<string, ChanceClass> _NoCloseChances;
        static Dictionary<string, AssetUnitClass> _AssetUnits;
        static List<StragChance> _AllNewChances;
        static List<ExchangeChance> _AllExchangeChances;
        static Dictionary<string, ChanceClass> _AllNoClosedChances;
        static Dictionary<string, List<double>> _AllStdDevs = new Dictionary<string, List<double>>();
        public GlobalClass gc//全局配置
        {
            get { return _gc; }
            set { _gc = value; }
        }
        public ExchangeService ES
        {
            get { return _ES; }
            set { _ES = value; }
        }
        public ExpectList LastDataSector;
        public Dictionary<string, StragClass> AllStrags
        {
            get
            {
                return _AllStrags;
            }
            set
            {
                _AllStrags = value;
            }
        }

        public Dictionary<string, StragRunPlanClass> AllRunPlannings
        {
            get { return _AllRunPlannings; }
            set { _AllRunPlannings = value; }
        }
        public Dictionary<string, CalcStragGroupClass> AllRunningPlanGrps
        {
            get { return _AllRunningPlanGrps; }
            set { _AllRunningPlanGrps = value; }
        }

        public Dictionary<string, List<double>> AllTotalStdDevList
        {
            get { return _AllStdDevs; }
            set { _AllStdDevs = value; }
        }
        //public Dictionary<string, ChanceClass> NoCloseChances
        //{
        //    get { return _NoCloseChances; }
        //    set { _NoCloseChances = value; }
        //}
        ////public List<StragChance> AllNewChances
        ////{
        ////    get { return _AllNewChances; }
        ////    set { _AllNewChances = value; }
        ////}
        ////public List<ExchangeChance> AllExchangeChances
        ////{
        ////    get { return _AllExchangeChances; }
        ////    set { _AllExchangeChances = value; }
        ////}
        public Dictionary<DateTime, string> AllLogs
        {
            get
            {
                return new LogInfo().GetLogAfterDate(DateTime.Now.AddHours(-12));
            }
        }
        public Dictionary<string, ChanceClass> AllNoClosedChanceList { get { return _AllNoClosedChances; } set { _AllNoClosedChances = value; } }

        public Dictionary<string, AssetUnitClass> AllAssetUnits
        {
            get
            {
                return _AssetUnits;
            }
            set
            {
                _AssetUnits = value;
            }
        }

        public DataTable LogTable
        {
            get
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("时间", typeof(DateTime));
                dt.Columns.Add("内容", typeof(string));
                foreach (DateTime key in AllLogs.Keys)
                {
                    dt.Rows.Add(new object[] { key, AllLogs[key] });
                }
                return dt;
            }
        }

        public void Init(GlobalClass gc)
        {
            if (gc == null)
                gc = new GlobalClass();
            this.gc = gc;
            LogableClass.ToLog("初始化服务器设置","初始化策略列表");
            this.AllStrags = InitServerClass.Init_StragList();
            LogableClass.ToLog("初始化服务器设置", "初始化运行计划列表");
            this.AllRunPlannings = InitServerClass.Init_StragPlans();
            LogableClass.ToLog("初始化服务器设置", "初始资产单元列表");
            this.AllAssetUnits = InitServerClass.Init_AssetUnits();
            this.AllNoClosedChanceList = new Dictionary<string, ChanceClass>();
            InitSecurity();


        }

        public void InitSecurity()
        {
            foreach(string key in GlobalClass.TypeDataPoints.Keys)
            {
                DataTypePoint dtp = GlobalClass.TypeDataPoints[key];
                if(dtp.IsSecurityData == 0)
                {
                    LogableClass.ToLog(string.Format("Type:{0}", key), "非证券类型，无须加载基本信息！");
                    continue;
                }
                dtp.RuntimeInfo = new DataPointBuff(dtp);
                LogableClass.ToLog(string.Format("准备获取[{0}]股票清单",key), "开始");
                dtp.RuntimeInfo.SecurityInfoList = InitSecurityClass.getAllCodes(key);
                if (dtp.RuntimeInfo.SecurityInfoList == null)
                {
                    LogableClass.ToLog(string.Format("准备获取[{0}]股票清单", key), "失败");
                    continue;
                }
                LogableClass.ToLog(string.Format("获取[{0}]股票清单", key), string.Format("股票数量:{0}",dtp.RuntimeInfo.SecurityInfoList.Count));
                string[] codes = dtp.RuntimeInfo.SecurityInfoList.Keys.ToArray();
                dtp.RuntimeInfo.SecurityCodes = codes;
                LogableClass.ToLog(string.Format("准备获取[{0}]日期数据", key), "开始");
                dtp.RuntimeInfo.HistoryDateList = InitSecurityClass.getAllDateList(key);
                LogableClass.ToLog(string.Format("获取[{0}]日期数据", key), string.Format("日期数量:{0}", dtp.RuntimeInfo.HistoryDateList.Count));
                LogableClass.ToLog(string.Format("准备获取[{0}]除权除息数据", key), "开始");
                dtp.RuntimeInfo.XDXRList = InitSecurityClass.getAllXDXRData(key,dtp.RuntimeInfo.getGrpCodes);
                LogableClass.ToLog(string.Format("获取[{0}]除权除息数据", key), string.Format("总数量:{0}", dtp.RuntimeInfo.XDXRList.Sum(p=>p.Value.Count)));
                //GlobalClass.TypeDataPoints[key] = dtp;
            }
        }

        public void GrpThePlan( bool IsBackTest)
        {
            //Log("初始化服务器设置","分组准备运行计划列表");
            Dictionary<string,CalcStragGroupClass> outret = null;
            this.AllRunningPlanGrps = InitServerClass.InitCalcStrags(ref outret, this.AllStrags, this.AllRunPlannings, this.AllAssetUnits, true, IsBackTest);
        }

        public bool SetPlanStatus(string PGUID, bool Running)
        {
            if (!this.AllRunPlannings.ContainsKey(PGUID))
            {
                Log("操作失败", "指定的计划不存在！");
                return false;
            }
            this.AllRunPlannings[PGUID].Running = Running;
            return true;
        }

        public void ClearAllData()
        {
            LogableClass.ToLog("清空指令", "清空所有数据！");
        }

    }
}

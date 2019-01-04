using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using Strags;
using LogLib;
using ExchangeLib;
namespace ServerInitLib
{
    public class InitServerClass:LogableClass
    {
        

        public static Dictionary<string, StragClass> Init_StragList()
        {

            string stragList = GlobalClass.ReReadStragList();

            Dictionary<string, StragClass>  AllStragList = new Dictionary<string, StragClass>();

            if (stragList == null || stragList.Trim().Length == 0)
            {
                ToLog("策略列表","为空！");
                return AllStragList;
            }
            List<StragClass> list = StragClass.getObjectListByXml<StragClass>(stragList); //StragClass.getStragsByXml(stragList);
            if (list == null)
            {
                return AllStragList;
            }
            AllStragList = list.ToDictionary(t => t.GUID, v => v);

            ToLog("策略列表", AllStragList.Count.ToString());
            return AllStragList;
        }

        public static Dictionary<string, AssetUnitClass> Init_AssetUnits()
        {
            string strUnits = GlobalClass.ReReadAssetUnitList();
            Dictionary<string, AssetUnitClass> AllStragList = new Dictionary<string, AssetUnitClass>();

            if (strUnits == null || strUnits.Trim().Length == 0)
            {
                ToLog("资产单元列表", "为空！");
                return AllStragList;
            }
            List<AssetUnitClass> list = StragClass.getObjectListByXml<AssetUnitClass>(strUnits); //StragClass.getStragsByXml(stragList);
            if (list == null)
            {
                return AllStragList;
            }
            AllStragList = list.ToDictionary(t => t.UnitId, v => v);

            ToLog("资产单元列表", AllStragList.Count.ToString());
            return AllStragList;
        }

        public static Dictionary<string, StragRunPlanClass> Init_StragPlans()
        {
            string stragList = GlobalClass.getStragRunningPlan(true);
            Dictionary<string, StragRunPlanClass> AllRunPlans = new Dictionary<string, StragRunPlanClass>();
            if (stragList == null || stragList.Trim().Length == 0)
            {
                ToLog("策略运行计划列表", "为空！");
                return AllRunPlans;
            }
            List<StragRunPlanClass> list = StragRunPlanClass.getObjectListByXml<StragRunPlanClass>(stragList);
            if (list == null)
            {
                return AllRunPlans;
            }
            AllRunPlans = list.ToDictionary(t => t.GUID, v => v);

            ToLog("策略列表", AllRunPlans.Count.ToString());
            return AllRunPlans;
        }

        public static SettingClass Init_CommSetting()
        {
            SettingClass sc = new SettingClass();
            return sc;
        }

        /// <summary>
        /// 初始化计划，将数据类型和运算类型相同的计划分组
        /// </summary>
        public static Dictionary<string, CalcStragGroupClass> InitCalcStrags(ref Dictionary<string, CalcStragGroupClass> AllStatusStrags,Dictionary<string, StragClass> AllStrags, Dictionary<string, StragRunPlanClass> list,Dictionary<string,AssetUnitClass> AssetUnits, bool StartTheAuto,bool IsBackTest)
        {
            if(AllStatusStrags == null)
                AllStatusStrags = new Dictionary<string, CalcStragGroupClass>();
            foreach (string key in list.Keys) //按相同类型+视图分类，相同类分在一组
            {
                StragRunPlanClass spc = list[key];
                if (AllStatusStrags.SelectMany(t => t.Value.UseSPlans.Select(a => a.GUID)).Contains(key))//支持后续加入计划，只要状态合适都可以加入
                    continue;
                if (!AllStrags.ContainsKey(spc.PlanStrag.GUID))
                {
                    ToLog("计划非法", "计划所对应策略已注销");
                    continue;
                }
                StragClass sc = AllStrags[spc.PlanStrag.GUID];
                spc.PlanStrag = sc;
                if (spc.AssetUnitInfo != null)//分配资产单元号
                {
                    
                    sc.AssetUnitId = spc.AssetUnitInfo.UnitId;
                }
                string strModel = "Type_{0}_ViewBySerial_{1}";//按类和数据视图分类
                string strKey = string.Format(strModel, sc.GetType().Name, sc.BySer);
                if (!IsBackTest)
                {
                    if (spc.ExpiredTime < DateTime.Now)
                    {
                        ToLog("计划时间不匹配", "超过计划过期时间");
                        continue;
                    }
                    if (!JudgeInRunTime(DateTime.Now, spc))//没在指定运行时间内，跳过
                    {
                        ToLog("计划时间不匹配", "未在指定时间区间内");
                        continue;
                    }
                }
                if(StartTheAuto)//如果第一次运行，将此标志设为真，将自动启动策略
                    spc.Running = spc.AutoRunning;
                if (!spc.Running)
                {
                    continue;
                }
                
                CalcStragGroupClass csg = null;
                if (!AllStatusStrags.ContainsKey(strKey))
                {
                    csg = new CalcStragGroupClass();
                    AllStatusStrags.Add(strKey, csg);
                }
                csg = AllStatusStrags[strKey];
                if(spc.AssetUnitInfo != null)
                {
                    string uid = spc.AssetUnitInfo.UnitId;
                    if (AssetUnits.ContainsKey(uid))
                    {
                        if (!csg.UseAssetUnits.ContainsKey(uid))
                        {
                            csg.UseAssetUnits.Add(uid, AssetUnits[uid]); //注意，一定是要用全局设置的单元资产，才有动态的金额等信息
                        }
                    }
                    else
                    {
                        ToLog("计划资产单元不匹配", "计划所属资产单元不存在");
                    }
                }
                //csg = AllStatusStrags[strKey];
                csg.UseSPlans.Add(spc);
                ToLog("加入计划", spc.StragDescript);
                csg.UseStrags.Add(sc.GUID,sc);
                csg.UseSerial = sc.BySer;
                //ToLog("初始化待计算的计划", string.Format("准备运行的计划数为:{0};", AllStatusStrags.Count));
            }
            return AllStatusStrags;
        }

        /// <summary>
        /// 判断是否超期运行        /// </summary>
        /// <param name="CurrTime"></param>
        /// <param name="spc"></param>
        /// <returns></returns>
        public static bool JudgeInRunTime(DateTime CurrTime, StragRunPlanClass spc)
        {
            string strToday = CurrTime.ToShortDateString();
            DateTime setBegTime = DateTime.Parse(string.Format("{0} {1}", strToday, spc.DailyStartTime));
            DateTime setEndTime = DateTime.Parse(string.Format("{0} {1}", strToday, spc.DailyEndTime));
            if (CurrTime < setBegTime || CurrTime > setEndTime)
            {
                //ToLog("策略超出执行时间", spc.ToString());
                return false;
            }
            return true;
        }

    }
}

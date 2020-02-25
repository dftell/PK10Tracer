using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using WolfInv.com.LogLib;
using WolfInv.com.ExchangeLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.ServerInitLib
{
    public class InitServerClass:LogableClass
    {
        

        public static Dictionary<string, BaseStragClass<T>> Init_StragList<T>() where T : TimeSerialData
        {
            Dictionary<string, BaseStragClass<T>> AllStragList = new Dictionary<string, BaseStragClass<T>>();
            try
            {
                string stragList = GlobalClass.ReReadStragList();
                if (stragList == null || stragList.Trim().Length == 0)
                {
                    ToLog("策略列表", "为空！");
                    return AllStragList;
                }
                List<BaseStragClass<T>> list = BaseStragClass<T>.getObjectListByXml<BaseStragClass<T>>(stragList); //StragClass.getStragsByXml(stragList);
                if (list == null)
                {
                    return AllStragList;
                }
                AllStragList = list.ToDictionary(t => t.GUID, v => v);

                ToLog("策略列表", AllStragList.Count.ToString());
                return AllStragList;
            }
            catch (Exception ce)
            {
                ToLog("策略列表错误", ce.StackTrace);
                return AllStragList;
            }
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

        public static Dictionary<string, StragRunPlanClass<T>> Init_StragPlans<T>() where T:TimeSerialData
        {
            string stragList = GlobalClass.getStragRunningPlan(true);
            Dictionary<string, StragRunPlanClass<T>> AllRunPlans = new Dictionary<string, StragRunPlanClass<T>>();
            if (stragList == null || stragList.Trim().Length == 0)
            {
                ToLog("策略运行计划列表", "为空！");
                return AllRunPlans;
            }
            List<StragRunPlanClass<T>> list = StragRunPlanClass<T>.getObjectListByXml<StragRunPlanClass<T>>(stragList);
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
        public static Dictionary<string, CalcStragGroupClass<T>> InitCalcStrags<T>(DataTypePoint dpt, ref Dictionary<string, CalcStragGroupClass<T>> AllStatusStrags,Dictionary<string, BaseStragClass<T>> AllStrags, Dictionary<string, StragRunPlanClass<T>> list,Dictionary<string,AssetUnitClass> AssetUnits, bool StartTheAuto,bool IsBackTest) where T:TimeSerialData
        {
            if(AllStatusStrags == null)
                AllStatusStrags = new Dictionary<string, CalcStragGroupClass<T>>();
            foreach (string key in list.Keys) //按相同类型+视图分类，相同类分在一组
            {
                StragRunPlanClass<T> spc = list[key];
                if (AllStatusStrags.SelectMany(t => t.Value.UseSPlans.Select(a => a.GUID)).Contains(key))//支持后续加入计划，只要状态合适都可以加入
                    continue;
                if (!AllStrags.ContainsKey(spc.PlanStrag.GUID))
                {
                    ToLog(string.Format("计划{0}非法",spc.PlanStrag.StragScript), "计划所对应策略已注销");
                    
                    continue;
                }
                BaseStragClass<T> sc = AllStrags[spc.PlanStrag.GUID];
                spc.PlanStrag = sc;
                if (spc.AssetUnitInfo != null)//分配资产单元号
                {
                    
                    sc.AssetUnitId = spc.AssetUnitInfo.UnitId;
                }
                ///更新为首先按数据源分类，然后按类和视图分类
                //string strModel = "Type_{0}_ViewBySerial_{1}";//按类和数据视图分类
                //string strKey = string.Format(strModel, sc.GetType().Name, sc.BySer);
                string strModel = "DS_{2}Type_{0}_ViewBySerial_{1}";//更新为首先按数据源分类，然后按类和视图分类
                string strKey = string.Format(strModel, sc.GetType().Name, sc.BySer,spc.UseDataSource);
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
                if(spc.StragLotteryName!= dpt.DataType)
                {
                    ToLog("计划不属于使用的数据源", strKey);
                    continue;
                }

                
                CalcStragGroupClass<T> csg = null;
                if (!AllStatusStrags.ContainsKey(strKey))
                {
                    if (GlobalClass.TypeDataPoints.ContainsKey(spc.UseDataSource))//如果计划不属于数据源，不加载
                    {
                        csg = new CalcStragGroupClass<T>(GlobalClass.TypeDataPoints[spc.UseDataSource]);
                        AllStatusStrags.Add(strKey, csg);
                    }
                }
                if(!AllStatusStrags.ContainsKey(strKey))
                {
                    ToLog("计划不属于使用的数据", strKey);
                    continue;
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
        /// <returns></returns></T>
        public static bool JudgeInRunTime<T>(DateTime CurrTime, StragRunPlanClass<T> spc) where T:TimeSerialData
        {
            
            int diffHours = GlobalClass.TypeDataPoints.First().Value.DiffHours;
            DateTime cprTime = CurrTime.AddHours(diffHours);
            string strToday = CurrTime.AddHours(diffHours).ToShortDateString();
            DateTime setBegTime = DateTime.Parse(string.Format("{0} {1}", strToday, spc.DailyStartTime));
            DateTime setEndTime = DateTime.Parse(string.Format("{0} {1}", strToday, spc.DailyEndTime));
            if (cprTime < setBegTime || cprTime > setEndTime)
            {
                //ToLog("策略超出执行时间", spc.ToString());
                return false;
            }
            return true;
        }

    }
}

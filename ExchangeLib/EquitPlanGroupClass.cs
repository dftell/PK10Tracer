using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.ExchangeLib
{
    public class CalcEquitPlanGroupClass<T>: CalcStragGroupClass<T> where T:TimeSerialData
    {
        CalcEquitPlanGroupClass(DataTypePoint _dtp):base(_dtp)
        {

        }
        public new void ExecRun(object data)
        {
            ExpectList<T> el = data as ExpectList<T>;
            //2019/4/22日出现错过732497，732496两期记录错过，但是732498却收到的情况，同时，正好在732498多次重复策略正好开出结束，因错过2期导致一直未归零，
            //一直长时间追号开出近60期
            //为避免出现这种情况
            //判断是否错过了期数，如果错过期数，将所有追踪策略归零，不再追号,也不再执行选号程序，
            //是否要连续停几期？执行完后，在接收策略里面发现前10期有不连续的情况，直接跳过，只接收数据不执行选号。
            
            if(el.MissExpectCount()>1)//期号不连续
            {

            }



            //Log("计算服务","准备数据", "为每个策略分配数据");
            foreach (string key in UseStrags.Keys)
                UseStrags[key].SetLastUserData(el);
            //准备数据
            BaseCollection<T> cc = null;
            int maxViewCnt = (int)this.UseStrags.Max(t => t.Value.ReviewExpectCnt);
            //Log("计算服务", "最大回览期数", maxViewCnt.ToString());
            cc = new ExpectListProcessBuilder<T>(dtp,el).getProcess().getSerialData(maxViewCnt, this.UseSerial);
            // cc.orgData = el;//必须指定原始数据？
            //Log("计算服务", "中间数据长度",cc.Data.Count.ToString());
            Dictionary<StragClass, List<ChanceClass>> css = new Dictionary<StragClass, List<ChanceClass>>();
            //Log("计算服务", "计算数据", "为每个策略计算最大回顾周期数据");
            //遍历每个策略获得机会
            DbChanceList<T> OldDbList = new DbChanceList<T>();
            Dictionary<string, ChanceClass<T>> OldList = new Dictionary<string, ChanceClass<T>>();
            List<ChanceClass<T>> NewList = new List<ChanceClass<T>>();
            //Log("计算服务", "遍历所有策略", string.Format("策略数量:{0}",this.UseStrags.Count));
            CloseAllExchance(el);//清空所有可视化机会
            #region 获取交易机会
            for (int i = 0; i < this.UseSPlans.Count; i++)
            {
                StragRunPlanClass<T> currPlan = UseSPlans[i];
                if (currPlan.PlanStrag == null)//如果计划所执行的策略为空，只在chance上执行tracer
                {
                    List<ChanceClass<T>> emptycs = CurrExistChanceList.Values.Where(p => p.StragId == null).ToList<ChanceClass<T>>();
                    for (int c = 0; c < emptycs.Count; c++)
                    {
                        ChanceClass<T> CurrCc = emptycs[c];
                        TraceChance<T> tcc = CurrCc as TraceChance<T>;
                        CurrCc.UnitCost = tcc.getChipAmount(GlobalClass.DefaultMaxLost, CurrCc, GlobalClass._DefaultHoldAmtSerials.Value);
                        CurrCc.HoldTimeCnt = CurrCc.HoldTimeCnt + 1;
                        CurrCc.Cost += CurrCc.ChipCount * CurrCc.UnitCost;
                        CurrCc.UpdateTime = CurrCc.CreateTime;
                        OldList.Add(CurrCc.GUID, CurrCc);
                        if (!IsBackTest)//非回测需要额外保存数据
                        {
                            OldDbList.Add(CurrCc.ChanceIndex, CurrCc);
                        }
                    }
                    continue;
                }
                BaseStragClass<T> currStrag = UseStrags[currPlan.PlanStrag.GUID];
                currStrag.SetLastUserData(el);//必须<T>给策略填充数据
                List<ChanceClass<T>> cs = currStrag.getChances(cc, el.LastData);//获取该策略的机会
                if (currStrag is TotalStdDevTraceStragClass)//如果是整体标准差类，记录所有的标准差数据
                {
                    grpTotolStdDic = (currStrag as TotalStdDevTraceStragClass).getAllStdDev();
                }
                if (cs.Count > 0)
                    Log("计算服务", string.Format("策略[{0}/{1}]", currStrag.GUID, currStrag.StragScript), string.Format("取得机会数量为:{0}", cs.Count));
                Dictionary<string, ChanceClass<T>> StragChances = CurrExistChanceList.Where(p => p.Value.StragId == currStrag.GUID).ToDictionary(p => p.Value.ChanceCode, p => p.Value);
                AmoutSerials amts = GlobalClass.getOptSerials(CurrSetting.Odds, currPlan.InitCash, 1);
                Int64 restAmt = currStrag.CommSetting.GetGlobalSetting().DefMaxLost;//初始资金
                #region 遍历各机会
                for (int j = 0; j < cs.Count; j++)//对每个机会，检查上期遗留的机会是否包括
                {
                    bool NeedUseOldData = false;
                    ChanceClass<T> CurrCc = cs[j];
                    CurrCc.HoldTimeCnt = 1;
                    CurrCc.AllowMaxHoldTimeCnt = currPlan.AllowMaxHoldTimeCnt;
                    CurrCc.IncrementType = currPlan.IncreamType;
                    if (currPlan.IncreamType == InterestType.CompoundInterest)
                    {
                        CurrCc.FixRate = currPlan.FixRate;
                    }
                    else
                    {
                        CurrCc.FixAmt = currPlan.FixAmt;
                    }
                    //该语句存在机会重复的风险
                    if (StragChances.ContainsKey(CurrCc.ChanceCode))//未关闭的及机会列表中存在该机会
                    {
                        ChanceClass<T> OldCc = StragChances[CurrCc.ChanceCode];
                        //Log("计算服务", "老机会信息", string.Format("idx:{0};holdcnt:{1}", OldCc.ChanceIndex, OldCc.HoldTimeCnt));
                        //Log("计算服务", "老记录", string.Format("上期相同的机会{0}", CurrCc.ChanceCode));
                        //Log("计算服务", "判断是否允许重复", currStrag.AllowRepeat.ToString());
                        if (!currStrag.AllowRepeat)//如果不允许重复
                        {
                            CurrCc = OldCc;
                            CurrCc.HoldTimeCnt = CurrCc.HoldTimeCnt + 1;
                            NeedUseOldData = true;
                            Log("计算服务", "相同处理", string.Format("出现相同的机会{0},持有次数增1->{1}", CurrCc.ChanceCode, CurrCc.HoldTimeCnt));
                        }
                    }
                    else
                    {
                        //Log("计算服务", string.Format("上期相同未关闭的机会数{0},{1}", CurrExistChanceList.Count, CurrCc.ChanceCode), "本期未出现");
                    }

                    if (currPlan.AssetUnitInfo != null)
                    {
                        if (this.UseAssetUnits.ContainsKey(currPlan.AssetUnitInfo.UnitId))
                        {
                            AssetUnitClass useUnit = UseAssetUnits[currPlan.AssetUnitInfo.UnitId];
                            if (!useUnit.Running)
                            {
                                useUnit.Run();
                            }
                            restAmt = (long)useUnit.getCurrExchangeServer().summary;
                        }
                        else
                            continue;
                    }
                    //Log("计算服务", "再次检查数据", string.Format("出现相同的机会{0},持有次数增1.->{1}", CurrCc.ChanceCode, CurrCc.HoldTimeCnt));
                    CurrCc.UnitCost = -1;//先默认为-1
                    if (currStrag is WolfInv.com.BaseObjectsLib.ISpecAmount)//先从策略级别判断
                    {
                        WolfInv.com.BaseObjectsLib.ISpecAmount testStrag = (currStrag as WolfInv.com.BaseObjectsLib.ISpecAmount);
                        if (testStrag == null)
                        {
                            //等待下一步按机会级别判断
                        }
                        else
                        {
                            CurrCc.UnitCost = testStrag.getChipAmount(restAmt, CurrCc, amts);
                        }
                    }
                    if (CurrCc.UnitCost < 0)//如果策略级别未改变值
                    {
                        if (CurrCc.IsTracer == 1)//如果是自我追踪机会
                        {
                            Log("计算服务", "自我跟踪机会，当前持有次数", string.Format("HoldTimes:{0}", CurrCc.HoldTimeCnt));
                            TraceChance<T> useCc = Convert.ChangeType(CurrCc, currStrag.getTheChanceType()) as TraceChance<T>;
                            //Log("计算服务", "使用的机会持有次数", string.Format("HoldTimes:{0}", useCc.HoldTimeCnt));
                            if (useCc == null) //获得的类型并非跟踪类型
                            {
                                CurrCc.UnitCost = (currStrag as ChanceTraceStragClass).getChipAmount(restAmt, CurrCc, amts);
                            }
                            else
                            {
                                CurrCc.UnitCost = useCc.getChipAmount<T>(restAmt, CurrCc, amts);
                            }
                        }
                        else//默认为ChanceTraceStragClass,其实是不可能触发的，而且会出错，因为ChanceTraceStragClass本身就是ispaceamount
                        {
                            Log("计算服务", "非跟踪机会，持有次数", string.Format("HoldTimes:{0}", CurrCc.HoldTimeCnt));
                            CurrCc.UnitCost = (currStrag as ChanceTraceStragClass).getChipAmount(restAmt, CurrCc, amts);
                        }
                    }
                    //Log("计算服务", "再二次检查数据", string.Format("出现相同的机会{0},持有次数增1->{1}", CurrCc.ChanceCode, CurrCc.HoldTimeCnt));
                    if (NeedUseOldData)//未关闭的及机会列表中存在该机会
                    {
                        Log("计算服务", "策略不可以出现重复", string.Format("策略编号:{0}", CurrCc.UnitCost));
                        CurrCc.Cost += CurrCc.UnitCost * CurrCc.ChipCount;
                        CurrCc.UpdateTime = DateTime.Now;
                        OldList.Add(CurrCc.GUID, CurrCc);
                        if (!IsBackTest)
                        {
                            OldDbList.Add(CurrCc.ChanceIndex, CurrCc);
                        }
                        continue;

                    }
                    CurrCc.HoldTimeCnt = 1;
                    CurrCc.Cost = CurrCc.ChipCount * CurrCc.UnitCost;
                    CurrCc.Gained = 0;
                    CurrCc.Profit = 0;
                    CurrCc.ExecDate = DateTime.Today;
                    CurrCc.CreateTime = DateTime.Now;
                    CurrCc.UpdateTime = CurrCc.CreateTime;
                    CurrCc.StragId = currStrag.GUID;
                    CurrCc.ExpectCode = el.LastData.Expect;
                    CurrCc.AllowMaxHoldTimeCnt = currPlan.AllowMaxHoldTimeCnt;
                    CurrCc.ChanceType = currPlan.OutPutType;
                    NewList.Add(CurrCc);
                }
                #endregion

                #region 未关闭的机会需要自我跟踪
                foreach (string code in StragChances.Keys)
                {
                    ChanceClass<T> CurrCc = StragChances[code];
                    //if (!CurrCc.Tracerable) continue;
                    int cnt = OldList.Values.Where(p => p.ChanceCode.Equals(code)).Count();
                    if (cnt > 0)
                    {
                        continue;
                    }
                    if (currStrag is WolfInv.com.BaseObjectsLib.ISpecAmount)//先从策略级检查
                    {
                        WolfInv.com.BaseObjectsLib.ISpecAmount specStrag = currStrag as WolfInv.com.BaseObjectsLib.ISpecAmount;
                        if (specStrag != null)//如果没有方法，再从机会级检查
                        {
                            CurrCc.HoldTimeCnt++;
                            CurrCc.UnitCost = specStrag.getChipAmount(restAmt, CurrCc, amts);
                            CurrCc.Cost += CurrCc.ChipCount * CurrCc.UnitCost;
                            CurrCc.UpdateTime = DateTime.Now;
                            OldList.Add(CurrCc.GUID, CurrCc);
                            if (!IsBackTest)
                            {
                                OldDbList.Add(CurrCc.ChanceIndex, CurrCc);
                            }
                            continue;
                        }
                    }
                    if (CurrCc.Tracerable)//再检查机会级
                    {

                        CurrCc.HoldTimeCnt++;
                        TraceChance<T> testCc = (TraceChance<T>)CurrCc;
                        if (testCc == null) continue;
                        CurrCc.UnitCost = testCc.getChipAmount<T>(restAmt, CurrCc, amts);
                        CurrCc.Cost += CurrCc.ChipCount * CurrCc.UnitCost;
                        CurrCc.UpdateTime = DateTime.Now;
                        OldList.Add(CurrCc.GUID, CurrCc);
                        if (!IsBackTest)
                        {
                            OldDbList.Add(CurrCc.ChanceIndex, CurrCc);
                        }
                        continue;
                    }
                    else
                    {
                        CurrCc.HoldTimeCnt++;
                        WolfInv.com.BaseObjectsLib.ISpecAmount Strag = (WolfInv.com.BaseObjectsLib.ISpecAmount)currStrag;
                        if (Strag == null) continue;
                        CurrCc.UnitCost = Strag.getChipAmount(restAmt, CurrCc, amts);
                        CurrCc.Cost = CurrCc.ChipCount * CurrCc.UnitCost;
                        CurrCc.UpdateTime = DateTime.Now;
                        OldList.Add(CurrCc.GUID, CurrCc);
                        if (!IsBackTest)
                        {
                            OldDbList.Add(CurrCc.ChanceIndex, CurrCc);
                        }
                    }
                }
                #endregion
            }
            #endregion

            if (!IsBackTest)//额外保存
            {
                //int savecnt = OldDbList.Save(null);
                DataReader dr = DataReaderBuild.CreateReader(GlobalClass.DataTypes.First().Key, null, null);
                //int savecnt = new PK10ExpectReader().SaveChances<T>(OldDbList.Values.ToList<ChanceClass<T>>(), null);
                int savecnt = dr.SaveChances<T>(OldDbList.Values.ToList<ChanceClass<T>>(), null);
                if (OldList.Count > 0)
                    Log("计算服务", "保存已有机会", string.Format("条数：{0};实际条数:{1}", OldList.Count, savecnt));
                savecnt = dr.SaveChances<T>(NewList, null);
                if (NewList.Count > 0)
                    Log("计算服务", "保存新增机会", string.Format("条数：{0};实际条数:{1}", NewList.Count, savecnt));
            }
            //合并到未关闭机会列表中
            NewList.ForEach(p => AllNoClosedChances.Add(p.GUID, p));
            OldList.Values.ToList<ChanceClass<T>>().ForEach(p => AllNoClosedChances.Add(p.GUID, p));//就算是老记录未有guid,当ToTable时已经生成了guid
            ExChange(AllNoClosedChances.Values.ToList<ChanceClass<T>>(), el.LastData.Expect);//执行交易提供可视化
        }

    }
}

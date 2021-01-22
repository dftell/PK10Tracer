﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.BaseObjectsLib;
//using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using WolfInv.com.Strags.Security;
using WolfInv.com.LogLib;
using WolfInv.com.SecurityLib;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using WolfInv.com.GuideLib;
namespace WolfInv.com.ExchangeLib
{
    public delegate bool CalcStragGroupDelegate();
    public delegate void ReturnChances<T>(Dictionary<string, ChanceClass<T>> ChanceList) where T : TimeSerialData;
    public delegate void ReturnStdDevList(Dictionary<string, List<double>> list);
    public class CalcStragGroupClass<T> : DisplayAsTableClass where T : TimeSerialData
    {
        public string GrpName;
        protected DataTypePoint dtp;
        WXLogClass wxl;

        public Action<string,string,string> afterStragProcessed;
        public CalcStragGroupClass(DataTypePoint _dtp)
        {
            wxl = new WXLogClass("服务器管理员",GlobalClass.LogUser, string.Format(GlobalClass.LogUrl,GlobalClass.WXLogHost));
            dtp = _dtp;
        }
        public ConcurrentDictionary<string,MongoReturnDataList<T>> allSecurityDic;
        public bool IsBackTest { get; set; }
        public CalcStragGroupDelegate Finished;
        public ReturnChances<T> GetNoClosedChances;
        public ReturnStdDevList GetAllStdDevList;
        public KLineData<T>.getSingleDataFunc getSingleData;
        public Type UseStragType;
        bool UseBySer;
        public List<StragRunPlanClass<T>> UseSPlans = new List<StragRunPlanClass<T>>();
        public Dictionary<string,BaseStragClass<T>> UseStrags = new Dictionary<string,BaseStragClass<T>>();
        protected SettingClass CurrSetting;
        protected Dictionary<string, ChanceClass<T>> CurrExistChanceList = new Dictionary<string, ChanceClass<T>>();
        public Dictionary<string, AssetUnitClass<T>> UseAssetUnits = new Dictionary<string, AssetUnitClass<T>>();
        public List<ExchangeChance<T>> AllExchance = new List<ExchangeChance<T>>();
        public Dictionary<string, ChanceClass<T>> AllNoClosedChances = new Dictionary<string, ChanceClass<T>>();
        protected Dictionary<string, List<double>> grpTotolStdDic = null;
        public Dictionary<string, List<IndexExpectData>> grpIndexs = null;

        ////public List<StragClass> UseStrags
        ////{
        ////    get
        ////    {
        ////        return _UseStrags;
        ////    }
        ////}

        public string strUseStragClass
        {
            get
            {
                if (UseStrags != null && UseStrags.Count > 0)
                {
                    return UseStrags.First().Value.StragClassName;
                }
                return "";
            }
        }

        public bool UseSerial { get; set; }
        public bool Running { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="el">当期数据</param>
        /// <param name="CommSetting">通用设置</param>
        /// <param name="ExistChances">需要分配给组的未关闭的数据</param>
        public bool initRunningData(SettingClass CommSetting, List<ChanceClass<T>> ExistChances)
        {
            ////if (el.Count>this.UseStrag.ReviewExpectCnt)
            ////{
            ////    return false;
            ////}
            //未关闭列表只代表服务器计算，未包括客户自定义的。所以不是全部，要和数据库合并才全面
            CurrSetting = CommSetting;
            CurrExistChanceList = new Dictionary<string, ChanceClass<T>>();
            //Log("错误", "并没有","测试");
            //ToLog("计算服务", "策略执行准备", "为每个组合分配未关闭的机会");
            for (int i = 0; i < ExistChances.Count; i++)
            {
                ChanceClass<T> cc = ExistChances[i];
                //Log("错误", cc.ChanceCode, cc.ToDetailString());
                bool Matched = false;
                if (this.AllNoClosedChances.ContainsKey(cc.GUID))//先检查内存，如果已经存在,直接加入
                {
                    if(!CurrExistChanceList.ContainsKey(cc.GUID))
                        CurrExistChanceList.Add(cc.GUID, cc);
                    continue;
                }
                foreach(string j in this.UseStrags.Keys)//检查数据表，如果该机会是在本组所拥有的表内
                {
                    if (j == cc.StragId)
                    {
                        if (!CurrExistChanceList.ContainsKey(cc.GUID))
                        {
                            if(CurrExistChanceList.Where(a=>(a.Value.ChanceCode == cc.ChanceCode&& a.Value.StragId == cc.StragId)).Count() == 0)//防止多个机会意外重复
                                CurrExistChanceList.Add(cc.GUID, cc);
                            else
                            {
                                ToLog("计算服务", "策略出现了相同内容的机会", string.Format("策略{0}:{1}", cc.StragId, cc.ChanceCode));
                            }
                        }
                        else
                            ToLog("计算服务", "相同的策略出现了同一个机会", string.Format("策略{0}:{1}", cc.StragId, cc.ChanceCode));
                        Matched = true;
                        break;
                    }
                }
                if (!Matched)
                {
                    //ToLog("计算服务", "该机会不隶属于任何策略", cc.ChanceCode);//用户自定义机会，等会处理
                }
            }
            AllNoClosedChances = new Dictionary<string, ChanceClass<T>>();
            //ToLog("计算服务", "策略执行准备", string.Format("为本组合分配未关闭的机会数为{0};", CurrExistChanceList.Count));
            return true;
        }

        public void Run(object data)
        {
            try
            {
                ExecRun(data);
                if(!IsBackTest)
                    Log("计算服务", "运行完成", "触发回调");
                GetNoClosedChances(AllNoClosedChances);
            }
            catch (Exception e)
            {
                Log("错误","计算服务", string.Format("{0}:{1}",e.Message,  e.StackTrace));
            }
            //GetNoClosedChances(AllNoClosedChances);
            GetAllStdDevList?.Invoke(grpTotolStdDic);
            Finished?.Invoke();
            allSecurityDic = null;

        }
        
        public void ExecRun(object data)
        {
            ExpectList<T> el = data as ExpectList<T>;
            

            //2019/4/22日出现错过732497，732496两期记录错过，但是732498却收到的情况，同时，正好在732498多次重复策略正好开出结束，因错过2期导致一直未归零，
            //一直长时间追号开出近60期
            //为避免出现这种情况
            //判断是否错过了期数，如果错过期数，将所有追踪策略归零，不再追号,也不再执行选号程序，
            //是否要连续停几期？执行完后，在接收策略里面发现前10期有不连续的情况，直接跳过，只接收数据不执行选号。
            bool MissExpects = false;
            if (dtp.IsSecurityData == 0)
            {
                if (el.MissExpectCount() > 1)//期号不连续
                {
                    if (dtp.DataType == "PK10" && IsBackTest == false)
                        MissExpects = true;
                }
            }
            DbChanceList<T> OldDbList = new DbChanceList<T>();
            Dictionary<string, ChanceClass<T>> OldList = new Dictionary<string, ChanceClass<T>>();
            List<ChanceClass<T>> NewList = new List<ChanceClass<T>>();
            if (MissExpects)//不处理任何事情
            {
                return;
            }

            //Log("计算服务","准备数据", "为每个策略分配数据");
            foreach (string key in UseStrags.Keys)
                UseStrags[key].SetLastUserData(el);
            //准备数据
            BaseCollection<T> cc = null;
            int maxViewCnt = 2;
            if(this.UseStrags.Count>0)
                maxViewCnt = Math.Max((int)this.UseStrags.Max(t => t.Value?.ReviewExpectCnt),el.Count);
            //Log("计算服务", "最大回览期数", maxViewCnt.ToString());
            if(dtp.IsSecurityData == 1)
            {
                cc = new WolfInv.com.ExchangeLib.ExpectListProcessBuilderForAll<T>(dtp, el).getProcess()?.getSerialData(maxViewCnt, this.UseSerial);//获取连续数据，可能是外部数据，这个的内部处理必须要改。 2020.6.18
            }
            else
                cc = new WolfInv.com.PK10CorePress.ExpectListProcessBuilder<T>(dtp,el).getProcess().getSerialData(maxViewCnt, this.UseSerial);//获取连续数据，可能是外部数据，这个的内部处理必须要改。 2020.3.30
            // cc.orgData = el;//必须指定原始数据？
            //Log("计算服务", "中间数据长度",cc.Data.Count.ToString());
            Dictionary<StragClass, List<ChanceClass<T>>> css = new Dictionary<StragClass, List<ChanceClass<T>>>();
            //Log("计算服务", "计算数据", "为每个策略计算最大回顾周期数据");
            //遍历每个策略获得机会
            //Log("计算服务", "遍历所有策略", string.Format("策略数量:{0}",this.UseStrags.Count));
            CloseAllExchance(el);//清空所有可视化机会
            #region 遍历每个计划
            //Log("计算服务", string.Format("遍历当前分组",this.GrpName), string.Format("包含计划:{0}",string.Join(";",this.UseSPlans.Select(a=>a.Plan_Name))));
            for (int i = 0; i < this.UseSPlans.Count; i++)
            {
                StragRunPlanClass<T> currPlan = UseSPlans[i];//当前计划
                //Log("计算服务", "处理计划。", currPlan.Plan_Name);
                if (currPlan.PlanStrag == null)//如果计划所执行的策略为空，只在chance上执行tracer
                {

                    //Log("计算服务", "计划所执行的策略为空,为客户端人工指定计划准备。", currPlan.Plan_Name);
                    currPlan.PlanStrag.allowInvestmentMaxValue = currPlan.MaxLostAmount;
                    List<ChanceClass<T>> emptycs = CurrExistChanceList.Values.Where(p => p.StragId == null).ToList<ChanceClass<T>>();
                    for (int c = 0; c < emptycs.Count; c++)
                    {
                        ChanceClass<T> CurrCc = emptycs[c];
                        TraceChance<T> tcc = CurrCc as TraceChance<T>;
                        CurrCc.UnitCost = tcc.getChipAmount(GlobalClass.DefaultMaxLost, CurrCc, GlobalClass._DefaultHoldAmtSerials.Value);
                        //CurrCc.HoldTimeCnt = CurrCc.HoldTimeCnt + 1;
                        if (dtp.IsSecurityData == 0)
                        {
                            CurrCc.HoldTimeCnt = (int)DataReader<T>.getInterExpectCnt(CurrCc.ExpectCode, el.LastData.Expect, dtp) + 1;
                        }
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
                
                currStrag.RunningPlan = currPlan;//必须赋值，供后面证券策略使用
                currStrag.AssetUnitId = currPlan.AssetUnitInfo?.UnitId;//必须复制，后面交易单元要用
                if(dtp.IsSecurityData == 1)
                {
                    (currStrag as BaseSecurityStragClass<T>).setSingleDataFunc(getSingleData);//可穿透到最外面自由获取单个或多个codes的数据
                    (currStrag as BaseSecurityStragClass<T>).FillSecDic(allSecurityDic);
                }
                if (IsBackTest)
                {
                    currStrag.IsBackTest = IsBackTest;
                    currStrag.CommSetting.Odds = currPlan.AssetUnitInfo.Odds;//如果回测，可以用使用计划的赔率
                }
                if (currStrag is ReferIndexStragClass)//附加索引数据
                {
                    if (this.grpIndexs.ContainsKey(currStrag.GUID))
                    {
                        (currStrag as ReferIndexStragClass).Indexs = this.grpIndexs[currStrag.GUID];
                    }
                    else
                    {
                        (currStrag as ReferIndexStragClass).Indexs = new List<IndexExpectData>();
                    }
                    (currStrag as ReferIndexStragClass).ClearTmpData();
                }
                currStrag.SetLastUserData(el);//必须给策略填充数据
                currStrag.setDataTypePoint(dtp);
                ////////////////////////////////////////////////////////////////////
                ///
                ///
                ///////////////////////////////////////////////////////////////////
                currStrag.IsBackTest = IsBackTest;
                if(dtp.IsSecurityData == 1)
                {
                    (currStrag as BaseSecurityStragClass<T>).interProcessFinished((k) =>
                    {
                        this.afterStragProcessed?.Invoke(currStrag.StragScript, el.LastData.Expect, k);
                    });
                }
                List<ChanceClass<T>> cs = currStrag.getChances(cc, el.LastData,dtp.IsSecurityData==1);//获取该策略的机会
                if (currStrag is ReferIndexStragClass)
                {
                    if (grpIndexs == null)
                    {
                        grpIndexs = new Dictionary<string, List<IndexExpectData>>();
                    }
                    lock (grpIndexs)
                    {
                        if (grpIndexs.ContainsKey(currStrag.GUID))
                        {
                            grpIndexs[currStrag.GUID] = (currStrag as ReferIndexStragClass).Indexs;
                        }
                        else
                        {
                            grpIndexs.Add(currStrag.GUID, (currStrag as ReferIndexStragClass).Indexs);
                        }
                    }
                }
                if (currStrag is TotalStdDevTraceStragClass)//如果是整体标准差类，记录所有的标准差数据
                {
                    grpTotolStdDic = (currStrag as TotalStdDevTraceStragClass).getAllStdDev();
                }
                if (cs.Count > 0 && !IsBackTest)
                {
                    Log("计算服务", string.Format("策略[{0}/{1}/第{2}期]", currStrag.GUID, currStrag.StragScript, el.LastData.Expect), string.Format("取得机会数量为:{0};{1}", cs.Count, string.Join(",", cs.Select(a => string.Format("{0}/{1}", a.ChanceCode, a.UnitCost)))));
                    //wxl.Log("计算服务", string.Format("策略[{0}/{1}]", currStrag.GUID, currStrag.StragScript), string.Format("取得机会数量为:{0}", cs.Count));
                }

                Dictionary<string, ChanceClass<T>> StragChances = new Dictionary<string, ChanceClass<T>>();
                var items = CurrExistChanceList.Where(p => p.Value.StragId == currStrag.GUID);
                foreach(var item in items)
                {
                    if(!StragChances.ContainsKey(item.Value.ChanceCode))
                    {
                        StragChances.Add(item.Value.ChanceCode, item.Value);
                    }
                }
                //Log("计算服务", string.Format("当前策略:{0}", currStrag.StragScript), string.Format("当前策略对应的未关闭机会:{0}", string.Join(";",StragChances.Select(a=>a.Key).ToArray())));
                AmoutSerials amts = GlobalClass.getOptSerials(CurrSetting.Odds, currPlan.InitCash, 1);
                Int64 restAmt = currStrag.CommSetting.GetGlobalSetting().DefMaxLost;//初始资金
                if (currPlan.AssetUnitInfo != null)
                {
                    if (this.UseAssetUnits.ContainsKey(currPlan.AssetUnitInfo.UnitId))
                    {
                        AssetUnitClass<T> useUnit = UseAssetUnits[currPlan.AssetUnitInfo.UnitId];
                        if (!useUnit.Running)
                        {
                            useUnit.Run(dtp);
                        }
                        restAmt = (long)useUnit.getCurrExchangeServer().summary.currCash;
                    }
                    else
                        continue;
                }
                #region 遍历各新机会
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
                    if (StragChances.ContainsKey(CurrCc.ChanceCode))//未关闭的同策略id机会列表中存在该机会
                    {
                        if (dtp.IsSecurityData == 1)
                        {
                            continue;
                        }

                        ChanceClass<T> OldCc = StragChances[CurrCc.ChanceCode];
                        //Log("计算服务", "老机会信息", string.Format("idx:{0};holdcnt:{1}", OldCc.ChanceIndex, OldCc.HoldTimeCnt));
                        //Log("计算服务", "老记录", string.Format("上期相同的机会{0}", CurrCc.ChanceCode));
                        //Log("计算服务", "判断是否允许重复", currStrag.AllowRepeat.ToString());
                        if (!currStrag.AllowRepeat)//如果不允许重复
                        {
                            CurrCc = OldCc;
                            //CurrCc.HoldTimeCnt = CurrCc.HoldTimeCnt + 1;
                            if (dtp.IsSecurityData == 0)
                            {
                                CurrCc.HoldTimeCnt = (int)DataReader<T>.getInterExpectCnt(CurrCc.ExpectCode, el.LastData.Expect, dtp) + 1;
                                CurrCc.UnitCost = 0;
                            }
                            NeedUseOldData = true;
                            //Log("计算服务", "相同处理", string.Format("出现相同的机会{0},持有次数增1->{1}", CurrCc.ChanceCode, CurrCc.HoldTimeCnt));
                        }
                        else
                        {
                            //Log("计算服务", "策略允许重复！", currStrag.StragClassName);
                        }
                    }
                    else
                    {
                        Log("计算服务", string.Format("上期相同未关闭的机会数{0},{1}", string.Join(";", CurrExistChanceList.Select(a => a.Value.ChanceCode)), CurrCc.ChanceCode), "本期未出现");
                    }


                    //Log("计算服务", "再次检查数据", string.Format("出现相同的机会{0},持有次数增1->{1}", CurrCc.ChanceCode, CurrCc.HoldTimeCnt));
                    if (dtp.IsSecurityData == 0)
                        CurrCc.UnitCost = -1;//先默认为-1
                    if (currStrag is ISpecAmount<T>)//先从策略级别判断
                    {
                        ISpecAmount<T> testStrag = (currStrag as ISpecAmount<T>);
                        if (testStrag == null)
                        {
                            //等待下一步按机会级别判断
                        }
                        else
                        {
                            (testStrag as BaseStragClass<T>).allowInvestmentMaxValue = currPlan.MaxLostAmount;
                            if (dtp.IsSecurityData == 0)
                                CurrCc.UnitCost = testStrag.getChipAmount(restAmt, CurrCc, amts);
                            else
                            {
                                (testStrag as BaseSecurityStragClass<T>).FillSecDic(allSecurityDic);
                                //这句主要不是获得价格，而是为了开仓时获得开仓数量
                                (CurrCc as SecurityChance<T>).currUnitPrice = testStrag.getChipAmount(restAmt, CurrCc, amts);
                            }
                        }
                    }
                    if (CurrCc.UnitCost < 0)//如果策略级别未改变值
                    {
                        if (CurrCc.IsTracer == 1)//如果是自我追踪机会
                        {
                            //Log("计算服务", "自我跟踪机会，当前持有次数", string.Format("HoldTimes:{0}", CurrCc.HoldTimeCnt));
                            TraceChance<T> useCc = Convert.ChangeType(CurrCc, currStrag.getTheChanceType()) as TraceChance<T>;
                            //Log("计算服务", "使用的机会持有次数", string.Format("HoldTimes:{0}", useCc.HoldTimeCnt));
                            if (useCc == null) //获得的类型并非跟踪类型
                            {
                                CurrCc.UnitCost = (currStrag as ISpecAmount<T>).getChipAmount(restAmt, CurrCc, amts);
                            }
                            else
                            {
                                CurrCc.UnitCost = useCc.getChipAmount(restAmt, CurrCc, amts);
                            }
                        }
                        else//默认为ChanceTraceStragClass,其实是不可能触发的，而且会出错，因为ChanceTraceStragClass本身就是ispaceamount
                        {
                            //Log("计算服务", "非跟踪机会，持有次数", string.Format("HoldTimes:{0}", CurrCc.HoldTimeCnt));
                            CurrCc.UnitCost = (currStrag as ISpecAmount<T>).getChipAmount(restAmt, CurrCc, amts);
                        }
                    }

                    //Log("计算服务", "再二次检查数据", string.Format("出现相同的机会{0},持有次数增1->{1}", CurrCc.ChanceCode, CurrCc.HoldTimeCnt));
                    if (NeedUseOldData)//未关闭的及机会列表中存在该机会
                    {
                        //Log("计算服务", "策略不可以出现重复", string.Format("策略编号:{0};code:{1}", CurrCc.StragId,CurrCc.ChanceCode));
                        CurrCc.Cost += CurrCc.UnitCost * CurrCc.ChipCount;
                        CurrCc.UpdateTime = DateTime.Now;
                        if (!OldList.ContainsKey(CurrCc.GUID))
                            OldList.Add(CurrCc.GUID, CurrCc);
                        if (!IsBackTest)
                        {
                            if (!OldDbList.ContainsKey(CurrCc.ChanceIndex))
                                OldDbList.Add(CurrCc.ChanceIndex, CurrCc);
                        }
                        continue;

                    }
                    CurrCc.HoldTimeCnt = 1;
                    CurrCc.Cost = CurrCc.ChipCount * CurrCc.UnitCost;
                    if (dtp.IsSecurityData == 0)
                    {
                        CurrCc.Gained = 0;
                        CurrCc.Profit = 0;
                    }
                    else
                    {
                        //均已在检查是否需要关闭内处理，不再处理
                        //CurrCc.Gained = ((CurrCc as SecurityChance<T>).currUnitPrice - CurrCc.UnitCost) * CurrCc.ChipCount;
                        //CurrCc.Profit = CurrCc.Gained - CurrCc.Cost;
                    }
                    CurrCc.ExecDate = DateTime.Today;
                    CurrCc.CreateTime = DateTime.Now;
                    CurrCc.UpdateTime = CurrCc.CreateTime;
                    if (currStrag.CommSetting.Odds > 0)
                        CurrCc.Odds = currStrag.CommSetting.Odds;// CurrCc.getRealOdds();
                    CurrCc.StragId = currStrag.GUID;
                    //CurrCc.ExpectCode = el.LastData.Expect;
                    CurrCc.AllowMaxHoldTimeCnt = currPlan.AllowMaxHoldTimeCnt;
                    CurrCc.ChanceType = currPlan.OutPutType;
                    NewList.Add(CurrCc);
                }
                #endregion
                if (1==1)
                {
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
                        if (1 == 0)
                        {
                            double unitCost = getAmount(currStrag, CurrCc, el.LastData.Expect, restAmt, amts);
                            CurrCc.HoldTimeCnt = (int)DataReader<T>.getInterExpectCnt(CurrCc.ExpectCode, el.LastData.Expect, dtp) + 1;
                            CurrCc.UnitCost = unitCost;
                            CurrCc.Cost = CurrCc.ChipCount * CurrCc.UnitCost;
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
                            if (currStrag is ISpecAmount<T>)//先从策略级检查
                            {
                                ISpecAmount<T> specStrag = currStrag as ISpecAmount<T>;
                                if(dtp.IsSecurityData == 1)
                                {
                                    (specStrag as BaseSecurityStragClass<T>).FillSecDic(allSecurityDic);
                                }
                                if (specStrag != null)//如果没有方法，再从机会级检查
                                {
                                    //CurrCc.HoldTimeCnt++;
                                    if(dtp.IsSecurityData == 0)   CurrCc.HoldTimeCnt = (int)DataReader<T>.getInterExpectCnt(CurrCc.ExpectCode, el.LastData.Expect, dtp) + 1;
                                    CurrCc.UnitCost = specStrag.getChipAmount(restAmt, CurrCc, amts);
                                    CurrCc.Cost += CurrCc.ChipCount * CurrCc.UnitCost;
                                    CurrCc.UpdateTime = DateTime.Now;
                                    if (!OldList.ContainsKey(CurrCc.GUID))
                                        OldList.Add(CurrCc.GUID, CurrCc);
                                    if (!IsBackTest)
                                    {
                                        if (!OldDbList.ContainsKey(CurrCc.ChanceIndex))
                                            OldDbList.Add(CurrCc.ChanceIndex, CurrCc);
                                    }
                                    //Log("计算服务", "当前机会是可指定金额的策略", string.Format("策略名:{0};机会:{1},持有次数:{2},单位金额:{3}", currStrag.StragScript, CurrCc.ChanceCode, CurrCc.HoldTimeCnt, CurrCc.UnitCost));
                                    continue;
                                }
                                else
                                {
                                    Log("计算服务", "当前机会是本策略产生的，但本策略不是有能指定金额的策略，无法指定金额！", string.Format("策略名:{0};机会:{1}", currStrag.StragScript, CurrCc.ChanceCode));
                                }
                            }
                            if (CurrCc.Tracerable)//再检查机会级
                            {
                                Log("计算服务", "当前机会是跟踪策略", string.Format("策略名:{0};机会:{1}", currStrag.StragScript, CurrCc.ChanceCode));
                                //CurrCc.HoldTimeCnt++;
                                if(dtp.IsSecurityData == 0) CurrCc.HoldTimeCnt = (int)DataReader<T>.getInterExpectCnt(CurrCc.ExpectCode, el.LastData.Expect, dtp) + 1;
                                TraceChance<T> testCc = (TraceChance<T>)CurrCc;
                                if (testCc == null)
                                {
                                    Log("计算服务", "当前机会是跟踪策略，但是当前策略不是可指定策略！", string.Format("策略名:{0};机会:{1}", currStrag.StragScript, CurrCc.ChanceCode));
                                    continue;
                                }
                                CurrCc.UnitCost = testCc.getChipAmount(restAmt, CurrCc, amts);
                                CurrCc.Cost += CurrCc.ChipCount * CurrCc.UnitCost;
                                CurrCc.UpdateTime = DateTime.Now;
                                if (!OldList.ContainsKey(CurrCc.GUID))
                                    OldList.Add(CurrCc.GUID, CurrCc);
                                if (!IsBackTest)
                                {
                                    if (!OldDbList.ContainsKey(CurrCc.ChanceIndex))
                                        OldDbList.Add(CurrCc.ChanceIndex, CurrCc);
                                }

                                continue;
                            }
                            else
                            {
                                Log("计算服务", "当前机会是非跟踪策略，无需跟踪，但是我们还是跟踪了", string.Format("策略名:{0};机会:{1}", currStrag.StragScript, CurrCc.ChanceCode));
                                //CurrCc.HoldTimeCnt++;
                                if(dtp.IsSecurityData == 0)CurrCc.HoldTimeCnt = (int)DataReader<T>.getInterExpectCnt(CurrCc.ExpectCode, el.LastData.Expect, dtp) + 1;
                                ISpecAmount<T> Strag = (ISpecAmount<T>)currStrag;
                                if (Strag == null)
                                {
                                    Log("计算服务", "当前机会是跟踪策略，但是当前策略不是可指定策略！", string.Format("策略名:{0};机会:{1}", currStrag.StragScript, CurrCc.ChanceCode));
                                    continue;
                                }
                                CurrCc.UnitCost = Strag.getChipAmount(restAmt, CurrCc, amts);
                                CurrCc.Cost = CurrCc.ChipCount * CurrCc.UnitCost;
                                CurrCc.UpdateTime = DateTime.Now;
                                if (!OldList.ContainsKey(CurrCc.GUID))
                                    OldList.Add(CurrCc.GUID, CurrCc);
                                if (!IsBackTest)
                                {
                                    if (!OldDbList.ContainsKey(CurrCc.ChanceIndex))
                                        OldDbList.Add(CurrCc.ChanceIndex, CurrCc);
                                }
                                continue;
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion
            if (!IsBackTest)//额外保存
            {
                Log("计算服务", "保存机会", "开始！");
                DataReader<T> rd = DataReaderBuild.CreateReader<T>(this.dtp.DataType, null, null);
                int savecnt = rd.SaveChances(OldDbList.Values.ToList<ChanceClass<T>>(), null);
                //int savecnt = new PK10ExpectReader().SaveChances(OldDbList.Values.ToList<ChanceClass<T>>(), null);// OldDbList.Save(null);
                if (OldList.Count > 0)
                    Log("计算服务", "保存已有机会", string.Format("条数：{0};实际条数:{1}", OldList.Count, savecnt));
                //savecnt = new PK10ExpectReader().SaveChances(NewList, null);
                savecnt = rd.SaveChances(NewList, null);
                if (NewList.Count > 0)
                    Log("计算服务", "保存新增机会", string.Format("条数：{0};实际条数:{1}", NewList.Count, savecnt));
            }
            //合并到未关闭机会列表中
            //NewList.ForEach(p => AllNoClosedChances.Add(p.GUID, p));

            //OldList.Values.ToList<ChanceClass<T>>().ForEach(p => AllNoClosedChances.Add(p.GUID, p));//就算是老记录未有guid,当ToTable时已经生成了guid
            OldList.Values.ToList().ForEach(
                p=> {
                    if(!AllNoClosedChances.ContainsKey(p.GUID))
                    {
                        AllNoClosedChances.Add(p.GUID, p);
                    }
                });
            if (!IsBackTest)
            {
                OldDbList.Values.ToList().ForEach(a =>
                {
                    if (!AllNoClosedChances.ContainsKey(a.GUID))
                    {
                        AllNoClosedChances.Add(a.GUID, a);
                    }
                });
            }
            
            if(dtp.IsSecurityData==1)
            {
                //资产单元风险控制仓位，判定交易是否可以进行，超出部分直接过滤
                AssetRateControl(el, OldList, NewList);
                foreach (var item in UseStrags.Keys)//无论如何，都新增一次
                {
                    ExChange(new List<ChanceClass<T>>(), el.LastData.Expect);
                }
            }
            else
            {
                ExChange(AllNoClosedChances.Values.ToList(), el.LastData.Expect);//执行交易提供可视化
            }
        }
        /// <summary>
        /// 仓位控制模块，按安全垫所占资产比例提供相应的敞口规模，超出规模部分不交易
        /// </summary>
        /// <param name="el"></param>
        /// <param name="OldList"></param>
        /// <param name="NewList"></param>
        void AssetRateControl(ExpectList<T> el,Dictionary<string,ChanceClass<T>> OldList,List<ChanceClass<T>> NewList) 
        {
            var oldGrps = OldList.GroupBy(a => a.Value.StragId);
            var newGrps = NewList.GroupBy(a => a.StragId);
            List<string> secs = OldList.Select(a => a.Value.ChanceCode).ToList();
            MongoDataDictionary<T> alldata = new MongoDataDictionary<T>(el, true);
            string currExpect = el.LastData.Expect;
            foreach (var newItems in newGrps)//对每个策略进行计算
            {
                if (!UseStrags.ContainsKey(newItems.Key))//所有在用的策略不包含此策略，那就不处理
                {
                    continue;
                }
                if (newItems.Count() == 0)//分组没有内容
                {
                    continue;
                }
                var oldMatchItems = oldGrps.Where(a => a.Key == newItems.Key);
                BaseSecurityStragClass<T> sc = UseStrags[newItems.Key] as BaseSecurityStragClass<T>;
                StragRunPlanClass<T> usePlan = sc.RunningPlan as StragRunPlanClass<T>;
                if(usePlan == null)
                {
                    continue;
                }
                AssetUnitClass<T> useUnit = usePlan.AssetUnitInfo;
                if(useUnit == null)
                {
                    continue;
                }
                int noStopSecs = 0;//未停牌的股票数
                if (oldMatchItems.Count() > 0)
                {
                    var oldItems = oldMatchItems.First();
                    foreach (var oldItem in oldItems)//对单个策略中的老机会进行检查，是否是停牌股票
                    {
                        string code = oldItem.Value.ChanceCode;
                        if (!alldata.ContainsKey(code))
                            continue;
                        if (alldata[code].Last().Expect == currExpect)
                        {
                            noStopSecs++;
                        }
                    }
                }
                //所有的交易日数据
                ConcurrentDictionary<string,ExchangeData> allDayLines = useUnit.getCurrExchangeServer().getMoneys();
                double currTotalVal = useUnit.getCurrExchangeServer().InitCash;//默认当前资产及现金规模为初始资金
                if(allDayLines!= null&& allDayLines.Count>0)//如果有交易曲线，当前总值取交易曲线总值
                {
                    currTotalVal = allDayLines.OrderBy(a=>a.Key).Last().Value.currTotal;
                }
                double useAllowHoldMaxChanceRate = sc.getAllowMaxRiseExp(currTotalVal, useUnit.getCurrExchangeServer().InitCash);
                List<ChanceClass<T>> useList = new List<ChanceClass<T>>();
                int newI = 0;
                int MaxAllowHoldCnt = 0;
                //单利,等于敞口比例*总初始资金/单份金额
                if (newItems.First().IncrementType == InterestType.SimpleInterest)
                {
                    //单利，最大不能超过最大持仓数(初始资金/单份规模)
                    MaxAllowHoldCnt = (int)Math.Floor(usePlan.InitCash * Math.Max(1,useAllowHoldMaxChanceRate) / usePlan.FixAmt.Value);  
                }
                else//复利等于敞口比例/单份比例
                {
                    MaxAllowHoldCnt = (int)Math.Floor(useAllowHoldMaxChanceRate / (100*usePlan.FixRate.Value));
                }
                foreach (var newItem in newItems)
                {

                    newI++;
                    if (newI > MaxAllowHoldCnt - noStopSecs)
                    {
                        break;
                    }
                    useList.Add(newItem);
                    AllNoClosedChances.Add(newItem.GUID, newItem);
                }
                ExChange(useList, el.LastData.Expect);
            }

        }

        /// <summary>
        /// 记录交易，可以可视化查看交易结果及查看资金曲线，同时复利策略的计划必须绑定资产单元
        /// </summary>
        /// <param name="list"></param>
        /// <param name="newList"></param>
        /// <returns></returns>
        protected bool ExChange(List<ChanceClass<T>> list,string lastExpectNo)
        {
            
            ////List<ChanceClass> list = new List<ChanceClass>();
            ////Oldlist.Values.ToList<ChanceClass>().ForEach(p => list.Add(p));
            ////newList.ForEach(p => list.Add(p));
            //对所有记录都进行登记
            for (int i = 0; i < list.Count; i++)
            {
                ChanceClass<T> cc = list[i];
                BaseStragClass<T> sc = UseStrags[cc.StragId];
                if (sc.AssetUnitId == null)
                {
                    continue; //所属计划未指定资产单元，不记录交易信息
                }
                if(!UseAssetUnits.ContainsKey(sc.AssetUnitId))
                {
                    continue;//所属分类无记录的资产单元，不记录信息
                }
                if (cc.UnitCost == 0)//无交易金额，不处理
                {
                        continue;
                }
                AssetUnitClass<T> uu = UseAssetUnits[sc.AssetUnitId];
                ExchangeChance<T> ec = new ExchangeChance<T>(uu.getCurrExchangeServer(), sc,cc.ExpectCode, lastExpectNo, cc);
                
                ec.ExchangeAmount = cc.UnitCost;
                if(dtp.IsSecurityData == 1)
                {
                    ec.ExchangeAmount = (cc as SecurityChance<T>).openPrice;
                    cc.UnitCost = ec.ExchangeAmount;
                    cc.Cost = cc.UnitCost * cc.ChipCount;
                    
                }
                ec.ExchangeRate = cc.UnitCost / uu.getCurrExchangeServer().summary.currTotal;
                if (!string.IsNullOrEmpty(ec.OwnerChance.ChanceCode))
                {
                    uu.getCurrExchangeServer().Push(ref ec, false, dtp.IsSecurityData == 1, IsBackTest);
                }
            }
            if (dtp.IsSecurityData == 1 && list.Count == 0)
            {
                foreach (var sc in UseStrags.Values)
                {
                    if (!UseAssetUnits.ContainsKey(sc.AssetUnitId))
                    {
                        continue;//所属分类无记录的资产单元，不记录信息
                    }
                    AssetUnitClass<T> uu = UseAssetUnits[sc.AssetUnitId];
                    SecurityChance<T> cc = new SecurityChance<T>();
                    cc.ExpectCode = lastExpectNo;
                    cc.ChipCount = 1;
                    cc.UnitCost = 0;
                    ExchangeChance<T> ec = new ExchangeChance<T>(uu.getCurrExchangeServer(), sc, cc.ExpectCode, lastExpectNo, cc);
                    uu.getCurrExchangeServer().Push(ref ec,true,true,IsBackTest);//空记录
                }
            }
            return true;
        }

        protected bool CloseAllExchance(ExpectList<T> el)
        {
            //Dictionary<string, ChanceClass<T>> tmp = new Dictionary<string, ChanceClass<T>>();
            //CurrExistChanceList.Keys.ToList().ForEach(a => tmp.Add(a, CurrExistChanceList[a]));
            //Dictionary<string, ExchangeChance<T>> dic = AllExchance.ToDictionary(a => a.OwnerChance.GUID, a => a);
            //CurrExistChanceList = new Dictionary<string, ChanceClass<T>>();
            //tmp.ToList().ForEach(a =>
            //{
            //    if (!dic.ContainsKey(a.Key))
            //    {
            //        CurrExistChanceList.Add(a.Key, a.Value);
            //    }
            //});
            for (int i = 0; i < AllExchance.Count; i++)
            {
                ExchangeChance<T> ec = AllExchance[i];
                int MatchCnt = 0;
                ec.OwnerChance.Matched(el.LastData, out MatchCnt);
                ec.MatchChips = MatchCnt;
                ec.OwnerChance.MatchChips = MatchCnt;
                if (dtp.IsSecurityData == 1)
                {
                    ec.Server.UpdateSecurity(ec,true, "清仓", 0);
                }
                else
                {
                    ec.Server.Update(ec);
                }
                //if (MatchCnt == 0)
                //{
                //    if (tmp.ContainsKey(ec.OwnerChance.GUID) && !CurrExistChanceList.ContainsKey(ec.OwnerChance.GUID))
                //        CurrExistChanceList.Add(ec.OwnerChance.GUID, ec.OwnerChance);
                //}
            }
            AllExchance = new List<ExchangeChance<T>>();//全部清空
            return true;
        }
        

        public CalcStragGroupClass<T> Copy()
        {
            CalcStragGroupClass<T> ret = new CalcStragGroupClass<T>(this.dtp);
            ret.Finished = this.Finished;
            ret.UseSPlans = getObjectListByXml<StragRunPlanClass<T>>(getXmlByObjectList<StragRunPlanClass<T>>(this.UseSPlans));
            ret.UseStrags = getObjectListByXml<BaseStragClass<T>>(getXmlByObjectList<BaseStragClass<T>>(UseStrags.Values.ToList<BaseStragClass<T>>())).ToDictionary(p=>p.GUID,p=>p);
            ret.CurrSetting = this.CurrSetting;
            ret.UseStragType = this.UseStragType;
            ret.allSecurityDic = this.allSecurityDic;
            return ret;
        }

        double getAmount(BaseStragClass<T> currStrag, ChanceClass<T> CurrCc,string lastExpect,double restAmt, AmoutSerials amts)
        {
            if (currStrag is ISpecAmount<T>)//先从策略级检查
            {
                ISpecAmount<T> specStrag = currStrag as ISpecAmount<T>;
                if (specStrag != null)//如果没有方法，再从机会级检查
                {
                    return specStrag.getChipAmount(restAmt, CurrCc, amts);
                }
                else
                {
                    //Log("计算服务", "当前机会是本策略产生的，但本策略不是有能指定金额的策略，无法指定金额！", string.Format("策略名:{0};机会:{1}", currStrag.StragScript, CurrCc.ChanceCode));
                    return 0;
                }
            }
            if (CurrCc.Tracerable)//再检查机会级
            {
                //Log("计算服务", "当前机会是跟踪策略", string.Format("策略名:{0};机会:{1}", currStrag.StragScript, CurrCc.ChanceCode));
                ////CurrCc.HoldTimeCnt++;
                //CurrCc.HoldTimeCnt = (int)DataReader.getInterExpectCnt(CurrCc.ExpectCode, el.LastData.Expect, dtp) + 1;
                TraceChance<T> testCc = (TraceChance<T>)CurrCc;
                return testCc.getChipAmount(restAmt, CurrCc, amts);
                //continue;
            }
            else
            {
                //Log("计算服务", "当前机会是非跟踪策略，无需跟踪，但是我们还是跟踪了", string.Format("策略名:{0};机会:{1}", currStrag.StragScript, CurrCc.ChanceCode));
                ISpecAmount<T> Strag = (ISpecAmount<T>)currStrag;
                return Strag.getChipAmount(restAmt, CurrCc, amts);
            }
            return 0;
        }
    }

}

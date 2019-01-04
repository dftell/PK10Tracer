using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strags;
using PK10CorePress;
using ExchangeLib;
using System.Data;
using DataRecSvr;
using ServerInitLib;
using System.Threading;
namespace BackTestLib
{
    public delegate void SuccEvent();
        
    public class BackTestClass
    {
        long BegExpect;
        long LoopCnt;
        double Odds;
        SettingClass CurrSetting;
        public long testIndex = 0;
        public BackTestReturnClass ret = new BackTestReturnClass();
        public StragClass teststrag;
        public DataTable SystemStdDevs = new DataTable();
        public SuccEvent FinishedProcess; 
        public BackTestClass(long From, long buffCnt,SettingClass setting)
        {
            BegExpect = From;
            LoopCnt = buffCnt;
            CurrSetting = setting;
            testIndex = 0;
        }

        public void  Run()
        {
            LoopCnt = 0;
            testIndex = 0;
            ret = new BackTestReturnClass();
            long begNo = BegExpect;

            ExpectReader er = new ExpectReader();
            ExpectList el = null;
            long cnt = 0;
            
            ret.HoldCntDic = new Dictionary<int, int>();
            ret.HoldWinCntDic = new Dictionary<int, int>();
            ret.InChipsDic = new Dictionary<int, int>();
            ret.WinChipsDic = new Dictionary<int, int>();

            ExpectList AllData = new ExpectList();
            testIndex = teststrag.ReviewExpectCnt-1;
            ExpectList testData = null;
            Dictionary<string, ChanceClass> NoCloseChances = new Dictionary<string, ChanceClass>();
            Dictionary<string, ChanceClass> tmpChances = new Dictionary<string, ChanceClass>();
            
            while (el == null || el.Count > 0) //如果取到的数据长度大于0
            {
                el = er.ReadHistory(begNo, LoopCnt);
                if (el == null)
                {
                    ret.LoopCnt = cnt * LoopCnt;
                    ret.succ = false;
                    ret.Msg = "读取历史数据错误！";
                    break;
                }
                if (el.Count == 0)
                {
                    ret.LoopCnt = testIndex;
                    ret.succ = true;
                    ret.Msg = string.Format("成功遍历{0}条记录！共发现机会{1}次！其中,{2}.", testIndex, ret.ChanceList.Count,ret.HoldInfo);
                    break;
                }
                AllData = ExpectList.Concat(AllData, el);
                begNo = el.LastData.LExpectNo + 1;

                cnt++;
                //Todo:

                while (testIndex < AllData.Count)
                {
                    if (testData == null)
                    {
                        testData = AllData.getSubArray(0, teststrag.ReviewExpectCnt);
                    }
                    else
                    {
                        if (AllData[(int)testIndex].ExpectIndex != testData.LastData.ExpectIndex + 1)
                        {
                            throw new Exception(string.Format("{1}第{0}期后出现数据遗漏，请补充数据后继续测试！", testData.LastData.Expect, testData.LastData.OpenTime));
                        }
                        testData.RemoveAt(0);
                        testData.Add(AllData[(int)testIndex]);
                    }
                    tmpChances = new Dictionary<string, ChanceClass>();
                    foreach (string key in NoCloseChances.Keys)
                    {
                        ChanceClass cc = NoCloseChances[key];
                        if (cc.Closed == false)
                        {
                            int matchcnt = 0;
                            if (teststrag.GetRev)//如果求相反组合
                            {
                                if (cc.Matched(testData.LastData, out matchcnt, true))//不关闭
                                {
                                    if (cc.HoldTimeCnt < 0)
                                    {
                                        cc.HoldTimeCnt = (int)(testData.LastData.ExpectIndex - cc.InputExpect.ExpectIndex);
                                    }
                                }
                            }
                            bool Matched = cc.Matched(testData.LastData, out matchcnt, false);
                            if (cc.NeedConditionEnd)
                            {
                                cc.MatchChips += matchcnt;
                                if (Matched)
                                {
                                    int LastMatchId = cc.LastMatchTimesId;//最后一次匹配次序号
                                    int maxHoldCnt = cc.MaxHoldTimeCnt;
                                    if (cc.HoldTimeCnt - cc.LastMatchTimesId > maxHoldCnt )
                                    {
                                        cc.MaxHoldTimeCnt = cc.HoldTimeCnt - cc.LastMatchTimesId;
                                    }
                                    cc.LastMatchTimesId = cc.HoldTimeCnt;
                                }
                                if (teststrag is ITraceChance)
                                {
                                    ITraceChance its = teststrag as ITraceChance;
                                    if (its == null)
                                        cc.Closed = cc.OnCheckTheChance(cc, Matched);
                                    else
                                        cc.Closed = its.CheckNeedEndTheChance(cc, Matched);
                                }
                                else
                                {
                                    cc.Closed = cc.OnCheckTheChance(cc, Matched);
                                }
                                if (cc.Closed)
                                {
                                    cc.EndExpectNo = testData.LastData.Expect;
                                    cc.UpdateTime = testData.LastData.OpenTime;
                                }
                                else
                                {
                                    cc.HoldTimeCnt++;
                                    tmpChances.Add(key, cc);
                                }
                            }
                            else
                            {
                                if (Matched || (cc.HoldTimeCnt>0 && cc.HoldTimeCnt == cc.AllowMaxHoldTimeCnt))//关闭
                                {
                                    cc.Closed = true;
                                    cc.EndExpectNo = testData.LastData.Expect;
                                    cc.MatchChips = matchcnt;
                                    if (!teststrag.GetRev)//只有不求相反值的情况下，才赋持有是次数
                                    {
                                        cc.HoldTimeCnt = (int)(testData.LastData.ExpectIndex - cc.InputExpect.ExpectIndex);
                                    }
                                    else
                                    {
                                        if (cc.HoldTimeCnt < 0)
                                        {
                                            cc.HoldTimeCnt = 999;
                                        }
                                    }
                                    cc.UpdateTime = testData.LastData.OpenTime;
                                    ret.ChanceList.Add(cc);
                                    
                                    
                                }
                                else
                                {
                                    cc.HoldTimeCnt++;
                                    tmpChances.Add(key, cc);
                                }
                            }
                            if (cc.Closed)
                            {
                                int HCnt = 1;
                                if (ret.HoldCntDic.ContainsKey(cc.HoldTimeCnt))
                                {
                                    HCnt = ret.HoldCntDic[cc.HoldTimeCnt];
                                    HCnt++;
                                    ret.HoldCntDic[cc.HoldTimeCnt] = HCnt;
                                    ret.HoldWinCntDic[cc.HoldTimeCnt] = ret.HoldWinCntDic[cc.HoldTimeCnt] + matchcnt;
                                    ret.InChipsDic[cc.HoldTimeCnt] = ret.InChipsDic[cc.HoldTimeCnt] + cc.ChipCount;
                                    ret.WinChipsDic[cc.HoldTimeCnt] = ret.WinChipsDic[cc.HoldTimeCnt] + cc.MatchChips;
                                }
                                else
                                {
                                    ret.HoldCntDic.Add(cc.HoldTimeCnt, 1);
                                    ret.HoldWinCntDic.Add(cc.HoldTimeCnt, matchcnt);
                                    ret.InChipsDic.Add(cc.HoldTimeCnt, cc.ChipCount);
                                    ret.WinChipsDic.Add(cc.HoldTimeCnt, cc.MatchChips);
                                }
                               
                            }
                        }
                    }
                    CommCollection sc = new ExpectListProcess(testData).getSerialData(teststrag.ReviewExpectCnt, teststrag.BySer);
                    if (testData.Count == 0) 
                        break;
                    teststrag.SetLastUserData(testData);
                    List<ChanceClass> cs = teststrag.getChances(sc,testData.LastData);//获取所有机会
                    if (ret.ChanceList == null)
                    {
                        ret.ChanceList = new List<ChanceClass>();
                    }
                    //ret.ChanceList.AddRange(cs);
                    NoCloseChances = new Dictionary<string, ChanceClass>();
                    foreach(string key in tmpChances.Keys)
                    {
                        ChanceClass cc = tmpChances[key];
                        NoCloseChances.Add(key, cc);
                    }
                    for (int i = 0; i < cs.Count; i++)
                    {
                        //string key = string.Format("{0}_{1}", cs[i].SignExpectNo, cs[i].ChanceCode);
                        string key = string.Format("{0}", cs[i].ChanceCode);
                        if (NoCloseChances.ContainsKey(key))
                        {
                            if (teststrag.AllowRepeat)
                            {
                                string test = key;
                                //NoCloseChances.Add(key, cs[i]);
                            }
                        }
                        else
                        {
                            NoCloseChances.Add(key, cs[i]);
                        }
                    }
                    testIndex++;
                }
            }
            FinishedProcess();
            //return ret;
        }

        /// <summary>
        /// 新逻辑调用服务的计算类，注入策略运行计划清单和数据
        /// </summary>
        /// <param name="es"></param>
        /// <param name="teststragplans"></param>
        /// <returns></returns>
        public BackTestReturnClass VirExchange(ServiceSetting sc,ref ExchangeService es, StragRunPlanClass[] teststragplans)
        {
            LoopCnt = 0;
            testIndex = 0;
            //调用计算服务进行计算
            if (!teststragplans[0].AssetUnitInfo.Running) //如果资产单元没有启动，启动资产单元
                teststragplans[0].AssetUnitInfo.Run();
             es = teststragplans[0].AssetUnitInfo.ExchangeServer;//设置资产单元的模拟交易器
            CalcService cs = new CalcService(true,sc,teststragplans.ToDictionary(t=>t.GUID,t=>t));
            cs.IsTestBack = true;
            
            long begNo = BegExpect;
            ExpectReader er = new ExpectReader();
            ExpectList el = null;
            long cnt = 0;
            BackTestReturnClass ret = new BackTestReturnClass();
            ret.HoldCntDic = new Dictionary<int, int>();
            ret.HoldWinCntDic = new Dictionary<int, int>();
            ret.InChipsDic = new Dictionary<int, int>();
            ret.WinChipsDic = new Dictionary<int, int>();

            ExpectList AllData = new ExpectList();
            //long testIndex = teststrag.ReviewExpectCnt - 1;
            StragClass[] teststrags = teststragplans.Select(p => p.PlanStrag).ToArray<StragClass>();
            testIndex = teststrags.Max<StragClass>(s => s.ReviewExpectCnt);//取所有策略中回览期最大的开始，之前的数据不看
            long InitIndex = testIndex;
            ExpectList testData = null;
            ////Dictionary<string, StragChance> NoCloseChances = new Dictionary<string, StragChance>();
            ////Dictionary<string, StragChance> tmpChances = new Dictionary<string, StragChance>();
            ////Dictionary<Int64, ExchangeChance> NewExchangeRecord = new Dictionary<Int64, ExchangeChance>();
            int AllCnt = 0;
            while (el == null || el.Count > 0) //如果取到的数据长度大于0
            {
                el = er.ReadHistory(begNo, LoopCnt);
                if (el == null)
                {
                    ret.LoopCnt = cnt * LoopCnt;
                    ret.succ = false;
                    ret.Msg = "读取历史数据错误！";
                    break;
                }
                if (el.Count == 0)
                {
                    ret.LoopCnt = testIndex;
                    ret.succ = true;
                    //ret.Msg = string.Format("成功遍历{0}条记录！共发现机会{1}次！其中,{2}.", testIndex, ret.ChanceList.Count, ret.HoldInfo);
                    break;
                }
                AllData = ExpectList.Concat(AllData, el);
                begNo = el.LastData.LExpectNo + 1;

                cnt++;
                //Todo:

                while (testIndex < AllData.Count)
                {
                    int CurrExpectClose = 0;
                    AllCnt++;
                    es.UpdateExpectCnt(AllCnt);
                    if (testData == null)
                    {
                        //testData = AllData.getSubArray(0, teststrag.ReviewExpectCnt);
                        testData = AllData.getSubArray(0, (int)InitIndex + 1);
                    }
                    else
                    {
                        if (AllData[(int)testIndex].ExpectIndex != testData.LastData.ExpectIndex + 1)
                        {
                            throw new Exception(string.Format("{1}第{0}期后出现数据遗漏，请补充数据后继续测试！", testData.LastData.Expect, testData.LastData.OpenTime));
                        }
                        testData.RemoveAt(0);
                        testData.Add(AllData[(int)testIndex]);
                    }
                    //只是取数据的逻辑一样，以后均调用CalcService
                    //ToAdd:以下是内容
                    cs.CurrData = testData;
                    cs.Calc();
                    while (!cs.CalcFinished)
                    {
                        Thread.Sleep(100);
                    }
                    this.SystemStdDevs = cs.getSystemStdDevList();
                    testIndex++;
                }
            }
            FinishedProcess();
            return ret;
        }
        
        public BackTestReturnClass VirExchange_oldLogic(ExchangeService es, StragRunPlanClass[] teststragplans)
        {
            long begNo = BegExpect;
            ExpectReader er = new ExpectReader();
            ExpectList el = null;
            long cnt = 0;
            BackTestReturnClass ret = new BackTestReturnClass();
            ret.HoldCntDic = new Dictionary<int, int>();
            ret.HoldWinCntDic = new Dictionary<int, int>();
            ret.InChipsDic = new Dictionary<int, int>();
            ret.WinChipsDic = new Dictionary<int, int>();

            ExpectList AllData = new ExpectList();
            //long testIndex = teststrag.ReviewExpectCnt - 1;
            StragClass[] teststrags = teststragplans.Select(p => p.PlanStrag).ToArray<StragClass>();
            long testIndex = teststrags.Max<StragClass>(s => s.ReviewExpectCnt);//取所有策略中回览期最大的开始，之前的数据不看
            long InitIndex = testIndex;
            ExpectList testData = null;
            Dictionary<string, StragChance> NoCloseChances = new Dictionary<string, StragChance>();
            Dictionary<string, StragChance> tmpChances = new Dictionary<string, StragChance>();
            Dictionary<Int64, ExchangeChance> NewExchangeRecord = new Dictionary<Int64, ExchangeChance>();
            int AllCnt = 0;
            while (el == null || el.Count > 0) //如果取到的数据长度大于0
            {
                el = er.ReadHistory(begNo, LoopCnt);
                if (el == null)
                {
                    ret.LoopCnt = cnt * LoopCnt;
                    ret.succ = false;
                    ret.Msg = "读取历史数据错误！";
                    break;
                }
                if (el.Count == 0)
                {
                    ret.LoopCnt = testIndex;
                    ret.succ = true;
                    ret.Msg = string.Format("成功遍历{0}条记录！共发现机会{1}次！其中,{2}.", testIndex, ret.ChanceList.Count, ret.HoldInfo);
                    break;
                }
                AllData = ExpectList.Concat(AllData, el);
                begNo = el.LastData.LExpectNo + 1;

                cnt++;
                //Todo:

                while (testIndex < AllData.Count)
                {
                    int CurrExpectClose = 0;
                    AllCnt++;
                    es.UpdateExpectCnt(AllCnt);
                    if (testData == null)
                    {
                        //testData = AllData.getSubArray(0, teststrag.ReviewExpectCnt);
                        testData = AllData.getSubArray(0, (int)InitIndex+1);
                    }
                    else
                    {
                        if (AllData[(int)testIndex].ExpectIndex != testData.LastData.ExpectIndex + 1)
                        {
                            throw new Exception(string.Format("{1}第{0}期后出现数据遗漏，请补充数据后继续测试！", testData.LastData.Expect, testData.LastData.OpenTime));
                        }
                        testData.RemoveAt(0);
                        testData.Add(AllData[(int)testIndex]);
                    }
                    for (int i = 0; i < teststrags.Length; i++)//专门针对需要程序话关闭机会，且关闭时需要知道当前数据策略使用
                    {
                        teststrags[i].SetLastUserData(testData);
                    }
                    tmpChances = new Dictionary<string, StragChance>();

                    //关闭所有交易
                    foreach (int id in NewExchangeRecord.Keys)
                    {
                        ExchangeChance ec = NewExchangeRecord[id];
                        int matchcnt = 0;
                        ec.OwnerChance.Matched(testData.LastData, out matchcnt, false);
                        ec.MatchChips = matchcnt;
                        es.Update(ec);
                        ec = null;
                    }
                    NewExchangeRecord = new Dictionary<Int64, ExchangeChance>();

                    foreach (string key in NoCloseChances.Keys)
                    {
                        StragChance scc = NoCloseChances[key];
                        ChanceClass cc = scc.Chance;
                        if (cc.Closed == false)
                        {
                            int matchcnt = 0;
                            //////if (teststrag.GetRev)//如果求相反组合
                            //////{
                            //////    if (cc.Matched(testData.LastData, out matchcnt, true))//不关闭
                            //////    {
                            //////        if (cc.HoldTimeCnt < 0)
                            //////        {
                            //////            cc.HoldTimeCnt = (int)(testData.LastData.ExpectIndex - cc.InputExpect.ExpectIndex);
                            //////        }
                            //////    }
                            //////}
                            bool Matched = cc.Matched(testData.LastData, out matchcnt, false);
                            if (cc.NeedConditionEnd) //需要策略自定义条件结束
                            {
                                cc.MatchChips += matchcnt;
                                if (Matched)//匹配到了
                                {
                                    int LastMatchId = cc.LastMatchTimesId;//最后一次匹配次序号
                                    int maxHoldCnt = cc.MaxHoldTimeCnt;
                                    if (cc.HoldTimeCnt - cc.LastMatchTimesId > maxHoldCnt)
                                    {
                                        cc.MaxHoldTimeCnt = cc.HoldTimeCnt - cc.LastMatchTimesId;
                                    }
                                    cc.LastMatchTimesId = cc.HoldTimeCnt;
                                }
                                if (CurrExpectClose==1)//如果当期已关闭，后面所有机会均关闭
                                {
                                    cc.Closed = true;
                                }
                                else if(CurrExpectClose == -1)
                                {
                                    cc.Closed = Matched;
                                }
                                else
                                {
                                    cc.Closed = cc.OnCheckTheChance(cc, Matched);
                                    if (teststrags[0].StagSetting.IsLongTermCalc)//如果是长期计算，设置当期是否关闭
                                    {
                                        if (!Matched && cc.Closed)//匹配和状态相背，一定是状态已关闭
                                        {
                                            CurrExpectClose = 1;
                                        }
                                        if (!Matched && !cc.Closed)//第一次非匹配状态能判断出当期是否关闭
                                        {
                                            CurrExpectClose = -1;
                                        }
                                    }
                                }
                                if (cc.Closed)
                                {
                                    cc.EndExpectNo = testData.LastData.Expect;
                                    cc.UpdateTime = testData.LastData.OpenTime;
                                }
                                else
                                {
                                    cc.HoldTimeCnt++;
                                    tmpChances.Add(key, scc);
                                }
                            }
                            else
                            {
                                if (Matched || cc.HoldTimeCnt == cc.AllowMaxHoldTimeCnt)//关闭
                                {
                                    cc.Closed = true;
                                    cc.EndExpectNo = testData.LastData.Expect;
                                    cc.MatchChips = matchcnt;
                                    //////if (!teststrag.GetRev)//只有不求相反值的情况下，才赋持有是次数
                                    //////{
                                    cc.HoldTimeCnt = (int)(testData.LastData.ExpectIndex - cc.InputExpect.ExpectIndex);
                                    //////}
                                    //////else
                                    //////{
                                    //////    if (cc.HoldTimeCnt < 0)
                                    //////    {
                                    //////        cc.HoldTimeCnt = 999;
                                    //////    }
                                    //////}
                                    cc.UpdateTime = testData.LastData.OpenTime;
                                    ret.ChanceList.Add(cc);
                                }
                                else
                                {
                                    cc.HoldTimeCnt++;
                                    tmpChances.Add(key, scc);
                                }
                            }
                            #region 
                            //////////if (cc.Closed)
                            //////////{
                            //////////    int HCnt = 1;
                            //////////    if (cc.NeedConditionEnd)
                            //////////    {
                            //////////        if (ret.HoldCntDic.ContainsKey(cc.MaxHoldTimeCnt))
                            //////////        {
                            //////////            HCnt = ret.HoldCntDic[cc.MaxHoldTimeCnt];
                            //////////            HCnt++;
                            //////////            ret.HoldCntDic[cc.MaxHoldTimeCnt] = HCnt;
                            //////////            ret.HoldWinCntDic[cc.MaxHoldTimeCnt] = ret.HoldWinCntDic[cc.MaxHoldTimeCnt] + matchcnt;
                            //////////            ret.InChipsDic[cc.MaxHoldTimeCnt] = ret.InChipsDic[cc.MaxHoldTimeCnt] + cc.ChipCount * cc.HoldTimeCnt;
                            //////////            ret.WinChipsDic[cc.MaxHoldTimeCnt] = ret.WinChipsDic[cc.MaxHoldTimeCnt] + cc.MatchChips;
                            //////////        }
                            //////////        else
                            //////////        {
                            //////////            ret.HoldCntDic.Add(cc.MaxHoldTimeCnt, 1);
                            //////////            ret.HoldWinCntDic.Add(cc.MaxHoldTimeCnt, matchcnt);
                            //////////            ret.InChipsDic.Add(cc.MaxHoldTimeCnt, cc.ChipCount * cc.HoldTimeCnt);
                            //////////            ret.WinChipsDic.Add(cc.MaxHoldTimeCnt, cc.MatchChips);
                            //////////        }
                            //////////    }
                            //////////    else
                            //////////    {
                            //////////        if (ret.HoldCntDic.ContainsKey(cc.HoldTimeCnt))
                            //////////        {
                            //////////            HCnt = ret.HoldCntDic[cc.HoldTimeCnt];
                            //////////            HCnt++;
                            //////////            ret.HoldCntDic[cc.HoldTimeCnt] = HCnt;
                            //////////            ret.HoldWinCntDic[cc.HoldTimeCnt] = ret.HoldWinCntDic[cc.HoldTimeCnt] + matchcnt;
                            //////////            ret.InChipsDic[cc.HoldTimeCnt] = ret.InChipsDic[cc.HoldTimeCnt] + cc.ChipCount;
                            //////////            ret.WinChipsDic[cc.HoldTimeCnt] = ret.WinChipsDic[cc.HoldTimeCnt] + cc.MatchChips;
                            //////////        }
                            //////////        else
                            //////////        {
                            //////////            ret.HoldCntDic.Add(cc.HoldTimeCnt, 1);
                            //////////            ret.HoldWinCntDic.Add(cc.HoldTimeCnt, matchcnt);
                            //////////            ret.InChipsDic.Add(cc.HoldTimeCnt, cc.ChipCount);
                            //////////            ret.WinChipsDic.Add(cc.HoldTimeCnt, cc.MatchChips);
                            //////////        }
                            //////////    }
                            //////////}
                            #endregion
                        }
                    }

                    List<StragChance> cs = new List<StragChance>();
                    for (int i = 0; i < teststrags.Length; i++)
                    {
                        CommCollection sc = new ExpectListProcess(testData).getSerialData(teststrags[i].ReviewExpectCnt, teststrags[i].BySer);
                        if (testData.Count == 0)
                            break;
                        List<ChanceClass> scs = teststrags[i].getChances(sc, testData.LastData);//获取所有机会
                        for (int j = 0; j < scs.Count; j++)
                        {
                            ChanceClass CurrCc = scs[j];
                            ////scs[j].IncrementType = teststragplans[i].IncreamType;
                            ////scs[j].FixAmt = teststragplans[i].FixAmt;
                            ////scs[j].FixRate = teststragplans[i].FixRate;
                            StragRunPlanClass currPlan = teststragplans[i];
                            StragClass currStrag = currPlan.PlanStrag;
                            CurrCc.HoldTimeCnt = 1;
                            CurrCc.Cost = CurrCc.ChipCount * CurrCc.UnitCost;
                            CurrCc.Gained = 0;
                            CurrCc.Profit = 0;
                            CurrCc.ExecDate = DateTime.Today;
                            CurrCc.CreateTime = el.LastData.OpenTime;
                            CurrCc.UpdateTime = CurrCc.CreateTime;
                            CurrCc.StragId = currStrag.GUID;
                            CurrCc.ExpectCode = el.LastData.Expect;
                            CurrCc.MaxHoldTimeCnt = currPlan.AllowMaxHoldTimeCnt;
                            CurrCc.FixAmt = currPlan.FixAmt;
                            CurrCc.FixRate = currPlan.FixRate;
                            CurrCc.IncrementType = currPlan.IncreamType;
                            cs.Add(new StragChance(teststrags[i], CurrCc));
                        }
                    }
                    if (ret.ChanceList == null)
                    {
                        ret.ChanceList = new List<ChanceClass>();
                    }
                    //ret.ChanceList.AddRange(cs);
                    NoCloseChances = new Dictionary<string, StragChance>();
                    foreach (string key in tmpChances.Keys)
                    {
                        StragChance scc = tmpChances[key];
                        ChanceClass cc = scc.Chance;
                        NoCloseChances.Add(key, scc);
                        //////ProbWaveSelectStragClass组合改为统一交易
                        ////if ((scc.Strag is ProbWaveSelectStragClass) == false)
                        ////{
                        ////    ExchangeChance ec = new ExchangeChance(scc.Strag, testData.LastData.Expect, cc);
                        ////    bool Suc = es.Push(ref ec);
                        ////    if (Suc)
                        ////        NewExchangeRecord.Add(ec.Id, ec);
                        ////}
                    }
                    tmpChances = null;
                    //如果设置了最大持仓，必须按照最大持仓限制下注。
                    for (int i = 0; i < Math.Min(cs.Count,teststrags[0].CommSetting.MaxHoldingCnt-NoCloseChances.Count); i++)
                    {
                        //string key = string.Format("{0}_{1}", cs[i].SignExpectNo, cs[i].ChanceCode);
                        string key = string.Format("{0}", cs[i].Chance.ChanceCode);
                        
                        if (NoCloseChances.ContainsKey(key))
                        {
                            //////if (teststrag.AllowRepeat)
                            //////{
                            //////    string test = key;
                            //////    //NoCloseChances.Add(key, cs[i]);
                            //////}
                        }
                        else
                        {
                            cs[i].Chance.BaseAmount = es.summary<es.InitCash?1:es.summary/es.InitCash;
                            NoCloseChances.Add(key,cs[i]);
                            ////////ProbWaveSelectStragClass组合改为统一交易
                            //////if ((cs[i].Strag is ProbWaveSelectStragClass)==false)
                            //////{
                            //////    ExchangeChance ec = new ExchangeChance(cs[i].Strag, testData.LastData.Expect, cs[i].Chance);//交易
                            //////    bool Suc = es.Push(ref ec);
                            //////    if (Suc)
                            //////        NewExchangeRecord.Add(ec.Id, ec);
                            //////}
                        }
                        
                    }
                    //if ((cs[0].Strag is ProbWaveSelectStragClass) == false)
                    //{
                    foreach (string key in NoCloseChances.Keys)
                    {

                        ExchangeChance ec = new ExchangeChance(es, NoCloseChances[key].Strag, testData.LastData.Expect, NoCloseChances[key].Chance);//交易
                        if (ec.OccurStrag is ProbWaveSelectStragClass)//对于ProbWaveSelectStragClass，一开始就计算好了Amount
                        {
                            ProbWaveSelectStragClass strag = ec.OccurStrag as ProbWaveSelectStragClass;
                            if (!strag.UseAmountList().ContainsKey(testData.LastData.Expect))
                            {
                                Int64 AllAmt = (ec.OccurStrag as ChanceTraceStragClass).getChipAmount(es.summary, ec.OwnerChance, ec.OccurStrag.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials);
                                Int64 ChipAmt = (Int64)Math.Floor((double)AllAmt / NoCloseChances.Count);
                                ec.ExchangeAmount = ChipAmt;
                                ec.ExchangeRate = ChipAmt/es.summary;
                                if(!strag.UseAmountList().ContainsKey(testData.LastData.Expect))
                                    strag.UseAmountList().Add(testData.LastData.Expect, ChipAmt);
                                
                            }
                            else
                            {
                                ec.ExchangeAmount = strag.UseAmountList()[testData.LastData.Expect];
                            }

                        }
                        bool Suc = es.Push(ref ec);
                        if (Suc)
                            NewExchangeRecord.Add(ec.Id, ec);
                        
                    }
                    //}
                    testIndex++;
                }
            }

            return ret;
        }


        public RoundBackTestReturnClass RunRound(StragClass teststrag, long TestLong, long StepLong)//滚动获取
        {
            long begNo = BegExpect;

            ExpectReader er = new ExpectReader();
            ExpectList el = null;
            long cnt = 0;
            RoundBackTestReturnClass ret = new RoundBackTestReturnClass();
            ExpectList AllData = new ExpectList();
            long testIndex = teststrag.ReviewExpectCnt - 1;
            ExpectList testData = null;
            Dictionary<string, ChanceClass> NoCloseChances = new Dictionary<string, ChanceClass>();
            Dictionary<string, ChanceClass> tmpChances = new Dictionary<string, ChanceClass>();
    
            long roundId=0;
            int currRid=0;
            List<Dictionary<string,ChanceClass>> roundNoMatchedChances = new List<Dictionary<string,ChanceClass>>();
            List<long> roundBegIds=new List<long>();
            
            while (el == null || el.Count > 0) //如果取到的数据长度大于0
            {
                el = er.ReadHistory(begNo, LoopCnt);
                if (el == null)
                {
                    ret.LoopCnt = cnt * LoopCnt;
                    ret.succ = false;
                    ret.Msg = "读取历史数据错误！";
                    break;
                }
                if (el.Count == 0)
                {
                    ret.LoopCnt = testIndex;
                    ret.succ = true;
                    ret.Msg = string.Format("成功遍历{0}条记录！", testIndex);
                    break;
                }
                AllData = ExpectList.Concat(AllData, el);
                begNo = el.LastData.LExpectNo + 1;

                cnt++;
                //Todo:

                while (testIndex < AllData.Count)
                {
                    if (testData == null)
                    {
                        testData = AllData.getSubArray(0, teststrag.ReviewExpectCnt);
                    }
                    else
                    {
                        if (AllData[(int)testIndex].ExpectIndex != testData.LastData.ExpectIndex + 1)
                        {
                            throw new Exception(string.Format("{1}第{0}期后出现数据遗漏，请补充数据后继续测试！", testData.LastData.Expect, testData.LastData.OpenTime));
                        }
                        testData.RemoveAt(0);
                        testData.Add(AllData[(int)testIndex]);
                    }
                    if (roundBegIds.Count > 0 && testIndex == roundBegIds[currRid] + TestLong)//切换当前滚动id
                    {
                        //取出当前未完成队列，
                        Dictionary<string,ChanceClass> NoMatchDic = roundNoMatchedChances[currRid];
                        BackTestReturnClass brc = ret.RoundData[currRid];
                        foreach (string key in NoMatchDic.Keys)//结束所有未玩成的结果
                        {
                            ChanceClass cc = NoMatchDic[key];
                            int matchcnt = 0;
                            if (cc.Matched(testData.LastData, out matchcnt, teststrag.GetRev))
                            {
                                cc.HoldTimeCnt = (int)(testData.LastData.ExpectIndex - cc.InputExpect.ExpectIndex);
                            }
                            else
                            {
                                cc.HoldTimeCnt = 99;//还未开出但是已经结束了的
                            }
                            cc.MatchChips = matchcnt;
                            cc.Closed = true;
                            cc.EndExpectNo = testData.LastData.Expect;
                            cc.Closed = true;
                            cc.EndExpectNo = testData.LastData.Expect;
                            cc.UpdateTime = testData.LastData.OpenTime;
                            brc.ChanceList.Add(cc);
                            if (brc.HoldCntDic.ContainsKey(cc.HoldTimeCnt))
                            {
                                brc.HoldCntDic[cc.HoldTimeCnt] = brc.HoldCntDic[cc.HoldTimeCnt] + 1;
                            }
                            else
                            {
                                brc.HoldCntDic.Add(cc.HoldTimeCnt, 1);
                            }
                        }
                        currRid ++;
                    }
                    if ((testIndex + 1 - teststrag.ReviewExpectCnt) % (StepLong) == 0) //每到周期整数倍，滚动开始id队列增加当前id
                    {
                        
                        roundBegIds.Add(testIndex);//加入队列
                        roundNoMatchedChances.Add(new Dictionary<string,ChanceClass>());//加入未完成的机会表到指定队列中
                        BackTestReturnClass brc = new BackTestReturnClass();
                        ret.RoundData.Add(brc);
                        roundId++;
                    }
                    for(int i=currRid;i<roundBegIds.Count;i++)//处理每个滚动周期的
                    {
                        ExchanceClass cc = new ExchanceClass();
                        Dictionary<string,ChanceClass> NoMatchDic = roundNoMatchedChances[i];
                        BackTestReturnClass brc = ret.RoundData[i];
                        cc.Run(testData, teststrag,ref brc.ChanceList,ref NoMatchDic,ref brc.HoldCntDic);
                        roundNoMatchedChances[i] = NoMatchDic;
                        brc.LoopCnt = testIndex - roundBegIds[i] + 1;
                        ret.RoundData[i] = brc;
                    }
                    testIndex++;
                }
            }

            return ret;
        }
    }

    public class ReturnClass
    {
        public bool succ;
        public long LoopCnt;
        public string Msg;
    }
    
    public class BackTestReturnClass : ReturnClass
    {
        
        public List<ExpectData> MatchList;
        public List<ChanceClass> ChanceList;
        
        public Dictionary<int, int> HoldCntDic;
        public Dictionary<int, int> HoldWinCntDic;
        public Dictionary<int, int> InChipsDic;
        public Dictionary<int, int> WinChipsDic;
        public string HoldInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (int key in HoldCntDic.Keys)
                {
                    sb.Append(string.Format(",持有期数为{0}次的机会出现{1}次",key,HoldCntDic[key]));
                }
                return sb.ToString();
            }
        }
    }

    public class RoundBackTestReturnClass : ReturnClass
    {
        public List<float> RoundWinRate
        {
            get
            {
                List<float> ret = new List<float>();
                for (int i = 0; i < RoundData.Count; i++)
                {
                    if (RoundData[i].HoldCntDic == null)
                    {
                        ret.Add(0);
                    }
                    else
                    {
                        ret.Add((float)(100.00*(float)RoundData[i].HoldCntDic[1]/(float)RoundData[i].ChanceList.Count));
                    }
                }
                return ret;
            }
        }
        public List<BackTestReturnClass> RoundData ;
        public RoundBackTestReturnClass()
        {
            RoundData = new List<BackTestReturnClass>();
        }
    }


}

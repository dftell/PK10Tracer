using DataRecSvr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.ExchangeLib;
//using WolfInv.com.PK10CorePress;
using WolfInv.com.ServerInitLib;
using WolfInv.com.Strags;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
using WolfInv.com.PK10CorePress;
using System.Collections;
using System.Threading.Tasks;
using WolfInv.com.Strags.Security;
using WolfInv.com.GuideLib;
namespace WolfInv.com.BackTestLib
{
    public delegate void SuccEvent(string expect);
        
    public class BackTestClass<T> where T:TimeSerialData
    {
        DataTypePoint dtp;
        string BegExpect;
        public string useBegExpect;
        string EndExpect;
        long LoopCnt;
        double Odds;
        SettingClass CurrSetting;
        public long testIndex = 0;
        long reviewCnt;
        public BackTestReturnClass<T> ret = new BackTestReturnClass<T>();
        public BaseStragClass<T> teststrag;
        public DataTable SystemStdDevs = new DataTable();
        public SuccEvent FinishedProcess;
        public Action<string,Hashtable> ExpectProcessedEvent;
        public Action<string,long, Hashtable> ExpectTip;
        public Action<string,string, string> StagInterProcessEvent;
        public string SecPools;
        public string benchMark;
        public BackTestClass(DataTypePoint _dtpName,string Codes,string bMarkCode, string FromE,long loopCnt,long buffCnt,SettingClass setting, string EndE=null)
        {
            SecPools = Codes;
            dtp = _dtpName;
            benchMark = bMarkCode;
            BegExpect = FromE;
            useBegExpect = BegExpect;
            EndExpect = EndE;
            LoopCnt = loopCnt;
            reviewCnt = buffCnt;
            if (_dtpName.DataType.ToUpper().Equals("CN_STOCK_A"))
            {
                //LoopCnt = 1000 * 60 * 60 * 24 * buffCnt;//1000*60*60*24 一天
            }
            CurrSetting = setting;
            testIndex = 0;
        }

        public void  Run()
        {
            //LoopCnt = 0;
            testIndex = 0;
            ret = new BackTestReturnClass<T>();
            string begNo = BegExpect;

            //ExpectReader er = new ExpectReader();
            DataReader<T> er = DataReaderBuild.CreateReader<T>(dtp.DataType, "", null);
            ExpectList<T> el = null;
            long cnt = 0;
            
            ret.HoldCntDic = new Dictionary<int, int>();
            ret.HoldWinCntDic = new Dictionary<int, int>();
            ret.InChipsDic = new Dictionary<int, int>();
            ret.WinChipsDic = new Dictionary<int, int>();

            ExpectList<T> AllData = new ExpectList<T>(dtp.IsSecurityData==1);
            long reviewCnt = 0;
            testIndex = teststrag.ReviewExpectCnt-1;
            reviewCnt = testIndex + 1;
            ExpectList<T> testData = null;
            Dictionary<string, ChanceClass<T>> NoCloseChances = new Dictionary<string, ChanceClass<T>>();
            Dictionary<string, ChanceClass<T>> tmpChances = new Dictionary<string, ChanceClass<T>>();
            int maxLoopCnt = 5;
            bool isRepeat = false;
            while (el == null || el.Count > 0) //如果取到的数据长度大于0
            {
                el = er.ReadHistory(begNo, LoopCnt+ (isRepeat?0:reviewCnt), SecPools);
                int ri = 0;
                while(el==null)
                {
                    if(ri>maxLoopCnt)
                    {
                        break;
                    }
                    Thread.Sleep(1 * 1000);
                    el = er.ReadHistory(begNo, LoopCnt+ (isRepeat ? 0 : reviewCnt), SecPools);
                    ri++;
                }
                if (el == null)
                {
                    ret.LoopCnt = (cnt+1) * LoopCnt;
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
                
                AllData = ExpectList<T>.Concat(AllData, el);
                begNo = el.LastData.LastExpect + 1;

                cnt++;
                //Todo:
                int pastCnt = 0;
                while (testIndex < AllData.Count)
                {
                    ret.LoopCnt = pastCnt + testIndex;
                    if (testData == null)
                    {
                        testData = AllData.getSubArray(0, teststrag.ReviewExpectCnt);
                    }
                    else
                    {
                        //if (dtp.DataType == "PK10")
                        //{
                        //    if (AllData[(int)testIndex].ExpectIndex != testData.LastData.ExpectIndex + 1)
                        //    {
                        //        throw new Exception(string.Format("{1}第{0}期后出现数据遗漏，请补充数据后继续测试！", testData.LastData.Expect, testData.LastData.OpenTime));
                        //    }
                        //}
                        
                            testData.RemoveAt(0);
                        testData.Add(AllData[(int)testIndex]);
                    }
                    tmpChances = new Dictionary<string, ChanceClass<T>>();
                    foreach (string key in NoCloseChances.Keys)
                    {
                        ChanceClass<T> cc = NoCloseChances[key];
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
                            if (dtp.IsXxY == 1)
                            {
                                (cc as iXxYClass).AllNums = dtp.AllNums;
                                (cc as iXxYClass).SelectNums = dtp.SelectNums;
                                (cc as iXxYClass).strAllTypeBaseOdds = dtp.strAllTypeOdds;
                                (cc as iXxYClass).strCombinTypeBaseOdds = dtp.strCombinTypeOdds;
                                (cc as iXxYClass).strPermutTypeBaseOdds = dtp.strPermutTypeOdds;
                            }
                            bool Matched = cc.Matched(testData.LastData, out matchcnt, false);
                            if (cc.NeedConditionEnd)
                            {
                                cc.MatchChips += matchcnt;
                                if (Matched)
                                {
                                    int LastMatchId = cc.LastMatchTimesId;//最后一次匹配次序号
                                    int maxHoldCnt = cc.AllowMaxHoldTimeCnt;
                                    if (cc.HoldTimeCnt - cc.LastMatchTimesId > maxHoldCnt )
                                    {
                                        cc.AllowMaxHoldTimeCnt = cc.HoldTimeCnt - cc.LastMatchTimesId;
                                    }
                                    cc.LastMatchTimesId = cc.HoldTimeCnt;
                                }
                                if (teststrag is ITraceChance<T>)
                                {
                                    ITraceChance<T> its = teststrag as ITraceChance<T>;
                                    if (its == null)
                                        cc.Closed = cc.OnCheckTheChance(cc, Matched);
                                    else
                                        cc.Closed = its.CheckNeedEndTheChance(cc, Matched,dtp.IsSecurityData==1);
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
                                if (Matched || (!Matched && cc.HoldTimeCnt>0 && cc.HoldTimeCnt == cc.AllowMaxHoldTimeCnt))//关闭
                                {
                                    cc.Closed = true;
                                    cc.EndExpectNo = testData.LastData.Expect;
                                    cc.MatchChips = matchcnt;
                                    if (!teststrag.GetRev)//只有不求相反值的情况下，才赋持有是次数
                                    {
                                        if (dtp.DataType == "PK10")
                                        {
                                            cc.HoldTimeCnt = (int)(testData.LastData.ExpectIndex - cc.InputExpect.ExpectIndex);
                                        }
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
                            if (cc.Closed  && cc.MatchChips>0)
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
                    BaseCollection<T> sc = new ExpectListProcessBuilder<T>(dtp,testData).getProcess().getSerialData(teststrag.ReviewExpectCnt, teststrag.BySer);
                    if (testData.Count == 0) 
                        break;
                    teststrag.SetLastUserData(testData);
                    teststrag.setDataTypePoint(dtp);
                    if(teststrag is ReferIndexStragClass)
                    {
                        (teststrag as ReferIndexStragClass).ClearTmpData();
                    }
                    List<ChanceClass<T>> cs = teststrag.getChances(sc,testData.LastData,dtp.IsSecurityData==1);//获取所有机会
                    if (ret.ChanceList == null)
                    {
                        ret.ChanceList = new List<ChanceClass<T>>();
                    }
                    //ret.ChanceList.AddRange(cs);
                    NoCloseChances = new Dictionary<string, ChanceClass<T>>();
                    foreach(string key in tmpChances.Keys)
                    {
                        ChanceClass<T> cc = tmpChances[key];
                        cc.AllowMaxHoldTimeCnt = int.MaxValue;
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
                            cs[i].AllowMaxHoldTimeCnt = int.MaxValue;
                            NoCloseChances.Add(key, cs[i]);
                        }
                    }
                    testIndex++;
                }
                pastCnt += el.Count;
            }
            FinishedProcess(el.LastData.Expect);
            //return ret;
        }

        /// <summary>
        /// 新逻辑调用服务的计算类，注入策略运行计划清单和数据
        /// </summary>
        /// <param name="es"></param>
        /// <param name="teststragplans"></param>
        /// <returns></returns>
        public BackTestReturnClass<T> VirExchange(ServiceSetting<T> sc, ref Hashtable ess, StragRunPlanClass<T>[] teststragplans, string codes)
        {
            BackTestReturnClass<T> ret = new BackTestReturnClass<T>();
            ret.HoldCntDic = new Dictionary<int, int>();
            ret.HoldWinCntDic = new Dictionary<int, int>();
            ret.InChipsDic = new Dictionary<int, int>();
            ret.WinChipsDic = new Dictionary<int, int>();
            List<string[]> arrs = sc.AllRunPlannings.Where(a => (a.Value.PlanStrag as BaseSecurityStragClass<T>).useRefrenceCodes).Select(a => (a.Value.PlanStrag as BaseSecurityStragClass<T>).RefrenceArray).ToList();
            List<string> refString = new List<string>();
            for (int i = 0; i < arrs.Count; i++)
            {
                refString.AddRange(arrs[i]);
            }
            ExpectList<T> el = null;
            try
            {
                //LoopCnt = 0;
                testIndex = 0;
                //调用计算服务进行计算
                //if (!teststragplans[0].AssetUnitInfo.Running) //如果资产单元没有启动，启动资产单元
                //    teststragplans[0].AssetUnitInfo.Run(false);
                bool mergeAsset = false;
                if (ess.Count == 1 && teststragplans.Length > ess.Count)
                {
                    mergeAsset = true;
                }
                if (mergeAsset)
                {
                    ess[teststragplans[0].AssetUnitInfo.UnitId] = teststragplans[0].AssetUnitInfo.getCurrExchangeServer();
                }
                else
                {
                    for (int i = 0; i < teststragplans.Length; i++)
                    {
                        if (!teststragplans[i].AssetUnitInfo.Running)
                        {
                            teststragplans[i].AssetUnitInfo.Run(dtp, false);
                        }
                        ess[teststragplans[i].AssetUnitInfo.UnitId] = teststragplans[i].AssetUnitInfo.getCurrExchangeServer();
                    }
                }
                // es = teststragplans[0].AssetUnitInfo.getCurrExchangeServer();//设置资产单元的模拟交易器
                CalcService<T> cs = new CalcService<T>(true, sc, teststragplans.ToDictionary(t => t.GUID, t => t));
                cs.DataPoint = dtp;
                cs.StragAfterProcessEvent = this.StagInterProcessEvent;
                if (dtp.IsSecurityData == 1)
                {
                    cs.ReadDataTableName = dtp.NewestTable;
                    cs.Codes = null;
                    cs.benchMark = benchMark;
                }
                cs.IsTestBack = true;
                string begNo = BegExpect;
                //ExpectReader er = new ExpectReader();
                DataReader<T> er = DataReaderBuild.CreateReader<T>(dtp.DataType, dtp.HistoryTable, dtp.RuntimeInfo.SecurityCodes); //支持所有数据
                long cnt = 0;
                cs.getSingleData = (code, expect, cnt1) => { return new MongoDataDictionary<T>(er.ReadNewestData(expect, cnt1, true, code), true); };
                ExpectList<T> AllData = new ExpectList<T>(dtp.IsSecurityData == 1);
                //long testIndex = teststrag.ReviewExpectCnt - 1;
                BaseStragClass<T>[] teststrags = teststragplans.Select(p => p.PlanStrag).ToArray<BaseStragClass<T>>();
                int maxReviewCnt = teststrags.Max<BaseStragClass<T>>(s => s.ReviewExpectCnt);
                testIndex = maxReviewCnt;//取所有策略中回览期最大的开始，之前的数据不看
                long InitIndex = testIndex;
                ExpectList<T> testData = null;
                ////Dictionary<string, StragChance<T>> NoCloseChances = new Dictionary<string, StragChance<T>>();
                ////Dictionary<string, StragChance<T>> tmpChances = new Dictionary<string, StragChance<T>>();
                ////Dictionary<Int64, ExchangeChance> NewExchangeRecord = new Dictionary<Int64, ExchangeChance>();
                int AllCnt = 0;
                bool inited = false;
                bool isRepeat = false;
                long orgLen = 0;
                int sumDiff = 0;
                string lastExpect = null;
                while (el == null || el.Count > 0) //如果取到的数据长度大于0或者还没有数据
                {
                    //如果第一次，先获取数据，以后的数据异步获取
                    if (el == null)
                    {
                        el = getHistoryData(sc,er, codes, refString, begNo, ref isRepeat, ref inited);//读取历史数据
                        lastExpect = el[(int)testIndex].Expect;
                    }
                    int noticeId = 0;
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
                    if(AllData.Count>0 && orgLen ==0)//第一次的长度够了就够了，后面再也不增加
                        orgLen = AllData.Count;
                    ExpectList<T> MergeData = ExpectList<T>.Concat(AllData, el);
                    long allLen = AllData.Count;
                    //string lastExpect = MergeData[(int)testIndex].Expect;//最后索引对应的期号
                    sc.wxlog.Log(string.Format("原有索引{0}对应期号",testIndex), lastExpect, string.Format(sc.gc.WXLogUrl, sc.gc.WXSVRHost));
                    if (dtp.IsSecurityData == 1 && orgLen>0)//连接后
                    {                        
                        ExpectList<T> reData = MergeData.LastDatas((int)(orgLen), true);//因停牌原因，新生成的长度可能要大于原始长度
                        int lastIndex = reData.DataList.Select(a => a.Expect).ToList().IndexOf(lastExpect);
                        if(lastIndex<0)
                        {
                            sc.wxlog.Log(string.Format("无法在合并数据中找到{0}对应的数据,终止测试！", lastExpect), AllData[(int)testIndex].Expect, string.Format(sc.gc.WXLogUrl, sc.gc.WXSVRHost));
                            break;
                        }
                        lastIndex++;//新索引要在上面+1
                        sumDiff += (int)(testIndex- lastIndex );
                        testIndex = lastIndex;//从对应那期继续执行计算
                        AllData = reData;                        
                        sc.wxlog.Log(string.Format("原始长度{0},实际长度{1},单个最大长度{2},缩短后索引{3}对应期号.",orgLen,reData.Count,reData.MongoData.Max(a=>a.Value.Count),testIndex), AllData[(int)testIndex].Expect, string.Format(sc.gc.WXLogUrl, sc.gc.WXSVRHost));
                    }
                    else
                    {
                        AllData = MergeData;
                    }
                    if (dtp.IsSecurityData == 0)
                    {
                        //begNo = el.LastData.LExpectNo + 1;//加一期
                        begNo = DataReader<T>.getNextExpectNo(el.LastData.Expect, dtp);
                    }
                    else
                    {
                        DateTime dt = el.LastData.Expect.ToDate();
                        begNo = dt.AddDays(1).WDDate();//加一个周期,如果要回测其他周期，AddDays许更换为其他时间周期
                    }
                    cnt++;
                    //Todo:
                    ExpectList<T> nextBatchData = null;
                    Task dataTask = null;
                    
                    bool running = false;
                    while (testIndex < AllData.Count)
                    {
                        int CurrExpectClose = 0;
                        
                        AllCnt++;
                        //ess.First().Value.UpdateExpectCnt(AllCnt);
                        if (testData == null)
                        {
                            //testData = AllData.getSubArray(0, teststrag.ReviewExpectCnt);
                            testData = AllData.getSubArray(0, (int)InitIndex + 1);
                        }
                        else
                        {
                            if (dtp.IsSecurityData == 0)//如果非证券，判断两个期号之间是否连续
                            {
                                ////if (AllData[(int)testIndex].Expect != testData.LastData.ExpectIndex + 1)
                                ////{
                                ////    if (dtp.DataType == "PK10")
                                ////    {
                                ////        throw new Exception(string.Format("{1}第{0}期后出现数据遗漏，请补充数据后继续测试！", testData.LastData.Expect, testData.LastData.OpenTime));
                                ////    }
                                ////}
                            }
                            if (testData.Count > maxReviewCnt)
                                testData.RemoveAt(0);
                            testData.Add(AllData[(int)testIndex]);
                        }
                        //只是取数据的逻辑一样，以后均调用CalcService
                        //ToAdd:以下是内容
                        if (dtp.IsSecurityData == 1)//证券类型，如果期号小于实际期号，全部跳过
                        {
                            if (testData.LastData.Expect.ToDate() < useBegExpect.ToDate())
                            {
                                testIndex++;
                                continue;
                            }
                        }
                        if (testData.LastData.Expect.ToDate() > EndExpect.ToDate())//如果达到了最后一期，结束
                        {
                            FinishedProcess(testData.LastData.Expect);
                            return ret;
                        }
                        lastExpect = testData.LastData.Expect;
                        cs.CurrData = testData;
                        cs.OnFinishedCalc += OnCalcFinished;
                        cs.setGlobalClass(Program<T>.gc);
                        cs.setAllSettingConfig(Program<T>.AllServiceConfig);
                        cs.Calc();
                        
                        while (!cs.CalcFinished)
                        {
                            Thread.Sleep(1 * 100);
                        }
                        this.SystemStdDevs = cs.getSystemStdDevList();
                        //testIndex++;
                        Hashtable htb = ess;                        
                        if (AllCnt > noticeId + 22)//22期才刷新一次
                        {
                            Task.Factory.StartNew(() => {
                                ExpectTip(testData.LastData.Expect, sumDiff+testIndex, htb);

                            });
                            noticeId = AllCnt;
                            ////Task.Factory.StartNew((ht) =>
                            ////{
                            ////    ExpectProcessedEvent(testData.LastData.Expect, ht as Hashtable);
                            ////}, ess);
                        }
                        testIndex++;
                        if (!running)//获取历史数据过长，异步获取，与计算同时进行
                        {
                            running = true;
                            dataTask = Task.Factory.StartNew(() => {
                                //以两个线程执行获取历史数据
                                nextBatchData = getHistoryData(sc, er, codes, refString, begNo, ref isRepeat, ref inited, 1);
                            }, TaskCreationOptions.LongRunning);
                            
                        }
                    }
                    if(dataTask == null)
                    {
                        sc.wxlog.Log("执行完成","等待接收数据", string.Format(sc.gc.WXLogUrl, sc.gc.WXSVRHost));
                    }
                    else
                    {
                        sc.wxlog.Log("执行完成", "使用已有数据", string.Format(sc.gc.WXLogUrl, sc.gc.WXSVRHost));
                    }
                    Task.WaitAll(dataTask);
                    el = nextBatchData;
                }
                
                return ret;
            }
            catch (Exception ce)
            {
                sc.wxlog.Log("回测错误",string.Format("{0}:{1}",ce.Message,ce.StackTrace), string.Format(sc.gc.WXLogUrl, sc.gc.WXSVRHost));
                return ret;
            }
            finally
            {
                FinishedProcess(el.LastData?.Expect);
            }
        }

        ExpectList<T> getHistoryData(ServiceSetting<T> sc,DataReader<T> er,string codes,List<string> refString,string begNo,ref bool isRepeat,ref bool inited,int ThreadCnt=10)
        {
            ExpectList<T> el = null;
            DateTime startTime = DateTime.Now;
            if (dtp.IsSecurityData == 1)
            {                
                //sc.wxlog.Log("开始获取历史数据", string.Format("{0}", startTime.ToString()), string.Format(sc.gc.WXLogUrl, sc.gc.WXSVRHost));
                er.setThreadCnt(ThreadCnt);
                el = er.ReadHistory(begNo, LoopCnt + (isRepeat ? 0 : reviewCnt), codes);
                if (el == null || el.Count == 0)
                {
                    
                }
                else
                {
                    string endT = el.LastData.Expect;
                    el = MergeExpectList(el, er, benchMark, begNo, endT);
                    el = MergeExpectList(el, er, string.Join(";", refString), begNo, endT);
                    el.reSort();
                    if (!inited)
                    {
                        int InitIndex = el.DataList.Select(a => a.Expect).ToArray().IndexOf(useBegExpect);
                        if (testIndex > InitIndex)
                            testIndex = InitIndex;
                        inited = true;
                        isRepeat = true;
                    }
                }
            }
            else
            {
                el = er.ReadHistory(begNo, LoopCnt, codes);
            }
            int rptCnt = 0;
            int maxRptCnt = 5;
            while (el == null)
            {
                Thread.Sleep(2 * 1000);
                rptCnt++;
                if (rptCnt > maxRptCnt)
                {
                    break;
                }
                if (dtp.IsSecurityData == 1)
                {
                    el = er.ReadHistory(begNo, LoopCnt + (isRepeat ? 0 : reviewCnt), codes);
                    string endT = el.LastData.Expect;
                    el = MergeExpectList(el, er, benchMark, begNo, endT);
                    el = MergeExpectList(el, er, string.Join(";", refString), begNo, endT);
                    el.reSort();
                }
                else
                {
                    el = er.ReadHistory(begNo, LoopCnt, codes);
                }
            }
            sc.wxlog.Log("获取历史数据成功", string.Format("{0}", DateTime.Now.Subtract(startTime).TotalMinutes), string.Format(sc.gc.WXLogUrl, sc.gc.WXSVRHost));
            return el;
        }

        ExpectList<T> MergeExpectList(ExpectList<T> el, DataReader<T> er,string codes,string begNo,string endNo)
        {
            if (!string.IsNullOrEmpty(codes))
            {
                ExpectList<T> benchMarkList = null;
                if (string.IsNullOrEmpty(endNo))
                    benchMarkList = er.ReadHistory(begNo, LoopCnt, codes);
                else
                    benchMarkList = er.ReadHistory(begNo, endNo, codes);
                for (int i = 0; i < benchMarkList.Count; i++)
                {
                    ExpectData<T> data = benchMarkList[i];
                    if (!el.Contains(data.Expect))
                    {
                        el.Add(data);
                        continue;
                    }
                    ExpectData<T> edata = el[data.Expect];
                    foreach (string key in data.Keys)
                    {
                        if (!edata.ContainsKey(key))
                        {
                            edata.Add(key, data[key]);
                        }
                    }
                }
            }
            return el;
        }
        void OnCalcFinished(DataTypePoint dtp)
        {
            //this.SystemStdDevs = cs.getSystemStdDevList();
            //testIndex++;
            //testIndex++;
        }
        
        public BackTestReturnClass<T> VirExchange_oldLogic(ExchangeService<T> es, StragRunPlanClass<T>[] teststragplans)
        {
            string begNo = BegExpect;
            ExpectReader<T> er = new ExpectReader<T>();
            ExpectList<T> el = null;
            long cnt = 0;
            BackTestReturnClass<T> ret = new BackTestReturnClass<T>();
            ret.HoldCntDic = new Dictionary<int, int>();
            ret.HoldWinCntDic = new Dictionary<int, int>();
            ret.InChipsDic = new Dictionary<int, int>();
            ret.WinChipsDic = new Dictionary<int, int>();

            ExpectList<T> AllData = new ExpectList<T>(dtp.IsSecurityData == 1);
            //long testIndex = teststrag.ReviewExpectCnt - 1;
            BaseStragClass<T>[] teststrags = teststragplans.Select(p => p.PlanStrag).ToArray<BaseStragClass<T>>();
            long testIndex = teststrags.Max<BaseStragClass<T>>(s => s.ReviewExpectCnt);//取所有策略中回览期最大的开始，之前的数据不看
            long InitIndex = testIndex;
            ExpectList<T> testData = null;
            Dictionary<string, StragChance<T>> NoCloseChances = new Dictionary<string, StragChance<T>>();
            Dictionary<string, StragChance<T>> tmpChances = new Dictionary<string, StragChance<T>>();
            Dictionary<Int64, ExchangeChance<T>> NewExchangeRecord = new Dictionary<Int64, ExchangeChance<T>>();
            int AllCnt = 0;
            while (el == null || el.Count > 0) //如果取到的数据长度大于0
            {
                el = er.ReadHistory(begNo, LoopCnt,SecPools);
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
                AllData = ExpectList<T>.Concat(AllData, el);
                begNo = (el.LastData.LExpectNo + 1).ToString();

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
                    tmpChances = new Dictionary<string, StragChance<T>>();

                    //关闭所有交易
                    foreach (int id in NewExchangeRecord.Keys)
                    {
                        ExchangeChance<T>  ec = NewExchangeRecord[id];
                        int matchcnt = 0;
                        ec.OwnerChance.Matched(testData.LastData, out matchcnt, false);
                        ec.MatchChips = matchcnt;
                        if (dtp.IsSecurityData == 1)
                            es.UpdateSecurity(ec);
                        else
                            es.Update(ec);
                        ec = null;
                    }
                    NewExchangeRecord = new Dictionary<Int64, ExchangeChance<T>>();

                    foreach (string key in NoCloseChances.Keys)
                    {
                        StragChance<T> scc = NoCloseChances[key];
                        ChanceClass<T> cc = scc.Chance;
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
                                    int maxHoldCnt = cc.AllowMaxHoldTimeCnt;
                                    if (cc.HoldTimeCnt - cc.LastMatchTimesId > maxHoldCnt)
                                    {
                                        cc.AllowMaxHoldTimeCnt = cc.HoldTimeCnt - cc.LastMatchTimesId;
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

                    List<StragChance<T>> cs = new List<StragChance<T>>();
                    for (int i = 0; i < teststrags.Length; i++)
                    {
                        BaseCollection<T> sc = new ExpectListProcessBuilder<T>(dtp,testData).getProcess().getSerialData(teststrags[i].ReviewExpectCnt, teststrags[i].BySer);
                        if (testData.Count == 0)
                            break;
                        List<ChanceClass<T>> scs = teststrags[i].getChances(sc, testData.LastData,dtp.IsSecurityData==1);//获取所有机会
                        for (int j = 0; j < scs.Count; j++)
                        {
                            ChanceClass<T> CurrCc = scs[j];
                            ////scs[j].IncrementType = teststragplans[i].IncreamType;
                            ////scs[j].FixAmt = teststragplans[i].FixAmt;
                            ////scs[j].FixRate = teststragplans[i].FixRate;
                            StragRunPlanClass<T> currPlan = teststragplans[i];
                            BaseStragClass<T> currStrag = currPlan.PlanStrag;
                            CurrCc.HoldTimeCnt = 1;
                            CurrCc.Cost = CurrCc.ChipCount * CurrCc.UnitCost;
                            CurrCc.Gained = 0;
                            CurrCc.Profit = 0;
                            CurrCc.ExecDate = DateTime.Today;
                            CurrCc.CreateTime = el.LastData.OpenTime;
                            CurrCc.UpdateTime = CurrCc.CreateTime;
                            CurrCc.StragId = currStrag.GUID;
                            CurrCc.ExpectCode = el.LastData.Expect;
                            CurrCc.AllowMaxHoldTimeCnt = currPlan.AllowMaxHoldTimeCnt;
                            CurrCc.FixAmt = currPlan.FixAmt;
                            CurrCc.FixRate = currPlan.FixRate;
                            CurrCc.IncrementType = currPlan.IncreamType;
                            cs.Add(new StragChance<T>(teststrags[i], CurrCc));
                        }
                    }
                    if (ret.ChanceList == null)
                    {
                        ret.ChanceList = new List<ChanceClass<T>>();
                    }
                    //ret.ChanceList.AddRange(cs);
                    NoCloseChances = new Dictionary<string, StragChance<T>>();
                    foreach (string key in tmpChances.Keys)
                    {
                        StragChance<T> scc = tmpChances[key];
                        ChanceClass<T> cc = scc.Chance;
                        NoCloseChances.Add(key, scc);
                        //////ProbWaveSelectStragClass组合改为统一交易
                        ////if ((scc.Strag is ProbWaveSelectStragClass) == false)
                        ////{
                        ////    ExchangeChance<T>  ec = new ExchangeChance(scc.Strag, testData.LastData.Expect, cc);
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
                            cs[i].Chance.BaseAmount = es.summary.currCash<es.InitCash?1:es.summary.currCash/es.InitCash;
                            NoCloseChances.Add(key,cs[i]);
                            ////////ProbWaveSelectStragClass组合改为统一交易
                            //////if ((cs[i].Strag is ProbWaveSelectStragClass)==false)
                            //////{
                            //////    ExchangeChance<T>  ec = new ExchangeChance(cs[i].Strag, testData.LastData.Expect, cs[i].Chance);//交易
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

                        ExchangeChance<T>  ec = new ExchangeChance<T>(es, NoCloseChances[key].Strag, NoCloseChances[key].Chance.ExpectCode,testData.LastData.Expect, NoCloseChances[key].Chance);//交易
                        if (ec.OccurStrag is ProbWaveSelectStragClass)//对于ProbWaveSelectStragClass，一开始就计算好了Amount
                        {
                            ProbWaveSelectStragClass strag = ec.OccurStrag as ProbWaveSelectStragClass;
                            if (!strag.UseAmountList().ContainsKey(testData.LastData.Expect))
                            {
                                double AllAmt = (ec.OccurStrag as BaseObjectsLib.ISpecAmount<T>).getChipAmount(es.summary.currCash, ec.OwnerChance, ec.OccurStrag.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials);
                                Int64 ChipAmt = (Int64)Math.Floor((double)AllAmt / NoCloseChances.Count);
                                ec.ExchangeAmount = ChipAmt;
                                //ec.ExchangeRate = ChipAmt/es.summary;
                                if(!strag.UseAmountList().ContainsKey(testData.LastData.Expect))
                                    strag.UseAmountList().Add(testData.LastData.Expect, ChipAmt);
                                
                            }
                            else
                            {
                                ec.ExchangeAmount = strag.UseAmountList()[testData.LastData.Expect];
                            }

                        }
                        bool Suc = es.Push(ref ec,true,dtp.IsSecurityData==1,true);
                        if (Suc)
                            NewExchangeRecord.Add(ec.Id, ec);
                        
                    }
                    //}
                    testIndex++;
                }
            }

            return ret;
        }


        public RoundBackTestReturnClass<T> RunRound(BaseStragClass<T> teststrag, long TestLong, long StepLong)//滚动获取
        {
            string begNo = BegExpect;

            ExpectReader<T> er = new ExpectReader<T>();
            ExpectList<T> el = null;
            long cnt = 0;
            RoundBackTestReturnClass<T> ret = new RoundBackTestReturnClass<T>();
            ExpectList<T> AllData = new ExpectList<T>(dtp.IsSecurityData==1);
            long testIndex = teststrag.ReviewExpectCnt - 1;
            ExpectList<T> testData = null;
            Dictionary<string, ChanceClass<T>> NoCloseChances = new Dictionary<string, ChanceClass<T>>();
            Dictionary<string, ChanceClass<T>> tmpChances = new Dictionary<string, ChanceClass<T>>();
    
            long roundId=0;
            int currRid=0;
            List<Dictionary<string,ChanceClass<T>>> roundNoMatchedChances = new List<Dictionary<string,ChanceClass<T>>>();
            List<long> roundBegIds=new List<long>();
            
            while (el == null || el.Count > 0) //如果取到的数据长度大于0
            {
                el = er.ReadHistory(begNo.ToString(), LoopCnt,SecPools);
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
                AllData = ExpectList<T>.Concat(AllData, el);
                begNo = (el.LastData.LExpectNo + 1).ToString();

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
                        Dictionary<string,ChanceClass<T>> NoMatchDic = roundNoMatchedChances[currRid];
                        BackTestReturnClass<T> brc = ret.RoundData[currRid];
                        foreach (string key in NoMatchDic.Keys)//结束所有未玩成的结果
                        {
                            ChanceClass<T> cc = NoMatchDic[key];
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
                        roundNoMatchedChances.Add(new Dictionary<string,ChanceClass<T>>());//加入未完成的机会表到指定队列中
                        BackTestReturnClass<T> brc = new BackTestReturnClass<T>();
                        ret.RoundData.Add(brc);
                        roundId++;
                    }
                    for(int i=currRid;i<roundBegIds.Count;i++)//处理每个滚动周期的
                    {
                        ExchanceClass<T> cc = new ExchanceClass<T>();
                        Dictionary<string,ChanceClass<T>> NoMatchDic = roundNoMatchedChances[i];
                        BackTestReturnClass<T> brc = ret.RoundData[i];
                        cc.Run(dtp,testData, teststrag,ref brc.ChanceList,ref NoMatchDic,ref brc.HoldCntDic);
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
    
    public class BackTestReturnClass<T> : ReturnClass where T:TimeSerialData
    {
        
        public List<ExpectData<T>> MatchList;
        public List<ChanceClass<T>> ChanceList;
        
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

    public class RoundBackTestReturnClass<T> : ReturnClass where T:TimeSerialData
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
        public List<BackTestReturnClass<T>> RoundData ;
        public RoundBackTestReturnClass()
        {
            RoundData = new List<BackTestReturnClass<T>>();
        }
    }


}

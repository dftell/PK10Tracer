using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using System.ComponentModel;
using WolfInv.com.ProbMathLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.Strags
{
    [DescriptionAttribute("二项分布选号策略"),
        DisplayName("二项分布选号策略")]
    [Serializable]
    public class Strag_BinomialDistrClass : ChanceTraceStragClass
    { 
        int StepCnt = 5;
        int MinFilterCnt = 20;
        int MaxFilterCnt = 100;
        static Dictionary<string, List<int>> AllBinomPeaks = new Dictionary<string, List<int>>();
        public Strag_BinomialDistrClass()
            : base()
        {
            _StragClassName = "二项分布选号策略";
        }

        /// <summary>
        /// 计算峰值
        /// </summary>
        /// <param name="N">回览次数</param>
        /// <param name="K">成功次数</param>
        /// <param name="p">正常概率</param>
        void CalcPeakValues(int N, int K,double p)
        {
            string strModel = "{0}_{1}";
            string key = string.Format(strModel, N, K);
            List<int> PeakValues = new List<int>();
            if (AllBinomPeaks.ContainsKey(key))
            {
                return;
            }
            Dictionary<int, double> AllProbs = new Dictionary<int, double>();
            for (int i = 1; i <= K; i++)
            {
                AllProbs.Add(i,ProbMath.GetBinomial(N, i, p));
            }
            PeakValues = AllProbs.OrderByDescending(c => c.Value).ToDictionary(a => a.Key, b => b.Value).Keys.ToArray<int>().ToList<int>();
            AllBinomPeaks.Add(key, PeakValues.GetRange(0,3));//获取前3位作为峰值列表
        }

        void InitAllPeaks()
        {
            for (int i = MinFilterCnt; i <= MaxFilterCnt; i = i + StepCnt)
            {
                CalcPeakValues(i, this.InputMaxTimes, 0.0975);
            }
        }

        public override List<ChanceClass> getChances(BaseCollection sc, ExpectData ed)
        {
            int minRows = this.CommSetting.GetGlobalSetting().MutliColMinTimes;
            List<ChanceClass> ret = new List<ChanceClass>();
            Dictionary<int, List<int>> preReduceCnts = Strag_LongHoldStartReduceClass.getReduceTimes(this.UsingDpt, sc, minRows, 1, 2, 1, true);//只检查上期
            if (preReduceCnts.Count < 2)//如果整体基本没有变化，直接排除所有
            {
                return ret;
            }
            int checkTerms = 10;
            int checkStringLen = 3;
            int minHoldTimes = 5;
            //Dictionary<int, List<int>> colReduceCnts = Strag_LongHoldStartReduceClass.getReduceTimes(this.UsingDpt, sc, minRows, checkTerms, checkStringLen, minHoldTimes, true);
            InitAllPeaks();//初始化峰值列表            
            if (sc == null || sc.Table == null || sc.Table.Rows.Count < this.ReviewExpectCnt)
                return ret;
            int AllItemCnt = AllBinomPeaks.Count;
            Dictionary<string, Dictionary<int, int>> AllList = new Dictionary<string, Dictionary<int, int>>();
            for (int i = 0; i < 10; i++)//遍历各车/各名次
            {
                //if (!colReduceCnts.ContainsKey(i))
                //{
                //    //continue;
                //}
                //if (colReduceCnts.ContainsKey(i) && (colReduceCnts[i].Count ==0 || colReduceCnts[i].Count > checkTerms/5 || (colReduceCnts[i].Count>0 && colReduceCnts[i][0] > checkTerms/2)))//如果太少，太多，太远都不行
                //{
                //    //continue;
                //}
                string strCol = string.Format("{0}", (i + 1) % 10);
                //string strVal = ed.ValueList[i];
                Dictionary<int, int> ValCntItems = new Dictionary<int, int>();
                for (int val = 0; val < 10; val++)//对每个数字出现的次数检查
                {
                    ValCntItems.Add(val, 0);
                    string strVal = string.Format("{0}", val);
                    foreach (string key in AllBinomPeaks.Keys)//遍历所有峰值清单
                    {
                        int ViewCnt = int.Parse(key.Split('_')[0]);//获得峰值清单对应的实验次数
                        int ExistCnt = sc.FindLastDataExistCount(ViewCnt, strCol, strVal);//获得前N-1次该车次出现的次数
                        if (AllBinomPeaks[key].Contains(ExistCnt))// + 1))//如果该二项分布检查的峰值是7，8，9，值出现的次数是6，7，8,匹配，+1
                        {
                            ValCntItems[val] = ValCntItems[val] + 1;
                        }
                    }
                }
                //满足所有二项分布的才入选
                Dictionary<int, int> ValidItems = ValCntItems.Where(c => c.Value == AllItemCnt ).ToDictionary(a => a.Key, b => b.Value);
                Dictionary<int,int> MatchPoisonItems = new Dictionary<int,int>();
                foreach(int key in ValidItems.Keys)
                {
                    int ViewCnt = MinFilterCnt -1; //找出所有值在前100次里出现的次数
                    int ExistCnt = sc.FindLastDataExistCount(ViewCnt,strCol,key.ToString());//满足最泊松分布最顶峰
                    if(ExistCnt == ViewCnt/10-1)
                    {
                        MatchPoisonItems.Add(key,ValidItems[key]);
                    }
                }
                AllList.Add(strCol, MatchPoisonItems);
            }
            //检查所有满足条件的清单
            
            Dictionary<string, int> AllCodes = new Dictionary<string, int>();
            List<string> strCnts = new List<string>();
            bool RetMutliChances = this.FixChipCnt;
            foreach (string strcol in AllList.Keys)
            {
                if (AllList[strcol].Count == 0)
                    continue;
                int MaxCnt = AllList[strcol].Values.Max();//选最优
                //Dictionary<int,int> strCodeList = AllList[strcol].Where(a => a.Value == MaxCnt).ToDictionary(a => a.Key, b => b.Value);
                Dictionary<int, int> strCodeList = AllList[strcol];
                string StrCode = "";
                string StrVal = string.Join("", strCodeList.Keys.ToArray());
                string strCnt = string.Join("_", strCodeList.Values.ToArray());
                strCnts.Add(strCnt);
                if (BySer)
                    StrCode = string.Format("{0}/{1}", strcol, StrVal);
                else
                    StrCode = string.Format("{0}/{1}", StrVal, strcol);
                AllCodes.Add(StrCode,strCodeList.Count);
                
            }
            Log("所有选出来的记录",string.Join("+",AllCodes.Keys.ToArray()));
            if (AllCodes.Count ==0)
                return ret;
            if (RetMutliChances)
            {
                foreach (string key in AllCodes.Keys)
                {
                    int Chips = AllCodes[key];
                    if (Chips == 1)
                        continue;
                    if (Chips < this.ChipCount) 
                        continue;
                    ChanceClass cc = new ChanceClass();
                    cc.ChanceCode = key;
                    cc.SignExpectNo = ed.Expect;
                    cc.ChanceType = 2;
                    cc.ChipCount = AllCodes[key];
                    cc.InputTimes = 1;
                    cc.strInputTimes = "";// string.Join("_", strCnts.ToArray());
                    //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                    cc.InputExpect = ed;
                    cc.StragId = this.GUID;
                    //cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                    cc.IsTracer = 0;
                    cc.HoldTimeCnt = 1;
                    cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                    cc.ExpectCode = ed.Expect;
                    cc.CreateTime = DateTime.Now;
                    cc.UpdateTime = DateTime.Now;
                    cc.AllowMaxHoldTimeCnt = 1;
                    cc.Closed = false;
                    ret.Add(cc);
                }
                return ret;
            }
            ////if (AllCodes.Count == 1 && AllCodes.First().Value == 1)//独此一份
            ////    return ret;
            string strAllCode = string.Join("+", AllCodes.Keys.ToArray());
            if (ChanceClass.getChipsByCode(strAllCode) < this.ChipCount)
            {
                return ret;
            }
            if (!RetMutliChances)
            {
                ChanceClass cc = new ChanceClass();
                cc.SignExpectNo = ed.Expect;
                cc.ChanceType = 2;
                cc.ChipCount = AllCodes.Sum(p => p.Value);
                cc.InputTimes = 1;
                cc.strInputTimes = string.Join("_", strCnts.ToArray());
                //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                cc.InputExpect = ed;
                cc.StragId = this.GUID;
                //cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                cc.IsTracer = 0;
                cc.HoldTimeCnt = 1;
                cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                cc.ExpectCode = ed.Expect;
                cc.ChanceCode = string.Join("+", AllCodes.Keys.ToArray());
                cc.CreateTime = DateTime.Now;
                cc.UpdateTime = DateTime.Now;
                cc.AllowMaxHoldTimeCnt = 1;
                cc.Closed = false;
                ret.Add(cc);
            }

            return ret;
        }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }

        
        public override Type getTheChanceType()
        {
            return typeof(NolimitTraceChance);
        }

        bool _IsTracing;
        public bool IsTracing
        {
            get
            {
                return _IsTracing;
            }
            set
            {
                _IsTracing = value;
            }
        }

        public new bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            return LastExpectMatched;
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            cc.AllowMaxHoldTimeCnt = 1;
            try
            {
                if (LastUseData().LastData.Expect != cc.ExpectCode)
                {
                    return 0;
                }
                if (cc.IncrementType == InterestType.CompoundInterest)
                {
                    if(cc.HoldTimeCnt >1)
                    {
                        return 0;
                    }
                    if (cc.AllowMaxHoldTimeCnt > 0 && cc.HoldTimeCnt > cc.AllowMaxHoldTimeCnt)
                    {
                        return 0;
                    }
                    if(LastUseData().LastData.Expect != cc.ExpectCode)
                    {
                        return 0;
                    }
                    //
                    double rate = KellyMethodClass.KellyFormula(1, 10, 9.75, 1.001);
                    //double rate = KellyMethodClass.KellyFormula(1, 9.75, 0.1032);
                    long ret = (long)Math.Ceiling((double)(RestCash * rate));
                    return ret;
                }
                //大于5码的不受限制
                int shift = 0;
                
                if (cc.ChipCount < this.InputMinTimes && cc.HoldTimeCnt > cc.AllowMaxHoldTimeCnt && cc.AllowMaxHoldTimeCnt > 0)
                {
                    int CurrChipCount = ChanceClass.getChipsByCode(cc.ChanceCode);
                    if(CurrChipCount >= 2)//大于等于3的，超过了一定期数以后可以跟号。4码5次，3码7次,2码11次。6-n+(6-n+1)^2 
                    {
                        shift = (this.InputMinTimes+1 - CurrChipCount) +  (int)(Math.Pow((this.InputMinTimes +1- CurrChipCount+1) , 2));
                        //shift = (6 - CurrChipCount) + (6 - CurrChipCount + 1) ^ 2;
                    }
                    else
                    {
                        shift = 50;
                    }
                    //                [组合信息:1/0+6/7;组合长度:2;指定最小长度:6:Log]当前次数:10;最小入场次数:9
                    Log(string.Format("组合信息:{0};组合长度:{1};指定最小长度:{2}",cc.ChanceCode, CurrChipCount, this.InputMinTimes),string.Format("当前次数:{0};最小入场次数:{1}",cc.HoldTimeCnt,shift));
                    if(cc.HoldTimeCnt<shift)
                        return 0;
                }
                if (cc.HoldTimeCnt <= cc.AllowMaxHoldTimeCnt && cc.AllowMaxHoldTimeCnt > 0) //如果小于等于1
                {
                    return cc.FixAmt.Value;
                }
                int hcnt = cc.HoldTimeCnt - shift;
                if (hcnt < 0)
                    return 0;
                int chips = cc.ChipCount - 1;
                int maxcnt = amts.MaxHoldCnts[chips];
                int bShift = 0;
                if (hcnt > maxcnt)
                {
                    Log("风险", "达到最大上限", string.Format("机会{0}持有次数达到{1}次总投入金额已为{2}", cc.ChanceCode, hcnt, cc.Cost));
                    bShift = (int)maxcnt * 2 / 3;
                }
                int RCnt = (hcnt % (maxcnt + 1)) + bShift-1;
                return amts.Serials[chips][RCnt];
            }
            catch (Exception e)
            {
                Log("错误", string.Format("二项分布，获取单码金额错误:{0}", e.Message), e.StackTrace);
            }
            return 1;
        }
    
    }
}

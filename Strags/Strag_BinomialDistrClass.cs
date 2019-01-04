using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.ComponentModel;
using ProbMathLib;
namespace Strags
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
                CalcPeakValues(i, this.InputMaxTimes, 0.098);
            }
        }

        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
        {
            InitAllPeaks();//初始化峰值列表
            List<ChanceClass> ret = new List<ChanceClass>();
            if (sc == null || sc.Table == null || sc.Table.Rows.Count < this.ReviewExpectCnt)
                return ret;
            int AllItemCnt = AllBinomPeaks.Count;
            Dictionary<string, Dictionary<int, int>> AllList = new Dictionary<string, Dictionary<int, int>>();
            for (int i = 0; i < 10; i++)//遍历各车/各名次
            {
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
                        if (AllBinomPeaks[key].Contains(ExistCnt + 1))//如果该二项分布检查的峰值是7，8，9，值出现的次数是6，7，8,匹配，+1
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
                    int ViewCnt = MinFilterCnt-1; //找出所有值在前100次里出现的次数
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
                    cc.MaxHoldTimeCnt = 1;
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
                cc.MaxHoldTimeCnt = 1;
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

        public override bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            return LastExpectMatched;
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            try
            {
                
                if (cc.IncrementType == InterestType.CompoundInterest)
                {
                    if (cc.MaxHoldTimeCnt >0 && cc.HoldTimeCnt > cc.MaxHoldTimeCnt)
                    {
                        return 0;
                    }
                    //
                    double rate = KellyMethodClass.KellyFormula(1, 10, 9.75, 1.001);
                    //double rate = KellyMethodClass.KellyFormula(1, 9.75, 0.1032);
                    long ret = (long)Math.Ceiling((double)(RestCash * rate));
                    return ret;
                }
                if (cc.ChipCount <= 3 && cc.HoldTimeCnt > cc.MaxHoldTimeCnt && cc.MaxHoldTimeCnt > 0)
                {
                    return 0;
                }
                if (cc.HoldTimeCnt <= cc.MaxHoldTimeCnt && cc.MaxHoldTimeCnt > 0) //如果小于等于1
                {
                    return cc.FixAmt.Value;
                }
                int chips = cc.ChipCount - 1;
                int maxcnt = amts.MaxHoldCnts[chips];
                int bShift = 0;
                if (cc.HoldTimeCnt > maxcnt)
                {
                    Log("风险", "达到最大上限", string.Format("机会{0}持有次数达到{1}次总投入金额已为{2}", cc.ChanceCode, cc.HoldTimeCnt, cc.Cost));
                    bShift = (int)maxcnt * 2 / 3;
                }
                int RCnt = (cc.HoldTimeCnt % (maxcnt + 1)) + bShift-1;
                return amts.Serials[chips][RCnt];
            }
            catch (Exception e)
            {
                Log("错误", "获取单码金额错误", e.Message);
            }
            return 1;
        }
    
    }
}

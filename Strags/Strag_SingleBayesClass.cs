using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.ComponentModel;
using ProbMathLib;
namespace Strags
{
    [DescriptionAttribute("单列贝叶斯选号策略"),
        DisplayName("单列贝叶斯选号策略")]
    [Serializable]
    public class Strag_SingleBayesClass : ChanceTraceStragClass
    { 
        int StepCnt = 5;
        int MinFilterCnt = 20;
        int MaxFilterCnt = 100;
        static Dictionary<string, List<int>> AllBinomPeaks = new Dictionary<string, List<int>>();
        public Strag_SingleBayesClass()
            : base()
        {
            _StragClassName = "单列贝叶斯选号策略";
        }

        /// <summary>
        /// 计算峰值
        /// </summary>
        /// <param name="N">回览次数</param>
        /// <param name="K">成功次数</param>
        /// <param name="p">正常概率</param>
        
        

        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            PKDataListSetFactory pkdls = new PKDataListSetFactory(this.LastUseData());
            Dictionary<int,int> res = pkdls.OccurProbList(this.ReviewExpectCnt-2, 1);
            Dictionary<string,int> AllCodes = new Dictionary<string, int>();
            foreach (int key in res.Keys)
                AllCodes.Add(string.Format("{0}/{1}",key,res[key]),1);
            string strAllCode = string.Join("+", AllCodes.Keys.ToArray());
            if (ChanceClass.getChipsByCode(strAllCode) < this.ChipCount)
            {
                return ret;
            }
            if (true)
            {
                ChanceClass cc = new ChanceClass();
                cc.SignExpectNo = ed.Expect;
                cc.ChanceType = 2;
                
                cc.InputTimes = 1;
                cc.strInputTimes = string.Join("_", new string[2] { "1", "1"});
                //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                cc.InputExpect = ed;
                cc.StragId = this.GUID;
                //cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                cc.IsTracer = 0;
                cc.HoldTimeCnt = 1;
                cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                cc.ExpectCode = ed.Expect;
                cc.ChanceCode = string.Join("+", AllCodes.Keys.ToArray());
                cc.ChipCount = ChanceClass.getChipsByCode(cc.ChanceCode);
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
                    if (cc.MaxHoldTimeCnt >0 && cc.HoldTimeCnt > 1)
                    {
                        return 0;
                    }
                    //
                    double rate = cc.FixRate.Value;
                    //double rate = KellyMethodClass.KellyFormula(1, 9.75, 0.1032);
                    long ret = (long)Math.Ceiling((double)(RestCash * rate));
                    return ret;
                }
                if ( cc.HoldTimeCnt > 1)
                {
                    return 0;
                }
                return cc.FixAmt.Value;

            }
            catch (Exception e)
            {
                Log("错误", "获取单码金额错误", e.Message);
            }
            return 1;
        }
    
    }

    public class PKDataListSetFactory
    {
        ExpectList Data;
        public PKDataListSetFactory(ExpectList el)
        {
            Data = el;
            //jisuan
        }

        BayesDicClass OccurrDir(int col, int TestLength, int LastTimes)//add by zhouys 2019/1/15
        {
            BayesDicClass ret = new BayesDicClass();
            int iShift = Data.Count - TestLength;
            if (iShift <= LastTimes) //Data length must more than TestLength+LastTimes+1
                return ret;
            Dictionary<string, int> defaultDic = getDefaultCombDic();
            Dictionary<int, int> PreA = InitPriorProbabilityDic();
            Dictionary<int, int> PreB = InitPriorProbabilityDic();
            //for (int col=0;col<10;col++)
            //{
            Dictionary<string, int> combDic = defaultDic;
            for (int i = iShift - 1; i < Data.Count; i++)
            {
                int CurrA = getIntValue(Data[i].ValueList[col]);
                int CurrB = getIntValue(Data[i - LastTimes].ValueList[col]);
                string key = string.Format("{0}_{1}", CurrA, CurrB);
                int cnt = combDic[key];
                combDic[key] = cnt + 1;
                PreA[CurrA] = PreA[CurrA] + 1;
                PreB[CurrB] = PreB[CurrB] + 1;
            }
            ret.PosteriorProbDic = combDic;
            ret.PriorProbDicA = PreA;
            ret.PriorProbDicB = PreB;
            ret.TestLength = TestLength;
            //}
            return ret;
        }

        Dictionary<int, int> InitPriorProbabilityDic()
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            for (int i = 1; i <= 10; i++)
                ret.Add(i%10, 0);
            return ret;
        }

 


        Dictionary<string, int> getDefaultCombDic()
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    string key = string.Format("{0}_{1}", i%10, j%10);
                    ret.Add(key, 0);
                }
            }
            return ret;
        }

        public Dictionary<string, double> OccurColumnProb(int col, int TestLength, int LastTimes)
        {
            Dictionary<string, double> ret = new Dictionary<string, double>();
            BayesDicClass bdic = OccurrDir(col, TestLength, LastTimes);
            ret = bdic.getBA();
            return ret;
        }

        public Dictionary<int, List<int>> OccurProbList(int TestLength, int LastTimes, int SelectListCnt)
        {
            Dictionary<int, List<int>> ret = new Dictionary<int, List<int>>();
            for (int i = 0; i < 10; i++)
            {
                Dictionary<string, double> res = OccurColumnProb(i, TestLength, LastTimes);
                string str = Data.LastData.ValueList[i];
                //str = str == "0" ? "10" : str;
                int col = (i + 1) % 10;
                List<int> colList = BayesDicClass.getBAMaxNValue(res, int.Parse(str), SelectListCnt);
                ret.Add(col, colList);
            }
            return ret;
        }

        public Dictionary<int, int> OccurProbList(int TestLength, int LastTimes)
        {
            Dictionary<int, List<int>> ret = OccurProbList(TestLength, LastTimes, 1);
            return ret.ToDictionary(p => p.Key, p => p.Value[0]);
        }



     
        int getIntValue(string val)
        {
            //if (val == "0") return 10;
            return int.Parse(val);
        }
    }

    public class BayesDicClass
    {
        public int TestLength;
        public Dictionary<string, int> PosteriorProbDic;
        public Dictionary<int, int> PriorProbDicA;
        public Dictionary<int, int> PriorProbDicB;

        public Dictionary<string, double> getBA()
        {
            Dictionary<string, double> ret = new Dictionary<string, double>();
            foreach (string key in PosteriorProbDic.Keys)
            {
                string[] strIdx = key.Split('_');
                int A = int.Parse(strIdx[0]);
                int B = int.Parse(strIdx[1]);
                double ProbBA = (double)PriorProbDicA[A] * PosteriorProbDic[key] / PriorProbDicB[B] / TestLength;
                ret.Add(key, ProbBA);
            }
            return ret;
        }

        public static Dictionary<int, double> getBADic(Dictionary<string, double> testResult, int CheckVal)
        {
            Dictionary<int, double> ret = new Dictionary<int, double>();
            for (int i = 1; i <= 10; i++)
            {
                string key = string.Format("{0}_{1}", i%10, CheckVal);
                ret.Add(i%10, testResult[key]);
            }
            return ret;
        }

        public static string getBAString(Dictionary<string, double> testResult, int CheckVal)
        {
            Dictionary<int, double> ret = getBADic(testResult, CheckVal);
            ret.OrderBy(p => p.Value);
            StringBuilder sb = new StringBuilder();
            double sum = 0;
            foreach (int key in ret.Keys)
            {
                sb.AppendLine(string.Format("{0}:{1}", key, ret[key]));
                sum += ret[key];
            }
            sb.AppendLine(string.Format("合计:{0}", sum));
            return sb.ToString();
        }

        public static int getBAMaxValue(Dictionary<string, double> testResult, int CheckVal)
        {
            Dictionary<int, double> ret = getBADic(testResult, CheckVal);
            return ret.OrderByDescending(p => p.Value).First().Key;
            //return ret.First().Key;
        }

        public static List<int> getBAMaxNValue(Dictionary<string, double> testResult, int CheckVal, int MaxN)
        {
            Dictionary<int, double> ret = getBADic(testResult, CheckVal);
            var list = ret.OrderByDescending(p => p.Value);
            return list.ToDictionary(p => p.Key, p => p.Value).Keys.Take(MaxN).ToList();
            //.Take(MaxN).ToList();
            //return ret.First().Key;
        }
    }

}

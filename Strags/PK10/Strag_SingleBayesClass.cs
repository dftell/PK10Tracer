using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;
namespace WolfInv.com.Strags
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
        
        

        public override List<ChanceClass> getChances(BaseCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            PKDataListSetFactory pkdls = new PKDataListSetFactory(new ExpectList(this.LastUseData<TimeSerialData>()?.Table));
            Dictionary<int, Dictionary<int, int>> res = pkdls.OccurrSpecLengthShiftProbList(this.ReviewExpectCnt, 1,0,10);
            Dictionary<string,int> AllCodes = new Dictionary<string, int>();
            foreach (int key in res.Keys)
            {
                Dictionary<int, int> sres = res[key].Where(p => p.Value > this.InputMinTimes).ToDictionary(p=>p.Key,p=>p.Value);
                if (sres.Count == 0) continue;
                AllCodes.Add(string.Format("{0}/{1}", key, string.Join("", sres.Keys.ToArray())), 1);
            }
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

        public override bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            return LastExpectMatched;
        }

        public override double getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            try
            {
                
                if (cc.IncrementType == InterestType.CompoundInterest)
                {
                    if (cc.AllowMaxHoldTimeCnt > 0 && cc.HoldTimeCnt > 1)
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
                Log("错误", string.Format("获取单码金额错误:{0}", e.Message), e.StackTrace);
            }
            return 1;
        }
    
    }

    public class PKDataListSetFactory:MLDataFactory<TimeSerialData>
    {
        ExpectList Data;
        public PKDataListSetFactory(ExpectList el)
        {
            Data = el;
            //jisuan
        }

        BayesDicClass OccurrDir(int col, int shiftCol, int TestLength, int LastTimes)//add by zhouys 2019/1/15
        {
            BayesDicClass ret = new BayesDicClass();
            int iShift = Data.Count - TestLength;
            if (iShift < LastTimes) //Data length must more than TestLength+LastTimes+1
                return ret;
            Dictionary<string, int> defaultDic =  PKProbVector.getDefaultCombDic();
            Dictionary<int, int> PreA = PKProbVector.InitPriorProbabilityDic();
            Dictionary<int, int> PreB = PKProbVector.InitPriorProbabilityDic();
            //for (int col=0;col<10;col++)
            //{
            Dictionary<string, int> combDic = defaultDic;
            int BColIndex = (col + shiftCol) % 10;//对于大于10的取模
            if (BColIndex < 0)//对于小于0的，+10 如：0 + （-1） = 9
            {
                BColIndex = BColIndex + 10;
            }
            for (int i = iShift; i < Data.Count; i++)
            {
                int CurrA = getIntValue(Data[i].ValueList[col]);
                int CurrB = getIntValue(Data[i - LastTimes].ValueList[BColIndex]);
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

        

        ////public Vector getVectors(int index, int CheckCnt)
        ////{
        ////    Vector v = Vector.Zero(0);
        ////    for (int c = 0; c < 10; c++)
        ////    {
        ////        for (int r = index - CheckCnt + 1; r <= index; r++)
        ////        {
        ////            v.Append(double.Parse(Data[r].ValueList[c]));
        ////        }
        ////    }
        ////    return v;
        ////}

        public Dictionary<string, double> OccurColumnProb(int col, int shiftCol, int TestLength, int LastTimes)
        {
            Dictionary<string, double> ret = new Dictionary<string, double>();
            BayesDicClass bdic = OccurrDir(col, shiftCol, TestLength, LastTimes);
            ret = bdic.getBA();
            return ret;
        }

        public Dictionary<int, List<int>> OccurProbList(int shift, int TestLength, int LastTimes, int SelectListCnt)
        {
            Dictionary<int, List<int>> ret = new Dictionary<int, List<int>>();
            for (int i = 0; i < 10; i++)
            {

                Dictionary<string, double> res = OccurColumnProb(i, shift, TestLength, LastTimes);
                string str = Data.LastData.ValueList[i];
                //str = str == "0" ? "10" : str;
                int col = (i + 1) % 10;
                List<int> colList = BayesDicClass.getBAMaxNValue(res, int.Parse(str), SelectListCnt);
                ret.Add(col, colList);
            }
            return ret;
        }

        public Dictionary<int, Dictionary<int, double>> OccurProbDetailList(int shift, int TestLength, int LastTimes, int SelectListCnt)
        {
            Dictionary<int, Dictionary<int, double>> ret = new Dictionary<int, Dictionary<int, double>>();
            for (int i = 0; i < 10; i++)
            {

                Dictionary<string, double> res = OccurColumnProb(i, shift, TestLength, LastTimes);
                string str = Data.LastData.ValueList[i];//最后记录对应i名的车号
                int col = (i + 1) % 10;
                Dictionary<int, double> colList = BayesDicClass.getBAMaxNProbValue(res, int.Parse(str), SelectListCnt);//该列上，以该车号为条件B推出所有车号的概率
                ret.Add(col, colList);
            }
            return ret;
        }

        public Dictionary<int, int> OccurProbList(int shift, int TestLength, int LastTimes)
        {

            Dictionary<int, List<int>> ret = OccurProbList(shift, TestLength, LastTimes, 1);
            return ret.ToDictionary(p => p.Key, p => p.Value[0]);

        }

        public Dictionary<int, List<int>> OccurSelectLengthProbDetailList(int shift, int TestLength, int LastTimes,int SelectLength)
        {

            Dictionary<int, List<int>> ret = OccurProbList(shift, TestLength, LastTimes, SelectLength);
            return ret.ToDictionary(p => p.Key, p => p.Value);

        }
        public Dictionary<int, Dictionary<int, int>> OccurrSpecLengthShiftProbList(int TestLength, int LastTimes,int startPos,int shiftLen)
        {
            Dictionary<int, Dictionary<int, int>> ret = new Dictionary<int, Dictionary<int, int>>();
            for (int i = 0; i < 10; i++)
                ret.Add(i, new Dictionary<int, int>());
            for (int i = startPos; i < startPos+shiftLen; i++)//遍历所有偏移的高概率
            {
                //Dictionary<int, int> res = OccurProbList(i, TestLength, LastTimes);//获取当前偏移的所有高概率
                Dictionary<int, List<int>> res = OccurProbList(i, TestLength, LastTimes,3);//获取当前偏移的所有高概率
                foreach (int key in res.Keys)
                {
                    for (int j = 0; j < res[key].Count; j++)
                    {
                        if (!ret[key].ContainsKey(res[key][j]))
                        {
                            ret[key].Add(res[key][j], 0);
                        }
                        ret[key][res[key][j]] = ret[key][res[key][j]] + 1;
                    }
                }
            }
            return ret;
        }

        public Dictionary<int, Dictionary<int, Dictionary<int, double>>> OccurrAllShiftProbList(int TestLength, int LastTimes, int MaxCnt)
        {
            Dictionary<int, Dictionary<int, Dictionary<int, double>>> ret = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>();
            for (int i = 0; i < 10; i++)
                ret.Add(i, new Dictionary<int, Dictionary<int, double>>());
            for (int i = 0; i < 10; i++)//遍历所有偏移的高概率
            {
                Dictionary<int, Dictionary<int, double>> res = this.OccurProbDetailList(i, TestLength, LastTimes, MaxCnt);//获取当前偏移的所有高概率
                ret.Add(i, res);
            }
            return ret;
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
                double ProbBA = 0;
                if (PriorProbDicB[B] != 0)
                    ProbBA = (double)PriorProbDicA[A] * PosteriorProbDic[key] / PriorProbDicB[B] / TestLength;
                ret.Add(key, ProbBA);
            }
            return ret;
        }

        public static Dictionary<int, double> getBADic(Dictionary<string, double> testResult, int CheckVal)
        {
            Dictionary<int, double> ret = new Dictionary<int, double>();
            for (int i = 1; i <= 10; i++)
            {
                string key = string.Format("{0}_{1}", i % 10, CheckVal);
                ret.Add(i % 10, testResult[key]);
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
            return getBAMaxNProbValue(testResult, CheckVal, MaxN).Keys.ToList();
        }

        public static Dictionary<int, double> getBAMaxNProbValue(Dictionary<string, double> testResult, int CheckVal, int MaxN)
        {
            Dictionary<int, double> ret = getBADic(testResult, CheckVal);
            var list = ret.OrderByDescending(p => p.Value);
            return list.Take(MaxN).ToDictionary(p => p.Key, p => p.Value);
            //.Take(MaxN).ToList();
            //return ret.First().Key;
        }
    }

    public class PKProbVector
    {
        public static Dictionary<int, int> InitPriorProbabilityDic()
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            for (int i = 1; i <= 10; i++)
                ret.Add(i % 10, 0);
            return ret;
        }

        public static Dictionary<string, int> getDefaultCombDic()
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    string key = string.Format("{0}_{1}", i % 10, j % 10);
                    ret.Add(key, 0);
                }
            }
            return ret;
        }

        public static Dictionary<string, int> getDefaultCombDic(int VCnt)
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            string strModel = "{0}_{1}";
  
            for (int i = 1; i <= 10; i++)
            {
                string strKey = string.Format("{0}", i % 10);
                for (int v = 1;v<VCnt;v++)
                {
                    strKey = strKey + "_{0}";
                    for(int j=1;j<=10;j++)
                    {
                        ret.Add(string.Format(strKey,j%10), 0);
                    }
                }
            }
            return ret;
        }

        //////public static List<List<int>> getDefaultCombDic(int Vcnt,int XMin,int XMax)
        //////{
        //////    List<List<int>> ret = new List<List<int>>();
        //////    List<int> feature = new List<int>();
        //////    string strKey = "";
        //////    for (int i = XMin; i <= 10; i++)
        //////    {
        //////        int FirstX = (i+1)% 10;

        //////        for (int v = 1; v < VCnt; v++)
        //////        {
        //////            strKey = strKey + "_{0}";
        //////            for (int j = 1; j <= 10; j++)
        //////            {
        //////                ret.Add(string.Format(strKey, j % 10), 0);
        //////            }
        //////        }
        //////    }
        //////    return ret;
        //////}

        static List<List<int>> getNextFeature(int Vcnt,int Index,int XMin,int XMax,List<int> orgList)
        {
            List<List<int>> retList = new List<List<int>>();
            List<int> ret = new List<int>();
            ret.AddRange(orgList);
            if(Index >= Vcnt)
            {
                return retList;
            }
            if(XMin<=XMax)
            {
                ret[Index] = XMin;
                retList.Add(ret);
            }
            retList.Add(ret);
            List<List<int>> nextList = getNextFeature(Vcnt + 1, Index + 1, XMin + 1, XMax, ret);
            retList.AddRange(nextList.ToArray());
            return retList;
        }


        public static Dictionary<int, int> getDefault1VIntCombDic() //
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int key = 10 * i + j;
                    ret.Add(key, 0);
                }
            }
            return ret;
        }

        public static int getIntIndexFromString(string strIdx)
        {
            string[] arr = strIdx.Split('_');
            for(int i=0;i<arr.Length;i++)
            {
                int a = int.Parse(arr[i]);
                arr[i] = string.Format("{0}", a == 0 ? 9 : a - 1);
            }
            return int.Parse(string.Join("",arr));
        }
    }
}

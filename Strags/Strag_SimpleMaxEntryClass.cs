using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.ComponentModel;
using ProbMathLib;
using MachineLearnLib;
namespace Strags
{
    [DescriptionAttribute("简单最大熵选号策略"),
        DisplayName("简单最大熵选号策略")]
    [Serializable]
    public class Strag_SimpleMaxEntryClass : ChanceTraceStragClass
    { 
        int StepCnt = 5;
        int MinFilterCnt = 20;
        int MaxFilterCnt = 100;
        public Strag_SimpleMaxEntryClass()
            : base()
        {
            _StragClassName = "简单最大熵选号策略";
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
            DataFactory pkdls = new DataFactory(this.LastUseData());
            Dictionary<int, Dictionary<int, int>> res = pkdls.getAllShiftAndColMaxProbList(this.ReviewExpectCnt- this.InputMinTimes-1, this.InputMinTimes,true);
            Dictionary<string,int> AllCodes = new Dictionary<string, int>();
            foreach (int key in res.Keys)
                AllCodes.Add(string.Format("{0}/{1}",key,string.Join("",res[key].Keys.ToArray())),1);
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
    
        class DataFactory
        {
            ExpectList Data;
            public DataFactory(ExpectList el)
            {
                Data = el;
            }

            BayesDicClass OccurrDir(int col, int shiftCol, int TestLength, int LastTimes,bool LastisSerial)//add by zhouys 2019/1/15
            {
                BayesDicClass ret = new BayesDicClass();
                int iShift = Data.Count - TestLength;
                if (iShift <= LastTimes) //Data length must more than TestLength+LastTimes+1
                    return ret;
                Dictionary<string, int> defaultDic = PKProbVector.getDefaultCombDic();
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
                for (int i = iShift - 1; i < Data.Count; i++)
                {
                    int CurrA = int.Parse(Data[i].ValueList[col]);
                    int CurrB = int.Parse(Data[i - LastTimes].ValueList[BColIndex]);
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

            public List<Instance> OccurrTestInstances(int col, int shiftCol, int TestLength, int LastTimes, bool LastisSerial)
            {
                List<Instance> ret = new List<Instance>();
                int iShift = Data.Count - TestLength;
                if (iShift <= LastTimes) //Data length must more than TestLength+LastTimes+1
                    return ret;
                int BColIndex = (col + shiftCol) % 10;//对于大于10的取模
                if (BColIndex < 0)//对于小于0的，+10 如：0 + （-1） = 9
                {
                    BColIndex = BColIndex + 10;
                }
                
                for (int i = iShift - 1; i < Data.Count; i++)
                {
                    
                    Dictionary<int, List<int>> combDic = new Dictionary<int, List<int>>();
                    int CurrA = int.Parse(Data[i].ValueList[col]);
                    List<int> list = getFeatures(col, shiftCol, i - 1, LastTimes, LastisSerial);
                    combDic.Add(CurrA, list);
                    Instance it = new Instance(CurrA, list.ToArray());
                    ret.Add(it);
                }
                return ret;
            }

            List<int> getFeatures(int col, int shiftCol,int index, int LastTimes, bool LastisSerial)
            {
                List<int> list = new List<int>();
                int i = index;
                int BColIndex = (col + shiftCol) % 10;//对于大于10的取模
                if (BColIndex < 0)//对于小于0的，+10 如：0 + （-1） = 9
                {
                    BColIndex = BColIndex + 10;
                }
                for (int bi = i - LastTimes+1; bi <= i; bi++)//如果连续取前N期
                {
                    int CurrB = int.Parse(Data[bi].ValueList[BColIndex]);
                    list.Add(CurrB);
                    if (!LastisSerial)
                        break;
                }
                return list;
            }

            public  List<Instance> getLastFeatures(int col, int shiftCol, int index, int LastTimes, bool LastisSerial)
            {
                List<Instance> ret = new List<Instance>();
                List<int> testList = getFeatures(col, shiftCol, index, LastTimes, LastisSerial);
                //testList.Add(TestVal);
                ret.Add(new Instance(0, testList.ToArray()));
                return ret;
            }
            public Dictionary<int,double> getMaxProb(int col,int shiftCol,int TestLength, int LastTimes, bool LastisSerial)
            {
                Dictionary<int, double> ret = new Dictionary<int, double>();
                List<Instance> TrainSet = OccurrTestInstances(col, shiftCol, TestLength, LastTimes, LastisSerial);
                List<Instance> TestSet = getLastFeatures(col, shiftCol, Data.Count-1, LastTimes, LastisSerial);
                ret = MaxEnt.getLabels(TrainSet, TestSet);
                
                
                return ret;
            }
            
            public Dictionary<int, Dictionary<int,int>> getAllShiftAndColMaxProbList(int TestLength, int LastTimes, bool LastisSerial)
            {
                Dictionary<int, Dictionary<int, int>> ret = new Dictionary<int, Dictionary<int, int>>();
                for(int sft = 0;sft<10;sft++)
                {
                    Dictionary<int, int> shiftRs = new Dictionary<int, int>();
                    for(int col=0;col<10;col++)
                    {
                        Dictionary<int, double> res = getMaxProb(col, sft, TestLength, LastTimes, LastisSerial);
                        int MaxKey = res.OrderByDescending(p => p.Value).First().Key;
                        shiftRs.Add((col+1)%10,MaxKey);
                        if (col > 1)
                            break;
                    }
                    ret.Add(sft, shiftRs);
                    if (sft > 1)
                        break;
                }
                return ret;
            }

            
        }
    }

 
}

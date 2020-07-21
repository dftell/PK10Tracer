using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;
using WolfInv.com.ProbMathLib;
using WolfInv.com.MachineLearnLib.Markov;
using WolfInv.com.MachineLearnLib;

namespace WolfInv.com.Strags.MLStragClass
{
    [DescriptionAttribute("马尔可夫选号策略"),
        DisplayName("马尔可夫选号策略")]
    [Serializable]
    public class strag_MarkovClass : StragClass
    {
        public strag_MarkovClass()
            : base()
        {
            _StragClassName = "马尔可夫选号策略";
        }

        public override bool CheckNeedEndTheChance(ChanceClass cc1, bool LastExpectMatched1)
        {
            return LastExpectMatched1;
        }

        public override List<ChanceClass> getChances(BaseCollection sc, ExpectData ed)
        {
            bool isXxY = (sc is CommCollection_KLXxY);
            List <ChanceClass> ret = new List<ChanceClass>();
            Dictionary<int, List<int>> useList = new Dictionary<int, List<int>>();
            MarkovCategoryFactioryClass mcf = new MarkovCategoryFactioryClass();
            mcf.Init(this.LastUseData());
            List<string> useResList = new List<string>();
            Dictionary<int,List<KeyValuePair<int, double>>> allColList = new Dictionary<int,List<KeyValuePair<int, double>>>();
            Task[] tks = new Task[(isXxY ? 1 : sc.SelNums)];
            //Log("任务数", sc.SelectNums.ToString());
            try
            {
                for (int i = 0; i < (isXxY?1:sc.SelNums); ++i)
                {

                    int col = (i + 1) % 10;
                    runningClass rc = new runningClass();
                    rc.Log = Log;
                    rc.TrainSet = mcf.getCategoryData(i, ReviewExpectCnt, isXxY ? 1:0);
                    rc.i = i;
                    rc.col = col;
                    rc.SelectNums = sc.AllNums;
                    rc.FilterCnt = ChipCount;
                    rc.TopN = InputMinTimes;
                    
                    rc.MoveCnt = this.InputMaxTimes;
                    rc.ReviewExpectCnt = ReviewExpectCnt;
                    rc.allColList = allColList;
                    Task tk = new Task(() =>
                    {
                        //runningTask(mcf, allColList, sc.SelectNums, col, i);
                        rc.runningTask();
                    });
                    tks[i] = tk;
                    tk.Start();
                }
                Task.WaitAll(tks);
            }
            catch (Exception ce)
            {
                Log(ce.Message, ce.StackTrace);
                return ret;
            }
            Dictionary<int, Dictionary<int, double>> noReptRes = new Dictionary<int, Dictionary<int, double>>();
            foreach(int key in allColList.Keys)
            {
                allColList[key].ForEach(a=> {
                    
                    if(!noReptRes.ContainsKey(a.Key))
                    {
                        noReptRes.Add(key, new Dictionary<int, double>());
                    }
                    if(noReptRes.ContainsKey(key))
                        noReptRes[key].Add(a.Key, a.Value);
                });
            }
            var Res =this.GetRev?noReptRes.OrderBy(a=>a.Value.Values.First()):noReptRes.OrderByDescending(a => a.Value.Values.First());//按最大的排序           
            //var Res = noReptRes.Select(a => a.OrderByDescending(b => b.Value).First().Value).ToDictionary(a=>a.Key,a=>a);
            var useRes = Res.Take(this.InputMinTimes);
            int chipCnt = Math.Min(InputMinTimes, useRes.Count());
            string strChanceCode = ""; 
            if (!isXxY)//对应pk10，xyft
            {
                foreach (var res in useRes)
                {
                    useResList.Add(string.Format("{0}/{1}", res.Key, res.Value.Keys.First()));
                }
                strChanceCode = string.Join("+", useResList);
            }
            else
            {
                foreach(var res in useRes)
                {
                    useResList.Add(string.Format("{0}", res.Value.Keys.First()));
                }
                string[] strRev = useResList.ToArray();
                strChanceCode = string.Format("C{0}/{1}", 1,string.Join("", strRev));
            }
            if (useResList.Count>0)
            {
                ChanceClass cc = (sc is CommCollection_KLXxY) ? new ChanceClass_ForCombinXxY(): new ChanceClass();
                if(isXxY)
                {
                    (cc as ChanceClass_ForCombinXxY).AllNums = sc.AllNums;
                    (cc as ChanceClass_ForCombinXxY).SelectNums = sc.SelNums;
                    (cc as ChanceClass_ForCombinXxY).strAllTypeBaseOdds = (sc as CommCollection_KLXxY).strAllTypeOdds;
                    (cc as ChanceClass_ForCombinXxY).strCombinTypeBaseOdds = (sc as CommCollection_KLXxY).strCombinTypeOdds;
                    (cc as ChanceClass_ForCombinXxY).strPermutTypeBaseOdds = (sc as CommCollection_KLXxY).strPermutTypeOdds;
                }
                cc.ChanceCode = strChanceCode;
                //Log("结果",cc.ChanceCode);
                cc.SignExpectNo = ed.Expect;
                cc.ChanceType = 2;
                cc.ChipCount = FixChipCnt?1: ChanceClass.getChipsByCode(cc.ChanceCode);
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
        class runningClass
        {
            public Dictionary<int, List<KeyValuePair<int, double>>> allColList;
            public int SelectNums;
            public int TopN;
            public int MoveCnt;
            public int FilterCnt;
            public int col;
            public int i;
            public int ReviewExpectCnt;
            public Action<string, string> Log;
            public MLInstances<int, int> TrainSet;
            public int firstN=1;
            public void runningTask()
            {
                try
                {
                    MarkovClass SelectFunc = new MarkovClass();
                    SelectFunc.GroupId = col;
                    SelectFunc.LearnDeep = ReviewExpectCnt;
                    SelectFunc.TrainIterorCnt = MoveCnt;
                    SelectFunc.SelectCnt = SelectNums;
                    SelectFunc.FilterCnt = FilterCnt;
                    SelectFunc.TopN = TopN;
                    SelectFunc.FillTrainData(TrainSet);
                    SelectFunc.InitTrain();
                    List<KeyValuePair<int, double>> res = SelectFunc.getPredictResult(col);
                    if (res.Count > 0)
                    {
                        lock (allColList)
                        {
                            allColList.Add(col, res.Take(firstN).ToList());
                        }
                    }
                    else
                    {

                    }
                }
                catch(Exception ce)
                {
                    Log?.Invoke(ce.Message, ce.StackTrace);
                }
            }
        }

        class RunningClass
        {
            void Running()
            {
                
                    
                
            }
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            if (LastUseData().LastData.Expect != cc.ExpectCode)
            {
                return 0;
            }
            if (cc.IncrementType == InterestType.CompoundInterest)
            {
                double rate = KellyMethodClass.KellyFormula(1, 10, 9.75, 1.001);
                //double rate = KellyMethodClass.KellyFormula(1, 9.75, 0.1032);
                long ret = (long)Math.Ceiling((double)(RestCash * rate));
                return ret;
            }
            return cc.FixAmt==null?1:cc.FixAmt.Value;
        }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }

        public override Type getTheChanceType()
        {
            if(UsingDpt?.DataType == "GDKL11")
            {
                return typeof(ChanceClass_ForCombinXxY);
            }
            return typeof(NolimitTraceChance);
        }
    }
}

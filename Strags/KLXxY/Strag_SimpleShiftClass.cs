using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;
using WolfInv.com.ProbMathLib;

namespace WolfInv.com.Strags
{
    [DescriptionAttribute("简单概率偏移择时投注策略"),
        DisplayName("简单概率偏移择时投注策略")]
    public class Strag_SimpleShiftClass : ReferIndexStragClass
    {
        public Strag_SimpleShiftClass():base()
        {
            _StragClassName = "简单概率偏移择时投注策略";
        }
        public override bool CheckNeedEndTheChance(ChanceClass cc1, bool LastExpectMatched1)
        {
            return true;
        }

        public override List<ChanceClass> getChances(BaseCollection sc, ExpectData ed)
        {
            if(this.CurrIndex == null || this.CurrIndex.Count == 0)
                CurrIndex = this.calcCurrIndex(sc);
            List<ChanceClass> ret = new List<ChanceClass>();
            List<IndexExpectData> useIndex = getUseIndexs(ReviewExpectCnt);
            List<StringIntData> lastStatus = useIndex.Count>0? useIndex.Last()?.HoldStatus:null;
            List<StringDoubleArrayData> lastTarget = useIndex.Count > 0 ? useIndex.Last()?.TargetValue : null;
            if(lastStatus == null)
            {
                lastStatus = sc.getFeatureDic<int>(this.BySer).Select(a => new StringIntData(a.Key, a.Value)).ToList() ;
            }
            if(lastTarget == null)
            {
                lastTarget = sc.getFeatureDic<double[]>(this.BySer).Select(a => new StringDoubleArrayData(a.Key, new double[4])).ToList();
            }
            IndexExpectData ied = new IndexExpectData();
            ied.Expect = ed.Expect;
            ied.Datas = CurrIndex;
            ied.HoldStatus = lastStatus;
            ied.TargetValue = lastTarget;
            //ied.HoldStatus = LastStatus;
            
            if (useIndex.Count==0)
            {
                this.Indexs.Add(ied);
                return ret;
            }
            List<List<List<StringDoubleData>>> useIndexsData = useIndex.Select(a => a.Datas).ToList() ;

            Dictionary<string, int> fs = sc.getFeatureDic<int>(true);
            
            if (useIndexsData.Count<this.InputMaxTimes || CurrIndex.Count!= 2)
            {
                this.Indexs.Add(ied);
                return ret;
            }            
            Dictionary<string, List<double>> allFeaturns = new Dictionary<string, List<double>>();//按标签分类时间序列上大周期出现个数数组
            Dictionary<string, List<double>> lastFeaturns = new Dictionary<string, List<double>>();//按标签分类时间序列上最近周期出现个数数组
            Dictionary<string, List<double>> currIndexData = new Dictionary<string, List<double>>();
            int cnt = 0;
            foreach (var kv in fs)
            {
                currIndexData.Add(kv.Key,new double[] { CurrIndex[0][cnt].Value,CurrIndex[1][cnt].Value }.ToList());
                cnt++;
            }
            for(int i=0;i< useIndexsData.Count; i++)//遍历过往索引
            {
                cnt = 0;
                foreach(string key in fs.Keys)//对每个标签
                {
                    if(!allFeaturns.ContainsKey(key))
                    {
                        allFeaturns.Add(key, new List<double>());
                    }
                    allFeaturns[key].Add(useIndexsData[i][0][cnt].Value);//大周期标签第一组Key值
                    if(!lastFeaturns.ContainsKey(key))
                        {
                            lastFeaturns.Add(key, new List<double>());
                        }
                        lastFeaturns[key].Add(useIndexsData[i][1][cnt].Value);//最近周期标签取第二组key值
                    
                    cnt++;
                }
            }
            cnt = 0;
            foreach (var kv in lastStatus)
            {
                List<List<double>> datas = new List<List<double>>();
                datas.Add(allFeaturns[kv.Label]);
                datas.Add(lastFeaturns[kv.Label]);
                List<double> targetArr = lastTarget[cnt].Value.ToList();
                if (kv.Value==0)//未持仓
                {
                    
                    bool openCc = NeedIn(kv.Label, datas, currIndexData[kv.Label],ref targetArr,sc.AllNums);
                    if (openCc)
                    {
                        ied.HoldStatus[cnt].Value = 1;
                       
                    }
                }
                else//持仓  
                {
                    bool closeCc = NeedEnd(kv.Label, datas, currIndexData[kv.Label],lastTarget[cnt].Value.ToList(),sc.AllNums,kv.Value);
                    if(closeCc)
                    {
                        ied.HoldStatus[cnt].Value = 0;
                        targetArr = new double[4].ToList();
                    }
                    else
                    {
                        ied.HoldStatus[cnt].Value++;
                    }
                }                
                ied.TargetValue[cnt].Value = targetArr.ToArray();
                if (ied.HoldStatus[cnt].Value>0)
                {
                    bool useXxy = sc is CommCollection_KLXxY;
                    ChanceClass cc = useXxy ? new ChanceClass_ForCombinXxY() : new ChanceClass();
                    if (useXxy)
                    {
                        (cc as ChanceClass_ForCombinXxY).AllNums = sc.AllNums;
                        (cc as ChanceClass_ForCombinXxY).SelectNums = sc.SelNums;
                        (cc as ChanceClass_ForCombinXxY).strAllTypeBaseOdds = (sc as CommCollection_KLXxY).strAllTypeOdds;
                        (cc as ChanceClass_ForCombinXxY).strCombinTypeBaseOdds = (sc as CommCollection_KLXxY).strCombinTypeOdds;
                        (cc as ChanceClass_ForCombinXxY).strPermutTypeBaseOdds = (sc as CommCollection_KLXxY).strPermutTypeOdds;
                    }
                    cc.ChanceCode = kv.Label;
                    //Log("结果",cc.ChanceCode);
                    cc.SignExpectNo = ed.Expect;
                    cc.ChanceType = 2;
                    cc.ChipCount = FixChipCnt ? 1 : ChanceClass.getChipsByCode(cc.ChanceCode);
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
                cnt++;
            }
            this.Indexs.Add(ied);
            return ret;
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            if(cc.IncrementType == InterestType.CompoundInterest)
            {
                return 0;
            }
            else
            {
                return cc.FixAmt.Value;
            }
            
        }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }

        public override Type getTheChanceType()
        {
            return typeof(NolimitTraceChance);
        }
        string strModel = "{0}/{1}";
        protected override List<List<StringDoubleData>> calcCurrIndex(BaseCollection sc)
        {
            
            List<List<StringDoubleData>> ret = new List<List<StringDoubleData>>();
            ret.Add(new List<StringDoubleData>());
            ret.Add(new List<StringDoubleData>());
            Dictionary<string, int> fs = sc.getFeatureDic<int>(BySer);
            
            Dictionary<string, int> bigCycleFeatures = fs.ToDictionary(a => a.Key, a => a.Value);
            Dictionary<string, int> lastCycleFeatures = fs.ToDictionary(a => a.Key, a => a.Value);
            List<string> forList = bigCycleFeatures.Keys.ToList();
            foreach (string strkey in forList)
            {
                string[] arr = strkey.Split('/');
                bigCycleFeatures[strkey] = sc.FindLastDataExistCount(ReviewExpectCnt, arr[0], arr[1]);//获取大周期各特征值
                lastCycleFeatures[strkey] = sc.FindLastDataExistCount(InputMaxTimes, arr[0], arr[1]);//获取最近一周期各特征值
            }
            bigCycleFeatures.ToList().ForEach(a => ret[0].Add(new StringDoubleData(a.Key,a.Value)));
            lastCycleFeatures.ToList().ForEach(a => ret[1].Add(new StringDoubleData(a.Key, a.Value)));
            return ret;
        }
        
         Dictionary<string,int> getSoctor(int selNums,int features)
        {
            Dictionary<string, int> fs = new Dictionary<string, int>();
            for (int s = 0; s < selNums; s++)
            {
                for (int f = 0; f < features; f++)
                {
                    fs.Add(string.Format(strModel, s, f), 0);
                }
            }
            return fs;
        }

        protected override bool NeedIn(string label, List<List<double>> features, List<double> currVal,ref List<double> TargetList,int AllNum)
        {
            if(TargetList==null||TargetList.Count<4)
            {
                TargetList = new double[4].ToList();
            }
            double currRCVal = currVal[0];//大周期数
            double currLCVal = currVal[1];//小周期数
            double AllStdDev = ProbMath.CalculateStdDev(features[0]);//历史大周期数标准差
            double AllAvg = features[0].Average();//历史大周期数平均值
            double lastStdDev = ProbMath.CalculateStdDev(features[1]);
            double lastAvg = features[1].Average();
            if(AllAvg > ReviewExpectCnt/AllNum || lastAvg>InputMaxTimes/AllNum)//如果大于平均值，不可信
            {
                return false;
            }
            if (currLCVal< features[1].Min() && currLCVal < lastAvg - 2 * lastStdDev)// && currRCVal<AllAvg-3*AllStdDev)//this.InputMinTimes)//当前大周期值小于历史大周期平均值-N倍历史大周期标准差
            {
                
                if (currRCVal<features[0].Min() && currRCVal<AllAvg-1* AllStdDev)//&& currLCVal < lastAvg - 3 * lastStdDev)//this.InputMinTimes)//小周期当前值也小于小周期历史平均值-N倍小周期历史标准差
                {
                    TargetList = new double[4].ToList();
                    TargetList[0] = AllAvg;
                    TargetList[1] = AllStdDev;
                    TargetList[2] = lastAvg;
                    TargetList[3] = lastStdDev;
                    return true;
                }
            }
            return false;
        }

        protected override bool NeedEnd(string label, List<List<double>> features, List<double> currVal, List<double> TargetList,int allNum,int HoldCnt)
        {
            double currRCVal = currVal[0];//大周期数
            double currLCVal = currVal[1];//小周期数
            
            double lastStdDev = ProbMath.CalculateStdDev(features[1]);
            double lastAvg = features[1].Average();
            double AllAvg = ReviewExpectCnt / allNum;
            double bigAvg = InputMaxTimes / allNum;
            if (HoldCnt < (InputMaxTimes / 2))
            {
                if(currLCVal > lastAvg )
                {
                    return true;
                }
            }
            else
            {
                if (HoldCnt > ReviewExpectCnt)
                {
                    if(currLCVal >= bigAvg)
                    {
                        return true;
                    }
                }
                else
                {
                    if (currLCVal >= bigAvg && currRCVal >= AllAvg)//小周期当前值大于开仓时小周期历史平均值
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

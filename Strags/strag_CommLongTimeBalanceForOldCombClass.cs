using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using GuideLib;
using System.ComponentModel;
namespace Strags
{
    [Serializable]
    [DescriptionAttribute("N码长期概率分布择时组合选号策略"),
        DisplayName("N码长期概率分布择时组合选号策略")]
    public class strag_CommLongTimeBalanceForOldCombClass :ProbWaveSelectStragClass
    {
        public strag_CommLongTimeBalanceForOldCombClass():base()
        {
            _StragClassName = "N码长期概率分布择时组合选号策略";
        }

        public override bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            if (LastExpectMatched)
            {
                return true;
            }
            if (this.IsTracing == false) //如果策略不持仓，无论是否命中，所有后续机会均关闭
            {
                return true;
            }
            strag_CommOldClass coc = new strag_CommOldClass();
            coc.CommSetting = this.CommSetting;
            coc.ChipCount = this.ChipCount;
            coc.FixChipCnt = true;
            ////coc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
            coc.BySer = this.BySer;
            //coc.ReviewExpectCnt = ;
            coc.InputMinTimes = this.InputMinTimes;
            coc.StagSetting = new StagConfigSetting();
            double sucrate = double.NaN;
            if (this.CommSetting.UseLocalWaveData)//如果使用本地数据，获取该期本地数据
            {
                sucrate = this.LocalWaveData[this.LastUseData().LastData.Expect];
            }
            if (sucrate == double.NaN)//如果数据异常，继续去寻找
            {
                sucrate = 100*getSucRate(this.LastUseData(), coc, cc.AllowMaxHoldTimeCnt == 1 ? true : false);
            }
            if (!this.RateDic.ContainsKey(this.LastUseData().LastData.Expect))
            {
                RateDic.Add(this.LastUseData().LastData.Expect, sucrate);
            }
            if (this.CheckEnableOut())
            {
                return true;
            }
            
            return false;
        }

        public override StagConfigSetting getInitStagSetting()
        {
            StagConfigSetting scs = new StagConfigSetting();
            scs.IsLongTermCalc = true;
            scs.BaseType = new BaseTypeSetting();
            //scs.BaseType.IncrementType = InterestType.CompoundInterest;
            scs.BaseType.AllowHoldTimeCnt = -1;//无穷
            scs.BaseType.ChipRate = 0;
            scs.BaseType.traceType = TraceType.WaveTrace;
            return scs;
        }

        

        public override List<PK10CorePress.ChanceClass> getChances(PK10CorePress.CommCollection sc, PK10CorePress.ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            ExpectList el = sc.orgData;
            List<ChanceClass> scs = new List<ChanceClass>();
            strag_CommOldClass coc = new strag_CommOldClass();
            coc.CommSetting = this.CommSetting;
            coc.ChipCount = this.ChipCount;
            coc.FixChipCnt = true;
            //coc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
            coc.BySer = this.BySer;
            //coc.ReviewExpectCnt = ;
            coc.InputMinTimes = this.InputMinTimes;
            coc.StagSetting = new StagConfigSetting();
            if (!this.IsTracing)//未持仓时才计算过往概率
            {
                double sucrate = double.NaN;
                if (this.CommSetting.UseLocalWaveData)//如果使用本地数据，获取该期本地数据
                {
                    sucrate = this.LocalWaveData[el.LastData.Expect];
                }
                if ( double.IsNaN(sucrate))//如果数据异常，继续去寻找
                {
                    sucrate = getSucRate(el, coc, true);//该类策略全部是一次性机会，可反复下注
                }
                if (this.RateDic == null)
                {
                    this.RateDic = new Dictionary<string, double>();
                }
                if (!this.RateDic.ContainsKey(this.LastUseData().LastData.Expect))//加入胜率队列
                {
                    RateDic.Add(this.LastUseData().LastData.Expect, sucrate);
                }
                if (!this.CheckEnableIn())
                {
                    return ret;
                }

                this.IsTracing = true;//满足条件，开始持仓
                //this.debug_maxRate = 0;
            }
            CommCollection scc = new ExpectListProcess(el).getSerialData(InputMinTimes, BySer);
            ret = coc.getChances(scc, el.LastData);
            for (int i = 0; i < ret.Count; i++)
            {
                ret[i].NeedConditionEnd = true;
                ret[i].OnCheckTheChance += CheckNeedEndTheChance;
            }
            return ret;
        }

        double getSucRate(ExpectList el,strag_CommOldClass coc,bool OnlyOnceChance)
        {
            
            
            Dictionary<string, ChanceClass> NoEndChances = new Dictionary<string, ChanceClass>();
            Dictionary<string, ChanceClass> TmpChances = new Dictionary<string, ChanceClass>();
            int MatchTimes = 0;
            int AllTimes = 0;
            for (int i = this.InputMaxTimes; i < el.Count; i++)//因为最后一次数据如果下注就不知道结果，所以最后一次排除下注，只做验证上次结果用
            {
                ExpectList useList = el.getSubArray(i-this.InputMinTimes, this.InputMinTimes);
                ExpectData CurrData = useList.LastData;
                TmpChances = new Dictionary<string, ChanceClass>();
                foreach (string key in NoEndChances.Keys)
                {
                    ChanceClass cc = NoEndChances[key];
                    int MatchCnt = 0;
                    bool Matched = cc.Matched(CurrData, out MatchCnt, false);
                    if (Matched)
                    {
                        if (OnlyOnceChance)//如果是一次性机会
                        {

                            if (cc.HoldTimeCnt == 1)
                            {
                                MatchTimes++;
                                continue;
                            }
                        }
                        else
                        {
                            MatchTimes++;
                        }
                    }
                    else
                    {
                        if (!TmpChances.ContainsKey(key))
                        {
                            cc.HoldTimeCnt++;
                            cc.Closed = false;
                            TmpChances.Add(key,cc);
                            if (!OnlyOnceChance)//如果不是一次性机会，每次机会都加1
                            {
                                AllTimes++;
                            }
                        }
                    }
                }
                if (i == el.Count - 1) 
                    break;//最后一期不找机会
                NoEndChances = new Dictionary<string, ChanceClass>();
                foreach (string key in TmpChances.Keys)//遗留机会
                {
                    NoEndChances.Add(key, TmpChances[key]);
                }
                CommCollection scc = new ExpectListProcess(useList).getSerialData(InputMinTimes, BySer);
                List<ChanceClass> ccs = coc.getChances(scc, CurrData);
                for (int j = 0; j < ccs.Count; j++)//新增机会
                {
                    if (!NoEndChances.ContainsKey(ccs[j].ChanceCode))
                    {
                        NoEndChances.Add(ccs[j].ChanceCode, ccs[j]);
                        AllTimes++;
                    }
                }
                
            }
            return (double)100*MatchTimes / AllTimes;
        }

        public override bool CheckEnableIn()
        {
            double minRate = (double)this.ChipCount / this.CommSetting.Odds;
            double midRate = (double)this.ChipCount / 10;
            double maxRate = 2 * midRate - minRate;
            double lastVal = this.RateDic[this.LastUseData().LastData.Expect];
            if (this.RateDic.Count == 1) 
                return false;
            double preval = this.RateDic[string.Format("{0}",Int32.Parse(this.LastUseData().LastData.Expect) - 1)];
            //布林线验证
            Bull bull = null;
            double[][] res = null;
            double[][] preres = null;
            if (this.UseMainGuides() == null || !this.UseMainGuides().ContainsKey(this.LastUseData().LastData.Expect))
            {
                bull = new Bull(this.RateDic.Values.ToArray<double>(), 20);
                res = bull.getLastData();
                preres = bull.Ref(1);
                if (this.UseMainGuides() == null)
                {
                    this.SetUseMainGuides(new Dictionary<string, MGuide>());
                }
                this.UseMainGuides().Add(this.LastUseData().LastData.Expect, bull);
            }
            else
            {
                bull = this.UseMainGuides()[this.LastUseData().LastData.Expect] as Bull;
                res = bull.getLastData();
                preres = bull.Ref(1);
            }
            
            if (lastVal > 100*maxRate) //如果大于允许进入的最大值，返回否
            {
                return false;
            }
            
            int bullCnt = res[0].Length;
            if (lastVal > res[2][0] && preval < preres[2][0])//当前值上穿下布林线，入场
            {
                return true;
            }
            return false;
        }

        public override bool CheckEnableOut()
        {
            double minRate = (double)this.ChipCount / this.CommSetting.Odds;
            //double midRate = (double)this.ChipCount / 10;
            //double maxRate = 2 * midRate - minRate;
            double lastVal = this.RateDic[this.LastUseData().LastData.Expect];
            
            //进行布林线验证
            Bull bull = null;
            double[][] res = null;
            if (!this.UseMainGuides().ContainsKey(this.LastUseData().LastData.Expect))
            {
                bull = new Bull(this.RateDic.Values.ToArray<double>(), 20);
                res = bull.getLastData();
                this.UseMainGuides().Add(this.LastUseData().LastData.Expect, bull);
            }
            else
            {
                bull = this.UseMainGuides()[this.LastUseData().LastData.Expect] as Bull;
                res = bull.BuffData;
            }
            if (lastVal > 100*minRate) //如果大于保本的最小值，返回真
            {
                return true;
            }
            
            int bullCnt = res[0].Length;
            if (lastVal > res[1][bullCnt - 1])//当前值上穿上布林线，出场
            {
                return true;
            }
            return false;
        }

        public override Type getTheChanceType()
        {
            throw new NotImplementedException();
        }
    }
}

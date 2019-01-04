using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.ComponentModel;
using ProbMathLib;
using GuideLib;
namespace Strags
{
    [DescriptionAttribute("标准差起伏择时选号策略"),
        DisplayName("标准差起伏择时选号策略")]
    [Serializable]
    public class Strag_StdDevWaveClass:TotalStdDevTraceStragClass,ITraceChance
    {
        
        public Strag_StdDevWaveClass():base()
        {
            _StragClassName = "标准差起伏择时选号策略";
        }

        public override bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {

            return LastExpectMatched;//持续机会
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            try
            {
                if (cc.HoldTimeCnt > cc.MaxHoldTimeCnt && cc.MaxHoldTimeCnt>0)//但是只有第一次有投入
                {
                    return 0;
                }
                if (cc.IncrementType == InterestType.CompoundInterest)
                {
                    double rate = KellyMethodClass.KellyFormula(cc.ChipCount, 10, 9.75, 1.001);
                    //double rate = KellyMethodClass.KellyFormula(cc.ChipCount, 9.75, 0.3396);
                    long ret = (long)Math.Floor((double)(RestCash * rate) / cc.ChipCount);
                    return ret;
                }
                int chips = cc.ChipCount - 1;
                int maxcnt = amts.MaxHoldCnts[chips];
                int bShift = 0;
                if (cc.HoldTimeCnt > maxcnt)
                {
                    Log("风险", "达到最大上限", string.Format("机会{0}持有次数达到{1}次总投入金额已为{2}", cc.ChanceCode, cc.HoldTimeCnt, cc.Cost));
                    bShift = (int)maxcnt * 2 / 3;
                }
                int RCnt = (cc.HoldTimeCnt % (maxcnt + 1)) + bShift - 1;
                return amts.Serials[chips][RCnt];
            }
            catch
            {
            }
            return 0;
        }

        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            if (sc == null || sc.Table == null || sc.Table.Rows.Count < this.ReviewExpectCnt)
                return ret;
            if (this.getAllStdDev() == null)
            {
                this.setAllStdDev(new Dictionary<string, List<double>>());
            }
            double stdval = 0;
            List<double> list = sc.getAllDistrStdDev(this.ReviewExpectCnt, this.InputMaxTimes);
            if (!this.getAllStdDev().ContainsKey(ed.Expect))
            {
                stdval = list[10];//最后一位
                this.getAllStdDev().Add(ed.Expect, list);
            }
            if (getAllStdDev().Count < 20)
                return ret;
            MA ma20 = new MA(getAllStdDev().Values.Select(p=>p[10]).ToArray(), 20);
            MA ma5 = new MA(getAllStdDev().Values.Select(p => p[10]).ToArray(), 5);
            if (!IsTracing)
            {
                //if (stdval > 0.2) return ret;
                if (ma20.IsDownCross())//下穿均线，开始跟踪
                {
                    IsTracing = true;
                }
                else
                {
                    return ret;
                }
            }
            else
            {
                ////if (stdval > 0.2)
                ////{
                ////    IsTracing = false;
                ////    return ret;
                ////}
                if (ma5.IsUpCross())//上穿均线，停止跟踪
                {
                    IsTracing = false;
                    return ret;
                }
            }
            strag_CommOldClass strag = new strag_CommOldClass();
            strag.BySer = this.BySer;
            strag.ChipCount = this.ChipCount;
            strag.InputMinTimes = this.InputMinTimes;
            List<ChanceClass> tmp =  strag.getChances(sc, ed);
            Dictionary<string,double> dic = new Dictionary<string,double>();
            for(int i=0;i<10;i++)
            {
                if(list[i] < stdval)//小于整体的才有效
                {
                    string strcol = string.Format("{0}", (i + 1) % 10);
                    dic.Add(strcol, list[i]);
                }
            }
            for (int i = 0; i < tmp.Count; i++)
            {
                if(dic.ContainsKey(tmp[i].GetCodeKey(this.BySer)))
                {
                    ret.Add(tmp[i]);
                }
            }
            return ret;
        }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }

        public override Type getTheChanceType()
        {
            return typeof(ChanceClass);
        }
    }
}

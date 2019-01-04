using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.Data;
using ProbMathLib;
namespace Strags
{
    public class strag_CommLongMissBackBalanceClass : ChanceTraceStragClass, IProbCheckClass
    {
        public strag_CommLongMissBackBalanceClass()
            : base()
        {
        }
        public override bool CheckNeedEndTheChance(ChanceClass cc,bool LastExpectMatched)
        {
            if (!LastExpectMatched) return false;
            double p = (double)cc.ChipCount / this.CommSetting.Odds ;
            double Normal_p = (double)cc.ChipCount / 10;
            double _MinRate = Normal_p + this.MinWinRate * (p - Normal_p);
            if (cc.MatchChips > _MinRate * cc.HoldTimeCnt)
            {
                return true;
            }
            if (10 / cc.ChipCount > cc.HoldTimeCnt)
            {
                return true;
            }
            return false;
        }
        public override List<PK10CorePress.ChanceClass> getChances(PK10CorePress.CommCollection sc, PK10CorePress.ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            DataTableEx dt = null;
            if (this.BySer)
            {
                dt = sc.SerialDistributionTable;
            }
            else
            {
                dt = sc.CarDistributionTable;
            }
            if (dt == null)
            {
                throw new Exception("无法获得概率分布表！");
            }
            int MatchCnt = 0;
            string strAllCodes = "";
            double MinSucRate = 0;
            if (this.FixChipCnt)
            {
                MinSucRate = this.CommSetting.Odds / this.ChipCount; //指定注数需要的最小胜率
            }
            for (int i = 0; i < 10; i++)
            {
                //获得各项的最小的
                List<double> coldata = null;
                string strCol = string.Format("{0}", (i + 1) % 10);
                
                string strVal = ed.ValueList[i];
                int ExistCnt = sc.FindLastDataExistCount(this.InputMinTimes,strCol,strVal);
                if (ExistCnt > 1)//前n次不是最后一次才出现
                {
                    continue;
                }
                dt.getColumnData(strCol, ref coldata);
                double avgval = coldata.Average();
                double stdval = ProbMath.CalculateStdDev(coldata);
                string strSql = string.Format("[{0}]={1}", "Id", strVal);
                string strSort = string.Format("[{0}] asc", "Id");
                DataRow[] drs = dt.Select(strSql, "");
                if (drs.Length != 1)
                    throw new Exception("概率数据表错乱！");
                int InAllViewExistCnt = int.Parse(drs[0][strCol].ToString());//前100（指定的viewcnt）期出现的次数
                if (InAllViewExistCnt > avgval - stdval * this.StdvCnt)//如果前100期内出现的概率大于指定的标准差数，跳过
                {
                    continue;
                }
                if(InAllViewExistCnt >  this.ReviewExpectCnt*(1-MinSucRate))//如果成功数小于对应注数的失败数
                {
                    continue;
                }
                string strCode = string.Format("{0}/{1}", BySer ? strCol : strVal, BySer ? strVal : strCol);

                MatchCnt++;
                strAllCodes = string.Format("{0}{1}{2}", strAllCodes, MatchCnt > 1 ? "+" : "", strCode);
            }
            if (MatchCnt < this.ChipCount)
            {
                return ret;
            }
            ChanceClass cc = new ChanceClass();
            cc.SignExpectNo = ed.Expect;
            cc.ChanceType = 3;
            cc.InputTimes = 1;
            cc.strInputTimes = "1";
            cc.AllowMaxHoldTimeCnt = 1;
            cc.InputExpect = ed;
            cc.ChipCount = MatchCnt;
            cc.ChanceCode = strAllCodes;
            cc.CreateTime = ed.OpenTime;
            cc.NeedConditionEnd = true;
            cc.OnCheckTheChance += CheckNeedEndTheChance;
            cc.Closed = false;
            ret.Add(cc);
            return ret;
        }



        public override  StagConfigSetting getInitStagSetting()
        {
            StagConfigSetting scs = new StagConfigSetting();
            scs.BaseType = new BaseTypeSetting();
            //scs.BaseType.IncrementType = InterestType.CompoundInterest;
            scs.BaseType.AllowHoldTimeCnt = -1;//无穷
            scs.BaseType.ChipRate = 0;
            return scs;
        }

        public override Int64 getChipAmount(double RestCash, ChanceClass cc, AmoutSerials ams)
        {
            if(cc.IncrementType==  InterestType.SimpleInterest) return (int)Math.Floor(this.CommSetting.InitCash*0.01);
            double p = (double)(cc.ChipCount / this.CommSetting.Odds);
            double Normal_p = (double)cc.ChipCount / 10;
            double _MinRate = Normal_p + this.MinWinRate * (p - Normal_p);
            p = _MinRate;
            double b = (double)this.CommSetting.Odds;
            double q = 1 - p;
            double rate = (p * b - q) / b/10;
            return (int)Math.Floor(RestCash * rate / cc.ChipCount);
        }
        double _stdcnt;
        public double StdvCnt
        {
            get
            {
                return _stdcnt;
            }
            set
            {
                _stdcnt = value;
            }
        }

        public override Type getTheChanceType()
        {
            throw new NotImplementedException();
        }
    }
}

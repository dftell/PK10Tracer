using System;
using System.Collections.Generic;
using System.Linq;
using WolfInv.com.PK10CorePress;
using WolfInv.com.GuideLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.Strags
{
    public abstract class ProbWaveSelectStragClass : ChanceTraceStragClass
    {
        
        protected Dictionary<string, double> RateDic;
        public GuideResult LocalWaveData;
        Dictionary<string, MGuide> _UseMainGuides;
        public Dictionary<string, MGuide> UseMainGuides()
        {
            return _UseMainGuides;
        }

        public void SetUseMainGuides(Dictionary<string, MGuide> value)
        {
            _UseMainGuides = value;
        }
        public override bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            return LastExpectMatched;
        }
        Dictionary<string, Int64> _UseAmountList = new Dictionary<string,long>();
        public Dictionary<string, Int64> UseAmountList()
        {
            return _UseAmountList;
        }
        public void UseAmountList(Dictionary<string, Int64> value)
        {
            _UseAmountList = value;
        }
        public GuideResultSet BaseWaves()
        {
            //get
            //{
            return new GuideResultSet(RateDic);
            //}
        }

        public GuideResultSet GuideWaves()
        {
            //get
            //{
                GuideResultSet ret = new GuideResultSet();
                if (UseMainGuides() == null || UseMainGuides().Count == 0) return ret;
                int len = UseMainGuides().Last().Value.CurrValues.Length;
                for (int i = 0; i < len; i++)
                {
                    Dictionary<string, double> dic = new Dictionary<string, double>();
                    foreach (string key in UseMainGuides().Keys)
                    {
                        dic.Add(key, UseMainGuides()[key].CurrValues[i]);
                    }
                    ret.NewTable(dic);
                }
                return ret;
            //}
        }

        public abstract bool CheckEnableIn();
        public abstract bool CheckEnableOut();

        /// <summary>
        /// 统一下注数量
        /// </summary>
        /// <param name="RestCash"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        public override double getChipAmount(double RestCash, ChanceClass cc, AmoutSerials ams)
        {
            ////if (this.UseAmountList.ContainsKey(this.LastUseData.LastData.Expect))
            ////{
            ////    //return this.UseAmountList[this.LastUseData.LastData.Expect];
            ////}
            if (cc.IncrementType == InterestType.SimpleInterest) return (int)Math.Floor(this.CommSetting.InitCash * 0.01);
            double p = (double)(cc.ChipCount / this.CommSetting.Odds);
            double Normal_p = (double)cc.ChipCount / 10;
            double _MinRate = Normal_p + this.MinWinRate * (p - Normal_p);
            p = _MinRate;
            double b = (double)this.CommSetting.Odds;
            double q = 1 - p;
            double rate = (p * b - q) / b;
            double cs = Math.Sqrt(this.CommSetting.MaxHoldingCnt);
            Int64 AllAmount = (Int64)Math.Floor(RestCash * rate / cc.ChipCount);
            //this.UseAmountList.Add(this.LastUseData.LastData.Expect, AllAmount);
            return AllAmount;
        }

        
    }
}

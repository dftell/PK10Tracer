using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PK10CorePress;

namespace Strags
{
    [DescriptionAttribute("通用跳转选号策略"),
        DisplayName("通用跳转选号策略")]
    [Serializable]
    public class strag_CommJumpClass : ChanceTraceStragClass
    {
        [DescriptionAttribute("当前持仓次数"),
        DisplayName("当前持仓次数"),
        CategoryAttribute("持仓属性")]
        public int HoldCnt { get; set; }
        int RealCnt;

        public strag_CommJumpClass():base()
        {
            _StragClassName = "通用跳转选号策略";
        }

        public override bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            cc.HoldTimeCnt = HoldCnt;
            if (LastExpectMatched)
            {
                HoldCnt = 0;
            }
            else
            {
                //HoldCnt++;
            }
            
            return true;
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            return amts.Serials[0][HoldCnt-1];
            
        }

        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            if (sc == null || sc.Table == null || sc.Table.Rows.Count < this.ReviewExpectCnt)
                return ret;
            DataTableEx dt = sc.CarTable;
            if (dt == null)
            {
                throw new Exception("无法获得概率分布表！");
            }
            //Log("最后一期数据", string.Join(",", dt.Rows[0].ItemArray));
            //Log("最后二期数据", string.Join(",", dt.Rows[1].ItemArray));
            string val = dt.Rows[dt.Rows.Count-1][string.Format("{0}", InputMinTimes)].ToString();
            OnceChance cc = new OnceChance();
            cc.SignExpectNo = ed.Expect;
            cc.ChanceType = 1;
            string code_model = "{0}/{1}";
            HoldCnt++;
            cc.ChanceCode = BySer ? string.Format(code_model, InputMaxTimes, val) : string.Format(code_model, val, InputMaxTimes);
            cc.ChipCount = 1;//码数必须是实际码数，使用的金额队列必须是1码的队列
            cc.InputTimes = HoldCnt;
            cc.strInputTimes = string.Format("{0}", cc.InputTimes);
            cc.AllowMaxHoldTimeCnt = 1;
            cc.InputExpect = ed;
            cc.StragId = this.GUID;
            cc.NeedConditionEnd = true;//这是必须的，才能在整体回测中触发CheckNeedEndTheChance
            //cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
            cc.IsTracer = 0;
            cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
            //cc.UnitCost = this.getChipAmount(0,cc,);
            ret.Add(cc);
            return ret;
        }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }

        public override Type getTheChanceType()
        {
            return typeof(OnceChance);
        }
    }
}

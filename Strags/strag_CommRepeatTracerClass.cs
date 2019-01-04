using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.Data;
using ProbMathLib;
using System.ComponentModel;
namespace Strags
{
    [DescriptionAttribute("通用重复号码跟踪策略"),
        DisplayName("通用重复号码跟踪策略")]
    public class strag_CommRepeatTracerClass : ChanceTraceStragClass
    {
        [DescriptionAttribute("当前持仓次数"),
        DisplayName("当前持仓次数"),
        CategoryAttribute("持仓属性")]
        public int HoldCnt { get; set; }
        int RealCnt;
        int CurrChancesCnt=0;
        public strag_CommRepeatTracerClass()
            : base()
        {
            _StragClassName = "通用重复号码跟踪策略";
        }
        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            CurrChancesCnt = 0;//置零，防止后面留存
            if (this.LastUseData().Count < ReviewExpectCnt)
            {
                Log("基础数据数量不足", string.Format("小于回览期数:{0}",ReviewExpectCnt));
                return ret;
            }
            ExpectList LastDataList = this.LastUseData();
            ExpectData PreData = LastDataList[LastDataList.Count - 2];
            //Log(string.Format("前期{0}:{1}", PreData.Expect, PreData.OpenCode), string.Format("当期{0}:{1}", LastDataList.LastData.Expect, LastDataList.LastData.OpenCode));
            //Log(string.Format("el数据长度:{0},First:{1};Last{2}", LastDataList.Count,LastDataList.FirstData.Expect,LastDataList.LastData.Expect), string.Format("原始数据长度:{0};First:{1};Last:{2}", sc.orgData.Count,sc.orgData.FirstData.Expect,sc.orgData.LastData.Expect));
            DataTableEx dt = sc.getSubTable(sc.orgData.Count - this.ReviewExpectCnt, this.ReviewExpectCnt);
            List<string> strCodes = new List<string>();
            for(int i=0;i<10;i++)//遍历每个车号/名次
            {
                List<int> coldata = null;
                string strCol = string.Format("{0}", (i + 1) % 10);
                dt.getColumnData(strCol, ref coldata);
                //Log(string.Format("车/次:{0}",strCol), string.Format("取得的数据:{0}",string.Join(",",coldata)));
                int RepeatCnt = 0;
                for (int j = 1; j < ReviewExpectCnt; j++)
                {
                    if (coldata[j] == coldata[j - 1])
                        RepeatCnt++;
                }
                if (RepeatCnt < this.ReviewExpectCnt - 1)//如果重复次数小于回顾次数减一，表示重复次数不够，跳过
                {
                    continue;
                }
                string strCode = "";//其实无需比较，对单个车/名次来说，矩阵都一样,策略只需建立一个即可
                strCode = string.Format("{0}/{1}",strCol,coldata[0]);
                
                //Log(string.Format("车/次:{0}", strCol), strCode);
                strCodes.Add(strCode);
                
                if(HoldCnt >=0) //当持有次数超过指定次数后，不再增加
                    HoldCnt++;//持有次数加1
                //Log("获得机会处理", string.Format("当前持有次数：{0}", HoldCnt));
                RealCnt++;
            }
            
            if (strCodes.Count == 0)//机会数为0
                return ret;
            if (!GetRev)
            {
                OnceChance cc = new OnceChance();
                cc.SignExpectNo = ed.Expect;
                cc.ChanceType = 1;
                cc.ChanceCode = string.Join("+", strCodes);
                cc.ChipCount = strCodes.Count;//码数必须是实际码数，使用的金额队列必须是1码的队列
                cc.InputTimes = RealCnt;
                cc.strInputTimes = string.Format("{0}", cc.InputTimes);
                cc.AllowMaxHoldTimeCnt = 1;
                cc.InputExpect = ed;
                cc.NeedConditionEnd = true;
                cc.StragId = this.GUID;
                //cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                cc.IsTracer = 0;
                cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                CurrChancesCnt = strCodes.Count;
                //cc.UnitCost = this.getChipAmount(0,cc,);
                ret.Add(cc);
                //获得所有机会后，统一为本次机会分配资金
                return ret;
            }
            else
            {
                for (int i = 0; i < strCodes.Count; i++)
                {
                    NolimitTraceChance cc = new NolimitTraceChance();
                    cc.SignExpectNo = ed.Expect;
                    cc.ChanceType = 1;
                    cc.ChanceCode = ChanceClass.getRevChance(strCodes[i]);
                    cc.ChipCount = 9;//码数必须是实际码数，使用的金额队列必须是1码的队列
                    cc.InputTimes = RealCnt;
                    cc.strInputTimes = string.Format("{0}", cc.InputTimes);
                    cc.AllowMaxHoldTimeCnt = 1;
                    cc.InputExpect = ed;
                    cc.StragId = this.GUID;
                    //cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                    cc.IsTracer = 1;
                    cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                    CurrChancesCnt = strCodes.Count;
                    //cc.UnitCost = this.getChipAmount(0,cc,);
                    ret.Add(cc);
                }
                return ret;
            }
        }

        public override StagConfigSetting getInitStagSetting()
        {
            StagConfigSetting scs = new StagConfigSetting();
            scs.BaseType = new BaseTypeSetting();
            ////if (CommSetting != null)
            ////{
            ////    scs.BaseType.AllowHoldTimeCnt = 1000;// CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxHoldCnts[0];
            ////}
            return scs;
        }

        public override Type getTheChanceType()
        {
            return typeof(OnceChance);
        }
        
        public override bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched) //检查需要关闭时（能否保证后面的实例是同一个？）
        {
            cc.HoldTimeCnt = RealCnt;
            if (LastExpectMatched)
            {
                this.RealCnt = 0;
                this.HoldCnt = 0;
            }
            return true;// LastExpectMatched;
            if (LastExpectMatched)//如果任何一个机会命中了，将持有次数置0
            {
                this.HoldCnt = 0;
                RealCnt = 0;
                Log("有结果命中", string.Format("持有次数:{0};允许机会持有的最大次数:{1}", this.HoldCnt, cc.MaxHoldTimeCnt));
            }
            if (this.HoldCnt > cc.MaxHoldTimeCnt)//如果没有命中，但是超出了最大持仓次数
            {
                Log("持有次数大于机会的最大次数", string.Format("持有次数:{0};允许机会持有的最大次数:{1}", this.HoldCnt, cc.MaxHoldTimeCnt));
                this.HoldCnt = -1;
            }
            return true;//一次性机会，无论如何都关闭
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            int chips = 0;
            int maxcnt = amts.MaxHoldCnts[chips];
            int bShift = 0;
            int eShift = 0;
            int bHold = HoldCnt - CurrChancesCnt+1;
            if (cc.IncrementType == InterestType.CompoundInterest)
            {
                if (cc.AllowMaxHoldTimeCnt > 0 && cc.HoldTimeCnt > cc.AllowMaxHoldTimeCnt)
                {
                    return 0;
                }
                return (long)Math.Floor(cc.FixRate.Value*RestCash/cc.ChipCount);
            }
            //Log("获取机会金额处理", string.Format("当前持有次数：{0}", HoldCnt));
            if (HoldCnt == -1)//如果超出指定的最大持仓次数，跟踪不投注，直到实现后继续跟踪
            {
                return 0;
            }
            if (bHold > maxcnt)
            {
                Log("风险", "通用重复策略开始次数达到最大上限", string.Format("机会{0}持有次数达到{1}次总投入金额已为{2}", cc.ChanceCode, bHold, "未知"));
                bShift = (int)maxcnt * 2 / 3;
            }
            if (HoldCnt > maxcnt)
            {
                Log("风险", "通用重复策略结束次数达到最大上限", string.Format("机会{0}持有次数达到{1}次总投入金额已为{2}", cc.ChanceCode, HoldCnt, "未知"));
                eShift = (int)maxcnt * 2 / 3;
            }
            
            int bRCnt = (bHold % (maxcnt + 1)) + bShift-1;
            int eRCnt = (HoldCnt % (maxcnt + 1)) + eShift - 1;
            if (CurrChancesCnt < 4)//如果是4码以下取平均值
            {
                return (amts.Serials[chips][bRCnt] + amts.Serials[chips][eRCnt]) / 2;
            }
            //四码以上取最大值，防止投入不够导致亏损
            return amts.Serials[chips][eRCnt];

        }
    }
}

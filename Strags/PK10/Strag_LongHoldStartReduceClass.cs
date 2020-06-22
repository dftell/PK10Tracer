using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;
using WolfInv.com.SecurityLib;
using System.Data;

namespace WolfInv.com.Strags
{
    [DescriptionAttribute("长期持有开始减少策略"),
        DisplayName("长期持有开始减少策略")]
    [Serializable]
    public class Strag_LongHoldStartReduceClass : StragClass
    {
        
        public Strag_LongHoldStartReduceClass():base()
        {
            _StragClassName = "久旱逢甘露策略";
        }

        public override bool CheckNeedEndTheChance(ChanceClass cc1, bool LastExpectMatched1)
        {
            return LastExpectMatched1;
        }

        public override List<ChanceClass> getChances(BaseCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            int minRow = this.CommSetting.GetGlobalSetting().MutliColMinTimes;
            int stringLen = this.ChipCount;
            int minHoldCnt = this.MaxHoldCnt;
            int loopCnt = this.InputMaxTimes;
            int minCnt = this.InputMinTimes;
            stringLen = 3;
            minHoldCnt = 15;
            loopCnt = 10;
            minCnt = 1;
            Dictionary<int, List<int>> preChances = getReduceTimes(this.UsingDpt, sc, minRow, 1 , 2, 1, this.BySer);
            if(preChances.Where(a=>a.Value.Count==1).Count()<2)//整体基本无变化，就不要看了。
            {
                return ret;
            }
            

            Dictionary<int, List<int>> colChances = getReduceTimes(this.UsingDpt, sc,minRow, loopCnt, stringLen, minHoldCnt, this.BySer);
            var chances = colChances.Where(a => a.Value.Count >= minCnt && a.Value.Count<=loopCnt/5 && a.Value[0]<=1); //大于一定次数，少于检查次数的1/5，并且最后一次必须是在最近1/3的区间
            
            StringBuilder allCode = new StringBuilder() ;

            List<string> list = new List<string>();
            MergeChances = true;
            foreach (var item in chances)
            {
                if(item.Value==null)
                {
                    continue;
                }
                string currVal = sc.Table.Rows[minRow+1][item.Key].ToString().Trim();
                if (currVal.Length < stringLen || currVal.Length>5)
                    continue;
                if (MergeChances)
                    allCode = new StringBuilder();
                //string strCode = this.BySer ? string.Format("{0}/{1}", item.Key, currVal) : string.Format("{0}/{1}", currVal, item.Key);
                allCode.AppendFormat("+{0}/{1}", this.BySer ? new object[] { (item.Key+1)%10, currVal } : new object[] { currVal, (item.Key+1)%10 });
                if(!MergeChances)
                    list.Add(allCode.ToString());
            }
            if (MergeChances)
                list.Add(allCode.ToString());
            for (int i=0;i<list.Count;i++)
            {
                string code = list[i];
                if (code.Trim().Length == 0)
                    continue;
                if(code.Trim().StartsWith("+"))
                {
                    code = code.Trim().Substring(1);
                }
                ChanceClass cc = new ChanceClass();
                cc.SignExpectNo = ed.Expect;
                cc.ChanceType = 0;
                cc.ChipCount = ChanceClass.getChipsByCode(code);
                cc.ChanceCode = code;// this.BySer ? string.Format("{0}/{1}", item.Key, currVal) : string.Format("{0}/{1}", currVal, item.Key);
                cc.InputTimes = 1;
                cc.strInputTimes = "1";
                //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                cc.InputExpect = ed;
                cc.StragId = this.GUID;
                cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                cc.IsTracer = 1;
                cc.HoldTimeCnt = 1;
                cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                cc.ExpectCode = ed.Expect;
                
                cc.CreateTime = DateTime.Now;
                cc.UpdateTime = DateTime.Now;
                cc.Closed = false;
                ret.Add(cc);
            }
            
            return ret;
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            int holdTimes = (int)DataReader.getInterExpectCnt(cc.ExpectCode, this.LastUseData().LastData.Expect,this.UsingDpt);
            if (holdTimes > 0)
                return 0;
            if(cc.IncrementType == InterestType.CompoundInterest)
            {
                long ret = (long)Math.Ceiling((double)(RestCash * cc.FixRate));
                return ret;
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

        public static Dictionary<int,List<int>> getReduceTimes(DataTypePoint dtp, BaseCollection sc,int minRows,int checkPastTerms,int Chips,int MinHoldTimes,bool BySer)
        {
            WolfInv.com.PK10CorePress.BaseCollection lsc = sc as WolfInv.com.PK10CorePress.BaseCollection;
            ExpectList<TimeSerialData> el = lsc.orgData;
            ExpectList<TimeSerialData> useList = el;
            int minRow = minRows;
            BaseCollection<TimeSerialData> useSc = sc;
            int loopCnt = checkPastTerms;
            Dictionary<int, List<int>> colChances = new Dictionary<int, List<int>>();
            for (int i = 0; i < loopCnt; i++)
            {
                Dictionary<int, string> lastVals = new Dictionary<int, string>();
                for (int col = 0; col < 10; col++)
                {
                    string currVal = useSc.Table.Rows[minRow + 1][col].ToString();//第10次1~0一直未出现的值
                    if (currVal.Length < Chips)//如果最后长度小于长度，不管它
                        continue;
                    lastVals.Add(col, currVal);
                }
                useList.RemoveAt(useList.Count - 1);//将第一个节点移除
                ExpectListProcessBuilder<TimeSerialData> elp = new ExpectListProcessBuilder<TimeSerialData>(dtp, el);
                useSc = elp.getProcess().getSerialData(180, BySer);
                LongDrgTableProcessor ldtp = new LongDrgTableProcessor(useSc.Table, minRow);
                for (int col = 0; col < 10; col++)
                {
                    if (!lastVals.ContainsKey(col))//如果这列早就过滤了，根本就不考虑了。
                    {
                        continue;
                    }
                    
                    string lastVal = lastVals[col];
                    string currVal = useSc.Table.Rows[minRow + 1][col].ToString();//第10次1~0一直未出现的值
                    string sameString = ChanceClass.getSameString(lastVals[col], currVal);
                    string reduceChar = ChanceClass.ReduceString(currVal, sameString);
                    
                    int tabcol = (col + 1) % 10;
                    Dictionary<int, LongDrgNumberInfo> currColInfo = ldtp.AllLongDrgInfs[tabcol];
                    if (lastVal.Length < Chips)//前一个值未出现个数太少，跳过
                    {
                        continue;
                    }
                    if ( currVal.Contains(lastVal))//后面一个正在增加，肯定要过滤，不算
                    {
                        continue;
                    }
                    if (currColInfo.Where(a => {
                        if (a.Value.displayLong > MinHoldTimes)
                            return true;
                        return false;
                    }).Count()<Chips)//如果超过持续长度的数字小于注数
                    {
                        continue;
                    }
                    if(reduceChar.Length>0 && currColInfo[int.Parse(reduceChar)].Long<MinHoldTimes)//爆出的并不是长龙的数字，照样忽略
                    {
                        continue;
                    }                    
                    if (!colChances.ContainsKey(col))
                    {
                        colChances.Add(col, new List<int>());
                    }
                    colChances[col].Add(i);//前第几次有缩减
                }
            }
            return colChances;
        }

        
        
    }

    
}

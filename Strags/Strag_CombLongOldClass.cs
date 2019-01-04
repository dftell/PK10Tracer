using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.ComponentModel;
namespace Strags
{
    [DescriptionAttribute("长期组合跟踪策略"),
        DisplayName("长期组合跟踪策略")]
    [Serializable]
    public class Strag_CombLongOldClass:StragClass
    {
        
        public Strag_CombLongOldClass():base()
        {
            _StragClassName = "长期组合跟踪策略";
        }
        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            Dictionary<string, string> MatchCols = new Dictionary<string, string>();
            Dictionary<string, int> MatchCnt = new Dictionary<string, int>();
            int MutliMinCnt = this.CommSetting.GetGlobalSetting().SingleColMinTimes;
            int TwoMinCnt = (this.CommSetting.GetGlobalSetting().MinTimeForChance(1) + this.CommSetting.GetGlobalSetting().MinTimeForChance(2)) / 2;
            string colmodel = "{0}_{1}";//列_长度
            for (int i = 0; i < 10; i++)
            {
                int ishift = MutliMinCnt;
                if (!sc.Data[i].ContainsKey(ishift) || sc.Data[i][ishift].Trim().Length == 0 )//不及最小值，跳过
                {
                    continue;
                }
                string strCol = string.Format(colmodel, i, sc.Data[i][ishift].Length);
                if (!MatchCols.ContainsKey(strCol))
                {
                    MatchCols.Add(strCol, sc.Data[i][ishift]);
                    MatchCnt.Add(strCol, ishift + 1);//实际数量
                }
                ishift++;
                while (sc.Data[i].ContainsKey(ishift) &&  sc.Data[i][ishift].Trim().Length >0 )//一直往上找
                {
                    string lastVal = sc.Data[i][ishift];//可能不止一个数字
                    strCol = string.Format(colmodel, i, lastVal.Length);
                    if (!MatchCols.ContainsKey(strCol))
                    {
                        MatchCols.Add(strCol, lastVal);
                        MatchCnt.Add(strCol, ishift + 1);//实际数量
                    }
                    else
                    {
                        MatchCnt[strCol] = MatchCnt[strCol] + 1;
                    }
                    ishift++;
                }
                //ishift--;

            }
            //替换列中数据长度大于1的数据，只留最短的
            for (int i = 0; i < 10; i++)
            {
                string keymodel = "{0}_{1}";
                string key = string.Format(keymodel , i,1);
                if (!MatchCols.ContainsKey(key))//如果1个都不存在，跳过
                {
                    continue;
                }
                string strVal = MatchCols[key];
                int k = 2;
                key = string.Format(keymodel, i,k);
                while (MatchCols.ContainsKey(key))//逐级替换
                {
                    string tmp = MatchCols[key];
                    MatchCols[key] = MatchCols[key].Replace(strVal, "");
                    strVal = tmp;
                    k++;
                    key = string.Format(keymodel, i, k);
                }
            }
            if(MatchCols.Count>1)
                Log("50以上的组合", string.Join("+", MatchCols.Select(p => string.Format("{0}/{1}:{2}", p.Key, p.Value,MatchCnt[p.Key])).ToArray()));
            if (MatchCols.Count < 2) return ret;
            List<string> strCodes = new List<string>();
            List<string> strCodes2 = new List<string>();
            List<int> intCnt2 = new List<int>();
            foreach (string key in MatchCnt.Keys)
            {
                string[] keys = key.Split('_');
                string strCode = string.Format("{0}/{1}", (int.Parse(keys[0]) + 1) % 10, MatchCols[key]);
                if (MatchCnt[key] > TwoMinCnt)//如果2个数小于58
                {
                    strCodes2.Add(strCode);
                    intCnt2.Add(MatchCnt[key]);
                }
                strCodes.Add(strCode);
            }
            if (strCodes.Count == 2)
            {
                if(strCodes2.Count < 2)
                    return ret;
            }
            List<string> strCommCodes = new List<string>();
            List<string> strCommCnts = new List<string>();
            if (strCodes2.Count >= 2) //合成2个的组合
            {
                Log("50以上机会：",string.Join("+",strCodes2.ToArray()));
                for (int i = 0; i < strCodes2.Count; i++)
                {
                    for (int j = i + 1; j < strCodes2.Count; j++)
                    {
                        strCommCodes.Add(string.Format("{0}+{1}", strCodes2[i], strCodes2[j]));
                        strCommCnts.Add(string.Format("{0}_{1}", intCnt2[i], intCnt2[j]));
                    }
                }
            }
            for (int i = 0; i < strCommCodes.Count; i++)
            {
                NolimitTraceChance cc = new NolimitTraceChance();
                cc.SignExpectNo = ed.Expect;
                cc.ChanceType = 0;
                cc.ChipCount = 2;
                cc.InputTimes = MatchCnt.Values.Min<int>();
                cc.strInputTimes = strCommCnts[i];
                //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                cc.InputExpect = ed;
                cc.StragId = this.GUID;
                cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                cc.IsTracer = 1;
                cc.HoldTimeCnt = 1;
                cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                cc.ExpectCode = ed.Expect;
                cc.ChanceCode = strCommCodes[i];
                cc.CreateTime = DateTime.Now;
                cc.UpdateTime = DateTime.Now;
                cc.Closed = false;
                ret.Add(cc);
            }
            if (strCodes.Count > 2)
            {
                string strAllCodes = string.Join("+", strCodes.ToArray());
                NolimitTraceChance cc = new NolimitTraceChance();
                cc.SignExpectNo = ed.Expect;
                cc.ChanceType = 0;
                cc.ChipCount = strCodes.Count;
                cc.InputTimes = MatchCnt.Values.Min<int>();
                cc.strInputTimes = string.Join("_",MatchCnt.Values.ToArray<int>());
                //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                cc.InputExpect = ed;
                cc.StragId = this.GUID;
                cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                cc.IsTracer = 1;
                cc.HoldTimeCnt = 1;
                cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                cc.ExpectCode = ed.Expect;
                cc.ChanceCode = strAllCodes;
                cc.CreateTime = DateTime.Now;
                cc.UpdateTime = DateTime.Now;
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

        


        
    }
}

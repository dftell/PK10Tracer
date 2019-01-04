using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using Strags;
namespace BackTestLib
{
    
    public class ExchanceClass
    {
        Dictionary<string, ChanceClass> tmpChances = null;
        public void Run(ExpectList testData, StragClass teststrag, ref List<ChanceClass> ChanceList, ref Dictionary<string, ChanceClass> NoCloseChances, ref Dictionary<int, int> HoldCntDic)
        {
            tmpChances = new Dictionary<string, ChanceClass>();
            if (ChanceList == null) ChanceList = new List<ChanceClass>();
            CommCollection sc = new ExpectListProcess(testData).getSerialData(teststrag.ReviewExpectCnt, teststrag.BySer);
            foreach (string key in NoCloseChances.Keys)
            {
                ChanceClass cc = NoCloseChances[key];
                if (cc.Closed == false)
                {
                    int matchcnt = 0;
                    if (teststrag.GetRev)//如果求相反组合
                    {
                        if (cc.Matched(testData.LastData, out matchcnt, true))//不关闭
                        {
                            cc.HoldTimeCnt = (int)(testData.LastData.ExpectIndex - cc.InputExpect.ExpectIndex);
                        }
                    }
                    if (cc.Matched(testData.LastData, out matchcnt, false))//如果用相反组合，不改变真正关闭
                    {
                        cc.Closed = true;
                        cc.EndExpectNo = testData.LastData.Expect;
                        if (!teststrag.GetRev)//只有不求相反值的情况下，才赋持有是次数
                        {
                            cc.HoldTimeCnt = (int)(testData.LastData.ExpectIndex - cc.InputExpect.ExpectIndex);
                        }
                        cc.MatchChips = matchcnt;
                        cc.UpdateTime = testData.LastData.OpenTime;
                        ChanceList.Add(cc);
                        int HCnt = 1;
                        if (HoldCntDic == null) HoldCntDic = new Dictionary<int, int>();
                        if (HoldCntDic.ContainsKey(cc.HoldTimeCnt))
                        {
                            HCnt = HoldCntDic[cc.HoldTimeCnt];
                            HCnt++;
                            HoldCntDic[cc.HoldTimeCnt] = HCnt;
                        }
                        else
                        {
                            HoldCntDic.Add(cc.HoldTimeCnt, 1);
                        }
                    }
                    else
                    {
                        tmpChances.Add(key, cc);
                    }
                }
            }
            //List<ChanceClass> cs = teststrag.getChances(testData);

            List<ChanceClass> cs = teststrag.getChances(sc, testData.LastData);
            if (ChanceList == null)
            {
                ChanceList = new List<ChanceClass>();
            }
            //ret.ChanceList.AddRange(cs);
            NoCloseChances = new Dictionary<string, ChanceClass>();
            foreach (string key in tmpChances.Keys)
            {
                ChanceClass cc = tmpChances[key];
                NoCloseChances.Add(key, cc);
            }
            for (int i = 0; i < cs.Count; i++)
            {
                //string key = string.Format("{0}_{1}", cs[i].SignExpectNo, cs[i].ChanceCode);
                string key = string.Format("{0}", cs[i].ChanceCode);
                if (NoCloseChances.ContainsKey(key))
                {
                    if (teststrag.AllowRepeat)
                    {
                        string test = key;
                        //NoCloseChances.Add(key, cs[i]);
                    }
                }
                else
                {
                    NoCloseChances.Add(key, cs[i]);
                }
            }
        }

        public ExchanceReuslt Run(ExpectList testData, StragClass teststrag)
        {

            return null;
        }
    }
}

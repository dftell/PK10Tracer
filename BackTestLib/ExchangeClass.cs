using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.ServerInitLib;
using WolfInv.com.ExchangeLib;
namespace WolfInv.com.BackTestLib
{
    
    public class ExchanceClass<T> where T : TimeSerialData
    {
        Dictionary<string, ChanceClass<T>> tmpChances = null;
        public void Run(DataTypePoint dtp,ExpectList<T> testData, BaseStragClass<T> teststrag, ref List<ChanceClass<T>> ChanceList, ref Dictionary<string, ChanceClass<T>> NoCloseChances, ref Dictionary<int, int> HoldCntDic) 
        {
            tmpChances = new Dictionary<string, ChanceClass<T>>();
            if (ChanceList == null) ChanceList = new List<ChanceClass<T>>();
            BaseCollection<T> sc = new ExpectListProcessBuilder<T>(dtp,testData).getProcess().getSerialData(teststrag.ReviewExpectCnt, teststrag.BySer);
            foreach (string key in NoCloseChances.Keys)
            {
                ChanceClass<T> cc = NoCloseChances[key];
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
            //List<ChanceClass<T>> cs = teststrag.getChances(testData);

            List<ChanceClass<T>> cs = teststrag.getChances(sc, testData.LastData);
            if (ChanceList == null)
            {
                ChanceList = new List<ChanceClass<T>>();
            }
            //ret.ChanceList.AddRange(cs);
            NoCloseChances = new Dictionary<string, ChanceClass<T>>();
            foreach (string key in tmpChances.Keys)
            {
                ChanceClass<T> cc = tmpChances[key];
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

        public ExchanceReuslt Run(ExpectList<T> testData, BaseStragClass<T> teststrag)
        {

            return null;
        }
    }

    }

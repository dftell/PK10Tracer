﻿//using WolfInv.com.SecurityLib;
using System;
using System.Collections.Generic;
using WolfInv.com.WebCommunicateClass;
using System.Linq;
using WolfInv.com.ProbMathLib;
using System.Security.Permissions;
using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.WebRuleLib
{
    /// <summary>
    /// XxY类，包括11选5，12选5，重庆时时彩等
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public  class XxYLotteryConfigClass : LotteryConfigClass
    {

        public XxYLotteryConfigClass(WebRule we, DataTypePoint dtp, LotteryTypes rs,string name) : base(we,dtp, rs,name)
        {
            
            
        }

        public override string IntsToJsonString(string ccs, int unit)
        {
            
            //unit为单位 0，元；1，角；2，分，3，其他
            ccs = ccs.Trim();
            ccs = ccs.Replace("  ", "");
            ccs = ccs.Replace("+", " ");
            ccs = ccs.Trim();
            if (ccs.Length == 0) return "";
            String[] ccsarr = ccs.Split(' ');
            List<InstClass> InsArr = new List<InstClass>();
            double unitVal = 0;
            unitVal = getUnitValue(unit);
            //int ArrCnt = 0;
            for (int i = 0; i < ccsarr.Length; i++)//遍历所有指令
            {
                String cc;
                String ccNos;// As String,
                String ccCars;//As String,
                Int64 ccUnitCost;// As Long,
                String[] ccArr;
                cc = ccsarr[i].Trim();
                if (cc.Length == 0)
                    continue;
                ccArr = cc.Split('/');
                if (ccArr.Length < 3)
                    continue;
                ccNos = ccArr[0].Trim();//第一个是指令类型 ，A1~8 C2~3 P2~3,以及其他趣味类型
                var items = rules.AllRules.Where(a => a.Value.instType == ccNos);
                string strPoint = ccNos.Substring(1);
                if (items.Count() != 1)//如果并未识别到投注类型,投注跳过
                    continue;
                LotteryBetRuleClass currRule = items.First().Value;//获取到类型
                string strNums = ccArr[1];//
                String ccOrgCars = ccArr[1].Trim();
                ccCars = ToStdFmt(ccOrgCars,ccNos.ToUpper().StartsWith("P"),5, ccNos.ToUpper().StartsWith("E")? strPoint : null).Trim();//车号组合标准格式
                ccUnitCost = Int64.Parse(ccArr[2]);
                if (ccUnitCost == 0)
                    continue;
                
                

                InstClass ic = new InstClass();
                ic.ToWebJson = wr.ToInstItem;
                //ic.Unit = unitVal;
                ic.ruleId = currRule.BetRule;//配置文件里面指定的规则号
                ic.betNum = getNums(ccNos,rules, ccCars).ToString();//所有指令数量都为1？好像也不是的，而是组合数
                ic.itemTimes = String.Format("{0:N2}", unitVal * ccUnitCost);
                ic.itemUnitTimes = ccUnitCost.ToString();
                ic.selNums = ccCars; //Array2String(strNums.Split(',')).Replace(", ", ",");
                string strOdds = null;
                if(currRule.OddsDic.ContainsKey(gb.Odds.ToString()))
                {
                    strOdds = currRule.OddsDic[gb.Odds.ToString()];
                }
                double odds = 0.0;
                bool needCalcOdds = true;
                if (!string.IsNullOrEmpty(strOdds))
                {
                    if(double.TryParse(strOdds,out odds))
                    {
                        needCalcOdds = false;
                    }
                }
                if(needCalcOdds)//当没有取到赔率，或者赔率非法，才去计算
                    odds = getRealOdds(ccNos, rules, gb.Odds);
                ic.jsOdds = string.Format("{0:N2}",Math.Round(odds,2));//string.Format("{0:N2}",getRealOdds(setting.Odds, rules.elementCount, rules.baseTimes, rules.calcTimes, currRule.oddsTimes));//  String.Format("{0:N2}", setting.Odds);
                ic.priceMode = unit;
                InsArr.Add(ic);

            }
            if (InsArr.Count > 0)
            {
                return this.IntsListToJsonString(InsArr);
            }
            else
            {
                return "";
            }
        }

        string ToStdFmt(string strInput,bool isP,int AllArrLen = 1,string strPoint=null)
        {
            
            string str = strInput;
            if(strPoint!=null)
            {
                string[] arrs = new string[AllArrLen];
                int point = int.Parse(strPoint);
                arrs[point - 1] = strInput;
                str = string.Join(",", arrs);
            }
            string[] arr = str.Trim().Split(',');
            for(int i=0;i<arr.Length;i++)
            {
                if(isP)
                {
                    string[] pArr = arr[i].Split('-');
                    for(int p=0;p<pArr.Length;p++)
                    {
                        if(!dtp.ClientNoNeedZero)
                            pArr[p] =  pArr[p].Trim().PadLeft(2,'0');
                    }
                    arr[i] = string.Join(" ", pArr);
                }
                else
                {
                    arr[i] = string.IsNullOrEmpty(arr[i])?"": dtp.ClientNoNeedZero?arr[i]:arr[i].Trim().PadLeft(2, '0');
                }
            }
            return string.Join((isP|| strPoint!=null) ? "," : " ", arr);
        }

        double getRealOdds(string ccNos,LotteryTypes lts, double orgOdds)
        {
            //ProbMath 概率计算类
            string pType = ccNos.Substring(0, 1);//A,C,P
            string pVal = ccNos.Substring(1, 1);//1~8
            int nVal = int.Parse(pVal);
            double ret = 0.00;
            switch (pType.ToUpper())
            {
                case "E":
                    {
                        decimal c = 1;
                        decimal sc = lts.elementCount;
                        ret = (double)(c / sc);
                        break;
                    }
                case "C":
                    {
                        Decimal c =  ProbMath.GetCombination(lts.elementCount, nVal);//11任意5个的组合为C(11,5)
                        Decimal sc = 1;
                        ret = (double)(c / sc);
                        break;
                    }
                case "P":
                    {
                        Decimal c = ProbMath.GetFactorial(lts.elementCount, nVal);//11任意5个的组合为C(11,5)
                        Decimal sc = 1;
                        ret = (double)(c / sc);
                        break;
                    }
                case "A":
                default:
                    {
                        Decimal c = ProbMath.GetCombination(lts.elementCount, nVal);//11任意5个的组合为C(11,5)
                        Decimal sc = ProbMath.GetCombination(lts.seletElementCnt, nVal);
                        if (nVal > lts.seletElementCnt)
                            sc = ProbMath.GetCombination(lts.elementCount - lts.seletElementCnt, nVal - lts.seletElementCnt);
                        ret = Math.Round((double)(c / sc),2);
                        break;
                    }
            }
            return (double)Decimal.Round((Decimal)( orgOdds * ret*2/10),2); ;
        }

        int getNums(string ccNos, LotteryTypes lts, string nums)
        {
            string pType = ccNos.Substring(0, 1);//A,C,P
            string pVal = ccNos.Substring(1);//1~8
            int nVal = int.Parse(pVal);
            int ret = 1;
            switch (pType.ToUpper())
            {
                case "E":
                    {
                        ret = nums.Split(' ').Length;
                        break;
                    }
                case "C":
                    {
                        int n = nums.Split(' ').Length;
                        Decimal c = ProbMath.GetCombination(n,nVal);
                        ret = (int)c;
                        break;
                    }
                case "P":
                    {
                        //Decimal c = ProbMath.GetFactorial(n, nVal);//11任意5个的组合为C(11,5)
                        string[] arr = nums.Split(',');
                        int n = 1;
                        for (int i=0;i<arr.Length;i++)
                        {
                            n = n * arr[i].Split(' ').Length;
                        }
                        ret = n;//重复的暂时不考虑，正常应该要考虑到 1-2-3，3-2-1，1-3-2这种情况
                        break;
                    }
                case "A":
                default:
                    {
                        int n = nums.Split(' ').Length;
                        Decimal c = ProbMath.GetCombination(n, nVal);//11任意5个的组合为C(11,5)
                        ret = (int)c;
                        break;
                    }
            }
            return ret;
        }
        
    }

    public class LotteryConfigClass_GDKL11 : XxYLotteryConfigClass
    {
        public LotteryConfigClass_GDKL11(WebRule we, DataTypePoint dtp, LotteryTypes rs,string name) : base(we,dtp, rs,name)
        {
            //cRuleId_S = rules.AllRules["34140101"].BetRule;
            //cRuleId_B = rules.AllRules["34140102"].BetRule;
            //g01_00 = "8010101";//猜冠军
            //g0102_00 = "8020101";//猜冠亚军
            //g0102_01 = "8020201";//猜冠亚军单式
            //cRuleId_S = "8140101";// '前5位定胆
            //cRuleId_B = "8140102";// '后5位定胆
        }
    }



}

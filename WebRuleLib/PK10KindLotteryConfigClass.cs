using System;
using System.Collections.Generic;
using WolfInv.com.WebCommunicateClass;
//using WolfInv.com.SecurityLib;
namespace WolfInv.com.WebRuleLib
{

    public class LotteryConfigClass_XYFT : PK10KindLotteryConfigClass
    {
        public LotteryConfigClass_XYFT(WebRule we, LotteryTypes rs,string name) : base(we, rs,name)
        {
            if (rules.AllRules.ContainsKey("34140101"))
            {
                LotteryBetRuleClass lbr_s = rules.AllRules["34140101"];
                LotteryBetRuleClass lbr_b = rules.AllRules["34140102"];
                cRuleId_S = rules.AllRules["34140101"].BetRule;
                cRuleId_B = rules.AllRules["34140102"].BetRule;
            }
            Init_ruleId("34140101", "34140102");
            //g01_00 = "8010101";//猜冠军
            //g0102_00 = "8020101";//猜冠亚军
            //g0102_01 = "8020201";//猜冠亚军单式
            //cRuleId_S = "8140101";// '前5位定胆
            //cRuleId_B = "8140102";// '后5位定胆
        }
    }



    /// <summary>
    /// PK10类彩种
    /// </summary>
    public class PK10KindLotteryConfigClass: F2LotteryyConfigClass
    {

        protected void Init_ruleId(string fore_key,string after_key)
        {
            if (rules.AllRules.ContainsKey(fore_key) && rules.AllRules.ContainsKey(after_key))
            {
                LotteryBetRuleClass lbr_s = rules.AllRules[fore_key];
                LotteryBetRuleClass lbr_b = rules.AllRules[after_key];
                bool useDiffType = false;
                if(string.IsNullOrEmpty(lbr_s.BetType))
                {
                    useDiffType = true;
                }
                else
                {
                    useDiffType = !lbr_s.BetRule.Contains(lbr_s.BetType);//如果ruleid不包括type,那肯定是使用了其他命名规则
                }
                if(useDiffType)
                {
                    cRuleId_B = lbr_s.BetType;
                    cRuleId_S = lbr_b.BetType;
                }
                else
                {
                    cRuleId_S = lbr_s.BetRule;
                    cRuleId_B = lbr_b.BetRule;
                }
                //cRuleId_S = rules.AllRules["34140101"].BetRule;
                //cRuleId_B = rules.AllRules["34140102"].BetRule;
            }
        }
        public PK10KindLotteryConfigClass(WebRule we, LotteryTypes rs,string name) : base(we, rs,name)
        {
        
            g01_00 = "8010101";//猜冠军
            g0102_00 = "8020101";//猜冠亚军
            g0102_01 = "8020201";//猜冠亚军单式
            //cRuleId_S = "8140101";// '前5位定胆
            //cRuleId_B = "8140102";// '后5位定胆
            if(rules.AllRules.ContainsKey("8140101"))
                cRuleId_S = rules.AllRules["8140101"].BetRule;
            if (rules.AllRules.ContainsKey("8140102"))
                cRuleId_B = rules.AllRules["8140102"].BetRule;
            Init_ruleId("8140101", "8140102");
        }

        public override string IntsToJsonString(String ccs, int unit)
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
            for (int i = 0; i < ccsarr.Length; i++)
            {
                String cc;
                String ccNos;// As String,
                String ccCars;//As String,
                Int64 ccUnitCost;// As Long,
                String[] ccArr;
                cc = ccsarr[i].Trim();
                if (cc.Length == 0) continue;
                ccArr = cc.Split('/');
                if (ccArr.Length < 3) continue;
                ccNos = ccArr[0].Trim();
                String ccOrgCars = ccArr[1].Trim();
                ccCars = toStdCarFmt(ccOrgCars).Trim();//车号组合标准格式
                ccUnitCost = Int64.Parse(ccArr[2]);
                if (ccUnitCost == 0)
                    continue;
                String[] sArr = new String[5];
                String[] bArr = new String[5];
                String[] fArr = new string[10];
                for (int j = 0; j < 5; j++)
                {
                    sArr[j] = "";
                    bArr[j] = "";
                }
                for (int j = 0; j < 10; j++)
                    fArr[j] = "";
                int sArrCnt, bArrCnt;
                sArrCnt = 0;
                bArrCnt = 0;
                for (int j = 0; j < ccNos.Length; j++)
                {
                    String strsNo;//As String
                    int iNo;// As Integer
                    strsNo = ccNos.Substring(j, 1);// Mid(ccNos, j, 1)
                    if (strsNo.Equals("0"))
                    {
                        strsNo = "10";
                    }
                    iNo = int.Parse(strsNo);
                    if (iNo <= 5)
                    {
                        sArr[iNo - 1] = ccCars.Trim();
                        sArrCnt++;
                    }
                    else
                    {
                        bArr[iNo - 5 - 1] = ccCars.Trim();
                        bArrCnt++;
                    }
                    fArr[iNo-1] = ccCars.Trim();
                }
                InstClass ic = new InstClass();
                ic.ToWebJson = wr.ToInstItem;
                ic.fullSelNums = Array2String(fArr).Replace(", ", ",");
                ic.itemTimes = String.Format("{0:N2}", unitVal * ccUnitCost);
                ic.itemUnitTimes = ccUnitCost.ToString();
                ic.jsOdds = String.Format("{0:N2}", setting.Odds);
                ic.priceMode = unit;
                //ic.Unit = unitVal;
                if (sArrCnt > 0)
                {
                    
                    ic.ruleId = cRuleId_S;
                    ic.betNum = String.Format("{0}", ccOrgCars.Length * sArrCnt);
                    ic.selNums = Array2String(sArr).Replace(", ", ",");
                    
                }
                if (bArrCnt > 0)
                {
                    ic.ruleId = cRuleId_B;
                    ic.betNum = String.Format("{0}", ccOrgCars.Length * bArrCnt);
                    ic.selNums = Array2String(bArr).Replace(", ", ",");
                    //InsArr.Add(ic);
                }
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

        



        
    }

   
    
}

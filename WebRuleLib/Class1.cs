using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
namespace WebRuleLib
{

    public abstract class WebRule
    {
        public abstract string IntsListToJsonString(List<InstClass> Insts);
        public abstract string IntsToJsonString(String ccs, int unit);
        public GlobalClass GobalSetting;
        protected WebRule(GlobalClass setting)
        {
            GobalSetting = setting;
        }
    }



    public class Rule_ForKcaiCom : WebRule
    {
        public Rule_ForKcaiCom(GlobalClass setting)
            : base(setting)
        {
        }
        String cRuleId_S = "8140101";// '前5位定胆
        String cRuleId_B = "8140102";// '后5位定胆
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
            int ArrCnt = 0;
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
                String[] sArr = new String[5];
                String[] bArr = new String[5];
                for (int j = 0; j < 5; j++)
                {
                    sArr[j] = "";
                    bArr[j] = "";
                }
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
                }
                if (sArrCnt > 0)
                {
                    InstClass ic = new InstClass();
                    ic.ruleId = cRuleId_S;
                    ic.betNum = String.Format("{0}", ccOrgCars.Length * sArrCnt);
                    ic.itemTimes = String.Format("{0:N2}", unitVal * ccUnitCost);
                    ic.selNums = Array2String(sArr).Replace(", ", ",");
                    ic.jsOdds = String.Format("{0:N2}", GobalSetting.Odds);
                    ic.priceMode = unit;
                    InsArr.Add(ic);
                }
                if (bArrCnt > 0)
                {
                    InstClass ic = new InstClass();
                    ic.ruleId = cRuleId_B;
                    ic.betNum = String.Format("{0}", ccOrgCars.Length * bArrCnt);
                    ic.itemTimes = String.Format("{0:N2}", unitVal * ccUnitCost);
                    ic.selNums = Array2String(bArr).Replace(", ", ",");
                    ic.jsOdds = String.Format("{0:N2}", GobalSetting.Odds);
                    ic.priceMode = unit;
                    InsArr.Add(ic);
                }

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

        public override string IntsListToJsonString(List<InstClass> Insts)
        {

            String[] strInsts = new String[Insts.Count];
            for (int i = 0; i < Insts.Count; i++)
            {
                InstClass ic = Insts[i];
                strInsts[i] = ic.GetJson();
            }
            return String.Format("[{0}]", Array2String(strInsts));
        }

        String toStdCarFmt(String cars)
        {
            cars = cars.Trim();
            String[] CarArr = new String[cars.Length];
            for (int i = 0; i < cars.Length; i++)
            {
                String strCar = cars.Substring(i, 1);
                if (strCar.Equals("0"))
                    strCar = "10";
                String tmp = String.Format("0{0}", strCar).Trim();
                CarArr[i] = tmp.Substring(tmp.Length - 2).Trim();
            }
            CarArr.ToString();
            return Array2String(CarArr).Replace(" ", "").Replace(",", " ").Trim();
            //[{"jsOdds":"9.78","priceMode":2,"selNums":",01020710,01020710,,","betNum":8,"itemTimes":"0.01","ruleId":"8140101"}]
        }

        String Array2String(string[] arr)
        {
            String ret = string.Join(",", arr);
            return ret;
            //return ret.substring(1, ret.Length - 1).Trim();//去掉中括号
        }

        double getUnitValue(int unit)
        {
            return Math.Pow(0.1, unit);
        }

    }

}

using System;
using System.Collections.Generic;
using WolfInv.com.WebCommunicateClass;
using WolfInv.com.BaseObjectsLib;
//using WolfInv.com.SecurityLib;
namespace WolfInv.com.WebRuleLib
{
    public abstract class LotteryConfigClass
    {
        protected string Name;
        protected WebRule wr;
        public LotteryTypes rules;
        protected LotteryConfigClass(WebRule we, LotteryTypes rs,string name)
        {
            rules = rs;
            wr = we;
            Name = name;
        }
        public GlobalClass setting;
        protected String Array2String(string[] arr)
        {
            String ret = string.Join(",", arr);
            return ret;
            //return ret.substring(1, ret.Length - 1).Trim();//去掉中括号
        }

        protected double getUnitValue(int unit)
        {
            return Math.Pow(0.1, unit);
        }

        public abstract string IntsToJsonString(String ccs, int unit);

        public string IntsListToJsonString(List<InstClass> Insts)
        {

            String[] strInsts = new String[Insts.Count];
            for (int i = 0; i < Insts.Count; i++)
            {
                InstClass ic = Insts[i];
                strInsts[i] = ic.GetJson();
            }
            return String.Format("[{0}]", Array2String(strInsts));
        }

        public String toStdCarFmt(String cars)
        {
            cars = cars.Trim();
            String[] CarArr = new String[cars.Length];
            for (int i = 0; i < cars.Length; i++)
            {
                String strCar = cars.Substring(i, 1);
                if (strCar.Equals("0"))
                    strCar = "10";
                //String tmp = String.Format("0{0}", strCar).Trim();
                //CarArr[i] = tmp.Substring(tmp.Length - 2).Trim();
                CarArr[i] = strCar.PadLeft(2, '0');
            }
            CarArr.ToString();
            return Array2String(CarArr).Replace(" ", "").Replace(",", " ").Trim();
            //[{"jsOdds":"9.78","priceMode":2,"selNums":",01020710,01020710,,","betNum":8,"itemTimes":"0.01","ruleId":"8140101"}]
        }

        public static double getRealOdds(double orgOdds, int elementCnt, int baseTimes, int calcTimes,double oddsTimes)
        {
            double LastOdds = elementCnt / baseTimes / calcTimes * orgOdds / 10 * oddsTimes;//翻了2倍，然后还要除以最小投注倍数2，最后要乘以整体赔率9.50-9.78/10
            return LastOdds;
        }

    }

   
    
}

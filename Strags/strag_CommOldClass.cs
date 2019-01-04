using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using LogLib;
using System.ComponentModel;

namespace Strags
{
    [DescriptionAttribute("通用N码穷追选号策略"),
        DisplayName("通用N码穷追选号策略")]
    public class strag_CommOldClass:StragClass
    {
        public strag_CommOldClass()
            : base()
        {
            _StragClassName = "通用N码穷追选号策略";
        }

        public override List<ChanceClass> getChances(CommCollection sc,ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            //ExpectListProcess elp = new ExpectListProcess(Data);
            //CommCollection sc = elp.getSerialData(ReviewExpectCnt,BySer);
            //            Log("策略关键参数",string.Format("name:{4};ViewCnt:{0};MinInput:{1};DataCnt:{2};Chipcnt:{3}",this.ReviewExpectCnt,this.InputMinTimes,sc.Data[0].Count,this.ChipCount,this.StragScript));
            for (int i = 0; i < 10; i++)
            {
                Dictionary<int, string> SerStrs = sc.Data[i];
                if (!SerStrs.ContainsKey(this.InputMinTimes - 1))
                {
                    continue;
                }
                if (SerStrs[this.InputMinTimes - 1].Length < this.ChipCount)
                {
                    continue;
                }
                int ishift = this.InputMinTimes - 1;
                //Log("起始位置", ishift.ToString());
                List<string> strCcs = ChanceClass.getAllSubCode(SerStrs[ishift].Trim(), this.ChipCount);
                for (int j = 0; j < strCcs.Count; j++)
                {
                    string strCars = strCcs[j];
                    if (this.ExcludeBS && ChanceClass.isBS(strCars))
                    {
                        continue;
                    }
                    if (this.ExcludeSD && ChanceClass.isSD(strCars))
                    {
                        continue;
                    }
                    if (this.OnlyBS && !ChanceClass.isBS(strCars))
                    {
                        continue ;
                    }
                    if (this.OnlySD && !ChanceClass.isSD(strCars))
                    {
                        continue;
                    }
                    NolimitTraceChance cc = new NolimitTraceChance();
                    cc.SignExpectNo = ed.Expect;
                    cc.ChanceType = 0;
                    cc.ChipCount = strCars.Length;
                    cc.InputTimes = ishift + 1;
                    cc.strInputTimes = cc.InputTimes.ToString();
                    //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                    cc.InputExpect = ed;
                    cc.StragId = this.GUID;
                    cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount-1];
                    cc.IsTracer = 1;
                    cc.HoldTimeCnt = 1;
                    cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                    cc.ExpectCode = ed.Expect;
                    string strCode = "";
                    if (BySer)
                        strCode = string.Format("{0}/{1}", (i + 1) % 10, strCcs[j].Trim());
                    else
                        strCode = string.Format("{0}/{1}", strCcs[j].Trim(), i);
                    cc.ChanceCode = strCode;
                    cc.CreateTime = DateTime.Now;
                    cc.UpdateTime = DateTime.Now;
                    cc.Closed = false;
                    ret.Add(cc);
                }
                //}
                //}
            }
            return ret;
        }


        public override StagConfigSetting getInitStagSetting()
        {
            StagConfigSetting sett = new StagConfigSetting();
            sett.BaseType = new BaseTypeSetting();
            string[] list = new string[]{};
            GlobalClass gc = this.CommSetting.GetGlobalSetting();
            AmoutSerials As = GlobalClass.getOptSerials(gc.Odds,gc.DefMaxLost,gc.DefFirstAmt);
            if (As.MaxHoldCnts.Length > 0)
            {
                Int64[] UnitAmtArr = As.Serials[this.ChipCount - 1];
                list = UnitAmtArr.ToList().Select(t=>t.ToString()).ToArray();            
            }
            else
            {
                list = this.CommSetting.GetGlobalSetting().UnitChipArray(this.ChipCount);
            }
            List<int> ret = new List<int>();
            for (int i = 0; i < list.Length; i++)
                ret.Add(int.Parse(list[i]));
            sett.BaseType.ChipSerial = ret;
            return sett;
        }



        public override Type getTheChanceType()
        {
            return typeof(NolimitTraceChance);
        }

     }
}

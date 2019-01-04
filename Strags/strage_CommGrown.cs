using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.ComponentModel;
namespace Strags
{
    [DescriptionAttribute("通用N码成长策略"),
        DisplayName("通用N码成长策略")]
    public class strage_CommGrown:StragClass
    {
        public strage_CommGrown():base()
        {
            this._StragClassName = "通用N码成长策略";
        }
        public override List<ChanceClass> getChances(CommCollection sc,ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            //ExpectListProcess elp = new ExpectListProcess(Data);
            //CommCollection sc = elp.getSerialData(ReviewExpectCnt,BySer);
            for (int i = 0; i < 10; i++)
            {
                Dictionary<int, string> SerStrs = sc.Data[i];
                int ishift = sc.Data[0].Count - 1;
                while (ishift >= this.InputMinTimes)
                {
                    if (SerStrs.ContainsKey(ishift))
                    {
                        if (SerStrs[ishift].Trim().Length == (10 - this.ChipCount))
                        {
                            if (this.ExcludeBS && ChanceClass.isBS(SerStrs[ishift].Trim()))
                            {
                                break;
                            }
                            if (this.ExcludeSD && ChanceClass.isSD(SerStrs[ishift].Trim()))
                            {
                                break;
                            }
                            if (this.OnlyBS && !ChanceClass.isBS(SerStrs[ishift].Trim()))
                            {
                                break;
                            }
                            if (this.OnlySD && !ChanceClass.isSD(SerStrs[ishift].Trim()))
                            {
                                break;
                            }
                            ChanceClass cc = new ChanceClass();
                            cc.SignExpectNo = ed.Expect;
                            cc.ChanceType = 2;
                            cc.InputTimes = ishift;
                            cc.InputExpect = ed;
                            string strCode = "";
                            if (BySer)
                                strCode = string.Format("{0}/{1}", (i + 1) % 10, SerStrs[ishift].Trim());
                            else
                                strCode = string.Format("{0}/{1}", SerStrs[ishift].Trim(), i);
                            cc.ChanceCode = ChanceClass.getRevChance(strCode);
                            cc.CreateTime = ed.OpenTime;
                            cc.Closed = false;
                            ret.Add(cc);
                            break;
                        }
                        else
                        {
                            ishift--;
                        }
                    }
                }
            }
            return ret;
        }


        public override StagConfigSetting getInitStagSetting()
        {
            throw new NotImplementedException();
        }



        public override Type getTheChanceType()
        {
            throw new NotImplementedException();
        }

      
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.ComponentModel;
namespace Strags
{
    [DescriptionAttribute("N码指定条件成长策略"),
        DisplayName("N码指定条件成长策略")]
    public class strag_grownclass:StragClass
    {
        public strag_grownclass():base()
        {
            _StragClassName = "N码指定条件成长策略";
            AllowRepeat = true;

        }

        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            ////ExpectListProcess elp = new ExpectListProcess(Data);
            ////CommCollection sc = elp.getSerialData(ReviewExpectCnt,BySer);
            for (int i = 0; i < 10; i++)
            {
                Dictionary<int, string> SerStrs = sc.Data[i];
                int MaxLen = CommSetting.minColTimes[4];//5码线
                int Max2Len = this.InputMaxTimes-1;//7码线
                if (SerStrs.ContainsKey(MaxLen + 1)&&SerStrs[MaxLen].Trim().Length > 0)//最大长度超过了5码线,过滤
                {
                    continue;
                }
                string strKey = "";
                if (SerStrs.ContainsKey(Max2Len))
                {
                    strKey = SerStrs[Max2Len];
                }
                if (strKey.Trim().Length >= 2) //7码线所包括的内容大于等于2
                {
                    continue;
                }
                int ishift = sc.Data[0].Count - 1;
                while (ishift >= this.ChipCount)
                {
                    if (SerStrs.ContainsKey(ishift))
                    {
                        if (SerStrs[ishift].Trim().Length == (10-this.ChipCount))
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
                            if (ishift < this.InputMinTimes-1)
                            {
                                break;
                            }
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

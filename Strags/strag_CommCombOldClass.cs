using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.ComponentModel;
namespace Strags
{
    [DescriptionAttribute("通用N码混合组合穷追选号策略"),
        DisplayName("通用N码混合组合穷追选号策略")]
    public class strag_CommCombOldClass:StragClass
    {
        public strag_CommCombOldClass()
        {
            _StragClassName = "通用组合穷追选号策略";
        }
        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            //ExpectListProcess elp = new ExpectListProcess(Data);
            //CommCollection sc = elp.getSerialData(ReviewExpectCnt,BySer);
            Dictionary<string, int> MatchTimes = new Dictionary<string, int>();
            Dictionary<string, string> MatchChances = new Dictionary<string, string>();
            Dictionary<string, string> CombDic = new Dictionary<string, string>();
            Dictionary<int, int> MaxCnts = new Dictionary<int, int>();
            Dictionary<int, int> NoCnts = new Dictionary<int, int>();
            int MinLimit = this.CommSetting.minColTimes[this.ChipCount + 3 - 1];
            #region 获取所有满足基本条件的组合
            for (int i = 0; i < 10; i++)
            {
                Dictionary<int, string> SerStrs = sc.Data[i];
                int ishift = MinLimit - 1;//多三级
                if (SerStrs[ishift].Length < this.ChipCount)//不满足基本条件
                {
                    continue;
                }
                MaxCnts.Add(i, this.ReviewExpectCnt);//默认长度都是reviewcnt,如果大于指定长度的机会持续次数大于review,所有子机会都会加一
                NoCnts.Add(i, SerStrs[ishift].Length);
                string strColKeymModel = "{0}_{1}";
                while (ishift<this.ReviewExpectCnt)//从前三级开始爬
                {
                    ishift++;
                    if (SerStrs.ContainsKey(ishift) && SerStrs.ContainsKey(ishift-1))
                    {
                        string strCode = "";
                        int INo = -1;

                        if (BySer)
                        {
                            INo = (i) % 10;
                            strCode = SerStrs[ishift - 1].Trim();//1,2,3,4,5,6,7,8,9,0
                        }
                        else
                        {
                            INo = i;
                            strCode = SerStrs[ishift - 1].Trim();//0,1,2,3,4,5,6,7,8,9
                        }
                        if (SerStrs[ishift].Trim().Length >= this.ChipCount)//往上看
                        {
                            bool Matched = false;
                            if (SerStrs[ishift].Trim().Length < SerStrs[ishift - 1].Trim().Length)//收缩,当前行小于上一行
                            {
                                Matched = true;
                            }
                            else
                            {
                                if (ishift >= this.ReviewExpectCnt-1)//结束了还未收缩，则加入
                                {
                                    Matched = true;
                                }
                            }
                            if (Matched)
                            {
                                List<string> AllStrCode = new List<string>();
                                if (strCode.Trim().Length == this.ChipCount)
                                {
                                    AllStrCode.Add(strCode);
                                }
                                else
                                {
                                    AllStrCode = ChanceClass.getAllSubCode(strCode, this.ChipCount);
                                }
                                for (int j = 0; j < AllStrCode.Count; j++)
                                {
                                    string strColKey = string.Format(strColKeymModel, INo, AllStrCode[j]);
                                    if (!MatchChances.ContainsKey(strColKey))
                                    {
                                        MatchChances.Add(strColKey, AllStrCode[j]);
                                        MatchTimes.Add(strColKey, ishift + 1 - 1);
                                    }
                                    else
                                    {
                                        MatchTimes[strColKey] = ishift + 1 - 1;
                                    }
                                    
                                }
                            }
                        }
                        else//爬到看到的数量小于长度时，很可能会匹配不到，因为到最后一条数据显示长度仍然大于等于长度，所以，review需要足够长，最好大于高一码的最大长度
                        {
                            string strColKey = string.Format(strColKeymModel, INo, strCode);
                            if (!MatchChances.ContainsKey(strColKey))
                            {
                                MatchChances.Add(strColKey, strCode);
                                MatchTimes.Add(strColKey, ishift + 1 - 1);
                            }
                            else
                            {
                                MatchTimes[strColKey] = ishift + 1 - 1;
                            }
                            MaxCnts[i] = ishift;//每列符合条件的最大值，作为判断是否是本车/本排名的最大列用
                            break; //
                           
                        }
                    }
                }
                
            }
            #endregion
            string ComKeyModel="{0}+{1}";//小大排列
            if (MatchChances.Count <= 1) return ret;//只有一个组合
            foreach (string _key in MatchChances.Keys)
            {
                string[] keyArr = _key.Split('_');
                int key = int.Parse(keyArr[0]);
                //int cnt = int.Parse(keyArr[1]);
                foreach (string _key1 in MatchChances.Keys)
                {
                    string[] keyArr1 = _key1.Split('_');
                    int key1 = int.Parse(keyArr1[0]);
                    //int cnt1 = int.Parse(keyArr1[1]);
                    if(key == key1) continue;
                    string sKey,bKey;
                    if(key<key1)
                    {
                        sKey=_key;
                        bKey = _key1;
                    }
                    else
                    {
                        sKey = _key1;
                        bKey = _key;
                    }
                    string CombKey = string.Format(ComKeyModel, sKey, bKey);
                    if (CombDic.ContainsKey(CombKey)) continue;//防止实质相同但是顺序相反组合重复进入
                    int Time1, Time2,SameNoCnt;
                    Time1 = MatchTimes[_key];
                    Time2 = MatchTimes[_key1];
                    SameNoCnt = ChanceClass.getSameNoCnt(MatchChances[_key], MatchChances[_key1]);//获得相同号码的数量
                    //判断两个是不是本车/本排名中最大长度的机会
                    int AddSameNocnt = -2;//只有两个都是最长才不减，否则每个减1
                    if (MaxCnts[key] > Time1)
                        Time1-=2;
                    if (MaxCnts[key1] > Time2)
                        Time2-=2;
                    if (MaxCnts[key] < this.CommSetting.minColTimes[this.ChipCount])
                    {
                        Time1 -= 2;
                    }
                    if (MaxCnts[key1] < this.CommSetting.minColTimes[this.ChipCount])
                    {
                        Time2 -= 2;
                    }
                    //SameNoCnt += AddSameNocnt;
                    bool Matched = false;
                    //Level1
                    int LevelU2 = this.CommSetting.minColTimes[this.ChipCount + 2 - 1]; //上两级
                    int LevelU1 = this.CommSetting.minColTimes[this.ChipCount + 1 - 1];//上一级
                    int LevelU0 = this.CommSetting.minColTimes[this.ChipCount - 1];//本级
                    List<string> strCodes = getAllCodes(MatchChances[_key], MatchChances[_key1], this.BySer);
                    //////if (BySer)
                    //////    strCode = string.Format("{0}/{1}", (i + 1) % 10, SerStrs[ishift].Trim());
                    //////else
                    //////    strCode = string.Format("{0}/{1}", SerStrs[ishift].Trim(), i);
                    string strCode = this.BySer ? string.Format("{0}/{1}+{2}/{3}", (key+1)%10, MatchChances[_key], (key1+1)%10, MatchChances[_key1]) : string.Format("{0}/{1}+{2}/{3}", MatchChances[_key], key, MatchChances[_key1], key1);
                    if (SameNoCnt >= 3)
                    {
                        if (Time1> MinLimit && Time2 > MinLimit &&  this.ChipCount == SameNoCnt)
                        {
                            Matched = true;
                        }
                        //////else
                        //////{
                            if ((Time1 + Time2) > (LevelU1 + LevelU2) && Time1>LevelU2 && Time2>LevelU2)
                            {
                                Matched = true;
                            }
                        //////}
                    }
                    if (SameNoCnt >= 2)
                    {
                        if(Time1*2>(LevelU2+LevelU1) && Time2*2>(LevelU2+LevelU1) && (Time1+Time2)>2*LevelU1)
                        {
                            Matched = true;
                        }
                    }
                    if (SameNoCnt >= 1)
                    {
                        if (Time1 > LevelU1 && Time2 > LevelU1 && (Time1 + Time2) > (LevelU1 + LevelU0))
                        {
                            Matched = true;
                        }
                    }
                    if (SameNoCnt >= -2)
                    {
                        if (Time1 * 2 > (LevelU0 + LevelU1) && Time2 * 2 > (LevelU1 + LevelU0))
                        {
                            Matched = true;
                        }
                    }
                    if (!Matched) continue;
                    CombDic.Add(CombKey, strCode);
                    NolimitTraceChance cc = new NolimitTraceChance();
                    cc.ChipCount = this.ChipCount * 2;
                    //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                    cc.SignExpectNo = ed.Expect;
                    cc.ChanceType = 0;
                    cc.InputTimes = Math.Min(Time1,Time2);
                    cc.strInputTimes = string.Format("{0}_{1}", Time1, Time2);
                    cc.InputExpect = ed;
                    cc.ChanceCode = strCode;
                    cc.IsTracer = 1;
                    cc.CreateTime = ed.OpenTime;
                    cc.Closed = false;
                    ret.Add(cc);
                }

            }
            return ret;
        }

        public List<string> getAllCodes(string str1, string str2,bool isCar)
        {
            
            //string strCode = this.BySer ? string.Format("{0}/{1}+{2}/{3}", key, MatchChances[key], key1, MatchChances[key1]) : string.Format("{0}/{1}+{2}/{3}", MatchChances[key], key, MatchChances[key1], key1);
            return new List<string>();  
        }

        public override StagConfigSetting getInitStagSetting()
        {
            StagConfigSetting sett = new StagConfigSetting();
            sett.BaseType = new BaseTypeSetting();
            string[] list = this.CommSetting.GetGlobalSetting().UnitChipArray(this.ChipCount*2);
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

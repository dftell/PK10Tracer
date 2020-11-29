using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;
using WolfInv.com.ProbMathLib;
namespace WolfInv.com.Strags.KLXxY
{
    [DescriptionAttribute("X选Y小敞口暴露组合投注策略"),
        DisplayName("X选Y小敞口暴露组合投注策略")]
    public class Strag_KLXxY_Combin_All8_All3 : StragClass
    {
        public Strag_KLXxY_Combin_All8_All3()
        {
            this._StragClassName = "X选Y小敞口暴露组合投注策略";
        }

        public override bool CheckNeedEndTheChance(ChanceClass cc1, bool LastExpectMatched1)
        {
            return true;
        }

        List<ChanceClass> getChances(CommCollection_KLXxY sc,CombinClass restComb,string[] AllArr, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            for (int i = 0; i < restComb.Count; i++)
            {
                ChanceClass_ForCombinXxY cc = new ChanceClass_ForCombinXxY();
                cc.AllNums = sc.AllNums;
                cc.SelectNums = sc.SelNums;
                cc.strAllTypeBaseOdds = sc.strAllTypeOdds;
                cc.strCombinTypeBaseOdds = sc.strCombinTypeOdds;
                cc.strPermutTypeBaseOdds = sc.strPermutTypeOdds;

                string a7 = restComb[i];
                string[] Arr4 = CombinClass.getReconvertString(AllArr, a7.Split(','));
                List<string> al = new List<string>();
                for (int j = 0; j < Arr4.Length - 1; j++)
                {
                    al.Add(string.Format("A2/{0},{1}", Arr4[j], Arr4[j + 1]));
                }
                //al.Add(string.Format("A2/{0}", string.Join(",",Arr4)));//支持2码多数
                cc.ChanceCode = string.Format("A7/{0}+{1}", a7, string.Join("+", al));
                

                cc.UnitCost = 2;
                cc.ChipCount = 4;
                cc.ChanceType = 2;
                cc.InputTimes = 1;
                //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                cc.InputExpect = ed;
                cc.StragId = this.GUID;
                //cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                cc.IsTracer = 0;
                cc.HoldTimeCnt = 1;
                cc.Odds = this.CommSetting.GetGlobalSetting().Odds;
                cc.ExpectCode = ed.Expect;
                cc.CreateTime = DateTime.Now;
                cc.UpdateTime = DateTime.Now;
                cc.AllowMaxHoldTimeCnt = 1;
                cc.Closed = false;
                ret.Add(cc);
            }
            return ret;
        }
        class MatchItem
        {
            public string LongItem;
            public string ShortItem;
            //public CombinClass SelectItemsMiss0;
            //public CombinClass SelectItemsMiss1;
            //public CombinClass SelectItemsMiss2;
            //public CombinClass SelectItemsMiss3;
            //public CombinClass SelectItemsMiss4;
            public Dictionary<int, CombinClass> SubItems;
            public Dictionary<int, int> SubItemMatchCnt = new Dictionary<int, int>();
            //public int Miss0=0;
            //public int Miss1=0;
            //public int Miss2=0;
            //public int Miss3=0;
            string[] AllStr;
            int SelectCnt;
            int MissCnt;
            public MatchItem(string[] AllArr,string SelectString,int selectNum,int MissNum)
            {
                AllStr = AllArr;
                LongItem = SelectString;
                SelectCnt = selectNum;
                MissCnt = MissNum;
                ShortItem = string.Join(",",CombinClass.getReconvertString(AllArr,SelectString.Split(',')));
                SubItems = new Dictionary<int, CombinClass>();
                for(int i=0;i<MissNum;i++)
                {
                    CombinClass cc = new CombinClass(LongItem, selectNum - i); 
                    SubItems.Add(selectNum - i, cc);
                    SubItemMatchCnt.Add(selectNum - i, 0);
                }

            }

        }
        public override List<ChanceClass> getChances(BaseCollection bsc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            try
            {
                CommCollection_KLXxY sc = bsc as CommCollection_KLXxY;
                if (sc == null)
                {
                    throw new Exception("数据收集类！");
                }
                //this.ChipCount;//12选5和11选5都是5
                //this.InputMaxTimes; //12选5选为8，1选5选为7
                //this.InputMinTimes;//12选5和11选5都是4
                string[] AllArr = CombinClass.CreateNumArr(sc.AllNums);
                //CombinClass allcmb = CombinClass.CireateNumCombin(sc,iAllNums, 3);//获得所有的3组合
                CombinClass allbigcmb = CombinClass.CreateNumCombin(sc.AllNums, this.InputMaxTimes);//获得所有的8/7个组合
                Dictionary<string, MatchItem> allItems = new Dictionary<string, MatchItem>();
                allbigcmb.ForEach(a =>
                {

                    MatchItem mi = new MatchItem(AllArr, a, this.ChipCount, this.InputMinTimes);
                    mi.LongItem = a;
                    mi.ShortItem = string.Join(",", mi);
                    allItems.Add(a, mi);
                });

                /*
                 列出所有11/7或者12/8码的组合，列出这些组合在回览期内的下列数据
                 1，全部命中次数
                 2，错过1个的次数
                 3，错过2个的次数n
                 3，错过3个次数
                 4，错过4个的次数)
                 */

                int lastid = sc.orgData.Count - 1;
                for (int i = 0; i < this.ReviewExpectCnt; i++)
                {
                    int index = lastid - i;
                    if (index < 0)
                    {
                        break;
                    }
                    string opencode = sc.orgData[index].OpenCode;
                    opencode = CombinGenerator.ResortNumString(opencode, ",");
                    foreach (MatchItem mi in allItems.Values)
                    {
                        for (int c = 0; c < this.InputMinTimes; c++)
                        {
                            int MatchCnt = this.ChipCount - c;
                            CombinClass occ = new CombinClass(opencode, MatchCnt);
                            int mcnt = 0;
                            foreach (string si in occ)//如果开奖号中5-c命中，停止，计数器加1
                            {
                                if (mi.SubItems[MatchCnt].Contains(si))
                                {
                                    mcnt++;
                                }
                            }
                            mi.SubItemMatchCnt[MatchCnt] += mcnt;
                            if (mcnt > 0)
                            {
                                break;
                            }
                        }
                    }



                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                for (int i = 0; i < this.InputMinTimes; i++)
                {
                    dt.Columns.Add(string.Format("MCnt_{0}", this.ChipCount - i), typeof(int));
                }
                foreach (MatchItem mi in allItems.Values)
                {
                    DataRow dr = dt.NewRow();
                    dr["Id"] = mi.LongItem;
                    for (int i = 0; i < this.InputMinTimes; i++)
                    {
                        dr[string.Format("MCnt_{0}", this.ChipCount - i)] = mi.SubItemMatchCnt[this.ChipCount - i];
                    }
                    dt.Rows.Add(dr);
                }
                double cnt4avg = allItems.Values.Select(a => a.SubItemMatchCnt[4]).Average();
                double cnt3avg = allItems.Values.Select(a => a.SubItemMatchCnt[3]).Average();
                double cnt2avg = allItems.Values.Select(a => a.SubItemMatchCnt[2]).Average();
                double cnt5avg = allItems.Values.Select(a => a.SubItemMatchCnt[5]).Average();
                double cnt4std = ProbMath.CalculateStdDev(allItems.Values.Select(a => a.SubItemMatchCnt[4]));
                double cnt3std = ProbMath.CalculateStdDev(allItems.Values.Select(a => a.SubItemMatchCnt[3]));
                double cnt2std = ProbMath.CalculateStdDev(allItems.Values.Select(a => a.SubItemMatchCnt[2]));
                double cnt5std = ProbMath.CalculateStdDev(allItems.Values.Select(a => a.SubItemMatchCnt[5]));
                DataTable sdt = dt.Clone();
                DataView dv = new DataView(dt);
                double stdcnt = 2;
                //
                string sql = string.Format("MCnt_5<={0} and MCnt_4>{1} and MCnt_3<={2} and MCnt_2<={3}", Math.Max(cnt5avg - stdcnt * cnt5std, 0), cnt4avg + stdcnt * cnt4std, Math.Max(0, cnt3avg - 0 * cnt3std), Math.Max(0, cnt2avg - 0 * cnt2std));

                //string sql = string.Format("MCnt_5<={0} and MCnt_4>{1} and MCnt_3<={2}", Math.Max(cnt5avg - stdcnt * cnt5std, 0), cnt4avg + stdcnt * cnt4std, Math.Max(0, cnt3avg - 0 * cnt3std));
                dt.Select(sql).ToList().ForEach(a => sdt.Rows.Add(a.ItemArray));
                dv = new DataView(sdt);

                //4个匹配要求最低。
                CombinClass cc = new CombinClass();
                for (int i = 0; i < Math.Min(dv.Count, 100); i++)
                {
                    cc.Add(dv[i].Row[0].ToString());
                }
                ret = getChances(sc, cc, AllArr, ed);
            }
            catch(Exception ce)
            {
                Log(ce.Message, ce.StackTrace);
            }
            return ret;
        }

        public override double getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            ExpectList data = new ExpectList(LastUseData()?.Table);
            if(data == null)
            {
                return 0;
            }
            if(data.LastData.Expect == cc.ExpectCode)
            {
                return 1;
            }
            return 0;
        }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }

        public override Type getTheChanceType()
        {
            return typeof(ChanceClass_ForCombinXxY);
        }
    }
}

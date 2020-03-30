using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
using System.Linq;
using WolfInv.com.ProbMathLib;
using System;

namespace WolfInv.com.PK10CorePress
{

    public interface iXxYClass
    {
         int AllNums { get; set; }
         int SelectNums { get; set; }

         string strAllTypeBaseOdds { get; set; }

         string strCombinTypeBaseOdds { get; set; }

         string strPermutTypeBaseOdds { get; set; }
    }


    public class ChanceClass_ForCombinXxY:ChanceClass, iXxYClass
    {
        public ChanceClass_ForCombinXxY()
        {

        }
        public int AllNums { get; set; }
        public int SelectNums { get; set; }

        public string strAllTypeBaseOdds { get; set; }

        public string strCombinTypeBaseOdds { get; set; }

        public string strPermutTypeBaseOdds { get; set; }

        List<ChanceItem> _SubItems;
        public List<ChanceItem> SubItems
        {
            get
            {
                if(_SubItems == null)
                {
                    if (this.ChanceCode == null)
                        return new List<ChanceItem>();
                    _SubItems = new List<ChanceItem>();

                    string[] arr = this.ChanceCode.Split('+');
                    for(int i=0;i<arr.Length;i++)
                    {
                        ChanceItem ci = new ChanceItem(arr[i]);
                        _SubItems.Add(ci);
                    }
                }
                return _SubItems;
            }
        }


        string[] _allArr = null;
        public string[] AllNumArr
        {
            get
            {
                if(_allArr==null)
                {
                    _allArr = CombinClass.CreateNumArr(AllNums);
                }
                return _allArr;
            }
        }
        public override bool Matched(ExpectData<TimeSerialData> data, out int MatchCnt)
        {

            ExpectList<TimeSerialData> el = new ExpectList<TimeSerialData>();
            el.Add(data);
            MatchCnt = 0;
            return Matched(el,out MatchCnt,false);
        }

        Dictionary<int, int> baseodds = null;
        Dictionary<int,int> AllTypeChipsBaseOdds
        {
            get
            {
                if(baseodds==null)
                {
                    if (this.strAllTypeBaseOdds == null || this.strAllTypeBaseOdds.Length == 0)
                        return new Dictionary<int, int>();
                    baseodds = new Dictionary<int, int>();
                    int basindex = 2;
                    string[] arr = strAllTypeBaseOdds.Split(',');
                    for(int i=0;i<arr.Length;i++)
                    {
                        baseodds.Add(i + basindex, int.Parse(arr[i]));
                    }
                }
                return baseodds;
            }
        }

        Dictionary<int, int> CombinTypebaseodds = null;
        Dictionary<int, int> CombinTypeChipsBaseOdds
        {
            get
            {
                if (CombinTypebaseodds == null)
                {
                    if (this.strCombinTypeBaseOdds == null || this.strCombinTypeBaseOdds.Length == 0)
                        return new Dictionary<int, int>();
                    CombinTypebaseodds = new Dictionary<int, int>();
                    int basindex = 2;
                    string[] arr = strCombinTypeBaseOdds.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        CombinTypebaseodds.Add(i + basindex, int.Parse(arr[i]));
                    }
                }
                return CombinTypebaseodds;
            }
        }

        Dictionary<int, int> PermutTypebaseodds = null;
        Dictionary<int, int> PermutTypeChipsBaseOdds
        {
            get
            {
                if (PermutTypebaseodds == null)
                {
                    if (this.strPermutTypeBaseOdds == null || this.strPermutTypeBaseOdds.Length == 0)
                        return new Dictionary<int, int>();
                    PermutTypebaseodds = new Dictionary<int, int>();
                    int basindex = 1;
                    string[] arr = strPermutTypeBaseOdds.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        PermutTypebaseodds.Add(i + basindex, int.Parse(arr[i]));
                    }
                }
                return PermutTypebaseodds;
            }
        }


        public override bool Matched(ExpectList<TimeSerialData> el, out int MatchCnt, bool getRev)
        {
            //ExpectData data = el.LastData;
            if(ChanceCode == null)//停止
            {
                MatchCnt = 0;
                return true;
            }
            string[] strArr = ChanceCode.Split('+');
            MatchCnt = 0;
            int begid = -1;//默认为0，多期再去寻找起点
            if (el.Count > 1)//多期考虑
            {
                if (long.Parse(this.ExpectCode) < long.Parse(el.FirstData.Expect))
                {
                    Log("错误", "匹配是否需要关闭错误", "需要关闭的机会早于数据以前,无论是否中奖，立即关闭！");
                    return true;
                }
                begid = el.IndexOf(this.ExpectCode);
                if (begid < 0)
                {
                    Log("错误", "匹配是否需要关闭错误", "需要关闭的机会不在数据中,无论是否中奖，立即关闭！");
                    return true;
                }
            }
            //////ExpectData inputEd = el[begid];
            //////Log("计算服务", "获取到期号信息", string.Format("expect:{0};openCode:{1}",inputEd.Expect,inputEd.OpenCode));
            if (el.Count>begid+2 ||el.Count == 1)//只匹配一期
            {
                int ei = begid + 1;
                ExpectData<TimeSerialData> data = el[ei];
                string[] strOrgArr = data.OpenCode.Split(',');
                string strRes =  CombinGenerator.ResortNumString(data.OpenCode,",");
                foreach (ChanceItem ci in this.SubItems)
                {
                    CombinClass container = null;
                    string[] strCiArr = null;
                    if(ci.betChipCnt<=SelectNums)
                    {
                        strCiArr = new string[ci.betChipCnt];
                        Array.Copy(strOrgArr, strCiArr, ci.betChipCnt);
                    }
                    switch (ci.betType)
                    {
                        case CombinBetType.Permut:
                            {
                                if(string.Join(",", strCiArr) == ci.betCode)//未排序的数组和投注串完全相等
                                {
                                    if (this.PermutTypebaseodds.ContainsKey(ci.betChipCnt-1))
                                    {
                                        MatchCnt += PermutTypebaseodds[ci.betChipCnt-1];
                                        //break;
                                    }
                                    else//找不到，返回基本倍数
                                    {
                                        MatchCnt += 2;
                                    }
                                }
                                
                                break;
                            }
                        case CombinBetType.Combin:
                            {
                                string[] strSArr = CombinGenerator.ResortNumString(strCiArr);// 获取前N位排序好的中奖号码
                                if (string.Join(",", strSArr) == CombinGenerator.ResortNumString(ci.betCode,",")) //如果排序好的前N位中奖串等于重排后的投注串，命中
                                {
                                    if (this.CombinTypebaseodds.ContainsKey(ci.betChipCnt-1))
                                    {
                                        MatchCnt += CombinTypebaseodds[ci.betChipCnt-1];
                                        //break;
                                    }
                                    else//找不到，返回基本倍数
                                    {
                                        MatchCnt += 2;
                                    }
                                }
                                break;
                            }
                        case CombinBetType.All:
                        default:
                            {
                                int tn = ci.betChipCnt+1;//一定要加1，是标志基础
                                if (ci.betChipCnt > SelectNums)//如果大于选择码
                                {
                                    container = new CombinClass(ci.betCode, SelectNums);//找出N码选出数目的所有组合
                                    if(container.Contains(strRes))//命中
                                    {
                                        
                                        if (this.AllTypeChipsBaseOdds.ContainsKey(tn))
                                        {
                                            
                                            MatchCnt +=AllTypeChipsBaseOdds[tn];
                                            //break;
                                        }
                                        else//找不到，返回基本倍数
                                        {
                                            MatchCnt += 2;
                                        }
                                    }
                                }
                                else
                                {
                                    container = new CombinClass(strRes, ci.betChipCnt);//中奖结果的C(S,N)组合
                                    if(container.Contains(ci.betCode))
                                    {
                                        if (this.AllTypeChipsBaseOdds.ContainsKey(tn))
                                        {
                                            MatchCnt +=AllTypeChipsBaseOdds[tn];
                                            //break;
                                        }
                                        else//找不到，返回基本倍数
                                        {
                                            MatchCnt += 2;
                                        }
                                    }
                                }
                                break;
                            }
                    }
                    if(ci.betChipCnt>= SelectNums && MatchCnt>0)//只要有一个子长机会命中，就不再检查其他子机会
                    {
                        break;
                    }
                }
                
            }
            if (MatchCnt > 0)
                return true;
            return false;
        }

        public static string getRevChance(string code)
        {
            string[] strarr = code.Split('/');
            string strtype = strarr[0];
            string strsel = strarr[1];
            int selnum = int.Parse(strtype.Substring(1));
            
            return null;
        }


        public override void CalcProfit(double matchcnt)
        {
            //输入的matchcnt 要除以2
            double LastOdds = matchcnt * 11.0 / 2.0 / 2.0 * Odds/10;//翻了2倍，然后还要除以最小投注倍数2，最后要乘以整体赔率9.50-9.78/10
            this.Gained = matchcnt * LastOdds * this.UnitCost;
            this.Profit = this.Gained - this.Cost;
            return;
            ////string[] oddsArr = "2,6,24,168,28,8,3".Split(',');//要除以2*11
            ////int FixOdd = 11;
            ////Dictionary<int, double> CombOdds = new Dictionary<int, double>();
            ////for (int i = 2; i <= 8; i++)
            ////{
            ////    double LastOdds = int.Parse(oddsArr[i - 2]) * 11.0 / 2.0 / 2.0 * Odds;//翻了2倍，然后还要除以最小投注倍数2，最后要乘以整体赔率9.50-9.78/10
            ////}
        }

        public class ChanceItem
        {
            public CombinBetType betType;
            public string betCode;
            /// <summary>
            /// 投注数目，此变量只对Ａ，Ｃ，Ｐ　（１～８）有效，对趣味玩法全部无效
            /// </summary>
            public int betChipCnt;
            
            public ChanceItem()
            {

            }
            public ChanceItem(string code)
            {
                string[] arr = code.ToUpper().Split('/');
                betCode = arr[1];
                betType = arr[0][0] == 'A' ? CombinBetType.All : (arr[0][0] == 'C' ? CombinBetType.Combin : CombinBetType.Permut);
                betChipCnt = int.Parse(arr[0].Substring(1));//只对ACP（1~8有效）
            }
        }

        public override double getRealOdds()
        {
            double LastOdds = this.MatchChips * 11.0 / 2.0 / 2.0 * Odds / 10;//翻了2倍，然后还要除以最小投注倍数2，最后要乘以整体赔率9.50-9.78/10
            return LastOdds;
        }
    }

    public enum CombinBetType
    {
        All,
        Combin,
        Permut
    }
}

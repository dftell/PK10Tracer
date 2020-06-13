using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;
using System.Data;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.Strags.KLXxY
{
    [DescriptionAttribute("X选Y外部遗漏优化投注策略"),
        DisplayName("X选Y外部遗漏优化投注策略")]
    public class Strag_KLXxY_ExData_Missed : StragClass
    {
        public Strag_KLXxY_ExData_Missed()
        {
            _StragClassName = "通用X选Y外部遗漏优化投注策略";
        }
        public override bool CheckNeedEndTheChance(ChanceClass cc1, bool LastExpectMatched1)
        {
            return LastExpectMatched1;
        }
        CommCollection_KLXxY mSc = null;
        public override List<ChanceClass> getChances(BaseCollection bsc, ExpectData ed)
        {
            CommCollection_KLXxY sc = bsc as CommCollection_KLXxY;
            mSc = sc;
            DataTypePoint dtp = UsingDpt;
            string strLogType = string.Format("彩种:{0};类型:{1};目标:{2};Expect:{3}", dtp.ExDataConfig.interfaceKey, this.InputMaxTimes, this.InputMinTimes,ed.Expect);
            List<ChanceClass> ret = new List<ChanceClass>();
            MissDataCalcClass mic = new MissDataCalcClass(bsc);
            string[] strPeriods = dtp.ExDataConfig.periods.Split(',');
            Dictionary<string, MissDataItem> commList = null;
            int maxPeriod = int.Parse(strPeriods[strPeriods.Length - 1]);
            if (bsc.orgData.Count< maxPeriod)
            {
                Log(strLogType, string.Format("最大周期{0}的所需不能小于记录条数{1}!", maxPeriod, bsc.orgData.Count));
                return ret;
            }
            Dictionary<string, MissDataItem> Maxmissdata = mic.getMissData(bsc.orgData, dtp, ed.Expect, maxPeriod, InputMaxTimes.ToString(), InputMinTimes.ToString(), new object[0] { });
            Dictionary<string, int> maxMissVal = new Dictionary<string, int>();
            if (Maxmissdata == null)
            {
                return ret;
            }
            //Log(strLogType, string.Format("最大周期{0}的成员数量为{1}个!", strPeriods[strPeriods.Length - 1], Maxmissdata.Count));
            if (Maxmissdata.Count ==0)
            {
                
                return ret;
            }
            maxMissVal = Maxmissdata.ToDictionary(a => a.Key, a => int.Parse(a.Value.max_miss));
            int allMax = maxMissVal.Values.Max();
            for (int i= strPeriods.Length-1; i<strPeriods.Length;i++)//只看最大周期
            {
                int peroid = int.Parse(strPeriods[i]);
                int avg_times = (int) (bsc as CommCollection_KLXxY).getProbTimes(dtp, peroid, InputMaxTimes.ToString(), InputMinTimes.ToString(), new object[0]);
                Dictionary<string,MissDataItem> missdata =  mic.getMissData(bsc.orgData,dtp, ed.Expect, int.Parse(strPeriods[i]), InputMaxTimes.ToString(), InputMinTimes.ToString(), new object[0] { });
                if(missdata == null)
                {
                    return ret;
                }
                int currPeriodMax = missdata.Max(a => int.Parse(a.Value.max_miss));
                int currPeriodMissMax = missdata.Max(a => int.Parse(a.Value.miss));
                if(currPeriodMax*4< allMax*3)
                {
                    Log(strLogType, string.Format("当期最大数量遗漏数量{0}小于最大周期遗漏数量{1}的2/3，整期跳过!", allMax, currPeriodMissMax));
                    return ret;
                }
                missdata = missdata.Where(a =>
                {
                    int test = 0;
                    Single stest = 0F;
                    Single avg_miss = Single.TryParse(a.Value.avg_miss, out stest) ? stest : 0F;
                    //if (avg_miss == 0)
                    //    return false;
                    int times = a.Value.times;
                    int miss = int.TryParse(a.Value.miss.ToString(), out test) ? test : 0;
                    int miss1 = int.TryParse(a.Value.miss1?.ToString(), out test) ? test : 0;
                    int max_miss = int.TryParse(a.Value.max_miss.ToString(), out test) ? test : 0;
                    Single investment = Single.TryParse(a.Value.investment.ToString(), out stest) ? stest : 0F;
                    Single supplement = Single.TryParse(a.Value.supplement.ToString(), out stest) ? stest : 0F;
                    if(maxMissVal.ContainsKey(a.Key) == false)
                    {
                        return false;
                    }
                    
                    if(miss*5< allMax * 4)
                    {
                        return false;
                    }
                    return true;//所有当前次数大于最大周期最大的值3/4的值方允许通过
                    if(miss < allMax/2)
                    {
                        return false;
                    }
                    if(miss+miss1>allMax)
                    {
                        return true;
                    }
                    if(miss == currPeriodMax)
                    {
                        return true;
                    }
                    if(miss == currPeriodMissMax)
                    {
                        if(miss > currPeriodMax*2/3)
                        {
                            return miss > allMax / 3;
                        }
                    }
                    if (miss *3 < 2*miss1 && miss1 != max_miss)
                        return false;
                    if (i < 3)
                    {
                        if(3*miss < 2*max_miss)
                        {
                            return false;
                        }
                        return 2*times >  avg_times ;
                    }
                    else
                        return miss > (2 * max_miss / 3) && investment > 0;

                }).ToDictionary(a=>a.Key,a=>a.Value);
                Dictionary<string,MissDataItem> list = missdata.OrderByDescending(a => a.Value.investment).ToDictionary(a=>a.Key,a=>a.Value);
                if (list.Count == 0)
                    return ret;
                if (commList == null)
                {
                    commList = list;
                }
                else
                {
                    Dictionary<string, MissDataItem> test = list.Where(a => commList.ContainsKey(a.Key)).ToDictionary(a => a.Key, a => a.Value);
                    if (test.Count > 0)
                    {
                        commList = test;
                    }
                    else
                    {
                        if (i > 1)
                        {
                            //commList = test;
                            break;
                        }
                        else
                        {
                            commList = test;
                        }
                    }
                }
            }
            if (commList.Count == 0)
                return ret;
            Log(strLogType, string.Join(";", commList.Select(a=>string.Format("{0}=>最小周期遗漏信息[当前:{1};前1:{2};最大:{3};次数:{4}]",a.Key,a.Value.miss,a.Value.miss1,a.Value.max_miss,a.Value.times)).ToList()));
            foreach (string key in commList.Keys)
            {
                ChanceClass_ForCombinXxY cc = new ChanceClass_ForCombinXxY();
                cc.ChanceCode = string.Format("{0}{1}/{2}", InputMaxTimes > 30 ? "C" : (InputMaxTimes > 10) ? "P" : "A", InputMinTimes, key);
                Dictionary<int,int> ints =  cc.CombinTypeChipsBaseOdds;
                cc.AllNums = 11;
                cc.SelectNums = 5;
                cc.strAllTypeBaseOdds = sc.strAllTypeOdds;
                cc.strCombinTypeBaseOdds = sc.strCombinTypeOdds;
                cc.strPermutTypeBaseOdds = sc.strPermutTypeOdds;
                //cc.UnitCost = 2;
                cc.ChipCount = 1;
                cc.ChanceType = 2;
                cc.InputTimes = 1;
                //cc.AllowMaxHoldTimeCnt = this.AllowMaxHoldTimeCnt;
                cc.InputExpect = ed;
                cc.StragId = this.GUID;
                //cc.MinWinRate = this.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials.MaxRates[cc.ChipCount - 1];
                cc.IsTracer = 1;
                //cc.Tracerable
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
            return ret;
        }
        public  List<ChanceClass> getChances1(BaseCollection bsc, ExpectData ed)
        {
            DataTypePoint dtp = UsingDpt;
            List<ChanceClass> ret = new List<ChanceClass>();
            Web52CP_MissData_ProcessClass prs = new Web52CP_MissData_ProcessClass();
            if(!prs.hasNewsMissData(dtp,ed.LExpectNo.ToString()))
            {
                return ret;
            }
            //int[] periods = new int[] { 100,300,1000, 3000, 10000 };
            string[] strPeriods = dtp.ExDataConfig.periods.Split(',');
            DataSet ds = new DataSet();
            string strLogType = string.Format("彩种:{0};类型:{1};目标:{2}", dtp.ExDataConfig.interfaceKey, this.InputMaxTimes, this.InputMinTimes);
            for (int i = 0; i < strPeriods.Length; i++)//获取多个周期的遗漏数据
            {
                DataTable dt = prs.getData(dtp, null, dtp.ExDataConfig.argsModel, dtp.ExDataConfig.interfaceKey, strPeriods[i], this.InputMaxTimes, this.InputMinTimes);
                if(dt == null)
                {
                    Log(strLogType, string.Format("无法获取到{0}期外部数据",strPeriods[i]));
                    return ret;
                }
                dt.TableName = strPeriods[i];
                ds.Tables.Add(dt);
            }
            Dictionary<string,string> commList = null;
            for(int i=0;i<Math.Max(3,ds.Tables.Count);i++)
            {
                DataTable dt = ds.Tables[i];

                DataRow[] drs = dt.Select().Where(a => {
                    int test = 0;
                    Single stest = 0F;
                    Single avg_miss = Single.TryParse(a["avg_miss"].ToString(), out stest) ? stest : 0F;
                    if (avg_miss == 0)
                        return false;
                    int miss = int.TryParse(a["miss"].ToString(), out test) ? test : 0;
                    int miss1 = int.TryParse(a["miss1"].ToString(), out test) ? test : 0;
                    int max_miss = int.TryParse(a["max_miss"].ToString(), out test) ? test : 0;
                    Single investment = Single.TryParse(a["investment"].ToString(),out stest)?stest:0F;
                    Single supplement = Single.TryParse(a["supplement"].ToString(), out stest) ? stest : 0F;
                    if (i < 3)
                        return miss1 > 2 * avg_miss && miss >miss1 && miss == max_miss && investment > 0 ;
                    else
                        return miss1 > 2 * avg_miss && miss > miss1 && miss > (2 * max_miss / 3) && investment > 0;
                }).ToArray();
                drs = drs.OrderByDescending(a => a["investment"].ToString()).ToArray();
                Dictionary<string,string> list = drs.Select(a=>a["num"].ToString()).Take(20).ToDictionary(a=>a,a=>a);
                if(commList == null)
                {
                    commList = list;
                }
                else
                {
                    Dictionary<string,string> test = list.Where(a => commList.ContainsKey(a.Key)).ToDictionary(a=>a.Key,a=>a.Value);
                    if(test.Count>0)
                    {
                        commList = test;
                    }
                    else
                    {
                        if(i>2)
                        {
                            //commList = test;
                            break;
                        }
                        else
                        {
                            commList = test;
                        }
                    }
                }
            }
            Log(strLogType, string.Join(";", commList.Keys.ToList()));
            CommCollection_KLXxY sc = bsc as CommCollection_KLXxY;
            foreach (string key in commList.Keys)
            {
                ChanceClass_ForCombinXxY cc = new ChanceClass_ForCombinXxY();
                cc.ChanceCode = string.Format("{0}{1}/{2}",InputMaxTimes>30?"C":(InputMaxTimes>10)?"P":"A",InputMinTimes,key);
                cc.AllNums = 11;
                cc.SelectNums = 5;
                cc.strAllTypeBaseOdds = sc.strAllTypeOdds;
                cc.strCombinTypeBaseOdds = sc.strCombinTypeOdds;
                cc.strPermutTypeBaseOdds = sc.strPermutTypeOdds;
                cc.UnitCost = 2;
                cc.ChipCount = 1;
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

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            DataTypePoint dtp = UsingDpt;
            if (mSc != null)
            {
                string strCode = cc.ChanceCode;
                string type = strCode.Substring(0, 1);
                string target = strCode.Substring(1,1);
                amts = mSc.getOptSerials(dtp, type, int.Parse(target), this.CommSetting.Odds,allowInvestmentMaxValue, 1,false);
            }
            if (amts.Serials == null)
                return 1;
            return getDefaultChipAmount(RestCash, cc, amts);
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

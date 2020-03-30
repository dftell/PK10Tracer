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
            return true;
        }

        public override List<ChanceClass> getChances(BaseCollection sc, ExpectData ed)
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
            for (int i = 0; i < strPeriods.Length; i++)//获取多个周期的遗漏数据
            {
                DataTable dt = prs.getData(dtp, null, dtp.ExDataConfig.argsModel, dtp.ExDataConfig.interfaceKey, strPeriods[i], this.InputMaxTimes, this.InputMinTimes);
                dt.TableName = strPeriods[i];
                ds.Tables.Add(dt);
            }
            Dictionary<string,string> commList = null;
            for(int i=0;i<ds.Tables.Count;i++)
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
                    return miss1>2*avg_miss && miss> 2 * avg_miss&& miss>(2*max_miss/3) && investment>0 && supplement>0 ;
                }).ToArray();
                drs = drs.OrderByDescending(a => a["investment"].ToString()).ToArray();
                Dictionary<string,string> list = drs.Select(a=>a["num"].ToString()).Take(20).ToDictionary(a=>a,a=>a);
                if(commList == null)
                {
                    commList = list;
                }
                else
                {
                    commList = list.Where(a => commList.ContainsKey(a.Key)).ToDictionary(a=>a.Key,a=>a.Value);
                }
            }
            foreach(string key in commList.Keys)
            {
                ChanceClass cc = new ChanceClass();
                cc.ChanceCode = string.Format("{0}{1}/{2}",InputMaxTimes>30?"C":(InputMaxTimes>10)?"P":"A",InputMinTimes,key);
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
            return 1;
        }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }

        public override Type getTheChanceType()
        {
            return typeof(TraceChance);
        }
    }
}

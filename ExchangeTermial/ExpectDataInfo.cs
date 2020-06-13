using System.Collections.Generic;
using WolfInv.com.WebCommunicateClass;
using WolfInv.com.BaseObjectsLib;
//using WolfInv.Com.SocketLib;
using WolfInv.com.PK10CorePress;
using System;
using System.Threading.Tasks;
using WolfInv.com.WebRuleLib;
using WolfInv.com.WinInterComminuteLib;
using System.Data;
using System.Linq;
using WolfInv.com.RemoteObjectsLib;

namespace ExchangeTermial
{
    public class ExpectDataInfo
    {
        DataTypePoint dtp;
        public string ExpectCode;
        public Dictionary<long?, ChanceClass> ExpectChances;
        public List<InstClass> ExpectInsts;
        public double TotalAmount;
        public DataTable SelectTimeTable;
        public string SelectMessage;
        public ExpectDataStatus DataStatus;
        public long AllSum;
        RequestClass currRec;
        Dictionary<string, string> AssetUnitList;
        WebRule wr;
        public string Message;
        bool SendMsgFromWebRequest;
        public ExpectDataInfo(DataTypePoint _dtp,WebRule _wr, bool _SendMsgFromWebReques,  Dictionary<string, string> alist,RequestClass rc, string existExpect, bool IsRefreshData)
        {
            AssetUnitList = alist;
            dtp = _dtp;
            wr = _wr;
            SendMsgFromWebRequest = _SendMsgFromWebReques;
            ExpectDataReturnClass edrc = Load(rc, existExpect, IsRefreshData);
            if (edrc.succ)
            {
                currRec = rc;
                Message = edrc.Message;//也加上，中间过程可能有错误。
                DataStatus = ExpectDataStatus.Created;
            }
            else
            {
                //一定要置空，中间作为了临时变量
                SelectMessage = null;
                SelectTimeTable = null;
                DataStatus = ExpectDataStatus.NoValidate;
                Message = edrc.Message;
            }
        }

        public ExpectDataReturnClass Load(RequestClass ic,string ExistExpect,bool IsRefreshData)
        {
            ExpectDataReturnClass ret = new ExpectDataReturnClass();
            if (!string.IsNullOrEmpty(ic.Error))//如果返回错误
            {
                ret.Message = ic.Error;
                return ret;
            }
            ExpectChances = new Dictionary<long?, ChanceClass>();
            ExpectInsts = new List<InstClass>();
            if (ic == null)
            {
                ret.Message = "指令内容错误！";
                //Program.wxl.Log(string.Format("[{0}]指令内容错误！", Program.gc.ClientAliasName), string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
                return ret;
            }
            Int64 CurrExpectNo = Int64.Parse(ic.Expect);
            Int64 ExistExpectNo = Int64.Parse(ExistExpect);
            DateTime CurrTime = DateTime.Now.AddHours(dtp.DiffHours).AddMinutes(dtp.DiffMinutes);//调整后的时间
            //string strList = "";
            //if ((CurrExpectNo > this.NewExpects[dtp.DataType] && this.NewExpects[dtp.DataType] > 0) || (RefreshTimes == 0)) //获取到最新指令
            if ((CurrExpectNo > ExistExpectNo && ExistExpectNo > 0)|| IsRefreshData == false)
            {
                ic.SelectTimeChanged = AfterSelectTimeProcess;
                ic.SetChanceList = (dc) => {
                    foreach(ChanceClass cc in dc.Keys)
                    {
                        long amt = dc[cc];
                        if (amt == 0)
                            continue;
                        if(!this.ExpectChances.ContainsKey(cc.ChanceIndex))
                        {
                            this.ExpectChances.Add(cc.ChanceIndex, cc);
                        }
                    }
                };
                ret.InstsText = ic.getUserInsts(Program.gc, dtp, ic.Expect, Program.gc.ForWeb, TotalAmount,getSelectInsts);
                string[] insts = ret.InstsText.Trim().Replace("+", " ").Split(' ');
                ret.AllSum = insts.Where(a => a.Trim().Length > 1).ToList().Select(a => long.Parse(a.Trim().Split('/')[2])).Sum();
                GlobalClass.SetConfig(Program.gc.ForWeb);
            }
            ret.succ = true;
            return ret;
        }



        SelectTimeInstClass getSelectInsts(string dp, string ec, AssetInfoConfig aic)
        {
            SelectTimeInstClass sti = new SelectTimeInstClass();
            //string selectTimeAmtUrlModel = "pk10/app/getwaveselecttimeamount.asp?type={0}&expect={1}&cnt={2}&defaultReturnValue={3}&AutoResumeDefaultValue={4}";
            CommunicateToServer cts = new CommunicateToServer();
            /*
                         "pk10/app/getwaveselecttimeamount.asp?
                         type={0}
                         &expect={1}
                         &cnt={2}
                         &defaultReturnValue={3}
                         &AutoResumeDefaultValue={4}
                         &EmergencyStop={5}
                         &StopIgnoreLength={6}
                         &StopStepLen={7}
                         &StopPower={8}";
            */
            ////CommResult cs = cts.getRequestInsts<SelectTimeInstClass>(string.Format("{0}/{1}", dtp.InstHost, string.Format(selectTimeAmtUrlModel, dp, ec, cnt,defaultReturnValue,AutoResumeDefaultValue)));
            string selectTimeAmtUrlModel = "pk10/app/getwaveselecttimeamount.asp?type={0}&expect={1}&cnt={2}&defaultReturnValue={3}&AutoResumeDefaultValue={4}&EmergencyStop={5}&StopIgnoreLength={6}&StopStepLen={7}&StopPower={8}";
            CommResult cs = cts.getRequestInsts<SelectTimeInstClass>(string.Format("{0}/{1}", dtp.InstHost, string.Format(
                selectTimeAmtUrlModel,
                dp,
                ec,
                aic.CurrTimes,
                aic.DefaultReturnTimes,
                aic.AutoResumeDefaultReturnValue,
                aic.EmergencyStop,
                aic.StopIgnoreLength,
                aic.StopStepLen,
                aic.StopPower
                )));
            if (!cs.Succ)
            {
                sti.Error = cs.Message;
                return sti;
            }
            if (cs.Cnt != 1)
            {
                sti.Error = "择时信息异常！";
                return sti;
            }
            sti = cs.Result[0] as SelectTimeInstClass;
            return sti;
        }

        void AfterSelectTimeProcess(string a, int b, SelectTimeInstClass c, ChanceClass cc)
        {
            Single currProb100 = 0.0F;
            Single safeProb100 = 0.0F;
            Single currProb300 = 0.0F;
            Single safeProb300 = 0.0F;
            Single currProb1000 = 0.0F;
            Single safeProb1000 = 0.0F;
            int No2MatchCnt = 0;
            if (c.List.Count > 0)
            {

                currProb100 = getProbByPeriod(c.List, 100, out safeProb100);
                currProb300 = getProbByPeriod(c.List, 300, out safeProb300);
                currProb1000 = getProbByPeriod(c.List, 1000, out safeProb1000);
                DataTable dt = DisplayAsTableClass.ToTable<MatchGroupClass>(c.List);
                SelectTimeTable = dt;
                DataRow[] drs = dt.Select("MatchCnt>1", "SerNo");
                int maxId = int.Parse(dt.Rows[dt.Rows.Count - 1]["SerNo"].ToString());//最后一个SerNo
                if (drs.Length > 0)
                {
                    maxId = int.Parse(drs[0]["SerNo"].ToString());
                }
                drs = dt.Select("SerNo < " + maxId);
                No2MatchCnt = drs.ToList().Sum(dr => int.Parse(dr["chipCount"].ToString()));

            }
            string strProbModel = "概率(%)100期:当前[{0}]/安全[{1}];300期:当前[{2}]/安全[{3}];1000期:当前[{4}]/安全[{5}];持续{6}次机会未出双响！";
            string strProb = string.Format(strProbModel, currProb100, safeProb100, currProb300, safeProb300, currProb1000, safeProb1000, No2MatchCnt);
            string strList = string.Format("[{0}期提示:[机会:{1};数量:{2}]];{3}最近5轮出现的组合信息为:{4}", this.ExpectCode, cc.ChanceCode, cc.UnitCost, strProb, string.Join(",", c.List.Select(curr => string.Format("{0}次{1}个组合", curr.times, curr.chipCount)).Take(5).ToArray()));
            string strDiff = "";
            if (b != c.RequestCnt)
                strDiff = string.Format("资产单元{0}信号由{1}改为{2}!", AssetUnitList[a], b, c.ReturnCnt);
            SelectMessage = strList;

        }
                

        public static Single getProbByPeriod(List<MatchGroupClass> list, int n, out Single safeProb)
        {
            safeProb = 0.00F;
            try
            {
                list = list.Take(n).ToList();
                if (list.Count < n)
                {
                    return 0.0F;
                }
                int currChips = list[0].chipCount;
                MatchGroupClass lastMaxObj = list.Skip(1).Where(a => a.chipCount >= currChips)?.OrderByDescending(a => a.chipCount * 100 + a.times)?.First();
                if (lastMaxObj == null)
                {
                    lastMaxObj = list.Skip(1).Where(a => a.chipCount <= currChips)?.OrderByDescending(a => a.chipCount * 100 + a.times)?.First();
                }
                if (lastMaxObj == null)
                {
                    return 0.0F;
                }
                list = list.Where(a => a.SerNo < lastMaxObj.SerNo).ToList();
                int matchCnt = list.Sum(a => a.MatchCnt);
                int ccChips = list.Sum(a => a.chipCount);
                safeProb = (Single)((matchCnt + 1) * 100.00 / ccChips);
                Single ret = (Single)((matchCnt * 100.00) / ccChips);
                return ret;
            }
            catch (Exception ce)
            {
                //WXMsgBox("获取概率错误！", string.Format(ce.Message, ce.StackTrace));
                return 0;
            }
        }

    }
    class WebData
    {
        public string orderNum;
        public string ImgData;
        public string chargeAmt;
        public string err_msg;
        public string SelectTimeInfoText;
        public DataTable SelectTable;
        public int LoadCnt;
    }
    public class ExpectDataReturnClass
    {
        public bool succ;
        public string Message;
        public string InstsText;
        public long AllSum;
        public Dictionary<string, ChanceClass> Chances;

    }
}

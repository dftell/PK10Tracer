using System.Collections.Generic;
using WolfInv.com.CFZQ_LHProcess;
using WAPIWrapperCSharp;
using WolfInv.com.StrategyLibForWD;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.PK10CorePress
{
    public class WD_SubscriptTimeDataClass : SubscriptClass
    {
        protected static SystemGlobal sglb;
        public WD_SubscriptTimeDataClass()
        {
            if (sglb == null)
                sglb = new SystemGlobal(new WindAPI());
        }

        public override void CancelSubscript()
        {
            if (ReqId == 0) return;
            SystemGlobal.w.cancelRequest(ReqId);
            for (int i = 0; i < ExecObjs.Length; i++)
            {
                ExecObjs[i] = null;
            }
            ExecObjs = null;
        }

        public override ulong Subscript(string[] grpcodes, string Fields, string options, WindCallback wc)
        {
            int errcode = 0;
            return SystemGlobal.w.wsq(ref errcode, string.Join(",", grpcodes), Fields, options, wc, UpdateAll);
        }


        public override Dictionary<string, ExpectList> RequestData(string[] grpcodes, string Fields, string options)
        {
            WindData wd = SystemGlobal.w.wsq(string.Join(",", grpcodes), Fields, options);
            DataTable dt = WDDataAdapter.getRecords(wd);
            Dictionary<string, ExpectList> ret = new Dictionary<string, ExpectList>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ExpectList el = new ExpectList();
                
                //ret.Add();
            }
            return ret;
        }

        public override void getData(string Fields, string options, int grpCnt, bool NeedReqData)
        {

            if (this.SubscriptCodes.Count == 0) throw new WDErrorException(SystemGlobal.w, -9999);

            GrpSubScript(Fields, options, grpCnt, NeedReqData);

        }

        public override bool GetAllEquits(string SectorCode)
        {
            if (SystemGlobal.AllSecSet.Count == 0)
                return false;
            if (!SystemGlobal.AllSecSet.ContainsKey(SecType.Equit))
                return false;
            Dictionary<string, SecurityInfo> allsecs = SystemGlobal.AllSecSet[SecType.Equit];
            foreach (string key in allsecs.Keys)
            {
                SubscriptCodes.Add(allsecs[key].BaseInfo.SecCode);
            }
            return true;
        }




        /// <summary>
        /// 分组订阅
        /// </summary>
        /// <param name="grpCnt">分组数量，默认-1，所有列表元素分别建立线程开始订阅</param>

    }

}
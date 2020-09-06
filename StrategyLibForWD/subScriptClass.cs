using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.LogLib;
using WolfInv.com.CFZQ_LHProcess;
using WAPIWrapperCSharp;
using WolfInv.com.StrategyLibForWD;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.PK10CorePress
{
    public delegate void UpdateDataAction(Dictionary<string, ExpectList> els);
    public abstract class SubscriptClass : LogableClass
    {

        protected ulong ReqId;
        public List<string> SubscriptCodes;
        public Dictionary<string, ExpectList> Data;
        public bool UpdateAll;
        public UpdateDataAction AfterUpdate;
        protected ExecClass[] ExecObjs;
        //protected abstract void getResult(ulong reqid,WindData data);
        public abstract void getData(string Fields, string options, int grpCnt, bool NeedRequestData);
        public abstract ulong Subscript(string[] grpcodes, string Fields, string options, WindCallback wc);
        public abstract Dictionary<string, ExpectList> RequestData(string[] grpcodes, string Fields, string options);
        //public ulong wsq(ref int errCode, string windCodes, string fields, string options, WindCallback wc, bool updateAll = true) 
        public abstract void CancelSubscript();

        public abstract bool GetAllEquits(string SectorCode);

        public void GrpSubScript(string Fields, string options, int grpCnt, bool NeedRequestData)
        {
            int errCode = 0;
            int realcnt = grpCnt;
            if (grpCnt < 0)
                realcnt = this.SubscriptCodes.Count;
            int grpInnerCnt = (int)Math.Ceiling((double)SubscriptCodes.Count / realcnt);//每组个数
            int lastInnerCnt = SubscriptCodes.Count - grpInnerCnt * (realcnt - 1);
            ExecObjs = new ExecClass[realcnt];
            for (int i = 0; i < realcnt; i++)
            {
                int copycnt = grpInnerCnt;
                string[] grpcodes;
                if (i == realcnt - 1)
                {
                    copycnt = lastInnerCnt;
                }
                grpcodes = new string[copycnt];
                SubscriptCodes.CopyTo(i * grpInnerCnt, grpcodes, 0, copycnt);//复制到数组中去
                ExecClass thrdobj = new ExecClass();
                thrdobj.AfterUpdate = this.AfterUpdate;
                if (NeedRequestData)
                {
                    Dictionary<string, ExpectList> initData = this.RequestData(grpcodes, Fields, options);
                }
                else
                {
                    ReqId = this.Subscript(grpcodes, Fields, options, thrdobj.getResult);
                }
                ExecObjs[i] = thrdobj;
                if (errCode != 0)
                {
                    throw new WDErrorException(SystemGlobal.w, errCode);
                }
            }
        }


        protected class ExecClass
        {
            public UpdateDataAction AfterUpdate;
            Dictionary<string, ExpectList> Data;
            public void getResult(ulong reqid, WindData data)
            {
                DataTable dt = WDDataAdapter.getRecords(data);
                List<DetailStringClass> retList = WDDataAdapter.GetList(dt);
                AfterUpdate(Data);
            }
        }
    }

    public delegate void FinishedOpt(DataTypePoint dtp, Dictionary<string, ExpectList> el);

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogLib;
using CFZQ_LHProcess;
using WAPIWrapperCSharp;
using StrategyLibForWD;
namespace PK10CorePress
{
    public delegate ExpectData CallBackRequest();
    public abstract class SubscriptClass: LogableClass
    {
        SystemGlobal sglb = new SystemGlobal(new WindAPI());
        public List<string> SubscriptCodes;
        public bool UpdateAll;
        public CallBackRequest getResult;
        public abstract void Subscript(ref int ErrCode, string Fields, string options, CallBackRequest CallBackRequest,bool UpdateAll);
        //public ulong wsq(ref int errCode, string windCodes, string fields, string options, WindCallback wc, bool updateAll = true) 
        public abstract void CancelSubscript();
    }

    public class WD_SubscriptTimeDataClass:SubscriptClass
    {
        public WD_SubscriptTimeDataClass()
        {

        }

        public override void CancelSubscript()
        {
            throw new NotImplementedException();
        }

        public override void Subscript(ref int ErrCode, string Fields, string options, CallBackRequest CallBackRequest, bool UpdateAll)
        {
            WindAPI w = new WindAPI();
            w.start();
        }
    }
}

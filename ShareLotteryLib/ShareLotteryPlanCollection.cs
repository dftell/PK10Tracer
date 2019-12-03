using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
//using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.WXMessageLib;
namespace WolfInv.com.ShareLotteryLib
{
    [Serializable]
    public class ShareLotteryPlanCollection:Dictionary<string,ShareLotteryPlanClass>
    {
        public WXMsgProcess MsgProcess { get; set; }
        public ShareLotteryPlanCollection()
        {
            MsgProcess = new WXMsgProcess();
            MsgProcess.ProcessName = "fddffdsfds";
            MsgProcess.AllPlan = this;
        }

        protected ShareLotteryPlanCollection(SerializationInfo info, StreamingContext context):base(info,context)
        {
            
            //Value = info.GetBoolean("Test_Value");
            try
            {
                MsgProcess = (WXMsgProcess)info.GetValue("ShareLotteryPlanCollection_MsgProcess", typeof(WXMsgProcess));
                MsgProcess.AllPlan = this;
            }
            catch(Exception ce)
            {

            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ShareLotteryPlanCollection_MsgProcess", MsgProcess,typeof(WXMsgProcess));
        }

        public Dictionary<string,Dictionary<string, ShareLotteryPlanClass>> UserOwnerPlans
        {
            get
            {
                Dictionary<string,Dictionary<string, ShareLotteryPlanClass> > ret = new Dictionary<string,  Dictionary<string, ShareLotteryPlanClass>>();
                var groups = this.GroupBy(a => a.Value.creator);
                foreach(var obj in groups)
                {
                    string skey = obj.Key;
                    ret.Add(skey,obj.ToDictionary(a => a.Key, a => a.Value));

                }
                return ret;
            }
        }

        public ShareLotteryPlanClass getCurrRoomPlan(string roomid)
        {
            if (roomid != null)
            {
                var res = this.Where(a => {
                    if(a.Value.wxRootNo == roomid && a.Value.sharePlanStatus != SharePlanStatus.Completed)
                    {
                        return true;
                    }
                    return false;
                });
                if(res.Count()>0)
                {
                    return res.First().Value;
                }
                return null;
            }
            return null;
        }

        public bool setCurrPlan(ShareLotteryPlanClass plan)
        {
            if(!this.ContainsKey(plan.guid))
            {
                this.Add(plan.guid, plan);
            }
            else
            {
                this[plan.guid] = plan;
            }
            return true;
        }

        public Dictionary<string,KeyText> getUserCreatedLotteries(string user)
        {
            Dictionary<string, KeyText> ret = new Dictionary<string, KeyText>();
            if (user == null)
                return ret;
            if (UserOwnerPlans == null)
                return ret;
            if (!UserOwnerPlans.ContainsKey(user))
                return ret;
            UserOwnerPlans[user].Select(a => a.Value.betLottery);
            return ret;
        }

    }

    

    

}

using WolfInv.com.WXMessageLib;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WolfInv.Com.WCS_Process;
namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_ShowSystemInfo : ResponseActionClass
    {
        public ResponseAction_ShowSystemInfo(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "响应显示合买信息";
        }

        protected override string getMsg()
        {
            string m = 
@"当前合买信息如下:
{0}
目前认购信,共计:{1}人{2}份
{3}
";
            string ret = string.Format(m, 
                currPlan.ToBasePlanInfo(),
                currPlan.subscribeList.GroupBy(a=>a.betWxName).Count(),
                currPlan.subscribeList.Sum(a=>a.subscribeShares), 
                currPlan.ToGroupedSubscribeInfo());
            return ret;
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            Regex regTr = new Regex(actionDefine.regRule);
            MatchCollection mcs = regTr.Matches(pureMsg);
            string matchKey = null;
            string content = null;
            bussinessConfigClass bcc = null;
            Dictionary<string, string> allList = null;
            matchResult mr = null;
            string useBussinessType = actionDefine.useBussinessType;
            matchKey = useBussinessType;
            bool Matched = false;
            bool isSystem = false;
            if (mcs.Count == 1 && mcs[0].Groups.Count > actionDefine.typeIndex + 1)
            {
                matchKey = mcs[0].Groups[1].Value;
                content = mcs[0].Groups[2].Value;
                Matched = true;
                isSystem = matchKey.Trim() == "系统";
            }
            if(!Matched)
            {
                answerMsg("无法识别的命令！");
                return false;
            }
            if(wxprocess.currChatUser == null && !isSystem)
            {
                answerMsg("请先登录！");
                return false;
            }
            if(isSystem)
            {
                string[] atts = content.Split(' ');
                answerMsg(string.Format("{0}:{1}", content,GlobalShare.GlobalData?.getSystemInfo(atts[0],atts.ToList().Skip(1).ToArray())));
            }
            else
            {
                answerMsg(string.Format("{0}:{1}", content, wxprocess.currChatUser.getUserInfo(content)));
            }
            return true;
                optPlan = null;
            if (currPlan != null)
                optPlan = currPlan;
            answerMsg(optPlan==null?"本群目前没有合买":null);
            return true;
        }
    }


}

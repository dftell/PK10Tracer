using System.Collections.Generic;
using System.Text.RegularExpressions;
using WolfInv.com.WXMessageLib;
using System.Linq;

namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_ValidateInfo : ResponseActionClass
    {
        List<string> answers = new List<string>();
        public ResponseAction_ValidateInfo(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            Regex regTr = new Regex(string.Format(@"\d+", pureMsg));
            MatchCollection mcs = regTr.Matches(pureMsg);
            int chargeAmt = 0;

            for(int i=0;i<mcs.Count;i++)
            {
                
                answers.Add(mcs[i].Value);
            }
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            optPlan = null;
            lastAsk = wxprocess.getLastAsk(roomId, requestUser);
            if(lastAsk == null)
            {

                return base.Response(ref optPlan);//当作不知道
            }
            if(answers.Count == 0)
            {
                answerMsg("请按提示回答上一个问题！");
                return false;
            }
            MutliLevelData mld = lastAsk.askData;
            if (!mld.SubList.Keys.Select(a => a.key).ToList().Contains(answers[0]))
            {
                if (mld != null && mld.Parent!= null && mld.Parent.SubList.Keys.Select(a => a.key).ToList().Contains(answers[0]))//返回上级
                {
                    MutliLevelData parent = mld.Parent?.Parent;
                    mld = mld.Parent;
                    if(mld.Parent!=null)
                    {
                        mld.Parent = parent;
                    }
                }
                else
                {
                    string msg = @"你回复的数字未在我期望范围内:请继续回复上一个问题！";
                    answerMsg(msg);
                    answerMsg(lastAsk.askMsg);//重复上一个问题
                    return false;
                }
            }
            KeyText result = mld.SubList.Where(a => a.Key.key == answers[0]).First().Key;
            lastAsk.AnswerResult = result;
            if (lastAsk.UserResponseAnswer == null)
            {
                lastAsk.UserResponseAnswer = new List<string>();
            }
            lastAsk.UserResponseAnswer.Add(answers[0]);
            if (mld.SubList[result]!= null && mld.SubList[result].SubList.Count>0)//如果还有下级问题，继续问
            {
                lastAsk.askData = mld.SubList[result];
                lastAsk.askData.Parent = mld;
                lastAsk.askMsg = lastAsk.AskText;
                lastAsk.LastRequestWaitResponse.lastAsk = lastAsk;
                wxprocess.setLastAsk(lastAsk);
                answerMsg(lastAsk.askMsg);
                return false;
            }
            
            
            wxprocess.CopyToHistoryAsks(lastAsk);//保存至历史
            wxprocess.CloseCurrAsk(lastAsk);//关闭当前等待的
            lastAsk.LastRequestWaitResponse.currPlan = currPlan;
            bool succ = lastAsk.LastRequestWaitResponse.ResponseAsk(lastAsk);//转入最后处理
            if(succ)
            {
                optPlan = lastAsk.LastRequestWaitResponse.currPlan;
            }
            return succ;
        }

        

        public override bool ResponseAsk(TheAskWaitingUserAnswer ask)
        {
            return false;
        }
    }


}

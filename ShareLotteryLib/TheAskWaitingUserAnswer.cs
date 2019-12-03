using System;
using System.Collections.Generic;
using System.Linq;
namespace WolfInv.com.ShareLotteryLib
{
    [Serializable]
    public class TheAskWaitingUserAnswer
    {
        public string roomName;
        public string userName;
        public string userNike;
        public string atName;
        public string askMsg;
        public Dictionary<int, string> ExpectAnswer;
        public bool Closed;
        public ResponseActionClass LastRequestWaitResponse;
        public MutliLevelData askData;
        /// <summary>
        /// 微信回复的答案
        /// </summary>
        public List<string> UserResponseAnswer;
        public KeyText AnswerResult;
        string _id;
        public string askId
        {
            get
            {
                if(_id==null)
                {
                    _id = Guid.NewGuid().ToString();
                }
                return _id;
            }
        }

        public TheAskWaitingUserAnswer()
        {

        }

        public string AskText
        {
            get
            {
                string asktxt = @"请按提示回复相应数字！
"+ string.Join(@";
", askData.SubList.Select(a =>
                {
                    string ret = string.Format("{0}:{1}", a.Key.key, a.Key.text);
                    return ret;
                }));
                return asktxt;
            }
        }

        public TheAskWaitingUserAnswer(ResponseActionClass rac,string askRoom=null,string askName=null,string askNikeName=null)
        {
            
            rac.lastAsk = this;
            this.LastRequestWaitResponse = rac;
            roomName = askRoom ?? rac.roomId;
            userName = askName ?? rac.requestUser;
            userNike = askNikeName ?? rac.requestNike;
        }

    }

    
}

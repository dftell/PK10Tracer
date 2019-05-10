using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WXMsgSvr
{
    [Serializable]
    public class WXContacter
    {
        public long Uin { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string HeadImgUrl { get; set; }
        public long ContactFlag { get; set; }
        public long MemberCount { get; set; }
        public WXContacter[] MemberList { get; set; }
        public string RemarkName { get; set; }
        public long HideInputBarFlag { get; set; }
        public long Sex { get; set; }
        public string Signature { get; set; }
        public long VerifyFlag { get; set; }
        public long OwnerUin { get; set; }
        public string PYInitial { get; set; }
        public string PYQuanPin { get; set; }
        public string RemarkPYInitial { get; set; }
        public string RemarkPYQuanPin { get; set; }
        public long StarFriend { get; set; }
        public long AppAccountFlag { get; set; }
        public long Statues { get; set; }
        public long AttrStatus { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Alias { get; set; }
        public long SnsFlag { get; set; }
        public long UniFriend { get; set; }
        public string DisplayName { get; set; }
        public long ChatRoomId { get; set; }
        public string KeyWord { get; set; }
        public string EncryChatRoomId { get; set; }
        public long IsOwner { get; set; }

    }

    [Serializable]
    public class WXUser
    {
        public long Uin { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string HeadImgUrl { get; set; }
        public string RemarkName { get; set; }
        public string PYInitial { get; set; }
        public string PYQuanPin { get; set; }
        public string RemarkPYInitial { get; set; }
        public string RemarkPYQuanPin { get; set; }
        public long HideInputBarFlag { get; set; }
        public long StarFriend { get; set; }
        public long Sex { get; set; }
        public string Signature { get; set; }
        public long AppAccountFlag { get; set; }
        public long VerifyFlag { get; set; }
        public long ContactFlag { get; set; }
        public long WebWxPluginSwitch { get; set; }
        public long HeadImgFlag { get; set; }
        public long SnsFlag { get; set; }


    }

    [Serializable]
    public class WXUserInfo
    {
        public BaseResponseClass BaseResponse{get;set;}
        public long Count { get; set; }

        public WXContacter[] ContactList { get; set; }
        public SyncKeys SyncKey { get; set; }
        public WXUser User { get; set; }
        public string ChatSet { get; set; }
        public string SKey { get; set; }
        public string ClientVersion { get; set; }
        public long SystemTime { get; set; }
        public long GrayScale { get; set; }
        public long InviteStartCount { get; set; }
        public long MPSubscribeMsgCount { get; set; }
        public MPSubscribeMsgListObject[] MPSubscribeMsgList { get; set; }
        public long ClickReportInterval { get; set; }
    }

    [Serializable]
    public class BaseResponseClass
    {
        public long Ret { get; set; }
        public string ErrMsg { get; set; }
    }

    [Serializable]
    public class SyncKeys:ToJsonClass
    {
        public long Count { get; set; }
        public SyncKeyClass[] List { get; set; }
        public new string ToString()
        {
            return string.Join("|", List.Select(a => string.Format("{0}_{1}", a.Key, a.Val)).ToArray());
        }
    }

    [Serializable]
    public class SyncKeyClass:ToJsonClass
    {
        public long Key { get; set; }
        public long Val { get; set; }

    }

    [Serializable]
    public class MPSubscribeMsgListObject
    {
        public string UserName { get; set; }
        public long MPArticleCount { get; set; }
        public MPArticleListClass[] MPArticleList { get; set; }
        public long Time { get; set; }
        public string NickName { get; set; }
        
    }

    [Serializable]
    public class MPArticleListClass
    {
        public string Title { get; set; }
        public string Digest { get; set; }
        public string Cover { get; set; }
        public string Url { get; set; }

    }
    public class ContactListOfUser
    {
        public BaseResponseClass BaseResponse { get; set; }
        public long MemberCount { get; set; }
        public WXContacter[] MemberList { get; set; }
        public long Seq { get; set; }

    }

    public class synccheckResult
    {
        public string retcode { get; set; }
        public string selector { get; set; }
        ////        window.synccheck={retcode:"0",selector:"2"}
        ////    retcode
        ////    SUCCESS("0", "成功"),
        ////TICKET_ERROR("-14", "ticket错误"),
        ////PARAM_ERROR("1", "传入参数错误"),
        ////NOT_LOGIN_WARN("1100", "未登录提示"),
        ////NOT_LOGIN_CHECK("1101", "未检测到登录"),
        ////COOKIE_INVALID_ERROR("1102", "cookie值无效"),
        ////LOGIN_ENV_ERROR("1203", "当前登录环境异常，为了安全起见请不要在web端进行登录"),
        ////TOO_OFEN("1205", "操作频繁");
        ////    selector
        ////    NORMAL("0", "正常"),
        ////NEW_MSG("2", "有新消息"),
        ////MOD_CONTACT("4", "有人修改了自己的昵称或你修改了别人的备注"),
        ////ADD_OR_DEL_CONTACT("6", "存在删除或者新增的好友信息"),
        ////ENTER_OR_LEAVE_CHAT("7", "进入或离开聊天界面");
        public bool Success(out string Msg)
        {
            Msg = null;
            if (retcode == "0")
                return true;
            switch(retcode)
            {
                case "-14":
                    {
                        Msg = "ticket错误";
                        break;
                    }
                case "1":
                    {
                        Msg = "传入参数错误";
                        break;
                    }
                case "1100":
                    {
                        Msg = "未登录提示";
                        break;
                    }
                case "1101":
                    {
                        Msg = "未检测到登录";
                        break;
                    }
                case "1102":
                    {
                        Msg = "cookie值无效";
                        break;
                    }
                case "1203":
                    {
                        Msg = "当前登录环境异常，为了安全起见请不要在web端进行登录";
                        break;
                    }
                case "1205":
                    {
                        Msg = "操作频繁";
                        break;
                    }
                default:
                    {
                        Msg = "未知错误";
                        break;
                    }


            }
            return false;

        }

        public SelectorStatus SelStatus()
        {
            return (SelectorStatus)int.Parse(selector);
        }
    }

    public enum SelectorStatus
    {
        Normal=0,
        New_Msg =2,
        Mod_Contact =4,
        Add_Or_Del_Contact=6,
        Enter_Or_Leave_Chat=7
    }

    public class WXMessage
    {
        public string MsgId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public long MsgType { get; set; }
        public string Content { get; set; }
        public long Status { get; set; }
        public long ImgStatus { get; set; }
        public long CreateTime { get; set; }
        public long VoiceLength { get; set; }
        public long PlayLength { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string MediaId { get; set; }
        public string Url { get; set; }
        public long AppMsgType { get; set; }
        public long StatusNotifyCode { get; set; }
        public long ForwardFlag { get; set; }
        public AppInfoClass AppInfo { get; set; }
        public long HasProductId { get; set; }
        public string Ticket { get; set; }
        public long ImgHeight { get; set; }
        public long ImgWidth { get; set; }
        public long SubMsgType { get; set; }
        public long NewMsgId { get; set; }
        public string OriContent { get; set; }
        public string EncryFileName { get; set; }


        public class AppInfoClass
        {
            public string AppID { get; set; }
            public int Type { get; set; }
        }
    }

    public class NewestMsgClass:ToJsonClass
    {
        public BaseResponseClass BaseResponse {get;set;}
        public long AddMsgCount { get; set; }
        public WXMessage[] AddMsgList { get; set; }
        public WXMessage[] ModContactList { get; set; }
        public WXContacter[] DelContactCount { get; set; }
        public long DelContactList { get; set; }
        public long ModChatRoomMemberCount { get; set; }
        public WXContacter[] ModChatRoomMemberList { get; set; }
        public long ContinueFlag { get; set; }
        public string SKey { get; set; }
        public SyncKeys SyncCheckKey { get; set; }

        

    }
    ////    {
    ////"BaseResponse": {
    ////"Ret": 1102,
    ////"ErrMsg": ""
    ////}
    ////,
    ////"MemberCount": 0,
    ////"MemberList": [],
    ////"Seq": 0
    ////}
}

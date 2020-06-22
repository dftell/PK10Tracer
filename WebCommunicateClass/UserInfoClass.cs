using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.RemoteObjectsLib;
//using WolfInv.com.WebRuleLib;

namespace WolfInv.com.WebCommunicateClass
{
    public class UserInfoClass:RecordObject
    {
        public UserBaseInfo BaseInfo;
        public Role UserInfo;
        public UserInfoClass()
        {
            BaseInfo = new UserBaseInfo(); 
        }

        public static UserInfoClass GetUserInfo(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            UserBaseInfo bi = null;
            try
            {
                bi = serializer.Deserialize<UserBaseInfo>(json);  //序列化：对象=>JSON字符串
            }
            catch (Exception ce)
            {
                return null;
            }
            ////Response.Write(jsonStr);
            ////if (bi == null)
            ////{
            ////    return null;
            ////}

            UserInfoClass ret = new UserInfoClass();
            ret.BaseInfo = bi;
            ////ret.BaseInfo = new UserBaseInfo();
            ////ret.BaseInfo.UserCode = bi["UserCode"].ToString();
            ////ret.BaseInfo.Password = bi["Password"].ToString();
            ////DateTime.TryParse(bi["ExpireDate"].ToString(), out ret.BaseInfo.ExpireDate);
            ////ret.BaseInfo.Odds = double.Parse(bi["Odds"].ToString());
            return ret;
        }

        
    }

    public class UserBaseInfo : RecordObject
    {
        public int UserId;
        public string UserCode;
        public string Password;
        public int UserType;
        public DateTime ExpireDate;
        public double Odds;
        public UserBaseInfo ParentInfo;
        public int GroupId;
        public int IsAdmin;//是否是管理员
        public string WXToUser;
        public string AssetConfig;
        public string AliasName;
        public string WebLoginId;
        public string WebLoginPwd;
        public int AppMode;//0:自由模式，可根据自有配置投注，1:App模式，该模式下没有修改配置的权限，只能跟随主账户按对应比例投注
        public int RiskTimes;//风险倍数
        
    }
}

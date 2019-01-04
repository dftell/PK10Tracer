using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using BaseObjectsLib;
namespace WebCommunicateClass
{
    public class CommunicateToServer
    {
        public CommResult Login(string loginUrl,string username,string pwd)
        {
            CommResult ret = new CommResult();
            string url = string.Format(loginUrl,username,pwd);
            string json = AccessWebServerClass.GetData(url, Encoding.Default);
            UserInfoClass user = UserInfoClass.GetUserInfo(json);
            if (user == null)
            {
                ret.Message = json;
                return ret;
            }
            ret.Succ = true;
            ret.Message = null;
            ret.Json = "";
            ret.Result = new List<RecordObject>();
            ret.Result.Add(user);
            return ret;
        }

        public CommResult getRequestInsts(string requestUrl)
        {
            string json = null;
            CommResult ret = new CommResult();
            try
            {
                json = AccessWebServerClass.GetData(requestUrl, Encoding.Default);
                if (json == null)
                {
                    ret.Message = "未请求到内容！";
                    return ret;
                }
            }
            catch(Exception ce)
            {
                ret.Message = ce.Message;
                return ret;
            }
            RequestClass rc = null;
            try
            {
                rc = new RequestClass().getObjectByJsonString(json);
            }
            catch(Exception e)
            {
                ret.Message = e.Message;
                return ret;
            }
            ret.Json = json;
            if (rc == null)
            {
                ret.Message = "数据格式错误！";
                return ret;
            }
            ret.Succ = true;
            ret.Result = new List<RecordObject>();
            ret.Cnt = 1;
            ret.Result.Add(rc);
            return ret;
        }
    }
}

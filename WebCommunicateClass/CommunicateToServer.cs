using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WolfInv.com.BaseObjectsLib;
using WolfInv.com.RemoteObjectsLib;

namespace WolfInv.com.WebCommunicateClass
{
    public class CommunicateToServer
    {
        public CommResult Login(string loginUrl,string username,string pwd,string webcode)
        {
            CommResult ret = new CommResult();
            string url = string.Format(loginUrl,username,pwd,webcode);
            string json = AccessWebServerClass.GetData(url, Encoding.UTF8);
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

        public CommResult getRequestInsts<T>(string requestUrl)
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
            RecordObject rc = Activator.CreateInstance(typeof(T)) as RecordObject; 
            try
            {
                rc = (rc as iSerialJsonClass<T>).getObjectByJsonString(json) as RecordObject;
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

        

        public CommResult getRequestAssetList(string requestUrl)
        {
            string json = null;
            CommResult ret = new CommResult();
            try
            {
                json = AccessWebServerClass.GetData(requestUrl, Encoding.UTF8);
                if (json == null)
                {
                    ret.Message = "未请求到内容！";
                    return ret;
                }
            }
            catch (Exception ce)
            {
                ret.Message = ce.Message;
                return ret;
            }
            mAssetUnitList rc = null;
            try
            {
                rc = new AssetUnitList().getObjectByJsonString(json);
            }
            catch (Exception e)
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

        public CommResult SendStatusInfo(string requestUrl)
        {
            CommResult ret = new CommResult();
            string url = requestUrl;
            string json = AccessWebServerClass.GetData(url, Encoding.Default);
            try
            {
                ret.Succ = true;
                ret.Message = null;
                ret.Json = json;
                ret.Result = new List<RecordObject>();
            }
            catch(Exception ce)
            {
                ret.Message = ce.StackTrace;
                ret.Json = ce.Message;
                ret.Result = null;
            }
            return ret;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Text;
using WolfInv.Com.AccessWebLib;
using WolfInv.Com.JsLib;

namespace WolfInv.com.JdUnionLib
{
    public interface iReturnMsg
    {
        string code { get; set; }
        string msg { get; set; }
    }
    public class JdUnion_ReturnClass : iReturnMsg
    {
        public string code { get; set; }
        public string msg { get; set; }
    }

    public class JdUnion_ReturnItemsClass : JdUnion_ReturnClass
    {
        public string totalsize{get;set;} 
    }
    

    

    public abstract class JdUnion_RequestClass: JdUnion_ModuleProcessClass,iReturnMsg
    {

        
        public string code { get; set; }
        public string msg { get; set; }
        public AccessTokenClass data;
        
        
        public JdUnion_RequestClass()
        {
            awsobj = new AccessWebServerClass();
        }

        public int errcode { get; set; }
        public string description { get; set; }

        

        protected AccessWebServerClass awsobj = null;
        protected string ReqJson;
        protected string ReqAccessToken;
        protected string ReqBaseUrl;
        public string Req_PostData { get; set; }
        protected string ReqUrl
        {
            get
            {
                return string.Format("{0}", ReqBaseUrl);
            }
        }
        public bool InitRequestJson()
        {
            //https://open.jdy.com/doc/api/14/169
            strJsonName = Module.ModuleName;
            ReqBaseUrl = Module.AccessUrl;
            string strJsonModel = this.getJsonContent(strJsonName);
            if (strJsonModel == null)
            {
                return false;
            }
            ReqJson = strJsonModel.Replace("{0}", username).Replace("{1}", password).Replace("{2}", client_id).Replace("{3}", client_secret);
            return true;
        }

        public string client_id { get; set; }
        public string client_secret { get; set; }
        
        public string username { get; set; }
        public string password { get; set; }
        

        

        //public abstract bool InitRequestJson();

        public string PostRequest()
        {
            if (ReqJson == null)
            {
                if(!InitRequestJson())
                {
                    //return null;
                }
            }
            string url = string.Format("{0}{2}{1}",ReqUrl,ReqJson, (ReqUrl.EndsWith("?") ? "" : "?")); 
            return AccessWebServerClass.PostData(url, this.Req_PostData, Encoding.UTF8);
        }

        public string GetRequest()
        {
            if (ReqJson == null)
            {
                if (!this.InitRequestJson())
                {
                    //return null;
                }
            }
            string url = ReqUrl+ (ReqUrl.EndsWith("?")?"":"?") + ReqJson;
            return AccessWebServerClass.GetData(url, Encoding.UTF8);
        }

        
    }


    public class JdUnion_SystemClass: JdUnion_ModuleProcessClass
    {
        public JdUnion_SystemClass()
        {
            this.strJsonName = "System.config.modules";

        }


    }

    public class JdUnion_Modules: JsonableClass<JdUnion_Modules>
    {
        public List<JdUnion_ModuleClass> Modules { get; set; }
        public JdUnion_Modules()
        {
            Modules = new List<JdUnion_ModuleClass>();
        }
    }


}

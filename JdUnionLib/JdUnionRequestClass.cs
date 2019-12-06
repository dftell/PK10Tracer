using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WolfInv.Com.AccessWebLib;
using WolfInv.Com.JsLib;

namespace WolfInv.com.JdUnionLib
{
    public abstract class JdyModuleProcessClass : JdyJsonClass
    {
        public JDY_ModuleClass Module { get; set; }

        public bool InitClass(JDY_ModuleClass module)
        {
            if (module == null)
                return false;
            Module = module;
            return true;
            string json = this.getJsonContent(this.strJsonName);
            if (json == null)
                return false;
            Module = module;
            return true;
        }
    }
    public interface iReturnMsg
    {
        string code { get; set; }
        string msg { get; set; }
    }
    public class JdyReturnClass : iReturnMsg
    {
        public string code { get; set; }
        public string msg { get; set; }
    }

    public class JdyReturnItemsClass : JdyReturnClass
    {
        public string totalsize{get;set;} 
    }
    


    public abstract class JdyRequestClass: JdyModuleProcessClass,iReturnMsg
    {
        public string code { get; set; }
        public string msg { get; set; }
        public AccessTokenClass data;
        public string access_token { get; set; }
        public JdyRequestClass()
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
            string url = string.Format("{0}?{1}",ReqUrl,ReqJson); 
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
            string url = ReqUrl+"?" + ReqJson;
            return AccessWebServerClass.GetData(url, Encoding.UTF8);
        }
    }

    public abstract class JdyJsonClass:JsonableClass<JdyJsonClass>
    {
        public string strJsonName;
        
        public string getJsonPath(string name)
        {
            string strPath = string.Format("{0}Json\\{1}.json", AppDomain.CurrentDomain.BaseDirectory, name.Replace("/", "\\"));
            if (File.Exists(strPath))
            {
                return strPath;
            }
            return null;
        }

        public string getFilePath(string name,string folder="",string type="")
        {
            if (name == null)
                return null;
            string strPath = string.Format("{0}{2}\\{1}{3}", AppDomain.CurrentDomain.BaseDirectory, name.Replace("/","\\"),folder,type);
            if (File.Exists(strPath))
            {
                return strPath;
            }
            return null;
        }

        public string getJsonContent(string name)
        {
            string strPath = getJsonPath(name);
            if (strPath == null)
                return null;
            using (TextReader tr = File.OpenText(strPath))
            {
                return tr.ReadToEnd();
            }
            //return null;
        }

        public bool saveJsonContent(string path,string str)
        {
            try
            {
                StringWriter tw = new StringWriter();
                tw.Write(str);
            }
            catch(Exception ce)
            {

            }
            return true;
        }
    }

    public class JdySystemClass: JdyModuleProcessClass
    {
        public JdySystemClass()
        {
            this.strJsonName = "System.config.modules";

        }

        

        


    }

    public class JDY_Modules: JsonableClass<JDY_Modules>
    {
        public List<JDY_ModuleClass> Modules { get; set; }
        public JDY_Modules()
        {
            Modules = new List<JDY_ModuleClass>();
        }
    }
}

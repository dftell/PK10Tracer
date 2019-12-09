using System;
using System.Collections.Generic;
using System.IO;
using WolfInv.Com.JsLib;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace WolfInv.com.JdUnionLib
{
    public abstract class JdUnion_JsonClass : JsonableClass<JdUnion_JsonClass>
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

        public string getFilePath(string name, string folder = "", string type = "")
        {
            if (name == null)
                return null;
            string strPath = string.Format("{0}{2}\\{1}{3}", AppDomain.CurrentDomain.BaseDirectory, name.Replace("/", "\\"), folder, type);
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

        public bool saveJsonContent(string path, string str)
        {
            try
            {
                StringWriter tw = new StringWriter();
                tw.Write(str);
            }
            catch (Exception ce)
            {

            }
            return true;
        }
    }

    public abstract class JdUnion_BaseClass : JdUnion_JsonClass
    {
        public string method { get { return strJsonName; } }
        public string app_key { get; set; }
        public string app_secret { get; set; }
        public string access_token { get; set; }

        string _format = "json";
        public string format
        {
            get { return _format; }
            set { _format = value; }
        }
        string _v = "1.0";
        public string v
        {
            get { return _v; }
            set { _v = value; }
        }
        string _sign_method = "md5";
        public string sign_method
        {
            get { return _sign_method; }
            set { _sign_method = value; }
        }
        string _sign;
        public string sign
        {
            get
            {
                if (_sign == null)
                    _sign = getSign();
                return _sign;
            }
            set
            {
                _sign = value;
            }
        }
        string _timestamp;
        public string timestamp
        {
            get
            {
                if(_timestamp == null)
                    _timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                return _timestamp;
            }
            set
            {
                _timestamp = value;
            }
        }

        /// <summary>
        /// MD5字符串加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns>加密后字符串</returns>
        public static string GenerateMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }


        string getSign()
        {
            return GenerateMD5(SignRequestString).ToUpper();
        }

        
        public Dictionary<string, object> params_System(bool includeSign = false)
        {
            Dictionary<string, object> t_sysParams = new Dictionary<string, object>();
            t_sysParams.Add("method", this.method);
            t_sysParams.Add("sign_method", this.sign_method);
            t_sysParams.Add("format", this.format);
            if(!string.IsNullOrEmpty(this.access_token)|| includeSign)
            {
                t_sysParams.Add("access_token", this.access_token);
            }
            t_sysParams.Add("timestamp", this.timestamp);
            t_sysParams.Add("app_key", this.app_key);
            t_sysParams.Add("v", this.v);
            t_sysParams.Add("param_json", params_360buy);
            if (includeSign)
            {
                t_sysParams.Add("sign", this.sign);
                
            }
            return t_sysParams;
        }

        public string SystemString(Dictionary<string, object> Params, bool toSign = false)
        {
            
                List<string> arr = Params.Keys.ToList();
                List<string> sortArr = arr.OrderBy(a => a).ToList();
                List<string> ret = new List<string>();
                sortArr.ForEach(a => {
                    if (a != "param_json")
                        ret.Add(string.Format("{0}{2}{1}", a, Params[a], toSign ? "" : "="));
                    else
                    {
                        Dictionary<string, object> param = Params[a] as Dictionary<string, object>;
                        ret.Add(string.Format("{0}{2}{1}", a, Buy360String( param ), toSign ? "" : "="));
                    }
                });
                
                return string.Join(toSign?"":"&",ret);
            
        }



        public string Buy360String(Dictionary<string,object> Params)
        {
            
            List<string> arr = Params.Keys.ToList();
            List<string> sortArr = arr.OrderBy(a => a).ToList();
            List<string> list = new List<string>();
            sortArr.ForEach(a => {
                if (Params[a] == null)
                {
                    list.Add(string.Format("\"{0}\":null", a));
                }
                else
                {
                    if (Params[a] is object)
                    {
                        if(Params[a].ToString().Trim().StartsWith("{") && Params[a].ToString().Trim().EndsWith("}"))
                        {
                            list.Add(string.Format("\"{0}\":{1}", a, Params[a].ToString().Trim()));
                        }
                        else
                            list.Add(string.Format("\"{0}\":\"{1}\"", a, Params[a].ToString().Trim()));
                    }
                    else
                    {
                        list.Add(string.Format("\"{0}\":{1}", a, Params[a].ToString().Trim()));
                    }
                }
            });
            return "{{0}}".Replace("{0}",string.Join(",",list));
        }

        public string SignRequestString
        {
            get
            {
                return string.Format("{0}{1}{0}", this.app_secret, SystemString(params_System(false),true));
            }
        }

        public string FullRequestString
        {
            get
            {
                return SystemString(params_System(true));
            }
        }

        public Dictionary<string, object> params_360buy = new Dictionary<string, object>();
    }

    public abstract class JdUnion_ModuleProcessClass : JdUnion_BaseClass
    {
        public JdUnion_ModuleClass Module { get; set; }

        public bool InitClass(JdUnion_ModuleClass module)
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


    
}

using System;
using System.Collections.Generic;
using System.IO;
using WolfInv.Com.JsLib;

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

    public abstract class JdUnion_ModuleProcessClass : JdUnion_BaseClass
    {
        public JdUnion_ModuleClass Module { get; set; }

        public bool InitClass(JdUnion_ModuleClass module)
        {
            if (module == null)
                return false;
            Module = module;
            if(this.params_360buy.Count == 0)
                LoadParams();
            return true;
            string json = this.getJsonContent(this.strJsonName);
            if (json == null)
                return false;
            Module = module;
            return true;
        }

        void LoadParams()
        {
            JdUnion_ParamModel jp = new JdUnion_ParamModel();
            if (string.IsNullOrEmpty(Module.RequestModel))
                return;
            string json = new JdUnion_GlbObject().getJsonContent(Module.RequestModel);
            if (json == null)
                return;
            JdUnion_ParamModel ret = jp.GetFromJson<JdUnion_ParamModel>(json);
            if(ret == null)
            {
                throw new Exception(string.Format("加载{0}模板错误！",jp.name));
            }
            this.params_360buy.Clear();
            ret.Params.ForEach(a => {
                string txt = a.ToJson();
                this.params_360buy.Add(a.name,txt);
            });
        }
        
    }

    public class JdUnion_ParamModel:JsonableClass<JdUnion_ParamModel>
    {
        public string name { get; set; }
        public int isObject { get; set; }
        public int allowNull { get; set; }
        public string value { get; set; }
        public string defaultValue { get; set; }
        public List<JdUnion_ParamModel> Params { get; set; }

        public string ToJson()
        {
            string ret = null;
            if(!string.IsNullOrEmpty(defaultValue))
            {
                value = defaultValue;
            }
            if (isObject == 1)
            {
                List<string> retList = new List<string>();
                Params.ForEach(a => {
                    if (a.allowNull == 0)
                       retList.Add(a.ToJson());
                });
                ret = "{{1}}".Replace("{1}",string.Join(",", retList));
            }
            else
            {
                
                ret = string.Format("\"{0}\":\"{1}\"", name, value);
            }
            return ret;
        }
    }
    
}

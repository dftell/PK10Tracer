using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.Com.JsLib;
using System.Reflection;
using System.Data;
using System.IO;


namespace WolfInv.com.JdUnionLib
{

    
    public class JdUnion_GlbObject:JdUnion_BaseClass
    {
        static long leftSecs;
        static DateTime lastLoginTime;
        static string t_access_token;
        static string t_dbId = null;
        public static Dictionary<string, JdUnion_ModuleClass> mlist = new Dictionary<string, JdUnion_ModuleClass>();

        public static string Access_token
        {
            get
            {
                return "";
            }
        }




        public static string getJsonText(string filepath)
        {
            string file = new JdUnion_SystemClass().getJsonPath(filepath);
            if(!File.Exists(file))
            {
                return null;
            }
            try
            {
                return File.ReadAllText(file);
            }
            catch
            {
                return null;
            }

        }
        public static string getText(string filepath,string folder="",string type="")
        {
            string file = new JdUnion_SystemClass().getFilePath(filepath,folder,type);
            if (!File.Exists(file))
            {
                return null;
            }
            try
            {
                return File.ReadAllText(file);
            }
            catch
            {
                return null;
            }

        }
        
        /*
        /// <summary>
        /// 访问令牌
        /// </summary>
        public static string Access_token {
            get
            {
                long pastSecs = (long)DateTime.Now.Subtract(lastLoginTime).TotalSeconds;
                if (pastSecs > leftSecs || t_access_token == null)//重新访问
                {
                    getAccessToken();
                }
                return t_access_token;

            }
        }
        
         
        public static void ResetAccess()
        {
            t_access_token = null;
        }


        public static string getAccessToken()
        {
            if (AccessObj == null)
            {
                AccessObj = new AccessTokenClass();
            }
            if(AccessObj.Module == null)
            {
                AccessObj.InitClass(JdUnion_GlbObject.mlist[AccessObj.GetType().Name]);
                
            }
            AccessObj.InitRequestJson();
            DateTime now = DateTime.Now;
            //AccessObj.InitRequestJson();
            AccessObj = new JsonableClass<string>().GetFromJson<AccessTokenClass>(AccessObj.GetRequest());
            if (AccessObj == null)
                return null;
            if (AccessObj.errcode == 0)
            {
                leftSecs = AccessObj.data.expires_in;
                lastLoginTime = now;
                t_access_token = AccessObj.data.access_token;
                return AccessObj.data.access_token;
            }
            return t_access_token;
        }
        */
       
        public static JdUnion_Modules modules;
        public static Dictionary<string, Type> AllModuleClass = new Dictionary<string, Type>(); 

        static JdUnion_GlbObject()
        {
            Type t = typeof(JdUnion_ModuleProcessClass);
            List<Type> subList = getAllSubClass(t);
            modules = new JdUnion_Modules();
            subList.ForEach(a => {
                JdUnion_ModuleClass mdl = new JdUnion_ModuleClass();
                mdl.ClassName = a.Name;
                mdl.AccessUrl = "";
                mdl.ModuleName = a.Name;
                mdl.RequestModel = "";
                modules.Modules.Add(mdl);
            });
            string json = modules.ToJson();
            string path = new JdUnion_SystemClass().getJsonPath("system.config.modules");
            
            if (!File.Exists(path))
            {
                SaveFile(path, json);
            }
            else
            {
                
                string strJson = File.OpenText(path).ReadToEnd();
                modules = modules.GetFromJson<JdUnion_Modules>(strJson);
            }
            modules.Modules.ForEach(a => mlist.Add(a.ClassName, a));
            subList.ForEach(a => {
                if (mlist.ContainsKey(a.Name))
                {
                    AllModuleClass.Add(a.Name, a);
                    JdUnion_ModuleProcessClass obj = Activator.CreateInstance(a) as JdUnion_ModuleProcessClass;
                    if (obj != null)
                        obj.InitClass(mlist[a.Name]);
                    //if(obj is AccessTokenClass)
                    //{
                    //    AccessObj = obj as AccessTokenClass;
                    //    AccessObj.InitRequestJson();
                    //}
                    if (obj is JdUnion_RequestClass)
                    {
                        (obj as JdUnion_RequestClass).InitRequestJson();
                    }

                }
            });
            //string strtest = Access_token;
        }
            
        static List<Type> getAllSubClass(Type pt)
        {
            List<Type> ret = new List<Type>();

            Type[] AllType = pt.Assembly.GetTypes();
            var subtypes = AllType.Where(a=>
            {
                if (a.BaseType == null)
                    return false;
                Type t = a;
                while (t.BaseType != null)
                {
                    if (t.BaseType.Equals(pt) && !a.IsAbstract)
                    {
                        return true;
                    }
                    t = t.BaseType;
                }
                return false;
            });
            foreach(var val in subtypes)
            {
              
                ret.Add(val);
            }
            return ret;
        }

        static bool SaveFile(string filename, string strContent)
        {
            
            string strJsonPath = filename;
            try
            {
                StreamWriter sw = new StreamWriter(strJsonPath, false);
                sw.Write(strContent);
                sw.Close();
                //ToLog("保存策略", "成功");
            }
            catch (Exception c)
            {
                //ToLog("错误", string.Format("保存到文件[{0}]错误:{1}", filename, strContent), string.Format("{0}", c.Message));
                return false;
            }
            return true;
        }
    }

    public class JdUnion_ModuleClass : JsonableClass<JdUnion_ModuleClass>
    {
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string AccessUrl { get; set; }
        public string RequestModel { get; set; }

        public bool RequestMethodUseGET { get; set; }
        public string RequestSchema { get; set; }


    }
}

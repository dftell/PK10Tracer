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
    public class jdy_GlbObject
    {
        static long leftSecs;
        static DateTime lastLoginTime;
        static string t_access_token;
        static string t_dbId = null;
        public static Dictionary<string, JDY_ModuleClass> mlist = new Dictionary<string, JDY_ModuleClass>();



        static AccessTokenClass AccessObj { get; set; }
        static JDYSCM_Service_List_Class SvcObj { get; set; }
        static JDYSCM_Product_Unit_List_Class UnitObj { get; set; }
        static JDYSCM_WareHouse_List_Class WhouseObj { get; set; }
        static JDYSCM_Supplier_List_Class SuppObj { get; set; }

        static JDYSCM_Customer_List_Class CtmObj { get; set; }

        static JDYSCM_Product_List_Class ProdObj { get; set; }

        static Dictionary<string, string> t_WareHouses;
        public static Dictionary<string, string> AllWareHouses
        {
            get
            {
                if(t_WareHouses == null)
                    t_WareHouses = getWareHouses();
                return t_WareHouses;
            }
        }

        static Dictionary<string,string> t_Supplier;
        public Dictionary<string, string> AllSuppliers
        {
            get
            {
                t_Supplier = getSuppliers();
                return t_Supplier;
            }
        }

        static Dictionary<string, string> t_Customers;
        public static Dictionary<string, string> AllCustomers
        {
            get
            {
                if(t_Customers == null)
                    t_Customers = getCustomers();
                return t_Customers;
            }
        }

        private static Dictionary<string, string> getCustomers()
        {
            if (CtmObj == null)
            {
                CtmObj = new JDYSCM_Customer_List_Class();
            }
            if (CtmObj.Module == null)
            {
                CtmObj.InitClass(jdy_GlbObject.mlist[CtmObj.GetType().Name]);

            }
            CtmObj.InitRequestJson();
            JDYSCM_Bussiness_List_Class.JDYSCM_Bussiness_Filter_Class flt = new JDYSCM_Bussiness_List_Class.JDYSCM_Bussiness_Filter_Class();
            flt.pageSize = 500;
            flt.page = 1;
            CtmObj.Req_PostData = "{\"filter\":{0}}".Replace("{0}",flt.ToJson());
            //AccessObj.InitRequestJson();
            CtmObj = new JsonableClass<string>().GetFromJson<JDYSCM_Customer_List_Class>(CtmObj.PostRequest());
            if (CtmObj == null)
                return null;
            if (CtmObj.code == "0")
            {
                t_Customers = new Dictionary<string, string>();
                CtmObj.items.ForEach(a =>
                {
                    if (!t_Customers.ContainsKey(a.name))
                        t_Customers.Add(a.name, a.number);
                });
            }
            return t_Customers;
        }

        static Dictionary<string, string> t_UnitList;
        /// <summary>
        /// 所有单位
        /// </summary>
        public static Dictionary<string,string> UnitList
        {
            get
            {
                if(t_UnitList == null)
                    getUnitList();
                return t_UnitList;
            }
        }

        public static string getJsonText(string filepath)
        {
            string file = new JdySystemClass().getJsonPath(filepath);
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
            string file = new JdySystemClass().getFilePath(filepath,folder,type);
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
        static Dictionary<string, string> t_ProductList;
        /// <summary>
        /// 所有单位
        /// </summary>
        public static Dictionary<string, string> ProductList
        {
            get
            {
                if (t_ProductList == null)
                    t_ProductList = getProducts();
                return t_ProductList;
            }
        }

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

        public static string dbId
        {
            get
            {
                long pastSecs = (long)DateTime.Now.Subtract(lastLoginTime).TotalSeconds;
                if (pastSecs > leftSecs || t_dbId == null)//重新访问
                {
                    getDbId();
                }
                return t_dbId;
            }
        }  
        public static void ResetAccess()
        {
            t_access_token = null;
        }

        /// <summary>
        /// 获取所有仓库
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string,string> getWareHouses()
        {
            if (WhouseObj == null)
            {
                WhouseObj = new JDYSCM_WareHouse_List_Class();
            }
            if (WhouseObj.Module == null)
            {
                WhouseObj.InitClass(jdy_GlbObject.mlist[WhouseObj.GetType().Name]);

            }
            WhouseObj.InitRequestJson();
            //AccessObj.InitRequestJson();
            WhouseObj = new JsonableClass<string>().GetFromJson<JDYSCM_WareHouse_List_Class>(WhouseObj.GetRequest());
            if (WhouseObj == null)
                return null;
            if (WhouseObj.errcode == 0)
            {
                t_WareHouses = new Dictionary<string, string>();
                WhouseObj.items.ForEach(a =>
                {
                    if (!t_WareHouses.ContainsKey(a.name))
                        t_WareHouses.Add(a.name, a.number);
                });
            }
            return t_WareHouses;
        }

        /// <summary>
        /// 获取所有供应商
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> getSuppliers()
        {
            if (SuppObj == null)
            {
                SuppObj = new JDYSCM_Supplier_List_Class();
            }
            if (SuppObj.Module == null)
            {
                SuppObj.InitClass(jdy_GlbObject.mlist[SuppObj.GetType().Name]);

            }
            SuppObj.InitRequestJson();
            //AccessObj.InitRequestJson();
            SuppObj = new JsonableClass<string>().GetFromJson<JDYSCM_Supplier_List_Class>(SuppObj.PostRequest());
            if (SuppObj == null)
                return null;
            if (SuppObj.errcode == 0)
            {
                t_Supplier = new Dictionary<string, string>();
                SuppObj.items.ForEach(a =>
                {
                    if (!t_Supplier.ContainsKey(a.name))
                        t_Supplier.Add(a.name, a.number);
                });
            }
            return t_Supplier;
        }

        /// <summary>
        /// 获取所有商品
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> getProducts()
        {
            if (ProdObj == null)
            {
                ProdObj = new JDYSCM_Product_List_Class();
            }
            if (ProdObj.Module == null)
            {
                ProdObj.InitClass(jdy_GlbObject.mlist[ProdObj.GetType().Name]);

            }
            ProdObj.InitRequestJson();
            //AccessObj.InitRequestJson();
            ProdObj = new JsonableClass<string>().GetFromJson<JDYSCM_Product_List_Class>(ProdObj.PostRequest());
            if (ProdObj == null)
                return null;
            if (ProdObj.errcode == 0)
            {
                ProdObj.InitClass(jdy_GlbObject.mlist[ProdObj.GetType().Name]);
                int allcnt = ProdObj.records;
                JDYSCM_Bussiness_List_Class.JDYSCM_Bussiness_Filter_Class filter = new JDYSCM_Bussiness_List_Class.JDYSCM_Bussiness_Filter_Class();
                filter.pageSize = allcnt *2;
                filter.page = 1;
                ProdObj.Req_PostData = "{\"filter\":{0}}".Replace("{0}", filter.ToJson());
                
                ProdObj = new JsonableClass<string>().GetFromJson<JDYSCM_Product_List_Class>(ProdObj.PostRequest());
                if (ProdObj == null)
                    return null;
                if(ProdObj.errcode == 0)
                {
                    t_ProductList = new Dictionary<string, string>();
                    ProdObj.items.ForEach(a =>
                    {
                        if (!t_ProductList.ContainsKey(a.productNumber))
                            t_ProductList.Add(a.productNumber, a.productName);
                    });
                }
                if(ProdObj.totalPages>1)
                {
                    int pagesize = ProdObj.totalsize;
                    JDYSCM_Bussiness_List_Class.JDYSCM_Bussiness_Filter_Class flt = new JDYSCM_Bussiness_List_Class.JDYSCM_Bussiness_Filter_Class();
                    flt.pageSize = pagesize;
                    for (int i=2;i<=ProdObj.totalPages;i++)
                    {
                        flt.page = i;
                        ProdObj.InitClass(jdy_GlbObject.mlist[ProdObj.GetType().Name]);
                        ProdObj.Req_PostData = "{\"filter\":{0}}".Replace("{0}", flt.ToJson());
                        ProdObj = new JsonableClass<string>().GetFromJson<JDYSCM_Product_List_Class>(ProdObj.PostRequest());
                        if (ProdObj == null)
                            return null;
                        if (ProdObj.errcode == 0)
                        {
                            //t_ProductList = new Dictionary<string, string>();
                            ProdObj.items.ForEach(a =>
                            {
                                if (!t_ProductList.ContainsKey(a.productNumber))
                                    t_ProductList.Add(a.productNumber, a.productName);
                            });
                        }
                    }
                }
                
            }
            return t_ProductList;
        }

        public static string getAccessToken()
        {
            if (AccessObj == null)
            {
                AccessObj = new AccessTokenClass();
            }
            if(AccessObj.Module == null)
            {
                AccessObj.InitClass(jdy_GlbObject.mlist[AccessObj.GetType().Name]);
                
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

        public static Dictionary<string,string> getUnitList()
        {
            if (UnitObj == null)
            {
                UnitObj = new JDYSCM_Product_Unit_List_Class();
            }
            if (UnitObj.Module == null)
            {
                UnitObj.InitClass(jdy_GlbObject.mlist[UnitObj.GetType().Name]);

            }
            UnitObj.InitRequestJson();
            //AccessObj.InitRequestJson();
            UnitObj = new JsonableClass<string>().GetFromJson<JDYSCM_Product_Unit_List_Class>(UnitObj.PostRequest());
            if (UnitObj == null)
                return null;
            if (UnitObj.errcode == 0)
            {
                t_UnitList = new Dictionary<string, string>();
                UnitObj.items.ForEach(a =>
                {
                    if(!t_UnitList.ContainsKey(a.name))
                        t_UnitList.Add( a.name,a.ID);
                });
            }
            return t_UnitList ;
        }

        public static string getDbId()
        {
            if(SvcObj == null)
            {
                SvcObj = new JDYSCM_Service_List_Class();
            }
            if(SvcObj.Module == null)
            {
                SvcObj.InitClass(jdy_GlbObject.mlist[SvcObj.GetType().Name]);
            }
            SvcObj.InitRequestJson();
            SvcObj = new JsonableClass<string>().GetFromJson<JDYSCM_Service_List_Class>(SvcObj.GetRequest());
            if(SvcObj != null)
            {
                if (SvcObj.items == null || SvcObj.items.Count == 0)
                    return null;
                t_dbId= SvcObj.items[0].dbId;
                return t_dbId;
            }
            return t_dbId;
        }
        public static JDY_Modules modules;
        public static Dictionary<string, Type> AllModuleClass = new Dictionary<string, Type>(); 

        static jdy_GlbObject()
        {
            Type t = typeof(JdyModuleProcessClass);
            List<Type> subList = getAllSubClass(t);
            modules = new JDY_Modules();
            subList.ForEach(a => {
                JDY_ModuleClass mdl = new JDY_ModuleClass();
                mdl.ClassName = a.Name;
                mdl.AccessUrl = "";
                mdl.ModuleName = a.Name;
                mdl.RequestModel = "";
                modules.Modules.Add(mdl);
            });
            string json = modules.ToJson();
            string path = new JdySystemClass().getJsonPath("system.config.modules");
            
            if (!File.Exists(path))
            {
                SaveFile(path, json);
            }
            else
            {
                
                string strJson = File.OpenText(path).ReadToEnd();
                modules = modules.GetFromJson<JDY_Modules>(strJson);
            }
            modules.Modules.ForEach(a => mlist.Add(a.ClassName, a));
            subList.ForEach(a => {
                if (mlist.ContainsKey(a.Name))
                {
                    AllModuleClass.Add(a.Name, a);
                    JdyModuleProcessClass obj = Activator.CreateInstance(a) as JdyModuleProcessClass;
                    if (obj != null)
                        obj.InitClass(mlist[a.Name]);
                    if(obj is AccessTokenClass)
                    {
                        AccessObj = obj as AccessTokenClass;
                        AccessObj.InitRequestJson();
                    }
                    if(obj is JDYSCM_Service_List_Class)
                    {
                        SvcObj = obj as JDYSCM_Service_List_Class;
                        SvcObj.InitRequestJson();
                    }
                    if(obj is JDYSCM_Product_Unit_List_Class)
                    {
                        UnitObj = obj as JDYSCM_Product_Unit_List_Class;
                        UnitObj.InitRequestJson();
                    }
                    if(obj is JDYSCM_Supplier_List_Class)
                    {
                        SuppObj = obj as JDYSCM_Supplier_List_Class;
                        SuppObj.InitRequestJson();
                    }
                    if (obj is JDYSCM_WareHouse_List_Class)
                    {
                        WhouseObj = obj as JDYSCM_WareHouse_List_Class;
                        WhouseObj.InitRequestJson();
                    }
                    if(obj is JDYSCM_Customer_List_Class)
                    {
                        CtmObj = obj as JDYSCM_Customer_List_Class;
                        CtmObj.InitRequestJson();
                    }
                    if(obj is JDYSCM_Product_List_Class)
                    {
                        ProdObj = obj as JDYSCM_Product_List_Class;
                        ProdObj.InitRequestJson();
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

    public class JDY_ModuleClass : JsonableClass<JDY_ModuleClass>
    {
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string AccessUrl { get; set; }
        public string RequestModel { get; set; }

        public bool RequestMethodUseGET { get; set; }
        public string RequestSchema { get; set; }


    }
}

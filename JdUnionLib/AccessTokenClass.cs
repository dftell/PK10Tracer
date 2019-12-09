using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WolfInv.com.JdUnionLib
{
    public class Jd_360Buy_Sign:JdUnion_RequestClass
    {
        public Jd_360Buy_Sign()
        {

        }
    }
    public class AccessTokenClass : JdUnion_RequestClass
    {
        //JDY_ModuleClass module;
        ////public void SetModule(JDY_ModuleClass mod)
        ////{
        ////    module = mod;
        ////}
        public AccessTokenClass()
        {
            //strJsonName = "system_access_token";
            //ReqBaseUrl = "https://api.kingdee.com/auth/user/access_token";
            //InitRequestJson();
        }

        


        //public JDY_ModuleClass UseModule { get; set; }
        
        public string avatar { get; set; }
        public long expires { get; set; }
        public long expires_in { get; set; }
        public int gender { get; set; }
        public string nickname { get; set; }
        public long uid { get; set; }

  }
    
    
}

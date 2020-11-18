using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlProcess;
using System.Xml;
using System.Reflection;
using WolfInv.Com.WCS_Process;
//using WolfInv.com.WinInterComminuteLib;
namespace WolfInv.com.ShareLotteryLib
{
    public class SystemSetting
    {

        public static string allProviteChat
        {
            get
            {
                string name = "allProviteChat";
                if(SysParams.ContainsKey(name))
                {
                    return SysParams[name];
                }
                return null;
            }
        }
        public static string saveMsg
        {
            get
            {
                string name = "saveMsg";
                if (SysParams.ContainsKey(name))
                {
                    return SysParams[name];
                }
                return null;
            }
        }
       
        static Dictionary<string, string> tSysParams;
        public static Dictionary<string, string> SysParams
        {
            get
            {
                if(tSysParams==null)
                {
                    tSysParams = getConfig();
                }
                return tSysParams;
            }
        }
        static Dictionary<string, string> getConfig()
        {

            Dictionary<string, string> ret = new Dictionary<string, string>();
            string xml = TextFileComm.getFileText("config.xml", "xml");
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(xml);
                XmlNode root = xmldoc.SelectSingleNode("root");
                XmlNodeList items = root.SelectNodes("system/item");
                foreach (XmlNode item in items)
                {
                    string key = XmlUtil.GetSubNodeText(item, "@key");
                    string val = XmlUtil.GetSubNodeText(item, "@value");
                    if(!ret.ContainsKey(key))
                    {
                        ret.Add(key, val);
                    }
                }
            }
            catch (Exception ce)
            {
                return ret;
            }
            return ret;
        }

        public static Dictionary<string, CITMSUser> UserInfos_del = new Dictionary<string, CITMSUser>();

        
    }



    public class UserInfo_del:WolfInv.Com.WCS_Process.CITMSUser
    {
        public string wxOpenId;
        public string Tel;
        public string wxUId;

        
    }
}

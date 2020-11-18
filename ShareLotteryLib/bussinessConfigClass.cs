using System;
using System.Collections.Generic;
using System.Xml;
namespace WolfInv.com.ShareLotteryLib
{

    public class bussinessConfigItemClass
    {
        public int id=1;
        public int regIndex;
        public int refIndex;
        public string title;
        public string reqInfo;
        public string dataType = "string";
        public string defaultValue;
        public int maxLen = -1;
        public int minLen = -1;
        public bool noNull;
        public string splitStrings = "[,| | |;]";
        public bool hiden;
        public bool isUserInfo;
        public bool isTypeInfo;
        public bool isActionInfo;
        public bool isCombo;
        public string sysType;
        public string parentRefId;
        public string parentRefSplit=",";
        public bool isKey;
    }

    public class bussinessConfigClass
    {
        public string matchKey;
        public string title;
        public string reqSrc;
        public bool needLogin;
        public string keyCol;
        public List<bussinessConfigItemClass> item = new List<bussinessConfigItemClass>();
        public List<bussinessCondition> condition = new List<bussinessCondition>();
    }

    public class bussinessCondition
    {
        public string i;
        public string o ="=";
        public string l = "And";
        public string v;
    }

    public class bussinessConfig
    {
        public List<bussinessConfigClass> configs = new List<bussinessConfigClass>();
    }

    public class bussinessProcess
    {
        public static bussinessConfig bussinessList;
        static bussinessProcess()
        {
            //Load();
        }
        public static void Load()
        {
            if (bussinessList == null)
                bussinessList = new bussinessConfig();
            string xml = TextFileComm.getFileText("bussinessList.xml", "xml");
            if (xml == null)
                return ;
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(xml);
                XmlNodeList bs = xmldoc.SelectNodes("config/bussiness");
                List<string> bitem = xmlBaseClass.ReadXml(bs);
                bussinessList.configs = xmlBaseClass.ReadDatas<bussinessConfigClass>( bitem);

            }
            catch(Exception ce)
            {

            }
        }
    }
    
}

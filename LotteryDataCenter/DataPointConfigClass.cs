using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using XmlProcess;
namespace LotteryDataCenter
{
    public class DataPointConfigClass
    {
        public Dictionary<string, LotteryAccessDataPoint> Points;
        public DataPointConfigClass()
        {
            Load();
        }

        void Load()
        {
            XmlDocument xmldoc = new XmlDocument();
            string strXML = GlobalClass.ReadFile("DataPointConfig.xml");
            try
            {
                xmldoc.LoadXml(strXML);
                XmlNodeList nodes = xmldoc.SelectNodes("root/Datapoint");
                Points = new Dictionary<string, LotteryAccessDataPoint>();
                foreach (XmlNode node in nodes)
                {
                    XmlNodeList inodes = node.SelectNodes("item");
                    Dictionary<string, string> lotteryList = new Dictionary<string, string>();
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    string dpid = XmlUtil.GetSubNodeText(node, "@id");
                    string dpTitle = XmlUtil.GetSubNodeText(node, "@title");
                    foreach (XmlNode inode in inodes)
                    {
                        if(XmlUtil.GetSubNodeText(inode,"@key").Equals("lotteryList"))
                        {
                            XmlNodeList nodelist = inode.SelectNodes("item"); 
                            foreach(XmlNode li in nodelist)
                            {
                                lotteryList.Add(XmlUtil.GetSubNodeText(li, "@key"), XmlUtil.GetSubNodeText(li, "@value"));
                            }
                            continue;
                        }
                        keyValues.Add(XmlUtil.GetSubNodeText(inode, "@key"), XmlUtil.GetSubNodeText(inode, "@value"));
                    }
                    LotteryAccessDataPoint dp = new LotteryAccessDataPoint();
                    DataTypePoint.ReadData(dp.GetType(), dp,keyValues);
                    dp.PointId = dpid;
                    dp.lotteryList = lotteryList;
                    dp.PointTitle = dpTitle;
                    Points.Add(dpid, dp);
                }
            }
            catch (Exception ce)
            {
                throw ce;
            }
        }
    }

    public class LotteryAccessDataPoint:IWebFlags
    {
        public string PointId;
        public string PointTitle;
        public bool needNavigateAccess;
        public bool needLogin;
        public string loginPageUrl;
        public string loginId;
        public string loginPwd;
        public string dataFormat;
        public string host;
        public string paramsModel;
        public string period;
        public bool useFixUrl;
        public string LotteryPage;
        public string hostKey;
        public string loginedFlag;
        public Dictionary<string, string> lotteryList;

        public string HostKey
        {
            get
            {
                return hostKey;
            }
        }

        public string LoginedFlag
        {
            get
            {
                return loginedFlag;
            }
        }

        public string AmountId
        {
            get
            {
                return "";
            }
        }
    }

    
}

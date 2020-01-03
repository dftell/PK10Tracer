using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using WolfInv.com.ShareLotteryLib;
using XmlProcess;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using WolfInv.com.WebCommunicateClass;

namespace HappyShareLottery
{
    /// <summary>
    /// 群消息推送服务
    /// </summary>
    partial class ChartPushService : ServiceBase
    {
        List<Timer> AllTimers = new List<Timer>();
        XmlNode defaultSetting = null;
        public Action<string> MessageTo;
        public ChartPushService()
        {
            InitializeComponent();
            
        }

        public static string getShortLink(string url)
        {
            string createUrl = "https://dwz.cn/admin/v2/create";
            String SIGN = "7d2772ad2792943aa67ae1213f9288d9";
            string content_type = "application/json";
            string strUrl = "{\"Url\":\"{0}\"}";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(createUrl);
            string ret = null;
            try
            {
                req.Method = "Post";
                //req.Headers.Add("Content-Type", content_type);
                req.ContentType = content_type;
                req.Headers.Add("Token", SIGN);
                

                string Data = strUrl.Replace("{0}", url);
                byte[] byteArray = Encoding.UTF8.GetBytes(Data);
                Stream newStream = req.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    ret = new StreamReader(wr.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                    wr.Close();
                }
                if (!string.IsNullOrEmpty(ret))
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    shortLinkReturnResult returnResult = javaScriptSerializer.Deserialize<shortLinkReturnResult>(ret);
                    if(returnResult.Code != 0)
                    {
                        return null;
                    }
                    return returnResult.ShortUrl;
                }
                return null;
            }
            catch (Exception ce)
            {

            }
            return "";
        }

        /*
         {
    "Code": 0,
    "ShortUrl": "https://dwz.cn/de3rp2Fl",
    "LongUrl": "http://www.baidu.com",
    "ErrMsg": ""
}
             */
        class shortLinkReturnResult
        {
            public int Code;
            public string ShortUrl;
            public string LongUrl;
            public string ErrMsg;
        }
        public void Init()
        {
            Dictionary<string, XmlNode> allPlans = getPlanXmlDictionary();
            AllTimers = new List<Timer>();
            foreach (string key in allPlans.Keys)
            {
                XmlNode node = allPlans[key];
                wxChartPushClass wxp = new wxChartPushClass();
                wxp.MessageTo = msgTo;
                wxp.shortUrlFunc = getShortLink;
                wxp.Init(node);
                
                if (wxp.disabled)
                    continue;
                if (string.IsNullOrEmpty(wxp.chartName))
                {
                    continue;
                }
                if (Program.allContacts == null)
                {
                    continue;
                }
                var contact = Program.allContacts.Where(a => a.Key.StartsWith("@@") == true);
                contact = contact.Where(a => a.Value.Equals(wxp.chartName));
                if (contact.Count() == 0)
                {
                    continue;
                }

                wxp.MessageTo?.Invoke(string.Format("群名[{0}]数据已经加载!", wxp.chartName));
                wxp.chartUid = contact.First().Key;
                Timer tm = new Timer();
                tm.Interval = wxp.interMinutes * 60 * 1000;
                tm.Tick += sendTo;
                tm.Tag = wxp;
                tm.Enabled = false;
                AllTimers.Add(tm);
            }
        }

        void msgTo(string msg)
        {
            MessageTo?.Invoke(msg);
        }


        private void sendTo(object sender, EventArgs e)
        {
            wxChartPushClass wxp = (sender as Timer).Tag as wxChartPushClass;
            if (wxp == null)
                return;
            if(wxp.chartUid == null)
            {
                return;
            }
            wxp.pushGoods();
        }



        public void AddChartPoints(List<wxChartPushClass> newLists)
        {

        }

        Dictionary<string, XmlNode> getPlanXmlDictionary()
        {

            Dictionary<string, XmlNode> ret = new Dictionary<string, XmlNode>();
            
            string xml = TextFileComm.getFileText("pushPlan.xml", "xml");
            if (xml == null)
                return ret;
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(xml);
                
                XmlNode root = xmldoc.SelectSingleNode("root");
                defaultSetting = root.SelectSingleNode("DefaultSetting");
                wxChartPushClass.initSysSetting(defaultSetting);
                wxChartPushClass.initEliteList(root.SelectSingleNode("elites"));
                XmlNodeList nlist = root.SelectNodes("toWxChats/chatPoint");
                foreach (XmlNode node in nlist)
                {

                    string name = XmlUtil.GetSubNodeText(node, "@name");
                    if(!ret.ContainsKey(name))
                    {
                        ret.Add(name, node);
                    }
                }
            }
            catch (Exception ce)
            {
                return ret;
            }
            return ret;
        }


        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            for(int i=0;i< AllTimers.Count;i++)
            {
                AllTimers[i].Enabled = true;
                sendTo(AllTimers[i], null);
            }

        }

        

        public void Start()
        {
            OnStart(null);
        }
        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            for (int i = 0; i < AllTimers.Count; i++)
            {
                AllTimers[i].Enabled = false;
            }

        }
    }
}

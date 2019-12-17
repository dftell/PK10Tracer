using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Reflection;
using System.Linq;
using WolfInv.com.JdUnionLib;
using WolfInv.com.ShareLotteryLib;
using XmlProcess;

namespace HappyShareLottery
{
    /// <summary>
    /// 群消息推送服务
    /// </summary>
    partial class ChartPushService : ServiceBase
    {
        List<Timer> AllTimers = new List<Timer>();
        XmlNode defaultSetting = null;
        public ChartPushService()
        {
            InitializeComponent();
            Dictionary<string, XmlNode> allPlans = getPlanXmlDictionary(ref defaultSetting);
            foreach(string key in allPlans.Keys)
            {
                XmlNode node = allPlans[key];
                wxChartPushClass wxp = new wxChartPushClass();
                wxp.Init(node);
                if (wxp.disabled)
                    continue;

                
            }
        }

        public void AddChartPoints(List<wxChartPushClass> newLists)
        {

        }

        Dictionary<string, XmlNode> getPlanXmlDictionary(ref XmlNode defaultSetting)
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
                defaultSetting = root.SelectSingleNode("DefaultSetting = ");
                XmlNodeList nlist = root.SelectNodes("toWxChats");
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
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }
    }

    public class wxChartPushClass
    {
        public string chartUid { get; set; }
        public static Dictionary<string, string> sysDefaultCommSetting;
        public Dictionary<string, string> MySetting;
        DateTime startTime;
        DateTime endTime;
        int CurrTimes = 0;
        int defaultInterMinutes = 30;//默认间隔为30分钟
        bool needPushNews = true;
        bool needPushKnowedge = true;
        bool needPushGeeting = true;
        int picsOnce = 1;
        public bool needSleep = false;//修改时间
        public int needSleepMinuts = 30;//需要调整Timer的间隔
        public List<JdGoodSummayInfoItemClass> CurrDayNeedPushGoods;
        public bool disabled { get; set; }
        public wxChartPushClass()
        {
            startTime = DateTime.MinValue;
            endTime = DateTime.MinValue;
            MySetting = sysDefaultCommSetting;
        }
        
        public void Init(XmlNode node)
        {
            foreach(XmlAttribute att in node.Attributes)
            {
                if(MySetting.ContainsKey(att.Name))
                {
                    MySetting[att.Name] = att.Value;
                }
                else
                {
                    MySetting.Add(att.Name, att.Value);
                }
            }
            if(MySetting.ContainsKey("interMinutes"))//间隔分钟数
            {
                
                int mins = 0;
                bool succ = int.TryParse(MySetting["interMinutes"], out mins);
                if (succ)
                    defaultInterMinutes = mins;
            }
            if (MySetting.ContainsKey("picsOneTime"))//一次发送条数
            {

                int mins = 0;
                bool succ = int.TryParse(MySetting["picsOneTime"], out mins);
                if (succ)
                    picsOnce = mins;
            }
            if(MySetting.ContainsKey("disabled"))
            {
                if(MySetting["disabled"] == "1")
                {
                    disabled = true;
                }
            }
        }

        bool inPushTime()
        {
            
            string strStartTime = null;
            if(MySetting.ContainsKey("startTime"))
            {
                strStartTime = MySetting["startTime"];
            }
            if(string.IsNullOrEmpty(strStartTime))
            {
                strStartTime = "8:00";
            }
            startTime = DateTime.Now.Date.Add(Convert.ToDateTime(strStartTime).TimeOfDay);
            string strEndTime = null;
            if (MySetting.ContainsKey("endTime"))
            {
                strEndTime = MySetting["endTime"];
            }
            if (string.IsNullOrEmpty(strEndTime))
            {
                strEndTime = "23:55";
            }
            endTime = DateTime.Now.Date.Add(Convert.ToDateTime(strEndTime).TimeOfDay);
            DateTime CurrTime = DateTime.Now;
            if (CurrTime >= startTime && CurrTime<=endTime)
            {
                needSleep = false;
                return true;
            }
            return false;
        }

        bool willEndThePush()
        {
            if (inPushTime() == false)
                return false;
            if (endTime.AddMinutes(defaultInterMinutes) > endTime)
                return true;
            return false;
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        public void pushGoods()
        {
            if(!inPushTime())
            {
                CurrTimes = 0;//归0
            }
            if(CurrTimes == 0)//如果是一天的开始
            {
                pushNews();
            }
            CurrTimes++;
            pushGoodsContent(CurrTimes, picsOnce);
            if(willEndThePush())//如果是一天的结尾
            {
                pushGeeting();
                needSleep = true;
                needSleepMinuts = (int)(startTime.AddDays(1).Subtract(DateTime.Now).TotalMinutes);
            }
        }

        public void InitChatGoods()
        {
            CurrDayNeedPushGoods = new List<JdGoodSummayInfoItemClass>();
            int pushTimes = (int)(endTime.Subtract(startTime).TotalMinutes / picsOnce);//总分钟数/每次个数+1
            CurrDayNeedPushGoods = FilterGoods().Take(pushTimes).ToList();
        }

        List<JdGoodSummayInfoItemClass> FilterGoods()
        {
            List<JdGoodSummayInfoItemClass> ret = new List<JdGoodSummayInfoItemClass>();
            ret = JdGoodsQueryClass.AllcommissionGoods.Values.ToList();
            return ret;
        }

        /// <summary>
        /// 推送新闻
        /// </summary>
        public void pushNews()
        {
            if (!JdGoodsQueryClass.Inited)
                return;
            
        }

        /// <summary>
        /// 推送问候语
        /// </summary>
        public void pushGeeting()
        {
            
        }

        /// <summary>
        /// 推送商品内容
        /// </summary>
        /// <param name="ps"></param>
        public void pushGoodsContent(int times,int ps)
        {
            if(CurrDayNeedPushGoods == null)
            {
                return;
            }
            if(CurrDayNeedPushGoods.Count<((times-1)*ps))
            {
                return;
            }
            int index = (times - 1) * ps;
            for (int i = index; i<Math.Min(CurrDayNeedPushGoods.Count, index+ps);i++)
            {
                JdGoodSummayInfoItemClass jdi = CurrDayNeedPushGoods[i];
                Program.plancolls.MsgProcess.SendUrlImgMsg(jdi.imgageUrl,chartUid);
                Program.plancolls.MsgProcess.SendMsg(jdi.getFullContent(true),chartUid);
            }
        }
    }
}

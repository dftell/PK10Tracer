using System;
using System.Collections.Generic;
using WolfInv.com.WebCommunicateClass;
using WolfInv.com.BaseObjectsLib;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;
using mshtml;
using Gecko;
using WolfInv.com.ProbMathLib;
//using WolfInv.com.SecurityLib;
namespace WolfInv.com.WebRuleLib
{
    //前后页类
    public abstract class FHPLotteryConfigClass : LotteryConfigClass
    {
        public string cRuleId_S = "8140101";// '前5位定胆
        public string cRuleId_B = "8140102";// '后5位定胆
        protected FHPLotteryConfigClass(WebRule we, LotteryTypes rs,string name) : base(we, rs,name)
        {
        }
    }
    /// <summary>
    /// 冠亚军类
    /// </summary>
    public abstract class F2LotteryyConfigClass : FHPLotteryConfigClass
    {
        public string g01_00 = "8010101";//猜冠军
        public string g0102_00 = "8020101";//猜冠亚军
        public string g0102_01 = "8020201";//猜冠亚军单式
        protected F2LotteryyConfigClass(WebRule we, LotteryTypes rs,string name) : base(we, rs,name)
        {
        }
    }

    
    

   
    public class Rule_ForKcaiCom : WebRule
    {
        GlobalClass gsetting;
        public Rule_ForKcaiCom(string name,GlobalClass setting)
            : base(name,setting)
        {
            
            

        }
        
        public override string IntsToJsonString(string lotteryName, String ccs, int unit)
        {
            LotteryConfigClass lcc = null;
            WebConfig wc = this.config;
            LotteryTypes lt = null;
            if (!wc.lotteryTypes.ContainsKey(lotteryName))
                return ccs;
            lt = wc.lotteryTypes[lotteryName];
            switch (lotteryName)
            {
                
                case "XYFT":
                    {
                        lcc = new LotteryConfigClass_XYFT(this,lt,lotteryName); 
                        break;
                    }
                case "GDKL11":
                    {
                        lcc = new LotteryConfigClass_GDKL11(this, lt,lotteryName);
                        break;
                    }
                
                case "PK10":
                default:
                    {
                        lcc = new PK10KindLotteryConfigClass(this, lt,lotteryName);
                        break;
                    }
                    
                    
            }
            if (lcc == null)
                return null;
            lcc.setting = GobalSetting;
            return lcc.IntsToJsonString(ccs, unit);
        }

        
        

        


       



        public override string getRealUrl(string strHtml)
        {
            /*<html><head><title>Attention Required</title></head><body><script>
             * window.location.href="https://www.kcai868.com/?jskey=CM6xfF0AAAAAa4ePQGlTB96zPMQSfuw5+GpeO7+PLkMjP3w8srs=";</script>
             * </body></html>
            */
            Regex regTr = new Regex(@"window.location.href=""(.*?)""");
            MatchCollection mcs = regTr.Matches(strHtml);
            string ret = null;
            if (mcs.Count == 1)
            {
                ret = mcs[0].Value.Replace("\"", "");
                ret = ret.Replace("window.location.href=", "");
            }

            return ret;
        }

        protected override Dictionary<string, int> GetChanlesInfo(string NavUrl)
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            AccessWebServerClass wcc = new AccessWebServerClass();
            string strHtml = AccessWebServerClass.GetData(NavUrl);
            if (strHtml == null)
            {
                return ret;
            }
            Regex regTr = new Regex(@"www\.kcai(.*?)\.com");
            MatchCollection mcs = regTr.Matches(strHtml);
            List<string> list = new List<string>();
            string urlModel = "https://www.kcai{0}.com";
            Task[] tasks = new Task[mcs.Count];
            for (int i = 0; i < mcs.Count; i++)
            {
                string name = mcs[i].Value.Replace("www.kcai", "").Replace(".com", "");
                DateTime begT = DateTime.Now;
                string url = string.Format(urlModel, name);
                ConnectClass cls = new ConnectClass(ret, name, url);
                //new Thread(new ThreadStart(cls.ConnectToUrl)).Start();
                tasks[i] = new Task(cls.ConnectToUrl);
                tasks[i].Start();
                ////string reqdata = AccessWebServerClass.GetData(url);
                ////DateTime endT = DateTime.Now;
                ////if(reqdata == null)
                ////{
                ////    ret.Add(name, 0);
                ////    continue;
                ////}
                ////int rate = (int)(reqdata.Length/ endT.Subtract(begT).TotalSeconds);
                ////ret.Add(name, rate);
            }
            Task.WaitAll(tasks);
             return ret;
        }
        class ConnectClass
        {
            string name;
            Dictionary<string, int> ret;
            string ConnUrl;
            public ConnectClass(Dictionary<string, int> _ret, string _name, string _url)
            {
                ret = _ret;
                ConnUrl = _url;
                name = _name;
            }
            public void ConnectToUrl()
            {
                DateTime begT = DateTime.Now;
                string reqdata = AccessWebServerClass.GetData(ConnUrl);
                DateTime endT = DateTime.Now;
                if (reqdata == null)
                {
                    ret.Add(name, 0);
                    return;
                }
                int rate = 0;
                if (reqdata.IndexOf("K彩在线娱乐") > 0 || reqdata.IndexOf("www.kcai") > 0)
                {
                    rate = (int)(reqdata.Length / endT.Subtract(begT).TotalSeconds);
                }
                ret.Add(name, rate);
            }
        }

        
        
        public override object getVerCodeImage(HtmlDocument indoc)
        {
            HTMLDocument doc = (HTMLDocument)indoc.DomDocument;
            if (doc == null)
                return doc;
            //Image img = null;
            HtmlElement ImgeTag = indoc.GetElementById("img-valcode");

            //mshtml.IHTMLElementCollection
            HTMLBody body = (HTMLBody)doc.body;
            IHTMLControlRange rang = (IHTMLControlRange)body.createControlRange();
            IHTMLControlElement Img = (IHTMLControlElement)ImgeTag.DomElement; //图片地址     
            object oldobj = Clipboard.GetDataObject(); //备份粘贴版数据    
            rang.add(Img);
            rang.execCommand("Copy", false, null);  //拷贝到内存      
            Image numImage = Clipboard.GetImage();
            rang = null;
            return numImage;
        }
        public override string getChargeQCode(HtmlDocument indoc)
        {
            HTMLDocument doc = (HTMLDocument)indoc.DomDocument;
            if (doc == null)
                return null;//网页文档对象为空
            IHTMLElementCollection hcols = doc.getElementsByTagName("canvas");
            if(hcols.length==0)
            {
                return "";//图片对象为空
            }
            var canvas = hcols.item(null,0);
            var imagedata = canvas.toDataURL();
            return imagedata;
        }
        

        public override string getChargeNum(HtmlDocument indoc)
        {
            if (indoc == null)
                return null;//网页文档对象为空
            HTMLDocument doc = indoc.DomDocument as HTMLDocument;
            IHTMLElementCollection hcols = doc.getElementsByClassName("order-number");
            if (hcols.length == 0)
            {
                return null;
            }
            var item = hcols.item(null, 0);
            return item.innerText;
        }

        public override string getChargeAmt(HtmlDocument indoc)
        {
            if (indoc == null)
                return null;
            HTMLDocument doc = indoc.DomDocument as HTMLDocument;
            IHTMLElementCollection hcols = doc.getElementsByClassName("order-amount");
            if (hcols.length == 0)
            {
                return null;
            }
            var item = hcols.item(null, 0);
            return item.innerText;
        }

        public override string getErr_Msg(HtmlDocument indoc)
        {
            if (indoc == null)
                return null;
            HTMLDocument doc = indoc.DomDocument as HTMLDocument;
            IHTMLElementCollection hcols = doc.getElementsByClassName("err_msg");
            if (hcols.length == 0)
            {
                return null;
            }
            var item = hcols.item(null, 0);
            return item.innerText;
        }

        

       

        

    }

   
    
}

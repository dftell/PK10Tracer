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
using System.Security.Permissions;
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




    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
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




        public override string ToInstItem(InstClass ic)
        {
            return ic.GetJson(true);//直接转换为Json
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


        public override bool LoginSuccFunc(string res)
        {
            
            WebUserInfoClass wic = new WebUserInfoClass();
            wic.returnJson = res;
            wic.Msg = res;
            if (res.ToLower().IndexOf("suc")<0)
            {
                this.SuccLogin?.Invoke(wic);
                return false;
            }
            wic.Succ = true;
            this.SuccLogin?.Invoke(wic);
            //return base.LoginSuccFunc(res);
            return true;
        }

        public override bool GetGameInfoSuccFunc(string res)
        {
            GameInfoClass gic = new GameInfoClass();
            gic.returnJson = res;
            KCai_GameInfoReturnClass girc = new KCai_GameInfoReturnClass();
            girc = JsonableClass<KCai_GameInfoReturnClass>.FromJson<KCai_GameInfoReturnClass>(res);
            if (girc == null)
            {
                gic.Msg = "获取游戏信息错误！";
                this.SuccGetGameInfo?.Invoke(gic);
                return false;
            }
            gic.Succ = true;
            this.SuccGetGameInfo?.Invoke(gic);
            return true;
        }

        public override bool GetAmountSuccFunc(string res)
        {
            KCai_UserGamePoint amtobj = new KCai_UserGamePoint();
            amtobj = amtobj.FromJson(res);
            
            
            AmountInfoClass aic = new AmountInfoClass();
            if(amtobj == null)
            {
                aic.CurrMoney = 0;
                this.SuccGetAmount?.Invoke(aic);
                return false;
            }
            aic.Succ = true;
            double val = 0;
            bool ret = double.TryParse(amtobj.GamePoint, out aic.CurrMoney);
            //aic.CurrMoney = val;
            this.SuccGetAmount?.Invoke(aic);
            return ret;
        }

        public override bool CancelBetSuccFunc(string res)
        {
            KCai_CancelBetReturnClass resret = new KCai_CancelBetReturnClass();
            resret = resret.FromJson(res);
            
            WebServerReturnClass ret = new WebServerReturnClass();
            ret.returnJson = res;
            if(resret == null)
            {
                this.SuccCancelBet?.Invoke(ret);
                return false;
            }
            if (resret.result != "OK")
            {
                ret.Msg = resret.result;
                this.SuccCancelBet?.Invoke(ret);
                return false;
            }
            ret.Succ = true;
            this.SuccCancelBet?.Invoke(ret);
            return false;
        }

        public override bool SendCompletedFunc(string res)
        {

            return base.SendCompletedFunc(res);
        }

        public override bool BetRecSuccFunc(string res)
        {
            BetRecordListClass gic = new BetRecordListClass();
            gic.returnJson = res;
            string fullres = "{\"recentData\":" + res + "}";
            KCai_BetRecReturnClass girc = JsonableClass<string>.FromJson<KCai_BetRecReturnClass>(fullres);
            if (girc == null)
            {
                gic.Msg = "获取游戏信息错误！";
                this.SuccGetBetRec?.Invoke(gic);
                return false;
            }
            gic.Data = new List<BetRecordClass>();
            girc.recentData.ForEach(a => {

                gic.Data.Add(
                new BetRecordClass()
                {
                    Nums = a.BetContent,
                    BetType = a.BetRuleId,
                    Cost = a.TotalAmount,
                    GameName = a.BetRuleId,
                    GameId = a.LotteryCode,
                    Status = a.Mark,
                    EarnedAmount = a.BonusAmount,
                    BetAmount = a.TotalAmount,
                    SerialNo = a.IssueNumber,
                    ID = a.EncodeId,
                    Key = a.IssueNumber,
                    CreateTime = a.CreateTime,
                    ReturnAmount = a.profit
                });
            });
            gic.Succ = true;
            this.SuccGetBetRec?.Invoke(gic);
            return true;

        }


        public override bool SendSuccFunc(string res, string dtp,string expect, string InstsText)
        {
            KCai_BetReturnClass ret = new KCai_BetReturnClass();
            ret = ret.FromJson(res);
            WebBetReturnInfoClass wric = new WebBetReturnInfoClass();
            wric.returnJson = res;
            wric.dtp = dtp;
            wric.SerialNo = expect;
            wric.SendData = InstsText;
            if (ret == null)
            {
                wric.Msg = "发送错误!";
                this.SuccSend?.Invoke(wric);
                return false;
            }
            if (ret.Ok != 1)
            {
                wric.Msg = "投注错误:"+ret.Tip;
                this.SuccSend?.Invoke(wric);
                return false;
            }
            double.TryParse(ret.gamePoint, out wric.restAmount);
            wric.Succ = true;
            wric.Msg = null;
            //wric.SerialNo = ret.;//千万不能用返回的期号，因为不同平台的序号不一定相同，会导致接收时判断错误，接收到新记录而误认为不是新记录
            //wric.betRecordCnt = ret.BetDatas.Count;
            wric.betAmt = string.Format("{0}", ret.BetAmount);
            wric.betRecInfo = string.Format("本次投注共计{0}元", ret.BetAmount);
            this.SuccSend?.Invoke(wric);
            return true;
            //return base.SendSuccFunc(res);
        }

        public override object[] getBetKeys(string strCookie, string strHtml, BetRecordClass bet)
        {
            string key = bet.Key;
            string id = bet.ID;
            string val = null;
            WebRule.existElement(strHtml, "input|name|__RequestVerificationToken|value", out val);
            string __req = val;
            return new object[] { __req, id, bet.SerialNo,bet.GameId };
        }

        public override void AjaxErrorFunc(string title,string res)
        {
            base.AjaxErrorFunc(title,res);
        }


        public class KCai_BetReturnClass:JsonableClass<KCai_BetReturnClass>
        {
            public int Ok;//: 1,
            public string Tip;
            public string gamePoint;//": 453.1180,
            public string BonusNum;//": "",
            public string BonusTotalNumber;//: -1,
            public string BonusAmount;//: 0,
            public string BetAmount;//: 0.02,
            public string rightTraces;
            public List<KCai_BetDetailReturnClass> Orders;
        }

        public class KCai_UserGamePoint:JsonableClass<KCai_UserGamePoint>
        {
            /*{"Id":191833,
             * "RealName":null,
             * "Name":"gsyh",
             * "ReturnPoint":0.1280,
             * "UserLevel":0,
             * "UserType":1,
             * "Avator":null,
             * "GamePoint":453.1380,
             * "AgentLevel":0}
        */
            public string Id;
            public string GamePoint;
        }

        public class KCai_GameInfoReturnClass:JsonableClass<KCai_GameInfoReturnClass>
        {

        }

        public class KCai_BetRecReturnClass:JsonableClass<KCai_BetRecReturnClass>
        {
            public List<KCai_BetDetailReturnClass> recentData;
            /*
             {"recentData":[
             {"EncodeId":"3D77E8FAH2",
             "Id":1031268602,
             "Uid":191833,
             "LotteryCode":32,
             "IssueNumber":"2005080020",
             "OrderTime":"\/Date(1588868345000)\/",
             "CreateTime":"\/Date(1588868351000)\/",
             "AnnounceTime":"\/Date(1588868345000)\/",
             "BatchNo":"200508001905191833",
             "BetContent":"大",
             "SortOrder":0,
             "BetRuleId":32010101,
             "Odds":1.95,
             "BetUnitPrice":0.0200,
             "TotalBet":1,
             "TotalAmount":0.0200,
             "BonusAmount":0.0000,
             "BonusTotalNumber":-1,
             "PlusOdds":0.00,
             "OrderStatus":10,
             "IsTraceOrder":false,
             "Mark":"玩家撤单! 撤单时间:2020-05-08 00:19:11",
             "BoNums":"","InsertDate":"2020-05-08 00:19:05",
             "CloseTime":"",
             "BonusNumber":"4,2,6",
             "ReturnPoint":0.1280,
             "LoginName":"gsyh",
             "UserType":1,
             "RuleName":"和值-大",
             "LotteryName":"KC一分快3",
             "profit":null,
             "hasRunLottery":"否"},
             {"EncodeId":"3D77E8B3H7","Id":1031268531,"Uid":191833,"LotteryCode":6,"IssueNumber":"2020080","OrderTime":"\/Date(1588868312000)\/","CreateTime":"\/Date(1588868312000)\/","AnnounceTime":"\/Date(1588868312000)\/","BatchNo":"200508001832191833","BetContent":"0,,","SortOrder":0,"BetRuleId":6040101,"Odds":9.68,"BetUnitPrice":0.0200,"TotalBet":1,"TotalAmount":0.0200,"BonusAmount":0.0000,"BonusTotalNumber":-1,"PlusOdds":0.00,"OrderStatus":0,"IsTraceOrder":false,"Mark":"普通投注","BoNums":"","InsertDate":"2020-05-08 00:18:32","CloseTime":"","BonusNumber":null,"ReturnPoint":0.1280,"LoginName":"gsyh","UserType":1,"RuleName":"定位胆","LotteryName":"福彩3D","profit":null,"hasRunLottery":"否"},
             {"EncodeId":"3D77E3D0HG","Id":1031267280,"Uid":191833,"LotteryCode":8,"IssueNumber":"744352","OrderTime":"\/Date(1588867916000)\/","CreateTime":"\/Date(1588867932000)\/","AnnounceTime":"\/Date(1588867916000)\/","BatchNo":"200508001156191833","BetContent":",,01,,","SortOrder":0,"BetRuleId":8140101,"Odds":9.78,"BetUnitPrice":0.0200,"TotalBet":1,"TotalAmount":0.0200,"BonusAmount":0.0000,"BonusTotalNumber":-1,"PlusOdds":0.00,"OrderStatus":10,"IsTraceOrder":false,"Mark":"玩家撤单! 撤单时间:2020-05-08 00:12:12","BoNums":"","InsertDate":"2020-05-08 00:11:56","CloseTime":"","BonusNumber":null,"ReturnPoint":0.1280,"LoginName":"gsyh","UserType":1,"RuleName":"前五定位胆","LotteryName":"PK10","profit":null,"hasRunLottery":"否"},
             {"EncodeId":"3D77BF64HR","Id":1031257956,"Uid":191833,"LotteryCode":8,"IssueNumber":"744351","OrderTime":"\/Date(1588865820000)\/","CreateTime":"\/Date(1588865820000)\/","AnnounceTime":"\/Date(1588866847000)\/","BatchNo":"200507233700191833","BetContent":"10,,,,","SortOrder":0,"BetRuleId":8140102,"Odds":9.78,"BetUnitPrice":0.0200,"TotalBet":1,"TotalAmount":0.0200,"BonusAmount":0.0000,"BonusTotalNumber":0,"PlusOdds":0.00,"OrderStatus":1,"IsTraceOrder":false,"Mark":"普通投注","BoNums":"","InsertDate":"2020-05-07 23:37:00","CloseTime":"","BonusNumber":"04,06,09,10,02,08,05,07,01,03","ReturnPoint":0.1280,"LoginName":"gsyh","UserType":1,"RuleName":"后五定位胆","LotteryName":"PK10","profit":-0.0200,"hasRunLottery":"是"},
             {"EncodeId":"3D77A625H4","Id":1031251493,"Uid":191833,"LotteryCode":8,"IssueNumber":"744350","OrderTime":"\/Date(1588864505000)\/","CreateTime":"\/Date(1588864505000)\/","AnnounceTime":"\/Date(1588865704000)\/","BatchNo":"200507231505191833","BetContent":",,,,07","SortOrder":0,"BetRuleId":8140102,"Odds":9.78,"BetUnitPrice":0.0200,"TotalBet":1,"TotalAmount":0.0200,"BonusAmount":0.0000,"BonusTotalNumber":0,"PlusOdds":0.00,"OrderStatus":1,"IsTraceOrder":false,"Mark":"普通投注","BoNums":"","InsertDate":"2020-05-07 23:15:05","CloseTime":"","BonusNumber":"04,09,02,08,06,10,07,03,01,05","ReturnPoint":0.1280,"LoginName":"gsyh","UserType":1,"RuleName":"后五定位胆","LotteryName":"PK10","profit":-0.0200,"hasRunLottery":"是"},
             {"EncodeId":"3D77A61DHF","Id":1031251485,"Uid":191833,"LotteryCode":21,"IssueNumber":"20050801","OrderTime":"\/Date(1588864502000)\/","CreateTime":"\/Date(1588864502000)\/","AnnounceTime":"\/Date(1588864502000)\/","BatchNo":"200507231502191833","BetContent":"02 06 10","SortOrder":0,"BetRuleId":21110101,"Odds":161.37,"BetUnitPrice":18.0000,"TotalBet":1,"TotalAmount":18.0000,"BonusAmount":0.0000,"BonusTotalNumber":-1,"PlusOdds":0.00,"OrderStatus":0,"IsTraceOrder":false,"Mark":"普通投注","BoNums":"","InsertDate":"2020-05-07 23:15:02","CloseTime":"","BonusNumber":null,"ReturnPoint":0.1280,"LoginName":"gsyh","UserType":1,"RuleName":"前三组选复式","LotteryName":"广东11选5","profit":null,"hasRunLottery":"否"}]}
             */

        }
        public class KCai_BetDetailReturnClass:JsonableClass<KCai_BetDetailReturnClass>
        {
            public string EncodeId;
            public string Id;
            public string Uid;
            public string LotteryCode;
            public string IssueNumber;
            public string OrderTime;
            public string CreateTime;
            public string AnnounceTime;
            public string BatchNo;
            public string BetContent;
            public string SortOrder;
            public string BetRuleId;
            public string Odds;
            public string BetUnitPrice;
            public string TotalBet;
            public string TotalAmount;
            public string BonusAmount;
            public string BonusTotalNumber;
            public string PlusOdds;
            public string OrderStatus;
            public string IsTraceOrder;
            public string Mark;
            public string BoNums;
            public string CloseTime;
            public string BonusNumber;
            public string ReturnPoint;
            public string LoginName;
            public string UserType;
            public string RuleName;
            public string LotteryName;
            public string profit;
            public string hasRunLottery;
        }


        public class KCai_CancelBetReturnClass:JsonableClass<KCai_CancelBetReturnClass>
        {
            public string result;
            public string orders; 
        }

    }

   
    
}

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
//using WolfInv.com.SecurityLib;
namespace WolfInv.com.WebRuleLib
{
    public class Rule_ForKcaiCom : WebRule
    {
        public Rule_ForKcaiCom(GlobalClass setting)
            : base(setting)
        {
            Load("rule_kcai.xml", "Rules");

        }
        string g01_00 = "8010101";//猜冠军
        string g0102_00 = "8020101";//猜冠亚军
        string g0102_01 = "8020201";//猜冠亚军单式
        String cRuleId_S = "8140101";// '前5位定胆
        String cRuleId_B = "8140102";// '后5位定胆
        public override string IntsToJsonString(String ccs, int unit)
        {
            //unit为单位 0，元；1，角；2，分，3，其他
            ccs = ccs.Trim();
            ccs = ccs.Replace("  ", "");
            ccs = ccs.Replace("+", " ");
            ccs = ccs.Trim();
            if (ccs.Length == 0) return "";
            String[] ccsarr = ccs.Split(' ');
            List<InstClass> InsArr = new List<InstClass>();
            double unitVal = 0;
            unitVal = getUnitValue(unit);
            //int ArrCnt = 0;
            for (int i = 0; i < ccsarr.Length; i++)
            {
                String cc;
                String ccNos;// As String,
                String ccCars;//As String,
                Int64 ccUnitCost;// As Long,
                String[] ccArr;
                cc = ccsarr[i].Trim();
                if (cc.Length == 0) continue;
                ccArr = cc.Split('/');
                if (ccArr.Length < 3) continue;
                ccNos = ccArr[0].Trim();
                String ccOrgCars = ccArr[1].Trim();
                ccCars = toStdCarFmt(ccOrgCars).Trim();//车号组合标准格式
                ccUnitCost = Int64.Parse(ccArr[2]);
                if (ccUnitCost == 0)
                    continue;
                String[] sArr = new String[5];
                String[] bArr = new String[5];
                for (int j = 0; j < 5; j++)
                {
                    sArr[j] = "";
                    bArr[j] = "";
                }
                int sArrCnt, bArrCnt;
                sArrCnt = 0;
                bArrCnt = 0;
                for (int j = 0; j < ccNos.Length; j++)
                {
                    String strsNo;//As String
                    int iNo;// As Integer
                    strsNo = ccNos.Substring(j, 1);// Mid(ccNos, j, 1)
                    if (strsNo.Equals("0"))
                    {
                        strsNo = "10";
                    }
                    iNo = int.Parse(strsNo);
                    if (iNo <= 5)
                    {
                        sArr[iNo - 1] = ccCars.Trim();
                        sArrCnt++;
                    }
                    else
                    {
                        bArr[iNo - 5 - 1] = ccCars.Trim();
                        bArrCnt++;
                    }
                }
                if (sArrCnt > 0)
                {
                    InstClass ic = new InstClass();
                    ic.ruleId = cRuleId_S;
                    ic.betNum = String.Format("{0}", ccOrgCars.Length * sArrCnt);
                    ic.itemTimes = String.Format("{0:N2}", unitVal * ccUnitCost);
                    ic.selNums = Array2String(sArr).Replace(", ", ",");
                    ic.jsOdds = String.Format("{0:N2}", GobalSetting.Odds);
                    ic.priceMode = unit;
                    InsArr.Add(ic);
                }
                if (bArrCnt > 0)
                {
                    InstClass ic = new InstClass();
                    ic.ruleId = cRuleId_B;
                    ic.betNum = String.Format("{0}", ccOrgCars.Length * bArrCnt);
                    ic.itemTimes = String.Format("{0:N2}", unitVal * ccUnitCost);
                    ic.selNums = Array2String(bArr).Replace(", ", ",");
                    ic.jsOdds = String.Format("{0:N2}", GobalSetting.Odds);
                    ic.priceMode = unit;
                    InsArr.Add(ic);
                }

            }
            if (InsArr.Count > 0)
            {
                return this.IntsListToJsonString(InsArr);
            }
            else
            {
                return "";
            }
        }

        public override string IntsListToJsonString(List<InstClass> Insts)
        {

            String[] strInsts = new String[Insts.Count];
            for (int i = 0; i < Insts.Count; i++)
            {
                InstClass ic = Insts[i];
                strInsts[i] = ic.GetJson();
            }
            return String.Format("[{0}]", Array2String(strInsts));
        }

        static String toStdCarFmt(String cars)
        {
            cars = cars.Trim();
            String[] CarArr = new String[cars.Length];
            for (int i = 0; i < cars.Length; i++)
            {
                String strCar = cars.Substring(i, 1);
                if (strCar.Equals("0"))
                    strCar = "10";
                String tmp = String.Format("0{0}", strCar).Trim();
                CarArr[i] = tmp.Substring(tmp.Length - 2).Trim();
            }
            CarArr.ToString();
            return Array2String(CarArr).Replace(" ", "").Replace(",", " ").Trim();
            //[{"jsOdds":"9.78","priceMode":2,"selNums":",01020710,01020710,,","betNum":8,"itemTimes":"0.01","ruleId":"8140101"}]
        }

        static String Array2String(string[] arr)
        {
            String ret = string.Join(",", arr);
            return ret;
            //return ret.substring(1, ret.Length - 1).Trim();//去掉中括号
        }

        static double getUnitValue(int unit)
        {
            return Math.Pow(0.1, unit);
        }


        public override bool IsVaildWeb(HtmlDocument doc)
        {
            return true;
            return doc?.GetElementById("img-valcode") != null;
        }

        public override bool IsLogined(HtmlDocument doc)
        {

            //HtmlElement ElPoint = doc?.GetElementById("userGamePointId");
            return doc?.GetElementById("userGamePointId") != null;
        }

        public override double GetCurrMoney(HtmlDocument doc)
        {
            HtmlElement ElPoint = doc?.GetElementById("userGamePointId");
            double ret = 0;
            if (ElPoint != null)
            {
                double.TryParse(ElPoint?.InnerText, out ret);
            }
            return ret;
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

        public abstract class LotterClass : ILotteryRule
        {
            protected WebRule wr;
            public LotteryTypes rules;
            protected LotterClass(WebRule we, LotteryTypes rs)
            {
                rules = rs;
                wr = we;
            }
            public abstract string IntsListToJsonString(List<InstClass> Insts);

            public abstract string IntsToJsonString(string ccs, int unit);
        }

        public class PK10 : LotterClass
        {
            
            LotteryBetRuleClass cRuleId_S;
            LotteryBetRuleClass cRuleId_B;
            public PK10(WebRule we, LotteryTypes rs):base(we,rs)
            {
                

                cRuleId_S = rules.AllRules["8140101"];
                cRuleId_B = rules.AllRules["8140102"];
            }

            public override string IntsToJsonString(String ccs, int unit)
            {
                //unit为单位 0，元；1，角；2，分，3，其他
                ccs = ccs.Trim();
                ccs = ccs.Replace("  ", "");
                ccs = ccs.Replace("+", " ");
                ccs = ccs.Trim();
                if (ccs.Length == 0) return "";
                String[] ccsarr = ccs.Split(' ');
                List<InstClass> InsArr = new List<InstClass>();
                double unitVal = 0;
                unitVal = getUnitValue(unit);
                //int ArrCnt = 0;
                for (int i = 0; i < ccsarr.Length; i++)
                {
                    String cc;
                    String ccNos;// As String,
                    String ccCars;//As String,
                    Int64 ccUnitCost;// As Long,
                    String[] ccArr;
                    cc = ccsarr[i].Trim();
                    if (cc.Length == 0) continue;
                    ccArr = cc.Split('/');
                    if (ccArr.Length < 3) continue;
                    ccNos = ccArr[0].Trim();
                    String ccOrgCars = ccArr[1].Trim();
                    ccCars = toStdCarFmt(ccOrgCars).Trim();//车号组合标准格式
                    ccUnitCost = Int64.Parse(ccArr[2]);
                    if (ccUnitCost == 0)
                        continue;
                    String[] sArr = new String[5];
                    String[] bArr = new String[5];
                    for (int j = 0; j < 5; j++)
                    {
                        sArr[j] = "";
                        bArr[j] = "";
                    }
                    int sArrCnt, bArrCnt;
                    sArrCnt = 0;
                    bArrCnt = 0;
                    for (int j = 0; j < ccNos.Length; j++)
                    {
                        String strsNo;//As String
                        int iNo;// As Integer
                        strsNo = ccNos.Substring(j, 1);// Mid(ccNos, j, 1)
                        if (strsNo.Equals("0"))
                        {
                            strsNo = "10";
                        }
                        iNo = int.Parse(strsNo);
                        if (iNo <= 5)
                        {
                            sArr[iNo - 1] = ccCars.Trim();
                            sArrCnt++;
                        }
                        else
                        {
                            bArr[iNo - 5 - 1] = ccCars.Trim();
                            bArrCnt++;
                        }
                    }
                    if (sArrCnt > 0)
                    {
                        InstClass ic = new InstClass();
                        ic.ruleId = cRuleId_S.BetRule;
                        ic.betNum = String.Format("{0}", ccOrgCars.Length * sArrCnt);
                        ic.itemTimes = String.Format("{0:N2}", unitVal * ccUnitCost);
                        ic.selNums = Array2String(sArr).Replace(", ", ",");
                        ic.jsOdds = String.Format("{0:N2}", wr.config.WebWholeOdds);
                        ic.priceMode = unit;
                        InsArr.Add(ic);
                    }
                    if (bArrCnt > 0)
                    {
                        InstClass ic = new InstClass();
                        ic.ruleId = cRuleId_B.BetRule;
                        ic.betNum = String.Format("{0}", ccOrgCars.Length * bArrCnt);
                        ic.itemTimes = String.Format("{0:N2}", unitVal * ccUnitCost);
                        ic.selNums = Array2String(bArr).Replace(", ", ",");
                        ic.jsOdds = String.Format("{0:N2}", wr.config.WebWholeOdds);
                        ic.priceMode = unit;
                        InsArr.Add(ic);
                    }

                }
                if (InsArr.Count > 0)
                {
                    return this.IntsListToJsonString(InsArr);
                }
                else
                {
                    return "";
                }
            }

            public override string IntsListToJsonString(List<InstClass> Insts)
            {

                String[] strInsts = new String[Insts.Count];
                for (int i = 0; i < Insts.Count; i++)
                {
                    InstClass ic = Insts[i];
                    strInsts[i] = ic.GetJson();
                }
                return String.Format("[{0}]", Array2String(strInsts));
            }

        }

        public class A11C5:LotterClass
        {


            public A11C5(WebRule we, LotteryTypes rs):base(we,rs)
            {
                rules = rs;
                wr = we;

                //cRuleId_S = rules.AllRules["8140101"];
                //cRuleId_B = rules.AllRules["8140102"];
            }

            public override string IntsListToJsonString(List<InstClass> Insts)
            {
                throw new NotImplementedException();
            }

            public override string IntsToJsonString(string ccs, int unit)
            {
                throw new NotImplementedException();
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
                return null;
            IHTMLElementCollection hcols = doc.getElementsByTagName("canvas");
            if(hcols.length==0)
            {
                return null;
            }
            var canvas = hcols.item(null,0);
            var imagedata = canvas.toDataURL();
            return imagedata;
        }
        public override bool IsLoadCompleted(HtmlDocument indoc)
        {
            string strNotice = "网站重要通知！！！";
            HTMLDocument hdoc = indoc.DomDocument as HTMLDocument;
            if (hdoc.body.outerHTML.IndexOf(strNotice) > 0)
            {
                return true;
            }
            return false;
        }

        public override string getChargeNum(HtmlDocument indoc)
        {
            if (indoc == null)
                return null;
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

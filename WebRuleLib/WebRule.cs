﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.WebCommunicateClass;
using WolfInv.com.BaseObjectsLib;
using System.Windows.Forms;
using XmlProcess;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;
using Gecko;
using System.Security.Permissions;
namespace WolfInv.com.WebRuleLib
{

    public class BetUnits:List<BetUnit>
    {
        public BetUnit DefaultUnit
        {
            get
            {
                if (this.Count > 0)
                    return this[0];
                return null;
            }
        }

        public void LoadXml(XmlNode node)
        {
            int cnt = int.Parse(XmlUtil.GetSubNodeText(node, "@basecnt"));
            float level = float.Parse(XmlUtil.GetSubNodeText(node, "@baselevel"));
            XmlNodeList nodes = node.SelectNodes("Node");
            foreach(XmlNode sn in nodes)
            {
                BetUnit bu = new BetUnit();
                bu.baseCnt = cnt;
                bu.baseLevel = level;
                bu.LoadXml(sn);
                this.Add(bu);
            }
        }
    }
    public class BetUnit
    {
        protected double baseval = 0.1;
        public string id { get; set; }
        public string Name { get; set; }
        public int Power { get; set; } //0.1的次方，分为2
        public int baseCnt { get; set; }//基本数
        public Single baseLevel { get; set; }
        public BetUnit() //默认0.02
        {
            id = "cent";
            Name = "分";
            Power = 2;
            baseCnt = 2;
            baseLevel = 0.1F;
        }

        public double getOneUnitValue()
        {
            return baseCnt * Math.Pow(baseLevel , Power);
        }
        public void LoadXml(XmlNode node)
        {
            id = XmlUtil.GetSubNodeText(node, "@id");
            Name = XmlUtil.GetSubNodeText(node, "@name");
            Power = int.Parse(XmlUtil.GetSubNodeText(node, "@power"));
        }
    }

    public class WebServerReturnClass
    {
        public bool Succ;
        public string dtp;
        public string SendData;
        public string returnJson;
        public string Title;
        public string Msg;
    }
    public class WebUserInfoClass : WebServerReturnClass
    {
        public string Name;
        public string Pwd;
        public string BankId;
        public string AssetPwd;
    }

    public class AmountInfoClass:WebServerReturnClass
    {
        public double CurrMoney;
    }

    public class WebBetReturnInfoClass: WebServerReturnClass
    {
        public double restAmount;
        public string SerialNo;
        public int betRecordCnt;
        public string betRecInfo;
        public string betAmt;
        

    }
    public class BetRecordClass
    {
        public string SerialNo;
        public string ID;
        public string Key;
        public string GameName;
        public string GameId;
        public string CreateTime;
        public string Position;
        public string Nums;
        public string Cost;
        public string EarnedAmount;
        public string Status;
        public string BetAmount;
        public string ReturnAmount;
        public string Odds;
        public string BetType;
    }

    public class BetRecordListClass:WebServerReturnClass
    {
        public List<BetRecordClass> Data;
    }
    public class GameInfoClass : WebServerReturnClass
    {
        public string GameId;
        public string GameCode;
        public string NewestExpect;
        public Dictionary<string, string> AllPastIssues;
        public List<string> MyBetList;
    }
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public abstract class WebRule : ILotteryRule
    {
        public IWebFlags GobalSetting;
        public WebConfig config;
        public GlobalClass gb;
        public string webName;
        //public abstract string IntsListToJsonString(List<InstClass> Insts);
        public virtual string IntsToJsonString(DataTypePoint dtp, string LotteryName,String ccs, int unit)
        {
            return ccs;
        }
        //IWebFlags
        //public GlobalClass GobalSetting;
        
        protected WebRule(string name,IWebFlags setting,GlobalClass _gb)
        {
            webName = name;
            GobalSetting = setting;
            gb = _gb;
            if(!Load("rule.xml", string.Format("config\\{0}\\Rules\\", name)))
            {
                Load("rule.xm", "config\\Rules");
            }
        }

        public Action<WebBetReturnInfoClass> SuccSend;
        public Action<WebUserInfoClass> SuccLogin;
        public Action<AmountInfoClass> SuccGetAmount;
        public Action<GameInfoClass> SuccGetGameInfo;
        public Action<string,string> CompleteSendMsg;
        public Action<BetRecordListClass> SuccGetBetRec;
        public Action<WebServerReturnClass> AJaxError;
        public Action<WebServerReturnClass> SuccCancelBet;
        public Action<string, string> MsgBox;

        public abstract string ToInstItem(InstClass ic);

        public bool Load(string filename,string foldername)
        {
            config = new WebConfig();
            string xmltext = GlobalClass.LoadTextFile(filename, foldername);
            if(string.IsNullOrEmpty(xmltext))
            {
                return false;
            }
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(xmltext);
                config.LoadXml(xmldoc);
                return true;
            }
            catch (Exception ce)
            {
                return false;
            }
        }
                
        #region 获取或判断对象值
        public  bool IsLoadCompleted(HtmlDocument indoc)
        {
            string strNotice = GobalSetting.HostKey;
            bool suc = WebRule.existElement(indoc, GobalSetting.HostKey);
            if (suc)
                return true;
            for (int i = 0; i < indoc.Window.Frames.Count; i++)
            {
                suc = WebRule.existElement(indoc.Window.Frames[i].Document, GobalSetting.HostKey);
                if (suc)
                    return true;
            }
            return false;
        }

        public  bool IsVaildWeb(GeckoDocument doc)
        {
            return true;
        }

        public bool IsLogined(GeckoDocument doc)
        {
            return WebRule.existElement(doc, GobalSetting.LoginedFlag);
            return doc?.GetElementById(GobalSetting.LoginedFlag) != null;
        }

        public bool IsLoadCompleted(GeckoDocument indoc)
        {
            string strNotice = GobalSetting.HostKey;
            
            return WebRule.existElement(indoc, GobalSetting.HostKey);


            if (indoc.Head.OuterHtml.IndexOf(strNotice) > 0)
            {
                return true;
            }
            return false;
        }

        public bool IsLogined(HtmlDocument doc)
        {

            bool suc = WebRule.existElement(doc, GobalSetting.LoginedFlag);
            if (suc)
                return suc;
            for (int i=0;i<doc.Window.Frames.Count;i++)
            {
                suc = WebRule.existElement(doc.Window.Frames[i].Document, GobalSetting.LoginedFlag);
                if (suc)
                {
                    return true;
                }
            }
            return false;
        }

        public double GetCurrMoney(HtmlDocument doc)
        {
            string outval = "";
            bool succ = WebRule.existElement(doc, GobalSetting.AmountId, out outval);
            if(succ)
            {
                double outRes = 0.00;
                if (double.TryParse(outval, out outRes))
                {
                    return outRes;
                }
                return 0;
            }
            for (int i = 0; i < doc.Window.Frames.Count; i++)
            {
                succ = WebRule.existElement(doc.Window.Frames[i].Document, GobalSetting.AmountId, out outval);
                if (succ)
                {
                    double outRes = 0.00;
                    if (double.TryParse(outval, out outRes))
                    {
                        return outRes;
                    }
                    //return 0;
                }
            }
            return 0;
            HtmlElement ElPoint = doc?.GetElementById(GobalSetting.AmountId);
            double ret = 0;
            if (ElPoint != null)
            {
                double.TryParse(ElPoint?.InnerText, out ret);
            }
            return ret;
        }

        public double GetCurrMoney(GeckoDocument doc)
        {
            string outval = "";
            bool succ = WebRule.existElement(doc, GobalSetting.AmountId, out outval);
            if (succ)
            {
                double outRes = 0.00;
                if (double.TryParse(outval, out outRes))
                {
                    return outRes;
                }
                return 0;
            }
            return 0;
            GeckoElement ElPoint = doc?.GetElementById(GobalSetting.AmountId);
            double ret = 0;
            if (ElPoint != null)
            {
                double.TryParse(ElPoint?.TextContent, out ret);
            }
            return ret;
        }


        public bool IsVaildWeb(HtmlDocument doc)
        {
            return true;
        }

        #endregion


        public virtual string getCookieByName(string strCookie, string key)
        {
            string[] arr = strCookie.Split(';');
            for (int i = 0; i < arr.Length; i++)
            {
                string[] carr = arr[i].Split('=');
                if(carr[0] == key)
                {

                    return carr[1];
                }
            }
            return null;
        }

        public virtual object[] getBetKeys(string strCookie,string html,BetRecordClass bet)
        {
            return new object[] { };
        }

        protected abstract Dictionary<string, int> GetChanlesInfo(string url);

        public string GetChanle(string url,string CurrChanle,bool ForceGetFastChanle=false)
        {
            Dictionary<string, int> chls = GetChanlesInfo(url);//获得所有线路的信息
            string ret = CurrChanle;
            if (!chls.ContainsKey(CurrChanle))//如果通道已经不包括当前通道了，获取最新通道
            {
                ret = GetFastChanle(chls);
            }
            else//当前通道仍在通道清单中
            {
                if (chls.Where(a => a.Value == 0).Select(a => a.Key).ToList().Contains(CurrChanle))//如果当前通道已经无法访问，切换为最快的线路！
                {
                    ret = GetFastChanle(chls);
                }
                else
                {
                    if (ForceGetFastChanle)//强制获取最快线路
                    {
                        ret = GetFastChanle(chls);
                    }
                    else
                    {
                        if (chls.Where(a => a.Value > 0).OrderBy(a => a.Value).First().Key.Equals(CurrChanle))//如果能访问的通道中当前通道排名最后，切换为最快线路
                        {
                            ret = GetFastChanle(chls);
                        }
                    }
                }
            }
            return ret ?? CurrChanle;//如果未空使用当前通道
        }

        string GetFastChanle(Dictionary<string, int> Chanles)
        {
            if (Chanles == null || Chanles.Count == 0)
                return null;
            int val = Chanles.Where(a => a.Value > 0).Max(a => a.Value);//获取最大网速值
            if (val <= 0)//小于等于0返回空
                return null;
            return Chanles.Where(a => a.Value == val)?.First().Key;//返回等于最大网速值得那个通道编号
        }

        public abstract object getVerCodeImage(HtmlDocument doc);

        public abstract string getRealUrl(string html);

        public abstract string getChargeQCode(HtmlDocument doc);

        public abstract string getChargeNum(HtmlDocument doc);
        public abstract string getChargeAmt(HtmlDocument doc);

        public abstract string getErr_Msg(HtmlDocument doc);

        public static bool existElement(object doc, string tag)
        {
            string outval = null;
            return existElement(doc, tag,out outval);
        }

        public static bool existElement(object doc,string tag,out string outVal)
        {
            outVal = null;
            if (doc == null)
                return false;
            bool isIE = doc is HtmlDocument;
            string[] arr = tag.Split('|'); //p|class|lkjlfd 
            string tagName = arr[0];//tag p ，a，table ，td，div
            string tagKeyType = "";//class ,id,name
            string tagKeyValue = "";//
            if (arr.Length > 1)
            {
                tagKeyType = arr[1];
            }
            if (arr.Length > 2)
            {
                tagKeyValue = tag.Substring(tagName.Length + 1 + tagKeyType.Length + 1);//除去 tag 和 约束[id|name]的值
            }
            if (isIE)
            {
                HtmlElementCollection hec = (doc as HtmlDocument).GetElementsByTagName(tagName);
                foreach (HtmlElement he in hec)
                {
                    string idval = null;
                    idval = he.GetAttribute(tagKeyType.Trim());
                    if (string.IsNullOrEmpty(idval)) //标签都没有
                        continue;
                    if (idval?.Trim().ToLower() == tagKeyValue.ToLower())//如果匹配上了
                    {
                        outVal = he.InnerText;
                        return true;
                    }
                }
                outVal = null;
                return false;
            }
            else
            {
                GeckoElementCollection hec = (doc as GeckoDocument).GetElementsByTagName(tagName);
                foreach (GeckoElement he in hec)
                {
                    string idval = null;
                    idval = he.GetAttribute(tagKeyType.Trim());
                    if (string.IsNullOrEmpty(idval)) //标签都没有
                        continue;
                    if (idval?.Trim() == tagKeyValue)//如果匹配上了
                    {
                        outVal = he.TextContent;
                        return true;
                    }
                }
                outVal = null;
                return false;
            }
        }

        public static bool existElement(string html, string tag, out string outVal)
        {
            outVal = null;
            HtmlTagClass ret = HtmlTagClass.getTagInfo(html, tag);
            if (ret == null)
                return false;
            outVal = ret.AttValue;
            return true;
        }

        public virtual bool LoginSuccFunc(string res)
        {
            return false;
        }

        public virtual bool GetGameInfoSuccFunc(string res)
        {
            return false;
        }

        public virtual bool GetAmountSuccFunc(string res)
        {
            return false;
        }

        public virtual void LoginErrorFunc(string res)
        {

        }

        public virtual bool CancelBetSuccFunc(string res)
        {
            return false;
        }
        public virtual bool SendCompletedFunc(string res)
        {
            return false;
        }
        public virtual bool SendSuccFunc(string res,string dtp,string expect,string data)
        {
            return false;
        }

        public virtual void MsgTo(string title,string msg)
        {
            this.MsgBox?.Invoke(title, msg);
        }

        public virtual void AjaxErrorFunc(string title,string res)
        {
            WebBetReturnInfoClass wri = new WebBetReturnInfoClass();
            wri.returnJson = res;
            wri.Msg = res;
            wri.Title = title;
            wri.Succ = false;
            this.AJaxError?.Invoke(wri);
        }

        public virtual bool BetRecSuccFunc(string res)
        {
            return false;
        }

    }
}

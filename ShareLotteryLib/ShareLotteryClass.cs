using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using WolfInv.com.WXMessageLib;
using System.Xml;
using XmlProcess;

namespace WolfInv.com.ShareLotteryLib
{
    


    /// <summary>
    /// 共享彩票计划类 乐享彩
    /// </summary>
    [Serializable]
    public class ShareLotteryPlanClass
    {
        static Dictionary<SharePlanStatus, string> tStatusDic;
        static string stringStatus = "筹备期;认购期;缴款期;等待开奖;结算期;终止";
        public static Dictionary<SharePlanStatus,string> StatusDic
        {
            get
            {
                if(tStatusDic == null)
                {
                    tStatusDic = new Dictionary<SharePlanStatus, string>();
                    
                    List<string> arr = stringStatus.Split(';').ToList();
                    Predicate<string> pre = (a)=> {  return true; };
                    Dictionary<int,string> dic = arr.ToDictionary(a => arr.IndexOf(a), a => a);
                    Type t = typeof(SharePlanStatus);
                    foreach (int myCode in Enum.GetValues(t))
                    {
                        if(dic.ContainsKey(myCode))
                        {
                            tStatusDic.Add((SharePlanStatus)myCode, dic[myCode]);
                        }
                    }
                }
                return tStatusDic;
            }
        }

        public static MutliLevelData _AllLotteryList;
        public static MutliLevelData AllLottery
        {
            get
            {
                if (_AllLotteryList == null)
                {
                    _AllLotteryList = getLotteryDic();
                }
                return _AllLotteryList;
        }
        }

        static Dictionary<string, string> tAllLotteryNames;
        public static Dictionary<string,string> AllLotteryKeyNames
        {
            get
            {
                if(tAllLotteryNames == null)
                {
                    tAllLotteryNames = new Dictionary<string, string>();
                    AllLottery.SubList.Keys.ToList().ForEach(a => 
                    {
                        AllLottery.SubList[a].SubList.Keys.ToList().ForEach(
                            b=>
                            {
                                AllLottery.SubList[a].SubList[b].SubList.Keys.ToList().ForEach(
                                    c =>
                                    {
                                        string ret = "{0}_{1}_{2}";
                                        string key = string.Format(ret, a.key, b.key, c.key);
                                        
                                        if(!tAllLotteryNames.ContainsKey(key))
                                        {
                                            tAllLotteryNames.Add(key, c.text);
                                        }
                                    }
                                    );
                            }
                            );
                        
                        
                    });

                }
                return tAllLotteryNames;
            }
        }
        static Dictionary<string, string> tAllLotteryNameKeys;
        public static Dictionary<string,string> AllLotteryNameKeys
        {
            get
            {
                if(tAllLotteryNameKeys==null)
                {
                    tAllLotteryNameKeys = new Dictionary<string, string>();
                    AllLotteryKeyNames.Keys.ToList().ForEach(a=> {
                        if (!tAllLotteryNameKeys.ContainsKey(tAllLotteryNames[a]))
                        {
                            tAllLotteryNameKeys.Add(tAllLotteryNames[a], a);
                        }
                    });
                }
                return tAllLotteryNameKeys;
            }
        }

        public static void Reset()
        {
            _AllLotteryList = null;
            tAllLotteryNames = null;
            tAllLotteryNameKeys = null;
        }

        #region 基本信息
        /// <summary>
        /// 计划序号
        /// </summary>
        public long PlanIndex{get;set;} // "序号";
                                        

        string _guid;
        /// <summary>
        /// 计划标志号
        /// </summary>
        public string guid
        {
            get
            {
                if (string.IsNullOrEmpty(_guid))
                {
                    Guid gid = Guid.NewGuid();
                    _guid = gid.ToString();
                }
                return _guid;
            }
            set
            {
                _guid = value;
            }
        }
       
         /// <summary>
         /// 微信群名
         /// </summary>
        public string wxRootName{get;set;} // "群名";


        /// <summary>
        /// 合买wx群号
        /// </summary>
        public string wxRootNo{get;set;} // "群号";

        /// <summary>
        /// 发起人
        /// </summary>
        public string creator{get;set;} // "创建人";
        public string creatorNike { get; set; } //直接回复创建人使用

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime{get;set;} // "创建时间";

        /// <summary>
        /// 合买彩种
        /// </summary>
        public string betLottery{get;set;} // "目标彩种";
        
        /// <summary>
        ///目标期号
        /// </summary>
        public string betExpectNo { get; set; } // "目标期号";
        
        /// <summary>
        /// 每份金额
        /// </summary>
        public decimal? shareAmount {get;set;} // "单份金额";

        /// <summary>
        /// 目标分数
        /// </summary>
        public int? planShares{get;set;} // "目标份数";

        /// <summary>
        /// 成功认购的份数
        /// </summary>
        public int? subscribeShares{get;set;} // "实际份数";

        /// <summary>
        /// 成功认购金额
        /// </summary>
        public decimal? betAmount {get;set;} // "实际金额";

        /// <summary>
        /// 乐享彩计划状态
        /// </summary>
        public SharePlanStatus sharePlanStatus {get;set;} // "状态";

        /// <summary>
        /// 投注信息
        /// </summary>
        public string betInfo{get;set;} // "投注信息";

        /// <summary>
        /// 开奖信息
        /// </summary>
        public string openInfo{get;set;} // "投注结果";

        /// <summary>
        /// 是否中奖
        /// </summary>
        public bool matched{get;set;} // "中奖";

        /// <summary>
        /// 奖金
        /// </summary>
        public decimal? matchAmount{get;set;} // "奖金";

        /// <summary>
        /// 盈利
        /// </summary>
        public decimal? profitAmout{get;set;} // "盈利";

        
        /// <summary>
        /// 每份奖金
        /// </summary>
        public decimal? shareProfit {get;set;} // "每份奖金";

        #endregion

        #region 投注信息
        public List<PlanShareBetInfo> subscribeList;

        public Dictionary<string,PlanShareBetInfo> subscribDic
        {
            get
            {
                Dictionary<string, PlanShareBetInfo> ret = new Dictionary<string, PlanShareBetInfo>();
                var res = subscribeList.GroupBy(a => a.betWxName);
                foreach(var obj in res)
                {
                    PlanShareBetInfo bet = new PlanShareBetInfo(obj.First().shareAmt);
                    string key = obj.Key;
                    bet.betWxName = obj.First().betWxName;
                    bet.betNikeName = obj.First().betNikeName;
                    bet.subscribeShares = obj.Sum(a => a.subscribeShares);
                    bet.needPayAmount = obj.Where(a => a.Payed==false).Sum(a => a.needPayAmount);
                    ret.Add(key, bet);
                }
                return ret;
            }
        }
        #endregion

        public ShareLotteryPlanClass()
        {
            subscribeList = new List<PlanShareBetInfo>();
        }
        public string ToApplyFormatString()
        {
            string ret = "计划标识号:{0};";
            return ret;
        }

        public List<wxMessageClass> AllMsgs;

        

        static MutliLevelData getLotteryDic()
        {
            MutliLevelData ret = new MutliLevelData();
            string xml = TextFileComm.getFileText("LotteryList.xml","xml");
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(xml);
                XmlNode root = xmldoc.SelectSingleNode("root");
                XmlNodeList typenodes = root.SelectNodes("lotteryType/item");
                foreach(XmlNode typenode in typenodes)
                {
                    string val = XmlUtil.GetSubNodeText(typenode, "@value");
                    string text = XmlUtil.GetSubNodeText(typenode, "@text");
                    KeyText k = new KeyText(val, text);
                    if(ret.SubList.ContainsKey(k))
                    {
                        continue;
                    }
                    ret.SubList.Add(k, getNextLevel(root, val));
                }
                

            }
            catch(Exception ce)
            {
                return ret;
            }
            return ret;
        }

        static MutliLevelData getNextLevel(XmlNode root,string val)
        {
            MutliLevelData ret = new MutliLevelData();
            XmlNodeList slist = root.SelectNodes(string.Format("lottery[@type='{0}']/item", val));
            foreach (XmlNode snode in slist)
            {
                string v = XmlUtil.GetSubNodeText(snode, "@value");
                string t = XmlUtil.GetSubNodeText(snode, "@text");
                KeyText sk = new KeyText(v, t);
                if (ret.SubList.ContainsKey(sk))
                {
                    continue;
                }
                
                ret.SubList.Add(sk,getNextLevel(root,v));
            }
            return ret;
        }

        public string ToUserCreateModel()
        {
            string ret = @"
请填下以下信息并在群内回复我。合买信息[
合买编号:{0};
合买彩种:{1};
投注期号:{2};
投注内容:{3};
单份金额:{4};
认购份数:{5}]";
            if (!AllLotteryKeyNames.ContainsKey(this.betLottery))
            {
                return "请先回复正确的彩种名";
            }
            string fullret = string.Format(ret,this.guid, AllLotteryKeyNames[this.betLottery], this.betExpectNo??"您计划投注期号","计划投注的内容", "计划每份金额", "计划份数");
            return fullret;
        }

        public string ToBasePlanInfo()
        {
            string ret = @"[合买信息]
合买编号:{0};
合买彩种:{1};
投注期号:{2};
投注内容:{3};
开奖时间:{4};
单份金额:{5};
募集份数:{6};
已认购份数:{7};
已付款份数:{8};
当前状态:{9}";
            return string.Format(ret,
                this.guid, 
                AllLotteryKeyNames[this.betLottery],
                this.betExpectNo,
                this.betInfo??"未知",
                "",
                this.shareAmount,
                this.planShares,
                this.subscribeShares,
                this.subscribeList.Where(a=>a.Payed).Sum(a=>a.subscribeShares),
                StatusDic[this.sharePlanStatus]) ;
        }

        public string ToAllSubscribeInfo()
        {
            string m = 
@"认购信息如下:
{0}";
            string ret = string.Format(m, string.Join(";", this.subscribeList.Select(a => a.toSubscribeString())));
            return ret;
        }

        public string ToGroupedSubscribeInfo()
        {
            string m =
@"认购信息如下:
{0}";
            string ret = string.Format(m, string.Join(";", this.subscribDic.Select(a => a.Value.toSubscribeString())));
            return ret;
        }

        public string ToGroupedProfitInfo()
        {
            string m =
@"返奖结果:
{0}";
            string ret = string.Format(m, string.Join(";", this.subscribDic.Select(a => a.Value.toProfitString(this.shareProfit.Value))));
            return ret;
        }
    }

    [Serializable]
    public class KeyText
    {
        public string key;
        public string text;
        public KeyText(string k,string t)
        {
            key = k;
            text = t;
        }
    }

    /// <summary>
    /// 乐享彩计划状态
    /// </summary>
    public enum SharePlanStatus
    {
        Ready,//筹备中
        Subscribing,//正在认购中
        Paying,//认购完成，正在缴款中
        Paied,//缴款完成，等待开奖
        Opened,//开奖完成，返奖
        Completed //结束

    }
}

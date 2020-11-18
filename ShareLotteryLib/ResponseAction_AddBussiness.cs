using System.Collections.Generic;
using System.Text.RegularExpressions;
using WolfInv.com.WXMessageLib;
using System.Net;
using System.Net.Http;
using WolfInv.Com.JsLib;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using WolfInv.Com.WCS_Process;
using System.Text;
using WolfInv.Com.MetaDataCenter;

namespace WolfInv.com.ShareLotteryLib
{
    /// <summary>
    /// 彩票指令类型，目前不属于彩票合买流程
    /// </summary>
    public class ResponseAction_AddBussiness : ResponseActionClass
    {
        //public ResponseAction_Charge()
        //{

        //}
        //DbClass db;

        public ResponseAction_AddBussiness(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "新增业务";
            //db = new DbClass(SystemSetting.DbHost, SystemSetting.DbUser, SystemSetting.DbPwd, SystemSetting.DbName);
        }

        protected override string getMsg()
        {
            string ret = "请确认新增业务，决定是否继续！";
            return ret;
        }



        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {//(.*?)手动下注(|:)((\d+|.*)/(.*)\d+((/\d+)?))((\+|\s)((\d+|.*)/\d+((/\d+)?)))*
            //Regex regTr = new Regex(@"(.*?)手动下注(|:)((\d+|.*)/(.*)\d+((/\d+)?))((\+|\s)((\d+|.*)/\d+((/\d+)?)))*");
            Regex regTr = new Regex(actionDefine.regRule);
            MatchCollection mcs = regTr.Matches(pureMsg);
            string matchKey = null;
            string content = null;
            bussinessConfigClass bcc = null;
            Dictionary<string, string> allList = null;
            matchResult mr = null;
            string useBussinessType = actionDefine.useBussinessType;
            matchKey = useBussinessType;
            if (mcs.Count == 1 && mcs[0].Groups.Count> actionDefine.typeIndex+1)
            {
                if(string.IsNullOrEmpty(matchKey))
                    matchKey = mcs[0].Groups[actionDefine.typeIndex].Value;//用作获取信息类型，去业务列表中匹配改信息
                if(WXMsgProcess.bcDic.ContainsKey(matchKey))
                {
                    bcc = WXMsgProcess.bcDic[matchKey];
                }
                if(bcc == null)
                {
                    answerMsg(string.Format("未找到{0}的业务配置！",matchKey));
                    return false;
                }
                mr = getBussinessFormat(mcs[0].Groups, bcc);
                if (!mr.success)
                {
                    answerMsg(string.Format("数据校验错误:\n{0}",string.Join("\n",mr.msgs)));
                    return false;
                }
                if(mr.items == null)
                {
                    mr.items = new Dictionary<string, string>();
                }
                DataRequestType reqType = DataRequest.getReqType(mr.action);
                
                if (bcc.reqSrc.Trim().ToLower().Equals("fsms_user"))
                {
                    string mapid = GlobalShare.GlobalData?.getMappingId(typeof(CITMSUser), DisableMultiTaskProcess ? "wxOpenId" : "wxUId");
                    string wxid = null;
                    if (mr.items.ContainsKey(mapid))
                    {
                        wxid = mr.items[mapid];
                    }
                    else
                    {
                        answerMsg(string.Format("数据点{0}不存在！", mapid));
                    }
                    CITMSUser useUser = GlobalShare.GlobalData?.getUserByWxId(wxid,!DisableMultiTaskProcess);
                    if(useUser!= null && (reqType == DataRequestType.Add || reqType == DataRequestType.Update) )
                    {
                        answerMsg(string.Format("你的微信账号已经绑定了其他账户！"));
                        return false;
                    }
                    string grpMapId = GlobalShare.GlobalData?.getMappingId(typeof(CITMSUser), "GroupId");
                    string grpName = null;
                    string codeMapId = null;
                    string codeValue = null;
                    if (!string.IsNullOrEmpty(grpMapId))
                    {
                        codeMapId = GlobalShare.GlobalData?.getMappingId(typeof(CITMSUser), "code");
                        if (mr.items.ContainsKey(grpMapId) && mr.items.ContainsKey(codeMapId))
                        {
                            codeValue = mr.items[codeMapId];
                            grpName = mr.items[grpMapId];
                            useUser = GlobalShare.GlobalData.getUserByGrpAndCode(grpName, codeValue);
                            
                        }
                    }
                    grpName = GlobalShare.GlobalData?.getGroupByCode(grpName)?.name;
                    if (!string.IsNullOrEmpty(useUser?.code))//存在当前账号
                    {
                        if (reqType == DataRequestType.Add)
                        {
                            answerMsg(string.Format("你准备{0}的{1}{2}{3}已存在！", mr.action, grpName, bcc.title, useUser.code));
                            return false;
                        }
                        if(reqType ==  DataRequestType.Update)
                        {
                            string useWxId = DisableMultiTaskProcess ? useUser.wxOpenId : useUser.wxUId;
                            if(!string.IsNullOrEmpty(useWxId) && wxid != useWxId )
                            {
                                answerMsg(string.Format("你准备{0}的{1}{2}{3}已属于其他微信账户！", mr.action, grpName, bcc.title, useUser.code));
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if(reqType == DataRequestType.Update || reqType == DataRequestType.Delete)
                        {
                            answerMsg(string.Format("你准备{0}的{1}{2}{3}不存在！", mr.action, grpName, bcc.title, codeValue));
                            return false;
                        }
                    }
                }


                allList = mr.items;
                content = string.Join(";", mr.displayItems.Select(a => {
                    return string.Format("{0}:{1}", a.Key, a.Value);
                }));
            }
            else
            {
                answerMsg("抱歉，本汪无法理解您提交的内容！");
                return false;
            }
            TheAskWaitingUserAnswer ask = new TheAskWaitingUserAnswer(this);

            this.Buffs.Clear();
            this.Buffs.Add(matchKey);
            this.Buffs.Add(mr);
            this.Buffs.Add(bcc);
            if (actionDefine.needAsk && actionDefine.AskDefine != null)
            {
                ask.askData = new MutliLevelData();
                FillAsk(actionDefine.AskDefine,ref ask.askData);
                ask.isOpenAsk = actionDefine.isOpenAsk;
                ask.askMsg = string.Format(actionDefine.AskDefine.text, mr.action , content, ask.AskText,bcc.title);
                wxprocess.InjectAsk(ask);
                answerMsg(ask.askMsg);
                /*
                ask.askData = new MutliLevelData();

                ask.askData.AddSub("1", "确定", null);
                MutliLevelData noselect = ask.askData.AddSub("0", "否", new MutliLevelData());
                noselect.AddSub("0", "停止增加", null);
                noselect.AddSub("1", "重新提交其他命令", null);
                ask.askMsg = string.Format(@"确定增加业务{0}:{1}?
{2}", lotteryName, content, ask.AskText);
                wxprocess.InjectAsk(ask);
                answerMsg(ask.askMsg);*/
            }
            return false;


            optPlan = null;
            return false;
            //return base.Response();
        }




        
       

        public override bool ResponseAsk(TheAskWaitingUserAnswer ask)
        {
            //wxprocess.CopyToHistoryAsks(ask);
            // wxprocess.CloseCurrAsk(ask);
            if (ask.AnswerResult.key == "0")//结束流程
            {

                answerMsg("欢迎下次使用！");
                return false;
            }
            else
            {
                ShareLotteryPlanClass plan = null;
                if (ask.UserResponseAnswer.Count == 1) //确定上次的金额
                {
                    if (Buffs.Count <3)
                    {
                        answerMsg("上次存储的新增业务信息丢失！请重新申请");
                        return false;
                    }
                    string signatrue="";
                
                    //if (signatrue.ToString().ToUpper().Equals(sha1result))
                    //{

                    //}
                    try
                    {
                        
                        if(!DisableMultiTaskProcess)
                            Task.Run(() => {
                                submitData(Buffs[0] as string,Buffs[1] as matchResult, Buffs[2] as bussinessConfigClass);
                            });
                        else
                            submitData(Buffs[0] as string, Buffs[1] as matchResult, Buffs[2] as bussinessConfigClass);
                    }
                    catch (Exception ce)
                    {
                        answerMsg(string.Format("{0}:{1}", ce.Message, ce.StackTrace));
                        //throw ce;
                        return false;
                    }

                }
                else
                {
                    answerMsg("请重新提交业务信息！");
                    return false;
                }

                return false;
            }
            return false;
        }

        void submitData(string btype,matchResult mr, bussinessConfigClass define)
        {
            string urlM = "http://www.wolfinv.com/pk10/app/AddBussiness.asp?key={0}&keys={1}&src={2}&action={3}&{4}";
            if (!string.IsNullOrEmpty(actionDefine.submitUrl))
            {
                urlM = actionDefine.submitUrl;
            }
            string pars = string.Join("&",mr.items.Select(a => { return string.Format("{0}={1}", a.Key, a.Value); }));
            string keys = string.Join(",", mr.keys.Select(a => a.Key));
            string urlReq = string.Format(urlM,define.keyCol, HttpUtility.UrlEncode(keys), define.reqSrc,mr.action, pars);
            string url = urlReq;
            string ret = "";
            try
            {
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                ret = wc.DownloadString(url);
            }
            catch(Exception ce)
            {
                ret = string.Format("{0}:{1}", ce.Message, url);
            }
            CITMSUser existUser = GlobalShare.GlobalData?.getUserByWxId(wxmsg.FromUserNam, !DisableMultiTaskProcess);
            if (ret == "succ")
            {
                if (wxprocess.currChatUser != null)
                {
                    GlobalShare.GlobalData?.init(typeof(CITMSUser));//重新获取用户列表
                    CITMSUser user = GlobalShare.GlobalData?.getUserByWxId(wxmsg.FromUserNam, !DisableMultiTaskProcess);
                    string wxid = DisableMultiTaskProcess ? user.wxOpenId : user.wxUId;
                    
                    DataRequestType reqType = DataRequest.getReqType(mr.action);
                    if (user != null)
                    {
                        if (reqType == DataRequestType.Add || reqType == DataRequestType.Update)
                        {
                            
                        }
                    }
                    if (existUser != null)
                    {
                        
                    }
                    
                }
                answerMsg(string.Format("{0}{1}成功！", mr.action, define.title));
                return;
            }
            answerMsg(ret);
            
        }
    }

   
}

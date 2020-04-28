using System.Collections.Generic;
using System.Security.Permissions;
using WolfInv.com.BaseObjectsLib;
using System.Linq;
namespace WolfInv.com.WebRuleLib
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ASHC_WebRule:Rule_ForKcaiCom
    {
        

        public ASHC_WebRule(string name,GlobalClass setting):base(name,setting)
        {

        }

        public override bool LoginSuccFunc(string res)
        {
            ASHC_LoginReturnClass lr = new ASHC_LoginReturnClass();
            lr = lr.FromJson(res);
            if(lr == null)
            {
                return false;
            }
            WebUserInfoClass wic = new WebUserInfoClass();
            wic.returnJson = res;
            wic.Msg = lr.ErrorMsg;
            if(lr.SignInStatus != "0")
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
            ASHC_GameInfoReturnClass girc = new ASHC_GameInfoReturnClass();
            girc = girc.FromJson(res);
            if(girc == null)
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
            double val = 0;
            bool ret = double.TryParse(res, out val);
            AmountInfoClass aic = new AmountInfoClass();
            aic.CurrMoney = val;
            this.SuccGetAmount?.Invoke(aic);
            
            return ret;
        }

        public override bool CancelBetSuccFunc(string res)
        {
            WebServerReturnClass ret = new WebServerReturnClass();
            ret.returnJson = res;
            if(res.ToLower().IndexOf("true")<0)
            {
                ret.Msg = res;
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
            ASHC_BetRecReturnClass girc = new ASHC_BetRecReturnClass();
            girc = girc.FromJson(res);
            if (girc == null)
            {
                gic.Msg = "获取游戏信息错误！";
                this.SuccGetBetRec?.Invoke(gic);
                return false;
            }
            gic.Data = new List<BetRecordClass>();
            girc.data.ForEach(a => {

                gic.Data.Add(
                new BetRecordClass()
                {
                    Nums = a.Number,
                    BetType = a.BetTypeName,
                    Cost = a.Cost.ToString(),
                    GameName = a.LotteryGameName,
                    Status = a.StateStr,
                    EarnedAmount = a.EarnedAmount.ToString(),
                    BetAmount = a.Cost.ToString(),
                    SerialNo = a.SerialNumber,
                    ID = a.ID,
                    Key = a.hashKey,
                    CreateTime = a.CreateTimeStr,
                    ReturnAmount = a.Prize.ToString()
                });
            });
            gic.Succ = true;
            this.SuccGetBetRec?.Invoke(gic);
            return true;

        }


        public override bool SendSuccFunc(string res,string dtp,string InstsText)
        {
            ASHC_BetReturnClass ret = new ASHC_BetReturnClass();
            ret = ret.FromJson(res);
            WebBetReturnInfoClass wric = new WebBetReturnInfoClass();
            wric.returnJson = res;
            wric.dtp = dtp;
            wric.SendData = InstsText;
            if (ret == null)
            {
                wric.Msg = "返回结果非法！";
                this.SuccSend?.Invoke(wric);
                return false;
            }
            if(ret.ErrorMessageCode != 0)
            {
                wric.Msg = ret.ErrorMessage;
                this.SuccSend?.Invoke(wric);
                return false;
            }
            double.TryParse(ret.WalletAmount, out wric.restAmount);
            wric.Succ = true;
            wric.Msg = ret.ErrorMessage;
            wric.betRecordCnt = ret.BetDatas.Count;
            wric.betRecInfo = string.Format("本次投注共计{0}元，{1}",ret.BetDatas.Sum(a=>a.Multiple*a.Unit*a.Count), string.Join(";", ret.BetDatas.Select(a=> {
                return string.Format("{0}/{1}/{2}", a.Position.Replace("10", "0").Replace(" ",""), a.Number.Replace("10", "999").Replace("0","").Replace(",", "").Replace("999","0").Replace(" ","").Trim(), a.Multiple);
            })));
            this.SuccSend?.Invoke(wric);
            return true;
            //return base.SendSuccFunc(res);
        }

        public override object[] getBetKeys(string strCookie,string strHtml, BetRecordClass bet)
        {
            string key = bet.Key;
            string id = bet.ID;
            string val = null;
            WebRule.existElement(strHtml, "input|name|__RequestVerificationToken|value",out val);
            string __req = val;
            return new object[] { __req,id,key };
        }

        public override void AjaxErrorFunc(string res)
        {
            base.AjaxErrorFunc(res);
        }

        class ASHC_LoginReturnClass:JsonableClass<ASHC_LoginReturnClass>
        {
            //{"SignInStatus":0,"Role":"FrontEndAgent","ErrorMsg":""}
            public string SignInStatus;
            public string Role;
            public string ErrorMsg;
        }

        class ASHC_BetReturnClass:JsonableClass<ASHC_BetReturnClass>
        {
            /*{"WalletAmount":8584.509,
             * "WinningPrize":0,
             * "GuidValidateResult":false,
             * "WinningCount":0,
             * "ErrorMessageCode":0,
             * "ReturnPoint":0,
             * "LotteryGameID":70,
             * "BetDatas":[
            ASHC_BetInfoClass
             * ],
             * "NickName":null,
             * "HeadUrl":"",
             * "WinningNumber":"",
             * "ErrorMessage":null,
             * "SerialNumber":"20200416122",
             * "LotteryGameName":"幸运飞艇"}*/
            public string WalletAmount;
            public string WinningPrize;
            public List<ASHC_BetInfoClass> BetDatas;
            public string SerialNumber;
            public string ErrorMessage;
            public string LotteryGameName;
            public int ErrorMessageCode;
        }

        class ASHC_BetInfoClass
        {
            public double Unit;
            public string BetTypeCodeId;
            public string StateStr;
            public string Number;
            public string CreateDate;
            public string SerialNumber;
            public string ID;
            public string OddsDisplay;
            public string BetTypeCode;
            public int Multiple;
            public int Count;
            public string Position;
            /*
              * {"Unit":0.01,
             * "Commission":0,
             * "Prize":0,
             * "Cost":0.01,
             * "BetMode":0,
             * "HashKey":5,
             * "BetTypeCodeId":3007,
             * "Multiple":1,
             * "Count":1,
             * "ReturnRate":0,
             * "ID":327164028,
             * "StateStr":"未开奖",
             * "State":0,
             * "OddsDisplay":"0.097",
             * "Position":"10",
             * "CreateDate":"2020/04/16 23:12:07",
             * "Number":",,,,,,,,,08",
             * "BetTypeName":"定位胆",
             * "BetTypeCode":"NumberPositionMatchForR1Star",
             * "SerialNumber":"116231207747781"}
             */
        }
        class ASHC_GameInfoReturnClass:JsonableClass<ASHC_GameInfoReturnClass>
        {
            public int LotteryCategoryId;
        }

        class ASHC_BetRecReturnClass:JsonableClass<ASHC_BetRecReturnClass>
        {
            public List<ASHC_BetRec> data;
        }

        class ASHC_BetRec
        {
            public string BetModel;
            public string BetProposalID;
            public string BetTypeCode;
            public string BetTypeName;
            public double Cost;
            public string CreateTime;
            public string CreateTimeStr;
            public double EarnedAmount;
            public string hashKey;
            public bool HideBetNumber;
            public string ID;
            public string IssueSerialNumber;
            public string LotteryGameName;
            public string Number;
            public string Prize;
            public string returnAmount;
            public string ReturnRate;
            public string SerialNumber;
            public string State;
            public string StateStr;
            public string WinningNumber;
        }
    }
}

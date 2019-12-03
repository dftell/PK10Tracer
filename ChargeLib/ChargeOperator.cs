using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.WinInterComminuteLib;
using System.Linq;
using WolfInv.com.DbAccessLib;

namespace WolfInv.com.ChargeLib
{

    [Serializable]
    public class ChargeOperator: RemoteServerClass,ISerializable
    {
        //public DbClass db;
        public string SvrNam { get; set; }
        public string reqId = null;
        public string strAmt = null;
        public string strWxId = null;
        public string strWxName = null;
        public string strChargeAccount = null;
        public string strAccount = null;
        public Action<string,string,string,string> OperateChargeForm;
        public void ResponseCompleted(string a)
        {
            receivedData(a);

        }

        public void receivedData(string a)
        {
            
            string ret = null;
            string[] arr = a.Split('&');
            Dictionary<string, string> dic = arr.ToDictionary(b => b.Split('=')[0], b => b.Substring(b.Split('=')[0].Length + 1));
            List<string> jsonlist = new List<string>();
            if(!dic.ContainsKey("respTime"))
            {
                dic.Add("respTime", "");
            }
            
            dic["respTime"] = DateTime.Now.ToLongTimeString();
            //dic["chargeAccount"] = 
            foreach (string key in dic.Keys)
            {
                jsonlist.Add(string.Format("\"{0}\":\"{1}\"", key, dic[key]));
            }
            ret = "{{0}}".Replace("{0}", string.Join(",", jsonlist));
            //page.Response.Write(ret);
            this.ResponseString = ret;
            //"reqid={0}&amt={1}&errcode={2}&msg={3}&img={4}&num={5}";
            string errcode = dic["errcode"];

            if (!string.IsNullOrEmpty(errcode))//有错先返回，无错就保存
            {
                this.responsed = true;
                //Response(ret);
                return;
            }
            string strnum = null;
            string strimg = null;
            if (dic.ContainsKey("orderNum"))
                strnum = dic["orderNum"];
            if (dic.ContainsKey("imgData"))
            {
                strimg = dic["imgData"];
            }
            //string insql = "insert into userchargetable(chargeid,wxid,wxname,chargeamt,ordernum,imgurl,ChargeAccount) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
            //if (db != null)
            //{
            //    //int cnt = db.ExecSql(new ConditionSql(string.Format(insql, reqId, strWxId, strWxName, strAmt, strnum, strimg, strChargeAccount)));
            //}
            this.responsed = true;
            //Response(ret);
            //tcpClientSocket = null;
        }


        public bool responsed;
        public string ResponseString;
        public ChargeOperator()
        {
            SvrNam = "默认充值机";
            //db = new DbClass("www.wolfinv.com", "sa", "bolts", "pk10db");
        }




        public ChargeOperator(SerializationInfo info, StreamingContext context)
        {
            DbClass db = new DbClass("www.wolfinv.com", "sa", "bolts", "pk10db");
        }

        public void Charge(string wxId,string wxName,string reqId,string chargeAmt)
        {
            try
            {
                OperateChargeForm.Invoke(wxId,wxName, reqId, chargeAmt);
            }
            catch(Exception ce)
            {
                throw ce;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }
    }

}

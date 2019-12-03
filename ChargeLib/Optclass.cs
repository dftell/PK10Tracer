using System;
using System.Collections.Generic;
using System.Linq;
using WolfInv.com.DbAccessLib;

namespace WolfInv.com.ChargeLib
{
    [Serializable]
    public class Optclass
    {
        public bool responsed = false;
        public string reqId = null;
        public string strAmt = null;
        public string strWxId = null;
        public string strWxName = null;
        public string strChargeAccount = null;
        public string ResponseString = null;
         
        public Action<string> Response;
        public Optclass()
        {
            //db = new DbClass("www.wolfinv.com", "sa", "bolts", "pk10db");
        }

        public void receivedData(string a)
        {
            ResponseString = a;
            responsed = true;

        }

        public void receivedData1(string a)
        {
            DbClass db = new DbClass();
            string ret = null;
            string[] arr = a.Split('&');
            Dictionary<string, string> dic = arr.ToDictionary(b => b.Split('=')[0], b => b.Substring(b.Split('=')[0].Length + 1));
            List<string> jsonlist = new List<string>();
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
            string insql = "insert into userchargetable(chargeid,wxid,wxname,chargeamt,ordernum,imgurl,ChargeAccount) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
            int cnt = db.ExecSql(new ConditionSql(string.Format(insql, reqId, strWxId, strWxName, strAmt, strnum, strimg, strChargeAccount)));
            this.responsed = true;
            //Response(ret);
            //tcpClientSocket = null;
        }

    }

}

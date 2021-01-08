using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using System.Data;
using WolfInv.com.LogLib;
using WolfInv.com.BaseObjectsLib;
using System.Timers;
using System.ComponentModel;
using System.Runtime.Serialization;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.ExchangeLib
{
     [Serializable]
     public class ExchangeDataTable : DataTable
    {
        Int64 Eindex = 0;
        double Odds = 0;
        public ExchangeDataTable()//纯粹为支持反序列化而建，从不调用
        {

            //Odds = 9.75;
            //InitColumns();
        }
        protected ExchangeDataTable(SerializationInfo info, StreamingContext context):base(info,context)
        {
            
            ////Odds = 9.75;
            ////InitColumns();
            ////DataTable dt = info.GetValue("this",typeof(DataTable)) as DataTable;
            ////this.Rows.Add(dt.Select());
            //base.GetObjectData(info, context);
            //Value = info.GetBoolean("Test_Value");
        }
        public ExchangeDataTable(double odds):base()
        {
            Odds = odds;
            InitColumns();
        }

        void InitColumns()
        {
            string cols = "Id:Int64,ChanceCode,ExpectNo,InStatus,Amount:Double,EndExpectNO,EndPrice:Double,EndStatus,Odds:Double,Chips:Int32,Cost:Double,ExExpectNo,ExecRate,OccurStrag,Gained:Double,Profit:Double,CreateTime,Closed:Int32,UpdateTime,StragId,UserId";
            string[] colArr = cols.Split(',');
            for (int i = 0; i < colArr.Length; i++)
            {
                string[] arr = colArr[i].Split(':');
                Type type = typeof(String);
                if (arr.Length>1)
                {
                    string strType = arr[1];
                    Type tmp = Type.GetType(string.Format("System.{0}",strType), false);
                    if(tmp != null)
                    {
                        type = tmp;
                    }
                }
                this.Columns.Add(arr[0],type);
            }
        }

        public ExchangeChance<T> AddAChance<T>(ExchangeChance<T> ec) where T : TimeSerialData
        {
            DataRow dr = this.NewRow();
            Eindex++;
            ec.Id = Eindex;
            ec.OwnerChance.exchangeId = ec.Id;
            dr["Id"] = Eindex;
            dr["ExpectNo"] = ec.ExpectNo ;
            ChanceClass<T> cc = ec.OwnerChance;
            dr["ChanceCode"] = ec.OwnerChance.ChanceCode;
            dr["Chips"] = ec.OwnerChance.ChipCount;
            dr["Odds"] = ec.OccurStrag.CommSetting.Odds;
            dr["Amount"] = cc.UnitCost;
            dr["ExecRate"] = ec.ExchangeRate;
            dr["Cost"] = cc.UnitCost*cc.ChipCount;
            dr["CreateTime"] = DateTime.Now.ToString();
            dr["Closed"] = 0;
            Rows.Add(dr);
            return ec;
        }

        public ExchangeChance<T> AddSecurityChance<T>(ExchangeChance<T> ec) where T : TimeSerialData
        {
            DataRow dr = this.NewRow();
            Eindex++;
            SecurityChance<T> cc = ec.OwnerChance as SecurityChance<T>;
            ec.Id = Eindex;
            ec.OwnerChance.exchangeId = ec.Id;
            dr["Id"] = Eindex;
            dr["ExpectNo"] = ec.ExpectNo;
            dr["ExExpectNo"] = cc.exchangeExpect;
            dr["ChanceCode"] = ec.OwnerChance.ChanceCode;
            dr["Chips"] = ec.OwnerChance.ChipCount;
            dr["Odds"] = ec.OccurStrag.CommSetting.Odds;
            dr["Amount"] = cc.openPrice;
            dr["ExecRate"] = ec.ExchangeRate;
            dr["Cost"] = cc.openPrice * cc.ChipCount;
            dr["CreateTime"] = DateTime.Now.ToString();
            dr["InStatus"] = cc.inputStatus;
            dr["Closed"] = 0;
            Rows.Add(dr);
            return ec;
        }

        public bool UpdateChance<T>(ExchangeChance<T> ec,out double Gained) where T:TimeSerialData
        {
            string sqlmodule = "{0}='{1}'";
            Gained = 0;
            string sql = string.Format(sqlmodule, "Id", ec.Id);
            DataRow[] drs = this.Select(sql,"");
            if (drs.Length < 1)
            {
                //throw new Exception("需要更新的下注Id不存在");
                return false;
            }
            DataRow dr = drs[0];
            if (ec.OwnerChance.Odds == 0)
            {
                ec.OwnerChance.Odds = double.Parse(dr["Odds"].ToString());
            }
            double CurrOdds = ec.OwnerChance.getRealOdds(); //double.Parse(dr["Odds"].ToString());
            double dGained = CurrOdds * ec.ExchangeAmount;
            double dCost = (double)ec.Cost;
            Gained = 0;
            if (ec.MatchChips > 0)
            {
                Gained = dGained;
            }
            dr["Gained"] = Gained;
            dr["Profit"] = dGained - dCost;
            dr["UpdateTime"] = DateTime.Now.ToString();
            dr["Closed"] = ec.OwnerChance.Closed?1:0;
            return true;
        }

        public bool UpdateSecurityChance<T>(ExchangeChance<T> ec, out double Gained,string status) where T : TimeSerialData
        {
            string sqlmodule = "{0}='{1}'";
            Gained = 0;
            string sql = string.Format(sqlmodule, "Id", ec.OwnerChance.exchangeId);
            DataRow[] drs = this.Select(sql, "");
            if (drs.Length < 1)
            {
                //throw new Exception("需要更新的下注Id不存在");
                return false;
            }
            DataRow dr = drs[0];
            SecurityChance<T> cc = ec.OwnerChance as SecurityChance<T>;
            double usePrice = cc.closePrice ?? cc.currUnitPrice;
            double dGained = cc.ChipCount * usePrice;
            double dCost = cc.openPrice*cc.ChipCount;
            //Gained = 0;
            //if (ec.MatchChips > 0)
            //{
                Gained = dGained;
            //}
            
            double rate = 100*(usePrice - cc.openPrice) / cc.openPrice;
            dr["Odds"] = rate;            
            dr["EndExpectNO"] = cc.EndExpectNo??ec.UpdateExpectNo; //如果没有结束期就以更新期为准
            dr["EndPrice"] = usePrice;//如果没有结束价就以当前价为准
            dr["Gained"] = dGained;
            dr["Profit"] = dGained-dCost;
            dr["UpdateTime"] = ec.UpdateExpectNo;
            dr["Closed"] = cc.Closed ? 1 : 0;
            dr["EndStatus"] = cc.endStatus??status;
            return true;
        }

        public bool ModifyChance<T>(ExchangeChance<T> ecc) where T:TimeSerialData
        {
            return false;
        }
    }

}

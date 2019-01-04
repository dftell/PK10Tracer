using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using Strags;
using System.Data;
using LogLib;
using BaseObjectsLib;
using System.Timers;
using System.ComponentModel;
namespace ExchangeLib
{
     [Serializable]
     public class ExchangeDataTable : DataTable
     {
        Int64 Eindex = 0;
        double Odds = 0;
        public ExchangeDataTable()
        {
        }
        
        public ExchangeDataTable(double odds)
        {
            Odds = odds;
            InitColumns();
        }

        void InitColumns()
        {
            string cols = "Id,ExpectNo,ChanceCode,OccurStrag,Odds,Chips,Amount,ExecRate,Cost,Gained,Profit,CreateTime,UpdateTime,StragId,UserId";
            string[] colArr = cols.Split(',');
            for (int i = 0; i < colArr.Length; i++)
            {
                this.Columns.Add(colArr[i]);
            }
        }

        public ExchangeChance AddAChance(ExchangeChance ec)
        {
            DataRow dr = this.NewRow();
            Eindex++;
            ec.Id = Eindex;
            dr["Id"] = Eindex;
            dr["ExpectNo"] = int.Parse(ec.ExpectNo) ;
            dr["ChanceCode"] = ec.OwnerChance.ChanceCode;
            dr["Chips"] = ec.OwnerChance.ChipCount;
            dr["Odds"] = ec.OccurStrag.CommSetting.Odds;
            dr["Amount"] = ec.ExchangeAmount;
            dr["ExecRate"] = ec.ExchangeRate;
            dr["Cost"] = ec.Cost;
            dr["CreateTime"] = DateTime.Now.ToString();
            Rows.Add(dr);
            return ec;
        }

        public bool UpdateChance(ExchangeChance ec,out double Gained)
        {
            string sqlmodule = "{0}='{1}'";
            string sql = string.Format(sqlmodule, "Id", ec.Id);
            DataRow[] drs = this.Select(sql,"");
            if (drs.Length != 1)
                throw new  Exception("需要更新的下注Id不存在");
            DataRow dr = drs[0];
            double CurrOdds = double.Parse(dr["Odds"].ToString());
            double dGained = CurrOdds * ec.ExchangeAmount * ec.MatchChips;
            double dCost = (double)ec.Cost;
            Gained = 0;
            if (ec.MatchChips > 0)
            {
                Gained = dGained;
            }
            dr["Gained"] = dGained;
            dr["Profit"] = dGained - dCost;
            dr["UpdateTime"] = DateTime.Now.ToString();
            return true;
        }
    }

}

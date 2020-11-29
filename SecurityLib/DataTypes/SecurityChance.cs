using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class SecurityChance<T>:ChanceClass<T> where T:TimeSerialData
    {
        public override bool Matched(ExpectData<T> data)
        {
            return base.Matched(data);
        }

        public override bool Matched(ExpectData<T> data, out int MatchCnt)
        {
            MatchCnt = 0;
            return false;
        }

        public override bool Matched(ExpectData<T> data, out int MatchCnt, bool getRev)
        {
            MatchCnt = 0;
            return false;
        }

        public override bool Matched(ExpectList<T> data, out int MatchCnt)
        {
            MatchCnt = 0;
            return false;
        }

        public override bool Matched(ExpectList<T> el, out int MatchCnt, bool getRev)
        {
            MatchCnt = 0;
            return false;
        }
        //股票当期价值
        public double currUnitPrice;

        public override void CalcProfit(double matchcnt)
        {
            this.Gained = matchcnt * this.ChipCount; //最后价格*数量
            this.Profit = this.Gained - this.Cost;
        }

        public override double getRealOdds()
        {
            return 1;
        }
    }
}

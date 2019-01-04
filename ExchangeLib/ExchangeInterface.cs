using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using Strags;
namespace ExchangeLib
{
    ////public class ExchangeInterface
    ////{
    ////    public bool Save(List<ExchanceClass> ccs)
    ////    { return true; }
    ////}

    public class ExchangeChance:MarshalByRefObject
    {
        public Int64 Id;
        ChanceClass _cc;
        StragClass _sc;
        ExchangeService es;
        public ExchangeService Server { get { return es; } }
        string _InExpect;
        public string ExpectNo
        {
            get
            {
                return _InExpect;
            }
        }

        public ChanceClass OwnerChance
        {
            get
            {
                return _cc;
            }
        }

        public StragClass OccurStrag
        {
            get
            {
                return _sc;
            }
        }

        public ExchangeChance(ExchangeService _es,StragClass sc, string CurrExpectNo, ChanceClass cc)
        {
            _cc = cc;
            _sc = sc;
            _InExpect = CurrExpectNo;
            es = _es;
        }

        public Int64 ExchangeAmount;
        public int MatchChips;
        public double ExchangeRate;
        public Int64 Cost
        {
            get
            {
                return this.ExchangeAmount * this.OwnerChance.ChipCount;
            }
        }
        public double Gained;
        public double Profit
        {
            get
            {
                return Gained - Cost;
            }
        }

        

    }
    
}

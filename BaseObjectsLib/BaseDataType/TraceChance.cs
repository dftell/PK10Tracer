namespace WolfInv.com.BaseObjectsLib
{
    public abstract class TraceChance<T> : ChanceClass<T>, ITraceChance<T> where T : TimeSerialData
    {
        public virtual bool CheckNeedEndTheChance(ChanceClass<T> cc, bool LastExpectMatched)
        {
            if (this.MatchChips > 0)//如果命中，即关闭
            {
                return true;
            }
            return false;
        }

        //public abstract long getChipAmount(double RestCash, ChanceClass<T> cc, AmoutSerials amts);

        bool _IsTracing;
        
////////bool ITraceChance<T>.CheckNeedEndTheChance(ChanceClass<T> cc, bool LastExpectMatched)
////////{
////////    throw new NotImplementedException();
////////}

        public bool IsTracing
        {
            get
            {
                return _IsTracing;
            }
            set
            {
                _IsTracing = value;
            }
        }

        
        //public abstract bool IsTracing { get; set; }

        //public abstract bool CheckNeedEndTheChance(ChanceClass<T> cc, bool LastExpectMatched) ;
        public abstract double getChipAmount(double RestCash, ChanceClass<T> cc, AmoutSerials amts) ;
    }


    
}

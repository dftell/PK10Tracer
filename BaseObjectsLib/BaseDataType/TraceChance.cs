namespace WolfInv.com.BaseObjectsLib
{
    public abstract class TraceChance<T> : ChanceClass<T>, ITraceChance where T : TimeSerialData
    {
        public bool CheckNeedEndTheChance(ChanceClass<T> cc, bool LastExpectMatched)
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

        public abstract bool CheckNeedEndTheChance<T1>(ChanceClass<T1> cc, bool LastExpectMatched) where T1 : TimeSerialData;
        public abstract long getChipAmount<T1>(double RestCash, ChanceClass<T1> cc, AmoutSerials amts) where T1 : TimeSerialData;
    }


    
}

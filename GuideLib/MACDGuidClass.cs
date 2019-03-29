namespace WolfInv.com.GuideLib
{
    public enum MACDType { DIFF, DEA, MACD }

    /// <summary>
    /// MACD指标类
    /// </summary>
    public class MACDGuidClass : MutliReturnValueGuidClass
    {
        int iL=26, iS=12, iN=9, iIO=3;
        protected override void InitClass()
        {
            GuidName = "MACD";
            strParamStyle = "MACD_L={0};MACD_S={1};MACD_N={2};MACD_IO={3};";
        }
        public MACDGuidClass()
        {
            InitClass();
        }
        public MACDGuidClass(int N, int M1, int M2)
        {
            InitClass();
            iL = N;
            iS = M1;
            iN = M2;
        }
        
        public override string getParamString()
        {
            string sCycle = cycle.ToString().Substring(0,1);
            string sPriceAdj = priceAdj.ToString().Substring(0, 1);
            return string.Format(strParamStyle, iL, iS, iN, getIOValue());
        }

        protected override int getIOValue()
        {
            return this.ReturnValueName == "DIFF" ? 1 : this.ReturnValueName == "DEA" ? 2 : 3;
        }
    }

}


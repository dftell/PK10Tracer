namespace WolfInv.com.GuideLib
{
    /// <summary>
    /// KDJ指标类
    /// </summary>
    public class KDJGuidClass : MutliReturnValueGuidClass
    {
        int iN=9, iM1=3, iM2=3, iIO=3;
        protected override void InitClass()
        {
            GuidName = "KDJ";
            strParamStyle = "KDJ_N={0};KDJ_M1={1};KDJ_M2={2};KDJ_IO={3}";
        }
        public KDJGuidClass()
        {
            InitClass();
        }
        public KDJGuidClass(int N, int M1, int M2)
        {
            InitClass();
            iN = N;
            iM1 = M1;
            iM2 = M2;

        }
        public override string getParamString()
        {
            return string.Format(strParamStyle,iN,iM1,iM2,getIOValue());
        }

        protected override int getIOValue()
        {
            return this.ReturnValueName == "K" ? 1 : this.ReturnValueName == "D" ? 2 : 3;
        }
    }

}


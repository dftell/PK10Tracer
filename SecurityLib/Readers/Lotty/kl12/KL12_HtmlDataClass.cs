using System;
using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
using System.Collections;
namespace WolfInv.com.SecurityLib
{
    public class KL12_HtmlDataClass : Web52CP_HtmlDataClass
    {
        public KL12_HtmlDataClass(DataTypePoint dp) : base(dp)
        {

        }
    }
    
    [Serializable]
    public class Web52CP_KL12_DataClass : JsonableClass<Web52CP_KL12_DataClass>
    {
        public int status = 0;
        public List<Web52CP_Lotty_KL12_DataClass> result;
    }
    [Serializable]
    public class Web52CP_Lotty_KL12_DataClass: Web52CP_Lotty_DataClass
    {
        
        public int sum_val;
        public string sum_ds;
        public string sum_dx;
        public string lh;
        public string pre3;
        public string cen3;
        public string aft3;
        public string num1;
        public string num2;
        public string num3;
        public string num4;
        public string num5;
        public string num6;
        public string num7;
        public string num8;
        public string num9;
        public string num10;
        //public string num11;
        //public string num12;专门为PK10准备的，所以没有11和12？
        public string sum_tail_dx;
        public string sum_tail_ds;
    }

    
    public class Web52CP_KL11_DataClass : JsonableClass<Web52CP_KL11_DataClass>
    {
        public int status = 0;
        public List<Web52CP_Lotty_KL11_DataClass> result;
    }
    [Serializable]
    public class Web52CP_Lotty_KL11_DataClass : Web52CP_Lotty_KL12_DataClass
    {

    }

}


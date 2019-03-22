using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
namespace WolfInv.com.SecurityLib
{

    public class XDXRData:MongoData,ICodeData,IDateData
    {
        public int category { get; set; }
        public string category_meaning { get; set; }
        public string code { get; set; }
        public string date{ get; set; }
        public double fenhong{ get; set; }
        public string fenshu{ get; set; }
        public string liquidity_after{ get; set; }
        public string liquidity_before{ get; set; }
        public string name{ get; set; }
        public double peigu { get; set; }
        public double peigujia { get; set; }
        public string shares_after{ get; set; }
        public string shares_before{ get; set; }
        public double songzhuangu { get; set; }
        public string suogu{ get; set; }
        public string xingquanjia { get; set; }
    }
}

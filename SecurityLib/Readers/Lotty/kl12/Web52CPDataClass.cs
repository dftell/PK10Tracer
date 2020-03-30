using System;
using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
using System.Data;
using System.Net;
namespace WolfInv.com.SecurityLib
{
    //{"status":1,"result":[{"expect":20190722032,"opentime":1563793897,"week":1,"expect_ds":0,"issue":32,
    //"num":"3,9,1,11,12","sum_val":36,"sum_ds":"双",
    //"sum_dx":"大","lh":"虎","pre3":"杂六","cen3":"杂六",
    //"aft3":"半顺","num1":"3","num2":"9","num3":"1",
    //"num4":"11","num5":"12","sum_tail_dx":"尾大","sum_tail_ds":"尾双"},
    [Serializable]
    public class Web52CPDataClass:JsonableClass<Web52CPDataClass>
    {
        public int status { get; set; }
        public List<Web52CP_Lotty_DataClass> result;
    }


    [Serializable]
    public class Web52CP_Lotty_DataClass : JsonableClass<Web52CP_Lotty_DataClass>
    {
        public string expect;
        public long opentime;
        public int week;
        public int expect_ds;
        public int issue;
        public string num;
    }
    
    public interface iConvertToDataSet
    {
        DataTable ConvertToData(string strHtml, DataTypePoint dtp,DataTable oldtable=null);
    }

    public interface iGenerateUrl
    {
        List<string> generateUrl(DataTypePoint dtp);
    }

    
}


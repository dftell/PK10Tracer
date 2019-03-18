using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace CFZQ_LHProcess
{
    public abstract class GuidBaseClass
    {
        protected string strParamStyle;
        public DateTime tradeDate;
        public String GuidName;
        public Cycle cycle;
        public PriceAdj priceAdj;
        protected string _strParam;
        public string strParam{get{return getParamString();}}
        public abstract string getParamString();
    }
    //周期
    public enum Cycle
    {
        Day,Week,Month,Quarter,SemiYear,Year
    }
    //复权方式
    public enum PriceAdj
    {
        UnDo,Fore,Beyond,Target
    }

    public class GuidBuilder
    {
        GuidBaseClass gbc;
        public GuidBuilder(GuidBaseClass guidClass)
        {
            gbc = guidClass;
        }
        public DataTable getRecords(string[] Sectors,DateTime dt)
        {
            if (gbc == null) return null;
            gbc.tradeDate = dt;
            WSSClass wsetobj;
            wsetobj = new WSSClass(string.Join(",",Sectors),gbc.GuidName, gbc.strParam);
            return WDDataAdapter.getRecords(wsetobj.getDataSet());
        }
    }

    public class KDJClass : GuidBaseClass
    {
        int iN, iM1, iM2, iIO;
        void InitClass()
        {
            GuidName = "KDJ";
            strParamStyle = "tradeDate={0};KDJ_N={1};KDJ_M1={2};KDJ_M2={3};KDJ_IO={4};priceAdj={5};cycle={6}";
        }
        public KDJClass()
        {
            InitClass();
        }
        public KDJClass(int N, int M1, int M2, string OutType)
        {
            InitClass();
            iN = N;
            iM1 = M1;
            iM2 = M2;
            iIO = OutType=="K"?1:(OutType=="D"?2:3);//K:1,D:2,J,other:3
        }
        public override string getParamString()
        {
            string sCycle = cycle.ToString().Substring(0, 1);
            string sPriceAdj = priceAdj.ToString().Substring(0, 1);
            return string.Format(strParamStyle,tradeDate.ToShortDateString().Replace("-",""),iN,iM1,iM2,iIO,sPriceAdj,sCycle);
        }
    }
}


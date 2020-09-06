using System;
using WAPIWrapperCSharp;
namespace WolfInv.com.CFZQ_LHProcess
{
    public class WSETClass : WDDateReadClass, iWDReader
    {
        public string SecCode;
        public DateTime Date;
        string sType = "sectorconstituent";//sectorconstituent
        
        #region 构造函数
        public WSETClass(WindAPI w):base(w)
        {
            strType = sType;
        }

        public WSETClass(WindAPI w,string strCode):base(w)
        {
            strType = sType;
            SecCode = strCode;
            Date = DateTime.Today;
        }
        public WSETClass(WindAPI w, string strCode, DateTime dt)
            : base(w)
        {
            strType = sType;
            SecCode = strCode;
            Date = dt;
        }

        public WSETClass(WindAPI w, string _Type, string strCode, DateTime dt)
            : base(w)
        {
            strType = _Type;
            SecCode = strCode;
            Date = dt;
        }
        #endregion

        #region iWDReader 成员

        public WindData getDataSet()
        {
            InitConnect();
            string strExecInfo = string.Format(strInfo, SecCode, Date.ToString("yyyy-MM-dd"));
            WindData wd =  w.wset(strType, strExecInfo);
            //if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public Int64 getDataSetCount()
        {
            //WindAPI w = new WindAPI();
            //w.start();
            string strExecInfo = string.Format(strInfo, SecCode, Date.ToShortDateString());
            WindData wd = w.wset(strType, strExecInfo);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            if (wd.codeList == null) return 0;
            return wd.codeList.Length;
        }

        #endregion

    }

}

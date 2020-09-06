using System;
using WAPIWrapperCSharp;
namespace WolfInv.com.CFZQ_LHProcess
{
    /// <summary>
    /// 万得日期序列数据类
    /// </summary>
    public class WSDClass : WDReadClass,iWDReader
    {
        string _SecCodes;
        string _Fields;
        DateTime _BegT, _EndT;
        string _Params;
        static WSDClass()
        {
            //WindAPI w = new WindAPI();
            //w.start();
        }

        public WSDClass(WindAPI w, string SecCodes, string Fields, DateTime BegT, DateTime EndT, string strParams)
            : base(w)
        {
            _Fields = Fields;
            _SecCodes = SecCodes;
            _BegT = BegT;
            _EndT = EndT;
            _Params = strParams;
        }

        #region iWDReader 成员

        public WindData getDataSet()
        {
            InitConnect();
            WindData wd = w.wsd(_SecCodes, _Fields, _BegT.ToString("yyyy-MM-dd"), _EndT.ToString("yyyy-MM-dd"),_Params);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public Int64 getDataSetCount()
        {
            InitConnect();
            WindData wd = w.wsd(_SecCodes, _Fields, _BegT.ToShortDateString(), _EndT.ToShortDateString(), _Params);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd.codeList.Length;
        }
        #endregion

    }

}

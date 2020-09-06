using System;
using System.Collections.Generic;
using WAPIWrapperCSharp;
using System.Linq;
namespace WolfInv.com.CFZQ_LHProcess
{
    public class WSSClass : WDReadClass,iWDReader
    {
        public string _SecCodes;
        public string _Fields;
        public string _Params ;
        string fs
        {
            get
            {
                if (_SelctFields != null && _SelctFields.Count > 0)
                {
                    return string.Join(",", _SelctFields.ToArray<string>());
                }
                return null;
            }
        }

        HashSet<string> _SelctFields;
        public WSSClass(WindAPI _w) : base(_w) { }
        public WSSClass(WindAPI _w,string SecCodes, string Fields):base(_w)
        {
            _Fields = Fields;
            _SecCodes = SecCodes;
            _Params = "";
        }

        public WSSClass(WindAPI _w, string SecCodes, HashSet<string> Fields)
            : base(_w)
        {
            _SelctFields = Fields;
            _SecCodes = SecCodes;
            _Params = "";
        }

        public WSSClass(WindAPI _w, string SecCodes, string Fields, string Params)
            : base(_w)
        {
            _Fields = Fields;
            _SecCodes = SecCodes;
            _Params = Params;
        }

        public WSSClass(WindAPI w, string SecCodes, HashSet<string> Fields, string Params)
            : base(w)
        {
            _SelctFields = Fields;
            _SecCodes = SecCodes;
            _Params = Params;
        }

        #region iWDReader 成员

        public WindData getDataSet()
        {
            WindData wd = w.wss(_SecCodes, fs ?? _Fields, _Params);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public Int64 getDataSetCount()
        {
            WindData wd = w.wss(_SecCodes,fs ?? _Fields, _Params);
            if (wd.errorCode!=0) throw (new WDErrorException(w, wd.errorCode));
            return wd.codeList.Length;
        }

        #endregion

    }

}

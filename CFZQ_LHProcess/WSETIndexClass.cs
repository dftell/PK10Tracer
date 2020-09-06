using System;
using WAPIWrapperCSharp;
namespace WolfInv.com.CFZQ_LHProcess
{
    /// <summary>
    /// 万得指数集合类（可查权重)
    /// </summary>
    public class WSETIndexClass : WSETClass
    {
        string _strInfo = "date={1};windcode={0};field=wind_code,sec_name";
        public WSETIndexClass(WindAPI w)
            : base(w)
        {
            strInfo = _strInfo;
        }

        public WSETIndexClass(WindAPI w,string code, bool needWeight = false)
            : base(w,code)
        {
            strInfo = _strInfo;
            if (needWeight)
                strInfo += ",i_weight";
        }

        public WSETIndexClass(WindAPI w,string code, DateTime dt,bool needWeight=false)
            : base(w,code, dt)
        {
            strInfo = _strInfo;
            if (needWeight)
                strInfo += ",i_weight";
        }

        public WSETIndexClass(WindAPI w,string _type,string code, DateTime dt, bool needWeight = false)
            : base(w,_type,code, dt)
        {
            strInfo = _strInfo;
            if (needWeight)
                strInfo += ",i_weight";
        }
    }

}

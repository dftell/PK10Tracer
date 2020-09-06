using System;
using WAPIWrapperCSharp;
namespace WolfInv.com.CFZQ_LHProcess
{
    /// <summary>
    /// 万得板块集合类
    /// </summary>
    public class WSETMarketClass : WSETClass
    {
        string _strInfo = "date={1};sectorid={0};field=wind_code,sec_name";
        public WSETMarketClass(WindAPI w)
            : base(w)
        {
            strInfo = _strInfo;
        }

        public WSETMarketClass(WindAPI w,string code)
            : base(w,code)
        {
            strInfo = _strInfo;
        }

        public WSETMarketClass(WindAPI w,string code, DateTime dt)
            : base(w,code, dt)
        {
            strInfo = _strInfo;
        }
    }

}

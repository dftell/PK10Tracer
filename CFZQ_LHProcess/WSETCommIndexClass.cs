using System;
using WAPIWrapperCSharp;
namespace WolfInv.com.CFZQ_LHProcess
{
    /// <summary>
    /// 板块/指数集合类
    /// </summary>
    public class WSETCommIndexClass : WSETClass 
    {
        string _strInfo = "date={1};sectorid={0}";
        public WSETCommIndexClass(WindAPI w)
            : base(w)
        {
            strInfo = _strInfo;
        }

        public WSETCommIndexClass(WindAPI w,string code)
            : base(w,code)
        {
            strInfo = _strInfo;
        }

        public WSETCommIndexClass(WindAPI w,string code, DateTime dt)
            : base(w,code, dt)
        {
            strInfo = _strInfo;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.PK10CorePress
{
    /// <summary>
    /// 证券数据基类
    /// </summary>
    public class SecurityData:ExpectData
    {
        //数据时间
        public DateTime DataTime
        {
            get
            {
                return DateTime.FromBinary(long.Parse(this.Expect));
            }
        }

        public string StrTime
        {
            get
            {
                return DataTime.ToLongTimeString();
            }
        }

        public OneCycleData CurrData;
    }

    

    }

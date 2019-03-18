using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfInv.com.StrategyLibForWD
{
    /// <summary>
    /// 概念类
    /// </summary>
    public class GLClass 
    {
        public string GLCode;
        public string GLName;
        public int GLNumberCnt;
        public DateTime UpdateDate;
        public List<string> NumberList;
        public GLClass()
        {
            GLCode = "a39901012c000000";
        }

        public GLClass(string strCode)
        {
            if (strCode != null && strCode.Length >0 )
                GLCode = strCode;
            else
                GLCode = "a39901012c000000";
        }
    }

}

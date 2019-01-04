using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogLib;
using WinInterComminuteLib;

namespace ServerInitLib
{
   
    [Serializable]
    public abstract class OperateFactory : RemoteServerClass
    {
        public abstract ExeOperateDeleage ExecuteEvent { get; set; }
        public abstract GetTableDeleage GetTable{get;set;}
        public void Test(string strTest)
        {
            LogableClass.ToLog("服务端日志", "测试", strTest);

        }
    }

  
}

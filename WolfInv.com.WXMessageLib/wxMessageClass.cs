using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolfInv.com.WXMessageLib
{
    [Serializable]
    public class wxMessageClass
    {
        public string FromUserNam { get; set; }
        public string FromNikeName { get; set; }
        public string ToUserName { get; set; }
        public string ToNikeName { get; set; }
        public string FromMemberUserName { get; set; }
        public string FromMemberNikeName { get; set; }
        public string AtMemberUserName { get; set; }
        public string[] AtMemberNikeName { get; set; }
        public string Msg { get; set; }
        public int CreateTime { get; set; }

        public string OrgMsg { get; set; }

        public bool IsAtToMe { get; set; }
    }
}

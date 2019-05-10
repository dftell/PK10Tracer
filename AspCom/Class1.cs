using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace AspCom
{
    [Guid("E96F09F2-F5A7-4095-9096-14AF5B4EA5C9")]
    [ComVisible(true)]
    public interface IWXMsg
    {
        [DispId(0)]
        string SendMsg(string ToUser, string Msg);
    }
    [ComVisible(true)]
    [Guid("92E15BA8-3679-43DA-BF7B-E2A0C18DE2E7")]
    [ProgId("AspCom.WXMsg")]
    public class WXMsg : IWXMsg
    {
        string _msg;
        public WXMsg()
        {
            _msg = "inited";
        }

        public string SendMsg(string ToUser, string Msg)
        {
            return string.Format("{0}:{1}=>{2}", ToUser, _msg, Msg);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using WolfInv.com.WXMsgCom;
//using WolfInv.com.WinInterComminuteLib;
using System.Runtime.InteropServices;
namespace WXCommInterface
{
    [Guid("69DEC065-DF3E-48E7-8B74-E26361629854")]
    public interface IMsgInterface
    {
        [DispId(0)]
        string SendMsg(string ToUser, string msg);
    }

    //////public class MsgInterface : IMsgInterface
    //////{
    //////    WebInterfaceClass msgs;
    //////    public MsgInterface()
    //////    {
    //////        //LogableClass.ToLog("初始化Comm接口", "成功");
    //////    }

    //////    public string SendMsg(string ToUser,string msg)
    //////    {
    //////        //LogableClass.ToLog("发送消息", "准备");
    //////        if (msgs == null)
    //////        {
    //////            //LogableClass.ToLog("准备IPC通道", "准备");
    //////            WinComminuteClass wc = new WinComminuteClass();
    //////            msgs = wc.GetServerObject<WebInterfaceClass>("wxmsg");
    //////        }
    //////        if (msgs != null)
    //////            return msgs.SendMsg(msg, ToUser);
    //////        else
    //////        {
    //////            return "远程对象未启动";
    //////        }
            
    //////    }
    //////}

    public class MsgInterface_ForTest : IMsgInterface
    {
        string initStr = "";
        public MsgInterface_ForTest()
        {
            initStr = "初始化成功！";
        }
        public string SendMsg(string ToUser, string msg)
        {
            return "kjdldf" + initStr;
        }
    }
}

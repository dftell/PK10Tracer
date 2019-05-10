using System.Runtime.InteropServices;

namespace WolfInv.com.WXMsgCom
{
    [Guid("F59E3934-4287-4F24-BAEA-9F3221703A01"),
InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface WebInterface
    {
        [DispId(1)]
        string SendMsg(string str, string ToUser);
        [DispId(2)]
        string Start();
        [DispId(3)]
        void Stop();
        [DispId(4)]
        string Init();
        [DispId(5)]
        void SetDisplayMethod(bool bDisplayByFrm);
        [DispId(6)]
        bool Valid { get; }
    }
}
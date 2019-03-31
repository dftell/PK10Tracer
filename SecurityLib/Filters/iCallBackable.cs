using System.Collections.Generic;
using System.Linq;
using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{

    public delegate BaseDataTable CommCallBackData(string SecCode,int cnt);

    public interface iCommCallBackable
    {
       BaseDataTable GetData(int RecordCnt);
    }
}

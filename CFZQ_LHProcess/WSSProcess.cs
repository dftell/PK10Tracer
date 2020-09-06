using WAPIWrapperCSharp;
using System.Data;
namespace WolfInv.com.CFZQ_LHProcess
{
    public class WSSProcess
    {
        public DataTable getRecods(WindAPI w, string SecCodes, string Fields)
        {
            WSSClass wss = new WSSClass(w,SecCodes, Fields);
            return WDDataAdapter.getRecords(wss.getDataSet());
        }
    }

}

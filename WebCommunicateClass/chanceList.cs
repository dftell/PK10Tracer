using System.Web.Script.Serialization;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.WebCommunicateClass
{
    public class chanceList : ChanceClass
    {
        public override string ToString()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(this);
        }
    }
}

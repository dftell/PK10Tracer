using System.Web.Script.Serialization;
using WolfInv.com.PK10CorePress;
namespace WolfInv.com.WebCommunicateClass
{
    public class chanceList: ChanceClass
    {
        public override string ToString()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(this);
        }
    }
}

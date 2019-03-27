using System.Collections.Generic;
using System.Web.Script.Serialization;
namespace WolfInv.com.WebCommunicateClass
{
    public class FullInsts
    {
        public int Count;
        public List<chanceList> ChanceList;

        public override string ToString()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(this);
        }
    }
}

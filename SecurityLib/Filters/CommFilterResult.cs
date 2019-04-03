using System.Text;
namespace WolfInv.com.SecurityLib
{
    public class CommFilterResult
    {
        public bool Enalbe;
        public StringBuilder Msg;
        public decimal[] DisplayValues;
        public CommFilterResult()
        {
            Msg = new StringBuilder();
        }
    }
}

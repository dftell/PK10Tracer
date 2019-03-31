using System.Text;
namespace WolfInv.com.StrategyLibForWD
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

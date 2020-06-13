//using WolfInv.Com.SocketLib;

namespace ExchangeTermial
{
    public class RunAreaInfo //运行区间信息
    {
        public int StartIndex;
        public double StartValue;
        public int EndIndex;
        public double EndValue;
        public int Len;
        public int Weight;
        public double DrawDownRate;
        /// <summary>
        /// 是否是负回撤，如果为真是上涨，默认是回撤
        /// </summary>
        public bool IsRaise;
        public string Descript()
        {
            string ret = "长度:{0}轮，包含次数:{1},幅度:{2}%;";
            return string.Format(ret,Len,Weight,100*DrawDownRate);
        }
    }

    
}

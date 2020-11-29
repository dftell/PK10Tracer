using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.GuideLib
{
    
    public class KLineData<T> where T : TimeSerialData
    {
        List<T> Data;
        public int Length;
        public int[] Indies;
        public string[] Expects;
        public double[] Closes;
        public double[] Opens;
        public double[] Highs;
        public double[] Lows;
        public double[] Vols;
        public KLineData(List<T> list) 
        {
            Data = list;
            if (list == null)
                return;
            Length = list.Count;
            Indies = new int[list.Count];
            Expects = new string[list.Count];
            Closes = new double[list.Count];
            Opens = new double[list.Count];
            Highs = new double[list.Count];
            Lows = new double[list.Count];
            Vols = new double[list.Count];
            dynamic rx = 1;
            for (int i = list.Count-1;i>=0 ; i--)
            {
                StockMongoData smd = list[i] as StockMongoData;
                
                Indies[i] = i;
                if(i<list.Count-1)
                {
                    StockMongoData psmd = list[i+1] as StockMongoData;
                    rx = rx * psmd.preclose/smd.close;
                }
                Expects[i] = smd.Expect;
                Closes[i] = (smd.close * rx);
                Closes[i] = Closes[i].ToEquitPrice(2);
                Opens[i] = (smd.open * rx);
                Opens[i] = Opens[i].ToEquitPrice(2);
                Highs[i] = (smd.high * rx);
                Highs[i] = Highs[i].ToEquitPrice(2);
                Lows[i] = (smd.low * rx);
                Lows[i] = Lows[i].ToEquitPrice(2);
                Vols[i] = (smd.vol / rx);
                Vols[i] = Vols[i].ToEquitPrice(0);
            }

        }

        public KLineData<T> Ref(int N)
        {
            
            if(Data.Count<N)
            {
                return new KLineData<T>(new List<T>());
            }
            KLineData<T> ret = new KLineData<T>(Data.Take(Length-N).ToList());
            return ret;

        }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuideLib
{
    public class MAClass:GuideClass
    {
        public MAClass(Dictionary<string, double> data)
            : base(data)
        {
        }
        public override GuideResultSet GetResult(params int[] inParams)
        {
            if (inParams.Length != 1)
            {
                return null;
            }
            GuideResultSet ret = this.BaseSet.Copy() as GuideResultSet;
            GuideResult gr = ret.NewTable("MA");
            GuideResult bt = ret.Tables[0] as GuideResult;
            List<double> lines = bt.getColumnData();
            int N = inParams[0];
            double firstVal = 0;
            double sum = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (i <= N)
                {
                    gr.Fill(i, lines[i]);
                    sum += lines[i];
                    firstVal = lines[0];
                    continue;
                }
                sum = sum - firstVal + lines[i];
                gr.Fill(i, sum / N);
                firstVal = lines[i - N];
            }
            return ret;
        }

        
    }
}

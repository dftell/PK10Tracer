using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.MachineLearnLib;
namespace WolfInv.com.MaxEntClass
{
    public class Entropy <LabelT, FeatureT> 
    {

        List<MLInstance<LabelT, FeatureT>> Instances;
        List<FeatureT> features;
        public Entropy()
        {

        }

        public void FillInstances(List<MLInstance<LabelT,FeatureT>> insts)
        {
            Instances = insts;
        }

        public double getValue(List<MLInstance<LabelT,FeatureT>> insts,FeatureT totalValue)
        {
            ////for(int i=0;i< insts.Count;i++)
            ////{
            ////    MLInstance<LabelT, FeatureT> mli = insts[i];
            ////    double probF = (double)((mli.Feature)/(totalValue));
            ////    return probF;
            ////}
            return 0;
        }
    }

    
}

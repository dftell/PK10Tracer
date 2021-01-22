using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using WolfInv.com.GuideLib.LinkGuid;
using WolfInv.com.SecurityLib.Filters.StrategyFilters;

namespace WolfInv.com.SecurityLib.Strategies.Bussyniess
{
    public class FirstPointFilter_Logic_Strategy<T> : CommStrategyBaseClass<T> where T:TimeSerialData
    {
        public FirstPointFilter_Logic_Strategy(CommDataIntface<T> _w) : base(_w)
        {
        }



        public override SelectResult BalanceSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override SelectResult BreachSelectSecurity(CommStrategyInClass Input)
        {
            throw new NotImplementedException();
        }

        public override BaseDataTable ReadSecuritySerialData(string Code)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 反转选股逻辑
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public override SelectResult ReverseSelectSecurity(CommStrategyInClass Input)
        {
            SelectResult ret = new SelectResult();
            if(!SelectTable.ContainsKey(Input.SecIndex))
            {
                return ret;
            }
            if(SelectTable[Input.SecIndex] == null)
            {
                return ret;
            }
            MongoReturnDataList<T> pricesInfo = SelectTable[Input.SecIndex];
            CommSecurityProcessClass<T> cspc = new CommSecurityProcessClass<T>(pricesInfo.SecInfo, pricesInfo);
            CommFilterLogicBaseClass<T> filter = new MinDaysFilter<T>(Input.EndExpect, cspc);
            try
            {
                ret.Key = Input.SecIndex;
                ret.Enable = false;
                
                //filter.EndExpect = Input.EndExpect;
                ret = filter.ExecFilter(Input);
                if (!ret.Enable)
                    return ret;
                filter = new IsSTFilter<T>(Input.EndExpect, cspc);
                ret = filter.ExecFilter(Input);
                if (!ret.Enable)
                    return ret;
                filter = new MutliCycle_MACDBottomRevFilter<T>(Input.EndExpect, cspc,PriceAdj.Fore,Input.useMaxCycle);
                ret = filter.ExecFilter(Input);
                if (!ret.Enable)
                    return ret;
                return ret;
            }
            catch(Exception ce)
            {
                return ret;
            }
            finally
            {
                pricesInfo = null;
                filter = null;
                cspc = null;
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CFZQ_LHProcess;
using WAPIWrapperCSharp;
namespace StrategyLibForWD
{
    public class secIndexBuilder:WDBuilder
    {
        SecIndexClass _sic = null;
        public secIndexBuilder(WindAPI _w, SecIndexClass sic):base(_w)
        {
            _sic = sic;
        }

        /// <summary>
        /// 获得该指数下所有证券清单
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public MTable getBkList(DateTime dt)
        {
            //WSETCommIndexClass wsetobj;
            WSETClass wsetobj;
            if (_sic.IndexCode == null)
                wsetobj = new WSETCommIndexClass(w, _sic.SummaryCode, dt);
            else
            {
                if (_sic.IndexCode.IndexOf(".") > 0)//指数成分类
                {
                    wsetobj = new WSETIndexClass(w, _sic.IndexCode, dt);
                }
                else //板块成分类
                {
                    wsetobj = new WSETMarketClass(w, _sic.IndexCode, dt);
                }
            }
            return WDDataAdapter.getTable(wsetobj.getDataSet());
        }

        /// <summary>
        /// 获得该指数下所有证券权重清单
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public MTable getBkWeight(DateTime dt)
        {
            //WSETCommIndexClass wsetobj;
            WSETClass wsetobj;
            if (_sic.IndexCode == null) //如果未指定代码，一定是板块类
                wsetobj = new WSETCommIndexClass(w, _sic.SummaryCode, dt);
            else
            {
                 wsetobj = new WSETIndexClass(w, "indexconstituent", _sic.IndexCode, dt);
            }
            return WDDataAdapter.getTable(wsetobj.getDataSet(),"i_weight",typeof(float));
        }

        //////public MTable getBkbyDate(string indexname, DateTime EndT)
        //////{
        //////    SecIndexClass sic = new SecIndexClass(w,indexname);
        //////    secIndexBuilder sib = new secIndexBuilder(sic);
        //////    MTable ret = sib.getBkList(EndT);
        //////    return ret;
        //////}
        //////public MTable getBkWeightbyDate(string indexname, DateTime EndT)
        //////{
        //////    SecIndexClass sic = new SecIndexClass(w, indexname);
        //////    secIndexBuilder sib = new secIndexBuilder(sic);
        //////    MTable ret = sib.getBkWeight(EndT);
        //////    return ret;
        //////}
        
    }
}

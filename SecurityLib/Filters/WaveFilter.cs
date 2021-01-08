using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.GuideLib;
using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.SecurityLib.Filters
{
    public interface iMACDFilter<T> where T : TimeSerialData
    {
        /// <summary>
        /// 在MACD红色区，上涨区
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <returns></returns>
        bool CurrInRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays);
        /// <summary>
        /// 在参考点近的红色区
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <param name="currStatusHoldDays"></param>
        /// <returns></returns>
        bool CurrInNearRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays);
        /// <summary>
        /// 在参考点远的红色区
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <param name="currStatusHoldDays"></param>
        /// <returns></returns>
        bool CurrInFarRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays);

        /// <summary>
        /// 在多峰值红色区
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <param name="currStatusHoldDays"></param>
        /// <param name="humps"></param>
        /// <param name="isNear"></param>
        /// <returns></returns>
        bool CurrInMultiHumpRedArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays, MaxMinElementClass<double>[] humps,bool isNear);
        /// <summary>
        /// 在MACD绿柱区
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <returns></returns>
        bool CurrInGreen(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays);
        /// <summary>
        /// 在离参考点近处的绿色区
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <param name="currStatusHoldDays"></param>
        /// <returns></returns>
        bool CurrInNearGreenArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays);
        /// <summary>
        /// 在离参考点远的绿色区
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <param name="currStatusHoldDays"></param>
        /// <returns></returns>
        bool CurrInFarGreenArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays);
        /// <summary>
        /// 在多峰值绿柱区
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="kline"></param>
        /// <param name="macds"></param>
        /// <param name="holdDays"></param>
        /// <param name="currStatusHoldDays"></param>
        /// <param name="humps"></param>
        /// <param name="isNear"></param>
        /// <returns></returns>
        bool CurrInMultiHumpGreenArea(CommStrategyInClass Input, KLineData<T> kline, MACDCollection macds, int holdDays, int currStatusHoldDays, MaxMinElementClass<double>[] humps, bool isNear);
    }
}

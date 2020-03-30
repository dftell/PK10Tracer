using System;
using System.Collections.Generic;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public interface IDataReader
    {
        ExpectList<T> GetMissedData<T>(bool IsHistoryData, string strBegT) where T : TimeSerialData;
        ExpectList<T> getNewestData<T>(ExpectList<T> NewestData, ExpectList<T> ExistData) where T : TimeSerialData;
        DbChanceList<T> getNoCloseChances<T>(string strDataOwner) where T : TimeSerialData;
        DbChanceList<T> getClosedChances<T>(string strDataOwner, int PassedDays) where T : TimeSerialData;
        ExpectList<T> ReadHistory<T>() where T : TimeSerialData;
        ExpectList<T> ReadHistory<T>(long buffs) where T : TimeSerialData;
        ExpectList<T> ReadHistory<T>(long From, long buffs) where T : TimeSerialData;
        ExpectList<T> ReadHistory<T>(long From, long buffs, bool desc) where T : TimeSerialData;
        ExpectList<T> ReadHistory<T>(string begt, string endt) where T : TimeSerialData;
        ExpectList<T> ReadNewestData<T>(DateTime fromdate) where T : TimeSerialData;
        ExpectList<T> ReadNewestData<T>(int LastLng) where T : TimeSerialData;
        ExpectList<T> ReadNewestData<T>(long ExpectNo, int Cnt) where T : TimeSerialData;
        ExpectList<T> ReadNewestData<T>(long ExpectNo, int Cnt, bool FromHistoryTable) where T : TimeSerialData;
        int SaveChances<T>(List<ChanceClass<T>> list, string strDataOwner) where T : TimeSerialData;
        int SaveHistoryData<T>(ExpectList<T> InData) where T : TimeSerialData;
        int SaveNewestData<T>(ExpectList<T> InData) where T : TimeSerialData;

        /// <summary>
        /// 专门针对外部中间数据用，此类序列数据均来源与外部，已初步计算出中间结果，最后提供给本系统策略最终使用
        /// </summary>
        /// <param name="dtp">数据点，包括外部数据要调用的格式</param>
        /// <param name="expectNo">期号，使用前先判断数据是否已是最新期号，如果不是就放弃？如外部数据产生时间与本地数据不同步，如何保证？？？所以，建议所使用彩种最好是序列数据及外部数据来源与同一个数据源，这样最能保证一致。</param>
        /// <returns></returns>
        DataSet ReadExData(DataTypePoint dtp, string expectNo, Func<DataTypePoint, string, System.Data.DataSet> ConvertFunc);
    }


}
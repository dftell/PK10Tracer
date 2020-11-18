using System;
using System.Collections.Generic;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public interface IDataReader<T> where T:TimeSerialData
    {
        ExpectList<T> GetMissedData(bool IsHistoryData, string strBegT);
        ExpectList<T> getNewestData(ExpectList<T> NewestData, ExpectList<T> ExistData);
        DbChanceList<T> getNoCloseChances(string strDataOwner);
        DbChanceList<T> getClosedChances(string strDataOwner, int PassedDays);
        ExpectList<T> ReadHistory();
        ExpectList<T> ReadHistory(long buffs);
        ExpectList<T> ReadHistory(long From, long buffs);
        ExpectList<T> ReadHistory(long From, long buffs, bool desc);
        ExpectList<T> ReadHistory(string begt, string endt) ;
        ExpectList<T> ReadNewestData(DateTime fromdate) ;
        ExpectList<T> ReadNewestData(int LastLng) ;
        ExpectList<T> ReadNewestData(long ExpectNo, int Cnt) ;
        ExpectList<T> ReadNewestData(long ExpectNo, int Cnt, bool FromHistoryTable) ;
        ExpectList<T> ReadNewestData(string ExpectNo, int Cnt, bool FromHistoryTable) ;
        int SaveChances(List<ChanceClass<T>> list, string strDataOwner) ;
        int SaveHistoryData(ExpectList<T> InData) ;
        int SaveNewestData(ExpectList<T> InData) ;

        /// <summary>
        /// 专门针对外部中间数据用，此类序列数据均来源与外部，已初步计算出中间结果，最后提供给本系统策略最终使用
        /// </summary>
        /// <param name="dtp">数据点，包括外部数据要调用的格式</param>
        /// <param name="expectNo">期号，使用前先判断数据是否已是最新期号，如果不是就放弃？如外部数据产生时间与本地数据不同步，如何保证？？？所以，建议所使用彩种最好是序列数据及外部数据来源与同一个数据源，这样最能保证一致。</param>
        /// <returns></returns>
        DataSet ReadExData(DataTypePoint dtp, string expectNo, Func<DataTypePoint, string, System.Data.DataSet> ConvertFunc);
    }


}
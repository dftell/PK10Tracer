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
        ExpectList<T> ReadNewestData<T>(int ExpectNo, int Cnt) where T : TimeSerialData;
        ExpectList<T> ReadNewestData<T>(int ExpectNo, int Cnt, bool FromHistoryTable) where T : TimeSerialData;
        int SaveChances<T>(List<ChanceClass<T>> list, string strDataOwner) where T : TimeSerialData;
        int SaveHistoryData<T>(ExpectList<T> InData) where T : TimeSerialData;
        int SaveNewestData<T>(ExpectList<T> InData) where T : TimeSerialData;
    }


}
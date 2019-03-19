using System;
using System.Collections.Generic;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public interface IDataReader
    {
        ExpectList GetMissedData(bool IsHistoryData, string strBegT);
        ExpectList getNewestData(ExpectList NewestData, ExpectList ExistData);
        DbChanceList getNoCloseChances(string strDataOwner);
        ExpectList ReadHistory();
        ExpectList ReadHistory(long buffs);
        ExpectList ReadHistory(long From, long buffs);
        ExpectList ReadHistory(long From, long buffs, bool desc);
        ExpectList ReadHistory(string begt, string endt);
        ExpectList ReadNewestData(DateTime fromdate);
        ExpectList ReadNewestData(int LastLng);
        ExpectList ReadNewestData(int ExpectNo, int Cnt);
        ExpectList ReadNewestData(int ExpectNo, int Cnt, bool FromHistoryTable);
        int SaveChances(List<ChanceClass> list, string strDataOwner);
        int SaveHistoryData(ExpectList InData);
        int SaveNewestData(ExpectList InData);
    }


}
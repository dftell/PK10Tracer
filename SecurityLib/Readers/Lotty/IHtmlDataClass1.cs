using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.SecurityLib
{
    public interface IHtmlDataClass
    {
        ExpectList<T> getData<T>(string strHtml) where T : TimeSerialData;
        ExpectList<T> getExpectList<T>() where T : TimeSerialData;
        ExpectList<T> getHisData<T>(string strHtml) where T : TimeSerialData;
        ExpectList<T> getHistoryData<T>(string strDate, int pageid) where T : TimeSerialData;
        ExpectList<T> getHistoryData<T>(string FolderPath, string filetype) where T : TimeSerialData;
        ExpectList<T> getXmlData<T>(string strXml) where T : TimeSerialData;
        ExpectList<T> getJsonData<T>(string strXml) where T : TimeSerialData;
        ExpectList<T> getTextData<T>(string strXml) where T : TimeSerialData;
    }
}
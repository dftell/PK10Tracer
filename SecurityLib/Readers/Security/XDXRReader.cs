using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class XDXRReader : DateSerialCodeDataReader
    {
        public XDXRReader(string db, string docname, string[] codes) : base(db, docname, codes)
        {
        }

        public override MongoDataDictionary<XDXRData> GetAllCodeDateSerialDataList<XDXRData>(bool DateAsc)
        {
            MongoDataDictionary<XDXRData> ret = new MongoDataDictionary<XDXRData>();
            MongoReturnDataList<XDXRData> list = (builder as DateSerialCodeDataBuilder).getData<XDXRData>(DateAsc);
            return DataListConverter<XDXRData>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<XDXRData> GetAllCodeDateSerialDataList<XDXRData>(string begT, bool DateAsc)
        {
            MongoDataDictionary<XDXRData> ret = new MongoDataDictionary<XDXRData>();
            MongoReturnDataList<XDXRData> list = (builder as DateSerialCodeDataBuilder).getData<XDXRData>(begT,DateAsc);
            return DataListConverter<XDXRData>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<XDXRData> GetAllCodeDateSerialDataList<XDXRData>(string begT, string EndT, bool DateAsc)
        {
            MongoDataDictionary<XDXRData> ret = new MongoDataDictionary<XDXRData>();
            MongoReturnDataList<XDXRData> list = (builder as DateSerialCodeDataBuilder).getData<XDXRData>(begT, EndT, DateAsc);
            return DataListConverter<XDXRData>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<XDXRData> GetAllCodeDateSerialDataList<XDXRData>(string endT, int Cnt, bool DateAsc)
        {
            MongoDataDictionary<XDXRData> ret = new MongoDataDictionary<XDXRData>();
            MongoReturnDataList<XDXRData> list = (builder as DateSerialCodeDataBuilder).getData<XDXRData>(endT,Cnt,DateAsc);
            return DataListConverter<XDXRData>.ToDirectionary(list, "code");
        }
    }
}

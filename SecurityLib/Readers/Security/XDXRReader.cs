using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class XDXRReader : DateSerialCodeDataReader
    {
        public XDXRReader(string db, string docname, string[] codes) : base(db, docname, codes)
        {
        }

        public override MongoDataDictionary GetAllCodeDateSerialDataList(bool DateAsc)
        {
            MongoDataDictionary ret = new MongoDataDictionary();
            List< XDXRData> list = (builder as DateSerialCodeDataBuilder).getData<XDXRData>(DateAsc);
            return DataListConverter.ToDirectionary<XDXRData>(list, "code");
        }

        public override MongoDataDictionary GetAllCodeDateSerialDataList(string begT, bool DateAsc)
        {
            MongoDataDictionary ret = new MongoDataDictionary();
            List<XDXRData> list = (builder as DateSerialCodeDataBuilder).getData<XDXRData>(begT,DateAsc);
            return DataListConverter.ToDirectionary<XDXRData>(list, "code");
        }

        public override MongoDataDictionary GetAllCodeDateSerialDataList(string begT, string EndT, bool DateAsc)
        {
            MongoDataDictionary ret = new MongoDataDictionary();
            List<XDXRData> list = (builder as DateSerialCodeDataBuilder).getData<XDXRData>(begT, EndT, DateAsc);
            return DataListConverter.ToDirectionary<XDXRData>(list, "code");
        }

        public override MongoDataDictionary GetAllCodeDateSerialDataList(string endT, int Cnt, bool DateAsc)
        {
            MongoDataDictionary ret = new MongoDataDictionary();
            List<XDXRData> list = (builder as DateSerialCodeDataBuilder).getData<XDXRData>(endT,Cnt,DateAsc);
            return DataListConverter.ToDirectionary<XDXRData>(list, "code");
        }
    }
}

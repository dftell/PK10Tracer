using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
using System;
using WolfInv.com.LogLib;
using System.Data;

namespace WolfInv.com.SecurityLib
{
    public class XDXRReader : DateSerialCodeDataReader
    {
        public XDXRReader() 
        {
        }
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

        public override DataSet ReadExData(DataTypePoint dtp, string expectNo, Func<DataTypePoint, string, System.Data.DataSet> ConvertFunc)
        {
                    throw new NotImplementedException();
        }

        public Func<string, string, MongoReturnDataList<XDXRData>> getXDXRList = delegate (string DataType, string code)
        {
            DataTypePoint dtp = GlobalClass.TypeDataPoints[DataType];
            MongoReturnDataList<XDXRData> list = null;
            if (dtp.RuntimeInfo.XDXRList.ContainsKey(code))//如果缓存中存在
            {
                list = dtp.RuntimeInfo.XDXRList[code];
                if (list != null)//并且不为空，返回缓存数据
                    return list;
            }
            XDXRReader reader = new XDXRReader(DataType, dtp.XDXRTable, new string[] { code });
            list = reader.GetAllCodeDateSerialDataList<XDXRData>(true)?[code];
            if (list == null)
            {
                LogableClass.ToLog("获取除权除息数据错误");
                return list;
            }
            dtp.RuntimeInfo.XDXRList[code] = list;
            LogableClass.ToLog("获取到除权除息数据", string.Format("股票数：{0}；总条数：{1}", list.Count, list.Count));
            return list;
        };

    }
}

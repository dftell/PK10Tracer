using WolfInv.com.BaseObjectsLib;
using WolfInv.com.DbAccessLib;
using System.Collections.Generic;
using MongoDB.Driver;
using System;

namespace WolfInv.com.SecurityLib
{

    public class SecurityReader: DateSerialCodeDataReader,ICodeDataList, IGetAllTimeSerialList
    {
        public SecurityReader(string db, string docname, string[] codes) : base(db, docname, codes)
        {
        }

        public MongoReturnDataList<T> GetAllCodeDataList<T>(bool IncludeStoped=false) where T : MongoData
        {
            builder = new CodeDataBuilder(this.DbTypeName,this.TableName, null);
            MongoReturnDataList<T> ret = (builder as CodeDataBuilder).getData<T>(IncludeStoped);
            return ret;
        }

        public MongoReturnDataList<T> GetAllCodeDataList<T>() where T : MongoData
        {
            return GetAllCodeDataList<T>(false);
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(bool DateAsc= true)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(DateAsc);   //.getData<T>(DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT, bool DateAsc = true)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(begT, DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT, string EndT, bool DateAsc = true) 
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(begT, EndT, DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string endT, int Cnt, bool DateAsc = true)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(endT, Cnt, DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }


        public MongoReturnDataList<T> GetAllTimeSerialList<T>() where T : MongoData
        {
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getFullTimeSerial<T>();
            return list;
        }

        

        public MongoReturnDataList<StockMongoData> Stock_FQ(string code,MongoReturnDataList<StockMongoData> org_bfq_data=null, MongoReturnDataList<XDXRData> org_xdxr_data = null, PriceAdj fqType = PriceAdj.Beyond) 
        {
            if (org_bfq_data == null)
                org_bfq_data = new SecurityReader(base.DbTypeName, base.TableName, new string[] { code }).GetAllCodeDateSerialDataList<StockMongoData>()?[code];
            if (org_xdxr_data == null)
                org_xdxr_data = new XDXRReader().getXDXRList(DbTypeName, code);
            MongoReturnDataList<XDXRData> xdxr_data = org_xdxr_data.Copy();
            MongoReturnDataList<StockMongoData> bfq_data = org_bfq_data.Copy();
            MongoReturnDataList<StockMongoData> ret = bfq_data;
            if (bfq_data.Count == 0) return ret;
            if (xdxr_data.Count == 0) return ret;
            MongoReturnDataList<XDXRData> info = xdxr_data.Query<int>("category", 1);
            if (info.Count > 0)
            {
                ret = MongoReturnDataList<StockMongoData>.Concat<XDXRData,string>(
                    bfq_data, 
                    info,
                    a=>a.date,
                    a=>a.date,
                    (s,a,b)=> {
                    XDXRData ExObj = s.ExtentData as XDXRData;
                    if(ExObj== null)
                        ExObj = new XDXRData();
                    ExObj.date = s.date;
                    if(b)
                    {
                        ExObj.category = a.category;
                    }
                    s.ExtentData = ExObj;
                });
                ret.FillNa<int, XDXRData>(
                    a => a.ExtentData as XDXRData,
                a => (a.ExtentData as XDXRData).category??0,
                (a, b, c) => 
                {
                    XDXRData data = a.ExtentData as XDXRData;
                    if(data == null)
                       data = new XDXRData();
                    data.date = a.date;
                    data.category = b?.category;
                    a.ExtentData = data;
                }
                ,FillType.FFill);
                    //(a, b) =>
                    //{
                    //    return (a.ExtentData as XDXRData).category;
                    //}, (a, b) => { a.ExtentData = b; }, FillType.FFill);
                ret = MongoReturnDataList<StockMongoData>.Concat<XDXRData,string>(ret, info,
                    a=>a.date,
                    a=>a.date,
                    (s, a, b) => {
                    XDXRData ExObj = s.ExtentData as XDXRData;
                    if (ExObj == null)
                        ExObj = new XDXRData();
                    ExObj.date = s.date;
                    if (b)
                    {
                        ExObj.fenhong = a.fenhong;
                        ExObj.peigu = a.peigu;
                        ExObj.peigujia = a.peigujia;
                        ExObj.songzhuangu = a.songzhuangu;
                    }
                    s.ExtentData = ExObj;
                });
                //ret.FillNa({ return a => a.ExtentData; }, "category", FillType.FFill);
            }
            return ret;
        }

        Func<StockMongoData, XDXRData> func = delegate (StockMongoData ac)
        {
            return ac.ExtentData as XDXRData;
         };

        
        /*
         def _QA_data_stock_to_fq(bfq_data, xdxr_data, fqtype):
    '使用数据库数据进行复权'
    info = xdxr_data.query('category==1')
    bfq_data = bfq_data.assign(if_trade=1)

    if len(info) > 0:
        data = pd.concat(
            [
                bfq_data,
                info.loc[bfq_data.index[0]:bfq_data.index[-1],
                         ['category']]
            ],
            axis=1
        )

        data['if_trade'].fillna(value=0, inplace=True)
        data = data.fillna(method='ffill')

        data = pd.concat(
            [
                data,
                info.loc[bfq_data.index[0]:bfq_data.index[-1],
                         ['fenhong',
                          'peigu',
                          'peigujia',
                          'songzhuangu']]
            ],
            axis=1
        )
    else:
        data = pd.concat(
            [
                bfq_data,
                info.
                loc[:,
                    ['category',
                     'fenhong',
                     'peigu',
                     'peigujia',
                     'songzhuangu']]
            ],
            axis=1
        )
    data = data.fillna(0)
    data['preclose'] = (
        data['close'].shift(1) * 10 - data['fenhong'] +
        data['peigu'] * data['peigujia']
    ) / (10 + data['peigu'] + data['songzhuangu'])

    if fqtype in ['01', 'qfq']:
        data['adj'] = (data['preclose'].shift(-1) /
                       data['close']).fillna(1)[::-1].cumprod()
    else:
        data['adj'] = (data['close'] /
                       data['preclose'].shift(-1)).cumprod().shift(1).fillna(1)

    for col in ['open', 'high', 'low', 'close', 'preclose']:
        data[col] = data[col] * data['adj']
    data['volume'] = data['volume'] / \
        data['adj'] if 'volume' in data.columns else data['vol']/data['adj']
    try:
        data['high_limit'] = data['high_limit'] * data['adj']
        data['low_limit'] = data['high_limit'] * data['adj']
    except:
        pass
    return data.query('if_trade==1 and open != 0').drop(
        ['fenhong',
         'peigu',
         'peigujia',
         'songzhuangu',
         'if_trade',
         'category'],
        axis=1,
        errors='ignore'
    )
*/
    }
}

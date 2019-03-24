using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
namespace WolfInv.com.SecurityLib
{

    public class SecurityReader: DateSerialCodeDataReader,ICodeDataList, IGetAllTimeSerialList
    {
        public SecurityReader(string db, string docname, string[] codes) : base(db, docname, codes)
        {
        }

        public MongoReturnDataList<T> GetAllCodeDataList<T>(bool IncludeStoped) where T : class, new()
        {
            builder = new CodeDataBuilder(this.DbTypeName,this.TableName, null);
            MongoReturnDataList<T> ret = (builder as CodeDataBuilder).getData<T>(IncludeStoped);
            return ret;
        }

        public MongoReturnDataList<T> GetAllCodeDataList<T>() where T : class, new()
        {
            return GetAllCodeDataList<T>(false);
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(bool DateAsc)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(DateAsc);   //.getData<T>(DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT, bool DateAsc)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(begT, DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string begT, string EndT, bool DateAsc)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(begT, EndT, DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList<T>(string endT, int Cnt, bool DateAsc)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(endT, Cnt, DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }


        public MongoReturnDataList<T> GetAllTimeSerialList<T>() where T : class, new()
        {
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getFullTimeSerial<T>();
            return list;
        }

        public static void Stock_FQ<T>(double[] bfq_data, MongoDataList xdxrData, PriceAdj fqType) where T : class, new()
        {
            
        }
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

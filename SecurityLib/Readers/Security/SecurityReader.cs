using WolfInv.com.BaseObjectsLib;
using WolfInv.com.DbAccessLib;
using System.Collections.Generic;
using MongoDB.Driver;
using System;
using WolfInv.com.ProbMathLib;
using System.Data;
using WolfInv.com.WDDataInit;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public delegate void CheckFinishedCnt(int total,int grpcnt,int FinishedGrpCnt,int InPoolCnt,int FinishedCnt); 

    public class StockReader: SecurityReader<StockMongoData>
    {
        public StockReader(string type) : base(type)
        { }
    }
    public class SecurityReader<T>: DateSerialCodeDataReader<T>,ICodeDataList<T>, IGetAllTimeSerialList<T> where T:TimeSerialData
    {
        public SecurityReader(string type):base(type)
        { }
        public SecurityReader(string db, string docname, string[] codes=null) : base(db, docname, codes)
        {
        }

        public MongoReturnDataList<T> GetAllCodeDataList(bool IncludeStoped=false)
        {
            builder = new CodeDataBuilder(this.DbTypeName,this.TableName, null);
            MongoReturnDataList<T> ret = (builder as CodeDataBuilder).getData<T>(IncludeStoped);
            return ret;
        }

        public MongoReturnDataList<T> GetAllCodeDataList()
        {
            return GetAllCodeDataList(false);
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList(bool DateAsc= true)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(DateAsc);   //.getData(DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList(string begT, bool DateAsc = true)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(begT, DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList(string begT, string EndT, bool DateAsc = true) 
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(begT, EndT, DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }

        public override MongoDataDictionary<T> GetAllCodeDateSerialDataList(string endT, int Cnt, bool DateAsc = true)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getData<T>(endT, Cnt, DateAsc);
            return DataListConverter<T>.ToDirectionary(list, "code");
        }


        public MongoReturnDataList<T> GetAllTimeSerialList() 
        {
            MongoReturnDataList<T> list = (builder as DateSerialCodeDataBuilder).getFullTimeSerial<T>();
            return list;
        }

        public MongoReturnDataList<T> GetAllVaildList(string[] sqls)
        {
            string[] ret = null;
            MongoReturnDataList<T> retdata = (builder as DateSerialCodeDataBuilder).getDataGroupBy<T>(sqls);
            return retdata;
        }

        public MongoDataDictionary<StockMongoData> Stock_FQ(MongoDataDictionary<StockMongoData> orgData, MongoDataDictionary<XDXRData> XData =null, PriceAdj fqType = PriceAdj.Fore)
        {
            MongoDataDictionary<StockMongoData> ret = new MongoDataDictionary<StockMongoData>();
            foreach(string key in orgData.Keys)
            {
                MongoReturnDataList<StockMongoData> val = orgData[key];
                MongoReturnDataList<XDXRData> xval = null;
                if(XData != null)
                {
                    if(XData.ContainsKey(key))
                    {
                        xval = XData[key];
                    }
                }
                if (!ret.ContainsKey(key))
                {
                    ret.Add(key,Stock_FQ(key, val, xval, fqType));
                }
            }
            return ret;
        }

        public MongoDataDictionary<StockMongoData> FQ(MongoDataDictionary<StockMongoData> orgData, MongoDataDictionary<XDXRData> xdata, PriceAdj fqType = PriceAdj.Fore)
        {
            MongoDataDictionary<StockMongoData> ret = new MongoDataDictionary<StockMongoData>();

            foreach (string key in orgData.Keys)
            {
                MongoReturnDataList<StockMongoData> val = orgData[key];
                MongoReturnDataList<XDXRData> xval = xdata[key];                
                if (!ret.ContainsKey(key))
                {
                    ret.Add(key, Stock_FQ(key, val, xval, fqType));
                }
            }
            return ret;
        }

        public MongoReturnDataList<StockMongoData> Stock_FQ(string code,MongoReturnDataList<StockMongoData> org_bfq_data=null, MongoReturnDataList<XDXRData> org_xdxr_data = null, PriceAdj fqType = PriceAdj.Fore) 
        {
            /*
             1、一般的股票交易软件中，都有计算复权的功能。股票行情有除权与复权，在计算股票涨跌幅时采用复权价格，这是经常要用到的。系统计算分为以下步骤：
①每次除权行为日，在登记日计算除权价；
除权价=（除权前一日收盘价+配股价X配股比率－每股派息）/（1+配股比率+送股比率）
②用登记日的收盘价除以除权价得到单次除权因子；
除权因子=收盘价/除权价
③将公司上市以来的除权因子连乘积，得到对应每一交易日的除权因子；
④向后复权价=股票收盘价（实际交易价）*每一交易日的除权因子；
⑤复权涨幅（不特别说明，涨幅均指复权涨幅）=复权价/前一日复权价-1。
2、所谓复权就是对股价和成交量进行权息修复，按照股票的实际涨跌绘制股价走势图,并把成交量调整为相同的股本口径。 
             */
            if (org_bfq_data == null)
                org_bfq_data = new SecurityReader<StockMongoData>(base.DbTypeName, base.TableName, new string[] { code }).GetAllCodeDateSerialDataList()?[code];
            if (org_xdxr_data == null)
                org_xdxr_data = new XDXRReader().getXDXRList(DbTypeName, code);
            MongoReturnDataList<XDXRData> xdxr_data = org_xdxr_data.Copy(false);
            MongoReturnDataList<StockMongoData> bfq_data = org_bfq_data.Copy(false);
            MongoReturnDataList<StockMongoData> ret = bfq_data;
            if (bfq_data.Count == 0) return ret;
            if (xdxr_data.Count == 0) return ret;
            MongoReturnDataList<XDXRData> info = xdxr_data.Query<int>("category", 1);
            MongoReturnDataList<StockMongoData> retList = null;
            List <StockMongoData> list = null;
            #region category = 1
            if (info.Count > 0)
            {
                list = Pandas.Concat<StockMongoData,XDXRData, string>(
                    ret, 
                    info,
                    a=>a.date,
                    a=>a.date,
                    (s,a,b)=> {
                    XDXRData ExObj = s.ExtentData() as XDXRData;
                    if(ExObj== null)
                        ExObj = new XDXRData();
                    if(b)
                    {
                        ExObj.date = s.date;
                        ExObj.category = a.category;
                    }
                    s.setExtentData(ExObj);
                });
                var testlist = list;
                //retList = new MongoReturnDataList<StockMongoData>(list);

                list = Pandas.FillNa<StockMongoData,string, XDXRData>(list,
                    a => a.ExtentData() as XDXRData,
                a => (a.ExtentData() as XDXRData).date,
                (a, b, c) => 
                {
                    XDXRData data = a.ExtentData() as XDXRData;
                    if(data == null)
                       data = new XDXRData();
                    //data.date = a.date;
                    data.category = b.category??0;
                    a.setExtentData ( data);
                }
                ,FillType.FFill);
                //retList = new MongoReturnDataList<StockMongoData>(list);
                //(a, b) =>
                //{
                //    return (a.ExtentData as XDXRData).category;
                //}, (a, b) => { a.ExtentData = b; }, FillType.FFill);
                list = Pandas.Concat<StockMongoData, XDXRData,string>(new MongoReturnDataList<StockMongoData>(list), info,
                    a=>a.date,
                    a=>a.date,
                    (s, a, b) => {
                    XDXRData ExObj = s.ExtentData() as XDXRData;
                    if (ExObj == null)
                        ExObj = new XDXRData();
                    //ExObj.date = s.date;
                    if (b)
                    {
                        ExObj.date = a.date;
                        ExObj.fenhong = a.fenhong;
                        ExObj.peigu = a.peigu;
                        ExObj.peigujia = a.peigujia;
                        ExObj.songzhuangu = a.songzhuangu;
                    }
                    s.setExtentData ( ExObj);
                });
                //ret.FillNa({ return a => a.ExtentData; }, "category", FillType.FFill);
                //retList = new MongoReturnDataList<StockMongoData>(list);
            }
            #endregion
            #region 其他分红
            else 
            {
                list = Pandas.Concat<StockMongoData,XDXRData, string>(ret, info,
                    a => a.date,
                    a => a.date,
                    (s, a, b) => {
                        XDXRData ExObj = s.ExtentData() as XDXRData;
                        if (ExObj == null)
                            ExObj = new XDXRData();
                        //ExObj.date = s.date;
                        if (b)
                        {
                            ExObj.date = s.date;
                            ExObj.category = a.category;
                            ExObj.fenhong = a.fenhong;
                            ExObj.peigu = a.peigu;
                            ExObj.peigujia = a.peigujia;
                            ExObj.songzhuangu = a.songzhuangu;
                        }
                        s.setExtentData( ExObj);
                    });
            }
            #endregion
            list = Pandas.FillNa<StockMongoData,string, XDXRData>(list,
                    a => a.ExtentData() as XDXRData,
                a => (a.ExtentData() as XDXRData).date,
                (a, b, c) =>
                {
                    XDXRData ExObj = a.ExtentData() as XDXRData;
                    if (ExObj == null)
                        ExObj = new XDXRData();
                    ExObj.date = a.date;
                    ExObj.fenhong = 0;
                    ExObj.peigu = 0;
                    ExObj.peigujia = 0;
                    ExObj.songzhuangu = 0;
                }, FillType.None);
            ret = new MongoReturnDataList<StockMongoData>(list);
            int xscnt = GlobalClass.TypeDataPoints[base.DbTypeName].RuntimeInfo.SecurityInfoList[code].decimal_point;
            long Lbase = (long)Math.Pow(10, xscnt);
            List<long?> CloseList = ret.ToList(a => (long?)(a.close*Lbase));
            List<long?> closes = Pandas.ShiftX<long?>(CloseList, 1);
            List<double?> xdxrcloses = new List<double?>();
            for(int i=0;i<ret.Count;i++)
            {
                XDXRData xdata = ret[i].ExtentData() as XDXRData;
                
                double midval = ((closes[i]??0) * 10 - xdata.fenhong.Value*Lbase + xdata.peigu.Value * xdata.peigujia.Value*Lbase)/(10+xdata.peigu.Value+xdata.songzhuangu.Value);
                //long xdprice = Math.Round(midval);
                xdxrcloses.Add(midval);
            }
            //precloses = Pandas.ShiftX<double>(precloses, -1);
            List<double?> ProAdjList = new List<double?>();
            
            List<double?> ppclose = Pandas.ShiftX<double?>(xdxrcloses, -1);//
            for (int i=0;i< ppclose.Count;i++)
            {
                if(ppclose[i] == null)
                {
                    ProAdjList.Add(1);
                    continue;
                }
                //double? midval = (double?)(1*((double)ppclose[i].Value)/ ((double)CloseList[i]) / 1);
                double? midval = (double?)(1 * ((double)ppclose[i].Value) / ((double)CloseList[i]) /   1);
                ProAdjList.Add((double?)midval);
                
            }
            List<double?> adj = new List<double?>();
            if(fqType == PriceAdj.Fore)
            {
                adj = ProAdjList;
                adj = Pandas.Recovert<double?>(adj);
                adj = Pandas.Cumprod(adj);
                adj = Pandas.Recovert<double?>(adj);
                double? curr = adj[adj.Count - 1];
                if (curr.Value != 1)
                {
                    //adj.ForEach(a => a = a / curr);
                    for (int i = 0; i < adj.Count; i++)
                    {
                        adj[i] /= curr;
                    }
                }
                //adj = Pandas.Recovert<double?>(adj);
            }
            else
            {
                adj = Pandas.Cumprod(ProAdjList);
                Pandas.ShiftX<double?>(adj, -1);
            }
            for(int i=0;i<ret.Count;i++)
            {
                ret[i].close *= adj[i].Value;
                ret[i].open *= adj[i].Value;
                ret[i].high *= adj[i].Value;
                ret[i].low *= adj[i].Value;
                ret[i].vol /= adj[i].Value;
            }
            return ret;
        }

        public override int DeleteChanceByIndex(long index, string strDataOwner = null)
        {
            throw new NotImplementedException();
        }

        public override int DeleteExpectData(string expectid)
        {
            throw new NotImplementedException();
        }

        public override void ExecProduce(string Procs)
        {
            throw new NotImplementedException();
        }

        public override DataSet ReadExData(DataTypePoint dtp, string expectNo, Func<DataTypePoint, string, System.Data.DataSet> ConvertFunc)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> ReadHistory(long cnt, string endExpect)
        {
            throw new NotImplementedException();
        }

        Func<StockMongoData, XDXRData> func = delegate (StockMongoData ac)
        {
            return ac.ExtentData() as XDXRData;
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

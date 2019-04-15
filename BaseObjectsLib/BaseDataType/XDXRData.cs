using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
namespace WolfInv.com.BaseObjectsLib
{
    ////////category = {
    ////////        '1': '除权除息', '2': '送配股上市', '3': '非流通股上市', '4': '未知股本变动', '5': '股本变化',
    ////////        '6': '增发新股', '7': '股份回购', '8': '增发新股上市', '9': '转配股上市', '10': '可转债上市',
    ////////        '11': '扩缩股', '12': '非流通股缩股', '13':  '送认购权证', '14': '送认沽权证'}
    /// <summary>
    /// def QA_data_calc_marketvalue(data, xdxr):
    /// 
    /*
    '使用数据库数据计算复权'
    mv = xdxr.query('category!=6').loc[:,
                                       ['shares_after',
                                        'liquidity_after']].dropna()
    res = pd.concat([data, mv], axis=1)

    res = res.assign(
        shares=res.shares_after.fillna(method='ffill'),
        lshares=res.liquidity_after.fillna(method='ffill')
    )
    return res.assign(mv=res.close* res.shares*10000, liquidity_mv= res.close * res.lshares * 10000).drop(['shares_after', 'liquidity_after'], axis= 1)\
            .loc[(slice(data.index.remove_unused_levels().levels[0][0], data.index.remove_unused_levels().levels[0][-1]), slice(None)),:]
*/
    /// </summary>

    public class XDXRData:MongoData,ICodeData,IDateData,IDateStampData
    {
        public int? category { get; set; }
        public string category_meaning { get; set; }
        public string code { get; set; }
        public string date{ get; set; }

        public double date_stamp { get; set; }
        public double? fenhong{ get; set; }
        public double? fenshu { get; set; }
        public double? liquidity_after { get; set; }
        public double? liquidity_before { get; set; }
        public string name{ get; set; }
        public double? peigu { get; set; }
        public double? peigujia { get; set; }
        public double? shares_after { get; set; }
        public double? shares_before { get; set; }
        public double? songzhuangu { get; set; }
        public double? suogu { get; set; }
        public double? xingquanjia { get; set; }

        public XDXRData()
        {

        }
    }

    
}

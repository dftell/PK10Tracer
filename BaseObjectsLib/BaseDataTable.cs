using System;
using System.Linq;
using System.Data;
namespace WolfInv.com.BaseObjectsLib
{
    public class BaseDataTable : MTable
    {
        public BaseDataTable()
        {
        }

        public BaseDataTable(DataTable dt) : base(dt)
        {
        }

        public BaseDataTable(MTable tb)
        {
            if (tb == null || tb.GetTable() == null)
            {
                return;
                throw (new Exception("数据异常！"));
            }
            this.Table = tb.GetTable().Copy();
        }

        public override iFillable this[int id]
        {
            get
            {
                if (id >= this.Table.Rows.Count) return new BaseDataItemClass();
                return new BaseDataItemClass(this.Table.Rows[id]);
            }
        }



        protected BaseDataTable _AvaliableData;//剔除非交易日期
        /// <summary>
        /// 交易日数据
        /// </summary>
        public BaseDataTable AvaliableData
        {
            get
            {
                if (_AvaliableData == null)
                {
                    if (this.Count > 0)
                    {
                        _AvaliableData = new BaseDataTable(this.tTable.Clone()); //必须要复制结构
                        var vv = from item in this.ToFillableList<BaseDataItemClass>()
                                 where item.ExchangeNormal
                                 select item;
                        BaseDataItemClass[] bics = vv.ToArray<BaseDataItemClass>();
                        _AvaliableData.FillByItems<BaseDataItemClass>(bics);
                    }
                }
                return _AvaliableData;
            }
        }

    }

}

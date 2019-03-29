using System.Collections.Generic;
namespace WolfInv.com.BaseObjectsLib
{
    public class MatrixTableRows<T>
    {
        public  MatrixTableRows()
        {
        }

        MatrixTableCell<T> _Cell;
        protected Dictionary<string, MatrixTableCell<T>> List;

        public MatrixTableCell<T> this[string colname]
        {
            get
            {
                return List[colname]; 
            }
            set
            {
                List[colname] = value;
            }
        }
    }
}

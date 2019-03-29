using System.Data;
namespace WolfInv.com.BaseObjectsLib
{
    public interface iListFillable
    {
        void FillByItems<T>(T[] item);
        T getObjectById<T>(int id);
    }

    public interface iFillable
    {
        void FillByDataRow(DataRow dr);
        DataRow FillRow(DataRow dr);

    }
}

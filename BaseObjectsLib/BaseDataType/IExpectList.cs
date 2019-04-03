using System.Collections.Generic;
using System.Data;

namespace WolfInv.com.BaseObjectsLib
{

    public interface IExpectList<T> where T: TimeSerialData 
    {
        ExpectData<T> this[int index] { get; set; }

        int Count { get; }
        ExpectData<T> FirstData { get; }
        bool IsReadOnly { get; }
        ExpectData<T> LastData { get; }
        DataTable Table { get; }

        void Add(ExpectData<T> item);
        void Clear();
        bool Contains(ExpectData<T> item);
        void CopyTo(ExpectData<T>[] array, int arrayIndex);
        ExpectList<T> FirstDatas(int RecLng);
        IEnumerator<ExpectData<T>> GetEnumerator();
        ExpectList<T> getSubArray(int FromIndex, int len);
        int IndexOf(ExpectData<T> item);
        int IndexOf(string ExpectNo);
        void Insert(int index, ExpectData<T> item);
        ExpectList<T> LastDatas(int RecLng);
        bool Remove(ExpectData<T> item);
        void RemoveAt(int index);
    }
}
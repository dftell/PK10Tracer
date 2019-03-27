using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.WebCommunicateClass
{
    public class AssetUnitList : RecordObject, IList<AssetUnitInfo>, iSerialJsonClass<mAssetUnitList>
    {
        List<AssetUnitInfo> list;
        public string Data;
        public mAssetUnitList List;
        int count;

        public mAssetUnitList getObjectByJsonString(string str)
        {

            mAssetUnitList ret = null;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ret = js.Deserialize<mAssetUnitList>(str);
            return ret;
        }

        public AssetUnitInfo this[int index] { get => ((IList<AssetUnitInfo>)list)[index]; set => ((IList<AssetUnitInfo>)list)[index] = value; }

        public int Count
        {
            get { return list.Count; }
            set { count = value; }
        }

        public bool IsReadOnly => ((IList<AssetUnitInfo>)list).IsReadOnly;

        public void Add(AssetUnitInfo item)
        {
            ((IList<AssetUnitInfo>)list).Add(item);
        }

        public void Clear()
        {
            ((IList<AssetUnitInfo>)list).Clear();
        }

        public bool Contains(AssetUnitInfo item)
        {
            return ((IList<AssetUnitInfo>)list).Contains(item);
        }

        public void CopyTo(AssetUnitInfo[] array, int arrayIndex)
        {
            ((IList<AssetUnitInfo>)list).CopyTo(array, arrayIndex);
        }

        public IEnumerator<AssetUnitInfo> GetEnumerator()
        {
            return ((IList<AssetUnitInfo>)list).GetEnumerator();
        }

        public int IndexOf(AssetUnitInfo item)
        {
            return ((IList<AssetUnitInfo>)list).IndexOf(item);
        }

        public void Insert(int index, AssetUnitInfo item)
        {
            ((IList<AssetUnitInfo>)list).Insert(index, item);
        }

        public bool Remove(AssetUnitInfo item)
        {
            return ((IList<AssetUnitInfo>)list).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<AssetUnitInfo>)list).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<AssetUnitInfo>)list).GetEnumerator();
        }
    }
}

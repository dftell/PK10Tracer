using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.RemoteObjectsLib;

namespace WolfInv.com.WebCommunicateClass
{
    public class mAssetUnitList: RecordObject
    {
        public string Data;
        public int Count;
        public List<AssetUnitInfo> List;
    }
}

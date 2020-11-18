using System;
using System.Collections.Generic;
namespace WolfInv.com.ShareLotteryLib
{
    [Serializable]
    public class MutliLevelData
    {
        public MutliLevelData Parent;
        //public List<KeyText> List;
        public Dictionary<KeyText, MutliLevelData> SubList;
        
        public MutliLevelData ()
        {

            //List = new List<KeyText>();
            SubList = new Dictionary<KeyText, MutliLevelData>();
        }

        public KeyText AddSub(KeyText key,MutliLevelData val)
        {
            if(SubList == null)
            {
                SubList = new Dictionary<KeyText, MutliLevelData>();
            }
            if (!SubList.ContainsKey(key))
            {
                SubList.Add(key, val);
                return key;
            }
            return null;
        }

        public MutliLevelData AddSub(string val,string text, MutliLevelData obj)
        {
            KeyText key = new KeyText(val, text);
            if (!SubList.ContainsKey(key))
            {
                SubList.Add(key, obj);
                return obj;
            }
            return null;
        }

        public static MutliLevelData createAValidateSubmitData(string yesVal,string strYes,string noVal,string strNo)
        {
            MutliLevelData ret = new MutliLevelData();
            ret.AddSub(yesVal, strYes , null);
            MutliLevelData noselect = ret.AddSub(noVal, strNo, null);
            return ret;
        }
    }
}

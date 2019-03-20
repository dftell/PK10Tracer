using System;
using System.Reflection;
namespace WolfInv.com.DbAccessLib
{
    public class MemberInfoTypeItem
    {
        public MemberInfo MemInfo;
        public Type DbType;
        public MemberInfoTypeItem(MemberInfo mi,Type dbtype)
        {
            MemInfo = mi;
            DbType = dbtype;
        }
    }
}

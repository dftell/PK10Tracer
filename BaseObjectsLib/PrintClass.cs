using System;
using System.Linq;
using System.Data;
namespace WolfInv.com.BaseObjectsLib
{
    public class PrintClass
    {
        public string ToClassInfo()
        {
            Type myinfo = this.GetType();//BindingFlags.Public|BindingFlags.GetField|BindingFlags.Instance
            var v = from n in myinfo.GetFields()
                    let strItem = string.Format("{0}={1}", n.Name, n.GetValue(this))
                    select strItem;
            return string.Join(";", v.ToArray<string>());
        }
    }

}

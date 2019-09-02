using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Globalization;
using WolfInv.com.BaseObjectsLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;
namespace WolfInv.com.SecurityLib
{
    public class KL11_HtmlDataClass : KL12_HtmlDataClass
    {
        public KL11_HtmlDataClass(DataTypePoint dp) : base(dp)
        {
            //dataUrl = GlobalClass.TXFFC_url;
            //this.dataUrl = dp.RuntimeInfo.DefaultDataUrl;
        }

        
    }

}


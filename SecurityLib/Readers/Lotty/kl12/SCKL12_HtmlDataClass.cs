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
    //四川快乐12
    public class SCKL12_HtmlDataClass : KL12_HtmlDataClass
    {
        public SCKL12_HtmlDataClass(DataTypePoint dp) : base(dp)
        {
            //dataUrl = GlobalClass.TXFFC_url;
            //this.dataUrl = dtp.RuntimeInfo.DefaultDataUrl;
        }

    }

    //辽宁快乐12
    public class NLKL12_HtmlDataClass : KL12_HtmlDataClass
    {
        public NLKL12_HtmlDataClass(DataTypePoint dp) : base(dp)
        {
            //dataUrl = GlobalClass.TXFFC_url;
            //this.dataUrl = dtp.RuntimeInfo.DefaultDataUrl;
        }

    }

    public class GDKL11_HtmlDataClass : KL11_HtmlDataClass
    {
        public GDKL11_HtmlDataClass(DataTypePoint dp) : base(dp)
        {
            //dataUrl = GlobalClass.TXFFC_url;
            //this.dataUrl = dtp.RuntimeInfo.DefaultDataUrl;
        }

    }

    public class XYFT_HtmlDataClass : KL11_HtmlDataClass
    {
        public XYFT_HtmlDataClass(DataTypePoint dp) : base(dp)
        {
            //dataUrl = GlobalClass.TXFFC_url;
            //this.dataUrl = dtp.RuntimeInfo.DefaultDataUrl;
        }

    }
}

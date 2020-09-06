using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolfInv.com.BaseObjectsLib
{
    public interface IWebHostFlags
    {
        string HostKey { get;  }

    }
    public interface IWebLoginFlags
    {
        string LoginedFlag { get;  }
    }

    public interface IWebAmountFlags
    {
        string AmountId { get;  }
    }

    public interface IWebFlags:IWebHostFlags,IWebLoginFlags,IWebAmountFlags
    {

    }
}

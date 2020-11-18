using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
namespace WolfInv.com.SecurityLib
{
    public class SecurityIndexReader: SecurityReader<StockIndexMongoData> 
    {
        public SecurityIndexReader(string db, string docname, string[] codes) : base(db, docname, codes)
        {
            
        }

        
    }
}

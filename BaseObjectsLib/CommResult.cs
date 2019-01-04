using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseObjectsLib
{
    public class CommResult : MarshalByRefObject  
    {
        public bool Succ;
        public string Message;
        public int Cnt;
        public string Json;
        public List<RecordObject> Result;
    }
    
    public class CommResult<T> : MarshalByRefObject 
    {
        public bool Succ;
        public string Message;
        public int Cnt;
        public string Json;
        public string Xml;
        public List<T> Result;
    }

}

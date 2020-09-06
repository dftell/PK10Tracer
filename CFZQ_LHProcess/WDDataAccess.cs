using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WAPIWrapperCSharp;
namespace WolfInv.com.CFZQ_LHProcess
{
    public class LogException : Exception, LogLib.iLog
    {
        public string LogName => throw new NotImplementedException();

        public void Log(string logname, string Topic, string msg)
        {
            LogLib.LogableClass.ToLog(logname, Topic, msg);
        }

        public void Log(string Topic, string Msg)
        {
            LogLib.LogableClass.ToLog( Topic, Msg);
        }

        public void Log(string msg)
        {
            LogLib.LogableClass.ToLog( msg);
        }
    }
    public class WDErrorException : LogException
    {
        string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
        }
        public int WDErrorCode;
        static Dictionary<int,string> ErrList;
        public WDErrorException(WindAPI _w, int ErrorCode)
        {
            this.WDErrorCode = ErrorCode;
            if (ErrList == null) ErrList = new Dictionary<int, string>();
            if (ErrList.ContainsKey(ErrorCode))
            {
                this._Message = ErrList[ErrorCode];
            }
            else
            {
                this._Message= _w.getErrorMsg(ErrorCode);
                ErrList.Add(ErrorCode, this._Message);
            }
        }
     
    }
    /// <summary>
    /// 所有运行万得数据类的基类
    /// </summary>
    public class WDBuilder
    {
        public  WindAPI w;
        public WDBuilder(WindAPI _w)
        {
            if (_w == null)
            {
                try
                {
                    _w = new WindAPI();
                    _w.start();
                }
                catch
                {
                    throw (new Exception("万得接口突然崩溃！"));
                }
                ////_w = new WindAPI();
                ////_w.start();
            }
            w = _w;
        }
    }

    

    public class WDReadClass : WDBuilder
    {

        public WDReadClass(WindAPI _w):base(_w)
        {
            
        }
        protected void InitConnect()
        {
            ////w = new WindAPI();
            ////w.start();
        }
    }
    
    public class WDDateReadClass:WDReadClass
    {
        
        protected string strType;
        protected string strInfo;
        public WDDateReadClass(WindAPI w):base(w)
        {
        }
    }

    interface iWDReader
    {
        WindData getDataSet();
        Int64 getDataSetCount();
    }

}

using System;
//using WolfInv.Com.SocketLib;

namespace ExchangeTermial
{
    public class ExtremeClass<T>
    {
        public int SerialId;
        public T Value;
        public int TimeWeight = 1;
        public double Net;
        public Double DValue
        {
            get
            {
                return Convert.ToDouble(Value);
            }
        }
        public ExtremeType type;

        public enum ExtremeType
        {
            None,Max, Min 
        }
    }

    public enum NetSelectBasicValue
    {
        StartValue, InitValue, MinValue
    }

    public class MaxMinPoint //极值点
    {
        public double Value;
        public int Index;
    }

    public class TimeSerialPoint<T>
    {
        public int TimeWeight;
        public T Value;
        public TimeSerialPoint(int tw,T val)
        {
            TimeWeight = tw;
            Value = val;
        }
    }
}

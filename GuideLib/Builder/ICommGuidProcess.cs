using System;

namespace WolfInv.com.GuideLib
{
    public interface ICommGuidProcess
    {
        RunResultClass getDateSerialResult(string secCode, DateTime begt, DateTime endt, params object[] DataPoints);
        RunResultClass getSetDataResult(string secCodes, string DataPoints, DateTime dt);
        RunResultClass getSetDataResult(string[] secCodes, DateTime dt);
        RunResultClass getSetDataResult(string[] secCodes, DateTime dt, params object[] DataPoints);
        RunResultClass getSetDataResult(string[] secCodes, string DataPoints, DateTime dt);
    }
}
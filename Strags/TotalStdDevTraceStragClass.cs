using System.Collections.Generic;
namespace WolfInv.com.Strags
{
    /// <summary>
    /// 整体标准差追踪类
    /// </summary>
    public abstract class TotalStdDevTraceStragClass : ChanceTraceStragClass
    {
        Dictionary<string, List<double>> AllStdDev;
        public Dictionary<string, List<double>> getAllStdDev()
        {
            return AllStdDev;
        }

        public void setAllStdDev(Dictionary<string, List<double>> value)
        {
            AllStdDev = value;
        }
    }
}

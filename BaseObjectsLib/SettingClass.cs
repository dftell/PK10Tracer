using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.BaseObjectsLib
{
    public class SettingClass : DetailStringClass
    {


        public SettingClass()
        {
            GlobalSetting = new GlobalClass();
        }
        GlobalClass GlobalSetting;
        public GlobalClass GetGlobalSetting()
        {
            return GlobalSetting;
        }
        public void SetGlobalSetting(GlobalClass value)
        {
            GlobalSetting = value;
            if (value != null)
            {
                DispRows = value.MutliColMinTimes;
                minColTimes = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    minColTimes[i] = value.MinTimeForChance(i + 1);
                }
                Odds = value.Odds;
                MaxHoldingCnt = 1000;
                InitCash = value.InterVal;
            }
        }
        public int DispRows { get; set; }
        public int[] minColTimes { get; set; }
        public int GrownMinVal { get; set; }
        public int GrownMaxVal { get; set; }
        public double Odds { get; set; }
        public int MaxHoldingCnt { get; set; }
        public Int64 InitCash { get; set; }
        public bool UseLocalWaveData { get; set; }

    }

}

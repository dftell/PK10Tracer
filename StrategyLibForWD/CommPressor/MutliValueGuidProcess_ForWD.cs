using System;
using System.Linq;
using System.Data;

using System.Reflection;
using Microsoft.VisualBasic;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using WAPIWrapperCSharp;

namespace WolfInv.com.StrategyLibForWD
{
    /// <summary>
    /// 多值指标处理类
    /// </summary>
    public class MutliValueGuidProcess_ForWD : CommGuidProcess_ForWD
    {
        public string  GuildName;//指标名
        public string[] ValueNames;
        public MutliValueGuidProcess_ForWD(WindAPI _w, string guidName,params string[] args) : base(new CommDataInterface_ForWD(_w)) 
        {
            w = _w;
            GuildName = guidName;
            ValueNames = args;
        }

        public override RunResultClass getSetDataResult(string[] secCodes, DateTime dt)
        {
            RunResultClass ret = new RunResultClass();
            MTable tab = new MTable();


            //
            //MutliReturnValueGuidClass gd =  assembly.CreateInstance(ct.FullName) as MutliReturnValueGuidClass;
            MutliReturnValueGuidClass gd = GuidBaseClass.CreateGuideInstance(GuildName) as MutliReturnValueGuidClass;
            //MACDGuidClass gd = new MACDGuidClass(MACDType.MACD);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            GuidBuilder_ForWD gb = null;
            for (int i = 0; i < ValueNames.Length; i++)
            {
                gd.ReturnValueName = ValueNames[i];
                gb = new GuidBuilder_ForWD(w, gd);
                MTable tmp = gb.getRecords(secCodes, dt);
                tab.AddColumnByArray(ValueNames[i], tmp, GuildName);
            }
            ret.Notice.Success = true;
            ret.Result = tab;
            return ret;
        }

        public override RunResultClass getSetDataResult(string[] secCodes, string DataPoints, DateTime dt)
        {
            return getSetDataResult(secCodes, dt);
        }

        public override RunResultClass getSetDataResult(string secCodes, string DataPoints, DateTime dt)
        {
            return getSetDataResult(secCodes.Split(','), dt);
        }

        public override RunResultClass getSetDataResult(string[] secCodes, DateTime dt, params object[] DataPoints)
        {
            return getSetDataResult(secCodes, dt);
        }

        public override RunResultClass getDateSerialResult(string secCode, DateTime begt, DateTime endt, params object[] DataPoints)
        {
            RunResultClass ret = new RunResultClass();
            MTable tab = new MTable();
            Type t = Type.GetType(GuildName + "GuidClass");
            Assembly assembly = Assembly.GetExecutingAssembly();
            //MutliReturnValueGuidClass gd = assembly.CreateInstance(t.Name) as MutliReturnValueGuidClass;
            MutliReturnValueGuidClass gd = GuidBaseClass.CreateGuideInstance(GuildName) as MutliReturnValueGuidClass;
            //MACDGuidClass gd = new MACDGuidClass(MACDType.MACD);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            DateSerialGuidBuilder_ForWD gb = null;
            for (int i = 0; i < ValueNames.Length; i++)
            {
                gd.ReturnValueName = ValueNames[i];
                gb = new DateSerialGuidBuilder_ForWD(w, gd);
                MTable tmp = gb.getRecords(secCode, begt,endt);
                tab.AddColumnByArray(ValueNames[i], tmp, GuildName);
            }
            ret.Notice.Success = true;
            ret.Result = tab;
            return ret;
        }
    }

}


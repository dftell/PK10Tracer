﻿using WAPIWrapperCSharp;
using WolfInv.com.GuideLib;
using WolfInv.com.BaseObjectsLib;
using System.Collections.Concurrent;

namespace WolfInv.com.StrategyLibForWD
{
    public class CommDataInterface_ForWD:CommDataIntface<TimeSerialData>
    {
        public WindAPI w;
        protected CommDataInterface_ForWD():base()
        {

        }

        public CommDataInterface_ForWD(WindAPI _w) : base()
        {
            w = _w;
        }

        public override ConcurrentDictionary<string, MongoReturnDataList<TimeSerialData>> getData()
        {
            throw new System.NotImplementedException();
        }
    }

}


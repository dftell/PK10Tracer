using System.Collections.Generic;
using WolfInv.com.StrategyLibForWD;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.PK10CorePress
{
    public class ExecSubscriptClass
    {
        DataTypePoint dtp;
        public FinishedOpt OnFinishedSubScript;
        public FinishedOpt OnCancelSubScript;
        public FinishedOpt OnUpdateData;
        SubscriptClass SubObj;
        Dictionary<string, ExpectList> Data;
        public ExecSubscriptClass(DataTypePoint _dtp)
        {
            dtp = _dtp;
        }

        void UpdateData(Dictionary<string, ExpectList> els)
        {
            Data = els;
            OnUpdateData(dtp, els);//上传给上一级
        }
        public void GetAllPastData()
        {

        }

        public void Subscript(bool NeedReqData)
        {
            //SubObj = new WD_SubscriptTimeDataClass();
            SubObj.UpdateAll = dtp.SubScriptUpdateAll == 1;
            SubObj.AfterUpdate = UpdateData;
            bool bAllsec = SubObj.GetAllEquits(dtp.SubScriptSector);//获得指定板块下的所有品种清单
            if (!bAllsec)
                return;
            SubObj.getData(dtp.SubScriptFields, dtp.SubScriptOptions, dtp.SubScriptGrpCnt, NeedReqData);
            OnFinishedSubScript(dtp, Data);
        }

        public void CancelSubscript()
        {
            OnCancelSubScript(dtp, Data);
        }
    }

}
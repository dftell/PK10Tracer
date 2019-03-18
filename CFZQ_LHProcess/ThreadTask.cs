using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
namespace CFZQ_LHProcess
{

    

    public class GLTask : MultiTask
    {
        /// <summary>
        ///  方式：1，更新成分股 2、更新行情
        /// </summary>
        public int Method;
        public GLTask(DataRow dr, int mth)
        {
            Data = dr;
            Method = mth;
        }
    }

    public class SaveDataTask : ThdTask
    {
        public string[] Sqls;
    }

    public abstract class MultiTaskThrd : ThreadProcess
    {
        public ThdTask SaveOjb;
        public bool FinishFlag = false;
        public List<MultiTask> DataPool;
        public List<ThdTask> SaveDataPool;
        public void FillPool(List<MultiTask> Datas)
        {
            DataPool = Datas;
        }
        public void AddData()
        {
            lock (DataPool)
            {
                if (DataPool.Count > 0)
                {

                    ExecObj = DataPool[DataPool.Count - 1];
                    DataPool.Remove(ExecObj as MultiTask);
                }
                else
                {
                    ExecObj = null;
                }
            }
        }
        public void FillSaveData()
        {
            if (SaveDataPool == null)
            {
                SaveOjb = null;
                return;
            } 
            lock (SaveDataPool)
            {
                if (SaveDataPool.Count > 0)
                {
                    SaveOjb = SaveDataPool[SaveDataPool.Count - 1];
                    SaveDataPool.Remove(SaveOjb);
                }
                else
                {
                    SaveOjb = null;
                }
            }
        }
    }

    public abstract class ThreadProcess
    {
        public Thread Trdobj = null;
        public object CheckPin = null; //检查针，用于刺探内部状况
        protected JRGCDBClass jgdb;
        protected ThdTask ExecObj;
        protected DateTime EndT;
        public abstract void Execute();
    }

    public abstract class ThdTask
    {
        public object Data;
    }

    public abstract class MultiTask : ThdTask
    {

    }
   
}

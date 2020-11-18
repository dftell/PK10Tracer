using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
namespace WolfInv.com.SecurityLib
{
    public class SecurityGrpReader<T> where T: TimeSerialData
    {
        string DataType;
        string DocumentTable;
        string[] allcodes;
        //List<string[]> codelist;
        //List<MongoDataDictionary<T>> mongoCodeList;
        public CheckFinishedCnt CheckEvent;
        Func<string[], MongoDataDictionary<T>> ExeFunc;
        Func<MongoDataDictionary<T>, MongoDataDictionary<T>> ExecMongoFunc;
        MongoDataDictionary<T> GroupResult;
        int finished = 0;
        int PoolCnt = 0;
        int LastId = 0;
        public SecurityGrpReader()
        {



        }

        public MongoDataDictionary<T> GetResult(List<string[]> codes, Func<string[],MongoDataDictionary<T>> func,int MaxThreadCnt=20,int InterValMilSecs=1000)
        {
            CheckEvent?.Invoke(codes.Select(a=>a.Length).Sum(), codes.Count, 0, 0,0);
            List<string[]> codelist = codes;
            ExeFunc = func;
            GroupResult = new MongoDataDictionary<T>();
            ////for (int i=0;i< MaxThreadCnt; i++)
            ////{
            ////    new Task(ExecSr, codelist[i]).Start();
            ////    LastId = i;
            ////    PoolCnt++;
            ////    Thread.Sleep(InterValMilSecs);

            ////}
            LastId = 0;
            finished = 0;
            PoolCnt = 0;
            while (finished< codes.Count)
            {
                
                if (PoolCnt < MaxThreadCnt)
                {
                    
                    int CurrCnt = PoolCnt;
                    for (int i = CurrCnt; i < MaxThreadCnt; i++)
                    {
                        if (LastId >= codelist.Count)
                            break;
                        new Task(ExecSr, codelist[LastId]).Start();
                        LastId++;
                        PoolCnt++;
                        Thread.Sleep(InterValMilSecs);
                    }
                    CheckEvent?.Invoke(codes.Select(a => a.Length).Sum(),codes.Count, finished, PoolCnt, GroupResult.Count);
                    continue;
                }
                System.Threading.Thread.Sleep(InterValMilSecs);
            }
            return GroupResult;
        }

        public MongoDataDictionary<T> GetResult(List<MongoDataDictionary<T>> codes, Func<MongoDataDictionary<T>, MongoDataDictionary<T>> func, int MaxThreadCnt = 20, int InterValMilSecs = 1000)
        {
            CheckEvent.Invoke(codes.Count, codes.Count, 0, 0, 0);
            List<MongoDataDictionary<T>> mongoCodeList = codes;
            ExecMongoFunc = func;
            GroupResult = new MongoDataDictionary<T>();
            ////for (int i=0;i< MaxThreadCnt; i++)
            ////{
            ////    new Task(ExecSr, codelist[i]).Start();
            ////    LastId = i;
            ////    PoolCnt++;
            ////    Thread.Sleep(InterValMilSecs);

            ////}
            LastId = 0;
            finished = 0;
            PoolCnt = 0;
            while (finished < codes.Count)
            {

                if (PoolCnt < MaxThreadCnt)
                {

                    int CurrCnt = PoolCnt;
                    for (int i = CurrCnt; i < MaxThreadCnt; i++)
                    {
                        if (LastId >= mongoCodeList.Count)
                            break;
                        new Task(ExecMongoSr, mongoCodeList[LastId]).Start();
                        LastId++;
                        PoolCnt++;
                        Thread.Sleep(InterValMilSecs);
                    }
                    CheckEvent.Invoke(codes.Select(a=>a.Values.Count).Sum(), codes.Count, finished, PoolCnt, GroupResult.Count);
                    continue;
                }
                System.Threading.Thread.Sleep(InterValMilSecs);
            }
            return GroupResult;
        }

        public void ExecSr(object obj) 
        {
            string[] codes = obj as string[];
            MongoDataDictionary<T>  res = ExeFunc(codes);
            if(res != null)
            {
                GroupResult.Union(res);
                
            }
            else
            {

            }
            finished++;
            PoolCnt--;
            //(obj as SecurityReader);
        }

        public void ExecMongoSr(object obj)
        {
            MongoDataDictionary<T> codes = obj as MongoDataDictionary<T>;
            MongoDataDictionary<T> res = ExecMongoFunc(codes);
            if (res != null)
            {
                GroupResult.Union(res);

            }
            else
            {

            }
            finished++;
            PoolCnt--;
            //(obj as SecurityReader);
        }

    }
}

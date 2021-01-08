using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolfInv.com.BaseObjectsLib
{
    public class WolfTaskClass
    {
        public static void MultiTaskProcess<T1,T2,T3,T4>(ICollection<T1> items, IDictionary<T1,T2> mainSet,IDictionary<T3,T4> subSet, Action<T1,T2,IDictionary<T1,T2>,IDictionary<T3,T4>, Action<T1>> act, Action<T1> notice,int TaskCnt = 10,int grpCnt = 5,bool needSync=false)
        {
            int index = 0;
            List<List<T1>> grps = new List<List<T1>>();
            List<Task> tasks = new List<Task>();
            while (index < items.Count)
            {
                List<T1> batchItems = items.Skip(index).Take(grpCnt).ToList();
                if (batchItems.Count == 0)
                    break;
                index += batchItems.Count;
                Task tk = Task.Factory.StartNew(
                    (obj) =>
                    {
                        List<T1> subList = obj as List<T1>;
                        for(int i=0;i<subList.Count;i++)
                        {
                            T2 tobj = mainSet[subList[i]];                            
                            act(subList[i],tobj,mainSet,subSet,notice);
                        }
                    }, batchItems
                    );
                if (tasks.Count < TaskCnt)
                {
                    tasks.Add(tk);
                }
                else
                {
                    tasks.Add(tk);
                    int finished = Task.WaitAny(tasks.ToArray());
                    tasks.RemoveAt(finished);

                }
            }
            if (needSync)
            {
                if(tasks.Count>0)
                    Task.WaitAll(tasks.ToArray());
            }
        }

        public static void MultiTaskProcess<T1, T2, T3, T4>(ICollection<T1> items, IDictionary<T2, T3> mainSet, IDictionary<T2, T4> subSet, Action<T1, IDictionary<T2, T3>, IDictionary<T2, T4>,ICollection<T1>> act, int TaskCnt = 10, int grpCnt = 5, bool needSync = false)
        {
            int index = 0;
            List<List<T1>> grps = new List<List<T1>>();
            List<Task> tasks = new List<Task>();
            while (index < items.Count)
            {
                List<T1> batchItems = items.Skip(index).Take(grpCnt).ToList();
                if (batchItems.Count == 0)
                    break;
                index += batchItems.Count;
                Task tk = Task.Factory.StartNew(
                    (obj) =>
                    {
                        List<T1> subList = obj as List<T1>;
                        for (int i = 0; i < subList.Count; i++)
                        {
                            act(subList[i],  mainSet, subSet,items);
                        }
                    }, batchItems
                    );
                if (tasks.Count < TaskCnt)
                {
                    
                }
                else
                {
                    //tasks.Add(tk);
                    int finished = Task.WaitAny(tasks.ToArray());
                    tasks.RemoveAt(finished);

                }
                tasks.Add(tk);
            }
            if (needSync)
            {
                if(tasks.Count>0)
                    Task.WaitAll(tasks.ToArray());
            }
        }

    }
}

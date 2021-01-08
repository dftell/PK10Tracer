using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace WolfInv.com.BaseObjectsLib
{
    public class MongoDataDictionary<T> : ConcurrentDictionary<string, MongoReturnDataList<T>> where T : TimeSerialData
    {
        public bool isSecurity = false;
        public void Union(MongoDataDictionary<T> vals)
        {
            foreach (string key in vals.Keys)
            {
                if (!this.ContainsKey(key))
                    this.TryAdd(key, vals[key]);
            }
        }

        public MongoDataDictionary(bool sectype)
        {
            isSecurity = sectype;
        }

        public MongoDataDictionary(ExpectList<T> list,bool sectype)
        {
            isSecurity = sectype;
            if (list == null)
                return;
            this.Clear();
            for(int i=0;i<list.Count;i++)
            {
                ExpectData<T> a = list[i];
                string expect = a.Key;
                foreach(var data in a.Values)
                {
                    MongoReturnDataList<T> currList = new MongoReturnDataList<T>(new StockInfoMongoData(data.Key,data.KeyName),isSecurity);
                    if(this.ContainsKey(data.Key))
                    {
                        currList = this[data.Key]; 
                    }
                    else
                    {
                        this.TryAdd(data.Key, currList);
                    }
                    currList.Add(data);
                }
            }
        }

        public MongoDataDictionary(MongoReturnDataList<T>[] inputdata)
        {
            inputdata.ToList().ForEach((a)=>{
                MongoReturnDataList<T> val = a;
                if (val == null || val.Count == 0)
                    return;
                this.TryAdd((a[0] as ICodeData).code, a);//如果有一条及以上记录，加入
            });
        }

        public ExpectList<T> ToExpectList(int threadCnt = 10)
        {
            List<ExpectData<T>> ret = new List<ExpectData<T>>();
            List<string> alldates = null;
            if (isSecurity)
            {
                alldates = AllDates.Select(a => a.WDDate()).ToList();
            }
            else
            {
                alldates = AllDates.Select(a => a.ToString()).ToList() ;
            }
            alldates = alldates.OrderBy(a => a).ToList();
            Dictionary<string, string> keys1 = new Dictionary<string, string>();
            foreach (string key in this.Keys)
            {
                if (key == null)
                {
                    continue;
                }
                if (!keys1.ContainsKey(key))
                {
                    keys1.Add(key, this[key].SecInfo?.KeyName);
                }
            }
            WolfTaskClass.MultiTaskProcess<string,string,MongoReturnDataList<T>,string>(alldates,this, keys1, 
                (expectNo,ms,retList,items)=> {
                
                ExpectData<T> data = new ExpectData<T>(isSecurity);
                if (isSecurity)
                {
                    data.Expect = expectNo;
                }
                else
                {
                    //data.Expect = 
                    data.OpenTime = DateTime.Parse(expectNo);
                }
                foreach (var key in retList)
                {
                    if (ms[key.Key].ContainKey(expectNo))
                    {

                        T tdata = ms[key.Key][expectNo];
                        if (tdata == null)
                            continue;
                        if (!isSecurity)
                        {
                            data.Expect = tdata.Expect;
                            data.OpenCode = tdata.OpenCode;
                            data.OpenTime = tdata.OpenTime;
                        }
                        tdata.Key = key.Key;
                        tdata.KeyName = key.Value;
                        if (!data.ContainsKey(key.Key))
                        {
                            //tdata.Key = key.Key;
                            data.Add(key.Key, tdata);
                        }

                    }
                }
                ret.Add(data);
                retList = null;
            },
            11,
            5,
            true);
            ExpectList<T> res1 = new ExpectList<T>(isSecurity);
            foreach (var item in ret.OrderBy(a => a.Expect))
            {
                res1.Add(item);
            }
            ret = null;
            return res1;



            int grpCnt = Math.Min(alldates.Count, alldates.Count / threadCnt + 1);
            List<string[]> grps = new List<string[]>();
            int index = 0;
            
            Task[] tasks = new Task[threadCnt];
            //改写成更高效率的多线程
            for (int i=0;i<threadCnt;i++)
            {
                string[] agrp = alldates.Skip(index).Take(grpCnt).ToArray();
                index += agrp.Length;
                Task task = Task.Factory.StartNew((obj) => {
                    string[] list = obj as string[];
                    for(int s=0;s<list.Length;s++)
                    {
                        ExpectData<T> data = new ExpectData<T>(isSecurity);
                        if (isSecurity)
                        {
                            data.Expect = list[s];
                        }
                        else
                        {
                            //data.Expect = 
                            data.OpenTime = DateTime.Parse(list[s]);
                        }                        
                        foreach(var key in keys1)
                        {
                            if (this[key.Key].ContainKey(list[s]))
                            {

                                T tdata = this[key.Key][list[s]];
                                if (tdata == null)
                                    continue;
                                if (!isSecurity)
                                {
                                    data.Expect = tdata.Expect;
                                    data.OpenCode = tdata.OpenCode;
                                    data.OpenTime = tdata.OpenTime;
                                }
                                tdata.Key = key.Key;
                                tdata.KeyName = key.Value;
                                if (!data.ContainsKey(key.Key))
                                {
                                    //tdata.Key = key.Key;
                                    data.Add(key.Key, tdata);
                                }

                            }
                        }
                        lock (ret)
                        {
                            ret.Add(data);
                        }
                    }
                }, agrp);
                tasks[i] = task;
            }
            Task.WaitAll(tasks);
            ExpectList<T> res = new ExpectList<T>(isSecurity);
            foreach(var item in ret.OrderBy(a => a.Expect))
            {
                res.Add(item);
            }
            return res;
        }

        List<string> _alldates;
        DateTime[] AllDates
        {
            get
            {
                if (_alldates == null)
                {
                    _alldates = new List<string>();
                    HashSet<string> tmp = new HashSet<string>();
                    List<string> tkeys = this.Keys.ToList();
                    foreach (string key in tkeys)
                    {
                        if (string.IsNullOrEmpty(key))
                            continue;
                        if (!this.ContainsKey(key))
                        {
                            continue;
                        }
                        MongoReturnDataList<T> datas = this[key];
                        List<string> keys = null;
                        if (isSecurity)//股票
                        {
                            keys = datas.Keys;
                        }
                        else//彩票
                        {
                            keys = datas.Select(a => a.OpenTime.ToString()).ToList();
                        }
                        keys.ForEach(a =>
                        {
                            if (!tmp.Contains(a))
                            {
                                tmp.Add(a);
                            }
                        });
                    }
                    _alldates = tmp.ToList().OrderBy(a => a).ToList();
                    
                }
                if (isSecurity)//股票
                {
                    return _alldates.Select(a => a.ToDate()).ToArray();
                }
                else//彩票
                {
                    return _alldates.Select(a =>DateTime.Parse(a)).ToArray();
                }
                
            }
        }

        public DateTime[] getAllDates()
        {
            return AllDates;
        }
        
    }
}

using System;
using WolfInv.com.BaseObjectsLib;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
namespace WolfInv.com.SecurityLib
{
    public class Web52CP_MissData_ProcessClass : iConvertToDataSet, iGenerateUrl
    {
        HttpClient req;
        HttpClientHandler handler;
        public DataTable ConvertToData(string strHtml, DataTypePoint dtp,DataTable oldTable = null)
        {
            Type t = Type.GetType(dtp.ExDataConfig.convertClass);
            if (t == null)
                return null;
            if (strHtml == null)
                return null;
            DataTable dt = new DataTable();
            HashSet<string> useCols = new HashSet<string>();
            if (oldTable != null)//如果提供了架构，复制架构
            {
                dt = dt.Clone();
                for (int i = 0; i < dt.Columns.Count; i++)
                    useCols.Add(dt.Columns[i].ColumnName);
            }
            else//如果没有架构，重新定义架构
            {
                
                string[] names = dtp.ExDataConfig.dataColumns.Split(',');
                Dictionary<string, string> nameDic = names.ToDictionary(a => a, a => a);

                FieldInfo[] flds = t.GetFields();

                //根据配置里指定的列建立表结构
                flds.ToList().ForEach(a =>
                {
                    if (nameDic.ContainsKey(a.Name) && !dt.Columns.Contains(a.Name))
                    {
                        dt.Columns.Add(a.Name);
                        useCols.Add(a.Name);
                    }
                });
            }
            Web52CPMissDataClass data = new Web52CPMissDataClass();
            data = data.FromJson(strHtml);
            if (data == null)
                return null;
            if(data.status != 1)
            {
                return null;
            }
            data.result.ForEach(a => 
            {
                DataRow dr = dt.NewRow();
                useCols.ToList().ForEach(
                    c =>
                    {
                        FieldInfo fi = t.GetField(c);
                        if(fi!= null)
                        {
                            dr[c] = fi.GetValue(a);
                        }
                    }
                    );
                dt.Rows.Add(dr);
            }          
                );
            return dt;
        }

        public List<string> generateUrl(DataTypePoint dtp)
        {
            return null;
        }

        public DataTable getData(DataTypePoint dtp, DataTable dt, params object[] objs)
        {
            string url = dtp.ExDataConfig.InterfaceUrl;
            if (objs.Length > 1)
            {
                string missUrlModel = objs[0].ToString(); // "lottery={0}&period={1}&pos={2}&target={3}";
                object[] args = objs.Skip(1).ToArray();
                url = string.Format(dtp.ExDataConfig.InterfaceUrl+"{0}", string.Format(missUrlModel, args));
            }
            string res = getHtml(url);
            if (res == null)
                return null;
            return ConvertToData(res,dtp,dt);            
        }

        public string getHtml(string url)
        {
            if (handler == null)
            {
                handler = new HttpClientHandler();
                handler.UseCookies = true;
                handler.AllowAutoRedirect = true;
            }
            if (req == null)
            {
                req = new HttpClient(handler);
            }
            try
            {
                var res = req.GetAsync(url).Result;
                return res.Content.ReadAsStringAsync().Result;
            }
            catch(Exception ce)
            {
                return null;
            }
            
        }

        public string getCurrExpect(string strHtml)
        {
            return null;
        }

        public bool hasNewsMissData(DataTypePoint dtp,string expectNo)
        {
            string strHtml = getHtml(dtp.ExDataConfig.MissHtmlUrl);
            if (strHtml == null)
                return false;
            string strReg = dtp.ExDataConfig.keyReg;
            Regex reg = new Regex(strReg);
            MatchCollection mcs = reg.Matches(strHtml);
            if(mcs.Count>0)
            {
                for(int i=0;i<mcs.Count;i++)//任何一个指定正则表达式匹配，就说明是当前期数
                {
                    if(mcs[i].Groups[1].Value.ToString().Trim() == expectNo)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    [Serializable]
    public class Web52CP_Lotty_Miss_DataItemClass : Web52CP_Lotty_DataClass
    {
        public string num;// "10,11",
        public int times;// 3,
        public Single m_times;//: 2.73,
        public string miss3;// "-",
        public string miss2;// 38,
        public string miss1;// 124,
        public string miss;// 102,
        public string avg_miss;// 54,
        public string max_miss;// 124,
        public string cycle;//: 110,
        public string arise3;//: 0,
        public string arise2;//: 2,
        public string arise1;//: 0,
        public string arise;//: 1,
        public Single investment;//: 0.93,
        public Single supplement;//: 0.2
    }
    [Serializable]
    public class Web52CPMissDataClass : JsonableClass<Web52CPMissDataClass>
    {
        public int status { get; set; }
        public List<Web52CP_Lotty_Miss_DataItemClass> result;
    }

}


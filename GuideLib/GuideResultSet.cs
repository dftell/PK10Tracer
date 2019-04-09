using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
namespace WolfInv.com.GuideLib
{
    
    public class GuideResultSet : DataSet
    {
        protected Dictionary<string, double> _data;
        public GuideResultSet()
        {
        }

        public GuideResultSet(Dictionary<string, double> data)
        {
            this.Tables.Clear();
            _data = data;
            Fill(_data);
        }

        public GuideResult Fill(Dictionary<string, double> _data)
        {
            GuideResult gr = new GuideResult();
            if (_data == null) return gr;
            Dictionary<string, double> data = _data.ToDictionary(p => p.Key, p => p.Value);
            //gr.TableName = "Base";
            gr.Columns.Add("Id",typeof(Int64));
            gr.Columns.Add("Expect", typeof(Int64));
            gr.Columns.Add("val",typeof(double));
            Int64 i=0;
            foreach (string key in data.Keys)
            {
                i++;
                DataRow dr = gr.NewRow();
                dr["Id"] = i;
                dr["Expect"] = Int64.Parse(key);
                dr["val"] = data[key];
                gr.Rows.Add(dr);
            }
            this.Tables.Add(gr);
            return gr;
        }

        public GuideResult NewTable(string TableName)
        {
            GuideResult ret = new GuideResult(TableName);
            ret.Columns.Add("Id",typeof(Int64));
            ret.Columns.Add("Expect", typeof(Int64));
            ret.Columns.Add("val",typeof(double));
            if(!this.Tables.Contains(TableName))
            {
                this.Tables.Add(ret);
            }
            for (int i = 0; i < this.Tables[0].Rows.Count; i++)
            {
                DataRow dr = this.Tables[TableName].NewRow();
                dr["Id"] = this.Tables[0].Rows[i]["Id"];
                dr["Expect"] = this.Tables[0].Rows[i]["Expect"];
                this.Tables[TableName].Rows.Add(dr);
            }
            return ret;
        }

        public GuideResult NewTable(Dictionary<string, double> data)
        {
            return Fill(data);
        }
    }
}

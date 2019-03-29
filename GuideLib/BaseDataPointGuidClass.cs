using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace WolfInv.com.GuideLib
{ 
    public class BaseDataPointGuidClass : GuidBaseClass
    {
        //无指标名称
        
        public BaseDataPointGuidClass(params BaseDataPoint[] args)
        {
            this.Fields = new HashSet<string>();
            for (int i = 0; i < args.Length; i++)
                this.Fields.Add(args[i].ToString());
        }

        public BaseDataPointGuidClass(string strNames)
        {
            this.Fields = new HashSet<string>();
            if (strNames.IndexOf(",") > 0)
            {
                string[] args = strNames.Split(',');
                
                for (int i = 0; i < args.Length; i++)
                {
                    string field = args[i];
                    if (!this.Fields.Contains(field))
                        this.Fields.Add(field);
                }
            }
            else
            {
                this.Fields.Add(strNames);
            }
        }

        public BaseDataPointGuidClass(bool AllBasePoint)
        {
            Type t = typeof(BaseDataPoint);
            FieldInfo[] flds = t.GetFields(BindingFlags.Public | BindingFlags.Static);
            this.Fields = new HashSet<string>();
            for (int i = 0; i < flds.Length; i++)
            {
                if(!this.Fields.Contains(flds[i].Name))
                    this.Fields.Add(flds[i].Name);
            }
        }

        public BaseDataPointGuidClass(params object[] args)
        {
            this.Fields = new HashSet<string>();
            for (int i = 0; i < args.Length; i++)
            {
                string field = args[i].ToString();
                if (args[i].GetType() == typeof(BaseDataPoint))
                {
                    field = args[i].ToString();
                }
                if (!this.Fields.Contains(field))
                    this.Fields.Add(field);
            }
        }

        public override string getParamString()
        {
            return "";
            //throw new NotImplementedException();
        }
    }

}


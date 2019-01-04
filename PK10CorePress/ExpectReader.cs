using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Xml;
using System.Web;
using System.IO;
using System.Globalization;
using LogLib;
namespace PK10CorePress
{
    public class PK10ExpectReader : CommExpectReader
    {
        public PK10ExpectReader()
            : base()
        {
            this.strDataType = "PK10";
            InitTables();
        }
    }

    public class ExpectReader : PK10ExpectReader
    {
        public ExpectReader()
            : base()
        {
            strNewestTable = "Newestdata";
            strHistoryTable = "historydata";

        }
    }
    
    public class TXFFCExpectReader : CommExpectReader
    {
        public TXFFCExpectReader()
        {
            strDataType = "TXFFC";


            strNewestTable = "TXFFC_Newestdata";
            strHistoryTable = "TXFFC_historydata";
            strMissHistoryTable = "v_TXFFC_HistoryData_Miss";
            strMissNewestTable = "v_TXFFC_NewestData_Miss";
            InitTables();
        }
    }

    public class PK10ProbWaveDataInterface : PK10ExpectReader
    {
        public PK10ProbWaveDataInterface()
        {
            strResultTable = "tmp_PK10_LongTermAll_ProbWaveTable";
            InitTables();
        }
    }

    public abstract class CommExpectReader:LogableClass
    {
        /// <summary>
        /// 数据类型，PK10,TXFFC
        /// </summary>
        protected string strDataType;

        protected string strNewestTable;
        protected string strHistoryTable;
        protected string strMissHistoryTable;
        protected string strMissNewestTable;
        protected string strResultTable;
        protected string strChanceTable;
        public ExpectList ReadHistory(long From, long buffs)
        {
            return ReadHistory(From, buffs, false);
        }

        protected void InitTables()
        {
            if (GlobalClass.SystemDbTables == null)
            {
                return;
            }
            if (GlobalClass.SystemDbTables.ContainsKey(this.strDataType))
            {
                Dictionary<string, string> dic = GlobalClass.SystemDbTables[this.strDataType];

                if (dic.ContainsKey("NewestTable")) strNewestTable = dic["NewestTable"];
                if (dic.ContainsKey("HistoryTable")) strHistoryTable = dic["HistoryTable"];
                if (dic.ContainsKey("MissHistoryTable")) strMissHistoryTable = dic["MissHistoryTable"];
                if (dic.ContainsKey("MissNewestTable")) strMissNewestTable = dic["MissNewestTable"];
                if (dic.ContainsKey("ResultTable")) strResultTable = dic["ResultTable"];
                if (dic.ContainsKey("ChanceTable")) strChanceTable = dic["ChanceTable"];
            }
        }

        public ExpectList ReadHistory(long From,long buffs,bool desc)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top {1} * from {2} where expect>='{1}'  order by expect {3}", buffs, From,strHistoryTable,desc?"desc":"");
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        
        public ExpectList ReadHistory(long buffs)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top {0} * from {2}  order by expect desc", buffs,  strHistoryTable);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public ExpectList ReadHistory()
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select  * from {0}  order by expect", strHistoryTable);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }
        
        public ExpectList ReadHistory(string begt,string endt)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select  * from {0}  where opentime between '{1}' and '{2}'", strHistoryTable,begt,endt);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public ExpectList ReadNewestData(DateTime fromdate)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {1} where opentime>='{0}' order by expect", fromdate.ToShortDateString(),strNewestTable);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public ExpectList ReadNewestData(int LastLng)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from (select top {0} * from {1} order by expect desc) order by expect", LastLng, strNewestTable);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public ExpectList ReadNewestData(int ExpectNo, int Cnt)
        {
            return ReadNewestData(ExpectNo, Cnt, false);
        }

        public ExpectList ReadNewestData(int ExpectNo, int Cnt,bool FromHistoryTable)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {2} where Expect<='{0}' and Expect>({0}-{1}) order by expect", ExpectNo, Cnt, FromHistoryTable?strHistoryTable:strNewestTable);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }

        public int SaveNewestData(ExpectList InData)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top 0 * from {0}", strNewestTable);
            return db.SaveList(sql, InData.Table);
        }

        public ExpectList GetMissedData(bool IsHistoryData,string strBegT)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {1} where opentime>='{0}'", strBegT, IsHistoryData?strMissHistoryTable:strMissNewestTable);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return new ExpectList(ds.Tables[0]);
        }


        public int SaveHistoryData(ExpectList InData)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top 0 * from {0}", strHistoryTable);
            return db.SaveList(sql, InData.Table);
        }


        public int SaveProbWaveResult(DataTable dt)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {0}",strResultTable );
            return db.SaveNewList(sql, dt);
        }

        public DataTable GetProWaveResult(Int64 begid)
        {
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select * from {0} where Expect>='{1}' order by Expect", this.strResultTable, begid);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            return ds.Tables[0];
        }

        public ExpectList getNewestData(ExpectList NewestData, ExpectList ExistData)
        {
            ExpectList ret = new ExpectList();
            if (NewestData == null) return ret;
            if (ExistData == null) return NewestData;
            HashSet<string> existDic = new HashSet<string>();
            for (int i = 0; i < ExistData.Count; i++)
            {
                existDic.Add(ExistData[i].Expect);
            }
            for (int i = NewestData.Count - 1;i>=0 ; i--)
            {
                if (existDic.Contains(NewestData[i].Expect))
                {
                    continue;
                }
                ret.Add((ExpectData)NewestData[i].Clone());
            }
            return ret;
        }

        public DbChanceList getNoCloseChances(string strDataOwner)
        {
           DbChanceList ret = new DbChanceList();
            DbClass db = GlobalClass.getCurrDb();
            string sql = null;
            if (strDataOwner == null || strDataOwner.Trim().Length == 0)
                sql = string.Format("Select * from {0} where IsEnd=0", strChanceTable);
            else
                sql = string.Format("Select * from {0} where IsEnd=0 and UserId='{1}'", strChanceTable, strDataOwner);
            DataSet ds = db.Query(sql);
            if (ds == null) return null;
            //ToLog("数据库结果",string.Format("未关闭机会数量为{0}",ds.Tables[0].Rows.Count));
            ret = new DbChanceList(ds.Tables[0]);
            return ret;
        }

        public int SaveChances(List<ChanceClass> list,string strDataOwner)
        {
            DbChanceList ret = new DbChanceList();
            DbClass db = GlobalClass.getCurrDb();
            string sql = string.Format("select top 0 * from {0}", strChanceTable);
            DataTable dt = db.getTableBySqlAndList<ChanceClass>(sql, list);
            if (dt == null)
            {
                ToLog("保存机会数据错误", "根据数据表结构和提供的列表返回的机会列表错误！");
                return -1;
            }
            if (strDataOwner == null || strDataOwner.Trim().Length == 0)
                sql = string.Format("Select * from {0} where IsEnd=0", strChanceTable);
            else
                sql = string.Format("Select * from {0} where IsEnd=0 and UserId='{1}'", strChanceTable,strDataOwner);
            return db.UpdateOrNewList(sql, dt);

        }
    }

    public abstract class HtmlDataClass
    {
        protected string dataUrl;
        public ExpectList getExpectList()
        {
            ExpectList ret = new ExpectList();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(dataUrl);
            req.Method = "Get";
            string htmltxt = null;
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    htmltxt = new StreamReader(wr.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
                    wr.Close();
                }

                return getData(htmltxt);
            }
            catch
            {
            }
            return ret;
        }

        public abstract ExpectList getHistoryData(string FolderPath,string filetype);

        public abstract ExpectList getHistoryData(string strDate, int pageid);

        protected abstract ExpectList getData(string strHtml);

        protected abstract ExpectList getHisData(string strHtml);
    }

    public class TXFFC_HtmlDataClass : HtmlDataClass
    {
        public TXFFC_HtmlDataClass()
        {
            dataUrl = GlobalClass.TXFFC_url;
        }

        protected override ExpectList getData(string strHtml)
        {
            ExpectList ret = new ExpectList();
            string strBeg = "<table";
            string strEnd = "</table>";
            int ibeg = strHtml.IndexOf(strBeg);
            int iend = strHtml.IndexOf(strEnd)+strEnd.Length;
            if (ibeg == 0) return ret;
            if (iend <= ibeg)
            {
                return ret;
            }
            string txtTable = strHtml.Substring(ibeg, iend - ibeg);
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(txtTable);
                XmlNodeList nodelist = doc.SelectNodes("table/tr");
                for (int i = 0; i < nodelist.Count; i++)
                {
                    XmlNodeList tdlist = nodelist[i].SelectNodes("td");
                    if (tdlist.Count == 0) continue;
                    ExpectData ed = new ExpectData();
                    ed.Expect = tdlist[1].InnerText.Replace("-","");
                    ed.OpenCode = tdlist[4].InnerText;
                    ed.OpenTime = DateTime.Parse(tdlist[2].InnerText);
                    ret.Add(ed);
                }
            }
            catch (Exception ce)
            {
            }
            return ret;
        }

        protected override ExpectList getHisData(string strHtml)
        {
            ExpectList ret = new ExpectList();
            string strBeg = "<table cellspacing=\"0\" cellpadding=\"0\" class=\"dt caipiao mbm\"";
            string strEnd = "</table>";
            int ibeg = strHtml.IndexOf(strBeg);
            strHtml = strHtml.Substring(ibeg);
            int iend = strHtml.IndexOf(strEnd) + strEnd.Length;
            if (ibeg == 0) return ret;
            if (iend <= 0)
            {
                return ret;
            }
            string txtTable = strHtml.Substring(0, iend);
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(txtTable);
                XmlNodeList nodelist = doc.SelectNodes("table/tr");
                for (int i = 0; i < nodelist.Count; i++)
                {
                    if (i == 0) continue;
                    XmlNodeList tdlist = nodelist[i].SelectNodes("td");
                    if (tdlist.Count == 0) continue;
                    ExpectData ed = new ExpectData();
                    string strExpect = tdlist[0].InnerText;
                    string strIndex = tdlist[1].InnerText;
                    string strOpenCodes = string.Join(",",tdlist[2].InnerText.Substring(0,5).ToCharArray());
                    string[] strTimes = strExpect.Split(' ');
                    DateTime dt = DateTime.ParseExact(strExpect, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture);
                    DateTime bdt = DateTime.Parse(dt.ToShortDateString());
                    string strRindex = "0000" +dt.Subtract(bdt).TotalMinutes.ToString();
                    ed.Expect = string.Format("{0}{1}",strTimes[0],strRindex.Substring(strRindex.Length- 4));
                    ed.OpenCode = strOpenCodes;
                    ed.OpenTime = dt;
                    ret.Add(ed);
                }
            }
            catch (Exception ce)
            {
            }
            return ret;
        }


        public override ExpectList getHistoryData(string FolderPath,string fileType)
        {
            ExpectList ret = new ExpectList();
            DirectoryInfo dir = new DirectoryInfo(FolderPath);
            FileInfo[] fil = dir.GetFiles();
            foreach (FileInfo f in fil)
            {
                long size = f.Length;
                if (f.FullName.ToLower().Substring(f.FullName.Length - fileType.Length) != fileType.ToLower())
                {
                    continue;//非指定类型跳过
                }
                

            }
            return ret;
        }

        public ExpectList getFileData(string filename)
        {
            ExpectList ret = new ExpectList();
            FileStream file = new FileStream(filename, FileMode.Open,FileAccess.Read);
            StreamReader str = new StreamReader(file, Encoding.Default);
            try
            {
                
                //用以接受FileStream对象中的数据,第2个参数是字节数组中开始写入数据的位置,它通常是0,
                //表示从数组的开端文件中向数组写数据,最后一个参数规定从文件读多少字符.
                Decoder d = Encoding.Default.GetDecoder();
                
                int lcnt = 0;
                string txtline ;
                while ((txtline = str.ReadLine())!=null)
                {
                    lcnt++;
                    if (lcnt <= 1)
                    {
                        continue;//标题跳过
                    }
                    if (txtline.Trim().Length == 0) continue;
                    txtline = txtline.Replace("	", ",");
                    string[] items = txtline.Split(',');
                    if (items.Length != 2)
                    {
                        throw new Exception(string.Format("{0}第{1}行数据异常！", filename, lcnt));
                    }
                    ExpectData ed = new ExpectData();
                    ed.OpenCode = string.Join(",", items[1].ToCharArray());
                    string strOrg = items[0];
                    string[] strOrgs = strOrg.Split('-');
                    if (strOrgs.Length > 1)
                    {
                        string strDate = strOrgs[0];
                        string strId = strOrgs[1];
                        string DstrDate = string.Format("{0}-{1}-{2}", strDate.Substring(0, 4), strDate.Substring(4, 2), strDate.Substring(6));
                        DateTime etime = DateTime.Parse(DstrDate);
                        ed.OpenTime = etime.AddMinutes(int.Parse(strId)).AddSeconds(6);
                        ed.Expect = ed.OpenTime.ToString("yyyy-MM-dd HH-mm-SS").Replace("-", "").Replace("/","").Replace(":","").Replace(" ","").Substring(0, 12);
                    }
                    else
                    {
                        ed.Expect = strOrg;
                        string strDate = strOrg.Substring(0, 8);
                        string strId = ed.Expect.Substring(8);
                        string DstrDate = string.Format("{0}-{1}-{2} {3}:{4}:{5}", strDate.Substring(0, 4), strDate.Substring(4, 2), strDate.Substring(6), strId.Substring(0, 2), strId.Substring(2), "06");
                        DateTime etime = DateTime.Parse(DstrDate);
                        ed.OpenTime = etime;
                    }
                    //ed.Expect = items[0].Replace("-", "");
                    
                    //if (ed.OpenCode == "0,0,0,0,0") 
                    
                    
                    
                    ret.Add(ed);
                }

                str.Close();
                //file.Close();
            }
            catch (IOException e)
            {
                str.Close();
                //file.Close();
                throw e;
            }
            return ret;
        }
        
        public override ExpectList getHistoryData(string strdate, int pageid)
        {
            string url = "https://www.e3sh.com/txffc/{0}.html?page={1}";
            string dataUrl =string.Format(url, strdate, pageid);
            ExpectList ret = new ExpectList();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(dataUrl);
            req.Method = "Get";
            string htmltxt = null;
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    htmltxt = new StreamReader(wr.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
                    wr.Close();
                }
            }
            catch (Exception ce)
            {
                return ret;
            }
            return getHisData(htmltxt); ;
        }
    }

    public class PK10_HtmlDataClass : HtmlDataClass
    {
        public PK10_HtmlDataClass()
        {
            this.dataUrl = GlobalClass.PK10_url;
        }
        public override ExpectList getHistoryData(string FolderPath, string filetype)
        {
            throw new NotImplementedException();
        }

        public override ExpectList getHistoryData(string strDate, int pageid)
        {
            throw new NotImplementedException();
        }

        protected override ExpectList getData(string strHtml)
        {
            ExpectList ret = new ExpectList();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(strHtml);
                XmlNodeList rows = doc.SelectNodes("/xml/row");
                if (rows.Count == 0) 
                    return ret;
                for (int i = rows.Count - 1; i >= 0; i--)
                {
                    ExpectData ed = new ExpectData();
                    ed.Expect = rows[i].Attributes["expect"].Value ;
                    ed.OpenCode = rows[i].Attributes["opencode"].Value;
                    ed.OpenTime = DateTime.Parse(rows[i].Attributes["opentime"].Value);
                    ret.Add(ed);
                }
            }
            catch
            {
            }
            return ret;
        }

        protected override ExpectList getHisData(string strHtml)
        {
            throw new NotImplementedException();
        }
    }

}

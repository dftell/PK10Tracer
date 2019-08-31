using System;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Globalization;
using WolfInv.com.BaseObjectsLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
namespace WolfInv.com.SecurityLib
{
    public class CAN28_HtmlDataClass : HtmlDataClass
    {
        public CAN28_HtmlDataClass(DataTypePoint dp) : base(dp)
        {
            //dataUrl = GlobalClass.TXFFC_url;
            this.dataUrl = GlobalClass.TypeDataPoints["CAN28"].RuntimeInfo.DefaultDataUrl;
        }

        public override ExpectList<T> getData<T>(string strHtml)
        {
            DateTime Now = DateTime.Now;
            ExpectList<T> ret = new ExpectList<T>();
            string classTable = @"class=""list""";
            string regtxt = @"<table class=""list"">.*(<tr>.*?</tr>)</tbody></table>";
            Regex regTr = new Regex(@"(?is)(?<=<table[^>]*?" + classTable + "[^>]*?>(?:(?!</?table).)*)(?is)<tr[^>]*?>(?:\\s*<td[^>]*>(.*?)</td>)*\\s*</tr>");

            //MatchCollection mc = Regex.Matches(strHtml, regtxt);
            MatchCollection mc = regTr.Matches(strHtml);
            HashSet<string> AllEds = new HashSet<string>();
            for (int ci=mc.Count-1;ci>=0;ci--)
            {
                Match m = mc[ci];
                Regex regTd = new Regex(@"(?is)(?<=<td(\s+align=[^>]+)?>).*?(?=\s*</td)");
                MatchCollection mtd = regTd.Matches(m.Value);
                ExpectData<T> ed = new ExpectData<T>();
                ed.Expect = mtd[0].Value;
                string[] strs = mtd[2].Value.Split('=')[0].Replace(" ","").Split('+');
                for(int i=0;i<strs.Length;i++)
                {
                    strs[i] = ((strs[i]=="0")?"1":"0") + strs[i];
                    
                }
                ed.OpenCode = string.Join(",", strs);
                DateTime currVal = DateTime.Parse(string.Format("{0}-{1}", Now.Year, mtd[1].Value));
                if(currVal>Now) //如果匹配出的时间大于当前时间
                {
                    currVal = DateTime.Parse(string.Format("{0}-{1}", Now.Year-1, mtd[1].Value));
                }
                ed.OpenTime = currVal;
                if(AllEds.Contains(ed.Expect))
                {
                    continue;
                }
                AllEds.Add(ed.Expect);
                ret.Add(ed);
            }
            return ret;
            string strBeg = "<table";
            string strEnd = "</table>";
            int ibeg = strHtml.IndexOf(strBeg);
            int iend = strHtml.IndexOf(strEnd) + strEnd.Length;
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
                    ExpectData<T> ed = new ExpectData<T>();
                    ed.Expect = tdlist[1].InnerText.Replace("-", "");
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

        public override ExpectList<T> getHisData<T>(string strHtml)
        {
            return getData<T>(strHtml);
            ExpectList<T> ret = new ExpectList<T>();
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
                    ExpectData<T> ed = new ExpectData<T>();
                    string strExpect = tdlist[0].InnerText;
                    string strIndex = tdlist[1].InnerText;
                    string strOpenCodes = string.Join(",", tdlist[2].InnerText.Substring(0, 5).ToCharArray());
                    string[] strTimes = strExpect.Split(' ');
                    DateTime dt = DateTime.ParseExact(strExpect, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture);
                    DateTime bdt = DateTime.Parse(dt.ToShortDateString());
                    string strRindex = "0000" + dt.Subtract(bdt).TotalMinutes.ToString();
                    ed.Expect = string.Format("{0}{1}", strTimes[0], strRindex.Substring(strRindex.Length - 4));
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


        public override ExpectList<T> getHistoryData<T>(string FolderPath, string fileType)
        {
            ExpectList<T> ret = new ExpectList<T>();
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

        public ExpectList<T> getFileData<T>(string filename) where T : TimeSerialData
        {
            ExpectList<T> ret = new ExpectList<T>();
            FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader str = new StreamReader(file, Encoding.Default);
            try
            {

                //用以接受FileStream对象中的数据,第2个参数是字节数组中开始写入数据的位置,它通常是0,
                //表示从数组的开端文件中向数组写数据,最后一个参数规定从文件读多少字符.
                Decoder d = Encoding.Default.GetDecoder();

                int lcnt = 0;
                string txtline;
                while ((txtline = str.ReadLine()) != null)
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
                    ExpectData<T> ed = new ExpectData<T>();
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
                        ed.Expect = ed.OpenTime.ToString("yyyy-MM-dd HH-mm-SS").Replace("-", "").Replace("/", "").Replace(":", "").Replace(" ", "").Substring(0, 12);
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

        public override ExpectList<T> getHistoryData<T>(string strdate, int pageid)
        {
            string url = "https://www.dashen28.com/jianada28/lishi/{0}";
            string dataUrl = string.Format(url, pageid);
            ExpectList<T> ret = new ExpectList<T>();
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
            return getHisData<T>(htmltxt); ;
        }

        public override ExpectList<T> getXmlData<T>(string strXml)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> getJsonData<T>(string strXml)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> getTextData<T>(string strXml)
        {
            throw new NotImplementedException();
        }
    }


}

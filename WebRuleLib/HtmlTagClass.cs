using System.Xml;
using System.Text.RegularExpressions;
namespace WolfInv.com.WebRuleLib
{
    public class HtmlTagClass
    {
        public string TagName;
        public string KeyName;
        public string KeyValue;
        public string AttName;
        public string AttValue;
        public static HtmlTagClass getTagInfo(string html, string tag)
        {
            HtmlTagClass ret = new HtmlTagClass();
            string[] arr = tag.Split('|'); //p|class|lkjlfd 
            string tagName = arr[0];//tag p ，a，table ，td，div
            string tagKeyType = "";//class ,id,name
            string tagKeyValue = "";//
            string tagValAtt = "";
            if (arr.Length > 1)
            {
                tagKeyType = arr[1];
            }
            if (arr.Length > 2)
            {
                tagKeyValue = arr[2];//除去 tag 和 约束[id|name]的值
            }
            if (arr.Length > 3)
            {
                tagValAtt = tag.Substring(tagName.Length + 1 + tagKeyType.Length + 1 + tagKeyValue.Length + 1);
            }
            ret.TagName = tagName;
            ret.KeyName = tagKeyType;
            ret.KeyValue = tagKeyValue;
            ret.AttName = tagValAtt;
            string strReg = string.Format(@"<{0}[^>]+>", tagName);
            Regex reg = new Regex(strReg);//查找所有的符合标志的记录
            MatchCollection matchs = reg.Matches(html);
            XmlDocument xmldoc = new XmlDocument();
            for (int i = 0; i < matchs.Count; i++)
            {
                string xml = matchs[i].Value;

                try
                {
                    xmldoc.LoadXml(xml);
                }
                catch
                {
                    if (!xml.EndsWith("/>"))
                    {
                        xml = xml.Substring(0, xml.Length - 1) + "/>";
                    }
                    try
                    {
                        xmldoc.LoadXml(xml);
                    }
                    catch
                    {
                        continue;
                    }

                }
                XmlNode node = xmldoc.SelectSingleNode(string.Format("{0}[@{1}='{2}']", tagName, tagKeyType, tagKeyValue));
                if (node != null)
                {
                    ret.AttValue = node.Attributes[tagValAtt]?.Value;
                    return ret;
                }
            }
            return null;
        }


    }
}

using System.Collections.Generic;
using System.Xml;
using XmlProcess;

namespace WolfInv.com.JdUnionLib
{
    public class XmlSchemaClass
    {
        string _rootPath = "root";
        /// <summary>
        /// 数据根结点
        /// </summary>
        public string rootNodePath { get { return _rootPath; } set { _rootPath = value; } }

        string _dataItemName = "data";
        /// <summary>
        /// 数据项名
        /// </summary>
        public string dataItemName { get { return _dataItemName; } set { _dataItemName = value; } }

        string _codeItemName = "code";
        string _msgItemName = "msg";
        public string codeItemName
        {
            get { return _codeItemName; }
            set { _codeItemName = value; }
        }
        public string msgItemName
        {
            get { return _msgItemName; }
            set { _msgItemName = value; }
        }
        //totalCountItemName="totalCount" pageCountItemName="pages" pageSizeItemName="pageSize" pageIndexItemName="pageIndex"
        public string totalCountItemName { get; set; }
        public string pageCountItemName { get; set; }
        public string pageSizeItemName { get; set; }
        public string pageIndexName { get; set; }

        public Dictionary<string, TableGuider> TableList = new Dictionary<string, TableGuider>();

        public XmlSchemaClass(XmlDocument xml)
        {
            if (xml == null)
                return;
            XmlNode schemaNode =  xml.SelectSingleNode("req/Schema");
            rootNodePath = XmlUtil.GetSubNodeText(schemaNode, "@rootNodeName");
            dataItemName = XmlUtil.GetSubNodeText(schemaNode, "@dataItemName");
            codeItemName = XmlUtil.GetSubNodeText(schemaNode, "@codeItemName");
            msgItemName = XmlUtil.GetSubNodeText(schemaNode, "@msgItemName");
            totalCountItemName = XmlUtil.GetSubNodeText(schemaNode, "@totalCountItemName");
            pageCountItemName = XmlUtil.GetSubNodeText(schemaNode, "@pageCountItemName");
            pageSizeItemName = XmlUtil.GetSubNodeText(schemaNode, "@pageSizeItemName");
            pageIndexName = XmlUtil.GetSubNodeText(schemaNode, "@pageIndexItemName");
            XmlNodeList tabs =  xml.SelectNodes("Table");
            TableList = getTableGuiders(schemaNode);
        }

        Dictionary<string, TableGuider> getTableGuiders(XmlNode xmlSchema)
        {
            Dictionary<string, TableGuider> ret = new Dictionary<string, TableGuider>();
            if (xmlSchema == null)
            {
                return ret;
            }
            XmlNodeList nodes = xmlSchema.SelectNodes("Table");
            foreach (XmlNode node in nodes)
            {
                TableGuider tg = new TableGuider(node);
                if (!ret.ContainsKey(tg.TableName))
                {
                    ret.Add(tg.TableName, tg);
                }
            }
            return ret;
        }

    }


    public class TableGuider
    {
        public string TableName { get; set; }
        public string Key { get; set; }
        public string KeyValue;

        public string KeySplitor { get; set; }

        string _rootPath = "root";
        /// <summary>
        /// 数据根结点
        /// </summary>
        public string rootNodePath { get { return _rootPath; } set { _rootPath = value; } }

        string _dataItemName = "data";
        /// <summary>
        /// 数据项名
        /// </summary>
        public string dataItemName { get { return _dataItemName; } set { _dataItemName = value; } }
        public string NextRef { get; set; }
        public Dictionary<string, ResponseDataColumn> Columns = new Dictionary<string, ResponseDataColumn>();

        string _codeItemName = "code";
        string _msgItemName = "msg";
        public string codeItemName
        {
            get { return _codeItemName; }
            set { _codeItemName = value; }
        }
        public string msgItemName
        {
            get { return _msgItemName; }
            set { _msgItemName = value; }
        }
        public TableGuider() { }
        public TableGuider(XmlNode node)
        {
            LoadXml(node);
        }

        public void LoadXml(XmlNode node)
        {
            TableName = XmlUtil.GetSubNodeText(node, "@Name");
            Key = XmlUtil.GetSubNodeText(node, "@MainKey");
            NextRef = XmlUtil.GetSubNodeText(node, "@MainRef");
            KeySplitor = XmlUtil.GetSubNodeText(node, "@Split");
            string _tmp = XmlUtil.GetSubNodeText(node, "@rootNodePath");
            if (!string.IsNullOrEmpty(_tmp))
            {
                rootNodePath = _tmp;
            }
            _tmp = XmlUtil.GetSubNodeText(node, "@dataItemName");
            if (!string.IsNullOrEmpty(_tmp))
            {
                dataItemName = _tmp;
            }
            _tmp = XmlUtil.GetSubNodeText(node, "@codeItemName");
            if (!string.IsNullOrEmpty(_tmp))
            {
                codeItemName = _tmp;
            }
            _tmp = XmlUtil.GetSubNodeText(node, "@msgItemName");
            if (!string.IsNullOrEmpty(_tmp))
            {
                msgItemName = _tmp;
            }
            XmlNodeList colNodes = node.SelectNodes("Columns/Col");
            if (colNodes.Count == 0)
            {
                return;
            }
            Columns = new Dictionary<string, ResponseDataColumn>();
            foreach (XmlNode colNode in colNodes)
            {
                string strColName = XmlUtil.GetSubNodeText(colNode, "@name");
                if (Columns.ContainsKey(strColName))
                {
                    continue;
                }
                ResponseDataColumn jdc = new ResponseDataColumn();
                jdc.name = strColName;
                jdc.format = XmlUtil.GetSubNodeText(colNode, "@format");
                jdc.xPath = XmlUtil.GetSubNodeText(colNode, "@xPath");
                jdc.dataType = XmlUtil.GetSubNodeText(colNode, "@dataType");
                string outXml = XmlUtil.GetSubNodeText(colNode, "@outXml");
                if(!string.IsNullOrEmpty(outXml) && (outXml == "1" || outXml.Trim().ToLower() == "true"))
                {
                    jdc.outXml = true;
                }
                Columns.Add(jdc.name, jdc);
            }
        }

    }

    public class ResponseDataColumn
    {
        public string name { get; set; }
        public string xPath { get; set; }
        public string format { get; set; }
        public string dataType { get; set; }

        public bool outXml { get; set; }
    }

}

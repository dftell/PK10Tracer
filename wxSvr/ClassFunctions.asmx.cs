using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;
using Business;
using CommFunction;
using BehindImplement;
using System.IO;
namespace BackOffice
{
	/// <summary>
	/// Summary description for ClassFunctions.
	/// </summary>
	public class ClassFunctions : System.Web.Services.WebService
	{
		public ClassFunctions()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		// WEB SERVICE EXAMPLE
		// The HelloWorld() example service returns the string Hello World
		// To build, uncomment the following lines then save and build the project
		// To test this web service, press F5

//		[WebMethod]
//		public string HelloWorld()
//		{
//			return "Hello World";
//		}

		[WebMethod]
		public XmlDocument GetClass(string file)
		{
			string str = null;
			try
			{
				System.IO.Stream st = new System.IO.MemoryStream();
				st = this.Context.Request.InputStream;
				byte[] bs = new byte[st.Length];
				st.Read(bs,0,(int)st.Length);
				str = System.Text.Encoding.ASCII.GetString(bs);
			}
			catch(Exception ce)
			{
				///--return null;
			}
			XmlDocument xmlreq = new XmlDocument();
			xmlreq.LoadXml("<root></root>");
			try
			{
				xmlreq.LoadXml(str);
			}
			catch(Exception ce)
			{
				///return xmlreq;
			}
			XmlDocument xml = new XmlDocument();
			xml.LoadXml("<root></root>");
			XmlNode root = xml.SelectSingleNode("root");
			ArrayList arr = Adapter.NSClass(file);
			if(arr == null) return xml;
			for(int i = 0 ;i < arr.Count ;i++)
			{
				XmlNode classnode = xml.CreateElement("class");
				Type type= arr[i] as Type;
				XmlAttribute name = xml.CreateAttribute("Name");
				XmlAttribute allname = xml.CreateAttribute("AllName");
				classnode.Attributes.Append(name);
				classnode.Attributes.Append(allname);
				name.Value = type.Name;
				allname.Value  = type.FullName;
				//classnode.InnerText =type.Name;
				root.AppendChild(classnode);
			}
			return xml;
		}


		[WebMethod]
		public XmlDocument GetXml(string xml)
		{
			XmlDocument XmlDoc = new XmlDocument();
			XmlDoc.LoadXml(xml);
			return XmlDoc;
		}

		[WebMethod] 
		public XmlDocument GetFunctionByClassName(string filename,string classname,string onlydecl)
		{
			if(classname == null || classname.Length == 0)
			{
				classname = null;
			}
            return Adapter.GetFunctions(filename,classname,onlydecl == "1" ? true : false);
		}

		[WebMethod]
		public XmlDocument GetType(string type)
		{
			XmlDocument xml = new XmlDocument();
			xml.LoadXml("<root>" + Comm.ConvertSimpleType(type) +"</root>");
			return xml;
		}

		[WebMethod]
		public XmlDocument GetDataSet(int cnt)
		{
			DataMappingrefinfo dmri = new DataMappingrefinfo();
			dmri.cmpr = "=";
			dmri.col ="jdkjdl";
			dmri.operate = "and";
			dmri.refcol = "kdjdl";
			dmri.reftable ="remus";
			DataMappingrefinfos dmrefs = new DataMappingrefinfos();
			dmrefs.Add(dmri);
			dmrefs.Add(dmri);
			DataMappingColumn col = new DataMappingColumn();
			col.name = "column";
			col.refs = dmrefs;
			XmlDocument xml = col.ToSingleXml();
			DataMappingColumn col1 = new DataMappingColumn();
			col1.GetFromXml(xml);
			return null;
		}
		[WebMethod]
		public XmlDocument GetFolder(string path)
		{
////			Folder_Builder fb = new Folder_Builder(path);
////			if(fb.Root == null)
////			{
////				return null;
////			}
			XmlDocument xml = Folder_Builder.GetFolder(path);
			return xml;
		}

        [WebMethod]
        public XmlDocument GetWebFolder(string path)
        {
            string phyPath = new System.Web.UI.Page().Server.MapPath(path);
            XmlDocument xml = Folder_Builder.GetFolder(phyPath);
            return xml;
        }

        [WebMethod]
        public XmlDocument UploadFile(string filepath, Stream FileSteam)
        {
            string phyPath = new System.Web.UI.Page().Server.MapPath(filepath);
            XmlDocument ret = Folder_Builder.SaveFile(phyPath, FileSteam);
            return ret;
        }

		[WebMethod]
		public XmlDocument GetTableData(string cols,string vals,string opts,string split,string ignore,int topcnt)
		{
			ignore = ignore==null?ignore:"";
			TableClass tb;
			baseman rm = new baseman();
			tb = rm;
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(tb.GetDataSet(cols,vals,opts,split,ignore=="1"?true:false,topcnt).GetXml());
			return xml;
		}

		[WebMethod]
		public XmlDocument GetTableDataBySql(string sql)
		{
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(TableClass.GetDataSetBySqlStatic(sql).GetXml());
			return xml;
		}

		[WebMethod]
		public XmlDocument GetFileString(string path)
		{
			return Folder_Builder.GetFileString(path);
			
		}
	}
}

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;
using Business;
using BehindImplement;
using CommAccessDB;
namespace chinazhou_plus
{
	/// <summary>
	/// GetXml 的摘要说明。
	/// </summary>
	public class GetXml : System.Web.Services.WebService
	{
		public GetXml()
		{
			//CODEGEN: 该调用是 ASP.NET Web 服务设计器所必需的
			InitializeComponent();
		}

		#region 组件设计器生成的代码
		
		//Web 服务设计器所必需的
		private IContainer components = null;
				
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// 清理所有正在使用的资源。
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

		// WEB 服务示例
		// HelloWorld() 示例服务返回字符串 Hello World
		// 若要生成，请取消注释下列行，然后保存并生成项目
		// 若要测试此 Web 服务，请按 F5 键

//		[WebMethod]
//		public string HelloWorld()
//		{
//			return "Hello World";
//		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="AssemName"></param>
		/// <param name="ClassName"></param>
		/// <param name="MethodName"></param>
		/// <param name="IsStatic"></param>
		/// <param name="method"></param>
		/// <param name="paramlist">
		/// <Method AssemName="asem name"  ClassName="class name" IsStatic="1|0" MethodName="method name">
		/// <Params>
		///		[<Param Name="name" Type="type">value</Param>]..
		/// </Params>
		/// </Method>
		/// </param>
		/// <returns>
		/// <root><Response>content</Response><Message>error message</Message></root></returns>

		
		[WebMethod]
		public XmlDocument GetDataSet(string AssemName,string ClassName,string MethodName,string IsStatic,string paramlist)
		{
			XmlDocument ret = new XmlDocument();
			if(AssemName == null || AssemName.Trim()=="")
			{
				return ErrorMsg("please entry the assem name!");
			}
			if(ClassName == null || ClassName.Trim()=="")
			{
				return ErrorMsg("please entry the class name!");
			}
			if(MethodName == null || MethodName.Trim()=="")
			{
				return ErrorMsg("please entry the Method Name !");
			}
			if(IsStatic == null || IsStatic.Trim()=="")
			{
				IsStatic = "0";  // default is 0
			}

			XmlNode xroot = ret.CreateElement("Method");
			ret.AppendChild(xroot);
			XmlAttribute xName = ret.CreateAttribute("AssemName");
			XmlAttribute xClassName = ret.CreateAttribute("ClassName");
			XmlAttribute xStatic = ret.CreateAttribute("IsStatic");
			XmlAttribute xMethod = ret.CreateAttribute("MethodName");
			xName.Value  = AssemName;
			xClassName.Value = ClassName;
			xStatic.Value = IsStatic;
			xMethod.Value = MethodName;
			xroot.Attributes.Append (xMethod);
			xroot.Attributes.Append (xName);
			xroot.Attributes.Append (xClassName);
			xroot.Attributes.Append (xStatic);
			xroot.AppendChild(ret.CreateElement("Params"));
			XmlNode xparams = xroot.SelectSingleNode("Params");
			string[] arrParams  = paramlist.Split(';');
			for( int i = 0 ; i<arrParams.Length ; i++ )
			{
				string[] paraminfo = arrParams[i].Split('^');
				XmlNode param = ret.CreateElement("Param");
				xparams.AppendChild(param);
				XmlAttribute xatt_name = ret.CreateAttribute("Name");
				XmlAttribute xatt_type = ret.CreateAttribute("Type");
				xatt_name.Value = paraminfo[0].Trim();
				xatt_type.Value = paraminfo[1].Trim();
				param.Attributes.Append(xatt_name);
				param.Attributes.Append(xatt_type);
				if(paraminfo[2].Trim().ToLower() != "xml" && paraminfo[2].Trim().ToLower() != "dataset")
				{
					param.InnerText  = paraminfo[2].Trim();
				}
				else
				{
					XmlDocument xml = new XmlDocument();
					xml.LoadXml(paraminfo[2].Trim());
					XmlNode txtnode = ret.ImportNode(xml.SelectSingleNode(".").Clone(),true);
                    param.AppendChild(txtnode);
				}
			}
			try
			{
				//root.AppendChild(xdoc.ImportNode(xreq.SelectSingleNode("/*"),true));
				//XmlDocument ret = Adapter.CallMethod(xreq);
				Method mth = new Method(ret);
				XmlDocument retm = mth.Invoke() as XmlDocument;
				return retm;
				//root.AppendChild(xdoc.ImportNode(ret.SelectSingleNode("/*"),true));
			}
			catch(Exception ce)
			{
				return this.ErrorMsg(ce.Message);
			}


			
		}
       

		[WebMethod]
		public string GetCallStack()
		{
			return CommAccessDB.AccessDB.VirusCallStackInfo;
		}
		XmlDocument ErrorMsg(string err)
		{
			string reterr="<root><Response></Response><Message>{0}</Message></root></returns>";
			XmlDocument ret = new XmlDocument();
			ret.LoadXml(string.Format(reterr,err));
			return ret;
		}
		

	}
}

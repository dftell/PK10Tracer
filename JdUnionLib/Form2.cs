using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WolfInv.Com.JsLib;

namespace WolfInv.com.JdUnionLib
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            bool inited = JdUnion_BaseClass.Inited;
        }

        private void btn_request_Click(object sender, EventArgs e)
        {
            this.txt_url.Text = "";
            this.txt_result.Text = "正在准备请求！";
            ////jdy_GlbObject.ResetAccess();
            ////string ret = jdy_GlbObject.Access_token;
            ////this.txt_req_name.Text = ret;
            ////this.txt_bdId.Text = jdy_GlbObject.bdId.ToString();

            JdUnion_Bussiness_Class jdy = ddl_className.Tag as JdUnion_Bussiness_Class;
            if(jdy == null)
            {
                return;
            }
            //////Type cls = JdUnion_GlbObject.AllModuleClass[ddl_className.Text];
            //////object obj = Activator.CreateInstance(cls);// as;
            //////JdUnion_Bussiness_Class jdy = obj as JdUnion_Bussiness_Class;// as
            //JDYSCM_Class jdy = jdy_GlbObject.AllModuleClass[ddl_className.SelectedValue.ToString()];
            if (jdy == null)
                return;
            jdy.InitClass(JdUnion_GlbObject.mlist[ddl_className.Text]);
            this.txt_app_key.Text =jdy.app_key ;
            this.txt_app_secret.Text = jdy.app_secret;
            this.txt_access_token.Text = jdy.access_token;
            //jdy.timestamp = this.txt_timestamp.Text.Trim();
            //jdy.params_360buy.Clear();
            jdy.sign = null;//必须要置空
            
            if (this.txt_params_1_val.Text.Trim().Length > 0)
            {
                jdy.setBussiessItems(this.txt_params_1_key.Text, this.txt_params_1_val.Text);
            }
            if (this.txt_params_2_val.Text.Trim().Length > 0)
            {
                jdy.setBussiessItems(this.txt_params_1_key.Text, this.txt_params_1_val.Text);
            }
            if (this.txt_params_3_val.Text.Trim().Length > 0)
            {
                jdy.setBussiessItems(this.txt_params_1_key.Text, this.txt_params_1_val.Text);
            }
            if (txt_PostData.Text.Trim().Length > 0)
            {
                if (jdy.params_360buy.Count == 0)
                {
                    jdy.params_360buy.Add(this.txt_params_1_key.Text, this.txt_PostData.Text);
                }
            }
            if (jdy is JdUnion_Bussiness_List_Class)
            {
                (jdy as JdUnion_Bussiness_List_Class).pager = new JdUnion_Bussiness_List_Class.JdUnion_Bussiness_Filter_Class();
                (jdy as JdUnion_Bussiness_List_Class).pager.pageIndex = int.Parse(txt_PageNo.Text);
                (jdy as JdUnion_Bussiness_List_Class).pager.pageSize = int.Parse(txt_PageSize.Text);
            }


            
            //this.txt_PostData.Text  = 
            //jdy.InitRequestJson();
            ////if (jdy is JdUnion_Bussiness_List_Class)
            ////{
            ////    (jdy as JdUnion_Bussiness_List_Class).filter = new JdUnion_Bussiness_List_Class.JdUnion_Bussiness_Filter_Class();
            ////    (jdy as JdUnion_Bussiness_List_Class).filter.pageSize = int.Parse(txt_PageSize.Text);
            ////    (jdy as JdUnion_Bussiness_List_Class).filter.page = int.Parse(txt_PageNo.Text);
            ////    jdy.Req_PostData = "{\"filter\":" + (jdy as JdUnion_Bussiness_List_Class).filter.ToJson().Replace("null", "\"\"") + "}";
            ////}
            //this.txt_url.Text = jdy.getUrl();
            //this.txt_PostData.Text = jdy.Req_PostData;

            //jdy.Req_PostData = this.txt_PostData.Text;
            this.txt_url.Text = jdy.getUrl();
            //this.txt_result.Text = this.chkbox_Post.Checked?jdy.PostRequest():jdy.GetRequest();
            string msg = null;
            XmlDocument xmldoc = null;
            XmlDocument schema = null;
            bool succ = jdy.getXmlData(ref xmldoc,ref schema, ref msg,false,false);
            if (xmldoc != null)
                this.txt_result.Text = xmldoc.OuterXml;

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string ret = JdUnion_GlbObject.Access_token;
            this.txt_access_token.Text = ret;
            
            //this.txt_bdId.Text = JdUnion_GlbObject.dbId;
            DataTable dt = new DataTable();
            dt.Columns.Add("text");
            dt.Columns.Add("value");
            JdUnion_GlbObject.AllModuleClass.Values.ToList().ForEach(a =>
            {
                DataRow dr = dt.NewRow();
                dr["text"] = a.Name;
                dr["value"] = a;
                dt.Rows.Add(dr);
            });
            this.ddl_className.DisplayMember = "text";
            this.ddl_className.ValueMember = "value";
            this.ddl_className.DataSource = dt;
            this.ddl_className.Tag = dt;

        }

        private void ddl_className_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_className.SelectedIndex < 0)
            {
                return;
            }
          
            Type cls = JdUnion_GlbObject.AllModuleClass[ddl_className.Text];
            object obj = Activator.CreateInstance(cls);// as;
            JdUnion_Bussiness_Class jdy = obj as JdUnion_Bussiness_Class;// as
            //JDYSCM_Class jdy = jdy_GlbObject.AllModuleClass[ddl_className.SelectedValue.ToString()];
            if (jdy == null)
                return;
            JdUnion_ModuleClass jm = JdUnion_GlbObject.modules.Modules.FindLast(a => a.ClassName == ddl_className.Text);
            if(jm == null)
            {
                return;
            }
            jdy.InitClass(jm);
            if(jdy.defaultRequestJson!=null)
            {
                this.txt_PostData.Text = jdy.defaultRequestJson;
            }
            this.ddl_className.Tag = jdy;
            return;
            jdy.InitClass(JdUnion_GlbObject.mlist[ddl_className.Text]);
            ////jdy.app_key = this.txt_app_key.Text.Trim();
            ////jdy.app_secret = this.txt_app_secret.Text.Trim();
            ////jdy.access_token = this.txt_access_token.Text.Trim();
            if(this.txt_params_1_val.Text.Trim().Length>0)
            {
                jdy.params_360buy.Add(this.txt_params_1_key.Text.Trim(), this.txt_params_1_val.Text.Trim());
            }
            if (this.txt_params_2_val.Text.Trim().Length > 0)
            {
                jdy.params_360buy.Add(this.txt_params_2_key.Text.Trim(), this.txt_params_2_val.Text.Trim());
            }
            if (this.txt_params_3_val.Text.Trim().Length > 0)
            {
                jdy.params_360buy.Add(this.txt_params_3_key.Text.Trim(), this.txt_params_3_val.Text.Trim());
            }
            jdy.InitRequestJson();
            if (jdy is JdUnion_Bussiness_List_Class)
            {
                ////(jdy as JdUnion_Bussiness_List_Class).filter = new JdUnion_Bussiness_List_Class.JdUnion_Bussiness_Filter_Class();
                ////(jdy as JdUnion_Bussiness_List_Class).filter.pageSize = int.Parse(txt_PageSize.Text);
                ////(jdy as JdUnion_Bussiness_List_Class).filter.pageIndex = int.Parse(txt_PageNo.Text);
                ////jdy.Req_PostData = "{\"filter\":" + (jdy as JdUnion_Bussiness_List_Class).filter.ToJson().Replace("null", "\"\"") + "}";
            }
            this.txt_url.Text = jdy.getUrl();
            this.txt_PostData.Text = jdy.Req_PostData;
            
        }

        private void txt_url_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

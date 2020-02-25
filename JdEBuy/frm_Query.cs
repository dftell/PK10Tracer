using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using WolfInv.com.JdUnionLib;

namespace JdEBuy
{
    public partial class frm_Query : Form
    {
        public string txtCondition { get; set; }
        public frm_Query()
        {
            InitializeComponent();
            
        }


        public List<JdGoodSummayInfoItemClass> Query(string strCondition)
        {
            List<JdGoodSummayInfoItemClass> ret = new List<JdGoodSummayInfoItemClass>();

            return ret;
        }

        private void frm_Query_Load(object sender, EventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate("https://union.jd.com/proManager/index?pageNo=1");
        }
    }
}

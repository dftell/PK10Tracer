using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    
    public partial class TranslateFrm : Form
    {
        public bool Status = false;
        public TranslateFrm()
        {
            InitializeComponent();
            string strUrl = Application.ExecutablePath + @"\Model.htm";
            //this.webBrowser1.Url = new Uri();
            this.webBrowser1.Navigate(strUrl);
            this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }


    }

    
}

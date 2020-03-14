using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExchangeTermial
{
    public partial class testWeb : Form
    {
        public testWeb()
        {
            InitializeComponent();
        }

        private void testWeb_Load(object sender, EventArgs e)
        {
            try
            {
                //this.webBrowser1.Navigate("https://4562070.com",false);
                //this.webBrowser1.Navigate("http://www.baidu.com", false);
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.webBrowser1.Navigate(this.textBox1.Text,false);
                //this.webBrowser1.Navigate("http://www.baidu.com", false);
            }
            catch (Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }
    }
}

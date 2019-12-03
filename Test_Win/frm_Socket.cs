using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.Com.SocketLib;
namespace Test_Win
{
    public partial class frm_Socket : Form
    {
        TcpClientSocketAsync client;
        int buffsize = 1024 * 1024 * 2;
        public frm_Socket()
        {
            InitializeComponent();
            client = new TcpClientSocketAsync(buffsize);
            client.recvMessageEvent += (string a)=>{
                this.txt_receive.Text = a;
            };
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            client.ConnectedImmSendMsg = this.txt_send.Text;
            client.Start(this.txt_ip.Text, int.Parse(this.txt_port.Text));
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            client.SendMessage(this.txt_send.Text);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Leestar54.WeChat.WebAPI.Modal;
using Leestar54.WeChat.WebAPI;
using Leestar54.WeChat.WebAPI.Modal.Response;

namespace WolfInv.com.WXMsgCom
{
    delegate void SetContactViewList(ListView lv,List<Contact> contacts);
    delegate void SetMsgViewList(RichTextBox lv, List<AddMsg>  msgs);
    public partial class frm_MainWin : Form
    {
        Client client;
        Dictionary<string, Contact> AllUsers;
        string ToUser;
        Dictionary<string, bool> AllRespUser;
        public frm_MainWin(Client ccc)
        {
            client = ccc;
            this.AllUsers = new Dictionary<string, Contact>();
            AllRespUser = new Dictionary<string, bool>();
            InitializeComponent();
            
            //////Program.wxobj = new WXUtils(refreshQRCode,(a,b,c)=> {
            //////    this.tss_Status.Text = a;
            //////    this.tss_Msg.Text = string.Format("{0}:{1}", b,c);

            //////});
            //////Program.wxobj.ReceivedMsg += RefreshMsg;

        }

        

        public void RefreshMsg(object sender,TEventArgs<List<AddMsg>> tmsg)
        {
            WebInterfaceClass.ClientValid = true;
            List<AddMsg> msg = tmsg.Result;
            txt_MsgList.Invoke(new SetMsgViewList(RefreshMsg), new object[] { this.txt_MsgList, msg });
        }

        public void RefreshMsg(RichTextBox tbox, List<AddMsg> msg)
        {
            
            List<string> msgs = tbox.Lines.ToList();
            for (int i = 0; i < msg.Count; i++)
            {
                AddMsg wxmsg = msg[i];
                if(wxmsg.MsgType != MsgType.MM_DATA_TEXT)
                {
                    continue;
                }
                string fromName = wxmsg.FromUserName;
                string NickName = fromName;
                if (this.AllUsers.ContainsKey(fromName))
                {
                    ////if(fromName.StartsWith("@@"))
                    ////{
                    ////    client.GetBatchGetContactAsync(string.Join(",",this.AllUsers[fromName].MemberDict.Keys.ToArray()),fromName);
                    ////}
                    NickName = this.AllUsers[fromName].NickName;
                }
                else
                {
                    client.GetBatchGetContactAsync(fromName);
                    continue;
                }
                string ToName = wxmsg.ToUserName;
                if(this.AllUsers.ContainsKey(ToName))
                {
                    ToName = this.AllUsers[ToName].NickName;
                }
                
                string strMsg = "";
                if(fromName.StartsWith("@@"))
                {

                    string[] content = wxmsg.Content.Split(new string[] { ":<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                    string memName = "";
                    if (this.AllUsers.ContainsKey(content[0]))
                    {
                        memName = this.AllUsers[content[0]].NickName;
                    }
                    
                    strMsg = string.Format("[{0}]{1}:{2}", NickName, memName, string.Join("",content.Skip(1).ToArray()));
                }
                else
                {
                    strMsg = string.Format("{0}:{1}", NickName,  wxmsg.Content);
                }
                
                msgs.Add(strMsg);
            }
            tbox.Lines = msgs.ToArray(); 
            tbox.Focus();
            tbox.Select(tbox.Text.Length, 0);
            tbox.ScrollToCaret();
        }


        private void frm_MainWin_Load(object sender, EventArgs e)
        {
            //InitLoginTimer(true);

        }
        
        public void RefreshContact(object sender, TEventArgs<List<Contact>> tcontacts)
        {
            this.AllUsers = WebInterfaceClass.contactDict;
            List<Contact> contacts = this.AllUsers.Values.ToList();
            listView1.Invoke(new SetContactViewList(RefreshContact),new object[] { listView1, contacts });
        }
 
        

        void RefreshContact(ListView lv, List<Contact> contacts)
        {
            lock (AllUsers)
            {
                lv.Items.Clear();
                for (int i = 0; i < contacts.Count; i++)
                {

                    ListViewItem lvi = new ListViewItem(i.ToString());//
                    // new string[] { contacts[i].NickName, contacts[i].UserName, contacts[i].UserName.Substring(0, 2) == "@@" ? "群" : "好友" });
                    lvi.SubItems.Add(contacts[i].NickName);
                    lvi.SubItems.Add(contacts[i].RemarkName);
                    lvi.SubItems.Add(contacts[i].UserName);
                    lvi.SubItems.Add(contacts[i].UserName.Substring(0, 2) == "@@" ? "群" : "好友");
                    lv.Items.Add(lvi);
                }
                lv.Tag = contacts;
            }
        }


        void SendMsg(string str,string toUser)
        {
            SendMsgResponse smr =  client.SendMsg(str, toUser);
            if (smr.BaseResponse.Ret == 0)
            {
                AddMsg msg = new AddMsg();
                msg.FromUserName = client.CurrentUser.UserName;
                msg.ToUserName = toUser;
                msg.Content = str;
                msg.MsgType = MsgType.MM_DATA_TEXT;
                this.RefreshMsg(null, new TEventArgs<List<AddMsg>>(new AddMsg[]{ msg}.ToList()));
                this.tss_Msg.Text = "发送成功！";
                this.txt_msg.Text = "";
            }
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            if(this.ToUser == null)
            {
                this.tss_Msg.Text = "发送用户为空！";
                return;
            }
            if(this.ddl_ToUser.SelectedIndex != -1)
            {
                this.ToUser = this.ddl_ToUser.SelectedValue.ToString();
            }
            if(!this.AllUsers.ContainsKey(this.ToUser))
            {
                this.tss_Msg.Text = "发送用户已删除！";
                return;
            }
            SendMsg(this.txt_msg.Text, this.ToUser);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count != 1)
                return;
            List<Contact> contacts = this.listView1.Tag as List<Contact>;
            if(contacts == null)
            {
                this.tss_Msg.Text = "联系人列表为空！";
                return;
            }
            this.ToUser = contacts[this.listView1.SelectedIndices[0]].UserName;
            this.ddl_ToUser.Text = this.ToUser;
            ddl_ToUser_KeyUp(null, null);
            ddl_ToUser.SelectedIndex = 0;
            //this.listView1.SelectedItems[0].SubItems[2].
            Clipboard.SetDataObject(this.ToUser, true);
        }

        private void ddl_ToUser_KeyUp(object sender, KeyEventArgs e)
        {
            List<Contact> list = WebInterfaceClass.getContactListByName(ddl_ToUser.Text);
            if(list.Count>0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("name");
                dt.Columns.Add("val");
                for (int i = 0; i < list.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr.ItemArray = new string[] { string.Format("{0}|{1}|{2}|{3}",list[i].NickName,list[i].Province,list[i].City,list[i].Signature), list[i].UserName };
                    dt.Rows.Add(dr);
                }
                ddl_ToUser.DisplayMember = "name";
                ddl_ToUser.ValueMember = "val";
                ddl_ToUser.DataSource = dt;
                if(list.Count == 1)
                {
                    ddl_ToUser.SelectedIndex = 0;
                    return;
                }
                ddl_ToUser.SelectedIndex = -1;
            }
        }

        
    }
}

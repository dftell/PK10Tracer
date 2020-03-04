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
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.WXMessageLib;
namespace WolfInv.com.WXMsgCom
{
    delegate void SetContactViewList(ListView lv,List<Contact> contacts);
    delegate void SetMsgViewList(ListView lv, List<AddMsg>  msgs);
    delegate void SetMsgListView(ListView lv, List<wxMessageClass> msgs);
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

        public void RefreshMsg(object sender,List<wxMessageClass> msgs)
        {
            WebInterfaceClass.ClientValid = true;
            listView_msg.Invoke(new SetMsgListView(RefreshMsg), new object[] { this.listView_msg, msgs });
        }

        public void RefreshMsg(object sender,TEventArgs<List<AddMsg>> tmsg)
        {
            WebInterfaceClass.ClientValid = true;
            List<AddMsg> msg = tmsg.Result;
            listView_msg.Invoke(new SetMsgViewList(RefreshMsg), new object[] { this.listView_msg, msg });
        }

        public void RefreshMsg(ListView msglist, List<wxMessageClass> msg)
        {
            try
            {
                for (int i = 0; i < msg.Count; i++)
                {
                    wxMessageClass wxmsg = msg[i];
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = wxmsg.CreateTime.ToString();
                    string tousers = (wxmsg.AtMemberNikeName == null || wxmsg.AtMemberNikeName.Length == 0) ? "" : string.Join(";", wxmsg.AtMemberNikeName);
                    lvi.SubItems.AddRange(new string[] { wxmsg.FromNikeName ?? "", wxmsg.FromMemberNikeName ?? "", (wxmsg.IsAtToMe ? ">>" : "") + tousers, wxmsg.Msg });
                    msglist.Items.Add(lvi);
                }
                int cnt = msglist.Items.Count;
                if (cnt == 0)
                    return;
                msglist.Items[cnt - 1].EnsureVisible();
            }
            catch(Exception ce)
            {
                MessageBox.Show(string.Format("{0}:{1}",ce.Message,ce.StackTrace));
            }
        }

        public void RefreshMsg(ListView msglist, List<AddMsg> msg)
        {

            //List<string> msgs = tbox.Lines.ToList();
            List<ListViewItem> msgs = new List<ListViewItem>();
            for (int i = 0; i < msg.Count; i++)
            {
                AddMsg wxmsg = msg[i];
                if(wxmsg.MsgType != MsgType.MM_DATA_TEXT)
                {
                    continue;
                }
                string fromName = wxmsg.FromUserName;
                string NickName = fromName;
                if (this.AllUsers.ContainsKey(fromName))//联系人发来的
                {
                    ////if(fromName.StartsWith("@@"))
                    ////{
                    ////    client.GetBatchGetContactAsync(string.Join(",",this.AllUsers[fromName].MemberDict.Keys.ToArray()),fromName);
                    ////}
                    NickName = this.AllUsers[fromName].NickName;
                    if(this.AllUsers[fromName].MemberList.Count>0)//是群
                    {
                        
                        if(!this.AllUsers[fromName].DisplayNikeName)//所有的群都显示昵称
                        {
                            client.GetBatchGetContactAsync(string.Join(",",AllUsers[fromName].MemberList.Select(a=>a.UserName)), fromName);
                            this.AllUsers[fromName].DisplayNikeName = true;
                        }
                    }
                }
                else
                {
                    
                    client.GetBatchGetContactAsync(fromName);
                    continue;
                }
                string ToName = wxmsg.ToUserName;
                string ToNameNike = null;
                if(this.AllUsers.ContainsKey(ToName))
                {
                    ToNameNike = this.AllUsers[ToName].NickName;
                }

                string[] strMsg = new string[5];
                if(fromName.StartsWith("@@"))
                {

                    string[] content = wxmsg.Content.Split(new string[] { ":<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                    content[1] = content[1].Replace("<br/>", "\r\n");
                    string memName = "";
                    if (1==2 && this.AllUsers.ContainsKey(content[0]))
                    {
                        memName = this.AllUsers[content[0]].DisplayName;
                        if (string.IsNullOrEmpty(memName))
                        {
                            memName = this.AllUsers[content[0]].NickName;
                        }
                    }
                    else
                    {
                        if (this.AllUsers[fromName].MemberDict.ContainsKey(content[0]))
                            memName =  this.AllUsers[fromName].MemberDict[content[0]].DisplayName;
                        if(string.IsNullOrEmpty(memName))
                        {
                            memName = this.AllUsers[fromName].MemberDict[content[0]].NickName;
                        }
                    }

                    //strMsg = string.Format("[{0}]{1}:{2}", NickName, memName, string.Join("",content.Skip(1).ToArray()));
                    strMsg = new string[] { DateTime.Now.ToShortTimeString(), NickName, memName, "",string.Join("", content.Skip(1).ToArray()) };
                }
                else
                {
                    if (ToName.StartsWith("@@"))
                    {
                        //strMsg = string.Format("{0}<{1}>:{2}", ToNameNike, NickName, wxmsg.Content.Replace("<br/>", "\r\t"));
                        strMsg = new string[] { DateTime.Now.ToShortTimeString(), ToNameNike, NickName,"", wxmsg.Content.Replace("<br/>", "\r\t") };
                    }
                    else
                    {
                        //strMsg = string.Format("{0}:{1}", NickName, wxmsg.Content.Replace("<br/>", "\r\t"));
                        strMsg = new string[] { DateTime.Now.ToShortTimeString(), NickName,"", "", wxmsg.Content.Replace("<br/>", "\r\t") };
                    }
                }
                
                msgs.Add(new ListViewItem(strMsg));
            }
            //tbox.Lines = msgs.ToArray(); 
            //tbox.Focus();
            //tbox.Select(tbox.Text.Length, 0);
            //tbox.ScrollToCaret();
            
            listView_msg.Items.AddRange(msgs.ToArray());
            //listView_msg.sel
            int cnt = listView_msg.Items.Count;
            listView_msg.MultiSelect = false;
            //listView_msg.Items[cnt - 1].Selected = true;
            //listView_msg.Items[cnt - 1].Focused = true;
            if(cnt>0)
                listView_msg.Items[cnt - 1].EnsureVisible() ;
        }


        private void frm_MainWin_Load(object sender, EventArgs e)
        {
            //InitLoginTimer(true);

        }

        public void RefreshContact(object sender, TEventArgs<List<Contact>> tcontacts)
        {
            try
            {
                this.AllUsers = WebInterfaceClass.contactDict;
                List<Contact> contacts = this.AllUsers.Values.ToList();
                listView1.Invoke(new SetContactViewList(RefreshContact), new object[] { listView1, contacts });

            }
            catch (Exception ce)
            {
                MessageBox.Show(string.Format("{0}:{1}", ce.Message, ce.StackTrace));
            }
        }
 
        

        void RefreshContact(ListView lv, List<Contact> contacts)
        {
            try
            {
                lock (AllUsers)
                {
                    lv.Items.Clear();
                    for (int i = 0; i < contacts.Count; i++)
                    {
                        if(!contacts[i].UserName.StartsWith("@@"))
                        {
                            continue;
                        }
                        ListViewItem lvi = new ListViewItem(i.ToString());
                        lvi.Tag = contacts[i];
                        //
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
            catch(Exception ce)
            {
                MessageBox.Show(string.Format("{0}:{1}", ce.Message, ce.StackTrace));
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
            Contact ct = this.listView1.SelectedItems[0].Tag as Contact;
            this.ToUser = ct.UserName;
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

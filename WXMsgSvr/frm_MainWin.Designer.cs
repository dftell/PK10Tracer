namespace WolfInv.com.WXMsgCom
{
    partial class frm_MainWin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_msg = new System.Windows.Forms.TabPage();
            this.txt_MsgList = new System.Windows.Forms.RichTextBox();
            this.tabPage_contact = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.ddl_ToUser = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_msg = new System.Windows.Forms.TextBox();
            this.btn_Send = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tss_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.tss_Msg = new System.Windows.Forms.ToolStripStatusLabel();
            this.tss_Counter = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage_msg.SuspendLayout();
            this.tabPage_contact.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel1MinSize = 200;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2MinSize = 100;
            this.splitContainer1.Size = new System.Drawing.Size(1219, 807);
            this.splitContainer1.SplitterDistance = 562;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_msg);
            this.tabControl1.Controls.Add(this.tabPage_contact);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1219, 562);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage_msg
            // 
            this.tabPage_msg.Controls.Add(this.txt_MsgList);
            this.tabPage_msg.Location = new System.Drawing.Point(8, 39);
            this.tabPage_msg.Name = "tabPage_msg";
            this.tabPage_msg.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage_msg.Size = new System.Drawing.Size(1203, 515);
            this.tabPage_msg.TabIndex = 0;
            this.tabPage_msg.Text = "消息";
            this.tabPage_msg.UseVisualStyleBackColor = true;
            // 
            // txt_MsgList
            // 
            this.txt_MsgList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_MsgList.Location = new System.Drawing.Point(3, 3);
            this.txt_MsgList.Name = "txt_MsgList";
            this.txt_MsgList.ReadOnly = true;
            this.txt_MsgList.Size = new System.Drawing.Size(1197, 509);
            this.txt_MsgList.TabIndex = 0;
            this.txt_MsgList.Text = "";
            // 
            // tabPage_contact
            // 
            this.tabPage_contact.Controls.Add(this.listView1);
            this.tabPage_contact.Location = new System.Drawing.Point(8, 39);
            this.tabPage_contact.Name = "tabPage_contact";
            this.tabPage_contact.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage_contact.Size = new System.Drawing.Size(1203, 516);
            this.tabPage_contact.TabIndex = 1;
            this.tabPage_contact.Text = "联系人";
            this.tabPage_contact.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1197, 510);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "编号";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "昵称";
            this.columnHeader1.Width = 99;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "备注";
            this.columnHeader4.Width = 173;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "UID";
            this.columnHeader2.Width = 493;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "类型";
            this.columnHeader3.Width = 150;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.ddl_ToUser);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txt_msg);
            this.panel1.Controls.Add(this.btn_Send);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1219, 241);
            this.panel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "好友";
            // 
            // ddl_ToUser
            // 
            this.ddl_ToUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddl_ToUser.FormattingEnabled = true;
            this.ddl_ToUser.Location = new System.Drawing.Point(77, 15);
            this.ddl_ToUser.Name = "ddl_ToUser";
            this.ddl_ToUser.Size = new System.Drawing.Size(311, 32);
            this.ddl_ToUser.TabIndex = 3;
            this.ddl_ToUser.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ddl_ToUser_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "消息";
            // 
            // txt_msg
            // 
            this.txt_msg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_msg.Location = new System.Drawing.Point(75, 68);
            this.txt_msg.Multiline = true;
            this.txt_msg.Name = "txt_msg";
            this.txt_msg.Size = new System.Drawing.Size(1028, 166);
            this.txt_msg.TabIndex = 1;
            // 
            // btn_Send
            // 
            this.btn_Send.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Send.Location = new System.Drawing.Point(1109, 68);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(91, 168);
            this.btn_Send.TabIndex = 0;
            this.btn_Send.Text = "发送";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tss_Status,
            this.tss_Msg,
            this.tss_Counter});
            this.statusStrip1.Location = new System.Drawing.Point(0, 822);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 13, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1219, 38);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tss_Status
            // 
            this.tss_Status.AutoSize = false;
            this.tss_Status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tss_Status.Name = "tss_Status";
            this.tss_Status.Size = new System.Drawing.Size(300, 33);
            this.tss_Status.Text = "状态";
            this.tss_Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tss_Msg
            // 
            this.tss_Msg.AutoSize = false;
            this.tss_Msg.Name = "tss_Msg";
            this.tss_Msg.Size = new System.Drawing.Size(500, 33);
            this.tss_Msg.Text = "消息";
            // 
            // tss_Counter
            // 
            this.tss_Counter.AutoSize = false;
            this.tss_Counter.Name = "tss_Counter";
            this.tss_Counter.Size = new System.Drawing.Size(300, 33);
            this.tss_Counter.Text = "计数器";
            this.tss_Counter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frm_MainWin
            // 
            this.AcceptButton = this.btn_Send;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1219, 860);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Name = "frm_MainWin";
            this.Text = "微信收发平台";
            this.Load += new System.EventHandler(this.frm_MainWin_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage_msg.ResumeLayout(false);
            this.tabPage_contact.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tss_Status;
        private System.Windows.Forms.ToolStripStatusLabel tss_Msg;
        private System.Windows.Forms.ToolStripStatusLabel tss_Counter;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_msg;
        private System.Windows.Forms.TabPage tabPage_contact;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.RichTextBox txt_MsgList;
        private System.Windows.Forms.TextBox txt_msg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ddl_ToUser;
    }
}
namespace ExchangeTermial
{
    partial class MainWindow
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
            System.Windows.Forms.Application.ExitThread();
        }

        

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmi_System = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_View = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Operate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Setting = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_AddHedge = new System.Windows.Forms.Button();
            this.btn_SelfAddCombo = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_NewInsts = new System.Windows.Forms.TextBox();
            this.btn_Send = new System.Windows.Forms.Button();
            this.txt_OpenTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_OpenCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Insts = new System.Windows.Forms.TextBox();
            this.txt_ExpectNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.timer_RequestInst = new System.Timers.Timer();// new System.Windows.Forms.Timer(this.components);
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_System,
            this.tsmi_View,
            this.tsmi_Operate,
            this.tsmi_Setting});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(855, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmi_System
            // 
            this.tsmi_System.Name = "tsmi_System";
            this.tsmi_System.Size = new System.Drawing.Size(44, 21);
            this.tsmi_System.Text = "系统";
            // 
            // tsmi_View
            // 
            this.tsmi_View.Name = "tsmi_View";
            this.tsmi_View.Size = new System.Drawing.Size(44, 21);
            this.tsmi_View.Text = "查看";
            // 
            // tsmi_Operate
            // 
            this.tsmi_Operate.Name = "tsmi_Operate";
            this.tsmi_Operate.Size = new System.Drawing.Size(44, 21);
            this.tsmi_Operate.Text = "操作";
            // 
            // tsmi_Setting
            // 
            this.tsmi_Setting.Name = "tsmi_Setting";
            this.tsmi_Setting.Size = new System.Drawing.Size(44, 21);
            this.tsmi_Setting.Text = "设置";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(855, 580);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.webBrowser1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(847, 554);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "概要";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_AddHedge);
            this.groupBox1.Controls.Add(this.btn_SelfAddCombo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txt_NewInsts);
            this.groupBox1.Controls.Add(this.btn_Send);
            this.groupBox1.Controls.Add(this.txt_OpenTime);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txt_OpenCode);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_Insts);
            this.groupBox1.Controls.Add(this.txt_ExpectNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(841, 120);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "指令信息";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btn_AddHedge
            // 
            this.btn_AddHedge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_AddHedge.Location = new System.Drawing.Point(773, 91);
            this.btn_AddHedge.Name = "btn_AddHedge";
            this.btn_AddHedge.Size = new System.Drawing.Size(62, 21);
            this.btn_AddHedge.TabIndex = 11;
            this.btn_AddHedge.Text = "加对冲";
            this.btn_AddHedge.UseVisualStyleBackColor = true;
            // 
            // btn_SelfAddCombo
            // 
            this.btn_SelfAddCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_SelfAddCombo.Location = new System.Drawing.Point(696, 91);
            this.btn_SelfAddCombo.Name = "btn_SelfAddCombo";
            this.btn_SelfAddCombo.Size = new System.Drawing.Size(71, 21);
            this.btn_SelfAddCombo.TabIndex = 10;
            this.btn_SelfAddCombo.Text = "加组合";
            this.btn_SelfAddCombo.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(694, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "自定义组合";
            // 
            // txt_NewInsts
            // 
            this.txt_NewInsts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_NewInsts.Location = new System.Drawing.Point(696, 38);
            this.txt_NewInsts.Multiline = true;
            this.txt_NewInsts.Name = "txt_NewInsts";
            this.txt_NewInsts.Size = new System.Drawing.Size(139, 47);
            this.txt_NewInsts.TabIndex = 8;
            // 
            // btn_Send
            // 
            this.btn_Send.Location = new System.Drawing.Point(648, 19);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(40, 94);
            this.btn_Send.TabIndex = 7;
            this.btn_Send.Text = "发送";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // txt_OpenTime
            // 
            this.txt_OpenTime.Location = new System.Drawing.Point(505, 20);
            this.txt_OpenTime.Name = "txt_OpenTime";
            this.txt_OpenTime.Size = new System.Drawing.Size(139, 21);
            this.txt_OpenTime.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(448, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "上期时间";
            // 
            // txt_OpenCode
            // 
            this.txt_OpenCode.Location = new System.Drawing.Point(240, 20);
            this.txt_OpenCode.Name = "txt_OpenCode";
            this.txt_OpenCode.Size = new System.Drawing.Size(202, 21);
            this.txt_OpenCode.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(181, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "上期结果";
            // 
            // txt_Insts
            // 
            this.txt_Insts.Location = new System.Drawing.Point(16, 46);
            this.txt_Insts.Multiline = true;
            this.txt_Insts.Name = "txt_Insts";
            this.txt_Insts.Size = new System.Drawing.Size(628, 68);
            this.txt_Insts.TabIndex = 2;
            // 
            // txt_ExpectNo
            // 
            this.txt_ExpectNo.Location = new System.Drawing.Point(71, 19);
            this.txt_ExpectNo.Name = "txt_ExpectNo";
            this.txt_ExpectNo.Size = new System.Drawing.Size(95, 21);
            this.txt_ExpectNo.TabIndex = 1;
            this.txt_ExpectNo.TextChanged += new System.EventHandler(this.txt_ExpectNo_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "下注期号";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(3, 129);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(836, 404);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(847, 554);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "数据";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 583);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(855, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // timer_RequestInst
            // 
            this.timer_RequestInst.Enabled = true;
            this.timer_RequestInst.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_RequestInst_Tick);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 605);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmi_System;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Operate;
        private System.Windows.Forms.ToolStripMenuItem tsmi_View;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Setting;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_ExpectNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_OpenCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Insts;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.TextBox txt_OpenTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_AddHedge;
        private System.Windows.Forms.Button btn_SelfAddCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_NewInsts;
        private System.Timers.Timer timer_RequestInst;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}
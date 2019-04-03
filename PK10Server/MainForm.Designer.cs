namespace PK10Server
{
    partial class MainForm<T>
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm<T>));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_Timer = new System.Windows.Forms.Label();
            this.btn_Subtract = new System.Windows.Forms.Button();
            this.txt_NewestOpenTime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_NewestOpenCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_AddExpectNo = new System.Windows.Forms.Button();
            this.txt_NextExpectNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_NewestExpect = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listView_forSerial = new System.Windows.Forms.ListView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.listView_ForCar = new System.Windows.Forms.ListView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.listView_PK10Data = new System.Windows.Forms.ListView();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.listView_TXFFCData = new System.Windows.Forms.ListView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.listView_SIStrags = new System.Windows.Forms.ListView();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.listView_CIStrags = new System.Windows.Forms.ListView();
            this.timer_For_NewestData = new System.Windows.Forms.Timer(this.components);
            this.timer_For_CurrTime = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_StartSvr = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_StopSvr = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_StartRefreshWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_StopRefreshWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_StartCalc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStopCalcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_RunMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tMI_HistoryBackTest = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_getTXFFCHistoryTxtData = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_getTXFFCHistoryFromWeb = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_CommSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_StragManager = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_StragRunPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_AssetUnitMgr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.timer_For_getHtmlData = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1034, 470);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(1026, 444);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "概况";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(2, 2);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(1022, 440);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Location = new System.Drawing.Point(602, 68);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(408, 74);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "附加操作";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Location = new System.Drawing.Point(2, 68);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(594, 74);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "指令信息";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(517, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 25);
            this.button1.TabIndex = 0;
            this.button1.Text = "发送";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 22);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(486, 42);
            this.textBox1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lbl_Timer);
            this.groupBox1.Controls.Add(this.btn_Subtract);
            this.groupBox1.Controls.Add(this.txt_NewestOpenTime);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txt_NewestOpenCode);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btn_AddExpectNo);
            this.groupBox1.Controls.Add(this.txt_NextExpectNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_NewestExpect);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(2, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(1008, 55);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "最新信息";
            // 
            // lbl_Timer
            // 
            this.lbl_Timer.AutoSize = true;
            this.lbl_Timer.Location = new System.Drawing.Point(697, 22);
            this.lbl_Timer.Name = "lbl_Timer";
            this.lbl_Timer.Size = new System.Drawing.Size(41, 12);
            this.lbl_Timer.TabIndex = 10;
            this.lbl_Timer.Text = "xxxxxx";
            // 
            // btn_Subtract
            // 
            this.btn_Subtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Subtract.Location = new System.Drawing.Point(954, 16);
            this.btn_Subtract.Name = "btn_Subtract";
            this.btn_Subtract.Size = new System.Drawing.Size(49, 21);
            this.btn_Subtract.TabIndex = 9;
            this.btn_Subtract.Text = "Sub";
            this.btn_Subtract.UseVisualStyleBackColor = true;
            this.btn_Subtract.Click += new System.EventHandler(this.btn_Subtract_Click);
            // 
            // txt_NewestOpenTime
            // 
            this.txt_NewestOpenTime.Location = new System.Drawing.Point(563, 20);
            this.txt_NewestOpenTime.Name = "txt_NewestOpenTime";
            this.txt_NewestOpenTime.Size = new System.Drawing.Size(119, 21);
            this.txt_NewestOpenTime.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(528, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "时间";
            // 
            // txt_NewestOpenCode
            // 
            this.txt_NewestOpenCode.Location = new System.Drawing.Point(314, 21);
            this.txt_NewestOpenCode.Name = "txt_NewestOpenCode";
            this.txt_NewestOpenCode.Size = new System.Drawing.Size(208, 21);
            this.txt_NewestOpenCode.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(281, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "号码";
            // 
            // btn_AddExpectNo
            // 
            this.btn_AddExpectNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_AddExpectNo.Location = new System.Drawing.Point(899, 17);
            this.btn_AddExpectNo.Name = "btn_AddExpectNo";
            this.btn_AddExpectNo.Size = new System.Drawing.Size(49, 21);
            this.btn_AddExpectNo.TabIndex = 4;
            this.btn_AddExpectNo.Text = "Add";
            this.btn_AddExpectNo.UseVisualStyleBackColor = true;
            this.btn_AddExpectNo.Click += new System.EventHandler(this.btn_AddExpectNo_Click);
            // 
            // txt_NextExpectNo
            // 
            this.txt_NextExpectNo.Location = new System.Drawing.Point(68, 20);
            this.txt_NextExpectNo.Name = "txt_NextExpectNo";
            this.txt_NextExpectNo.Size = new System.Drawing.Size(72, 21);
            this.txt_NextExpectNo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "目标期号";
            // 
            // txt_NewestExpect
            // 
            this.txt_NewestExpect.Location = new System.Drawing.Point(202, 21);
            this.txt_NewestExpect.Name = "txt_NewestExpect";
            this.txt_NewestExpect.Size = new System.Drawing.Size(72, 21);
            this.txt_NewestExpect.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(148, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "最新期号";
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1018, 262);
            this.tabControl2.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listView_forSerial);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage3.Size = new System.Drawing.Size(1010, 236);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "排名信息";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listView_forSerial
            // 
            this.listView_forSerial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_forSerial.GridLines = true;
            this.listView_forSerial.Location = new System.Drawing.Point(2, 2);
            this.listView_forSerial.Margin = new System.Windows.Forms.Padding(2);
            this.listView_forSerial.Name = "listView_forSerial";
            this.listView_forSerial.Size = new System.Drawing.Size(1006, 232);
            this.listView_forSerial.TabIndex = 0;
            this.listView_forSerial.UseCompatibleStateImageBehavior = false;
            this.listView_forSerial.View = System.Windows.Forms.View.Details;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.listView_ForCar);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage4.Size = new System.Drawing.Size(1010, 235);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "对象信息";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // listView_ForCar
            // 
            this.listView_ForCar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_ForCar.GridLines = true;
            this.listView_ForCar.Location = new System.Drawing.Point(2, 2);
            this.listView_ForCar.Margin = new System.Windows.Forms.Padding(2);
            this.listView_ForCar.Name = "listView_ForCar";
            this.listView_ForCar.Size = new System.Drawing.Size(1006, 231);
            this.listView_ForCar.TabIndex = 0;
            this.listView_ForCar.UseCompatibleStateImageBehavior = false;
            this.listView_ForCar.View = System.Windows.Forms.View.Details;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.listView_PK10Data);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(1010, 235);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "PK10数据";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // listView_PK10Data
            // 
            this.listView_PK10Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_PK10Data.GridLines = true;
            this.listView_PK10Data.Location = new System.Drawing.Point(0, 0);
            this.listView_PK10Data.Margin = new System.Windows.Forms.Padding(2);
            this.listView_PK10Data.Name = "listView_PK10Data";
            this.listView_PK10Data.Size = new System.Drawing.Size(1010, 235);
            this.listView_PK10Data.TabIndex = 2;
            this.listView_PK10Data.UseCompatibleStateImageBehavior = false;
            this.listView_PK10Data.View = System.Windows.Forms.View.Details;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.listView_TXFFCData);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(1010, 235);
            this.tabPage6.TabIndex = 3;
            this.tabPage6.Text = "TXFFC数据";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // listView_TXFFCData
            // 
            this.listView_TXFFCData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_TXFFCData.GridLines = true;
            this.listView_TXFFCData.Location = new System.Drawing.Point(0, 0);
            this.listView_TXFFCData.Margin = new System.Windows.Forms.Padding(2);
            this.listView_TXFFCData.Name = "listView_TXFFCData";
            this.listView_TXFFCData.Size = new System.Drawing.Size(1010, 235);
            this.listView_TXFFCData.TabIndex = 1;
            this.listView_TXFFCData.UseCompatibleStateImageBehavior = false;
            this.listView_TXFFCData.View = System.Windows.Forms.View.Details;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 262);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 7, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1018, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(1026, 443);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "设置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(2, 2);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl3);
            this.splitContainer2.Size = new System.Drawing.Size(1022, 439);
            this.splitContainer2.SplitterDistance = 144;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage7);
            this.tabControl3.Controls.Add(this.tabPage8);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(0, 0);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(1022, 291);
            this.tabControl3.TabIndex = 0;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.listView_SIStrags);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1014, 265);
            this.tabPage7.TabIndex = 0;
            this.tabPage7.Text = "单利策略";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // listView_SIStrags
            // 
            this.listView_SIStrags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_SIStrags.Location = new System.Drawing.Point(3, 3);
            this.listView_SIStrags.Name = "listView_SIStrags";
            this.listView_SIStrags.Size = new System.Drawing.Size(1008, 259);
            this.listView_SIStrags.TabIndex = 0;
            this.listView_SIStrags.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.listView_CIStrags);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(1014, 265);
            this.tabPage8.TabIndex = 1;
            this.tabPage8.Text = "复利策略";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // listView_CIStrags
            // 
            this.listView_CIStrags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_CIStrags.Location = new System.Drawing.Point(3, 3);
            this.listView_CIStrags.Name = "listView_CIStrags";
            this.listView_CIStrags.Size = new System.Drawing.Size(1008, 259);
            this.listView_CIStrags.TabIndex = 0;
            this.listView_CIStrags.UseCompatibleStateImageBehavior = false;
            // 
            // timer_For_NewestData
            // 
            this.timer_For_NewestData.Tick += new System.EventHandler(this.timer_For_NewestData_Tick);
            // 
            // timer_For_CurrTime
            // 
            this.timer_For_CurrTime.Tick += new System.EventHandler(this.timer_For_CurrTime_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem6,
            this.toolStripMenuItem4});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1034, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_StartSvr,
            this.TSMI_StopSvr,
            this.tsmi_StartRefreshWindowsToolStripMenuItem,
            this.tsmi_StopRefreshWindows,
            this.tsmi_StartCalc,
            this.tsmiStopCalcToolStripMenuItem,
            this.tsmi_RunMonitor});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(41, 20);
            this.toolStripMenuItem1.Text = "系统";
            // 
            // TSMI_StartSvr
            // 
            this.TSMI_StartSvr.Name = "TSMI_StartSvr";
            this.TSMI_StartSvr.Size = new System.Drawing.Size(180, 22);
            this.TSMI_StartSvr.Text = "开始接收数据";
            this.TSMI_StartSvr.Click += new System.EventHandler(this.TSMI_StartSvr_Click);
            // 
            // TSMI_StopSvr
            // 
            this.TSMI_StopSvr.Name = "TSMI_StopSvr";
            this.TSMI_StopSvr.Size = new System.Drawing.Size(180, 22);
            this.TSMI_StopSvr.Text = "停止接收数据";
            this.TSMI_StopSvr.Click += new System.EventHandler(this.TSMI_StopSvr_Click);
            // 
            // tsmi_StartRefreshWindowsToolStripMenuItem
            // 
            this.tsmi_StartRefreshWindowsToolStripMenuItem.Name = "tsmi_StartRefreshWindowsToolStripMenuItem";
            this.tsmi_StartRefreshWindowsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tsmi_StartRefreshWindowsToolStripMenuItem.Text = "开始刷新数据";
            this.tsmi_StartRefreshWindowsToolStripMenuItem.Click += new System.EventHandler(this.tsmi_StartRefreshWindowsToolStripMenuItem_Click);
            // 
            // tsmi_StopRefreshWindows
            // 
            this.tsmi_StopRefreshWindows.Name = "tsmi_StopRefreshWindows";
            this.tsmi_StopRefreshWindows.Size = new System.Drawing.Size(180, 22);
            this.tsmi_StopRefreshWindows.Text = "停止刷新数据";
            this.tsmi_StopRefreshWindows.Click += new System.EventHandler(this.tsmi_StopRefreshWindows_Click);
            // 
            // tsmi_StartCalc
            // 
            this.tsmi_StartCalc.Name = "tsmi_StartCalc";
            this.tsmi_StartCalc.Size = new System.Drawing.Size(180, 22);
            this.tsmi_StartCalc.Text = "开始计算结果";
            this.tsmi_StartCalc.Click += new System.EventHandler(this.tsmiStartCalcToolStripMenuItem_Click);
            // 
            // tsmiStopCalcToolStripMenuItem
            // 
            this.tsmiStopCalcToolStripMenuItem.Name = "tsmiStopCalcToolStripMenuItem";
            this.tsmiStopCalcToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tsmiStopCalcToolStripMenuItem.Text = "停止计算结果";
            // 
            // tsmi_RunMonitor
            // 
            this.tsmi_RunMonitor.Name = "tsmi_RunMonitor";
            this.tsmi_RunMonitor.Size = new System.Drawing.Size(180, 22);
            this.tsmi_RunMonitor.Text = "运行监控";
            this.tsmi_RunMonitor.Click += new System.EventHandler(this.tsmi_RunMonitor_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(41, 20);
            this.toolStripMenuItem2.Text = "刷新";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.tMI_HistoryBackTest,
            this.tsmi_getTXFFCHistoryTxtData,
            this.tsmi_getTXFFCHistoryFromWeb});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(41, 20);
            this.toolStripMenuItem3.Text = "操作";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(238, 22);
            this.toolStripMenuItem5.Text = "筛选机会";
            // 
            // tMI_HistoryBackTest
            // 
            this.tMI_HistoryBackTest.Name = "tMI_HistoryBackTest";
            this.tMI_HistoryBackTest.Size = new System.Drawing.Size(238, 22);
            this.tMI_HistoryBackTest.Text = "历史回测";
            this.tMI_HistoryBackTest.Click += new System.EventHandler(this.tMI_HistoryBackTest_Click);
            // 
            // tsmi_getTXFFCHistoryTxtData
            // 
            this.tsmi_getTXFFCHistoryTxtData.Name = "tsmi_getTXFFCHistoryTxtData";
            this.tsmi_getTXFFCHistoryTxtData.Size = new System.Drawing.Size(238, 22);
            this.tsmi_getTXFFCHistoryTxtData.Text = "读取腾讯分分彩历史数据(本地)";
            this.tsmi_getTXFFCHistoryTxtData.Click += new System.EventHandler(this.tsmi_getTXFFCHistoryTxtData_Click);
            // 
            // tsmi_getTXFFCHistoryFromWeb
            // 
            this.tsmi_getTXFFCHistoryFromWeb.Name = "tsmi_getTXFFCHistoryFromWeb";
            this.tsmi_getTXFFCHistoryFromWeb.Size = new System.Drawing.Size(238, 22);
            this.tsmi_getTXFFCHistoryFromWeb.Text = "读取腾讯分分彩历史数据(web)";
            this.tsmi_getTXFFCHistoryFromWeb.Click += new System.EventHandler(this.tsmi_getTXFFCHistoryFromWeb_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_CommSetting,
            this.toolStripMenuItem_StragManager,
            this.ToolStripMenuItem_StragRunPlan,
            this.tsmi_AssetUnitMgr});
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(41, 20);
            this.toolStripMenuItem6.Text = "设置";
            // 
            // toolStripMenuItem_CommSetting
            // 
            this.toolStripMenuItem_CommSetting.Name = "toolStripMenuItem_CommSetting";
            this.toolStripMenuItem_CommSetting.Size = new System.Drawing.Size(142, 22);
            this.toolStripMenuItem_CommSetting.Text = "环境设置";
            // 
            // toolStripMenuItem_StragManager
            // 
            this.toolStripMenuItem_StragManager.Name = "toolStripMenuItem_StragManager";
            this.toolStripMenuItem_StragManager.Size = new System.Drawing.Size(142, 22);
            this.toolStripMenuItem_StragManager.Text = "策略管理";
            this.toolStripMenuItem_StragManager.Click += new System.EventHandler(this.toolStripMenuItem_StragManager_Click);
            // 
            // ToolStripMenuItem_StragRunPlan
            // 
            this.ToolStripMenuItem_StragRunPlan.Name = "ToolStripMenuItem_StragRunPlan";
            this.ToolStripMenuItem_StragRunPlan.Size = new System.Drawing.Size(142, 22);
            this.ToolStripMenuItem_StragRunPlan.Text = "策略计划";
            this.ToolStripMenuItem_StragRunPlan.Click += new System.EventHandler(this.ToolStripMenuItem_StragRunPlan_Click);
            // 
            // tsmi_AssetUnitMgr
            // 
            this.tsmi_AssetUnitMgr.Name = "tsmi_AssetUnitMgr";
            this.tsmi_AssetUnitMgr.Size = new System.Drawing.Size(142, 22);
            this.tsmi_AssetUnitMgr.Text = "资产单元设置";
            this.tsmi_AssetUnitMgr.Click += new System.EventHandler(this.tsmi_AssetUnitMgr_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(41, 20);
            this.toolStripMenuItem4.Text = "帮助";
            // 
            // timer_For_getHtmlData
            // 
            this.timer_For_getHtmlData.Interval = 60000;
            this.timer_For_getHtmlData.Tick += new System.EventHandler(this.timer_For_getHtmlData_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 494);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "快乐猎车服务端";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListView listView_forSerial;
        private System.Windows.Forms.ListView listView_ForCar;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btn_AddExpectNo;
        private System.Windows.Forms.TextBox txt_NextExpectNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_NewestExpect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_NewestOpenTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_NewestOpenCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_Subtract;
        private System.Windows.Forms.Timer timer_For_NewestData;
        private System.Windows.Forms.Label lbl_Timer;
        private System.Windows.Forms.Timer timer_For_CurrTime;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem tMI_HistoryBackTest;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem TSMI_StartSvr;
        private System.Windows.Forms.ToolStripMenuItem TSMI_StopSvr;
        private System.Windows.Forms.Timer timer_For_getHtmlData;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.ListView listView_TXFFCData;
        private System.Windows.Forms.ListView listView_PK10Data;
        private System.Windows.Forms.ToolStripMenuItem tsmi_getTXFFCHistoryTxtData;
        private System.Windows.Forms.ToolStripMenuItem tsmi_getTXFFCHistoryFromWeb;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.ListView listView_SIStrags;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.ListView listView_CIStrags;
        private System.Windows.Forms.ToolStripMenuItem tsmi_StartRefreshWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmi_StopRefreshWindows;
        private System.Windows.Forms.ToolStripMenuItem tsmi_StartCalc;
        private System.Windows.Forms.ToolStripMenuItem tsmiStopCalcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommSetting;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_StragManager;
        private System.Windows.Forms.ToolStripMenuItem tsmi_RunMonitor;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_StragRunPlan;
        private System.Windows.Forms.ToolStripMenuItem tsmi_AssetUnitMgr;
    }
}
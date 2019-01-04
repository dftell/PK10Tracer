namespace BackTestSys
{
    partial class BackTestFrm
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
            try
            {
                this.th.Abort();
            }
            catch
            {
            }
            th = null;
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackTestFrm));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_DistrCheck = new System.Windows.Forms.Button();
            this.btn_CalcEr = new System.Windows.Forms.Button();
            this.btn_VirExchange = new System.Windows.Forms.Button();
            this.btn_CheckData = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txt_RoundStepLong = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txt_RoundCycLong = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.btn_roundTest = new System.Windows.Forms.Button();
            this.btn_export = new System.Windows.Forms.Button();
            this.btn_startTest = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txt_LearnCnt = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.runPlanPicker1 = new ExchangeLib.RunPlanPicker();
            this.txt_AllowMaxHoldTimeCnt = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.txt_minRate = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.txt_StdvCnt = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.chkb_getRev = new System.Windows.Forms.CheckBox();
            this.chkb_onlyBS = new System.Windows.Forms.CheckBox();
            this.chkb_onlySD = new System.Windows.Forms.CheckBox();
            this.chkb_exclueBS = new System.Windows.Forms.CheckBox();
            this.chkb_exclueSD = new System.Windows.Forms.CheckBox();
            this.txt_maxInputTimes = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkb_bySer = new System.Windows.Forms.CheckBox();
            this.txt_minInputTimes = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txt_ChipCnt = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_FixChipCnt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_reviewExpCnt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_Timer_Interval = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.checkBox_UseBuffRsult = new System.Windows.Forms.CheckBox();
            this.checkBox_MixAll = new System.Windows.Forms.CheckBox();
            this.checkBox_CreamModel = new System.Windows.Forms.CheckBox();
            this.txt_MaxHoldChanceCnt = new System.Windows.Forms.TextBox();
            this.txt_InitCash = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.txt_minColTimes10 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txt_minColTimes9 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txt_minColTimes8 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txt_minColTimes7 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txt_minColTimes6 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txt_minColTimes5 = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txt_minColTimes4 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txt_minColTimes3 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txt_minColTimes2 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txt_minColTimes1 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txt_GrownMaxVal = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_GrownMinVal = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txt_MinCols = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_Odds = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_LoopCnt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_begExpNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.contextMenuStrip_ForListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmi_ExportExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView2 = new System.Windows.Forms.ListView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listView3 = new System.Windows.Forms.ListView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.dataGridView_ExchangeDetail = new System.Windows.Forms.DataGridView();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.chart_ForProb = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.dataGridView_ProbData = new System.Windows.Forms.DataGridView();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.chart_ForSystemStdDev = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.dg_forStdDev = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.timer_Tip = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.contextMenuStrip_ForListView.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ExchangeDetail)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_ForProb)).BeginInit();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ProbData)).BeginInit();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_ForSystemStdDev)).BeginInit();
            this.tabPage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_forStdDev)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Margin = new System.Windows.Forms.Padding(2);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(2, 550);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(2, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btn_DistrCheck);
            this.splitContainer1.Panel1.Controls.Add(this.btn_CalcEr);
            this.splitContainer1.Panel1.Controls.Add(this.btn_VirExchange);
            this.splitContainer1.Panel1.Controls.Add(this.btn_CheckData);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.btn_roundTest);
            this.splitContainer1.Panel1.Controls.Add(this.btn_export);
            this.splitContainer1.Panel1.Controls.Add(this.btn_startTest);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(942, 550);
            this.splitContainer1.SplitterDistance = 211;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 1;
            // 
            // btn_DistrCheck
            // 
            this.btn_DistrCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_DistrCheck.Location = new System.Drawing.Point(862, 97);
            this.btn_DistrCheck.Margin = new System.Windows.Forms.Padding(2);
            this.btn_DistrCheck.Name = "btn_DistrCheck";
            this.btn_DistrCheck.Size = new System.Drawing.Size(61, 22);
            this.btn_DistrCheck.TabIndex = 10;
            this.btn_DistrCheck.Text = "分布检测";
            this.btn_DistrCheck.UseVisualStyleBackColor = true;
            this.btn_DistrCheck.Click += new System.EventHandler(this.btn_DistrCheck_Click);
            // 
            // btn_CalcEr
            // 
            this.btn_CalcEr.Location = new System.Drawing.Point(864, 177);
            this.btn_CalcEr.Name = "btn_CalcEr";
            this.btn_CalcEr.Size = new System.Drawing.Size(60, 21);
            this.btn_CalcEr.TabIndex = 9;
            this.btn_CalcEr.Text = "计算器";
            this.btn_CalcEr.UseVisualStyleBackColor = true;
            this.btn_CalcEr.Click += new System.EventHandler(this.btn_CalcEr_Click);
            // 
            // btn_VirExchange
            // 
            this.btn_VirExchange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_VirExchange.Location = new System.Drawing.Point(862, 46);
            this.btn_VirExchange.Margin = new System.Windows.Forms.Padding(2);
            this.btn_VirExchange.Name = "btn_VirExchange";
            this.btn_VirExchange.Size = new System.Drawing.Size(61, 22);
            this.btn_VirExchange.TabIndex = 8;
            this.btn_VirExchange.Text = "模拟交易";
            this.btn_VirExchange.UseVisualStyleBackColor = true;
            this.btn_VirExchange.Click += new System.EventHandler(this.btn_VirExchange_Click);
            // 
            // btn_CheckData
            // 
            this.btn_CheckData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_CheckData.Location = new System.Drawing.Point(863, 150);
            this.btn_CheckData.Name = "btn_CheckData";
            this.btn_CheckData.Size = new System.Drawing.Size(61, 22);
            this.btn_CheckData.TabIndex = 7;
            this.btn_CheckData.Text = "验证数据";
            this.btn_CheckData.UseVisualStyleBackColor = true;
            this.btn_CheckData.Click += new System.EventHandler(this.btn_CheckData_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Location = new System.Drawing.Point(649, 92);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(166, 115);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "敏感性分析设置";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txt_RoundStepLong);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.txt_RoundCycLong);
            this.groupBox3.Controls.Add(this.label24);
            this.groupBox3.Location = new System.Drawing.Point(649, 12);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(166, 76);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "滚动回测设置";
            // 
            // txt_RoundStepLong
            // 
            this.txt_RoundStepLong.Location = new System.Drawing.Point(78, 41);
            this.txt_RoundStepLong.Margin = new System.Windows.Forms.Padding(2);
            this.txt_RoundStepLong.Name = "txt_RoundStepLong";
            this.txt_RoundStepLong.Size = new System.Drawing.Size(70, 21);
            this.txt_RoundStepLong.TabIndex = 11;
            this.txt_RoundStepLong.Text = "1000";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(11, 42);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(53, 12);
            this.label23.TabIndex = 10;
            this.label23.Text = "滚动步长";
            // 
            // txt_RoundCycLong
            // 
            this.txt_RoundCycLong.Location = new System.Drawing.Point(78, 21);
            this.txt_RoundCycLong.Margin = new System.Windows.Forms.Padding(2);
            this.txt_RoundCycLong.Name = "txt_RoundCycLong";
            this.txt_RoundCycLong.Size = new System.Drawing.Size(70, 21);
            this.txt_RoundCycLong.TabIndex = 9;
            this.txt_RoundCycLong.Text = "5000";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(11, 22);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(53, 12);
            this.label24.TabIndex = 8;
            this.label24.Text = "滚动周期";
            // 
            // btn_roundTest
            // 
            this.btn_roundTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_roundTest.Location = new System.Drawing.Point(862, 71);
            this.btn_roundTest.Margin = new System.Windows.Forms.Padding(2);
            this.btn_roundTest.Name = "btn_roundTest";
            this.btn_roundTest.Size = new System.Drawing.Size(61, 22);
            this.btn_roundTest.TabIndex = 4;
            this.btn_roundTest.Text = "滚动回测";
            this.btn_roundTest.UseVisualStyleBackColor = true;
            this.btn_roundTest.Click += new System.EventHandler(this.btn_roundTest_Click);
            // 
            // btn_export
            // 
            this.btn_export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_export.Location = new System.Drawing.Point(862, 123);
            this.btn_export.Margin = new System.Windows.Forms.Padding(2);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(61, 22);
            this.btn_export.TabIndex = 3;
            this.btn_export.Text = "导出";
            this.btn_export.UseVisualStyleBackColor = true;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // btn_startTest
            // 
            this.btn_startTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_startTest.Location = new System.Drawing.Point(862, 22);
            this.btn_startTest.Margin = new System.Windows.Forms.Padding(2);
            this.btn_startTest.Name = "btn_startTest";
            this.btn_startTest.Size = new System.Drawing.Size(61, 22);
            this.btn_startTest.TabIndex = 2;
            this.btn_startTest.Text = "整体回测";
            this.btn_startTest.UseVisualStyleBackColor = true;
            this.btn_startTest.Click += new System.EventHandler(this.btn_startTest_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.txt_LearnCnt);
            this.groupBox2.Controls.Add(this.label31);
            this.groupBox2.Controls.Add(this.runPlanPicker1);
            this.groupBox2.Controls.Add(this.txt_AllowMaxHoldTimeCnt);
            this.groupBox2.Controls.Add(this.label29);
            this.groupBox2.Controls.Add(this.txt_minRate);
            this.groupBox2.Controls.Add(this.label26);
            this.groupBox2.Controls.Add(this.txt_StdvCnt);
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.chkb_getRev);
            this.groupBox2.Controls.Add(this.chkb_onlyBS);
            this.groupBox2.Controls.Add(this.chkb_onlySD);
            this.groupBox2.Controls.Add(this.chkb_exclueBS);
            this.groupBox2.Controls.Add(this.chkb_exclueSD);
            this.groupBox2.Controls.Add(this.txt_maxInputTimes);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.chkb_bySer);
            this.groupBox2.Controls.Add(this.txt_minInputTimes);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.txt_ChipCnt);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txt_FixChipCnt);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txt_reviewExpCnt);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(347, 12);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(298, 195);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "回测策略设置";
            // 
            // txt_LearnCnt
            // 
            this.txt_LearnCnt.Location = new System.Drawing.Point(78, 36);
            this.txt_LearnCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_LearnCnt.Name = "txt_LearnCnt";
            this.txt_LearnCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_LearnCnt.TabIndex = 30;
            this.txt_LearnCnt.Text = "100";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(12, 40);
            this.label31.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(53, 12);
            this.label31.TabIndex = 29;
            this.label31.Text = "学习期数";
            // 
            // runPlanPicker1
            // 
            this.runPlanPicker1.Location = new System.Drawing.Point(95, 7);
            this.runPlanPicker1.Name = "runPlanPicker1";
            this.runPlanPicker1.Plans = null;
            this.runPlanPicker1.Size = new System.Drawing.Size(198, 27);
            this.runPlanPicker1.TabIndex = 28;
            // 
            // txt_AllowMaxHoldTimeCnt
            // 
            this.txt_AllowMaxHoldTimeCnt.Location = new System.Drawing.Point(183, 147);
            this.txt_AllowMaxHoldTimeCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_AllowMaxHoldTimeCnt.Name = "txt_AllowMaxHoldTimeCnt";
            this.txt_AllowMaxHoldTimeCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_AllowMaxHoldTimeCnt.TabIndex = 27;
            this.txt_AllowMaxHoldTimeCnt.Text = "9";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(127, 152);
            this.label29.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(53, 12);
            this.label29.TabIndex = 26;
            this.label29.Text = "止损次数";
            // 
            // txt_minRate
            // 
            this.txt_minRate.Location = new System.Drawing.Point(183, 169);
            this.txt_minRate.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minRate.Name = "txt_minRate";
            this.txt_minRate.Size = new System.Drawing.Size(35, 21);
            this.txt_minRate.TabIndex = 25;
            this.txt_minRate.Text = "1.001";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(126, 172);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(53, 12);
            this.label26.TabIndex = 24;
            this.label26.Text = "最小赔率";
            // 
            // txt_StdvCnt
            // 
            this.txt_StdvCnt.Location = new System.Drawing.Point(78, 169);
            this.txt_StdvCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_StdvCnt.Name = "txt_StdvCnt";
            this.txt_StdvCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_StdvCnt.TabIndex = 23;
            this.txt_StdvCnt.Text = "1.5";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(13, 172);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(53, 12);
            this.label25.TabIndex = 22;
            this.label25.Text = "标准差数";
            // 
            // chkb_getRev
            // 
            this.chkb_getRev.AutoSize = true;
            this.chkb_getRev.Location = new System.Drawing.Point(132, 136);
            this.chkb_getRev.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_getRev.Name = "chkb_getRev";
            this.chkb_getRev.Size = new System.Drawing.Size(48, 16);
            this.chkb_getRev.TabIndex = 21;
            this.chkb_getRev.Text = "求反";
            this.chkb_getRev.UseVisualStyleBackColor = true;
            // 
            // chkb_onlyBS
            // 
            this.chkb_onlyBS.AutoSize = true;
            this.chkb_onlyBS.Location = new System.Drawing.Point(132, 118);
            this.chkb_onlyBS.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_onlyBS.Name = "chkb_onlyBS";
            this.chkb_onlyBS.Size = new System.Drawing.Size(84, 16);
            this.chkb_onlyBS.TabIndex = 20;
            this.chkb_onlyBS.Text = "只考虑大小";
            this.chkb_onlyBS.UseVisualStyleBackColor = true;
            // 
            // chkb_onlySD
            // 
            this.chkb_onlySD.AutoSize = true;
            this.chkb_onlySD.Location = new System.Drawing.Point(132, 98);
            this.chkb_onlySD.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_onlySD.Name = "chkb_onlySD";
            this.chkb_onlySD.Size = new System.Drawing.Size(84, 16);
            this.chkb_onlySD.TabIndex = 19;
            this.chkb_onlySD.Text = "只考虑单双";
            this.chkb_onlySD.UseVisualStyleBackColor = true;
            // 
            // chkb_exclueBS
            // 
            this.chkb_exclueBS.AutoSize = true;
            this.chkb_exclueBS.Location = new System.Drawing.Point(132, 78);
            this.chkb_exclueBS.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_exclueBS.Name = "chkb_exclueBS";
            this.chkb_exclueBS.Size = new System.Drawing.Size(72, 16);
            this.chkb_exclueBS.TabIndex = 18;
            this.chkb_exclueBS.Text = "排除大小";
            this.chkb_exclueBS.UseVisualStyleBackColor = true;
            // 
            // chkb_exclueSD
            // 
            this.chkb_exclueSD.AutoSize = true;
            this.chkb_exclueSD.Location = new System.Drawing.Point(132, 57);
            this.chkb_exclueSD.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_exclueSD.Name = "chkb_exclueSD";
            this.chkb_exclueSD.Size = new System.Drawing.Size(72, 16);
            this.chkb_exclueSD.TabIndex = 17;
            this.chkb_exclueSD.Text = "排除单双";
            this.chkb_exclueSD.UseVisualStyleBackColor = true;
            // 
            // txt_maxInputTimes
            // 
            this.txt_maxInputTimes.Location = new System.Drawing.Point(78, 146);
            this.txt_maxInputTimes.Margin = new System.Windows.Forms.Padding(2);
            this.txt_maxInputTimes.Name = "txt_maxInputTimes";
            this.txt_maxInputTimes.Size = new System.Drawing.Size(35, 21);
            this.txt_maxInputTimes.TabIndex = 16;
            this.txt_maxInputTimes.Text = "8";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 152);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 15;
            this.label5.Text = "最大入期";
            // 
            // chkb_bySer
            // 
            this.chkb_bySer.AutoSize = true;
            this.chkb_bySer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkb_bySer.Checked = true;
            this.chkb_bySer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkb_bySer.Location = new System.Drawing.Point(129, 40);
            this.chkb_bySer.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_bySer.Name = "chkb_bySer";
            this.chkb_bySer.Size = new System.Drawing.Size(84, 16);
            this.chkb_bySer.TabIndex = 14;
            this.chkb_bySer.Text = "是否按排名";
            this.chkb_bySer.UseVisualStyleBackColor = true;
            // 
            // txt_minInputTimes
            // 
            this.txt_minInputTimes.Location = new System.Drawing.Point(78, 124);
            this.txt_minInputTimes.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minInputTimes.Name = "txt_minInputTimes";
            this.txt_minInputTimes.Size = new System.Drawing.Size(35, 21);
            this.txt_minInputTimes.TabIndex = 12;
            this.txt_minInputTimes.Text = "30";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(12, 128);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(53, 12);
            this.label22.TabIndex = 11;
            this.label22.Text = "最小入期";
            // 
            // txt_ChipCnt
            // 
            this.txt_ChipCnt.Location = new System.Drawing.Point(78, 102);
            this.txt_ChipCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_ChipCnt.Name = "txt_ChipCnt";
            this.txt_ChipCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_ChipCnt.TabIndex = 9;
            this.txt_ChipCnt.Text = "3";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 105);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 8;
            this.label8.Text = "注数";
            // 
            // txt_FixChipCnt
            // 
            this.txt_FixChipCnt.Location = new System.Drawing.Point(78, 80);
            this.txt_FixChipCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_FixChipCnt.Name = "txt_FixChipCnt";
            this.txt_FixChipCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_FixChipCnt.TabIndex = 7;
            this.txt_FixChipCnt.Text = "1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 84);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "是否定注";
            // 
            // txt_reviewExpCnt
            // 
            this.txt_reviewExpCnt.Location = new System.Drawing.Point(78, 58);
            this.txt_reviewExpCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_reviewExpCnt.Name = "txt_reviewExpCnt";
            this.txt_reviewExpCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_reviewExpCnt.TabIndex = 5;
            this.txt_reviewExpCnt.Text = "28";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "回览期数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 20);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "策略运行计划";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.txt_Timer_Interval);
            this.groupBox1.Controls.Add(this.label30);
            this.groupBox1.Controls.Add(this.checkBox_UseBuffRsult);
            this.groupBox1.Controls.Add(this.checkBox_MixAll);
            this.groupBox1.Controls.Add(this.checkBox_CreamModel);
            this.groupBox1.Controls.Add(this.txt_MaxHoldChanceCnt);
            this.groupBox1.Controls.Add(this.txt_InitCash);
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.txt_minColTimes10);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.txt_minColTimes9);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.txt_minColTimes8);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.txt_minColTimes7);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.txt_minColTimes6);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.txt_minColTimes5);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txt_minColTimes4);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txt_minColTimes3);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txt_minColTimes2);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txt_minColTimes1);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txt_GrownMaxVal);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txt_GrownMinVal);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txt_MinCols);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txt_Odds);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txt_LoopCnt);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_begExpNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, 12);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(330, 196);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "回测数据设置";
            // 
            // txt_Timer_Interval
            // 
            this.txt_Timer_Interval.Location = new System.Drawing.Point(61, 169);
            this.txt_Timer_Interval.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Timer_Interval.Name = "txt_Timer_Interval";
            this.txt_Timer_Interval.Size = new System.Drawing.Size(35, 21);
            this.txt_Timer_Interval.TabIndex = 38;
            this.txt_Timer_Interval.Text = "1";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(4, 175);
            this.label30.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(53, 12);
            this.label30.TabIndex = 37;
            this.label30.Text = "刷新速度";
            // 
            // checkBox_UseBuffRsult
            // 
            this.checkBox_UseBuffRsult.AutoSize = true;
            this.checkBox_UseBuffRsult.Location = new System.Drawing.Point(59, 149);
            this.checkBox_UseBuffRsult.Name = "checkBox_UseBuffRsult";
            this.checkBox_UseBuffRsult.Size = new System.Drawing.Size(96, 16);
            this.checkBox_UseBuffRsult.TabIndex = 36;
            this.checkBox_UseBuffRsult.Text = "使用缓存结果";
            this.checkBox_UseBuffRsult.UseVisualStyleBackColor = true;
            // 
            // checkBox_MixAll
            // 
            this.checkBox_MixAll.AutoSize = true;
            this.checkBox_MixAll.Location = new System.Drawing.Point(205, 171);
            this.checkBox_MixAll.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_MixAll.Name = "checkBox_MixAll";
            this.checkBox_MixAll.Size = new System.Drawing.Size(72, 16);
            this.checkBox_MixAll.TabIndex = 35;
            this.checkBox_MixAll.Text = "混合所有";
            this.checkBox_MixAll.UseVisualStyleBackColor = true;
            // 
            // checkBox_CreamModel
            // 
            this.checkBox_CreamModel.AutoSize = true;
            this.checkBox_CreamModel.Location = new System.Drawing.Point(205, 151);
            this.checkBox_CreamModel.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_CreamModel.Name = "checkBox_CreamModel";
            this.checkBox_CreamModel.Size = new System.Drawing.Size(72, 16);
            this.checkBox_CreamModel.TabIndex = 34;
            this.checkBox_CreamModel.Text = "复利模式";
            this.checkBox_CreamModel.UseVisualStyleBackColor = true;
            // 
            // txt_MaxHoldChanceCnt
            // 
            this.txt_MaxHoldChanceCnt.Location = new System.Drawing.Point(271, 124);
            this.txt_MaxHoldChanceCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_MaxHoldChanceCnt.Name = "txt_MaxHoldChanceCnt";
            this.txt_MaxHoldChanceCnt.Size = new System.Drawing.Size(55, 21);
            this.txt_MaxHoldChanceCnt.TabIndex = 27;
            this.txt_MaxHoldChanceCnt.Text = "100";
            // 
            // txt_InitCash
            // 
            this.txt_InitCash.Location = new System.Drawing.Point(271, 102);
            this.txt_InitCash.Margin = new System.Windows.Forms.Padding(2);
            this.txt_InitCash.Name = "txt_InitCash";
            this.txt_InitCash.Size = new System.Drawing.Size(55, 21);
            this.txt_InitCash.TabIndex = 33;
            this.txt_InitCash.Text = "20000";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(203, 128);
            this.label28.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(53, 12);
            this.label28.TabIndex = 26;
            this.label28.Text = "最大持仓";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(203, 105);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(29, 12);
            this.label27.TabIndex = 32;
            this.label27.Text = "本金";
            // 
            // txt_minColTimes10
            // 
            this.txt_minColTimes10.Location = new System.Drawing.Point(166, 116);
            this.txt_minColTimes10.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes10.Name = "txt_minColTimes10";
            this.txt_minColTimes10.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes10.TabIndex = 31;
            this.txt_minColTimes10.Text = "1";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(110, 118);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 12);
            this.label17.TabIndex = 30;
            this.label17.Text = "10码";
            // 
            // txt_minColTimes9
            // 
            this.txt_minColTimes9.Location = new System.Drawing.Point(166, 98);
            this.txt_minColTimes9.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes9.Name = "txt_minColTimes9";
            this.txt_minColTimes9.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes9.TabIndex = 29;
            this.txt_minColTimes9.Text = "6";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(110, 98);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(23, 12);
            this.label18.TabIndex = 28;
            this.label18.Text = "9码";
            // 
            // txt_minColTimes8
            // 
            this.txt_minColTimes8.Location = new System.Drawing.Point(166, 78);
            this.txt_minColTimes8.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes8.Name = "txt_minColTimes8";
            this.txt_minColTimes8.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes8.TabIndex = 27;
            this.txt_minColTimes8.Text = "8";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(110, 80);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(23, 12);
            this.label19.TabIndex = 26;
            this.label19.Text = "8码";
            // 
            // txt_minColTimes7
            // 
            this.txt_minColTimes7.Location = new System.Drawing.Point(166, 60);
            this.txt_minColTimes7.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes7.Name = "txt_minColTimes7";
            this.txt_minColTimes7.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes7.TabIndex = 25;
            this.txt_minColTimes7.Text = "12";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(110, 61);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(23, 12);
            this.label20.TabIndex = 24;
            this.label20.Text = "7码";
            // 
            // txt_minColTimes6
            // 
            this.txt_minColTimes6.Location = new System.Drawing.Point(166, 40);
            this.txt_minColTimes6.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes6.Name = "txt_minColTimes6";
            this.txt_minColTimes6.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes6.TabIndex = 23;
            this.txt_minColTimes6.Text = "15";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(110, 43);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(23, 12);
            this.label21.TabIndex = 22;
            this.label21.Text = "6码";
            // 
            // txt_minColTimes5
            // 
            this.txt_minColTimes5.Location = new System.Drawing.Point(59, 116);
            this.txt_minColTimes5.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes5.Name = "txt_minColTimes5";
            this.txt_minColTimes5.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes5.TabIndex = 21;
            this.txt_minColTimes5.Text = "16";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 118);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(23, 12);
            this.label16.TabIndex = 20;
            this.label16.Text = "5码";
            // 
            // txt_minColTimes4
            // 
            this.txt_minColTimes4.Location = new System.Drawing.Point(59, 98);
            this.txt_minColTimes4.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes4.Name = "txt_minColTimes4";
            this.txt_minColTimes4.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes4.TabIndex = 19;
            this.txt_minColTimes4.Text = "22";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 98);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(23, 12);
            this.label15.TabIndex = 18;
            this.label15.Text = "4码";
            // 
            // txt_minColTimes3
            // 
            this.txt_minColTimes3.Location = new System.Drawing.Point(59, 78);
            this.txt_minColTimes3.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes3.Name = "txt_minColTimes3";
            this.txt_minColTimes3.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes3.TabIndex = 17;
            this.txt_minColTimes3.Text = "31";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 80);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(23, 12);
            this.label14.TabIndex = 16;
            this.label14.Text = "3码";
            // 
            // txt_minColTimes2
            // 
            this.txt_minColTimes2.Location = new System.Drawing.Point(59, 60);
            this.txt_minColTimes2.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes2.Name = "txt_minColTimes2";
            this.txt_minColTimes2.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes2.TabIndex = 15;
            this.txt_minColTimes2.Text = "45";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 61);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(23, 12);
            this.label13.TabIndex = 14;
            this.label13.Text = "2码";
            // 
            // txt_minColTimes1
            // 
            this.txt_minColTimes1.Location = new System.Drawing.Point(59, 40);
            this.txt_minColTimes1.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes1.Name = "txt_minColTimes1";
            this.txt_minColTimes1.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes1.TabIndex = 13;
            this.txt_minColTimes1.Text = "70";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 43);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 12);
            this.label12.TabIndex = 12;
            this.label12.Text = "1码";
            // 
            // txt_GrownMaxVal
            // 
            this.txt_GrownMaxVal.Location = new System.Drawing.Point(271, 58);
            this.txt_GrownMaxVal.Margin = new System.Windows.Forms.Padding(2);
            this.txt_GrownMaxVal.Name = "txt_GrownMaxVal";
            this.txt_GrownMaxVal.Size = new System.Drawing.Size(55, 21);
            this.txt_GrownMaxVal.TabIndex = 11;
            this.txt_GrownMaxVal.Text = "10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(203, 61);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 10;
            this.label11.Text = "成长最大值";
            // 
            // txt_GrownMinVal
            // 
            this.txt_GrownMinVal.Location = new System.Drawing.Point(271, 36);
            this.txt_GrownMinVal.Margin = new System.Windows.Forms.Padding(2);
            this.txt_GrownMinVal.Name = "txt_GrownMinVal";
            this.txt_GrownMinVal.Size = new System.Drawing.Size(55, 21);
            this.txt_GrownMinVal.TabIndex = 9;
            this.txt_GrownMinVal.Text = "10";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(203, 42);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 8;
            this.label10.Text = "成长最小值";
            // 
            // txt_MinCols
            // 
            this.txt_MinCols.Location = new System.Drawing.Point(271, 14);
            this.txt_MinCols.Margin = new System.Windows.Forms.Padding(2);
            this.txt_MinCols.Name = "txt_MinCols";
            this.txt_MinCols.Size = new System.Drawing.Size(55, 21);
            this.txt_MinCols.TabIndex = 7;
            this.txt_MinCols.Text = "3";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(203, 18);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "隐藏期数";
            // 
            // txt_Odds
            // 
            this.txt_Odds.Location = new System.Drawing.Point(271, 80);
            this.txt_Odds.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Odds.Name = "txt_Odds";
            this.txt_Odds.Size = new System.Drawing.Size(55, 21);
            this.txt_Odds.TabIndex = 5;
            this.txt_Odds.Text = "9.75";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(204, 82);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "赔率";
            // 
            // txt_LoopCnt
            // 
            this.txt_LoopCnt.Location = new System.Drawing.Point(166, 14);
            this.txt_LoopCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_LoopCnt.Name = "txt_LoopCnt";
            this.txt_LoopCnt.Size = new System.Drawing.Size(36, 21);
            this.txt_LoopCnt.TabIndex = 3;
            this.txt_LoopCnt.Text = "1000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(110, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "批次期数";
            // 
            // txt_begExpNo
            // 
            this.txt_begExpNo.Location = new System.Drawing.Point(59, 16);
            this.txt_begExpNo.Margin = new System.Windows.Forms.Padding(2);
            this.txt_begExpNo.Name = "txt_begExpNo";
            this.txt_begExpNo.Size = new System.Drawing.Size(50, 21);
            this.txt_begExpNo.TabIndex = 1;
            this.txt_begExpNo.Text = "590963";
            this.txt_begExpNo.Click += new System.EventHandler(this.txt_begExpNo_Click);
            this.txt_begExpNo.DoubleClick += new System.EventHandler(this.txt_begExpNo_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "开始期号";
            // 
            // tabControl1
            // 
            this.tabControl1.ContextMenuStrip = this.contextMenuStrip_ForListView;
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Controls.Add(this.tabPage9);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(940, 299);
            this.tabControl1.TabIndex = 3;
            // 
            // contextMenuStrip_ForListView
            // 
            this.contextMenuStrip_ForListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_ExportExcel});
            this.contextMenuStrip_ForListView.Name = "contextMenuStrip_ForListView";
            this.contextMenuStrip_ForListView.Size = new System.Drawing.Size(101, 26);
            this.contextMenuStrip_ForListView.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_ForListView_Opening);
            // 
            // tsmi_ExportExcel
            // 
            this.tsmi_ExportExcel.Name = "tsmi_ExportExcel";
            this.tsmi_ExportExcel.Size = new System.Drawing.Size(100, 22);
            this.tsmi_ExportExcel.Text = "导出";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(932, 273);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "整体概率分布";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(2, 2);
            this.listView2.Margin = new System.Windows.Forms.Padding(2);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(928, 269);
            this.listView2.TabIndex = 0;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView2_ColumnClick);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(932, 273);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "小概率机会列表";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(2, 2);
            this.listView1.Margin = new System.Windows.Forms.Padding(2);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(928, 269);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listView3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(932, 273);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "滚动测试结果";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listView3
            // 
            this.listView3.ContextMenuStrip = this.contextMenuStrip_ForListView;
            this.listView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView3.FullRowSelect = true;
            this.listView3.GridLines = true;
            this.listView3.Location = new System.Drawing.Point(0, 0);
            this.listView3.Margin = new System.Windows.Forms.Padding(2);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(932, 273);
            this.listView3.TabIndex = 0;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            this.listView3.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView3_ColumnClick);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.chart1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(932, 273);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "资金曲线";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "收益率曲线";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(932, 273);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "收益率曲线图";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.dataGridView_ExchangeDetail);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(932, 273);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "交易明细";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // dataGridView_ExchangeDetail
            // 
            this.dataGridView_ExchangeDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ExchangeDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_ExchangeDetail.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_ExchangeDetail.Name = "dataGridView_ExchangeDetail";
            this.dataGridView_ExchangeDetail.RowTemplate.Height = 23;
            this.dataGridView_ExchangeDetail.Size = new System.Drawing.Size(932, 273);
            this.dataGridView_ExchangeDetail.TabIndex = 0;
            this.dataGridView_ExchangeDetail.DoubleClick += new System.EventHandler(this.dataGridView_ExchangeDetail_DoubleClick);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.chart_ForProb);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(932, 273);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "概率波动曲线";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // chart_ForProb
            // 
            chartArea2.Name = "ChartArea1";
            this.chart_ForProb.ChartAreas.Add(chartArea2);
            this.chart_ForProb.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.chart_ForProb.Legends.Add(legend2);
            this.chart_ForProb.Location = new System.Drawing.Point(3, 3);
            this.chart_ForProb.Name = "chart_ForProb";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "概率变动曲线";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Series2";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "Series3";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Legend = "Legend1";
            series5.Name = "Series4";
            this.chart_ForProb.Series.Add(series2);
            this.chart_ForProb.Series.Add(series3);
            this.chart_ForProb.Series.Add(series4);
            this.chart_ForProb.Series.Add(series5);
            this.chart_ForProb.Size = new System.Drawing.Size(926, 267);
            this.chart_ForProb.TabIndex = 0;
            this.chart_ForProb.Text = "chart2";
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.dataGridView_ProbData);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(932, 273);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "概率数据";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // dataGridView_ProbData
            // 
            this.dataGridView_ProbData.AllowUserToAddRows = false;
            this.dataGridView_ProbData.AllowUserToDeleteRows = false;
            this.dataGridView_ProbData.AllowUserToOrderColumns = true;
            this.dataGridView_ProbData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ProbData.ContextMenuStrip = this.contextMenuStrip_ForListView;
            this.dataGridView_ProbData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_ProbData.Location = new System.Drawing.Point(3, 3);
            this.dataGridView_ProbData.Name = "dataGridView_ProbData";
            this.dataGridView_ProbData.ReadOnly = true;
            this.dataGridView_ProbData.RowTemplate.Height = 23;
            this.dataGridView_ProbData.Size = new System.Drawing.Size(926, 267);
            this.dataGridView_ProbData.TabIndex = 0;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.chart_ForSystemStdDev);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(932, 273);
            this.tabPage8.TabIndex = 7;
            this.tabPage8.Text = "整体离散度变化曲线";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // chart_ForSystemStdDev
            // 
            chartArea3.Name = "ChartArea1";
            this.chart_ForSystemStdDev.ChartAreas.Add(chartArea3);
            this.chart_ForSystemStdDev.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Name = "Legend1";
            this.chart_ForSystemStdDev.Legends.Add(legend3);
            this.chart_ForSystemStdDev.Location = new System.Drawing.Point(3, 3);
            this.chart_ForSystemStdDev.Name = "chart_ForSystemStdDev";
            series6.BorderWidth = 5;
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Legend = "Legend1";
            series7.Name = "Series2";
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series8.Legend = "Legend1";
            series8.Name = "Series3";
            this.chart_ForSystemStdDev.Series.Add(series6);
            this.chart_ForSystemStdDev.Series.Add(series7);
            this.chart_ForSystemStdDev.Series.Add(series8);
            this.chart_ForSystemStdDev.Size = new System.Drawing.Size(926, 267);
            this.chart_ForSystemStdDev.TabIndex = 0;
            this.chart_ForSystemStdDev.Text = "系统整体散乱程度";
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.dg_forStdDev);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage9.Size = new System.Drawing.Size(932, 273);
            this.tabPage9.TabIndex = 8;
            this.tabPage9.Text = "整体离散度数据";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // dg_forStdDev
            // 
            this.dg_forStdDev.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_forStdDev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_forStdDev.Location = new System.Drawing.Point(3, 3);
            this.dg_forStdDev.Name = "dg_forStdDev";
            this.dg_forStdDev.RowTemplate.Height = 23;
            this.dg_forStdDev.Size = new System.Drawing.Size(926, 267);
            this.dg_forStdDev.TabIndex = 0;
            this.dg_forStdDev.DoubleClick += new System.EventHandler(this.dg_forStdDev_DoubleClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripProgressBar1,
            this.toolStripDropDownButton1,
            this.toolStripSplitButton1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 299);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 7, 0);
            this.statusStrip1.Size = new System.Drawing.Size(940, 36);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(400, 31);
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(400, 31);
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(250, 30);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 20);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(32, 20);
            this.toolStripSplitButton1.Text = "toolStripSplitButton1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // timer_Tip
            // 
            this.timer_Tip.Interval = 1000;
            this.timer_Tip.Tick += new System.EventHandler(this.timer_Tip_Tick);
            // 
            // BackTestFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 550);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.splitter1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "BackTestFrm";
            this.Text = "快乐猎车回测平台";
            this.Load += new System.EventHandler(this.BackTestFrm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.contextMenuStrip_ForListView.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ExchangeDetail)).EndInit();
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_ForProb)).EndInit();
            this.tabPage7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ProbData)).EndInit();
            this.tabPage8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_ForSystemStdDev)).EndInit();
            this.tabPage9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_forStdDev)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btn_startTest;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txt_reviewExpCnt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_LoopCnt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_begExpNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TextBox txt_ChipCnt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_FixChipCnt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_Odds;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.TextBox txt_minColTimes10;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txt_minColTimes9;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txt_minColTimes8;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txt_minColTimes7;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txt_minColTimes6;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txt_minColTimes5;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txt_minColTimes4;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt_minColTimes3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txt_minColTimes2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txt_minColTimes1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txt_GrownMaxVal;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_GrownMinVal;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_MinCols;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.TextBox txt_minInputTimes;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.Button btn_roundTest;
        private System.Windows.Forms.CheckBox chkb_bySer;
        private System.Windows.Forms.TextBox txt_maxInputTimes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkb_exclueBS;
        private System.Windows.Forms.CheckBox chkb_exclueSD;
        private System.Windows.Forms.CheckBox chkb_onlyBS;
        private System.Windows.Forms.CheckBox chkb_onlySD;
        private System.Windows.Forms.TextBox txt_RoundStepLong;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txt_RoundCycLong;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.CheckBox chkb_getRev;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_ForListView;
        private System.Windows.Forms.ToolStripMenuItem tsmi_ExportExcel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btn_CheckData;
        private System.Windows.Forms.TextBox txt_StdvCnt;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txt_minRate;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txt_InitCash;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button btn_VirExchange;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Timer timer_Tip;
        private System.Windows.Forms.TextBox txt_MaxHoldChanceCnt;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataGridView dataGridView_ExchangeDetail;
        private System.Windows.Forms.CheckBox checkBox_CreamModel;
        private System.Windows.Forms.TextBox txt_AllowMaxHoldTimeCnt;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.CheckBox checkBox_MixAll;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_ForProb;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.DataGridView dataGridView_ProbData;
        private System.Windows.Forms.CheckBox checkBox_UseBuffRsult;
        private System.Windows.Forms.Button btn_CalcEr;
        private System.Windows.Forms.TextBox txt_Timer_Interval;
        private System.Windows.Forms.Label label30;
        private ExchangeLib.RunPlanPicker runPlanPicker1;
        private System.Windows.Forms.Button btn_DistrCheck;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_ForSystemStdDev;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.DataGridView dg_forStdDev;
        private System.Windows.Forms.TextBox txt_LearnCnt;
        private System.Windows.Forms.Label label31;
    }
}
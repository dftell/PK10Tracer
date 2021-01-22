using WolfInv.com.ExchangeLib;

namespace BackTestSys
{
    partial class BackTestFrm<T>
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea8 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend8 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series23 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series16 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series24 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series25 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series26 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea9 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend9 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series27 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series28 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series29 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl_setting = new System.Windows.Forms.TabControl();
            this.tabPage_dataSource = new System.Windows.Forms.TabPage();
            this.chkb_useBechmark = new System.Windows.Forms.CheckBox();
            this.txt_benchMarkCode = new System.Windows.Forms.TextBox();
            this.chkb_useSpecList = new System.Windows.Forms.CheckBox();
            this.txt_SecPools = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.ddl_DataSource = new System.Windows.Forms.ComboBox();
            this.txt_endExpNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_begExpNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_LoopCnt = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.tabPage_usePlan = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage_conditionSetting = new System.Windows.Forms.TabPage();
            this.txt_LearnCnt = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
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
            this.tabPage_subCondition = new System.Windows.Forms.TabPage();
            this.txt_MaxHoldChanceCnt = new System.Windows.Forms.TextBox();
            this.txt_InitCash = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.txt_Odds = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
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
            this.tabPage_display = new System.Windows.Forms.TabPage();
            this.chkb_noDetailTable = new System.Windows.Forms.CheckBox();
            this.chkb_useOdds = new System.Windows.Forms.CheckBox();
            this.ckb_useCondition = new System.Windows.Forms.CheckBox();
            this.txt_Timer_Interval = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.checkBox_UseBuffRsult = new System.Windows.Forms.CheckBox();
            this.checkBox_MixAll = new System.Windows.Forms.CheckBox();
            this.checkBox_CreamModel = new System.Windows.Forms.CheckBox();
            this.tabPage_roundBackTest = new System.Windows.Forms.TabPage();
            this.txt_RoundStepLong = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txt_RoundCycLong = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.btn_singleTest = new System.Windows.Forms.Button();
            this.btn_trainPlan = new System.Windows.Forms.Button();
            this.btn_DistrCheck = new System.Windows.Forms.Button();
            this.btn_CalcEr = new System.Windows.Forms.Button();
            this.btn_VirExchange = new System.Windows.Forms.Button();
            this.btn_CheckData = new System.Windows.Forms.Button();
            this.btn_roundTest = new System.Windows.Forms.Button();
            this.btn_export = new System.Windows.Forms.Button();
            this.btn_startTest = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.contextMenuStrip_ForListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmi_ExportExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_ImportExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_ExportCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_DisplayAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.dataGridView_ExchangeDetail = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView2 = new System.Windows.Forms.ListView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listView3 = new System.Windows.Forms.ListView();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.chart_ForProb = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.dataGridView_ProbData = new System.Windows.Forms.DataGridView();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.chart_ForSystemStdDev = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.dg_forStdDev = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.timer_Tip = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl_setting.SuspendLayout();
            this.tabPage_dataSource.SuspendLayout();
            this.tabPage_usePlan.SuspendLayout();
            this.tabPage_conditionSetting.SuspendLayout();
            this.tabPage_subCondition.SuspendLayout();
            this.tabPage_display.SuspendLayout();
            this.tabPage_roundBackTest.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.contextMenuStrip_ForListView.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ExchangeDetail)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
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
            this.splitter1.Size = new System.Drawing.Size(2, 657);
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
            this.splitContainer1.Panel1.Controls.Add(this.tabControl_setting);
            this.splitContainer1.Panel1.Controls.Add(this.btn_singleTest);
            this.splitContainer1.Panel1.Controls.Add(this.btn_trainPlan);
            this.splitContainer1.Panel1.Controls.Add(this.btn_DistrCheck);
            this.splitContainer1.Panel1.Controls.Add(this.btn_CalcEr);
            this.splitContainer1.Panel1.Controls.Add(this.btn_VirExchange);
            this.splitContainer1.Panel1.Controls.Add(this.btn_CheckData);
            this.splitContainer1.Panel1.Controls.Add(this.btn_roundTest);
            this.splitContainer1.Panel1.Controls.Add(this.btn_export);
            this.splitContainer1.Panel1.Controls.Add(this.btn_startTest);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1192, 657);
            this.splitContainer1.SplitterDistance = 125;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControl_setting
            // 
            this.tabControl_setting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl_setting.Controls.Add(this.tabPage_dataSource);
            this.tabControl_setting.Controls.Add(this.tabPage_usePlan);
            this.tabControl_setting.Controls.Add(this.tabPage_conditionSetting);
            this.tabControl_setting.Controls.Add(this.tabPage_subCondition);
            this.tabControl_setting.Controls.Add(this.tabPage_display);
            this.tabControl_setting.Controls.Add(this.tabPage_roundBackTest);
            this.tabControl_setting.Location = new System.Drawing.Point(2, 6);
            this.tabControl_setting.Name = "tabControl_setting";
            this.tabControl_setting.SelectedIndex = 0;
            this.tabControl_setting.Size = new System.Drawing.Size(1098, 116);
            this.tabControl_setting.TabIndex = 32;
            // 
            // tabPage_dataSource
            // 
            this.tabPage_dataSource.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_dataSource.Controls.Add(this.chkb_useBechmark);
            this.tabPage_dataSource.Controls.Add(this.txt_benchMarkCode);
            this.tabPage_dataSource.Controls.Add(this.chkb_useSpecList);
            this.tabPage_dataSource.Controls.Add(this.txt_SecPools);
            this.tabPage_dataSource.Controls.Add(this.label32);
            this.tabPage_dataSource.Controls.Add(this.ddl_DataSource);
            this.tabPage_dataSource.Controls.Add(this.txt_endExpNo);
            this.tabPage_dataSource.Controls.Add(this.label1);
            this.tabPage_dataSource.Controls.Add(this.txt_begExpNo);
            this.tabPage_dataSource.Controls.Add(this.label2);
            this.tabPage_dataSource.Controls.Add(this.txt_LoopCnt);
            this.tabPage_dataSource.Controls.Add(this.label33);
            this.tabPage_dataSource.Location = new System.Drawing.Point(4, 22);
            this.tabPage_dataSource.Name = "tabPage_dataSource";
            this.tabPage_dataSource.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_dataSource.Size = new System.Drawing.Size(1090, 90);
            this.tabPage_dataSource.TabIndex = 0;
            this.tabPage_dataSource.Text = "数据源";
            // 
            // chkb_useBechmark
            // 
            this.chkb_useBechmark.AutoSize = true;
            this.chkb_useBechmark.Location = new System.Drawing.Point(553, 7);
            this.chkb_useBechmark.Name = "chkb_useBechmark";
            this.chkb_useBechmark.Size = new System.Drawing.Size(96, 16);
            this.chkb_useBechmark.TabIndex = 54;
            this.chkb_useBechmark.Text = "使用参考标的";
            this.chkb_useBechmark.UseVisualStyleBackColor = true;
            // 
            // txt_benchMarkCode
            // 
            this.txt_benchMarkCode.Location = new System.Drawing.Point(664, 5);
            this.txt_benchMarkCode.Name = "txt_benchMarkCode";
            this.txt_benchMarkCode.Size = new System.Drawing.Size(216, 21);
            this.txt_benchMarkCode.TabIndex = 53;
            this.txt_benchMarkCode.Text = "000001.SH";
            // 
            // chkb_useSpecList
            // 
            this.chkb_useSpecList.AutoSize = true;
            this.chkb_useSpecList.Checked = true;
            this.chkb_useSpecList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkb_useSpecList.Location = new System.Drawing.Point(217, 9);
            this.chkb_useSpecList.Name = "chkb_useSpecList";
            this.chkb_useSpecList.Size = new System.Drawing.Size(96, 16);
            this.chkb_useSpecList.TabIndex = 52;
            this.chkb_useSpecList.Text = "使用指定标的";
            this.chkb_useSpecList.UseVisualStyleBackColor = true;
            // 
            // txt_SecPools
            // 
            this.txt_SecPools.Location = new System.Drawing.Point(319, 6);
            this.txt_SecPools.Name = "txt_SecPools";
            this.txt_SecPools.Size = new System.Drawing.Size(216, 21);
            this.txt_SecPools.TabIndex = 51;
            this.txt_SecPools.Text = "600858.SH";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(21, 10);
            this.label32.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(41, 12);
            this.label32.TabIndex = 50;
            this.label32.Text = "数据源";
            // 
            // ddl_DataSource
            // 
            this.ddl_DataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddl_DataSource.FormattingEnabled = true;
            this.ddl_DataSource.Items.AddRange(new object[] {
            "PK10",
            "TXFFC",
            "CN_Stock_A"});
            this.ddl_DataSource.Location = new System.Drawing.Point(77, 7);
            this.ddl_DataSource.Margin = new System.Windows.Forms.Padding(2);
            this.ddl_DataSource.Name = "ddl_DataSource";
            this.ddl_DataSource.Size = new System.Drawing.Size(132, 20);
            this.ddl_DataSource.TabIndex = 49;
            // 
            // txt_endExpNo
            // 
            this.txt_endExpNo.Location = new System.Drawing.Point(385, 47);
            this.txt_endExpNo.Margin = new System.Windows.Forms.Padding(2);
            this.txt_endExpNo.Name = "txt_endExpNo";
            this.txt_endExpNo.Size = new System.Drawing.Size(76, 21);
            this.txt_endExpNo.TabIndex = 48;
            this.txt_endExpNo.Text = "20491231180";
            this.txt_endExpNo.DoubleClick += new System.EventHandler(this.txt_endExpNo_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 43;
            this.label1.Text = "开始期号";
            // 
            // txt_begExpNo
            // 
            this.txt_begExpNo.Location = new System.Drawing.Point(77, 47);
            this.txt_begExpNo.Margin = new System.Windows.Forms.Padding(2);
            this.txt_begExpNo.Name = "txt_begExpNo";
            this.txt_begExpNo.Size = new System.Drawing.Size(78, 21);
            this.txt_begExpNo.TabIndex = 44;
            this.txt_begExpNo.Text = "2008-01-01";
            this.txt_begExpNo.DoubleClick += new System.EventHandler(this.txt_begExpNo_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(169, 51);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 45;
            this.label2.Text = "批次期数";
            // 
            // txt_LoopCnt
            // 
            this.txt_LoopCnt.Location = new System.Drawing.Point(236, 48);
            this.txt_LoopCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_LoopCnt.Name = "txt_LoopCnt";
            this.txt_LoopCnt.Size = new System.Drawing.Size(77, 21);
            this.txt_LoopCnt.TabIndex = 46;
            this.txt_LoopCnt.Text = "200";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(326, 52);
            this.label33.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(53, 12);
            this.label33.TabIndex = 47;
            this.label33.Text = "截止期号";
            // 
            // tabPage_usePlan
            // 
            this.tabPage_usePlan.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_usePlan.Controls.Add(this.label3);
            this.tabPage_usePlan.Location = new System.Drawing.Point(4, 22);
            this.tabPage_usePlan.Name = "tabPage_usePlan";
            this.tabPage_usePlan.Size = new System.Drawing.Size(1090, 73);
            this.tabPage_usePlan.TabIndex = 5;
            this.tabPage_usePlan.Text = "回测用策略";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 27);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 44;
            this.label3.Text = "投资计划";
            // 
            // tabPage_conditionSetting
            // 
            this.tabPage_conditionSetting.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_conditionSetting.Controls.Add(this.txt_LearnCnt);
            this.tabPage_conditionSetting.Controls.Add(this.label31);
            this.tabPage_conditionSetting.Controls.Add(this.txt_AllowMaxHoldTimeCnt);
            this.tabPage_conditionSetting.Controls.Add(this.label29);
            this.tabPage_conditionSetting.Controls.Add(this.txt_minRate);
            this.tabPage_conditionSetting.Controls.Add(this.label26);
            this.tabPage_conditionSetting.Controls.Add(this.txt_StdvCnt);
            this.tabPage_conditionSetting.Controls.Add(this.label25);
            this.tabPage_conditionSetting.Controls.Add(this.chkb_getRev);
            this.tabPage_conditionSetting.Controls.Add(this.chkb_onlyBS);
            this.tabPage_conditionSetting.Controls.Add(this.chkb_onlySD);
            this.tabPage_conditionSetting.Controls.Add(this.chkb_exclueBS);
            this.tabPage_conditionSetting.Controls.Add(this.chkb_exclueSD);
            this.tabPage_conditionSetting.Controls.Add(this.txt_maxInputTimes);
            this.tabPage_conditionSetting.Controls.Add(this.label5);
            this.tabPage_conditionSetting.Controls.Add(this.chkb_bySer);
            this.tabPage_conditionSetting.Controls.Add(this.txt_minInputTimes);
            this.tabPage_conditionSetting.Controls.Add(this.label22);
            this.tabPage_conditionSetting.Controls.Add(this.txt_ChipCnt);
            this.tabPage_conditionSetting.Controls.Add(this.label8);
            this.tabPage_conditionSetting.Controls.Add(this.txt_FixChipCnt);
            this.tabPage_conditionSetting.Controls.Add(this.label7);
            this.tabPage_conditionSetting.Controls.Add(this.txt_reviewExpCnt);
            this.tabPage_conditionSetting.Controls.Add(this.label4);
            this.tabPage_conditionSetting.Location = new System.Drawing.Point(4, 22);
            this.tabPage_conditionSetting.Name = "tabPage_conditionSetting";
            this.tabPage_conditionSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_conditionSetting.Size = new System.Drawing.Size(1090, 73);
            this.tabPage_conditionSetting.TabIndex = 1;
            this.tabPage_conditionSetting.Text = "运行参数";
            // 
            // txt_LearnCnt
            // 
            this.txt_LearnCnt.Location = new System.Drawing.Point(76, 9);
            this.txt_LearnCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_LearnCnt.Name = "txt_LearnCnt";
            this.txt_LearnCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_LearnCnt.TabIndex = 54;
            this.txt_LearnCnt.Text = "180";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(10, 13);
            this.label31.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(53, 12);
            this.label31.TabIndex = 53;
            this.label31.Text = "学习期数";
            // 
            // txt_AllowMaxHoldTimeCnt
            // 
            this.txt_AllowMaxHoldTimeCnt.Location = new System.Drawing.Point(211, 116);
            this.txt_AllowMaxHoldTimeCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_AllowMaxHoldTimeCnt.Name = "txt_AllowMaxHoldTimeCnt";
            this.txt_AllowMaxHoldTimeCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_AllowMaxHoldTimeCnt.TabIndex = 52;
            this.txt_AllowMaxHoldTimeCnt.Text = "9";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(155, 121);
            this.label29.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(53, 12);
            this.label29.TabIndex = 51;
            this.label29.Text = "止损次数";
            // 
            // txt_minRate
            // 
            this.txt_minRate.Location = new System.Drawing.Point(182, 159);
            this.txt_minRate.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minRate.Name = "txt_minRate";
            this.txt_minRate.Size = new System.Drawing.Size(35, 21);
            this.txt_minRate.TabIndex = 50;
            this.txt_minRate.Text = "1.001";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(125, 162);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(53, 12);
            this.label26.TabIndex = 49;
            this.label26.Text = "最小赔率";
            // 
            // txt_StdvCnt
            // 
            this.txt_StdvCnt.Location = new System.Drawing.Point(77, 159);
            this.txt_StdvCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_StdvCnt.Name = "txt_StdvCnt";
            this.txt_StdvCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_StdvCnt.TabIndex = 48;
            this.txt_StdvCnt.Text = "1.5";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(12, 162);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(53, 12);
            this.label25.TabIndex = 47;
            this.label25.Text = "标准差数";
            // 
            // chkb_getRev
            // 
            this.chkb_getRev.AutoSize = true;
            this.chkb_getRev.Location = new System.Drawing.Point(160, 105);
            this.chkb_getRev.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_getRev.Name = "chkb_getRev";
            this.chkb_getRev.Size = new System.Drawing.Size(48, 16);
            this.chkb_getRev.TabIndex = 46;
            this.chkb_getRev.Text = "求反";
            this.chkb_getRev.UseVisualStyleBackColor = true;
            // 
            // chkb_onlyBS
            // 
            this.chkb_onlyBS.AutoSize = true;
            this.chkb_onlyBS.Location = new System.Drawing.Point(160, 87);
            this.chkb_onlyBS.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_onlyBS.Name = "chkb_onlyBS";
            this.chkb_onlyBS.Size = new System.Drawing.Size(84, 16);
            this.chkb_onlyBS.TabIndex = 45;
            this.chkb_onlyBS.Text = "只考虑大小";
            this.chkb_onlyBS.UseVisualStyleBackColor = true;
            // 
            // chkb_onlySD
            // 
            this.chkb_onlySD.AutoSize = true;
            this.chkb_onlySD.Location = new System.Drawing.Point(160, 67);
            this.chkb_onlySD.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_onlySD.Name = "chkb_onlySD";
            this.chkb_onlySD.Size = new System.Drawing.Size(84, 16);
            this.chkb_onlySD.TabIndex = 44;
            this.chkb_onlySD.Text = "只考虑单双";
            this.chkb_onlySD.UseVisualStyleBackColor = true;
            // 
            // chkb_exclueBS
            // 
            this.chkb_exclueBS.AutoSize = true;
            this.chkb_exclueBS.Location = new System.Drawing.Point(160, 47);
            this.chkb_exclueBS.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_exclueBS.Name = "chkb_exclueBS";
            this.chkb_exclueBS.Size = new System.Drawing.Size(72, 16);
            this.chkb_exclueBS.TabIndex = 43;
            this.chkb_exclueBS.Text = "排除大小";
            this.chkb_exclueBS.UseVisualStyleBackColor = true;
            // 
            // chkb_exclueSD
            // 
            this.chkb_exclueSD.AutoSize = true;
            this.chkb_exclueSD.Location = new System.Drawing.Point(160, 26);
            this.chkb_exclueSD.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_exclueSD.Name = "chkb_exclueSD";
            this.chkb_exclueSD.Size = new System.Drawing.Size(72, 16);
            this.chkb_exclueSD.TabIndex = 42;
            this.chkb_exclueSD.Text = "排除单双";
            this.chkb_exclueSD.UseVisualStyleBackColor = true;
            // 
            // txt_maxInputTimes
            // 
            this.txt_maxInputTimes.Location = new System.Drawing.Point(76, 119);
            this.txt_maxInputTimes.Margin = new System.Windows.Forms.Padding(2);
            this.txt_maxInputTimes.Name = "txt_maxInputTimes";
            this.txt_maxInputTimes.Size = new System.Drawing.Size(35, 21);
            this.txt_maxInputTimes.TabIndex = 41;
            this.txt_maxInputTimes.Text = "8";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 125);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 40;
            this.label5.Text = "最大入期";
            // 
            // chkb_bySer
            // 
            this.chkb_bySer.AutoSize = true;
            this.chkb_bySer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkb_bySer.Checked = true;
            this.chkb_bySer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkb_bySer.Location = new System.Drawing.Point(157, 9);
            this.chkb_bySer.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_bySer.Name = "chkb_bySer";
            this.chkb_bySer.Size = new System.Drawing.Size(84, 16);
            this.chkb_bySer.TabIndex = 39;
            this.chkb_bySer.Text = "是否按排名";
            this.chkb_bySer.UseVisualStyleBackColor = true;
            // 
            // txt_minInputTimes
            // 
            this.txt_minInputTimes.Location = new System.Drawing.Point(76, 97);
            this.txt_minInputTimes.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minInputTimes.Name = "txt_minInputTimes";
            this.txt_minInputTimes.Size = new System.Drawing.Size(35, 21);
            this.txt_minInputTimes.TabIndex = 38;
            this.txt_minInputTimes.Text = "30";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(10, 101);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(53, 12);
            this.label22.TabIndex = 37;
            this.label22.Text = "最小入期";
            // 
            // txt_ChipCnt
            // 
            this.txt_ChipCnt.Location = new System.Drawing.Point(76, 75);
            this.txt_ChipCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_ChipCnt.Name = "txt_ChipCnt";
            this.txt_ChipCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_ChipCnt.TabIndex = 36;
            this.txt_ChipCnt.Text = "3";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 78);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 35;
            this.label8.Text = "注数";
            // 
            // txt_FixChipCnt
            // 
            this.txt_FixChipCnt.Location = new System.Drawing.Point(76, 53);
            this.txt_FixChipCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_FixChipCnt.Name = "txt_FixChipCnt";
            this.txt_FixChipCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_FixChipCnt.TabIndex = 34;
            this.txt_FixChipCnt.Text = "1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 57);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 33;
            this.label7.Text = "是否定注";
            // 
            // txt_reviewExpCnt
            // 
            this.txt_reviewExpCnt.Location = new System.Drawing.Point(76, 31);
            this.txt_reviewExpCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_reviewExpCnt.Name = "txt_reviewExpCnt";
            this.txt_reviewExpCnt.Size = new System.Drawing.Size(35, 21);
            this.txt_reviewExpCnt.TabIndex = 32;
            this.txt_reviewExpCnt.Text = "180";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 35);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 31;
            this.label4.Text = "回览期数";
            // 
            // tabPage_subCondition
            // 
            this.tabPage_subCondition.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_subCondition.Controls.Add(this.txt_MaxHoldChanceCnt);
            this.tabPage_subCondition.Controls.Add(this.txt_InitCash);
            this.tabPage_subCondition.Controls.Add(this.label28);
            this.tabPage_subCondition.Controls.Add(this.label27);
            this.tabPage_subCondition.Controls.Add(this.txt_Odds);
            this.tabPage_subCondition.Controls.Add(this.label6);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes10);
            this.tabPage_subCondition.Controls.Add(this.label17);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes9);
            this.tabPage_subCondition.Controls.Add(this.label18);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes8);
            this.tabPage_subCondition.Controls.Add(this.label19);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes7);
            this.tabPage_subCondition.Controls.Add(this.label20);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes6);
            this.tabPage_subCondition.Controls.Add(this.label21);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes5);
            this.tabPage_subCondition.Controls.Add(this.label16);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes4);
            this.tabPage_subCondition.Controls.Add(this.label15);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes3);
            this.tabPage_subCondition.Controls.Add(this.label14);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes2);
            this.tabPage_subCondition.Controls.Add(this.label13);
            this.tabPage_subCondition.Controls.Add(this.txt_minColTimes1);
            this.tabPage_subCondition.Controls.Add(this.label12);
            this.tabPage_subCondition.Controls.Add(this.txt_GrownMaxVal);
            this.tabPage_subCondition.Controls.Add(this.label11);
            this.tabPage_subCondition.Controls.Add(this.txt_GrownMinVal);
            this.tabPage_subCondition.Controls.Add(this.label10);
            this.tabPage_subCondition.Controls.Add(this.txt_MinCols);
            this.tabPage_subCondition.Controls.Add(this.label9);
            this.tabPage_subCondition.Location = new System.Drawing.Point(4, 22);
            this.tabPage_subCondition.Name = "tabPage_subCondition";
            this.tabPage_subCondition.Size = new System.Drawing.Size(1090, 170);
            this.tabPage_subCondition.TabIndex = 2;
            this.tabPage_subCondition.Text = "约束条件";
            // 
            // txt_MaxHoldChanceCnt
            // 
            this.txt_MaxHoldChanceCnt.Location = new System.Drawing.Point(334, 142);
            this.txt_MaxHoldChanceCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_MaxHoldChanceCnt.Name = "txt_MaxHoldChanceCnt";
            this.txt_MaxHoldChanceCnt.Size = new System.Drawing.Size(55, 21);
            this.txt_MaxHoldChanceCnt.TabIndex = 61;
            this.txt_MaxHoldChanceCnt.Text = "100";
            // 
            // txt_InitCash
            // 
            this.txt_InitCash.Location = new System.Drawing.Point(334, 122);
            this.txt_InitCash.Margin = new System.Windows.Forms.Padding(2);
            this.txt_InitCash.Name = "txt_InitCash";
            this.txt_InitCash.Size = new System.Drawing.Size(55, 21);
            this.txt_InitCash.TabIndex = 63;
            this.txt_InitCash.Text = "10000000";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(266, 143);
            this.label28.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(53, 12);
            this.label28.TabIndex = 60;
            this.label28.Text = "最大持仓";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(266, 124);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(29, 12);
            this.label27.TabIndex = 62;
            this.label27.Text = "本金";
            // 
            // txt_Odds
            // 
            this.txt_Odds.Location = new System.Drawing.Point(334, 102);
            this.txt_Odds.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Odds.Name = "txt_Odds";
            this.txt_Odds.Size = new System.Drawing.Size(55, 21);
            this.txt_Odds.TabIndex = 59;
            this.txt_Odds.Text = "9.75";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(266, 104);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 58;
            this.label6.Text = "赔率";
            // 
            // txt_minColTimes10
            // 
            this.txt_minColTimes10.Location = new System.Drawing.Point(223, 91);
            this.txt_minColTimes10.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes10.Name = "txt_minColTimes10";
            this.txt_minColTimes10.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes10.TabIndex = 57;
            this.txt_minColTimes10.Text = "1";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(167, 93);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 12);
            this.label17.TabIndex = 56;
            this.label17.Text = "10码";
            // 
            // txt_minColTimes9
            // 
            this.txt_minColTimes9.Location = new System.Drawing.Point(223, 73);
            this.txt_minColTimes9.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes9.Name = "txt_minColTimes9";
            this.txt_minColTimes9.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes9.TabIndex = 55;
            this.txt_minColTimes9.Text = "6";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(167, 73);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(23, 12);
            this.label18.TabIndex = 54;
            this.label18.Text = "9码";
            // 
            // txt_minColTimes8
            // 
            this.txt_minColTimes8.Location = new System.Drawing.Point(223, 53);
            this.txt_minColTimes8.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes8.Name = "txt_minColTimes8";
            this.txt_minColTimes8.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes8.TabIndex = 53;
            this.txt_minColTimes8.Text = "8";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(167, 55);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(23, 12);
            this.label19.TabIndex = 52;
            this.label19.Text = "8码";
            // 
            // txt_minColTimes7
            // 
            this.txt_minColTimes7.Location = new System.Drawing.Point(223, 35);
            this.txt_minColTimes7.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes7.Name = "txt_minColTimes7";
            this.txt_minColTimes7.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes7.TabIndex = 51;
            this.txt_minColTimes7.Text = "12";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(167, 36);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(23, 12);
            this.label20.TabIndex = 50;
            this.label20.Text = "7码";
            // 
            // txt_minColTimes6
            // 
            this.txt_minColTimes6.Location = new System.Drawing.Point(223, 15);
            this.txt_minColTimes6.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes6.Name = "txt_minColTimes6";
            this.txt_minColTimes6.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes6.TabIndex = 49;
            this.txt_minColTimes6.Text = "15";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(167, 18);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(23, 12);
            this.label21.TabIndex = 48;
            this.label21.Text = "6码";
            // 
            // txt_minColTimes5
            // 
            this.txt_minColTimes5.Location = new System.Drawing.Point(74, 88);
            this.txt_minColTimes5.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes5.Name = "txt_minColTimes5";
            this.txt_minColTimes5.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes5.TabIndex = 47;
            this.txt_minColTimes5.Text = "16";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(18, 90);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(23, 12);
            this.label16.TabIndex = 46;
            this.label16.Text = "5码";
            // 
            // txt_minColTimes4
            // 
            this.txt_minColTimes4.Location = new System.Drawing.Point(74, 70);
            this.txt_minColTimes4.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes4.Name = "txt_minColTimes4";
            this.txt_minColTimes4.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes4.TabIndex = 45;
            this.txt_minColTimes4.Text = "22";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(18, 70);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(23, 12);
            this.label15.TabIndex = 44;
            this.label15.Text = "4码";
            // 
            // txt_minColTimes3
            // 
            this.txt_minColTimes3.Location = new System.Drawing.Point(74, 50);
            this.txt_minColTimes3.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes3.Name = "txt_minColTimes3";
            this.txt_minColTimes3.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes3.TabIndex = 43;
            this.txt_minColTimes3.Text = "31";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(18, 52);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(23, 12);
            this.label14.TabIndex = 42;
            this.label14.Text = "3码";
            // 
            // txt_minColTimes2
            // 
            this.txt_minColTimes2.Location = new System.Drawing.Point(74, 32);
            this.txt_minColTimes2.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes2.Name = "txt_minColTimes2";
            this.txt_minColTimes2.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes2.TabIndex = 41;
            this.txt_minColTimes2.Text = "45";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 33);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(23, 12);
            this.label13.TabIndex = 40;
            this.label13.Text = "2码";
            // 
            // txt_minColTimes1
            // 
            this.txt_minColTimes1.Location = new System.Drawing.Point(74, 12);
            this.txt_minColTimes1.Margin = new System.Windows.Forms.Padding(2);
            this.txt_minColTimes1.Name = "txt_minColTimes1";
            this.txt_minColTimes1.Size = new System.Drawing.Size(36, 21);
            this.txt_minColTimes1.TabIndex = 39;
            this.txt_minColTimes1.Text = "70";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 15);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 12);
            this.label12.TabIndex = 38;
            this.label12.Text = "1码";
            // 
            // txt_GrownMaxVal
            // 
            this.txt_GrownMaxVal.Location = new System.Drawing.Point(380, 52);
            this.txt_GrownMaxVal.Margin = new System.Windows.Forms.Padding(2);
            this.txt_GrownMaxVal.Name = "txt_GrownMaxVal";
            this.txt_GrownMaxVal.Size = new System.Drawing.Size(55, 21);
            this.txt_GrownMaxVal.TabIndex = 37;
            this.txt_GrownMaxVal.Text = "10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(311, 52);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 36;
            this.label11.Text = "成长最大值";
            // 
            // txt_GrownMinVal
            // 
            this.txt_GrownMinVal.Location = new System.Drawing.Point(380, 32);
            this.txt_GrownMinVal.Margin = new System.Windows.Forms.Padding(2);
            this.txt_GrownMinVal.Name = "txt_GrownMinVal";
            this.txt_GrownMinVal.Size = new System.Drawing.Size(55, 21);
            this.txt_GrownMinVal.TabIndex = 35;
            this.txt_GrownMinVal.Text = "10";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(312, 35);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 34;
            this.label10.Text = "成长最小值";
            // 
            // txt_MinCols
            // 
            this.txt_MinCols.Location = new System.Drawing.Point(380, 12);
            this.txt_MinCols.Margin = new System.Windows.Forms.Padding(2);
            this.txt_MinCols.Name = "txt_MinCols";
            this.txt_MinCols.Size = new System.Drawing.Size(55, 21);
            this.txt_MinCols.TabIndex = 33;
            this.txt_MinCols.Text = "3";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(312, 16);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 32;
            this.label9.Text = "隐藏期数";
            // 
            // tabPage_display
            // 
            this.tabPage_display.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_display.Controls.Add(this.chkb_noDetailTable);
            this.tabPage_display.Controls.Add(this.chkb_useOdds);
            this.tabPage_display.Controls.Add(this.ckb_useCondition);
            this.tabPage_display.Controls.Add(this.txt_Timer_Interval);
            this.tabPage_display.Controls.Add(this.label30);
            this.tabPage_display.Controls.Add(this.checkBox_UseBuffRsult);
            this.tabPage_display.Controls.Add(this.checkBox_MixAll);
            this.tabPage_display.Controls.Add(this.checkBox_CreamModel);
            this.tabPage_display.Location = new System.Drawing.Point(4, 22);
            this.tabPage_display.Name = "tabPage_display";
            this.tabPage_display.Size = new System.Drawing.Size(1090, 75);
            this.tabPage_display.TabIndex = 3;
            this.tabPage_display.Text = "显示设置";
            // 
            // chkb_noDetailTable
            // 
            this.chkb_noDetailTable.AutoSize = true;
            this.chkb_noDetailTable.Location = new System.Drawing.Point(356, 35);
            this.chkb_noDetailTable.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_noDetailTable.Name = "chkb_noDetailTable";
            this.chkb_noDetailTable.Size = new System.Drawing.Size(84, 16);
            this.chkb_noDetailTable.TabIndex = 53;
            this.chkb_noDetailTable.Text = "不显示明细";
            this.chkb_noDetailTable.UseVisualStyleBackColor = true;
            // 
            // chkb_useOdds
            // 
            this.chkb_useOdds.AutoSize = true;
            this.chkb_useOdds.Location = new System.Drawing.Point(236, 37);
            this.chkb_useOdds.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_useOdds.Name = "chkb_useOdds";
            this.chkb_useOdds.Size = new System.Drawing.Size(96, 16);
            this.chkb_useOdds.TabIndex = 52;
            this.chkb_useOdds.Text = "使用指定赔率";
            this.chkb_useOdds.UseVisualStyleBackColor = true;
            // 
            // ckb_useCondition
            // 
            this.ckb_useCondition.AutoSize = true;
            this.ckb_useCondition.Location = new System.Drawing.Point(115, 38);
            this.ckb_useCondition.Margin = new System.Windows.Forms.Padding(2);
            this.ckb_useCondition.Name = "ckb_useCondition";
            this.ckb_useCondition.Size = new System.Drawing.Size(96, 16);
            this.ckb_useCondition.TabIndex = 51;
            this.ckb_useCondition.Text = "使用指定条件";
            this.ckb_useCondition.UseVisualStyleBackColor = true;
            // 
            // txt_Timer_Interval
            // 
            this.txt_Timer_Interval.Location = new System.Drawing.Point(69, 9);
            this.txt_Timer_Interval.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Timer_Interval.Name = "txt_Timer_Interval";
            this.txt_Timer_Interval.Size = new System.Drawing.Size(35, 21);
            this.txt_Timer_Interval.TabIndex = 50;
            this.txt_Timer_Interval.Text = "25";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(12, 15);
            this.label30.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(53, 12);
            this.label30.TabIndex = 49;
            this.label30.Text = "刷新速度";
            // 
            // checkBox_UseBuffRsult
            // 
            this.checkBox_UseBuffRsult.AutoSize = true;
            this.checkBox_UseBuffRsult.Location = new System.Drawing.Point(115, 12);
            this.checkBox_UseBuffRsult.Name = "checkBox_UseBuffRsult";
            this.checkBox_UseBuffRsult.Size = new System.Drawing.Size(96, 16);
            this.checkBox_UseBuffRsult.TabIndex = 48;
            this.checkBox_UseBuffRsult.Text = "使用缓存结果";
            this.checkBox_UseBuffRsult.UseVisualStyleBackColor = true;
            // 
            // checkBox_MixAll
            // 
            this.checkBox_MixAll.AutoSize = true;
            this.checkBox_MixAll.Location = new System.Drawing.Point(356, 15);
            this.checkBox_MixAll.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_MixAll.Name = "checkBox_MixAll";
            this.checkBox_MixAll.Size = new System.Drawing.Size(72, 16);
            this.checkBox_MixAll.TabIndex = 47;
            this.checkBox_MixAll.Text = "混合所有";
            this.checkBox_MixAll.UseVisualStyleBackColor = true;
            // 
            // checkBox_CreamModel
            // 
            this.checkBox_CreamModel.AutoSize = true;
            this.checkBox_CreamModel.Location = new System.Drawing.Point(236, 14);
            this.checkBox_CreamModel.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_CreamModel.Name = "checkBox_CreamModel";
            this.checkBox_CreamModel.Size = new System.Drawing.Size(72, 16);
            this.checkBox_CreamModel.TabIndex = 46;
            this.checkBox_CreamModel.Text = "复利模式";
            this.checkBox_CreamModel.UseVisualStyleBackColor = true;
            // 
            // tabPage_roundBackTest
            // 
            this.tabPage_roundBackTest.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_roundBackTest.Controls.Add(this.txt_RoundStepLong);
            this.tabPage_roundBackTest.Controls.Add(this.label23);
            this.tabPage_roundBackTest.Controls.Add(this.txt_RoundCycLong);
            this.tabPage_roundBackTest.Controls.Add(this.label24);
            this.tabPage_roundBackTest.Location = new System.Drawing.Point(4, 22);
            this.tabPage_roundBackTest.Name = "tabPage_roundBackTest";
            this.tabPage_roundBackTest.Size = new System.Drawing.Size(1090, 73);
            this.tabPage_roundBackTest.TabIndex = 4;
            this.tabPage_roundBackTest.Text = "滚动设置";
            // 
            // txt_RoundStepLong
            // 
            this.txt_RoundStepLong.Location = new System.Drawing.Point(80, 29);
            this.txt_RoundStepLong.Margin = new System.Windows.Forms.Padding(2);
            this.txt_RoundStepLong.Name = "txt_RoundStepLong";
            this.txt_RoundStepLong.Size = new System.Drawing.Size(70, 21);
            this.txt_RoundStepLong.TabIndex = 15;
            this.txt_RoundStepLong.Text = "1000";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(13, 30);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(53, 12);
            this.label23.TabIndex = 14;
            this.label23.Text = "滚动步长";
            // 
            // txt_RoundCycLong
            // 
            this.txt_RoundCycLong.Location = new System.Drawing.Point(80, 9);
            this.txt_RoundCycLong.Margin = new System.Windows.Forms.Padding(2);
            this.txt_RoundCycLong.Name = "txt_RoundCycLong";
            this.txt_RoundCycLong.Size = new System.Drawing.Size(70, 21);
            this.txt_RoundCycLong.TabIndex = 13;
            this.txt_RoundCycLong.Text = "5000";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(13, 10);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(53, 12);
            this.label24.TabIndex = 12;
            this.label24.Text = "滚动周期";
            // 
            // btn_singleTest
            // 
            this.btn_singleTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_singleTest.Location = new System.Drawing.Point(1112, 187);
            this.btn_singleTest.Name = "btn_singleTest";
            this.btn_singleTest.Size = new System.Drawing.Size(61, 24);
            this.btn_singleTest.TabIndex = 13;
            this.btn_singleTest.Text = "单期测试";
            this.btn_singleTest.UseVisualStyleBackColor = true;
            this.btn_singleTest.Click += new System.EventHandler(this.btn_singleTest_Click);
            // 
            // btn_trainPlan
            // 
            this.btn_trainPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_trainPlan.Location = new System.Drawing.Point(1112, 164);
            this.btn_trainPlan.Name = "btn_trainPlan";
            this.btn_trainPlan.Size = new System.Drawing.Size(62, 21);
            this.btn_trainPlan.TabIndex = 11;
            this.btn_trainPlan.Text = "机器学习";
            this.btn_trainPlan.UseVisualStyleBackColor = true;
            this.btn_trainPlan.Click += new System.EventHandler(this.btn_trainPlan_Click);
            // 
            // btn_DistrCheck
            // 
            this.btn_DistrCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_DistrCheck.Location = new System.Drawing.Point(1112, 77);
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
            this.btn_CalcEr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_CalcEr.Location = new System.Drawing.Point(1112, 143);
            this.btn_CalcEr.Name = "btn_CalcEr";
            this.btn_CalcEr.Size = new System.Drawing.Size(62, 21);
            this.btn_CalcEr.TabIndex = 9;
            this.btn_CalcEr.Text = "计算器";
            this.btn_CalcEr.UseVisualStyleBackColor = true;
            this.btn_CalcEr.Click += new System.EventHandler(this.btn_CalcEr_Click);
            // 
            // btn_VirExchange
            // 
            this.btn_VirExchange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_VirExchange.Location = new System.Drawing.Point(1112, 33);
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
            this.btn_CheckData.Location = new System.Drawing.Point(1112, 121);
            this.btn_CheckData.Name = "btn_CheckData";
            this.btn_CheckData.Size = new System.Drawing.Size(61, 22);
            this.btn_CheckData.TabIndex = 7;
            this.btn_CheckData.Text = "验证数据";
            this.btn_CheckData.UseVisualStyleBackColor = true;
            this.btn_CheckData.Click += new System.EventHandler(this.btn_CheckData_Click);
            // 
            // btn_roundTest
            // 
            this.btn_roundTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_roundTest.Location = new System.Drawing.Point(1112, 55);
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
            this.btn_export.Location = new System.Drawing.Point(1112, 99);
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
            this.btn_startTest.Location = new System.Drawing.Point(1112, 11);
            this.btn_startTest.Margin = new System.Windows.Forms.Padding(2);
            this.btn_startTest.Name = "btn_startTest";
            this.btn_startTest.Size = new System.Drawing.Size(61, 22);
            this.btn_startTest.TabIndex = 2;
            this.btn_startTest.Text = "整体回测";
            this.btn_startTest.UseVisualStyleBackColor = true;
            this.btn_startTest.Click += new System.EventHandler(this.btn_startTest_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.ContextMenuStrip = this.contextMenuStrip_ForListView;
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Controls.Add(this.tabPage9);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1190, 528);
            this.tabControl1.TabIndex = 3;
            // 
            // contextMenuStrip_ForListView
            // 
            this.contextMenuStrip_ForListView.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip_ForListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_ExportExcel,
            this.tsmi_ImportExcel,
            this.tsmi_ExportCSV,
            this.tsmi_DisplayAll});
            this.contextMenuStrip_ForListView.Name = "contextMenuStrip_ForListView";
            this.contextMenuStrip_ForListView.Size = new System.Drawing.Size(125, 92);
            this.contextMenuStrip_ForListView.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_ForListView_Opening);
            // 
            // tsmi_ExportExcel
            // 
            this.tsmi_ExportExcel.Name = "tsmi_ExportExcel";
            this.tsmi_ExportExcel.Size = new System.Drawing.Size(124, 22);
            this.tsmi_ExportExcel.Text = "导出";
            // 
            // tsmi_ImportExcel
            // 
            this.tsmi_ImportExcel.Name = "tsmi_ImportExcel";
            this.tsmi_ImportExcel.Size = new System.Drawing.Size(124, 22);
            this.tsmi_ImportExcel.Text = "导入";
            // 
            // tsmi_ExportCSV
            // 
            this.tsmi_ExportCSV.Name = "tsmi_ExportCSV";
            this.tsmi_ExportCSV.Size = new System.Drawing.Size(124, 22);
            this.tsmi_ExportCSV.Text = "导出CSV";
            this.tsmi_ExportCSV.Click += new System.EventHandler(this.tsmi_ExportCSV_Click);
            // 
            // tsmi_DisplayAll
            // 
            this.tsmi_DisplayAll.Name = "tsmi_DisplayAll";
            this.tsmi_DisplayAll.Size = new System.Drawing.Size(124, 22);
            this.tsmi_DisplayAll.Text = "显示所有";
            this.tsmi_DisplayAll.Click += new System.EventHandler(this.tsmi_DisplayAll_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.chart1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1182, 502);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "资金曲线";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // chart1
            // 
            chartArea8.CursorX.IsUserEnabled = true;
            chartArea8.CursorX.IsUserSelectionEnabled = true;
            chartArea8.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea8);
            this.chart1.ContextMenuStrip = this.contextMenuStrip_ForListView;
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend8.Name = "Legend1";
            this.chart1.Legends.Add(legend8);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            series23.ChartArea = "ChartArea1";
            series23.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series23.Legend = "Legend1";
            series23.Name = "收益率曲线";
            this.chart1.Series.Add(series23);
            this.chart1.Size = new System.Drawing.Size(1182, 502);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "收益率曲线图";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.dataGridView_ExchangeDetail);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(1182, 517);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "交易明细";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // dataGridView_ExchangeDetail
            // 
            this.dataGridView_ExchangeDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ExchangeDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_ExchangeDetail.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_ExchangeDetail.MultiSelect = false;
            this.dataGridView_ExchangeDetail.Name = "dataGridView_ExchangeDetail";
            this.dataGridView_ExchangeDetail.RowTemplate.Height = 23;
            this.dataGridView_ExchangeDetail.ShowCellErrors = false;
            this.dataGridView_ExchangeDetail.ShowRowErrors = false;
            this.dataGridView_ExchangeDetail.Size = new System.Drawing.Size(1182, 517);
            this.dataGridView_ExchangeDetail.TabIndex = 0;
            this.dataGridView_ExchangeDetail.DoubleClick += new System.EventHandler(this.dataGridView_ExchangeDetail_DoubleClick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(1182, 517);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "整体概率分布";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.HideSelection = false;
            this.listView2.Location = new System.Drawing.Point(2, 2);
            this.listView2.Margin = new System.Windows.Forms.Padding(2);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(1178, 513);
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
            this.tabPage1.Size = new System.Drawing.Size(1182, 517);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "小概率机会列表";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(2, 2);
            this.listView1.Margin = new System.Windows.Forms.Padding(2);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1178, 513);
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
            this.tabPage3.Size = new System.Drawing.Size(1182, 517);
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
            this.listView3.HideSelection = false;
            this.listView3.Location = new System.Drawing.Point(0, 0);
            this.listView3.Margin = new System.Windows.Forms.Padding(2);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(1182, 517);
            this.listView3.TabIndex = 0;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            this.listView3.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView3_ColumnClick);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.chart_ForProb);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1182, 517);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "概率波动曲线";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // chart_ForProb
            // 
            chartArea6.Name = "ChartArea1";
            this.chart_ForProb.ChartAreas.Add(chartArea6);
            this.chart_ForProb.Dock = System.Windows.Forms.DockStyle.Fill;
            legend6.Name = "Legend1";
            this.chart_ForProb.Legends.Add(legend6);
            this.chart_ForProb.Location = new System.Drawing.Point(3, 3);
            this.chart_ForProb.Name = "chart_ForProb";
            series16.ChartArea = "ChartArea1";
            series16.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series16.Legend = "Legend1";
            series16.Name = "概率变动曲线";
            series24.ChartArea = "ChartArea1";
            series24.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series24.Legend = "Legend1";
            series24.Name = "Series2";
            series25.ChartArea = "ChartArea1";
            series25.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series25.Legend = "Legend1";
            series25.Name = "Series3";
            series26.ChartArea = "ChartArea1";
            series26.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series26.Legend = "Legend1";
            series26.Name = "Series4";
            this.chart_ForProb.Series.Add(series16);
            this.chart_ForProb.Series.Add(series24);
            this.chart_ForProb.Series.Add(series25);
            this.chart_ForProb.Series.Add(series26);
            this.chart_ForProb.Size = new System.Drawing.Size(1176, 511);
            this.chart_ForProb.TabIndex = 0;
            this.chart_ForProb.Text = "chart2";
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.dataGridView_ProbData);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1182, 517);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "概率数据";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // dataGridView_ProbData
            // 
            this.dataGridView_ProbData.AllowUserToAddRows = false;
            this.dataGridView_ProbData.AllowUserToDeleteRows = false;
            this.dataGridView_ProbData.AllowUserToOrderColumns = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ProbData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView_ProbData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ProbData.ContextMenuStrip = this.contextMenuStrip_ForListView;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_ProbData.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView_ProbData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_ProbData.Location = new System.Drawing.Point(3, 3);
            this.dataGridView_ProbData.Name = "dataGridView_ProbData";
            this.dataGridView_ProbData.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_ProbData.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView_ProbData.RowTemplate.Height = 23;
            this.dataGridView_ProbData.Size = new System.Drawing.Size(1176, 511);
            this.dataGridView_ProbData.TabIndex = 0;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.chart_ForSystemStdDev);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(1182, 517);
            this.tabPage8.TabIndex = 7;
            this.tabPage8.Text = "整体离散度变化曲线";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // chart_ForSystemStdDev
            // 
            chartArea9.Name = "ChartArea1";
            this.chart_ForSystemStdDev.ChartAreas.Add(chartArea9);
            this.chart_ForSystemStdDev.Dock = System.Windows.Forms.DockStyle.Fill;
            legend9.Name = "Legend1";
            this.chart_ForSystemStdDev.Legends.Add(legend9);
            this.chart_ForSystemStdDev.Location = new System.Drawing.Point(3, 3);
            this.chart_ForSystemStdDev.Name = "chart_ForSystemStdDev";
            series27.BorderWidth = 5;
            series27.ChartArea = "ChartArea1";
            series27.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series27.Legend = "Legend1";
            series27.Name = "Series1";
            series28.ChartArea = "ChartArea1";
            series28.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series28.Legend = "Legend1";
            series28.Name = "Series2";
            series29.ChartArea = "ChartArea1";
            series29.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series29.Legend = "Legend1";
            series29.Name = "Series3";
            this.chart_ForSystemStdDev.Series.Add(series27);
            this.chart_ForSystemStdDev.Series.Add(series28);
            this.chart_ForSystemStdDev.Series.Add(series29);
            this.chart_ForSystemStdDev.Size = new System.Drawing.Size(1176, 511);
            this.chart_ForSystemStdDev.TabIndex = 0;
            this.chart_ForSystemStdDev.Text = "系统整体散乱程度";
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.dg_forStdDev);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage9.Size = new System.Drawing.Size(1182, 517);
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
            this.dg_forStdDev.Size = new System.Drawing.Size(1176, 511);
            this.dg_forStdDev.TabIndex = 0;
            this.dg_forStdDev.DoubleClick += new System.EventHandler(this.dg_forStdDev_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // timer_Tip
            // 
            this.timer_Tip.Interval = 1000;
            this.timer_Tip.Tick += new System.EventHandler(this.timer_Tip_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 657);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 7, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1194, 25);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(400, 20);
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(400, 20);
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(380, 19);
            this.toolStripProgressBar1.Step = 1;
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // BackTestFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1194, 682);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.statusStrip1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "BackTestFrm";
            this.Text = "武府量化投资研究回测系统";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BackTestFrm_FormClosed);
            this.Load += new System.EventHandler(this.BackTestFrm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl_setting.ResumeLayout(false);
            this.tabPage_dataSource.ResumeLayout(false);
            this.tabPage_dataSource.PerformLayout();
            this.tabPage_usePlan.ResumeLayout(false);
            this.tabPage_usePlan.PerformLayout();
            this.tabPage_conditionSetting.ResumeLayout(false);
            this.tabPage_conditionSetting.PerformLayout();
            this.tabPage_subCondition.ResumeLayout(false);
            this.tabPage_subCondition.PerformLayout();
            this.tabPage_display.ResumeLayout(false);
            this.tabPage_display.PerformLayout();
            this.tabPage_roundBackTest.ResumeLayout(false);
            this.tabPage_roundBackTest.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.contextMenuStrip_ForListView.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ExchangeDetail)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
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
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.Button btn_roundTest;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_ForListView;
        private System.Windows.Forms.ToolStripMenuItem tsmi_ExportExcel;
        private System.Windows.Forms.ToolStripMenuItem tsmi_ImportExcel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btn_CheckData;
        private System.Windows.Forms.Button btn_VirExchange;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Timer timer_Tip;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataGridView dataGridView_ExchangeDetail;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_ForProb;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.DataGridView dataGridView_ProbData;
        private System.Windows.Forms.Button btn_CalcEr;
        private System.Windows.Forms.Button btn_DistrCheck;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_ForSystemStdDev;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.DataGridView dg_forStdDev;
        private System.Windows.Forms.Button btn_trainPlan;
        private RunPlanPicker<T> runPlanPicker1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripMenuItem tsmi_ExportCSV;
        private System.Windows.Forms.ToolStripMenuItem tsmi_DisplayAll;
        private System.Windows.Forms.Button btn_singleTest;
        private System.Windows.Forms.TabControl tabControl_setting;
        private System.Windows.Forms.TabPage tabPage_dataSource;
        private System.Windows.Forms.TextBox txt_SecPools;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.ComboBox ddl_DataSource;
        private System.Windows.Forms.TextBox txt_endExpNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_begExpNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_LoopCnt;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TabPage tabPage_conditionSetting;
        private System.Windows.Forms.TextBox txt_LearnCnt;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox txt_AllowMaxHoldTimeCnt;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox txt_minRate;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txt_StdvCnt;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.CheckBox chkb_getRev;
        private System.Windows.Forms.CheckBox chkb_onlyBS;
        private System.Windows.Forms.CheckBox chkb_onlySD;
        private System.Windows.Forms.CheckBox chkb_exclueBS;
        private System.Windows.Forms.CheckBox chkb_exclueSD;
        private System.Windows.Forms.TextBox txt_maxInputTimes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkb_bySer;
        private System.Windows.Forms.TextBox txt_minInputTimes;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txt_ChipCnt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_FixChipCnt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_reviewExpCnt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage_subCondition;
        private System.Windows.Forms.TextBox txt_MaxHoldChanceCnt;
        private System.Windows.Forms.TextBox txt_InitCash;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox txt_Odds;
        private System.Windows.Forms.Label label6;
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
        private System.Windows.Forms.TabPage tabPage_display;
        private System.Windows.Forms.CheckBox chkb_noDetailTable;
        private System.Windows.Forms.CheckBox chkb_useOdds;
        private System.Windows.Forms.CheckBox ckb_useCondition;
        private System.Windows.Forms.TextBox txt_Timer_Interval;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.CheckBox checkBox_UseBuffRsult;
        private System.Windows.Forms.CheckBox checkBox_MixAll;
        private System.Windows.Forms.CheckBox checkBox_CreamModel;
        private System.Windows.Forms.TabPage tabPage_roundBackTest;
        private System.Windows.Forms.TextBox txt_RoundStepLong;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txt_RoundCycLong;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TabPage tabPage_usePlan;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkb_useSpecList;
        private System.Windows.Forms.CheckBox chkb_useBechmark;
        private System.Windows.Forms.TextBox txt_benchMarkCode;
    }
}
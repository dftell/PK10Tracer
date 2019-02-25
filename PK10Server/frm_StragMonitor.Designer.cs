namespace PK10Server
{
    partial class frm_StragMonitor
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
            this.LogTimer.Enabled = false;
            this.LogTimer = null;
            this.PK10DataTimer.Close();
            this.PK10DataTimer.Enabled = false;
            this.PK10DataTimer = null;
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer_Main = new System.Windows.Forms.SplitContainer();
            this.splitContainer_Up = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dg_baseData = new System.Windows.Forms.DataGridView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.grpbox_control = new System.Windows.Forms.GroupBox();
            this.btn_adjustAssetTimeLength = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_AssetTimeLength = new System.Windows.Forms.TextBox();
            this.chart_ForGuide = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.dg_AssetUnits = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.dg_PlanGrps = new System.Windows.Forms.DataGridView();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.dg_stragStatus = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip_OperatePlan = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tmi_refreshPlans = new System.Windows.Forms.ToolStripMenuItem();
            this.tmi_StartPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.tmi_StopPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.tmi_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.dg_StragList = new System.Windows.Forms.DataGridView();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.dg_NoCloseChances = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dg_ExchangeList = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dg_LoginList = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.系统ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bootServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).BeginInit();
            this.splitContainer_Main.Panel1.SuspendLayout();
            this.splitContainer_Main.Panel2.SuspendLayout();
            this.splitContainer_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Up)).BeginInit();
            this.splitContainer_Up.Panel1.SuspendLayout();
            this.splitContainer_Up.Panel2.SuspendLayout();
            this.splitContainer_Up.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_baseData)).BeginInit();
            this.tabPage5.SuspendLayout();
            this.grpbox_control.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_ForGuide)).BeginInit();
            this.tabPage10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_AssetUnits)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_PlanGrps)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_stragStatus)).BeginInit();
            this.contextMenuStrip_OperatePlan.SuspendLayout();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_StragList)).BeginInit();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_NoCloseChances)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_ExchangeList)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_LoginList)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer_Main
            // 
            this.splitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Main.Location = new System.Drawing.Point(0, 43);
            this.splitContainer_Main.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.splitContainer_Main.Name = "splitContainer_Main";
            this.splitContainer_Main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_Main.Panel1
            // 
            this.splitContainer_Main.Panel1.Controls.Add(this.splitContainer_Up);
            // 
            // splitContainer_Main.Panel2
            // 
            this.splitContainer_Main.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer_Main.Size = new System.Drawing.Size(1540, 1103);
            this.splitContainer_Main.SplitterDistance = 781;
            this.splitContainer_Main.SplitterWidth = 8;
            this.splitContainer_Main.TabIndex = 0;
            // 
            // splitContainer_Up
            // 
            this.splitContainer_Up.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Up.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_Up.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.splitContainer_Up.Name = "splitContainer_Up";
            // 
            // splitContainer_Up.Panel1
            // 
            this.splitContainer_Up.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer_Up.Panel2
            // 
            this.splitContainer_Up.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer_Up.Size = new System.Drawing.Size(1540, 781);
            this.splitContainer_Up.SplitterDistance = 526;
            this.splitContainer_Up.SplitterWidth = 8;
            this.splitContainer_Up.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tabControl2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox2.Size = new System.Drawing.Size(526, 781);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "交易品种监控";
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage10);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(6, 34);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(514, 741);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.dg_baseData);
            this.tabPage4.Location = new System.Drawing.Point(8, 39);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage4.Size = new System.Drawing.Size(498, 694);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "基础数据";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dg_baseData
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_baseData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dg_baseData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg_baseData.DefaultCellStyle = dataGridViewCellStyle2;
            this.dg_baseData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_baseData.Location = new System.Drawing.Point(6, 6);
            this.dg_baseData.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dg_baseData.Name = "dg_baseData";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_baseData.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dg_baseData.RowTemplate.Height = 23;
            this.dg_baseData.Size = new System.Drawing.Size(486, 682);
            this.dg_baseData.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.grpbox_control);
            this.tabPage5.Controls.Add(this.chart_ForGuide);
            this.tabPage5.Location = new System.Drawing.Point(8, 39);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage5.Size = new System.Drawing.Size(498, 694);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "波动曲线图";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // grpbox_control
            // 
            this.grpbox_control.Controls.Add(this.btn_adjustAssetTimeLength);
            this.grpbox_control.Controls.Add(this.label1);
            this.grpbox_control.Controls.Add(this.txt_AssetTimeLength);
            this.grpbox_control.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpbox_control.Location = new System.Drawing.Point(6, 6);
            this.grpbox_control.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grpbox_control.Name = "grpbox_control";
            this.grpbox_control.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.grpbox_control.Size = new System.Drawing.Size(486, 70);
            this.grpbox_control.TabIndex = 1;
            this.grpbox_control.TabStop = false;
            this.grpbox_control.Text = "调整";
            // 
            // btn_adjustAssetTimeLength
            // 
            this.btn_adjustAssetTimeLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_adjustAssetTimeLength.Location = new System.Drawing.Point(378, 18);
            this.btn_adjustAssetTimeLength.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btn_adjustAssetTimeLength.Name = "btn_adjustAssetTimeLength";
            this.btn_adjustAssetTimeLength.Size = new System.Drawing.Size(86, 44);
            this.btn_adjustAssetTimeLength.TabIndex = 2;
            this.btn_adjustAssetTimeLength.Text = "确定";
            this.btn_adjustAssetTimeLength.UseVisualStyleBackColor = true;
            this.btn_adjustAssetTimeLength.Click += new System.EventHandler(this.btn_adjustAssetTimeLength_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "调整显示长度";
            // 
            // txt_AssetTimeLength
            // 
            this.txt_AssetTimeLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_AssetTimeLength.Location = new System.Drawing.Point(252, 18);
            this.txt_AssetTimeLength.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txt_AssetTimeLength.Name = "txt_AssetTimeLength";
            this.txt_AssetTimeLength.Size = new System.Drawing.Size(104, 35);
            this.txt_AssetTimeLength.TabIndex = 0;
            this.txt_AssetTimeLength.Text = "1000";
            // 
            // chart_ForGuide
            // 
            this.chart_ForGuide.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart_ForGuide.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart_ForGuide.Legends.Add(legend1);
            this.chart_ForGuide.Location = new System.Drawing.Point(6, 34);
            this.chart_ForGuide.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chart_ForGuide.Name = "chart_ForGuide";
            this.chart_ForGuide.RightToLeft = System.Windows.Forms.RightToLeft.No;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "数据4";
            this.chart_ForGuide.Series.Add(series1);
            this.chart_ForGuide.Size = new System.Drawing.Size(470, 604);
            this.chart_ForGuide.TabIndex = 0;
            this.chart_ForGuide.Text = "波动曲线图";
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.dg_AssetUnits);
            this.tabPage10.Location = new System.Drawing.Point(8, 39);
            this.tabPage10.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Size = new System.Drawing.Size(498, 663);
            this.tabPage10.TabIndex = 2;
            this.tabPage10.Text = "资产单元";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // dg_AssetUnits
            // 
            this.dg_AssetUnits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_AssetUnits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_AssetUnits.Location = new System.Drawing.Point(0, 0);
            this.dg_AssetUnits.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dg_AssetUnits.Name = "dg_AssetUnits";
            this.dg_AssetUnits.RowTemplate.Height = 23;
            this.dg_AssetUnits.Size = new System.Drawing.Size(498, 663);
            this.dg_AssetUnits.TabIndex = 0;
            this.dg_AssetUnits.DoubleClick += new System.EventHandler(this.dg_AssetUnits_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabControl3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(1006, 781);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "策略状态监控";
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage9);
            this.tabControl3.Controls.Add(this.tabPage6);
            this.tabControl3.Controls.Add(this.tabPage7);
            this.tabControl3.Controls.Add(this.tabPage8);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(6, 34);
            this.tabControl3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(994, 741);
            this.tabControl3.TabIndex = 1;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.dg_PlanGrps);
            this.tabPage9.Location = new System.Drawing.Point(8, 39);
            this.tabPage9.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Size = new System.Drawing.Size(978, 694);
            this.tabPage9.TabIndex = 3;
            this.tabPage9.Text = "分组状态";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // dg_PlanGrps
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_PlanGrps.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dg_PlanGrps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg_PlanGrps.DefaultCellStyle = dataGridViewCellStyle5;
            this.dg_PlanGrps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_PlanGrps.Location = new System.Drawing.Point(0, 0);
            this.dg_PlanGrps.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dg_PlanGrps.Name = "dg_PlanGrps";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_PlanGrps.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dg_PlanGrps.RowTemplate.Height = 23;
            this.dg_PlanGrps.Size = new System.Drawing.Size(978, 694);
            this.dg_PlanGrps.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.dg_stragStatus);
            this.tabPage6.Location = new System.Drawing.Point(8, 39);
            this.tabPage6.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage6.Size = new System.Drawing.Size(978, 663);
            this.tabPage6.TabIndex = 0;
            this.tabPage6.Text = "计划运行状态";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // dg_stragStatus
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_stragStatus.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dg_stragStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_stragStatus.ContextMenuStrip = this.contextMenuStrip_OperatePlan;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg_stragStatus.DefaultCellStyle = dataGridViewCellStyle8;
            this.dg_stragStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_stragStatus.Location = new System.Drawing.Point(6, 6);
            this.dg_stragStatus.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dg_stragStatus.MultiSelect = false;
            this.dg_stragStatus.Name = "dg_stragStatus";
            this.dg_stragStatus.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_stragStatus.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dg_stragStatus.RowTemplate.Height = 23;
            this.dg_stragStatus.Size = new System.Drawing.Size(966, 651);
            this.dg_stragStatus.TabIndex = 0;
            this.dg_stragStatus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dg_stragStatus_MouseUp);
            // 
            // contextMenuStrip_OperatePlan
            // 
            this.contextMenuStrip_OperatePlan.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip_OperatePlan.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmi_refreshPlans,
            this.tmi_StartPlan,
            this.tmi_StopPlan,
            this.tmi_Edit});
            this.contextMenuStrip_OperatePlan.Name = "contextMenuStrip_OperatePlan";
            this.contextMenuStrip_OperatePlan.Size = new System.Drawing.Size(233, 148);
            // 
            // tmi_refreshPlans
            // 
            this.tmi_refreshPlans.Name = "tmi_refreshPlans";
            this.tmi_refreshPlans.Size = new System.Drawing.Size(232, 36);
            this.tmi_refreshPlans.Text = "刷新";
            this.tmi_refreshPlans.Click += new System.EventHandler(this.tsmi_refreshPlans_Click);
            // 
            // tmi_StartPlan
            // 
            this.tmi_StartPlan.Name = "tmi_StartPlan";
            this.tmi_StartPlan.Size = new System.Drawing.Size(232, 36);
            this.tmi_StartPlan.Text = "开始运行计划";
            this.tmi_StartPlan.Click += new System.EventHandler(this.tmi_StartPlan_Click);
            // 
            // tmi_StopPlan
            // 
            this.tmi_StopPlan.Name = "tmi_StopPlan";
            this.tmi_StopPlan.Size = new System.Drawing.Size(232, 36);
            this.tmi_StopPlan.Text = "停止运行计划";
            this.tmi_StopPlan.Click += new System.EventHandler(this.tmi_StopPlan_Click);
            // 
            // tmi_Edit
            // 
            this.tmi_Edit.Name = "tmi_Edit";
            this.tmi_Edit.Size = new System.Drawing.Size(232, 36);
            this.tmi_Edit.Text = "Edit";
            this.tmi_Edit.Click += new System.EventHandler(this.tmi_Edit_Click);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.dg_StragList);
            this.tabPage7.Location = new System.Drawing.Point(8, 39);
            this.tabPage7.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage7.Size = new System.Drawing.Size(978, 663);
            this.tabPage7.TabIndex = 1;
            this.tabPage7.Text = "策略清单";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // dg_StragList
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_StragList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dg_StragList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg_StragList.DefaultCellStyle = dataGridViewCellStyle11;
            this.dg_StragList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_StragList.Location = new System.Drawing.Point(6, 6);
            this.dg_StragList.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dg_StragList.Name = "dg_StragList";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_StragList.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dg_StragList.RowTemplate.Height = 23;
            this.dg_StragList.Size = new System.Drawing.Size(966, 651);
            this.dg_StragList.TabIndex = 0;
            this.dg_StragList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dg_StragList_MouseUp);
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.dg_NoCloseChances);
            this.tabPage8.Location = new System.Drawing.Point(8, 39);
            this.tabPage8.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Size = new System.Drawing.Size(978, 663);
            this.tabPage8.TabIndex = 2;
            this.tabPage8.Text = "委托状态";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // dg_NoCloseChances
            // 
            this.dg_NoCloseChances.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_NoCloseChances.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_NoCloseChances.Location = new System.Drawing.Point(0, 0);
            this.dg_NoCloseChances.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dg_NoCloseChances.Name = "dg_NoCloseChances";
            this.dg_NoCloseChances.RowTemplate.Height = 23;
            this.dg_NoCloseChances.Size = new System.Drawing.Size(978, 663);
            this.dg_NoCloseChances.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1540, 314);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dg_ExchangeList);
            this.tabPage1.Location = new System.Drawing.Point(8, 39);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage1.Size = new System.Drawing.Size(1524, 267);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "交易记录";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dg_ExchangeList
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_ExchangeList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.dg_ExchangeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg_ExchangeList.DefaultCellStyle = dataGridViewCellStyle14;
            this.dg_ExchangeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_ExchangeList.Location = new System.Drawing.Point(6, 6);
            this.dg_ExchangeList.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dg_ExchangeList.Name = "dg_ExchangeList";
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_ExchangeList.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dg_ExchangeList.RowTemplate.Height = 23;
            this.dg_ExchangeList.Size = new System.Drawing.Size(1512, 255);
            this.dg_ExchangeList.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(8, 39);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage2.Size = new System.Drawing.Size(1524, 265);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "策略记录";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dg_LoginList);
            this.tabPage3.Location = new System.Drawing.Point(8, 39);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1524, 265);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "运行日志";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dg_LoginList
            // 
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_LoginList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle16;
            this.dg_LoginList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg_LoginList.DefaultCellStyle = dataGridViewCellStyle17;
            this.dg_LoginList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dg_LoginList.Location = new System.Drawing.Point(0, 0);
            this.dg_LoginList.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dg_LoginList.Name = "dg_LoginList";
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_LoginList.RowHeadersDefaultCellStyle = dataGridViewCellStyle18;
            this.dg_LoginList.RowTemplate.Height = 23;
            this.dg_LoginList.Size = new System.Drawing.Size(1524, 265);
            this.dg_LoginList.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.系统ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(12, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(1540, 43);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 系统ToolStripMenuItem
            // 
            this.系统ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bootServiceToolStripMenuItem,
            this.stopServiceToolStripMenuItem});
            this.系统ToolStripMenuItem.Name = "系统ToolStripMenuItem";
            this.系统ToolStripMenuItem.Size = new System.Drawing.Size(74, 35);
            this.系统ToolStripMenuItem.Text = "系统";
            // 
            // bootServiceToolStripMenuItem
            // 
            this.bootServiceToolStripMenuItem.Name = "bootServiceToolStripMenuItem";
            this.bootServiceToolStripMenuItem.Size = new System.Drawing.Size(248, 38);
            this.bootServiceToolStripMenuItem.Text = "BootService";
            this.bootServiceToolStripMenuItem.Click += new System.EventHandler(this.bootServiceToolStripMenuItem_Click);
            // 
            // stopServiceToolStripMenuItem
            // 
            this.stopServiceToolStripMenuItem.Name = "stopServiceToolStripMenuItem";
            this.stopServiceToolStripMenuItem.Size = new System.Drawing.Size(248, 38);
            this.stopServiceToolStripMenuItem.Text = "StopService";
            this.stopServiceToolStripMenuItem.Click += new System.EventHandler(this.stopServiceToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Location = new System.Drawing.Point(0, 1124);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 28, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1540, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // frm_StragMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1540, 1146);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer_Main);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frm_StragMonitor";
            this.Text = "策略运行监控窗口";
            this.Load += new System.EventHandler(this.frm_StragMonitor_Load);
            this.splitContainer_Main.Panel1.ResumeLayout(false);
            this.splitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).EndInit();
            this.splitContainer_Main.ResumeLayout(false);
            this.splitContainer_Up.Panel1.ResumeLayout(false);
            this.splitContainer_Up.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Up)).EndInit();
            this.splitContainer_Up.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_baseData)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.grpbox_control.ResumeLayout(false);
            this.grpbox_control.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_ForGuide)).EndInit();
            this.tabPage10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_AssetUnits)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_PlanGrps)).EndInit();
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_stragStatus)).EndInit();
            this.contextMenuStrip_OperatePlan.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_StragList)).EndInit();
            this.tabPage8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_NoCloseChances)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_ExchangeList)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_LoginList)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer_Main;
        private System.Windows.Forms.SplitContainer splitContainer_Up;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 系统ToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.DataGridView dg_stragStatus;
        private System.Windows.Forms.DataGridView dg_ExchangeList;
        private System.Windows.Forms.DataGridView dg_StragList;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dg_LoginList;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView dg_baseData;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_ForGuide;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.DataGridView dg_PlanGrps;
        private System.Windows.Forms.ToolStripMenuItem bootServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopServiceToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_OperatePlan;
        private System.Windows.Forms.ToolStripMenuItem tmi_StartPlan;
        private System.Windows.Forms.ToolStripMenuItem tmi_StopPlan;
        private System.Windows.Forms.ToolStripMenuItem tmi_refreshPlans;
        private System.Windows.Forms.DataGridView dg_NoCloseChances;
        private System.Windows.Forms.ToolStripMenuItem tmi_Edit;
        private System.Windows.Forms.TabPage tabPage10;
        private System.Windows.Forms.DataGridView dg_AssetUnits;
        private System.Windows.Forms.GroupBox grpbox_control;
        private System.Windows.Forms.Button btn_adjustAssetTimeLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_AssetTimeLength;
    }
}
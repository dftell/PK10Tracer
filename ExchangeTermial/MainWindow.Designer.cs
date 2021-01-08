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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmi_System = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_View = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_RefreshInsts = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_sendStatusInfor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Operate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRefreshWebToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_knockTheEgg = new System.Windows.Forms.ToolStripMenuItem();
            this.switchChanleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chargeMoneyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideTheFloatWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTheNavigateWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userLoginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inLotteryHomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchWebBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchPlatformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshBetRecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setLoginedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OCExchangeChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_Setting = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu_SetAssetUnitCnt = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cancelBetRecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshBetRecToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.webBrowser_charge = new System.Windows.Forms.WebBrowser();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.webBrowser_nav = new System.Windows.Forms.WebBrowser();
            this.geckoWebBrowser1 = new Gecko.GeckoWebBrowser();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage_summary = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ddl_datatype = new System.Windows.Forms.ComboBox();
            this.btn_Send = new System.Windows.Forms.Button();
            this.txt_OpenTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_OpenCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Insts = new System.Windows.Forms.TextBox();
            this.txt_ExpectNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btn_TestJscript = new System.Windows.Forms.Button();
            this.btn_AddHedge = new System.Windows.Forms.Button();
            this.btn_SelfAddCombo = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_NewInsts = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.pic_serImage = new System.Windows.Forms.PictureBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.pic_carImage = new System.Windows.Forms.PictureBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.pic_ChanceImage = new System.Windows.Forms.PictureBox();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.pic_chartImage = new System.Windows.Forms.PictureBox();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.dgv_matchGroupList = new System.Windows.Forms.DataGridView();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.dgv_betRecs = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage_summary.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_serImage)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_carImage)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ChanceImage)).BeginInit();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_chartImage)).BeginInit();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_matchGroupList)).BeginInit();
            this.tabPage9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_betRecs)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_System,
            this.tsmi_View,
            this.tsmi_Operate,
            this.tsmi_Setting});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1303, 25);
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
            this.tsmi_View.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnu_RefreshInsts,
            this.TSMI_sendStatusInfor});
            this.tsmi_View.Name = "tsmi_View";
            this.tsmi_View.Size = new System.Drawing.Size(44, 21);
            this.tsmi_View.Text = "查看";
            // 
            // mnu_RefreshInsts
            // 
            this.mnu_RefreshInsts.Name = "mnu_RefreshInsts";
            this.mnu_RefreshInsts.Size = new System.Drawing.Size(124, 22);
            this.mnu_RefreshInsts.Text = "刷新指令";
            this.mnu_RefreshInsts.Click += new System.EventHandler(this.mnu_RefreshInsts_Click);
            // 
            // TSMI_sendStatusInfor
            // 
            this.TSMI_sendStatusInfor.Name = "TSMI_sendStatusInfor";
            this.TSMI_sendStatusInfor.Size = new System.Drawing.Size(124, 22);
            this.TSMI_sendStatusInfor.Text = "上报状态";
            this.TSMI_sendStatusInfor.Click += new System.EventHandler(this.TSMI_sendStatusInfor_Click);
            // 
            // tsmi_Operate
            // 
            this.tsmi_Operate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRefreshWebToolStripMenuItem,
            this.reLoadToolStripMenuItem,
            this.tsmi_knockTheEgg,
            this.switchChanleToolStripMenuItem,
            this.chargeMoneyToolStripMenuItem,
            this.hideTheFloatWindowToolStripMenuItem,
            this.loadTheNavigateWindowToolStripMenuItem,
            this.userLoginToolStripMenuItem,
            this.inLotteryHomeToolStripMenuItem,
            this.switchWebBrowserToolStripMenuItem,
            this.switchPlatformToolStripMenuItem,
            this.refreshBetRecToolStripMenuItem,
            this.setLoginedToolStripMenuItem,
            this.OCExchangeChartToolStripMenuItem});
            this.tsmi_Operate.Name = "tsmi_Operate";
            this.tsmi_Operate.Size = new System.Drawing.Size(44, 21);
            this.tsmi_Operate.Text = "操作";
            // 
            // mnuRefreshWebToolStripMenuItem
            // 
            this.mnuRefreshWebToolStripMenuItem.Name = "mnuRefreshWebToolStripMenuItem";
            this.mnuRefreshWebToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.mnuRefreshWebToolStripMenuItem.Text = "睡觉";
            this.mnuRefreshWebToolStripMenuItem.Click += new System.EventHandler(this.mnuRefreshWebToolStripMenuItem_Click);
            // 
            // reLoadToolStripMenuItem
            // 
            this.reLoadToolStripMenuItem.Name = "reLoadToolStripMenuItem";
            this.reLoadToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.reLoadToolStripMenuItem.Text = "醒来";
            this.reLoadToolStripMenuItem.Click += new System.EventHandler(this.reLoadToolStripMenuItem_Click);
            // 
            // tsmi_knockTheEgg
            // 
            this.tsmi_knockTheEgg.Name = "tsmi_knockTheEgg";
            this.tsmi_knockTheEgg.Size = new System.Drawing.Size(177, 22);
            this.tsmi_knockTheEgg.Text = "砸蛋";
            this.tsmi_knockTheEgg.Click += new System.EventHandler(this.tsmi_knockTheEgg_Click);
            // 
            // switchChanleToolStripMenuItem
            // 
            this.switchChanleToolStripMenuItem.Name = "switchChanleToolStripMenuItem";
            this.switchChanleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.switchChanleToolStripMenuItem.Text = "切换通道";
            this.switchChanleToolStripMenuItem.Click += new System.EventHandler(this.switchChanleToolStripMenuItem_Click);
            // 
            // chargeMoneyToolStripMenuItem
            // 
            this.chargeMoneyToolStripMenuItem.Name = "chargeMoneyToolStripMenuItem";
            this.chargeMoneyToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.chargeMoneyToolStripMenuItem.Text = "ChargeMoney";
            this.chargeMoneyToolStripMenuItem.Click += new System.EventHandler(this.chargeMoneyToolStripMenuItem_Click);
            // 
            // hideTheFloatWindowToolStripMenuItem
            // 
            this.hideTheFloatWindowToolStripMenuItem.Name = "hideTheFloatWindowToolStripMenuItem";
            this.hideTheFloatWindowToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.hideTheFloatWindowToolStripMenuItem.Text = "隐藏/显示充值窗口";
            this.hideTheFloatWindowToolStripMenuItem.Click += new System.EventHandler(this.hideTheFloatWindowToolStripMenuItem_Click);
            // 
            // loadTheNavigateWindowToolStripMenuItem
            // 
            this.loadTheNavigateWindowToolStripMenuItem.Name = "loadTheNavigateWindowToolStripMenuItem";
            this.loadTheNavigateWindowToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.loadTheNavigateWindowToolStripMenuItem.Text = "载入导航";
            this.loadTheNavigateWindowToolStripMenuItem.Click += new System.EventHandler(this.loadTheNavigateWindowToolStripMenuItem_Click);
            // 
            // userLoginToolStripMenuItem
            // 
            this.userLoginToolStripMenuItem.Name = "userLoginToolStripMenuItem";
            this.userLoginToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.userLoginToolStripMenuItem.Text = "用户登录";
            this.userLoginToolStripMenuItem.Click += new System.EventHandler(this.userLoginToolStripMenuItem_Click);
            // 
            // inLotteryHomeToolStripMenuItem
            // 
            this.inLotteryHomeToolStripMenuItem.Name = "inLotteryHomeToolStripMenuItem";
            this.inLotteryHomeToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.inLotteryHomeToolStripMenuItem.Text = "进入游戏厅";
            this.inLotteryHomeToolStripMenuItem.Click += new System.EventHandler(this.inLotteryHomeToolStripMenuItem_Click);
            // 
            // switchWebBrowserToolStripMenuItem
            // 
            this.switchWebBrowserToolStripMenuItem.Name = "switchWebBrowserToolStripMenuItem";
            this.switchWebBrowserToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.switchWebBrowserToolStripMenuItem.Text = "切换浏览器";
            this.switchWebBrowserToolStripMenuItem.Click += new System.EventHandler(this.switchWebBrowserToolStripMenuItem_Click);
            // 
            // switchPlatformToolStripMenuItem
            // 
            this.switchPlatformToolStripMenuItem.Name = "switchPlatformToolStripMenuItem";
            this.switchPlatformToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.switchPlatformToolStripMenuItem.Text = "切换平台";
            // 
            // refreshBetRecToolStripMenuItem
            // 
            this.refreshBetRecToolStripMenuItem.Name = "refreshBetRecToolStripMenuItem";
            this.refreshBetRecToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.refreshBetRecToolStripMenuItem.Text = "刷新投注记录";
            this.refreshBetRecToolStripMenuItem.Click += new System.EventHandler(this.refreshBetRecToolStripMenuItem_Click);
            // 
            // setLoginedToolStripMenuItem
            // 
            this.setLoginedToolStripMenuItem.Name = "setLoginedToolStripMenuItem";
            this.setLoginedToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.setLoginedToolStripMenuItem.Text = "手动设置登录成功";
            this.setLoginedToolStripMenuItem.Click += new System.EventHandler(this.setLoginedToolStripMenuItem_Click);
            // 
            // OCExchangeChartToolStripMenuItem
            // 
            this.OCExchangeChartToolStripMenuItem.Name = "OCExchangeChartToolStripMenuItem";
            this.OCExchangeChartToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.OCExchangeChartToolStripMenuItem.Text = "开启/屏蔽交易曲线";
            this.OCExchangeChartToolStripMenuItem.Click += new System.EventHandler(this.OCExchangeChartToolStripMenuItem_Click);
            // 
            // tsmi_Setting
            // 
            this.tsmi_Setting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnu_SetAssetUnitCnt});
            this.tsmi_Setting.Name = "tsmi_Setting";
            this.tsmi_Setting.Size = new System.Drawing.Size(44, 21);
            this.tsmi_Setting.Text = "设置";
            // 
            // mnu_SetAssetUnitCnt
            // 
            this.mnu_SetAssetUnitCnt.Name = "mnu_SetAssetUnitCnt";
            this.mnu_SetAssetUnitCnt.Size = new System.Drawing.Size(172, 22);
            this.mnu_SetAssetUnitCnt.Text = "资产单元投资规模";
            this.mnu_SetAssetUnitCnt.Click += new System.EventHandler(this.mnu_SetAssetUnitCnt_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1303, 669);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Controls.Add(this.webBrowser_charge);
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1295, 643);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "概要";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chart1);
            this.splitContainer1.Panel2.Controls.Add(this.webBrowser1);
            this.splitContainer1.Size = new System.Drawing.Size(1289, 637);
            this.splitContainer1.SplitterDistance = 242;
            this.splitContainer1.TabIndex = 5;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cancelBetRecToolStripMenuItem,
            this.refreshBetRecToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 48);
            // 
            // cancelBetRecToolStripMenuItem
            // 
            this.cancelBetRecToolStripMenuItem.Name = "cancelBetRecToolStripMenuItem";
            this.cancelBetRecToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.cancelBetRecToolStripMenuItem.Text = "撤消下注";
            this.cancelBetRecToolStripMenuItem.Click += new System.EventHandler(this.cancelBetRecToolStripMenuItem_Click);
            // 
            // refreshBetRecToolStripMenuItem1
            // 
            this.refreshBetRecToolStripMenuItem1.Name = "refreshBetRecToolStripMenuItem1";
            this.refreshBetRecToolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
            this.refreshBetRecToolStripMenuItem1.Text = "刷新下注信息";
            this.refreshBetRecToolStripMenuItem1.Click += new System.EventHandler(this.refreshBetRecToolStripMenuItem1_Click);
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(79, 0);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(671, 363);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            // 
            // webBrowser1
            // 
            this.webBrowser1.CausesValidation = false;
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(1289, 391);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Url = new System.Uri("", System.UriKind.Relative);
            this.webBrowser1.Visible = false;
            // 
            // webBrowser_charge
            // 
            this.webBrowser_charge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser_charge.Location = new System.Drawing.Point(915, 201);
            this.webBrowser_charge.Margin = new System.Windows.Forms.Padding(2);
            this.webBrowser_charge.MinimumSize = new System.Drawing.Size(10, 10);
            this.webBrowser_charge.Name = "webBrowser_charge";
            this.webBrowser_charge.Size = new System.Drawing.Size(323, 283);
            this.webBrowser_charge.TabIndex = 3;
            this.webBrowser_charge.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(592, 289);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(116, 42);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage2.Controls.Add(this.webBrowser_nav);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1295, 643);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "数据";
            // 
            // webBrowser_nav
            // 
            this.webBrowser_nav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser_nav.Location = new System.Drawing.Point(3, 3);
            this.webBrowser_nav.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser_nav.Name = "webBrowser_nav";
            this.webBrowser_nav.ScriptErrorsSuppressed = true;
            this.webBrowser_nav.Size = new System.Drawing.Size(1289, 637);
            this.webBrowser_nav.TabIndex = 0;
            // 
            // geckoWebBrowser1
            // 
            this.geckoWebBrowser1.FrameEventsPropagateToMainWindow = false;
            this.geckoWebBrowser1.Location = new System.Drawing.Point(0, 0);
            this.geckoWebBrowser1.Name = "geckoWebBrowser1";
            this.geckoWebBrowser1.Size = new System.Drawing.Size(0, 0);
            this.geckoWebBrowser1.TabIndex = 0;
            this.geckoWebBrowser1.UseHttpActivityObserver = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 694);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1303, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(400, 17);
            this.toolStripStatusLabel1.Text = "    ";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(200, 17);
            this.toolStripStatusLabel2.Text = "Status";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.AutoSize = false;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(500, 17);
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage_summary);
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Controls.Add(this.tabPage7);
            this.tabControl2.Controls.Add(this.tabPage8);
            this.tabControl2.Controls.Add(this.tabPage9);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1289, 242);
            this.tabControl2.TabIndex = 4;
            // 
            // tabPage_summary
            // 
            this.tabPage_summary.Controls.Add(this.groupBox1);
            this.tabPage_summary.Location = new System.Drawing.Point(4, 22);
            this.tabPage_summary.Name = "tabPage_summary";
            this.tabPage_summary.Size = new System.Drawing.Size(1281, 216);
            this.tabPage_summary.TabIndex = 7;
            this.tabPage_summary.Text = "概要";
            this.tabPage_summary.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.ddl_datatype);
            this.groupBox1.Controls.Add(this.btn_Send);
            this.groupBox1.Controls.Add(this.txt_OpenTime);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txt_OpenCode);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_Insts);
            this.groupBox1.Controls.Add(this.txt_ExpectNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1281, 216);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "***";
            // 
            // ddl_datatype
            // 
            this.ddl_datatype.FormattingEnabled = true;
            this.ddl_datatype.Location = new System.Drawing.Point(77, 21);
            this.ddl_datatype.Name = "ddl_datatype";
            this.ddl_datatype.Size = new System.Drawing.Size(59, 20);
            this.ddl_datatype.TabIndex = 8;
            // 
            // btn_Send
            // 
            this.btn_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Send.Location = new System.Drawing.Point(1182, 19);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(93, 22);
            this.btn_Send.TabIndex = 7;
            this.btn_Send.Text = "手动发送";
            this.btn_Send.UseVisualStyleBackColor = true;
            // 
            // txt_OpenTime
            // 
            this.txt_OpenTime.Location = new System.Drawing.Point(577, 19);
            this.txt_OpenTime.Name = "txt_OpenTime";
            this.txt_OpenTime.Size = new System.Drawing.Size(124, 21);
            this.txt_OpenTime.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(502, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "上期时间";
            this.label3.Visible = false;
            // 
            // txt_OpenCode
            // 
            this.txt_OpenCode.Location = new System.Drawing.Point(399, 19);
            this.txt_OpenCode.Name = "txt_OpenCode";
            this.txt_OpenCode.Size = new System.Drawing.Size(80, 21);
            this.txt_OpenCode.TabIndex = 4;
            this.txt_OpenCode.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(325, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "上期结果";
            // 
            // txt_Insts
            // 
            this.txt_Insts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Insts.Location = new System.Drawing.Point(14, 46);
            this.txt_Insts.Multiline = true;
            this.txt_Insts.Name = "txt_Insts";
            this.txt_Insts.Size = new System.Drawing.Size(1261, 162);
            this.txt_Insts.TabIndex = 2;
            // 
            // txt_ExpectNo
            // 
            this.txt_ExpectNo.Location = new System.Drawing.Point(210, 19);
            this.txt_ExpectNo.Name = "txt_ExpectNo";
            this.txt_ExpectNo.Size = new System.Drawing.Size(95, 21);
            this.txt_ExpectNo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(142, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "交易日期";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btn_TestJscript);
            this.tabPage3.Controls.Add(this.btn_AddHedge);
            this.tabPage3.Controls.Add(this.btn_SelfAddCombo);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.txt_NewInsts);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(568, 216);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "指令信息";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btn_TestJscript
            // 
            this.btn_TestJscript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_TestJscript.Location = new System.Drawing.Point(506, 78);
            this.btn_TestJscript.Name = "btn_TestJscript";
            this.btn_TestJscript.Size = new System.Drawing.Size(45, 21);
            this.btn_TestJscript.TabIndex = 15;
            this.btn_TestJscript.Text = "测脚本";
            this.btn_TestJscript.UseVisualStyleBackColor = true;
            this.btn_TestJscript.Visible = false;
            // 
            // btn_AddHedge
            // 
            this.btn_AddHedge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_AddHedge.Location = new System.Drawing.Point(506, 24);
            this.btn_AddHedge.Name = "btn_AddHedge";
            this.btn_AddHedge.Size = new System.Drawing.Size(45, 21);
            this.btn_AddHedge.TabIndex = 14;
            this.btn_AddHedge.Text = "加对冲";
            this.btn_AddHedge.UseVisualStyleBackColor = true;
            this.btn_AddHedge.Visible = false;
            // 
            // btn_SelfAddCombo
            // 
            this.btn_SelfAddCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_SelfAddCombo.Location = new System.Drawing.Point(506, 51);
            this.btn_SelfAddCombo.Name = "btn_SelfAddCombo";
            this.btn_SelfAddCombo.Size = new System.Drawing.Size(45, 21);
            this.btn_SelfAddCombo.TabIndex = 13;
            this.btn_SelfAddCombo.Text = "加组合";
            this.btn_SelfAddCombo.UseVisualStyleBackColor = true;
            this.btn_SelfAddCombo.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "自定义交易";
            // 
            // txt_NewInsts
            // 
            this.txt_NewInsts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_NewInsts.Location = new System.Drawing.Point(6, 24);
            this.txt_NewInsts.Multiline = true;
            this.txt_NewInsts.Name = "txt_NewInsts";
            this.txt_NewInsts.Size = new System.Drawing.Size(494, 188);
            this.txt_NewInsts.TabIndex = 10;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.pic_serImage);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(568, 216);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "证券信息";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // pic_serImage
            // 
            this.pic_serImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic_serImage.Location = new System.Drawing.Point(3, 3);
            this.pic_serImage.Name = "pic_serImage";
            this.pic_serImage.Size = new System.Drawing.Size(562, 210);
            this.pic_serImage.TabIndex = 0;
            this.pic_serImage.TabStop = false;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.pic_carImage);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(568, 216);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "其他信息";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // pic_carImage
            // 
            this.pic_carImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic_carImage.Location = new System.Drawing.Point(0, 0);
            this.pic_carImage.Name = "pic_carImage";
            this.pic_carImage.Size = new System.Drawing.Size(568, 216);
            this.pic_carImage.TabIndex = 1;
            this.pic_carImage.TabStop = false;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.pic_ChanceImage);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(568, 216);
            this.tabPage6.TabIndex = 3;
            this.tabPage6.Text = "持仓列表";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // pic_ChanceImage
            // 
            this.pic_ChanceImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic_ChanceImage.Location = new System.Drawing.Point(0, 0);
            this.pic_ChanceImage.Name = "pic_ChanceImage";
            this.pic_ChanceImage.Size = new System.Drawing.Size(568, 216);
            this.pic_ChanceImage.TabIndex = 1;
            this.pic_ChanceImage.TabStop = false;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.pic_chartImage);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(568, 216);
            this.tabPage7.TabIndex = 4;
            this.tabPage7.Text = "走势图";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // pic_chartImage
            // 
            this.pic_chartImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic_chartImage.Location = new System.Drawing.Point(0, 0);
            this.pic_chartImage.Name = "pic_chartImage";
            this.pic_chartImage.Size = new System.Drawing.Size(568, 216);
            this.pic_chartImage.TabIndex = 1;
            this.pic_chartImage.TabStop = false;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.dgv_matchGroupList);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Size = new System.Drawing.Size(568, 216);
            this.tabPage8.TabIndex = 5;
            this.tabPage8.Text = "择时信息";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // dgv_matchGroupList
            // 
            this.dgv_matchGroupList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_matchGroupList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_matchGroupList.Location = new System.Drawing.Point(0, 0);
            this.dgv_matchGroupList.Name = "dgv_matchGroupList";
            this.dgv_matchGroupList.RowTemplate.Height = 23;
            this.dgv_matchGroupList.Size = new System.Drawing.Size(568, 216);
            this.dgv_matchGroupList.TabIndex = 0;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.dgv_betRecs);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Size = new System.Drawing.Size(568, 216);
            this.tabPage9.TabIndex = 6;
            this.tabPage9.Text = "委托信息";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // dgv_betRecs
            // 
            this.dgv_betRecs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_betRecs.ContextMenuStrip = this.contextMenuStrip1;
            this.dgv_betRecs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_betRecs.Location = new System.Drawing.Point(0, 0);
            this.dgv_betRecs.Name = "dgv_betRecs";
            this.dgv_betRecs.RowTemplate.Height = 23;
            this.dgv_betRecs.Size = new System.Drawing.Size(568, 216);
            this.dgv_betRecs.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "交易品种";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1303, 716);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage_summary.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_serImage)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_carImage)).EndInit();
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_ChanceImage)).EndInit();
            this.tabPage7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_chartImage)).EndInit();
            this.tabPage8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_matchGroupList)).EndInit();
            this.tabPage9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_betRecs)).EndInit();
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
        
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripMenuItem mnu_SetAssetUnitCnt;
        private System.Windows.Forms.ToolStripMenuItem mnu_RefreshInsts;
        private System.Windows.Forms.ToolStripMenuItem mnuRefreshWebToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripMenuItem TSMI_sendStatusInfor;
        private System.Windows.Forms.ToolStripMenuItem tsmi_knockTheEgg;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem switchChanleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chargeMoneyToolStripMenuItem;
        private System.Windows.Forms.WebBrowser webBrowser_charge;
        private System.Windows.Forms.ToolStripMenuItem hideTheFloatWindowToolStripMenuItem;
        private System.Windows.Forms.WebBrowser webBrowser_nav;
        private System.Windows.Forms.ToolStripMenuItem loadTheNavigateWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userLoginToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Gecko.GeckoWebBrowser geckoWebBrowser1;
        private System.Windows.Forms.ToolStripMenuItem inLotteryHomeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchWebBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchPlatformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshBetRecToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cancelBetRecToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshBetRecToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setLoginedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OCExchangeChartToolStripMenuItem;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage_summary;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ddl_datatype;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.TextBox txt_OpenTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_OpenCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Insts;
        private System.Windows.Forms.TextBox txt_ExpectNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btn_TestJscript;
        private System.Windows.Forms.Button btn_AddHedge;
        private System.Windows.Forms.Button btn_SelfAddCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_NewInsts;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.PictureBox pic_serImage;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.PictureBox pic_carImage;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.PictureBox pic_ChanceImage;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.PictureBox pic_chartImage;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.DataGridView dgv_matchGroupList;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.DataGridView dgv_betRecs;
    }
}
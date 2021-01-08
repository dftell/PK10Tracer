namespace WolfInvQuantIDevStudio
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.stragToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_FormTop = new System.Windows.Forms.ToolStrip();
            this.toolBar_NewStrage = new System.Windows.Forms.ToolStripButton();
            this.openStagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip_FormBottom = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar2 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer_Main = new System.Windows.Forms.SplitContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.splitContainer_Editor = new System.Windows.Forms.SplitContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.editor_Menu_Run = new System.Windows.Forms.ToolStripButton();
            this.editor_Menu_debug = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tabControl_Editor = new System.Windows.Forms.TabControl();
            this.tabControl_Result = new System.Windows.Forms.TabControl();
            this.tabPage_Table = new System.Windows.Forms.TabPage();
            this.tabPage_Chart = new System.Windows.Forms.TabPage();
            this.dataGridView_result = new System.Windows.Forms.DataGridView();
            this.tabControl_Comm = new System.Windows.Forms.TabControl();
            this.tabPage_files = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.menuStrip1.SuspendLayout();
            this.toolStrip_FormTop.SuspendLayout();
            this.statusStrip_FormBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).BeginInit();
            this.splitContainer_Main.Panel1.SuspendLayout();
            this.splitContainer_Main.Panel2.SuspendLayout();
            this.splitContainer_Main.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Editor)).BeginInit();
            this.splitContainer_Editor.Panel1.SuspendLayout();
            this.splitContainer_Editor.Panel2.SuspendLayout();
            this.splitContainer_Editor.SuspendLayout();
            this.tabControl_Result.SuspendLayout();
            this.tabPage_Table.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_result)).BeginInit();
            this.tabControl_Comm.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.newToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1313, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.newToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.newToolStripMenuItem.Text = "View";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openStagToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // newToolStripMenuItem1
            // 
            this.newToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stragToolStripMenuItem});
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            this.newToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.newToolStripMenuItem1.Text = "New";
            // 
            // stragToolStripMenuItem
            // 
            this.stragToolStripMenuItem.Name = "stragToolStripMenuItem";
            this.stragToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.stragToolStripMenuItem.Text = "New Strag";
            this.stragToolStripMenuItem.Click += new System.EventHandler(this.stragToolStripMenuItem_Click);
            // 
            // toolStrip_FormTop
            // 
            this.toolStrip_FormTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBar_NewStrage});
            this.toolStrip_FormTop.Location = new System.Drawing.Point(0, 25);
            this.toolStrip_FormTop.Name = "toolStrip_FormTop";
            this.toolStrip_FormTop.Size = new System.Drawing.Size(1313, 25);
            this.toolStrip_FormTop.TabIndex = 2;
            this.toolStrip_FormTop.Text = "toolStrip1";
            // 
            // toolBar_NewStrage
            // 
            this.toolBar_NewStrage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBar_NewStrage.Image = ((System.Drawing.Image)(resources.GetObject("toolBar_NewStrage.Image")));
            this.toolBar_NewStrage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBar_NewStrage.Name = "toolBar_NewStrage";
            this.toolBar_NewStrage.Size = new System.Drawing.Size(90, 22);
            this.toolBar_NewStrage.Text = "New Strategy";
            this.toolBar_NewStrage.Click += new System.EventHandler(this.toolBar_NewStrage_Click);
            // 
            // openStagToolStripMenuItem
            // 
            this.openStagToolStripMenuItem.Name = "openStagToolStripMenuItem";
            this.openStagToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openStagToolStripMenuItem.Text = "OpenStag";
            this.openStagToolStripMenuItem.Click += new System.EventHandler(this.openStagToolStripMenuItem_Click);
            // 
            // statusStrip_FormBottom
            // 
            this.statusStrip_FormBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar2,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel1});
            this.statusStrip_FormBottom.Location = new System.Drawing.Point(0, 657);
            this.statusStrip_FormBottom.Name = "statusStrip_FormBottom";
            this.statusStrip_FormBottom.Size = new System.Drawing.Size(1313, 22);
            this.statusStrip_FormBottom.TabIndex = 4;
            this.statusStrip_FormBottom.Text = "statusStrip1";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.AutoSize = false;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // toolStripProgressBar2
            // 
            this.toolStripProgressBar2.AutoSize = false;
            this.toolStripProgressBar2.Name = "toolStripProgressBar2";
            this.toolStripProgressBar2.Size = new System.Drawing.Size(300, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(500, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // splitContainer_Main
            // 
            this.splitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Main.Location = new System.Drawing.Point(0, 50);
            this.splitContainer_Main.Name = "splitContainer_Main";
            // 
            // splitContainer_Main.Panel1
            // 
            this.splitContainer_Main.Panel1.Controls.Add(this.tabControl_Comm);
            // 
            // splitContainer_Main.Panel2
            // 
            this.splitContainer_Main.Panel2.Controls.Add(this.splitContainer_Editor);
            this.splitContainer_Main.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer_Main.Size = new System.Drawing.Size(1313, 607);
            this.splitContainer_Main.SplitterDistance = 242;
            this.splitContainer_Main.TabIndex = 5;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editor_Menu_Run,
            this.toolStripSeparator1,
            this.editor_Menu_debug});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1067, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // splitContainer_Editor
            // 
            this.splitContainer_Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Editor.Location = new System.Drawing.Point(0, 25);
            this.splitContainer_Editor.Name = "splitContainer_Editor";
            this.splitContainer_Editor.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_Editor.Panel1
            // 
            this.splitContainer_Editor.Panel1.Controls.Add(this.tabControl_Editor);
            // 
            // splitContainer_Editor.Panel2
            // 
            this.splitContainer_Editor.Panel2.Controls.Add(this.tabControl_Result);
            this.splitContainer_Editor.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer_Editor.Size = new System.Drawing.Size(1067, 582);
            this.splitContainer_Editor.SplitterDistance = 296;
            this.splitContainer_Editor.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 260);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1067, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // editor_Menu_Run
            // 
            this.editor_Menu_Run.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editor_Menu_Run.Image = ((System.Drawing.Image)(resources.GetObject("editor_Menu_Run.Image")));
            this.editor_Menu_Run.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editor_Menu_Run.Name = "editor_Menu_Run";
            this.editor_Menu_Run.Size = new System.Drawing.Size(34, 22);
            this.editor_Menu_Run.Text = "Run";
            // 
            // editor_Menu_debug
            // 
            this.editor_Menu_debug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editor_Menu_debug.Image = ((System.Drawing.Image)(resources.GetObject("editor_Menu_debug.Image")));
            this.editor_Menu_debug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editor_Menu_debug.Name = "editor_Menu_debug";
            this.editor_Menu_debug.Size = new System.Drawing.Size(51, 22);
            this.editor_Menu_debug.Text = "Debug";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tabControl_Editor
            // 
            this.tabControl_Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Editor.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl_Editor.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Editor.Name = "tabControl_Editor";
            this.tabControl_Editor.SelectedIndex = 0;
            this.tabControl_Editor.Size = new System.Drawing.Size(1067, 296);
            this.tabControl_Editor.TabIndex = 0;
            this.tabControl_Editor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl_Editor_DrawItem);
            this.tabControl_Editor.SelectedIndexChanged += new System.EventHandler(this.tabControl_Editor_SelectedIndexChanged);
            this.tabControl_Editor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl_Editor_MouseDown);
            // 
            // tabControl_Result
            // 
            this.tabControl_Result.Controls.Add(this.tabPage_Table);
            this.tabControl_Result.Controls.Add(this.tabPage_Chart);
            this.tabControl_Result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Result.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Result.Name = "tabControl_Result";
            this.tabControl_Result.SelectedIndex = 0;
            this.tabControl_Result.Size = new System.Drawing.Size(1067, 260);
            this.tabControl_Result.TabIndex = 1;
            // 
            // tabPage_Table
            // 
            this.tabPage_Table.Controls.Add(this.dataGridView_result);
            this.tabPage_Table.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Table.Name = "tabPage_Table";
            this.tabPage_Table.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Table.Size = new System.Drawing.Size(1059, 234);
            this.tabPage_Table.TabIndex = 0;
            this.tabPage_Table.Text = "数据";
            this.tabPage_Table.UseVisualStyleBackColor = true;
            // 
            // tabPage_Chart
            // 
            this.tabPage_Chart.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Chart.Name = "tabPage_Chart";
            this.tabPage_Chart.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Chart.Size = new System.Drawing.Size(1059, 234);
            this.tabPage_Chart.TabIndex = 1;
            this.tabPage_Chart.Text = "图表";
            this.tabPage_Chart.UseVisualStyleBackColor = true;
            // 
            // dataGridView_result
            // 
            this.dataGridView_result.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_result.Location = new System.Drawing.Point(3, 3);
            this.dataGridView_result.Name = "dataGridView_result";
            this.dataGridView_result.RowTemplate.Height = 23;
            this.dataGridView_result.Size = new System.Drawing.Size(1053, 228);
            this.dataGridView_result.TabIndex = 0;
            // 
            // tabControl_Comm
            // 
            this.tabControl_Comm.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl_Comm.Controls.Add(this.tabPage_files);
            this.tabControl_Comm.Controls.Add(this.tabPage2);
            this.tabControl_Comm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Comm.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Comm.Multiline = true;
            this.tabControl_Comm.Name = "tabControl_Comm";
            this.tabControl_Comm.SelectedIndex = 0;
            this.tabControl_Comm.Size = new System.Drawing.Size(242, 607);
            this.tabControl_Comm.TabIndex = 0;
            // 
            // tabPage_files
            // 
            this.tabPage_files.Location = new System.Drawing.Point(22, 4);
            this.tabPage_files.Name = "tabPage_files";
            this.tabPage_files.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_files.Size = new System.Drawing.Size(216, 599);
            this.tabPage_files.TabIndex = 0;
            this.tabPage_files.Text = "策略";
            this.tabPage_files.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(22, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(216, 599);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 679);
            this.Controls.Add(this.splitContainer_Main);
            this.Controls.Add(this.statusStrip_FormBottom);
            this.Controls.Add(this.toolStrip_FormTop);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "武府量化投资研究开发平台";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip_FormTop.ResumeLayout(false);
            this.toolStrip_FormTop.PerformLayout();
            this.statusStrip_FormBottom.ResumeLayout(false);
            this.statusStrip_FormBottom.PerformLayout();
            this.splitContainer_Main.Panel1.ResumeLayout(false);
            this.splitContainer_Main.Panel2.ResumeLayout(false);
            this.splitContainer_Main.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).EndInit();
            this.splitContainer_Main.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer_Editor.Panel1.ResumeLayout(false);
            this.splitContainer_Editor.Panel2.ResumeLayout(false);
            this.splitContainer_Editor.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Editor)).EndInit();
            this.splitContainer_Editor.ResumeLayout(false);
            this.tabControl_Result.ResumeLayout(false);
            this.tabPage_Table.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_result)).EndInit();
            this.tabControl_Comm.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem stragToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip_FormTop;
        private System.Windows.Forms.ToolStripButton toolBar_NewStrage;
        private System.Windows.Forms.ToolStripMenuItem openStagToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip_FormBottom;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SplitContainer splitContainer_Main;
        private System.Windows.Forms.SplitContainer splitContainer_Editor;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton editor_Menu_Run;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton editor_Menu_debug;
        private System.Windows.Forms.TabControl tabControl_Editor;
        private System.Windows.Forms.TabControl tabControl_Result;
        private System.Windows.Forms.TabPage tabPage_Table;
        private System.Windows.Forms.DataGridView dataGridView_result;
        private System.Windows.Forms.TabPage tabPage_Chart;
        private System.Windows.Forms.TabControl tabControl_Comm;
        private System.Windows.Forms.TabPage tabPage_files;
        private System.Windows.Forms.TabPage tabPage2;
    }
}


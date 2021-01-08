namespace PK10Server
{
    partial class frm_MoniteStrag<T>
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
            this.btn_startMonite = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_DtpName = new System.Windows.Forms.TextBox();
            this.Txt_Chances = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.txt_intersec = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_reviewcnt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_lastExpect = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkb_useTargeExpect = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_SecPools = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_startMonite
            // 
            this.btn_startMonite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_startMonite.Location = new System.Drawing.Point(722, 11);
            this.btn_startMonite.Margin = new System.Windows.Forms.Padding(2);
            this.btn_startMonite.Name = "btn_startMonite";
            this.btn_startMonite.Size = new System.Drawing.Size(58, 22);
            this.btn_startMonite.TabIndex = 0;
            this.btn_startMonite.Text = "开始";
            this.btn_startMonite.UseVisualStyleBackColor = true;
            this.btn_startMonite.Click += new System.EventHandler(this.btn_startMonite_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "策略";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 16);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "市场";
            // 
            // txt_DtpName
            // 
            this.txt_DtpName.Location = new System.Drawing.Point(37, 12);
            this.txt_DtpName.Margin = new System.Windows.Forms.Padding(2);
            this.txt_DtpName.Name = "txt_DtpName";
            this.txt_DtpName.Size = new System.Drawing.Size(52, 21);
            this.txt_DtpName.TabIndex = 5;
            // 
            // Txt_Chances
            // 
            this.Txt_Chances.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Txt_Chances.Location = new System.Drawing.Point(4, 42);
            this.Txt_Chances.Margin = new System.Windows.Forms.Padding(2);
            this.Txt_Chances.Multiline = true;
            this.Txt_Chances.Name = "Txt_Chances";
            this.Txt_Chances.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Txt_Chances.Size = new System.Drawing.Size(778, 454);
            this.Txt_Chances.TabIndex = 6;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 484);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 7, 0);
            this.statusStrip1.Size = new System.Drawing.Size(786, 38);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(500, 33);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(257, 33);
            // 
            // txt_intersec
            // 
            this.txt_intersec.Location = new System.Drawing.Point(391, 12);
            this.txt_intersec.Margin = new System.Windows.Forms.Padding(2);
            this.txt_intersec.Name = "txt_intersec";
            this.txt_intersec.Size = new System.Drawing.Size(32, 21);
            this.txt_intersec.TabIndex = 9;
            this.txt_intersec.Text = "60";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(358, 16);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "秒数";
            // 
            // txt_reviewcnt
            // 
            this.txt_reviewcnt.Location = new System.Drawing.Point(460, 12);
            this.txt_reviewcnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_reviewcnt.Name = "txt_reviewcnt";
            this.txt_reviewcnt.Size = new System.Drawing.Size(42, 21);
            this.txt_reviewcnt.TabIndex = 11;
            this.txt_reviewcnt.Text = "50";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(428, 16);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "次数";
            // 
            // txt_lastExpect
            // 
            this.txt_lastExpect.Location = new System.Drawing.Point(535, 12);
            this.txt_lastExpect.Margin = new System.Windows.Forms.Padding(2);
            this.txt_lastExpect.Name = "txt_lastExpect";
            this.txt_lastExpect.Size = new System.Drawing.Size(90, 21);
            this.txt_lastExpect.TabIndex = 13;
            this.txt_lastExpect.Text = "50";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(503, 16);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "期号";
            // 
            // chkb_useTargeExpect
            // 
            this.chkb_useTargeExpect.AutoSize = true;
            this.chkb_useTargeExpect.Location = new System.Drawing.Point(637, 14);
            this.chkb_useTargeExpect.Name = "chkb_useTargeExpect";
            this.chkb_useTargeExpect.Size = new System.Drawing.Size(72, 16);
            this.chkb_useTargeExpect.TabIndex = 15;
            this.chkb_useTargeExpect.Text = "特定期号";
            this.chkb_useTargeExpect.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(230, 16);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "证券池";
            // 
            // txt_SecPools
            // 
            this.txt_SecPools.Location = new System.Drawing.Point(275, 12);
            this.txt_SecPools.Margin = new System.Windows.Forms.Padding(2);
            this.txt_SecPools.Name = "txt_SecPools";
            this.txt_SecPools.Size = new System.Drawing.Size(79, 21);
            this.txt_SecPools.TabIndex = 17;
            // 
            // frm_MoniteStrag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 522);
            this.Controls.Add(this.txt_SecPools);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.chkb_useTargeExpect);
            this.Controls.Add(this.txt_lastExpect);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_reviewcnt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txt_intersec);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Txt_Chances);
            this.Controls.Add(this.txt_DtpName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_startMonite);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frm_MoniteStrag";
            this.Text = "策略监控";
            this.Load += new System.EventHandler(this.frm_MoniteStrag_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_startMonite;
        private WolfInv.com.ExchangeLib.RunPlanPicker<T> runPlanPicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_DtpName;
        private System.Windows.Forms.TextBox Txt_Chances;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.TextBox txt_intersec;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_reviewcnt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_lastExpect;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkb_useTargeExpect;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_SecPools;
    }
}
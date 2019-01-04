namespace BackTestSystem
{
    partial class frm_DistrCheck
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ddl_CheckFunc = new System.Windows.Forms.ComboBox();
            this.txt_BegExpect = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_LoopCnt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_CheckCnt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_TirgetCnt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_ExpectProb = new System.Windows.Forms.TextBox();
            this.btn_Check = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btn_Check);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(737, 463);
            this.splitContainer1.SplitterDistance = 139;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txt_ExpectProb);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txt_TirgetCnt);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txt_CheckCnt);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txt_LoopCnt);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_BegExpect);
            this.groupBox1.Controls.Add(this.ddl_CheckFunc);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(398, 123);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "检测内容";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "检测方法";
            // 
            // ddl_CheckFunc
            // 
            this.ddl_CheckFunc.FormattingEnabled = true;
            this.ddl_CheckFunc.Location = new System.Drawing.Point(91, 15);
            this.ddl_CheckFunc.Name = "ddl_CheckFunc";
            this.ddl_CheckFunc.Size = new System.Drawing.Size(126, 20);
            this.ddl_CheckFunc.TabIndex = 1;
            // 
            // txt_BegExpect
            // 
            this.txt_BegExpect.Location = new System.Drawing.Point(92, 48);
            this.txt_BegExpect.Name = "txt_BegExpect";
            this.txt_BegExpect.Size = new System.Drawing.Size(124, 21);
            this.txt_BegExpect.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "开始期号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "循环读取期数";
            // 
            // txt_LoopCnt
            // 
            this.txt_LoopCnt.Location = new System.Drawing.Point(91, 84);
            this.txt_LoopCnt.Name = "txt_LoopCnt";
            this.txt_LoopCnt.Size = new System.Drawing.Size(124, 21);
            this.txt_LoopCnt.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(252, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "检测长度";
            // 
            // txt_CheckCnt
            // 
            this.txt_CheckCnt.Location = new System.Drawing.Point(316, 18);
            this.txt_CheckCnt.Name = "txt_CheckCnt";
            this.txt_CheckCnt.Size = new System.Drawing.Size(61, 21);
            this.txt_CheckCnt.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(252, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "目标长度";
            // 
            // txt_TirgetCnt
            // 
            this.txt_TirgetCnt.Location = new System.Drawing.Point(316, 48);
            this.txt_TirgetCnt.Name = "txt_TirgetCnt";
            this.txt_TirgetCnt.Size = new System.Drawing.Size(61, 21);
            this.txt_TirgetCnt.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(252, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "期待概率";
            // 
            // txt_ExpectProb
            // 
            this.txt_ExpectProb.Location = new System.Drawing.Point(316, 84);
            this.txt_ExpectProb.Name = "txt_ExpectProb";
            this.txt_ExpectProb.Size = new System.Drawing.Size(61, 21);
            this.txt_ExpectProb.TabIndex = 10;
            // 
            // btn_Check
            // 
            this.btn_Check.Location = new System.Drawing.Point(654, 20);
            this.btn_Check.Name = "btn_Check";
            this.btn_Check.Size = new System.Drawing.Size(62, 23);
            this.btn_Check.TabIndex = 1;
            this.btn_Check.Text = "检测";
            this.btn_Check.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(58, 58);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(596, 166);
            this.dataGridView1.TabIndex = 0;
            // 
            // frm_DistrCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 463);
            this.Controls.Add(this.splitContainer1);
            this.Name = "frm_DistrCheck";
            this.Text = "分布检测";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_ExpectProb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_TirgetCnt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_CheckCnt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_LoopCnt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_BegExpect;
        private System.Windows.Forms.ComboBox ddl_CheckFunc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Check;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
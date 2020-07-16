namespace BackTestSys
{
    partial class frm_TrainForm<T>
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ddl_dataSource = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.chkb_AllUseShift = new System.Windows.Forms.CheckBox();
            this.txt_DataLength = new System.Windows.Forms.TextBox();
            this.txt_BegExpect = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_CheckResult = new System.Windows.Forms.Button();
            this.btn_stopTrain = new System.Windows.Forms.Button();
            this.txt_endT = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_begT = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_Train = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_TopN = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txt_threadCnt = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_featureCnt = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ddl_categryFunc = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_IteratCnt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_LearnDeep = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ddl_MLFunc = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(959, 605);
            this.splitContainer1.SplitterDistance = 217;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.ddl_dataSource);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.chkb_AllUseShift);
            this.groupBox2.Controls.Add(this.txt_DataLength);
            this.groupBox2.Controls.Add(this.txt_BegExpect);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(261, 10);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(444, 197);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据集";
            // 
            // ddl_dataSource
            // 
            this.ddl_dataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddl_dataSource.FormattingEnabled = true;
            this.ddl_dataSource.Location = new System.Drawing.Point(66, 18);
            this.ddl_dataSource.Margin = new System.Windows.Forms.Padding(2);
            this.ddl_dataSource.Name = "ddl_dataSource";
            this.ddl_dataSource.Size = new System.Drawing.Size(121, 20);
            this.ddl_dataSource.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 22);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "数据源";
            // 
            // chkb_AllUseShift
            // 
            this.chkb_AllUseShift.AutoSize = true;
            this.chkb_AllUseShift.Location = new System.Drawing.Point(66, 97);
            this.chkb_AllUseShift.Margin = new System.Windows.Forms.Padding(2);
            this.chkb_AllUseShift.Name = "chkb_AllUseShift";
            this.chkb_AllUseShift.Size = new System.Drawing.Size(72, 16);
            this.chkb_AllUseShift.TabIndex = 4;
            this.chkb_AllUseShift.Text = "使用偏移";
            this.chkb_AllUseShift.UseVisualStyleBackColor = true;
            // 
            // txt_DataLength
            // 
            this.txt_DataLength.Location = new System.Drawing.Point(66, 71);
            this.txt_DataLength.Margin = new System.Windows.Forms.Padding(2);
            this.txt_DataLength.Name = "txt_DataLength";
            this.txt_DataLength.Size = new System.Drawing.Size(121, 21);
            this.txt_DataLength.TabIndex = 3;
            this.txt_DataLength.Text = "10000";
            // 
            // txt_BegExpect
            // 
            this.txt_BegExpect.Location = new System.Drawing.Point(66, 45);
            this.txt_BegExpect.Margin = new System.Windows.Forms.Padding(2);
            this.txt_BegExpect.Name = "txt_BegExpect";
            this.txt_BegExpect.Size = new System.Drawing.Size(121, 21);
            this.txt_BegExpect.TabIndex = 2;
            this.txt_BegExpect.Text = "233049";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 74);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "期数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 49);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "起始期号";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btn_CheckResult);
            this.panel1.Controls.Add(this.btn_stopTrain);
            this.panel1.Controls.Add(this.txt_endT);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txt_begT);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.btn_Train);
            this.panel1.Location = new System.Drawing.Point(708, 17);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(237, 190);
            this.panel1.TabIndex = 1;
            // 
            // btn_CheckResult
            // 
            this.btn_CheckResult.Location = new System.Drawing.Point(148, 133);
            this.btn_CheckResult.Margin = new System.Windows.Forms.Padding(2);
            this.btn_CheckResult.Name = "btn_CheckResult";
            this.btn_CheckResult.Size = new System.Drawing.Size(64, 22);
            this.btn_CheckResult.TabIndex = 8;
            this.btn_CheckResult.Text = "验证";
            this.btn_CheckResult.UseVisualStyleBackColor = true;
            this.btn_CheckResult.Click += new System.EventHandler(this.btn_CheckResult_Click);
            // 
            // btn_stopTrain
            // 
            this.btn_stopTrain.Location = new System.Drawing.Point(78, 133);
            this.btn_stopTrain.Margin = new System.Windows.Forms.Padding(2);
            this.btn_stopTrain.Name = "btn_stopTrain";
            this.btn_stopTrain.Size = new System.Drawing.Size(64, 22);
            this.btn_stopTrain.TabIndex = 7;
            this.btn_stopTrain.Text = "停止";
            this.btn_stopTrain.UseVisualStyleBackColor = true;
            this.btn_stopTrain.Click += new System.EventHandler(this.btn_stopTrain_Click);
            // 
            // txt_endT
            // 
            this.txt_endT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_endT.Location = new System.Drawing.Point(72, 36);
            this.txt_endT.Margin = new System.Windows.Forms.Padding(2);
            this.txt_endT.Name = "txt_endT";
            this.txt_endT.Size = new System.Drawing.Size(154, 21);
            this.txt_endT.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 39);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "结束时间";
            // 
            // txt_begT
            // 
            this.txt_begT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_begT.Location = new System.Drawing.Point(72, 15);
            this.txt_begT.Margin = new System.Windows.Forms.Padding(2);
            this.txt_begT.Name = "txt_begT";
            this.txt_begT.Size = new System.Drawing.Size(152, 21);
            this.txt_begT.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 18);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "开始时间";
            // 
            // btn_Train
            // 
            this.btn_Train.Location = new System.Drawing.Point(7, 133);
            this.btn_Train.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Train.Name = "btn_Train";
            this.btn_Train.Size = new System.Drawing.Size(64, 22);
            this.btn_Train.TabIndex = 0;
            this.btn_Train.Text = "训练";
            this.btn_Train.UseVisualStyleBackColor = true;
            this.btn_Train.Click += new System.EventHandler(this.btn_Train_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.txt_TopN);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txt_threadCnt);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txt_featureCnt);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.ddl_categryFunc);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txt_IteratCnt);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txt_LearnDeep);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.ddl_MLFunc);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(245, 197);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "训练方法";
            // 
            // txt_TopN
            // 
            this.txt_TopN.Location = new System.Drawing.Point(92, 169);
            this.txt_TopN.Margin = new System.Windows.Forms.Padding(2);
            this.txt_TopN.Name = "txt_TopN";
            this.txt_TopN.Size = new System.Drawing.Size(139, 21);
            this.txt_TopN.TabIndex = 12;
            this.txt_TopN.Text = "1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 172);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 11;
            this.label12.Text = "选取个数";
            // 
            // txt_threadCnt
            // 
            this.txt_threadCnt.Location = new System.Drawing.Point(92, 71);
            this.txt_threadCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_threadCnt.Name = "txt_threadCnt";
            this.txt_threadCnt.Size = new System.Drawing.Size(139, 21);
            this.txt_threadCnt.TabIndex = 10;
            this.txt_threadCnt.Text = "10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 75);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 9;
            this.label11.Text = "并发数量";
            // 
            // txt_featureCnt
            // 
            this.txt_featureCnt.Location = new System.Drawing.Point(92, 94);
            this.txt_featureCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_featureCnt.Name = "txt_featureCnt";
            this.txt_featureCnt.Size = new System.Drawing.Size(139, 21);
            this.txt_featureCnt.TabIndex = 10;
            this.txt_featureCnt.Text = "10";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 98);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 9;
            this.label9.Text = "状态数量";
            // 
            // ddl_categryFunc
            // 
            this.ddl_categryFunc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddl_categryFunc.FormattingEnabled = true;
            this.ddl_categryFunc.Location = new System.Drawing.Point(92, 43);
            this.ddl_categryFunc.Margin = new System.Windows.Forms.Padding(2);
            this.ddl_categryFunc.Name = "ddl_categryFunc";
            this.ddl_categryFunc.Size = new System.Drawing.Size(139, 20);
            this.ddl_categryFunc.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 47);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "数据分类方法";
            // 
            // txt_IteratCnt
            // 
            this.txt_IteratCnt.Location = new System.Drawing.Point(92, 144);
            this.txt_IteratCnt.Margin = new System.Windows.Forms.Padding(2);
            this.txt_IteratCnt.Name = "txt_IteratCnt";
            this.txt_IteratCnt.Size = new System.Drawing.Size(139, 21);
            this.txt_IteratCnt.TabIndex = 6;
            this.txt_IteratCnt.Text = "1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 147);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "迭代次数";
            // 
            // txt_LearnDeep
            // 
            this.txt_LearnDeep.Location = new System.Drawing.Point(92, 119);
            this.txt_LearnDeep.Margin = new System.Windows.Forms.Padding(2);
            this.txt_LearnDeep.Name = "txt_LearnDeep";
            this.txt_LearnDeep.Size = new System.Drawing.Size(139, 21);
            this.txt_LearnDeep.TabIndex = 4;
            this.txt_LearnDeep.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 123);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "学习深度";
            // 
            // ddl_MLFunc
            // 
            this.ddl_MLFunc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddl_MLFunc.FormattingEnabled = true;
            this.ddl_MLFunc.Location = new System.Drawing.Point(92, 19);
            this.ddl_MLFunc.Margin = new System.Windows.Forms.Padding(2);
            this.ddl_MLFunc.Name = "ddl_MLFunc";
            this.ddl_MLFunc.Size = new System.Drawing.Size(139, 20);
            this.ddl_MLFunc.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "训练方法";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 37;
            this.dataGridView1.Size = new System.Drawing.Size(959, 386);
            this.dataGridView1.TabIndex = 0;
            // 
            // frm_TrainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 605);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frm_TrainForm";
            this.Text = "机器学习训练平台";
            this.Load += new System.EventHandler(this.frm_TrainForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Train;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddl_MLFunc;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_DataLength;
        private System.Windows.Forms.TextBox txt_BegExpect;
        private System.Windows.Forms.TextBox txt_LearnDeep;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_endT;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_begT;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckBox chkb_AllUseShift;
        private System.Windows.Forms.Button btn_stopTrain;
        private System.Windows.Forms.TextBox txt_IteratCnt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_CheckResult;
        private System.Windows.Forms.ComboBox ddl_categryFunc;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_featureCnt;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox ddl_dataSource;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_threadCnt;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_TopN;
        private System.Windows.Forms.Label label12;
    }
}
namespace Test_Win
{
    partial class Form2
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
            this.label1 = new System.Windows.Forms.Label();
            this.btn_gl = new System.Windows.Forms.Button();
            this.btn_forSI = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btn_swhyCommQuery = new System.Windows.Forms.Button();
            this.btn_SubScript = new System.Windows.Forms.Button();
            this.btn_TestDayData = new System.Windows.Forms.Button();
            this.lbl_process = new System.Windows.Forms.Label();
            this.txt_maxThreadCnt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_GrpUnitCnt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_ThrdInterval = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_begT = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_endT = new System.Windows.Forms.TextBox();
            this.btn_svrmgr = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(344, 110);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(251, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "万得数据工具";
            // 
            // btn_gl
            // 
            this.btn_gl.Location = new System.Drawing.Point(90, 218);
            this.btn_gl.Margin = new System.Windows.Forms.Padding(6);
            this.btn_gl.Name = "btn_gl";
            this.btn_gl.Size = new System.Drawing.Size(206, 60);
            this.btn_gl.TabIndex = 1;
            this.btn_gl.Text = "万得概念指数";
            this.btn_gl.UseVisualStyleBackColor = true;
            this.btn_gl.Click += new System.EventHandler(this.btn_gl_Click);
            // 
            // btn_forSI
            // 
            this.btn_forSI.Location = new System.Drawing.Point(296, 218);
            this.btn_forSI.Margin = new System.Windows.Forms.Padding(6);
            this.btn_forSI.Name = "btn_forSI";
            this.btn_forSI.Size = new System.Drawing.Size(186, 60);
            this.btn_forSI.TabIndex = 1;
            this.btn_forSI.Text = "申万二级行业";
            this.btn_forSI.UseVisualStyleBackColor = true;
            this.btn_forSI.Click += new System.EventHandler(this.btn_forSI_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(492, 220);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 58);
            this.button1.TabIndex = 2;
            this.button1.Text = "测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 667);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1027, 36);
            this.progressBar1.TabIndex = 3;
            // 
            // btn_swhyCommQuery
            // 
            this.btn_swhyCommQuery.Location = new System.Drawing.Point(640, 220);
            this.btn_swhyCommQuery.Margin = new System.Windows.Forms.Padding(6);
            this.btn_swhyCommQuery.Name = "btn_swhyCommQuery";
            this.btn_swhyCommQuery.Size = new System.Drawing.Size(232, 56);
            this.btn_swhyCommQuery.TabIndex = 4;
            this.btn_swhyCommQuery.Text = "申万行业通用查询";
            this.btn_swhyCommQuery.UseVisualStyleBackColor = true;
            this.btn_swhyCommQuery.Click += new System.EventHandler(this.btn_swhyCommQuery_Click);
            // 
            // btn_SubScript
            // 
            this.btn_SubScript.Location = new System.Drawing.Point(641, 304);
            this.btn_SubScript.Name = "btn_SubScript";
            this.btn_SubScript.Size = new System.Drawing.Size(230, 44);
            this.btn_SubScript.TabIndex = 5;
            this.btn_SubScript.Text = "订阅A股行情";
            this.btn_SubScript.UseVisualStyleBackColor = true;
            this.btn_SubScript.Click += new System.EventHandler(this.btn_SubScript_Click);
            // 
            // btn_TestDayData
            // 
            this.btn_TestDayData.Location = new System.Drawing.Point(642, 354);
            this.btn_TestDayData.Name = "btn_TestDayData";
            this.btn_TestDayData.Size = new System.Drawing.Size(230, 44);
            this.btn_TestDayData.TabIndex = 6;
            this.btn_TestDayData.Text = "测试A股行情";
            this.btn_TestDayData.UseVisualStyleBackColor = true;
            this.btn_TestDayData.Click += new System.EventHandler(this.btn_TestDayData_Click);
            // 
            // lbl_process
            // 
            this.lbl_process.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_process.AutoSize = true;
            this.lbl_process.Location = new System.Drawing.Point(12, 729);
            this.lbl_process.Name = "lbl_process";
            this.lbl_process.Size = new System.Drawing.Size(82, 24);
            this.lbl_process.TabIndex = 7;
            this.lbl_process.Text = "label2";
            // 
            // txt_maxThreadCnt
            // 
            this.txt_maxThreadCnt.Location = new System.Drawing.Point(269, 459);
            this.txt_maxThreadCnt.Name = "txt_maxThreadCnt";
            this.txt_maxThreadCnt.Size = new System.Drawing.Size(138, 35);
            this.txt_maxThreadCnt.TabIndex = 8;
            this.txt_maxThreadCnt.Text = "40";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(114, 463);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 24);
            this.label2.TabIndex = 9;
            this.label2.Text = "最大线程数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(114, 517);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 24);
            this.label3.TabIndex = 11;
            this.label3.Text = "分组单元数量";
            // 
            // txt_GrpUnitCnt
            // 
            this.txt_GrpUnitCnt.Location = new System.Drawing.Point(269, 513);
            this.txt_GrpUnitCnt.Name = "txt_GrpUnitCnt";
            this.txt_GrpUnitCnt.Size = new System.Drawing.Size(138, 35);
            this.txt_GrpUnitCnt.TabIndex = 10;
            this.txt_GrpUnitCnt.Text = "25";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(114, 574);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 24);
            this.label4.TabIndex = 13;
            this.label4.Text = "线程间隙";
            // 
            // txt_ThrdInterval
            // 
            this.txt_ThrdInterval.Location = new System.Drawing.Point(269, 570);
            this.txt_ThrdInterval.Name = "txt_ThrdInterval";
            this.txt_ThrdInterval.Size = new System.Drawing.Size(138, 35);
            this.txt_ThrdInterval.TabIndex = 12;
            this.txt_ThrdInterval.Text = "1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(114, 354);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 24);
            this.label5.TabIndex = 15;
            this.label5.Text = "开始日期";
            // 
            // txt_begT
            // 
            this.txt_begT.Location = new System.Drawing.Point(269, 350);
            this.txt_begT.Name = "txt_begT";
            this.txt_begT.Size = new System.Drawing.Size(138, 35);
            this.txt_begT.TabIndex = 14;
            this.txt_begT.Text = "2017-1-1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(114, 412);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 24);
            this.label6.TabIndex = 17;
            this.label6.Text = "结束日期";
            // 
            // txt_endT
            // 
            this.txt_endT.Location = new System.Drawing.Point(269, 408);
            this.txt_endT.Name = "txt_endT";
            this.txt_endT.Size = new System.Drawing.Size(138, 35);
            this.txt_endT.TabIndex = 16;
            this.txt_endT.Text = "2019-4-7";
            // 
            // btn_svrmgr
            // 
            this.btn_svrmgr.Location = new System.Drawing.Point(643, 420);
            this.btn_svrmgr.Name = "btn_svrmgr";
            this.btn_svrmgr.Size = new System.Drawing.Size(227, 39);
            this.btn_svrmgr.TabIndex = 18;
            this.btn_svrmgr.Text = " 服务器管理";
            this.btn_svrmgr.UseVisualStyleBackColor = true;
            this.btn_svrmgr.Click += new System.EventHandler(this.btn_svrmgr_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 762);
            this.Controls.Add(this.btn_svrmgr);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txt_endT);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_begT);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txt_ThrdInterval);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_GrpUnitCnt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_maxThreadCnt);
            this.Controls.Add(this.lbl_process);
            this.Controls.Add(this.btn_TestDayData);
            this.Controls.Add(this.btn_SubScript);
            this.Controls.Add(this.btn_swhyCommQuery);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_forSI);
            this.Controls.Add(this.btn_gl);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_gl;
        private System.Windows.Forms.Button btn_forSI;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btn_swhyCommQuery;
        private System.Windows.Forms.Button btn_SubScript;
        private System.Windows.Forms.Button btn_TestDayData;
        private System.Windows.Forms.Label lbl_process;
        private System.Windows.Forms.TextBox txt_maxThreadCnt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_GrpUnitCnt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_ThrdInterval;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_begT;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_endT;
        private System.Windows.Forms.Button btn_svrmgr;
    }
}
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
            this.progressBar1.Location = new System.Drawing.Point(12, 708);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(974, 32);
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
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 762);
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
    }
}
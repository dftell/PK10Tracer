namespace JdEBuy
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
            this.btn_recieveData = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txt_ask = new System.Windows.Forms.TextBox();
            this.txt_answer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_recieveData
            // 
            this.btn_recieveData.Location = new System.Drawing.Point(699, 33);
            this.btn_recieveData.Name = "btn_recieveData";
            this.btn_recieveData.Size = new System.Drawing.Size(81, 27);
            this.btn_recieveData.TabIndex = 0;
            this.btn_recieveData.Text = "接收新数据";
            this.btn_recieveData.UseVisualStyleBackColor = true;
            this.btn_recieveData.Click += new System.EventHandler(this.btn_recieveData_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(699, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txt_ask
            // 
            this.txt_ask.Location = new System.Drawing.Point(86, 12);
            this.txt_ask.Multiline = true;
            this.txt_ask.Name = "txt_ask";
            this.txt_ask.Size = new System.Drawing.Size(437, 87);
            this.txt_ask.TabIndex = 2;
            // 
            // txt_answer
            // 
            this.txt_answer.Location = new System.Drawing.Point(86, 125);
            this.txt_answer.Multiline = true;
            this.txt_answer.Name = "txt_answer";
            this.txt_answer.Size = new System.Drawing.Size(437, 239);
            this.txt_answer.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 504);
            this.Controls.Add(this.txt_answer);
            this.Controls.Add(this.txt_ask);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_recieveData);
            this.Name = "Form1";
            this.Text = "京东联盟";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_recieveData;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txt_ask;
        private System.Windows.Forms.TextBox txt_answer;
    }
}


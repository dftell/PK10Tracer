namespace WindowsFormsApplication1
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_Insts = new System.Windows.Forms.TextBox();
            this.btn_send = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_ExpectNo = new System.Windows.Forms.TextBox();
            this.lbl_UserInfo = new System.Windows.Forms.Label();
            this.timer_sender = new System.Windows.Forms.Timer(this.components);
            this.txt_login = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "个人信息";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "期号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "指令";
            // 
            // txt_Insts
            // 
            this.txt_Insts.Location = new System.Drawing.Point(56, 168);
            this.txt_Insts.Multiline = true;
            this.txt_Insts.Name = "txt_Insts";
            this.txt_Insts.Size = new System.Drawing.Size(1136, 273);
            this.txt_Insts.TabIndex = 4;
            // 
            // btn_send
            // 
            this.btn_send.Location = new System.Drawing.Point(1042, 459);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(149, 46);
            this.btn_send.TabIndex = 5;
            this.btn_send.Text = "发送";
            this.btn_send.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(135, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 24);
            this.label4.TabIndex = 6;
            // 
            // txt_ExpectNo
            // 
            this.txt_ExpectNo.Location = new System.Drawing.Point(177, 70);
            this.txt_ExpectNo.Name = "txt_ExpectNo";
            this.txt_ExpectNo.Size = new System.Drawing.Size(316, 35);
            this.txt_ExpectNo.TabIndex = 7;
            // 
            // lbl_UserInfo
            // 
            this.lbl_UserInfo.AutoSize = true;
            this.lbl_UserInfo.Location = new System.Drawing.Point(173, 31);
            this.lbl_UserInfo.Name = "lbl_UserInfo";
            this.lbl_UserInfo.Size = new System.Drawing.Size(118, 24);
            this.lbl_UserInfo.TabIndex = 8;
            this.lbl_UserInfo.Text = "XXXXXXXXX";
            // 
            // timer_sender
            // 
            this.timer_sender.Interval = 20000;
            // 
            // txt_login
            // 
            this.txt_login.Location = new System.Drawing.Point(63, 511);
            this.txt_login.Multiline = true;
            this.txt_login.Name = "txt_login";
            this.txt_login.Size = new System.Drawing.Size(1129, 344);
            this.txt_login.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1216, 853);
            this.Controls.Add(this.txt_login);
            this.Controls.Add(this.lbl_UserInfo);
            this.Controls.Add(this.txt_ExpectNo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_send);
            this.Controls.Add(this.txt_Insts);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_Insts;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_ExpectNo;
        private System.Windows.Forms.Label lbl_UserInfo;
        private System.Windows.Forms.Timer timer_sender;
        private System.Windows.Forms.TextBox txt_login;
    }
}


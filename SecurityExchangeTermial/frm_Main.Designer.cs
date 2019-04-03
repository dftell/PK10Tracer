namespace SecurityExchangeTermial
{
    partial class frm_Main
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_host = new System.Windows.Forms.TextBox();
            this.txt_hostPort = new System.Windows.Forms.TextBox();
            this.txt_SecDept = new System.Windows.Forms.TextBox();
            this.txt_ExchAccount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_ExchPwd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_AmtAccount = new System.Windows.Forms.TextBox();
            this.btn_login = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_VersionNo = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txt_VersionNo);
            this.groupBox1.Controls.Add(this.btn_login);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txt_AmtAccount);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txt_ExchPwd);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txt_ExchAccount);
            this.groupBox1.Controls.Add(this.txt_SecDept);
            this.groupBox1.Controls.Add(this.txt_hostPort);
            this.groupBox1.Controls.Add(this.txt_host);
            this.groupBox1.Location = new System.Drawing.Point(40, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1125, 482);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // txt_host
            // 
            this.txt_host.Location = new System.Drawing.Point(168, 52);
            this.txt_host.Name = "txt_host";
            this.txt_host.Size = new System.Drawing.Size(208, 35);
            this.txt_host.TabIndex = 0;
            this.txt_host.Text = "222.240.176.149";
            // 
            // txt_hostPort
            // 
            this.txt_hostPort.Location = new System.Drawing.Point(168, 97);
            this.txt_hostPort.Name = "txt_hostPort";
            this.txt_hostPort.Size = new System.Drawing.Size(208, 35);
            this.txt_hostPort.TabIndex = 1;
            this.txt_hostPort.Text = "8002";
            // 
            // txt_SecDept
            // 
            this.txt_SecDept.Location = new System.Drawing.Point(168, 193);
            this.txt_SecDept.Name = "txt_SecDept";
            this.txt_SecDept.Size = new System.Drawing.Size(208, 35);
            this.txt_SecDept.TabIndex = 2;
            this.txt_SecDept.Text = "2100";
            // 
            // txt_ExchAccount
            // 
            this.txt_ExchAccount.Location = new System.Drawing.Point(168, 288);
            this.txt_ExchAccount.Name = "txt_ExchAccount";
            this.txt_ExchAccount.Size = new System.Drawing.Size(208, 35);
            this.txt_ExchAccount.TabIndex = 3;
            this.txt_ExchAccount.Tag = "";
            this.txt_ExchAccount.Text = "210000020732";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 24);
            this.label1.TabIndex = 4;
            this.label1.Text = "主机";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "端口";
            // 
            // txt_ExchPwd
            // 
            this.txt_ExchPwd.Location = new System.Drawing.Point(168, 330);
            this.txt_ExchPwd.Name = "txt_ExchPwd";
            this.txt_ExchPwd.PasswordChar = '*';
            this.txt_ExchPwd.Size = new System.Drawing.Size(208, 35);
            this.txt_ExchPwd.TabIndex = 6;
            this.txt_ExchPwd.Text = "850114";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(44, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "营业部";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 291);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 24);
            this.label4.TabIndex = 8;
            this.label4.Text = "交易账号";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(44, 330);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 24);
            this.label5.TabIndex = 9;
            this.label5.Text = "密码";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 241);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 24);
            this.label6.TabIndex = 11;
            this.label6.Text = "资金账号";
            // 
            // txt_AmtAccount
            // 
            this.txt_AmtAccount.Location = new System.Drawing.Point(168, 238);
            this.txt_AmtAccount.Name = "txt_AmtAccount";
            this.txt_AmtAccount.Size = new System.Drawing.Size(208, 35);
            this.txt_AmtAccount.TabIndex = 10;
            this.txt_AmtAccount.Text = "210000020732";
            // 
            // btn_login
            // 
            this.btn_login.Location = new System.Drawing.Point(973, 45);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(122, 41);
            this.btn_login.TabIndex = 12;
            this.btn_login.Text = "登录";
            this.btn_login.UseVisualStyleBackColor = true;
            this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(44, 147);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 24);
            this.label7.TabIndex = 14;
            this.label7.Text = "端口";
            // 
            // txt_VersionNo
            // 
            this.txt_VersionNo.Location = new System.Drawing.Point(168, 144);
            this.txt_VersionNo.Name = "txt_VersionNo";
            this.txt_VersionNo.Size = new System.Drawing.Size(208, 35);
            this.txt_VersionNo.TabIndex = 13;
            this.txt_VersionNo.Text = "8.0";
            // 
            // frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1217, 582);
            this.Controls.Add(this.groupBox1);
            this.Name = "frm_Main";
            this.Text = "frm_Main";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_ExchAccount;
        private System.Windows.Forms.TextBox txt_SecDept;
        private System.Windows.Forms.TextBox txt_hostPort;
        private System.Windows.Forms.TextBox txt_host;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_ExchPwd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_AmtAccount;
        private System.Windows.Forms.Button btn_login;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_VersionNo;
    }
}
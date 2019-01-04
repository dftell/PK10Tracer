namespace WindowsFormsApplication1
{
    partial class loginFrm
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
            this.label2 = new System.Windows.Forms.Label();
            this.txt_username = new System.Windows.Forms.TextBox();
            this.txt_valicode = new System.Windows.Forms.TextBox();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_login = new System.Windows.Forms.Button();
            this.lbl_host1 = new System.Windows.Forms.Label();
            this.lbl_host2 = new System.Windows.Forms.Label();
            this.lbl_host3 = new System.Windows.Forms.Label();
            this.lbl_host4 = new System.Windows.Forms.Label();
            this.lbl_defaultHost = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_password = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(172, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "密码";
            // 
            // txt_username
            // 
            this.txt_username.Location = new System.Drawing.Point(295, 93);
            this.txt_username.Name = "txt_username";
            this.txt_username.Size = new System.Drawing.Size(286, 35);
            this.txt_username.TabIndex = 2;
            this.txt_username.Text = "user331";
            // 
            // txt_valicode
            // 
            this.txt_valicode.Location = new System.Drawing.Point(295, 217);
            this.txt_valicode.Name = "txt_valicode";
            this.txt_valicode.Size = new System.Drawing.Size(286, 35);
            this.txt_valicode.TabIndex = 3;
            // 
            // btn_cancel
            // 
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_cancel.Location = new System.Drawing.Point(433, 269);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(83, 47);
            this.btn_cancel.TabIndex = 4;
            this.btn_cancel.Text = "取消";
            this.btn_cancel.UseVisualStyleBackColor = true;
            // 
            // btn_login
            // 
            this.btn_login.Location = new System.Drawing.Point(347, 269);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(80, 47);
            this.btn_login.TabIndex = 5;
            this.btn_login.Text = "登陆";
            this.btn_login.UseVisualStyleBackColor = true;
            this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
            // 
            // lbl_host1
            // 
            this.lbl_host1.AutoSize = true;
            this.lbl_host1.Location = new System.Drawing.Point(201, 350);
            this.lbl_host1.Name = "lbl_host1";
            this.lbl_host1.Size = new System.Drawing.Size(82, 24);
            this.lbl_host1.TabIndex = 6;
            this.lbl_host1.Text = "label3";
            // 
            // lbl_host2
            // 
            this.lbl_host2.AutoSize = true;
            this.lbl_host2.Location = new System.Drawing.Point(201, 395);
            this.lbl_host2.Name = "lbl_host2";
            this.lbl_host2.Size = new System.Drawing.Size(82, 24);
            this.lbl_host2.TabIndex = 7;
            this.lbl_host2.Text = "label4";
            // 
            // lbl_host3
            // 
            this.lbl_host3.AutoSize = true;
            this.lbl_host3.Location = new System.Drawing.Point(201, 431);
            this.lbl_host3.Name = "lbl_host3";
            this.lbl_host3.Size = new System.Drawing.Size(82, 24);
            this.lbl_host3.TabIndex = 8;
            this.lbl_host3.Text = "label5";
            // 
            // lbl_host4
            // 
            this.lbl_host4.AutoSize = true;
            this.lbl_host4.Location = new System.Drawing.Point(201, 464);
            this.lbl_host4.Name = "lbl_host4";
            this.lbl_host4.Size = new System.Drawing.Size(82, 24);
            this.lbl_host4.TabIndex = 9;
            this.lbl_host4.Text = "label6";
            // 
            // lbl_defaultHost
            // 
            this.lbl_defaultHost.AutoSize = true;
            this.lbl_defaultHost.Location = new System.Drawing.Point(25, 329);
            this.lbl_defaultHost.Name = "lbl_defaultHost";
            this.lbl_defaultHost.Size = new System.Drawing.Size(70, 24);
            this.lbl_defaultHost.TabIndex = 10;
            this.lbl_defaultHost.Text = "fdfdf";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 228);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 24);
            this.label3.TabIndex = 12;
            this.label3.Text = "验证码";
            // 
            // txt_password
            // 
            this.txt_password.Location = new System.Drawing.Point(299, 155);
            this.txt_password.Name = "txt_password";
            this.txt_password.Size = new System.Drawing.Size(281, 35);
            this.txt_password.TabIndex = 13;
            this.txt_password.Text = "abcdef123";
            this.txt_password.UseSystemPasswordChar = true;
            // 
            // loginFrm
            // 
            this.AcceptButton = this.btn_login;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_cancel;
            this.ClientSize = new System.Drawing.Size(958, 927);
            this.Controls.Add(this.txt_password);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbl_defaultHost);
            this.Controls.Add(this.lbl_host4);
            this.Controls.Add(this.lbl_host3);
            this.Controls.Add(this.lbl_host2);
            this.Controls.Add(this.lbl_host1);
            this.Controls.Add(this.btn_login);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.txt_valicode);
            this.Controls.Add(this.txt_username);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "loginFrm";
            this.ShowIcon = false;
            this.Text = "快乐赛车";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_username;
        private System.Windows.Forms.TextBox txt_valicode;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_login;
        private System.Windows.Forms.Label lbl_host1;
        private System.Windows.Forms.Label lbl_host2;
        private System.Windows.Forms.Label lbl_host3;
        private System.Windows.Forms.Label lbl_host4;
        private System.Windows.Forms.Label lbl_defaultHost;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox txt_password;
    }
}
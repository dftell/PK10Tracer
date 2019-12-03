namespace Test_Win
{
    partial class frm_Socket
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
            this.btn_connect = new System.Windows.Forms.Button();
            this.btn_send = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_ip = new System.Windows.Forms.TextBox();
            this.txt_port = new System.Windows.Forms.TextBox();
            this.txt_receive = new System.Windows.Forms.TextBox();
            this.txt_send = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(1095, 20);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(112, 52);
            this.btn_connect.TabIndex = 0;
            this.btn_connect.Text = "连接";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // btn_send
            // 
            this.btn_send.Location = new System.Drawing.Point(1095, 739);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(112, 46);
            this.btn_send.TabIndex = 1;
            this.btn_send.Text = "发送";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.btn_send_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "主机";
            // 
            // txt_ip
            // 
            this.txt_ip.Location = new System.Drawing.Point(139, 37);
            this.txt_ip.Name = "txt_ip";
            this.txt_ip.Size = new System.Drawing.Size(379, 35);
            this.txt_ip.TabIndex = 3;
            this.txt_ip.Text = "182.61.36.196";
            // 
            // txt_port
            // 
            this.txt_port.Location = new System.Drawing.Point(569, 37);
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(141, 35);
            this.txt_port.TabIndex = 4;
            this.txt_port.Text = "80";
            // 
            // txt_receive
            // 
            this.txt_receive.Location = new System.Drawing.Point(40, 104);
            this.txt_receive.Multiline = true;
            this.txt_receive.Name = "txt_receive";
            this.txt_receive.Size = new System.Drawing.Size(1167, 233);
            this.txt_receive.TabIndex = 5;
            // 
            // txt_send
            // 
            this.txt_send.Location = new System.Drawing.Point(40, 366);
            this.txt_send.Multiline = true;
            this.txt_send.Name = "txt_send";
            this.txt_send.Size = new System.Drawing.Size(1167, 355);
            this.txt_send.TabIndex = 6;
            this.txt_send.Text = "reqId=jkljflsd&chargeAmt=2001";
            // 
            // frm_Socket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 816);
            this.Controls.Add(this.txt_send);
            this.Controls.Add(this.txt_receive);
            this.Controls.Add(this.txt_port);
            this.Controls.Add(this.txt_ip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_send);
            this.Controls.Add(this.btn_connect);
            this.MaximizeBox = false;
            this.Name = "frm_Socket";
            this.Text = "frm_Socket";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_ip;
        private System.Windows.Forms.TextBox txt_port;
        private System.Windows.Forms.TextBox txt_receive;
        private System.Windows.Forms.TextBox txt_send;
    }
}
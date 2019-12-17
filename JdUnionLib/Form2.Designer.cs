namespace WolfInv.com.JdUnionLib
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
            this.btn_request = new System.Windows.Forms.Button();
            this.txt_result = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ddl_className = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_PageSize = new System.Windows.Forms.TextBox();
            this.txt_PageNo = new System.Windows.Forms.TextBox();
            this.txt_PostData = new System.Windows.Forms.TextBox();
            this.txt_url = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkbox_Post = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_timestamp = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txt_app_secret = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_app_key = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_access_token = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txt_params_3_val = new System.Windows.Forms.TextBox();
            this.txt_params_3_key = new System.Windows.Forms.TextBox();
            this.txt_params_2_val = new System.Windows.Forms.TextBox();
            this.txt_params_2_key = new System.Windows.Forms.TextBox();
            this.txt_params_1_val = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_params_1_key = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_request
            // 
            this.btn_request.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_request.Location = new System.Drawing.Point(793, 5);
            this.btn_request.Margin = new System.Windows.Forms.Padding(2);
            this.btn_request.Name = "btn_request";
            this.btn_request.Size = new System.Drawing.Size(37, 26);
            this.btn_request.TabIndex = 0;
            this.btn_request.Text = "请求";
            this.btn_request.UseVisualStyleBackColor = true;
            this.btn_request.Click += new System.EventHandler(this.btn_request_Click);
            // 
            // txt_result
            // 
            this.txt_result.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_result.Location = new System.Drawing.Point(18, 285);
            this.txt_result.Margin = new System.Windows.Forms.Padding(2);
            this.txt_result.Multiline = true;
            this.txt_result.Name = "txt_result";
            this.txt_result.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_result.Size = new System.Drawing.Size(812, 186);
            this.txt_result.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 143);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "dataTypes";
            // 
            // ddl_className
            // 
            this.ddl_className.FormattingEnabled = true;
            this.ddl_className.Location = new System.Drawing.Point(106, 143);
            this.ddl_className.Margin = new System.Windows.Forms.Padding(2);
            this.ddl_className.Name = "ddl_className";
            this.ddl_className.Size = new System.Drawing.Size(287, 20);
            this.ddl_className.TabIndex = 7;
            this.ddl_className.SelectedIndexChanged += new System.EventHandler(this.ddl_className_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(666, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "页大小";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(666, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "页数";
            // 
            // txt_PageSize
            // 
            this.txt_PageSize.Location = new System.Drawing.Point(712, 24);
            this.txt_PageSize.Margin = new System.Windows.Forms.Padding(2);
            this.txt_PageSize.Name = "txt_PageSize";
            this.txt_PageSize.Size = new System.Drawing.Size(61, 21);
            this.txt_PageSize.TabIndex = 10;
            this.txt_PageSize.Text = "50";
            // 
            // txt_PageNo
            // 
            this.txt_PageNo.Location = new System.Drawing.Point(712, 52);
            this.txt_PageNo.Margin = new System.Windows.Forms.Padding(2);
            this.txt_PageNo.Name = "txt_PageNo";
            this.txt_PageNo.Size = new System.Drawing.Size(61, 21);
            this.txt_PageNo.TabIndex = 11;
            this.txt_PageNo.Text = "1";
            // 
            // txt_PostData
            // 
            this.txt_PostData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_PostData.Location = new System.Drawing.Point(18, 193);
            this.txt_PostData.Margin = new System.Windows.Forms.Padding(2);
            this.txt_PostData.Multiline = true;
            this.txt_PostData.Name = "txt_PostData";
            this.txt_PostData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_PostData.Size = new System.Drawing.Size(812, 79);
            this.txt_PostData.TabIndex = 12;
            this.txt_PostData.Text = "{\"eliteId\":\"22\",\"pageSize\":50,\"pageIndex\":1}";
            // 
            // txt_url
            // 
            this.txt_url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_url.Location = new System.Drawing.Point(106, 168);
            this.txt_url.Margin = new System.Windows.Forms.Padding(2);
            this.txt_url.Name = "txt_url";
            this.txt_url.Size = new System.Drawing.Size(724, 21);
            this.txt_url.TabIndex = 13;
            this.txt_url.TextChanged += new System.EventHandler(this.txt_url_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 171);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "URL";
            // 
            // chkbox_Post
            // 
            this.chkbox_Post.AutoSize = true;
            this.chkbox_Post.Location = new System.Drawing.Point(667, -3);
            this.chkbox_Post.Name = "chkbox_Post";
            this.chkbox_Post.Size = new System.Drawing.Size(48, 16);
            this.chkbox_Post.TabIndex = 15;
            this.chkbox_Post.Text = "Post";
            this.chkbox_Post.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_timestamp);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txt_app_secret);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txt_app_key);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_access_token);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(18, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 132);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "系统参数";
            // 
            // txt_timestamp
            // 
            this.txt_timestamp.Location = new System.Drawing.Point(88, 93);
            this.txt_timestamp.Margin = new System.Windows.Forms.Padding(2);
            this.txt_timestamp.Name = "txt_timestamp";
            this.txt_timestamp.Size = new System.Drawing.Size(181, 21);
            this.txt_timestamp.TabIndex = 25;
            this.txt_timestamp.Text = "2018-10-18 11:13:12";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 96);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 24;
            this.label10.Text = "timestamp";
            // 
            // txt_app_secret
            // 
            this.txt_app_secret.Location = new System.Drawing.Point(88, 68);
            this.txt_app_secret.Margin = new System.Windows.Forms.Padding(2);
            this.txt_app_secret.Name = "txt_app_secret";
            this.txt_app_secret.Size = new System.Drawing.Size(181, 21);
            this.txt_app_secret.TabIndex = 23;
            this.txt_app_secret.Text = "6d34r0d0kild46460654b42f5e350982";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 71);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 12);
            this.label7.TabIndex = 22;
            this.label7.Text = "app_secret ";
            // 
            // txt_app_key
            // 
            this.txt_app_key.Location = new System.Drawing.Point(88, 43);
            this.txt_app_key.Margin = new System.Windows.Forms.Padding(2);
            this.txt_app_key.Name = "txt_app_key";
            this.txt_app_key.Size = new System.Drawing.Size(181, 21);
            this.txt_app_key.TabIndex = 21;
            this.txt_app_key.Text = "eefc33bDRea044cb8ctre5hycf0ac1934";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 20;
            this.label2.Text = "app_key";
            // 
            // txt_access_token
            // 
            this.txt_access_token.Location = new System.Drawing.Point(88, 19);
            this.txt_access_token.Margin = new System.Windows.Forms.Padding(2);
            this.txt_access_token.Name = "txt_access_token";
            this.txt_access_token.Size = new System.Drawing.Size(181, 21);
            this.txt_access_token.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "Access_Token";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txt_params_3_val);
            this.groupBox2.Controls.Add(this.txt_params_3_key);
            this.groupBox2.Controls.Add(this.txt_params_2_val);
            this.groupBox2.Controls.Add(this.txt_params_2_key);
            this.groupBox2.Controls.Add(this.txt_params_1_val);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txt_params_1_key);
            this.groupBox2.Location = new System.Drawing.Point(322, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(316, 132);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "业务参数";
            // 
            // txt_params_3_val
            // 
            this.txt_params_3_val.Location = new System.Drawing.Point(163, 87);
            this.txt_params_3_val.Margin = new System.Windows.Forms.Padding(2);
            this.txt_params_3_val.Name = "txt_params_3_val";
            this.txt_params_3_val.Size = new System.Drawing.Size(123, 21);
            this.txt_params_3_val.TabIndex = 27;
            // 
            // txt_params_3_key
            // 
            this.txt_params_3_key.Location = new System.Drawing.Point(15, 87);
            this.txt_params_3_key.Margin = new System.Windows.Forms.Padding(2);
            this.txt_params_3_key.Name = "txt_params_3_key";
            this.txt_params_3_key.Size = new System.Drawing.Size(126, 21);
            this.txt_params_3_key.TabIndex = 26;
            this.txt_params_3_key.Text = "siteId";
            // 
            // txt_params_2_val
            // 
            this.txt_params_2_val.Location = new System.Drawing.Point(163, 62);
            this.txt_params_2_val.Margin = new System.Windows.Forms.Padding(2);
            this.txt_params_2_val.Name = "txt_params_2_val";
            this.txt_params_2_val.Size = new System.Drawing.Size(123, 21);
            this.txt_params_2_val.TabIndex = 25;
            // 
            // txt_params_2_key
            // 
            this.txt_params_2_key.Location = new System.Drawing.Point(15, 62);
            this.txt_params_2_key.Margin = new System.Windows.Forms.Padding(2);
            this.txt_params_2_key.Name = "txt_params_2_key";
            this.txt_params_2_key.Size = new System.Drawing.Size(126, 21);
            this.txt_params_2_key.TabIndex = 24;
            this.txt_params_2_key.Text = "skuIds";
            // 
            // txt_params_1_val
            // 
            this.txt_params_1_val.Location = new System.Drawing.Point(163, 37);
            this.txt_params_1_val.Margin = new System.Windows.Forms.Padding(2);
            this.txt_params_1_val.Name = "txt_params_1_val";
            this.txt_params_1_val.Size = new System.Drawing.Size(123, 21);
            this.txt_params_1_val.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(197, 17);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "参数值";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(63, 19);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "参数名称";
            // 
            // txt_params_1_key
            // 
            this.txt_params_1_key.Location = new System.Drawing.Point(15, 37);
            this.txt_params_1_key.Margin = new System.Windows.Forms.Padding(2);
            this.txt_params_1_key.Name = "txt_params_1_key";
            this.txt_params_1_key.Size = new System.Drawing.Size(126, 21);
            this.txt_params_1_key.TabIndex = 20;
            this.txt_params_1_key.Text = "goodsReq";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 477);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkbox_Post);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txt_url);
            this.Controls.Add(this.txt_PostData);
            this.Controls.Add(this.txt_PageNo);
            this.Controls.Add(this.txt_PageSize);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ddl_className);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_result);
            this.Controls.Add(this.btn_request);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_request;
        private System.Windows.Forms.TextBox txt_result;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ddl_className;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_PageSize;
        private System.Windows.Forms.TextBox txt_PageNo;
        private System.Windows.Forms.TextBox txt_PostData;
        private System.Windows.Forms.TextBox txt_url;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkbox_Post;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_app_secret;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_app_key;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_access_token;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txt_params_3_val;
        private System.Windows.Forms.TextBox txt_params_3_key;
        private System.Windows.Forms.TextBox txt_params_2_val;
        private System.Windows.Forms.TextBox txt_params_2_key;
        private System.Windows.Forms.TextBox txt_params_1_val;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_params_1_key;
        private System.Windows.Forms.TextBox txt_timestamp;
        private System.Windows.Forms.Label label10;
    }
}
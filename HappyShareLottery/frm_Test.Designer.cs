namespace HappyShareLottery
{
    partial class frm_Test
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
            this.txt_testUrl = new System.Windows.Forms.TextBox();
            this.txt_returnUrl = new System.Windows.Forms.TextBox();
            this.btn_test = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_testUrl
            // 
            this.txt_testUrl.Location = new System.Drawing.Point(62, 12);
            this.txt_testUrl.Name = "txt_testUrl";
            this.txt_testUrl.Size = new System.Drawing.Size(463, 21);
            this.txt_testUrl.TabIndex = 0;
            // 
            // txt_returnUrl
            // 
            this.txt_returnUrl.Location = new System.Drawing.Point(62, 58);
            this.txt_returnUrl.Name = "txt_returnUrl";
            this.txt_returnUrl.Size = new System.Drawing.Size(463, 21);
            this.txt_returnUrl.TabIndex = 1;
            // 
            // btn_test
            // 
            this.btn_test.Location = new System.Drawing.Point(686, 12);
            this.btn_test.Name = "btn_test";
            this.btn_test.Size = new System.Drawing.Size(74, 20);
            this.btn_test.TabIndex = 2;
            this.btn_test.Text = "test";
            this.btn_test.UseVisualStyleBackColor = true;
            this.btn_test.Click += new System.EventHandler(this.btn_test_Click);
            // 
            // frm_Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_test);
            this.Controls.Add(this.txt_returnUrl);
            this.Controls.Add(this.txt_testUrl);
            this.Name = "frm_Test";
            this.Text = "frm_Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_testUrl;
        private System.Windows.Forms.TextBox txt_returnUrl;
        private System.Windows.Forms.Button btn_test;
    }
}
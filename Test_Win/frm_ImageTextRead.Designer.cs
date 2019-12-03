namespace Test_Win
{
    partial class frm_ImageTextRead
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_select = new System.Windows.Forms.Button();
            this.btn_read = new System.Windows.Forms.Button();
            this.txt_imagePath = new System.Windows.Forms.TextBox();
            this.txt_readText = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(51, 95);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(703, 429);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btn_select
            // 
            this.btn_select.Location = new System.Drawing.Point(802, 14);
            this.btn_select.Name = "btn_select";
            this.btn_select.Size = new System.Drawing.Size(127, 35);
            this.btn_select.TabIndex = 1;
            this.btn_select.Text = "选择文件";
            this.btn_select.UseVisualStyleBackColor = true;
            this.btn_select.Click += new System.EventHandler(this.btn_select_Click);
            // 
            // btn_read
            // 
            this.btn_read.Location = new System.Drawing.Point(802, 72);
            this.btn_read.Name = "btn_read";
            this.btn_read.Size = new System.Drawing.Size(127, 38);
            this.btn_read.TabIndex = 2;
            this.btn_read.Text = "识别";
            this.btn_read.UseVisualStyleBackColor = true;
            this.btn_read.Click += new System.EventHandler(this.btn_read_Click);
            // 
            // txt_imagePath
            // 
            this.txt_imagePath.Location = new System.Drawing.Point(55, 14);
            this.txt_imagePath.Name = "txt_imagePath";
            this.txt_imagePath.Size = new System.Drawing.Size(698, 35);
            this.txt_imagePath.TabIndex = 3;
            // 
            // txt_readText
            // 
            this.txt_readText.Location = new System.Drawing.Point(51, 550);
            this.txt_readText.Multiline = true;
            this.txt_readText.Name = "txt_readText";
            this.txt_readText.Size = new System.Drawing.Size(966, 458);
            this.txt_readText.TabIndex = 4;
            // 
            // frm_ImageTextRead
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1046, 1031);
            this.Controls.Add(this.txt_readText);
            this.Controls.Add(this.txt_imagePath);
            this.Controls.Add(this.btn_read);
            this.Controls.Add(this.btn_select);
            this.Controls.Add(this.pictureBox1);
            this.Name = "frm_ImageTextRead";
            this.Text = "frm_ImageTextRead";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_select;
        private System.Windows.Forms.Button btn_read;
        private System.Windows.Forms.TextBox txt_imagePath;
        private System.Windows.Forms.TextBox txt_readText;
    }
}
namespace Strags
{
    partial class CommSelectOjectDialog<T>
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
            this.dg_Datas = new System.Windows.Forms.DataGridView();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.btn_Selected = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dg_Datas)).BeginInit();
            this.SuspendLayout();
            // 
            // dg_Datas
            // 
            this.dg_Datas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dg_Datas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_Datas.Location = new System.Drawing.Point(5, 22);
            this.dg_Datas.Name = "dg_Datas";
            this.dg_Datas.RowTemplate.Height = 23;
            this.dg_Datas.Size = new System.Drawing.Size(510, 317);
            this.dg_Datas.TabIndex = 0;
            // 
            // lbl_Title
            // 
            this.lbl_Title.AutoSize = true;
            this.lbl_Title.Location = new System.Drawing.Point(10, 7);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Size = new System.Drawing.Size(161, 12);
            this.lbl_Title.TabIndex = 1;
            this.lbl_Title.Text = "XXXXXXXXXXXXXXXXXXXXXXXXXX";
            // 
            // btn_Selected
            // 
            this.btn_Selected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Selected.Location = new System.Drawing.Point(451, 343);
            this.btn_Selected.Name = "btn_Selected";
            this.btn_Selected.Size = new System.Drawing.Size(63, 27);
            this.btn_Selected.TabIndex = 2;
            this.btn_Selected.Text = "选择";
            this.btn_Selected.UseVisualStyleBackColor = true;
            this.btn_Selected.Click += new System.EventHandler(this.btn_Selected_Click);
            // 
            // CommSelectOjectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 373);
            this.Controls.Add(this.btn_Selected);
            this.Controls.Add(this.lbl_Title);
            this.Controls.Add(this.dg_Datas);
            this.Name = "CommSelectOjectDialog";
            this.Text = "CommSelectOjectDialog";
            ((System.ComponentModel.ISupportInitialize)(this.dg_Datas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dg_Datas;
        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Button btn_Selected;
    }
}
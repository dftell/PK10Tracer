namespace BackTestSys
{
    partial class frm_CalcEr
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
            this.btn_PoissonDistrub = new System.Windows.Forms.Button();
            this.txt_N = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_M = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_Source = new System.Windows.Forms.TextBox();
            this.txt_Return = new System.Windows.Forms.TextBox();
            this.btn_Permutation = new System.Windows.Forms.Button();
            this.btn_Combination = new System.Windows.Forms.Button();
            this.btn_Binomial = new System.Windows.Forms.Button();
            this.txt_N1 = new System.Windows.Forms.TextBox();
            this.txt_M1 = new System.Windows.Forms.TextBox();
            this.btn_Bayes = new System.Windows.Forms.Button();
            this.btn_CKTran = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_PoissonDistrub
            // 
            this.btn_PoissonDistrub.Location = new System.Drawing.Point(504, 21);
            this.btn_PoissonDistrub.Name = "btn_PoissonDistrub";
            this.btn_PoissonDistrub.Size = new System.Drawing.Size(74, 24);
            this.btn_PoissonDistrub.TabIndex = 0;
            this.btn_PoissonDistrub.Text = "泊松分布";
            this.btn_PoissonDistrub.UseVisualStyleBackColor = true;
            this.btn_PoissonDistrub.Click += new System.EventHandler(this.btn_PoissonDistrub_Click);
            // 
            // txt_N
            // 
            this.txt_N.Location = new System.Drawing.Point(68, 19);
            this.txt_N.Name = "txt_N";
            this.txt_N.Size = new System.Drawing.Size(101, 21);
            this.txt_N.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "N/A";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "M/B";
            // 
            // txt_M
            // 
            this.txt_M.Location = new System.Drawing.Point(68, 46);
            this.txt_M.Name = "txt_M";
            this.txt_M.Size = new System.Drawing.Size(101, 21);
            this.txt_M.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Source";
            // 
            // txt_Source
            // 
            this.txt_Source.Location = new System.Drawing.Point(68, 73);
            this.txt_Source.Name = "txt_Source";
            this.txt_Source.Size = new System.Drawing.Size(407, 21);
            this.txt_Source.TabIndex = 5;
            // 
            // txt_Return
            // 
            this.txt_Return.Location = new System.Drawing.Point(12, 100);
            this.txt_Return.Multiline = true;
            this.txt_Return.Name = "txt_Return";
            this.txt_Return.Size = new System.Drawing.Size(463, 287);
            this.txt_Return.TabIndex = 7;
            // 
            // btn_Permutation
            // 
            this.btn_Permutation.Location = new System.Drawing.Point(504, 49);
            this.btn_Permutation.Name = "btn_Permutation";
            this.btn_Permutation.Size = new System.Drawing.Size(74, 24);
            this.btn_Permutation.TabIndex = 8;
            this.btn_Permutation.Text = "排列";
            this.btn_Permutation.UseVisualStyleBackColor = true;
            this.btn_Permutation.Click += new System.EventHandler(this.btn_Permutation_Click);
            // 
            // btn_Combination
            // 
            this.btn_Combination.Location = new System.Drawing.Point(504, 79);
            this.btn_Combination.Name = "btn_Combination";
            this.btn_Combination.Size = new System.Drawing.Size(74, 24);
            this.btn_Combination.TabIndex = 9;
            this.btn_Combination.Text = "组合";
            this.btn_Combination.UseVisualStyleBackColor = true;
            this.btn_Combination.Click += new System.EventHandler(this.btn_Combination_Click);
            // 
            // btn_Binomial
            // 
            this.btn_Binomial.Location = new System.Drawing.Point(504, 109);
            this.btn_Binomial.Name = "btn_Binomial";
            this.btn_Binomial.Size = new System.Drawing.Size(74, 24);
            this.btn_Binomial.TabIndex = 10;
            this.btn_Binomial.Text = "二项分布";
            this.btn_Binomial.UseVisualStyleBackColor = true;
            this.btn_Binomial.Click += new System.EventHandler(this.btn_Binomial_Click);
            // 
            // txt_N1
            // 
            this.txt_N1.Location = new System.Drawing.Point(201, 19);
            this.txt_N1.Name = "txt_N1";
            this.txt_N1.Size = new System.Drawing.Size(101, 21);
            this.txt_N1.TabIndex = 11;
            // 
            // txt_M1
            // 
            this.txt_M1.Location = new System.Drawing.Point(201, 46);
            this.txt_M1.Name = "txt_M1";
            this.txt_M1.Size = new System.Drawing.Size(101, 21);
            this.txt_M1.TabIndex = 12;
            // 
            // btn_Bayes
            // 
            this.btn_Bayes.Location = new System.Drawing.Point(505, 141);
            this.btn_Bayes.Name = "btn_Bayes";
            this.btn_Bayes.Size = new System.Drawing.Size(72, 22);
            this.btn_Bayes.TabIndex = 13;
            this.btn_Bayes.Text = "贝叶斯";
            this.btn_Bayes.UseVisualStyleBackColor = true;
            this.btn_Bayes.Click += new System.EventHandler(this.btn_Bayes_Click);
            // 
            // btn_CKTran
            // 
            this.btn_CKTran.Location = new System.Drawing.Point(505, 169);
            this.btn_CKTran.Name = "btn_CKTran";
            this.btn_CKTran.Size = new System.Drawing.Size(72, 22);
            this.btn_CKTran.TabIndex = 14;
            this.btn_CKTran.Text = "C-K转换";
            this.btn_CKTran.UseVisualStyleBackColor = true;
            // 
            // frm_CalcEr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 411);
            this.Controls.Add(this.btn_CKTran);
            this.Controls.Add(this.btn_Bayes);
            this.Controls.Add(this.txt_M1);
            this.Controls.Add(this.txt_N1);
            this.Controls.Add(this.btn_Binomial);
            this.Controls.Add(this.btn_Combination);
            this.Controls.Add(this.btn_Permutation);
            this.Controls.Add(this.txt_Return);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_Source);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_M);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_N);
            this.Controls.Add(this.btn_PoissonDistrub);
            this.Name = "frm_CalcEr";
            this.Text = "常用计算器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_PoissonDistrub;
        private System.Windows.Forms.TextBox txt_N;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_M;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_Source;
        private System.Windows.Forms.TextBox txt_Return;
        private System.Windows.Forms.Button btn_Permutation;
        private System.Windows.Forms.Button btn_Combination;
        private System.Windows.Forms.Button btn_Binomial;
        private System.Windows.Forms.TextBox txt_N1;
        private System.Windows.Forms.TextBox txt_M1;
        private System.Windows.Forms.Button btn_Bayes;
        private System.Windows.Forms.Button btn_CKTran;
    }
}
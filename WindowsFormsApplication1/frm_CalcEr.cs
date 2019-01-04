using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProbMathLib;
namespace BackTestSys
{
    public partial class frm_CalcEr : Form
    {
        public frm_CalcEr()
        {
            InitializeComponent();
            
        }

        private void btn_PoissonDistrub_Click(object sender, EventArgs e)
        {
            int N = int.Parse(this.txt_N.Text);
            int M = int.Parse(this.txt_M.Text);
            double prob = double.Parse(this.txt_Source.Text);
            this.txt_Return.Text = string.Format("{0}", ProbMath.GetPoission(N, prob, M));
        }

        private void btn_Permutation_Click(object sender, EventArgs e)
        {
            int N = int.Parse(this.txt_N.Text);
            int M = int.Parse(this.txt_M.Text);
            this.txt_Return.Text = string.Format("{0}", ProbMath.GetFactorial(N,  M));
        }

        private void btn_Combination_Click(object sender, EventArgs e)
        {
            int N = int.Parse(this.txt_N.Text);
            int M = int.Parse(this.txt_M.Text);
            this.txt_Return.Text = string.Format("{0}", ProbMath.GetCombination(N, M));
        }

        private void btn_Binomial_Click(object sender, EventArgs e)
        {
            int N = int.Parse(this.txt_N.Text);
            int M = int.Parse(this.txt_M.Text);
            double prob = double.Parse(this.txt_Source.Text);
            this.txt_Return.Text = string.Format("{0}", ProbMath.GetBinomial(N, M,prob));
        }

        private void btn_Bayes_Click(object sender, EventArgs e)
        {
            double Atrue = double.Parse(this.txt_N.Text);
            double Btrue = double.Parse(this.txt_M.Text);
            double Afalse = double.Parse(this.txt_N1.Text);
            double Bfalse = double.Parse(this.txt_M1.Text);
            this.txt_Return.Text = string.Format("{0}", EntropyClass.GetBayes(Atrue,Afalse,Btrue,Bfalse));
        }
    }
}

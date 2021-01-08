using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.Com.WQLanguageLib;
using System.Text.RegularExpressions;

namespace WolfInvQuantIDevStudio
{
    public partial class useStragEditor : UserControl
    {
        public useStragEditor()
        {
            InitializeComponent();
        }

        private void richTextBox_Editor_TextChanged(object sender, EventArgs e)
        {
            string keys = "funtion,var,if,else,begin,end,{,},for,foreach,while,return,continue,break,goto,switch,case";
            RichTextBox textbox = sender as RichTextBox;
            int index = textbox.GetFirstCharIndexOfCurrentLine();
            if (textbox.Text.Length == 0)
                return;
            int line = textbox.GetLineFromCharIndex(index);
            string currLineString = textbox.Lines[Math.Min(line,textbox.Lines.Length-1)];
            int currIndex = index + currLineString.Length;
            SentenceExplainer se = new SentenceExplainer(currLineString);
            WordInfo[] keyWords = se.getKeyWords();
            for(int i=0;i<keyWords.Length;i++)
            {
                textbox.SelectionStart = index + keyWords[i].index;
                textbox.SelectionLength = keyWords[i].Word.Length;
                textbox.SelectionColor = Color.Red;
            }
            //textbox.ScrollToCaret();
            textbox.SelectionStart = currIndex;
            textbox.SelectionLength = 0;
            textbox.SelectionColor = Color.Black;
            //textbox.Focus();
        }

        private void richTextBox_Editor_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)//如果是回车键
            {
                RichTextBox textbox = sender as RichTextBox;
                int index = textbox.GetFirstCharIndexOfCurrentLine();
                if (textbox.Text.Length == 0)
                    return;
                int line = textbox.GetLineFromCharIndex(index);
                string currLineString = textbox.Lines[Math.Min(line-1, textbox.Lines.Length - 1)];
                Regex reg = new Regex(@"^(\t*)");
                MatchCollection ms = reg.Matches(currLineString);
                if(ms.Count>0)
                {
                    textbox.SelectionStart = index;
                    textbox.SelectionLength = 0;
                    textbox.Lines[line] = ms[0].Value;
                    //textbox.AppendText(ms[0].Value);//在最后追加
                    //textbox.SelectionColor = Color.Black;
                    }
                textbox.SelectionStart = index + ms[0].Value.Length;

            }
        }
    }
}

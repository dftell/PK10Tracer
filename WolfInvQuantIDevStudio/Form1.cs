using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WolfInvQuantIDevStudio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void stragToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openStagToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolBar_NewStrage_Click(object sender, EventArgs e)
        {
            useStragEditor useditor = new useStragEditor();
            useditor.Dock = DockStyle.Fill;
            TabPage tp = new TabPage("newStage");
            tp.Controls.Add(useditor);
            this.tabControl_Editor.TabPages.Add(tp);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void tabControl_Editor_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void tabControl_Editor_DrawItem(object sender, DrawItemEventArgs e)
        {
            Brush biaocolor = Brushes.Silver; //选项卡的背景色 
            TabControl TabCtrl = sender as TabControl;
            int iconWidth = 4;// TabCtrl.icon.Width;
            int iconHeight = 4;// icon.Height;
            Graphics g = e.Graphics;
            Rectangle r = TabCtrl.GetTabRect(e.Index);
            if (e.Index == TabCtrl.SelectedIndex)    //当前选中的Tab页，设置不同的样式以示选中 
            {
                Brush selected_color = Brushes.Gray; //选中的项的背景色 
                g.FillRectangle(selected_color, r); //改变选项卡标签的背景色 
                string title = TabCtrl.TabPages[e.Index].Text + "   ";
                g.DrawString(title, this.Font, new SolidBrush(Color.Black), new PointF(r.X + 3, r.Y + 6));//PointF选项卡标题的位置 
                r.Offset(r.Width - iconWidth - 3, 2);
                g.DrawString("X", this.Font, new SolidBrush(Color.Black), new Point(r.X - 2, r.Y + 2));
                //g.DrawImage(icon, new Point(r.X - 2, r.Y + 2));//选项卡上的图标的位置 fntTab = new System.Drawing.Font(e.Font, FontStyle.Bold); 
            }
            else//非选中的 
            {
                g.FillRectangle(biaocolor, r); //改变选项卡标签的背景色 
                string title = TabCtrl.TabPages[e.Index].Text + "   ";
                g.DrawString(title, this.Font, new SolidBrush(Color.Black), new PointF(r.X + 3, r.Y + 6));//PointF选项卡标题的位置 
                r.Offset(r.Width - iconWidth - 3, 2);
                g.DrawString("X", this.Font, new SolidBrush(Color.Black), new Point(r.X - 2, r.Y + 2));
                //g.DrawImage(icon, new Point(r.X - 2, r.Y + 2));//选项卡上的图标的位置 
            }
        }

        private void tabControl_Editor_MouseDown(object sender, MouseEventArgs e)
        {
            TabControl tabctrl = sender as TabControl;
            for (int i = 0; i < tabctrl.TabPages.Count; i++)
            {
                Rectangle r = tabControl_Editor.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
                if (closeButton.Contains(e.Location))
                {
                    tabctrl.TabPages.RemoveAt(i);
                    /*if (MessageBox.Show("Would you like to Close this Tab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        tabctrl.TabPages.RemoveAt(i);
                        break;
                    }*/
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoDraw
{
    public partial class newForm1 : Form
    {
        public newForm1()
        {
            InitializeComponent();
            button1.Location = new Point(this.Size.Width / 2 - (button1.Size.Width / 2), this.Size.Height / 2);

        }

        MainInterface mI;
        private void newForm1_Load(object sender, EventArgs e)
        {
            if (mI == null)
            {
                mI = new MainInterface();
                button1.Visible = false;
                mI.FormClosed += MI_FormClosed;
                mI.MdiParent = this;
                mI.WindowState = FormWindowState.Maximized;
                mI.Show();

                this.Text = "防灾系统图绘制工具";
                this.MaximumSize = new Size(1300, 800); 
            }
            else
            {
                mI.Show();
            }
        }

        private void MI_FormClosed(object sender, FormClosedEventArgs e)
        {
            button1.Visible = true;
            //throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (mI == null || mI.IsDisposed)
            {
                mI = new MainInterface();
                button1.Visible = false;
                mI.MdiParent = this;
                mI.WindowState = FormWindowState.Maximized;
                mI.Show();

                this.Text = "防灾系统图绘制工具";
                this.MaximumSize = new Size(1300, 800);
            }
            

        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mI == null || mI.IsDisposed)
            {
                mI = new MainInterface();
                button1.Visible = false;
                mI.MdiParent = this;
                mI.WindowState = FormWindowState.Maximized;
                mI.Show();

                this.Text = "防灾系统图绘制工具";
                this.MaximumSize = new Size(1300, 800);
            }
        }

        private void newForm1_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void newForm1_Resize(object sender, EventArgs e)
        {
            button1.Location = new Point(this.Size.Width / 2 - (button1.Size.Width / 2), this.Size.Height / 2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoDraw
{
    public partial class TuQian : Form
    {
        string settingP;
        public TuQian(string setting)
        {
            InitializeComponent();
            settingP = setting;
        }

        private void TuQian_Load(object sender, EventArgs e)
        {
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("Resunce.abc", this.GetType().Assembly);
            byte[] bit = rm.GetObject("abc") as byte[];
            textBox1.Text = bit.ToString();
            if (settingP == "")
            {
                settingP = "c:\\temp";
            }

        }

        private void B_Valide_Click(object sender, EventArgs e)
        {
            //写文件
            if (settingP == "c:\\temp")
            {
                Directory.CreateDirectory(@"c:\\temp");
                using (File.Create(@"c:\\temp\\Setting.xml"))
                {

                }
            }
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

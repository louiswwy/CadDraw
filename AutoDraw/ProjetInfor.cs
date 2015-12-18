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
    public partial class ProjetInfor : Form
    {
        string xmlDicPath = "";
        public ProjetInfor(string xmlPat)
        {
            InitializeComponent();
            xmlDicPath = xmlPat;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString() != "" && textBox2.Text.ToString() != "") //不为空时
            {
                XmlFunction xF = new XmlFunction();
                xF.writeProjrtInfo(xmlDicPath, textBox1.Text.ToString(), textBox2.Text.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

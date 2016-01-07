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
            if (textBox1.Text.ToString() != "" && textBox2.Text.ToString() != "" && textBox3.Text.ToString() != "" && textBox4.Text.ToString() != "") //不为空时
            {
                XmlFunction xF = new XmlFunction();
                try
                {
                    if (MessageBox.Show("项目名称:" + textBox1.Text.ToString().Replace(" ", ""), "\n图   号" + textBox2.Text.ToString().Replace(" ", "") + "\n确认保存?", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        xF.writeProjrtInfo(xmlDicPath + "\\setting.xml", textBox1.Text.ToString().Replace(" ", ""), textBox2.Text.ToString().Replace(" ", ""), textBox3.Text.ToString().Replace(" ", ""), textBox4.Text.ToString().Replace(" ", ""));
                        toolStripStatusLabel1.Text = "项目名称:" + textBox1.Text.ToString().Replace(" ", "") + "已保存.";

                    }
                }
                catch (Exception ee)
                {

                    MessageBox.Show("错误:\n" + ee.ToString(), "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ProjetInfor_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
        }
    }
}

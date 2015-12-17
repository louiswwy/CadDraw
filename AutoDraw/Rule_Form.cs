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
    public partial class F_Rule : Form
    {
        string imgDictionPath = "";
        XmlFunction xF;
        public F_Rule(string iconPath)
        {
            InitializeComponent();
            imgDictionPath = iconPath;
        }

        private void T_MaxDistance_KeyDown(object sender, KeyEventArgs e)
        {
            if (T_MaxDistance.Text.ToString() != "")  //textBox不为空
            {
                if (e.KeyCode == Keys.Enter) //当选中该textBox时按回车键时
                {
                    //输入数据

                }
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectItem = comboBox1.SelectedItem.ToString();
            TreeNode rootNode = new TreeNode(); //添加treenode
            rootNode.Name = selectItem;

            if (!treeView1.Nodes.Contains(rootNode)) //如果没有combobox的选中项
            {
                //添加选中项
                treeView1.Nodes.Add(rootNode);
                T_MaxDistance.Text = "";
                groupBox2.Text = "选中：" + selectItem;

                StringBuilder sB = new StringBuilder();
                foreach (var str in imgDictionPath.Split(new char[] { '\\' }))
                {
                    if (!str.Contains("icon"))
                    {
                        sB.Append(str + "\\");
                    }
                }
                xF.writeRule(sB.ToString(),selectItem);
            }
            else
            {
                treeView1.SelectedNode = rootNode;
            }

            //可以增加筛选combobox项部分
            if (selectItem!="")
            {
                foreach (var item in fileInfo)
                {
                    comboBox2.Items.Add(item.Name.ToString().Split(new char[] { '_' }[0]));//添加项  
                }
            }
        }

        FileInfo[] fileInfo;
        private void F_Rule_Load(object sender, EventArgs e)
        {
            xF = new XmlFunction();
            DirectoryInfo Dir = new DirectoryInfo(imgDictionPath);
            fileInfo = Dir.GetFiles("*.bmp");  //读取.bmp格式的文件
            if (fileInfo.Length > 0)  //如果icon文件夹不为空
            {
                foreach (FileInfo f in Dir.GetFiles("*.bmp")) //查找文件
                {
                    comboBox1.Items.Add(f.Name.ToString().Split(new char[] { '_' }[0]));//添加项
                    

                }
            }
            else
            {
                MessageBox.Show("icon文件夹中没有找到文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

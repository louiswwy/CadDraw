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
        string ruleFilePath = "";
        XmlFunction xF;
        public F_Rule(string iconPath)
        {
            InitializeComponent();
            imgDictionPath = iconPath;

            StringBuilder sB = new StringBuilder();
            foreach (var str in imgDictionPath.Split(new char[] { '\\' }))
            {
                if (!str.Contains("icon"))
                {
                    sB.Append(str + "\\");
                }
            }
            ruleFilePath = sB.ToString() + "\\setting\\rule.xml";
        }

        List<string> listMiNMax;
        private void T_MaxDistance_KeyDown(object sender, KeyEventArgs e)
        {
            listMiNMax = new List<string>();
            List<string> localMaxMin = new List<string>();
            PFunction pF = new PFunction();
            if (T_MaxDistance.Text.ToString() != "" ) 
            {

                if (T_MaxDistance.Text.ToString() != "")  //textBox不为空
                {
                    if (e.KeyCode == Keys.Enter) //当选中该textBox时按回车键时
                    {
                        //输入数据
                        if (pF.isExMatch(T_MaxDistance.Text.ToString().Replace(" ", ""), @"^(\d+)-(\d+)$", out localMaxMin))
                        {
                            if(Int32.Parse( localMaxMin[0])<= Int32.Parse(localMaxMin[1]))
                            {
                                string min = localMaxMin[0];
                                string max = localMaxMin[1];
                                listMiNMax.Add(min);
                                listMiNMax.Add(max);

                            }
                            else
                            {
                                string min = localMaxMin[1];
                                string max = localMaxMin[0];
                                listMiNMax.Add(min);
                                listMiNMax.Add(max);
                            }

                        }
                    }
                }

                else
                {
                    MessageBox.Show("格式不正确，格式应该为 xx-xx！ ", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectItem = comboBox1.SelectedItem.ToString();
            TreeNode rootNode = new TreeNode(); //添加treenode
            rootNode.Text = selectItem;
            //treeView1.Nodes.Add(rootNode);

            bool isDupli = false;
            foreach(TreeNode item in treeView1.Nodes)
            {
                if (item.Text.ToString().Contains(selectItem))
                {
                    isDupli = true;
                    break;
                }
            }
            if (isDupli==false) //如果没有combobox的选中项
            {
                //添加选中项
                treeView1.Nodes.Add(rootNode);
                T_MaxDistance.Text = "";
                groupBox2.Text = "选中：" + selectItem;


                xF.writeRule(ruleFilePath.ToString(), selectItem);
            }
            else
            {
                treeView1.SelectedNode = rootNode;
            }

            //可以增加筛选combobox项部分
            if (selectItem == "")
            {
                foreach (var item in fileInfo)
                {
                    comboBox2.Items.Add(item.Name.ToString().Split(new char[] { '_' }[0]));//添加项  
                }
            }
            else if (selectItem.Contains("地震"))  //当选择地震仪时，不考虑设备布放间隔
            {
                comboBox4.Enabled = false;
                T_MaxDistance.Enabled = false;
            }
            else if (selectItem.Contains("监控单位"))  //当选择地震仪时，不考虑设备布放间隔
            {
                comboBox4.Enabled = false;
                T_MaxDistance.Enabled = false;
            }
            else if (selectItem.Contains("接触网杆"))  //当选择地震仪时，不考虑设备布放间隔
            {
                comboBox4.Enabled = false;
                T_MaxDistance.Enabled = false;
            }
            else if (selectItem.Contains("防灾控制箱"))  //当选择地震仪时，不考虑设备布放间隔
            {
                comboBox4.Enabled = false;
                T_MaxDistance.Enabled = false;
            }
            else
            {
                comboBox4.Enabled = true;
                T_MaxDistance.Enabled = true;
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

                    if ((!f.Name.ToString().Contains("站") && !f.Name.ToString().Contains("所")) && !f.Name.ToString().Contains("图例") )
                    {
                        string splitName1 = f.Name.ToString().Split(new char[] { '_' })[0];
                        string splitName2 = splitName1.Split(new char[] { '.' })[0];
                        //
                        comboBox1.Items.Add(splitName2.ToString());     //添加项 
                    }
                    

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

        private void B_AddEquipe_Click(object sender, EventArgs e)
        {
            string seleName="";  //选中的设备名称
            List<string> listRules=new List<string>();

            string MaxDis = "";
            string MinDis = "";
            if (comboBox4.Enabled != false && T_MaxDistance.Enabled != false) //可以输入数据
            {
                if(comboBox4.SelectedItem.ToString() != "" && T_MaxDistance.Text.ToString() != "") //不为空
                {
                    
                    if (treeView1.SelectedNode.Level == 0)
                    {
                        seleName = treeView1.SelectedNode.Text.ToString();
                    }
                    else if(treeView1.SelectedNode.Level > 0)
                    {
                        seleName = treeView1.SelectedNode.Parent.Text.ToString();
                    }

                    MaxDis = T_MaxDistance.Text.ToString().Split(new char[] { '-' })[0]; //两设备间的最大距离
                    MinDis = T_MaxDistance.Text.ToString().Split(new char[] { '-' })[1];
                }
                else
                {
                    MessageBox.Show("值不能为空！", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            xF.updateRule(ruleFilePath, seleName, listRules);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox4.Enabled!=false&& T_MaxDistance.Enabled != false)
            {
                string Max = "";
            }
        }

        private void T_MaxDistance_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

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

        string projetName = "";
        public ProjetInfor(string xmlPat,out string projectName)
        {
            InitializeComponent();
            xmlDicPath = xmlPat;
            projectName = projetName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (T_Project_Name.Text.ToString() != "" && T_Print_Pattren.Text.ToString() != "" && T_Project_Phase.Text.ToString() != "") //不为空时
            {
                XmlFunction xF = new XmlFunction();
                try
                {
                    if (MessageBox.Show("项目名称:" + T_Project_Name.Text.ToString().Replace(" ", "") + "\n图纸阶段:" + T_Project_Phase.Text.ToString().Replace(" ", "") + "\n确认保存?", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        ClassStruct.ProjectPrintPattern ppPattern = new ClassStruct.ProjectPrintPattern(T_Print_Name.Text.ToString().Replace(" ", ""), T_Print_Chapter.Text.ToString().Replace(" ", ""));
                        ClassStruct.ProjectInfo pI = new ClassStruct.ProjectInfo(T_Project_Name.Text.ToString().Replace(" ", ""), T_Project_Phase.Text.Replace(" ", ""), ppPattern);

                        xF.writeProjrtInfo(xmlDicPath + "\\setting.xml", pI);
                        toolStripStatusLabel1.Text = "项目名称:" + T_Project_Name.Text.ToString().Replace(" ", "") + "已保存.";

                        MainInterface ower = (MainInterface)this.Owner;
                        ower.refreshTitleName(pI.ProjectName);

                        projetName = T_Project_Name.Text.ToString().Replace(" ", "");
                    }
                }
                catch (Exception ee)
                {

                    MessageBox.Show("错误:\n" + ee.ToString(), "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    toolStripStatusLabel1.Text = "出现错误，未能录入信息.";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ProjetInfor_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = ""; XmlFunction xf = new XmlFunction();
            ClassStruct.ProjectInfo pI = xf.readProjrtInfo(xmlDicPath + "\\setting.xml");
            if (pI != null)
            {
                T_Project_Name.Text = pI.ProjectName;
                T_Project_Phase.Text = pI.ProjectPhase;
                T_Print_Chapter.Text = pI.PrintNamePattern.PrintChapter;
                T_Print_Name.Text = pI.PrintNamePattern.PrintName;
                //T = pI.ProjectPhase;
                //return;
            }
        }

        private void T_Print_Name_TextChanged(object sender, EventArgs e)
        {
            patternPrintName();

        }

        private void T_Print_Chapter_TextChanged(object sender, EventArgs e)
        {
            patternPrintName();

        }

        //自动更新命名规则栏
        private void patternPrintName()
        {
            if (T_Print_Chapter.Text.Replace(" ", "") != "")
            {
                T_Print_Pattren.Text = T_Print_Name.Text.Replace(" ", "") + "-" + T_Print_Chapter.Text.Replace(" ", "") + "-";
            }
            else
            {
                T_Print_Pattren.Text = T_Print_Name.Text.Replace(" ", "") + "-";
            }
        }
    }
}

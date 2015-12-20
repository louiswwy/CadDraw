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
    public partial class Form_LineType : Form
    {
        public string dataTimeTag = "";
        string XmlFilePath = "";
        List<string> NandT;
        XmlFunction xf;
        DataSet LineType;
        Dictionary<string, string> DicNandT;
        Dictionary<string, string> newDicNandT;

        public Form_LineType(string xmlFile)
        {
            InitializeComponent();
            XmlFilePath = xmlFile;

            /*NandT = new List<string>();

            NandT.Add("敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,8芯");
            NandT.Add("敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,12芯");
            NandT.Add("敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,16芯");
            NandT.Add("敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,21芯");
            NandT.Add("敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,28芯");
            NandT.Add("敷设铝护套信号电缆,PTYL,23,12芯");*/


        }

        public Dictionary<string, string> defautLineType(string xmlfile)
        {
            DicNandT = new Dictionary<string, string>();
            DicNandT.Add("SPTYWPL23 8芯", "敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,8芯");
            DicNandT.Add("SPTYWPL23 12芯", "敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,12芯");
            DicNandT.Add("SPTYWPL23 16芯", "敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,16芯");
            DicNandT.Add("SPTYWPL23 21芯", "敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,21芯");
            DicNandT.Add("SPTYWPL23 29芯", "敷设内屏蔽铝护套数字信号电缆,SPTYWPL,23,28芯");
            DicNandT.Add("PTYL23 12芯", "敷设铝护套信号电缆,PTYL,23,12芯");

            try
            {
                xf = new XmlFunction();
                xf.createLineType(xmlfile, "lineName", DicNandT); //只在xml文件中没有lineName节点时写入

                //只在运行过程中变更线类型表时触发,此处取消
                dataTimeTag = DateTime.Now.ToLongTimeString();        //记录时间
                //var args = new TimeMarkUpdateEventArgs(DicNandT);   //
                //TimeMarkUpdated(this, args);                        //
            }
            catch (System.Exception ee)
            {

                MessageBox.Show("错误：" + ee.ToString());
                return null;
            }


            return DicNandT;

        }
        private void Form_LineType_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "项目路径： " + XmlFilePath;


            if (XmlFilePath != "")
            {
                Dictionary<string, string> defautLineList = new Dictionary<string, string>();
                xf = new XmlFunction();

                try
                {
                    newDicNandT = xf.loadLineType(XmlFilePath); //读取的项
                    if (newDicNandT.Count == 0)
                    {
                        defautLineList = defautLineType(XmlFilePath);
                        newDicNandT = defautLineList;
                    }

                    fileDataView(newDicNandT);
                    //dataGridView1.Columns[0].ReadOnly = true;  //只读
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("错误:" + ex.ToString());
                    return;
                }
            }
        }

        //初始化
        public void fileDataView(Dictionary<string, string> DicNandT)
        {
            if (LineType == null)
            {
                LineType = new DataSet();
                System.Data.DataTable lineTable = new DataTable();

                LineType = new DataSet("LineTypeName"); //创建‘线缆类型名字’表
                lineTable = LineType.Tables.Add("Line");//创建‘所亭’表
                System.Data.DataColumn ST = lineTable.Columns.Add("名称", typeof(string));// station.Key.ToUpper());

                lineTable.Columns.Add("缩写", typeof(string));

                lineTable.Columns.Add("数量1", typeof(string));

                lineTable.Columns.Add("芯数", typeof(string));

                //lineTable.PrimaryKey = new System.Data.DataColumn[] { ST };



                foreach (var line in DicNandT)
                {
                    string text = line.Value.ToString();
                    //string[] a = text.Split(new char[] { ',' });
                    string name = text.Split(new char[] { ',' })[0];
                    string sx = text.Split(new char[] { ',' })[1];
                    string num = text.Split(new char[] { ',' })[2];
                    string numX = text.Split(new char[] { ',' })[3];

                    if (true)//!lineTable.Rows.Contains(station.Key.ToUpper())) //如果datatable中不含有所亭项 
                    {
                        lineTable.Rows.Add(name, sx, num, numX);  //添加
                        
                    }
                }
                dataGridView1.DataSource = LineType.Tables["Line"];
                dataGridView1.Columns[0].Width = 200;
                LineType.AcceptChanges();
            }
        }

        /// <summary>
        /// 取消变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_CancelChange_Click(object sender, EventArgs e)
        {
            LineType.RejectChanges();

            toolStripStatusLabel1.Text = "取消变更";

            timer1.Enabled=true;

            timer1.Interval = 3000;

            //timer1.Enabled = false;

            //toolStripStatusLabel1.Text = "项目路径： " + XmlFilePath;
        }

        /// <summary>
        /// 保存变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_ApplyChange_Click(object sender, EventArgs e)
        {
            XmlFunction xf = new XmlFunction();
            LineType.AcceptChanges();
            Dictionary<string,string> newDataSet=new Dictionary<string,string>();

            foreach (DataRow NewRow in LineType.Tables[0].Rows)
            {
                object[] a = NewRow.ItemArray;

                string type = a[0].ToString();
                string sx = a[1].ToString();
                string num1 = a[2].ToString();
                string numX = a[3].ToString();

                newDataSet.Add(sx + num1 + " " + numX, type + "," + sx + "," + num1 + "," + numX);

                //DataColumn col=NewRow.Table.Rows.co
            }
            xf.updataLineType(XmlFilePath, "LineName", newDataSet);

            //
            dataTimeTag = DateTime.Now.ToLongTimeString();
            //var args = new TimeMarkUpdateEventArgs(newDataSet);
            //TimeMarkUpdated(this, args);
            //xf.updataLineType(XmlFilePath,)
            /*bool done=false;

            //LineType.AcceptChanges();
            if (true)
            {
                DataSet changeDataSet = LineType.GetChanges();


                foreach (DataRow row in LineType.Tables[0].Rows)
                {
                    
                    //

                    //
                    if (row.RowState == DataRowState.Unchanged) //行未改变
                    {
                        continue;

                    }
                    else if (row.RowState == DataRowState.Added)  //新增行
                    {
                        string a = row.Table.ToString();
                        //row.col
                        string newData = "";
                        xf.addLineType(XmlFilePath, newData);
                    }
                    else if (row.RowState == DataRowState.Modified)  //修改
                    {

                        string pause = "";
                        string oldValue = "";
                        string newValue = "";
                        xf.updataLineType(XmlFilePath, oldValue, newValue);
                    }
                    else if (row.RowState == DataRowState.Deleted)  //删除
                    {
                        DataRow deletR = row;
                        //int a=row.le
                        //string a = deletR.Table.Columns[0].ToString();
                    }
                    else if (row.RowState == DataRowState.Detached)
                    {

                    }
                    
                }
            }*/

                
        }



        int i = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "项目路径： " + XmlFilePath;
            timer1.Enabled = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //声明一个更新Address的委托
        public delegate void TimeMarkUpdateHandler(object sender, TimeMarkUpdateEventArgs e);

        //声明一个更新Address的事件
        public event TimeMarkUpdateHandler TimeMarkUpdated;

        public class TimeMarkUpdateEventArgs : System.EventArgs
        {
            private Dictionary<string, string> passDictionary;
            public TimeMarkUpdateEventArgs(Dictionary<string, string> PassValue)
            {
                this.passDictionary = PassValue;
            }
            public Dictionary<string, string> markTheTime { get { return passDictionary; } }
        }


    }
}


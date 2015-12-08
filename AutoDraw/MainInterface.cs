using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Text;
using System;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using DotNetARX;
using System.Resources;
using System.IO;
using System.Xml;

namespace AutoDraw
{
    public partial class MainInterface : Form
    {
        private string filePath;  //文件位置
        private string newFilePath;
        string xmlFilePath;       //xml文件位置
        private string imgStoragePath; //图像文件位置 imgPath

        public MainInterface()
        {
            InitializeComponent();
        }

        private void B_Draw_Click(object sender, EventArgs e)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {

                //
                Point2d outerStartPoint = new Point2d(0, 0);
                Point2d outerEndPoint = new Point2d(outerStartPoint.X + 420, outerStartPoint.Y + 297);

                Point2d innerStartPoint = new Point2d(outerStartPoint.X + 25, outerStartPoint.Y + 5);
                Point2d innerEndPoint = new Point2d(outerEndPoint.X - 5, outerEndPoint.Y - 5);

                try
                {
                    DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();


                    //创建字体
                    ObjectId fontStyleId = createFont();
                    
                    #region 绘制图框
                    Polyline rectangleOuterLayer = new Polyline();
                    Polyline rectangleInnerLayer = new Polyline();
                    rectangleOuterLayer.CreateNewRectangle(outerStartPoint, outerEndPoint);
                    rectangleInnerLayer.CreateNewRectangle(innerStartPoint, innerEndPoint, 0, 1, 1);
                    #endregion

                    #region 图表
                    //绘制工程数量表 innerStartPoint
                    Point2d tableInsertPoint = new Point2d(innerStartPoint.X, innerEndPoint.Y);


                    Entity[] NumbEng = createNumberTable("工程数量表", new Point2d(tableInsertPoint.X, tableInsertPoint.Y), fontStyleId);
                    //绘制设备数量表
                    Entity[] NumbEqu = createNumberTable("设备数量表", new Point2d(tableInsertPoint.X, tableInsertPoint.Y - 90), fontStyleId);
                    #endregion

                    #region 添加图签

                    checkBlock(true, fontStyleId); //检查图块
                    ObjectId spaceId = db.CurrentSpaceId;//当期空间ID

                    //块属性的字典对象
                    //图签块
                    Dictionary<string, string> attST = new Dictionary<string, string>();
                    attST.Add("项目名称", "新建吉林至珲春铁路工程");
                    attST.Add("图纸名称", "吉珲施防-01");
                    attST.Add("图纸比例", "1：100");
                    attST.Add("绘制日期", "2013.5");
                    attST.Add("页数", "第1张，共1张");
                    #endregion

                    #region 添加图形
                    //插入图块
                    spaceId.InsertBlockReference("0", "三级图签", new Point3d(innerStartPoint.X + 390, innerStartPoint.Y, 0), new Scale3d(1), 0, attST);
                    db.AddToCurrentSpace(rectangleInnerLayer, rectangleOuterLayer);
                    db.AddToCurrentSpace(NumbEng); //工程数量表
                    db.AddToCurrentSpace(NumbEqu); //设备数量表

                    #endregion

                    //db.SaveAs(this.Text.ToString().Replace(" ", ""), DwgVersion.AC1021);
                    //db.SaveAs(db.Filename, DwgVersion.AC1021);

                    trans.Commit();
                    //saveFile(db, trans);
                    m_DocumentLock.Dispose();

                }
                catch (System.Exception ee)
                {

                    MessageBox.Show("出现错误！" + System.Environment.NewLine + "\t错误信息:" + ee.ToString(), "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trans.Abort();
                }
            }
        }

        //
        //todo
        public Entity[] createNumberTable(string stringTableName, Point2d insertPoint, ObjectId styleId)//Database db,Point2d insertPoint,Dictionary<string, string> ItemNumber)
        {
            Entity[] tableAndname = new Entity[2];

            #region 绘制表名
            DBText tableName = new DBText();
            tableName.Position = new Point3d(insertPoint.X + 52 + 5, insertPoint.Y - 5, 0);
            tableName.Height = 4.5;
            tableName.TextString = stringTableName;

            tableName.HorizontalMode = TextHorizontalMode.TextCenter;
            tableName.VerticalMode = TextVerticalMode.TextVerticalMid;
            tableName.AlignmentPoint = tableName.Position;
            tableName.WidthFactor = 0.7;

            #endregion

            #region 表格
            int tabCol = 6; //暂定

            Table tb = new Table();

            //tb.TableStyle = db.Tablestyle;

            tb.NumRows = 10;

            tb.NumColumns = tabCol;

            tb.SetRowHeight(6);

            /* //cad2008
            tb.SetColumnWidth(15);
             */
            //cad2010
            tb.Columns[0].Width = 10;
            tb.Columns[1].Width = 27;
            tb.Columns[2].Width = 27;
            tb.Columns[3].Width = 13;
            tb.Columns[4].Width = 15;
            tb.Columns[5].Width = 12;


            tb.Position = new Point3d(insertPoint.X + 5, insertPoint.Y - 10, 0);

            // Create a 2-dimensional array

            // of our table contents

            string[,] str = new string[10, 6];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    str[i, j] = " ";
                }
            }
            str[0, 0] = "编号";
            str[0, 1] = "工程名称 ";
            str[0, 2] = "说明";
            str[0, 3] = "单位";
            str[0, 4] = "数量";
            str[0, 5] = "备注";

            str[1, 0] = "1";
            str[2, 0] = "2";
            str[3, 0] = "3";
            str[4, 0] = "4";
            str[5, 0] = "5";
            str[6, 0] = "6";
            str[7, 0] = "7";
            str[8, 0] = "8";

            str[2, 1] = "Bolt";
            str[2, 2] = "SPTYWPL23-16B芯 ";
            str[2, 3] = "m";
            str[2, 4] = "2445";
            str[2, 5] = "";

            str[3, 1] = "Tile";
            str[3, 2] = "SPTYWPL23-16B芯 ";
            str[3, 3] = "m";
            str[3, 4] = "2445";
            str[3, 5] = "";

            str[4, 1] = "Kean";
            str[4, 2] = "SPTYWPL23-16B芯  ";
            str[4, 3] = "m";
            str[4, 4] = "2445";
            str[4, 5] = "";

            str[5, 1] = "Kean";
            str[5, 2] = "SPTYWPL23-16B芯 ";
            str[5, 3] = "m";
            str[5, 4] = "2445";
            str[5, 5] = "";

            // Use a nested loop to add and format each cell
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    /* //CAD2008
                    tb.SetTextHeight(i, j, 3);
                    tb.SetTextString(i, j, str[i, j]);
                    tb.SetAlignment(i, j, CellAlignment.MiddleCenter);
                     * */

                    //CAD2010
                    //文字内容
                    tb.Cells[i, j].TextString = str[i, j];
                    //文字宽度
                    tb.Cells[i, j].TextStyleId = styleId;

                    //文字宽度
                    if (i == 0)
                    {
                        tb.Cells[i, j].TextHeight = 3;
                    }
                    else if (i > 0 || i < 10)
                    {
                        tb.Cells[i, j].TextHeight = 3;
                    }


                }
            }
            tb.GenerateLayout();
            #endregion

            tableAndname[0] = tb;
            tableAndname[1] = tableName;
            return tableAndname;
        }


        //创建字体
        public ObjectId createFont()
        {

            //simsun.ttc 宋体
            Database db = HostApplicationServices.WorkingDatabase;

            ObjectId styleId;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                styleId = db.AddTextStyle("宋-0.7", "宋体.ttf");
                //styleId = db.AddTextStyle("宋体-0.7", "simsun.ttc");
                //styleId = db.AddTextStyle("宋体-0.7", "simsun.ttc", false, false, 134, 2 | 0);
                styleId.SetTextStyleProp(3, 0.7, 0, false, false, false, AnnotativeStates.True, true);
                trans.Commit();
            }
            return styleId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="withoutScale">是否不显示比例尺数值</param>
        public void checkBlock(bool withoutScale, ObjectId fontId)//Point2d innerStartPoint,
        {
            string[] t = new string[1];
            MakeSignatureTable("三级图签", withoutScale, fontId);
        }

        /// <summary>
        /// 生成三级或四级签名块
        /// </summary>
        /// <param name="insertPoint">设置插入点，插入点为图框右下角</param>
        /// <param name="nameTable">签名块设置</param>
        /// <returns></returns>
        public Entity[] MakeSignatureTable(string nameTable, bool withoutScale, ObjectId fontId)
        {
            #region 绘制签名图签
            //Point2d tqDownRight = new Point2d(insertPoint.X, insertPoint.Y);//图签右下
            Point2d tqDownRight = new Point2d(0, 0);//图签右下
            Point2d tqUpLeft = new Point2d(tqDownRight.X - 180, tqDownRight.Y + 37.44);//图签左上

            //图签外框
            Polyline rectangleSign = new Polyline();
            rectangleSign.CreateNewRectangle(tqDownRight, tqUpLeft, 0, 1, 1);

            //绘制图框内部线条
            Point2d Vline1A = new Point2d(tqUpLeft.X, tqUpLeft.Y - 10.44);
            Point2d Vline1B = new Point2d(tqUpLeft.X + 180, tqUpLeft.Y - 10.44);
            Polyline Vline1 = new Polyline();
            Vline1.CreatePolyline(Vline1A, Vline1B);

            Point2d Hline1A = new Point2d(tqUpLeft.X + 70, tqUpLeft.Y);
            Point2d Hline1B = new Point2d(tqUpLeft.X + 70, tqUpLeft.Y - 10.44);
            Polyline Hline1 = new Polyline();
            Hline1.CreatePolyline(Hline1A, Hline1B);

            Point2d Hline2A = new Point2d(tqUpLeft.X + 90, tqUpLeft.Y);
            Point2d Hline2B = new Point2d(tqUpLeft.X + 90, tqUpLeft.Y - 10.44);
            Polyline Hline2 = new Polyline();
            Hline2.CreatePolyline(Hline2A, Hline2B);

            Polyline Vline2 = new Polyline();
            Point2d Vline2A = new Point2d();
            Point2d Vline2B = new Point2d();

            Polyline Vline3 = new Polyline();
            Point2d Vline3A = new Point2d();
            Point2d Vline3B = new Point2d();

            Polyline Vline7 = new Polyline();
            Point2d Vline7A = new Point2d();
            Point2d Vline7B = new Point2d();

            if (nameTable == "三级图签")
            {
                Vline2A = new Point2d(tqUpLeft.X, tqUpLeft.Y - 19.44);
                Vline2B = new Point2d(tqUpLeft.X + 50, tqUpLeft.Y - 19.44);
                Vline2.CreatePolyline(Vline2A, Vline2B);

                Vline3A = new Point2d(tqUpLeft.X, tqUpLeft.Y - 19.44 - 9);
                Vline3B = new Point2d(tqUpLeft.X + 50, tqUpLeft.Y - 19.44 - 9);
                Vline3.CreatePolyline(Vline3A, Vline3B);
            }
            else if (nameTable == "四级图签")
            {
                Vline2A = new Point2d(tqUpLeft.X, tqUpLeft.Y - 16.44);
                Vline2B = new Point2d(tqUpLeft.X + 50, tqUpLeft.Y - 16.44);
                Vline2.CreatePolyline(Vline2A, Vline2B);

                Vline3A = new Point2d(tqUpLeft.X, tqUpLeft.Y - 16.44 - 7);
                Vline3B = new Point2d(tqUpLeft.X + 50, tqUpLeft.Y - 16.44 - 7);
                Vline3.CreatePolyline(Vline3A, Vline3B);

                Vline7A = new Point2d(tqUpLeft.X, tqUpLeft.Y - 16.44 - 14);
                Vline7B = new Point2d(tqUpLeft.X + 50, tqUpLeft.Y - 16.44 - 14);
                Vline7.CreatePolyline(Vline7A, Vline7B);
            }

            Point2d Hline3A = new Point2d(tqUpLeft.X + 25, tqUpLeft.Y - 10.44);
            Point2d Hline3B = new Point2d(tqUpLeft.X + 25, tqDownRight.Y);
            Polyline Hline3 = new Polyline();
            Hline3.CreatePolyline(Hline3A, Hline3B);

            Point2d Hline4A = new Point2d(tqUpLeft.X + 50, tqUpLeft.Y - 10.44);
            Point2d Hline4B = new Point2d(tqUpLeft.X + 50, tqDownRight.Y);
            Polyline Hline4 = new Polyline();
            Hline4.CreatePolyline(Hline4A, Hline4B);

            Point2d Hline5A = new Point2d(tqDownRight.X - 50, tqUpLeft.Y - 10.44);
            Point2d Hline5B = new Point2d(tqDownRight.X - 50, tqDownRight.Y);
            Polyline Hline5 = new Polyline();
            Hline5.CreatePolyline(Hline5A, Hline5B);

            Point2d Hline6A = new Point2d(tqDownRight.X - 35, tqUpLeft.Y - 10.44);
            Point2d Hline6B = new Point2d(tqDownRight.X - 35, tqDownRight.Y + 7);
            Polyline Hline6 = new Polyline();
            Hline6.CreatePolyline(Hline6A, Hline6B);

            Point2d Vline4A = new Point2d(tqDownRight.X - 50, tqUpLeft.Y - 10.44 - 6);
            Point2d Vline4B = new Point2d(tqDownRight.X, tqUpLeft.Y - 10.44 - 6);
            Polyline Vline4 = new Polyline();
            Vline4.CreatePolyline(Vline4A, Vline4B);

            Point2d Vline5A = new Point2d(tqDownRight.X - 50, tqUpLeft.Y - 10.44 - 6 - 7);
            Point2d Vline5B = new Point2d(tqDownRight.X, tqUpLeft.Y - 10.44 - 6 - 7);
            Polyline Vline5 = new Polyline();
            Vline5.CreatePolyline(Vline5A, Vline5B);

            Point2d Vline6A = new Point2d(tqDownRight.X - 50, tqUpLeft.Y - 10.44 - 6 - 7 - 7);
            Point2d Vline6B = new Point2d(tqDownRight.X, tqUpLeft.Y - 10.44 - 6 - 7 - 7);
            Polyline Vline6 = new Polyline();
            Vline6.CreatePolyline(Vline6A, Vline6B);

            //ObjectId styleId = db.AddTextStyle("宋体-0.7", "simsun.ttc");
            //styleId.SetTextStyleProp(3, 0.7, 0, false, false, false, AnnotativeStates.True, true);
            DBText text1 = new DBText();
            text1.Position = new Point3d(tqUpLeft.X + 35, tqUpLeft.Y - 5.22, 0);
            text1.Height = 3;
            text1.TextString = "中铁工程设计咨询集团有限公司";
            //text1.
            text1.HorizontalMode = TextHorizontalMode.TextCenter;
            text1.VerticalMode = TextVerticalMode.TextVerticalMid;
            text1.AlignmentPoint = text1.Position;
            text1.TextStyleId = fontId;

            DBText text2 = new DBText();
            text2.Position = new Point3d(tqUpLeft.X + 80, tqUpLeft.Y - 5.22, 0);
            text2.Height = 3;
            text2.TextString = "工程名称";
            //text1.
            text2.HorizontalMode = TextHorizontalMode.TextCenter;
            text2.VerticalMode = TextVerticalMode.TextVerticalMid;
            text2.AlignmentPoint = text2.Position;
            text2.TextStyleId = fontId;

            DBText text3 = new DBText();
            text3.Position = new Point3d(tqDownRight.X - 35 - 7.5, tqDownRight.Y + 24, 0);
            text3.Height = 3;
            text3.TextString = "图号";
            //text1.
            text3.HorizontalMode = TextHorizontalMode.TextCenter;
            text3.VerticalMode = TextVerticalMode.TextVerticalMid;
            text3.AlignmentPoint = text3.Position;
            text3.TextStyleId = fontId;

            DBText text4 = new DBText();
            text4.Position = new Point3d(tqDownRight.X - 35 - 7.5, tqDownRight.Y + 17.5, 0);
            text4.Height = 3;
            text4.TextString = "比例尺";
            //text1.
            text4.HorizontalMode = TextHorizontalMode.TextCenter;
            text4.VerticalMode = TextVerticalMode.TextVerticalMid;
            text4.AlignmentPoint = text4.Position;
            text4.TextStyleId = fontId;

            DBText text5 = new DBText();
            text5.Position = new Point3d(tqDownRight.X - 35 - 7.5, tqDownRight.Y + 10.5, 0);
            text5.Height = 3;
            text5.TextString = "日期";
            //text1.
            text5.HorizontalMode = TextHorizontalMode.TextCenter;
            text5.VerticalMode = TextVerticalMode.TextVerticalMid;
            text5.AlignmentPoint = text5.Position;
            text5.TextStyleId = fontId;

            DBText text6 = new DBText();
            if (nameTable == "三级图签")
            {
                text6.Position = new Point3d(tqUpLeft.X + 12.5, tqUpLeft.Y - 14.94, 0);
            }
            else if (nameTable == "四级图签")
            {
                text6.Position = new Point3d(tqUpLeft.X + 12.5, tqUpLeft.Y - 13.44, 0);
            }
            text6.Height = 3;
            text6.TextString = "设计";
            //text1.
            text6.HorizontalMode = TextHorizontalMode.TextCenter;
            text6.VerticalMode = TextVerticalMode.TextVerticalMid;
            text6.AlignmentPoint = text6.Position;
            text6.TextStyleId = fontId;

            DBText text7 = new DBText();
            if (nameTable == "三级图签")
            {
                text7.Position = new Point3d(tqUpLeft.X + 12.5, tqUpLeft.Y - 23.94, 0);
            }
            else if (nameTable == "四级图签")
            {
                text7.Position = new Point3d(tqUpLeft.X + 12.5, tqUpLeft.Y - 19.94, 0);
            }
            text7.Height = 3;
            text7.TextString = "复核";
            //text1.
            text7.HorizontalMode = TextHorizontalMode.TextCenter;
            text7.VerticalMode = TextVerticalMode.TextVerticalMid;
            text7.AlignmentPoint = text7.Position;
            text7.TextStyleId = fontId;



            DBText text8 = new DBText();
            if (nameTable == "三级图签")
            {
                text8.Position = new Point3d(tqUpLeft.X + 12.5, tqUpLeft.Y - 32.94, 0);
            }
            else if (nameTable == "四级图签")
            {
                text8.Position = new Point3d(tqUpLeft.X + 12.5, tqUpLeft.Y - 26.94, 0);
            }
            text8.Height = 3;
            text8.TextString = "专业审核";
            //text1.
            text8.HorizontalMode = TextHorizontalMode.TextCenter;
            text8.VerticalMode = TextVerticalMode.TextVerticalMid;
            text8.AlignmentPoint = text8.Position;
            text8.TextStyleId = fontId;


            DBText text9 = new DBText();
            if (nameTable == "四级图签")
            {
                text9.Position = new Point3d(tqUpLeft.X + 12.5, tqUpLeft.Y - 33.94, 0);
                text9.Height = 3;
                text9.TextString = "院审核";
                //text1.
                text9.HorizontalMode = TextHorizontalMode.TextCenter;
                text9.VerticalMode = TextVerticalMode.TextVerticalMid;
                text9.AlignmentPoint = text9.Position;

            }
            text9.TextStyleId = fontId;


            Entity[] TqLines = new Entity[23];
            ArrayList a = new ArrayList();
            TqLines[0] = rectangleSign;
            TqLines[1] = Hline1;
            TqLines[2] = Hline2;
            TqLines[3] = Hline3;
            TqLines[4] = Hline4;
            TqLines[5] = Hline5;
            TqLines[6] = Hline6;

            TqLines[7] = Vline1;
            TqLines[8] = Vline2;
            TqLines[9] = Vline3;
            TqLines[10] = Vline4;
            TqLines[11] = Vline5;
            TqLines[12] = Vline6;
            TqLines[13] = Vline7;

            TqLines[14] = text1;
            TqLines[15] = text2;
            TqLines[16] = text3;
            TqLines[17] = text4;
            TqLines[18] = text5;
            TqLines[19] = text6;
            TqLines[20] = text7;
            TqLines[21] = text8;
            TqLines[22] = text9;

            MakeTableRecode(nameTable, TqLines, withoutScale, fontId);
            #endregion

            return TqLines;
        }

        public ObjectId MakeTableRecode(string blockName, Entity[] entitys, bool withoutScale, ObjectId fontId)
        {
            ObjectId blockId = new ObjectId();//块参照ID

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    if (!bt.Has(blockName))
                    {
                        if (withoutScale)
                        {
                            Point2d LScaleA = new Point2d(-35, 14);
                            Point2d LScaleB = new Point2d(0, 21);
                            Polyline LScale = new Polyline();
                            LScale.CreatePolyline(LScaleA, LScaleB);

                            Entity[] tmpEntitys = new Entity[entitys.Length + 1];
                            for (int i = 0; i < entitys.Length; i++)
                            {
                                tmpEntitys[i] = entitys[i];
                            }
                            tmpEntitys[entitys.Length] = LScale;
                            blockId = db.AddBlockTableRecord(blockName, tmpEntitys);
                        }
                        else
                        {
                            blockId = db.AddBlockTableRecord(blockName, entitys);
                        }
                        AttributeDefinition[] attD = addAttSignaTable(withoutScale, fontId);
                        blockId.AddAttsToBlock(attD);
                        trans.Commit();
                    }
                    else
                    {
                        toolStripStatusLabel1.Text = ("块" + blockName + "已存在.");
                    }

                }
                catch (System.Exception ex)
                {

                    MessageBox.Show("发生错误!" + System.Environment.NewLine + "\t错误信息:" + ex.ToString(), "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trans.Abort();
                }
            }
            return blockId;
        }

        /// <summary>
        /// 添加属性快
        /// </summary>
        /// <param name="invisibilityScale">是否显示比例尺可视性，true时不显示；false时显示</param>
        /// <returns>返回块属性</returns>
        public AttributeDefinition[] addAttSignaTable(bool invisibilityScale, ObjectId fontId)
        {
            /*Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //AttributeDefinition 
            }*/

            AttributeDefinition[] attD = new AttributeDefinition[5];

            //表示门符号的属性定义
            AttributeDefinition attProjetName = new AttributeDefinition(Point3d.Origin, "xx铁路", "项目名称", "输入项目名称：", fontId);
            attProjetName.TextStyleId = fontId;
            //表示属性定义的通用样式
            setStyleForAtt(attProjetName, 3, false);
            //对其点
            attProjetName.AlignmentPoint = new Point3d(-45, 32.22, 0);
            

            //表示门符号的属性定义
            AttributeDefinition attDrawName = new AttributeDefinition(Point3d.Origin, "xxx防-xx-xx", "图纸名称", "输入图纸名称：", fontId);
            //表示属性定义的通用样式
            setStyleForAtt(attDrawName, 3, false);
            //对其点
            attDrawName.AlignmentPoint = new Point3d(-17.5, 24, 0);

            //表示门符号的属性定义
            AttributeDefinition attScale = new AttributeDefinition(Point3d.Origin, "1：100", "图纸比例", "输入图纸比例：", fontId);
            //表示属性定义的通用样式
            setStyleForAtt(attScale, 3, invisibilityScale);
            //对其点
            attScale.AlignmentPoint = new Point3d(-17.5, 17.5, 0);

            //表示门符号的属性定义
            AttributeDefinition attYearMonth = new AttributeDefinition(Point3d.Origin, "xxxx.xx", "绘制日期", "输入图纸绘制日期：", fontId);
            //表示属性定义的通用样式
            setStyleForAtt(attYearMonth, 3, false);
            //对其点
            attYearMonth.AlignmentPoint = new Point3d(-17.5, 10.5, 0);

            //表示门符号的属性定义
            AttributeDefinition attPage = new AttributeDefinition(Point3d.Origin, "第 x 张,共 x 张", "页数", "输入图纸页数：", fontId);
            //表示属性定义的通用样式
            setStyleForAtt(attPage, 3, false);
            //对其点
            attPage.AlignmentPoint = new Point3d(-25, 3.5, 0);

            attD[0] = attProjetName;
            attD[1] = attDrawName;
            attD[2] = attYearMonth;
            attD[3] = attPage;
            attD[4] = attScale;

            return attD;
        }
        private void setStyleForAtt(AttributeDefinition att, double height, bool invisible)
        {
            att.Height = height;//文字高度
            att.HorizontalMode = TextHorizontalMode.TextCenter;
            att.VerticalMode = TextVerticalMode.TextVerticalMid;
            att.Invisible = invisible;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExplodingABlock();


        }


        private void 图签名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("功能未完成", "注意", MessageBoxButtons.OK) != DialogResult.OK)
            {
                System.Resources.ResourceWriter rw = new ResourceWriter(@"..\..\abc.txt");
                rw.AddResource("abc", new byte[10000000]);
                rw.Generate();
                rw.Close();

                string TuQianFilePath = "";
                TuQian tQ = new TuQian(TuQianFilePath);
                tQ.Owner = this;
                tQ.Show();
            }
        }

        private void 比例尺ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能未完成", "注意", MessageBoxButtons.OK);
        }

        //用处不大
        /*
        public void saveFile(Database db, Transaction trans)
        {
            //创建新数据库并储存

            PromptSaveFileOptions opt = new PromptSaveFileOptions("请选择文件储存位置。");
            opt.Filter = "图形(*.dwg)|*.dwg|图形(*.dxf)|*.dxf";
            opt.FilterIndex = 0; //默认选项
            opt.DialogCaption = "项目另存为";
            opt.InitialDirectory = @"c:\";//默认位置
            opt.InitialFileName = "防灾";
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptFileNameResult result = ed.GetFileNameForSave(opt);
            if (result.Status != PromptStatus.OK)
            {
                MessageBox.Show("请选择文件储存位置！", "注意！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; //如果未选择则退出
            }

            this.Text = result.StringResult; //文件名
                                             //DwgVersion.AC1021为2008版本
            db.SaveAs(result.StringResult, DwgVersion.AC1021);//保存为当前版本
            //db.Save();

        }
        */


        //string filePath;
        private void MainInterface_Load(object sender, EventArgs e)
        {
            try
            {
                //添加eventHandler监控文件状态更改（保存，另存为）
                //Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.DocumentLockModeChanged += new DocumentLockModeChangedEventHandler(callback_DocumentManager_DocumentLockModeChanged);

                //添加commandEnded事件,当每个命令完成后触发
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.CommandEnded += new CommandEventHandler(doc_CommandEnded);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ee)
            {
                MessageBox.Show("发生错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           

            //toolStripStatusLabel1.Text = getFilePath();
            //combobox添加项
            //TypeWayPoint.ad

            string a= getFilePath();
            if (getFilePath().ToLower().Contains("template"))
            {
                toolStripStatusLabel1.Text = "未保存的文件.";
            }
            else
            {
                filePath = a; ;  //文件位置
                xmlFilePath = a + "\\setting";       //xml文件位置
                imgStoragePath = a + "\\icon"; //图像文件位置 imgPath
            }

            
        }

        //当文件状态将改变时触发，废弃。
        private void callback_DocumentManager_DocumentLockModeChanged(object sender, DocumentLockModeChangedEventArgs e)
        {
            if (e.GlobalCommandName == "QSAVE" || e.GlobalCommandName == "SAVE" || e.GlobalCommandName == "SAVEAS")
            {
                // do you code

                toolStripStatusLabel1.Text = getFilePath();

            }
        }
        
        /// <summary>
        /// 当保存完文件以后触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void doc_CommandEnded(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "QSAVE" || e.GlobalCommandName == "SAVE" || e.GlobalCommandName == "SAVEAS")
            {
                newFilePath = getFilePath();

                toolStripStatusLabel1.Text = newFilePath;

                //当第一次运行切找不到setting文件时
                if (filePath == "" || filePath == null)
                {
                    filePath = newFilePath;
                }
                else
                {
                    //调用库位置变更功能
                    moveDictionary(newFilePath);
                    //设置新库位置
                    filePath = newFilePath;

                }
                if (filePath != newFilePath)
                {
                    //文件位置
                    
                }
                
                Document doc = sender as Document;
                if (!doc.Editor.IsQuiescent) return;
                else { MessageBox.Show("doc IsQuiescent?"); }

                //DoSomeWork();
            }
        }

        /// <summary>
        /// 当图纸换边储存位置时，将setting文件及icon文件移动至新文件夹
        /// </summary>
        /// <param name="newFilePath"></param>
        private void moveDictionary(string newFilePath)
        {
            //todo
           
        }
    
        /// <summary>
        /// 获得当前文件储存位置
        /// </summary>
        /// <returns>文件位置</returns>
        private string getFilePath()
        {
            string p;

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = acDoc.Database;
            string strDWGName = acDoc.Name;
            object obj = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("DWGTITLED");
            // Check to see if the drawing has been named
            if (System.Convert.ToInt16(obj) == 0) //未保存过
            {
                p = db.Filename;
            }
            else if (System.Convert.ToInt16(obj) != 1) //保存过的
            {
                p = db.Filename;
            }

            p = db.Filename;

            return p;
        }

        //string icon 


        /// <summary>
        /// 写入xml信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_Add_Click(object sender, EventArgs e)
        {

            string CadFilePath = getFilePath();
            string[] Names = CadFilePath.Split(new char[1] { '\\' });
            string name = Names[Names.Length - 1];
            //写XML文件
            if (name != "acadiso.dwt")
            {
                xmlFilePath = "c://defaut.xml";
                if (!File.Exists(xmlFilePath))
                {
                    createXml(xmlFilePath);
                }
            }
            else
            {
                MessageBox.Show("请先保存文件.");
            }

            //string a = getFilePath();
            //toolStripStatusLabel1.Text = a.ToString();
        }

        private void 导入图块ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Multiselect = false;
                fileDialog.Title = "请选择文件.";
                fileDialog.Filter = "cad|*.dwg|所有文件(*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string importFilePath = fileDialog.FileName;

                    //从目标文件导入图块
                    GetBlocksFromDwgs(importFilePath, imgStoragePath);
                    //autoFitBlock();
                }

            }
        }

        /// <summary>
        /// 从目标文件导入图块
        /// </summary>
        /// <param name="openFilePath">目标文件</param>
        public void GetBlocksFromDwgs(string openFilePath,string imgPath)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId spaceId = db.CurrentSpaceId;//获取当前空间(模型空间或图纸空间)
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            // 提示用户选择文件
            //PromptFileNameResult result = ed.GetFileNameForOpen("请选择需要预览的文件");
            //if (result.Status != PromptStatus.OK) return; // 如果未选择，则返回
            //string filename = result.StringResult; // 获取带有路径的文件名
            string filename = openFilePath;
            // 在C盘根目录下创建一个临时文件夹，用来存放文件中的块预览图标
            string path = string.Empty;
#if DEBUG

            path = imgPath;//  StockLocation;

            //string b=
#else
            path = "..\\Resourse\\";
#endif

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

                    // 导入外部文件中的块
                    db.ImportBlocksFromDwg(filename);
                    //打开块表
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    // 循环遍历块表中的块表记录
                    //int i=0;
                    foreach (ObjectId blockTableId in bt)
                    {
                        // 打开 块表 记录对象
                        BlockTableRecord btRecord = (BlockTableRecord)trans.GetObject(blockTableId, OpenMode.ForRead);
                        // 如果是匿名块、布局块及没有预览图形的块，则返回
                        if (btRecord.IsAnonymous || btRecord.IsLayout || !btRecord.HasPreviewIcon) continue;

                        //统一大小
                       
                        Bitmap preview;
                        try
                        {
                            StringBuilder str = new StringBuilder();
                            if (!btRecord.IsDynamicBlock)
                            {
                                str.Append("Dynamique");
                            }

                            if (btRecord.ExtensionDictionary == null)
                            {
                                str.Append("Extension");
                            }

                            // 获取块预览图案（适用于AutoCAD 2008及以下版本）
                            //preview = BlockThumbnailHelper.GetBlockThumbanail(btr.ObjectId);

                            preview = btRecord.PreviewIcon; // 适用于AutoCAD 2009及以上版本 
                            preview.Save(path + "\\" + btRecord.Name + "_" + str.ToString() + ".bmp"); // 保存块预览图案
                            //trans.Commit();
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ee)
                        {
                            trans.Abort();
                            ed.WriteMessage("错误;  " + ee.ToString());
                            //preview = btr.PreviewIcon; // 适用于AutoCAD 2009及以上版本
                        }


                    }
                    m_DocumentLock.Dispose();
                }


                catch (System.Exception ee)
                {
                    trans.Abort();
                    ed.WriteMessage("错误;  " + ee.ToString());
                    //preview = btr.PreviewIcon; // 适用于AutoCAD 2009及以上版本
                }
            }
        }

        public void autoFitBlock()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            ObjectId spaceId = db.CurrentSpaceId;//当前空间（模型空间或图纸空间）的Id
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                   //blockTable 中每一项
                    foreach (ObjectId blockTableId in bt)
                    {
                        int count = 0;
                        double minX = new double();
                        double minY = new double();

                        double maxX = new double();
                        double maxY = new double();


                        // 打开 块表 记录对象
                        BlockTableRecord blockTR = (BlockTableRecord)trans.GetObject(blockTableId, OpenMode.ForRead);

                        blockTR.UpgradeOpen();

                        ObjectIdCollection EntityIdInOldBlock = new ObjectIdCollection();

                        DateTime dt = DateTime.Now;
                        string tmp = dt.ToString();
                        ed.WriteMessage(tmp + ":块:" + blockTR.Name + "\n");
                        //块中每一个实体

                        if (blockTR.Name.ToString() == "*Model_Space") continue;
                        else if (blockTR.Name.ToString() == "*Paper_Space") continue;
                        else if (blockTR.Name.ToString() == "*Paper_Space0") continue;
                        else if (blockTR.Name.ToString() == "*E991") continue;
                        foreach (ObjectId entId in blockTR)
                        {
                            EntityIdInOldBlock.Add(entId);

                            DBObject objSubBlock = (DBObject)trans.GetObject(entId, OpenMode.ForWrite);
                            Entity acEnt = objSubBlock as Entity;
                            string entityType = (string)acEnt.GetType().ToString();

                            ed.WriteMessage("计数:" + count + "实体类型：" + entityType + "\n" + "ID:" + entId.ToString() + "\n");
                            if (acEnt.GetType().ToString().ToLower() == "autodesk.autocad.databaseservices.dbtext")
                            {
                                //DBText dT = acEnt as DBText;
                                //ed.WriteMessage("计数:" + count + "实体类型：" + entityType + "文字：" + dT.ToString() + "\n");
                                continue;
                            }
                            else if (acEnt.GetType().ToString().ToLower() == "autodesk.autocad.databaseservices.blockreference")
                            {
                                continue;
                            }
                            else if (acEnt.GetType().ToString().ToLower() == "autodesk.autocad.databaseservices.attributedefinition")
                            {
                                continue;
                            }

                            //更新块的最大最小值
                            if (count == 0 || minX > acEnt.GeometricExtents.MinPoint.X)
                            {
                                minX = acEnt.GeometricExtents.MinPoint.X;
                            }
                            if (count == 0 || minY > acEnt.GeometricExtents.MinPoint.Y)
                            {
                                minY = acEnt.GeometricExtents.MinPoint.Y;
                            }

                            if (count == 0 || maxX < acEnt.GeometricExtents.MaxPoint.X)
                            {
                                maxX = acEnt.GeometricExtents.MaxPoint.X;
                            }
                            if (count == 0 || maxY < acEnt.GeometricExtents.MaxPoint.Y)
                            {
                                maxY = acEnt.GeometricExtents.MaxPoint.Y;
                            }
                            count++;
                        }

                        //deepcloneObject
                        //make model space as owner for new entity 
                        ObjectId ModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(db);
                        IdMapping mapping = new IdMapping();
                        db.DeepCloneObjects(EntityIdInOldBlock, ModelSpaceId, mapping, false);
                        DBObjectCollection EntityInNewBlock = new DBObjectCollection();

                        foreach (ObjectId item in EntityIdInOldBlock)
                        {                            
                            //get the map.
                            IdPair pair1 = mapping[item];

                            //new object
                            Entity ent = trans.GetObject(pair1.Value, OpenMode.ForWrite) as Entity;
                            EntityInNewBlock.Add(ent);
                            ed.WriteMessage("New ID:" + pair1.Value.ToString() + "\n");
                        }

                        //便利所有实体后获得'max' 'min' 两个点，并绘制extents框
                        Polyline rectangleExtents = new Polyline();
                        rectangleExtents.CreateNewRectangle(new Point2d(minX, minY), new Point2d(maxX, maxY));

                        //EntityInNewBlock.Add(rectangleExtents);
                        /*
                        ObjectId a = blockTR.Id;

                        //acBlkRef.Explode(dbObjCol);
*/
                        List<Entity> entityCollection = new List<Entity>();
                        foreach (Object objEntity in EntityInNewBlock)
                        {
                            entityCollection.Add(objEntity as Entity);
                        }
                        //blockTR.AppendEntity(rectangleExtents);
                        //新建块
                        if (entityCollection.Count != 0)
                        {
                            if (true)
                            {
                                string entityNewName = blockTR.Name + "_ZTZX";
                                db.AddBlockTableRecord(entityNewName, entityCollection); 
                            }

                        }
                    }
                    trans.Commit();

                    m_DocumentLock.Dispose();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ee)
                {
                    trans.Abort();
                    MessageBox.Show("错误;  " + ee.ToString());
                    //preview = btr.PreviewIcon; // 适用于AutoCAD 2009及以上版本
                }
            }


        }

        public void ExplodingABlock()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                ObjectId blkRecId = ObjectId.Null;

                if (!acBlkTbl.Has("CircleBlock"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlock";

                        // Set the insertion point for the block
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(0, 0, 0);
                            acCirc.Radius = 2;

                            DBText mt = new DBText();
                            mt.TextString = "11";
                            mt.Position = Point3d.Origin;
                            mt.Height = 3;

                            acBlkTblRec.AppendEntity(acCirc);
                            acBlkTblRec.AppendEntity(mt);

                            acBlkTbl.UpgradeOpen();
                            acBlkTbl.Add(acBlkTblRec);
                            acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                        }

                        blkRecId = acBlkTblRec.Id;
                    }
                }
                else
                {
                    blkRecId = acBlkTbl["CircleBlock"];
                }

                // Insert the block into the current space
                if (blkRecId != ObjectId.Null)
                {
                    using (BlockReference acBlkRef = new BlockReference(new Point3d(0, 0, 0), blkRecId))
                    {
                        BlockTableRecord acCurSpaceBlkTblRec;
                        acCurSpaceBlkTblRec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                        //acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
                        //acTrans.AddNewlyCreatedDBObject(acBlkRef, true);
                        StringBuilder sbb = new StringBuilder();
                        using (DBObjectCollection dbObjCol = new DBObjectCollection())
                        {
                            acBlkRef.Explode(dbObjCol);

                            foreach (DBObject dbObj in dbObjCol)
                            {
                                Entity acEnt = dbObj as Entity;

                                int c = acEnt.ColorIndex;
                                acEnt.ColorIndex = c - 1;
                                //Point3d p = acEnt.Bounds.Value.MaxPoint;
                                //Point3d p1 = acEnt.Bounds.Value.MaxPoint;
                                //sbb.Append("x:" + p.X + ", y:" + p.Y + " ,z:" + p.Z);
                                //sbb.Append("x:" + p1.X + ", y:" + p1.Y + " ,z:" + p1.Z);

                                acCurSpaceBlkTblRec.AppendEntity(acEnt);
                                acTrans.AddNewlyCreatedDBObject(dbObj, true);

                                acEnt = acTrans.GetObject(dbObj.ObjectId, OpenMode.ForWrite) as Entity;


                                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Exploded Object: " + acEnt.GetRXClass().DxfName);
                            }
                        }

                    }
                }
                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction            
                m_DocumentLock.Dispose();
            }

        }

        //再次尝试
        private void button1_Click(object sender, EventArgs e)
        {
             Database db = HostApplicationServices.WorkingDatabase;
            ObjectId spaceId = db.CurrentSpaceId;//当前空间（模型空间或图纸空间）的Id
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);

                    List<Entity> le = new List<Entity>();
                    //blockTable 中每一项
                    foreach (ObjectId blockTableId in bt)
                    {
                        int count = 0;
                        double minX = new double();
                        double minY = new double();

                        double maxX = new double();
                        double maxY = new double();


                        // 打开 块表 记录对象
                        BlockTableRecord blockTR = (BlockTableRecord)trans.GetObject(blockTableId, OpenMode.ForRead);
                        blockTR.UpgradeOpen();

                        PromptPointOptions prPointOptions = new PromptPointOptions("Select a point");
                        PromptPointResult prPointRes;
                        prPointRes = ed.GetPoint(prPointOptions);
                        if (prPointRes.Status != PromptStatus.OK)
                        {
                            ed.WriteMessage("Error");
                            continue;
                        }
                        Point3d p3 = prPointRes.Value;
                        
                        ObjectIdCollection EntityIdInOldBlock = new ObjectIdCollection();

                        DateTime dt = DateTime.Now;
                        string tmp = dt.ToString();
                        ed.WriteMessage(tmp + ":块:" + blockTR.Name + "\n");
                        //块中每一个实体

                        if (blockTR.Name.ToString() == "*Model_Space") continue;
                        else if (blockTR.Name.ToString() == "*Paper_Space") continue;
                        else if (blockTR.Name.ToString() == "*Paper_Space0") continue;
                        else if (blockTR.Name.ToString() == "*E991") continue;

                        spaceId.InsertBlockReference("0", blockTR.Name, prPointRes.Value, new Scale3d(1), 0);
                        foreach (ObjectId entId in blockTR)
                        {
                            EntityIdInOldBlock.Add(entId);

                            DBObject objSubBlock = (DBObject)trans.GetObject(entId, OpenMode.ForWrite);
                            Entity acEnt = objSubBlock as Entity;
                            string entityType = (string)acEnt.GetType().ToString();

                            ed.WriteMessage("计数:" + count + "实体类型：" + entityType + "\n" + "ID:" + entId.ToString() + "\n");
                            if (acEnt.GetType().ToString().ToLower() == "autodesk.autocad.databaseservices.dbtext")
                            {
                                //DBText dT = acEnt as DBText;
                                //ed.WriteMessage("计数:" + count + "实体类型：" + entityType + "文字：" + dT.ToString() + "\n");
                                continue;
                            }
                            else if (acEnt.GetType().ToString().ToLower() == "autodesk.autocad.databaseservices.blockreference")
                            {
                                continue;
                            }
                            else if (acEnt.GetType().ToString().ToLower() == "autodesk.autocad.databaseservices.attributedefinition")
                            {
                                continue;
                            }

                            //更新块的最大最小值
                            if (count == 0 || minX > acEnt.GeometricExtents.MinPoint.X)
                            {
                                minX = acEnt.GeometricExtents.MinPoint.X;
                            }
                            if (count == 0 || minY > acEnt.GeometricExtents.MinPoint.Y)
                            {
                                minY = acEnt.GeometricExtents.MinPoint.Y;
                            }

                            if (count == 0 || maxX < acEnt.GeometricExtents.MaxPoint.X)
                            {
                                maxX = acEnt.GeometricExtents.MaxPoint.X;
                            }
                            if (count == 0 || maxY < acEnt.GeometricExtents.MaxPoint.Y)
                            {
                                maxY = acEnt.GeometricExtents.MaxPoint.Y;
                            }
                            count++;
                        }
                        Polyline rectangleExtents = new Polyline();
                        rectangleExtents.CreateNewRectangle(new Point2d(minX + p3.X, minY + p3.Y), new Point2d(maxX, maxY));

                        
                        le.Add(rectangleExtents);
                        
                    }

                    Entity[] arrE=new Entity[le.Count];
                    int i=0;
                    foreach (Entity ent in le)
                    {
                        arrE[i] = ent;
                        i++;
                    }
                    db.AddToCurrentSpace(arrE);
                    trans.Commit();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ee)
                {
                    trans.Abort();
                    MessageBox.Show("错误;  " + ee.ToString());
                    //preview = btr.PreviewIcon; // 适用于AutoCAD 2009及以上版本
                }
            }

        }

        /// <summary>
        /// 添加站点信息
        /// </summary>
        Dictionary<string, string> InfoStation = new Dictionary<string, string>();
        private void B_AddWayPoint_Click(object sender, EventArgs e)
        {
            if (modifData == false)
            {
                #region 添加字典信息
                if (TypeWayPoint.SelectedItem.ToString() != "" && T_SLocation.Text != "" && T_SName.ToString() != "")
                {
                    string sLocation = "";
                    string sName = T_SName.Text.ToString();
                    string sType = TypeWayPoint.SelectedItem.ToString();

                    if (sType == "桥梁")
                    {
                        string locationPart2 = "";
                        foreach (var component in splitContainer1.Panel1.Controls)
                        {
                            TextBox temp = component as TextBox;

                            if (temp != null)
                            {
                                if (temp.Name == "tempText")
                                {
                                    locationPart2 = temp.Text.ToString();
                                }
                            }
                        }
                        sLocation = T_SLocation.Text.ToString().ToUpper() + "-" + locationPart2;
                    }
                    else
                    {
                        sLocation = T_SLocation.Text.ToString().ToUpper();
                    }
                    //添加字典项
                    if (!InfoStation.ContainsKey(sLocation))
                    {
                        InfoStation.Add(sLocation, sName + "," + sType);
                        treeView1.Nodes.Clear();
                        refreshTreeview(true);
                    }
                    else
                    {
                        MessageBox.Show("已定义该站.");
                    }
                }
                else
                {
                    MessageBox.Show("请录入所有信息.", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                #endregion
            }
            else //变更dictionary中数据
            {
                #region 修改字典信息

                TreeNode selectedNode = treeView1.SelectedNode;

                TreeNode keyLNode = new TreeNode(); //里程
                TreeNode nameNode = new TreeNode(); //站名
                TreeNode typeNode = new TreeNode(); //类型

                if (selectedNode.Level == 0)
                {
                    nameNode = selectedNode;
                    keyLNode = selectedNode.FirstNode;
                    typeNode = selectedNode.LastNode;
                }
                else
                {
                    //如果选中项包含‘+’
                    if (selectedNode.Text.ToString().Split(new char[] { '+' }).Length > 1)
                    {
                        keyLNode = selectedNode;
                        nameNode = selectedNode.Parent;
                        typeNode = selectedNode.NextNode;
                    }
                        //选中项不包含‘+’
                    else if(selectedNode.Text.ToString().Split(new char[] { '+' }).Length == 1)
                    {
                        typeNode = selectedNode;
                        nameNode = selectedNode.Parent;
                        keyLNode = selectedNode.PrevNode;
                    }
                }
                //为3个变量赋值
                string location = "";

                //
                string name = T_SName.Text.ToString().Replace(" ", "");
                string type = TypeWayPoint.SelectedItem.ToString();
                if (type != "桥梁")
                {
                    location = T_SLocation.Text.ToString().Replace(" ", "").ToUpper();
                }
                else
                {
                    location = T_SLocation.Text.ToString().Replace(" ", "").ToUpper() + "-" + tempText.Text.ToString().ToUpper().Replace(" ", "");
                }

                if (typeNode.Text.ToString() == type && nameNode.Text.ToString() == name && keyLNode.Text.ToString() == location)
                {
                    MessageBox.Show("项目数据没有改变.", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    InfoStation.Remove(keyLNode.Text.ToString());
                    InfoStation.Add(location, name + "," + type);
                }
                /*
                //改变key-value
                if (InfoStation.ContainsKey(location)) //如果字典中包含key（里程点）
                {
                    InfoStation.Remove(location);
                    InfoStation.Add(location, name + "," + type);
                }
                else
                {
                    foreach (var item in InfoStation)
                    {
                        //字典中的值
                        string[] itemValue = item.Value.ToString().Split(new char[] { ',' });

                        //如果修改了里程，站点名保存不变
                        if (itemValue[0] == name)
                        {
                            //删除该站点对应的key-value
                            InfoStation.Remove(item.Key);
                            //添加新的项
                            InfoStation.Add(location, name + "," + type);
                            break;
                        }
                        else if (itemValue[1] == type)
                        {
                            InfoStation.Remove(item.Key);
                            //添加新的项
                            InfoStation.Add(location, name + "," + type);
                            //如果里程（key）与站点名均不存在则建议手动删除
                            MessageBox.Show("请删除该项.", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }*/

                treeView1.Nodes.Clear();
                refreshTreeview(true);

                modifData = false;
                B_AddWayPoint.Text = "+";
                B_SupWayPoint.Text = "-";
                #endregion
            }
        }

        public void createXml(string CreatXmlFilePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("User");
            xmlDoc.AppendChild(root);
            CreateNode(xmlDoc, root, "name", "xuwei");
            CreateNode(xmlDoc, root, "sex", "male");
            CreateNode(xmlDoc, root, "age", "25");
            try
            {
                xmlDoc.Save(CreatXmlFilePath);
            }
            catch (System.Exception e)
            {
                //显示错误信息  
                MessageBox.Show("发生错误！" + e.ToString(), "错误", MessageBoxButtons.OK);
            }
            //Console.ReadLine();  
        }

        /// <summary>    
        /// 创建节点    
        /// </summary>    
        /// <param name="xmldoc"></param>  xml文档  
        /// <param name="parentnode"></param>父节点    
        /// <param name="name"></param>  节点名  
        /// <param name="value"></param>  节点值  
        ///   
        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }  

        //读XML文件
        public void loadXmlFile(string loadXmlFilePath)
        {
            StringBuilder tb = new StringBuilder();
            XmlTextReader xtr = new XmlTextReader(loadXmlFilePath);
            string str = "";
            try
            {
                while (xtr.Read())

                ///此时，xtr里面有着当前节点的信息也就是从刚刚获得数据的那个节点的信息。也就是xtr.value就是当前元素的值，可以用if（str.name==“”）来获得感兴趣的元素的值
                {
                    switch (xtr.NodeType)
                    {
                        case XmlNodeType.Element:
                            str += "Element:" + xtr.Name;
                            break;
                        case XmlNodeType.Text:
                            str += "Text:" + xtr.Value;
                            break;
                        case XmlNodeType.EndElement:
                            str += "EndElement:" + xtr.Name;
                            break;
                        default:
                            break;
                    }


                }
                //tb.AppendText(str);
                tb.Append(str);

                xtr.Close();

            }

            catch
            {

            }
        }

        /// <summary>
        /// 刷新treeView列表
        /// </summary>
        /// <param name="NoL">true时站名为主节点，false时里程为主节点</param>
        public void refreshTreeview(bool NoL)
        {
            //InfoStation=InfoStation.
            foreach (var item in InfoStation)
            {
                if (NoL==true)
                {
                    TreeNode node1 = new TreeNode();
                    node1.Text = item.Value.Split(new char[] { ',' })[0];
                    treeView1.Nodes.Add(node1);

                    TreeNode node1_1 = new TreeNode();
                    node1_1.Text = item.Key;
                    node1.Nodes.Add(node1_1);

                    TreeNode node1_2 = new TreeNode();
                    node1_2.Text = item.Value.Split(new char[] { ',' })[1];
                    node1.Nodes.Add(node1_2);
                }
                else
                {
                    TreeNode node1 = new TreeNode();
                    node1.Text = item.Key;  
                    treeView1.Nodes.Add(node1);

                    TreeNode node1_1 = new TreeNode();
                    node1_1.Text = item.Value.Split(new char[] { ',' })[0]; 
                    node1.Nodes.Add(node1_1);

                    TreeNode node1_2 = new TreeNode();
                    node1_2.Text = item.Value.Split(new char[] { ',' })[1];
                    node1.Nodes.Add(node1_2); 
                }
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            supprimeTreeView();
        }

        /// <summary>
        /// 删除选中路点信息功能
        /// </summary>
        bool modifData = false;
        private void supprimeTreeView()
        {
            if (treeView1.SelectedNode!=null)
            {
                TreeNode selectNode = treeView1.SelectedNode;

                if (selectNode.Level == 0)
                {
                    //TreeNode selectKeyNode = selectNode.LastNode;

                    if (MessageBox.Show("确定删除项:\n里程:" + selectNode.FirstNode.Text.ToString() + "\n类型:" + selectNode.LastNode.Text.ToString() + "\n站名" + selectNode.Text.ToString(), "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        treeView1.Nodes.Clear();
                        InfoStation.Remove(selectNode.FirstNode.Text.ToString());

                        refreshTreeview(true);
                    }
                }
                else if (selectNode.Level == 1)
                {
                    if (selectNode.Text.ToString().Split(new char[] { '+' }).Length > 1)
                    {
                        TreeNode selectKeyNode = selectNode;
                        TreeNode typeNode = selectNode.NextNode;
                        TreeNode selectParentNode = selectNode.Parent;

                        if (MessageBox.Show("确定删除项:\n里程:" + selectKeyNode.Text.ToString() + "\n类型:" + typeNode.Text.ToString() + "\n站名" + selectParentNode.Text.ToString(), "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            treeView1.Nodes.Clear();
                            InfoStation.Remove(selectKeyNode.Text.ToString());

                            refreshTreeview(true);
                        }

                    }
                    else if (selectNode.Text.ToString().Split(new char[] { '+' }).Length == 1)
                    {
                        TreeNode selectKeyNode = selectNode.PrevNode;
                        TreeNode selectNameNode = selectNode.Parent;
                        TreeNode selectTypeNode = selectNode;

                        if (MessageBox.Show("确定删除项:\n里程:" + selectKeyNode.Text.ToString() + "\n类型:" + selectTypeNode.Text.ToString() + "\n站名" + selectNameNode.Text.ToString(), "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            treeView1.Nodes.Clear();
                            InfoStation.Remove(selectKeyNode.Text.ToString());

                            refreshTreeview(true);
                        }
                    }
                } 
            }
            else
            {
                MessageBox.Show("未选择项目.", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 删除treeview中选中的路点信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_SupWayPoint_Click(object sender, EventArgs e)
        {
            if (modifData == false)
            {
                supprimeTreeView();
            }
            else//当修改数据时
            {
                B_AddWayPoint.Text = "+";
                B_SupWayPoint.Text = "-";
                modifData = false;
                T_SName.Text = "";
                T_SLocation.Text = "";
                TypeWayPoint.SelectedItem = "";
                //如果类型为桥梁
                //foreach(var comp in this.sp)
            }
            
        }
        Label tempLabel;
        TextBox tempText;
        private void TypeWayPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TypeWayPoint.SelectedItem.ToString() == "桥梁")
            {
                T_SLocation.Text = "";
                T_SLocation.Size = new Size(45, 21);

                tempLabel = new Label();
                tempLabel.Text = "<->";
                tempLabel.Name = "tempLabel";
                tempLabel.Location = new Point(107, 72);
                tempLabel.Size = new Size(23, 12);

                tempText = new TextBox();
                tempText.Location = new Point(132, 69);
                tempText.Name = "tempText";
                tempText.Size = new Size(45, 21);

                //添加控件
                this.splitContainer1.Panel1.Controls.Add(tempLabel);
                this.splitContainer1.Panel1.Controls.Add(tempText);
                
            }
            else if (TypeWayPoint.SelectedItem.ToString() != "桥梁" && T_SLocation.Size.Width == 45)
            {

                T_SLocation.Size = new System.Drawing.Size(121, 21);
                int i = 0;
                                   
                if (tempLabel != null)
                    {
                        this.splitContainer1.Panel1.Controls.Remove(tempLabel);
                    }
                if (tempText != null)
                    {
                        this.splitContainer1.Panel1.Controls.Remove(tempText);
                    }
                /* //优化后删除循环
                foreach (var component in splitContainer1.Panel1.Controls)
                {

                   
                    TextBox tempT = component as TextBox;
                    Label tempL = component as Label;
                    //如果控件为不为空(控件为textbox、label)
                    if (tempT != null)// || tempL != null)
                    {
                        if (tempT.Name == "tempText")
                        {
                            //移除控件
                            this.splitContainer1.Panel1.Controls.Remove(tempT);
                        }
                        continue;
                    }
                    if (tempL != null)// || tempL != null)
                    {
                        if (tempL.Name == "tempLabel")
                        {
                            //移除控件
                            this.splitContainer1.Panel1.Controls.Remove(tempL);
                        }
                        continue;
                    }
                    i++;
                }*/
               
            }

        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count > 0)
            {
                modifData = true;
                B_AddWayPoint.Text = "确认";
                B_SupWayPoint.Text = "取消";
                TreeNode selectedNode = treeView1.SelectedNode;

                TreeNode SLocaNode = new TreeNode();
                TreeNode SNameNode = new TreeNode();
                TreeNode STypeNode = new TreeNode();
                if (selectedNode.Level == 0)
                {
                    SLocaNode = selectedNode.FirstNode;
                    SNameNode = selectedNode;
                    STypeNode = selectedNode.LastNode;

                    T_SLocation.Text = SLocaNode.Text.ToString().ToUpper();
                    T_SName.Text = SNameNode.Text.ToString();
                    TypeWayPoint.SelectedItem = STypeNode.Text;

                }
                else if (selectedNode.Level == 1)
                {
                    string[] temp = selectedNode.Text.Split(new char[] { '+' });
                    if (temp.Length == 1) //不是里程
                    {
                        SLocaNode = selectedNode.PrevNode;
                        SNameNode = selectedNode.Parent;
                        STypeNode = selectedNode;

                        T_SLocation.Text = SLocaNode.Text.ToString().ToUpper();
                        T_SName.Text = SNameNode.Text.ToString();
                        TypeWayPoint.SelectedItem = STypeNode.Text.ToString();

                    }
                    else if (temp.Length > 1) //是里程
                    {
                        SLocaNode = selectedNode;
                        SNameNode = selectedNode.Parent;
                        STypeNode = selectedNode.NextNode;

                        T_SLocation.Text = SLocaNode.Text.ToString().ToUpper();
                        T_SName.Text = SNameNode.Text.ToString();
                        TypeWayPoint.SelectedItem = STypeNode.Text.ToString();

                    }
                }
            }
            
        }




        
    }
}
 
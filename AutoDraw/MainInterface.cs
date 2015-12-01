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

namespace AutoDraw
{
    public partial class MainInterface : Form
    {
        private string filePath;
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

            #region
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



        }


        private void 图签名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("功能未完成", "注意", MessageBoxButtons.OK) != DialogResult.OK)
            {
                System.Resources.ResourceWriter rw = new ResourceWriter(@"..\..\abc.txt");
                rw.AddResource("abc", new byte[10000000]);
                rw.Generate();
                rw.Close();

                string filePath = "";
                TuQian tQ = new TuQian(filePath);
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
            //toolStripStatusLabel1.Text = getFilePath();
            
        }

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


        private void B_Add_Click(object sender, EventArgs e)
        {
            getFilePath();
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
                    GetBlocksFromDwgs(importFilePath);

                }

            }
        }

        /// <summary>
        /// 从目标文件导入图块
        /// </summary>
        /// <param name="openFilePath">目标文件</param>
        public void GetBlocksFromDwgs(string openFilePath)
        {

            bool proEnd = false;
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

            path = "C:\\Temp";//  StockLocation;

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
                        autoFitBlock(btRecord);
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

        public void autoFitBlock(BlockTableRecord btRecord)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //ObjectId bObjectId=db.GetObjectId()
                //Entity bEntity= trans.GetObject()
            }

        }
    }
}

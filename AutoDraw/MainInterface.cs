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


namespace AutoDraw
{
    public partial class MainInterface : Form
    {
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
                Point2d outerStartPoint=new Point2d(0,0);
                Point2d outerEndPoint = new Point2d(outerStartPoint.X + 420, outerStartPoint.Y + 297);                

                Point2d innerStartPoint = new Point2d(outerStartPoint.X + 25, outerStartPoint.Y + 5);
                Point2d innerEndPoint = new Point2d(outerEndPoint.X - 5, outerEndPoint.Y - 5);
                
                try
                {
                    DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                    //string[] t = new string[1];
                    //Entity[] TqLines;
                    //TqLines = 
                    //ObjectId styleId = createFont();

                    /*foreach (Entity entity in TqLines)
                    {
                        DBText textAdd = entity as DBText;
                        if (textAdd != null)
                        {
                            TextStyleTable newTextStyleTable = trans.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                            string a = textAdd.TextStyleName;                            
                        }
                        //if(entity.)
                    }
                     */
                    #region 绘制图框
                    Polyline rectangleOuterLayer = new Polyline();
                    Polyline rectangleInnerLayer = new Polyline();
                    rectangleOuterLayer.CreateNewRectangle(outerStartPoint, outerEndPoint);
                    rectangleInnerLayer.CreateNewRectangle(innerStartPoint, innerEndPoint, 0, 1, 1);
                    #endregion

                    #region 图表
                    //绘制工程数量表 innerStartPoint
                    Point2d tableInsertPoint = new Point2d(innerStartPoint.X, innerEndPoint.Y);


                    Entity[] NumbEng = createNumberTable("工程数量表", tableInsertPoint);
                    //绘制设备数量表
                    Entity[] NumbEqu = createNumberTable("设备数量表", new Point2d(tableInsertPoint.X, tableInsertPoint.Y - 100));
                    #endregion

                    #region 添加图签
                    
                    checkBlock(true); //检查图块
                    ObjectId spaceId = db.CurrentSpaceId;//当期空间ID

                    //块属性的字典对象
                    //图签块
                    Dictionary<string, string> attST = new Dictionary<string, string>();
                    attST.Add("项目名称", "新建吉林至珲春铁路工程");
                    attST.Add("图纸名称", "吉珲施防-01");
                    attST.Add("图纸比例", "1：100");
                    attST.Add("绘制日期", "2013.5");
                    attST.Add("页数", "第一张，共1张");
                    #endregion

                    #region 添加图形
                    //插入图块
                    spaceId.InsertBlockReference("0", "三级图签", new Point3d(innerStartPoint.X + 390, innerStartPoint.Y, 0), new Scale3d(1), 0, attST);
                    db.AddToCurrentSpace(rectangleInnerLayer, rectangleOuterLayer);
                    db.AddToCurrentSpace(NumbEng);
                    db.AddToCurrentSpace(NumbEqu);
                    #endregion

                    trans.Commit();
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
        public Entity[] createNumberTable(string stringTableName,Point2d insertPoint)//Database db,Point2d insertPoint,Dictionary<string, string> ItemNumber)
        {
            Entity[] tableAndname = new Entity[2];

            #region 绘制表名
            DBText tableName = new DBText();
            tableName.Position = new Point3d(insertPoint.X + 75, insertPoint.Y - 8, 0);
            tableName.Height = 4.5;
            tableName.TextString = stringTableName;

            tableName.HorizontalMode = TextHorizontalMode.TextCenter;
            tableName.VerticalMode = TextVerticalMode.TextVerticalMid;
            tableName.AlignmentPoint = tableName.Position;
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


            tb.Position = new Point3d(insertPoint.X, insertPoint.Y, 0);

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
            str[2, 5] = "Steel";

            str[3, 1] = "Tile";
            str[3, 2] = "SPTYWPL23-16B芯 ";
            str[3, 3] = "m";
            str[3, 4] = "2445";
            str[3, 5] = "Ceramic";

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
                    //tb.Cells[i,j].

                    //文字宽度
                    if (i == 0)
                    {
                        tb.Cells[i, j].TextHeight = 4;
                    }
                    else if (i > 0 || i < 10)
                    {
                        tb.Cells[i, j].TextHeight = 3.5;
                    }
                    

                }
            }
            tb.GenerateLayout();
            #endregion
            
            tableAndname[0] = tb;
            tableAndname[1] = tableName;
            return tableAndname;
            }

        
        

        /*
        //创建字体
        public ObjectId createFont()
        {
            //simsun.ttc 宋体
            Database db = HostApplicationServices.WorkingDatabase;

            ObjectId styleId;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {

                styleId = db.AddTextStyle("宋体-0.7", "simsun.ttc");
                styleId.SetTextStyleProp(3, 0.7, 0, false, false, false, AnnotativeStates.True, true);

            }
            return styleId;
        }
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="withoutScale">是否不显示比例尺数值</param>
        public void checkBlock(bool withoutScale)//Point2d innerStartPoint,
        {
            string[] t = new string[1];
            MakeSignatureTable("三级图签", withoutScale);
        }

        /// <summary>
        /// 生成三级或四级签名块
        /// </summary>
        /// <param name="insertPoint">设置插入点，插入点为图框右下角</param>
        /// <param name="nameTable">签名块设置</param>
        /// <returns></returns>
        public Entity[] MakeSignatureTable(string nameTable ,bool withoutScale)//, Point2d innerEndPoint)
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

            if (nameTable=="三级图签")
            {
                Vline2A = new Point2d(tqUpLeft.X, tqUpLeft.Y - 19.44);
                Vline2B = new Point2d(tqUpLeft.X + 50, tqUpLeft.Y - 19.44);
                Vline2.CreatePolyline(Vline2A, Vline2B);

                Vline3A = new Point2d(tqUpLeft.X, tqUpLeft.Y - 19.44 - 9);
                Vline3B = new Point2d(tqUpLeft.X + 50, tqUpLeft.Y - 19.44 - 9);
                Vline3.CreatePolyline(Vline3A, Vline3B); 
            }
            else if(nameTable=="四级图签")
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
            text1.WidthFactor = 0.7;

            DBText text2 = new DBText();
            text2.Position = new Point3d(tqUpLeft.X + 80, tqUpLeft.Y - 5.22, 0);
            text2.Height = 3;
            text2.TextString = "工程名称";
            //text1.
            text2.HorizontalMode = TextHorizontalMode.TextCenter;
            text2.VerticalMode = TextVerticalMode.TextVerticalMid;
            text2.AlignmentPoint = text2.Position;
            text2.WidthFactor = 0.7;

            DBText text3 = new DBText();
            text3.Position = new Point3d(tqDownRight.X - 35 - 7.5, tqDownRight.Y + 24, 0);
            text3.Height = 3;
            text3.TextString = "图号";
            //text1.
            text3.HorizontalMode = TextHorizontalMode.TextCenter;
            text3.VerticalMode = TextVerticalMode.TextVerticalMid;
            text3.AlignmentPoint = text3.Position;
            text3.WidthFactor = 0.7;

            DBText text4 = new DBText();
            text4.Position = new Point3d(tqDownRight.X - 35 - 7.5, tqDownRight.Y + 17.5, 0);
            text4.Height = 3;
            text4.TextString = "比例尺";
            //text1.
            text4.HorizontalMode = TextHorizontalMode.TextCenter;
            text4.VerticalMode = TextVerticalMode.TextVerticalMid;
            text4.AlignmentPoint = text4.Position;
            text4.WidthFactor = 0.7;

            DBText text5 = new DBText();
            text5.Position = new Point3d(tqDownRight.X - 35 - 7.5, tqDownRight.Y + 10.5, 0);
            text5.Height = 3;
            text5.TextString = "日期";
            //text1.
            text5.HorizontalMode = TextHorizontalMode.TextCenter;
            text5.VerticalMode = TextVerticalMode.TextVerticalMid;
            text5.AlignmentPoint = text5.Position;
            text5.WidthFactor = 0.7;

            DBText text6 = new DBText();
            if (nameTable=="三级图签")
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
            text6.WidthFactor = 0.7;

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
            text7.WidthFactor = 0.7;
   


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
            text8.WidthFactor = 0.7;
           

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
            text9.WidthFactor = 0.7;


            Entity[] TqLines = new Entity[23]; 
            ArrayList  a=new ArrayList ();
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

            MakeTableRecode(nameTable, TqLines, withoutScale);
            #endregion

            return TqLines;
        }

        public ObjectId MakeTableRecode(string blockName,Entity[] entitys,bool withoutScale)
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
                        AttributeDefinition[] attD = addAttSignaTable(withoutScale);
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
        public AttributeDefinition[] addAttSignaTable(bool invisibilityScale)//string blockName, Entity[] entitys)
        {
            /*Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //AttributeDefinition 
            }*/

            AttributeDefinition[] attD = new AttributeDefinition[5];

            //表示门符号的属性定义
            AttributeDefinition attProjetName = new AttributeDefinition(Point3d.Origin, "xx铁路", "项目名称", "输入项目名称：", ObjectId.Null);
            //表示属性定义的通用样式
            setStyleForAtt(attProjetName, 3, false);
            //对其点
            attProjetName.AlignmentPoint = new Point3d(-45, 32.22, 0);

            //表示门符号的属性定义
            AttributeDefinition attDrawName = new AttributeDefinition(Point3d.Origin, "xxx防-xx-xx", "图纸名称", "输入图纸名称：", ObjectId.Null);
            //表示属性定义的通用样式
            setStyleForAtt(attDrawName, 3, false);
            //对其点
            attDrawName.AlignmentPoint = new Point3d(-17.5, 24, 0);

            //表示门符号的属性定义
            AttributeDefinition attScale = new AttributeDefinition(Point3d.Origin, "1：100", "图纸比例", "输入图纸比例：", ObjectId.Null);
            //表示属性定义的通用样式
            setStyleForAtt(attScale, 3, invisibilityScale);
            //对其点
            attScale.AlignmentPoint = new Point3d(-17.5, 17.5, 0);

            //表示门符号的属性定义
            AttributeDefinition attYearMonth = new AttributeDefinition(Point3d.Origin, "xxxx.xx", "绘制日期", "输入图纸绘制日期：", ObjectId.Null);
            //表示属性定义的通用样式
            setStyleForAtt(attYearMonth, 3, false);
            //对其点
            attYearMonth.AlignmentPoint = new Point3d(-17.5, 10.5, 0);

            //表示门符号的属性定义
            AttributeDefinition attPage = new AttributeDefinition(Point3d.Origin, "第 x 张,共 x 张", "页数", "输入图纸页数：", ObjectId.Null);
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
            if (MessageBox.Show("功能未完成","注意",MessageBoxButtons.OK)!=DialogResult.OK)
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

        string filePath;
        private void MainInterface_Load(object sender, EventArgs e)
        {
            
            /* //另存为，
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            string strDWGName = acDoc.Name;
            object obj = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("DWGTITLED");
            // Check to see if the drawing has been named
            if (System.Convert.ToInt16(obj) == 0)
            {
                // If the drawing is using a default name (Drawing1, Drawing2, etc)
                // then provide a new name
                strDWGName = "c:\\MyDrawing.dwg";
            }
            // Save the active drawing
            acDoc.Database.SaveAs(strDWGName,  DwgVersion.Current);//,, acDoc.Database.SecurityParameters);
            */

            toolStripStatusLabel1.Text = getFilePath();
            
            /* */
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
    }
}

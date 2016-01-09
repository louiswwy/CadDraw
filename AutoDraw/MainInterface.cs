using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Text;
using System;
using System.Linq;

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
        private string lastLoadFilePath;  //文件位置
        private string newFilePath;
        string xmlFilePath;       //xml文件位置
        private string imgStoragePath; //图像文件位置 imgPath

        List<string> colName;

        /// <summary>
        /// 添加站点信息
        /// </summary>
        DataSet tableST;
        DataSet oldTableSt;
        System.Data.DataTable stTable;
        Dictionary<string, string> InfoStation = new Dictionary<string, string>();

        /// <summary>
        /// 风监控
        /// </summary>
        DataSet tableWind;
        System.Data.DataTable WindTable;
        Dictionary<string, string> InfoWindPoint = new Dictionary<string, string>();

        /// <summary>
        /// 雨监控
        /// </summary>
        DataSet tableRain;
        System.Data.DataTable RainTable;
        Dictionary<string, string> InfoRainPoint = new Dictionary<string, string>();

        /// <summary>
        /// 雪监控
        /// </summary>
        DataSet tableSnow;
        System.Data.DataTable SnowTable;
        Dictionary<string, string> InfoSnowPoint = new Dictionary<string, string>();

        /// <summary>
        /// 地震监控
        /// </summary>
        System.Data.DataTable EarthTable;
        Dictionary<string, string> InfoEarthPoint = new Dictionary<string, string>();

        /// <summary>
        /// 线缆
        /// </summary>
        List<string> ListFournniseur = new List<string>();
        Dictionary<string, string> LineTypeInfo = new Dictionary<string, string>();


        public MainInterface()
        {
            InitializeComponent();
        }

        private void B_Draw_Click(object sender, EventArgs e)
        {

            //DrawPicture();
        }

        /// <summary>
        /// 绘制防灾设备电缆径路图
        /// </summary>
        /// <param name="dictRailStation">基站、所亭表</param>
        /// <param name="connectionDict">‘基站-防灾设备’连接情况</param>
        public void DrawPicture(Dictionary<string,string> dictRailStation, List<ClassStruct.ConnectionAndLine> List_ConnectionAndLine)
        {
            XmlFunction xF = new XmlFunction();
            List<string> projetInfor;

            projetInfor = xF.readProjrtInfo(xmlFilePath + "\\setting.xml");
            DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

            //创建字体
            ObjectId fontStyleId = createFont();
            //检查图块
            checkBlock(true, fontStyleId); //检查图块

            int[] dictChecked = new int[List_ConnectionAndLine.Count]; //生成一个数组，用来记录已经遍历过的字典项
            List<string> CheckList = new List<string>();

            //string stationBeDraw;
            ClassStruct.StationPoint stationBeDraw;
            //List<string> equipeBeDraw;
            List<ClassStruct.EquipePoint> equipeBeDraw;
            List<string> equipement = new List<string>(); //当前所亭连接的防灾设备
            
            //绘图原点
            Point2d insertOriginSignlePoint = new Point2d(0, 0);

            //绘图次数
            int drawRound = 0;

            List<ClassStruct.StationPoint> StationList = new List<ClassStruct.StationPoint>();
            foreach(var ConnectionAndLine in List_ConnectionAndLine)
            {
                if (!StationList.Contains(ConnectionAndLine.station))
                {
                    StationList.Add(ConnectionAndLine.station);
                }
                //StationList.Add(item.Split(new char[] { '-' })[0]); //车站
            }

            //List<string>  Stations= StationList.Union(StationList).ToList();  //去除重复项 得到无重复的基站、所亭的列表

            List<string> unCheckConnection = new List<string>();

            foreach (var _station in StationList) //
            {
                stationBeDraw = _station;
                equipeBeDraw = new List<ClassStruct.EquipePoint>();

                //以所亭信息为准合并同类项
                //foreach (var item in connectionDict)
                foreach (var item in List_ConnectionAndLine)
                {
                    if (item.station.name == stationBeDraw.name && item.station.location == stationBeDraw.location && item.station.type == stationBeDraw.type)
                    {
                        equipeBeDraw.Add(item.equipe); //将绘制的设备列表
                    }

                }

                ///确认本张图的绘图起点位置 （左下角）
                Point2d insertSignlePoint = new Point2d(insertOriginSignlePoint.X + drawRound * 450, insertOriginSignlePoint.Y);
                drawRound++;

                #region 找到所亭的左右两侧的点

                //string[] a = stationBeDraw.ToString().Split(new char[] { ',' });

                int STLocation = stationBeDraw.distance; //所亭里程

                List<string> stationList = new List<string>(); //站点信息List表
                //确定基站在哪个区间内，从后向前比较
                foreach (var stat in dictRailStation)
                {
                    stationList.Add(stat.Key + "," + stat.Value); //dictionary -> list
                }

                string leftStation = ""; //左侧站点
                string RighStation = ""; //右侧站点


                for (int _rsCount = 0; _rsCount < stationList.Count; _rsCount++)
                {
                    if (STLocation < int.Parse(stationList[_rsCount].Split(new char[] { ',' })[3])) //当所亭所在里程比当前站点小时
                    {
                        RighStation = stationList[_rsCount];//右侧站点等于该车站

                        if (_rsCount > 0) //当_rsCount大于0时，说明左侧还有车站
                        {
                            leftStation = stationList[_rsCount - 1];// 左侧车站为列表中前一个车站
                            break;
                        }
                    }
                    else if (STLocation > int.Parse(stationList[_rsCount].Split(new char[] { ',' })[3]) && _rsCount == stationList.Count - 1) //当基站里程比最远车站还要大时
                    {
                        RighStation = "";
                        leftStation = stationList[_rsCount];
                        break;
                    }

                }

                List<string> leftRightStation = new List<string>();

                leftRightStation.Add(leftStation);
                leftRightStation.Add(RighStation);

                #endregion

                drawSingleStationPicture(insertSignlePoint, fontStyleId, projetInfor, stationBeDraw, leftRightStation, equipeBeDraw, List_ConnectionAndLine, drawRound);// connectionDict);//绘制单基站

            }

            m_DocumentLock.Dispose();

        }

        /// <summary>
        /// 绘制单个基站的防灾设备径路示意图图纸
        /// </summary>
        /// <param name="insertSignlePoint">插入点</param>
        /// <param name="fontStyleId">字型id</param>
        /// <param name="projetInfo">项目信息</param>
        /// <param name="StationToBeDraw">基站、所亭</param>
        /// <param name="LeftRightStation">相对于基站的左右车站</param>
        /// <param name="N_Equipment">防灾设备清单</param>
        public void drawSingleStationPicture(Point2d insertSignlePoint, ObjectId fontStyleId, List<string> projetInfo, ClassStruct.StationPoint StationToBeDraw, List<string> LeftRightStation, List<ClassStruct.EquipePoint> N_Equipment, List<ClassStruct.ConnectionAndLine> List_ConnectionAndLine,int drawNum)//List<string> connectionDict)
        {
            Point2d outerStartPoint = insertSignlePoint;
            Point2d outerEndPoint = new Point2d(outerStartPoint.X + 420, outerStartPoint.Y + 297);

            Point2d innerStartPoint = new Point2d(outerStartPoint.X + 25, outerStartPoint.Y + 5);
            Point2d innerEndPoint = new Point2d(outerEndPoint.X - 5, outerEndPoint.Y - 5);
            Database db1 = HostApplicationServices.WorkingDatabase;

            #region 绘制背景图，图框、图表
            using (Transaction trans1 = db1.TransactionManager.StartTransaction())
            {
                //单个图框的范围
                try
                {
                    #region 绘制图框
                    Polyline rectangleOuterLayer = new Polyline();
                    Polyline rectangleInnerLayer = new Polyline();
                    rectangleOuterLayer.CreateNewRectangle(outerStartPoint, outerEndPoint);
                    rectangleInnerLayer.CreateNewRectangle(innerStartPoint, innerEndPoint, 0, 1, 1);
                    #endregion


                    
                    #region 绘制通信、信号电缆槽示意图
                    drawFunction df = new drawFunction();
                    Entity[] backgoundEntity = df.drawBackGround(db1, trans1, new Point2d(innerStartPoint.X, innerEndPoint.Y), fontStyleId);
                    #endregion

                    //添加图内、外框
                    db1.AddToCurrentSpace(rectangleInnerLayer, rectangleOuterLayer); //框
                    db1.AddToCurrentSpace(backgoundEntity); //背景块
                    trans1.Commit();
                }
                catch (System.Exception ee)
                {

                    MessageBox.Show("出现错误！" + System.Environment.NewLine + "\t错误信息:" + ee.ToString(), "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trans1.Abort();
                }
            }
            #endregion

            Database db2 = HostApplicationServices.WorkingDatabase;
            using (Transaction trans2 = db2.TransactionManager.StartTransaction())
            {
                try
                {
                    #region 添加图形
                    ObjectId spaceId = db2.CurrentSpaceId;//当期空间ID
                    //插入图签
                    #region 添加图签

                    string pictureNum = "";
                    if (drawNum < 10)
                    {
                        pictureNum = "0" + drawNum;
                    }
                    else
                    {
                        pictureNum = "" + drawNum;
                    }
                    //块属性的字典对象
                    //图签块
                    Dictionary<string, string> attTQ = new Dictionary<string, string>();
                    attTQ.Add("项目名称", projetInfo[0]);
                    attTQ.Add("图纸名称", projetInfo[1] + "-" + projetInfo[2] + "-"+ pictureNum);
                    attTQ.Add("图纸比例", "1：100");
                    attTQ.Add("绘制日期", System.DateTime.Now.Year + "." + System.DateTime.Now.Month);
                    attTQ.Add("页数", "第1张，共1张");
                    #endregion

                    #region 轨道图标
                    Dictionary<string, string> attGD = new Dictionary<string, string>();
                    attGD.Add("上行/下行线", projetInfo[3] + "下行线");

                    Dictionary<string, string> attGD2 = new Dictionary<string, string>();
                    attGD2.Add("上行/下行线", projetInfo[3] + "上行线");
                    #endregion

                    BlockTable acBlkTbl = trans2.GetObject(db2.BlockTableId, OpenMode.ForRead) as BlockTable;
                    acBlkTbl.UpgradeOpen();
                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec = trans2.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    acBlkTblRec.UpgradeOpen();

                    spaceId.InsertBlockReference("0", "三级图签", new Point3d(innerStartPoint.X + 390, innerStartPoint.Y, 0), new Scale3d(1), 0, attTQ);

                    spaceId.InsertBlockReference("0", "铁轨_Length_248", new Point3d(innerStartPoint.X + 116, innerEndPoint.Y - 137.2, 0), new Scale3d(1), 0, attGD);
                    spaceId.InsertBlockReference("0", "铁轨_Length_248", new Point3d(innerStartPoint.X + 116, innerEndPoint.Y - 159.2, 0), new Scale3d(1), 0, attGD2);
                    spaceId.InsertBlockReference("0", "图例", new Point3d(innerStartPoint.X + 5.5, innerStartPoint.Y + 57, 0), new Scale3d(1), 0);


                    #region 变动部分
                    #region  车站标

                    PFunction pf = new PFunction();
                    int departStation = 0;
                    int arriveStation = 0;

                    if (LeftRightStation[0] != "")
                    {
                        Dictionary<string, string> attArrive = new Dictionary<string, string>();
                       
                        attArrive.Add("站名", LeftRightStation[1].Split(new char[] { ',' })[1]);
                        attArrive.Add("里程", LeftRightStation[1].Split(new char[] { ',' })[0]);
                        spaceId.InsertBlockReference("0", "到达站站点标示", new Point3d(innerStartPoint.X + 175 + 188, innerStartPoint.Y + 238, 0), new Scale3d(1), 0, attArrive);

                        //换算成距离
                        departStation = Int32.Parse(LeftRightStation[0].Split(new char[] { ',' })[3]);
                    }

                    if (LeftRightStation[1] != "")
                    {
                        Dictionary<string, string> attdepart = new Dictionary<string, string>();
                        attdepart.Add("站名", LeftRightStation[0].Split(new char[] { ',' })[1]);
                        attdepart.Add("里程", LeftRightStation[0].Split(new char[] { ',' })[0]);
                        spaceId.InsertBlockReference("0", "始发站站点标示", new Point3d(innerStartPoint.X + 175-8.21, innerStartPoint.Y + 238, 0), new Scale3d(1), 0, attdepart);

                        //换算成距离
                        arriveStation = Int32.Parse(LeftRightStation[1].Split(new char[] { ',' })[3]);
                    }

                    string textDistance = " ";
                    //if (departStation != 0 && arriveStation != 0) //
                    {
                        textDistance = Math.Abs(departStation - arriveStation).ToString();
                    }
                    //站间里程
                    //DBText DistanceText = (DBText)insertText(textDistance, new Point3d(innerStartPoint.X + 264, innerStartPoint.Y + 240, 0), fontStyleId);
                    //db2.AddToModelSpace(DistanceText);
                    
                    AlignedDimension dimAlignedStations = new AlignedDimension();
                    dimAlignedStations.XLine1Point = new Point3d(innerStartPoint.X + 175 + 188, innerStartPoint.Y + 238 + 4, 0);
                    dimAlignedStations.XLine2Point = new Point3d(innerStartPoint.X + 175 - 8.21, innerStartPoint.Y + 238 + 4, 0);
                    dimAlignedStations.DimLinePoint = GeTools.MidPoint(new Point3d(innerStartPoint.X + 175 + 188, innerStartPoint.Y + 238 + 4, 0), new Point3d(innerStartPoint.X + 175 - 8.21, innerStartPoint.Y + 238 + 4, 0)).PolarPoint(0, 10);
                    //dimAlignedStations.te
                    dimAlignedStations.DimensionText = Math.Round(Convert.ToDouble(textDistance) / 1000, 1).ToString() + "km";
                    db2.AddToModelSpace(dimAlignedStations);


                    #endregion

                    #region 设备标志
                    //跟据图块数量决定每个块的间距   innerStartPoint.X + 168+, innerStartPoint.Y + 363, 0)
                    int marginEqui = 210 / (N_Equipment.Count + 1);
                    //绘图起始点
                    Point3d equipeInsertPoint = new Point3d(innerStartPoint.X + 170, innerStartPoint.Y + 180, 0);


                    //每一个都插入一次
                    bool stationInsertion = false;

                    List<Point3d> registBoxInsertPoint = new List<Point3d>();  //记录控制箱插入点用于画线
                    List<List<Point3d>> registEquipeInsertPoint = new List<List<Point3d>>();  //记录设备插入点用于画线

                    List<Point3d> registGouChaoInsertPoint = new List<Point3d>();  //记录设备插入点用于画线

                    Point3d registStationInsertPoint = new Point3d();          ///记录插入点用于画线


                    Dictionary<string, ClassStruct.KeyPoint> drawComponent = new Dictionary<string, ClassStruct.KeyPoint>();

                    //比较设备和基站的里程大小
                    List<KeyValuePair<int, string>> lstorder=new List<KeyValuePair<int, string>>();
                    Dictionary<string, int> lichengList = new Dictionary<string, int>();
                    try
                    {
                        lichengList.Add(StationToBeDraw.distance + "," + StationToBeDraw.type, StationToBeDraw.distance);
                        foreach (var a in N_Equipment)
                        {
                            lichengList.Add(a.distance + "," + a.type, a.distance);
                        }
                        lichengList.OrderBy(c => c.Value).ToList();
                    }
                    catch (System.Exception ee)
                    {
                        MessageBox.Show("" + ee.ToString());
                        throw;
                    }

                    //对list排序
                    //var result = from pair in lichengList orderby pair.Key select pair;
                    //List<KeyValuePair<int, string>> lstorder = lichengList.OrderBy(c => c.Key).ToList();
                    int numInsert = 0; //以绘制几个图形
                    foreach(var a in lichengList)
                    {
                        string aa = a.Key.ToString().Split(new char[] { ',' })[1];

                        lstorder.Add(new KeyValuePair<int, string>(a.Value, aa));
                    }


                    Dictionary<string, double> LineLength = new Dictionary<string, double>();//本张图中使用的线缆类型及

                    //根据设备、基站里程绘图
                    foreach (KeyValuePair<int,string> LiCheng_Type in lstorder)
                    {
                        if (StationToBeDraw.distance == LiCheng_Type.Key&& StationToBeDraw.type == LiCheng_Type.Value) //绘制基站
                        {
                            string stationName = transforBlockName(StationToBeDraw.type);

                            //DBText equipeText1 = (DBText)insertText(StationToBeDraw.location + " " + StationToBeDraw.name, new Point3d(equipeInsertPoint.X + marginEqui * numInsert, equipeInsertPoint.Y, 0), fontStyleId);
                            //db2.AddToModelSpace(equipeText1);
                            spaceId.InsertBlockReference("0", stationName, new Point3d(equipeInsertPoint.X + marginEqui * numInsert, equipeInsertPoint.Y, 0), new Scale3d(1), 0); //插入车站标

                            spaceId.InsertBlockReference("0", "监控单元_G", new Point3d(equipeInsertPoint.X + marginEqui * numInsert, equipeInsertPoint.Y + 2, 0), new Scale3d(1), 0); //插入监控单元图块

                            registStationInsertPoint = new Point3d(equipeInsertPoint.X + marginEqui * numInsert, equipeInsertPoint.Y + 2, 0); //记录插入点用于画线

                            registGouChaoInsertPoint.Add(new Point3d(equipeInsertPoint.X + marginEqui * numInsert + 1, equipeInsertPoint.Y, 0));  //绘制两条红线用于表示基站至信号电缆槽的挖沟
                            registGouChaoInsertPoint.Add(new Point3d(equipeInsertPoint.X + marginEqui * numInsert - 1, equipeInsertPoint.Y, 0));  //绘制两条红线用于表示基站至信号电缆槽的挖沟
                            DBText equipeText = (DBText)insertText(StationToBeDraw.location + " " + StationToBeDraw.name, new Point3d(innerStartPoint.X + 146 + (218 / (N_Equipment.Count + 1)) * numInsert + 20, innerStartPoint.Y + 89 + 7.5 / 2, 0), 3, TextHorizontalMode.TextLeft, fontStyleId);
                            db2.AddToModelSpace(equipeText);

                            numInsert++;
                            continue;
                        }
                        else
                        {
                            foreach(var equipe in N_Equipment)
                            {
                                if (equipe.distance == LiCheng_Type.Key&& equipe.type == LiCheng_Type.Value)//绘制设备
                                {
                                    //插入接触网杆
                                    spaceId.InsertBlockReference("0", "接触网杆_g", new Point3d(equipeInsertPoint.X + marginEqui * numInsert, equipeInsertPoint.Y +40, 0), new Scale3d(1), 0);
                                    //插入防灾控制箱
                                    spaceId.InsertBlockReference("0", "防灾控制箱_G", new Point3d(equipeInsertPoint.X + 1.4 + marginEqui * numInsert, equipeInsertPoint.Y + 40 - 23.4, 0), new Scale3d(1), 0);

                                    //记录防灾控制箱插入点插入点
                                    registBoxInsertPoint.Add(new Point3d(equipeInsertPoint.X + 2.8 + marginEqui * numInsert, equipeInsertPoint.Y + 40 - 23.4 - 0.9, 0));

                                    registGouChaoInsertPoint.Add(new Point3d(equipeInsertPoint.X + marginEqui * numInsert + 1, equipeInsertPoint.Y + 0.5, 0));  //绘制两条红线用于表示接触网杆至信号电缆槽的挖沟
                                    registGouChaoInsertPoint.Add(new Point3d(equipeInsertPoint.X + marginEqui * numInsert - 1, equipeInsertPoint.Y + 0.5, 0));  //绘制两条红线用于表示接触网杆至信号电缆槽的挖沟

                                    string blockName = transforBlockName(equipe.name);
                                    if (!blockName.Contains("速计"))
                                    {
                                        spaceId.InsertBlockReference("0", blockName, new Point3d(equipeInsertPoint.X + marginEqui * numInsert + 8.2, equipeInsertPoint.Y + 40 + 1, 0), new Scale3d(1), 0);
                                    }
                                    else //如果设备是风速计，则插入两次
                                    {
                                        spaceId.InsertBlockReference("0", blockName, new Point3d(equipeInsertPoint.X + marginEqui * numInsert + 8.2, equipeInsertPoint.Y + 40 + 1, 0), new Scale3d(1), 0);
                                        spaceId.InsertBlockReference("0", blockName, new Point3d(equipeInsertPoint.X + marginEqui * numInsert - 8.2, equipeInsertPoint.Y + 40 + 1, 0), new Scale3d(1), 0);
                                    }

                                    //插入文字‘防灾控制箱’
                                    DBText ControlBoxText = (DBText)insertText("防灾控制箱", new Point3d(equipeInsertPoint.X + 1.4 + marginEqui * numInsert + 2, equipeInsertPoint.Y + 40 - 23.4 + 4, 0), 3, TextHorizontalMode.TextLeft, fontStyleId);


                                    #region 文字：设备名称，线缆类型，方向及长度
                                    #region 设备名字
                                    //于下方表格中添加入设备文字信息
                                    DBText equipeText = (DBText)insertText(equipe.name + " " + equipe.location, new Point3d(innerStartPoint.X + 146 + (218 / (N_Equipment.Count + 1)) * numInsert, innerStartPoint.Y + 89 - 7.5 / 2, 0), 3, TextHorizontalMode.TextLeft, fontStyleId);
                                    db2.AddToModelSpace(equipeText, ControlBoxText);
                                    #endregion

                                    #region 设备至监控单元线缆类型，方向及长度，及标注
                                    //添加文字，显示设备至监控单元线型、长度
                                    StringBuilder lineText = new StringBuilder();
                                    ClassStruct.LineInfor selectedLine = new ClassStruct.LineInfor();
                                    foreach(var a in List_ConnectionAndLine)
                                    {
                                        if (a.station.name == StationToBeDraw.name && a.station.location == StationToBeDraw.location && a.station.type == StationToBeDraw.type && a.equipe.name == equipe.name && a.equipe.location == equipe.location && a.equipe.type == equipe.type)
                                        {
                                            selectedLine = a.line;
                                        }
                                    }

                                    lineText.Append(blockName + "至" + StationToBeDraw.type + "防灾监控单元" + System.Environment.NewLine + TextTools.StackText(selectedLine.shortfor, "", selectedLine.lineType.ToString(), StackType.Tolerance, 0.5) + "-" + selectedLine.lineXin + "-" + selectedLine.lineLength + "m");

                                    MText LineType_Length = new MText();
                                    LineType_Length.Location = new Point3d(equipeInsertPoint.X + marginEqui * numInsert - 35, equipeInsertPoint.Y - 60 + 35.4, 0);
                                    LineType_Length.Width = 50;
                                    LineType_Length.TextHeight = 2;
                                    LineType_Length.TextStyleId = fontStyleId;
                                    LineType_Length.Contents = lineText.ToString();
                                    LineType_Length.Attachment = AttachmentPoint.MiddleLeft;
                                    db2.AddToModelSpace(LineType_Length); //添加线缆类型、线缆长度的说明



                                    List<Point3d> listP = new List<Point3d>();
                                    Point3d endP = new Point3d(equipeInsertPoint.X + 1.4 + marginEqui * numInsert - 6, equipeInsertPoint.Y + 40 - 23.4 - 41.5, 0);
                                    listP.Add(endP);
                                    listP.Add(new Point3d(endP.X - 32.5, endP.Y, 0));
                                    listP.Add(new Point3d(endP.X - 32.5, endP.Y + 11, 0));
                                    Polyline[] mlineText = drawMutiLine(listP, 0.05, Autodesk.AutoCAD.Colors.Color.FromRgb(255, 255, 255));
                                    db2.AddToModelSpace(mlineText); //添加标注线
                                    #endregion
                                    #endregion

                                    #region
                                    if (LineLength.Count == 0)
                                    {
                                        LineLength.Add(selectedLine.name, selectedLine.lineLength);
                                    }
                                    else
                                    {
                                        int count = 0;
                                        //foreach (KeyValuePair<string, double> line in LineLength)
                                        {
                                            if (!LineLength.Keys.Contains(selectedLine.name))
                                            {
                                                LineLength.Add(selectedLine.name, selectedLine.lineLength);

                                                break;
                                            }
                                            else
                                            {
                                                //LineLength.Add(name, length + selectedLine.lineLength);
                                                LineLength[selectedLine.name] = LineLength[selectedLine.name] + selectedLine.lineLength;

                                                break;

                                            }
                                        }
                                    }
                                    #endregion

                                    //记录由防灾设备箱-防灾设备的点
                                    List<Point3d> oneLine = new List<Point3d>();
                                    oneLine.Add(new Point3d(equipeInsertPoint.X + 1.4 + marginEqui * numInsert, equipeInsertPoint.Y + 40 - 23.4, 0)); //防灾箱
                                    oneLine.Add(new Point3d(equipeInsertPoint.X + marginEqui * numInsert, equipeInsertPoint.Y + 40 - 23.4 + 2, 0)); //拐点
                                    oneLine.Add(new Point3d(equipeInsertPoint.X + marginEqui * numInsert, equipeInsertPoint.Y + 40 - 2, 0)); //拐点


                                    //记录从防灾控制箱至防灾设备的电缆示意图
                                    if (!blockName.Contains("速计"))//如果名称中不包含”速计“ ，除风速计以外都只有一个设备
                                    {
                                        oneLine.Add(new Point3d(equipeInsertPoint.X + 8.2 + marginEqui * numInsert, equipeInsertPoint.Y + 40 + 1, 0)); //设备点1
                                        registEquipeInsertPoint.Add(oneLine);
                                    }
                                    else  //风速计需要上两个
                                    {
                                        List<Point3d> twoLine = new List<Point3d>();
                                        twoLine.AddRange(oneLine); 
                                        oneLine.Add(new Point3d(equipeInsertPoint.X + 8.2 + marginEqui * numInsert, equipeInsertPoint.Y + 40 + 1, 0)); //设备点1
                                        registEquipeInsertPoint.Add(oneLine); //至右侧风速计
                                        twoLine.Add(new Point3d(equipeInsertPoint.X - 8.2 + marginEqui * numInsert, equipeInsertPoint.Y + 40 + 1, 0)); //设备点2

                                        registEquipeInsertPoint.Add(twoLine); //至左侧风速计
                                    }

                                    numInsert++;//画一次图，增加一次
                                    break;
                                }
                            }
                        }
                    }

                    #region 工程数量
                    int Num_DiXian = N_Equipment.Count * 10;  //地线
                    int Num_ChenDuan = N_Equipment.Count * 2; //电缆成端
                    int Num_GangGuan = 100;                   //钢管防护
                    int Num_Gou = 40;                         //0.8电缆沟
                    int Num_Chao = N_Equipment.Count * 10;    //100*50电缆槽
                                                              //LineLength

                    #region 图表 （暂时未完成）
                    //绘制工程数量表 innerStartPoint
                    Point2d tableInsertPoint = new Point2d(innerStartPoint.X, innerEndPoint.Y);

                    List<string> GongChenTable = new List<string>();
                    GongChenTable.Add("挖填光、电缆沟,沟深0.8米,m," + Num_Gou + ", ");
                    GongChenTable.Add("挖填光、电缆槽,100X50,m," + Num_Chao + ", ");
                    GongChenTable.Add("钢管防护,50,m," + Num_GangGuan + ", ");
                    GongChenTable.Add("编焊电缆成端,,个," + Num_ChenDuan + ", ");
                    foreach (KeyValuePair<string, double> line in LineLength)
                    {
                        GongChenTable.Add("敷设信号电缆," + line.Key + ",米," + line.Value + ", ");
                    }



                    Entity[] NumbEngineTable = createNumberTable("工程数量表", new Point2d(tableInsertPoint.X, tableInsertPoint.Y), fontStyleId, GongChenTable);

                    //绘制设备数量表
                    List<string> SheBeiTable = new List<string>();

                    SheBeiTable.Add("光、电缆槽,100X50,m," + Num_Chao + ", ");
                    SheBeiTable.Add("钢管,50,m," + Num_GangGuan + ", ");
                    SheBeiTable.Add("电缆成端,,个," + Num_ChenDuan + ", ");
                    foreach (KeyValuePair<string, double> line in LineLength)
                    {
                        SheBeiTable.Add("信号电缆," + line.Key + ",米," + line.Value + ", ");
                    }
                    Entity[] NumbEqipeTable = createNumberTable("设备数量表", new Point2d(tableInsertPoint.X, tableInsertPoint.Y - 90), fontStyleId, SheBeiTable);
                    #endregion


                    db2.AddToCurrentSpace(NumbEngineTable); //添加工程数量表
                    db2.AddToCurrentSpace(NumbEqipeTable); //添加设备数量表
                    #endregion

                    #endregion

                    #region 添加MText，绘制说明

                    //图纸说明文字
                    StringBuilder Shuoming = new StringBuilder();
                    Shuoming.Append("说明：" + System.Environment.NewLine + "  " + "1.图中所示风、雨、雪所用的控制电缆型号，施工阶段根据实际情况可做调整。" + System.Environment.NewLine + "  " + "2.风、雨、雪现场监测设备均安装于接触网杆上，具体里程可根据现场实际情况调整。");

                    if (StationToBeDraw.type.Contains("牵引"))
                    {
                        Shuoming.Append(System.Environment.NewLine + "3.电气化所亭内地震子系统的工程数量详见电气化所亭防灾安全监控系统设备平面布置图。");
                    }
                    // Create a multiline text object
                    MText acMText = new MText();
                    acMText.Location = new Point3d(innerStartPoint.X+10, innerStartPoint.Y + 110, 0);
                    acMText.Width = 115;
                    acMText.TextStyleId = fontStyleId;
                    acMText.Contents = Shuoming.ToString();

                    //绘制图纸名字
                    StringBuilder DrawNameString = new StringBuilder();
                    DrawNameString.Append("施工图" + System.Environment.NewLine + StationToBeDraw.type + "(" + StationToBeDraw.location + ")" + System.Environment.NewLine + "防灾安全监控系统电缆径路示意图");
                    MText DrawName = new MText();
                    DrawName.Location = new Point3d(innerStartPoint.X + 299, innerStartPoint.Y + 13.5, 0);
                    DrawName.TextStyleId = fontStyleId;
                    DrawName.TextHeight = 3;
                    DrawName.Width = 80;
                    DrawName.TextStyleId = fontStyleId;
                    DrawName.Contents = DrawNameString.ToString();
                    DrawName.Attachment = AttachmentPoint.MiddleCenter; //居中？

                    
                    //acBlkTblRec.AppendEntity(acMText);
                    db2.AddToModelSpace(acMText, DrawName); //插入文字
                    #endregion

                    #region 绘制多段线，表示线缆走向。同时绘制线缆名字

                    #endregion
                    db2.LineWeightDisplay = true;

                    LinetypeTable acLinTbl = trans2.GetObject(db2.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                    if (acLinTbl.Has("DASH") == true)
                    {
                        ObjectId loneTypeId = acLinTbl["DASH"];

                        Polyline[] mLines = drawMutiLine(registBoxInsertPoint, registStationInsertPoint, new Point3d(innerStartPoint.X + 116, innerEndPoint.Y - 123 + 1.1, 0), loneTypeId); //监控单元-接触网杆

                        Autodesk.AutoCAD.Colors.Color lineColor = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 0, 0);// 红色

                        Polyline[] mLinesGou =  drawMutiLine(registGouChaoInsertPoint, new Point3d(innerStartPoint.X+116, innerStartPoint.Y+167,0), 0.05,  lineColor); //站点-电缆沟
                        //防灾控制箱-设备 
                        Polyline[] mLines2 = drawMutiLine(registEquipeInsertPoint);
                        db2.AddToModelSpace(mLines);
                        db2.AddToModelSpace(mLines2);
                        db2.AddToModelSpace(mLinesGou);

                    }
                    else
                    {
                        MessageBox.Show("错误！缺少线型！");
                    }

                    #endregion



                    acBlkTbl.DowngradeOpen();
                    acBlkTblRec.DowngradeOpen();

                    trans2.Commit();

                    #endregion
                }
                catch (System.Exception ee)
                {

                    MessageBox.Show("出现错误！" + System.Environment.NewLine + "\t错误信息:" + ee.ToString(), "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trans2.Abort();
                }
            }

        }

        public Polyline[] drawMutiLine(List<List<Point3d>> registEquipeInsertPoint)
        {
            Polyline[] mLines = new Polyline[registEquipeInsertPoint.Count];
            int num = 0;
            foreach(List<Point3d> Mline in registEquipeInsertPoint)
            {
                Polyline pl = new Polyline();
                int node = 0;
                foreach(Point3d point in Mline)
                {
                    pl.AddVertexAt(node, new Point2d(point.X, point.Y), 0, 0.75, 0.75);
                    node++;
                }

                //.Color=Autodesk.AutoCAD.Colors.Color                
                pl.LinetypeScale = 20;
                pl.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255,159,127);
                pl.LineWeight = LineWeight.LineWeight025;//(LineWeight)20;
                mLines[num] = pl;
                num++;

            }

            return mLines;
        }

        public Polyline[] drawMutiLine(List<Point3d> registEquipeInsertPoint,double lineWidth, Autodesk.AutoCAD.Colors.Color linecolor)
        {
            Polyline[] mLines = new Polyline[1];
            int num = 0;

            Polyline pl = new Polyline();
            int node = 0;
            foreach (Point3d point in registEquipeInsertPoint)
            {
                pl.AddVertexAt(node, new Point2d(point.X, point.Y), 0, lineWidth, lineWidth);
                node++;
            }

            //.Color=Autodesk.AutoCAD.Colors.Color                
            pl.LinetypeScale = 20;
            pl.Color = linecolor;
            pl.LineWeight = LineWeight.LineWeight025;//(LineWeight)20;
            mLines[num] = pl;
            num++;



            return mLines;
        }


        /// <summary>
        /// 绘制由监控终端经由槽道至防灾控制箱的电缆示意图
        /// </summary>
        /// <param name="registEquipePoint">防灾控制箱示意图的位置列表</param>
        /// <param name="stationPoint">车站示意图的位置</param>
        /// <param name="ChaoDao">电缆示意图的拐点y坐标</param>
        /// <param name="lineType">多段线线型</param>
        /// <returns></returns>
        public Polyline[] drawMutiLine(List<Point3d> registEquipePoint, Point3d stationPoint, Point3d ChaoDao,ObjectId lineType)
        {
            Polyline[] mLines = new Polyline[registEquipePoint.Count];
            
            int num = 0;
            foreach (Point3d equipe in registEquipePoint)
            {
                Point3d vect1 = new Point3d(stationPoint.X, ChaoDao.Y + 1, 0); //所亭下方与槽道的交点
                Point3d vect2 = new Point3d(equipe.X, ChaoDao.Y + 1, 0);       //设备下方与槽道的交点

                Polyline pl = new Polyline();
                pl.AddVertexAt(0, new Point2d(stationPoint.X, stationPoint.Y), 0, 0.75, 0.75); //点1：车站
                pl.AddVertexAt(1, new Point2d(vect1.X, vect1.Y), 0, 0.75, 0.75);               //点2：车站下
                pl.AddVertexAt(2, new Point2d(vect2.X - 2.79, vect2.Y), 0, 0.75, 0.75);           //点3：设备下
                pl.AddVertexAt(3, new Point2d(equipe.X - 2.79, equipe.Y - 1), 0, 0.75, 0.75);     //点4：设备旁                
                pl.AddVertexAt(4, new Point2d(equipe.X, equipe.Y), 0, 0.75, 0.75);             //点5：设备旁
                pl.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(127, 255, 127);
                pl.LinetypeId = lineType;
                pl.LinetypeScale = 20;
                pl.LineWeight = LineWeight.LineWeight025;//(LineWeight)20;
                mLines[num] = pl;
                num++;
            }



            return mLines;
        }

        public Polyline[] drawMutiLine(List<Point3d> registEquipePoint, Point3d ChaoDao, double lineWidth, Autodesk.AutoCAD.Colors.Color lineColor)
        {
            Polyline[] mLines = new Polyline[registEquipePoint.Count];

            int num = 0;
            foreach (Point3d equipe in registEquipePoint)
            {
                Point3d vect2 = new Point3d(equipe.X, ChaoDao.Y + 1, 0);       //设备下方与槽道的交点

                Polyline pl = new Polyline();
                pl.AddVertexAt(0, new Point2d(equipe.X, equipe.Y), 0, lineWidth, lineWidth);
                pl.AddVertexAt(1, new Point2d(vect2.X, vect2.Y), 0, lineWidth, lineWidth);
                pl.Color = lineColor;
                pl.LinetypeScale = 20;
                pl.LineWeight = LineWeight.LineWeight025;//(LineWeight)20;
                mLines[num] = pl;
                num++;
            }



            return mLines;
        }

        public Entity insertText(string text, Point3d insertPoint, ObjectId fontStyleId)
        {
            DBText equipeText = new DBText();
            equipeText.Position = insertPoint;
            equipeText.Height = 4.5;
            equipeText.TextString = text;
            equipeText.HorizontalMode = TextHorizontalMode.TextCenter;
            equipeText.VerticalMode = TextVerticalMode.TextVerticalMid;
            equipeText.AlignmentPoint = equipeText.Position;
            equipeText.WidthFactor = 0.7;
            equipeText.TextStyleId = fontStyleId;
            return equipeText;
        }
        public Entity insertText(string text, Point3d insertPoint, double textHeight, TextHorizontalMode ALigneMode, ObjectId fontStyleId)
        {
            DBText equipeText = new DBText();
            equipeText.Position = insertPoint;
            equipeText.Height = textHeight;
            equipeText.TextString = text;
            equipeText.HorizontalMode = ALigneMode;
            equipeText.VerticalMode = TextVerticalMode.TextVerticalMid;
            equipeText.AlignmentPoint = equipeText.Position;
            equipeText.WidthFactor = 0.7;
            equipeText.TextStyleId = fontStyleId;
            return equipeText;
        }

        /// <summary>
        /// 将程序中的设备名称与块名称对应
        /// </summary>
        /// <param name="equipeName"></param>
        /// <returns></returns>
        public string transforBlockName(string equipeName)
        {
            string stantardName="";

            if (equipeName == "风速计")
            {
                stantardName = "风向风速计_G";
            }
            else if(equipeName == "雪深计")
            {
                stantardName = "雪深计_G";
            }
            else if(equipeName == "雨量计")
            {
                stantardName = "雨量计_G";
            }
            else if (equipeName == "基站")
            {
                stantardName = "GSM-R基站_G";
            }
            else if (equipeName == "车站")
            {
                stantardName = "车站_GS";
            }
            else if (equipeName.Contains("所"))
            {
                stantardName = "牵引变电所_G";
            }
            else
            {
                stantardName = equipeName;
            }

            return stantardName;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringTableName">表格名字</param>
        /// <param name="insertPoint">插入点</param>
        /// <param name="styleId">文字样式</param>
        /// <param name="tableContent"></param>
        /// <returns></returns>
        public Entity[] createNumberTable(string stringTableName, Point2d insertPoint, ObjectId styleId, List<string> tableContent)//Database db,Point2d insertPoint,Dictionary<string, string> ItemNumber)
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
            tableName.TextStyleId = styleId;

            #endregion

            #region 表格
            int tabCol = 6; //表格含6列

            Table tb = new Table();

            tb.SetSize(10, tabCol);
            //tb.NumRows = 10;

            //tb.NumColumns = tabCol;

            tb.SetRowHeight(6);

            //设置表格列宽 （cad2010)            
            tb.Columns[0].Width = 10;
            tb.Columns[1].Width = 27;
            tb.Columns[2].Width = 27;
            tb.Columns[3].Width = 13;
            tb.Columns[4].Width = 15;
            tb.Columns[5].Width = 12;


            tb.Position = new Point3d(insertPoint.X + 5, insertPoint.Y - 10, 0);

            // Create a 2-dimensional array

            // of our table contents

            //清空
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

            for (int row = 1; row < tableContent.Count + 1; row++)
            {
                for (int col = 1; col < tableContent[1].Split(new char[] { ',' }).Length + 1; col++)
                {
                    str[row, col] = tableContent[row - 1].Split(new char[] { ',' })[col - 1];
                }
            }

            

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
            ObjectId styleId = new ObjectId();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    //simsun.ttc 宋体
                    styleId = db.AddTextStyle("宋-0.7", "宋体.ttf");
                    //styleId = db.AddTextStyle("宋体-0.7", "simsun.ttc");
                    //styleId = db.AddTextStyle("宋体-0.7", "simsun.ttc", false, false, 134, 2 | 0);
                    styleId.SetTextStyleProp(3, 0.7, 0, false, false, false, AnnotativeStates.True, true);
                    trans.Commit();
                }
                catch (System.Exception ee)
                {

                    MessageBox.Show("出现错误！" + System.Environment.NewLine + "\t错误信息:" + ee.ToString(), "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trans.Abort();
                }
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
            drawFunction dF = new drawFunction();
            dF.CheckBlock( fontId); //db, trans,
        }

        /// <summary>
        /// 生成三级或四级签名块
        /// </summary>
        /// <param name="insertPoint">设置插入点，插入点为图框右下角</param>
        /// <param name="nameTable">签名块设置</param>
        /// <returns></returns>
        public void MakeSignatureTable(string nameTable, bool withoutScale, ObjectId fontId)
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
            text1.WidthFactor = 0.7;

            DBText text2 = new DBText();
            text2.Position = new Point3d(tqUpLeft.X + 80, tqUpLeft.Y - 5.22, 0);
            text2.Height = 3;
            text2.TextString = "工程名称";
            //text1.
            text2.HorizontalMode = TextHorizontalMode.TextCenter;
            text2.VerticalMode = TextVerticalMode.TextVerticalMid;
            text2.AlignmentPoint = text2.Position;
            text2.TextStyleId = fontId;
            text2.WidthFactor = 0.7;

            DBText text3 = new DBText();
            text3.Position = new Point3d(tqDownRight.X - 35 - 7.5, tqDownRight.Y + 24, 0);
            text3.Height = 3;
            text3.TextString = "图号";
            //text1.
            text3.HorizontalMode = TextHorizontalMode.TextCenter;
            text3.VerticalMode = TextVerticalMode.TextVerticalMid;
            text3.AlignmentPoint = text3.Position;
            text3.TextStyleId = fontId;
            text3.WidthFactor = 0.7;

            DBText text4 = new DBText();
            text4.Position = new Point3d(tqDownRight.X - 35 - 7.5, tqDownRight.Y + 17.5, 0);
            text4.Height = 3;
            text4.TextString = "比例尺";
            //text1.
            text4.HorizontalMode = TextHorizontalMode.TextCenter;
            text4.VerticalMode = TextVerticalMode.TextVerticalMid;
            text4.AlignmentPoint = text4.Position;
            text4.TextStyleId = fontId;
            text4.WidthFactor = 0.7;

            DBText text5 = new DBText();
            text5.Position = new Point3d(tqDownRight.X - 35 - 7.5, tqDownRight.Y + 10.5, 0);
            text5.Height = 3;
            text5.TextString = "日期";
            //text1.
            text5.HorizontalMode = TextHorizontalMode.TextCenter;
            text5.VerticalMode = TextVerticalMode.TextVerticalMid;
            text5.AlignmentPoint = text5.Position;
            text5.TextStyleId = fontId;
            text5.WidthFactor = 0.7;

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
            text7.TextStyleId = fontId;
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
            text8.TextStyleId = fontId;
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
            text9.TextStyleId = fontId;
            text9.WidthFactor = 0.7;

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

            //return TqLines;
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
            attProjetName.WidthFactor = 0.7;
            //表示属性定义的通用样式
            setStyleForAtt(attProjetName, 3, false);
            //对其点
            attProjetName.AlignmentPoint = new Point3d(-45, 32.22, 0);
            

            //表示门符号的属性定义
            AttributeDefinition attDrawName = new AttributeDefinition(Point3d.Origin, "xxx防-xx-xx", "图纸名称", "输入图纸名称：", fontId);
            attDrawName.TextStyleId = fontId;
            attDrawName.WidthFactor = 0.7;
            //表示属性定义的通用样式
            setStyleForAtt(attDrawName, 3, false);
            //对其点
            attDrawName.AlignmentPoint = new Point3d(-17.5, 24, 0);

            //表示门符号的属性定义
            AttributeDefinition attScale = new AttributeDefinition(Point3d.Origin, "1：100", "图纸比例", "输入图纸比例：", fontId);
            attScale.TextStyleId = fontId;
            attScale.WidthFactor = 0.7;
            //表示属性定义的通用样式
            setStyleForAtt(attScale, 3, invisibilityScale);
            //对其点
            attScale.AlignmentPoint = new Point3d(-17.5, 17.5, 0);

            //表示门符号的属性定义
            AttributeDefinition attYearMonth = new AttributeDefinition(Point3d.Origin, "xxxx.xx", "绘制日期", "输入图纸绘制日期：", fontId);
            attYearMonth.TextStyleId = fontId;
            attYearMonth.WidthFactor = 0.7;
            //表示属性定义的通用样式
            setStyleForAtt(attYearMonth, 3, false);
            //对其点
            attYearMonth.AlignmentPoint = new Point3d(-17.5, 10.5, 0);

            //表示门符号的属性定义
            AttributeDefinition attPage = new AttributeDefinition(Point3d.Origin, "第 x 张,共 x 张", "页数", "输入图纸页数：", fontId);
            attPage.TextStyleId = fontId;
            attPage.WidthFactor = 0.7;
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
            string filePath = getFilePath();

            if (!filePath.ToLower().Contains("template"))
            {
                ExplodingABlock();
            }

            else
            {
                MessageBox.Show("请先保存图形文件", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            }


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
            InforStatusLabel.Text = "";
            colName = new List<string>();
            colName.Add("里程");
            colName.Add("名称");
            colName.Add("类型");
            colName.Add("公里");
            #region 添加事件
            try
            {
                //添加commandEnded事件,当每个命令完成后触发
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.CommandEnded += new CommandEventHandler(doc_CommandEnded);                
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ee)
            {
                MessageBox.Show("发生错误!" + ee.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            #endregion

            #region 添加文件
            string currentFilePath= getFilePath();
            if (currentFilePath.ToLower().Contains("template"))
            {
                toolStripStatusLabel1.Text = "未保存的文件.";
                return;
            }
            else
            {
                addFiletoSystem(currentFilePath);  //新建xml文件
            }
            #endregion

            #region 从xml文件中读取wayPoint点
            if (xmlFilePath != null)
            {
                XmlFunction Xf = new XmlFunction();
                InfoStation = Xf.loadWayPoint(xmlFilePath + "\\setting.xml", "StationPointLists");
                InfoWindPoint = Xf.loadWayPoint(xmlFilePath + "\\setting.xml", "WindPointLists");
                InfoRainPoint = Xf.loadWayPoint(xmlFilePath + "\\setting.xml", "RainPointLists");
                InfoSnowPoint = Xf.loadWayPoint(xmlFilePath + "\\setting.xml", "SnowPointLists");

                #region 给各个dataGridView指定datasource
                if (tableST == null)
                {
                    tableST = new DataSet("FzProjet"); //创建‘防灾项目’表

                    stTable = tableST.Tables.Add("StationTable");//创建‘所亭’表
                    System.Data.DataColumn ST = stTable.Columns.Add(colName[0], typeof(string));// station.Key.ToUpper());
                    stTable.Columns.Add(colName[1], typeof(string));
                    stTable.Columns.Add(colName[2], typeof(string));
                    stTable.Columns.Add(colName[3], typeof(string));
                    stTable.PrimaryKey = new System.Data.DataColumn[] { ST };

                    WindTable = tableST.Tables.Add("WindTable");//创建‘风点’表
                    System.Data.DataColumn WT = WindTable.Columns.Add(colName[0], typeof(string));// station.Key.ToUpper());
                    WindTable.Columns.Add(colName[1], typeof(string));
                    WindTable.Columns.Add(colName[2], typeof(string));
                    WindTable.Columns.Add(colName[3], typeof(string));
                    WindTable.PrimaryKey = new System.Data.DataColumn[] { WT };

                    RainTable = tableST.Tables.Add("RainTable");//创建‘雨点’表
                    System.Data.DataColumn RT = RainTable.Columns.Add(colName[0], typeof(string));// station.Key.ToUpper());
                    RainTable.Columns.Add(colName[1], typeof(string));
                    RainTable.Columns.Add(colName[2], typeof(string));
                    RainTable.Columns.Add(colName[3], typeof(string));
                    RainTable.PrimaryKey = new System.Data.DataColumn[] { RT };

                    SnowTable = tableST.Tables.Add("SnowTable");//创建‘雪点’表
                    System.Data.DataColumn SnowT = SnowTable.Columns.Add(colName[0], typeof(string));// station.Key.ToUpper());
                    SnowTable.Columns.Add(colName[1], typeof(string));
                    SnowTable.Columns.Add(colName[2], typeof(string));
                    SnowTable.Columns.Add(colName[3], typeof(string));
                    SnowTable.PrimaryKey = new System.Data.DataColumn[] { SnowT };

                    EarthTable = tableST.Tables.Add("EarthTable");//创建‘地震点’表
                    System.Data.DataColumn EarthT = EarthTable.Columns.Add(colName[0], typeof(string));// station.Key.ToUpper());
                    EarthTable.Columns.Add(colName[1], typeof(string));
                    EarthTable.Columns.Add(colName[2], typeof(string));
                    EarthTable.Columns.Add(colName[3], typeof(string));
                    EarthTable.PrimaryKey = new System.Data.DataColumn[] { EarthT };

                }
                #endregion
                if (InfoStation.Count > 0)
                {
                    treeView1.Nodes.Clear(); //清空treeView
                    refreshTreeview(treeView1, InfoStation, true);  //刷新treeView1

                }

                stTable.Clear();
                Dictionary<double, string> tempDict = new Dictionary<double, string>();

                string licheng = "";
                tempDict = LocationToInt(InfoStation, out licheng);
                InfoStation.Clear();
                foreach (KeyValuePair<double, string> pair in tempDict)
                {
                    string[] p_Values = pair.Value.Split(new char[] { ',' });

                    string values = "";

                    for (int a = 0; a < p_Values.Length - 1; a++)
                    {
                        if (a == 0)
                        {
                            values = p_Values[a];
                        }
                        else
                        {

                            values = values + "," + p_Values[a];
                        }
                    }

                    InfoStation.Add(p_Values[p_Values.Length - 1], values);

                    //string name = pair.Value.Split(new char[] { ',' })[0];
                    //stTable.Rows.Add(pair.Key.ToUpper(), Loadvalue[0], Loadvalue[1], Loadvalue[2]);  //添加
                    //tempDict.Add(,InfoStation.Keys.ToString()+"-"+InfoStation.Values.ToString());
                }


                fileStationDataView(dataGridStation, colName, InfoStation, "StationTable");         //填充车站datagridView
                fileStationDataView(dataGridWind, tableST, colName, InfoWindPoint, "WindTable");         //填充风点datagridView
                fileStationDataView(dataGridRain, tableST, colName, InfoRainPoint, "RainTable");             //填充雨点datagridView
                fileStationDataView(dataGridSnow, tableST, colName, InfoSnowPoint, "SnowTable");           //填充雪点datagridView
                //fileStationDataView(dataGridearth, colName, new Dictionary<string, string>());          //填充地震点datagridView
                
            }

            #endregion
            
            #region 添加线型
            Form_LineType fmL = new Form_LineType(xmlFilePath+"\\setting.xml");
            fmL.defautLineType(xmlFilePath + "\\setting.xml");
            #endregion

            #region 读取线型
            XmlFunction xf = new XmlFunction();
            Dictionary<string, string> defautLineList = new Dictionary<string, string>();
            
            LineTypeInfo = xf.loadLineType(xmlFilePath+"\\setting.xml"); //读取的项
            #endregion

            /*
            #region 检查块
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    toolStripStatusLabel1.Text = "检查块";
                    DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();



                    trans.Commit();
                    m_DocumentLock.Dispose();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ee)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Message: " + ee.Message.ToString() + System.Environment.NewLine + "Source: " + ee.Source.ToString() + System.Environment.NewLine + "TargetSite: " + ee.TargetSite.ToString() + System.Environment.NewLine + "StackTrace: " + ee.StackTrace.ToString());
                }
            }
            #endregion*/
        }





        string FileName = "";
        /// <summary>
        /// 当文件状态将改变时触发，废弃。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void callback_DocumentManager_DocumentLockModeChanged(object sender, DocumentLockModeChangedEventArgs e)
        {
            if (e.GlobalCommandName == "QSAVE" || e.GlobalCommandName == "SAVE" || e.GlobalCommandName == "SAVEAS")
            {
                // do you code
                FileName= getFilePath();
                toolStripStatusLabel1.Text = FileName;
                string[] splitFileName = FileName.Split(new char[] { '\\' });
                FileName = splitFileName[splitFileName.Length - 1];
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

                //获取文件位置
                //当程序第一次运行

                lastLoadFilePath = newFilePath;
                addFiletoSystem(newFilePath); //检查并设置xml、icon文件夹

                /****废弃******/
                Form_LineType fLT = new Form_LineType(newFilePath + "\\setting.xml");
                fLT.TimeMarkUpdated += new Form_LineType.TimeMarkUpdateHandler(getTimeMark);
                /****废弃******/

                /*if (Local_lastLoadedLineTime == "" && Local_lastLoadedLineTime == fLT.dataTimeTag) //如果还没有读取linelist，切没有打开linetype界面
                {
                    lineList = fLT.defautLineType(xmlFilePath);//调用生成linetype界面的defautLineType功能更新时间戳
                    Local_lastLoadedLineTime = fLT.dataTimeTag; //统一时间一致
                }*/
                //}
                //else //当文件运行过程中已经保存过一次 to be continu
                //{
                //文件打开过程中更换库位置时

                //调用库位置变更功能
                //moveDictionary(newFilePath);
                //设置新库位置
                //filePath = newFilePath;

            }
            if (lastLoadFilePath != newFilePath)
            {
                //文件位置

            }

            Document doc = sender as Document;
            if (!doc.Editor.IsQuiescent) return;
            else { MessageBox.Show("doc IsQuiescent?"); }

            //DoSomeWork();
        }

        private void modifyXml(string path, string newName)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlNode rootNode = xmlDoc.SelectSingleNode("ProjetName");
            rootNode.InnerText = newName;
        }
        /// <summary>
        /// 设置变量、创建文件
        /// </summary>
        /// <param name="path"></param>
        private void addFiletoSystem(string path)
        {
            string[] directoryPaths = path.Split(new char[] { '\\' });
            string directory = "";
            for (int Npart = 0; Npart < directoryPaths.Length - 1; Npart++)
            {
                directory += directoryPaths[Npart];

                directory += "\\";

            }
            if (!Directory.Exists(directory + "\\setting"))
            {
                Directory.CreateDirectory(directory + "\\setting");
            }
            if (!Directory.Exists(directory + "\\icon"))
            {
                Directory.CreateDirectory(directory + "\\icon");
                
            }
            imgStoragePath = directory + "icon"; //icon位置
            xmlFilePath = directory + "setting";
            //创建配置文件
            if (!File.Exists(directory + "\\setting\\Setting.xml"))
            {
                //File.Create(path + "\\setting\\Setting.xml");
                createXml(directory + "\\setting\\Setting.xml");
            }
            //创建icon储存位置
            
            
        }
        /// <summary>
        /// 当图纸变更储存位置时，将setting文件及icon文件移动至新文件夹
        /// </summary>
        /// <param name="newFilePath"></param>
        private void moveDictionary(string newFilePath)
        {
            //todo
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("to be continue");
        }
    
        /// <summary>
        /// 获得当前文件储存位置
        /// </summary>
        /// <returns>文件位置</returns>
        public string getFilePath()
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
            string filePath = getFilePath();

            if (!filePath.ToLower().Contains("template"))
            {
                string CadFilePath = getFilePath();
                string[] Names = CadFilePath.Split(new char[1] { '\\' });
                string name = Names[Names.Length - 1];
                //写XML文件
                if (name != "acadiso.dwt")
                {
                    //xmlFilePath = "c://defaut.xml";
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
            else
            {
                MessageBox.Show("请先保存图形文件", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            }
        }

        private void 导入图块ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = getFilePath();

            if (!filePath.ToLower().Contains("template"))
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
                        if (imgStoragePath != "" || imgStoragePath != null)
                        {
                            Size defSize = new System.Drawing.Size(30, 30);
                            listView1.Clear();
                            fillImageList(imgStoragePath, defSize);
                            fileListView(imgStoragePath);

                            C_equipeType.Items.Clear(); //清除combobox项
                            DirectoryInfo Dir = new DirectoryInfo(imgStoragePath);
                            foreach (FileInfo f in Dir.GetFiles("*.bmp")) //查找文件
                            {
                                if (f.Name.ToString().Contains("图例"))
                                {

                                    C_equipeType.Items.Add(f.Name.ToString().Split(new char[] { '_' })[0]); //添加项 
                                }
                            }

                            
                        }
                        else
                        {
                            MessageBox.Show("指定块图案存储位置", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("请先先保存文件.", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            //#if DEBUG

            path = imgPath;//  StockLocation;

            //string b=
            //#else
            //path = "..\\Resourse\\";
            //#endif

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

                    foreach (string d in Directory.GetFileSystemEntries(path))
                    {
                        if (File.Exists(d))
                        {
                            FileInfo fi = new FileInfo(d);
                            if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                                fi.Attributes = FileAttributes.Normal;
                            File.Delete(d);//直接删除其中的文件  
                        }
                    }

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
                            // 获取块预览图案（适用于AutoCAD 2008及以下版本）
                            //preview = BlockThumbnailHelper.GetBlockThumbanail(btr.ObjectId);

                            preview = btRecord.PreviewIcon; // 适用于AutoCAD 2009及以上版本 

                            if (!getFileName(path).Contains(btRecord.Name.ToString()))
                            {
                                if (!btRecord.Name.ToString().Contains("图例"))
                                {
                                    preview.Save(path + "\\" + btRecord.Name + ".bmp"); // 保存块预览图案 

                                }

                            }
                            else
                            {

                            }
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ee)
                        {
                            trans.Abort();
                            ed.WriteMessage("错误;  " + ee.ToString());
                            //preview = btr.PreviewIcon; // 适用于AutoCAD 2009及以上版本
                        }


                    }
                    trans.Commit();
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

        public string getFileName(string flodePath)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@flodePath);
            StringBuilder sb = new StringBuilder();
            FileInfo[] dirInfo = theFolder.GetFiles();

            foreach (FileInfo NextFile in dirInfo)  //遍历文件
            {
                if (NextFile.Extension == "bmp")
                {
                    sb.Append(NextFile.Name.ToString());
                }
            }
            return sb.ToString();
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
             /*Database db = HostApplicationServices.WorkingDatabase;
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
            }*/

        }

        private void B_AddWayPoint_Click(object sender, EventArgs e)
        {
            string filePath = getFilePath();

            if (!filePath.ToLower().Contains("template"))
            {
                //修改功能关闭时
                if (modifData == false)
                {
                    #region 添加字典信息
                    if (TypeWayPoint.SelectedItem != null)
                    {
                        if (TypeWayPoint.SelectedItem.ToString() != "" && T_SLocation.Text != "" && T_SName.Text.ToString().Replace(" ","") != "")
                        {
                            PFunction pF=new PFunction();
                            if (!pF.isExMatch(T_SName.Text.ToString().Replace(" ", ""), @"^((\d*)\#*[\u4e00-\u9fa5]*)$")) //如果不是汉字
                            {
                                MessageBox.Show("站名： '" + T_SName.Text.ToString().Replace(" ", "") + "'格式不符合规范。");
                                return;
                            }
                            //if (!pF.isExMatch(T_SLocation.Text.ToString().Replace(" ", ""), @"^([A-Z]*)(\d*)([A-Z]*)(\d*).(\d{0,4})$"))   (\d+)+(\d{0,4})
                            if (!pF.isExMatch(T_SLocation.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})$")) //如果里程不符合规范
                            {
                                MessageBox.Show("里程： '" + T_SLocation.Text.ToString().Replace(" ", "") + "'格式不符合规范。");
                                return;
                            }
                            else if (!pF.isExMatch(T_SLocation.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})$") && TypeWayPoint.SelectedItem.ToString() != "桥梁") //如果里程不符合规范
                            {
                                MessageBox.Show("里程： '" + T_SLocation.Text.ToString().Replace(" ", "") + "'格式不符合规范。");
                                return;
                            }
                            string sLocation = "";
                            string sName = T_SName.Text.ToString().Replace(" ","");
                            string sType = TypeWayPoint.SelectedItem.ToString().Replace(" ", "");

                            #region 如果录入桥梁里程时添加一个textbox
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
                                            if (!pF.isExMatch(temp.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})$") && TypeWayPoint.SelectedItem.ToString() == "桥梁") //如果里程不符合规范
                                            {
                                                MessageBox.Show("里程： '" + T_SLocation.Text.ToString().Replace(" ", "") + "'格式不符合规范。");
                                                return;
                                            }
                                            locationPart2 = temp.Text.ToString().ToUpper().Replace(" ", "");
                                        }
                                    }
                                }

                                sLocation = transferDistanceToNumberToString(T_SLocation.Text.ToString().ToUpper().Replace(" ", "")) + "-" + transferDistanceToNumberToString(locationPart2);
                            }
                            else
                            {
                                sLocation = transferDistanceToNumberToString(T_SLocation.Text.ToString().ToUpper().Replace(" ", ""));
                            }
                            #endregion

                            //添加字典项
                            if (!InfoStation.ContainsKey(sLocation))
                            {
                                InfoStation.Add(sLocation, sName + "," + sType);
                                

                                List<string> colName = new List<string>();
                                

                               
                                if (InfoStation.Count > 0)
                                {
                                    #region 读写xml-wayPoint
                                    XmlFunction xF = new XmlFunction();

                                    xF.addWayPointNode(xmlFilePath + "\\setting.xml", "StationPointLists", "StationPoints", InfoStation);
                                    if (sType.Contains("牵引变电所")|| sType.Contains("AT所")|| sType.Contains("分区所")) //如果是牵引变电所、分区所、AT所则默认安装地震仪，写入信息
                                    {
                                        PFunction pf = new PFunction();
                                        List<string> listLoc = new List<string>();

                                        pf.isExMatch(sLocation, @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                                        double distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);
                                        InfoEarthPoint.Add(sLocation, sName + "," + "地震仪" + "," + distance);
                                        xF.addWayPointNode(xmlFilePath + "\\setting.xml", "EarthPointLists", "EarthPoints", InfoEarthPoint);
                                    }


                                    Dictionary<string, string> loadedInfor = xF.loadWayPoint(xmlFilePath + "\\setting.xml","StationPointLists");
                                    #endregion

                                    //
                                    if (stTable != null)
                                    {
                                        stTable.Clear();
                                    }
                                    
                                    Dictionary<double, string> tempDict = new Dictionary<double, string>();

                                    string licheng = "";
                                    tempDict = LocationToInt(loadedInfor, out licheng);
                                    InfoStation.Clear();
                                    foreach (KeyValuePair<double, string> pair in tempDict)
                                    {
                                        string[] p_Values = pair.Value.Split(new char[] { ',' });

                                        string values = "";

                                        for (int a = 0; a < p_Values.Length - 1; a++)
                                        {
                                            if (a == 0)
                                            {
                                                values = p_Values[a];
                                            }
                                            else
                                            {

                                                values = values + "," + p_Values[a];
                                            }
                                        }

                                        InfoStation.Add(p_Values[p_Values.Length - 1], values);

                                        //string name = pair.Value.Split(new char[] { ',' })[0];
                                        //stTable.Rows.Add(pair.Key.ToUpper(), Loadvalue[0], Loadvalue[1], Loadvalue[2]);  //添加
                                        //tempDict.Add(,InfoStation.Keys.ToString()+"-"+InfoStation.Values.ToString());
                                    }


                                    dataGridStation.DataSource = "";

                                    //dataGridStation(st)
                                    fileStationDataView(dataGridStation, colName, InfoStation, "StationTable");
                                    //fileStationDataView(dataGridStation, colName, loadedInfor, "StationTable"); //填充datagridView 

                                    treeView1.Nodes.Clear();
                                    refreshTreeview(treeView1, loadedInfor, true);

                                    //如果数据插入成功，清空各输入栏
                                    T_SLocation.Text = "";
                                    T_SName.Text = "";
                                    TypeWayPoint.SelectedItem = "";
                                }
                                
                            }
                            else
                            {
                                MessageBox.Show("已定义"+System.Environment.NewLine +"里程为:'" + sLocation+"'" + System.Environment.NewLine + "名称为:'" + sName + "'的路点.");
                            }
                        }

                        else
                        {
                            MessageBox.Show("请录入所有信息.", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    #endregion


                    
                }
                else //modifData==true的时候。修改功能开启时，变更dictionary中数据
                {
                    #region 修改字典信息

                    PFunction pF = new PFunction();
                    if (!pF.isExMatch(T_SName.Text.ToString().Replace(" ", ""), @"^((\d*)\#*[\u4e00-\u9fa5]*)")) //如果不是汉字
                    {
                        MessageBox.Show("站名： " + T_SName.Text.ToString().Replace(" ", "") + "'格式不符合规范。");
                        return;
                    }
                    if (!pF.isExMatch(T_SLocation.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})$") && TypeWayPoint.SelectedItem.ToString() != "桥梁") //如果里程不符合规范
                    {
                        MessageBox.Show("里程： '" + T_SLocation.Text.ToString().Replace(" ", "") + "'格式不符合规范。");
                        return;
                    }
                    else if (!pF.isExMatch(T_SLocation.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})-([A-Z]+)(\d+)\+(\d{0,4})$") && TypeWayPoint.SelectedItem.ToString() == "桥梁") //如果里程不符合规范
                    {
                        MessageBox.Show("里程： '" + T_SLocation.Text.ToString().Replace(" ", "") + "'格式不符合规范。");
                        return;
                    }

                    
                    //treeview选中项
                    TreeNode selectNode = treeView1.SelectedNode;

                    List<string> LNT = new List<string>();
                    if (selectNode.Level == 1)
                    {
                        LNT = treeViewFunction(selectNode);
                    }
                    else if (selectNode.Level == 2)
                    {
                        LNT = treeViewFunction(selectNode.Parent);

                    }

                    //为3个变量赋值
                    string location = "";

                    //
                    string name = T_SName.Text.ToString().Replace(" ", "");
                    string type = TypeWayPoint.SelectedItem.ToString();
                    if (type != "桥梁")
                    {
                        location = transferDistanceToNumberToString(T_SLocation.Text.ToString().ToUpper().Replace(" ", ""));
                    }
                    else
                    {
                        location = transferDistanceToNumberToString(T_SLocation.Text.ToString().ToUpper().Replace(" ", "")) + "-" + transferDistanceToNumberToString(tempText.Text.ToString().ToUpper().Replace(" ", ""));
                    }

                    if (LNT[2] == type && LNT[1] == name && LNT[0] == location) //如果数据没有变
                    {
                        InforStatusLabel.Text = "项目数据没有改变.";
                        return;
                    }
                    else //如果数据改变了
                    {
                        InfoStation.Remove(LNT[0]);
                        PFunction pf = new PFunction();
                        List<string> listLoc = new List<string>();
                        pf.isExMatch(LNT[0], @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                        double distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);
                        InfoStation.Add(location.Split(new char[]{'-'})[0], name + "," + type + "," + distance);

                        //修改xml文件
                        //


                        XmlFunction xf = new XmlFunction();
                        //supprimWayPoint(xmlFilePath + "\\setting.xml", inforOneStation[0].ToString().Replace(" ", ""));
                        //旧字典
                        List<string> oldDic = new List<string>();
                        oldDic.Add(LNT[0] + "," + LNT[1] + "," + LNT[2]);
                        //新字典
                        List<string> newDic = new List<string>();
                        newDic.Add(location + "," + name + "," + type);
                        xf.modifWayPoint(xmlFilePath + "\\setting.xml", oldDic, newDic);


                        stTable.Clear();
                        Dictionary<double, string> tempDict = new Dictionary<double, string>();

                        string licheng = "";
                        tempDict = LocationToInt(InfoStation, out licheng);
                        InfoStation.Clear();
                        foreach (KeyValuePair<double, string> pair in tempDict)
                        {
                            string[] p_Values = pair.Value.Split(new char[] { ',' });

                            string values = "";

                            for (int a = 0; a < p_Values.Length - 1; a++)
                            {
                                if (a == 0)
                                {
                                    values = p_Values[a];
                                }
                                else
                                {

                                    values = values + "," + p_Values[a];
                                }
                            }
                            InfoStation.Add(p_Values[p_Values.Length - 1], values);

                            //string name = pair.Value.Split(new char[] { ',' })[0];
                            //stTable.Rows.Add(pair.Key.ToUpper(), Loadvalue[0], Loadvalue[1], Loadvalue[2]);  //添加
                            //tempDict.Add(,InfoStation.Keys.ToString()+"-"+InfoStation.Values.ToString());
                        }

                        //清除treeview
                        treeView1.Nodes.Clear();
                        refreshTreeview(treeView1, InfoStation, true);
                        dataGridStation.DataSource = "";

                        //dataGridStation(st)
                        fileStationDataView(dataGridStation, colName, InfoStation, "StationTable");
                        modifData = false;
                        B_AddWayPoint.Text = "+";
                        B_SupWayPoint.Text = "-";

                        T_SLocation.Text = "";
                        T_SName.Text = "";
                        TypeWayPoint.SelectedItem = "";

                    }
                    #endregion
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


                }
            }
            else
            {
                MessageBox.Show("请先保存图形文件", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        /// <summary>
        /// 将里程转换为数字，再转换为字符串，避免出现 ‘01米的问题
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public string transferDistanceToNumberToString(string distance)
        {
            PFunction pF = new PFunction();
            List<string> listLoc = new List<string>();
            pF.isExMatch(distance, @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);
            //检验数字格式
            int mile = int.Parse(listLoc[1]);
            int meter = int.Parse(listLoc[2]);

            return listLoc[0] + mile + "+" + meter;
        }

        

        //生成xml文件
        public void createXml(string CreatXmlFilePath)
        {
            string filepath = getFilePath();
            string[] strings = filepath.Split(new char[] { '\\' });
            string PName = strings[strings.Length - 1];

            //生成setting文件
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("Projet");
            xmlDoc.AppendChild(root);

            XmlNode node1= CreateNode(xmlDoc, root, "ProjetInfor", "");
            CreateNode(xmlDoc, node1, "ProjetName","?");
            CreateNode(xmlDoc, node1, "PictureName", "?");
            CreateNode(xmlDoc, node1, "ChapterName", "?");
            CreateNode(xmlDoc, node1, "ProjectShortName", "?");

            XmlNode WayPointNode = CreateNode(xmlDoc, root, "WayPoints", ""); //所亭
            CreateNode(xmlDoc, WayPointNode, "StationPointLists", ""); //所亭
            CreateNode(xmlDoc, WayPointNode, "WindPointLists", "");//风点
            CreateNode(xmlDoc, WayPointNode, "RainPointLists", "");//雨点
            CreateNode(xmlDoc, WayPointNode, "SnowPointLists", "");//雪点
            CreateNode(xmlDoc, WayPointNode, "EarthPointLists", "");//地震点


            CreateNode(xmlDoc, root, "Connection", ""); //监控设备与站点的连接信息
            
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
        public XmlNode CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
            return node;
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
        /// <param name="component">要刷新的控件</param>
        /// <param name="dictionaryToFill">要用来填充的字典项</param>
        /// <param name="NoL">true时站名为主节点，false时里程为主节点（废弃）</param>
        public void refreshTreeview(Control component,Dictionary<string,string> dictionaryToFill, bool NoL)
        {
            TreeView treeVIewToRefresh = (TreeView)component;
            TreeNode rootStation = new TreeNode();  //添加车站
            rootStation.Text = "车站";
            treeVIewToRefresh.Nodes.Add(rootStation);

            TreeNode rootTeleTower = new TreeNode();  //添加基站
            rootTeleTower.Text = "基站";
            treeVIewToRefresh.Nodes.Add(rootTeleTower);

            TreeNode rootAT = new TreeNode();  //添加AT所
            rootAT.Text = "AT所";
            treeVIewToRefresh.Nodes.Add(rootAT);

            TreeNode rootQY = new TreeNode();  //添加牵引变电所
            rootQY.Text = "牵引变电所";
            treeVIewToRefresh.Nodes.Add(rootQY);

            TreeNode rootFQ = new TreeNode();  //添加牵引变电所
            rootFQ.Text = "分区所";
            treeVIewToRefresh.Nodes.Add(rootFQ);
            //rootQY.Nodes.Add(rootQY);
            //InfoStation=InfoStation.
            int numS = 0;
            int numJ = 0;
            int numA = 0;
            int numQ = 0;
            int numF = 0;
            foreach (var item in InfoStation)
            {
                if (NoL==true)
                {
                    TreeNode nameNode = new TreeNode();
                    nameNode.Text = item.Value.Split(new char[] { ',' })[0];

                    TreeNode disNode = new TreeNode();
                    disNode.Text = item.Key;

                    TreeNode typeNode = new TreeNode();
                    typeNode.Text = item.Value.Split(new char[] { ',' })[1];

                    nameNode.Nodes.Add(disNode);

                    nameNode.Nodes.Add(typeNode);

                    if (item.Value.Split(new char[] { ',' })[1]=="车站")
                    {
                        rootStation.Nodes.Add(nameNode);
                        numS++;
                    }
                    else if (item.Value.Split(new char[] { ',' })[1] == "基站")
                    {
                        rootTeleTower.Nodes.Add(nameNode);
                        numJ++;
                    }
                    else if (item.Value.Split(new char[] { ',' })[1] == "AT所")
                    {
                        rootAT.Nodes.Add(nameNode);
                        numA++;
                    }
                    else if (item.Value.Split(new char[] { ',' })[1] == "牵引变电所")
                    {
                        rootQY.Nodes.Add(nameNode);
                        numQ++;
                    }
                    else if (item.Value.Split(new char[] { ',' })[1] == "分区所")
                    {
                        rootFQ.Nodes.Add(nameNode);
                        numF++;
                    }
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

            rootStation.Text = rootStation.Text + " 共有：" + numS + "个";
            rootTeleTower.Text = rootTeleTower.Text + " 共有：" + numJ + "个";
            rootAT.Text = rootAT.Text + " 共有：" + numA + "个";
            rootQY.Text = rootQY.Text + " 共有：" + numQ + "个";
            rootFQ.Text = rootFQ.Text + " 共有：" + numF + "个";
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            supprimeTreeView(selectedNode);
        }

        /// <summary>
        /// 删除选中路点信息功能
        /// </summary>
        bool modifData = false;
        private void supprimeTreeView(TreeNode selectNode)
        {
            if (treeView1.SelectedNode!=null)
            {

                XmlFunction xf = new XmlFunction();
                if (selectNode.Level == 1)
                {
                    //TreeNode selectKeyNode = selectNode.LastNode;
                    List<string> inforOneStation = new List<string>();
                    inforOneStation = treeViewFunction(selectNode);
                    if (MessageBox.Show("确定删除项:\n里程:" + inforOneStation[0].ToString() + "\n类型:" + inforOneStation[2].ToString() + "\n站名:" + inforOneStation[1].ToString(), "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        
                        xf.supprimWayPoint(xmlFilePath + "\\setting.xml", inforOneStation[0].ToString().Replace(" ", ""));
                        treeView1.Nodes.Clear();
                        InfoStation.Remove(inforOneStation[0].ToString());
                        System.Data.DataTable stationData = tableST.Tables["StationTable"];

                        int removeIndex = 0;
                        foreach (DataRow row in stationData.Rows)
                        {
                            //row
                            if (row.ItemArray.Contains(inforOneStation[0].ToString().Replace(" ", "")))
                            {
                                stationData.Rows[removeIndex].Delete();
                                stationData.AcceptChanges();
                                break;
                            }
                            removeIndex++;

                        }
                        refreshTreeview(treeView1, InfoStation, true);
                    }
                }
                else if (selectNode.Level > 1)
                {
                    List<string> inforOneStation = new List<string>();
                    inforOneStation = treeViewFunction(selectNode.Parent);

                    if (MessageBox.Show("确定删除项:\n里程:" + inforOneStation[0].ToString() + "\n类型:" + inforOneStation[2].ToString() + "\n站名:" + inforOneStation[1].ToString(), "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                            treeView1.Nodes.Clear();
                            xf.supprimWayPoint(xmlFilePath + "\\setting.xml", inforOneStation[0].ToString().Replace(" ", ""));
                            InfoStation.Remove(inforOneStation[0].ToString());

                            refreshTreeview(treeView1, InfoStation, true);
                        }

                }
                else if (selectNode.Level == 0)
                {
                    MessageBox.Show("此节点：" + treeView1.SelectedNode.Text.ToString().Split(new char[] { ' ' })[0] + "。不能删除！", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("未选择项目.", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //
        public List<string> treeViewFunction(TreeNode selectedNode)
        {
            List<string> LNT = new List<string>(); //里程＼名字＼类型
            string licheng = "";
            string Name = "";
            string TypeS = "";
            PFunction pF = new PFunction();
            foreach (TreeNode childNode in selectedNode.Nodes)
            {
                if (pF.isExMatch(childNode.Text.ToString().Replace(" ", ""), @"^((\d*)\#*[\u4e00-\u9fa5]*)") || childNode.Text.ToString().Replace(" ", "").Contains("AT所")) //汉字
                {
                    if (childNode.Text.ToString().Replace(" ", "").Contains("AT所") || childNode.Text.ToString().Replace(" ", "").Contains("牵引变电所") || childNode.Text.ToString().Replace(" ", "").Contains("车站") || childNode.Text.ToString().Replace(" ", "").Contains("基站") || childNode.Text.ToString().Replace(" ", "").Contains("桥梁"))
                    {
                        TypeS = childNode.Text.ToString();
                    }
                    else
                    {
                        Name = childNode.Text.ToString();
                    }
                }
                else if (pF.isExMatch(childNode.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})$"))
                {
                    licheng = childNode.Text.ToString();
                } 
            }

            if (pF.isExMatch(selectedNode.Text.ToString().Replace(" ", ""), @"^((\d*)\#*[\u4e00-\u9fa5]*)"))
            {
                if (!selectedNode.Text.ToString().Contains("所") || !selectedNode.Text.ToString().Contains("站"))
                {
                    Name = selectedNode.Text.ToString();
                }
            }
            LNT.Add(licheng);
            LNT.Add(Name);
            LNT.Add(TypeS);

            return LNT;
        }

        /// <summary>
        /// 删除treeview中选中的路点信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_SupWayPoint_Click(object sender, EventArgs e)
        {
            string filePath = getFilePath();

            if (!filePath.ToLower().Contains("template"))
            {
                if (modifData == false)
                {
                    //treeView1.SelectedNode;
                    删除ToolStripMenuItem_Click(null, null);

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
            else
            {
                MessageBox.Show("请先保存图形文件", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
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

        //保存要修改项的原数值
        //string registName = "";
        //string registLocation = "";
        //string registType = "";

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count > 3) //默认有三个节点
            {
                //清空
                T_SName.Text = "";
                T_SLocation.Text = "";
                TypeWayPoint.SelectedItem = "";

                modifData = true;
                B_AddWayPoint.Text = "确认";
                B_SupWayPoint.Text = "取消";

                TreeNode selectedNode = treeView1.SelectedNode;

                TreeNode SLocaNode = new TreeNode();
                TreeNode SNameNode = new TreeNode();
                TreeNode STypeNode = new TreeNode();

                PFunction pF = new PFunction();

                if (selectedNode.Level == 1)
                {
                    foreach (TreeNode childNode in selectedNode.Nodes)
                    {
                        if (pF.isExMatch(childNode.Text.ToString().Replace(" ", ""), @"^((\d*)\#*[\u4e00-\u9fa5]*)")) //汉字
                        {
                            if (childNode.Text.ToString().Replace(" ", "") == "AT所" || childNode.Text.ToString().Replace(" ", "") == "牵引变电所" || childNode.Text.ToString().Replace(" ", "") == "车站" || childNode.Text.ToString().Replace(" ", "") == "基站" || childNode.Text.ToString().Replace(" ", "") == "桥梁")
                            {
                                TypeWayPoint.SelectedItem = childNode.Text.ToString();
                            }
                            else
                            {
                                T_SName.Text = childNode.Text.ToString();
                            }
                        }
                        else if (pF.isExMatch(childNode.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})$"))
                        {
                            T_SLocation.Text = childNode.Text.ToString();
                        }
                    }
                    if (pF.isExMatch(selectedNode.Text.ToString().Replace(" ", ""), @"^((\d*)\#*[\u4e00-\u9fa5]*)"))
                    {
                        if (!selectedNode.Text.ToString().Contains("所") || !selectedNode.Text.ToString().Contains("站"))
                        {
                            T_SName.Text = selectedNode.Text.ToString();
                        }
                    }



                }
                else if (selectedNode.Level == 2)
                {
                    foreach (TreeNode childNode in selectedNode.Parent.Nodes)
                    {
                        if (pF.isExMatch(childNode.Text.ToString().Replace(" ", ""), @"^((\d*)\#*[\u4e00-\u9fa5]*)")) //汉字
                        {
                            if (childNode.Text.ToString().Replace(" ", "") == "AT所" || childNode.Text.ToString().Replace(" ", "") == "牵引变电所" || childNode.Text.ToString().Replace(" ", "") == "车站" || childNode.Text.ToString().Replace(" ", "") == "基站" || childNode.Text.ToString().Replace(" ", "") == "桥梁")
                            {
                                TypeWayPoint.SelectedItem = childNode.Text.ToString();
                            }
                            else
                            {
                                T_SName.Text = childNode.Text.ToString();
                            }
                        }
                        else if (pF.isExMatch(childNode.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})$"))
                        {
                            T_SLocation.Text = childNode.Text.ToString();
                        }
                    }
                    if (pF.isExMatch(selectedNode.Parent.Text.ToString().Replace(" ", ""), @"^((\d*)\#*[\u4e00-\u9fa5]*)"))
                    {
                        if (!selectedNode.Text.ToString().Contains("所") || !selectedNode.Text.ToString().Contains("站"))
                        {
                            T_SName.Text = selectedNode.Parent.Text.ToString();
                        }
                    }
                    else if (pF.isExMatch(selectedNode.Parent.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})$"))
                    {
                        T_SLocation.Text = selectedNode.Parent.Text.ToString();
                    }

                }
                else if (selectedNode.Level ==0)
                {
                    MessageBox.Show("此节点：" + selectedNode.Text.ToString().Split(new char[] { ' ' })[0] + "。不能修改！", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
        }
          
        ImageList imageList1;
        List<string> filename;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmgFilesLocation"></param>
        /// <param name="imgSize"></param>
        private void fillImageList(string bmgFilesLocation, Size imgSize)
        {
            DirectoryInfo Dir = new DirectoryInfo(@bmgFilesLocation);
            imageList1 = new ImageList();
            filename = new List<string>();
            try
            {

                foreach (FileInfo f in Dir.GetFiles("*.bmp")) //查找文件
                {
                    imageList1.Images.Add(Bitmap.FromFile(@"" + bmgFilesLocation + "\\" + f));
                    imageList1.ImageSize = imgSize;
                    filename.Add(f.ToString().Split('.')[0]);
                    //imageListSmall.Images.Add(Bitmap.FromFile(@"..\..\绘图.bmp"));
                }
            }



            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmgFilesLocation"></param>
        private void fileListView(string bmgFilesLocation)
        {
            //to do           

            this.listView1.View = View.LargeIcon;
            this.listView1.LargeImageList = imageList1;

            this.listView1.BeginUpdate();
            for (int i = 0; i < imageList1.Images.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.ImageIndex = i;

                lvi.Text = filename[i] + ""; //imageList1.Images[i].;

                this.listView1.Items.Add(lvi);
            }

            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。  
        }




        /// <summary>
        /// 填充站点信息
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="listGridName"></param>
        /// <param name="dictionyToFill"></param>
        /// <param name="tableName"></param>
        public void fileStationDataView(Control comp, List<string> listGridName, Dictionary<string, string> dictionyToFill, string tableName)
        {
            DataGridView componant = (DataGridView)comp;
            System.Data.DataTable a = tableST.Tables["StationTable"];
            //stTable.Clear();

            foreach (var station in dictionyToFill)
            {
                if (station.Value.ToString().Contains("所") || station.Value.ToString().Contains("站"))
                {
                    if (!stTable.Rows.Contains(station.Key.ToUpper())) //如果datatable中不含有所亭项 
                    {
                        string[] Loadvalue = station.Value.ToString().Split(new char[] { ',' });

                        stTable.Rows.Add(station.Key.ToUpper(), Loadvalue[0] , Loadvalue[1], Loadvalue[2]);  //添加
                    }

                }
                else
                {

                }
            }
            //stTable.DefaultView.Sort = "公里" + " " + "ASC";
            //DataView dv = new DataView(stTable);
            //dv.Sort = "公里" + " " + "ASC";
            //stTable.DefaultView.ToTable();

            //componant.cle

            stTable.AcceptChanges();
            componant.DataSource = tableST.Tables[tableName];

            //componant.Sort(componant.Columns["公里"], ListSortDirection.Ascending);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="listGridName"></param>
        /// <param name="dictionyToFill"></param>
        /// <param name="tableName"></param>
        public void fileStationDataView(Control comp, System.Data.DataSet tableFill, List<string> listGridName, Dictionary<string, string> dictionyToFill, string tableName)
        {
            DataGridView componant = (DataGridView)comp;

            System.Data.DataTable table = tableFill.Tables[tableName];
            table.Clear();
            foreach (var station in dictionyToFill)
            {

                if (!table.Rows.Contains(station.Key.ToUpper())) //如果datatable中不含有所亭项 
                {
                    string[] a=station.Value.ToString().Split(new char[]{','});  //以‘，’号分割
                    table.Rows.Add(station.Key.ToUpper(), a[0], a[1], a[2]);  //添加
                }


            }
            table.DefaultView.Sort = "公里" + " " + "ASC";

            DataView dv = new DataView(table);
            dv.Sort = "公里" + " " + "ASC";


            stTable.DefaultView.ToTable();
            table.AcceptChanges();
            //dataGridView1.Columns.Add("里程", "名称");
            //dataGridView1.Columns.Add("EnglishName", "ChineseName"); 
            componant.DataSource = tableFill.Tables[tableName];
            componant.Sort(componant.Columns["公里"], ListSortDirection.Ascending);

            /*dataGridView1.Columns.Add("设备1","equipe1");
            dataGridView1.Columns.Add("设备2", "equipe2");
            dataGridView1.Columns.Add("设备3", "equipe3");
            dataGridView1.Columns.Add("设备4", "equipe4");
            dataGridView1.Columns.Add("设备5", "equipe5");*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string selectNode = tabControl1.SelectedTab.Text.ToString();
            XmlFunction xF = new XmlFunction();

            if (selectNode.Contains("风"))
            {
                try
                {
                    if (MessageBox.Show("确认要保存风速计列表么", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        //addAttSignaTable
                        System.Data.DataTable windT = tableST.Tables["WindTable"];

                        int rowNum = 1;
                        //检查数据格式
                        foreach (var item in WindTable.Rows)
                        {
                            DataRow rowItem = (DataRow)item;
                            string location = rowItem.ItemArray[0].ToString().ToUpper().Replace(" ", "");
                            rowItem[0] = location;


                            string name = rowItem.ItemArray[1].ToString();

                            if (name == "")
                            {
                                name = "风速计";
                                rowItem[1] = name;
                            }
                            else if (name == "风速计,风速计")
                            {
                                name = "风速计";
                                rowItem[1] = name;
                            }

                            PFunction pF = new PFunction();

                            //名字可以为空，里程不能为空
                            if (location != "")
                            {
                                if (!pF.isExMatch(location, @"^([A-Z]+)(\d+)\+(\d{0,4})$")) //里程格式
                                {
                                    MessageBox.Show("数据：" + location + "不符合规范", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                                //if (!pF.isExMatch(name, @"^([\u4e00-\u9fa5]*)\d*$"))  //
                                {
                                    //MessageBox.Show("数据：" + name + "不符合规范", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    //return;
                                }

                                if (!InfoWindPoint.ContainsKey(location))
                                {
                                    InfoWindPoint.Add(location, name + ",风速计"); //添加到字典

                                }
                            }
                            else
                            {

                                MessageBox.Show("第" + rowNum + "行数据里程数为空", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            rowNum++;


                        }
                        Dictionary<string, string> tempD = new Dictionary<string, string>();
                        foreach(KeyValuePair<string,string> pair in InfoWindPoint)
                        {
                            string key = pair.Key;
                            
                            List<string> listLoc = new List<string>();
                            PFunction pF = new PFunction();
                            pF.isExMatch(key, @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                            double distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);

                            string value = pair.Value;
                            if (!value.Contains("" + distance))
                            {
                                value = pair.Value + "," + distance;
                            }
                            //InfoWindPoint.Remove(key);
                            tempD.Add(key, value);
                        }
                        InfoWindPoint.Clear();
                        InfoWindPoint = LocationToIntToString(tempD);
                        dataGridWind.DataSource="";
                        fileStationDataView(dataGridWind, tableST, colName, InfoWindPoint, "WindTable");

                        //WindTable = sortedWindDiction;
                        WindTable.AcceptChanges();

                        xF.supprimWayPoint(xmlFilePath + "\\setting.xml", "WindPointLists", ""); //移除WindPointLists下所有子节点
                        xF.addWayPointNode(xmlFilePath + "\\setting.xml", "WindPointLists", "WindPoints", InfoWindPoint);

                        
                    }
                }
                catch (System.Exception ee)
                {
                    MessageBox.Show("处理风点数据时发生错误" + ee.ToString(), "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                //dataGridWind.DataSource
                //editDataSet=da
            }
            else if(selectNode.Contains("雨"))
            {
                try
                {
                    if (MessageBox.Show("确认要保存雨量计列表么", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        System.Data.DataTable RainT = tableST.Tables["RainTable"];
                        int rowNum = 1;
                        //检查数据格式
                        foreach (var item in RainTable.Rows)
                        {
                            DataRow rowItem = (DataRow)item;
                            string location = rowItem.ItemArray[0].ToString().ToUpper().Replace(" ", "");
                            rowItem[0] = location;

                            string name = rowItem.ItemArray[1].ToString();
                            if (name == "")
                            {
                                name = "雨量计";
                                rowItem[1] = name;
                            }
                            else if (name == "雨量计,雨量计")
                            {
                                name = "雨量计";
                                rowItem[1] = name;
                            }
                            PFunction pF = new PFunction();

                            //名字可以为空，里程不能为空
                            if (location != "")
                            {
                                if (!pF.isExMatch(location, @"^([A-Z]+)(\d+)\+(\d{0,4})$")) //里程格式
                                {
                                    MessageBox.Show("数据：" + location + "不符合规范", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                                if (!pF.isExMatch(name, @"^([\u4e00-\u9fa5]*)\d*$"))  //
                                {
                                    MessageBox.Show("数据：" + name + "不符合规范", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                                if (!InfoRainPoint.ContainsKey(location))
                                {
                                    InfoRainPoint.Add(location, name + ",雨量计"); //添加到字典

                                }
                            }
                            else
                            {

                                MessageBox.Show("第" + rowNum + "行数据里程数为空", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            rowNum++;

                        }

                        Dictionary<string, string> tempD = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, string> pair in InfoRainPoint)
                        {
                            string key = pair.Key;

                            List<string> listLoc = new List<string>();
                            PFunction pF = new PFunction();
                            pF.isExMatch(key, @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                            double distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);

                            string value = pair.Value;
                            if (!value.Contains("" + distance))
                            {
                                value = pair.Value + "," + distance;
                            }
                            //InfoWindPoint.Remove(key);
                            tempD.Add(key, value);
                        }
                        InfoRainPoint.Clear();
                        InfoRainPoint = LocationToIntToString(tempD);

                        dataGridRain.DataSource = null; //清空
                                                        //RainTable.sor
                        fileStationDataView(dataGridRain, tableST, colName, InfoRainPoint, "RainTable");

                        //WindTable = sortedWindDiction;
                        RainTable.AcceptChanges();

                        xF.supprimWayPoint(xmlFilePath + "\\setting.xml", "RainPointLists", ""); //移除RainPointLists下所有子节点
                        xF.addWayPointNode(xmlFilePath + "\\setting.xml", "RainPointLists", "RainPoints", InfoRainPoint);

                    }
                }
                catch (System.Exception ee)
                {
                    MessageBox.Show("处理雨点数据时发生错误" + ee.ToString(), "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            else if(selectNode.Contains("雪"))
            {
                try
                {
                    if (MessageBox.Show("确认要保存雪深计列表么", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        System.Data.DataTable snowT = tableST.Tables["SnowTable"];

                        int rowNum = 1;
                        //检查数据格式
                        foreach (var item in SnowTable.Rows)
                        {
                            DataRow rowItem = (DataRow)item;
                            string location = rowItem.ItemArray[0].ToString().ToUpper().Replace(" ", "");
                            rowItem[0] = location;

                            string name = rowItem.ItemArray[1].ToString();

                            if (name == "")
                            {
                                name = "雪深计";
                                rowItem[1] = name;
                            }
                            else if (name == "雪深计,雪深计")
                            {
                                name = "雪深计";
                                rowItem[1] = name;
                            }

                            PFunction pF = new PFunction();

                            //名字可以为空，里程不能为空
                            if (location != "")
                            {
                                if (!pF.isExMatch(location, @"^([A-Z]+)(\d+)\+(\d{0,4})\d*$")) //里程格式
                                {
                                    MessageBox.Show("数据：" + location + "不符合规范", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                                if (!pF.isExMatch(name, @"^((\d*)\#*[\u4e00-\u9fa5]*)"))  //
                                {
                                    MessageBox.Show("数据：" + name + "不符合规范", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                                if (!InfoSnowPoint.ContainsKey(location))
                                {
                                    InfoSnowPoint.Add(location, name + ",雪深计"); //添加到字典

                                }
                            }
                            else
                            {

                                MessageBox.Show("第" + rowNum + "行数据里程数为空", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            rowNum++;

                        }

                        Dictionary<string, string> tempD = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, string> pair in InfoSnowPoint)
                        {
                            string key = pair.Key;

                            List<string> listLoc = new List<string>();
                            PFunction pF = new PFunction();
                            pF.isExMatch(key, @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                            double distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);

                            string value = pair.Value;
                            if (!value.Contains("" + distance))
                            {
                                value = pair.Value + "," + distance;
                            }
                            //InfoWindPoint.Remove(key);
                            tempD.Add(key, value);
                        }

                        InfoSnowPoint.Clear();
                        InfoSnowPoint = LocationToIntToString(tempD);

                        fileStationDataView(dataGridSnow, tableST, colName, InfoSnowPoint, "SnowTable");

                        //WindTable = sortedWindDiction;
                        SnowTable.AcceptChanges();

                        xF.supprimWayPoint(xmlFilePath + "\\setting.xml", "SnowPointLists", ""); //移除SnowPointLists下所有子节点
                        xF.addWayPointNode(xmlFilePath + "\\setting.xml", "SnowPointLists", "SnowPoints", InfoSnowPoint);
                    }
                }
                catch (System.Exception ee)
                {
                    MessageBox.Show("处理雪点数据时发生错误" + ee.ToString(), "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
            else if(selectNode.Contains("地震"))
            {
                //不支持变更
                if (true)
                {

                    try
                    {
                        if (MessageBox.Show("确认要保存地震仪列表么", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {

                            System.Data.DataTable EarthT = tableST.Tables["EarthTable"];
                            int rowNum = 1;
                            //检查数据格式
                            foreach (var item in EarthTable.Rows)
                            {
                                DataRow rowItem = (DataRow)item;
                                string location = rowItem.ItemArray[0].ToString().ToUpper().Replace(" ", "");
                                rowItem[0] = location;

                                string name = rowItem.ItemArray[1].ToString();
                                if (name == "")
                                {
                                    name = "地震仪";
                                    rowItem[1] = name;
                                }
                                else if (name == "地震仪,地震仪")
                                {
                                    name = "地震仪";
                                    rowItem[1] = name;
                                }
                                PFunction pF = new PFunction();

                                //名字可以为空，里程不能为空
                                if (location != "")
                                {
                                    if (!pF.isExMatch(location, @"^([A-Z]+)(\d+)\+(\d{0,4})$")) //里程格式
                                    {
                                        MessageBox.Show("里程：" + location + "不符合规范", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                    if (!pF.isExMatch(name, @"^([\u4e00-\u9fa5]*)\d*$"))  //
                                    {
                                        MessageBox.Show("名称：" + name + "不符合规范", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                    if (!InfoRainPoint.ContainsKey(location))
                                    {
                                        InfoRainPoint.Add(location, name + ",地震仪"); //添加到字典

                                    }
                                }
                                else
                                {

                                    MessageBox.Show("第" + rowNum + "行数据里程数为空", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                                rowNum++;

                            }

                            Dictionary<string, string> tempD = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, string> pair in InfoEarthPoint)
                            {
                                string key = pair.Key;

                                List<string> listLoc = new List<string>();
                                PFunction pF = new PFunction();
                                pF.isExMatch(key, @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                                double distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);

                                string value = pair.Value;
                                if (!value.Contains("" + distance))
                                {
                                    value = pair.Value + "," + distance;
                                }
                                //InfoWindPoint.Remove(key);
                                tempD.Add(key, value);
                            }

                            InfoEarthPoint.Clear();
                            InfoEarthPoint = LocationToIntToString(tempD);


                            dataGridearth.DataSource = null; //清空
                                                             //RainTable.sor
                            fileStationDataView(dataGridearth, tableST, colName, InfoEarthPoint, "EarthTable");

                            //WindTable = sortedWindDiction;
                            EarthTable.AcceptChanges();

                            xF.supprimWayPoint(xmlFilePath + "\\setting.xml", "EarthPointLists", ""); //移除EarthPointLists下所有子节点
                            xF.addWayPointNode(xmlFilePath + "\\setting.xml", "EarthPointLists", "EarthPoints", InfoEarthPoint);

                        }
                    }
                    catch (System.Exception ee)
                    {
                        MessageBox.Show("处理地震点数据时发生错误" + ee.ToString(), "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                }

            }

            else if (selectNode.Contains("列表"))
            {

            }
            /*
            if (tableST != null)
            {
                tableST.AcceptChanges();
                oldTableSt = tableST;
            }
            */
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tableST.RejectChanges();
        }



        private void B_refresh_Click(object sender, EventArgs e)
        {

        }



        /// <summary>
        /// 在右侧treeview中添加block信息子节点
        /// </summary>
        List<string> selectListBlock;
        private void listView1_Click(object sender, EventArgs e)
        {
            
            
        }

        string selectDataGrid = "";

        /// <summary>
        /// 每在datagridview中选中一个非空项，就在右侧treeview中添加一个node。一次只显示一个node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_Click(object sender, EventArgs e)
        {
            if (false)
            {
                if(dataGridStation.CurrentRow != null) //如果选中项不为空
                {
                    int nRow = dataGridStation.CurrentRow.Index; //选中行

                    string selectLocation = dataGridStation.Rows[nRow].Cells[0].Value.ToString();

                    string selectName = dataGridStation.Rows[nRow].Cells[1].Value.ToString().Split(new char[] { ',' })[0];
                    string selectType = dataGridStation.Rows[nRow].Cells[1].Value.ToString().Split(new char[] { ',' })[1];


                    if (selectType != "车站")
                    {
                        //每次选中更新字段
                        selectDataGrid = selectLocation + "-" + selectName + selectType;

                        //填充selectedWayPoint表，一次只能显示一个

                        //清空treeview
                        //selectedWayPoint.Nodes.Clear();
                        //添加项到连接表
                        bool inTheList = false;
                        TreeNode NodeIndex = new TreeNode();

                        //便利列表，检查是否datagridView中选中项是否已经存在于treeview中
                        foreach (TreeNode node in selectedWayPoint.Nodes)
                        {
                            if (node.Level == 0)
                            {
                                if (node.Text == selectDataGrid)
                                {
                                    inTheList = true; //存在于treeview中
                                    NodeIndex = node;
                                    break;
                                }
                            }
                        }
                        if (!inTheList) //如果不存在
                        {
                            TreeNode firstNode = new TreeNode();
                            firstNode.Text = selectDataGrid;
                            selectedWayPoint.Nodes.Add(firstNode); //添加
                        }
                        else
                        {
                            selectedWayPoint.SelectedNode = NodeIndex; //如果选中项的项存在于treeView中，则treeView选中对应项
                        }
                    }


                }

            }
        }

        /// <summary>
        /// 检查选中项是否已经存在于treeview中
        /// </summary>
        /// <param name="checkString"></param>
        /// <returns></returns>
        public bool checkTreeView(string checkString)
        {
            bool isDuplicate = false;

            foreach (TreeNode node in selectedWayPoint.Nodes)
            {
                if (node.Level == 0)
                {
                    if (node.Text.ToUpper() == checkString)
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }

            return isDuplicate;
        }
        

        /// <summary>
        /// 是否使用待定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_AddEquipe_Click(object sender, EventArgs e)
        {

            if (selectedWayPoint.SelectedNode!=null)
            {
                //toolStripStatusLabel1.Text = "选中" + listView1.SelectedItems.Count + "个块";

                selectListBlock = new List<string>();

                //label7.Text = selectedWayPoint.Nodes.ToString();
                TreeNode rootNode=new TreeNode(); //第一级节点。本次设计只有2级节点。 

                rootNode = selectedWayPoint.SelectedNode.Parent;  //选中节点的父节点
                if (rootNode == null)    //如果选中项不存在父节点，则该节点为第一级节点。
                {
                    rootNode = selectedWayPoint.SelectedNode;
                }

                TreeNode Node = new TreeNode();
                TreeNodeCollection listNode;
                listNode = selectedWayPoint.Nodes;
                foreach (TreeNode node in selectedWayPoint.Nodes)  //选中父节点的所有子节点
                {
                    
                    if (node.FullPath.Contains(rootNode.Name.ToString())) //如果fullpath中有
                    {
                        Node = node;
                        break;
                    }
                }

                if (Node!=null)
                {
                    //当不为空时
                    if (textBox1.Text.ToString() != "" && C_equipeType.SelectedItem.ToString() != "" && C_Line.SelectedItem.ToString() != "" && C_equipeType.SelectedItem.ToString() != null && C_Line.SelectedItem.ToString() != null)
                    {
                        PFunction pF = new PFunction();
                        if (!pF.isExMatch(textBox1.Text.ToString().Replace(" ", ""), @"^((\d*)\#*[\u4e00-\u9fa5]*)")) //如果不是汉字
                        {
                            MessageBox.Show("站名： " + T_SName.Text.ToString().Replace(" ", "") + "格式不符合规范。");
                            return;
                        }
                        //if (!pF.isExMatch(T_SLocation.Text.ToString().Replace(" ", ""), @"^([A-Z]*)(\d*)([A-Z]*)(\d*).(\d{0,4})$"))   (\d+)+(\d{0,4})
                        if (!pF.isExMatch(textBox2.Text.ToString().ToUpper().Replace(" ", ""), @"^([A-Z]+)(\d+)\+(\d{0,4})$")) //如果里程不符合规范
                        {
                            MessageBox.Show("站名： " + T_SName.Text.ToString().Replace(" ", "") + "格式不符合规范。");
                            return;
                        }
                        //检查是否已经在表里
                        bool duplicate = false;
                        foreach (TreeNode node in listNode)
                        {
                            if (node.FullPath.Contains(textBox1.Text.ToUpper().Replace(" ", "") + "-" + C_equipeType.SelectedItem.ToString().Replace(" ", "")))
                            {
                                duplicate = true;
                                break;
                            }
                        }
                        selectListBlock.Add(textBox1.Text.ToUpper().Replace(" ", "") + "-" + C_equipeType.SelectedText.ToString());

                        if (!duplicate)
                        {
                            TreeNode equipeNode = new TreeNode(); //设备接点
                            equipeNode.Text = "设备: " + textBox1.Text.ToUpper().Replace(" ", "") + "-" + C_equipeType.SelectedItem.ToString().Replace(" ", "");
                            rootNode.Nodes.Add(equipeNode);


                            TreeNode childNode1 = new TreeNode();
                            childNode1.Text = "里程: " + textBox1.Text.ToUpper().Replace(" ", "");
                            equipeNode.Nodes.Add(childNode1);

                            TreeNode childNode2 = new TreeNode();
                            childNode2.Text = "线缆: " + C_Line.SelectedItem.ToString().Replace(" ", "");
                            equipeNode.Nodes.Add(childNode2);


                            //清空
                            textBox1.Text = "";
                            //C_equipeType.SelectedText = "";
                            //C_Line.SelectedText = "";
                        }
                        else
                        {
                            //清空
                            textBox1.Text = "";
                        }

                    }
                    else
                    {
                        MessageBox.Show("右侧面板数据不能为空。", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } 
                }

            }
            else
            {
                MessageBox.Show("没有选中所亭。");
            }


        }

        /// <summary>
        /// 清空selectedWayPoint treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_CanChange_Click(object sender, EventArgs e)
        {
            selectedWayPoint.Nodes.Clear();
        }

        private void 可选线缆类型ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (xmlFilePath != null || xmlFilePath != "")
            {
                Form_LineType LT = new Form_LineType(xmlFilePath + "\\setting.xml");
                LT.ShowDialog();
            }
        }

        private void selectedWayPoint_Click(object sender, EventArgs e)
        {
            if (selectedWayPoint.SelectedNode != null)
            {
                if (selectedWayPoint.SelectedNode.Level > 0) //l选中项非第一级node事
                {
                    //TreeNode parentNode = selectedWayPoint.SelectedNode.Parent;

                    //TreeNode parentNode = selectedWayPoint.SelectedNode.Parent;

                    //label7.Text = parentNode.Name;
                }
                else if (selectedWayPoint.SelectedNode.Level == 0)
                {
                    //label7.Text = selectedWayPoint.SelectedNode.Name;
                }
            }
        }

        public void getTimeMark(object sender, AutoDraw.Form_LineType.TimeMarkUpdateEventArgs e)
        {
            lineList = e.markTheTime;
            
        }
        string Local_lastLoadedLineTime = "";
        string remotTimeMark = "";
        Dictionary<string, string> lineList;
        List<string> listLine;
        private void C_Line_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form_LineType fLT = new Form_LineType(xmlFilePath);

            if (C_Line.SelectedText == "") { return; }
            else
            {
                //添加事件
               /* fLT.TimeMarkUpdated += new Form_LineType.TimeMarkUpdateHandler(getTimeMark);
                if (Local_lastLoadedLineTime == "" && Local_lastLoadedLineTime == fLT.dataTimeTag) //如果还没有读取linelist，切没有打开linetype界面
                {
                    lineList = fLT.defautLineType(xmlFilePath);//调用生成linetype界面的defautLineType功能更新时间戳
                    Local_lastLoadedLineTime = fLT.dataTimeTag; //统一时间一致
                */
                    //listLine = new List<string>();
                XmlFunction xF = new XmlFunction();
                lineList = xF.loadLineType(xmlFilePath + "\\setting.xml"); //每次都要查询，耗时很大
                    foreach (var line in lineList)
                    {
                        listLine.Add(line.Key.ToString());
                        C_Line.Items.Add(line.Key.ToString()); //填充combobox
                    }
                //}
                /*else if (Local_lastLoadedLineTime != fLT.dataTimeTag) //如果上一次更新listtype后更新过线缆类型
                {
                    XmlFunction xf = new XmlFunction();
                    lineList = xf.loadLineType(xmlFilePath);
                }*/

            }

        }

        /// <summary>
        /// 第一次点击用来填充列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void C_Line_Click(object sender, EventArgs e)
        {
            if (C_Line.Items.Count == 0)
            {
                XmlFunction xF = new XmlFunction();

                if (xF.loadLineType(xmlFilePath + "\\setting.xml") != null) 
                {
                    lineList = xF.loadLineType(xmlFilePath + "\\setting.xml");
                }
                else
                {
                    Form_LineType fLT = new Form_LineType(xmlFilePath + "\\setting.xml");
                    lineList = fLT.defautLineType(xmlFilePath + "\\setting.xml");//调用生成linetype界面的defautLineType功能更新时间戳
                    Local_lastLoadedLineTime = fLT.dataTimeTag; //统一时间一致
                }


                listLine = new List<string>();
                foreach (var line in lineList)
                {
                    listLine.Add(line.Key.ToString());
                    C_Line.Items.Add(line.Key.ToString()); //填充combobox
                }

                C_Line.Click -= new EventHandler(C_Line_Click);
            }
        }

        private void C_equipeType_SelectedIndexChanged(object sender, EventArgs e)
        {

            //如果选中下列设备，则里程自动变为选中项的里程，且textbox不能更改
            if (C_equipeType.SelectedText == "地震传感器" || C_equipeType.SelectedText == "信号中继站" || C_equipeType.SelectedText == "监控单元" || C_equipeType.SelectedText == "防灾控制箱")
            {
                int nRow = dataGridStation.CurrentRow.Index; //选中行

                string selectLocation = dataGridStation.Rows[nRow].Cells[0].Value.ToString();

                textBox1.Text = selectLocation; //
                if (textBox1.Enabled == true)
                {
                    textBox1.Enabled = false; 
                }
            }
            else
            {
                if (textBox1.Enabled == false)
                {
                    textBox1.Enabled = true; 
                }
            }
        }

        private void B_writeXML_Click(object sender, EventArgs e)
        {
            if (false)
            {
                XmlFunction xF = new XmlFunction();
                PFunction pF = new PFunction();
                foreach (TreeNode rootNode in selectedWayPoint.Nodes)
                {
                    string STInfo = rootNode.Text.ToString();
                    List<string> ListEquipe = new List<string>();
                    foreach (TreeNode ChildNode in rootNode.Nodes)
                    {
                        string name = ChildNode.Text.ToString().Split(new char[] { '-' })[1];
                        //name = name.Split(new char[] { ':' })[1];
                        string location = "";
                        string line = "";
                        string num = "";
                        foreach (TreeNode AttributeNode in ChildNode.Nodes)
                        {


                            if (AttributeNode.Text.Contains("+"))
                            {
                                location = AttributeNode.Text.ToString().Split(new char[] { ':' })[1];
                                continue;
                            }
                            else if (AttributeNode.Text.Contains("数量"))
                            {
                                num = AttributeNode.Text.ToString().Split(new char[] { ':' })[1];
                                continue;
                            }
                            else
                            {
                                line = AttributeNode.Text.ToString().Split(new char[] { ':' })[1];
                                continue;
                            }

                        }
                        ListEquipe.Add(location + "-" + num + "-" + line + "-" + name);

                    }
                    
                } 
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (InfoStation.Count > 0)
            {
                Dictionary<double, string> sortedDic;
                //里程换算成数字 ，排序. 含所有的waypoint
                sortedDic = LocationToInt(InfoStation); 

                Dictionary<double, string> dicWithoutStation = dicWithOutType(sortedDic, "车站");

                //生成除了车站外的所有路点
                //添加List<string>作为索引
                int count = 0;
                List<double> dicIndex = new List<double>();
                //生成sortedDic表的索引
                foreach (var item in sortedDic)
                {
                    //KeyValuePair
                    dicIndex.Add(item.Key);
                    count++;
                }
                foreach(var item in dicWithoutStation)
                {
                    //每一项的index
                    int itemIndex = dicIndex.IndexOf(item.Key);

                    string leftStation = "";
                    string rightStation = "";
                    //如果所亭的两端都有站
                    if (itemIndex > 0 && itemIndex < sortedDic.Count - 1)
                    {
                        leftStation = sortedDic[dicIndex[itemIndex - 1]];
                        rightStation = sortedDic[dicIndex[itemIndex + 1]];
                    }
                    //所亭在站点列表中排第一
                    else if(itemIndex == 0)
                    {
                        rightStation = sortedDic[dicIndex[itemIndex + 1]];
                    }
                    //所亭在站点列表中排最后
                    else if (itemIndex < sortedDic.Count - 1)
                    {
                        leftStation = sortedDic[dicIndex[itemIndex - 1]];
                    }

                    //绘制车站圆形图块

                }
            }
        }

        public void drawStationMark()
        {

        }

        public Dictionary<double, string> dicWithOutType(Dictionary<double, string> dic,string type)
        {
            Dictionary<double, string> dicWithoutType = new Dictionary<double, string>();

            foreach(var item in dic)
            {
                if (!item.Value.ToString().Contains(type))
                {
                    dicWithoutType.Add(item.Key, item.Value);
                }
            }
            //由于兼容性问题，放弃linq方式，
            /*var withoutType= (from objDic in dic
                             where !objDic.Value.ToString().Contains(type)
                              select objDic);
            foreach (KeyValuePair<double, string> kvp in withoutType)
            {
                dicWithoutType1.Add(kvp.Key, kvp.Value);
            }*/
            return dicWithoutType;
        }

        /// <summary>
        /// 里程格式的字符串换算为数字，并返回排序后的列表
        /// </summary>
        /// <param name="origDic"></param>
        /// <returns></returns>
        public Dictionary<double, string> LocationToInt(Dictionary<string,string> origDic)
        {

            Dictionary<double, string> distanceWP = new Dictionary<double, string>();
            foreach (var wayPoint in origDic)
            {
                PFunction pF=new PFunction();
                List<string> listLoc = new List<string>();
                pF.isExMatch(wayPoint.Key,@"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                double distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);
                distanceWP.Add(distance, wayPoint.Value + "," + wayPoint.Key);
            }
            Dictionary<double, string> Sortdic = DictonarySort(distanceWP);
            return Sortdic;
        }

        /// <summary>
        /// 里程格式的字符串换算为数字，并返回排序后的列表
        /// </summary>
        /// <param name="origDic"></param>
        /// <returns></returns>
        public Dictionary<double, string> LocationToInt(Dictionary<string, string> origDic,out string licheng)
        {
            licheng = "";
            Dictionary<double, string> distanceWP = new Dictionary<double, string>();
            foreach (var wayPoint in origDic)
            {
                PFunction pF = new PFunction();
                List<string> listLoc = new List<string>();
                double distance;
                string[] a=wayPoint.Key.Split(new char[] { '-' });
                if (wayPoint.Key.Split(new char[] { '-' }).Length == 1) //格式为dk1+1
                {
                    pF.isExMatch(wayPoint.Key, @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                    distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);
                }
                else//格式为dk1+1-dk1+2
                {
                    pF.isExMatch(wayPoint.Key.Split(new char[] { '-' })[0], @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                    distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);

                    pF.isExMatch(wayPoint.Key.Split(new char[] { '-' })[1], @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                    double distance2= Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);
                    //distance = distance + "-" + distance2;
                }

                distanceWP.Add(distance, wayPoint.Value + "," + wayPoint.Key);  //并没有用的样子
                licheng = listLoc[0];
            }
            Dictionary<double, string> Sortdic = DictonarySort(distanceWP);
            
            return Sortdic;
        }
        /// <summary>
        /// 里程格式的字符串换算为数字，并返回排序后的列表
        /// </summary>
        /// <param name="origDic">原字典</param>
        /// <returns>排序后的字典</returns>
        public Dictionary<string, string> LocationToIntToString(Dictionary<string, string> origDic)
        {

            Dictionary<double, string> distanceWP = new Dictionary<double, string>();
            foreach (var wayPoint in origDic)
            {
                PFunction pF = new PFunction();
                List<string> listLoc = new List<string>();
                pF.isExMatch(wayPoint.Key.ToString(), @"^([A-Z]+)(\d+)\+(\d{0,4})$", out listLoc);

                double distance = Int32.Parse(listLoc[1]) * 1000 + Int32.Parse(listLoc[2]);
                distanceWP.Add(distance, wayPoint.Key + "," + wayPoint.Value);
            }
            Dictionary<double, string> SortNumdic = DictonarySort(distanceWP);

            Dictionary<string, string> Sortdic = new Dictionary<string, string>();
            foreach (var pair in SortNumdic)
            {
                string[] location = pair.Value.ToString().Split(new char[] { ',' });
                Sortdic.Add(location[0], location[1] + "," + location[2] + "," + location[3]);
            }

            return Sortdic;
        }

        /// <summary>
        /// .net3.5引入linq以后的字典排序
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        private Dictionary<double, string> DictonarySort(Dictionary<double , string > dic)
        {
            Dictionary<double, string> Sortdic = new Dictionary<double, string>();
            var dicSort =
                (from objDic in dic
                 orderby objDic.Key ascending
                 select objDic);
            foreach (KeyValuePair<double, string> kvp in dicSort)
            {
                Sortdic.Add(kvp.Key, kvp.Value);
            }
            return Sortdic;
        }

        public string IsNumberInString(string str)
        {
            string returnStr = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {

                if (Char.IsNumber(str, i) == true)
                {
                    returnStr += str.Substring(i, 1);
                }
                else
                {
                    if (str.Substring(i, 1) == "A" || str.Substring(i, 1) == " ")
                    {
                        returnStr += str.Substring(i, 1);
                    }
                }

            }
            return returnStr;
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 根据加入的规则筛选站点信息
        /// </summary>
        /// <param name="originStationDictionary"></param>
        /// <returns></returns>
        public Dictionary<string, string> applyRule(Dictionary<string, string> originStationDictionary)
        {
            Dictionary<string, string> sortedDictionary = new Dictionary<string, string>();

            //添加规则读取
            //

            foreach(KeyValuePair<string,string> ValuePair in originStationDictionary)
            {
                //
                //添加
                //

                if (ValuePair.Key != "" && !ValuePair.Value.ToString().Replace(" ", "").Contains(""))
                {
                    sortedDictionary.Add(ValuePair.Key, ValuePair.Value);
                }
            }
            return sortedDictionary;
        }

        /// <summary>
        /// 自动生成连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count!=0)
            {

                if (comboFournisseur.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("请选择设备供应商！", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                XmlFunction XF = new XmlFunction();

                if (XF.readProjrtInfo(xmlFilePath + "\\setting.xml") == null || XF.readProjrtInfo(xmlFilePath + "\\setting.xml")[0] == "?")
                {
                    MessageBox.Show("请先设置图纸名称。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }
                    #region 读取数据
                    Dictionary<string, string> origStation = XF.loadWayPoint(xmlFilePath + "\\setting.xml", "StationPointLists");
                Dictionary<string, string> STstationPoint = new Dictionary<string, string>();
                Dictionary<string, string> RailstationPoint = new Dictionary<string, string>();
                foreach (var par in origStation)
                {
                    if (par.Value.ToString().Contains("所") || par.Value.ToString().Contains("基站"))
                    {
                        STstationPoint.Add(par.Key, par.Value);
                    }
                    else
                    {
                        RailstationPoint.Add(par.Key, par.Value);
                    }
                }
                origStation = applyRule(origStation); //根据新增规则筛选站点

                Dictionary<string, string> windPoint = XF.loadWayPoint(xmlFilePath + "\\setting.xml", "WindPointLists");
                Dictionary<string, string> rainPoint = XF.loadWayPoint(xmlFilePath + "\\setting.xml", "RainPointLists");
                Dictionary<string, string> snowPoint = XF.loadWayPoint(xmlFilePath + "\\setting.xml", "SnowPointLists");
                Dictionary<string, string> earthPoint = XF.loadWayPoint(xmlFilePath + "\\setting.xml", "EarthPointLists");
                #endregion

                #region 生成二维数组
                int stationNum = STstationPoint.Count();

                int[,] tableWindLengt = new int[windPoint.Count, stationNum]; //生成风距离表
                int[,] tableRainLengt = new int[rainPoint.Count, stationNum]; //生成雨距离表
                int[,] tableSnowLengt = new int[snowPoint.Count, stationNum]; //生成雪距离表

                //填充风距离表
                int W_w = 0;
                int W_h = 0;

                int R_w = 0;
                int R_h = 0;


                int S_w = 0;
                int S_h = 0;

                //填充风点,计算风点与站点间距
                foreach (var Sstation in STstationPoint)
                {
                    W_w = 0;
                    foreach (var wind in windPoint)
                    {
                        tableWindLengt[W_w, W_h] = (int.Parse(Sstation.Value.ToString().Split(new char[] { ',' })[2]) - int.Parse(wind.Value.ToString().Split(new char[] { ',' })[2]));
                        W_w++;
                    }
                    W_h++;
                }

                //填充雨距离表,计算雨点与站点间距
                foreach (var Sstation in STstationPoint)
                {
                    R_w = 0;
                    foreach (var rain in rainPoint)
                    {
                        tableRainLengt[R_w, R_h] = (int.Parse(Sstation.Value.ToString().Split(new char[] { ',' })[2]) - int.Parse(rain.Value.ToString().Split(new char[] { ',' })[2]));
                        R_w++;
                    }
                    R_h++;
                }

                //填充雪距离表,计算雪点与站点间距
                foreach (var Sstation in STstationPoint)
                {
                    S_w = 0;
                    foreach (var snow in snowPoint)
                    {
                        tableSnowLengt[S_w, S_h] = (int.Parse(Sstation.Value.ToString().Split(new char[] { ',' })[2]) - int.Parse(snow.Value.ToString().Split(new char[] { ',' })[2]));
                        S_w++;
                    }
                    S_h++;
                }
                #endregion

                List<string> WindToStation = findNearestStation(tableWindLengt);//找到离每个风点最近的基站、所亭
                List<string> RainToStation = findNearestStation(tableRainLengt);//找到离每个雨点最近的基站、所亭
                List<string> SnowToStation = findNearestStation(tableSnowLengt);//找到离每个雪点最近的基站、所亭

                List<string> Station_Equi_List = new List<string>(); //各设备离最近所亭列表
                List<string> SEWindPair = StationToEquipInfo(WindToStation, STstationPoint, windPoint);//生成各个风点最近站点、所亭列表
                List<string> SERainPair = StationToEquipInfo(RainToStation, STstationPoint, rainPoint);//生成各个雨点最近站点、所亭列表
                List<string> SESnowPair = StationToEquipInfo(SnowToStation, STstationPoint, snowPoint);//生成各个雪点最近站点、所亭列表

                List<string> connectionDict = new List<string>();

                //汇总风、雨、雪设备的‘站点_设备'表
                foreach (var wind in SEWindPair)
                {
                    Station_Equi_List.Add(wind);
                }
                foreach (var rain in SERainPair)
                {
                    Station_Equi_List.Add(rain);
                }
                foreach (var snow in SESnowPair)
                {
                    Station_Equi_List.Add(snow);
                }

                //写入连接情况
                //XmlFunction xF = new XmlFunction();
                XF.removeConnection(xmlFilePath + "\\setting.xml"); //删除所有connection节点

                selectedWayPoint.Nodes.Clear(); //清空点

                #region 读取线缆规则类型
                LineTypeInfo = XF.loadLineType(xmlFilePath + "\\setting.xml"); //读取线缆类型表

                List<ClassStruct.LineType> lineOfFournisseur = new List<ClassStruct.LineType>();
                //Dictionary<string, string> LineByFournisseur = new Dictionary<string, string>();
                foreach(KeyValuePair<string,string> linelist in LineTypeInfo)
                {
                    if (linelist.Value.Contains(comboFournisseur.SelectedItem.ToString()))  //根据下拉框选择的设备商名字筛选线型规则
                    { 
                        //LineByFournisseur.Add(linelist.Key, linelist.Value);
                        string[] splitValue = linelist.Value.Split(new char[] { ',' });


                        ClassStruct.LineType lineDefinition = new ClassStruct.LineType(splitValue[1], linelist.Key, splitValue[4], double.Parse(splitValue[2]), splitValue[3], double.Parse(splitValue[5]), double.Parse(splitValue[6]));

                        lineOfFournisseur.Add(lineDefinition);
                    }
                }
                #endregion


                List<ClassStruct.ConnectionAndLine> Station_Equipe_Line_List = new List<ClassStruct.ConnectionAndLine>();
                //写入‘基站-设备’的连接信息。
                Station_Equipe_Line_List = XF.createConnectionXml(xmlFilePath + "\\setting.xml", lineOfFournisseur, Station_Equi_List);

                foreach (var _station_Equipe in Station_Equi_List)
                {
                    string station = _station_Equipe.Split(new char[] { '_' })[0];
                    string equipe = _station_Equipe.Split(new char[] { '_' })[1];
                    connectionDict.Add(station + "-" + equipe);
                }
                #region 该控件已隐藏。功能暂时废弃
                /*



                    #region 写入连接信息
                    //线型
                    string station = _station_Equipe.Split(new char[] { '_' })[0];
                    string equipe = _station_Equipe.Split(new char[] { '_' })[1];
                    // ClassStruct.LineInfor lineInfor=new ClassStruct.LineInfor()

                    //ClassStruct.ConnectionAndLine Station_Equipe_Line = new ClassStruct.ConnectionAndLine(stationInfor, equipeInfor,);



                    #endregion



                    TreeNode root = new TreeNode();
                    root.Text = (station.Split(new char[] { ',' })[0] + "," + station.Split(new char[] { ',' })[1]);
                    bool isduplicate = false;
                    foreach (TreeNode node in selectedWayPoint.Nodes)
                    {
                        if (node.Text == station.Split(new char[] { ',' })[0] + "," + station.Split(new char[] { ',' })[1])
                        {
                            isduplicate = true;
                            root = node;
                            break;
                        }
                    }
                    if (!isduplicate)
                    {
                        selectedWayPoint.Nodes.Add(root);
                    }


                    TreeNode equipeRoot = new TreeNode();
                    bool equiIsduplicate = false;
                    foreach (TreeNode node in root.Nodes)
                    {
                        if (node.Text == station.Split(new char[] { ',' })[0] + "," + station.Split(new char[] { ',' })[1])
                        {
                            equiIsduplicate = true;
                            break;
                        }
                    }
                    equipeRoot.Text = equipe.Split(new char[] { ',' })[0] + "," + equipe.Split(new char[] { ',' })[1];
                    if (!equiIsduplicate)
                    {
                        root.Nodes.Add(equipeRoot);
                    }


                }

                */
                #endregion

                //绘制系统图


                //绘制电缆径路示意图。传递RailstationPoint站点信息用户绘制车站标，传递connectionDict连接信息绘制设备、线缆
                DrawPicture(RailstationPoint, Station_Equipe_Line_List);
            }
            else
            {
                MessageBox.Show("请先导入标准图块", "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 将二维数组中每个设备对应的距离最近的站点、所亭的index转换为对应的项
        /// </summary>
        /// <param name="EquipeStationPair">设备及离其最近的站点对在各自字典中的序号index</param>
        /// <param name="stationDict">筛选过的车站字典表</param>
        /// <param name="equipeDict">设备字典表</param>
        /// <returns>站点信息_设备信息</returns>
        public List<string> StationToEquipInfo(List<string> EquipeStationPair, Dictionary<string, string> stationDict, Dictionary<string, string> equipeDict)
        {
            List<string> stationEquipePair = new List<string>();
            foreach (var singalPair in EquipeStationPair)//WindToStation)
            {
                string equipeIndex = singalPair.Split(new char[] { '-' })[0]; //设备编号
                string stationIdex = singalPair.Split(new char[] { '-' })[1]; //站点编号

                string stationInfo = "";
                string equipeInfo = "";
                //for (int a = 0; a < stationPoint.Count; a++)

                //序号对应的站点信息
                int a = 0;
                foreach (var station in stationDict)
                {
                    if (a.ToString() == stationIdex)
                    {
                        stationInfo = station.Key + "," + station.Value;
                        break;
                    }
                    a++;
                }

                //序号对应的设备信息
                int b = 0;
                foreach (var equipe in equipeDict)
                {
                    if (b.ToString() == equipeIndex)
                    {
                        equipeInfo = equipe.Key + "," + equipe.Value;
                        break;
                    }
                    b++;
                }
                stationEquipePair.Add(stationInfo + "_" + equipeInfo);

            }
            return stationEquipePair;
        }


        /// <summary>
        /// 找到离横坐标（防灾设备）对应的监控点最近的基站、所亭
        /// </summary>
        /// <param name="tableDistance"></param>
        /// <returns></returns>
        public List<string> findNearestStation(int[,] tableDistance)
        {
            List<string> connectStation = new List<string>();

            string rowIndex = ""; //横向
            string colIndex = ""; //纵向
            for (int nW = 0; nW < tableDistance.GetLength(0); nW++) //横向
            {
                int distance = 0;
                for (int nH = 0; nH < tableDistance.GetLength(1); nH++)  //纵向
                {
                    if (nH == 0)
                    {
                        distance = Math.Abs(tableDistance[nW, nH]);
                        rowIndex = "" + nW;
                        colIndex = "" + nH;
                    }
                    if (distance > Math.Abs(tableDistance[nW, nH]))
                    {
                        distance = Math.Abs(tableDistance[nW, nH]);
                        rowIndex = "" + nW;
                        colIndex = "" + nH;
                    }

                }
                connectStation.Add(rowIndex + "-" + colIndex);
            }

            return connectStation;
        }

        private void 项目信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjetInfor pJ = new ProjetInfor(xmlFilePath);
            pJ.ShowDialog();
        }

        private void dataGridWind_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("发生错误" + e.Exception.ToString().Split(new char[]{' '})[1], "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void dataGridSnow_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("发生错误" + e.Exception.ToString().Split(new char[] { ' ' })[1], "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void dataGridRain_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("发生错误" + e.Exception.ToString().Split(new char[] { ' ' })[1], "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void dataGridearth_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("发生错误" + e.Exception.ToString().Split(new char[] { ' ' })[1], "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void dataGridStation_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("发生错误" + e.Exception.ToString().Split(new char[] { ' ' })[1], "注意", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text.ToString() == "地震")
            {
                //string Type = "";
                //string Name = "";
                //string Location = "";

                XmlFunction xF = new XmlFunction();
                Dictionary<string, string> loadInfor = new Dictionary<string, string>();

                loadInfor = xF.loadWayPoint(xmlFilePath + "\\setting.xml", "EarthPointLists");
                foreach (KeyValuePair<string,string> infor in loadInfor)
                {
                    string a = infor.Value;
                    if (true)//infor.Value.Contains("牵引变电所")) //如果添加有牵引变电所
                    {
                        if (InfoEarthPoint.Count != 0 && !InfoEarthPoint.ContainsKey(infor.Key)) //如果不为空且不重复
                        {
                            InfoEarthPoint.Add(infor.Key, infor.Value); //添加到字典中

                        }
                        else if (InfoEarthPoint.Count != 0 && InfoEarthPoint.ContainsKey(infor.Key)) //如果不为空且重复
                        {
                            InfoEarthPoint.Remove(infor.Key); //删除既有项，
                            InfoEarthPoint.Add(infor.Key, infor.Value); //数据添加到字典中，防止重复
                        }
                        else if(InfoEarthPoint.Count == 0)
                        {
                            InfoEarthPoint.Add(infor.Key, infor.Value); //添加到字典中
                        }


                    }
                    else //如果EarthPoints表中的项包含非牵引变电项
                    {

                    }
                }

                string licheng = "";
                Dictionary<double,string> tempDict = LocationToInt(InfoEarthPoint, out licheng); //排序
                InfoEarthPoint.Clear();
                foreach (KeyValuePair<double, string> pair in tempDict) 
                {
                    string[] p_Values = pair.Value.Split(new char[] { ',' });

                    string values = "";

                    for (int a = 0; a < p_Values.Length - 1; a++)
                    {
                        if (a == 0)
                        {
                            values = p_Values[a];
                        }
                        else
                        {

                            values = values + "," + p_Values[a];
                        }
                    }

                    //
                    InfoEarthPoint.Add(p_Values[p_Values.Length - 1], values);

                    //string name = pair.Value.Split(new char[] { ',' })[0];
                    //stTable.Rows.Add(pair.Key.ToUpper(), Loadvalue[0], Loadvalue[1], Loadvalue[2]);  //添加
                    //tempDict.Add(,InfoStation.Keys.ToString()+"-"+InfoStation.Values.ToString());
                }


                dataGridearth.DataSource = "";

                fileStationDataView(dataGridearth, tableST, colName, InfoEarthPoint, "EarthTable");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*XmlFunction xf = new XmlFunction();
            LineTypeInfo = xf.loadLineType(xmlFilePath + "\\setting.xml");

            ListFournniseur = new List<string>();
            foreach(KeyValuePair<string,string> a in LineTypeInfo)
            {
                string fournisseur = a.Value.Split(new char[] { ',' })[4];
                if (!ListFournniseur.Contains(fournisseur))
                {
                    comboFournisseur.Items.Add(fournisseur);
                }
            }*/
        }

        private void comboFournisseur_Click(object sender, EventArgs e)
        {
            XmlFunction xf = new XmlFunction();
            LineTypeInfo = xf.loadLineType(xmlFilePath + "\\setting.xml");

            ListFournniseur = new List<string>();
            comboFournisseur.Items.Clear();

            foreach (KeyValuePair<string, string> a in LineTypeInfo)
            {
                string fournisseur = a.Value.Split(new char[] { ',' })[4];
                if (!ListFournniseur.Contains(fournisseur))
                {
                    comboFournisseur.Items.Add(fournisseur);
                    ListFournniseur.Add(fournisseur);
                }
            }
        }
    }
}
 
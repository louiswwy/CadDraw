﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DotNetARX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDraw
{
    class drawFunction
    {

        public class StationAndLocation
        {
            private string id = string.Empty;
            private string name = string.Empty;
            private string location = string.Empty;

            //可以根据自己的需求继续添加,如：private Int32 m_Index；

            public StationAndLocation()
            { }
            public StationAndLocation(string sid, string sname, string slocation)
            {
                id = sid;
                name = sname;
                location = slocation;
            }
            public override string ToString()
            {
                return this.name;
            }

            public string ID
            {
                get
                {
                    return this.id;
                }
                set
                {
                    this.id = value;
                }
            }
            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    this.name = value;
                }
            }
            public string Location
            {
                get
                {
                    return this.location;
                }
                set
                {
                    this.location = value;
                }
            }
        }


        #region 生成图块
        public bool CheckBlock()
        {
            bool allCheck = false;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    DocumentLock m_DocumentLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    //bool hasLeftSideMark = false;
                    //bool hasRightSideMark = false;
                    //bool hasRailWayMark = false;

                    //查找是已存在站点标识
                    bt.UpgradeOpen();
                    Point3d insertPoint = new Point3d();
                    //如果不存在右侧标识则新建块
                    if (!bt.Has("到达站站点标示"))
                    {
                        //StationAndLocation station_location = new StationAndLocation("1", "吉林站", "DK0+000");
                        CreateStationMark(db, trans, insertPoint, false);
                    }

                    //如果不存在左侧标识则新建块
                    //Point3d insertPoint2 = new Point3d(insertPoint.X + 100, insertPoint.Y, insertPoint.Z);
                    if (!bt.Has("始发站站点标示"))
                    {
                        //StationAndLocation station_location = new StationAndLocation("1", "蛟河西站", "DK66+535");
                        CreateStationMark(db, trans, insertPoint, true);
                    }

                    //如果不存在铁轨标识则新建块
                    if (!bt.Has("铁轨_Length_248"))
                    {
                        CreateRailWayMark(db, trans, insertPoint);//, "XX上行线/下行线");
                    }
                    bt.DowngradeOpen();
                    trans.Commit();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ee)
                {
                    Application.ShowAlertDialog("Message: " + ee.Message.ToString() + System.Environment.NewLine + "Source: " + ee.Source.ToString() + System.Environment.NewLine + "TargetSite: " + ee.TargetSite.ToString() + System.Environment.NewLine + "StackTrace: " + ee.StackTrace.ToString());
                }
            }
            allCheck = true;
            return allCheck;
        }

        //如果不存在则添加铁轨标识
        private void CreateRailWayMark(Database db, Transaction trans, Point3d insertPoint)//, string RailWayDirection)
        {
            // Open the Block table for read
            BlockTable acBlkTbl = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

            // Open the Block table record Model space for write
            BlockTableRecord acBlkTblRec = new BlockTableRecord();
            acBlkTbl.UpgradeOpen();
            acBlkTbl.Add(acBlkTblRec);
            acBlkTblRec.Name = "铁轨_Length_208";

            //块属性
            double textHeight = 3;
            //AttributeDefinition ProjectNameShortAtt = new AttributeDefinition(new Point3d(insertPoint.X, insertPoint.Y - 7 / 2, 0), RailWayDirection, "上/下行", "输入线路名简称", ObjectId.Null);

            AttributeDefinition ProjectNameShortAtt = new AttributeDefinition();
            ProjectNameShortAtt.TextString = "XXX上行/下行线";
            ProjectNameShortAtt.Tag = "上行/下行线";
            ProjectNameShortAtt.Prompt = "输入线路名称";
            ProjectNameShortAtt.TextStyleId = ObjectId.Null;
            ProjectNameShortAtt.Justify = AttachmentPoint.MiddleLeft;
            ProjectNameShortAtt.HorizontalMode = TextHorizontalMode.TextLeft;
            ProjectNameShortAtt.VerticalMode = TextVerticalMode.TextVerticalMid;

            ProjectNameShortAtt.AlignmentPoint = new Point3d(insertPoint.X, insertPoint.Y - textHeight / 2, 0);
            SetStyleForAttribut(ProjectNameShortAtt, textHeight, false);

            acBlkTblRec.AppendEntity(ProjectNameShortAtt);
            trans.AddNewlyCreatedDBObject(ProjectNameShortAtt, true);
            //ProjectNameShortAtt.AlignmentPoint = new Point3d(1, 1, 0);

            Polyline BigRectangle = new Polyline(4);
            BigRectangle.AddVertexAt(0, new Point2d(insertPoint.X + 15, insertPoint.Y), 0, 0.1, 0.1);
            BigRectangle.AddVertexAt(1, new Point2d(insertPoint.X + 15 + 228, insertPoint.Y), 0, 0.1, 0.1);
            BigRectangle.AddVertexAt(2, new Point2d(insertPoint.X + 15 + 228, insertPoint.Y - 7), 0, 0.1, 0.1);
            BigRectangle.AddVertexAt(3, new Point2d(insertPoint.X + 15, insertPoint.Y - 7), 0, 0.1, 0.1);
            BigRectangle.Closed = true;
            acBlkTblRec.AppendEntity(BigRectangle);
            trans.AddNewlyCreatedDBObject(BigRectangle, true);

            Polyline InsideRectangle_1 = new Polyline(4);
            InsideRectangle_1.AddVertexAt(0, new Point2d(insertPoint.X + 65, insertPoint.Y), 0, 0.1, 0.1);
            InsideRectangle_1.AddVertexAt(1, new Point2d(insertPoint.X + 110, insertPoint.Y), 0, 0.1, 0.1);
            InsideRectangle_1.AddVertexAt(2, new Point2d(insertPoint.X + 110, insertPoint.Y - 7), 0, 0.1, 0.1);
            InsideRectangle_1.AddVertexAt(3, new Point2d(insertPoint.X + 65, insertPoint.Y - 7), 0, 0.1, 0.1);
            InsideRectangle_1.Closed = true;
            acBlkTblRec.AppendEntity(InsideRectangle_1);
            trans.AddNewlyCreatedDBObject(InsideRectangle_1, true);

            Polyline InsideRectangle_2 = new Polyline(4);
            InsideRectangle_2.AddVertexAt(0, new Point2d(insertPoint.X + 155, insertPoint.Y), 0, 0.1, 0.1);
            InsideRectangle_2.AddVertexAt(1, new Point2d(insertPoint.X + 200, insertPoint.Y), 0, 0.1, 0.1);
            InsideRectangle_2.AddVertexAt(2, new Point2d(insertPoint.X + 200, insertPoint.Y - 7), 0, 0.1, 0.1);
            InsideRectangle_2.AddVertexAt(3, new Point2d(insertPoint.X + 155, insertPoint.Y - 7), 0, 0.1, 0.1);
            InsideRectangle_2.Closed = true;
            acBlkTblRec.AppendEntity(InsideRectangle_2);
            trans.AddNewlyCreatedDBObject(InsideRectangle_2, true);

            Hatch hatch = new Hatch();
            acBlkTblRec.AppendEntity(hatch);
            trans.AddNewlyCreatedDBObject(hatch, true);

            hatch.SetDatabaseDefaults();
            hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
            hatch.Associative = true;
            ObjectIdCollection ids = new ObjectIdCollection();
            ids.Add(InsideRectangle_1.ObjectId);

            ObjectIdCollection ids1 = new ObjectIdCollection();
            ids1.Add(InsideRectangle_2.ObjectId);

            hatch.AppendLoop(HatchLoopTypes.Default, ids);

            hatch.AppendLoop(HatchLoopTypes.Default, ids1);

            hatch.EvaluateHatch(true);
            db.TransactionManager.AddNewlyCreatedDBObject(acBlkTblRec, true);
            //acBlkTblRec.ObjectId.AddAttsToBlock(ProjectNameShortAtt);

            //Dictionary<string,string> att=new Dictionary<string,string>();
            //att.Add("xx上行线/下行线","吉珲上行线");
            //double textHeight=50;
            //db.CurrentSpaceId.InsertBlockReference("0", "铁轨_Length_248", new Point3d(insertPoint.X, insertPoint.Y - textHeight / 2, 0), new Scale3d(2), 0, att);
            acBlkTbl.DowngradeOpen();
        }

        private void SetStyleForAttribut(AttributeDefinition att, bool invisible)
        {
            att.Height = 0.15;//高度
            att.HorizontalMode = TextHorizontalMode.TextCenter;
            att.VerticalMode = TextVerticalMode.TextVerticalMid;
            att.Invisible = invisible;
        }

        private void SetStyleForAttribut(AttributeDefinition att, double textHeight, bool invisible)
        {
            att.Height = textHeight;//高度
            att.HorizontalMode = TextHorizontalMode.TextCenter;
            att.VerticalMode = TextVerticalMode.TextVerticalMid;
            att.Invisible = invisible;
        }

        //新建站点标识
        private void CreateStationMark(Database db, Transaction trans, Point3d insertPoint, bool isLeft)
        {
            insertPoint = new Point3d(insertPoint.X, insertPoint.Y + 36, insertPoint.Z);
            // Open the Block table for read
            BlockTable acBlkTbl;

            acBlkTbl = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

            // Open the Block table record Model space for write

            BlockTableRecord acBlkTblRec = new BlockTableRecord();
            acBlkTbl.UpgradeOpen();
            acBlkTbl.Add(acBlkTblRec);
            if (isLeft == false)
            {
                acBlkTblRec.Name = "到达站站点标示";
            }
            else
            {
                acBlkTblRec.Name = "始发站站点标示";
            }
            //acBlkTblRec = trans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;


            Point2d CircleCenter = new Point2d(insertPoint.X, insertPoint.Y);

            //画圆（左右两个半圆）
            //起始、终止角度
            double startAngle = 90;
            double endAngle = 270;
            //内圆 由两个arc组成
            //左侧
            Arc InnerArcLeft = new Arc(new Point3d(CircleCenter.X, CircleCenter.Y, 0), 2, startAngle.DegreeToRadian(), endAngle.DegreeToRadian());
            acBlkTblRec.AppendEntity(InnerArcLeft);
            trans.AddNewlyCreatedDBObject(InnerArcLeft, true);

            //右侧
            Arc InnerArcRight = new Arc(new Point3d(CircleCenter.X, CircleCenter.Y, 0), 2, endAngle.DegreeToRadian(), startAngle.DegreeToRadian());
            acBlkTblRec.AppendEntity(InnerArcRight);
            trans.AddNewlyCreatedDBObject(InnerArcRight, true);

            //外圆 由两个arc组成
            //左侧
            Arc OuterArcLeft = new Arc(new Point3d(CircleCenter.X, CircleCenter.Y, 0), 4, startAngle.DegreeToRadian(), endAngle.DegreeToRadian());
            acBlkTblRec.AppendEntity(OuterArcLeft);
            trans.AddNewlyCreatedDBObject(OuterArcLeft, true);

            //右侧
            Arc OuterArcRight = new Arc(new Point3d(CircleCenter.X, CircleCenter.Y, 0), 4, endAngle.DegreeToRadian(), startAngle.DegreeToRadian());
            acBlkTblRec.AppendEntity(OuterArcRight);
            trans.AddNewlyCreatedDBObject(OuterArcRight, true);

            //直线
            Polyline pline = new Polyline();
            pline.CreatePolyline(new Point2d(CircleCenter.X, CircleCenter.Y + 4), new Point2d(CircleCenter.X, CircleCenter.Y - 36));
            acBlkTblRec.AppendEntity(pline);
            trans.AddNewlyCreatedDBObject(pline, true);

            //内辅助直线
            Polyline innerPline = new Polyline();
            innerPline.CreatePolyline(new Point2d(CircleCenter.X, CircleCenter.Y + 2), new Point2d(CircleCenter.X, CircleCenter.Y - 2));
            acBlkTblRec.AppendEntity(innerPline);
            trans.AddNewlyCreatedDBObject(innerPline, true);

            //外辅助直线
            Polyline outerPline = new Polyline();
            outerPline.CreatePolyline(new Point2d(CircleCenter.X, CircleCenter.Y + 4), new Point2d(CircleCenter.X, CircleCenter.Y - 4));
            acBlkTblRec.AppendEntity(outerPline);
            trans.AddNewlyCreatedDBObject(outerPline, true);

            //添加两个属性块： 站名，里程
            double textHeight = 3;
            AttributeDefinition AttStationName = new AttributeDefinition();

            //设置两个属性块对齐方式，对齐点
            AttStationName.TextString = "XXX站";
            AttStationName.Tag = "站名";
            AttStationName.Prompt = "输入站点名称";
            SetStyleForAttribut(AttStationName, textHeight, false);
            AttStationName.TextStyleId = ObjectId.Null;
            AttStationName.Justify = AttachmentPoint.BaseCenter;

            //AttStationName.HorizontalMode = TextHorizontalMode.TextCenter; //水平方向取中点
            //AttStationName.VerticalMode = TextVerticalMode.TextVerticalMid;
            AttStationName.AlignmentPoint = new Point3d(insertPoint.X, insertPoint.Y + 6 + 36, 0);
            acBlkTblRec.AppendEntity(AttStationName);
            trans.AddNewlyCreatedDBObject(AttStationName, true);

            AttributeDefinition AttStationLocation = new AttributeDefinition();
            AttStationLocation.TextString = "XXX站";
            AttStationLocation.Tag = "里程";
            AttStationLocation.Prompt = "输入站点里程";
            SetStyleForAttribut(AttStationLocation, textHeight, false);
            AttStationLocation.TextStyleId = ObjectId.Null;
            AttStationLocation.Justify = AttachmentPoint.TopLeft;
            /*
             * AttStationLocation.HorizontalMode = TextHorizontalMode.TextCenter; //水平方向取终点
             * AttStationLocation.VerticalMode = TextVerticalMode.TextBottom;
             * AttStationLocation.AlignmentPoint = new Point3d(insertPoint.X + 1, insertPoint.Y - 18, 0);          
             */

            //double rotatedAngle = 90;
            //AttStationLocation.Rotation = rotatedAngle.DegreeToRadian();
            AttStationLocation.AlignmentPoint = new Point3d(insertPoint.X + 1, insertPoint.Y - 18 + 36, 0);

            acBlkTblRec.AppendEntity(AttStationLocation);
            trans.AddNewlyCreatedDBObject(AttStationLocation, true);

            //填充
            if (isLeft == false)
            {
                ObjectIdCollection ids = new ObjectIdCollection();
                ids.Add(OuterArcLeft.ObjectId);
                ids.Add(outerPline.ObjectId);

                ObjectIdCollection ids2 = new ObjectIdCollection();
                ids2.Add(InnerArcLeft.ObjectId);
                ids2.Add(innerPline.ObjectId);

                Hatch hatch = new Hatch();
                acBlkTblRec.AppendEntity(hatch);

                trans.AddNewlyCreatedDBObject(hatch, true);

                hatch.SetDatabaseDefaults();

                hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");

                hatch.Associative = true;

                hatch.AppendLoop(HatchLoopTypes.Outermost, ids);

                hatch.AppendLoop(HatchLoopTypes.Default, ids2);

                hatch.EvaluateHatch(true);

                ids.Clear();
                ids2.Clear();

            }
            else
            {
                ObjectIdCollection ids = new ObjectIdCollection();
                ids.Add(OuterArcRight.ObjectId);
                ids.Add(outerPline.ObjectId);

                ObjectIdCollection ids2 = new ObjectIdCollection();
                ids2.Add(InnerArcRight.ObjectId);
                ids2.Add(innerPline.ObjectId);

                Hatch hatch = new Hatch();

                acBlkTblRec.AppendEntity(hatch);

                trans.AddNewlyCreatedDBObject(hatch, true);

                hatch.SetDatabaseDefaults();

                hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                //hatch.CreateHatch(HatchPatternType.PreDefined, "SOLID", true);
                hatch.Associative = true;

                hatch.AppendLoop(HatchLoopTypes.Outermost, ids);

                hatch.AppendLoop(HatchLoopTypes.Default, ids2);

                hatch.EvaluateHatch(true);

                ids.Clear();
                ids2.Clear();

            }
            //acBlkTbl.Add(acBlkTblRec);
            db.TransactionManager.AddNewlyCreatedDBObject(acBlkTblRec, true);
            acBlkTbl.DowngradeOpen();

        }

        #endregion

        /// <summary>
        /// 绘制通信、信号电缆槽示意图
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        /// <param name="insertPoint"></param>
        public void drawDoubleFibre(Database db, Transaction trans, Point3d insertPoint)
        {
            Polyline pHorizonline1 = new Polyline();
            pHorizonline1.CreatePolyline(new Point2d(insertPoint.X, insertPoint.Y), new Point2d(insertPoint.X + 248, insertPoint.Y));
            Polyline pHorizonline2 = new Polyline();
            pHorizonline2.CreatePolyline(new Point2d(insertPoint.X, insertPoint.Y - 4), new Point2d(insertPoint.X + 248, insertPoint.Y - 4));
            Polyline pHorizonline3 = new Polyline();
            pHorizonline3.CreatePolyline(new Point2d(insertPoint.X, insertPoint.Y - 8), new Point2d(insertPoint.X + 248, insertPoint.Y - 8));

            DBText TFibreName1 = new DBText();
            TFibreName1.TextString = "信号电缆槽";
            TFibreName1.Height = 3;
            TFibreName1.VerticalMode = TextVerticalMode.TextVerticalMid;
            TFibreName1.HorizontalMode = TextHorizontalMode.TextLeft;
            TFibreName1.AlignmentPoint = new Point3d(insertPoint.X, insertPoint.Y - 2, 0);

            DBText TFibreName2 = new DBText();
            TFibreName2.TextString = "通信电缆槽";
            TFibreName2.Height = 3;
            TFibreName2.VerticalMode = TextVerticalMode.TextVerticalMid;
            TFibreName2.HorizontalMode = TextHorizontalMode.TextLeft;
            TFibreName2.AlignmentPoint = new Point3d(insertPoint.X, insertPoint.Y - 6, 0);

            db.AddToModelSpace(TFibreName1, TFibreName2);
            db.AddToModelSpace(pHorizonline1, pHorizonline2, pHorizonline3);
        }

        public void drawTable(Database db, Transaction trans, Point3d insertPoint)
        {
            BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
            bt.UpgradeOpen();
            BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

            Polyline pHorizonline1 = new Polyline();
            pHorizonline1.CreatePolyline(new Point2d(insertPoint.X, insertPoint.Y), new Point2d(insertPoint.X + 30, insertPoint.Y));
            btr.AppendEntity(pHorizonline1);
            trans.AddNewlyCreatedDBObject(pHorizonline1, true);

            Polyline pHorizonline2 = new Polyline();
            pHorizonline2.CreatePolyline(new Point2d(insertPoint.X, insertPoint.Y - 8.5), new Point2d(insertPoint.X + 30, insertPoint.Y - 8.5));
            btr.AppendEntity(pHorizonline2);
            trans.AddNewlyCreatedDBObject(pHorizonline2, true);

            Polyline pHorizonline3 = new Polyline();
            pHorizonline3.CreatePolyline(new Point2d(insertPoint.X, insertPoint.Y - 19), new Point2d(insertPoint.X + 30, insertPoint.Y - 19));
            btr.AppendEntity(pHorizonline3);
            trans.AddNewlyCreatedDBObject(pHorizonline3, true);

            Polyline pHorizonline4 = new Polyline();
            pHorizonline4.CreatePolyline(new Point2d(insertPoint.X, insertPoint.Y - 32.5), new Point2d(insertPoint.X + 247, insertPoint.Y - 32.5));
            btr.AppendEntity(pHorizonline4);
            trans.AddNewlyCreatedDBObject(pHorizonline4, true);

            Polyline pHorizonline5 = new Polyline();
            pHorizonline5.CreatePolyline(new Point2d(insertPoint.X, insertPoint.Y - 41), new Point2d(insertPoint.X + 247, insertPoint.Y - 41));
            btr.AppendEntity(pHorizonline5);
            trans.AddNewlyCreatedDBObject(pHorizonline5, true);

            Polyline pVerticalLine1 = new Polyline();
            pVerticalLine1.CreatePolyline(new Point2d(insertPoint.X, insertPoint.Y), new Point2d(insertPoint.X, insertPoint.Y - 41));
            btr.AppendEntity(pVerticalLine1);
            trans.AddNewlyCreatedDBObject(pVerticalLine1, true);

            Polyline pVerticalLine2 = new Polyline();
            pVerticalLine2.CreatePolyline(new Point2d(insertPoint.X + 30, insertPoint.Y), new Point2d(insertPoint.X + 30, insertPoint.Y - 41));
            btr.AppendEntity(pVerticalLine2);
            trans.AddNewlyCreatedDBObject(pVerticalLine2, true);

            DBText TStationName = new DBText();
            TStationName.TextString = "站名";
            TStationName.Height = 3;
            TStationName.VerticalMode = TextVerticalMode.TextVerticalMid;
            TStationName.HorizontalMode = TextHorizontalMode.TextCenter;
            TStationName.AlignmentPoint = new Point3d(insertPoint.X + 30 / 2, insertPoint.Y - 8.5 / 2, 0);

            DBText TStationmark = new DBText();
            TStationmark.TextString = "图示";
            TStationmark.Height = 3;
            TStationmark.VerticalMode = TextVerticalMode.TextVerticalMid;
            TStationmark.HorizontalMode = TextHorizontalMode.TextCenter;
            TStationmark.AlignmentPoint = new Point3d(insertPoint.X + 30 / 2, insertPoint.Y - 8.5 - 8.5 / 2, 0);

            DBText TStationLocation = new DBText();
            TStationLocation.TextString = "站中心里程";
            TStationLocation.Height = 3;
            TStationLocation.VerticalMode = TextVerticalMode.TextVerticalMid;
            TStationLocation.HorizontalMode = TextHorizontalMode.TextCenter;
            TStationLocation.AlignmentPoint = new Point3d(insertPoint.X + 30 / 2, insertPoint.Y - (19 + (32.5 - 19) / 2), 0);

            DBText TStationDistance = new DBText();
            TStationDistance.TextString = "站间距离";
            TStationDistance.Height = 3;
            TStationDistance.VerticalMode = TextVerticalMode.TextVerticalMid;
            TStationDistance.HorizontalMode = TextHorizontalMode.TextCenter;
            TStationDistance.AlignmentPoint = new Point3d(insertPoint.X + 30 / 2, insertPoint.Y - (32.5 + (41 - 32.5) / 2), 0);

            //db.AddToModelSpace(pHorizonline1, pHorizonline2, pHorizonline3, pHorizonline4, pHorizonline5, pVerticalLine1, pVerticalLine2);
            db.AddToModelSpace(TStationName, TStationmark, TStationLocation, TStationDistance);
            bt.DowngradeOpen();
            btr.DowngradeOpen();
        }

    }
}
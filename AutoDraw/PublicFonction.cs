using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using DotNetARX;

namespace AutoDraw
{
    static class PublicFonction
    {
        #region 绘制长方形多段线
        /// <summary>
        /// 绘制默认宽度为0的长方形多段线
        /// </summary>
        /// <param name="pline"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        public static void CreateNewRectangle(this Polyline pline, Point2d pt1, Point2d pt2)
        {
            //设置矩形的4个顶点
            double minX = Math.Min(pt1.X, pt2.X);
            double maxX = Math.Max(pt1.X, pt2.X);
            double minY = Math.Min(pt1.Y, pt2.Y);
            double maxY = Math.Max(pt1.Y, pt2.Y);

            Point2dCollection pts = new Point2dCollection();
            pts.Add(new Point2d(minX, minY));
            pts.Add(new Point2d(minX, maxY));
            pts.Add(new Point2d(maxX, maxY));
            pts.Add(new Point2d(maxX, minY));

            pline.CreatePolyline(pts);

            for (int i = 0; i < pts.Count; i++)
            {
                //添加多段线的顶点
                //后三个数值分别为要创建的顶点的凸度, startWidth, endWidth
                pline.AddVertexAt(i, pts[i], 0, 0, 0);
            }
            //闭合多短线
            pline.Closed = true;
        }
        /// <summary>
        /// 绘制统一宽度长方形多段线
        /// </summary>
        /// <param name="pline"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="bulge">顶点的凸度,0为直线、1为半圆、0和1之间为劣弧，大于1为优弧</param>
        /// <param name="startWidth">多段线起点宽度</param>
        /// <param name="endWidth">多段线终点宽度</param>
        public static void CreateNewRectangle(this Polyline pline, Point2d pt1, Point2d pt2, double bulge, double startWidth, double endWidth)
        {
            //设置矩形的4个顶点
            double minX = Math.Min(pt1.X, pt2.X);
            double maxX = Math.Max(pt1.X, pt2.X);
            double minY = Math.Min(pt1.Y, pt2.Y);
            double maxY = Math.Max(pt1.Y, pt2.Y);

            Point2dCollection pts = new Point2dCollection();
            pts.Add(new Point2d(minX, minY));
            pts.Add(new Point2d(minX, maxY));
            pts.Add(new Point2d(maxX, maxY));
            pts.Add(new Point2d(maxX, minY));

            pline.CreatePolyline(pts);

            for (int i = 0; i < pts.Count; i++)
            {
                //添加多段线的顶点
                //后三个数值分别为要创建的顶点的凸度, startWidth, endWidth
                pline.AddVertexAt(i, pts[i], bulge, startWidth, endWidth);
            }
            //闭合多短线
            pline.Closed = true;
        }
        #endregion


    }
}

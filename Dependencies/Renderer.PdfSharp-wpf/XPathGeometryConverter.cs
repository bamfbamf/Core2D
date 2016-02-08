﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using PdfSharp.Drawing;
using System;
using System.Linq;
#if WPF
using System.Windows.Media;
#endif

namespace Dependencies
{
    /// <summary>
    /// 
    /// </summary>
    public static class XPathGeometryConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static XGraphicsPath ToXGraphicsPath(this Core2D.Path.XPathGeometry pg, double dx, double dy, Func<double, double> scale)
        {
            var gp = new XGraphicsPath();
            gp.FillMode = pg.FillRule == Core2D.Path.XFillRule.EvenOdd ? XFillMode.Alternate : XFillMode.Winding;

            foreach (var pf in pg.Figures)
            {
                var startPoint = pf.StartPoint;

                foreach (var segment in pf.Segments)
                {
                    if (segment is Core2D.Path.Segments.XArcSegment)
                    {
#if CORE
                        //var arcSegment = segment as Core2D.XArcSegment;
                        // TODO: Convert WPF/SVG elliptical arc segment format to GDI+ bezier curves.
                        //startPoint = arcSegment.Point;
#endif
#if WPF
                        var arcSegment = segment as Core2D.Path.Segments.XArcSegment;
                        var point1 = new XPoint(
                            scale(startPoint.X),
                            scale(startPoint.Y));
                        var point2 = new XPoint(
                            scale(arcSegment.Point.X),
                            scale(arcSegment.Point.Y));
                        var size = new XSize(
                            scale(arcSegment.Size.Width),
                            scale(arcSegment.Size.Height));
                        gp.AddArc(
                            point1,
                            point2,
                            size, arcSegment.RotationAngle, arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == Core2D.Path.XSweepDirection.Clockwise ? XSweepDirection.Clockwise : XSweepDirection.Counterclockwise);
                        startPoint = arcSegment.Point;
#endif
                    }
                    else if (segment is Core2D.Path.Segments.XCubicBezierSegment)
                    {
                        var cubicBezierSegment = segment as Core2D.Path.Segments.XCubicBezierSegment;
                        gp.AddBezier(
                            scale(startPoint.X),
                            scale(startPoint.Y),
                            scale(cubicBezierSegment.Point1.X),
                            scale(cubicBezierSegment.Point1.Y),
                            scale(cubicBezierSegment.Point2.X),
                            scale(cubicBezierSegment.Point2.Y),
                            scale(cubicBezierSegment.Point3.X),
                            scale(cubicBezierSegment.Point3.Y));
                        startPoint = cubicBezierSegment.Point3;
                    }
                    else if (segment is Core2D.Path.Segments.XLineSegment)
                    {
                        var lineSegment = segment as Core2D.Path.Segments.XLineSegment;
                        gp.AddLine(
                            scale(startPoint.X),
                            scale(startPoint.Y),
                            scale(lineSegment.Point.X),
                            scale(lineSegment.Point.Y));
                        startPoint = lineSegment.Point;
                    }
                    else if (segment is Core2D.Path.Segments.XPolyCubicBezierSegment)
                    {
                        var polyCubicBezierSegment = segment as Core2D.Path.Segments.XPolyCubicBezierSegment;
                        if (polyCubicBezierSegment.Points.Count >= 3)
                        {
                            gp.AddBezier(
                                scale(startPoint.X),
                                scale(startPoint.Y),
                                scale(polyCubicBezierSegment.Points[0].X),
                                scale(polyCubicBezierSegment.Points[0].Y),
                                scale(polyCubicBezierSegment.Points[1].X),
                                scale(polyCubicBezierSegment.Points[1].Y),
                                scale(polyCubicBezierSegment.Points[2].X),
                                scale(polyCubicBezierSegment.Points[2].Y));
                        }

                        if (polyCubicBezierSegment.Points.Count > 3
                            && polyCubicBezierSegment.Points.Count % 3 == 0)
                        {
                            for (int i = 3; i < polyCubicBezierSegment.Points.Count; i += 3)
                            {
                                gp.AddBezier(
                                    scale(polyCubicBezierSegment.Points[i - 1].X),
                                    scale(polyCubicBezierSegment.Points[i - 1].Y),
                                    scale(polyCubicBezierSegment.Points[i].X),
                                    scale(polyCubicBezierSegment.Points[i].Y),
                                    scale(polyCubicBezierSegment.Points[i + 1].X),
                                    scale(polyCubicBezierSegment.Points[i + 1].Y),
                                    scale(polyCubicBezierSegment.Points[i + 2].X),
                                    scale(polyCubicBezierSegment.Points[i + 2].Y));
                            }
                        }

                        startPoint = polyCubicBezierSegment.Points.Last();
                    }
                    else if (segment is Core2D.Path.Segments.XPolyLineSegment)
                    {
                        var polyLineSegment = segment as Core2D.Path.Segments.XPolyLineSegment;
                        if (polyLineSegment.Points.Count >= 1)
                        {
                            gp.AddLine(
                                scale(startPoint.X),
                                scale(startPoint.Y),
                                scale(polyLineSegment.Points[0].X),
                                scale(polyLineSegment.Points[0].Y));
                        }

                        if (polyLineSegment.Points.Count > 1)
                        {
                            for (int i = 1; i < polyLineSegment.Points.Count; i++)
                            {
                                gp.AddLine(
                                    scale(polyLineSegment.Points[i - 1].X),
                                    scale(polyLineSegment.Points[i - 1].Y),
                                    scale(polyLineSegment.Points[i].X),
                                    scale(polyLineSegment.Points[i].Y));
                            }
                        }

                        startPoint = polyLineSegment.Points.Last();
                    }
                    else if (segment is Core2D.Path.Segments.XPolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as Core2D.Path.Segments.XPolyQuadraticBezierSegment;
                        if (polyQuadraticSegment.Points.Count >= 2)
                        {
                            var p1 = startPoint;
                            var p2 = polyQuadraticSegment.Points[0];
                            var p3 = polyQuadraticSegment.Points[1];
                            double x1 = p1.X;
                            double y1 = p1.Y;
                            double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                            double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                            double x3 = x2 + (p3.X - p1.X) / 3.0;
                            double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                            double x4 = p3.X;
                            double y4 = p3.Y;
                            gp.AddBezier(
                                scale(x1 + dx),
                                scale(y1 + dy),
                                scale(x2 + dx),
                                scale(y2 + dy),
                                scale(x3 + dx),
                                scale(y3 + dy),
                                scale(x4 + dx),
                                scale(y4 + dy));
                        }

                        if (polyQuadraticSegment.Points.Count > 2
                            && polyQuadraticSegment.Points.Count % 2 == 0)
                        {
                            for (int i = 3; i < polyQuadraticSegment.Points.Count; i += 3)
                            {
                                var p1 = polyQuadraticSegment.Points[i - 1];
                                var p2 = polyQuadraticSegment.Points[i];
                                var p3 = polyQuadraticSegment.Points[i + 1];
                                double x1 = p1.X;
                                double y1 = p1.Y;
                                double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                                double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                                double x3 = x2 + (p3.X - p1.X) / 3.0;
                                double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                                double x4 = p3.X;
                                double y4 = p3.Y;
                                gp.AddBezier(
                                    scale(x1 + dx),
                                    scale(y1 + dy),
                                    scale(x2 + dx),
                                    scale(y2 + dy),
                                    scale(x3 + dx),
                                    scale(y3 + dy),
                                    scale(x4 + dx),
                                    scale(y4 + dy));
                            }
                        }

                        startPoint = polyQuadraticSegment.Points.Last();
                    }
                    else if (segment is Core2D.Path.Segments.XQuadraticBezierSegment)
                    {
                        var quadraticBezierSegment = segment as Core2D.Path.Segments.XQuadraticBezierSegment;
                        var p1 = startPoint;
                        var p2 = quadraticBezierSegment.Point1;
                        var p3 = quadraticBezierSegment.Point2;
                        double x1 = p1.X;
                        double y1 = p1.Y;
                        double x2 = p1.X + (2.0 * (p2.X - p1.X)) / 3.0;
                        double y2 = p1.Y + (2.0 * (p2.Y - p1.Y)) / 3.0;
                        double x3 = x2 + (p3.X - p1.X) / 3.0;
                        double y3 = y2 + (p3.Y - p1.Y) / 3.0;
                        double x4 = p3.X;
                        double y4 = p3.Y;
                        gp.AddBezier(
                            scale(x1 + dx),
                            scale(y1 + dy),
                            scale(x2 + dx),
                            scale(y2 + dy),
                            scale(x3 + dx),
                            scale(y3 + dy),
                            scale(x4 + dx),
                            scale(y4 + dy));
                        startPoint = quadraticBezierSegment.Point2;
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }

                if (pf.IsClosed)
                {
                    gp.CloseFigure();
                }
                else
                {
                    gp.StartFigure();
                }
            }

            return gp;
        }
    }
}

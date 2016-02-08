﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Path;
using Core2D.Shapes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Dependencies
{
    /// <summary>
    /// 
    /// </summary>
    public static class PathGeometryConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IList<XPoint> ToXPoints(this IList<Point> points)
        {
            var xpoints = new List<XPoint>();
            foreach (var point in points)
            {
                xpoints.Add(XPoint.Create(point.X, point.Y));
            }
            return xpoints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pg"></param>
        /// <returns></returns>
        public static XPathGeometry ToXPathGeometry(this PathGeometry pg)
        {
            var geometry = XPathGeometry.Create(
                new List<XPathFigure>(),
                pg.FillRule == FillRule.EvenOdd ? XFillRule.EvenOdd : XFillRule.Nonzero);

            var context = new XPathGeometryContext(geometry);

            foreach (var pf in pg.Figures)
            {
                context.BeginFigure(
                    XPoint.Create(pf.StartPoint.X, pf.StartPoint.Y),
                    pf.IsFilled,
                    pf.IsClosed);

                foreach (var segment in pf.Segments)
                {
                    if (segment is ArcSegment)
                    {
                        var arcSegment = segment as ArcSegment;
                        context.ArcTo(
                            XPoint.Create(arcSegment.Point.X, arcSegment.Point.Y),
                            XPathSize.Create(arcSegment.Size.Width, arcSegment.Size.Height),
                            arcSegment.RotationAngle,
                            arcSegment.IsLargeArc,
                            arcSegment.SweepDirection == SweepDirection.Clockwise ? XSweepDirection.Clockwise : XSweepDirection.Counterclockwise,
                            arcSegment.IsStroked,
                            arcSegment.IsSmoothJoin);
                    }
                    else if (segment is BezierSegment)
                    {
                        var cubicBezierSegment = segment as BezierSegment;
                        context.CubicBezierTo(
                            XPoint.Create(cubicBezierSegment.Point1.X, cubicBezierSegment.Point1.Y),
                            XPoint.Create(cubicBezierSegment.Point2.X, cubicBezierSegment.Point2.Y),
                            XPoint.Create(cubicBezierSegment.Point3.X, cubicBezierSegment.Point3.Y),
                            cubicBezierSegment.IsStroked,
                            cubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is LineSegment)
                    {
                        var lineSegment = segment as LineSegment;
                        context.LineTo(
                            XPoint.Create(lineSegment.Point.X, lineSegment.Point.Y),
                            lineSegment.IsStroked,
                            lineSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyBezierSegment)
                    {
                        var polyCubicBezierSegment = segment as PolyBezierSegment;
                        context.PolyCubicBezierTo(
                            ToXPoints(polyCubicBezierSegment.Points),
                            polyCubicBezierSegment.IsStroked,
                            polyCubicBezierSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyLineSegment)
                    {
                        var polyLineSegment = segment as PolyLineSegment;
                        context.PolyLineTo(
                            ToXPoints(polyLineSegment.Points),
                            polyLineSegment.IsStroked,
                            polyLineSegment.IsSmoothJoin);
                    }
                    else if (segment is PolyQuadraticBezierSegment)
                    {
                        var polyQuadraticSegment = segment as PolyQuadraticBezierSegment;
                        context.PolyQuadraticBezierTo(
                            ToXPoints(polyQuadraticSegment.Points),
                            polyQuadraticSegment.IsStroked,
                            polyQuadraticSegment.IsSmoothJoin);
                    }
                    else if (segment is QuadraticBezierSegment)
                    {
                        var quadraticBezierSegment = segment as QuadraticBezierSegment;
                        context.QuadraticBezierTo(
                            XPoint.Create(quadraticBezierSegment.Point1.X, quadraticBezierSegment.Point1.Y),
                            XPoint.Create(quadraticBezierSegment.Point2.X, quadraticBezierSegment.Point2.Y),
                            quadraticBezierSegment.IsStroked,
                            quadraticBezierSegment.IsSmoothJoin);
                    }
                    else
                    {
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                    }
                }
            }

            return geometry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static XPathGeometry ToXPathGeometry(this string source)
        {
            var g = Geometry.Parse(source);
            var pg = PathGeometry.CreateFromGeometry(g);
            return ToXPathGeometry(pg);
        }
    }
}

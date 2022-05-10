using System;
using System.Collections.Generic;
using System.Linq;
namespace triangle
{
    public class Edge
    {
        public List<Point> BrDots { get => BresenhamLine(Point1, Point2); }
        public Point Point1 { get; }
        public Point Point2 { get; }

        public Edge(Point point1, Point point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
        List<Point> BresenhamLine(Point point1, Point point2)
        {
            List<Point> vs = new List<Point>();
            if (point1 == point2)
                return vs;
            var x0 = Convert.ToInt32(point1.Y);
            var y0 = Convert.ToInt32(point1.X);
            var x1 = Convert.ToInt32(point2.Y);
            var y1 = Convert.ToInt32(point2.X);
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            var buf = x0;
            if (steep)
            {
                x0 = y0;
                y0 = buf;
                buf = x1;
                x1 = y1;
                y1 = buf;
            }
            if (x0 > x1)
            {
                buf = x0;
                x0 = x1;
                x1 = buf;
                buf = y0;
                y0 = y1;
                y1 = buf;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x < x1; x++)
            {
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
                vs.Add(new Point(Convert.ToInt32(steep ? x : y), Convert.ToInt32(steep ? y : x)));
            }
            return vs;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var edge = obj as Edge;

            var samePoints = Point1 == edge.Point1 && Point2 == edge.Point2;
            var samePointsReversed = Point1 == edge.Point2 && Point2 == edge.Point1;
            return samePoints || samePointsReversed;
        }

        public override int GetHashCode()
        {
            int hCode = (int)Point1.X ^ (int)Point1.Y ^ (int)Point2.X ^ (int)Point2.Y;
            return hCode.GetHashCode();
        }
    }
}
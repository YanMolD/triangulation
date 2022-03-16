using System;
using System.Collections.Generic;
using System.Linq;

namespace triangle
{
    public class Triangle
    {
        public Point[] Dots { get; } = new Point[3]; //вершины
        public Point CircC { get; private set; } //центр окружности
        public double RSc; //квадрат радиуса

        public IEnumerable<Triangle> TrianglesWithSharedEdge {
            get {
                var neighbors = new HashSet<Triangle>();
                foreach (var dot in Dots)
                {
                    var trianglesWithSharedEdge = dot.NearbyTriangles.Where(o =>
                    {
                        return o != this && SharesEdgeWith(o);
                    });
                    neighbors.UnionWith(trianglesWithSharedEdge);
                }

                return neighbors;
            }
        }

        public Triangle(Point point1, Point point2, Point point3)
        {
            if (point1 == point2 || point1 == point3 || point2 == point3)
            {
                throw new ArgumentException();
            }

            if (!IsCounter(point1, point2, point3))
            {
                Dots[0] = point1;
                Dots[1] = point3;
                Dots[2] = point2;
            }
            else
            {
                Dots[0] = point1;
                Dots[1] = point2;
                Dots[2] = point3;
            }

            Dots[0].NearbyTriangles.Add(this);
            Dots[1].NearbyTriangles.Add(this);
            Dots[2].NearbyTriangles.Add(this);
            UpdateCircC();
        }

        private void UpdateCircC()  //апдейт центра
        {
            var p0 = Dots[0];
            var p1 = Dots[1];
            var p2 = Dots[2];
            var dA = p0.X * p0.X + p0.Y * p0.Y;
            var dB = p1.X * p1.X + p1.Y * p1.Y;
            var dC = p2.X * p2.X + p2.Y * p2.Y;

            var aux1 = (dA * (p2.Y - p1.Y) + dB * (p0.Y - p2.Y) + dC * (p1.Y - p0.Y));
            var aux2 = -(dA * (p2.X - p1.X) + dB * (p0.X - p2.X) + dC * (p1.X - p0.X));
            var div = (2 * (p0.X * (p2.Y - p1.Y) + p1.X * (p0.Y - p2.Y) + p2.X * (p1.Y - p0.Y)));

            if (div == 0)
            {
                throw new DivideByZeroException();
            }

            var center = new Point(aux1 / div, aux2 / div);
            CircC = center;
            RSc = (center.X - p0.X) * (center.X - p0.X) + (center.Y - p0.Y) * (center.Y - p0.Y);
        }

        private bool IsCounter(Point point1, Point point2, Point point3)  //против часовой
        {
            var result = (point2.X - point1.X) * (point3.Y - point1.Y) -
                (point3.X - point1.X) * (point2.Y - point1.Y);
            return result > 0;
        }

        public bool SharesEdgeWith(Triangle triangle)
        {
            var sharedDots = Dots.Where(o => triangle.Dots.Contains(o)).Count();
            return sharedDots == 2;
        }

        public bool IsPointInsideCircle(Point point)
        {
            var d_squared = (point.X - CircC.X) * (point.X - CircC.X) +
                (point.Y - CircC.Y) * (point.Y - CircC.Y);
            return d_squared < RSc;
        }
    }
}
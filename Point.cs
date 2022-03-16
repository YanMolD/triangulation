using System.Collections.Generic;

namespace triangle
{
    public class Point
    {
        public double X { get; }
        public double Y { get; }
        public HashSet<Triangle> NearbyTriangles { get; } = new HashSet<Triangle>();
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
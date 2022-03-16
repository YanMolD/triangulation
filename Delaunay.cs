using System;
using System.Collections.Generic;
using System.Linq;

namespace triangle
{
    public class DelaunayTriangulator
    {
        private double MaxX { get; set; }
        private double MaxY { get; set; }
        private IEnumerable<Triangle> MCS;//минимальная выпуклая оболочка

        public IEnumerable<Point> GenP(int amount, double maxX, double maxY)// генерация точек
        {
            MaxX = maxX;
            MaxY = maxY;
            var point0 = new Point(0, 0);
            var point1 = new Point(0, MaxY);
            var point2 = new Point(MaxX, MaxY);
            var point3 = new Point(MaxX, 0);
            var points = new List<Point>() { point0, point1, point2, point3 };
            var tri1 = new Triangle(point0, point1, point2);
            var tri2 = new Triangle(point0, point2, point3);
            MCS = new List<Triangle>() { tri1, tri2 };

            var random = new Random();
            for (int i = 0; i < amount - 4; i++)
            {
                var pointX = random.NextDouble() * MaxX;
                var pointY = random.NextDouble() * MaxY;
                points.Add(new Point(pointX, pointY));
            }

            return points;
        }

        public IEnumerable<Triangle> BowyerWatson(IEnumerable<Point> points)// метод Боуэра-Ватсона
        {
            var triangulation = new HashSet<Triangle>(MCS);

            foreach (var point in points)
            {
                var badTriangles = FindBadTriangles(point, triangulation);
                var polygon = FindHoleBoundaries(badTriangles);

                foreach (var triangle in badTriangles)
                {
                    foreach (var vertex in triangle.Dots)
                    {
                        vertex.NearbyTriangles.Remove(triangle);
                    }
                }
                triangulation.RemoveWhere(o => badTriangles.Contains(o));

                foreach (var edge in polygon.Where(possibleEdge => possibleEdge.Point1 != point && possibleEdge.Point2 != point))
                {
                    var triangle = new Triangle(point, edge.Point1, edge.Point2);
                    triangulation.Add(triangle);
                }
            }
            return triangulation;
        }

        private List<Edge> FindHoleBoundaries(ISet<Triangle> badTriangles)// поиск всех границ
        {
            var edges = new List<Edge>();
            foreach (var triangle in badTriangles)
            {
                edges.Add(new Edge(triangle.Dots[0], triangle.Dots[1]));
                edges.Add(new Edge(triangle.Dots[1], triangle.Dots[2]));
                edges.Add(new Edge(triangle.Dots[2], triangle.Dots[0]));
            }
            var grouped = edges.GroupBy(e => e);
            var boundaryEdges = edges.GroupBy(e => e).Where(e => e.Count() == 1).Select(e => e.First());
            return boundaryEdges.ToList();
        }
        private ISet<Triangle> FindBadTriangles(Point point, HashSet<Triangle> triangles) //неподходящие треугольники
        {
            var badTriangles = triangles.Where(o => o.IsPointInsideCircle(point));
            return new HashSet<Triangle>(badTriangles);
        }
    }
}
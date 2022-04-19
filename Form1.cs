using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace triangle
{
    public partial class Form1 : Form
    {
        private DelaunayTriangulator delaunay = new DelaunayTriangulator();
        List<Point> points=new List<Point>();
        public Form1()
        {
            InitializeComponent();
            
            
        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int CursorX = Cursor.Position.X - pictureBox1.Location.X - 8 - this.Left;
            int CursorY = Cursor.Position.Y - pictureBox1.Location.Y - 30 - this.Top;
            Graphics gr = pictureBox1.CreateGraphics();
            gr.DrawLine(new Pen(Color.Violet, 10), CursorX - 5, CursorY, CursorX + 5, CursorY);
            points.Add(new Point(CursorX * pictureBox1.Image.Width / pictureBox1.Width, CursorY * pictureBox1.Image.Height / pictureBox1.Height));
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //var points = delaunay.GenP(Convert.ToInt32(textBox1.Text),pictureBox1.Width-10,pictureBox1.Height-10);
            Bitmap bitmap = new Bitmap(pictureBox1.Image);
            var triangulation = delaunay.BowyerWatson(points, bitmap.Height - 10, bitmap.Width - 10);
            pictureBox1.Refresh();

                Bitmap outputY = new Bitmap(bitmap.Width, bitmap.Height);
                Bitmap outputCb = new Bitmap(bitmap.Width, bitmap.Height);
                Bitmap outputCr = new Bitmap(bitmap.Width, bitmap.Height);

                for (int j = 0; j < bitmap.Height; j++)
                    for (int i = 0; i < bitmap.Width; i++)
                    {


                        UInt32 pixel = (UInt32)(bitmap.GetPixel(i, j).ToArgb());
                        double R = ((pixel & 0x00FF0000) >> 16);
                        double G = ((pixel & 0x0000FF00) >> 8);
                        double B = (pixel & 0x000000FF);
                        double Y = R * .299000 + G * .587000 + B * .114000;
                        double Cb = R * -.168736 + G * -.331264 + B * .500000 + 128;
                        double Cr = 0.439 * R - 0.368 * G - 0.071 * B + 128;
                        UInt32 newPixelY = 0xFF000000 | ((UInt32)Y << 16) | ((UInt32)Y << 8) | ((UInt32)Y);
                        UInt32 newPixelCb = 0xFF000000 | ((UInt32)Cb) | ((UInt32)Y << 8);
                        UInt32 newPixelCr = 0xFF000000 | ((UInt32)Cr << 16) | ((UInt32)Y << 8);
                        outputY.SetPixel(i, j, Color.FromArgb((int)newPixelY));
                        outputCb.SetPixel(i, j, Color.FromArgb((int)newPixelCb));
                        outputCr.SetPixel(i, j, Color.FromArgb((int)newPixelCr));
                    }
                
            DrawTriangulation(triangulation, pictureBox1.CreateGraphics(), outputY);
            DrawTriangulation(triangulation, pictureBox1.CreateGraphics(), outputCb);
            DrawTriangulation(triangulation, pictureBox1.CreateGraphics(), outputCr);
            Bitmap outputBase = new Bitmap(outputY.Width, outputY.Height);
            for (int j = 0; j < outputY.Height; j++)
                for (int i = 0; i < outputY.Width; i++)
                {
                    UInt32 pixelY = (UInt32)(outputY.GetPixel(i, j).ToArgb());
                    UInt32 pixelCb = (UInt32)(outputCb.GetPixel(i, j).ToArgb());
                    UInt32 pixelCr = (UInt32)(outputCr.GetPixel(i, j).ToArgb());
                    double Y = ((pixelY & 0x00FF0000) >> 16);
                    double Cb = ((pixelCb & 0x000000FF));
                    double Cr = ((pixelCr & 0x00FF0000) >> 16);
                    double R = Y + 1.4075 * (Cr - 128);
                    double G = Y - 0.3455 * (Cb - 128) - 0.7169 * (Cr - 128);
                    double B = Y + 1.779 * (Cb - 128);
                    UInt32 newPixelBase = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                    outputBase.SetPixel(i, j, Color.FromArgb((int)newPixelBase));
                }

            pictureBox1.Image = outputBase;
        }
        
        private void DrawTriangulation(IEnumerable<Triangle> triangulation, Graphics gr, Bitmap bitmap)
        {
            var edges = new List<Edge>();
            foreach (var triangle in triangulation)
            {
                edges.Add(new Edge(triangle.Dots[0], triangle.Dots[1]));
                edges.Add(new Edge(triangle.Dots[1], triangle.Dots[2]));
                edges.Add(new Edge(triangle.Dots[2], triangle.Dots[0]));
                BresenhamLine(triangle.Dots[0], triangle.Dots[1], bitmap);
                BresenhamLine(triangle.Dots[1], triangle.Dots[2], bitmap);
                BresenhamLine(triangle.Dots[2], triangle.Dots[0], bitmap);
                PaintTriangle(BresenhamLine(triangle.Dots[0], triangle.Dots[1], bitmap),
                BresenhamLine(triangle.Dots[1], triangle.Dots[2], bitmap), bitmap);
                PaintTriangle(BresenhamLine(triangle.Dots[2], triangle.Dots[0], bitmap),
                BresenhamLine(triangle.Dots[0], triangle.Dots[1], bitmap), bitmap);
                PaintTriangle(BresenhamLine(triangle.Dots[1], triangle.Dots[2], bitmap),
                BresenhamLine(triangle.Dots[1], triangle.Dots[0], bitmap), bitmap);
                /*            var x1 = triangle.Dots[0].X;
                               var x2 = triangle.Dots[1].X;
                               var x3 = triangle.Dots[2].X;
                               var y1 = triangle.Dots[0].Y;
                               var y2 = triangle.Dots[1].Y;
                               var y3 = triangle.Dots[2].Y;
                               var x4 = (x1 + x2) / 2;
                               var y4 = (y1 + y2) / 2;
                               var x5 = (x1 + x3) / 2;
                               var y5 = (y1 + y3) / 2;
                               var a1 = x2 - x1;
                               var b1 = y2 - y1;
                               var c1 = x4 * (x2 - x1) + y4 * (y2 - y1);
                               var a2 = x3 - x1;
                               var b2 = y3 - y1;
                               var c2 = x5 * (x3 - x1) + y5 * (y3 - y1);
                               var xo = (c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1);
                               var yo = (a1 * c2 - a2 * c1) / (a1 * b2 - a2 * b1);
                               var r = Math.Sqrt(Math.Pow((x1 - xo), 2) + Math.Pow(y1 - yo, 2))*2;
                               gr.DrawEllipse(new Pen(Color.Red), Convert.ToInt64(xo)-Convert.ToInt64(r)/2, Convert.ToInt64(yo)- Convert.ToInt64(r)/2, Convert.ToInt64(r), Convert.ToInt64(r));*/
            }

            foreach (var edge in edges)
            {
              //  gr.DrawLine(new Pen(Color.Black), Convert.ToInt64(edge.Point1.X), Convert.ToInt64(edge.Point1.Y), Convert.ToInt64(edge.Point2.X), Convert.ToInt64(edge.Point2.Y));
               // gr.DrawLine(new Pen(Color.Violet, 10), new PointF(Convert.ToInt64(edge.Point1.X - 5), Convert.ToInt64(edge.Point1.Y - 5)), new PointF(Convert.ToInt64(edge.Point1.X + 5), Convert.ToInt64(edge.Point1.Y + 5)));
               // gr.DrawLine(new Pen(Color.Violet, 10), new PointF(Convert.ToInt64(edge.Point2.X - 5), Convert.ToInt64(edge.Point2.Y - 5)), new PointF(Convert.ToInt64(edge.Point2.X + 5), Convert.ToInt64(edge.Point2.Y + 5)));
                

            }
            //Bitmap bitmap1 = new Bitmap(bitmap.Height, bitmap.Width);
            //for (int i = 0; i < bitmap1.Height; i++)
              //  for (int j = 0; j < bitmap1.Width; j++)
                //    bitmap1.SetPixel(j, i, bitmap.GetPixel(i, j));
        }

        
        List<Point> BresenhamLine(Point point1,Point point2,Bitmap bitmap)
        {
            List<Point> vs = new List<Point>();
            if (point1 == point2)
                return vs;
            UInt32 start = (UInt32)bitmap.GetPixel(Convert.ToInt32(point1.X), Convert.ToInt32(point1.Y)).ToArgb();
            UInt32 end = (UInt32)bitmap.GetPixel(Convert.ToInt32(point2.X), Convert.ToInt32(point2.Y)).ToArgb();
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
            UInt32 Colorstep;
            if (x1 != x0)
                Colorstep = Convert.ToUInt32((end - start) / (x1 - x0));
            else
                Colorstep = 0;
            for (int x = x0; x < x1; x++)
            {
                bitmap.SetPixel(steep ? x : y, steep ? y : x, Color.FromArgb((int)(start+Colorstep*(x-x0))));
                pictureBox1.Image = bitmap;
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
        void PaintTriangle(List<Point> edge1, List<Point> edge2, Bitmap bitmap)
        {
            if (!(DistCalc(edge1.First(), edge2.First()) < DistCalc(edge1.First(), edge2.Last()) &&
                DistCalc(edge1.First(), edge2.First()) < DistCalc(edge1.Last(), edge2.First()) &&
                DistCalc(edge1.First(), edge2.First()) < DistCalc(edge1.Last(), edge2.Last())))
            {
                edge1.Reverse();
                if (!(DistCalc(edge1.First(), edge2.First()) < DistCalc(edge1.First(), edge2.Last()) &&
                    DistCalc(edge1.First(), edge2.First()) < DistCalc(edge1.Last(), edge2.First()) &&
                    DistCalc(edge1.First(), edge2.First()) < DistCalc(edge1.Last(), edge2.Last())))
                {
                    edge2.Reverse();
                    if (!(DistCalc(edge1.First(), edge2.First()) < DistCalc(edge1.First(), edge2.Last()) &&
                        DistCalc(edge1.First(), edge2.First()) < DistCalc(edge1.Last(), edge2.First()) &&
                        DistCalc(edge1.First(), edge2.First()) < DistCalc(edge1.Last(), edge2.Last())))
                        edge1.Reverse();
                }
            }

                for (int i = edge1.Count - 1, j = edge2.Count - 1; i >= 0 && j >= 0; i--, j--)
            {
                BresenhamLine(edge1[i], edge2[j], bitmap);
            }
        }
        double DistCalc(Point FirstPoint, Point SecondPoint)
        {
            double scr= Math.Sqrt(Math.Pow(FirstPoint.X - SecondPoint.X, 2) + Math.Pow(FirstPoint.Y - SecondPoint.Y, 2)); ;
            return scr;
        }
    }
}

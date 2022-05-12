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
        List<Point> Dots= new List<Point>();
        public Form1()
        {
            InitializeComponent();
        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Graphics gr = pictureBox2.CreateGraphics();
            for (int i = 0; i < Dots.Count; i++)
                gr.DrawEllipse(new Pen(Color.Violet, 3), Convert.ToInt32(Dots[i].X), Convert.ToInt32(Dots[i].Y), 3, 3);
            int CursorX = Cursor.Position.X - pictureBox1.Location.X - 8 - this.Left;
            int CursorY = Cursor.Position.Y - pictureBox1.Location.Y - 30 - this.Top;
            gr.DrawEllipse(new Pen(Color.Violet, 3), CursorX - 3, CursorY - 3, 3, 3);
            if (!Dots.Contains(new Point(CursorX - 3, CursorY - 3)))
                Dots.Add(new Point(CursorX - 3, CursorY - 3));
            var OtstupY = pictureBox1.Height * pictureBox1.Image.Width / pictureBox1.Width - pictureBox1.Image.Height;
            var OtstupX = pictureBox1.Width * pictureBox1.Image.Height / pictureBox1.Height - pictureBox1.Image.Width;
            if (OtstupX < 0)
                OtstupX = 0;
            if (OtstupY < 0)
                OtstupY = 0;
            points.Add(new Point(CursorX * pictureBox1.Image.Width / pictureBox1.Width + OtstupX * (((float)CursorX / pictureBox1.Width) - 0.5), (CursorY) * pictureBox1.Image.Height / pictureBox1.Height+OtstupY*(((float)CursorY/pictureBox1.Height)-0.5)));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
                return;
            Bitmap bitmap = new Bitmap(pictureBox1.Image);
            //points = delaunay.GenP(1000, bitmap.Width - 1, bitmap.Height - 1);
            var triangulation = delaunay.BowyerWatson(points, bitmap.Height -1, bitmap.Width - 1);
            pictureBox1.Refresh();
            Bitmap outputY = new Bitmap(bitmap.Width, bitmap.Height);
            Bitmap outputCb = new Bitmap(bitmap.Width, bitmap.Height);
            Bitmap outputCr = new Bitmap(bitmap.Width, bitmap.Height);
            Bitmap outputY1 = new Bitmap(bitmap.Width, bitmap.Height);
            Bitmap outputCb1 = new Bitmap(bitmap.Width, bitmap.Height);
            Bitmap outputCr1 = new Bitmap(bitmap.Width, bitmap.Height);
            Bitmap outputBase1 = new Bitmap(outputY.Width, outputY.Height);
            for (int j = 0; j < bitmap.Height; j++)
                for (int i = 0; i < bitmap.Width; i++)
                {
                    double R = bitmap.GetPixel(i, j).R;
                    double G = bitmap.GetPixel(i, j).G;
                    double B = bitmap.GetPixel(i, j).B;
                    double Y = R * .299000 + G * .587000 + B * .114000;
                    if (Y < 16)
                        Y = 16;
                    if (Y > 235)
                        Y = 235;
                    double Cb = R * -.168736 + G * -.331264 + B * .500000 + 128;
                    if (Cb < 16)
                        Cb = 16;
                    if (Cb > 240)
                        Cb = 240;
                    double Cr = 0.5 * R - 0.418688 * G - 0.081312 * B + 128;
                    if (Cr < 16)
                        Cr = 16;
                    if (Cr > 240)
                        Cr = 240;
                    UInt32 newPixelY = 0xFF000000 | ((UInt32)Y << 16) | ((UInt32)Y << 8) | ((UInt32)Y);
                    UInt32 newPixelCb = 0xFF000000 | ((UInt32)Cb << 8) | ((UInt32)Cr);
                    UInt32 newPixelCr = 0xFF000000 | ((UInt32)Cr << 16) | ((UInt32)Cb << 8);
                    outputY.SetPixel(i, j, Color.FromArgb((int)newPixelY));
                    outputCb.SetPixel(i, j, Color.FromArgb((int)newPixelCb));
                    outputCr.SetPixel(i, j, Color.FromArgb((int)newPixelCr));
                }
            DrawTriangulation(triangulation, outputY, outputY1);
            DrawTriangulation(triangulation, outputCb, outputCb1);
            DrawTriangulation(triangulation, outputCr, outputCr1);

            for (int j = 1; j < outputY1.Height; j++)
            {
                for (int i = 1; i < outputY1.Width; i++)
                {
                    double Y = outputY1.GetPixel(i, j).R;
                    double Cb = outputCb1.GetPixel(i, j).G;
                    double Cr = outputCr1.GetPixel(i, j).R;
                    double A = outputY1.GetPixel(i, j).A;
                    if (A == 0)
                    {
                        outputY1.SetPixel(i, j, outputY1.GetPixel(i - 1, j));
                        outputCb1.SetPixel(i, j, outputCb1.GetPixel(i - 1, j));
                        outputCr1.SetPixel(i, j, outputCr1.GetPixel(i - 1, j));
                        outputBase1.SetPixel(i, j, outputBase1.GetPixel(i - 1, j));
                    }
                    else
                    {
                        double R = Y + 1.4075 * (Cr - 128);
                        if (R > 255)
                            R = 255;
                        if (R < 0)
                            R = 0;
                        double G = Y - 0.3455 * (Cb - 128) - 0.7169 * (Cr - 128);
                        if (G > 255)
                            G = 255;
                        if (G < 0)
                            G = 0;
                        double B = Y + 1.779 * (Cb - 128);
                        if (B > 255)
                            B = 255;
                        if (B < 0)
                            B = 0;
                        UInt32 newPixelBase = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        outputBase1.SetPixel(i, j, Color.FromArgb((int)newPixelBase));
                    }
                }
            }
            pictureBox12.Image = outputBase1;
            pictureBox3.Image = outputY1;
            pictureBox4.Image = outputCb1;
            pictureBox5.Image = outputCr1;
        }
        
        private void DrawTriangulation(IEnumerable<Triangle> triangulation, Bitmap bitmapBase, Bitmap bitmap)
        {
            Bitmap triangulat = new Bitmap(pictureBox1.Image);
            ;
            foreach (var triangle in triangulation)
            {
                //if ((delaunay.MCS.ToArray()[0].Dots.Any(s => s == triangle.Dots[0] || s == triangle.Dots[1] || s == triangle.Dots[2]))|| delaunay.MCS.ToArray()[1].Dots.Any(s => s == triangle.Dots[0] || s == triangle.Dots[1] || s == triangle.Dots[2]))
                //{
                    //PaintTriangle(BresenhamLineBase(triangle.Dots[1], triangle.Dots[0], bitmap, bitmapBase), BresenhamLineBase(triangle.Dots[2], triangle.Dots[0], bitmap, bitmapBase), bitmap, bitmapBase);
                //}
                //else
                //{
                    bitmap.SetPixel(Convert.ToInt32(triangle.Dots[0].X), Convert.ToInt32(triangle.Dots[0].Y), bitmapBase.GetPixel(Convert.ToInt32(triangle.Dots[0].X), Convert.ToInt32(triangle.Dots[0].Y)));
                    bitmap.SetPixel(Convert.ToInt32(triangle.Dots[1].X), Convert.ToInt32(triangle.Dots[1].Y), bitmapBase.GetPixel(Convert.ToInt32(triangle.Dots[1].X), Convert.ToInt32(triangle.Dots[1].Y)));
                    bitmap.SetPixel(Convert.ToInt32(triangle.Dots[2].X), Convert.ToInt32(triangle.Dots[2].Y), bitmapBase.GetPixel(Convert.ToInt32(triangle.Dots[2].X), Convert.ToInt32(triangle.Dots[2].Y)));
                    if (triangle.Edges[2].BrDots.Count != 0 && triangle.Edges[1].BrDots.Count != 0)

                        PaintTriangle(BresenhamLine(triangle.Dots[1], triangle.Dots[0], bitmap), BresenhamLine(triangle.Dots[2], triangle.Dots[0], bitmap), bitmap);

                    if (Dots.Count != 0)
                    {
                        BresenhamLineRed(triangle.Dots[0], triangle.Dots[1], triangulat);
                        BresenhamLineRed(triangle.Dots[1], triangle.Dots[2], triangulat);
                        BresenhamLineRed(triangle.Dots[2], triangle.Dots[0], triangulat);
                    }
                //}
            }
            pictureBox2.Image = triangulat;
            
        }


        List<Point> BresenhamLine(Point point1, Point point2, Bitmap bitmap)
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
            bool fal = false;
            if (point1.X == 367 && point2.X == 0)
                Task.Delay(100);
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
                fal = true;
                buf = x0;
                x0 = x1;
                x1 = buf;
                buf = y0;
                y0 = y1;
                y1 = buf;
            }
            double R = bitmap.GetPixel(Convert.ToInt32(point1.X), Convert.ToInt32(point1.Y)).R;
            double G = bitmap.GetPixel(Convert.ToInt32(point1.X), Convert.ToInt32(point1.Y)).G;
            double B = bitmap.GetPixel(Convert.ToInt32(point1.X), Convert.ToInt32(point1.Y)).B;
            double A = bitmap.GetPixel(Convert.ToInt32(point1.X), Convert.ToInt32(point1.Y)).A;
            double R1 = bitmap.GetPixel(Convert.ToInt32(point2.X), Convert.ToInt32(point2.Y)).R;
            double G1 = bitmap.GetPixel(Convert.ToInt32(point2.X), Convert.ToInt32(point2.Y)).G;
            double B1 = bitmap.GetPixel(Convert.ToInt32(point2.X), Convert.ToInt32(point2.Y)).B;
            double A1 = bitmap.GetPixel(Convert.ToInt32(point1.X), Convert.ToInt32(point1.Y)).A;
            if (R == 0)
                Task.Delay(100);
            if (A == 0 || A1 == 0)
                return vs;

            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            double ColorstepR;
            if (x1 != x0)
                ColorstepR = (((R - R1) / (x1 - x0)));
            else
                ColorstepR = 0;
            double ColorstepG;
            if (x1 != x0)
                ColorstepG = (((G - G1) / (x1 - x0)));
            else
                ColorstepG = 0;
            double ColorstepB;
            if (x1 != x0)
                ColorstepB = (((B - B1) / (x1 - x0)));
            else
                ColorstepB = 0;
            if (R + ColorstepR * (x1 - x0) > 255)
                dx = x1 - x0;
            bitmap.SetPixel(steep ? x0 : y, steep ? y : x0, fal ? Color.FromArgb((int)R1, (int)G1, (int)B1) :
                    Color.FromArgb((int)R, (int)G, (int)B));
            for (int x = x0 ; x <= x1; x++)
            {
                if ((B - ColorstepB * (x - x0)) == 0)
                    Task.Delay(100);
                if ((B1 + ColorstepB * (x - x0)) == 0)
                    Task.Delay(100);
                bitmap.SetPixel(steep ? x : y, steep ? y : x, fal ? Color.FromArgb((int)(R1 + ColorstepR * (x - x0)), (int)(G1 + ColorstepG * (x - x0)), (int)(B1 + ColorstepB * (x - x0))) :
                    Color.FromArgb((int)(R - ColorstepR * (x - x0)), (int)(G - ColorstepG * (x - x0)), (int)(B - ColorstepB * (x - x0))));
                vs.Add(new Point(Convert.ToInt32(steep ? x : y), Convert.ToInt32(steep ? y : x)));
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                    
                }
            }
            return vs;
        }
        void BresenhamLineRed(Point point1, Point point2, Bitmap bitmap)
        {
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
            for (int x = x0; x <= x1; x++)
            {
                bitmap.SetPixel(steep ? x : y, steep ? y : x, Color.Red);
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;

                }
            }
        }
        List<Point> BresenhamLineBase(Point point1, Point point2, Bitmap bitmap, Bitmap bitmapBase)
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
            for (int x = x0; x <= x1; x++)
            {
                bitmap.SetPixel(steep ? x : y, steep ? y : x, bitmapBase.GetPixel(steep ? x : y, steep ? y : x));
                vs.Add(new Point(Convert.ToInt32(steep ? x : y), Convert.ToInt32(steep ? y : x)));
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;

                }
            }
            return vs;
        }
           
            
            
            
            
        void PaintTriangle(List<Point> edge1, List<Point> edge2, Bitmap bitmap1)
        {
            if (edge1.Count == 0 || edge2.Count == 0)
                return;
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
                BresenhamLine(edge1[i], edge2[j], bitmap1);
            }
        }
        void PaintTriangle(List<Point> edge1, List<Point> edge2,Bitmap bitmap1, Bitmap bitmap)
        {
            if (edge1.Count == 0 || edge2.Count == 0)
                return;
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
                BresenhamLineBase(edge1[i], edge2[j], bitmap1, bitmap);
            }
        }
        double DistCalc(Point FirstPoint, Point SecondPoint)
        {
            double scr= Math.Sqrt(Math.Pow(FirstPoint.X - SecondPoint.X, 2) + Math.Pow(FirstPoint.Y - SecondPoint.Y, 2)); ;
            return Math.Ceiling(scr);
        }
        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                    pictureBox2.Image = new Bitmap(ofd.FileName);
                    pictureBox12.Image = new Bitmap(ofd.FileName);
                }
                catch 
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            int CursorX = Cursor.Position.X - pictureBox1.Location.X - 8 - this.Left;
            int CursorY = Cursor.Position.Y - pictureBox1.Location.Y - 30 - this.Top;
            Graphics gr = pictureBox1.CreateGraphics();
            gr.DrawEllipse(new Pen(Color.Violet, 3), CursorX - 3, CursorY - 3, 3, 3);
            Dots.Add(new Point(CursorX - 3, CursorY - 3));
            points.Add(new Point(CursorX * pictureBox1.Image.Width / pictureBox1.Width , CursorY * pictureBox1.Image.Height / pictureBox1.Height));
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Width= Size.Width / 3 - 20;
            pictureBox1.Height= Size.Height / 2 - 20;
            pictureBox1.Location = new System.Drawing.Point(10, 0);

            pictureBox2.Width = Size.Width / 3 - 20;
            pictureBox2.Height = Size.Height / 2 - 20;
            pictureBox2.Location = new System.Drawing.Point(Size.Width / 3 , 0);
            pictureBox12.Width = Size.Width / 3 - 20;
            pictureBox12.Height = Size.Height / 2 - 20;
            pictureBox12.Location = new System.Drawing.Point(Size.Width * 2 / 3, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            points.Clear();
            Dots.Clear();
            pictureBox2.Image=pictureBox1.Image;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Width = Size.Width / 3 - 20;
            pictureBox1.Height = Size.Height / 2 - 20;
            pictureBox1.Location = new System.Drawing.Point(10, 0);
            pictureBox2.Width = Size.Width / 3 - 20;
            pictureBox2.Height = Size.Height / 2 - 20;
            pictureBox2.Location = new System.Drawing.Point(Size.Width / 3, 0);
            
            pictureBox12.Width = Size.Width / 3 - 20;
            pictureBox12.Height = Size.Height / 2 - 20;
            pictureBox12.Location = new System.Drawing.Point(Size.Width * 2 / 3, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox12.Location.X != 0)
            {
                pictureBox12.Location = new System.Drawing.Point(0, 0);
                pictureBox12.Size = (this.Size);
            }
            else
            {
                pictureBox12.Width = Size.Width / 3 - 20;
                pictureBox12.Height = Size.Height / 2 - 20;
                pictureBox12.Location = new System.Drawing.Point(Size.Width * 2 / 3, 0);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (pictureBox12.Image != null) // если изображение в pictureBox2 имеется
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Сохранить картинку как...";
                sfd.OverwritePrompt = true;
                sfd.CheckPathExists = true;

                sfd.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                sfd.ShowHelp = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {

                        pictureBox12.Image.Save(sfd.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

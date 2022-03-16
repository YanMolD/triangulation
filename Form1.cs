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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var points = delaunay.GenP(Convert.ToInt32(textBox1.Text),pictureBox1.Width-10,pictureBox1.Height-10);

            var triangulation = delaunay.BowyerWatson(points);
            pictureBox1.Refresh();
            DrawTriangulation(triangulation, pictureBox1.CreateGraphics());
        }
        private void DrawTriangulation(IEnumerable<Triangle> triangulation, Graphics gr)
        {
            var edges = new List<Edge>();
            foreach (var triangle in triangulation)
            {
                edges.Add(new Edge(triangle.Dots[0], triangle.Dots[1]));
                edges.Add(new Edge(triangle.Dots[1], triangle.Dots[2]));
                edges.Add(new Edge(triangle.Dots[2], triangle.Dots[0]));
            }

            foreach (var edge in edges)
            {
                gr.DrawLine(new Pen(Color.Black),Convert.ToInt64(edge.Point1.X), Convert.ToInt64(edge.Point1.Y), Convert.ToInt64(edge.Point2.X), Convert.ToInt64(edge.Point2.Y));
            }
        }
    }
}

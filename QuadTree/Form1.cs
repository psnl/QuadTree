using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuadTree
{
    public partial class Form1 : Form
    {
        QuadTree qt = null;
        Rectangle req = new Rectangle();

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            var boundary = new QtRectangle(200, 200, 200, 200);
            qt = new QuadTree(boundary, 4);
            List<Point> points = new List<Point>();
            var rnd = new Random();
            for (int i = 0; i < 500; i++)
            {
                var point = new QtPoint(rnd.NextDouble()*400, rnd.NextDouble()*400);
                qt.insert(point);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //qt?.show(e.Graphics);
            e.Graphics.DrawEllipse(new Pen(Color.Green, 2), req);
            //e.Graphics.DrawRectangle(new Pen(Color.Green, 2), req);
            textBox1.Text = string.Format("X: {0} , Y: {1}", req.X+50, req.Y+50);

            var hlp = new List<QtPoint>();
            var qtr = new QtCircle(req.X + 50, req.Y + 50, 50);
            //var qtr = new QtRectangle(req.X + 30, req.Y + 30, 30, 30);
            qt?.query(qtr, ref hlp);
            foreach (var p in hlp)
            {
                e.Graphics.DrawEllipse(new Pen(Color.Green, 2), new Rectangle((int)p.x, (int)p.y, 2, 2));
            }

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.X < 400) && (e.Y < 400))
            {
                req = new Rectangle(e.X-50, e.Y-50, 100, 100);
                panel1.Invalidate();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
        }
    }
}

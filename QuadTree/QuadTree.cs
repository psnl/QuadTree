using System;
using System.Collections.Generic;
using System.Drawing;

namespace QuadTree
{
    public class QtPoint
    {
        public QtPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double x {get; set;}
        public double y {get; set;}
    }

    public class QtRectangle
    {
        public QtRectangle(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public bool contains(QtPoint point)
        {
                return (point.x >= this.x - this.w &&
                      point.x <= this.x + this.w &&
                      point.y >= this.y - this.h &&
                      point.y <= this.y + this.h);
        }

        public bool intersects(QtRectangle range)
        {
            return !(range.x - range.w > this.x + this.w ||
                range.x + range.w < this.x - this.w ||
                range.y - range.h > this.y + this.h ||
                range.y + range.h < this.y - this.h);
        }

        public int x {get; set;}
        public int y {get; set;}
        public int w {get; set;}
        public int h {get; set;}
  
     }
    public class QtCircle
    {
        public QtCircle(int x, int y, int radius)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.rSquared = this.radius * this.radius;
        }

        public bool contains(QtPoint point)
        {
            // check if the point is in the circle by checking if the euclidean distance of
            // the point and the center of the circle if smaller or equal to the radius of
            // the circle
            var d = Math.Pow((point.x - this.x), 2) + Math.Pow((point.y - this.y), 2);
            return d <= this.rSquared;
        }

        public bool intersects(QtRectangle range)
        {

            var xDist = Math.Abs(range.x - this.x);
            var yDist = Math.Abs(range.y - this.y);

            // radius of the circle
            var r = this.radius;

            var w = range.w;
            var h = range.h;

            var edges = Math.Pow((xDist - w), 2) + Math.Pow((yDist - h), 2);

            // no intersection
            if (xDist > (r + w) || yDist > (r + h))
                return false;

            // intersection within the circle
            if (xDist <= w || yDist <= h)
                return true;

            // intersection on the edge of the circle
            return edges <= this.rSquared;
        }

        public int x { get; set; }
        public int y { get; set; }
        public int radius { get; set; }
        public int rSquared { get; set; }

    }

    public class QuadTree
    { 
        public QuadTree(QtRectangle boundary, int capacity)
        {
            this.boundary = boundary;
            this.capacity = capacity;
            this.points = new List<QtPoint>();
            this.divided = false;
        }

        public void subdivide()
        {
            var x = this.boundary.x;
            var y = this.boundary.y;
            var w = this.boundary.w;
            var h = this.boundary.h;

            var nw = new QtRectangle(x - w/2, y - h/2, w/2, h/2);
            this.northwest = new QuadTree(nw, this.capacity);
            var ne = new QtRectangle(x + w/2, y - h/2, w/2, h/2);
            this.northeast = new QuadTree(ne, this.capacity);
            var sw = new QtRectangle(x - w/2, y + h/2, w/2, h/2);
            this.southwest = new QuadTree(sw, this.capacity);
            var se = new QtRectangle(x + w/2, y + h/2, w/2, h/2);
            this.southeast = new QuadTree(se, this.capacity);
        }

        public bool insert(QtPoint point)
        {
            if (!this.boundary.contains(point))
            {
                return false;
            }
            if (this.points.Count < this.capacity)
            {
                this.points.Add(point);
                return true;
            }
            else
            {
                if (this.boundary.w > 4 && this.boundary.h > 4)
                {
                    if (!divided)
                    {
                        this.subdivide();
                        this.divided = true;
                    }
                    return (this.northeast.insert(point) || this.northwest.insert(point) ||
                          this.southeast.insert(point) || this.southwest.insert(point));
                }
                else
                {
                    return false;
                }
            }
        }

        public void query(QtRectangle range, ref List<QtPoint> found)
        {
            if (found == null)
            {
                found = new List<QtPoint>();
            }

            if (!range.intersects(this.boundary))
            {
                return;
            }

            foreach (var p in this.points)
            {
                if (range.contains(p))
                {
                    found.Add(p);
                }
            }
            if (this.divided)
            {
                this.northwest.query(range, ref found);
                this.northeast.query(range, ref found);
                this.southwest.query(range, ref found);
                this.southeast.query(range, ref found);
            }
            return;
        }

        public void query(QtCircle range, ref List<QtPoint> found)
        {
            if (found == null)
            {
                found = new List<QtPoint>();
            }

            if (!range.intersects(this.boundary))
            {
                return;
            }

            foreach (var p in this.points)
            {
                if (range.contains(p))
                {
                    found.Add(p);
                }
            }
            if (this.divided)
            {
                this.northwest.query(range, ref found);
                this.northeast.query(range, ref found);
                this.southwest.query(range, ref found);
                this.southeast.query(range, ref found);
            }
            return;
        }


        public void show(Graphics graphics)
        {
            graphics.DrawRectangle(new Pen(Color.Red, 2), new Rectangle(this.boundary.x - this.boundary.w, this.boundary.y- this.boundary.h, this.boundary.w*2, this.boundary.h*2));

            if (this.divided)
            {
                this.northwest.show(graphics);
                this.northeast.show(graphics);
                this.southwest.show(graphics);
                this.southeast.show(graphics);
            }

            foreach (var point in this.points)
            {
                graphics.DrawEllipse(new Pen(Color.Blue, 2), new Rectangle((int)point.x, (int)point.y, 2, 2));
            }
        }


        public List<QtPoint> points {get; set;}
        public QtRectangle boundary {get;set;}
        public int capacity {get;set;}

        private bool divided = false;
        public QuadTree northwest = null;
        public QuadTree northeast = null;
        public QuadTree southwest = null;
        public QuadTree southeast = null;
    }


}
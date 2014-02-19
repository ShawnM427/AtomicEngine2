using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AtomicEngine2.Utils
{
    public class Triangle
    {
        Vector2 _p1;
        Vector2 _p2;
        Vector2 _p3;

        Vector2 _orgin;
        float _extent;

        public Vector2 Orgin
        {
            get { return _orgin; }
        }
        public float Extents
        {
            get { return _extent; }
        }

        public Vector2 P1
        {
            get { return _p1; }
            set
            {
                _p1 = value;
                CalcBounds();
            }
        }
        public Vector2 P2
        {
            get { return _p2; }
            set
            {
                _p2 = value;
                CalcBounds();
            }
        }
        public Vector2 P3
        {
            get { return _p3; }
            set
            {
                _p3 = value;
                CalcBounds();
            }
        }

        public RectangleF Bounds
        {
            get { return _bounds; }
        }
        RectangleF _bounds;

        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            _bounds = new RectangleF(0, 0, 0, 0);

            _p1 = p1;
            _p2 = p2;
            _p3 = p3;

            CalcBounds();
        }

        public Triangle(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            _bounds = new RectangleF(0, 0, 0, 0);

            _p1 = new Vector2(x0,y0);
            _p2 = new Vector2(x1, y1);
            _p3 = new Vector2(x2, y2);

            CalcBounds();
        }

        private void CalcBounds()
        {
            float minX = Math.Min(Math.Min(P1.X, P2.X), P3.X);
            float minY = Math.Min(Math.Min(P1.Y, P2.Y), P3.Y);

            float maxX = Math.Max(Math.Max(P1.X, P2.X), P3.X);
            float maxY = Math.Max(Math.Max(P1.Y, P2.Y), P3.Y);

            _bounds.X = minX;
            _bounds.Y = minY;
            _bounds.Width = maxX - minX;
            _bounds.Height = maxY - minY;

            _orgin = (P1 + P2 + P3) / 3;
            _extent = _bounds.Extents;
        }
        
        public bool Contains(Vector2 pt)
        {
            bool b1, b2, b3;

            b1 = Sign(pt, P1, P2) < 0.0f;
            b2 = Sign(pt, P2, P3) < 0.0f;
            b3 = Sign(pt, P3, P1) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }

        private float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public bool Intersects(Triangle tri)
        {
            return false;
        }

        public bool Intersects(RectangleF rect)
        {
            return rect.Intersects(this);
        }

        public bool Intersects(LineSegment line)
        {
            LineSegment t = new LineSegment(_p1, _p2);
            if (t.Intersects(line))
                return true;
            t = new LineSegment(_p2, _p3);
            if (t.Intersects(line))
                return true; t = new LineSegment(_p3, _p1);
            if (t.Intersects(line))
                return true;

            return Contains(line.Start) || Contains(line.End);
        }

        public Vector2[] GetIntersect(LineSegment line)
        {
            List<Vector2> intersects = new List<Vector2>();

            LineSegment t = new LineSegment(_p1, _p2);
            Vector2? i = t.GetFirstIntersect(line);
            if (i != null)
                intersects.Add(i.Value);
            t = new LineSegment(_p2, _p3);
            i = t.GetFirstIntersect(line);
            if (i != null)
                intersects.Add(i.Value);
            i = t.GetFirstIntersect(line);
            if (i != null)
                intersects.Add(i.Value);

            if (Contains(line.Start))
                intersects.Add(line.Start);

            return intersects.ToArray(); ;
        }
    }
}

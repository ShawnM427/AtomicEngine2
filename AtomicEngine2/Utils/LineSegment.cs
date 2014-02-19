using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AtomicEngine2.Utils
{
    /// <summary>
    /// Represents a 2D line segment
    /// </summary>
    public class LineSegment
    {
        Vector2 _start;
        Vector2 _end;

        public Vector2 Start
        {
            get { return _start; }
            set { _start = value; }
        }
        public Vector2 End
        {
            get { return _end; }
            set { _end = value; }
        }

        public Vector2 Orgin
        {
            get { return (_start + _end) / 2; }
        }
        public float Extents
        {
            get { return Vector2.Distance(_start, _end); }
        }

        public LineSegment(Vector2 start, Vector2 end)
        {
            _start = start;
            _end = end;
        }

        public bool Intersects(RectangleF rect)
        {
            if (rect.Contains(_start) || rect.Contains(_end))
                return true;

            else
            {
                LineSegment t = new LineSegment(rect.TL, rect.TR);
                if (t.Intersects(this))
                    return true;
                t._start = rect.TR;
                t._end = rect.BR;
                if (t.Intersects(this))
                    return true;
                t._start = rect.BR;
                t._end = rect.BL;
                if (t.Intersects(this))
                    return true;
                t._start = rect.BL;
                t._end = rect.TL;
                if (t.Intersects(this))
                    return true;
            }
            return false;
        }

        public bool Intersects(Triangle tri)
        {
            return tri.Intersects(this);
        }

        public bool Intersects(LineSegment other)
        {
            float a1 = SignTriArea(_start, _end, other._end);
            float a2 = SignTriArea(_start, _end, other._start);

            if (a1 * a2 < 0f)
            {
                float a3 = SignTriArea(other._start, other._end, _start);
                float a4 = a3 + a2 - a1;

                if (a3 * a4 < 0f)
                    return true;
            }

            return false;
        }

        public bool Contains(Vector2 point)
        {
            return false;
        }

        public Vector2? GetFirstIntersect(RectangleF rect)
        {
            if (rect.Contains(_start) || rect.Contains(_end))
                return _start;

            else
            {
                LineSegment t = new LineSegment(rect.TL, rect.TR);
                Vector2? top = t.GetFirstIntersect(this);
                t._start = rect.TR;
                t._end = rect.BR;
                Vector2? right = t.GetFirstIntersect(this);
                t._start = rect.BR;
                t._end = rect.BL;
                Vector2? bottom = t.GetFirstIntersect(this);
                t._start = rect.BL;
                t._end = rect.TL;
                Vector2? left = t.GetFirstIntersect(this);

                Vector2? ret = null;
                float min = float.MaxValue;

                float d = 0;
                if (top != null)
                {
                    d = (top.Value.X - _start.X) * (top.Value.X - _start.X) +
                        (top.Value.Y - _start.Y) * (top.Value.Y - _start.Y);
                    if (d < min)
                    {
                        min = d;
                        ret = top;
                    }
                }
                if (right != null)
                {
                    d = (right.Value.X - _start.X) * (right.Value.X - _start.X) +
                        (right.Value.Y - _start.Y) * (right.Value.Y - _start.Y);
                    if (d < min)
                    {
                        min = d;
                        ret = right;
                    }
                }
                if (bottom != null)
                {
                    d = (bottom.Value.X - _start.X) * (bottom.Value.X - _start.X) +
                        (bottom.Value.Y - _start.Y) * (bottom.Value.Y - _start.Y);
                    if (d < min)
                    {
                        min = d;
                        ret = bottom;
                    }
                }
                if (left != null)
                {
                    d = (left.Value.X - _start.X) * (left.Value.X - _start.X) +
                        (left.Value.Y - _start.Y) * (left.Value.Y - _start.Y);
                    if (d < min)
                    {
                        min = d;
                        ret = left;
                    }
                }

                return ret;
            }
        }

        public Vector2[] GetIntersect(RectangleF rect)
        {
            if (rect.Contains(_start) || rect.Contains(_end))
                return new Vector2[]{_start};

            else
            {
                LineSegment t = new LineSegment(rect.TL, rect.TR);
                Vector2? top = t.GetFirstIntersect(this);
                t._start = rect.TR;
                t._end = rect.BR;
                Vector2? right = t.GetFirstIntersect(this);
                t._start = rect.BR;
                t._end = rect.BL;
                Vector2? bottom = t.GetFirstIntersect(this);
                t._start = rect.BL;
                t._end = rect.TL;
                Vector2? left = t.GetFirstIntersect(this);
                
                int count = (top == null ? 0 : 1) + (right == null ? 0 : 1)
                    + (bottom == null ? 0 : 1) + (left == null ? 0 : 1);
                Vector2[] ret = new Vector2[count];

                if (top != null)
                {
                    ret[ret.Count()] = top.Value;
                }
                if (right != null)
                {
                    ret[ret.Count()] = right.Value;
                }
                if (bottom != null)
                {
                    ret[ret.Count()] = bottom.Value;
                }
                if (left != null)
                {
                    ret[ret.Count()] = left.Value;
                }

                return ret;
            }
        }

        /// <summary>
        /// Gets the POI between 2 line segments
        /// </summary>
        /// <param name="other">The segment to check against</param>
        /// <returns>The point of intersection, or null</returns>
        public Vector2? GetFirstIntersect(LineSegment other)
        {
            return GetIntersect(other)[0];
        }

        /// <summary>
        /// Gets the POI between 2 line segments
        /// </summary>
        /// <param name="other">The segment to check against</param>
        /// <returns>The point of intersection, or null</returns>
        public Vector2[] GetIntersect(LineSegment other)
        {
            float ua = (other._end.X - other._start.X) * (_start.Y - other._start.Y) - (other._end.Y - other._start.Y) * (_start.X - other._start.X);
            float ub = (_end.X - _start.X) * (_start.Y - other._start.Y) - (_end.Y - _start.Y) * (_start.X - other._start.X);
            float denominator = (other._end.Y - other._start.Y) * (_end.X - _start.X) - (other._end.X - other._start.X) * (_end.Y - _start.Y);

            if (Math.Abs(denominator) <= 0.00001f)
            {
                if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
                {
                    return new Vector2[]{(_start + _end) / 2};
                }
            }
            else
            {
                ua /= denominator;
                ub /= denominator;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    return new Vector2[]{new Vector2(_start.X + ua * (_end.X - _start.X),
                        _start.Y + ua * (_end.Y - _start.Y))};
                }
            }
            return null;
        }

        private float SignTriArea(Vector2 pointA, Vector2 pointB, Vector2 pointC)
        {
            return (pointA.X - pointC.X) * (pointC.Y - pointB.Y) - (pointC.Y - pointA.Y) * (pointB.X - pointC.X);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace AtomicEngine2.Utils
{
    /// <summary>
    /// Represents a floating-point rectangle
    /// </summary>
    [DebuggerDisplay("X: {Left} - {Right} | Y: {Top} - {Bottom}")]
    public class RectangleF
    {
        Vector2 _tl;
        Vector2 _tr;
        Vector2 _bl;
        Vector2 _br;
        //Vector2 _orgin;

        float _width;
        float _height;

        public Vector2 TL
        {
            get { return _tl; }
        }
        public Vector2 TR
        {
            get { return _tr; }
        }
        public Vector2 BL
        {
            get { return _bl; }
        }
        public Vector2 BR
        {
            get { return _br; }
        }

        /// <summary>
        /// Gets or sets the top-left x co-ord of the rectangle
        /// </summary>
        public float X
        {
            get { return _tl.X; }
            set { _tl.X = value; _bl.X = value; _tr.X = _tl.X + _width; _br.X = _tl.X + _width; }
        }
        /// <summary>
        /// Gets or sets the top-left y co-ord of the rectangle
        /// </summary>
        public float Y
        {
            get { return _tl.Y; }
            set { _tl.Y = value; _tr.Y = value; _bl.Y = _tl.Y + _height; _br.Y = _tl.Y + _height; }
        }
        /// <summary>
        /// Gets or sets width of the rectangle
        /// </summary>
        public float Width
        {
            get { return _width; }
            set { _width = value; _tr.X = _tl.X + _width; _br.X = _tl.X + _width; }
        }
        /// <summary>
        /// Gets or sets the height of the rectangle
        /// </summary>
        public float Height
        {
            get { return _height; }
            set { _height = value; _bl.Y = _tl.Y + _height; _br.Y = _tl.Y + _height; }
        }

        /// <summary>
        /// Gets the top y of this rectangle
        /// </summary>
        public float Top
        {
            get { return _tl.Y; }
        }
        /// <summary>
        /// Gets the bottom y of this rectangle
        /// </summary>
        public float Bottom
        {
            get { return _br.Y; }
        }
        /// <summary>
        /// Gets the left x of this rectangle
        /// </summary>
        public float Left
        {
            get { return _tl.X; }
        }
        /// <summary>
        /// Gets the right x of this rectangle
        /// </summary>
        public float Right
        {
            get { return _br.X; }
        }

        public Vector2 Orgin
        {
            get { return (_tl + _br) / 2; }
        }
        public float Extents
        {
            get { return (float)Math.Sqrt((Width / 2) * (Width / 2) + (Height / 2) + (Height / 2)); }
        }

        /// <summary>
        /// Creates a new floating-point rectangle
        /// </summary>
        /// <param name="x">The x co-ord of the rectangle</param>
        /// <param name="y">The y co-ord of the rectangle</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        public RectangleF(float x, float y, float width, float height)
        {
            _tl = new Vector2(x, y);
            _tr = new Vector2(x + width, y);

            _bl = new Vector2(x, y + height);
            _br = new Vector2(x + width, y + height);

            _width = width;
            _height = height;
        }

        /// <summary>
        /// Checks if a triangle intersects this rectangle
        /// </summary>
        /// <param name="tri">the triangle to check</param>
        /// <returns>True if tri intersects the rectangle</returns>
        public bool Intersects(Triangle tri)
        {
            float x0 = tri.P1.X;
            float y0 = tri.P1.Y;
            float x1 = tri.P2.X;
            float y1 = tri.P2.Y;
            float x2 = tri.P3.X;
            float y2 = tri.P3.Y;


            return LineCheck(x0, y0, x1, y1)
                  || LineCheck(x1, y1, x2, y2)
                  || LineCheck(x2, y2, x0, y0);
        }

        /// <summary>
        /// Checks for an intersection between this rectangle and a line
        /// </summary>
        /// <param name="line">The line to check</param>
        /// <returns></returns>
        public bool Intersects(LineSegment line)
        {
            return line.Intersects(this);
        }

        /// <summary>
        /// Checks if this rectangle intersects another
        /// </summary>
        /// <param name="other">The rectangle to check against</param>
        /// <returns></returns>
        public bool Intersects(RectangleF other)
        {
            return
                Right >= other.Left && Bottom >= other.Top
                && X <= other.Right && Top <= other.Bottom;
        }

        /// <summary>
        /// Checks if this rectangle intersects another
        /// </summary>
        /// <param name="other">The rectangle to check against</param>
        /// <returns></returns>
        public bool Intersects(RectangleF other, Vector2 offset)
        {
            return
                Right - offset.X >= other.Left && Bottom - offset.Y >= other.Top
                && X <= other.Right - offset.X && Top - offset.Y <= other.Bottom;
        }

        /// <summary>
        /// Gets all the intersections with a line
        /// </summary>
        /// <param name="line">The line to check</param>
        /// <returns></returns>
        public Vector2[] GetIntersect(LineSegment line)
        {
            return line.GetIntersect(this);
        }

        /// <summary>
        /// Checks if this rectangle contains the given point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is within this rectangle</returns>
        public bool Contains(Vector2 point)
        {
            return point.X >= Left & point.X <= Right & point.Y >= Top & point.Y <= Bottom;
        }

        /// <summary>
        /// Check if the line passes through the triangle
        /// </summary>
        /// <param name="x0">The first x co-ord</param>
        /// <param name="y0">The first y co-ord</param>
        /// <param name="x1">The second x co-ord</param>
        /// <param name="y1">The second y co-ord</param>
        /// <returns>True if the line intersects the rectangle</returns>
        private bool LineCheck(float x0, float y0, float x1, float y1)
        {
            float top_intersection;
            float bottom_intersection;

            float toptrianglepoint;
            float bottomtrianglepoint;

            // Calculate m and c for the equation for the line (y = mx+c)
            float m = (y1 - y0) / (x1 - x0);
            float c = y0 - (m * x0);

            // if the line is going up from right to left then the top intersect point is on the left
            if (m > 0)
            {
                top_intersection = (m * Left + c);
                bottom_intersection = (m * Right + c);
            }
            // otherwise it's on the right
            else
            {
                top_intersection = (m * Left + c);
                bottom_intersection = (m * Right + c);
            }

            // work out the top and bottom extents for the triangle
            if (y0 < y1)
            {
                toptrianglepoint = y0;
                bottomtrianglepoint = y1;
            }
            else
            {
                toptrianglepoint = y1;
                bottomtrianglepoint = y0;
            }

            float topoverlap;
            float botoverlap;

            // and calculate the overlap between those two bounds
            topoverlap = top_intersection > toptrianglepoint ? top_intersection : toptrianglepoint;
            botoverlap = bottom_intersection < bottomtrianglepoint ? bottom_intersection : bottomtrianglepoint;

            // (topoverlap<botoverlap) :
            // if the intersection isn't the right way up then we have no overlap

            // (!((botoverlap<t) || (topoverlap>b)) :
            // If the bottom overlap is higher than the top of the rectangle or the top overlap is
            // lower than the bottom of the rectangle we don't have intersection. So return the negative
            // of that. Much faster than checking each of the points is within the bounds of the rectangle.
            return (topoverlap < botoverlap) && (!((botoverlap < Top) || (topoverlap > Bottom)));

        }

        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public Vector2 GetIntersectionDepth(RectangleF rectB)
        {
            // Calculate half sizes.
            float halfWidthA = Width / 2.0f;
            float halfHeightA = Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(X + halfWidthA, Y + halfHeightA);
            Vector2 centerB = new Vector2(rectB.X + halfWidthB, rectB.Y + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public Vector2 GetIntersectionDepth(RectangleF rectB, Vector2 offset)
        {
            // Calculate half sizes.
            float halfWidthA = Width / 2.0f;
            float halfHeightA = Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(X + halfWidthA - offset.X, Y + halfHeightA - offset.Y);
            Vector2 centerB = new Vector2(rectB.X + halfWidthB, rectB.Y + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Gets the position of the center of the bottom edge of the rectangle.
        /// </summary>
        public Vector2 GetBottomCenter()
        {
            return new Vector2(X + Width / 2.0f, Bottom);
        }
    }
}

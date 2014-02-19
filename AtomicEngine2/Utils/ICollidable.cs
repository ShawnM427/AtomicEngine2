using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AtomicEngine2.Utils
{
    public interface ICollidable
    {
        Vector2 Orgin { get; }
        float Extents { get; }

        bool Intersects(RectangleF rect);

        bool Intersects(Triangle tri);

        bool Intersects(LineSegment line);

        bool Contains(Vector2 point);

        Vector2[] GetIntersect(LineSegment line);
    }
}

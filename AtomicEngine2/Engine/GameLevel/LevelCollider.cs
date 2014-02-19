using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AtomicEngine2.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace AtomicEngine2.Engine.GameLevel
{
    public class LevelCollider
    {
        List<RectangleF> _colliders = new List<RectangleF>();
        VertexBuffer _buffer;
        GraphicsDevice _graphics;

        public LevelCollider(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }

        public void AddCollider(RectangleF Collider)
        {
            _colliders.Add(Collider);
        }

        public bool Intersects(RectangleF check)
        {
            foreach (RectangleF rect in _colliders)
            {
                if (rect.Intersects(check))
                    return true;
            }
            return false;
        }

        public Vector2 GetIntersect(RectangleF check)
        {
            foreach (RectangleF rect in _colliders)
            {
                if (rect.Intersects(check))
                    return check.GetIntersectionDepth(rect);
            }
            return Vector2.Zero;
        }

        public Vector2 GetIntersect(RectangleF check, Vector2 offset)
        {
            foreach (RectangleF rect in _colliders)
            {
                if (rect.Intersects(check, offset))
                    return check.GetIntersectionDepth(rect, offset);
            }
            return Vector2.Zero;
        }

        public bool Contains(Vector2 point)
        {
            foreach (RectangleF rect in _colliders)
            {
                if (rect.Contains(point))
                    return true;
            }
            return false;
        }

        public void Finish()
        {
            _buffer = new VertexBuffer(_graphics, typeof(VertexPositionColor), _colliders.Count * 8, BufferUsage.WriteOnly);

            List<VertexPositionColor> temp = new List<VertexPositionColor>();

            foreach (RectangleF rect in _colliders)
            {
                temp.Add(new VertexPositionColor(new Vector3(rect.TL, 0), Color.Red));
                temp.Add(new VertexPositionColor(new Vector3(rect.TR, 0), Color.Red));

                temp.Add(new VertexPositionColor(new Vector3(rect.TR, 0), Color.Red));
                temp.Add(new VertexPositionColor(new Vector3(rect.BR, 0), Color.Red));

                temp.Add(new VertexPositionColor(new Vector3(rect.BR, 0), Color.Red));
                temp.Add(new VertexPositionColor(new Vector3(rect.BL, 0), Color.Red));

                temp.Add(new VertexPositionColor(new Vector3(rect.BL, 0), Color.Red));
                temp.Add(new VertexPositionColor(new Vector3(rect.TL, 0), Color.Red));
            }

            _buffer.SetData(temp.ToArray());
        }

        public void Render()
        {
            _graphics.SetVertexBuffer(_buffer);
            _graphics.DrawPrimitives(PrimitiveType.LineList, 0, _colliders.Count * 4);
        }
    }
}

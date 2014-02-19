using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomicEngine2.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtomicEngine2.Engine.LevelObjects
{
    public class LevelBlock : LevelObject
    {
        Color _color;
        int _texID;

        public LevelBlock(RectangleF bounds, Color color, int texIndex)
        {
            Bounds = bounds;

            X = bounds.X;
            Y = bounds.Y;
            MaxX = bounds.X + bounds.Width;
            MaxY = bounds.Y + bounds.Height;

            _color = color;
            _texID = texIndex;
        }

        public LevelBlock(float minX, float minY, float width, float height, Color color, int texIndex)
        {
            Bounds = new RectangleF(minX, minY, width, height);

            X = minX;
            Y = minY;
            MaxX = X + width;
            MaxY = Y + height;

            _color = color;
            _texID = texIndex;
        }

        public override void AddToGeometry(LevelGeometry geometry, TextureManager texManager)
        {
            VertexPositionColorTexture tl = new VertexPositionColorTexture(
                new Vector3(_pos, 0), _color, texManager.TL(_texID));
            VertexPositionColorTexture tr = new VertexPositionColorTexture(
                new Vector3(MaxX, Y, 0), _color, texManager.TR(_texID));
            VertexPositionColorTexture bl = new VertexPositionColorTexture(
                new Vector3(X, MaxY, 0), _color, texManager.BL(_texID));
            VertexPositionColorTexture br = new VertexPositionColorTexture(
                new Vector3(_max, 0), _color, texManager.BR(_texID));

            int tli = geometry.AddVertex(tl);
            int tri = geometry.AddVertex(tr);
            int bli = geometry.AddVertex(bl);
            int bri = geometry.AddVertex(br);

            geometry.AddTri(tli, bli, bri);
            geometry.AddTri(tli, bri, tri);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return point.X > X & point.X < MaxX & point.Y > Y & point.Y < MaxY;
        }
    }
}

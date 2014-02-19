using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomicEngine2.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtomicEngine2.Engine.LevelObjects
{
    /// <summary>
    /// Represents a triangle in the level
    /// </summary>
    public class LevelTri: LevelObject
    {
        Color _color;
        int _texID;
        Triangle _tri;
        TexMode _mode;

        public LevelTri(Triangle tri, Color color, int texIndex, TexMode mode = TexMode.TlBlBr)
        {
            Bounds = tri.Bounds;
            _tri = tri;
            _mode = mode;

            _color = color;
            _texID = texIndex;
        }

        public LevelTri(float x0, float y0, float x1, float y1, float x2, float y2, Color color, 
            int texIndex, TexMode mode = TexMode.TlBlBr)
        {
            _tri = new Triangle(x0, y0, x1, y1, x2, y2);
            Bounds = _tri.Bounds;
            _mode = mode;

            _color = color;
            _texID = texIndex;
        }

        public override void AddToGeometry(LevelGeometry geometry, TextureManager texManager)
        {
            VertexPositionColorTexture p1 = new VertexPositionColorTexture(
                new Vector3(_tri.P1, 0), _color, texManager.TL(_texID));
            VertexPositionColorTexture p2 = new VertexPositionColorTexture(
                new Vector3(_tri.P2, 0), _color, texManager.TR(_texID));
            VertexPositionColorTexture p3 = new VertexPositionColorTexture(
                new Vector3(_tri.P3, 0), _color, texManager.BL(_texID));

            int p1i = geometry.AddVertex(p1);
            int p2i = geometry.AddVertex(p2);
            int p3i = geometry.AddVertex(p3);

            geometry.AddTri(p1i, p2i, p3i);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return _tri.Contains(point);
        }
        
        public enum TexMode
        {
            TlBlBr = 0,
            TlTRBr = 1
        }
    }
}

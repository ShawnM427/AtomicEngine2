using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtomicEngine2.Engine.Render
{
    public struct TileVertex : IVertexType
    {
        Vector3 _position;
        Vector2 _texPos;
        Color _color;
        int _texID;

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public Vector2 TexPos
        {
            get { return _texPos; }
            set { _texPos = value; }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        //public int TexID
        //{
        //    get { return _texID; }
        //    set { _texID = value; }
        //}

        public TileVertex(Vector3 position, Vector2 texCoords, Color color, int texID)
        {
            _position = position;
            _texPos = texCoords;
            _color = color;
            _texID = texID;
        }

        public TileVertex(Vector3 position, Vector2 texCoords, int texID)
        {
            _position = position;
            _texPos = texCoords;
            _color = Color.White;
            _texID = texID;
        }

        static VertexDeclaration _declaration = new VertexDeclaration(new VertexElement[]{
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * (3), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * (3 + 2), VertexElementFormat.Vector4, VertexElementUsage.Color, 0)
            //,new VertexElement(sizeof(float) * (3 + 2 + 4), VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0)
        });

        public VertexDeclaration VertexDeclaration
        {
            get { return _declaration; }
        }
    }
}

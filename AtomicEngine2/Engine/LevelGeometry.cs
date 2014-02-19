using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace AtomicEngine2.Engine
{
    public class LevelGeometry
    {
        VertexBuffer _vBuffer;
        IndexBuffer _iBuffer;

        List<VertexPositionColorTexture> _tempVertices;
        List<int> _tempIndices;

        VertexPositionColorTexture[] _vertices;
        int _vCount;
        int[] _indices;
        int _iCount;

        GraphicsDevice _graphics;

        bool _canDraw = false;
        int _primitiveCount = 0;

        public Mode DrawMode = Mode.Buffered;

        public LevelGeometry(GraphicsDevice Graphics)
        {
            _tempVertices = new List<VertexPositionColorTexture>();
            _tempIndices = new List<int>();

            _graphics = Graphics;
        }

        public int AddVertex(VertexPositionColorTexture vert)
        {
            _tempVertices.Add(vert);
            return _tempVertices.Count - 1;
        }

        public void AddIndex(int index)
        {
            _tempIndices.Add(index);
        }

        public void AddTri(int i1, int i2, int i3)
        {
            _tempIndices.Add(i1);
            _tempIndices.Add(i2);
            _tempIndices.Add(i3);
        }

        public void End()
        {
            _vertices = _tempVertices.ToArray();
            _vCount = _vertices.Length;
            _indices = _tempIndices.ToArray();
            _iCount = _indices.Length;

            _vBuffer = new VertexBuffer(_graphics, typeof(VertexPositionColorTexture),
                _vCount, BufferUsage.WriteOnly);
            _vBuffer.SetData(_vertices, 0, _vCount);

            _iBuffer = new IndexBuffer(_graphics, IndexElementSize.ThirtyTwoBits,
                _iCount, BufferUsage.WriteOnly);
            _iBuffer.SetData(_indices, 0, _iCount);  

            _tempIndices.Clear();
            _tempVertices.Clear();

            _primitiveCount = _iCount / 3;
             
            _canDraw = true;
        }

        public void Render()
        {
            if (_canDraw)
            {
                switch (DrawMode)
                {
                    case Mode.Buffered:
                        _graphics.Indices = _iBuffer;
                        _graphics.SetVertexBuffer(_vBuffer);

                        _graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vCount, 0, _primitiveCount);
                        break;

                    case Mode.Array:
                        _graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>
                            (PrimitiveType.TriangleList, _vertices, 0, _vCount,
                            _indices, 0, _primitiveCount);
                        break;
                }
            }
            else
                throw new InvalidOperationException("End must be called before this can be rendered");
        }

        public enum Mode
        {
            Buffered = 0,
            Array = 1
        }
    }
}

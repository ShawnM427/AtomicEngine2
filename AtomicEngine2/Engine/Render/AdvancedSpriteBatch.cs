using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AtomicEngine2.Utils;

namespace AtomicEngine2.Engine.Render
{
    class AdvancedSpriteBatch
    {
        VertexPositionColorTexture[] _vertices;
        short[] _indices;
        int _vertexCount = 0;
        int _indexCount = 0;
        Texture2D _texture;
        GraphicsDevice _device;

        BasicEffect _effect;

        public AdvancedSpriteBatch(GraphicsDevice device)
        {
            _device = device;
            _vertices = new VertexPositionColorTexture[256];
            _indices = new short[_vertices.Length * 3 / 2];

            _effect = new BasicEffect(device);
            _effect.TextureEnabled = true;
            _effect.VertexColorEnabled = true;

            ResetMatrices(800, 480);
        }

        public void ResetMatrices(int width, int height)
        {
            _effect.World = Matrix.CreateTranslation(0,0,-1);
            _effect.View = Matrix.CreateOrthographicOffCenter(
                0, width, height, 9, 0.001F, 1000F);

            _effect.Projection = Matrix.Identity;
        }

        public void Draw(Texture2D texture, RectangleF dstRectangle, RectangleF srcRectangle, Color color)
        {
            //  if the texture changes, we flush all queued sprites.
            if (this._texture != null && this._texture != texture)
                this.Flush();
            this._texture = texture;

            //  ensure space for my vertices and indices.
            this.EnsureSpace(6, 4);

            //  add the new indices
            _indices[_indexCount++] = (short)(_vertexCount + 0);
            _indices[_indexCount++] = (short)(_vertexCount + 1);
            _indices[_indexCount++] = (short)(_vertexCount + 3);
            _indices[_indexCount++] = (short)(_vertexCount + 1);
            _indices[_indexCount++] = (short)(_vertexCount + 2);
            _indices[_indexCount++] = (short)(_vertexCount + 3);

            // add the new vertices
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Left, dstRectangle.Top, 0)
                , color, GetUV(srcRectangle.Left, srcRectangle.Top));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Right, dstRectangle.Top, 0)
                , color, GetUV(srcRectangle.Right, srcRectangle.Top));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Right, dstRectangle.Bottom, 0)
                , color, GetUV(srcRectangle.Right, srcRectangle.Bottom));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Left, dstRectangle.Bottom, 0)
                , color, GetUV(srcRectangle.Left, srcRectangle.Bottom));
        }

        Vector2 GetUV(float x, float y)
        {
            return new Vector2(x / (float)_texture.Width, y / (float)_texture.Height);
        }

        void EnsureSpace(int indexSpace, int vertexSpace)
        {
            if (_indexCount + indexSpace >= _indices.Length)
                Array.Resize(ref _indices, Math.Max(_indexCount + indexSpace, _indices.Length * 2));
            if (_vertexCount + vertexSpace >= _vertices.Length)
                Array.Resize(ref _vertices, Math.Max(_vertexCount + vertexSpace, _vertices.Length * 2));
        }

        public void Begin()
        {

        }

        public void Flush()
        {
            if (this._vertexCount > 0)
            {
                _effect.Texture = _texture;

                foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                        PrimitiveType.TriangleList, this._vertices, 0, this._vertexCount,
                        this._indices, 0, this._indexCount / 3);
                }

                this._vertexCount = 0;
                this._indexCount = 0;
            }
        }

        public void End()
        {
            Flush();
        }
    }
}

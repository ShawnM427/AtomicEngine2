using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AtomicEngine2.Utils;

namespace AtomicEngine2.Engine.Render
{
    public class AdvancedSpriteBatch
    {
        VertexPositionColorTexture[] _vertices;
        short[] _indices;
        int _vertexCount = 0;
        int _indexCount = 0;
        Texture2D _texture;
        GraphicsDevice _device;

        float _depth = 0.1F;

        BasicEffect _effect;

        Matrix _tempTranslation;
        Matrix _tempRotation;
        Matrix _tempScale;
        Matrix _tempShift;
        Matrix _tempFinal;

        Vector3 _mOrgin;
        Vector3 _mScale;
        Vector3 _mShift;
                
        public Matrix View
        {
            get { return _effect.View; }
            set { _effect.View = value; }
        }
        public Matrix Transform
        {
            get { return _effect.World; }
            set { _effect.World = value; }
        }

        /// <summary>
        /// Creates a new advanced sprite batch
        /// </summary>
        /// <param name="device">The graphics device to use</param>
        public AdvancedSpriteBatch(GraphicsDevice device)
        {
            _device = device;
            _vertices = new VertexPositionColorTexture[256];
            _indices = new short[_vertices.Length * 3 / 2];

            _effect = new BasicEffect(device);
            _effect.TextureEnabled = true;
            _effect.VertexColorEnabled = true;

            ResetMatrices(device.Viewport.Width, device.Viewport.Height);
        }

        /// <summary>
        /// Draws a sprite
        /// </summary>
        /// <param name="texture">The texture to draw</param>
        /// <param name="destination">The rectangle to draw to</param>
        /// <param name="color">The color to draw with</param>
        public void Draw(Texture2D texture, RectangleF destination, Color color)
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
                new Vector3(destination.Left, destination.Top, 0)
                , color, new Vector2(0, 0));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(destination.Right, destination.Top, 0)
                , color, new Vector2(1, 0));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(destination.Right, destination.Bottom, 0)
                , color, new Vector2(1, 1));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(destination.Left, destination.Bottom, 0)
                , color, new Vector2(0, 1));
        }

        #region SourcedDraws
        /// <summary>
        /// Draws a sprite
        /// </summary>
        /// <param name="texture">The texture to draw</param>
        /// <param name="destination">The rectangle to draw to</param>
        /// <param name="source">The source rectangle</param>
        /// <param name="color">The color to draw with</param>
        public void Draw(Texture2D texture, RectangleF destination, RectangleF source,
            Color color)
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
                new Vector3(destination.Left, destination.Top, _depth)
                , color, GetUV(source.Left, source.Top));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(destination.Right, destination.Top, _depth)
                , color, GetUV(source.Right, source.Top));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(destination.Right, destination.Bottom, _depth)
                , color, GetUV(source.Right, source.Bottom));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(destination.Left, destination.Bottom, _depth)
                , color, GetUV(source.Left, source.Bottom));

            _depth += 0.00001F;
        }

        /// <summary>
        /// Draws a sprite with a rotation around it's centre point
        /// </summary>
        /// <param name="texture">The texture to draw</param>
        /// <param name="destination">The destination rectangle</param>
        /// <param name="source">The source rectangle</param>
        /// <param name="color">The color to draw with</param>
        /// <param name="rotation">The angle in <b>radians</b> to rotate</param>
        public void Draw(Texture2D texture, RectangleF destination, RectangleF source, Color color, float rotation)
        {
            Vector2 rotationCentre = destination.Orgin;

            Draw(texture, destination, source, color, rotationCentre, rotation, Vector2.One);
        }

        /// <summary>
        /// Draws a sprite with a rotation around an orgin
        /// </summary>
        /// <param name="texture">The texture to draw</param>
        /// <param name="destination">The destination rectangle</param>
        /// <param name="source">The source rectangle</param>
        /// <param name="color">The color to draw with</param>
        /// <param name="rotation">The angle in <b>radians</b> to rotate</param>
        /// <param name="orgin">The orgin in world space relative to the Top Left corner of the sprite</param>
        public void Draw(Texture2D texture, RectangleF destination, RectangleF source,
            Color color, float rotation, Vector2 orgin)
        {
            Vector2 rotationCentre = destination.TL + orgin;

            Draw(texture, destination, source, color, rotationCentre, rotation, Vector2.One);
        }
        #endregion

        #region CoreDraw
        /// <summary>
        /// Draws a sprite with a rotation around a central point
        /// </summary>
        /// <param name="texture">The texture to draw</param>
        /// <param name="destination">The destination rectangle</param>
        /// <param name="source">The source rectangle</param>
        /// <param name="color">The color to draw with</param>
        /// <param name="rotationCentre">The point to rotate around</param>
        /// <param name="rotation">The angle in <b>radians</b> to rotate</param>
        public void Draw(Texture2D texture, RectangleF destination, RectangleF source,
            Color color, Vector2 rotationCentre, float rotation, Vector2 scale)
        {
            _mOrgin = new Vector3(-rotationCentre, 0);
            _mScale = new Vector3(scale, 1);
            _mShift = new Vector3(destination.TL, 0);

            Matrix.CreateTranslation(ref _mOrgin, out _tempTranslation);
            Matrix.CreateScale(ref _mScale, out _tempScale);
            Matrix.CreateRotationZ(rotation, out _tempRotation);
            Matrix.CreateTranslation(ref _mShift, out _tempShift);

            _tempFinal = _tempTranslation * _tempScale * _tempRotation * _tempShift;

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
                new Vector3(destination.Left, destination.Top, _depth)
                , color, GetUV(source.Left, source.Top));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(destination.Right, destination.Top, _depth)
                , color, GetUV(source.Right, source.Top));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(destination.Right, destination.Bottom, _depth)
                , color, GetUV(source.Right, source.Bottom));
            _vertices[_vertexCount++] = new VertexPositionColorTexture(
                new Vector3(destination.Left, destination.Bottom, _depth)
                , color, GetUV(source.Left, source.Bottom));

            for (int i = _vertexCount - 4; i < _vertexCount; i++)
                _vertices[i].Position = Vector3.Transform(_vertices[i].Position, _tempFinal);

            _depth += 0.00001F;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the UV coord (0-1) for the given texture coord
        /// </summary>
        /// <param name="x">The x coord to get</param>
        /// <param name="y">The y coord to get</param>
        /// <returns>The UV coord from the texture coord</returns>
        private Vector2 GetUV(float x, float y)
        {
            return new Vector2(x / (float)_texture.Width, y / (float)_texture.Height);
        }

        /// <summary>
        /// Ensures that the buffers have enough size to hold all the data for this draw call
        /// </summary>
        /// <param name="indexSpace">The amount of indices to claim</param>
        /// <param name="vertexSpace">The amount of vertices to claim</param>
        private void EnsureSpace(int indexSpace, int vertexSpace)
        {
            if (_indexCount + indexSpace >= _indices.Length)
                Array.Resize(ref _indices, Math.Max(_indexCount + indexSpace, _indices.Length * 2));
            if (_vertexCount + vertexSpace >= _vertices.Length)
                Array.Resize(ref _vertices, Math.Max(_vertexCount + vertexSpace, _vertices.Length * 2));
        }

        /// <summary>
        /// Rebuilds the world, view, and projection matrices
        /// </summary>
        /// <param name="width">The width of the view</param>
        /// <param name="height">The height of the view</param>
        public void ResetMatrices(int width, int height)
        {
            _effect.World = Matrix.CreateTranslation(0, 0, -1);
            _effect.View = Matrix.CreateOrthographicOffCenter(
                0, width, height, 9, 0.001F, 1000F);

            _effect.Projection = Matrix.Identity;
        }
        #endregion

        #region Drawing Methods
        /// <summary>
        /// Begins drawing with this spritebatch
        /// </summary>
        public void Begin()
        {

        }

        /// <summary>
        /// Clears all the temporary data and draws to the screen
        /// </summary>
        public void Flush()
        {
            if (this._vertexCount > 0)
            {
                _depth = 0.1F;
                _effect.Texture = _texture;
                _device.SamplerStates[0] = SamplerState.PointWrap;

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

        /// <summary>
        /// Finishes drawing with this spritebatch and renders to the screen
        /// </summary>
        public void End()
        {
            Flush();
        }
        #endregion
    }
}

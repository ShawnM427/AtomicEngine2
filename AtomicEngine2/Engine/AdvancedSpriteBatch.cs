using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AtomicEngine2.Utils;

namespace AtomicEngine2.Engine
{
    class AdvancedSpriteBatch
    {
        VertexPositionColorTexture[] vertices;
        short[] indices;
        int vertexCount = 0;
        int indexCount = 0;
        Texture2D texture;
        GraphicsDevice device;

        //  these should really be properties
        public BasicEffect Effect;

        public AdvancedSpriteBatch(GraphicsDevice device)
        {
            this.device = device;
            this.vertices = new VertexPositionColorTexture[256];
            this.indices = new short[vertices.Length * 3 / 2];
            this.Effect = new BasicEffect(device);

            ResetMatrices(800, 480);
        }

        public void ResetMatrices(int width, int height)
        {
            Effect.World = Matrix.CreateTranslation(0,0,-1);
            Effect.View = Matrix.CreateOrthographic(width, height, 0.1F, 100);
            Effect.Projection = Matrix.Identity;
        }

        public void Draw(Texture2D texture, RectangleF srcRectangle, RectangleF dstRectangle, Color color)
        {
            //  if the texture changes, we flush all queued sprites.
            if (this.texture != null && this.texture != texture)
                this.Flush();
            this.texture = texture;

            //  ensure space for my vertices and indices.
            this.EnsureSpace(6, 4);

            //  add the new indices
            indices[indexCount++] = (short)(vertexCount + 0);
            indices[indexCount++] = (short)(vertexCount + 1);
            indices[indexCount++] = (short)(vertexCount + 3);
            indices[indexCount++] = (short)(vertexCount + 1);
            indices[indexCount++] = (short)(vertexCount + 2);
            indices[indexCount++] = (short)(vertexCount + 3);

            // add the new vertices
            vertices[vertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Left, dstRectangle.Top, 0)
                , color, GetUV(srcRectangle.Left, srcRectangle.Top));
            vertices[vertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Right, dstRectangle.Top, 0)
                , color, GetUV(srcRectangle.Right, srcRectangle.Top));
            vertices[vertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Right, dstRectangle.Bottom, 0)
                , color, GetUV(srcRectangle.Right, srcRectangle.Bottom));
            vertices[vertexCount++] = new VertexPositionColorTexture(
                new Vector3(dstRectangle.Left, dstRectangle.Bottom, 0)
                , color, GetUV(srcRectangle.Left, srcRectangle.Bottom));
        }

        Vector2 GetUV(float x, float y)
        {
            return new Vector2(x / (float)texture.Width, y / (float)texture.Height);
        }

        void EnsureSpace(int indexSpace, int vertexSpace)
        {
            if (indexCount + indexSpace >= indices.Length)
                Array.Resize(ref indices, Math.Max(indexCount + indexSpace, indices.Length * 2));
            if (vertexCount + vertexSpace >= vertices.Length)
                Array.Resize(ref vertices, Math.Max(vertexCount + vertexSpace, vertices.Length * 2));
        }

        public void Begin()
        {

        }

        public void Flush()
        {
            if (this.vertexCount > 0)
            {
                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                        PrimitiveType.TriangleList, this.vertices, 0, this.vertexCount,
                        this.indices, 0, this.indexCount / 3);
                }

                this.vertexCount = 0;
                this.indexCount = 0;
            }
        }

        public void End()
        {
            Flush();
        }
    }
}

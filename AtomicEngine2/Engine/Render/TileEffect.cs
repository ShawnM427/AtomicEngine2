using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using AtomicEngine2.Properties;

namespace AtomicEngine2.Engine.Render
{
    public class TileEffect : Effect
    {
        public Matrix World
        {
            get { return Parameters["World"].GetValueMatrix();}
            set { Parameters["World"].SetValue(value); }
        }

        public Matrix View
        {
            get { return Parameters["View"].GetValueMatrix(); }
            set { Parameters["View"].SetValue(value); }
        }

        public Matrix Projection
        {
            get { return Parameters["Projection"].GetValueMatrix(); }
            set { Parameters["Projection"].SetValue(value); }
        }

        public Color AmbientColor
        {
            get { return Color.FromNonPremultiplied(Parameters["AmbientColor"].GetValueVector4()); }
            set { Parameters["AmbientColor"].SetValue(value.ToVector4()); }
        }

        public Texture2D Texture
        {
            get { return Parameters["AtlasSampler+Atlas"].GetValueTexture2D(); }
            set { Parameters["AtlasSampler+Atlas"].SetValue(value); }
        }

        public int XTexs
        {
            get { return Parameters["XTexs"].GetValueInt32(); }
            set { Parameters["XTexs"].SetValue(value); }
        }

        public int YTexs
        {
            get { return Parameters["YTexs"].GetValueInt32(); }
            set { Parameters["YTexs"].SetValue(value); }
        }

        public TileEffect(GraphicsDevice g)
            : base(g, Resources.TileShader)
        {
        }
    }
}

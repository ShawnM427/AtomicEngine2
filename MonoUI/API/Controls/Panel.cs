using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoUI.API.Controls
{
    public class Panel : Control
    {
        VertexPositionColor[] _cornerVerts = new VertexPositionColor[4];
        static short[] _indices = new short[] { 0, 1, 2, 3, 0 };
        BasicEffect _effect;
        Color _outlineColor = Color.Black;

        /// <summary>
        /// Gets or sets the background color for this panel
        /// </summary>
        public Color BackColor
        {
            get { return _clearColor; }
            set { _clearColor = value; }
        }
        /// <summary>
        /// Gets or sets the outline color for this panel
        /// </summary>
        public Color OutlineColor
        {
            get { return _outlineColor; }
            set
            {
                _outlineColor = value;
                BuildVerts();
            }
        }

        /// <summary>
        /// Creates a new panel
        /// </summary>
        /// <param name="graphics">The graphcis device to draw with</param>
        /// <param name="bounds">The bounds of the panel</param>
        /// <param name="parent">The parent control for this panel</param>
        public Panel(GraphicsDevice graphics, Rectangle bounds, Control parent = null)
            : base(bounds, graphics, parent)
        {
            _clearColor = Color.LightGray;
            _effect = new BasicEffect(graphics);
            _effect.VertexColorEnabled = true;
            BuildVerts();
            Invalidate();
        }

        /// <summary>
        /// Builds this panel's vertices
        /// </summary>
        protected void BuildVerts()
        {
            float pixelOffY = (1.0F - (2.0F / (_bounds.Height - 1.0F)));
            float pixelOffX = (1.0F - (2.0F / (_bounds.Width - 1.0F)));

            _cornerVerts[0] = new VertexPositionColor(new Vector3(-1, -pixelOffY, 0), _outlineColor); //left bottom
            _cornerVerts[1] = new VertexPositionColor(new Vector3(-1, 1, 0), _outlineColor); //left top
            _cornerVerts[2] = new VertexPositionColor(new Vector3(pixelOffX, 1, 0), _outlineColor); //right top
            _cornerVerts[3] = new VertexPositionColor(new Vector3(pixelOffX, -pixelOffY, 0), _outlineColor); //right bottom
        }

        /// <summary>
        /// Called when this panel invalidates
        /// </summary>
        protected override void _Invalidate()
        {
            foreach (EffectPass p in _effect.CurrentTechnique.Passes)
            {
                p.Apply();
                _graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                    _cornerVerts, 0, 4, _indices, 0, 4);
            }
        }
    }
}

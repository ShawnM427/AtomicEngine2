using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoUI.API.Controls
{
    public class TextBox : Control
    {
        StringBuilder _builder;
        SpriteFont _font;

        VertexPositionColor[] _cornerVerts = new VertexPositionColor[4];
        static short[] _indices = new short[] { 0, 1, 2, 3, 0 };
        BasicEffect _effect;
        Color _outlineColor = Color.Black;

        Vector2 _margine = new Vector2(5, 0);

        Color _textColor = Color.Black;

        /// <summary>
        /// Creates a new text box
        /// </summary>
        /// <param name="bounds">The bounds of this text box</param>
        /// <param name="graphics">The graphics device to use</param>
        /// <param name="handle">The handle to receive input from</param>
        /// <param name="parent">The parent control</param>
        public TextBox(Rectangle bounds, GraphicsDevice graphics, SpriteFont font, Control parent = null)
            : base(bounds, graphics, parent)
        {
            _builder = new StringBuilder();

            MonoTextInput.CharPressed += _CharEntered;

            _clearColor = Color.White;
            _font = font;
            
            _effect = new BasicEffect(graphics);
            _effect.VertexColorEnabled = true;
            _BuildVerts();

            Invalidate();
        }

        /// <summary>
        /// Builds this text box's vertices
        /// </summary>
        protected virtual void _BuildVerts()
        {
            float pixelOffY = (1.0F - (2.0F / (_bounds.Height - 1.0F)));
            float pixelOffX = (1.0F - (2.0F / (_bounds.Width - 1.0F)));

            _cornerVerts[0] = new VertexPositionColor(new Vector3(-1, -pixelOffY, 0), _outlineColor); //left bottom
            _cornerVerts[1] = new VertexPositionColor(new Vector3(-1, 1, 0), _outlineColor); //left top
            _cornerVerts[2] = new VertexPositionColor(new Vector3(pixelOffX, 1, 0), _outlineColor); //right top
            _cornerVerts[3] = new VertexPositionColor(new Vector3(pixelOffX, -pixelOffY, 0), _outlineColor); //right bottom
        }
        
        /// <summary>
        /// Called when a character is entered on the keyboard
        /// </summary>
        /// <param name="character">Character that has been entered</param>
        protected virtual void _CharEntered(CharPressedEventArgs e)
        {
            if (e.KeyChar == '\b')// backspace
            { 
                if (_builder.Length > 0)
                {
                    _builder.Remove(_builder.Length - 1, 1);
                }
            }
            else
            {
                _builder.Append(e.KeyChar);
            }

            Invalidate();
        }

        /// <summary>
        /// Called when this text box invalidates
        /// </summary>
        protected override void _Invalidate()
        {
            foreach (EffectPass p in _effect.CurrentTechnique.Passes)
            {
                p.Apply();
                _graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,
                    _cornerVerts, 0, 4, _indices, 0, 4);
            }

            try
            {
                _spriteBatch.DrawString(_font, _builder, _margine, _textColor);
            }
            catch (ArgumentException)
            {
                _builder.Clear();
            }

        }
    }
}

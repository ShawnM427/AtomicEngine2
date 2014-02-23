using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoUI.API.Controls
{
    /// <summary>
    /// Represents a simple text label
    /// </summary>
    public class Label : Control
    {
        string _text;
        SpriteFont _font;
        Color _color = Color.Black;

        /// <summary>
        /// Creates a new label
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="font">The sprite font to use</param>
        /// <param name="text">The text to draw</param>
        /// <param name="tl">The top-left corner of the label</param>
        /// <param name="parent">The parent control</param>
        public Label(GraphicsDevice graphics, SpriteFont font, string text, Point tl, Control parent = null) :
            base(new Rectangle(tl.X, tl.Y, (int)font.MeasureString(text).X + 1, (int)font.MeasureString(text).Y + 1), graphics, parent)
        {
            _text = text;
            _font = font;
            Invalidate();
        }

        /// <summary>
        /// Overrides the invalidate method
        /// </summary>
        protected override void _Invalidate()
        {
            _spriteBatch.DrawString(_font, _text, Vector2.Zero, _color);
        }
    }
}

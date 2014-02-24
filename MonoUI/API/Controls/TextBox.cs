using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Input;

namespace MonoUI.API.Controls
{
    /// <summary>
    /// Represents a box that can receive textual input
    /// </summary>
    public class TextBox : Control
    {
        string _text;
        string _wrappedText;
        SpriteFont _font;

        VertexPositionColor[] _cornerVerts = new VertexPositionColor[4];
        static short[] _indices = new short[] { 0, 1, 2, 3, 0 };
        BasicEffect _effect;

        Color _backColor = Color.White; //The background color for when not focused
        Color _outlineColor = Color.Black; //The outline color for when not focused

        Color _focusedOutlineColor = Color.Red; //The outline color for when focused
        Color _focusedBackColor = Color.White; //The background color for when focused

        Color _currOutlineColor = Color.Black; //The current outline color
        //Note: the current background color is just _clearColor

        Vector2 _margine = new Vector2(5, 0);

        Color _textColor = Color.Black;

        /// <summary>
        /// Gets or sets the text for this control
        /// </summary>
        public string Text
        {
            get { return _text.ToString(); ; }
            set
            {
                _text = value;
                WrapText();
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets the background color for this panel
        /// </summary>
        public Color BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                Invalidate();
            }
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
                _BuildVerts();
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets the outline color for when this text box is selected
        /// </summary>
        public Color FocusedOutlineColor
        {
            get { return _focusedOutlineColor; }
            set
            {
                _focusedOutlineColor = value;
                _BuildVerts();
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets the background color for when this text box is selected
        /// </summary>
        public Color FocusedBackColor
        {
            get { return _focusedBackColor; }
            set
            {
                _focusedBackColor = value;
                Invalidate();
            }
        }

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
            _text = "";
            _wrappedText = "";

            EventInput.CharPressed += _CharEntered;

            _clearColor = _backColor;
            _font = font;

            _font.LineSpacing = (int)(_font.MeasureString(" ").Y * 0.6F);
            
            _effect = new BasicEffect(graphics);
            _effect.VertexColorEnabled = true;
            _BuildVerts();

            FocusChanged += FocusedChange;

            Invalidate();
        }

        /// <summary>
        /// Builds this text box's vertices
        /// </summary>
        protected virtual void _BuildVerts()
        {
            float pixelOffY = (1.0F - (2.0F / (_bounds.Height - 1.0F)));
            float pixelOffX = (1.0F - (2.0F / (_bounds.Width - 1.0F)));

            _cornerVerts[0] = new VertexPositionColor(new Vector3(-1, -pixelOffY, 0), _currOutlineColor); //left bottom
            _cornerVerts[1] = new VertexPositionColor(new Vector3(-1, 1, 0), _currOutlineColor); //left top
            _cornerVerts[2] = new VertexPositionColor(new Vector3(pixelOffX, 1, 0), _currOutlineColor); //right top
            _cornerVerts[3] = new VertexPositionColor(new Vector3(pixelOffX, -pixelOffY, 0), _currOutlineColor); //right bottom
        }
        
        /// <summary>
        /// Called when a character is entered on the keyboard
        /// </summary>
        /// <param name="character">Character that has been entered</param>
        protected virtual void _CharEntered(CharPressedEventArgs e)
        {
            if (_focused)
            {
                if (e.KeyChar == '\b')// backspace
                {
                    if (_text.Length > 0)
                    {
                        _text = _text.Remove(_text.Length - 1, 1);
                    }
                }
                else
                {
                    if (e.KeyChar == '\r')
                        _text += '\n';
                    else
                        _text += e.KeyChar;
                }

                WrapText();
                Invalidate();
            }
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
                _spriteBatch.DrawString(_font, _wrappedText, _margine, _textColor);
            }
            catch (ArgumentException)
            {
                _text = "";
                _wrappedText = "";
            }

        }

        /// <summary>
        /// Generates the wrapped text for this text box
        /// </summary>
        private void WrapText()
        {
            string[] words = _text.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;

            float spaceWidth = _font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = _font.MeasureString(word);

                if (lineWidth + size.X < _bounds.Width - (_margine.X * 2))
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            _wrappedText = sb.ToString();
        }

        /// <summary>
        /// Called when this text box's focus changes
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The focus changed event args</param>
        private void FocusedChange(object sender, FocusChangedEventArgs e)
        {
            _currOutlineColor = e.Focused ? FocusedOutlineColor : OutlineColor;
            _clearColor = e.Focused ? FocusedBackColor : BackColor;

            _BuildVerts();
            Invalidate();
        }

        protected override void _Clicked(object sender, MouseButtonEventArgs e)
        {
            Focused = true;
        }

        protected override void _MousePressed(object sender, MouseButtonEventArgs e)
        {
            Focused = false;
        }
    }
}

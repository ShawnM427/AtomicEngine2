using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoUI.API
{
    /// <summary>
    /// Represents a UI element
    /// </summary>
    public abstract class Control
    {
        protected GraphicsDevice _graphics;
        protected SpriteBatch _spriteBatch;
        protected Rectangle _bounds;
        protected RenderTarget2D _renderTarget;
        protected Control _parent;
        protected Control[] _children = new Control[0];

        protected Color _clearColor = Color.Transparent;

        /// <summary>
        /// Gets the texture that represents this control
        /// </summary>
        public Texture2D Texture
        {
            get { return _renderTarget; }
        }
        /// <summary>
        /// Gets or sets the bounds of this control, relative to it's parent
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Creates a new control
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="graphics"></param>
        public Control(Rectangle bounds, GraphicsDevice graphics, Control parent = null)
        {
            _bounds = bounds;
            _graphics = graphics;
            _parent = parent;

            _spriteBatch = new SpriteBatch(_graphics);

            _renderTarget = new RenderTarget2D(graphics, _bounds.Width, _bounds.Height);

            if (_parent != null)
                _parent.AddControl(this);
        }

        /// <summary>
        /// Adds a control to this control
        /// </summary>
        /// <param name="control"></param>
        public void AddControl(Control control)
        {
            Array.Resize<Control>(ref _children, _children.Length + 1);
            _children[_children.Length - 1] = control;

            Invalidate();
        }

        /// <summary>
        /// Updates this control
        /// </summary>
        public void Update()
        {
        }

        /// <summary>
        /// Invalidates this control to redraw
        /// </summary>
        public void Invalidate()
        {
            _BeginInvalidate();
            _Invalidate();
            _EndInvalidate();
            
            if (_parent != null)
                _parent.Invalidate();
        }

        /// <summary>
        /// Called when this control needs to redraw
        /// </summary>
        protected virtual void _Invalidate()
        {
        }

        /// <summary>
        /// Called to begin the invalidation process
        /// </summary>
        protected virtual void _BeginInvalidate()
        {
            _graphics.SetRenderTarget(_renderTarget);
            _graphics.Clear(_clearColor);
            _spriteBatch.Begin();
        }
        
        /// <summary>
        /// Called when the invalidation process is complete
        /// </summary>
        protected virtual void _EndInvalidate()
        {
            foreach (Control child in _children)
                _spriteBatch.Draw(child.Texture, child.Bounds, Color.White);

            _spriteBatch.End();
            _graphics.SetRenderTarget(null);
        }
    }
}

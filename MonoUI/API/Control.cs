using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenTK.Input;

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
        protected Vector2 _clientOffset = Vector2.Zero;

        protected Color _clearColor = Color.Transparent;

        protected bool _focused;

        #region Events
        /// <summary>
        /// Invoked when this control's focus changes
        /// </summary>
        public FocusChangedEventHandler FocusChanged
        {
            get;
            set;
        }
        /// <summary>
        /// Invoked when a control's bounds has changed
        /// </summary>
        public BoundsChangedEventHandler BoundsChanged
        {
            get;
            set;
        }
        /// <summary>
        /// Invoked when a control is pressed
        /// </summary>
        public MouseButtonEventHandler MousePressed { get; set; }
        /// <summary>
        /// Invoked when a control is released
        /// </summary>
        public MouseButtonEventHandler MouseReleased { get; set; }
        #endregion

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
                if (BoundsChanged != null)
                    BoundsChanged.Invoke(this, new BoundsChangedEventArgs(_bounds, value));

                _bounds = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets whether this control has focus
        /// </summary>
        public bool Focused
        {
            get { return _focused; }
            set
            {
                _focused = value;

                if (FocusChanged != null)
                    FocusChanged.Invoke(this, new FocusChangedEventArgs(_focused));
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
            {
                _parent.AddControl(this);
                _clientOffset = new Vector2(parent.Bounds.X, parent.Bounds.Y);
            }

            EventInput.MouseDown += __MousePressed;
            EventInput.MouseUp += __MouseReleased;
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

        /// <summary>
        /// Called when the mouse is pressed anywhere in the window
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event ags to use</param>
        private void __MousePressed(object sender, MouseButtonEventArgs e)
        {
            if (_bounds.Contains(e.X - _clientOffset.X, e.Y - _clientOffset.Y))
            {
                if (MousePressed != null)
                    MousePressed.Invoke(this, e);

                _Clicked(sender, e);
            }
            else
            {
                _MousePressed(sender, e);
            }
        }

        /// <summary>
        /// Called when the mouse is released anywhere in the window
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event ags to use</param>
        private void __MouseReleased(object sender, MouseButtonEventArgs e)
        {
            if (_bounds.Contains(e.X - _clientOffset.X, e.Y - _clientOffset.Y) && MouseReleased != null)
            {
                if (MouseReleased != null)
                    MouseReleased.Invoke(this, e);
                _Released(sender, e);
            }
            else
            {
                _MouseReleased(sender, e);
            }
        }

        /// <summary>
        /// Called when the mouse is pressed in the window, and outside of this controls bounds
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event ags to use</param>
        protected virtual void _MousePressed(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Called when the mouse is released in the window, and outside of this controls bounds
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event ags to use</param>
        protected virtual void _MouseReleased(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Called when the mouse is pressed in this controls bounds
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event ags to use</param>
        protected virtual void _Clicked(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Called when the mouse is released int this controls bounds
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event ags to use</param>
        protected virtual void _Released(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

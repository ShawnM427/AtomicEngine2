using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtomicEngine2.Engine.Entities
{
    /// <summary>
    /// Represents a top-down entity
    /// </summary>
    public class TDEntity : IFocusable
    {
        protected Texture2D _texture;
        protected Vector2 _prevPosition;
        protected Vector2 _position;
        protected Rectangle _bounds;
        protected int _width;
        protected int _height;
        protected int _halfWidth;
        protected int _halfHeight;

        /// <summary>
        /// Gets or sets the position of this entity
        /// </summary>
        public virtual Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        /// <summary>
        /// Gets or sets the x coord of this entity
        /// </summary>
        public float X
        {
            get { return _position.X; }
            set { _position.X = value; }
        }
        /// <summary>
        /// Gets or sets the y coord of this entity
        /// </summary>
        public float Y
        {
            get { return _position.Y; }
            set { _position.Y = value; }
        }
        /// <summary>
        /// Gets the bounds of this entity
        /// </summary>
        public virtual Rectangle Bounds
        {
            get { return _bounds; }
        }

        /// <summary>
        /// Creates a new top down entity
        /// </summary>
        /// <param name="position">The position of this entity</param>
        /// <param name="width">The width of this entity</param>
        /// <param name="height">The height of this entity</param>
        public TDEntity(Vector2 position, int width, int height, Texture2D texture)
        {
            _position = position;
            _width = width;
            _height = height;

            _halfWidth = _width / 2;
            _halfHeight = _height / 2;

            SetBounds();

            _texture = texture;
        }

        /// <summary>
        /// Updates this entities bounds
        /// </summary>
        protected virtual void SetBounds()
        {
            _bounds = new Rectangle((int)_position.X - _halfWidth, (int)_position.Y - _halfHeight,
                _halfWidth, _halfHeight);
        }

        /// <summary>
        /// Updates the bounds without rebuilding it (saves garbage collection)
        /// </summary>
        protected virtual void UpdateBounds()
        {
            _bounds.X = (int)(_position.X - _halfWidth);
            _bounds.Y = (int)(_position.Y - _halfHeight);
        }

        /// <summary>
        /// Draws this entity to a sprite batch
        /// </summary>
        /// <param name="batch">The sprite batch to draw with</param>
        /// <param name="gameTime">The current game time</param>
        public virtual void Draw(SpriteBatch batch, GameTime gameTime)
        {
            batch.Draw(_texture, _bounds, Color.White);
        }

        /// <summary>
        /// Updates this entity
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public virtual void Update(GameTime gameTime)
        {
            UpdateBounds();
            _prevPosition = _position;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AtomicEngine2.Utils;
using Microsoft.Xna.Framework.Graphics;
using AtomicEngine2.Engine.GameLevel;

namespace AtomicEngine2.Engine.Entities
{
    public abstract class BipedalEntity
    {
        public const float PREFERED_UPS = 1000.0F / 60.0F;

        protected Vector2 _pos;
        protected Vector2 _prevPos;

        #region Speed Vars
        protected float _xSpeed = 0;
        protected float _ySpeed = 0;

        protected float _maxXSpeed;
        protected float _maxYSpeed;
        protected float _maxYAcc;

        protected float _reqX;
        protected float _reqY;

        protected float _xAcc;
        protected float _yAcc;

        protected float _groundedXFriction;
        protected float _airFriction;

        protected float _yAccSpeed;
        #endregion

        protected bool _isOnGround;

        #region Bounding Info
        protected RectangleF _bounds;

        protected RectangleF _verticalCheck;
        protected RectangleF _groundCheck;

        protected RectangleF _horizontalCheck;

        protected float _height;
        protected float _width;
        #endregion

        protected GraphicsDevice _graphics;

        protected abstract EntityController controller { get; }

        protected EntityState _state;

        /// <summary>
        /// Gets or sets the position of this entity
        /// </summary>
        public Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }
        /// <summary>
        /// Gets the bounds of this entity
        /// </summary>
        public RectangleF Bounds
        {
            get { return _bounds; }
        }

        /// <summary>
        /// A multiplier used to translate speeds to 60 frames per second
        /// </summary>
        protected float _speedMultiplier;
        
        /// <summary>
        /// Creates a new bipedal entity
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="pos">The position to create the entity at</param>
        /// <param name="maxXSpeed">The maximum x speed of the entity (terminal velocity)</param>
        /// <param name="maxYSpeed">The maximum y speed of the entity (terminal velocity)</param>
        /// <param name="height">The height of this entity</param>
        /// <param name="width">The width of this entity</param>
        public BipedalEntity(GraphicsDevice graphics, Vector2 pos, float maxXSpeed, float maxYSpeed, float height, float width)
        {
            _graphics = graphics;
            _pos = pos;
            _maxXSpeed = maxXSpeed;
            _maxYSpeed = maxYSpeed;
            
            _height = height;
            _width = width;

            _verticalCheck = new RectangleF(pos.X - width / 2, pos.Y, width, 1);
            _groundCheck = new RectangleF(pos.X - width / 2, pos.Y, _width, 1);

            _horizontalCheck = new RectangleF(pos.X - width / 2, pos.Y - height, width, height - 0.5F);
            _bounds = new RectangleF(_pos.X - _width / 2, _pos.Y - _height, _width, _height);
        }

        /// <summary>
        /// Updates this entity
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        /// <param name="collider">The level collider to check against</param>
        public virtual void Update(GameTime gameTime, LevelCollider collider)
        {
            _prevPos = _pos;

            _reqX = 0;
            _reqY = 0;

            _speedMultiplier = (float)gameTime.ElapsedGameTime.TotalMilliseconds / PREFERED_UPS;
                        
            ApplyPhysics();

            UpdateState();

            EntityState e = controller.Apply(_state);

            _yAcc = e.YAcc;

            _reqX += e.ReqX;
            _reqY += e.ReqY;

            _reqX *= _speedMultiplier;
            _reqY *= _speedMultiplier;
                   
            CheckCollisions(collider);

            _pos.X += _reqX;
            _pos.Y += _reqY;

            _bounds.X = _pos.X - _width / 2;
            _bounds.Y = _pos.Y - _height;
        }

        /// <summary>
        /// Updates the entity state
        /// </summary>
        private void UpdateState()
        {
            _state = new EntityState(_isOnGround, _pos, _reqX, _reqY, _yAcc);
        }

        /// <summary>
        /// Renders this instance
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        /// <param name="view">The view transformation to use</param>
        public abstract void Render(GameTime gameTime, Matrix view);

        /// <summary>
        /// Applies the physics to this entity
        /// </summary>
        protected virtual void ApplyPhysics()
        {
            if (!_isOnGround && _yAcc < _maxYAcc)
                _yAcc += _yAccSpeed;

            if (_isOnGround)
            {
                _xSpeed *= _groundedXFriction;
            }
            else
            {
                _xSpeed *= _airFriction;
            }

            //_yAcc = MathHelper.Clamp(_yAcc, -_maxYAcc, _maxYAcc);

            _xSpeed += _xAcc * _speedMultiplier;
            _ySpeed += _yAcc * _speedMultiplier;

            _xSpeed = MathHelper.Clamp(_xSpeed, -_maxXSpeed, _maxXSpeed);
            _ySpeed = MathHelper.Clamp(_ySpeed, -_maxYSpeed, _maxYSpeed);

            _reqX += _xSpeed;
            _reqY += _ySpeed;
        }

        /// <summary>
        /// Checks this entity for collisions
        /// </summary>
        /// <param name="collider">The collider to check against</param>
        protected virtual void CheckCollisions(LevelCollider collider)
        {
            _verticalCheck.X = _pos.X - _width / 2; _verticalCheck.Y = _pos.Y;
            _verticalCheck.Height = _reqY;

            _groundCheck.X = _pos.X - _width / 2; _groundCheck.Y = _pos.Y;
            
            _horizontalCheck.X = _pos.X - _width / 2 + _reqX;
            _horizontalCheck.Y = _pos.Y - _height;

            float verticalPenetration = collider.GetIntersect(_verticalCheck).Y;
            float horizontalPenetration = collider.GetIntersect(_horizontalCheck).X;
            
            if (verticalPenetration != 0)
            {
                _reqY += verticalPenetration;
                _ySpeed = 0;
                _yAcc = 0;
                _isOnGround = true;
            }
            else if (collider.GetIntersect(_groundCheck) == Vector2.Zero)
            {
                _isOnGround = false;
            }

            if (horizontalPenetration != 0)
            {
                _reqX += horizontalPenetration;
                _xSpeed = 0;
                _xAcc = 0;
            }
        }
    }
}

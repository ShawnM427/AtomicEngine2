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

        public Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public RectangleF Bounds
        {
            get { return _bounds; }
        }
        
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

        public virtual void Update(GameTime gameTime, LevelCollider collider)
        {
            _reqX = 0;
            _reqY = 0;
            
            ApplyPhysics();

            UpdateState();

            EntityState e = controller.Apply(_state);

            _reqX += e.ReqX;
            _reqY += e.ReqY;
                                                            
            CheckCollisions(collider);

            _pos.X += _reqX;
            _pos.Y += _reqY;

            _bounds.X = _pos.X - _width / 2;
            _bounds.Y = _pos.Y - _height;

            _prevPos = _pos;
        }

        private void UpdateState()
        {
            _state = new EntityState(_isOnGround, _pos, _reqX, _reqY);
        }

        public abstract void Render(GameTime gameTime, Matrix view);

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

            _xSpeed += _xAcc;
            _ySpeed += _yAcc;

            _xSpeed = MathHelper.Clamp(_xSpeed, -_maxXSpeed, _maxXSpeed);
            _ySpeed = MathHelper.Clamp(_ySpeed, -_maxYSpeed, _maxYSpeed);

            _reqX += _xSpeed;
            _reqY += _ySpeed;
        }

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

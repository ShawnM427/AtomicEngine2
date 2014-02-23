using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtomicEngine2.Engine.Input;
using Microsoft.Xna.Framework.Input;

namespace AtomicEngine2.Engine.Entities
{
    /// <summary>
    /// Represents a top-down player that has 8 degrees of motion
    /// </summary>
    class TDPlayer_8 : TDEntity
    {
        #region Constants
        /// <summary>
        /// The width of the player
        /// </summary>
        const int WIDTH = 64;
        /// <summary>
        /// The height of the player
        /// </summary>
        const int HEIGHT = 64;
        /// <summary>
        /// Speed, in pixels per second
        /// </summary>
        const float SPEED = 64;

        #region Key Constants
        /// <summary>
        /// The key to press to move upwards
        /// </summary>
        const Keys UP = Keys.W;
        /// <summary>
        /// The key to press to move downwards
        /// </summary>
        const Keys DOWN = Keys.S;
        /// <summary>
        /// The key to press to move left
        /// </summary>
        const Keys LEFT = Keys.A;
        /// <summary>
        /// The key to press to move right
        /// </summary>
        const Keys RIGHT = Keys.D;
        #endregion

        #region Anim Constants
        /// <summary>
        /// The target time per animation frame, in milliseconds
        /// </summary>
        const double FRAMETIME = 1000 / 8;

        const int XTEXS = 16;
        const int YTEXS = 16;

        const byte ANIM_STAND_EAST = 0;
        const byte ANIM_STAND_NORTHEAST = 1;
        const byte ANIM_STAND_NORTH = 2;
        const byte ANIM_STAND_NORTHWEST = 3;
        const byte ANIM_STAND_WEST = 4;
        const byte ANIM_STAND_SOUTHWEST = 5;
        const byte ANIM_STAND_SOUTH = 6;
        const byte ANIM_STAND_SOUTHEAST = 7;

        const byte ANIM_WALK_EAST = 8;
        const byte ANIM_WALK_NORTHEAST = 9;
        const byte ANIM_WALK_NORTH = 10;
        const byte ANIM_WALK_NORTHWEST = 11;
        const byte ANIM_WALK_WEST = 12;
        const byte ANIM_WALK_SOUTHWEST = 13;
        const byte ANIM_WALK_SOUTH = 14;
        const byte ANIM_WALK_SOUTHEAST = 15;
        #endregion
        #endregion

        #region Animation Variables
        Rectangle[,] _sourceRects;

        int _animX = 0;
        int _animY = 0;

        double _elapsedTime = 0;
        #endregion

        #region Movement Variables
        float _adjustedSpeed;

        float _reqX;
        float _reqY;

        byte _direction;
        #endregion

        /// <summary>
        /// Creates a new top-down player with 8 degrees of movement
        /// </summary>
        /// <param name="position">The position to start from</param>
        /// <param name="tex">The player's texture atlas</param>
        public TDPlayer_8(Vector2 position, Texture2D tex) : base(position, WIDTH, HEIGHT, tex)
        {
            BuildSources();
        }

        /// <summary>
        /// Builds the source rectangles for the animation system
        /// </summary>
        private void BuildSources()
        {
            int sourceWidth = _texture.Width / XTEXS;
            int sourceHeight = _texture.Height / YTEXS;

            _sourceRects = new Rectangle[XTEXS,YTEXS];

            for (int y = 0; y < YTEXS; y++)
            {
                for (int x = 0; x < XTEXS; x++)
                {
                    _sourceRects[x, y] = new Rectangle(x * sourceWidth, y * sourceHeight, sourceWidth, sourceHeight);
                }
            }
        }

        /// <summary>
        /// Updates this player
        /// </summary>
        /// <param name="gameTime">The cuurent game time</param>
        public override void Update(GameTime gameTime)
        {
            _adjustedSpeed = (float)(SPEED * gameTime.ElapsedGameTime.TotalSeconds);

            RequestSpeed();
            GetAnim();

            _position.X += _reqX;
            _position.Y += _reqY;

            base.Update(gameTime);
        }

        /// <summary>
        /// Handles all the requests for  movement change
        /// </summary>
        private void RequestSpeed()
        {
            _reqX = 0;
            _reqY = 0;

            _reqY -= KeyboardManager.IsKeyDown(UP) ? _adjustedSpeed : 0;
            _reqY += KeyboardManager.IsKeyDown(DOWN) ? _adjustedSpeed : 0;
            _reqX += KeyboardManager.IsKeyDown(RIGHT) ? _adjustedSpeed : 0;
            _reqX -= KeyboardManager.IsKeyDown(LEFT) ? _adjustedSpeed : 0;
        }

        /// <summary>
        /// Gets the naimimation index
        /// </summary>
        private void GetAnim()
        {
            if (_reqX != 0 || _reqY != 0)
            {
                if (_reqX < 0)
                {
                    _animY = 12;
                    _animY += _reqY > 0 ? 1 : 0;
                    _animY -= _reqY < 0 ? 1 : 0;
                }
                if (_reqX > 0)
                {
                    _animY = 8;
                    _animY += _reqY > 0 ? 7 : 0;
                    _animY += _reqY < 0 ? 1 : 0;
                }
                if (_reqX == 0)
                {
                    if (_reqY < 0)
                        _animY = 10;
                    if (_reqY > 0)
                        _animY = 14;
                }
                _direction = (byte)(_animY - 8);
            }
            else
            {
                _animY = _direction;
            }
        }

        /// <summary>
        /// Draws this player
        /// </summary>
        /// <param name="batch">The spriteBatch to use</param>
        /// <param name="gameTime">The current game time</param>
        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_elapsedTime > FRAMETIME)
            {
                _animX = _animX + 1 >= XTEXS ? 0 : _animX + 1;
                _elapsedTime = 0;
            }

            batch.Draw(_texture, _bounds, _sourceRects[_animX, _animY], Color.White);
        }
    }
}

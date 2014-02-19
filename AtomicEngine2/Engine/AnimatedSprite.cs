﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AtomicEngine2.Utils;

namespace AtomicEngine2.Engine
{
    public class AnimatedSprite
    {
        SpriteBatch _batch;
        int _xFrame;
        int _yFrame;
        Texture2D _texture;
        Rectangle _source;

        int _frameWidth;
        int _frameHeight;
        int _xFrameCount;
        int _yFrameCount;

        Color _modifier = Color.White;
        double _timePerFrame;
        double _elapsedTime;

        /// <summary>
        /// Gets or sets the y frame for this animation
        /// </summary>
        public int YFrame
        {
            get { return _yFrame; }
            set { _yFrame = value >= _yFrameCount ? _yFrameCount - 1 : value < 0 ? 0 : value; }
        }

        /// <summary>
        /// Creates a new animated sprite
        /// </summary>
        /// <param name="batch">The spritebatch to use for drawing</param>
        /// <param name="texture">The texture to refrence</param>
        /// <param name="xFrame">The number of frames along the x axis</param>
        /// <param name="yFrame">The number of frames along the y axis</param>
        /// <param name="targetFrameTime">The target amount of time per frame</param>
        public AnimatedSprite(SpriteBatch batch, Texture2D texture, int xFrame, int yFrame, TimeSpan targetFrameTime)
        {
            _batch = batch;
            _texture = texture;
            _xFrameCount = xFrame;
            _yFrameCount = yFrame;

            _frameWidth = texture.Width / xFrame;
            _frameHeight = texture.Height / yFrame;

            _source = new Rectangle(0, 0, _frameWidth, _frameHeight);
            _timePerFrame = targetFrameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Updates the source rectangle
        /// </summary>
        private void UpdateSource()
        {
            _source.X = _xFrame * _frameWidth;
            _source.Y = _yFrame * _frameHeight;
        }

        /// <summary>
        /// Draws this sprite
        /// </summary>
        /// <param name="destination">The destination rectangle</param>
        /// <param name="gameTime">The current game time</param>
        public void Draw(Rectangle destination, GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_elapsedTime > _timePerFrame)
            {
                _elapsedTime = 0;
                _xFrame = _xFrame + 1 >= _xFrameCount ? 0 : _xFrame + 1;
                UpdateSource();
            }

            _batch.Draw(_texture, destination, _source, _modifier);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using AtomicEngine2.Utils;
using AtomicEngine2.Engine.Render;

namespace AtomicEngine2.Engine.Entities
{
    public class Player : BipedalEntity
    {
        const float MAX_X_SPEED = 10.0F;
        const float MAX_Y_SPEED = 5F;
        const float HEIGHT = 48.0F;
        const float WIDTH = 32.0F;
        const float Y_ACC_SPEED = 0.01F;
        const float MAX_Y_ACC = 0.5F;

        const float LAYER_DEPTH = 3.0F;

        VertexBuffer _buffer;
        BasicEffect _effect;

        VertexPositionColorTexture[] _vertices;
        Texture2D _tex;

        RectangleF r2 = new RectangleF(10, 90, 20, 110);

        AnimatedSprite _sprite;

        /// <summary>
        /// The facing, true is right
        /// </summary>
        bool _facing = true;
        
        PlayerController _controller;
        protected override EntityController controller
        {
            get
            {
                return _controller;
            }
        }
        
        public Player(GraphicsDevice graphics, Vector2 position, ContentManager content) :
            base(graphics, position, MAX_X_SPEED, MAX_Y_SPEED, HEIGHT, WIDTH)
        {
            _tex = content.Load<Texture2D>("Enities\\Player\\char_stitched");

            _sprite = new AnimatedSprite(graphics, _tex, 16, 8, TimeSpan.FromSeconds(1.0 / 32.0));

            _yAccSpeed = Y_ACC_SPEED;
            _maxYAcc = MAX_Y_ACC;
            _groundedXFriction = 0.5F;
            _airFriction = 0.9F;

            //_buffer = new VertexBuffer(_graphics, typeof(VertexPositionColorTexture), 4, BufferUsage.WriteOnly);
            //_buffer.SetData(_vertices);

            _effect = new BasicEffect(graphics);
            _effect.VertexColorEnabled = true;
            _effect.TextureEnabled = true;
            _effect.Texture = _tex;

            _controller = new PlayerController();
        }
        
        public override void Render(GameTime gameTime, Matrix view, Matrix? transform)
        {
            GetSpriteState();
            _sprite.Draw(_bounds, gameTime, view, transform);
        }

        private void GetSpriteState()
        {
            float changeX =  _pos.X - _prevPos.X;
            float changeY = _pos.Y - _prevPos.Y;

            if (changeX == 0)
            {
                _sprite.YFrame = _facing ? 6 : 7;
            }
            else
            {
                _sprite.YFrame = changeX > 0 ? 0 : 1;
                _facing = changeX > 0;
            }

            if (changeY > 0)
            {
                _sprite.YFrame = _facing ? 4 : 5;
            }
            else if (changeY < 0)
            {

                _sprite.YFrame = _facing ? 2 : 3;
            }
        }
    }
}

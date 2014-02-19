using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using AtomicEngine2.Utils;

namespace AtomicEngine2.Engine.Entities
{
    public class Player : BipedalEntity
    {
        const float MAX_X_SPEED = 10.0F;
        const float MAX_Y_SPEED = 5F;
        const float HEIGHT = 48.0F;
        const float WIDTH = 32.0F;
        const float Y_ACC_SPEED = 0.01F;
        const float MAX_Y_ACC = 2.5F;

        const float LAYER_DEPTH = 3.0F;

        VertexBuffer _buffer;
        BasicEffect _effect;

        VertexPositionColorTexture[] _vertices;
        Texture2D _tex;

        RectangleF r2 = new RectangleF(10, 90, 20, 110);

        SpriteBatch _spriteBatch;

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
            _vertices = new VertexPositionColorTexture[]{
                new VertexPositionColorTexture(new Vector3(-WIDTH / 2,-HEIGHT,0), Color.White, new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(WIDTH / 2,-HEIGHT,0), Color.White, new Vector2(1,0)),
                new VertexPositionColorTexture(new Vector3(-WIDTH / 2,0,0), Color.White, new Vector2(0,1)),
                new VertexPositionColorTexture(new Vector3(WIDTH/2,0,0), Color.White, new Vector2(1,1)),
            };

            _tex = content.Load<Texture2D>("playerTex");
            _spriteBatch = new SpriteBatch(_graphics);

            _yAccSpeed = Y_ACC_SPEED;
            _maxYAcc = MAX_Y_ACC;
            _groundedXFriction = 0.5F;
            _airFriction = 0.9F;

            _buffer = new VertexBuffer(_graphics, typeof(VertexPositionColorTexture), 4, BufferUsage.WriteOnly);
            _buffer.SetData(_vertices);

            _effect = new BasicEffect(graphics);
            _effect.VertexColorEnabled = true;
            _effect.TextureEnabled = true;
            _effect.Texture = _tex;

            _controller = new PlayerController();
        }

        public override void Render(GameTime gameTime, Matrix view)
        {
            _effect.World = Matrix.CreateTranslation(new Vector3(_pos, -LAYER_DEPTH));
            _effect.View = view;

            _effect.CurrentTechnique.Passes[0].Apply();
            _graphics.SetVertexBuffer(_buffer);
            _graphics.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip,
                _vertices, 0, 2);
            
            Debug.WriteLine(_pos);
        }
    }
}

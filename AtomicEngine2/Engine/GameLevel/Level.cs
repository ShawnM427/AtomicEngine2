using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomicEngine2.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AtomicEngine2.Engine.Entities;

namespace AtomicEngine2.Engine.GameLevel
{
    public class Level : GameComponent
    {
        List<LevelObject> _objects;
        VertexBuffer _backBuffer;
        LevelCollider _collider;

        List<BipedalEntity> _bipedals;

        Player _player;

        #region Render Core Vars
        LevelGeometry _geometry;
        TextureManager _texManager;
        GraphicsDevice _graphics;
        SamplerState s;
        BasicEffect _effect;
        
        Matrix _backMatrix;
        Matrix _geometryMatrix;
        #endregion

        #region Window
        float _width = 800;
        float _height = 480;

        float _viewWidth = 800;
        float _viewHeight = 480;

        /// <summary>
        /// Gets or sets the width of the level
        /// </summary>
        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }
        /// <summary>
        /// Gets or sets the height of the level
        /// </summary>
        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }
                
        /// <summary>
        /// Gets or sets the background color for this level
        /// </summary>
        public Color ClearColor
        {
            get { return _color; }
            set { _color = value; }
        }
        Color _color = Color.White;
        /// <summary>
        /// Gets or sets the backdrop for this level
        /// </summary>
        public Texture2D BackDrop
        {
            get { return _backDrop; }
            set { _backDrop = value; BuildBackBuffer(); }
        }
        Texture2D _backDrop = null;
        bool _hasBackDrop = false;

        /// <summary>
        /// Gets this levels camera object
        /// </summary>
        public Camera2D Camera
        {
            get { return _camera; }
        }
        Camera2D _camera;
        #endregion

        /// <summary>
        /// Creates a new level
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="textureAtlas">The tecture atlas to use</param>
        /// <param name="width">The width of the level</param>
        /// <param name="height">The height of the level</param>
        public Level(Game game, TextureManager textureAtlas, float width, float height) : base(game)
        {
            _graphics = game.GraphicsDevice;
            _texManager = textureAtlas;
            _geometry = new LevelGeometry(_graphics);
            _width = width;
            _height = height;

            _viewWidth = _graphics.Viewport.Width;
            _viewHeight = _graphics.Viewport.Height;

            _collider = new LevelCollider(_graphics);

            _effect = new BasicEffect(_graphics);
            _effect.TextureEnabled = true;
            _effect.VertexColorEnabled = true;
            _effect.Texture = _texManager.Texture;

            _effect.World = Matrix.Identity;
            _effect.Projection = Matrix.Identity;

            _bipedals = new List<BipedalEntity>();

            _player = new Player(_graphics, new Vector2(100, 10), game.Content);

            s = SamplerState.PointWrap;

            _camera = new Camera2D(game);
            _camera.Focus = _player;

            _effect.View = Matrix.CreateOrthographicOffCenter(0, 800, 480, 0, 0, 100);

            game.Components.Add(this);
        }

        /// <summary>
        /// Begin building the level
        /// </summary>
        public void StartBuild()
        {
            _objects = new List<LevelObject>();
        }

        /// <summary>
        /// Checks the intersection between the level and a rectangle
        /// </summary>
        /// <param name="rect">The rectangle to check</param>
        /// <returns>The signed amount of intersect, or zero if none</returns>
        public Vector2 GetIntersect(RectangleF rect)
        {
            return _collider.GetIntersect(rect);
        }

        /// <summary>
        /// Adds a static object to the level
        /// </summary>
        /// <param name="obj">The object to add</param>
        public void AddStaticObject(LevelObject obj)
        {
            _objects.Add(obj);

            if (obj.GetType() == typeof(LevelBlock))
                _collider.AddCollider(obj.Bounds);
        }

        /// <summary>
        /// Adds a new entity to this level
        /// </summary>
        /// <param name="entity">The entity to add</param>
        public void AddEntity(BipedalEntity entity)
        {
            _bipedals.Add(entity);
        }

        /// <summary>
        /// Updates this level
        /// </summary>
        /// <param name="gameTime">The current gameTime</param>
        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime, _collider);

            foreach (BipedalEntity entity in _bipedals)
                entity.Update(gameTime, _collider);
        }

        /// <summary>
        /// Finished the build and finalizes the level
        /// </summary>
        public void EndBuild()
        {
            foreach (LevelObject obj in _objects)
            {
                obj.AddToGeometry(_geometry, _texManager);
            }

            _geometry.End();

            if (_hasBackDrop)
                BuildBackBuffer();

            _geometryMatrix = Matrix.CreateTranslation(0, 0, -1);

            _collider.Finish();
        }

        /// <summary>
        /// Build the buffer for the backdrop
        /// </summary>
        private void BuildBackBuffer()
        {
            if (!_hasBackDrop)
            {
                _backBuffer = new VertexBuffer(_graphics, typeof(VertexPositionColorTexture), 4, BufferUsage.WriteOnly);
                _backBuffer.SetData(new VertexPositionColorTexture[] 
                    { 
                    new VertexPositionColorTexture(new Vector3(0, 0, -1), Color.White, Vector2.Zero),
                    new VertexPositionColorTexture(new Vector3(_width, 0, -1), Color.White, new Vector2(1,0)),
                    new VertexPositionColorTexture(new Vector3(0, _height, -1), Color.White, new Vector2(0,1)),
                    new VertexPositionColorTexture(new Vector3(_width, _height, -1), Color.White, new Vector2(1,1))
                    }
                );
                _hasBackDrop = true;
            }

            _backMatrix = Matrix.CreateTranslation(0, 0, -0.5F);
        }
        
        /// <summary>
        /// Renders this level
        /// </summary>
        public void Render(GameTime gameTime)
        {
            _graphics.Clear(_color);
            _graphics.RasterizerState = RasterizerState.CullNone;

            if (_backDrop != null)
            {
                _effect.Texture = _backDrop;
                _effect.World = _camera.Transform;
                _effect.CurrentTechnique.Passes[0].Apply();
                _graphics.SetVertexBuffer(_backBuffer);
                _graphics.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }

            _graphics.SamplerStates[0] = s;
            _effect.Texture = _texManager.Texture;
            _effect.World = _camera.Transform;
            _effect.CurrentTechnique.Passes[0].Apply();
            _geometry.Render();

            foreach (BipedalEntity entity in _bipedals)
                entity.Render(gameTime, _effect.View, _camera.Transform);

            _effect.TextureEnabled = false;
            _effect.CurrentTechnique.Passes[0].Apply();
            _collider.Render();
            _effect.TextureEnabled = true;
        }
    }
}

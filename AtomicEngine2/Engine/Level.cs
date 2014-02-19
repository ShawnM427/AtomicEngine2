using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomicEngine2.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AtomicEngine2.Engine.LevelObjects;

namespace AtomicEngine2.Engine
{
    public class Level
    {
        List<LevelObject> _objects;
        VertexBuffer _backBuffer;
        LevelCollider _collider;

        List<BipedalEntity> _bipedals;

        #region Render Core Vars
        LevelGeometry _geometry;
        TextureManager _texManager;
        GraphicsDevice _graphics;
        SamplerState s;
        BasicEffect _effect;

        float _nearClip = 0.01F;
        float _farClip = 100.1F;

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
        /// Gets or sets the width of the view
        /// </summary>
        public float ViewWidth
        {
            get { return _viewWidth; }
            set
            {
                _viewWidth = value <= _width ? value : _width;
                CalcFrameBounds();
            }
        }
        /// <summary>
        /// Gets or sets the height of the view
        /// </summary>
        public float ViewHeight
        {
            get { return _viewHeight; }
            set
            {
                _viewHeight = value <= _height ? value : _height;
                CalcFrameBounds();
            }
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
        /// Gets or sets the camera position, ie. the centre of the frame
        /// </summary>
        public Vector2 CameraPos
        {
            get { return _cameraPos; }
            set { _cameraPos = value; CalcFrameBounds(); }
        }
        Vector2 _cameraPos;
        #endregion

        /// <summary>
        /// Creates a new level
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="textureAtlas">The tecture atlas to use</param>
        /// <param name="width">The width of the level</param>
        /// <param name="height">The height of the level</param>
        public Level(GraphicsDevice graphics, TextureManager textureAtlas, float width, float height)
        {
            _texManager = textureAtlas;
            _geometry = new LevelGeometry(graphics);
            _graphics = graphics;
            _width = width;
            _height = height;

            _collider = new LevelCollider(graphics);

            _effect = new BasicEffect(graphics);
            _effect.TextureEnabled = true;
            _effect.VertexColorEnabled = true;
            _effect.Texture = _texManager.Texture;

            _effect.World = Matrix.Identity;
            _effect.Projection = Matrix.Identity;
            CalcFrameBounds();

            _bipedals = new List<BipedalEntity>();

            s = SamplerState.PointWrap;
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

        public void AddEntity(BipedalEntity entity)
        {
            _bipedals.Add(entity);
        }

        /// <summary>
        /// Updates this level
        /// </summary>
        /// <param name="gameTime">The current gameTime</param>
        public void Update(GameTime gameTime)
        {
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
                    new VertexPositionColorTexture(new Vector3(0,0, 0), Color.White, Vector2.Zero),
                    new VertexPositionColorTexture(new Vector3(_width, 0, 0), Color.White, new Vector2(1,0)),
                    new VertexPositionColorTexture(new Vector3(0, _height, 0), Color.White, new Vector2(0,1)),
                    new VertexPositionColorTexture(new Vector3(_width, _height, 0), Color.White, new Vector2(1,1))
                    }
                );
                _hasBackDrop = true;
            }

            _backMatrix = Matrix.CreateTranslation(0, 0, -0.5F);
        }

        /// <summary>
        /// Recalculate the bounds of the frame... kinda slow ATM
        /// </summary>
        private void CalcFrameBounds()
        {
            float left = _cameraPos.X - _viewWidth / 2;
            left = left < 0 ? 0 : left;

            float top = _cameraPos.Y - _viewHeight / 2;
            top = top < 0 ? 0 : top;

            float right = left + _viewWidth;
            if (right > _width)
            {
                right = _width;
                left = _width - _viewWidth;
            }
            float bottom = top + _viewHeight;
            if (bottom > _height)
            {
                bottom = _height;
                top = _height - _viewHeight;
            }

            _effect.View = Matrix.CreateOrthographicOffCenter(
                left, right, bottom, top, _nearClip, _farClip);
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
                _effect.World = _backMatrix;
                _effect.CurrentTechnique.Passes[0].Apply();
                _graphics.SetVertexBuffer(_backBuffer);
                _graphics.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }

            _graphics.SamplerStates[0] = s;
            _effect.Texture = _texManager.Texture;
            _effect.World = _geometryMatrix;
            _effect.CurrentTechnique.Passes[0].Apply();
            _geometry.Render();

            foreach (BipedalEntity entity in _bipedals)
                entity.Render(gameTime, _effect.View);

            _effect.TextureEnabled = false;
            _effect.CurrentTechnique.Passes[0].Apply();
            _collider.Render();
            _effect.TextureEnabled = true;
        }
    }
}

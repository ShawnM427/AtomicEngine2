using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AtomicEngine2.Engine.Render;
using AtomicEngine2.Engine.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace AtomicEngine2.Engine
{
    public class TileTester : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;
        TileMap _map;

        double _elapsedTime;
        int _frames;
        int _fps = 0;

        public TileTester()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;

            TargetElapsedTime = TimeSpan.FromMilliseconds(1);
            IsFixedTimeStep = false;

            if (!Directory.Exists("Maps"))
                Directory.CreateDirectory("Maps");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            BuildLevel();
        }

        /// <summary>
        /// Constructs a test level
        /// </summary>
        private void BuildLevel()
        {
            TileSet tileSet = new TileSet(Content.Load<Texture2D>("atlas"), 16, 16, "testing");
            tileSet.SetName(0, "grass");
            tileSet.SetName(1, "stone");
            tileSet.SetName(2, "dirt");
            tileSet.SetName(3, "sideGrass");
            tileSet.SetName(4, "planks");
            tileSet.SetName(5, "slab");
            tileSet.SetName(6, "bigBrick");

            tileSet.SetSolid("planks", true);

            _map = new TileMap(this, tileSet, 50, 50, 32, 32);
            _map.SetData(2, 1, 4);
            _map.SetData(5, 6, 9);

            Stream s = File.OpenWrite(@"Maps\test.ael");
            _map.WriteToStream(s);
            s.Close();

            Stream o = File.OpenRead(@"Maps\test.ael");
            _map = TileMap.ReadFromStream(this, o);
            o.Close();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardManager.Begin();

            base.Update(gameTime);

            Window.Title = "FPS: " + _fps;

            KeyboardManager.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            _frames++;

            if (_elapsedTime > 1000)
            {
                _elapsedTime = 0;
                _fps = _frames;
                _frames = 0;
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _map.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}

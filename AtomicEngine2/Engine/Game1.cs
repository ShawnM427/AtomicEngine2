#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using AtomicEngine2.Utils;
using AtomicEngine2.Engine.GameLevel;
using AtomicEngine2.Engine.Input;
using AtomicEngine2.Engine.Render;
using AtomicEngine2.Engine.Entities;
#endregion

namespace AtomicEngine2.Engine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player p;
        Vector2 playerSpawn = new Vector2(100, 10);

        Level _level;
        RectangleF r2;

        AnimatedSprite sprite;
        Rectangle destination;

        AdvancedSpriteBatch _aBatch;

        KeyboardState prevState;
        Texture2D tex;

        double _elapsedTime;
        int _frames;
        int _fps = 0;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromMilliseconds(1);
            IsFixedTimeStep = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _aBatch = new AdvancedSpriteBatch(GraphicsDevice);

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            tex = Content.Load<Texture2D>("Enities\\Player\\char_stitched");

            sprite = new AnimatedSprite(spriteBatch, 
               tex, 16, 8, 
                TimeSpan.FromSeconds(1.0 / 32.0));

            destination = new Rectangle(10, 10, 32, 48);

            BuildLevel();            
        }

        /// <summary>
        /// Constructs a test level
        /// </summary>
        private void BuildLevel()
        {
            TextureManager texManager = new TextureManager(Content.Load<Texture2D>("atlas2"),21, 15);

            _level = new Level(GraphicsDevice, texManager, 800, 480);
            _level.BackDrop = Content.Load<Texture2D>("backdrop");
            _level.ClearColor = Color.Red;

            //TileEffect t = new TileEffect(GraphicsDevice);

            _level.StartBuild();

            //Triangle t = new Triangle(0, 0, 100, 10, 0, 10);
            RectangleF r = new RectangleF(10, 100, 300, 90);
            r2 = new RectangleF(10, 90, 20, 110);

            _level.AddStaticObject(new LevelBlock(r, Color.White, 1));
            _level.AddStaticObject(new LevelBlock(r2, 
                r2.Intersects(r) ? Color.Green: Color.Red, 1));

            p = new Player(GraphicsDevice, playerSpawn, Content);

            _level.AddEntity(p);
            
            _level.EndBuild();
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

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.R))
                p.Position = playerSpawn;
            
            _level.Update(gameTime);

            Window.Title = string.Format("MonoTest | {0} | {1}",
                _fps,
                gameTime.IsRunningSlowly ? "SLOW" : "GOOD");

            KeyboardState curState = Keyboard.GetState();

            if (KeyboardManager.IsPressed(Keys.Up))
                sprite.YFrame++;

            if (KeyboardManager.IsPressed(Keys.Down))
                sprite.YFrame--;

            prevState = curState;


            base.Update(gameTime);

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

            _level.Render(gameTime);

            spriteBatch.Begin();
            sprite.Draw(destination, gameTime);
            spriteBatch.End();

            _aBatch.Draw(tex, new RectangleF(0, 0, 12.5F, 69.69F), new RectangleF(100, 100, 100, 100), Color.White);
            _aBatch.End();

            base.Draw(gameTime);
        }
    }
}

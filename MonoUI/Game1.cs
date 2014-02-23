#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using MonoUI.API.Controls;
using MonoUI.API;

using Key = OpenTK.Input.Key;
#endregion

namespace MonoUI
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        Label _label;
        Panel _panel;
        TextBox _textBox;

        public Game1()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);

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
            MonoTextInput.HookKeys(Window);
            MonoTextInput.KeyPressed += CharPressed;
            MonoTextInput.KeyUp += CharReleased;

            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFont font = Content.Load<SpriteFont>("font");

            _label = new Label(GraphicsDevice, font, "text", new Point(10, 10));
            _label.Invalidate();

            _panel = new Panel(GraphicsDevice, new Rectangle(10, 40, 320, 273));
            _panel.AddControl(_label);

            _textBox = new TextBox(new Rectangle(10, 40, 80, 32), GraphicsDevice, font, _panel);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        private void CharPressed(KeyDownEventArgs e)
        {
            if (e.Key == Key.Q)
                Window.Title += "|";
        }

        private void CharReleased(KeyUpEventArgs e)
        {
            if (e.Key == Key.Q)
                Window.Title = "lol";
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_label.Texture, _label.Bounds, Color.White);
            _spriteBatch.Draw(_panel.Texture, _panel.Bounds, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

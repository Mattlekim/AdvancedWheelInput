﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using vJoy.Wrapper;
using Riddlersoft.Core.Input;
using Riddlersoft.Core.Debug;
namespace AdvancedInput
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        JoystickState joy;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        AdvanceWheel Wheel;
        

        public Game1()
        {
           
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //set resolution
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 500;
            this.IsMouseVisible = true; //make sure the mouse is visible
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
            Components.Add(new SimpleMouse(this));
            Components.Add(new KeyboardAPI(this));
            
            float tet = 1f / 80f;
           // this.TargetElapsedTime = new System.TimeSpan(0,0,0,0, (int)(tet * 1000));
            //KeyboardAPI.Active
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
            Wheel = new AdvanceWheel(this);
            Wheel.LoadContent(Content);

            Components.Add(new IRacingTelemitry(this));
            IRacingTelemitry irt = Components[2] as IRacingTelemitry;
            irt.SetWheel(Wheel);

            Wheel.SetTelemitory();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         
            
       
            // TODO: Add your update logic here
            Wheel.Update(dt);

            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            Wheel.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

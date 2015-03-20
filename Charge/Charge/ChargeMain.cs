#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Charge
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ChargeMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Platform> platforms; // Contains all of the platforms
        List<Enemy> enemies; // Contains all of the enemies
        public static float moveSpeed = 2; //The horizontal run speed of the player.
        readonly int TIER1 = 400, TIER2 = 240, TIER3 = 80; // Different y-values for platforms to appear on
        Random rand;
        int time = 0; // The current frame that the game is in

        public ChargeMain()
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
            // TODO: Add your initialization logic here
            platforms = new List<Platform>();
            enemies = new List<Enemy>();
            Player player = new Player(new Rectangle(200, TIER2 - 80, 100, 80), null);
            Platform startingPlatform = new Platform(new Rectangle(200, TIER2, 800, 50), null);
            platforms.Add(startingPlatform);
            time = 0;
            rand = new Random();
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
            // TODO: use this.Content to load your game content here
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

            // TODO: Add your update logic here
            time++;
            // Logic for creating new platforms
            if(time >= 60 * 3)
            {
                int val = (int)(3*rand.NextDouble()); // Determines which tier the new platform should be on
                int tier;
                if (val == 0)
                    tier = TIER1;
                else if (val == 1)
                    tier = TIER2;
                else
                    tier = TIER3;
                Platform nextPlatform = new Platform(new Rectangle(800, tier, 200, 50), null);
                platforms.Add(nextPlatform);
                time = 0;
            }
            foreach(Platform p in platforms)
            {
                // delta time is 1 (temporary)
                p.Update(1);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            // Drawing all of the platforms
            foreach(Platform p in platforms)
            {
                // Temporary code for drawing platforms without sprites
                Texture2D rect = new Texture2D(graphics.GraphicsDevice, p.position.Width, p.position.Height);
                Color[] data = new Color[p.position.Width * p.position.Height];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Color.Black;
                rect.SetData(data);
                Vector2 coor = new Vector2(p.position.X, p.position.Y);
                spriteBatch.Draw(rect, coor, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

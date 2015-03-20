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

        Player player; //The player character
        List<Platform> platforms; //All platforms in game
        List<Enemy> enemies; //All enemies in game
        List<Projectile> bullets; //All bullets in game
        List<WorldEntity> walls; //All walls in the game
        List<WorldEntity> batteries; //All batteries in the game
        Barrier backBarrier; //The death barrier behind the player
        Barrier frontBarrier; //The death barrier in front of the player

        int score; //Player score
        float globalCooldown; //The cooldown on powerups
        Random rand; //Used for generating random variables

        private static float playerSpeed;
        float barrierSpeed;

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

            //Init all objects and lists
            initLevelObjects();

            rand = new Random();
            base.Initialize();
        }

        /// <summary>
        /// Initialize all objects in the level
        /// </summary>
        public void initLevelObjects()
        {
            player = new Player(new Rectangle(GameplayVars.PlayerStartX, LevelGenerationVars.Tier2Height - 50, 20, 40), null); //The player character
            platforms = new List<Platform>(); //All platforms in game
            enemies = new List<Enemy>(); //All enemies in game
            bullets = new List<Projectile>(); //All bullets in game
            walls = new List<WorldEntity>(); //All walls in the game
            batteries = new List<WorldEntity>(); //All batteries in the game
            Barrier backBarrier = new Barrier(new Rectangle(GameplayVars.BackBarrierStartX, -50, 20, 500), null); //The death barrier behind the player
            Barrier frontBarrier = new Barrier(new Rectangle(GameplayVars.BackBarrierStartX, -50, 20, 500), null); //The death barrier in front of the player
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

            foreach(Platform p in platforms)
            {
                p.Update(gameTime.ElapsedGameTime.Milliseconds);
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

        /// <summary>
        /// Handles reading in all of the input
        /// </summary>
        public void ProcessInput()
        {

        }

        /// <summary>
        /// Generates new level content
        /// </summary>
        public void HandleLevelGeneration()
        {

        }

        /// <summary>
        /// Checks all collisions in the game
        /// </summary>
        public void CheckCollisions()
        {

        }

        /// <summary>
        /// Updates all the world entities
        /// </summary>
        public void UpdateWorldEntities()
        {

        }

        /// <summary>
        /// Provides the speed of the player (and thus
        /// the opposite of the speed that the world should scroll).
        /// </summary>
        /// <returns>The player's speed</returns>
        public static float getPlayerSpeed()
        {
            return playerSpeed;
        }

    }
}

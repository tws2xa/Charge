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
        Background background;

        int score; //Player score
        float tempScore; //Keeps track of fractional score increases
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
            //Set window size
            graphics.PreferredBackBufferWidth = GameplayVars.WinWidth;
            graphics.PreferredBackBufferHeight = GameplayVars.WinHeight;

            //Init all objects and lists
            InitLevelObjects();

            //Initialize starting values for all numeric variables
            InitVars();

            //Initialize the other bits and bobbles
            rand = new Random();
            base.Initialize();
        }

        /// <summary>
        /// Sets all numeric variables to their starting values
        /// </summary>
        public void InitVars()
        {
            score = 0;
            globalCooldown = 0;
            playerSpeed = GameplayVars.PlayerStartSpeed;
            barrierSpeed = GameplayVars.BarrierStartSpeed;
        }

        /// <summary>
        /// Initialize all objects in the level
        /// </summary>
        public void InitLevelObjects()
        {
            player = new Player(new Rectangle(GameplayVars.PlayerStartX, LevelGenerationVars.Tier2Height - 50, 20, 40), null); //The player character
            platforms = new List<Platform>(); //All platforms in game
            enemies = new List<Enemy>(); //All enemies in game
            bullets = new List<Projectile>(); //All bullets in game
            walls = new List<WorldEntity>(); //All walls in the game
            batteries = new List<WorldEntity>(); //All batteries in the game
            backBarrier = new Barrier(new Rectangle(GameplayVars.BackBarrierStartX, -50, 20, 500), null); //The death barrier behind the player
            frontBarrier = new Barrier(new Rectangle(GameplayVars.BackBarrierStartX, -50, 20, 500), null); //The death barrier in front of the player
            background = new Background();
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
            float deltaTime = gameTime.ElapsedGameTime.Milliseconds;

            background.Update(deltaTime); //Update the background scroll
            
            ProcessInput(); //Process all input
            
            player.Update(deltaTime); //Update the player
            
            UpdateWorldEntities(deltaTime); //Update all entities in the world

            CheckCollisions(); //Check for any collisions

            HandleLevelGeneration(); //Generate more level content

            UpdateCooldown(deltaTime); //Update the global cooldown

            UpdateScore(deltaTime); //Update the player score
            
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

            //Draw background
            background.Draw(spriteBatch);

            //Draw platforms
            foreach (Platform platform in platforms)
            {
                platform.Draw(spriteBatch);
            }

            //Draw Walls
            foreach (WorldEntity wall in walls)
            {
                wall.Draw(spriteBatch);
            }

            //Draw Enemies
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

            //Draw Projectiles
            foreach (Projectile projectile in bullets)
            {
                projectile.Draw(spriteBatch);
            }

            //Draw Batteries
            foreach (WorldEntity battery in batteries)
            {
                battery.Draw(spriteBatch);
            }

            //Draw the player
            player.Draw(spriteBatch);
            
            //Draw Barriers
            frontBarrier.Draw(spriteBatch);
            backBarrier.Draw(spriteBatch);

            //Draw UI

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Handles reading in all of the input
        /// </summary>
        public void ProcessInput()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        }

        /// <summary>
        /// Update the global cooldown
        /// </summary>
        public void UpdateCooldown(float deltaTime)
        {
            globalCooldown = Math.Max(0, globalCooldown - deltaTime);
        }

        /// <summary>
        /// Update the score
        /// </summary>
        public void UpdateScore(float deltaTime)
        {
            tempScore += deltaTime * GameplayVars.TimeToScoreCoefficient;
            //Add to score if tempScore is at least 1
            if (tempScore > 1)
            {
                int addAmt = Convert.ToInt32(Math.Floor(tempScore));
                score += addAmt;
                tempScore -= addAmt;
            }
        }

        /// <summary>
        /// Updates all the world entities
        /// </summary>
        public void UpdateWorldEntities(float deltaTime)
        {
            //Update platforms
            foreach (Platform platform in platforms)
            {
                platform.Update(deltaTime);
            }

            //Update Batteries
            foreach (WorldEntity battery in batteries)
            {
                battery.Update(deltaTime);
            }

            //Update Barriers
            frontBarrier.Update(deltaTime);
            backBarrier.Update(deltaTime);

            //Update Enemies
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(deltaTime);
            }

            //Update Walls
            foreach (WorldEntity wall in walls)
            {
                wall.Update(deltaTime);
            }

            //Update Projectiles
            foreach (Projectile projectile in bullets)
            {
                projectile.Update(deltaTime);
            }

        }

        /// <summary>
        /// Checks all collisions in the game
        /// </summary>
        public void CheckCollisions()
        {

        }

        /// <summary>
        /// Generates new level content
        /// </summary>
        public void HandleLevelGeneration()
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

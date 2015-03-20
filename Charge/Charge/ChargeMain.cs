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
        Background background; //The scrolling backdrop

        int score; //Player score
        int curLevel; //The current level
        float tempScore; //Keeps track of fractional score increases
        float globalCooldown; //The cooldown on powerups
        Random rand; //Used for generating random variables

        private static float playerSpeed; //Current run speed
        float barrierSpeed; //Speed of barriers

        //The right most platforms in each tier
        Platform[] rightMostInTiers;

        //Textures
        Texture2D BackgroundTex;
        Texture2D BarrierTex;
        Texture2D BatteryTex;
        Texture2D EnemyTex;
        Texture2D PlatformCenterTex;
        Texture2D PlatformLeftTex;
        Texture2D PlatformRightTex;
        Texture2D PlayerTex;
        Texture2D WallTex;

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

            //Initialize all lists
            platforms = new List<Platform>(); //All platforms in game
            enemies = new List<Enemy>(); //All enemies in game
            bullets = new List<Projectile>(); //All bullets in game
            walls = new List<WorldEntity>(); //All walls in the game
            batteries = new List<WorldEntity>(); //All batteries in the game

            rightMostInTiers = new Platform[3];
            rightMostInTiers[0] = null;
            rightMostInTiers[1] = null;
            rightMostInTiers[2] = null;

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
            curLevel = 0;
            globalCooldown = 0;
            playerSpeed = GameplayVars.PlayerStartSpeed;
            barrierSpeed = GameplayVars.BarrierStartSpeed;
        }

        /// <summary>
        /// Creates and Positions all objects for the start of the level
        /// </summary>
        public void SetupInitialConfiguration()
        {
            //Clear all entity lists
            platforms.Clear();
            enemies.Clear();
            bullets.Clear();
            walls.Clear();
            batteries.Clear();

            //Create the initial objects
            player = new Player(new Rectangle(GameplayVars.PlayerStartX, LevelGenerationVars.Tier2Height - 110, 50, 100), PlayerTex); //The player character
            backBarrier = new Barrier(new Rectangle(GameplayVars.BackBarrierStartX, -50, 20, 500), BarrierTex); //The death barrier behind the player
            frontBarrier = new Barrier(new Rectangle(GameplayVars.BackBarrierStartX, -50, 20, 500), BarrierTex); //The death barrier in front of the player
            background = new Background(BackgroundTex);

            //Long barrier to catch player at the beginning of the game
            int startPlatWidth = GameplayVars.WinWidth - GameplayVars.PlayerStartX/3;
            startPlatWidth -= (startPlatWidth % LevelGenerationVars.SegmentWidth); //Make it evenly split into segments
            Platform startPlat = new Platform(new Rectangle(GameplayVars.PlayerStartX, LevelGenerationVars.Tier3Height, startPlatWidth, LevelGenerationVars.PlatformHeight),
                PlatformLeftTex, PlatformCenterTex, PlatformRightTex);

            //Spawn a random platform in each of the upper two tiers
            int tier1X = rand.Next(0, GameplayVars.WinWidth);
            int tier1Width = LevelGenerationVars.SegmentWidth * rand.Next(LevelGenerationVars.MinNumSegments, LevelGenerationVars.MaxNumSegments);

            int tier2X = rand.Next(0, GameplayVars.WinWidth);
            int tier2Width = LevelGenerationVars.SegmentWidth * rand.Next(LevelGenerationVars.MinNumSegments, LevelGenerationVars.MaxNumSegments);

            Platform tier1 = new Platform(new Rectangle(tier1X, LevelGenerationVars.Tier1Height, tier1Width, LevelGenerationVars.PlatformHeight),
                PlatformLeftTex, PlatformCenterTex, PlatformRightTex);
            Platform tier2 = new Platform(new Rectangle(tier2X, LevelGenerationVars.Tier2Height, tier2Width, LevelGenerationVars.PlatformHeight),
                PlatformLeftTex, PlatformCenterTex, PlatformRightTex);

            //Since they're currently the only platform in their tier,
            //Set the newly created platforms as the right most in each of their tiers.
            rightMostInTiers[0] = tier1;
            rightMostInTiers[1] = tier2;
            rightMostInTiers[2] = startPlat;

            //Add them to the platform list
            platforms.Add(tier1);
            platforms.Add(tier2);
            platforms.Add(startPlat);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load all needed game textures
            BackgroundTex = this.Content.Load<Texture2D>("Background.png");
            BarrierTex = this.Content.Load<Texture2D>("Barrier.png");
            BatteryTex = this.Content.Load<Texture2D>("Battery.png");
            EnemyTex = this.Content.Load<Texture2D>("Enemy.png");
            PlatformCenterTex = this.Content.Load<Texture2D>("PlatformCenterPiece.png");
            PlatformLeftTex = this.Content.Load<Texture2D>("PlatformLeftCap.png");
            PlatformRightTex = this.Content.Load<Texture2D>("PlatformRightCap.png");
            PlayerTex = this.Content.Load<Texture2D>("Player.png");
            WallTex = this.Content.Load<Texture2D>("Wall.png");

            //Init all objects and lists
            SetupInitialConfiguration();
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
            //Check for exit button press
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Delta time in seconds
            float deltaTime = (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

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
            for (int i = 0; i < platforms.Count; i++)
            {
                Platform entity = platforms[i];
                entity.Update(deltaTime);
                //Check if it should be deleted
                if (entity.destroyMe)
                {
                    platforms.Remove(entity);
                    
                    //Check if it was the right most platform in a tier
                    //If so, set the right most platform to null
                    for (int j = 0; j < rightMostInTiers.Length; j++)
                    {
                        if (entity == rightMostInTiers[j]) rightMostInTiers[j] = null;
                    }

                    entity = null;
                    i--;
                }
            }

            //Update Batteries
            for (int i = 0; i < batteries.Count; i++)
            {
                WorldEntity entity = batteries[i];
                entity.Update(deltaTime);

                //Check if it should be deleted
                if (entity.destroyMe)
                {
                    batteries.Remove(entity);
                    entity = null;
                    i--;
                }
            }

            //Update Barriers
            frontBarrier.Update(deltaTime);
            backBarrier.Update(deltaTime);

            //Update Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy entity = enemies[i];
                entity.Update(deltaTime);

                //Check if it should be deleted
                if (entity.destroyMe)
                {
                    enemies.Remove(entity);
                    entity = null;
                    i--;
                }
            }

            //Update Walls
            for (int i = 0; i < walls.Count; i++)
            {
                WorldEntity entity = walls[i];
                entity.Update(deltaTime);

                //Check if it should be deleted
                if (entity.destroyMe)
                {
                    walls.Remove(entity);
                    entity = null;
                    i--;
                }
            }

            //Update Projectiles
            for (int i = 0; i < bullets.Count; i++)
            {
                Projectile entity = bullets[i];
                entity.Update(deltaTime);

                //Check if it should be deleted
                if (entity.destroyMe)
                {
                    bullets.Remove(entity);
                    entity = null;
                    i--;
                }
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

            //Update min and max spacing
            LevelGenerationVars.MinNumSegments = Convert.ToInt32(Math.Round(playerSpeed * LevelGenerationVars.SpeedToMinSpaceMultipler));
            LevelGenerationVars.MaxNumSegments = Convert.ToInt32(Math.Round(playerSpeed * LevelGenerationVars.SpeedToMaxSpaceMultipler));

            //Check if it should create a new platform for each tier
            for(int i=0; i<rightMostInTiers.Length; i++) {
                Platform rightMost = rightMostInTiers[i];
                if (ShouldSpawnPlatform(rightMost))
                {
                    //Find the new platforms's spawn height
                    int height = LevelGenerationVars.Tier1Height;
                    if (i == 1) height = LevelGenerationVars.Tier2Height;
                    if (i == 2) height = LevelGenerationVars.Tier3Height;
                    
                    //Make the new platform
                    Platform nextPlat = GenerateNewPlatform(rightMost, height);
                    
                    //Update the right most platform in the tier
                    //(Which is now the just created platform)
                    rightMostInTiers[i] = nextPlat;

                    //Add to the list of ground pieces
                    platforms.Add(nextPlat);
                }
            }
        }

        /// <summary>
        /// Checks if it should spawn a new platform in the tier
        /// of the given platform
        /// </summary>
        /// <param name="rightMostInTier">The current, right most platform in the tier</param>
        /// <returns>True if a new platform should be created</returns>
        private bool ShouldSpawnPlatform(Platform rightMostInTier)
        {
            //Do not exceeed the maximum number of ground pieces
            if (platforms.Count > LevelGenerationVars.MaxGroundPieces) return false;

            //If the row is empty, make a new platform
            if (rightMostInTier == null) return true;

            //If the row is about to exceed the maximum between distance, make a new platform
            if (rightMostInTier.position.Right <= (GameplayVars.WinWidth - LevelGenerationVars.MaxBetweenSpace)) return true;
            
            //Randomly spawn
            return (rand.NextDouble() < LevelGenerationVars.PlatformSpawnFreq);
        }

        /// <summary>
        /// Generates a new platform following the given platform
        /// At the given tier height
        /// </summary>
        /// <param name="rightMost">Right most platform of the tier in which to add the platform</param>
        /// <param name="tierHeight">The height of the tier</param>
        /// <returns>The newly generated platform</returns>
        private Platform GenerateNewPlatform(Platform rightMost, int tierHeight)
        {
            //Must spawn off the right side of the screen (at a minimum)
            int minX = GameplayVars.WinWidth;
            
            //Make sure it's at least the minimum distance from the previous platform in the tier
            if (rightMost != null &&
                minX < (rightMost.position.Right + LevelGenerationVars.MinBetweenSpace))
            {
                minX = rightMost.position.Right + LevelGenerationVars.MinBetweenSpace;
            }

            //Can't go over the maximum space between platforms
            int maxX = minX + LevelGenerationVars.MaxBetweenSpace;
            if (rightMost != null)
            {
                maxX = rightMost.position.Right + LevelGenerationVars.MaxBetweenSpace;
            }

            int spawnX = -1;

            if (minX > maxX) spawnX = minX; //If the minimum is less than the maximum, just spawn at the minimum.
            else spawnX = rand.Next(minX, maxX); //Randomly decide on new location.

            //Calculate random size
            int width = LevelGenerationVars.SegmentWidth * rand.Next(LevelGenerationVars.MinNumSegments, LevelGenerationVars.MaxNumSegments);
            
            Platform newPlatform = new Platform(new Rectangle(spawnX, tierHeight, width, LevelGenerationVars.PlatformHeight),
                PlatformLeftTex, PlatformCenterTex, PlatformRightTex);

            GeneratePlatformContents(newPlatform);

            return newPlatform;
        }

        private void GeneratePlatformContents(Platform platform)
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

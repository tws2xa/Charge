#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Charge
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ChargeMain : Game
    {

        public const bool DEBUG = true;

		enum GameState
		{
			TitleScreen,
			InGame,
			Paused,
			GameOver
		};

		GameState currentGameState;

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
		ChargeBar chargeBar; // The chargebar

        int score; //Player score
        int curLevel; //The current level
        float tempScore; //Keeps track of fractional score increases
        float globalCooldown; //The cooldown on powerups

        private static float playerSpeed; //Current run speed
        public static float barrierSpeed; //Speed of barriers

		float playerChargeLevel; // Current charge

        //Useful Tools
        Random rand; //Used for generating random variables
        LevelGenerator levelGenerator; //Generates the platforms
        Controls controls;

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
		Texture2D ChargeBarBackgroundTex;
		Texture2D ChargeBarForegroundTex;

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

            //Initialize tools
            rand = new Random();
            levelGenerator = new LevelGenerator();
            controls = new Controls();

            //Initialize starting values for all numeric variables
            InitVars();

            //Initialize Monogame Stuff
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

			playerChargeLevel = GameplayVars.ChargeBarCapacity / 2;	// Init the player charge level to half of the max
			UpdatePlayerSpeed(); // Use the current charge level to set the player speed

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

			// Initalize the gamestate
			// TODO: Should probably initialize this to TitleScreen once that is implemented
			currentGameState = GameState.InGame;

            //Create the initial objects
            player = new Player(new Rectangle(GameplayVars.PlayerStartX, LevelGenerationVars.Tier2Height - 110, GameplayVars.StartPlayerWidth, GameplayVars.StartPlayerHeight), PlayerTex); //The player character
            backBarrier = new Barrier(new Rectangle(GameplayVars.BackBarrierStartX, -50, 90, GameplayVars.WinHeight + 100), BarrierTex); //The death barrier behind the player
            frontBarrier = new Barrier(new Rectangle(GameplayVars.FrontBarrierStartX, -50, 90, GameplayVars.WinHeight + 100), BarrierTex); //The death barrier in front of the player
            background = new Background(BackgroundTex);
			chargeBar = new ChargeBar(new Rectangle(graphics.GraphicsDevice.Viewport.Width / 4, 5, graphics.GraphicsDevice.Viewport.Width / 2, 20), ChargeBarBackgroundTex, ChargeBarForegroundTex);

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
            levelGenerator.SetRightMost(tier1, 0);
            levelGenerator.SetRightMost(tier2, 1);
            levelGenerator.SetRightMost(startPlat, 2);

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
            BackgroundTex = this.Content.Load<Texture2D>("Background");
            BarrierTex = this.Content.Load<Texture2D>("Barrier");
            BatteryTex = this.Content.Load<Texture2D>("Battery");
            EnemyTex = this.Content.Load<Texture2D>("Enemy");
            PlatformCenterTex = this.Content.Load<Texture2D>("PlatformCenterPiece");
            PlatformLeftTex = this.Content.Load<Texture2D>("PlatformLeftCap");
            PlatformRightTex = this.Content.Load<Texture2D>("PlatformRightCap");
            PlayerTex = this.Content.Load<Texture2D>("Player");
            WallTex = this.Content.Load<Texture2D>("Wall");
            ChargeBarBackgroundTex = this.Content.Load<Texture2D>("ChargeBar");
			ChargeBarForegroundTex = this.Content.Load<Texture2D>("ChargeBar");

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
			// This should be done regardless of the GameState
			controls.Update(); //Collect input data
			ProcessPlayerInput(); //Process input

			if (currentGameState == GameState.InGame)
			{
				//Delta time in seconds
				float deltaTime = (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

				background.Update(deltaTime); //Update the background scroll

				player.Update(deltaTime); //Update the player

				UpdateWorldEntities(deltaTime);	//Update all entities in the world

				CheckCollisions(); //Check for any collisions

				UpdatePlayerCharge(deltaTime); // Decrements the player charge, given the amount of time that has passed

				UpdatePlayerSpeed(); // Updates the player speed based on the current charge

				levelGenerator.Update(deltaTime); //Update level generation info

				GenerateLevelContent();	//Generate more level content

				UpdateCooldown(deltaTime); //Update the global cooldown

				UpdateScore(deltaTime);	//Update the player score
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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

			if (currentGameState == GameState.InGame || currentGameState == GameState.Paused)
			{
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

				// Draw UI
				chargeBar.Draw(spriteBatch, playerChargeLevel);

				// Draw the pause screen on top of all of the game assets
				if (currentGameState == GameState.Paused)
				{
					// TODO: Need to figure out fonts so we can draw the pause strings
				}
			}

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
		/// This is called from the update loop to handle player input
		/// </summary>
		private void ProcessPlayerInput()
		{
			// TODO: We should probably change this to confirm that the player wants to quit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			if (currentGameState == GameState.InGame)
			{
				// Player has pressed the jump command (A button on controller, space bar on keyboard)
				if (controls.onPress(Keys.Space, Buttons.A) && (player.jmpNum < GameplayVars.playerNumJmps || player.grounded))
				{
					player.jmpNum++;
					player.vSpeed = GameplayVars.JumpInitialVelocity;
					player.grounded = false;
				} // Cut jump short on button release
				else if (controls.onRelease(Keys.Space, Buttons.A) && player.vSpeed < 0)
				{
					player.vSpeed /= 2;
				}

				// Player has pressed the Discharge command (A key or left arrow key on keyboard)
				if (controls.isPressed(Keys.A, Buttons.X) || controls.isPressed(Keys.Left, Buttons.X))
				{

				}

				// Player has pressed the Shoot command (S key or down arrow key on keyboard)
				if (controls.isPressed(Keys.S, Buttons.Y) || controls.isPressed(Keys.S, Buttons.Y))
				{

				}

				// Player has pressed the Overcharge command (D key or right arrow key on keyboard)
				if (controls.isPressed(Keys.D, Buttons.B) || controls.isPressed(Keys.D, Buttons.B))
				{

				}

				// Player has pressed the Pause command (P key or Start button)
				if (controls.onPress(Keys.P, Buttons.Start))
				{
					currentGameState = GameState.Paused;
				}

				//Commands For debugging
				if (DEBUG)
				{
					//Control player speed with up and down arrows/right and left bumper.
					if (controls.isPressed(Keys.Up, Buttons.RightShoulder))
					{
						playerChargeLevel += 5;
					}
					if (controls.isPressed(Keys.Down, Buttons.LeftShoulder))
					{
						playerChargeLevel -= 5;
					}
				}
			}
			else if (currentGameState == GameState.Paused)
			{
				// Player has pressed the Pause command (P key or Start button)
				if (controls.onPress(Keys.P, Buttons.Start))
				{
					currentGameState = GameState.InGame;
				}
			}
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
                    platforms.Remove(entity); //Remove from platforms list
                    levelGenerator.PlatformRemoved(entity); //Alert the level generator
                    entity = null; //Clear the platform
                    i--; //Move back one in the loop to adjust for the removal
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
            CheckPlayerPlatformCollisions();
			CheckPlayerBatteryCollisions();
        }

        /// <summary>
        /// Checks the player against all platforms in the world
        /// </summary>
        public void CheckPlayerPlatformCollisions()
		{
            player.grounded = false;
            foreach (Platform plat in platforms)
            {
                if (plat.position.Left < player.position.Right * 2)
                {
                    bool collided = player.CheckPlatformCollision(plat); //Handles the checking and results of collisions
                    if (collided) break; //Hit a platform. No need to check any more.
                }
            }
        }

		/// <summary>
		/// Checks the player against all batteries in the world
		/// </summary>
		public void CheckPlayerBatteryCollisions()
		{
			foreach (WorldEntity battery in batteries)
			{
				if (player.position.Intersects(battery.position))
				{
					playerChargeLevel += GameplayVars.BatteryChargeReplenish;

					battery.destroyMe = true;
					break;
				}
			}
		}

		/// <summary>
		/// Updates the player speed based on the current charge level
		/// </summary>
		public void 
		UpdatePlayerCharge(float deltaTime)
		{
			playerChargeLevel -= GameplayVars.ChargeDecreaseRate * deltaTime;

			// Make sure playerChargeLevel is at least 0
			playerChargeLevel = Math.Max(0, playerChargeLevel);

			// For now, make sure that playerChargeLevel does not go past the charge bar capacity
			// This will be removed once we have the different colors and charge levels implemented
			playerChargeLevel = Math.Min(GameplayVars.ChargeBarCapacity, playerChargeLevel);
		}

		/// <summary>
		/// Updates the player speed based on the current charge level
		/// </summary>
		public void UpdatePlayerSpeed()
		{
			playerSpeed = GameplayVars.ChargeToSpeedCoefficient * playerChargeLevel;
		}

		/// <summary>
		/// Generates new level content
		/// </summary>
		public void GenerateLevelContent()
        {
            //Get the new platforms
            List<Platform> newPlatforms = levelGenerator.GenerateNewPlatforms(platforms.Count, PlatformLeftTex, PlatformCenterTex, PlatformRightTex);

            //Add each platform to the list of platforms
            //And generates items to go above each platform
            foreach (Platform platform in newPlatforms)
            {
                platforms.Add(platform);
                GeneratePlatformContents(platform);
            }

        }

        /// <summary>
        /// Generates the items to go above the given platform,
        /// Like walls, enemies, and batteries, and adds them
        /// To the world
        /// </summary>
        /// <param name="platform">Platform for which to generate content.</param>
        private void GeneratePlatformContents(Platform platform)
        {
            //The number of sections in the platform
            int numSections = platform.sections.Count;

            int numWalls = 0;
            int numEnemies = 0;
            int numBatteries = 0;

            //Check whether or not to add somthing to each section
            for (int i = 0; i < numSections; i++)
            {
                int roll = rand.Next(0, LevelGenerationVars.SectionContentRollNum);

                int sectionCenter = platform.sections[i].position.Center.X;

                if (roll < LevelGenerationVars.BatterySpawnRollRange && numBatteries < LevelGenerationVars.MaxBatteriesPerPlatform)
                {
                    //Spawn Battery
                    int width = LevelGenerationVars.BatteryWidth;
                    int height = LevelGenerationVars.BatteryHeight;
                    WorldEntity battery = new WorldEntity(new Rectangle(sectionCenter - width / 2, platform.position.Top - height / 2 - GameplayVars.StartPlayerHeight / 3, width, height), BatteryTex);
                    batteries.Add(battery);
                    platform.sections[i].containedObj = PlatformSection.BATTERYSTR;
                    numBatteries++;
                }
                else if (roll < LevelGenerationVars.BatterySpawnRollRange + LevelGenerationVars.WallSpawnFrequency && numWalls < LevelGenerationVars.MaxWallsPerPlatform)
                {
                    //Spawn Wall (takes up two platform spaces)
                    if (i >= numSections - 1) continue; //Need two sections

                    int width = LevelGenerationVars.WallWidth;
                    int height = LevelGenerationVars.WallHeight;
                    WorldEntity wall = new WorldEntity(new Rectangle(platform.sections[i].position.Right - width / 2, platform.position.Top - height + 3, width, height), WallTex);
                    walls.Add(wall);
                    platform.sections[i].containedObj = PlatformSection.WALLSTR;
                    platform.sections[i+1].containedObj = PlatformSection.WALLSTR;
                    numWalls++;
                    i++; //Took up an extra section
                }
                else if (roll < LevelGenerationVars.BatterySpawnRollRange + LevelGenerationVars.WallSpawnFrequency + LevelGenerationVars.EnemySpawnFrequency
                    && numEnemies < LevelGenerationVars.MaxEnemiesPerPlatform && enemies.Count < LevelGenerationVars.MaxNumEnemiesTotal)
                {
                    //Spawn Enemy
                    int width = LevelGenerationVars.EnemyWidth;
                    int height = LevelGenerationVars.EnemyHeight;
                    Enemy enemy = new Enemy(new Rectangle(sectionCenter - width / 2, platform.position.Top - height, width, height), EnemyTex, platform);
                    enemies.Add(enemy);
                    numEnemies++;
                }
            }

        }

        /// <summary>
        /// Provides the speed of the player (and thus
        /// the opposite of the speed that the world should scroll).
        /// </summary>
        /// <returns>The player's speed</returns>
        public static float GetPlayerSpeed()
        {
            return playerSpeed;
        }
    }
}

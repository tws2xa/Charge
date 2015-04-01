#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
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

        private static readonly Color[] ChargeBarLevelColors = { new Color(50, 50, 50), new Color(0, 234, 6), Color.Yellow, Color.Red, Color.Blue, Color.Pink }; // The bar colors for each charge level
        private static readonly Color[] PlatformLevelColors = { new Color(50, 50, 50), new Color(0, 234, 6), Color.Yellow, Color.Red, Color.Blue, Color.Pink }; // The platform colors for each charge level

		enum GameState
		{
			TitleScreen,
            OptionsScreen,
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
        List<Projectile> projectiles; //All bullets in game
        List<WorldEntity> walls; //All walls in the game
        List<WorldEntity> batteries; //All batteries in the game
        List<WorldEntity> otherEnts; //Other objects, like effects
        Barrier backBarrier; //The death barrier behind the player
        Barrier frontBarrier; //The death barrier in front of the player
        Background background; //The scrolling backdrop
		ChargeBar chargeBar; // The chargebar
        SpecialAbilityIconSet specialAbilityIcons; //Discharge, Shoot, and Overcharge icons

        int score; //Player score
        List<Int32> highScores; //Top 10 scores
        int curLevel; //The current level
        float tempScore; //Keeps track of fractional score increases
        private static float globalCooldown; //The cooldown on powerups
        private static float totalGlobalCooldown; //The max from which the cooldown is decreasing

        private SpriteFont Font; //Sprite Font to draw score
        private static float playerSpeed; //Current run speed
        public static float barrierSpeed; //Speed of barriers

        //Useful Tools
        Random rand; //Used for generating random variables
        LevelGenerator levelGenerator; //Generates the platforms
        Controls controls;

        //For reading and writing files
        StreamReader streamReader;
        StreamWriter streamWriter;

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
		Texture2D ChargeBarTex;
        Texture2D DischargeTex;
        Texture2D DischargeIconTex;
        Texture2D ShootIconTex;
        Texture2D OverchargeIconTex;
        Texture2D WhiteTex;

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
            projectiles = new List<Projectile>(); //All bullets in game
            walls = new List<WorldEntity>(); //All walls in the game
            batteries = new List<WorldEntity>(); //All batteries in the game
            otherEnts = new List<WorldEntity>(); //All other objects needed.

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
            totalGlobalCooldown = 0;

			//UpdatePlayerSpeed(); // Use the current charge level to set the player speed

			barrierSpeed = GameplayVars.BarrierStartSpeed;

			// Initalize the gamestate
			// TODO: Should probably initialize this to TitleScreen once that is implemented
			currentGameState = GameState.InGame;
        }

        /// <summary>
        /// Creates and Positions all objects for the start of the level
        /// </summary>
        public void SetupInitialConfiguration()
        {
            //Clear all entity lists
            platforms.Clear();
            enemies.Clear();
            projectiles.Clear();
            walls.Clear();
            batteries.Clear();
            otherEnts.Clear();

            //Create the initial objects
            player = new Player(new Rectangle(GameplayVars.PlayerStartX, LevelGenerationVars.Tier2Height - 110, GameplayVars.StartPlayerWidth, GameplayVars.StartPlayerHeight), PlayerTex); //The player character
            backBarrier = new Barrier(new Rectangle(GameplayVars.BackBarrierStartX, -50, 90, GameplayVars.WinHeight + 100), BarrierTex); //The death barrier behind the player
            frontBarrier = new Barrier(new Rectangle(GameplayVars.FrontBarrierStartX, -50, 90, GameplayVars.WinHeight + 100), BarrierTex); //The death barrier in front of the player
            background = new Background(BackgroundTex);
			chargeBar = new ChargeBar(new Rectangle(graphics.GraphicsDevice.Viewport.Width / 4, 5, graphics.GraphicsDevice.Viewport.Width / 2, 25), ChargeBarTex, ChargeBarLevelColors[0], ChargeBarLevelColors[1]);

            //Create UI Icons
            int iconSpacer = 10;
            int iconY = GameplayVars.WinHeight - SpecialAbilityIconSet.iconHeight - iconSpacer;
            specialAbilityIcons = new SpecialAbilityIconSet(iconSpacer, iconY, iconSpacer, DischargeIconTex, ShootIconTex, OverchargeIconTex, WhiteTex);

            //Long floor to catch player at the beginning of the game
            int startPlatWidth = GameplayVars.WinWidth - GameplayVars.PlayerStartX/3;
            startPlatWidth -= (startPlatWidth % LevelGenerationVars.SegmentWidth); //Make it evenly split into segments
            Platform startPlat = new Platform(new Rectangle(GameplayVars.PlayerStartX, LevelGenerationVars.Tier3Height, startPlatWidth, LevelGenerationVars.PlatformHeight),
                PlatformLeftTex, PlatformCenterTex, PlatformRightTex, GetCurrentPlatformColor());

            //Spawn a random platform in each of the upper two tiers
            int tier1X = rand.Next(0, GameplayVars.WinWidth);
            int tier1Width = LevelGenerationVars.SegmentWidth * rand.Next(LevelGenerationVars.MinNumSegments, LevelGenerationVars.MaxNumSegments);

            int tier2X = rand.Next(0, GameplayVars.WinWidth);
            int tier2Width = LevelGenerationVars.SegmentWidth * rand.Next(LevelGenerationVars.MinNumSegments, LevelGenerationVars.MaxNumSegments);

            Platform tier1 = new Platform(new Rectangle(tier1X, LevelGenerationVars.Tier1Height, tier1Width, LevelGenerationVars.PlatformHeight),
                PlatformLeftTex, PlatformCenterTex, PlatformRightTex, GetCurrentPlatformColor());
            Platform tier2 = new Platform(new Rectangle(tier2X, LevelGenerationVars.Tier2Height, tier2Width, LevelGenerationVars.PlatformHeight),
                PlatformLeftTex, PlatformCenterTex, PlatformRightTex, GetCurrentPlatformColor());

            //Since they're currently the only platform in their tier,
            //Set the newly created platforms as the right most in each of their tiers.
            levelGenerator.SetRightMost(tier1, 0);
            levelGenerator.SetRightMost(tier2, 1);
            levelGenerator.SetRightMost(startPlat, 2);

            //Add them to the platform list
            platforms.Add(tier1);
            platforms.Add(tier2);
            platforms.Add(startPlat);

            //Reading the list of high scores
            String fileName = "HighScores.txt";

            //Create new text file for high scores
            if (!File.Exists(fileName))
            {
                streamWriter = new StreamWriter("HighScores.txt");
                for (int i = 0; i < 9; i++)
                    streamWriter.Write("0 ");
                streamWriter.Write("0");
                streamWriter.Close();
            }

            //Processing data in the list of scores
            highScores = new List<Int32>();
            StreamReader file = new StreamReader("HighScores.txt");
            String line = file.ReadLine();
            String[] data = line.Split(' ');
            foreach (String str in data)
            {
                highScores.Add(Convert.ToInt32(str));
            }
            file.Close();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load all needed game textures and fonts
            BackgroundTex = this.Content.Load<Texture2D>("Background");
            BarrierTex = this.Content.Load<Texture2D>("Barrier");
            BatteryTex = this.Content.Load<Texture2D>("BatteryGlow");
            EnemyTex = this.Content.Load<Texture2D>("Enemy");
            PlatformCenterTex = this.Content.Load<Texture2D>("WhitePlatformCenterPiece");
            PlatformLeftTex = this.Content.Load<Texture2D>("WhitePlatformLeftCap");
            PlatformRightTex = this.Content.Load<Texture2D>("WhitePlatformRightCap");
            PlayerTex = this.Content.Load<Texture2D>("Player");
            WallTex = this.Content.Load<Texture2D>("Wall");
            ChargeBarTex= this.Content.Load<Texture2D>("ChargeBar");
            DischargeTex = this.Content.Load<Texture2D>("Discharge");
            DischargeIconTex = this.Content.Load<Texture2D>("DischargeIcon");
            ShootIconTex = this.Content.Load<Texture2D>("ShootIcon");
            OverchargeIconTex = this.Content.Load<Texture2D>("OverchargeIcon");
            WhiteTex = this.Content.Load<Texture2D>("White");

            Font = this.Content.Load<SpriteFont>("OCR-A-Extended-24");

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
            if (!this.IsActive)
            {
                PauseGame();
            }

			// This should be done regardless of the GameState
			controls.Update(); //Collect input data
			ProcessPlayerInput(); //Process input

			if (currentGameState == GameState.InGame)
			{
				//Delta time in seconds
				float deltaTime = (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

				background.Update(deltaTime); //Update the background scroll

				
                if (player.isDead)
                {

                }
                else
                {
                    player.Update(deltaTime); //Update the player

                    UpdateWorldEntities(deltaTime);	//Update all entities in the world

                    CheckCollisions(); //Check for any collisions

                    UpdatePlayerCharge(deltaTime); // Decrements the player charge, given the amount of time that has passed

                    UpdatePlayerSpeed(); // Updates the player speed based on the current charge

                    levelGenerator.Update(deltaTime); //Update level generation info

                    GenerateLevelContent();	//Generate more level content

                    UpdateCooldown(deltaTime); //Update the global cooldown

                    UpdateScore(deltaTime);	//Update the player score
                    
                    UpdateEffects(deltaTime); //Handle effects for things like Overcharge, etc

                    specialAbilityIcons.Update(deltaTime); //Update the UI icons
                }
			}
				
            base.Update(gameTime);
			}
            
        /// <summary>
        /// Pauses the game
        /// </summary>
        private void PauseGame()
        {
            currentGameState = GameState.Paused;
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
				foreach (Projectile projectile in projectiles)
				{
					projectile.Draw(spriteBatch);
				}

				//Draw Batteries
				foreach (WorldEntity battery in batteries)
				{
					battery.Draw(spriteBatch);
				}

                //Draw Other
                foreach (WorldEntity ent in otherEnts)
                {
                    ent.Draw(spriteBatch);
                }
                
				//Draw the player
				player.Draw(spriteBatch);

				//Draw Barriers
				frontBarrier.Draw(spriteBatch);
				backBarrier.Draw(spriteBatch);

				// Draw UI
                DrawUI(spriteBatch);

                // Draw Score
                if (player.isDead)
                {
                    //spriteBatch.DrawString(Font, "Final Score: " + score, new Vector2(365, 250), Color.WhiteSmoke);
                    //spriteBatch.DrawString(Font, "Press [ENTER] to play again", new Vector2(290, 300), Color.WhiteSmoke);
                    for (int i = 0; i < 9; i++ )
                    {
                        String place;
                        if (i == 0)
                            place = "1st";
                        else if (i == 1)
                            place = "2nd";
                        else if (i == 2)
                            place = "3rd";
                        else
                            place = (i + 1) + "th";
                        DrawStringWithShadow(spriteBatch, place + ": " + highScores[i], new Vector2(405, 75 + 35 * i));
                    }

                    DrawStringWithShadow(spriteBatch, "Final Score: " + score, new Vector2(330, 450));
                    DrawStringWithShadow(spriteBatch, "Press [ENTER] to play again", new Vector2(240, 500));

                }
                else
                {
                    DrawStringWithShadow(spriteBatch, "Score:" + score, new Vector2(750, 500));
                }
                
                //DrawStringWithShadow(spriteBatch, "Cooldown: " + Convert.ToInt32(globalCooldown), new Vector2(15, 500));

				// Draw the pause screen on top of all of the game assets
				if (currentGameState == GameState.Paused)
				{
                    DrawStringWithShadow(spriteBatch, "Paused", new Vector2(15, 15));
				}
			}

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws all UI elements
        /// </summary>
        private void DrawUI(SpriteBatch spriteBatch)
        {
            chargeBar.Draw(spriteBatch, player.GetCharge());
            specialAbilityIcons.Draw(spriteBatch);
        }

        /// <summary>
        /// Draws a string with a slight, black shadow behind it
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="location">Upper left corner of string</param>
        void DrawStringWithShadow(SpriteBatch spriteBatch, String text, Vector2 location)
        {
            spriteBatch.DrawString(Font, text, new Vector2(location.X + 2, location.Y + 2), Color.Black);
            spriteBatch.DrawString(Font, text, location, Color.WhiteSmoke);
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
                if (controls.onPress(Keys.Enter, Buttons.Start) && player.isDead)
                {
                    player.isDead = false;
                    InitVars();
                    SetupInitialConfiguration();
                }
            
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
                    InitiateDischarge();
				}

				// Player has pressed the Shoot command (S key or down arrow key on keyboard)
				if (controls.isPressed(Keys.S, Buttons.Y) || controls.isPressed(Keys.S, Buttons.Y))
				{
                    InitiateShoot();
				}

				// Player has pressed the Overcharge command (D key or right arrow key on keyboard)
				if (controls.isPressed(Keys.D, Buttons.B) || controls.isPressed(Keys.D, Buttons.B))
				{
                    InitiateOvercharge();
				}

				// Player has pressed the Pause command (P key or Start button)
				if (controls.onPress(Keys.P, Buttons.Start))
				{
                    PauseGame();
				}

				//Commands For debugging
				if (DEBUG)
				{
					//Control player speed with up and down arrows/right and left bumper.
					if (controls.isPressed(Keys.D0, Buttons.RightShoulder))
					{
						player.IncCharge(5);
					}
					if (controls.isPressed(Keys.D9, Buttons.LeftShoulder))
					{
						player.DecCharge(5);
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
        /// Launches the overcharge special ability
        /// </summary>
        private void InitiateOvercharge()
        {
            if (globalCooldown > 0)
            {
                return;
			}

            player.Overcharge();

            SetGlobalCooldown(GameplayVars.OverchargeCooldownTime);
		}


        /// <summary>
        /// Launches the shoot special ability  
        /// </summary>
        private void InitiateShoot()
        {
            if (globalCooldown > 0)
            {
                return;
            }

            player.DecCharge(GameplayVars.ShootCost);

            int bulletWidth = 15;
            int bulletHeight = 8;
            int bulletX = player.position.Right + bulletWidth;
            int bulletY = player.position.Center.Y - bulletHeight/2 + 5;
            Projectile bullet = new Projectile(new Rectangle(bulletX, bulletY, bulletWidth, bulletHeight), ChargeBarTex, GameplayVars.BulletMoveSpeed);
            projectiles.Add(bullet);

            SetGlobalCooldown(GameplayVars.ShootCooldownTime);
        }

        /// <summary>
        /// Launches the discharge special ability
        /// </summary>
        private void InitiateDischarge()
        {
            if (globalCooldown > 0)
            {
                return;
            }

            player.DecCharge(GameplayVars.DischargeCost);

            DischargeAnimation discharge = new DischargeAnimation(new Rectangle(player.position.Left, player.position.Top, player.position.Width, player.position.Width), DischargeTex);
            otherEnts.Add(discharge);

            SetGlobalCooldown(GameplayVars.DischargeCooldownTime);
        }


        public void SetGlobalCooldown(float cooldown)
            {
            globalCooldown = cooldown;
            totalGlobalCooldown = cooldown;
        }

        public void UpdateEffects(float deltaTime)
                {
            if (player.OverchargeActive())
            {
                if (rand.NextDouble() < 0.4)
                {
                    int effectWidth = 5;
                    int effectHeight = 5;
                    int effectX = player.position.X - effectWidth;
                    int effectY = player.position.Center.Y - effectHeight / 2;
                    double heightRand = rand.NextDouble();
                    if (heightRand < 0.3)
                    {
                        effectY += player.position.Height / 3;
                }
                    else if (heightRand > 0.7)
                    {
                        effectY -= player.position.Height / 3;
            }
                    OverchargeEffect effect = new OverchargeEffect(new Rectangle(effectX, effectY, effectWidth, effectHeight), ChargeBarTex, player);
                    otherEnts.Add(effect);
        }
            }
        }

        /// <summary>
        /// Update the global cooldown
        /// </summary>
        public void UpdateCooldown(float deltaTime)
        {
            globalCooldown = Math.Max(0, globalCooldown - deltaTime);
            if (globalCooldown == 0) totalGlobalCooldown = 0;
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

            //Update Barriers (don't let them move any go faster than the player could ever go)
            //barrierSpeed = Math.Min(barrierSpeed + GameplayVars.BarrierSpeedUpRate * deltaTime, GameplayVars.ChargeBarCapacity * GameplayVars.ChargeToSpeedCoefficient);
            barrierSpeed += GameplayVars.BarrierSpeedUpRate * deltaTime;


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
            for (int i = 0; i < projectiles.Count; i++)
            {
                Projectile entity = projectiles[i];
                entity.Update(deltaTime);

                //Check if it should be deleted
                if (entity.destroyMe)
                {
                    projectiles.Remove(entity);
                    entity = null;
                    i--;
                }
            }

            //Update Other
            for (int i = 0; i < otherEnts.Count; i++)
            {
                WorldEntity entity = otherEnts[i];
                entity.Update(deltaTime);

                //Check if it should be deleted
                if (entity.destroyMe)
                {
                    otherEnts.Remove(entity);
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
            CheckPlayerEnemyCollisions();
            CheckPlayerWallCollisions();
            CheckPlayerBarrierCollisions();
            CheckEnemyDischargeBlastCollisions();
            CheckEnemyProjectileCollisions();
        }

        /// <summary>
        /// Checks if the player collided with either barrier
        /// </summary>
        public void CheckPlayerBarrierCollisions()
        {
            if (player.position.Right > frontBarrier.position.Center.X)
            {
                PlayerDeath();
            }
            if (player.position.Left < backBarrier.position.Center.X)
            {
                PlayerDeath();
            }

            if (player.position.Top > GameplayVars.WinWidth + 10)
            {
                PlayerDeath();
            }
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
					    player.IncCharge(GameplayVars.BatteryChargeReplenish);
					battery.destroyMe = true;
					break;
				}
			}
		}

        public void CheckPlayerEnemyCollisions()
        {
            foreach (WorldEntity enemy in enemies)
            {
                if (player.position.Intersects(enemy.position))
                {
                    PlayerDeath();
                }
            }
        }

        public void CheckPlayerWallCollisions()
        {
            foreach (WorldEntity wall in walls)
            {
                if (player.position.Intersects(wall.position))
                {
                    if (player.OverchargeActive())
                    {
                        wall.destroyMe = true;
                    }
                    else
                    {
                    PlayerDeath();
                }
            }
        }
        }

        /// <summary>
        /// Kill enemies as the discharge blast collides with them
        /// </summary>
        public void CheckEnemyDischargeBlastCollisions()
        {
            foreach (WorldEntity enemy in enemies)
            {
                foreach (WorldEntity effect in otherEnts)
                {
                    if (effect is DischargeAnimation && effect.position.Intersects(enemy.position))
                    {
                        enemy.destroyMe = true;
                    }
                }
            }
        }

        /// <summary>
        /// Destroy enemies hit by bullets
        /// </summary>
        public void CheckEnemyProjectileCollisions()
        {
            foreach (WorldEntity projectile in projectiles)
            {
                foreach (WorldEntity enemy in enemies)
                {
                    if (projectile.position.Intersects(enemy.position))
                    {
                        enemy.destroyMe = true;
                        projectile.destroyMe = true;
                    }
                }
            }
        }

        /// <summary>
        /// Kills the player
        /// This implementation temporary as I anticipte we will be adding a Title and death screen.
        /// </summary>
        public void PlayerDeath()
        {
           // freezeWorld();
            player.isDead = true;
            updateHighScore(score);
        }

        /// <summary>
        /// Freezes GameplayVars on death before score is displayed.
        /// </summary>
        /*public void freezeWorld()
        {
            GameplayVars.ChargeToSpeedCoefficient = 0;
            GameplayVars.ChargeDecreaseRate = 0;
            GameplayVars.TimeToScoreCoefficient = 0f;          
            barrierSpeed = 0;
		}*/
        
		/// <summary>
		/// Updates the player speed based on the current charge level
		/// </summary>
		public void UpdatePlayerCharge(float deltaTime)
		{
			player.DecCharge(GameplayVars.ChargeDecreaseRate * deltaTime);
            
            // Pick the background color for the charge bar
            int chargeBackgroundIndex;
            Color backColor;

            backColor = ChargeBarLevelColors[GetBackgroundColorIndex()];
            
            // Pick the foreground color for the charge bar
            Color foreColor = ChargeBarLevelColors[GetForegroundColorIndex()];

            // Set the colors for the charge bar
            chargeBar.SetBackgroundColor(backColor);
            chargeBar.SetForegroundColor(foreColor);
		}

        public Color GetCurrentPlatformColor()
        {
            return PlatformLevelColors[GetForegroundColorIndex()];
        }

        public int GetBackgroundColorIndex()
        {
            return (Convert.ToInt32(Math.Floor(player.GetCharge() / GameplayVars.ChargeBarCapacity)) % ChargeBarLevelColors.Length);
        }

        public int GetForegroundColorIndex()
        {
            return (GetBackgroundColorIndex() + 1) % ChargeBarLevelColors.Length;
        }

		/// <summary>
		/// Updates the player speed based on the current charge level
		/// </summary>
		public void UpdatePlayerSpeed()
		{
			playerSpeed = GameplayVars.ChargeToSpeedCoefficient * player.GetCharge();
		}

		/// <summary>
		/// Generates new level content
		/// </summary>
		public void GenerateLevelContent()
        {
            //Get the new platforms
            List<Platform> newPlatforms = levelGenerator.GenerateNewPlatforms(platforms.Count, PlatformLeftTex, PlatformCenterTex, PlatformRightTex, GetCurrentPlatformColor());

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

                int batteryRollRange = LevelGenerationVars.BatterySpawnRollRange;
                float playerBarrierSpeedDiff = GetPlayerSpeed() - barrierSpeed;
                float multiplier = playerBarrierSpeedDiff / barrierSpeed;
                batteryRollRange -= Convert.ToInt32(LevelGenerationVars.MaxBatteryVariation * multiplier);
                
                if (roll < batteryRollRange && numBatteries < LevelGenerationVars.MaxBatteriesPerPlatform)
                {
                    //Spawn Battery
                    int width = LevelGenerationVars.BatteryWidth;
                    int height = LevelGenerationVars.BatteryHeight;
                    WorldEntity battery = new WorldEntity(new Rectangle(sectionCenter - width / 2, platform.position.Top - height / 2 - GameplayVars.StartPlayerHeight / 3, width, height), BatteryTex);
                    batteries.Add(battery);
                    platform.sections[i].containedObj = PlatformSection.BATTERYSTR;
                    numBatteries++;
                }
                else if (roll < batteryRollRange + LevelGenerationVars.WallSpawnFrequency && numWalls < LevelGenerationVars.MaxWallsPerPlatform)
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
                else if (roll < batteryRollRange + LevelGenerationVars.WallSpawnFrequency + LevelGenerationVars.EnemySpawnFrequency
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

        /// <summary>
        /// Returns the remaining global cooldown time
        /// </summary>
        internal static float GetGlobalCooldown()
        {
            return globalCooldown;
        }

        /// <summary>
        /// Returns the amount from which the global cooldown is decreasing.
        /// </summary>
        internal static float GetTotalCooldown()
        {
            return totalGlobalCooldown;
        }

        public void updateHighScore(int finalScore)
        {
            highScores.Add(finalScore);
            highScores.Sort();
            highScores.Reverse();
            streamWriter = new StreamWriter("HighScores.txt");
            for (int i = 0; i < 9; i++)
                streamWriter.Write(highScores[i]+" ");
            streamWriter.Write(highScores[9]);
            streamWriter.Close();
        }
    }
}

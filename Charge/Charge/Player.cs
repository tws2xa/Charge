using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Charge
{
    //More methods and fields may be added later
    class Player : WorldEntity
    {
		public float vSpeed;
		public bool grounded;
        public int jmpNum;

		/// <summary>
		/// Create the player with position and sprite
		/// </summary>
		public Player(Rectangle position, Texture2D tex)
        {
            base.init(position, tex);
            vSpeed = 0;
            jmpNum = 0;
            grounded = false;
        }

        /// <summary>
        /// Change update to allow for player movement
        /// </summary>
        public override void Update(float deltaTime)
        {
            if (!grounded)
            {
                vSpeed += GameplayVars.Gravity * deltaTime;
                //Cap speed
                vSpeed = Math.Min(GameplayVars.maxPlayerVSpeed, Math.Max(-1 * GameplayVars.maxPlayerVSpeed, vSpeed));
                position.Y += Convert.ToInt32(Math.Round(vSpeed));
            }
            else
            {
                vSpeed = 0;
            }



        }

        /// <summary>
        /// Handles potential platform collision
        /// </summary>
        /// <param name="plat">Platform with which the player may collide</param>
        public bool CheckPlatformCollision(Platform plat)
        {
            if (HitPlatform(plat))
            {
                if (vSpeed > 0) this.position.Y = plat.position.Y - this.position.Height;
                grounded = true;
                jmpNum = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the player hits the top of a platform
        /// </summary>
        /// <param name="plat">Platform with which the player may collide</param>
        /// <returns>True if collides. False otherwise.</returns>
        public bool HitPlatform(Platform plat)
        {
            //We Check if the difference in X vals is less than the width of the left-most object
            int width = this.position.Width;
            if (plat.position.X < this.position.X) width = plat.position.Width;
            if (Math.Abs(this.position.X - plat.position.X) < width)
            {
                //Check if the player is going down and the distance between the bottom
                //of the player and the top of the platform is greater than the player's vSpeed.
                return (this.vSpeed >= 0 && Math.Abs(this.position.Bottom - plat.position.Top) <= Math.Abs(this.vSpeed));
            }
            return false;
        }
    }
}

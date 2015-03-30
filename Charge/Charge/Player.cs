﻿using System;
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

    enum OverchargeState { OFF, INCREASING, DECREASING };

    //More methods and fields may be added later
    class Player : WorldEntity
    {
		public float vSpeed;
		public bool grounded;
        public int jmpNum;
        public bool isDead;
        private float overcharge = 0;
        private float playerChargeLevel; // Current charge

        OverchargeState overchargeState;

		/// <summary>
		/// Create the player with position and sprite
		/// </summary>
		public Player(Rectangle position, Texture2D tex)
        {
            base.init(position, tex);
            vSpeed = 0;
            jmpNum = 0;
            grounded = false;
            isDead = false;

            SetCharge(GameplayVars.ChargeBarCapacity / 2);	// Init the player charge level to half of the max
        }

        /// <summary>
        /// Change update to allow for player movement
        /// </summary>
        public override void Update(float deltaTime)
        {

            if (overchargeState == OverchargeState.INCREASING)
            {
                overcharge += GameplayVars.OverchargeIncAmt * deltaTime;
                if (overcharge >= GameplayVars.OverchargeMax)
                {
                    overcharge = GameplayVars.OverchargeMax;
                    overchargeState = OverchargeState.DECREASING;
                }
            }
            else if (overchargeState == OverchargeState.DECREASING)
            {
                overcharge -= GameplayVars.OverchargeDecAmt * deltaTime;
                if (overcharge <= 0)
                {
                    overcharge = 0;
                    overchargeState = OverchargeState.OFF;
                }
            }

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

        /// <summary>
        /// Returns the total player charge (includes overcharge)
        /// </summary>
        public float GetCharge()
        {
            return playerChargeLevel + overcharge;
        }

        /// <summary>
        /// Increase player charge by given amount
        /// </summary>
        public void IncCharge(float amt)
        {
            SetCharge(playerChargeLevel + amt);
        }

        /// <summary>
        /// Decrease player charge by given amount
        /// </summary>
        public void DecCharge(float amt)
        {
            SetCharge(playerChargeLevel - amt);
        }

        /// <summary>
        /// Set the player charge to a given amount
        /// </summary>
        public void SetCharge(float val)
        {
            playerChargeLevel = Math.Max(0, val);
        }

        /// <summary>
        /// Handle overcharge charge effects
        /// </summary>
        public void Overcharge()
        {
            overchargeState = OverchargeState.INCREASING;
        }

        public bool OverchargeActive()
        {
            return !(overchargeState == OverchargeState.OFF);
        }
    }
}

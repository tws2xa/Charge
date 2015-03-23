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
    class Enemy : WorldEntity
    {
        float moveSpeed; //Movement speed
        Platform myPlatform;

        /// <summary>
        /// Create the enemy with position and sprite
        /// </summary>
        public Enemy(Rectangle position, Texture2D tex, Platform myPlatform)
        {
            moveSpeed = GameplayVars.EnemyMoveSpeed;
            this.myPlatform = myPlatform;
            if ((new Random()).NextDouble() < 0.5) moveSpeed *= -1; //Randomize start direction
            base.init(position, tex);
        }

        /// <summary>
        /// Override update to allow for enemy behaviour.
        /// </summary>
        public override void Update(float deltaTime)
        {
            float nextPos = this.position.X + moveSpeed;

            float toCheck = nextPos;
            if (moveSpeed > 0)
            {
                toCheck += position.Width;
            }
            PlatformSection destSection = myPlatform.getSectionAtX(toCheck);
            if (isValidMoveSection(destSection))
            {
                this.position.X = Convert.ToInt32(nextPos);
            }
            else
            {
                moveSpeed *= -1;
            }
            base.Update(deltaTime);
        }
        
        /// <summary>
        /// Checks if the platform section is a valid place for the enemy to move.
        /// </summary>
        public bool isValidMoveSection(PlatformSection section)
        {
            if (section == null) return false;
            if (section.containedObj == null) return true;
            if (section.containedObj.Equals(PlatformSection.WALLSTR)) return false;
            return true;
        }
    }
}

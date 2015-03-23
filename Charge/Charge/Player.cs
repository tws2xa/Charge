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

		/// <summary>
		/// Create the player with position and sprite
		/// </summary>
		public Player(Rectangle position, Texture2D tex)
        {
            base.init(position, tex);
            vSpeed = 0;
            grounded = true;
        }

        /// <summary>
        /// Change update to allow for player movement
        /// </summary>
        public override void Update(float deltaTime)
        {
            if (grounded)
                return;
            position.Y -= Convert.ToInt32((GameplayVars.Gravity * deltaTime));
        }
    }
}

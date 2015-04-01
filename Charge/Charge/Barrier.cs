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
    class Barrier : WorldEntity
    {
		private double timeElapsedSinceLastMovement;

        /// <summary>
        /// Create the barrier with position and sprite
        /// </summary>
        public Barrier(Rectangle position, Texture2D tex)
        {
			timeElapsedSinceLastMovement = 0;
            base.init(position, tex);
        }

        /// <summary>
        /// Override update to allow for correct barrier movememnt.
        /// </summary>
        public override void Update(float deltaTime)
        {
			timeElapsedSinceLastMovement += deltaTime;

			double movementInPixels = 0;
			if (ChargeMain.GetPlayerSpeed() > 0) // Avoid divide by zero errors
			{
				// Calculate how many full pixels the object should move in the timeElapsedSinceLastMovement interval
				movementInPixels = Math.Floor(timeElapsedSinceLastMovement * ChargeMain.barrierSpeed);
				timeElapsedSinceLastMovement -= movementInPixels * (1 / ChargeMain.barrierSpeed);
			}

			this.position.X += Convert.ToInt32(movementInPixels);

			base.Update(deltaTime);
        }
    }
}

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

        public bool doPixelEffect = false;
        private PixelEffect pixelEffect;

        /// <summary>
        /// Create the barrier with position and sprite
        /// </summary>
        public Barrier(Rectangle position, Texture2D tex, Texture2D pixelTex)
        {
			timeElapsedSinceLastMovement = 0;
            base.init(position, tex);

            if (doPixelEffect)
            {
                int pixelWidth = position.Width / 2;
                Rectangle pixelRect = new Rectangle(position.X + position.Width / 2 - pixelWidth / 2, position.Y, pixelWidth, position.Height);
                pixelEffect = new PixelEffect(pixelRect, pixelTex, new List<Color>() { Color.White, Color.Black });
                pixelEffect.spawnFadeTime = -1;
                pixelEffect.spawnFrequency = 0.2f;
            }
        }

        /// <summary>
        /// Override update to allow for correct barrier movememnt.
        /// </summary>
        public override void Update(float deltaTime)
        {
			timeElapsedSinceLastMovement += deltaTime;

			double movementInPixels = 0;
			if (ChargeMain.barrierSpeed > 0) // Avoid divide by zero errors
			{
				// Calculate how many full pixels the object should move in the timeElapsedSinceLastMovement interval
				movementInPixels = Math.Floor(timeElapsedSinceLastMovement * ChargeMain.barrierSpeed);
				timeElapsedSinceLastMovement -= movementInPixels * (1 / ChargeMain.barrierSpeed);
			}

			this.position.X += Convert.ToInt32(movementInPixels);

            if (doPixelEffect)
            {
                pixelEffect.position.X += Convert.ToInt32(movementInPixels);
                pixelEffect.Update(deltaTime);
            }

			base.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if(doPixelEffect) pixelEffect.Draw(spriteBatch);
        }
    }
}

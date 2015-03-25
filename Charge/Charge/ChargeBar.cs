using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Charge
{
	class ChargeBar
	{
		public Texture2D backgroundTex; //Sprite for the object
		public Texture2D foregroundTex; //Sprite for the object

		public Rectangle position; //Object's position in the world

		public ChargeBar(Rectangle position, Texture2D backgroundTex, Texture2D foregroundTex)
		{
			this.position = position;

			this.backgroundTex = backgroundTex;
			this.foregroundTex = foregroundTex;
		}

		public void Draw(SpriteBatch spriteBatch, float chargeLevel)
		{
			spriteBatch.Draw(backgroundTex, position, Color.White);

			Rectangle chargeRect = new Rectangle(position.Left, position.Top, Convert.ToInt32(chargeLevel / GameplayVars.ChargeBarCapacity * position.Width), position.Height);
			spriteBatch.Draw(foregroundTex, chargeRect, Color.White);
		}
	}
}

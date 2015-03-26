using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Charge
{
	class ChargeBar
	{
		public Texture2D tex;

		public Color backColor;
		public Color foreColor;

		public Rectangle position; //Object's position in the world

		public ChargeBar(Rectangle position, Texture2D tex)
		{
			this.position = position;

			this.tex = tex;

			backColor = new Color(50, 50, 50);
			foreColor = Color.Yellow;
		}

		public void Draw(SpriteBatch spriteBatch, float chargeLevel)
		{
			spriteBatch.Draw(tex, position, backColor);

			Rectangle chargeRect = new Rectangle(position.Left, position.Top, Convert.ToInt32(chargeLevel / GameplayVars.ChargeBarCapacity * position.Width), position.Height);
			spriteBatch.Draw(tex, chargeRect, foreColor);
		}

		public void SetForegroundColor(Color c)
		{
			foreColor = c;
		}

		public void SetBackgroundColor(Color c)
		{
			backColor = c;
		}
	}
}

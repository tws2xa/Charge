using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Charge
{
    class DischargeAnimation : WorldEntity
    {
        private static readonly int DischargeAnimationGrowthRate = 300; // The rate that the sphere will grow at
        private Player player;
        public Circle circle;

        public DischargeAnimation(Rectangle pos, Texture2D tex, Player player)
        {
            init(pos, tex);
            circle = new Circle(new Vector2(pos.X + player.position.Width / 2, pos.Y + player.position.Height / 2), pos.Width / 2);
            this.player = player;
        }

        public override void Update(float deltaTime)
        {
            int growth = Convert.ToInt32(DischargeAnimationGrowthRate * deltaTime);

            int newHeight = position.Height + growth * 2;
            position = new Rectangle(position.Left - growth, player.position.Center.Y - newHeight/2, position.Width + growth * 2, newHeight);
            circle.Center = new Vector2(position.X + position.Width / 2, position.Y + position.Height / 2);
            circle.Radius = position.Width / 2;
            // Once the animation fills the screen, destroy it
            if (position.Right > GameplayVars.WinWidth && position.Left < 0 && position.Top < 0 && position.Bottom > GameplayVars.WinHeight)
            {
                destroyMe = true;
            }
        }
    }
}

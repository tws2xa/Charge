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

        public DischargeAnimation(Rectangle pos, Texture2D tex)
        {
            init(pos, tex);
        }

        public override void Update(float deltaTime)
        {
            int growth = Convert.ToInt32(DischargeAnimationGrowthRate * deltaTime);

            position = new Rectangle(position.Left - growth, position.Top - growth, position.Width + growth * 2, position.Height + growth * 2);

            // Once the animation fills the screen, destroy it
            if (position.Right > GameplayVars.WinWidth && position.Left < 0 && position.Top < 0 && position.Bottom > GameplayVars.WinHeight)
            {
                destroyMe = true;
            }
        }
    }
}

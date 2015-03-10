using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Charge
{
    class WorldEntity
    {
        public Rectangle position;

        public void Update(float deltaTime)
        {
            //Move in the opposite direction of the player speed
            //Thus creating the illusion that the player is moving
            this.position.X -= Convert.ToInt32(ChargeMain.moveSpeed * deltaTime);
        }

    }
}

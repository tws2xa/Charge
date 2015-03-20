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
    //More methods and fields may be added later
    class Enemy : WorldEntity
    {
        float moveSpeed; //Movement speed

        /// <summary>
        /// Create the enemy with position and sprite
        /// </summary>
        public Enemy(Rectangle position, Texture2D tex)
        {
            base.init(position, tex);
        }

        /// <summary>
        /// Override update to allow for enemy behaviour.
        /// </summary>
        public override void Update(float deltaTime)
        {

            base.Update(deltaTime);
        }
    }
}

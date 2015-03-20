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
    class Barrier : WorldEntity
    {
        float moveSpeed; //Movement speed

        /// <summary>
        /// Create the barrier with position and sprite
        /// </summary>
        public Barrier(Rectangle position, Texture2D tex)
        {
            base.init(position, tex);
        }

        /// <summary>
        /// Override update to allow for correct barrier movememnt.
        /// </summary>
        public override void Update(float deltaTime)
        {

        }
    }
}

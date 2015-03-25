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
            position.X += (Convert.ToInt32(ChargeMain.barrierSpeed * deltaTime));
            base.Update(deltaTime);
        }
    }
}

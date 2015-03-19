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
    class ChargeBlast : WorldEntity
    {
        /**
         * Create the ChargeBlast with position and sprite
         */
        public ChargeBlast(Rectangle position, Texture2D tex)
        {
            base.init(position, tex);
        }

        /**
         * Override update so that blast can radiate outwards
         */
        public void Update(float deltaTime)
        {
            
        }
    }
}

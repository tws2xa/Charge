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
    class PlatformSection : WorldEntity
    {
        public PlatformSection(Rectangle position, Texture2D tex)
        {
            base.init(position, tex);
        }

        public void Draw(SpriteBatch spriteBatch) {

        }
    }
}

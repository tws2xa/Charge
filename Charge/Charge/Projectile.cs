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
    class Projectile : WorldEntity
    {
        public Projectile(int x, int y, int width, int height)
        {
            this.position = new Rectangle(x, y, width, height);
        }
    }
}

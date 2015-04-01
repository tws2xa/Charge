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
    class PlatformSection : WorldEntity
    {

        public string containedObj = null; //Object stored above the section

        public static string WALLSTR = "wall";
        public static string BATTERYSTR = "battery";

        Color tint;

        public PlatformSection(Rectangle position, Texture2D tex, Color tint)
        {
            this.tint = tint;
            base.init(position, tex);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, position, tint);
            //base.Draw(spriteBatch);
        }
    }
}

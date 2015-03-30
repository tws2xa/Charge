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
    class SpecialAbilityIconSet
    {
        public static int iconWidth = 50;
        public static int iconHeight = 50;
        
        SpecialAbilityIcon dischargeIcon;
        SpecialAbilityIcon shootIcon;
        SpecialAbilityIcon overchargeIcon;

        public SpecialAbilityIconSet(int x, int y, int hSpace, Texture2D DischargeIconTex, Texture2D ShootIconTex, Texture2D OverchargeIconTex, Texture2D WhiteTex)
        {
            dischargeIcon = new SpecialAbilityIcon(new Rectangle(x, y, iconWidth, iconHeight), DischargeIconTex, WhiteTex);
            x += (iconWidth + hSpace);
            shootIcon = new SpecialAbilityIcon(new Rectangle(x, y, iconWidth, iconHeight), ShootIconTex, WhiteTex);
            x += (iconWidth + hSpace);
            overchargeIcon = new SpecialAbilityIcon(new Rectangle(x, y, iconWidth, iconHeight), OverchargeIconTex, WhiteTex);
        }

        public void Update(float deltaTime)
        {
            dischargeIcon.Update(deltaTime);
            shootIcon.Update(deltaTime);
            overchargeIcon.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            dischargeIcon.Draw(spriteBatch);
            shootIcon.Draw(spriteBatch);
            overchargeIcon.Draw(spriteBatch);
        }
    }
}

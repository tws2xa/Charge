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
    class Background
    {

        float scrollPos; //The position of the background's scrolling

        /// <summary>
        /// Create the background.
        /// </summary>
        public Background()
        {
            scrollPos = 0;
        }

        /// <summary>
        /// Updates the background's scroll
        /// </summary>
        public void Update(float deltaTime)
        {

        }

        /// <summary>
        /// Draws the background
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}

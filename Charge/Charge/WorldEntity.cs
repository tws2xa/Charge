﻿using System;
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
        Texture2D tex; //Sprite for the object
        public Rectangle position; //Object's position in the world

        //Needed for inheritence
        public WorldEntity() { }

        /**
         * Creates a standard world entity
         * With the given position and sprite
         */
        public WorldEntity(Rectangle position, Texture2D tex)
        {
            init(position, tex);
        }

        /**
         * Sets up the WorldEntity
         * With the given position and sprite
         */
        public void init(Rectangle position, Texture2D tex) {
            this.position = position;
            this.tex = tex;
        }

        /**
         * Draws the object's texture at the object's position
         */
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(tex, position, Color.White);
        }

        /**
         * Move in the opposite direction of the player speed
         * Thus creating the illusion that the player is moving
         */
        public void Update(float deltaTime)
        {
            this.position.X -= Convert.ToInt32(ChargeMain.moveSpeed * deltaTime);
        }

    }
}

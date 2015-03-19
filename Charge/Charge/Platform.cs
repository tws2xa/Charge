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
    class Platform : WorldEntity
    {

        List<PlatformSection> sections; //Sections that make up the platform

        /// <summary>
        /// Create the platform with position and sprite
        /// The created platform will contain no sections
        /// </summary>
        public Platform(Rectangle position, Texture2D tex)
        {
            base.init(position, tex);
            this.sections = new List<PlatformSection>();
        }

        /// <summary>
        /// Create the floor with position, sprite, and list of sections
        /// </summary>
        public Platform(Rectangle position, Texture2D tex, List<PlatformSection> sections)
        {
            base.init(position, tex);
            this.sections = sections;
        }
    }
}

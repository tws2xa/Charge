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

        //Sections that make up the platform
        public List<PlatformSection> sections;

        /// <summary>
        /// Create the platform with position and sprite
        /// The created platform will be automatically split into sections
        /// </summary>
        public Platform(Rectangle position, int levelNum, Texture2D leftCap, Texture2D center, Texture2D rightCap)
        {
            base.init(position, null);
            AlignWidthToSegments();
            InitSections(leftCap, center, rightCap);
        }

        public override void Update(float deltaTime)
        {
            foreach(PlatformSection section in sections) {
                section.Update(deltaTime);
            }

            base.Update(deltaTime);

            //If off the left side of the screen, clear sections and flag platform for destruction.
            if (destroyMe)
            {
                sections.Clear();
            }
        }

        /// <summary>
        /// Draw each of the platform's sections
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach(PlatformSection section in sections) {
                section.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Initialize the sections
        /// </summary>
        /// <param name="leftCapTex">Texture for the left-most segment of the platform</param>
        /// <param name="centerTex">Texture for all middle segments of the platform</param>
        /// <param name="rightCapTex">Texture for the right-most segment of the platform</param>
        /// <param name="alignWidth">Optional (Default false): Align the platform width to evenly fit segments</param>
        public void InitSections(Texture2D leftCapTex, Texture2D centerTex, Texture2D rightCapTex, bool alignWidth = false)
        {
            if (alignWidth) AlignWidthToSegments();

            sections = new List<PlatformSection>();

            int numSections = Convert.ToInt32(Math.Floor((double)position.Width / (double)LevelGenerationVars.SegmentWidth));

            //Left section
            int StartSectionX = position.X;
            int SectionY = position.Y;
            int SectionWidth = LevelGenerationVars.SegmentWidth;
            int SectionHeight = LevelGenerationVars.PlatformHeight;

            PlatformSection left = new PlatformSection(new Rectangle(StartSectionX, SectionY, SectionWidth, SectionHeight), leftCapTex);
            sections.Add(left);

            //Middle sections
            for (int i = 1; i < numSections-1; i++)
            {
                PlatformSection mid = new PlatformSection(new Rectangle(StartSectionX + i*SectionWidth, SectionY, SectionWidth, SectionHeight), centerTex);
                sections.Add(mid);
            }

            //Right section
            PlatformSection right = new PlatformSection(new Rectangle(StartSectionX + (numSections-1)*SectionWidth, SectionY, SectionWidth, SectionHeight), rightCapTex);
            sections.Add(right);

        }

        /// <summary>
        /// Rounds the width to the closest size that evenly splits into segments.
        /// Ensures at least two segments.
        /// </summary>
        public void AlignWidthToSegments()
        {
            int myWidth = position.Width;
            myWidth -= (myWidth % LevelGenerationVars.SegmentWidth);
            int numSegments = myWidth / LevelGenerationVars.SegmentWidth;
            //Floor pieces need two segments minimum
            if (numSegments < 2)
            {
                numSegments = 2;
                myWidth = numSegments * LevelGenerationVars.SegmentWidth;
            }
            position.Width = myWidth; //Reset the width to fit segments
        }
    }
}

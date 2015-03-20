using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charge
{
    static class LevelGenerationVars
    {
        public static int Tier1Height;
        public static int Tier2Height;
        public static int Tier3Height = 480;
        public static int PlatformHeight = 50;
        public static int MaxGroundPieces;
        public static int MinGroundPieces;
        public static int SegmentWidth = PlatformHeight;
        public static int MinNumSegments;
        public static int MaxNumSegments;
        public static int MinBetweenSpace;
        public static int MaxBetweenSpace;
        public static int MaxNumEnemies;
        public static float BatteryFrequency;
        public static float WallFrequency;
        public static float EnemyFrequency;
    }
}

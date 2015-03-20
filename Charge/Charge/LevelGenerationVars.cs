using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charge
{
    static class LevelGenerationVars
    {
        public static int Tier1Height = 105;
        public static int Tier2Height = 305;
        public static int Tier3Height = 505;
        public static int PlatformHeight = 50;
        public static int MaxGroundPieces = 10;
        public static int MinGroundPieces = 3;
        public static int SegmentWidth = PlatformHeight;
        public static int MinNumSegments = 2;
        public static int MaxNumSegments = 6;
        public static int MinBetweenSpace = 75;
        public static int MaxBetweenSpace = 300;
        public static int MaxNumEnemies = 3;
        public static float SpeedToMinSpaceMultipler = (float)MinNumSegments / (float)GameplayVars.PlayerStartSpeed;
        public static float SpeedToMaxSpaceMultipler = (float)MaxNumSegments / (float)GameplayVars.PlayerStartSpeed;
        public static float PlatformSpawnFreq = 0.05f;
        public static float BatteryFrequency = 0.05f;
        public static float WallFrequency = 0.005f;
        public static float EnemyFrequency = 0.001f;
    }
}

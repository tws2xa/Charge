using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charge
{
    static class GameplayVars
    {
        public static int WinWidth = 950;
        public static int WinHeight = 550;
        public static int StartPlayerWidth = 44;
        public static int StartPlayerHeight = 71;
        public static int PlayerStartX = WinWidth/3;
        public static int playerNumJmps = 2;
        public static float maxPlayerVSpeed = 50;
        public static int BackBarrierStartX = -50;
        public static int FrontBarrierStartY = WinHeight + 50;
        public static float Gravity = 40; // The y-axis starts at 0 at the top of the screen, so gravity should increase Y
        public static float JumpInitialVelocity = -16; // The y-axis starts at 0 at the top of the screen, so jump should decrease Y
		public static float PlayerStartSpeed = 150;
        public static float BarrierStartSpeed = 150;
        public static float EnemyMoveSpeed = 1;
        public static float ChargeDecreaseRate;
        public static float BatteryChargeReplenish;
        public static float BarrierSpeedUpRate;
        public static float ChargeToSpeedCoefficient;
        public static float TimeToScoreCoefficient;
        public static float DischargeCooldownTime;
        public static float OverloadCooldownTime;
        public static float ShootCooldownTime;
    }
}

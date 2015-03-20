using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charge
{
    static class GameplayVars
    {
        public static int WinWidth = 800;
        public static int WinHeight = 500;
        public static int PlayerStartX = 100;
        public static int BackBarrierStartX = -50;
        public static int FrontBarrierStartY = WinHeight + 50;
        public static float Gravity;
        public static float JumpVelocity;
        public static float PlayerStartSpeed;
        public static float BarrierStartSpeed;
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

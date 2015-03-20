using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charge
{
    static class GameplayVars
    {
        public static int WinWidth = 1000;
        public static int WinHeight = 600;
        public static int PlayerStartX = WinWidth/3;
        public static int BackBarrierStartX = -50;
        public static int FrontBarrierStartY = WinHeight + 50;
        public static float Gravity;
        public static float JumpVelocity;
        public static float PlayerStartSpeed = 150;
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

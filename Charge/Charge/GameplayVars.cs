﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charge
{
    static class GameplayVars
    {
        public static int ChargeBarHeight = 15; //25 on Android.
        public static int WinWidth = 1022;
        public static int WinHeight = 575;
        public static int StartPlayerWidth = 44;
        public static int StartPlayerHeight = 71;
        public static int FrontBarrierStartX = WinWidth + 350;
        public static int FrontBarrierStartY = WinHeight + 50;
        public static float GlowThreshold = (FrontBarrierStartX - BackBarrierStartX) / 7.0f; // Div 4 works well for Android. 7 for PC.
        public static int BarrierWidth = 50;
        public static int PlayerXBuffer = 15;
        public static int PlayerYBuffer = 15;
        public static int PlayerStartX = WinWidth/3;
        public static int playerNumJmps = 2;
        public static int wallXBuffer = 10;
        public static int wallYBuffer = 30;
        public static int enemyXBuffer = 5;
        public static int enemyYBuffer = 5;
        public static int BackBarrierStartX = -150;
        public static float MaxPlayerVSpeed = 50;
        public static float Gravity = 40; // The y-axis starts at 0 at the top of the screen, so gravity should increase Y
        public static float JumpInitialVelocity = -16; // The y-axis starts at 0 at the top of the screen, so jump should decrease Y
        public static float PlayerStartSpeed = 150;
        public static float BarrierStartSpeed = 150;
        public static float EnemyMoveSpeed = 1;
        public static float BulletMoveSpeed = 10;
        public static float ChargeDecreaseRate = 2;
        public static float BatteryChargeReplenish = 5;

        //Level coefficients, determine speed changes on levels
        public static float Level1Speed = 1.0f;
        public static float Level2Speed = 0.65f;
        public static float Level3Speed = 0.45f;
        public static float Level4Speed = 0.30f;
        public static float[] LevelSpeeds = { Level1Speed, Level2Speed, Level3Speed, Level4Speed };

        //Barrier Speed up break points. Used to cause decreased rate of acceleration upon each level switch.
        public static float BarrierSpeedUpRate = 3.0f;
        public static float BarrierLevel2Rate = BarrierSpeedUpRate * Level2Speed;
        public static float BarrierLevel3Rate = BarrierSpeedUpRate * Level3Speed;
        public static float BarrierLevel4Rate = BarrierSpeedUpRate * Level4Speed;

        public static float ChargeToSpeedCoefficient = 5.0f;


        public static float TimeToScoreCoefficient = 4.5f;

        public static float[] DischargeCooldownTime = { 20, 17, 14, 11, 10, 10 }; //By level. In Seconds.
        public static float[] OverchargeCooldownTime = { 20, 17, 14, 11, 10, 10 }; //By level. In Seconds
        public static float[] ShootCooldownTime = {5, 4.5f, 4, 3.5f, 3, 2.5f}; //By level. In Seconds
       

		public static int ChargeBarCapacity = 75;

        public static float DischargeMaxCost = 50;
        public static float DischargeCost = .3f;
        public static float ShootCost = 10;
        public static float OverchargeMax = 50;
        public static float OverchargePermanentAdd = BatteryChargeReplenish*2;

        public static float OverchargeIncAmt = OverchargeMax * 3.0f; //Should take about 1/3 seconds to reach max speed
        public static float OverchargePermanentAddAmt = OverchargePermanentAdd * 3.0f;
        public static float OverchargeDecAmt = OverchargeMax / 5.0f; //Should take about 5 seconds to reach normality again

        public static float titleScrollSpeed = 100;
        public static int MinPlatformBrightness = 255; //255 = fully bright

        public static int NumScores = 10;
    }
}

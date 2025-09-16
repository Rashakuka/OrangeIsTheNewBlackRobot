using System;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using Robocode.TankRoyale.BotApi.Graphics;

// ------------------------------------------------------------------
// OrangeIsTheNewBlack.HardCarry
// ------------------------------------------------------------------
// Hard Carry bot - Late game damage dealer
// High damage output, positioning, and late game scaling
// ------------------------------------------------------------------
public class OrangeIsTheNewBlack_HardCarry : Bot
{
    private double lastEnemyDistance;
    private int carryMode;
    private bool isFarming;
    private double moveAmount;
    private int damageOutput;
    private double lastEnemyBearing;

    // The main method starts our bot
    static void Main()
    {
        new OrangeIsTheNewBlack_HardCarry().Start();
    }

    // Called when a new round is started -> initialize and do some movement
    public override void Run()
    {
        // Set colors - Gold for hard carry
        BodyColor = Color.Yellow;
        TurretColor = Color.Yellow;
        RadarColor = Color.Red;
        BulletColor = Color.White;
        ScanColor = Color.White;

        // Initialize variables
        carryMode = 0;
        isFarming = true;
        moveAmount = Math.Max(ArenaWidth, ArenaHeight) / 4;
        lastEnemyDistance = 0;
        damageOutput = 1;
        lastEnemyBearing = 0;

        // Start with farming behavior
        TurnGunLeft(90);
        Forward(moveAmount);
        
        // Main loop - hard carry behavior
        while (IsRunning)
        {
            // Hard carry movement pattern
            switch (carryMode)
            {
                case 0: // Farming phase
                    TurnGunRight(45);
                    Forward(30);
                    break;
                case 1: // Positioning phase
                    TurnGunLeft(90);
                    TurnLeft(30);
                    break;
                case 2: // Damage phase
                    TurnGunRight(60);
                    Forward(40);
                    break;
            }
            
            // Cycle through carry modes
            carryMode = (carryMode + 1) % 3;
            
            // Scale damage output over time (late game scaling)
            if (carryMode == 0)
            {
                damageOutput = Math.Min(5, damageOutput + 1);
            }
        }
    }

    // We scanned another bot -> hard carry response
    public override void OnScannedBot(ScannedBotEvent e)
    {
        lastEnemyDistance = e.Distance;
        lastEnemyBearing = e.Bearing;
        
        // Hard carry behavior - high damage output
        if (e.Distance < 300)
        {
            // Close range - maximum damage
            TurnLeft(e.Bearing);
            Forward(35);
            
            // High damage fire
            SetFire(Math.Min(5, damageOutput));
            TurnGunLeft(e.Bearing);
            
            // Predict enemy movement for better aim
            var predictedBearing = e.Bearing + (e.Velocity * Math.Sin(e.Direction * Math.PI / 180) * 0.15);
            TurnGunLeft(predictedBearing);
        }
        else if (e.Distance < 500)
        {
            // Medium range - positioning for damage
            TurnLeft(e.Bearing);
            Forward(50);
            
            // High damage fire
            SetFire(Math.Min(4, damageOutput));
            TurnGunLeft(e.Bearing);
        }
        else
        {
            // Long range - positioning
            TurnLeft(e.Bearing);
            Forward(60);
            
            // Long range damage
            SetFire(Math.Min(3, damageOutput));
            TurnGunLeft(e.Bearing);
        }
        
        // Rescan for continuous damage
        Rescan();
    }

    // We hit another bot -> hard carry response
    public override void OnHitBot(HitBotEvent e)
    {
        // Hard carry - maximum damage output
        SetFire(Math.Min(5, damageOutput));
        TurnGunLeft(e.Bearing);
        
        // Hard carry positioning - maintain damage range
        if (e.Distance < 200)
        {
            // Close combat - high damage
            TurnLeft(e.Bearing + 30);
            Forward(25);
        }
        else
        {
            // Optimal damage range
            TurnLeft(e.Bearing);
            Forward(35);
        }
    }

    // We got hit -> hard carry response
    public override void OnHitByBullet(HitByBulletEvent e)
    {
        // Hard carry - counter-attack with high damage
        TurnGunLeft(e.Bearing);
        SetFire(Math.Min(4, damageOutput));
        
        // Hard carry positioning - maintain damage output
        TurnLeft(60);
        Forward(40);
    }

    // We hit a wall -> hard carry repositioning
    public override void OnHitWall(HitWallEvent e)
    {
        // Hard carry repositioning - aggressive
        TurnLeft(90);
        Forward(60);
    }

    // We won -> hard carry celebration
    public override void OnWonRound(WonRoundEvent e)
    {
        // Hard carry victory dance - powerful and confident
        for (int i = 0; i < 10; i++)
        {
            TurnLeft(36);
            Forward(20);
            SetFire(5);
        }
    }
}
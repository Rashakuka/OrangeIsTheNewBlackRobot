using System;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using Robocode.TankRoyale.BotApi.Graphics;

// ------------------------------------------------------------------
// OrangeIsTheNewBlack.Offlane
// ------------------------------------------------------------------
// Offlane bot - Tanky initiator and disruptor
// High HP, initiates fights, disrupts enemy positioning
// ------------------------------------------------------------------
public class OrangeIsTheNewBlack_Offlane : Bot
{
    private double lastEnemyDistance;
    private int initiatorMode;
    private bool isInitiating;
    private double moveAmount;

    // The main method starts our bot
    static void Main()
    {
        new OrangeIsTheNewBlack_Offlane().Start();
    }

    // Called when a new round is started -> initialize and do some movement
    public override void Run()
    {
        // Set colors - Green for tanky offlane
        BodyColor = Color.Green;
        TurretColor = Color.Green;
        RadarColor = Color.Yellow;
        BulletColor = Color.Red;
        ScanColor = Color.Red;

        // Initialize variables
        initiatorMode = 0;
        isInitiating = false;
        moveAmount = Math.Max(ArenaWidth, ArenaHeight) / 4;
        lastEnemyDistance = 0;

        // Start with defensive positioning
        TurnGunLeft(90);
        Forward(moveAmount / 2);
        
        // Main loop - tanky offlane behavior
        while (IsRunning)
        {
            // Tanky movement pattern - slow but steady
            switch (initiatorMode)
            {
                case 0: // Defensive positioning
                    TurnGunRight(45);
                    Forward(20);
                    break;
                case 1: // Scanning for opportunities
                    TurnGunLeft(90);
                    TurnLeft(30);
                    break;
                case 2: // Initiating mode
                    if (isInitiating)
                    {
                        Forward(40);
                        TurnGunLeft(30);
                    }
                    else
                    {
                        TurnRight(45);
                    }
                    break;
            }
            
            // Cycle through modes
            initiatorMode = (initiatorMode + 1) % 3;
            
            // Change direction periodically for tanky positioning
            if (initiatorMode == 0)
            {
                TurnLeft(60);
            }
        }
    }

    // We scanned another bot -> tanky response
    public override void OnScannedBot(ScannedBotEvent e)
    {
        lastEnemyDistance = e.Distance;
        
        // Tanky behavior - initiate if close enough
        if (e.Distance < 300)
        {
            isInitiating = true;
            
            // Move towards enemy to initiate
            TurnLeft(e.Bearing);
            Forward(50);
            
            // Fire with moderate power (tanky, not glass cannon)
            SetFire(2);
            
            // Turn gun towards enemy
            TurnGunLeft(e.Bearing);
        }
        else
        {
            isInitiating = false;
            
            // Tanky positioning - maintain distance
            if (e.Distance > 400)
            {
                TurnLeft(e.Bearing);
                Forward(30);
            }
            else
            {
                TurnLeft(e.Bearing + 90);
                Forward(20);
            }
        }
        
        // Rescan for continuous awareness
        Rescan();
    }

    // We hit another bot -> tanky response
    public override void OnHitBot(HitBotEvent e)
    {
        // Tanky offlane - absorb damage and fight back
        SetFire(2);
        TurnGunLeft(e.Bearing);
        
        // Tanky positioning - don't back down easily
        if (e.Distance < 100)
        {
            // Close combat - tank it out
            TurnLeft(e.Bearing + 45);
            Forward(20);
        }
        else
        {
            // Maintain optimal tanky distance
            TurnLeft(e.Bearing);
            Forward(30);
        }
    }

    // We got hit -> tanky resilience
    public override void OnHitByBullet(HitByBulletEvent e)
    {
        // Tanky response - don't panic, fight back
        TurnGunLeft(e.Bearing);
        SetFire(2);
        
        // Tanky movement - absorb and counter
        TurnLeft(45);
        Forward(25);
    }

    // We hit a wall -> tanky repositioning
    public override void OnHitWall(HitWallEvent e)
    {
        // Tanky repositioning - slow but steady
        TurnLeft(90);
        Forward(40);
    }

    // We won -> tanky celebration
    public override void OnWonRound(WonRoundEvent e)
    {
        // Tanky victory dance
        for (int i = 0; i < 5; i++)
        {
            TurnLeft(72);
            Forward(20);
        }
    }
}
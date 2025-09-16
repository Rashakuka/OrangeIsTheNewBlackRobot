using System;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using Robocode.TankRoyale.BotApi.Graphics;

// ------------------------------------------------------------------
// OrangeIsTheNewBlack.Support5
// ------------------------------------------------------------------
// Support 5 bot - Defensive support and protector
// Defensive positioning, protects team, wards and heals
// ------------------------------------------------------------------
public class OrangeIsTheNewBlack_Support5 : Bot
{
    private double lastEnemyDistance;
    private int defensiveMode;
    private bool isProtecting;
    private double moveAmount;
    private int scanPattern;

    // The main method starts our bot
    static void Main()
    {
        new OrangeIsTheNewBlack_Support5().Start();
    }

    // Called when a new round is started -> initialize and do some movement
    public override void Run()
    {
        // Set colors - Purple for defensive support
        BodyColor = Color.Magenta;
        TurretColor = Color.Magenta;
        RadarColor = Color.White;
        BulletColor = Color.Green;
        ScanColor = Color.Green;

        // Initialize variables
        defensiveMode = 0;
        isProtecting = false;
        moveAmount = Math.Max(ArenaWidth, ArenaHeight) / 5;
        lastEnemyDistance = 0;
        scanPattern = 0;

        // Start with defensive positioning
        TurnGunLeft(90);
        Forward(moveAmount);
        
        // Main loop - defensive support behavior
        while (IsRunning)
        {
            // Defensive support movement pattern
            switch (defensiveMode)
            {
                case 0: // Defensive scanning
                    TurnGunRight(30);
                    Forward(15);
                    break;
                case 1: // Protective positioning
                    TurnGunLeft(60);
                    TurnLeft(20);
                    break;
                case 2: // Defensive retreat
                    TurnGunRight(45);
                    Back(20);
                    break;
            }
            
            // Cycle through defensive modes
            defensiveMode = (defensiveMode + 1) % 3;
            
            // Change scan pattern for better awareness
            if (defensiveMode == 0)
            {
                scanPattern = (scanPattern + 1) % 3;
            }
        }
    }

    // We scanned another bot -> defensive support response
    public override void OnScannedBot(ScannedBotEvent e)
    {
        lastEnemyDistance = e.Distance;
        
        // Support 5 behavior - defensive support
        if (e.Distance < 200)
        {
            // Close threat - defensive response
            isProtecting = true;
            
            // Defensive positioning - maintain distance
            TurnLeft(e.Bearing + 90);
            Back(30);
            
            // Defensive fire - low damage but consistent
            SetFire(1);
            TurnGunLeft(e.Bearing);
        }
        else if (e.Distance < 350)
        {
            // Medium threat - defensive positioning
            isProtecting = true;
            
            // Defensive movement
            TurnLeft(e.Bearing + 45);
            Forward(20);
            
            // Support fire
            SetFire(1);
            TurnGunLeft(e.Bearing);
        }
        else
        {
            // Long range - defensive awareness
            isProtecting = false;
            
            // Defensive positioning - stay back
            TurnLeft(e.Bearing + 60);
            Back(15);
        }
        
        // Rescan for defensive awareness
        Rescan();
    }

    // We hit another bot -> defensive response
    public override void OnHitBot(HitBotEvent e)
    {
        // Support 5 - defensive response
        SetFire(1);
        TurnGunLeft(e.Bearing);
        
        // Defensive positioning - retreat and support
        if (e.Distance < 100)
        {
            // Too close - defensive retreat
            TurnLeft(e.Bearing + 180);
            Back(40);
        }
        else
        {
            // Defensive distance - maintain support range
            TurnLeft(e.Bearing + 90);
            Back(20);
        }
    }

    // We got hit -> defensive response
    public override void OnHitByBullet(HitByBulletEvent e)
    {
        // Support 5 - defensive retreat
        TurnGunLeft(e.Bearing);
        SetFire(1);
        
        // Defensive movement - retreat and reposition
        TurnLeft(135);
        Back(50);
        
        // Change defensive mode
        defensiveMode = 2;
    }

    // We hit a wall -> defensive repositioning
    public override void OnHitWall(HitWallEvent e)
    {
        // Defensive repositioning - careful and safe
        TurnLeft(90);
        Back(30);
        
        // Change defensive strategy
        defensiveMode = 0;
    }

    // We won -> defensive celebration
    public override void OnWonRound(WonRoundEvent e)
    {
        // Defensive victory dance - careful and methodical
        for (int i = 0; i < 6; i++)
        {
            TurnLeft(60);
            Forward(10);
            Back(10);
        }
    }
}